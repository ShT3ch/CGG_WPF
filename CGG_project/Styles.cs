using System.Windows.Media;
using System.Windows.Shapes;

namespace CGG_project
{
    static public class Styles
    {

        public static Line ApplyPixelDilimeterLineStyle(Line target, double opacity)
        {
            target.StrokeThickness = 0.5;
            target.Opacity = opacity;
            target.Stroke = Brushes.IndianRed;
            return target;
        }

        public static Line ApplyMiniLineStyle(Line target)
        {
            target.StrokeThickness = 0.5;
            target.Stroke = Brushes.IndianRed;
            return target;
        }

        public static Line ApplyStyleCoord(Line target)
        {
            target.StrokeThickness = 1;
            target.Stroke = Brushes.OrangeRed;
            return target;
        }

        private static Line ApplyMouseStyleCoord(Line target)
        {
            target.StrokeThickness = 0.9;
            target.Stroke = Brushes.LightGreen;
            return target;
        }
    }
}