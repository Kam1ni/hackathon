using Hackaton.DataAccess;
using System;

namespace Hackaton.Managers
{
    public class DatabaseManager
    {
        private static Lazy<DatabaseManager> lazy = new Lazy<DatabaseManager>(() => new DatabaseManager());

        /// <summary>
        /// The instance of the DatabaseManager.
        /// </summary>
        public static DatabaseManager Instance
        {
            get { return lazy.Value; }
        }

        private DAAccelerometer _daAccelerometer;

        /// <summary>
        /// The entry-point to the Accelerometer-table.
        /// </summary>
        public DAAccelerometer AccelerometerTable => _daAccelerometer;

        public DatabaseManager()
        {
            _daAccelerometer = new DAAccelerometer();
        }
    }
}
