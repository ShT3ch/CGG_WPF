using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CGG_project
{
    public class lines
    {
        public Line VerticalLine(double x, Canvas workField)
        {
            return new Line { X1 = x, Y1 = 0, X2 = x, Y2 = workField.ActualHeight };
        }

        public Line HorizontalLine(double y, Canvas workField)
        {
            return new Line { X1 = 0, Y1 = y, X2 = workField.ActualWidth, Y2 = y };
        }
    }
}