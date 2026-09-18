using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonResources.Utilities
{
    public class ScrollViewerEventHandler
    {
        private bool isTouchScrolling = false;
        private Point lastTouchPosition;
        private double verticalOffset;
        private ScrollViewer? activeScrollViewer;

        public void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta > 0)
            {
                scrollViewer.LineUp();
            }
            else
            {
                scrollViewer.LineDown();
            }
            e.Handled = true;
        }

        public void TouchDown(object? sender, TouchEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                activeScrollViewer = scrollViewer;
                isTouchScrolling = true;
                lastTouchPosition = e.GetTouchPoint(activeScrollViewer).Position;
                verticalOffset = activeScrollViewer.VerticalOffset;
                activeScrollViewer.CaptureTouch(e.TouchDevice);
            }
        }

        public void TouchMove(object? sender, TouchEventArgs e)
        {
            if (isTouchScrolling && activeScrollViewer != null)
            {
                // Calculate the delta of the touch movement
                Point currentTouchPosition = e.GetTouchPoint(activeScrollViewer).Position;
                double deltaY = currentTouchPosition.Y - lastTouchPosition.Y;

                // Scroll the ScrollViewer
                activeScrollViewer.ScrollToVerticalOffset(verticalOffset - deltaY);
            }
        }

        public void TouchUp(object? sender, TouchEventArgs e)
        {
            if (activeScrollViewer != null)
            {
                // Release the touch capture and reset scrolling state
                isTouchScrolling = false;
                activeScrollViewer.ReleaseTouchCapture(e.TouchDevice);
                activeScrollViewer = null;
            }
        }
    }
}
