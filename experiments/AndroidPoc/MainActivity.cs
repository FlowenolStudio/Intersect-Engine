using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Microsoft.Xna.Framework;
using AndroidProcess = Android.OS.Process;

namespace Intersect.Android.Poc;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : AndroidGameActivity
{
    private static void Lifecycle(string stage)
    {
        Log.Info(
            "IntersectLifecycle",
            $"stage={stage} androidPid={AndroidProcess.MyPid()} " +
            $"elapsedMs={SystemClock.ElapsedRealtime()} mainThread={Looper.MyLooper() == Looper.MainLooper}"
        );
    }

    protected override void OnStart()
    {
        base.OnStart();
        Lifecycle("OnStart");
    }

    protected override void OnResume()
    {
        try
        {
            base.OnResume();
        }
        catch (NullReferenceException exception)
        {
            Log.Warn("IntersectAndroid", $"MonoGame Android resume state was not initialized: {exception.Message}");
        }
        Lifecycle("OnResume");
    }

    protected override void OnPause()
    {
        Lifecycle("OnPause");
        base.OnPause();
    }

    protected override void OnStop()
    {
        Lifecycle("OnStop");
        base.OnStop();
    }

    protected override void OnDestroy()
    {
        Lifecycle("OnDestroy");
        base.OnDestroy();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Lifecycle("OnCreate");
        Log.Info("IntersectAndroid", "OnCreate: entering client lifecycle");
        Intersect.Client.Core.AndroidClientHost.GameCreated = game =>
        {
            Lifecycle("GameCreated-callback");
            Log.Info("IntersectAndroid", "Client framework created IntersectGame; attaching view");
            var view = (View)game.Services.GetService(typeof(View))!;
            Lifecycle($"SetContentView-begin view={view.GetType().FullName}");
            SetContentView(view);
            Lifecycle("SetContentView-return");
        };
        Intersect.Client.Core.AndroidClientHost.StartupFailed = exception =>
            Log.Error("IntersectAndroid", exception.ToString());
        Intersect.Client.Core.AndroidClientHost.BootstrapStep = step =>
            Log.Info("IntersectAndroid", $"Bootstrap step: {step}");

        var appFilesPath = FilesDir?.AbsolutePath
                           ?? throw new InvalidOperationException("Android app files directory is unavailable.");
        System.Environment.CurrentDirectory = appFilesPath;
        var resourcesPath = Path.GetFullPath(Intersect.Configuration.ClientConfiguration.ResourcesDirectory);
        Directory.CreateDirectory(resourcesPath);
        Log.Info("IntersectAndroid", $"Prepared client resources directory: {resourcesPath}");

        Log.Info("IntersectAndroid", "Invoking Intersect.Client.Core.Program.Main");
        Intersect.Client.Core.Program.Main(
            typeof(Intersect.Client.Core.Program).Assembly,
            new[] { "--server", "10.0.2.2:5401" }
        );
        Log.Info("IntersectAndroid", "Program.Main returned");
    }
}