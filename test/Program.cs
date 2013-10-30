using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var a = -2;
            var b = 3;
            var maxP = new Point(50, 100);

            var filters = new Stack<Func<Point, Point>>();

            if (a < 0)
            {
                a = -a;
                b = -b;
                filters.Push(x=>new Point(x.X, -x.Y));
            }

            var y_x = new Func<double, double>(x => a * x + b);
            var x_y = new Func<double, double>(y => (y - b) / a);

            var x0 = 0.0;
            var y0 = y_x(x0);

            var xm = maxP.X;
            var ym = y_x(xm);

            if ((ym - y0) > (xm-x0))
            {
                double t;

                t = ym;
                ym = xm;
                xm = t;

                t = y0;
                y0 = x0;
                x0 = t;
                
                filters.Push(p=> new Point(p.Y,p.X));
            }

            var pts = new List<Point> {new Point(x0, y0), new Point(xm, ym)};

            while (filters.Count != 0)
            {
                var filter = filters.Pop();
                pts = pts.Select(filter).ToList();
            }
            
        }
    }
}
