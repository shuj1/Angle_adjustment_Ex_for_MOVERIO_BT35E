using Windows.Devices.Sensors;

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E.Watchers
{
    internal class GyrometerWatcher : MoverioWatcher
    {
        public GyrometerWatcher() : base(Gyrometer.GetDeviceSelector())
        {
        }
    }
}