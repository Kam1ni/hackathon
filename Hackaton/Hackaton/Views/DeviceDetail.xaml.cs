using ExtensionMethodCollection.Extensions;
using Hackaton.DataModels;
using Hackaton.Managers;
using Hackaton.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hackaton.Views
{
    public partial class DeviceDetail : ContentPage
    {
        private ObservableCollection<DataPointAccelerometer> _xAxisSeries;
        private ObservableCollection<DataPointAccelerometer> _yAxisSeries;
        private ObservableCollection<DataPointAccelerometer> _zAxisSeries;

        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// The different values of the Accelerometer for the X-Axis.
        /// </summary>
        public ObservableCollection<DataPointAccelerometer> XAxisSeries
        {
            get { return _xAxisSeries; }
            set
            {
                _xAxisSeries = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The different values of the Accelerometer for the Y-Axis.
        /// </summary>
        public ObservableCollection<DataPointAccelerometer> YAxisSeries
        {
            get { return _yAxisSeries; }
            set
            {
                _yAxisSeries = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The different values of the Accelerometer for the Z-Axis.
        /// </summary>
        public ObservableCollection<DataPointAccelerometer> ZAxisSeries
        {
            get { return _zAxisSeries; }
            set
            {
                _zAxisSeries = value;
                OnPropertyChanged();
            }
        }

        public DeviceDetail()
        {
            InitializeComponent();

            _cancellationTokenSource = new CancellationTokenSource();

            XAxisSeries = new ObservableCollection<DataPointAccelerometer>();
            YAxisSeries = new ObservableCollection<DataPointAccelerometer>();
            ZAxisSeries = new ObservableCollection<DataPointAccelerometer>();

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            CancellationToken token = _cancellationTokenSource.Token;

            // Use an extension method to run a Task continuously
            Task.Factory.StartNewContinuously(() =>
            {
                try
                {
                    if (token.IsCancellationRequested) return;

                    // Use temp-objects so we don't interfere with the UI
                    var tempX = new ObservableCollection<DataPointAccelerometer>();
                    var tempY = new ObservableCollection<DataPointAccelerometer>();
                    var tempZ = new ObservableCollection<DataPointAccelerometer>();

                    GetAccelerometerReadings(out tempX, out tempY, out tempZ);

                    // Once the temp-collections have been initialized and given value
                    // initialize the real series-collections to these temp
                    // This must be done on the Main UI-thread or we will get exceptions
                    // (controls on the UI can only be updated from the UI-thread!!!)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        XAxisSeries = tempX;
                        YAxisSeries = tempY;
                        ZAxisSeries = tempZ;
                    });
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            },
            token,
            TimeSpan.FromSeconds(1));
        }

        protected override void OnDisappearing()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void GetAccelerometerReadings(out ObservableCollection<DataPointAccelerometer> xSeries, out ObservableCollection<DataPointAccelerometer> ySeries, out ObservableCollection<DataPointAccelerometer> zSeries)
        {
            xSeries = new ObservableCollection<DataPointAccelerometer>();
            ySeries = new ObservableCollection<DataPointAccelerometer>();
            zSeries = new ObservableCollection<DataPointAccelerometer>();

            // We want to show the result for x seconds
            // In the table are a lot more readings (maybe 10 per second or more)
            // So we need to make an average for every second

            // First collect all readings between now and x seconds ago
            DateTime start = DateTime.Now.ToLocalTime().AddSeconds(-25);
            DateTime end = DateTime.Now.ToLocalTime();
            var list = DatabaseManager.Instance.AccelerometerTable.SelectBetweenDates(start, end);
            if (list == null) return;

            // Then initialize an empty Dictionary where the averages will be stored
            var dictionaryX = DictionaryExtensions.NewEmptyIntegerDictionaryForEverySecondInTimespan(start, end);
            var dictionaryY = DictionaryExtensions.NewEmptyIntegerDictionaryForEverySecondInTimespan(start, end);
            var dictionaryZ = DictionaryExtensions.NewEmptyIntegerDictionaryForEverySecondInTimespan(start, end);

            // Next loop through all seconds and fetch the readings for that second

            // Also keep track of the previous x, y and z-axes
            int averageX = 0;
            int averageY = 0;
            int averageZ = 0;
            start.ForEachSecondInTimespan(end, (second) =>
            {
                second = new DateTime(second.Year, second.Month, second.Day, second.Hour, second.Minute, second.Second);
                // First fetch all readings for this second
                var readingsCurrentSecond = GetReadingsForSecond(second, list.ToList());
                if (!readingsCurrentSecond.IsEmpty())
                {
                    // If readings for this second are found: calculate the average for this second
                    averageX = readingsCurrentSecond.Sum(accelerometer => accelerometer.X) / readingsCurrentSecond.Count;
                    averageY = readingsCurrentSecond.Sum(accelerometer => accelerometer.Y) / readingsCurrentSecond.Count;
                    averageZ = readingsCurrentSecond.Sum(accelerometer => accelerometer.Z) / readingsCurrentSecond.Count;
                }
                // If no readings are found for this second, just insert the the previous values again

                // This average can be inserted into the dictionary
                dictionaryX.AddOrUpdate(second, averageX);
                dictionaryY.AddOrUpdate(second, averageY);
                dictionaryZ.AddOrUpdate(second, averageZ);
            });

            var tempList = new List<DataPointAccelerometer>();
            dictionaryX.ForEach((second, average) => tempList.Add(new DataPointAccelerometer(second, average)));
            xSeries = new ObservableCollection<DataPointAccelerometer>(tempList);

            tempList = new List<DataPointAccelerometer>();
            dictionaryY.ForEach((second, average) => tempList.Add(new DataPointAccelerometer(second, average)));
            ySeries = new ObservableCollection<DataPointAccelerometer>(tempList);

            tempList = new List<DataPointAccelerometer>();
            dictionaryZ.ForEach((second, average) => tempList.Add(new DataPointAccelerometer(second, average)));
            zSeries = new ObservableCollection<DataPointAccelerometer>(tempList);
        }

        /// <summary>
        /// Get all accelerometer-readings for a specific second.
        /// </summary>
        /// <param name="second"></param>
        /// <param name="allReadings"></param>
        /// <returns></returns>
        private List<Accelerometer> GetReadingsForSecond(DateTime second, List<Accelerometer> allReadings)
        {
            DateTime end = second.AddSeconds(1);

            var list = new List<Accelerometer>();

            allReadings.ForEach((reading) =>
            {
                if (reading.DateTime >= second && reading.DateTime <= end) list.Add(reading);
            });

            return list;
        }
    }
}
