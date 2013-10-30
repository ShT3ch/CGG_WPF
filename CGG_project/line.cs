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
using Rectangle = System.Windows.Shapes.Rectangle;

namespace CGG_project
{
    internal class line
    {
        public line(MainWindow mainWindow)
        {
            window = mainWindow;

            window.Task2Canvas.Background = new LinearGradientBrush(Colors.Teal, Colors.SteelBlue, 10);
            window.Task2Canvas.Background.Opacity = .1;

            window.Task2Canvas.Initialized += WindowOnInitialized;
            window.SizeChanged += WindowOnInitialized;
            window.Task2Canvas.MouseWheel += Task2CanvasOnMouseWheel;
            window.Task2Canvas.MouseWheel += WindowOnInitialized;
            window.Task2Canvas.MouseMove += Task2CanvasOnMouseMove;
            window.Task2Canvas.MouseDown += Task2CanvasOnMouseDown;
            window.Task2Canvas.MouseLeave += Task2CanvasOnMouseLeave;
            window.Task2Canvas.MouseUp += Task2CanvasOnMouseUp;

            a_Field = 1;
            b_Field = 0;

            window.Recount.Click += RecountOnClick;
        }

        private void RecountOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var a = double.Parse(window.A_Source2.Text);
                var b = double.Parse(window.B_Source2.Text);

                REdRAW();
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
            return new System.Windows.Point(p.X + (int) shiftX, p.Y + (int) shiftY);
        }

        private Point Shift(Point p)
        {
            return new Point(p.X + (int) shiftX, p.Y + (int) shiftY);
        }

        private Point UnShift(Point p)
        {
            return new Point(p.X - (int) shiftX, p.Y - (int) shiftY);
        }

        private void REdRAW()
        {
            window.Task2Canvas.Children.Clear();

            var lineGenerator = new lines();

            var pixX = Enumerable.Range(-(int) shiftX, MaxX(window.Task2Canvas)).ToList();

            var xLines = pixX.Select(p => pixelsize*p).Select(p => lineGenerator.VerticalLine(p, window.Task2Canvas)).ToList();

            var pixY = Enumerable.Range((int) shiftY, MaxY(window.Task2Canvas)).ToList();

            var yLines = pixY.Select(p => pixelsize*p).Select(p => lineGenerator.HorizontalLine(p, window.Task2Canvas)).ToList();

            foreach (var yLine in yLines)
            {
                //                window.Task2Canvas.Children.Add(Styles.ApplyPixelDilimeterLineStyle(yLine, 1 - 1/(pixelsize + 1)));
            }
            foreach (var xLine in xLines)
            {
                //                window.Task2Canvas.Children.Add(Styles.ApplyPixelDilimeterLineStyle(xLine, 1 - 1/(pixelsize + 1)));
            }
//
            var bounds = Brezenhem.GetPointsBound(a_Field, b_Field);
            var points = Brezenhem.GetPoints(bounds.First(), bounds.Last());
            var filterReversed = Brezenhem.DeFilter(points);

            

            var elipse = BrezenhemElipse.GetElipse(c_Field, d_Field).Select(ShiftToCenter);

            var toDraw = filterReversed.Concat(elipse).Where(p => IsInCanvas(p, window.Task2Canvas)).ToList();
            //           
            var norm = toDraw.Select(p => new Point(p.X, MaxY(window.Task2Canvas) - p.Y)).ToList();
            norm.ForEach((p) => DrawPixel(window.Task2Canvas, p));
        }
        
        private Point ShiftToCenter(Point target)
        {
            var xShift = MaxX(window.Task2Canvas)/2;
            var yShift = MaxY(window.Task2Canvas)/2;
            return new Point(target.X + xShift, target.Y + yShift);
        } 

        private bool IsInCanvas(Point p, Canvas target)
        {
            return (p.X > 0 && p.X < MaxX(target) && 0 < p.Y && p.Y < MaxY(target));
        }

        private int MaxX(Canvas target)
        {
            return (int) target.ActualWidth/pixelsize;
        }

        private int MaxY(Canvas target)
        {
            return (int) target.ActualHeight/pixelsize;
        }

        public void DrawPixel(Canvas target, Point where)
        {
            var rect = new Rectangle {Stroke = new SolidColorBrush(pixelColor), StrokeThickness = pixelsize};
            Canvas.SetLeft(rect, where.X*pixelsize);
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
                shiftX += ((newPoint - LastPoint)).X*sens;
                shiftY -= ((newPoint - LastPoint)).Y*sens;
                LastPoint = newPoint;
                REdRAW();
            }

            var p = mouseEventArgs.GetPosition(window.Task2Canvas);
            p = new System.Windows.Point(p.X, (window.Task2Canvas.ActualHeight) - (p.Y));
            var pixel = ShiftReal(p);
            window.Coords2.Content = String.Format("X: {0}   Y: {1}", (int) (pixel.X)/pixelsize, (int) (pixel.Y)/pixelsize);
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


