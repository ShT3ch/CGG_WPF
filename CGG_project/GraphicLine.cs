using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CGG_project
{
    enum Fields
    {
        a_Field, b_Field, c_Field, d_Field
    }

    public class GraphicLine :  INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private string aFromForm;
        private string bFromForm;
        private string cFromForm;
        private string dFromForm;

        public string A_FieldFromForm
        {
            get
            {
                return aFromForm;
            }
            set
            {
                aFromForm = value.Replace('.',',');
                a_Field = double.Parse(aFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(a_Field.ToString()));
            }
        }
        public string B_FieldFromForm
        {
            get
            {
                return bFromForm;
            }
            set
            {
                bFromForm = value.Replace('.', ',');
                b_Field = double.Parse(bFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(b_Field.ToString()));
            }
        }
        public string C_FieldFromForm
        {
            get
            {
                return cFromForm;
            }
            set
            {
                cFromForm = value.Replace('.', ',');
                c_Field = double.Parse(cFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(c_Field.ToString()));
            }
        }
        public string D_FieldFromForm
        {
            get
            {
                return dFromForm;
            }
            set
            {
                dFromForm = value.Replace('.', ',');
                d_Field = double.Parse(dFromForm);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(d_Field.ToString()));
            }
        }

        public double a_Field { get; set; }

        public double b_Field { get; set; }

        public double c_Field { get; set; }
        
        public double d_Field { get; set; }

        private Func<double, double> y;
 
        public GraphicLine(double a, double b)
        {
            a_Field = 1;
            b_Field = 3.14159;
            c_Field = 4;
            d_Field = 1;

            y = x => 1 / (a_Field * x + b_Field) + Math.Sin(c_Field * x)*(x*x*d_Field) ;
            PropertyChanged+=OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            y = x => 1 / (a_Field * x + b_Field) + Math.Sin(c_Field * x) * (x * x * d_Field);
        }

        public IEnumerable<Point> GetUrPoints(Func<int, double> FromXP2XD, double count)
        {
            return Enumerable.Range(0, (int)count).Select(x => new Point(x, y(FromXP2XD(x))));
        } 

//        public bool IsInfinityBetween(Point pixelOne, Point pixelTwo, Func<double,double> XP2XD)
//        {
//            var y_one = y(XP2XD(pixelOne.X));
//            var y_two = y(XP2XD(pixelTwo.X));
//            var between = y(XP2XD(pixelOne.X) + (XP2XD(pixelOne.X) - XP2XD(pixelTwo.X))/2);
//            return (Math.Max(Math.Abs(y_one), Math.Abs(y_two))/Math.Abs(between)<0.1);
//        }

        public double InfinityPoint()
        {
            return -b_Field/a_Field;
        }

        public double MaxBetween(double leftX, double rightX, int iter)
        {
            var betweenX = (leftX - (leftX - rightX)/2);
            if (iter <= 0) return Math.Abs(y(betweenX));
            if (Math.Abs(y(leftX)) > Math.Abs(y(rightX)))
                return MaxBetween(leftX, betweenX, iter-1);
            else
                return MaxBetween(betweenX, rightX, iter-1);
        }

    }
}
