using CommonResources.Settings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CommonResources.Utilities
{
    public static class ControlStyler
    {
        public static void StylePage(Grid page)
        {
            page.Margin = new Thickness(CR_CommonSettings.PageMarginRight, CR_CommonSettings.PageMarginTop, CR_CommonSettings.PageMarginRight, CR_CommonSettings.PageMarginBottom);
        }

        public static void StylePageOOControls(Border IdBorder, Border PasswordBorder, PasswordBox PasswordInput, Button LoginButton, TextBlock LoginButtonText)
        {
            IdBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            IdBorder.VerticalAlignment = VerticalAlignment.Top;
            IdBorder.Height = 30;
            IdBorder.Background = new SolidColorBrush(Colors.White);
            IdBorder.BorderThickness = new Thickness(1);
            IdBorder.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);

            PasswordBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            PasswordBorder.VerticalAlignment = VerticalAlignment.Top;
            PasswordBorder.Height = 30;
            PasswordBorder.Background = new SolidColorBrush(Colors.White);
            PasswordBorder.BorderThickness = new Thickness(1);
            PasswordBorder.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);

            PasswordInput.FontSize = CR_CommonSettings.TextSize_1;

            StyleBasicButton(LoginButton, 85);
            LoginButton.Height = 85;
            LoginButton.Margin = new Thickness(5, 0, 0, 0);

            LoginButtonText.FontFamily = CR_CommonSettings.ApplicationFont;
            LoginButtonText.FontSize = CR_CommonSettings.TextSize_1;
            LoginButtonText.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_2);
        }

        public static void StyleMH_Reduce(Border MH_Border, Button button, Uri iconUri)
        {
            MH_Border.Width = CR_CommonSettings.MHR_Width;
            MH_Border.Height = CR_CommonSettings.MHR_Height;
            MH_Border.Margin = new Thickness(0, CR_CommonSettings.MHR_MarginTop, CR_CommonSettings.MHR_Margin, 0);
            MH_Border.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            MH_Border.BorderThickness = new Thickness(CR_CommonSettings.BorderThickness);
            MH_Border.CornerRadius = new CornerRadius(CR_CommonSettings.BackgroundCornerRadius);

            StyleButton(button, iconUri);
        }

        public static void StyleWindow(Border Background_1, Border? Background_2, Border TitleBorder, Image LogoImage, TextBlock TitleContent, TextBlock? MainContent)
        {
            // Appliquer les paramètres globaux
            // Background_1
            Background_1.Background = new SolidColorBrush(CR_CommonSettings.BackgroundColor_1);
            Background_1.Opacity = CR_CommonSettings.BackgroundOpacity;
            Background_1.Margin = new Thickness(CR_CommonSettings.BackgroundMargin);
            Background_1.Padding = new Thickness(CR_CommonSettings.BackgroundPadding);

            // Background_2
            if (Background_2 != null)
            {
                Background_2.Background = new SolidColorBrush(CR_CommonSettings.BackgroundColor_2);
                Background_2.Opacity = CR_CommonSettings.BackgroundOpacity;
                Background_2.Margin = new Thickness(CR_CommonSettings.BackgroundMargin);
                Background_2.Padding = new Thickness(CR_CommonSettings.BackgroundPadding);
                Background_2.Width = CR_CommonSettings.BackgroundWidth_2;
                Background_2.VerticalAlignment = VerticalAlignment.Stretch;
                Background_2.HorizontalAlignment = HorizontalAlignment.Left;
            }

            // Title
            TitleBorder.Background = new SolidColorBrush(CR_CommonSettings.TitleColor);
            TitleBorder.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            TitleBorder.BorderThickness = new Thickness(CR_CommonSettings.BorderThickness);
            TitleBorder.Opacity = CR_CommonSettings.BackgroundOpacity;
            TitleBorder.Margin = new Thickness(0, CR_CommonSettings.TitleMarginTop, 0, 0);
            TitleBorder.Padding = new Thickness(CR_CommonSettings.BackgroundPadding);
            TitleBorder.Height = CR_CommonSettings.TitleHeight;
            TitleBorder.VerticalAlignment = VerticalAlignment.Top;
            TitleBorder.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Image
            LogoImage.Height = CR_CommonSettings.LogoHeight;
            LogoImage.Margin = new Thickness(CR_CommonSettings.LogoLeftMargin, 3, 0, 0);
            LogoImage.VerticalAlignment = VerticalAlignment.Top;
            LogoImage.HorizontalAlignment = HorizontalAlignment.Left;
            LogoImage.Stretch = Stretch.Uniform;
            RenderOptions.SetBitmapScalingMode(LogoImage, BitmapScalingMode.HighQuality);
            LogoImage.SnapsToDevicePixels = true;
            LogoImage.UseLayoutRounding = true;

            // AppTitle
            TitleContent.FontFamily = CR_CommonSettings.ApplicationFontBold;
            TitleContent.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
            TitleContent.FontSize = CR_CommonSettings.TextSize_1;
            TitleContent.Margin = new Thickness(CR_CommonSettings.BackgroundWidth_2, 0, 0, 0);
            TitleContent.HorizontalAlignment = HorizontalAlignment.Center;
            TitleContent.VerticalAlignment = VerticalAlignment.Center;

            // MainContent
            if (MainContent != null)
            {
                MainContent.FontFamily = CR_CommonSettings.ApplicationFontBold;
                MainContent.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
                MainContent.FontWeight = FontWeights.Bold;
                MainContent.FontSize = CR_CommonSettings.TextSize_1;
                MainContent.Margin = new Thickness(0, 50, 0, 0);
                MainContent.HorizontalAlignment = HorizontalAlignment.Center;
                MainContent.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        public static void StyleWindowPad(Border Background_1, Border TitleBorder, Image LogoImage, TextBlock? TitleContent, TextBlock? MainContent)
        {
            // Appliquer les paramètres globaux
            // Background_1
            Background_1.Background = new SolidColorBrush(CR_CommonSettings.BackgroundColor_1);
            Background_1.Opacity = CR_CommonSettings.BackgroundOpacity;
            Background_1.Margin = new Thickness(CR_CommonSettings.BackgroundMargin);
            Background_1.Padding = new Thickness(CR_CommonSettings.BackgroundPadding);

            // Title
            TitleBorder.Background = new SolidColorBrush(CR_CommonSettings.TitleColor);
            TitleBorder.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            TitleBorder.BorderThickness = new Thickness(CR_CommonSettings.BorderThickness);
            TitleBorder.Opacity = CR_CommonSettings.BackgroundOpacity;
            TitleBorder.Margin = new Thickness(0, CR_CommonSettings.TitleMarginTop, 0, 0);
            TitleBorder.Padding = new Thickness(CR_CommonSettings.BackgroundPadding);
            TitleBorder.Height = CR_CommonSettings.TitleHeight;
            TitleBorder.VerticalAlignment = VerticalAlignment.Top;
            TitleBorder.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Image
            LogoImage.Height = CR_CommonSettings.LogoHeight;
            LogoImage.Margin = new Thickness(CR_CommonSettings.LogoLeftMargin, 3, 0, 0);
            LogoImage.VerticalAlignment = VerticalAlignment.Top;
            LogoImage.HorizontalAlignment = HorizontalAlignment.Left;

            // AppTitle
            if (TitleContent != null)
            {
                StyleApplicationTitle(TitleContent);
            }

            // MainContent
            if (MainContent != null)
            {
                MainContent.FontFamily = CR_CommonSettings.ApplicationFontBold;
                MainContent.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
                MainContent.FontWeight = FontWeights.Bold;
                MainContent.FontSize = CR_CommonSettings.TextSize_1;
                MainContent.Margin = new Thickness(0, 50, 0, 0);
                MainContent.HorizontalAlignment = HorizontalAlignment.Center;
                MainContent.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        public static void StyleHorizontalMenuGrid(Grid grid, ColumnDefinition column1, ColumnDefinition column2, Border border, double windowWidth)
        {
            // Grid Columns
            column1.Width = new GridLength((int)(CR_CommonSettings.BackgroundWidth_2));
            column2.Width = new GridLength((int)(windowWidth - CR_CommonSettings.BackgroundWidth_2 - (CR_CommonSettings.MHR_Margin * 2)));

            // Border
            border.Height = CR_CommonSettings.MHR_Height;
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.Margin = new Thickness(0, CR_CommonSettings.MHR_MarginTop, CR_CommonSettings.MHR_Margin, 0);
            border.Background = new SolidColorBrush(CR_CommonSettings.BackgroundColor_1) { Opacity = 1 };
            border.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            border.BorderThickness = new Thickness(CR_CommonSettings.BorderThickness);
            border.CornerRadius = new CornerRadius(CR_CommonSettings.BackgroundCornerRadius);
        }

        public static void StyleScrollViewer(ScrollViewer scrollViewer, TextBlock? titleText = null, Border? headerBorder = null, 
                TextBlock? headerText01 = null, TextBlock? headerText02 = null, TextBlock? headerText03 = null, TextBlock? headerText04 = null, 
                TextBlock? headerText05 = null, TextBlock? headerText06 = null, TextBlock? headerText07 = null, TextBlock? headerText08 = null, 
                TextBlock? headerText09 = null, TextBlock? headerText10 = null, TextBlock? headerText11 = null)
        {
            if (titleText != null) StyleTextBlockTitleList(titleText);
            if (headerBorder != null) StyleBorderHeader(headerBorder);
            if (headerText01 != null) StyleTextBlockHeader(headerText01);
            if (headerText02 != null) StyleTextBlockHeader(headerText02);
            if (headerText03 != null) StyleTextBlockHeader(headerText03);
            if (headerText04 != null) StyleTextBlockHeader(headerText04);
            if (headerText05 != null) StyleTextBlockHeader(headerText05);
            if (headerText06 != null) StyleTextBlockHeader(headerText06);
            if (headerText07 != null) StyleTextBlockHeader(headerText07);
            if (headerText08 != null) StyleTextBlockHeader(headerText08);
            if (headerText09 != null) StyleTextBlockHeader(headerText09);
            if (headerText10 != null) StyleTextBlockHeader(headerText10);
            if (headerText11 != null) StyleTextBlockHeader(headerText11);


            var scrollViewerEventHandler = new ScrollViewerEventHandler();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.PanningMode = PanningMode.VerticalOnly;

            scrollViewer.PreviewMouseWheel += scrollViewerEventHandler.PreviewMouseWheel;
            scrollViewer.TouchDown += scrollViewerEventHandler.TouchDown;
            scrollViewer.TouchMove += scrollViewerEventHandler.TouchMove;
            scrollViewer.TouchUp += scrollViewerEventHandler.TouchUp;
        }

        public static void StyleListView(ListView listView)
        {
            Style columnHeaderStyle = new Style(typeof(GridViewColumnHeader));
            columnHeaderStyle.Setters.Add(new Setter(GridViewColumnHeader.BackgroundProperty, new SolidColorBrush(CR_CommonSettings.BackgroundColor_1)));
            columnHeaderStyle.Setters.Add(new Setter(GridViewColumnHeader.ForegroundProperty, new SolidColorBrush(CR_CommonSettings.TextColor_2)));
            columnHeaderStyle.Setters.Add(new Setter(GridViewColumnHeader.FontFamilyProperty, CR_CommonSettings.ApplicationFont));
            columnHeaderStyle.Setters.Add(new Setter(GridViewColumnHeader.FontSizeProperty, (double)CR_CommonSettings.TextSize_2));
            columnHeaderStyle.Setters.Add(new Setter(GridViewColumnHeader.MinHeightProperty, (double)CR_CommonSettings.TextSize_2 + 30));

            ControlTemplate columnHeaderTemplate = new ControlTemplate(typeof(GridViewColumnHeader));
            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, new SolidColorBrush(CR_CommonSettings.BackgroundColor_1));
            borderFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(CR_CommonSettings.BorderColor));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 0, CR_CommonSettings.BorderThickness));
            borderFactory.SetValue(Border.MarginProperty, new Thickness(0, 0, 0, 10));
            borderFactory.SetValue(Border.PaddingProperty, new Thickness(5, 0, 0, 0));
            FrameworkElementFactory contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            borderFactory.AppendChild(contentPresenterFactory);
            columnHeaderTemplate.VisualTree = borderFactory;
            columnHeaderStyle.Setters.Add(new Setter(GridViewColumnHeader.TemplateProperty, columnHeaderTemplate));

            Style listViewItemStyle = new Style(typeof(ListViewItem));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.FontFamilyProperty, CR_CommonSettings.ApplicationFont));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.FontSizeProperty, (double)CR_CommonSettings.TextSize_2));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.ForegroundProperty, new SolidColorBrush(CR_CommonSettings.TextColor_1)));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.IsHitTestVisibleProperty, true));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.FocusableProperty, true));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.BorderThicknessProperty, new Thickness(0)));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.PaddingProperty, new Thickness(0)));
            listViewItemStyle.Setters.Add(new Setter(ListViewItem.MinHeightProperty, (double)(CR_CommonSettings.TextSize_2 + 10)));

            listView.Resources.Add(typeof(GridViewColumnHeader), columnHeaderStyle);
            listView.Resources.Add(typeof(ListViewItem), listViewItemStyle);

            listView.Background = new SolidColorBrush(Colors.Transparent);
            listView.Margin = new Thickness(3);
            listView.Padding = new Thickness(10);
            listView.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            listView.BorderThickness = new Thickness(CR_CommonSettings.BorderThickness);
        }

        public static void StyleDataGrid(DataGrid dataGrid, SolidColorBrush? rowForeground = null)
        {
            // Style for DataGrid column headers
            Style columnHeaderStyle = new Style(typeof(DataGridColumnHeader));
            columnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, new SolidColorBrush(CR_CommonSettings.BackgroundColor_1)));
            columnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, new SolidColorBrush(CR_CommonSettings.TextColor_2)));
            columnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontFamilyProperty, CR_CommonSettings.ApplicationFont));
            columnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontSizeProperty, (double)CR_CommonSettings.TextSize_2));
            columnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.MinHeightProperty, (double)CR_CommonSettings.TextSize_2 + 30));

            // Control template for DataGrid column headers
            ControlTemplate columnHeaderTemplate = new ControlTemplate(typeof(DataGridColumnHeader));
            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, new SolidColorBrush(CR_CommonSettings.BackgroundColor_1));
            borderFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(CR_CommonSettings.BorderColor));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 0, CR_CommonSettings.BorderThickness));
            borderFactory.SetValue(Border.PaddingProperty, new Thickness(5, 0, 0, 0));
            FrameworkElementFactory contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            borderFactory.AppendChild(contentPresenterFactory);
            columnHeaderTemplate.VisualTree = borderFactory;
            columnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.TemplateProperty, columnHeaderTemplate));

            // Style for DataGrid cells
            Style dataGridCellStyle = new Style(typeof(DataGridCell));
            dataGridCellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.Transparent)));
            dataGridCellStyle.Setters.Add(new Setter(DataGridCell.FontFamilyProperty, CR_CommonSettings.ApplicationFont));
            dataGridCellStyle.Setters.Add(new Setter(DataGridCell.FontSizeProperty, (double)CR_CommonSettings.TextSize_2));
            dataGridCellStyle.Setters.Add(new Setter(DataGridCell.MinHeightProperty, (double)CR_CommonSettings.TextSize_2 + 15));
            if (rowForeground != null)
            {
                dataGridCellStyle.Setters.Add(new Setter(DataGridCell.ForegroundProperty, rowForeground));
            }

            // Control template for DataGrid cells to align content to bottom
            ControlTemplate cellTemplate = new ControlTemplate(typeof(DataGridCell));
            FrameworkElementFactory cellBorder = new FrameworkElementFactory(typeof(Border));
            cellBorder.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(DataGridCell.BackgroundProperty));
            cellBorder.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(DataGridCell.BorderBrushProperty));
            cellBorder.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(DataGridCell.BorderThicknessProperty));
            FrameworkElementFactory cellContentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            cellContentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            cellContentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            cellBorder.AppendChild(cellContentPresenter);
            cellTemplate.VisualTree = cellBorder;
            dataGridCellStyle.Setters.Add(new Setter(DataGridCell.TemplateProperty, cellTemplate));

            // Trigger for selected cells to remove borders
            Trigger selectedCellTrigger = new Trigger
            {
                Property = DataGridCell.IsSelectedProperty,
                Value = true
            };
            selectedCellTrigger.Setters.Add(new Setter(DataGridCell.BorderThicknessProperty, new Thickness(0)));
            dataGridCellStyle.Triggers.Add(selectedCellTrigger);

            // Style for DataGrid rows
            Style dataGridRowStyle = new Style(typeof(DataGridRow));
            dataGridRowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty, new SolidColorBrush(Colors.Transparent)));

            // Trigger for selected row
            Trigger selectedTrigger = new Trigger
            {
                Property = DataGridRow.IsSelectedProperty,
                Value = true
            };
            selectedTrigger.Setters.Add(new Setter(DataGridRow.BackgroundProperty, new SolidColorBrush(CR_CommonSettings.BackgroundColor_1)));
            selectedTrigger.Setters.Add(new Setter(DataGridRow.OpacityProperty, CR_CommonSettings.BackgroundOpacity));
            dataGridRowStyle.Triggers.Add(selectedTrigger);

            // Apply styles to dataGrid
            dataGrid.Resources.Add(typeof(DataGridColumnHeader), columnHeaderStyle);
            dataGrid.Resources.Add(typeof(DataGridCell), dataGridCellStyle);
            dataGrid.Resources.Add(typeof(DataGridRow), dataGridRowStyle);

            // dataGrid Styles
            dataGrid.Background = new SolidColorBrush(Colors.Transparent);
            dataGrid.HorizontalGridLinesBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            dataGrid.VerticalGridLinesBrush = new SolidColorBrush(Colors.Transparent);
            dataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            dataGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            dataGrid.Margin = new Thickness(3);
        }

        public static void StyleAppInfoDoc(FlowDocument AppInfoDoc)
        {
            AppInfoDoc.FontFamily = CR_CommonSettings.ApplicationFont;
            AppInfoDoc.FontSize = CR_CommonSettings.TextSize_2;
            AppInfoDoc.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
        }


        #region Border

        public static void StyleBorder(Border border)
        {
            border.Margin = new Thickness(3, 3, 3, 0);
            border.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            border.BorderThickness = new Thickness(1);
        }

        public static void StyleBorderHeader(Border border)
        {
            border.Background = new SolidColorBrush(CR_CommonSettings.BackgroundColor_1);
            border.Margin = new Thickness(3, 3, 3, 0);
            border.Padding = new Thickness(10);
            border.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            border.BorderThickness = new Thickness(1);
            border.Height = 40;
        }

        #endregion


        #region TextBlock

        public static void StyleApplicationTitle(TextBlock TitleContent)
        {
            TitleContent.FontFamily = CR_CommonSettings.ApplicationFontBold;
            TitleContent.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            TitleContent.FontSize = CR_CommonSettings.TextSize_1;
            TitleContent.Margin = new Thickness(0);
            TitleContent.HorizontalAlignment = HorizontalAlignment.Center;
            TitleContent.VerticalAlignment = VerticalAlignment.Center;
        }

        public static void StyleTextBlockPageTitle(TextBlock textBlock, string? text = null, double? width = null)
        {
            if (text != null) { textBlock.Text = text; }
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_1;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            textBlock.Margin = new Thickness(0, 0, 15, 0);
            if (text != null) { textBlock.Width = width ?? Double.NaN; };
        }

        public static void StyleTextBlockTitle(TextBlock textBlock, string? text = null, double? width = null)
        {
            if (text != null) { textBlock.Text = text; }
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_1;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            textBlock.Margin = new Thickness(0, 0, 15, 0);
            if (text != null) { textBlock.Width = width ?? Double.NaN; };
        }

        public static void StyleTextBlockData(TextBlock textBlock, double? width = null)
        {
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_1;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
            textBlock.Margin = new Thickness(0, 0, 30, 0);
            textBlock.Width = width ?? Double.NaN;
        }

        public static void StyleTextBlockInput(TextBlock textBlock, double? width = null)
        {
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_1;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            textBlock.Margin = new Thickness(0, 0, 30, 0);
            textBlock.Width = width ?? 200.0;
        }

        public static void StyleTextBlockhHyperlink(TextBlock textBlock)
        {
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_2;
            textBlock.Margin = new Thickness(0, 5, 0, 0);
        }

        public static void StyleTextBlockTitleList(TextBlock textBlock)
        {
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_1;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_2);
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.Margin = new Thickness(0, 0, 0, 10);
        }

        public static void StyleTextBlockHeader(TextBlock textBlock)
        {
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_2);
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_2;
        }

        public static void StyleDateTime(TextBlock textBlock)
        {
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_1;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_2);
            var textDecoration = new TextDecoration
            {
                Location = TextDecorationLocation.Underline,
                Pen = new Pen(new SolidColorBrush(CR_CommonSettings.TextColor_2), 1),
                PenThicknessUnit = TextDecorationUnit.FontRecommended
            };
            textBlock.TextDecorations = new TextDecorationCollection { textDecoration };
            textBlock.Margin = new Thickness(30, 0, 0, 0);
        }

        public static void StyleTextBlockUser(TextBlock textBlock)
        {
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_4;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
            textBlock.Margin = new Thickness(10, 16, 0, 0);
        }

        #endregion


        #region TextBox

        public static void StyleTextBoxData(TextBox textBox, double? width = null)
        {
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            textBox.FontFamily = CR_CommonSettings.ApplicationFont;
            textBox.FontSize = CR_CommonSettings.TextSize_1;
            textBox.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
            textBox.Margin = new Thickness(0, 0, 30, 0);
            textBox.Width = width ?? Double.NaN;
            textBox.Background = new SolidColorBrush(Colors.Transparent);
            textBox.AcceptsReturn = true;
            textBox.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
        }

        public static void StyleTextBoxInput(TextBox textBox, double? width = null)
        {
            StyleTextBoxData(textBox, null);
            textBox.MinWidth = 20;
            textBox.AcceptsReturn = false;
            textBox.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_2);
            textBox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        #endregion


        #region Buttons

        public static void StyleAppMessageButton(Button button, Image icon1, Image icon2)
        {
            button.Height = 30;
            button.Width = 35;
            button.Margin = new Thickness(15, 3, 0, 0);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.VerticalAlignment = VerticalAlignment.Center;
            button.BorderThickness = new Thickness(0);
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;

            icon1.VerticalAlignment = VerticalAlignment.Center;
            icon1.Margin = new Thickness(5);

            icon2.VerticalAlignment = VerticalAlignment.Center;
            icon2.Margin = new Thickness(5);
        }

        public static void StyleAppLanguageButton(Button button, Image icon)
        {
            button.Height = 30;
            button.Width = 40;
            button.Margin = new Thickness(5, 3, 0, 0);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.VerticalAlignment = VerticalAlignment.Center;
            button.BorderThickness = new Thickness(0);
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;

            icon.Height = 20;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.Margin = new Thickness(5);
        }

        public static void StyleAppUserButton(Button button, TextBlock textBlock)
        {
            button.Height = 30;
            button.Margin = new Thickness(5, 3, 0, 0);
            button.Padding = new Thickness(5, 2, 5, 0);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.VerticalAlignment = VerticalAlignment.Center;
            button.BorderThickness = new Thickness(0);
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;

            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_4;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
        }

        public static void StyleAppInfoButton(Button button, TextBlock textBlock)
        {
            button.Height = 30;
            button.Width = 30;
            button.Margin = new Thickness(10, 3, 0, 0);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.VerticalAlignment = VerticalAlignment.Center;
            button.BorderThickness = new Thickness(0);
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;

            // AppInfoSign
            textBlock.Text = "?";
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_3;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            textBlock.Background = new SolidColorBrush(Colors.Transparent);
        }

        public static void StyleAppCloseButton(Button button, Image icon)
        {
            button.Height = 30;
            button.Width = 40;
            button.Margin = new Thickness(15, 3, 0, 0);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.VerticalAlignment = VerticalAlignment.Center;
            button.BorderThickness = new Thickness(0);
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;

            icon.Height = 19;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.Margin = new Thickness(5);
        }

        public static void StyleHorizontalMenuButton(Button button, Image icon, TextBlock textBlock, Uri iconUri, string? text = null)
        {
            // Button
            button.Width = CR_CommonSettings.Button_MinWidth;
            button.Height = CR_CommonSettings.Button_MinHeight;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            button.Margin = new Thickness(CR_CommonSettings.MHR_Margin, 0, CR_CommonSettings.MHR_Margin, 0);

            // Icon
            icon.Source = new BitmapImage(iconUri);
            icon.Width = CR_CommonSettings.Icon_Size;
            icon.Height = CR_CommonSettings.Icon_Size;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Top;
            icon.Margin = new Thickness(0, 0, 0, 4);

            // TextBlock
            if (text != null) { textBlock.Text = text; }
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_3;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
        }

        public static void StyleVerticalMenuButton(Button button, Image icon, TextBlock textBlock, Uri iconUri)
        {
            // Button
            button.Height = CR_CommonSettings.MV_ButtonHeight;
            button.Width = CR_CommonSettings.MV_ButtonWidth;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.HorizontalContentAlignment = HorizontalAlignment.Left;
            button.VerticalContentAlignment = VerticalAlignment.Center;
            button.Margin = new Thickness(0, 0, 0, CR_CommonSettings.MV_ButtonMargin);
            button.Padding = new Thickness(0);

            // Icon
            icon.Source = new BitmapImage(iconUri);
            icon.Height = CR_CommonSettings.MV_ButtonIconSize;
            icon.Width = CR_CommonSettings.MV_ButtonIconSize;
            icon.Margin = new Thickness(CR_CommonSettings.MV_ButtonMargin, 0, 0, 0);

            // TextBlock
            textBlock.Width = CR_CommonSettings.MV_ButtonMinWidth;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(CR_CommonSettings.MV_ButtonMargin, 4, 0, 0);
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_1;
            switch (button.Name)
            {
                case "MV1":
                    textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_1);
                    break;
                case "MVU":
                    textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
                    break;
                default:
                    textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_2);
                    break;
            }

        }

        public static void StyleButton(Button button, Uri iconUri, string? text = null)
        {
            var icon = button.FindName($"{button.Name}_Icon") as Image;
            var textBlock = button.FindName($"{button.Name}_Text") as TextBlock;

            if (icon != null && textBlock != null)
            {
                StyleHorizontalMenuButton(button, icon, textBlock, iconUri, text);
                ButtonMouseEventHandler.AttachMouseEvents(button, textBlock);
            }
        }

        public static void StyleBasicButton(Button button, double? width = null)
        {
            button.Height = CR_CommonSettings.MV_ButtonHeight;
            button.MinWidth = 50;
            if (width != null) { button.Width = width.Value; }
            button.VerticalAlignment = VerticalAlignment.Top;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.VerticalContentAlignment = VerticalAlignment.Center;
            button.Margin = new Thickness(0, -7, 0, CR_CommonSettings.MV_ButtonMargin);
            button.Padding = new Thickness(0);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        public static void StyleLanguageButton(Button button, Image flag, RadioButton radioButton, double? width = null)
        {
            button.Height = CR_CommonSettings.MV_ButtonHeight;
            button.MinWidth = 50;
            if (width != null) { button.Width = width.Value; }
            button.VerticalAlignment = VerticalAlignment.Top;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.HorizontalContentAlignment = HorizontalAlignment.Left;
            button.VerticalContentAlignment = VerticalAlignment.Center;
            button.Margin = new Thickness(0, -7, 0, CR_CommonSettings.MV_ButtonMargin);
            button.Padding = new Thickness(3);
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.BorderBrush = new SolidColorBrush(Colors.Transparent);

            flag.Stretch = Stretch.Uniform;
            flag.VerticalAlignment = VerticalAlignment.Top;
            flag.Height = 25;

            radioButton.Margin = new Thickness(30, 5, 30, 5);
            radioButton.VerticalAlignment = VerticalAlignment.Top;
            radioButton.HorizontalAlignment = HorizontalAlignment.Center;
            radioButton.IsHitTestVisible = false;
            radioButton.Focusable = false;
        }

        public static void StylePlusMinusButton(Button button, TextBlock textBlock)
        {
            textBlock.Margin = new Thickness(0, -12, 0, 0);
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);

            button.Margin = new Thickness(30, -10, 0, 0);
            button.Width = CR_CommonSettings.MV_ButtonHeight;
            button.Height = CR_CommonSettings.MV_ButtonHeight;
            button.FontSize = CR_CommonSettings.MV_ButtonHeight - 2;
            button.VerticalAlignment = VerticalAlignment.Top;
            ButtonMouseEventHandler.AttachMouseEvents(button, textBlock);
        }

        public static void StyleIsChecked(Button button, Image boxChecked, Image boxEmpty)
        {
            button.VerticalAlignment = VerticalAlignment.Top;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.Width = CR_CommonSettings.Icon_Size;
            button.Height = CR_CommonSettings.Icon_Size;
            button.Margin = new Thickness(0, 0, 30, 0);
            button.BorderBrush = new SolidColorBrush(Colors.Transparent);
            boxChecked.Source = new BitmapImage(new Uri("/CommonResources;component/Icons/boxChecked.png", UriKind.Relative));
            boxEmpty.Source = new BitmapImage(new Uri("/CommonResources;component/Icons/boxEmpty.png", UriKind.Relative));
        }

        #endregion


        #region Images

        public static void StyleWarnings(Image imageChecked, Image imageWarning, TextBlock textBlock, string warningColor)
        {
            imageChecked.VerticalAlignment = VerticalAlignment.Top;
            imageChecked.HorizontalAlignment = HorizontalAlignment.Left;
            imageChecked.Width = CR_CommonSettings.Icon_Size;
            imageChecked.Height = CR_CommonSettings.Icon_Size;
            imageChecked.Source = new BitmapImage(CR_IconsSettings.IconCheckedCircle);
            imageWarning.VerticalAlignment = VerticalAlignment.Top;
            imageWarning.HorizontalAlignment = HorizontalAlignment.Left;
            imageWarning.Width = CR_CommonSettings.Icon_Size;
            imageWarning.Height = CR_CommonSettings.Icon_Size;
            textBlock.Margin = new Thickness(15, 5, 0, 0);
            textBlock.FontFamily = CR_CommonSettings.ApplicationFont;
            textBlock.FontSize = CR_CommonSettings.TextSize_2;
            if (warningColor == "Red")
            {
                imageWarning.Source = new BitmapImage(CR_IconsSettings.IconWarningTriangleRed);
                textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_Red);
            }
            if (warningColor == "Orange")
            {
                imageWarning.Source = new BitmapImage(CR_IconsSettings.IconWarningTriangleOrange);
                textBlock.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_Orange);
            }
        }

        public static void StyleImageFlag(Image LanguageImage, Uri iconUri)
        {
            LanguageImage.Source = new BitmapImage(iconUri);
            LanguageImage.Height = 20;
            LanguageImage.VerticalAlignment = VerticalAlignment.Top;
            LanguageImage.Margin = new Thickness(10, 11, 5, 0);
        }

        #endregion


        #region ComboBox

        public static void StyleComboBox(ComboBox comboBox, double width)
        {
            comboBox.VerticalAlignment = VerticalAlignment.Top;
            comboBox.HorizontalAlignment = HorizontalAlignment.Left;
            comboBox.FontFamily = CR_CommonSettings.ApplicationFontBold;
            comboBox.FontSize = CR_CommonSettings.TextSize_1;
            comboBox.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            comboBox.Width = width;
            comboBox.Margin = new Thickness(0, -4, 30, 0);
            comboBox.Padding = new Thickness(0, 4, 0, 0);
            comboBox.Opacity = 0.75;
        }

        #endregion


        #region TabControl / TabItem

        public static void StyleTabControl(TabControl tabControl)
        {
            tabControl.Background = System.Windows.Media.Brushes.Transparent;
            tabControl.Padding = new Thickness(0, 5, 0, 5);
            tabControl.BorderBrush = System.Windows.Media.Brushes.Transparent;
            tabControl.BorderThickness = new Thickness(0);
            tabControl.VerticalAlignment = VerticalAlignment.Top;
        }

        public static void StyleTabItem(TabItem tabItem, TextBlock tabHeader, string textBlockTitle, double width)
        {
            StyleTextBlockTitle(tabHeader, textBlockTitle, width);
            tabItem.Header = tabHeader;
            tabItem.Background = Brushes.Transparent;
            tabItem.BorderBrush = Brushes.Transparent;
        }

        public static void StyleTabItemTitle(TabItem tabItem, double? width = null)
        {
            tabItem.VerticalAlignment = VerticalAlignment.Top;
            tabItem.HorizontalAlignment = HorizontalAlignment.Left;
            tabItem.FontFamily = CR_CommonSettings.ApplicationFont;
            tabItem.FontSize = CR_CommonSettings.TextSize_1;
            tabItem.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_3);
            tabItem.Margin = new Thickness(0, 0, 15, 0);
            if (width != null) { tabItem.Width = width ?? Double.NaN; };
        }

        #endregion


    }
}