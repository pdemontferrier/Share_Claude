using System.ComponentModel;

namespace DG244Cutting.A_Domain.Interfaces.Settings.Presentation
{
    /// <summary>
    /// Contrat du setting transverse de présentation centralisant l'état partagé des fenêtres
    /// principale et dialogue de l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette interface est définie dans <c>A_Domain</c> afin d'être accessible
    /// aux UseCases, aux ViewModels et aux services techniques de présentation. Son implémentation
    /// concrète <c>SE_Window</c> réside dans <c>D_Presentation/Settings</c>.</para>
    /// <para>Objectif : Exposer en lecture l'état partagé des fenêtres principale et dialogue,
    /// et n'autoriser l'écriture que via des opérations atomiques publiques. Les propriétés mutables
    /// dont l'évolution doit rester cohérente (dimensions, position, état dialogue) sont exposées
    /// en lecture seule par le contrat ; leur écriture est canalisée par les méthodes
    /// <see cref="UpdateWindowDimensions"/>, <see cref="UpdateWindowPosition"/>,
    /// <see cref="OpenDialog"/> et <see cref="CloseDialog"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer les dimensions minimales fixes de la fenêtre principale.</description></item>
    ///   <item><description>Exposer en lecture les dimensions courantes, la position et la marge ajustée de la fenêtre principale, mises à jour via opérations atomiques.</description></item>
    ///   <item><description>Exposer les dimensions fixes de la fenêtre dialogue.</description></item>
    ///   <item><description>Exposer en lecture l'état mutable de la fenêtre dialogue (titre, contenu, ouverture), modifié via opérations atomiques.</description></item>
    ///   <item><description>Fournir des opérations atomiques d'ouverture et de fermeture de la fenêtre dialogue.</description></item>
    ///   <item><description>Fournir des opérations atomiques de mise à jour des dimensions et de la position de la fenêtre principale.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune logique métier ni règle décisionnelle.</description></item>
    ///   <item><description>Ne réalise aucun calcul ; la marge ajustée est calculée par <c>SR_Window</c>.</description></item>
    ///   <item><description>N'accède à aucune ressource externe ni à la base de données.</description></item>
    ///   <item><description>Ne manipule aucun contrôle WPF.</description></item>
    /// </list>
    /// </remarks>
    public interface ISE_Window : INotifyPropertyChanged
    {
        // --- Groupe 1 : Fenêtre principale — dimensions minimales (immuables) ---

        /// <summary>
        /// Largeur minimale autorisée de la fenêtre principale, en pixels.
        /// </summary>
        int MainWindowMinWidth { get; }

        /// <summary>
        /// Hauteur minimale autorisée de la fenêtre principale, en pixels.
        /// </summary>
        int MainWindowMinHeight { get; }

        // --- Groupe 2 : Fenêtre principale — état mutable (lecture seule via le contrat) ---

        /// <summary>
        /// Largeur courante de la fenêtre principale, en pixels.
        /// </summary>
        /// <remarks>
        /// Écriture réservée à <c>SR_Window</c> via <see cref="UpdateWindowDimensions"/>.
        /// </remarks>
        int MainWindowWidth { get; }

        /// <summary>
        /// Hauteur courante de la fenêtre principale, en pixels.
        /// </summary>
        /// <remarks>
        /// Écriture réservée à <c>SR_Window</c> via <see cref="UpdateWindowDimensions"/>.
        /// </remarks>
        int MainWindowHeight { get; }

        /// <summary>
        /// Position de la fenêtre principale sur l'axe vertical (Y), en pixels.
        /// Correspond au bord supérieur de la fenêtre à l'écran.
        /// </summary>
        /// <remarks>
        /// Écriture réservée à <c>SR_Window</c> via <see cref="UpdateWindowPosition"/>.
        /// </remarks>
        double MainWindowTop { get; }

        /// <summary>
        /// Position de la fenêtre principale sur l'axe horizontal (X), en pixels.
        /// Correspond au bord gauche de la fenêtre à l'écran.
        /// </summary>
        /// <remarks>
        /// Écriture réservée à <c>SR_Window</c> via <see cref="UpdateWindowPosition"/>.
        /// </remarks>
        double MainWindowLeft { get; }

        /// <summary>
        /// Marge verticale ajustée utilisée pour adapter dynamiquement les espacements
        /// en fonction de la hauteur courante de la fenêtre.
        /// </summary>
        /// <remarks>
        /// Calculée par <c>SR_Window</c> et affectée via <see cref="UpdateWindowDimensions"/>.
        /// </remarks>
        int MainWindowMarginAdjusted { get; }

        // --- Groupe 3 : Fenêtre dialogue — dimensions immuables ---

        /// <summary>
        /// Largeur de référence de la fenêtre dialogue, en pixels.
        /// </summary>
        int DW_Width { get; }

        /// <summary>
        /// Hauteur de référence de la fenêtre dialogue, en pixels.
        /// </summary>
        int DW_Height { get; }

        /// <summary>
        /// Délai (en secondes) avant l'ouverture automatique de la fenêtre modale d'attente.
        /// </summary>
        int DW_ShowDelay { get; }

        // --- Groupe 4 : Fenêtre dialogue — état mutable (lecture seule via le contrat) ---

        /// <summary>
        /// Titre de la fenêtre dialogue courante.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="OpenDialog"/> et <see cref="CloseDialog"/>.
        /// </remarks>
        string DW_Title { get; }

        /// <summary>
        /// Contenu textuel affiché dans la fenêtre dialogue.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="OpenDialog"/> et <see cref="CloseDialog"/>.
        /// </remarks>
        string DW_Content { get; }

        /// <summary>
        /// État d'ouverture de la fenêtre modale.
        /// <see langword="true"/> si la modale est ouverte ; <see langword="false"/> sinon.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="OpenDialog"/> et <see cref="CloseDialog"/>.
        /// </remarks>
        bool DW_IsOpen { get; }

        // --- Groupe 5 : Opérations atomiques ---

        /// <summary>
        /// Met à jour de manière atomique les dimensions et la marge ajustée de la fenêtre principale.
        /// </summary>
        /// <param name="width">Nouvelle largeur de la fenêtre principale, en pixels.</param>
        /// <param name="height">Nouvelle hauteur de la fenêtre principale, en pixels.</param>
        /// <param name="marginAdjusted">Nouvelle marge verticale ajustée, calculée par <c>SR_Window</c>.</param>
        void UpdateWindowDimensions(int width, int height, int marginAdjusted);

        /// <summary>
        /// Met à jour de manière atomique la position de la fenêtre principale.
        /// </summary>
        /// <param name="top">Nouvelle position verticale (Y) de la fenêtre, en pixels.</param>
        /// <param name="left">Nouvelle position horizontale (X) de la fenêtre, en pixels.</param>
        void UpdateWindowPosition(double top, double left);

        /// <summary>
        /// Ouvre la fenêtre dialogue en définissant son titre et son contenu de manière atomique.
        /// </summary>
        /// <param name="title">Titre à afficher dans la fenêtre dialogue.</param>
        /// <param name="content">Contenu textuel à afficher dans la fenêtre dialogue.</param>
        void OpenDialog(string title, string content);

        /// <summary>
        /// Ferme la fenêtre dialogue et réinitialise son titre et son contenu.
        /// </summary>
        void CloseDialog();

        /// <summary>
        /// Réinitialise l'intégralité de l'état mutable aux valeurs par défaut.
        /// </summary>
        void Reset();
    }
}