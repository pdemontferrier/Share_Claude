using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using Shared.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Service de stylisation centralisée des contrôles visuels WPF.
    /// <para>
    /// Cette classe encapsule les appels au module <c>ControlStyler</c> du projet Shared
    /// pour harmoniser l’apparence graphique de l’application BatchStockRelease.
    /// </para>
    /// </summary>
    public class SR_ControlStyler : IS_ControlStyler
    {

        public void StylePage(Grid page) => ControlStyler.StylePage(page);
        public void StylePageOOControls(Border IdBorder, Border PasswordBorder, PasswordBox PasswordInput, Button LoginButton, TextBlock LoginButtonText)
                                => ControlStyler.StylePageOOControls(IdBorder, PasswordBorder, PasswordInput, LoginButton, LoginButtonText);
        public void StyleMH_Reduce(Border MH_Border, Button button, Uri iconUri) => ControlStyler.StyleMH_Reduce(MH_Border, button, iconUri);
        public void StyleWindow(Border background1, Border? background2, Border titleBorder, Image logoImage, TextBlock titleContent, TextBlock? mainContent)
                                => ControlStyler.StyleWindow(background1, background2, titleBorder, logoImage, titleContent, mainContent);
        public void StyleWindowPad(Border Background_1, Border TitleBorder, Image LogoImage, TextBlock TitleContent, TextBlock? MainContent)
                                => ControlStyler.StyleWindowPad(Background_1, TitleBorder, LogoImage, TitleContent, MainContent);
        public void StyleDialogWindow(Border background1, Border? background2, Border titleBorder, Image logoImage, TextBlock titleContent, TextBlock? mainContent)
                                => ControlStyler.StyleDialogWindow(background1, background2, titleBorder, logoImage, titleContent, mainContent);
        public void StyleHorizontalMenuGrid(Grid grid, ColumnDefinition column1, ColumnDefinition column2, Border border, double windowWidth)
                                => ControlStyler.StyleHorizontalMenuGrid(grid, column1, column2, border, windowWidth);
        public void StyleScrollViewer(ScrollViewer scrollViewer, TextBlock? titleText = null, Border? headerBorder = null,
                                TextBlock? headerText01 = null, TextBlock? headerText02 = null, TextBlock? headerText03 = null, TextBlock? headerText04 = null,
                                TextBlock? headerText05 = null, TextBlock? headerText06 = null, TextBlock? headerText07 = null, TextBlock? headerText08 = null,
                                TextBlock? headerText09 = null, TextBlock? headerText10 = null, TextBlock? headerText11 = null)
                                => ControlStyler.StyleScrollViewer(scrollViewer, titleText, headerBorder, headerText01, headerText02, headerText03, headerText04,
                                    headerText05, headerText06, headerText07, headerText08, headerText09, headerText10, headerText11);
        public void StyleListView(ListView listView) => ControlStyler.StyleListView(listView);
        public void StyleDataGrid(DataGrid dataGrid, SolidColorBrush? rowForeground = null) => ControlStyler.StyleDataGrid(dataGrid, rowForeground);
        public void StyleAppInfoDoc(FlowDocument AppInfoDoc) => ControlStyler.StyleAppInfoDoc(AppInfoDoc);


        #region Border
        public void StyleBorder(Border border) => ControlStyler.StyleBorder(border);
        public void StyleBorderHeader(Border border) => ControlStyler.StyleBorderHeader(border);
        #endregion


        #region TextBlock
        public void StyleApplicationTitle(TextBlock TitleContent)
                        => ControlStyler.StyleApplicationTitle(TitleContent);
        public void StyleTextBlockPageTitle(TextBlock textBlock, double? width = null)
                        => ControlStyler.StyleTextBlockPageTitle(textBlock, width);
        public void StyleTextBlockTitle(TextBlock textBlock, double? width = null)
                        => ControlStyler.StyleTextBlockTitle(textBlock, width);
        public void StyleTextBlockData(TextBlock textBlock, double? width = null) => ControlStyler.StyleTextBlockData(textBlock, width);
        public void StyleTextBlockInput(TextBlock textBlock, double? width = null) => ControlStyler.StyleTextBlockInput(textBlock, width);
        public void StyleTextBlockhHyperlink(TextBlock textBlock) => ControlStyler.StyleTextBlockhHyperlink(textBlock);
        public void StyleTextBlockTitleList(TextBlock textBlock) => ControlStyler.StyleTextBlockTitleList(textBlock);
        public void StyleTextBlockHeader(TextBlock textBlock) => ControlStyler.StyleTextBlockHeader(textBlock);
        public void StyleDateTime(TextBlock textBlock) => ControlStyler.StyleDateTime(textBlock);
        public void StyleTextBlockUser(TextBlock textBlock) => ControlStyler.StyleTextBlockUser(textBlock);
        public void ApplyStylesToTextBlocks(DependencyObject parent) => ApplyStylesRecursive(parent);
        #endregion


        #region TextBox
        public void StyleTextBoxData(TextBox textBox, double? width = null) => ControlStyler.StyleTextBoxData(textBox, width);
        public void StyleTextBoxInput(TextBox textBox, double? width = null) => ControlStyler.StyleTextBoxInput(textBox, width);
        #endregion


        #region Buttons
        public void StyleAppMessageButton(Button button, Image icon1, Image icon2) => ControlStyler.StyleAppMessageButton(button, icon1, icon2);
        public void StyleAppLanguageButton(Button button, Image icon) => ControlStyler.StyleAppLanguageButton(button, icon);
        public void StyleAppUserButton(Button button, TextBlock textBlock) => ControlStyler.StyleAppUserButton(button, textBlock);
        public void StyleAppInfoButton(Button button, TextBlock textBlock) => ControlStyler.StyleAppInfoButton(button, textBlock);
        public void StyleAppCloseButton(Button button, Image icon1, Image icon2) => ControlStyler.StyleAppCloseButton(button, icon1, icon2);
        public void StyleHorizontalMenuButton(Button button, Image icon, TextBlock textBlock, Uri iconUri)
                        => ControlStyler.StyleHorizontalMenuButton(button, icon, textBlock, iconUri);
        public void StyleVerticalMenuButton(Button button, Image icon, TextBlock textBlock, Uri iconUri)
                                => ControlStyler.StyleVerticalMenuButton(button, icon, textBlock, iconUri);
        public void StyleButton(Button button, Uri iconUri) => ControlStyler.StyleButton(button, iconUri);
        public void StyleBasicButton(Button button, double? width = null) => ControlStyler.StyleBasicButton(button, width);
        public void StyleLanguageButton(Button button, Image flag, RadioButton radioButton, double? width = null) => ControlStyler.StyleLanguageButton(button, flag, radioButton, width);
        public void StylePlusMinusButton(Button button, TextBlock textBlock) => ControlStyler.StylePlusMinusButton(button, textBlock);
        public void StyleIsChecked(Button button, Image boxChecked, Image boxEmpty) => ControlStyler.StyleIsChecked(button, boxChecked, boxEmpty);
        #endregion


        #region Images
        public void StyleWarnings(Image imageChecked, Image imageWarning, TextBlock textBlock, string warningColor)
                        => ControlStyler.StyleWarnings(imageChecked, imageWarning, textBlock, warningColor);
        public void StyleImageFlag(Image LanguageImage, Uri iconUri) => ControlStyler.StyleImageFlag(LanguageImage, iconUri);
        #endregion


        #region ComboBox
        public void StyleComboBox(ComboBox comboBox, double width) => ControlStyler.StyleComboBox(comboBox, width);
        #endregion


        #region TabControl / TabItem
        public void StyleTabControl(TabControl tabControl) => ControlStyler.StyleTabControl(tabControl);
        public void StyleTabItem(TabItem tabItem, TextBlock tabHeader, double width)
                                => ControlStyler.StyleTabItem(tabItem, tabHeader, width);
        public void StyleTabItemTitle(TabItem tabItem, double? width = null) => ControlStyler.StyleTabItemTitle(tabItem, width);
        #endregion


        #region TextBlock Recursive Styling

        /// <summary>
        /// Applique récursivement les styles aux contrôles enfants d’un conteneur WPF.
        /// </summary>
        /// <param name="parent">Élément racine à parcourir.</param>
        /// <remarks>
        /// Cette méthode recherche les contrôles de type <see cref="TextBlock"/> et <see cref="TextBox"/> 
        /// dont le nom respecte les conventions : 
        /// <list type="bullet">
        /// <item><description>Nom se terminant par <c>Title</c> → StyleTextBlockTitle</description></item>
        /// <item><description>Nom se terminant par <c>Data</c> → StyleTextBlockData</description></item>
        /// <item><description>Nom se terminant par <c>Input</c> → StyleTextBlockInput</description></item>
        /// <item><description>Nom commençant par <c>PageTitle</c> → StyleTextBlockPageTitle</description></item>
        /// </list>
        /// </remarks>
        public void ApplyStylesRecursive(DependencyObject parent)
        {
            if (parent == null) return;
            ApplyStylesRecursiveInternal(parent);
        }

        /// <summary>
        /// Parcourt récursivement la hiérarchie visuelle pour appliquer les styles aux contrôles.
        /// </summary>
        /// <param name="parent">Élément parent duquel explorer les enfants.</param>
        private void ApplyStylesRecursiveInternal(DependencyObject parent)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                switch (child)
                {
                    case TextBlock textBlock:
                        ApplyTextBlockStyle(textBlock);
                        break;

                    case TextBox textBox:
                        ApplyTextBoxStyle(textBox);
                        break;

                    case DependencyObject depObj:
                        ApplyStylesRecursiveInternal(depObj); // appel récursif
                        break;
                }
            }
        }

        /// <summary>
        /// Applique le style approprié à un <see cref="TextBlock"/> selon sa convention de nommage.
        /// </summary>
        /// <param name="textBlock">Contrôle TextBlock à styliser.</param>
        private static void ApplyTextBlockStyle(TextBlock textBlock)
        {
            var name = textBlock.Name;

            if (name.StartsWith("PageTitle", StringComparison.OrdinalIgnoreCase))
            {
                ControlStyler.StyleTextBlockPageTitle(textBlock);
            }
            else if (name.EndsWith("Title", StringComparison.OrdinalIgnoreCase))
            {
                ControlStyler.StyleTextBlockTitle(textBlock);
            }
            else if (name.EndsWith("Data", StringComparison.OrdinalIgnoreCase))
            {
                ControlStyler.StyleTextBlockData(textBlock);
            }
            else if (name.EndsWith("Input", StringComparison.OrdinalIgnoreCase))
            {
                ControlStyler.StyleTextBlockInput(textBlock);
            }
        }

        /// <summary>
        /// Applique le style approprié à un <see cref="TextBox"/> selon sa convention de nommage.
        /// </summary>
        /// <param name="textBox">Contrôle TextBox à styliser.</param>
        private static void ApplyTextBoxStyle(TextBox textBox)
        {
            var name = textBox.Name;

            if (name.EndsWith("Data", StringComparison.OrdinalIgnoreCase))
            {
                ControlStyler.StyleTextBoxData(textBox);
            }
            else if (name.EndsWith("Input", StringComparison.OrdinalIgnoreCase))
            {
                ControlStyler.StyleTextBoxInput(textBox);
            }
        }

        #endregion
    }
}