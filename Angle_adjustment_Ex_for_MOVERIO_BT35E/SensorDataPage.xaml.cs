using Angle_adjustment_Ex_for_MOVERIO_BT35E.Sensors;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SensorDataPage : Page
    {
        private ISensor currentSensor;
        
        public SensorDataPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Application.Current.Resuming += Current_Resuming;
            Application.Current.Suspending += Current_Suspending;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Application.Current.Resuming -= Current_Resuming;
            Application.Current.Suspending -= Current_Suspending;
        }

        private void Current_Resuming(object sender, object e)
        {
            currentSensor?.Active();
        }

        private void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            currentSensor?.Inactive();
        }

        private void SensorsTab_SelectionChanged(object sender, SelectionChangedEventArgs _)
        {
            SensorChanges();
        }

        private void SensorChanges()
        {
            var tabItem = sensorsTab.SelectedItem as TabViewItem;
            if (!(tabItem.Content is ISensor sensor))
            {
                return;
            }

            currentSensor?.Inactive();
            sensor.Active();

            currentSensor = sensor;
        }
    }
}
