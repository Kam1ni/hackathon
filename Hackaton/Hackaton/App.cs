using Hackaton.BC;
using Hackaton.Models;
using Hackaton.Views;

using Xamarin.Forms;

namespace Hackaton
{
    public class App : Application
    {
        private static BLEDevice _connectedDevice;
        private static BeanReader _beanReader = new BeanReader();

        /// <summary>
        /// The connected Bluetooth-device.
        /// </summary>
        public static BLEDevice ConnectedDevice
        {
            get { return _connectedDevice; }
            set { _connectedDevice = value; }
        }

        /// <summary>
        /// Class responsible for reading the Bean-characteristics.
        /// </summary>
        public static BeanReader BeanReader => _beanReader;
        
        public App()
        {
            SQLiteManager.Database.DatabaseFilename = "Hackaton.db3";
            MainPage = new NavigationPage(new DeviceList());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
