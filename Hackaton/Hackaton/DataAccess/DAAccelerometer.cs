using Hackaton.DataModels;
using SQLiteManager.DataAccess;
using System;
using System.Collections.Generic;

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
    }
}
