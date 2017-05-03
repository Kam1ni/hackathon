using System;

namespace Hackaton.Models
{
    public class DataPointAccelerometer
    {
        public string Date { get; set; }
        public int AxisValue { get; set; }

        public DataPointAccelerometer()
        {
            Date = String.Empty;
            AxisValue = 0;
        }

        public DataPointAccelerometer(string date, int axisValue)
            : this()
        {
            Date = date;
            AxisValue = axisValue;
        }

        public DataPointAccelerometer(DateTime date, int axisValue)
            : this(DateTime.Now.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), axisValue)
        {

        }
    }
}
