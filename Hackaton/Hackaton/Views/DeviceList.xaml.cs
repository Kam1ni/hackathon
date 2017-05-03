using Hackaton.Models;
using Plugin.BluetoothLE;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hackaton.Views
{
    public partial class DeviceList : ContentPage
    {
        // We need to use an ObservableCollection to automatically notify the UI that items have been added/removed
        private ObservableCollection<BLEDevice> _devices;

        private IDisposable _scanner;

        /// <summary>
        /// The found Bluetooth devices.
        /// </summary>
        public ObservableCollection<BLEDevice> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                // By calling "OnPropertyChanged" we notify the UI that the collection has changed
                OnPropertyChanged();
            }
        }

        public DeviceList()
        {
            InitializeComponent();
            // By setting the BindingContext to this instance, we enable DataBinding
            BindingContext = this;

            // Initialize "Devices" to avoid NullReference-exceptions!
            Devices = new ObservableCollection<BLEDevice>();
        }

        protected override void OnAppearing()
        {
            if (CrossBleAdapter.Current.Status == AdapterStatus.PoweredOn)
            {
                Scan();
            }
            else
            {
                CrossBleAdapter.Current.WhenStatusChanged().Subscribe(status =>
                {
                    if (status == AdapterStatus.PoweredOn) Scan();
                });
            }
        }
        
        private void Scan()
        {
            // Scan for any Bluetooth device

            // For Android: if we scan using the given config-object we can command the scanner to only scan for Bean-devices
            // This doesn't work in iOS (error in the NuGet-package)
            var config = new ScanConfig();
            if (Device.OS == TargetPlatform.Android) config.ServiceUuid = Constants.BeanServiceAdvertisingUuid;
            
            _scanner = CrossBleAdapter.Current.Scan(config).Subscribe(scanResult =>
            {
                // Only add the Bluetooth devices that have the right advertisement UUID
                // You should be using "new ScanConfig ..." as a parameter in "Scan()" but that's not doing anything (as of today)...
                if (Device.OS == TargetPlatform.iOS)
                {
                    // Since we are unable to use the config-object in iOS, we look at the advertisement-service once we've discovered a device
                    // If this ID isn't equal to our Constant, don't add the device
                    if (scanResult.AdvertisementData == null || scanResult.AdvertisementData.ServiceUuids == null || scanResult.AdvertisementData.ServiceUuids.Count() == 0 || scanResult.AdvertisementData.ServiceUuids[0] != Constants.BeanServiceAdvertisingUuid) return;
                }

                // First check if the found device is already added to the collection
                // The easiest way to do this, is by Id (UUID)
                if (IsDeviceAlreadyAdded(scanResult.Device))
                {
                    // If the device is already added to the collection, no need to re-add it
                    return;
                }

                // Device is not added yet, so add it
                // First create a new instance of "BLEDevice"
                var device = new BLEDevice
                {
                    Id = scanResult.Device.Uuid,
                    Name = scanResult.Device.Name,
                    NativeDevice = scanResult.Device
                };

                // Next add this to the collection
                // Since "Devices" is an ObservableCollection the UI will automatically be notified
                Devices.Add(device);
            });
        }

        /// <summary>
        /// Check if the device is already added to the collection of found devices.
        /// </summary>
        /// <param name="device">The newly found device.</param>
        /// <returns>TRUE = device is already added | FALSE = unknown device</returns>
        private bool IsDeviceAlreadyAdded(IDevice device)
        {
            // Any = once the app has found one item that has its Id equal to device.Uuid, this will return TRUE
            return Devices.Any(x => x.Id.Equals(device.Uuid));
        }

        private void OnItemSelected(object sender, EventArgs e)
        {
            if (((ListView)sender).SelectedItem == null) return;

            _scanner = null;

            var device = ((ListView)sender).SelectedItem as BLEDevice;
            ((ListView)sender).SelectedItem = null;

            App.ConnectedDevice = device;
            
            App.ConnectedDevice.NativeDevice.Connect().Subscribe(result =>
            {
                // Don't do anything
            });
            
            App.ConnectedDevice.NativeDevice.WhenStatusChanged().Subscribe(status =>
            {
                if (status == ConnectionStatus.Connected)
                {
                    Task.Delay(TimeSpan.FromSeconds(1));
                    App.BeanReader.ReadScratchData();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(new DeviceDetail());
                    });
                }
            });
        }
    }
}
