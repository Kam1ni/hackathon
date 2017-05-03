using System;

namespace Hackaton.DataModels
{
    public class Accelerometer : SQLiteManager.DataModels.BaseDataModel
    {
        private int _x;
        private int _y;
        private int _z;
        private DateTime _dateTime;

        public int X
        {
            get { return _x; }
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }
        public int Y
        {
            get { return _y; }
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }
        public int Z
        {
            get { return _z; }
            set
            {
                _z = value;
                OnPropertyChanged();
            }
        }
        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                OnPropertyChanged();
            }
        }

        public Accelerometer()
            : base()
        {
            X = 0;
            Y = 0;
            Z = 0;
            DateTime = DateTime.Now.ToLocalTime();
        }

        public Accelerometer(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
