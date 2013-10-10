using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace CGG_project
{
    public class FieldController
    {
        public Vector ZeroShiftFromNull;
        private readonly Canvas WorkField;
        private double scale;
        public Timer timerToSmoovy = new Timer();

        public int accu4Smoovy = 0;
        private int _speedCoeff;

        public event PropertyChangedEventHandler Rescalled;

        public FieldController(Canvas workfield)
        {
            timerToSmoovy.Interval = 3;
            timerToSmoovy.Tick += TimerToSmoovyOnElapsed;
            WorkField = workfield;
            scale = 1;
            ZeroShiftFromNull = GetZero2NullVectorAtBegin();
        }

        private void TimerToSmoovyOnElapsed(object sender, EventArgs eventArgs)
        {
            timerToSmoovy.Stop();
            if (accu4Smoovy != 0)
            {
                if (accu4Smoovy > 100)
                    _speedCoeff += 2;
                var up = Math.Sign(accu4Smoovy);
                accu4Smoovy -= up*_speedCoeff;
                ScaleChange(up > 0);
                timerToSmoovy.Start();
            }
            else
            {
                _speedCoeff = 2;
            }
            if (Rescalled != null) Rescalled(this, new PropertyChangedEventArgs("Rescalled"));
        }

        private void ScaleChange(bool up)
        {
            var oldScale = scale;
            
            if (up) 
                scale *= 1.05;
            else
                scale *= 0.95;

            ZeroShiftFromNull+=new Vector((WorkField.ActualWidth)*(oldScale-scale)/2, -WorkField.ActualHeight*(oldScale - scale)/2);

        }

        private Vector GetZero2NullVectorAtBegin()
        {
            return (PixelToDecart_relNull(new Point((WorkField.ActualWidth / 12), (WorkField.ActualHeight * 11 / 12)))-new Point(0,0))*(-1);
        }

        private Point PixelToDecart_relNull(Point pixel)
        {
            return ZeroShiftFromNull+new Point(pixel.X*scale, -pixel.Y*scale);
        }

        public void ShiftTo_InPixels(Vector ShiftInPixels)
        {
            var decartVector = new Vector(ShiftInPixels.X*scale, ShiftInPixels.Y*scale);

            ZeroShiftFromNull += decartVector;
        }

        public double MaxYD
        {
            get { return ZeroShiftFromNull.Y; }
        }
        
        public double MinYD
        {
            get { return MaxYD - MaxYP*scale; }
        }

        public double MaxYP
        {
            get { return WorkField.ActualHeight; }
        }

        public double YD2YP(double YD)
        {
            return MaxYP - (YD - MinYD) / scale;
        }

        public double YP2YD(double YP)
        {
            return (MaxYP - YP)*scale + MinYD;
        }

        public double XP2XD(double XP)
        {
            return XP*scale + ZeroShiftFromNull.X;
        }

        public double XD2XP(double XD)
        {
            return (XD-ZeroShiftFromNull.X)/scale;
        }

        public double YDScaledInP(double YD)
        {
            return YD/scale;
        }
        public void ShiftTo_InPixels(Vector shiftInPixels, bool invert_Y)
        {
            if (invert_Y)
                ShiftTo_InPixels(new Vector(shiftInPixels.X,-shiftInPixels.Y));
            else
                ShiftTo_InPixels(shiftInPixels);
        }
    }
}
