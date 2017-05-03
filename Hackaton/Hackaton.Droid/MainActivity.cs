using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Syncfusion.SfChart.XForms.Droid;
using Xamarin.Forms.Platform.Android;

namespace Hackaton.Droid
{
    [Activity(Label = "Hackaton", Icon = "@drawable/thomas_more", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        private readonly string[] Permissions =
        {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.BluetoothPrivileged,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            global::Xamarin.Forms.Forms.Init(this, bundle);
            new SfChartRenderer();

            CheckPermissions();
            
            Plugin.BluetoothLE.AndroidConfig.ForcePreLollipopScanner = true;
            
            LoadApplication(new App());
        }
        
        private void CheckPermissions()
        {
            // We need to have at least the following permissions
            // From Android 7.0 we need to have Location-permission in order to scan and connect to a Bluetooth LE device ... for some reason ...
            if (Android.OS.Build.VERSION.SdkInt <= BuildVersionCodes.M)
            {
                if (CheckSelfPermission(Manifest.Permission.Bluetooth) != Permission.Granted ||
                CheckSelfPermission(Manifest.Permission.BluetoothAdmin) != Permission.Granted ||
                CheckSelfPermission(Manifest.Permission.BluetoothPrivileged) != Permission.Granted ||
                CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted ||
                CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(Permissions, 0);
                }
            }
            else
            {
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) != Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothAdmin) != Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothPrivileged) != Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, Permissions, 0);
                }
            }
        }
    }
}

