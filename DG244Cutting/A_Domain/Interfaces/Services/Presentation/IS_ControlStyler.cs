using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    public interface IS_ControlStyler
    {
        void StylePage(Grid page);
        void StylePageOOControls(Border IdBorder, Border PasswordBorder, PasswordBox PasswordInput, Button LoginButton, TextBlock LoginButtonText);
        void StyleMH_Reduce(Border MH_Border, Button button, Uri iconUri);
        void StyleWindow(Border background1, Border? background2, Border titleBorder, Image logoImage, TextBlock titleContent, TextBlock? mainContent);
        void StyleWindowPad(Border Background_1, Border TitleBorder, Image LogoImage, TextBlock TitleContent, TextBlock? MainContent);
        void StyleDialogWindow(Border background1, Border? background2, Border titleBorder, Image logoImage, TextBlock titleContent, TextBlock? mainContent);
        void StyleApplicationTitle(TextBlock TitleContent);
        void StyleHorizontalMenuGrid(Grid grid, ColumnDefinition column1, ColumnDefinition column2, Border border, double windowWidth);
        void StyleScrollViewer(ScrollViewer scrollViewer, TextBlock? titleText = null, Border? headerBorder = null,
                TextBlock? headerText01 = null, TextBlock? headerText02 = null, TextBlock? headerText03 = null, TextBlock? headerText04 = null,
                TextBlock? headerText05 = null, TextBlock? headerText06 = null, TextBlock? headerText07 = null, TextBlock? headerText08 = null,
                TextBlock? headerText09 = null, TextBlock? headerText10 = null, TextBlock? headerText11 = null);
        void StyleListView(ListView listView);
        void StyleDataGrid(DataGrid dataGrid, SolidColorBrush? rowForeground = null);
        void StyleAppInfoDoc(FlowDocument AppInfoDoc);

        #region Border
        void StyleBorder(Border border);
        void StyleBorderHeader(Border border);
        #endregion

        #region TextBlock
        void StyleTextBlockPageTitle(TextBlock textBlock, double? width = null);
        void StyleTextBlockTitle(TextBlock textBlock, double? width = null);
        void StyleTextBlockData(TextBlock textBlock, double? width = null);
        void StyleTextBlockInput(TextBlock textBlock, double? width = null);
        void StyleTextBlockhHyperlink(TextBlock textBlock);
        void StyleTextBlockTitleList(TextBlock textBlock);
        void StyleTextBlockHeader(TextBlock textBlock);
        void StyleDateTime(TextBlock textBlock);
        void StyleTextBlockUser(TextBlock textBlock);
        void ApplyStylesToTextBlocks(DependencyObject parent);
        #endregion

        #region TextBox
        void StyleTextBoxData(TextBox textBox, double? width = null);
        void StyleTextBoxInput(TextBox textBox, double? width = null);
        #endregion

        #region Buttons
        void StyleAppMessageButton(Button button, Image icon1, Image icon2);
        void StyleAppLanguageButton(Button button, Image icon);
        void StyleAppUserButton(Button button, TextBlock textBlock);
        void StyleAppInfoButton(Button button, TextBlock textBlock);
        void StyleAppCloseButton(Button button, Image icon1, Image icon2);
        void StyleHorizontalMenuButton(Button button, Image icon, TextBlock textBlock, Uri iconUri);
        void StyleVerticalMenuButton(Button button, Image icon, TextBlock textBlock, Uri iconUri);
        void StyleButton(Button button, Uri iconUri);
        void StyleBasicButton(Button button, double? width = null);
        void StyleLanguageButton(Button button, Image flag, RadioButton radioButton, double? width = null);
        void StylePlusMinusButton(Button button, TextBlock textBlock);
        void StyleIsChecked(Button button, Image boxChecked, Image boxEmpty);
        #endregion

        #region Images
        void StyleWarnings(Image imageChecked, Image imageWarning, TextBlock textBlock, string warningColor);
        void StyleImageFlag(Image LanguageImage, Uri iconUri);
        #endregion

        #region ComboBox
        void StyleComboBox(ComboBox comboBox, double width);
        #endregion

        #region TabControl / TabItem
        void StyleTabControl(TabControl tabControl);
        void StyleTabItemTitle(TabItem tabItem, double? width = null);
        void StyleTabItem(TabItem tabItem, TextBlock tabHeader, double width);
        #endregion
    }
}