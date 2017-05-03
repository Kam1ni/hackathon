using Hackaton.DataModels;
using Hackaton.Managers;
using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;

namespace Hackaton.BC
{
    public class BeanReader
    {
        private List<IDisposable> _characteristics;

        public BeanReader()
        {
            _characteristics = new List<IDisposable>();
        }
        
        /// <summary>
        /// Read the scratch data from the Bean.
        /// </summary>
        public void ReadScratchData()
        {   
            _characteristics = new List<IDisposable>();
            
            var characteristicDiscover = App.ConnectedDevice.NativeDevice.WhenAnyCharacteristicDiscovered().Subscribe(characteristic =>
            {
                if (characteristic.Service.Uuid != Constants.BeanServiceScratchDataUuid) return;
                if (characteristic.Uuid != Constants.BeanCharacteristicScratchDataAccelerometerUuid) return;
                
                _characteristics.Add(characteristic.SubscribeToNotifications().Subscribe(result =>
                {
                    ProcessAccelerometerByteArray(result.Data);
                }));
            });
        }

        public void StopReadingScratchData()
        {
            if (_characteristics == null) return;
            foreach (var characteristic in _characteristics)
            {
                characteristic.Dispose();
            }
        }

        /// <summary>
        /// Process the Byte-array that was given from the Scratch Data of the Bean-device.
        /// </summary>
        /// <param name="byteArray"></param>
        private void ProcessAccelerometerByteArray(byte[] byteArray)
        {
            if (byteArray.Length != 6) return;

            // Create a new Accelerometer-object
            var accelerometer = new Accelerometer();
            accelerometer.X = GetXAxisFromByteArray(byteArray);
            accelerometer.Y = GetYAxisFromByteArray(byteArray);
            accelerometer.Z = GetZAxisFromByteArray(byteArray);

            // And insert it into SQLite
            DatabaseManager.Instance.AccelerometerTable.Insert(accelerometer);
        }

        /// <summary>
        /// Get the Integer-value for the X-Axis from the Byte-array.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private int GetXAxisFromByteArray(byte[] byteArray)
        {
            byte[] byteArrayXAxis = new byte[] { byteArray[0], byteArray[1] };
            return BitConverter.ToInt16(byteArrayXAxis, 0);
        }

        /// <summary>
        /// Get the Integer-value for the Y-Axis from the Byte-array.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private int GetYAxisFromByteArray(byte[] byteArray)
        {
            byte[] byteArrayYAxis = new byte[] { byteArray[2], byteArray[3] };
            return BitConverter.ToInt16(byteArrayYAxis, 0);
        }

        /// <summary>
        /// Get the Integer-value for the Z-Axis from the Byte-array.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private int GetZAxisFromByteArray(byte[] byteArray)
        {
            byte[] byteArrayZAxis = new byte[] { byteArray[4], byteArray[5] };
            return BitConverter.ToInt16(byteArrayZAxis, 0);
        }
    }
}
