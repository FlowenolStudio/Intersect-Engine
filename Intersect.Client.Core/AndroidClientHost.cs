using Intersect.Client.Framework.Gwen.Control;
using Microsoft.Xna.Framework;

namespace Intersect.Client.Core;

/// <summary>
/// Minimal platform seam for hosts that must attach the MonoGame view before Run.
/// Desktop hosts do not need to subscribe to this callback.
/// </summary>
public static class AndroidClientHost
{
    public static Action<Game>? GameCreated { get; set; }

    public static Action<Exception>? StartupFailed { get; set; }

    public static Action<string>? BootstrapStep { get; set; }

    public static Action<TextBox>? TextInputRequested { get; set; }

    internal static void NotifyGameCreated(Game game) => GameCreated?.Invoke(game);

    internal static void NotifyStartupFailed(Exception exception) => StartupFailed?.Invoke(exception);

    internal static void NotifyBootstrapStep(string step) => BootstrapStep?.Invoke(step);

    internal static void NotifyTextInputRequested(TextBox textBox) => TextInputRequested?.Invoke(textBox);
}
