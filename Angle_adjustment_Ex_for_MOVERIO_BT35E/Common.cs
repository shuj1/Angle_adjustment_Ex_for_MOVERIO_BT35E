using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

namespace Angle_adjustment_Ex_for_MOVERIO_BT35E
{
    public class Common
    {
        // 単位はpx
        public double SmartGlasses_ScreenWidth = 1280;
        public double SmartGlasses_ScreenHeight = 720;

        // 単位はdeg
        public double fixEllipseSize = 0.5;
        public double Distance_fixEllipse_and_Line = 1.5;
        public double lineSize = 4.0;
        public double lineThickness = 0.1;

        public double Tan(double deg)
        {
            return Math.Tan(deg * (Math.PI / 180));
        }

        public double PixelConverter(double deg)
        {
            /*
             * MOVERIO BT-35E
             * Resolution: 1280px * 720px
             * Virtual viewing distance: 2.5m
             * Virtual screen size: 40inches(88.39cm * 49.78cm)
             */
            return (2.5 * Tan(deg) * 100 * 1280) / 88.39;
        }
    }
}
