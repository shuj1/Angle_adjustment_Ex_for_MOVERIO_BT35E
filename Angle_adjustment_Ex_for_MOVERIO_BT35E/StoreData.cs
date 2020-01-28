using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Threading;

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E
{
    class StoreData
    {
        public static string trialDataFileName = "sample_trialData.txt";
        public static string sensorDataFileName = "sample_sensorData.txt";

        public static List<double> InitLeftLineAngle = new List<double>();
        public static List<double> InitRightLineAngle = new List<double>();
        public static List<double> RespLeftLineAngle = new List<double>();
        public static List<double> RespRightLineAngle = new List<double>();
        public static List<double> Resp_Time_List = new List<double>();

        public static List<double> Sensor_ValueX_List = new List<double>();
        public static List<double> Sensor_ValueY_List = new List<double>();
        public static List<double> Sensor_ValueZ_List = new List<double>();
        public static List<double> Sensor_Values_Record_Time_List = new List<double>();

        public static double current_ValueX;
        public static double current_ValueY;
        public static double current_ValueZ;

        public static double time_count = 0.0;

        private double timeSpan = 0.02; // 50Hzで記録する

        public StoreData()
        {
            SetupTimer();
        }

        private void SetupTimer()
        {
            TimeSpan period = TimeSpan.FromSeconds(timeSpan);

            ThreadPoolTimer timer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {
                if (SubPage.exFlag)
                {
                    time_count += timeSpan;
                    RecordSensorData();
                }
            }, period);
        }

        public static void RecordRespData(double leftRespAngle, double rightRespAngle)
        {
            RespLeftLineAngle.Add(leftRespAngle);
            RespRightLineAngle.Add(rightRespAngle);
            Resp_Time_List.Add(time_count);
        }

        public static async Task OutputTrialDataAsync()
        {
            // ファイルの作成 すでに存在する場合は置き換える
            StorageFolder documentsLibrary = KnownFolders.DocumentsLibrary;
            StorageFile file = await documentsLibrary.CreateFileAsync(trialDataFileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);

            // ファイルへの書き込み
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);

            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString("trial initLeftAngle initRightAngle respLeftAngle respRightAngle time\n");

                    if (Resp_Time_List != null)
                    {
                        for (int i = 0; i < Resp_Time_List.Count; i++)
                        {
                            dataWriter.WriteString($"{i} {Math.Round(InitLeftLineAngle[i],1)} {Math.Round(InitRightLineAngle[i], 1)} {Math.Round(RespLeftLineAngle[i], 1)} {Math.Round(RespRightLineAngle[i], 1)} {Math.Round(Resp_Time_List[i], 2)}\n");
                        }
                    }

                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
            }
            stream.Dispose(); // Or use the stream variable (see previous code snippet) with a using statement as well.
        }

        public static async Task OutputSensorDataAsync()
        {
            // ファイルの作成 すでに存在する場合は置き換える
            Windows.Storage.StorageFolder documentsLibrary = KnownFolders.DocumentsLibrary;
            Windows.Storage.StorageFile file = await documentsLibrary.CreateFileAsync(sensorDataFileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);

            // ファイルへの書き込み
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString("valueX valueY valueZ time\n");

                    if (Sensor_Values_Record_Time_List != null)
                    {
                        for(int i=0; i < Sensor_Values_Record_Time_List.Count; i++)
                        {
                            dataWriter.WriteString($"{Math.Round(Sensor_ValueX_List[i], 3)} {Math.Round(Sensor_ValueY_List[i], 3)} {Math.Round(Sensor_ValueZ_List[i], 3)} {Math.Round(Sensor_Values_Record_Time_List[i], 2)}\n");
                        }
                    }                    

                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
            }
            stream.Dispose(); // Or use the stream variable (see previous code snippet) with a using statement as well.          
        }

        private void RecordSensorData()
        {
            if (MainPage.use_sensor)
            {
                Sensor_ValueX_List.Add(current_ValueX);
                Sensor_ValueY_List.Add(current_ValueY);
                Sensor_ValueZ_List.Add(current_ValueZ);
                Sensor_Values_Record_Time_List.Add(time_count);
            }
        }
    }
}
