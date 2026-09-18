using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Shared.Utilities
{
    public static class MouseClickService
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public static void SimulateMouseClickWithinWindow(FrameworkElement? element, int posX, int posY)
        {
            // Get the window containing the element
            var window = Window.GetWindow(element);
            if (window == null)
                return;

            // Get the position of the window
            var windowPos = window.PointToScreen(new Point(0, 0));

            // Calculate the click position inside the window
            var clickX = (int)windowPos.X + posX;
            var clickY = (int)windowPos.Y + posY;

            // Set the cursor position
            SetCursorPos(clickX, clickY);

            // Simulate mouse click
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)clickX, (uint)clickY, 0, UIntPtr.Zero);
        }
    }
}
