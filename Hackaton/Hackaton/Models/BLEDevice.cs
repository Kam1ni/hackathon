using Plugin.BluetoothLE;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hackaton.Models
{
    public class BLEDevice : INotifyPropertyChanged
    {
        private Guid _id;
        private string _name;
        private IDevice _nativeDevice;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public IDevice NativeDevice
        {
            get { return _nativeDevice; }
            set
            {
                _nativeDevice = value;
                OnPropertyChanged();
            }
        }

        public BLEDevice()
        {
            Id = Guid.Empty;
            Name = String.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
