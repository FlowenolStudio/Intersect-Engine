using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CommandLine;
using Intersect.Client.Networking;
using Intersect.Client.Plugins;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Factories;
using Intersect.Framework.Logging;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.SystemInformation;
using Intersect.Network;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Helpers;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Intersect.Client.Core;

internal static partial class Bootstrapper
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor, typeof(ClientCommandLineOptions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(PacketHandlerRegistry))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(PacketHandler))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Intersect.Client.MonoGame.IntersectGame.MonoGameRunner))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AnimationDescriptor))]
    public static void Start(Assembly entryAssembly, params string[] args)
    {
        AndroidClientHost.NotifyBootstrapStep("parser");
        var parser = new Parser(
            parserSettings =>
            {
                if (parserSettings == null)
                {
                    throw new ArgumentNullException(
                        nameof(parserSettings), @"If this is null the CommandLineParser dependency is likely broken."
                    );
                }

                parserSettings.AutoHelp = true;
                parserSettings.AutoVersion = true;
                parserSettings.IgnoreUnknownArguments = true;
            }
        );

        var commandLineOptions = parser.ParseArguments<ClientCommandLineOptions>(args)
            .MapResult(HandleParsedArguments, HandleParserErrors);

        LoggingLevelSwitch loggingLevelSwitch =
            new(Debugger.IsAttached ? LogEventLevel.Debug : LogEventLevel.Information);

        var (loggerFactory, logger) = new LoggerConfiguration().CreateLoggerForIntersect(
            entryAssembly,
            "Client",
            loggingLevelSwitch
        );
        AndroidClientHost.NotifyBootstrapStep("logger");

        PlatformStatistics.Logger = loggerFactory.CreateLogger<PlatformStatistics>();

        var packetTypeRegistry = new PacketTypeRegistry(logger, typeof(IntersectPacket).Assembly);
        if (!packetTypeRegistry.TryRegisterBuiltIn())
        {
            AndroidClientHost.NotifyBootstrapStep("packet-registry-failed");
            logger.LogError("Failed to register built-in packets.");
            return;
        }
        AndroidClientHost.NotifyBootstrapStep("packet-registry-ready");

        var packetHandlerRegistry = new PacketHandlerRegistry(packetTypeRegistry, logger);
        AndroidClientHost.NotifyBootstrapStep("packet-handler-registry-ready");
        var packetHelper = new PacketHelper(packetTypeRegistry, packetHandlerRegistry);
        _ = FactoryRegistry<IPluginBootstrapContext>.RegisterFactory(PluginBootstrapContext.CreateFactory(typeof(IClientPluginContext), args, parser, packetHelper));
        AndroidClientHost.NotifyBootstrapStep("plugin-factory-ready");

        if (commandLineOptions is null && OperatingSystem.IsAndroid())
        {
            var serverIndex = Array.FindIndex(args, argument => argument is "--server" or "-S");
            var serverValue = serverIndex >= 0 && serverIndex + 1 < args.Length ? args[serverIndex + 1] : null;
            commandLineOptions = new ClientCommandLineOptions { Server = serverValue };
            AndroidClientHost.NotifyBootstrapStep("android-options-fallback");
        }

        if (!string.IsNullOrWhiteSpace(commandLineOptions.WorkingDirectory))
        {
            var workingDirectory = commandLineOptions.WorkingDirectory.Trim();
            var resolvedWorkingDirectory = Path.GetFullPath(workingDirectory);
            if (Directory.Exists(resolvedWorkingDirectory))
            {
                Environment.CurrentDirectory = resolvedWorkingDirectory;
            }
            else
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    "Failed to set working directory to '{Path}', path does not exist: {ResolvedPath}",
                    workingDirectory,
                    resolvedWorkingDirectory
                );
            }
        }

        var clientConfiguration = ClientConfiguration.LoadAndSave();
        AndroidClientHost.NotifyBootstrapStep("configuration-ready");
        loggingLevelSwitch.MinimumLevel = LevelConvert.ToSerilogLevel(clientConfiguration.LogLevel);

        if (commandLineOptions.Server is { } server)
        {
            var serverParts = server.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var host = serverParts.First();

            var portPart = serverParts.Skip(1).FirstOrDefault();
            if (string.IsNullOrEmpty(portPart))
            {
                portPart = "5400";
            }
            var port = ushort.Parse(portPart);

            clientConfiguration.Host = host;
            clientConfiguration.Port = port;
        }

        commandLineOptions = commandLineOptions with
        {
            Server = $"{clientConfiguration.Host}:{clientConfiguration.Port}",
        };

        ClientContext context = new(entryAssembly, commandLineOptions, clientConfiguration, logger, packetHelper);
        AndroidClientHost.NotifyBootstrapStep("context-created");
        context.Start();
    }

    private static ClientCommandLineOptions HandleParsedArguments(ClientCommandLineOptions clientCommandLineOptions) =>
        clientCommandLineOptions;

    private static ClientCommandLineOptions HandleParserErrors(IEnumerable<Error> errors)
    {
        var errorsAsList = errors.ToList();
        var fatalParsingError = errorsAsList.Any(error => error.StopsProcessing);
        var errorString = string.Join(", ", errorsAsList.ToList().Select(error => error.ToString()));

        var exception = new ArgumentException(
            $@"Error parsing command line arguments, received the following errors: {errorString}"
        );

        if (fatalParsingError)
        {
            ApplicationContext.Context.Value?.Logger.LogCritical(
                exception,
                "Critical error during command line argument parsing"
            );
        }
        else
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred during command line argument parsing"
            );
        }

        return default;
    }
}
