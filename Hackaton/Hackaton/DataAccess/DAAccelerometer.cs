using Hackaton.DataModels;
using SQLiteManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hackaton.DataAccess
{
    public class DAAccelerometer : BaseDataAccess<Accelerometer>
    {
        public DAAccelerometer()
            : base()
        {

        }

        /// <summary>
        /// Select the Accelerometer-records between 2 DateTime-objects.
        /// </summary>
        /// <param name="start">The start DateTime.</param>
        /// <param name="end">The end DateTime.</param>
        /// <returns>A collection of Accelerometer-records.</returns>
        public IEnumerable<Accelerometer> SelectBetweenDates(DateTime start, DateTime end)
        {
            return PerformQuery(() =>
            {
                return AsyncConnection.Table<Accelerometer>()
                    .Where(x => x.DateTime >= start && x.DateTime <= end)
                    .ToListAsync();
            });
        }

        public void SendLedValues(int red, int green, int blue)
        {
            byte[] redB = BitConverter.GetBytes(red);
            byte[] greenB = BitConverter.GetBytes(green);
            byte[] blueB = BitConverter.GetBytes(blue);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(redB);
                Array.Reverse(greenB);
                Array.Reverse(blueB);
            }
            Debug.WriteLine(red);
            Debug.WriteLine(redB);
            App.ConnectedDevice.NativeDevice.BeginReliableWriteTransaction().Write(null, new byte[] { redB[0], greenB[0], blueB[0] });
        }
    }
}
