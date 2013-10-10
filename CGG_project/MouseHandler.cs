using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CGG_project
{
    public class MouseHandler
    {
        public event PropertyChangedEventHandler ShiftDone;

        private readonly FieldController _fieldController;
        private readonly Canvas _task1Canvas;
        private bool MousePressed;
        private Point LastPoint = new Point(0,0);

        public void Task1CanvasOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MousePressed = false;
        }

        public void Task1CanvasOnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            _fieldController.accu4Smoovy += mouseWheelEventArgs.Delta < 0 ? 10 : -10;
            _fieldController.timerToSmoovy.Start();
        }

        public void Task1CanvasOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (MousePressed)
            {
                MousePressed = mouseEventArgs.LeftButton.HasFlag(MouseButtonState.Pressed);
                var newPoint = mouseEventArgs.GetPosition(_task1Canvas);
                _fieldController.ShiftTo_InPixels(-(newPoint-LastPoint),true);
                LastPoint = newPoint;
                if (ShiftDone != null) ShiftDone(this, new PropertyChangedEventArgs("NewPosition"));
            }
        }

        public void Task1CanvasOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            MousePressed = false;
        }

        public void Task1CanvasOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MousePressed = true;
            _fieldController.accu4Smoovy = 0;
            LastPoint = mouseButtonEventArgs.MouseDevice.GetPosition(_task1Canvas);
        }

        public MouseHandler(FieldController fieldController, Canvas task1Canvas)
        {
            _fieldController = fieldController;
            _task1Canvas = task1Canvas;
        }
    }
}
