using Android.App;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace Intersect.Android.Poc;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : AndroidGameActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var game = new Game1();
        SetContentView((View)game.Services.GetService(typeof(View))!);
        game.Run();
    }
}