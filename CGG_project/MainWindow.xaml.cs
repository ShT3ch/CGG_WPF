using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Timers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CGG_project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _lineClass = new line(this);

            Task2.DataContext = _lineClass;

            GraphicMaster = new GraphicLine(0.5,10);

            A_Source.DataContext = GraphicMaster;
            B_Source.DataContext = GraphicMaster;
            C_Sourse.DataContext = GraphicMaster;
            D_Sourse.DataContext = GraphicMaster;

            this.ContentRendered+=OnInitialized;


            Task1Canvas.Background = new LinearGradientBrush(Colors.Teal, Colors.SteelBlue, 10);
            Task1Canvas.Background.Opacity = .1;
            Task1Canvas.SizeChanged += OnInitialized;
            Task1Canvas.SizeChanged+=Task1CanvasOnSizeChanged;
            
            GraphicMaster.PropertyChanged +=(sender, args) => reDraw();
            fieldController = new FieldController(Task1Canvas);
            fieldController.Rescalled += MouseHandlerOnShiftDone;

            mouseHandler = new MouseHandler(fieldController,Task1Canvas);
            mouseHandler.ShiftDone+=MouseHandlerOnShiftDone;
            Task1Canvas.MouseDown += mouseHandler.Task1CanvasOnMouseDown;
            Task1Canvas.MouseLeave += mouseHandler.Task1CanvasOnMouseLeave;

            Task1Canvas.MouseMove += mouseHandler.Task1CanvasOnMouseMove;
            Task1Canvas.MouseMove += ImageMotion;

            Task1Canvas.MouseUp += mouseHandler.Task1CanvasOnMouseUp;

            Task1Canvas.MouseWheel += mouseHandler.Task1CanvasOnMouseWheel;
            Task1Canvas.MouseWheel += ImageMotion;
            _lines = new lines();


        }

        public lines Lines
        {
            get { return _lines; }
        }

        private void RecountCoordsInStatus(Point pixel)
        {
//            DrawMouseCoords(Task1Canvas, pixel);
            Coords.Content = String.Format("X: {0}   Y: {1}        pX: {2}  pY:{3}", fieldController.XP2XD(pixel.X), fieldController.YP2YD(pixel.Y), pixel.X, pixel.Y);
        }

        private void ImageMotion(object sender, MouseEventArgs mouseEventArgs)
        {
            RecountCoordsInStatus(mouseEventArgs.GetPosition(Task1Canvas));
        }

        private void MouseHandlerOnShiftDone(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            reDraw();
        }

        private void Task1CanvasOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            fieldController.ShiftTo_InPixels(
                new Vector(
                    -(sizeChangedEventArgs.NewSize.Width - sizeChangedEventArgs.PreviousSize.Width)/2,
                    (sizeChangedEventArgs.NewSize.Height - sizeChangedEventArgs.PreviousSize.Height)/2
                    )
                );
            reDraw();
        }

        private void OnInitialized(object sender, EventArgs eventArgs)
        {
            reDraw();
        }

        private void reDraw()
        {
            if (this.Task1.Visibility.Equals(Visibility.Visible))
            {

                LinesToDraw.Clear();
                DrawLinesInMemory(Task1Canvas);
                DrawCoords(Task1Canvas);
                DrawAXLine(Task1Canvas);
            }
        }

        private void DrawLabel(string what, Canvas target, Point where)
        {
            var label = new Label {Content = what, Opacity = .5 , FontSize = 9, MaxWidth = StepOfCoordWeb};
            Canvas.SetLeft(label, where.X-StepOfCoordWeb/7);
            Canvas.SetTop(label, where.Y - StepOfCoordWeb / 7);
            target.Children.Add(label);
        }

        private Timer timer = new Timer(2000);

