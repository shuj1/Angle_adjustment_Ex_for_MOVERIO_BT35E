using Angle_adjustment_Ex_for_MOVERIO_BT35E.Watchers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E.Sensors
{
    public sealed partial class AccelerometerControl : UserControl, ISensor
    {
        private readonly int keepRecords = 200;
        private readonly AccelerometerWatcher watcher = new AccelerometerWatcher();
        private Accelerometer sensor;

        public ObservableCollection<SensorData> ValueX { get; } = new ObservableCollection<SensorData>();
        public ObservableCollection<SensorData> ValueY { get; } = new ObservableCollection<SensorData>();
        public ObservableCollection<SensorData> ValueZ { get; } = new ObservableCollection<SensorData>();

        public AccelerometerControl()
        {
            InitializeComponent();
        }

        public void Active()
        {
            ClearValues();
            watcher.Add += Watcher_AddAsync;
            watcher.Remove += Watcher_Remove;
            watcher.Start();
        }

        public void Inactive()
        {
            watcher.Stop();
            watcher.Add -= Watcher_AddAsync;
            watcher.Remove -= Watcher_Remove;

            Disable();
        }

        private async void Watcher_AddAsync(object sender, string deviceId)
        {
            await EnableAsync(deviceId).ConfigureAwait(false);
        }

        private void Watcher_Remove(object sender, string deviceId)
        {
            Disable();
        }

        private async Task EnableAsync(string deviceId)
        {
            if (sensor != null)
            {
                Disable();
            }

            sensor = await Accelerometer.FromIdAsync(deviceId);
            sensor.ReadingChanged += Sensor_ReadingChangedAsync;
        }

        private void Disable()
        {
            if (sensor == null)
            {
                return;
            }

            sensor.ReadingChanged -= Sensor_ReadingChangedAsync;
            sensor = null;
        }

        private async void Sensor_ReadingChangedAsync(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Task.Run(() =>
            {
                StoreData.current_ValueX = args.Reading.AccelerationX;
                StoreData.current_ValueY = args.Reading.AccelerationY;
                StoreData.current_ValueZ = args.Reading.AccelerationZ;
            });

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AxisX.Text = args.Reading.AccelerationX.ToString();
                AxisY.Text = args.Reading.AccelerationY.ToString();
                AxisZ.Text = args.Reading.AccelerationZ.ToString();

                AddValues(ValueX, args.Reading.AccelerationX);
                AddValues(ValueY, args.Reading.AccelerationY);
                AddValues(ValueZ, args.Reading.AccelerationZ);
            });
        }

        private void AddValues(ObservableCollection<SensorData> values, double value)
        {
            values.Add(new SensorData() { Value = value });

            if (values.Count > keepRecords)
            {
                values.RemoveAt(0);
            }
        }

        private void ClearValues()
        {
            ValueX.Clear();
            ValueY.Clear();
            ValueZ.Clear();

            AxisX.Text = string.Empty;
            AxisY.Text = string.Empty;
            AxisZ.Text = string.Empty;
        }
    }
}