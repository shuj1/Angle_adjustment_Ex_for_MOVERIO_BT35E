using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CameraViewPage : Page
    {
        MediaCapture mediaCapture;

        LowLagMediaRecording _mediaRecording;

        DisplayRequest displayRequest = new DisplayRequest();

        private bool isPreviewing;
        private string videoDeviceId;
        private string MoverioCameraId = "VID_1BCF&PID_2CAC"; // 自分のデバイスのIDを入力する

        private DispatcherTimer timer;

        private bool cameraFirstStart = false;
        private int cameraStatus = 0; // 0: stop, 1: active, 2: rec

        public CameraViewPage()
        {
            this.InitializeComponent();
            DeviceSelect();
            Application.Current.Suspending += Application_Suspending;

            // Pageを閉じるときに確認ダイアログを出す
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += CameraViewPage_CloseRequested;

            SetupTimer();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            if (Frame.CurrentSourcePageType == typeof(CameraViewPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }

        private void SetupTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.05);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private async void timer_Tick(object sender, object e)
        {
            // ウィンドウが開かれたとき、カメラを自動で起動する
            if (!cameraFirstStart)
            {
                cameraFirstStart = true;
                StartPreviewAsync();
                cameraStatus = 1;
            }
            await CheckExFlag();
        }

        private async Task CheckExFlag()
        {
            if (SubPage.exFlag)
            {
                timer.Stop();
                RecStart();
                cameraStatus = 2;
                stopRecButton.IsEnabled = true;
                shutDownCameraButton.IsEnabled = false;
                recTB.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        
        private async void CameraViewPage_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            if (cameraStatus != 0)
            {
                System.Diagnostics.Debug.WriteLine(cameraStatus);
                e.Handled = true;
                var d = new MessageDialog("カメラをシャットダウンしてください");
                var okCommand = new UICommand("OK");
                d.Commands.Add(okCommand);
                await d.ShowAsync();
            }
            else
            {
                e.Handled = false;
            }
        }

        private async void StopRecButton_Click(object sender, RoutedEventArgs e)
        {
            RecStop();
            cameraStatus = 1;
            stopRecButton.IsEnabled = false;
            shutDownCameraButton.IsEnabled = true;
            recTB.Foreground = new SolidColorBrush(Colors.LightGray);
        }

        private async void ShutDownCameraButton_Click(object sender, RoutedEventArgs e)
        {
            shutDownCameraButton.IsEnabled = false;
            cameraStatus = 0;
            MainPage.use_camera = false;
            CleanupCameraAsync();
        }

        private async void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            await _mediaRecording.StopAsync();
            ShowMessageToUser("Record limitation exceeded.");
        }

        public async Task<string> GetVideoProfileSupportedDeviceIdAsync(Windows.Devices.Enumeration.Panel panel)
        {
            string deviceId = string.Empty;

            // すべてのビデオキャプチャデバイスを見つける
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            foreach (var device in devices)
            {
                // MoverioCameraIdがわからない場合は、下のコードのコメントを外して調べる
                // ShowMessageToUser(device.Id);
                if (device.Id.Contains(MoverioCameraId))
                {
                    deviceId = device.Id;
                    break;
                }
            }

            return deviceId;
        }

        private async Task DeviceSelect()
        {
            videoDeviceId = await GetVideoProfileSupportedDeviceIdAsync(Windows.Devices.Enumeration.Panel.Back);

            if (string.IsNullOrEmpty(videoDeviceId))
            {
                return;
            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                mediaCapture = new MediaCapture();

                mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;

                var mediaInitSetting = new MediaCaptureInitializationSettings { VideoDeviceId = videoDeviceId };

                await mediaCapture.InitializeAsync(mediaInitSetting);

                displayRequest.RequestActive();

                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                ShowMessageToUser("The app was denied access to the camera");
                return;
            }

            try
            {
                PreviewControl.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
            }
            catch (System.IO.FileLoadException)
            {
                mediaCapture.CaptureDeviceExclusiveControlStatusChanged += _mediaCapture_CaptureDeviceExclusiveControlStatusChanged;
            }
        }

        private async void _mediaCapture_CaptureDeviceExclusiveControlStatusChanged(MediaCapture sender, MediaCaptureDeviceExclusiveControlStatusChangedEventArgs args)
        {
            if (args.Status == MediaCaptureDeviceExclusiveControlStatus.SharedReadOnlyAvailable)
            {
                ShowMessageToUser("The camera preview can't be displayed because another app has exclusive access");
            }
            else if (args.Status == MediaCaptureDeviceExclusiveControlStatus.ExclusiveControlAvailable && !isPreviewing)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await StartPreviewAsync();
                });
            }
        }

        private async Task CleanupCameraAsync()
        {
            if (mediaCapture != null)
            {
                if (isPreviewing)
                {
                    await mediaCapture.StopPreviewAsync();
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PreviewControl.Source = null;
                    if (displayRequest != null)
                    {
                        displayRequest.RequestRelease();
                    }

                    mediaCapture.Dispose();
                    mediaCapture = null;
                });
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }

        private async Task RecStart()
        {
            var myVideos = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Videos);

            StorageFile file = await myVideos.SaveFolder.CreateFileAsync("video.mp4", CreationCollisionOption.GenerateUniqueName);

            _mediaRecording = await mediaCapture.PrepareLowLagRecordToStorageFileAsync(
                MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), file);

            await _mediaRecording.StartAsync();
        }

        private async Task RecStop()
        {
            await _mediaRecording.StopAsync();
            await _mediaRecording.FinishAsync();
        }

        private void ShowMessageToUser(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}
