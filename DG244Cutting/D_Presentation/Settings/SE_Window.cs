using System.ComponentModel;
using System.Runtime.CompilerServices;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Centralise l'état partagé des fenêtres principale et dialogue de l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant Singleton de présentation injectable via <see cref="ISE_Window"/>,
    /// enregistré dans <c>E_Miscellaneous/CompositionRoot/SR_ConteneurDI.cs</c>. Consommé par les
    /// ViewModels, les services de notification et le service technique <c>SR_Window</c>.</para>
    /// <para>Objectif : Fournir un point d'accès unique et observable à l'état visuel partagé
    /// des fenêtres principale et dialogue, en concentrant la notification INPC sur le helper
    /// canonique <see cref="SetField{T}"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Exposer les dimensions minimales fixes de la fenêtre principale.</description></item>
    /// <item><description>Maintenir et notifier les dimensions, la position et la marge ajustée courantes de la fenêtre principale.</description></item>
    /// <item><description>Maintenir et notifier l'état mutable de la fenêtre dialogue (titre, contenu, ouverture).</description></item>
    /// <item><description>Fournir des opérations atomiques d'ouverture et de fermeture de la fenêtre dialogue.</description></item>
    /// <item><description>Fournir des opérations atomiques de mise à jour des dimensions et de la position de la fenêtre principale.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Aucune logique métier ni règle décisionnelle.</description></item>
    /// <item><description>Aucun calcul ; la marge ajustée est calculée par <c>SR_Window</c>.</description></item>
    /// <item><description>Aucun accès aux données.</description></item>
    /// <item><description>Aucune orchestration de flux applicatif.</description></item>
    /// <item><description>Aucune gestion visuelle de rendu (couleurs, polices, marges).</description></item>
    /// <item><description>CallChain non construite ni propagée.</description></item>
    /// </list>
    /// </remarks>
    public class SE_Window : ISE_Window
    {
        #region === Propriétés privées ===

        // --- Backing fields — état mutable fenêtre principale ---
        private int _mainWindowWidth;
        private int _mainWindowHeight;
        private double _mainWindowTop;
        private double _mainWindowLeft;
        private int _mainWindowMarginAdjusted;

        // --- Backing fields — état mutable fenêtre dialogue ---
        private string _dw_Title = string.Empty;
        private string _dw_Content = string.Empty;
        private bool _dw_IsOpen;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        // --- Fenêtre principale — dimensions minimales (immuables) ---

        /// <summary>
        /// Obtient la largeur minimale autorisée de la fenêtre principale, en pixels.
        /// </summary>
        public int MainWindowMinWidth => 1020;

        /// <summary>
        /// Obtient la hauteur minimale autorisée de la fenêtre principale, en pixels.
        /// </summary>
        public int MainWindowMinHeight => 680;

        // --- Fenêtre principale — état mutable (lecture seule) ---

        /// <summary>
        /// Obtient la largeur courante de la fenêtre principale, en pixels.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Mise à jour exclusivement par <c>SR_Window</c> via
        /// <see cref="UpdateWindowDimensions"/>, à chaque événement <c>SizeChanged</c>
        /// de la fenêtre principale.</para>
        /// </remarks>
        public int MainWindowWidth => _mainWindowWidth;

        /// <summary>
        /// Obtient la hauteur courante de la fenêtre principale, en pixels.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Mise à jour exclusivement par <c>SR_Window</c> via
        /// <see cref="UpdateWindowDimensions"/>, à chaque événement <c>SizeChanged</c>
        /// de la fenêtre principale.</para>
        /// </remarks>
        public int MainWindowHeight => _mainWindowHeight;

        /// <summary>
        /// Obtient la position de la fenêtre principale sur l'axe vertical (Y).
        /// Correspond au bord supérieur de la fenêtre à l'écran.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Mise à jour exclusivement par <c>SR_Window</c> via
        /// <see cref="UpdateWindowPosition"/>.</para>
        /// </remarks>
        public double MainWindowTop => _mainWindowTop;

        /// <summary>
        /// Obtient la position de la fenêtre principale sur l'axe horizontal (X).
        /// Correspond au bord gauche de la fenêtre à l'écran.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Mise à jour exclusivement par <c>SR_Window</c> via
        /// <see cref="UpdateWindowPosition"/>.</para>
        /// </remarks>
        public double MainWindowLeft => _mainWindowLeft;

        /// <summary>
        /// Obtient la marge verticale ajustée utilisée pour adapter dynamiquement
        /// les espacements en fonction de la hauteur courante de la fenêtre.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Mise à jour exclusivement par <c>SR_Window</c> via
        /// <see cref="UpdateWindowDimensions"/>.</para>
        /// <para>Objectif : Centraliser une valeur dérivée des dimensions courantes pour
        /// alimenter les bindings XAML des espacements et marges dynamiques.</para>
        /// </remarks>
        public int MainWindowMarginAdjusted => _mainWindowMarginAdjusted;

        // --- Fenêtre dialogue — état mutable (lecture seule) ---

        /// <summary>
        /// Obtient le titre de la fenêtre dialogue courante.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Écriture exclusive via <see cref="OpenDialog"/> et
        /// <see cref="CloseDialog"/> pour garantir la cohérence avec <see cref="DW_Content"/>
        /// et <see cref="DW_IsOpen"/>.</para>
        /// </remarks>
        public string DW_Title => _dw_Title;

        /// <summary>
        /// Obtient le contenu textuel affiché dans la fenêtre dialogue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Écriture exclusive via <see cref="OpenDialog"/> et
        /// <see cref="CloseDialog"/> pour garantir la cohérence avec <see cref="DW_Title"/>
        /// et <see cref="DW_IsOpen"/>.</para>
        /// </remarks>
        public string DW_Content => _dw_Content;

        /// <summary>
        /// Obtient l'état d'ouverture de la fenêtre dialogue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Écriture exclusive via <see cref="OpenDialog"/> et
        /// <see cref="CloseDialog"/>. Cette restriction empêche les ViewModels et les
        /// services de basculer cet indicateur sans passer par le couple atomique
        /// titre/contenu/ouverture.</para>
        /// <para>Objectif : Centraliser ce flag de manière cohérente et lisible
        /// par tout composant abonné aux notifications de propriété.</para>
        /// </remarks>
        public bool DW_IsOpen => _dw_IsOpen;

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        /// <summary>
        /// Déclenché lorsqu'une propriété observable du setting est modifiée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Émis par le helper <see cref="SetField{T}"/> à chaque
        /// modification effective d'une propriété mutable, conformément au pattern
        /// INotifyPropertyChanged.</para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SE_Window"/> avec les valeurs par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instanciée exclusivement par le conteneur DI au démarrage de l'application.</para>
        /// <para>Objectif : Garantir un état initial cohérent et reproductible via <see cref="Reset"/>.</para>
        /// </remarks>
        public SE_Window()
        {
            Reset();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Met à jour de manière atomique les dimensions et la marge ajustée de la fenêtre principale.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée exclusivement par <c>SR_Window</c> à chaque événement
        /// <c>SizeChanged</c> de la fenêtre principale.</para>
        /// <para>Objectif : Garantir la cohérence des trois propriétés liées en invoquant
        /// directement <see cref="SetField{T}"/> sur chaque champ support, ce qui filtre les
        /// valeurs identiques et notifie uniquement les propriétés réellement modifiées.</para>
        /// </remarks>
        /// <param name="width">Nouvelle largeur de la fenêtre principale, en pixels.</param>
        /// <param name="height">Nouvelle hauteur de la fenêtre principale, en pixels.</param>
        /// <param name="marginAdjusted">Nouvelle marge verticale ajustée, calculée par <c>SR_Window</c>.</param>
        public void UpdateWindowDimensions(int width, int height, int marginAdjusted)
        {
            SetField(ref _mainWindowWidth, width, nameof(MainWindowWidth));
            SetField(ref _mainWindowHeight, height, nameof(MainWindowHeight));
            SetField(ref _mainWindowMarginAdjusted, marginAdjusted, nameof(MainWindowMarginAdjusted));
        }

        /// <summary>
        /// Met à jour de manière atomique la position de la fenêtre principale.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée exclusivement par <c>SR_Window</c> à chaque événement
        /// de déplacement de la fenêtre principale.</para>
        /// <para>Objectif : Garantir la cohérence du couple <see cref="MainWindowTop"/> /
        /// <see cref="MainWindowLeft"/> en invoquant directement <see cref="SetField{T}"/> sur
        /// chaque champ support.</para>
        /// </remarks>
        /// <param name="top">Nouvelle position verticale (Y) de la fenêtre, en pixels.</param>
        /// <param name="left">Nouvelle position horizontale (X) de la fenêtre, en pixels.</param>
        public void UpdateWindowPosition(double top, double left)
        {
            SetField(ref _mainWindowTop, top, nameof(MainWindowTop));
            SetField(ref _mainWindowLeft, left, nameof(MainWindowLeft));
        }

        /// <summary>
        /// Ouvre la fenêtre dialogue en définissant son titre et son contenu de manière atomique.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par les services de notification avant d'afficher la fenêtre dialogue.</para>
        /// <para>Objectif : Garantir la cohérence de l'état dialogue en mettant à jour titre, contenu
        /// et indicateur d'ouverture par invocation directe de <see cref="SetField{T}"/> sur chaque champ
        /// support ; chaque champ ne notifie qu'en cas de changement effectif de valeur.</para>
        /// </remarks>
        /// <param name="title">Titre à afficher dans la fenêtre dialogue.</param>
        /// <param name="content">Contenu textuel à afficher dans la fenêtre dialogue.</param>
        public void OpenDialog(string title, string content)
        {
            SetField(ref _dw_Title, title ?? string.Empty, nameof(DW_Title));
            SetField(ref _dw_Content, content ?? string.Empty, nameof(DW_Content));
            SetField(ref _dw_IsOpen, true, nameof(DW_IsOpen));
        }

        /// <summary>
        /// Ferme la fenêtre dialogue et réinitialise son titre et son contenu.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par les services de notification ou les ViewModels
        /// après la fermeture de la fenêtre dialogue.</para>
        /// <para>Objectif : Remettre l'état dialogue à une valeur neutre et cohérente
        /// par invocation directe de <see cref="SetField{T}"/> sur chaque champ support.</para>
        /// </remarks>
        public void CloseDialog()
        {
            SetField(ref _dw_Title, string.Empty, nameof(DW_Title));
            SetField(ref _dw_Content, string.Empty, nameof(DW_Content));
            SetField(ref _dw_IsOpen, false, nameof(DW_IsOpen));
        }

        /// <summary>
        /// Réinitialise l'intégralité de l'état mutable aux valeurs par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée à l'initialisation depuis le constructeur,
        /// et disponible pour un redémarrage applicatif sans recréation du Singleton.</para>
        /// <para>Objectif : Garantir un état initial cohérent et reproductible par
        /// invocation directe de <see cref="SetField{T}"/> sur chaque champ support ; seuls
        /// les champs dont la valeur change effectivement émettent une notification INPC.</para>
        /// </remarks>
        public void Reset()
        {
            SetField(ref _mainWindowWidth, 0, nameof(MainWindowWidth));
            SetField(ref _mainWindowHeight, 0, nameof(MainWindowHeight));
            SetField(ref _mainWindowTop, 0d, nameof(MainWindowTop));
            SetField(ref _mainWindowLeft, 0d, nameof(MainWindowLeft));
            SetField(ref _mainWindowMarginAdjusted, 0, nameof(MainWindowMarginAdjusted));
            SetField(ref _dw_Title, string.Empty, nameof(DW_Title));
            SetField(ref _dw_Content, string.Empty, nameof(DW_Content));
            SetField(ref _dw_IsOpen, false, nameof(DW_IsOpen));
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Met à jour un champ support et déclenche la notification INPC si la valeur a changé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Helper canonique invoqué par les opérations atomiques publiques
        /// (<see cref="UpdateWindowDimensions"/>, <see cref="UpdateWindowPosition"/>,
        /// <see cref="OpenDialog"/>, <see cref="CloseDialog"/>, <see cref="Reset"/>).</para>
        /// <para>Objectif : Centraliser le triptyque comparaison de valeur, écriture du
        /// champ support et émission de <see cref="PropertyChanged"/>, conformément à la signature
        /// canonique invariante des Settings INPC formalisée en 023 §4.14.7.</para>
        /// </remarks>
        /// <typeparam name="T">Type de la valeur stockée.</typeparam>
        /// <param name="field">Référence au champ support privé.</param>
        /// <param name="value">Nouvelle valeur à appliquer.</param>
        /// <param name="propertyName">Nom de la propriété notifiée. Résolu automatiquement
        /// par <see cref="CallerMemberNameAttribute"/> lorsqu'appelé depuis un setter public ;
        /// passé explicitement via <c>nameof(...)</c> dans les opérations atomiques.</param>
        /// <returns><see langword="true"/> si la valeur a effectivement changé et qu'une
        /// notification a été émise ; <see langword="false"/> sinon.</returns>
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        #endregion
    }
}