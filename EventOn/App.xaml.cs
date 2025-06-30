using OneSignalSDK.DotNet;
using OneSignalSDK.DotNet.Core;
using OneSignalSDK.DotNet.Core.Debug;

namespace EventOn;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
        {
            OneSignal.Initialize("625b9b85-1b80-48db-93cb-ee03bbded11d");
            OneSignal.Notifications.RequestPermissionAsync(true);
        }

        MainPage = new MainPage();
    }
}