//        private void DrawMouseCoords(Canvas canvas, Point pixel)
//        {
//            canvas.Children.Remove(NamedLinesToDraw[]);
//            canvas.Children.Add(ApplyMouseStyleCoord(VerticalLine(pixel.X, canvas)));
//            canvas.Children.Add(ApplyMouseStyleCoord(HorizontalLine(pixel.Y, canvas)));
//        }

        private void DrawCoords(Canvas canvas)
        {
            var coordLines = new List<Line>();

            for (var i = StepOfCoordWeb; i < canvas.ActualWidth; i += StepOfCoordWeb)
            {
                coordLines.Add(Styles.ApplyMiniLineStyle(Lines.VerticalLine(i, canvas)));
                var coord = fieldController.XP2XD(i).ToString();
                DrawLabel(coord.Substring(0, Math.Min(coord.Length, 6)),canvas,new Point(i,0));
            }
            for (var i = StepOfCoordWeb; i < canvas.ActualHeight; i += StepOfCoordWeb)
            {
                coordLines.Add(Styles.ApplyMiniLineStyle(Lines.HorizontalLine(i, canvas)));
                var coord = fieldController.YP2YD(i).ToString();
                DrawLabel(coord.Substring(0, Math.Min(coord.Length, 6)), canvas, new Point(0, i));
            }
            var XPixelOfZero = fieldController.XD2XP(0);
            var YPixelOfZero = fieldController.YD2YP(0);

            if (0 < XPixelOfZero && XPixelOfZero < canvas.ActualWidth)
                coordLines.Add(Styles.ApplyStyleCoord(Lines.VerticalLine(XPixelOfZero, canvas)));

            if (0 < YPixelOfZero && YPixelOfZero < canvas.ActualHeight)
                coordLines.Add(Styles.ApplyStyleCoord(Lines.HorizontalLine(YPixelOfZero, canvas)));

            foreach (var line in coordLines)
            {
                canvas.Children.Add(line);
            }
        }

        public void DrawLinesInMemory(Canvas canvas)
        {
            Task1Canvas.Children.Clear();
            LinesToDraw.ForEach(line=>canvas.Children.Add(line));
        }

        private void DrawAXLine(Canvas workField)
        {
            var listY = new List<Point>();

            listY.AddRange(GraphicMaster.GetUrPoints((d)=>fieldController.XP2XD(d),workField.ActualWidth));

            DrawDecartPoints(listY,workField);
        }

        private bool IsPointInField(Point p, double x, double y)
        {
            return p.X >= 0 && p.X <= x && p.Y >= 0 && p.Y <= y;
        }

        private IEnumerable<Point> ClearAndCutTheLines(List<Point> points, Canvas workField)
        {
            var InZone = new bool[points.Count()];

            var lastPoint = points[0];

            var counter = 0;

            foreach (var point in points)
            {
                if (lastPoint.X - point.X < 2 && (0 < point.Y && point.Y < workField.ActualHeight))
                {
                    InZone[counter] = true;
                }
                lastPoint = point;
                counter++;
            }

            counter = 0;
            var pointsWithoutBlindSpots = new List<Point>();
            foreach (var point in points)
            {
                pointsWithoutBlindSpots.Add(point);

                if (counter > 0)
                {
                    if (!InZone[counter] && !InZone[counter - 1])
                    {
                        if ((pointsWithoutBlindSpots[counter - 1].Y > fieldController.MaxYP
                             &&
                             pointsWithoutBlindSpots[counter].Y < 0
                            ))
                        {
                            pointsWithoutBlindSpots[counter - 1] = new Point(pointsWithoutBlindSpots[counter - 1].X, fieldController.MaxYP);
                            pointsWithoutBlindSpots[counter] = new Point(pointsWithoutBlindSpots[counter].X, 0);
                        }
                        else if ((pointsWithoutBlindSpots[counter - 1].Y < 0
                                  &&
                                  pointsWithoutBlindSpots[counter].Y > fieldController.MaxYP
                                 ))
                        {
                            pointsWithoutBlindSpots[counter - 1] = new Point(pointsWithoutBlindSpots[counter - 1].X, 0);
                            pointsWithoutBlindSpots[counter] = new Point(pointsWithoutBlindSpots[counter].X, fieldController.MaxYP);
                        }
                    }
                }
                counter++;
            }

            counter = 0;
            var pointsWithoutInfinitySyndrom = new List<Point>();

            foreach (var point in pointsWithoutBlindSpots)
            {
                pointsWithoutInfinitySyndrom.Add(point);

                counter++;
            }

            var infPixel = (int)fieldController.XD2XP(GraphicMaster.InfinityPoint());
            if (infPixel > 0 && infPixel< counter-1)
            {
                pointsWithoutInfinitySyndrom[infPixel] = new Point(infPixel, fieldController.MaxYP);
            }
            if (infPixel + 1 > 0 && infPixel< counter-1)
            {
                pointsWithoutInfinitySyndrom[infPixel + 1] = new Point(infPixel + 1, 0);
            }

            counter = 0;
            var pointsWithoutCuts = new List<Point>();

            foreach (var point in pointsWithoutInfinitySyndrom)
            {
                pointsWithoutCuts.Add(point);

                if (counter > 0)
                {
                    if (InZone[counter - 1] && !InZone[counter])
                    {
                        pointsWithoutCuts[counter] = new Point(pointsWithoutCuts[counter - 1].X, pointsWithoutCuts[counter].Y > 0 ? fieldController.MaxYP : 0);
                     }
                    if (!InZone[counter - 1] && InZone[counter])
                    {
                        pointsWithoutCuts[counter-1] = new Point(pointsWithoutCuts[counter].X, pointsWithoutCuts[counter-1].Y > 0 ? fieldController.MaxYP : 0);
                    }
                }
                counter++;
            }

            return pointsWithoutCuts;
        }

        private void DrawDecartPoints(IEnumerable<Point> points, Canvas workField)
        {
            var xLimit = workField.ActualWidth;
            var yLimit = workField.ActualHeight;

            var pointsToDraw = points.Select(point => new Point(point.X,fieldController.YD2YP(point.Y))).ToList();

            var pointsToDraw0 = ClearAndCutTheLines(pointsToDraw,workField).Where(point => IsPointInField(point, xLimit, yLimit)).ToList();

            var polyLines = new List<Polyline>{new Polyline{Stroke = Brushes.LightSeaGreen, StrokeThickness = 2}};

            var lastPoint = new Point();
            if (pointsToDraw0.Count != 0)
                lastPoint = pointsToDraw0.First();
            foreach (var point in pointsToDraw0)
            {
                if (Math.Abs(lastPoint.X - point.X) > 2 || ((int)fieldController.XD2XP(GraphicMaster.InfinityPoint())) == (int)lastPoint.X)
                    {
                        polyLines.Add(new Polyline {Stroke = Brushes.LightSeaGreen, StrokeThickness = 2});
                    }
                    polyLines.Last().Points.Add(point);
                lastPoint = point;
            }

            polyLines.ForEach(line => workField.Children.Add(line));
        }

        public GraphicLine GraphicMaster;
        public MouseHandler mouseHandler;
        public readonly FieldController fieldController;

        private Dictionary<string ,Line> NamedLinesToDraw = new Dictionary<string, Line>();
        private readonly List<Line> LinesToDraw = new List<Line>();
        private int StepOfCoordWeb = 50;
        private readonly lines _lines;
        private readonly line _lineClass;
    }
}

namespace Utils
{
    public class BoolToVisibitityConverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TextToDouble: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse((string) value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
