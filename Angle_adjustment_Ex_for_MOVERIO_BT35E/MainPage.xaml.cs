using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Common c = new Common();

        private ApplicationView _subWindowApplicationView;

        public static bool use_sensor = false;
        public static bool use_camera = false;

        public static bool move_independently = false; // Lineを独立に操作するならtrue

        private bool outputting = false; // データの書き出し中はtrue
        
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(600, 600));
            ApplicationView.PreferredLaunchViewSize = new Size(600, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += MainPage_CloseRequested;

            StoreData sd = new StoreData();
        }

        private async void MainPage_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            e.Handled = true;

            if (CheckExit())
            {
                Application.Current.Exit();
            }
            else
            {
                var d = new MessageDialog("終了を許可できません。以下を確認して下さい。\n\n・カメラのシャットダウン\n・データの書き出し中");
                var okCommand = new UICommand("OK");
                d.Commands.Add(okCommand);
                await d.ShowAsync();
            }
        }

        private async void ShowWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (moveSelectCB.SelectedIndex == 1)
            {
                move_independently = true;
            }

            await SetupSubPage();

            var currentViewId = ApplicationView.GetForCurrentView().Id;

            if (sensorCB.SelectedIndex == 1)
            {
                await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Window.Current.Content = new Frame();
                    ((Frame)Window.Current.Content).Navigate(typeof(SensorDataPage));

                    Window.Current.Activate();
                    await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                        ApplicationView.GetApplicationViewIdForWindow(Window.Current.CoreWindow),
                        ViewSizePreference.Default,
                        currentViewId,
                        ViewSizePreference.Default);
                });
                use_sensor = true;
            }

            if (cameraCB.SelectedIndex == 1)
            {
                await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Window.Current.Content = new Frame();
                    
                    ((Frame)Window.Current.Content).Navigate(typeof(CameraViewPage));

                    Window.Current.Activate();
                    await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                        ApplicationView.GetApplicationViewIdForWindow(Window.Current.CoreWindow),
                        ViewSizePreference.Default,
                        currentViewId,
                        ViewSizePreference.Default);
                });
                use_camera = true;
            }

            showWindowButton.IsEnabled = false;
            startTrialButton.IsEnabled = true;
        }

        private async void StartTrialButton_Click(object sender, RoutedEventArgs e)
        {
            SubPage.exFlag = true;
            startTrialButton.IsEnabled = false;
        }

        private async void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            outputting = true;
            StoreData.OutputTrialDataAsync();
            StoreData.OutputSensorDataAsync();
            outputting = false;
        }

        private async void EndButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async Task SetupSubPage()
        {
            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _subWindowApplicationView = ApplicationView.GetForCurrentView();
                Window.Current.Content = new SubPage();
                Window.Current.Activate();
            });

            await ProjectionManager.StartProjectingAsync(_subWindowApplicationView.Id, ApplicationView.GetForCurrentView().Id);
        }
        
        private bool CheckExit()
        {
            if (use_camera == false && outputting == false)
            {
                // カメラ使用中かどうか, ファイルの書き込みが終了しているかどうか確認する
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
