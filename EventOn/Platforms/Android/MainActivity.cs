using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using OneSignalSDK.DotNet;

namespace EventOn
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle SavedInstanceState)
        {
            base.OnCreate(SavedInstanceState);
            RequestedOrientation = ScreenOrientation.Portrait;

            ActivityCompat.RequestPermissions(this, [Manifest.Permission.Camera, Manifest.Permission.RecordAudio, Manifest.Permission.ModifyAudioSettings], 0);
        }
    }
}
