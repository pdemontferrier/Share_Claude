using System.Threading;
using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Contrat du service technique de présentation chargé de relayer les mutations
    /// dimensionnelles et positionnelles de la fenêtre principale vers le Setting
    /// <see cref="ISE_Window"/> au moyen de ses opérations atomiques publiques.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à la première
    /// obligation contractuelle de §4.14.3 amendée du 0230, et consommée par injection
    /// de dépendances depuis le code-behind de <c>MainWindow</c>. Son implémentation
    /// concrète <see cref="DG244Cutting.D_Presentation.Services.SR_Window"/> réside
    /// en <c>D_Presentation/Services/</c>, conformément à la cohérence couche/domaine
    /// posée par la deuxième obligation contractuelle de §4.14.3 amendée (sous-dossier
    /// <c>Presentation</c> → couche <c>D_Presentation</c>).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin de mise à jour
    /// atomique des dimensions et de la position de la fenêtre principale, en isolant
    /// d'une part le calcul de la marge verticale ajustée et d'autre part le routage
    /// vers les opérations atomiques d'écriture
    /// <see cref="ISE_Window.UpdateWindowDimensions"/> et
    /// <see cref="ISE_Window.UpdateWindowPosition"/>.
    /// </para>
    /// <para>
    /// Alignement événementiel : le contrat expose deux méthodes en parité une-à-une
    /// avec les deux événements WPF émis par la fenêtre principale (<c>SizeChanged</c>
    /// et <c>LocationChanged</c>) et avec les deux opérations atomiques publiques
    /// d'écriture du contrat <see cref="ISE_Window"/>. Cette parité événement-méthode-
    /// opération évite toute collision sémantique entre la couche Service et la couche
    /// Setting.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Appliquer atomiquement la nouvelle taille (largeur, hauteur) de la fenêtre principale dans <see cref="ISE_Window"/> via <see cref="ISE_Window.UpdateWindowDimensions"/>.</description></item>
    ///   <item><description>Appliquer atomiquement la nouvelle position (top, left) de la fenêtre principale dans <see cref="ISE_Window"/> via <see cref="ISE_Window.UpdateWindowPosition"/>.</description></item>
    ///   <item><description>Calculer la marge verticale ajustée à partir des dimensions courantes et de la hauteur minimale de référence, et la transmettre via l'opération atomique de dimensions.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne s'abonne pas aux événements WPF : ce rôle relève du code-behind de <c>MainWindow</c>.</description></item>
    ///   <item><description>Ne porte aucune logique métier propre à l'application.</description></item>
    ///   <item><description>Ne manipule aucun contrôle XAML.</description></item>
    ///   <item><description>N'écrit pas directement dans les propriétés exposées en <c>{ get; }</c> par <see cref="ISE_Window"/> : toute mutation passe par les opérations atomiques publiques du contrat (R-4.14.16 du 0231 ; SE-09 du 0232-SE-RS).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.D_Presentation.Services.SR_Window"/>
    /// <seealso cref="ISE_Window"/>
    public interface IS_Window
    {
        /// <summary>
        /// Applique atomiquement la nouvelle taille (largeur, hauteur) de la fenêtre
        /// principale dans le Setting <see cref="ISE_Window"/>, en y associant la marge
        /// verticale ajustée dérivée des dimensions courantes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le code-behind de <c>MainWindow</c> à chaque événement
        /// <c>SizeChanged</c>. La méthode délègue l'écriture à l'opération atomique
        /// <see cref="ISE_Window.UpdateWindowDimensions"/>, qui assure la cohérence des
        /// notifications INPC sur les propriétés liées.
        /// </para>
        /// <para>
        /// Dénomination dérogatoire au préfixe par défaut <c>Execute</c> (R-4.2.12) :
        /// le concept porté par le Service étant le relais d'une mesure WPF observée
        /// vers le Setting transverse, le verbe <c>Apply</c> exprime plus fidèlement la
        /// sémantique d'application d'une nouvelle valeur que <c>Execute</c> qui suggère
        /// une exécution de cas d'usage. Dérogation tracée au titre de SR20 cas Concept
        /// du 0232-SR.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant (typiquement <c>nameof(MainWindow)</c>), enrichie localement au format normatif de §4.5 du 0230. Ne doit pas être <see langword="null"/> ni vide.</param>
        /// <param name="width">Largeur courante de la fenêtre, en pixels.</param>
        /// <param name="height">Hauteur courante de la fenêtre, en pixels.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée au code <c>BU_ER_01</c> lorsque <paramref name="caller"/> est <see langword="null"/> ou vide (précondition structurelle).</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors du relais vers le Setting <see cref="ISE_Window"/>.</exception>
        /// <exception cref="System.OperationCanceledException">Levée lorsque l'annulation coopérative est sollicitée via <paramref name="ct"/>.</exception>
        void ApplySize(string caller, int width, int height, CancellationToken ct = default);

        /// <summary>
        /// Applique atomiquement la nouvelle position (top, left) de la fenêtre principale
        /// dans le Setting <see cref="ISE_Window"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le code-behind de <c>MainWindow</c> à chaque événement
        /// <c>LocationChanged</c>. La méthode délègue l'écriture à l'opération atomique
        /// <see cref="ISE_Window.UpdateWindowPosition"/>.
        /// </para>
        /// <para>
        /// Dénomination dérogatoire au préfixe par défaut <c>Execute</c> (R-4.2.12) :
        /// même justification que <see cref="ApplySize"/> — le verbe <c>Apply</c> exprime
        /// la sémantique d'application d'une mesure WPF observée vers le Setting
        /// transverse. Dérogation tracée au titre de SR20 cas Concept du 0232-SR.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant, enrichie localement au format normatif de §4.5 du 0230. Ne doit pas être <see langword="null"/> ni vide.</param>
        /// <param name="top">Position verticale (Y) du bord supérieur de la fenêtre, en pixels.</param>
        /// <param name="left">Position horizontale (X) du bord gauche de la fenêtre, en pixels.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée au code <c>BU_ER_01</c> lorsque <paramref name="caller"/> est <see langword="null"/> ou vide (précondition structurelle).</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors du relais vers le Setting <see cref="ISE_Window"/>.</exception>
        /// <exception cref="System.OperationCanceledException">Levée lorsque l'annulation coopérative est sollicitée via <paramref name="ct"/>.</exception>
        void ApplyPosition(string caller, double top, double left, CancellationToken ct = default);
    }
}