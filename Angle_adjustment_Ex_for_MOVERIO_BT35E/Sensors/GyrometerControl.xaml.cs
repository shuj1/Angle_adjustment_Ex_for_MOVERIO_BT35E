using Angle_adjustment_Ex_for_MOVERIO_BT35E.Watchers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E.Sensors
{
    public sealed partial class GyrometerControl : UserControl, ISensor
    {
        private readonly int keepRecords = 200;
        private readonly GyrometerWatcher watcher = new GyrometerWatcher();
        private Gyrometer sensor;

        public ObservableCollection<SensorData> ValueX { get; } = new ObservableCollection<SensorData>();
        public ObservableCollection<SensorData> ValueY { get; } = new ObservableCollection<SensorData>();
        public ObservableCollection<SensorData> ValueZ { get; } = new ObservableCollection<SensorData>();

        public GyrometerControl()
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

            sensor = await Gyrometer.FromIdAsync(deviceId);
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

        private async void Sensor_ReadingChangedAsync(Gyrometer sender, GyrometerReadingChangedEventArgs args)
        {
            await Task.Run(() =>
            {
                StoreData.current_ValueX = args.Reading.AngularVelocityX;
                StoreData.current_ValueY = args.Reading.AngularVelocityY;
                StoreData.current_ValueZ = args.Reading.AngularVelocityZ;
            });

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AxisX.Text = args.Reading.AngularVelocityX.ToString();
                AxisY.Text = args.Reading.AngularVelocityY.ToString();
                AxisZ.Text = args.Reading.AngularVelocityZ.ToString();

                AddValues(ValueX, args.Reading.AngularVelocityX);
                AddValues(ValueY, args.Reading.AngularVelocityY);
                AddValues(ValueZ, args.Reading.AngularVelocityZ);
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
