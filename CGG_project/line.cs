using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace CGG_project
{
    class line
    {
        public line(MainWindow mainWindow)
        {
            window = mainWindow;

            window.Task2Canvas.Background = new LinearGradientBrush(Colors.Teal, Colors.SteelBlue, 10);
            window.Task2Canvas.Background.Opacity = .1;
            
            window.Task2Canvas.Initialized+=WindowOnInitialized;
            window.SizeChanged += WindowOnInitialized;
            window.Task2Canvas.MouseWheel += Task2CanvasOnMouseWheel;
            window.Task2Canvas.MouseWheel += WindowOnInitialized;
            window.Task2Canvas.MouseMove+=Task2CanvasOnMouseMove;
            window.Task2Canvas.MouseDown += Task2CanvasOnMouseDown;
            window.Task2Canvas.MouseLeave += Task2CanvasOnMouseLeave;
            window.Task2Canvas.MouseUp += Task2CanvasOnMouseUp;


            window.Recount.Click+=RecountOnClick;
        }

        private void RecountOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var a = double.Parse(window.A_Source2.Text);
                var b = double.Parse(window.B_Source2.Text);

                Func<double, double> y_x = x => a*x + b;
                Func<double, double> x_y = y => (y - b)/a;

                if (a > 0)
                {
                    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Какой-то из параметров не прошел валидацию.");
            }
        }

        public void Task2CanvasOnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            pixelsize += mouseWheelEventArgs.Delta < 0 ? 1 : -1;
            if (pixelsize < 1)
                pixelsize = 1;
            if (pixelsize > 40)
                pixelsize = 40;

        }

        public void Task2CanvasOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MousePressed = false;
        }

        private void WindowOnInitialized(object sender, EventArgs eventArgs)
        {
            REdRAW();
        }


        private System.Windows.Point ShiftReal(System.Windows.Point p)
        {
            return new System.Windows.Point(p.X + (int)shiftX, p.Y + (int)shiftY);
        }

        private Point Shift(Point p)
        {
            return new Point(p.X + (int)shiftX, p.Y + (int)shiftY);
        }

        private Point UnShift(Point p)
        {
            return new Point(p.X - (int) shiftX, p.Y - (int) shiftY);
        }

        private void REdRAW()
        {
            window.Task2Canvas.Children.Clear();

            var lineGenerator = new lines();

            var pixX = Enumerable.Range(-(int)shiftX, MaxX(window.Task2Canvas)).ToList();

            var xLines = pixX.Select(p => pixelsize*p).Select(p => lineGenerator.VerticalLine(p, window.Task2Canvas)).ToList();

            var pixY = Enumerable.Range((int)shiftY, MaxY(window.Task2Canvas)).ToList();

            var yLines = pixY.Select(p => pixelsize*p).Select(p => lineGenerator.HorizontalLine(p, window.Task2Canvas)).ToList();

            foreach (var yLine in yLines)
            {
//                window.Task2Canvas.Children.Add(Styles.ApplyPixelDilimeterLineStyle(yLine, 1 - 1/(pixelsize + 1)));
            }
            foreach (var xLine in xLines)
            {
//                window.Task2Canvas.Children.Add(Styles.ApplyPixelDilimeterLineStyle(xLine, 1 - 1/(pixelsize + 1)));
            }

            var toDraw = Brezenhem.GetPoints(new Point(10, 5), new Point(120, 40)).Select(Shift).Where(p => IsInCanvas(p, window.Task2Canvas)).ToList();
            var norm = toDraw.Select(p => new Point(p.X, MaxY(window.Task2Canvas) - p.Y)).ToList();
            norm.ForEach((p) => DrawPixel(window.Task2Canvas, p));
        }

        private bool IsInCanvas(Point p, Canvas target)
        {
            return (p.X > 0 && p.X < MaxX(target) && 0 < p.Y && p.Y < MaxY(target));
        }

        private int MaxX(Canvas target)
        {
            return (int)target.ActualWidth / pixelsize;
        }

        private int MaxY(Canvas target)
        {
            return (int)target.ActualHeight / pixelsize;
        }

        public void DrawPixel(Canvas target, Point where)
        {
            var rect = new Rectangle {Stroke = new SolidColorBrush(pixelColor), StrokeThickness = pixelsize};
            Canvas.SetLeft(rect,where.X*pixelsize);
            Canvas.SetTop(rect, where.Y*pixelsize);
            target.Children.Add(rect);
        }


        public void Task2CanvasOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var sens = 0.1;
            if (MousePressed)
            {
                MousePressed = mouseEventArgs.LeftButton.HasFlag(MouseButtonState.Pressed);
                var newPoint = mouseEventArgs.GetPosition(window.Task2Canvas);
                shiftX+=((newPoint - LastPoint)).X*sens;
                shiftY -= ((newPoint - LastPoint)).Y*sens;
                LastPoint = newPoint;
                REdRAW();
            }

            var p = mouseEventArgs.GetPosition(window.Task2Canvas);
            p = new System.Windows.Point(p.X, (window.Task2Canvas.ActualHeight) - (p.Y));
            var pixel = ShiftReal(p);
            window.Coords2.Content = String.Format("X: {0}   Y: {1}", (int) (pixel.X)/pixelsize, (int) (pixel.Y)/pixelsize );
        }

        public void Task2CanvasOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            MousePressed = false;
        }

        public void Task2CanvasOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MousePressed = true;
            
            LastPoint = mouseButtonEventArgs.MouseDevice.GetPosition(window.Task2Canvas);
        }


        private double shiftX = 0;
        private double shiftY = 0;
        private Color pixelColor = Colors.Green;
        private int pixelsize = 10;
        private readonly MainWindow window;
        private bool MousePressed;
        private System.Windows.Point LastPoint;
    }

    class Brezenhem
    {
        static public IEnumerable<Point> GetPoints(Point from, Point to)
        {
            var ans = new List<Point>();

            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            var x = from.X;
            var y = from.Y;

            var D = dy*2;
            var e = -dx;

            ans.Add(new Point(x,y));

            for (x++; x<to.X; x++)
            {
                e += D;
                if (e > 0)
                {
                    e -= 2 * dx;
                    y++;
                }
                ans.Add(new Point(x, y));
            }
            return ans;
        } 

        public static Dictionary<string, Func<int,int>> generators = new Dictionary<string, Func<int, int>>
            {
                {"AXLine", Value}
            };

        private static int Value(int i)
        {
            throw new NotImplementedException();
        }
    }
}
