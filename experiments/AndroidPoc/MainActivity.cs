using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Intersect.Client.Framework.Gwen.Control;
using Microsoft.Xna.Framework;
using AndroidProcess = Android.OS.Process;

namespace Intersect.Android.Poc;

[Activity(
    Label = "@string/app_name",
    MainLauncher = true,
    ScreenOrientation = ScreenOrientation.Landscape,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden
)]
public class MainActivity : AndroidGameActivity
{
    private int _copiedResourceFiles;
    private EditText? _imeEditText;
    private TextBox? _activeTextBox;

    private void CopyAssetTree(string assetPath, string targetPath)
    {
        var children = Assets?.List(assetPath) ?? Array.Empty<string>();
        if (children.Length > 0)
        {
            Directory.CreateDirectory(targetPath);
            foreach (var child in children)
            {
                CopyAssetTree($"{assetPath}/{child}", Path.Combine(targetPath, child));
            }

            return;
        }

        if (File.Exists(targetPath))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        using var source = Assets!.Open(assetPath);
        using var destination = File.Create(targetPath);
        source.CopyTo(destination);
        _copiedResourceFiles++;
    }

    private void HandleImeTextChanged(object? sender, TextChangedEventArgs args)
    {
        if (_activeTextBox is not null && _imeEditText is not null && _activeTextBox.Text != _imeEditText.Text)
        {
            _activeTextBox.Text = _imeEditText.Text;
        }
    }

    private void ActivateIme(TextBox textBox)
    {
        RunOnUiThread(() =>
        {
            _activeTextBox = textBox;
            if (_imeEditText is null)
            {
                return;
            }

            _imeEditText.Text = textBox.Text;
            _imeEditText.RequestFocus();
            ((InputMethodManager)GetSystemService(InputMethodService)!).ShowSoftInput(_imeEditText, ShowFlags.Implicit);
        });
    }

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
            var root = new FrameLayout(this);
            root.AddView(view, new FrameLayout.LayoutParams(-1, -1));
            _imeEditText = new EditText(this)
            {
                Alpha = 0.01f,
                Background = null,
            };
            _imeEditText.SetTextColor(global::Android.Graphics.Color.Transparent);
            _imeEditText.SetCursorVisible(false);
            _imeEditText.InputType = global::Android.Text.InputTypes.ClassText | global::Android.Text.InputTypes.TextVariationVisiblePassword;
            _imeEditText.TextChanged += HandleImeTextChanged;
            root.AddView(_imeEditText, new FrameLayout.LayoutParams(1, 1));
            SetContentView(root);
            Lifecycle("SetContentView-return");
        };
        Intersect.Client.Core.AndroidClientHost.StartupFailed = exception =>
            Log.Error("IntersectAndroid", exception.ToString());
        Intersect.Client.Core.AndroidClientHost.TextInputRequested = ActivateIme;
        Intersect.Client.Core.AndroidClientHost.BootstrapStep = step =>
            Log.Info("IntersectAndroid", $"Bootstrap step: {step}");

        var appFilesPath = FilesDir?.AbsolutePath
                           ?? throw new InvalidOperationException("Android app files directory is unavailable.");
        System.Environment.CurrentDirectory = appFilesPath;
        var resourcesPath = Path.GetFullPath(Intersect.Configuration.ClientConfiguration.ResourcesDirectory);
        CopyAssetTree("resources", resourcesPath);
        Log.Info("IntersectAndroid", $"Prepared client resources directory: {resourcesPath}, copiedFiles={_copiedResourceFiles}");

        Log.Info("IntersectAndroid", "Invoking Intersect.Client.Core.Program.Main");
        Intersect.Client.Core.Program.Main(
            typeof(Intersect.Client.Core.Program).Assembly,
            new[] { "--server", "10.0.2.2:5401" }
        );
        Log.Info("IntersectAndroid", "Program.Main returned");
    }
}