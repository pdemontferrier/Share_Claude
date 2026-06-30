using CommonResources.Settings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonResources.Utilities
{
    public static class ButtonMouseEventHandler
    {
        public static Button? CurrentActiveButton { get; set; }
        private static SolidColorBrush OriginalForeground = new SolidColorBrush(CR_CommonSettings.TextColor_1);

        public static void AttachMouseEvents(Button button, TextBlock textBlock)
        {
            button.MouseEnter += (sender, e) => Button_Highlight(sender, textBlock);
            button.MouseLeave += (sender, e) => Button_Unhighlight(sender, textBlock);
            button.TouchDown += (sender, e) => Button_TouchDown(sender, textBlock);
            button.TouchUp += (sender, e) => Button_Unhighlight(sender, textBlock);
        }

        private static void Button_Highlight(object? sender, TextBlock textBlock)
        {
            if (sender is Button button)
            {
                OriginalForeground = (SolidColorBrush)textBlock.Foreground;
                textBlock.Tag = OriginalForeground; // Store original color in Tag property
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
                textBlock.FontWeight = FontWeights.DemiBold;
                button.Opacity = CR_CommonSettings.Button_Opacity;
            }
        }

        private static void Button_Unhighlight(object? sender, TextBlock textBlock)
        {
            if (sender is Button button)
            {
                if (button != CurrentActiveButton)
                {
                    textBlock.Foreground = (SolidColorBrush)textBlock.Tag;
                }
                else
                {
                    textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
                }
                textBlock.FontWeight = FontWeights.Normal;
                button.Opacity = 1;
            }
        }

        private static async void Button_TouchDown(object? sender, TextBlock textBlock)
        {
            if (sender is Button button)
            {
                await Task.Delay(200);
                MouseClickService.SimulateMouseClickWithinWindow(sender as FrameworkElement, 50, 50);
            }
        }
    }
}
