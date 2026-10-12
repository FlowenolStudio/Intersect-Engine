using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Microsoft.Xna.Framework;

namespace Intersect.Android.Poc;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : AndroidGameActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Log.Info("IntersectAndroid", "OnCreate: entering client lifecycle");
        Intersect.Client.Core.AndroidClientHost.GameCreated = game =>
        {
            Log.Info("IntersectAndroid", "Client framework created IntersectGame; attaching view");
            SetContentView((View)game.Services.GetService(typeof(View))!);
        };
        Intersect.Client.Core.AndroidClientHost.StartupFailed = exception =>
            Log.Error("IntersectAndroid", exception.ToString());
        Intersect.Client.Core.AndroidClientHost.BootstrapStep = step =>
            Log.Info("IntersectAndroid", $"Bootstrap step: {step}");

        Log.Info("IntersectAndroid", "Invoking Intersect.Client.Core.Program.Main");
        Intersect.Client.Core.Program.Main(
            typeof(Intersect.Client.Core.Program).Assembly,
            new[] { "--server", "10.0.2.2:5401" }
        );
        Log.Info("IntersectAndroid", "Program.Main returned");
    }
}