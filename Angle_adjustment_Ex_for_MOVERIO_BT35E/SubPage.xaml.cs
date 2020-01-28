using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SubPage : Page
    {
        Common c = new Common();

        Random rand = new Random();

        private DispatcherTimer timer;

        Canvas mainCanvas;

        Canvas leftEyeCanvas;
        TextBlock leftTextBlock;
        Ellipse leftFixEllipse;
        Line leftLine;
       
        Canvas rightEyeCanvas;
        TextBlock rightTextBlock;
        Ellipse rightFixEllipse;
        Line rightLine;

        public static double leftLineDeg = 0.0;
        public static double rightLineDeg = 0.0;

        public static bool exFlag = false; // トライアル実行時はtrue
        private int phase = 0; // 0: 準備中表示時, 1: トライアル数表示時, 2: Line操作時

        public static int nTrials = 50; // トライアル数
        private int currentTrial; // 現在のトライアル

        private double unitDeg = 0.1; // 操作する角度の単位 (unitAngle)deg刻みで動かす
        private double rangeAngle = 30; // Lineの初期最大角度

        public SubPage()
        {
            this.InitializeComponent();
            setupCanvas();
            setupObjects();
            SetupTimer();
        }

        private void setupCanvas()
        {
            mainCanvas = new Canvas();
            mainCanvas.Width = c.SmartGlasses_ScreenWidth;
            mainCanvas.Height = c.SmartGlasses_ScreenHeight;
            this.Content = mainCanvas;

            leftEyeCanvas = new Canvas();
            leftEyeCanvas.Background = new SolidColorBrush(Colors.Black);
            leftEyeCanvas.Width = c.SmartGlasses_ScreenWidth;
            leftEyeCanvas.Height = c.SmartGlasses_ScreenHeight;
            leftEyeCanvas.Margin = new Thickness(0, 0, c.SmartGlasses_ScreenWidth / 2, 0);
            leftEyeCanvas.Scale = new Vector3(0.5f, 1.0f, 1.0f); // side-by-side方式はX方向を1/2に縮小
            mainCanvas.Children.Add(leftEyeCanvas);

            rightEyeCanvas = new Canvas();
            rightEyeCanvas.Background = new SolidColorBrush(Colors.Black);
            rightEyeCanvas.Width = c.SmartGlasses_ScreenWidth;
            rightEyeCanvas.Height = c.SmartGlasses_ScreenHeight;
            rightEyeCanvas.Margin = new Thickness(c.SmartGlasses_ScreenWidth / 2, 0, 0, 0);
            rightEyeCanvas.Scale = new Vector3(0.5f, 1.0f, 1.0f); // side-by-side方式はX方向を1/2に縮小
            mainCanvas.Children.Add(rightEyeCanvas);
        }
        
        private void setupObjects()
        {
            leftTextBlock = new TextBlock();
            leftTextBlock.Text = "準備中...";
            leftTextBlock.Foreground = new SolidColorBrush(Colors.White);
            leftTextBlock.FontSize = 50;
            leftTextBlock.TextAlignment = TextAlignment.Center;
            leftTextBlock.Width = 250;
            leftTextBlock.Height = 100;
            leftTextBlock.Margin = new Thickness(c.SmartGlasses_ScreenWidth / 2 - leftTextBlock.Width / 2,
                                                 c.SmartGlasses_ScreenHeight / 2 - leftTextBlock.Height / 2,
                                                 0,
                                                 0);
            leftEyeCanvas.Children.Add(leftTextBlock);

            rightTextBlock = new TextBlock();
            rightTextBlock.Text = "準備中...";
            rightTextBlock.Foreground = new SolidColorBrush(Colors.White);
            rightTextBlock.FontSize = 50;
            rightTextBlock.TextAlignment = TextAlignment.Center;
            rightTextBlock.Width = 250;
            rightTextBlock.Height = 100;
            rightTextBlock.Margin = new Thickness(c.SmartGlasses_ScreenWidth / 2 - rightTextBlock.Width / 2,
                                                  c.SmartGlasses_ScreenHeight / 2 - rightTextBlock.Height / 2,
                                                  0,
                                                  0);
            rightEyeCanvas.Children.Add(rightTextBlock);

            leftLine = new Line();
            leftLine.X1 = c.SmartGlasses_ScreenWidth / 2 - c.PixelConverter(c.Distance_fixEllipse_and_Line + c.lineSize);
            leftLine.Y1 = c.SmartGlasses_ScreenHeight / 2;
            leftLine.X2 = leftLine.X1 + c.PixelConverter(c.lineSize);
            leftLine.Y2 = c.SmartGlasses_ScreenHeight / 2;
            leftLine.StrokeThickness = c.PixelConverter(c.lineThickness);
            leftLine.Stroke = new SolidColorBrush(Colors.White);

            leftFixEllipse = new Ellipse();
            leftFixEllipse.Width = c.PixelConverter(c.fixEllipseSize);
            leftFixEllipse.Height = c.PixelConverter(c.fixEllipseSize);
            leftFixEllipse.Margin = new Thickness(c.SmartGlasses_ScreenWidth / 2 - c.PixelConverter(c.fixEllipseSize) / 2, 
                                                  c.SmartGlasses_ScreenHeight / 2 - c.PixelConverter(c.fixEllipseSize) / 2,
                                                  c.SmartGlasses_ScreenWidth / 2 - c.PixelConverter(c.fixEllipseSize) / 2,
                                                  c.SmartGlasses_ScreenHeight / 2 - c.PixelConverter(c.fixEllipseSize) / 2);
            leftFixEllipse.Fill = new SolidColorBrush(Colors.White);

            rightFixEllipse = new Ellipse();
            rightFixEllipse.Width = c.PixelConverter(c.fixEllipseSize);
            rightFixEllipse.Height = c.PixelConverter(c.fixEllipseSize);
            rightFixEllipse.Margin = new Thickness(c.SmartGlasses_ScreenWidth / 2 - c.PixelConverter(c.fixEllipseSize) / 2,
                                                   c.SmartGlasses_ScreenHeight / 2 - c.PixelConverter(c.fixEllipseSize) / 2,
                                                   c.SmartGlasses_ScreenWidth / 2 - c.PixelConverter(c.fixEllipseSize) / 2,
                                                   c.SmartGlasses_ScreenHeight / 2 - c.PixelConverter(c.fixEllipseSize) / 2);
            rightFixEllipse.Fill = new SolidColorBrush(Colors.White);

            rightLine = new Line();
            rightLine.X1 = c.SmartGlasses_ScreenWidth / 2 + c.PixelConverter(c.Distance_fixEllipse_and_Line);
            rightLine.Y1 = c.SmartGlasses_ScreenHeight / 2;
            rightLine.X2 = rightLine.X1 + c.PixelConverter(c.lineSize);
            rightLine.Y2 = c.SmartGlasses_ScreenHeight / 2;
            rightLine.StrokeThickness = c.PixelConverter(c.lineThickness);
            rightLine.Stroke = new SolidColorBrush(Colors.White);
        }

        private void SetupTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.02);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private async void timer_Tick(object sender, object e)
        {
            await CheckStartTrial();
            await CheckGamepadInput();
        }

        private async Task CheckStartTrial()
        {
            if (exFlag == true && phase == 0)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    currentTrial = 1;
                    leftTextBlock.Text = $"{currentTrial} / {nTrials}";
                    rightTextBlock.Text = $"{currentTrial} / {nTrials}";
                    phase++;
                });
            }
        }

        private async Task CheckGamepadInput()
        {
            if (exFlag == true && Gamepad.Gamepads.Count > 0)
            {
                Gamepad gamepad = Gamepad.Gamepads.First();
                GamepadReading reading = gamepad.GetCurrentReading();

                switch (phase)
                {
                    case 1:
                        // トライアル数表示時
                        if (reading.Buttons == GamepadButtons.B)
                        {  
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                leftEyeCanvas.Children.Remove(leftTextBlock);
                                rightEyeCanvas.Children.Remove(rightTextBlock);

                                ObjectSetActivate();
                            });
                            phase++;
                        }
                        break;
                    case 2:
                        // Line操作時
                        double leftStickY = reading.LeftThumbstickY;
                        double rightStickY = reading.RightThumbstickY;

                        if (leftStickY > 0.7)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                RotateTransform leftRotateTransform = new RotateTransform();
                                leftRotateTransform.Angle = leftLineDeg + unitDeg;
                                leftRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                leftRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                leftLine.RenderTransform = leftRotateTransform;
                                leftLineDeg += unitDeg;

                                if (!MainPage.move_independently)
                                {
                                    RotateTransform rightRotateTransform = new RotateTransform();
                                    rightRotateTransform.Angle = rightLineDeg - unitDeg;
                                    rightRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                    rightRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                    rightLine.RenderTransform = rightRotateTransform;
                                    rightLineDeg -= unitDeg;
                                }
                            });
                        }
                        else if (leftStickY < -0.7)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                RotateTransform leftRotateTransform = new RotateTransform();
                                leftRotateTransform.Angle = leftLineDeg - unitDeg;
                                leftRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                leftRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                leftLine.RenderTransform = leftRotateTransform;
                                leftLineDeg -= unitDeg;

                                if (!MainPage.move_independently)
                                {
                                    RotateTransform rightRotateTransform = new RotateTransform();
                                    rightRotateTransform.Angle = rightLineDeg + unitDeg;
                                    rightRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                    rightRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                    rightLine.RenderTransform = rightRotateTransform;
                                    rightLineDeg += unitDeg;
                                }
                            });
                        }

                        if (rightStickY > 0.7)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                RotateTransform rightRotateTransform = new RotateTransform();
                                rightRotateTransform.Angle = rightLineDeg - unitDeg;
                                rightRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                rightRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                rightLine.RenderTransform = rightRotateTransform;
                                rightLineDeg -= unitDeg;

                                if (!MainPage.move_independently)
                                {
                                    RotateTransform leftRotateTransform = new RotateTransform();
                                    leftRotateTransform.Angle = leftLineDeg + unitDeg;
                                    leftRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                    leftRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                    leftLine.RenderTransform = leftRotateTransform;
                                    leftLineDeg += unitDeg;
                                }
                            });
                        }
                        else if (rightStickY < -0.7)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                RotateTransform rotateTransform = new RotateTransform();
                                rotateTransform.Angle = rightLineDeg + unitDeg;
                                rotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                rotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                rightLine.RenderTransform = rotateTransform;
                                rightLineDeg += unitDeg;

                                if (!MainPage.move_independently)
                                {
                                    RotateTransform leftRotateTransform = new RotateTransform();
                                    leftRotateTransform.Angle = leftLineDeg - unitDeg;
                                    leftRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
                                    leftRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
                                    leftLine.RenderTransform = leftRotateTransform;
                                    leftLineDeg -= unitDeg;
                                }
                            });
                        }

                        if (reading.Buttons == GamepadButtons.RightShoulder)
                        {
                            StoreData.RecordRespData(leftLineDeg, rightLineDeg);

                            leftEyeCanvas.Children.Remove(leftLine);
                            leftEyeCanvas.Children.Remove(leftFixEllipse);
                            rightEyeCanvas.Children.Remove(rightLine);
                            rightEyeCanvas.Children.Remove(rightFixEllipse);

                            if (currentTrial == nTrials)
                            {
                                // すべてのトライアルが終了したとき
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    leftTextBlock.Text = "終了しました";
                                    rightTextBlock.Text = "終了しました";

                                    leftEyeCanvas.Children.Add(leftTextBlock);
                                    rightEyeCanvas.Children.Add(rightTextBlock);
                                });
                                exFlag = false;
                            }
                            else
                            {
                                currentTrial++;

                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    leftTextBlock.Text = $"{currentTrial} / {nTrials}";
                                    rightTextBlock.Text = $"{currentTrial} / {nTrials}";

                                    leftEyeCanvas.Children.Add(leftTextBlock);
                                    rightEyeCanvas.Children.Add(rightTextBlock);
                                });
                                phase = 1;
                            }         
                        }
                        break;
                }          
            }
        }

        private void ObjectSetActivate()
        {
            if (MainPage.move_independently)
            {
                // 独立して動かすとき
                if (rand.NextDouble() < 0.5)
                {
                    leftLineDeg = Math.Round(rand.NextDouble() * rangeAngle, 1);
                }
                else
                {
                    leftLineDeg = Math.Round(rand.NextDouble() * -rangeAngle, 1);
                }

                if (rand.NextDouble() < 0.5)
                {
                    rightLineDeg = Math.Round(rand.NextDouble() * rangeAngle, 1);
                }
                else
                {
                    rightLineDeg = Math.Round(rand.NextDouble() * -rangeAngle, 1);
                }
            }
            else
            {
                // 同時に動かすとき
                if (rand.NextDouble() < 0.5)
                {
                    leftLineDeg = Math.Round(rand.NextDouble() * rangeAngle, 1);
                }
                else
                {
                    leftLineDeg = Math.Round(rand.NextDouble() * -rangeAngle, 1);
                }
                rightLineDeg = -leftLineDeg; // 符号が反対であることに注意！
            }

            RotateTransform leftRotateTransform = new RotateTransform();
            leftRotateTransform.Angle = leftLineDeg;
            leftRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
            leftRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
            leftLine.RenderTransform = leftRotateTransform;

            RotateTransform rightRotateTransform = new RotateTransform();
            rightRotateTransform.Angle = rightLineDeg;
            rightRotateTransform.CenterX = c.SmartGlasses_ScreenWidth / 2;
            rightRotateTransform.CenterY = c.SmartGlasses_ScreenHeight / 2;
            rightLine.RenderTransform = rightRotateTransform;

            leftEyeCanvas.Children.Add(leftLine);
            leftEyeCanvas.Children.Add(leftFixEllipse);
            rightEyeCanvas.Children.Add(rightLine);
            rightEyeCanvas.Children.Add(rightFixEllipse);

            StoreData.InitLeftLineAngle.Add(leftLineDeg);
            StoreData.InitRightLineAngle.Add(rightLineDeg);
        }
    }
}