        public string A_FieldFromForm
        {
            get { return aFromForm; }
            set
            {
                aFromForm = value.Replace('.', ',');
                a_Field = double.Parse(aFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(a_Field.ToString()));
            }
        }

        public string B_FieldFromForm
        {
            get { return bFromForm; }
            set
            {
                bFromForm = value.Replace('.', ',');
                b_Field = double.Parse(bFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(b_Field.ToString()));
            }
        }

        public string C_FieldFromForm
        {
            get { return cFromForm; }
            set
            {
                cFromForm = value.Replace('.', ',');
                c_Field = double.Parse(cFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(c_Field.ToString()));
            }
        }

        public string D_FieldFromForm
        {
            get { return dFromForm; }
            set
            {
                dFromForm = value.Replace('.', ',');
                d_Field = double.Parse(dFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(d_Field.ToString()));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double a_Field { get; set; }
        public double b_Field { get; set; }
        public double c_Field { get; set; }
        public double d_Field { get; set; }

        private double shiftX = 0;
        private double shiftY = 0;
        private Color pixelColor = Colors.Green;
        private int pixelsize = 10;
        private readonly MainWindow window;
        private bool MousePressed;
        private System.Windows.Point LastPoint;
        private string aFromForm;
        private string bFromForm;
        private string cFromForm;
        private string dFromForm;
    }

    internal class BrezenhemElipse
    {
        public static IEnumerable<Point> GetElipse(double a, double b)
        {
            var fSegment = GetFirstSegment(a, b);

            var top = fSegment.Select(p => new Point(-p.X, p.Y)).Concat(fSegment);

            var all = top.Select(p => new Point(p.X, -p.Y)).Concat(top);
            return all;
        }

        private static IEnumerable<Point> GetFirstSegment(double a, double b)
        {
            var ans = new List<Point>();
            ans.Add(new Point(a, b));
            ans.Add(new Point(b, a));
            return ans;
        } 

        private static IEnumerable<Point> GetFirstFirstPart(double a, double b)
        {
            Func<double, double, double> nextD_S = (d1, x1) => d1 + 4*b*b*x1 + 6*b*b;
            Func<double, double, double, double> nextD_T = (d1, x1, y1) => nextD_S(d1, x1) - 4*a*a*y1;

            Func<double, double> nextX = x1 => x1 + 1;
            Func<double, double> nextY_S = y1 => y1;
            Func<double, double> nextY_T = y1 => y1 - 1;

            var x = 0;
            var y = b;
            var d = 2*b*b + a*a - 2*b*a*a;
            return new List<Point>();
        } 
    }

    internal class Brezenhem
    {
        public static IEnumerable<Point> GetPoints(Point from, Point to)
        {
            var ans = new List<Point>();

            var dx = Math.Abs(to.X - from.X);
            var dy = Math.Abs(to.Y - from.Y);
            var x = from.X;
            var y = from.Y;

            var D = dy*2;
            var e = -dx;

            ans.Add(new Point(x, y));

            for (x++; x < to.X; x++)
            {
                e += D;
                if (e > 0)
                {
                    e -= 2*dx;
                    y++;
                }
                ans.Add(new Point(x, y));
            }
            return ans;
        }

        public static IEnumerable<Point> GetPointsBound(double a, double b)
        {
            var y_x = new Func<double, double>(x => a*x + b);
            var x_y = new Func<double, double>(y => (y - b)/a);

            var p1 = new Point(-1000, y_x(-1000));
            var p2 = new Point(2000, y_x(2000));

            if (p1.Y > p2.Y)
            {
                var t = p1;
                p1 = new Point(p1.X, p2.Y);
                p2 = new Point(p2.X, t.Y);
                filters.Add("invertOX");
            }


            var dx = Math.Abs(p1.X - p2.X);
            var dy = Math.Abs(p1.Y - p2.Y);

            if (dy > dx)
            {
                p1 = new Point(p1.Y, p1.X);
                p2 = new Point(p2.Y, p2.X);
                filters.Add("invertX=Y");
            }

            return new List<Point> {p1, p2};
        }

        public static List<string> filters = new List<string>();

        public static Dictionary<string, Func<int, int>> generators = new Dictionary<string, Func<int, int>>
            {
                {"AXLine", Value}
            };

        private static int Value(int i)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Point> DeFilter(IEnumerable<Point> points)
        {
            var ans = points.ToList();
            if (filters.Any(str => str.Equals("invertX=Y")))
            {
                ans = points.Select(p => new Point(p.Y, p.X)).ToList();
            }
            if (filters.Any(str => str.Equals("invertOX")))
            {
                throw new NotImplementedException();
            }
            return ans;
        }
    }
}
