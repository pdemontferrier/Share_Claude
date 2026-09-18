using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Implémentation du service technique de présentation chargé d'appliquer
    /// atomiquement les mutations dimensionnelles et positionnelles de la fenêtre
    /// principale dans le Setting <see cref="ISE_Window"/> au moyen de ses opérations
    /// atomiques publiques.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Service Singleton de présentation résidant en
    /// <c>D_Presentation/Services/</c>, conformément à la cohérence couche/domaine
    /// posée par la deuxième obligation contractuelle de §4.14.3 amendée du 0230
    /// (sous-dossier <c>Presentation</c> du contrat <see cref="IS_Window"/> →
    /// couche <c>D_Presentation</c>). Injecté via <see cref="IS_Window"/> et
    /// enregistré dans <c>E_Miscellaneous/CompositionRoot/SR_ConteneurDI.cs</c>.
    /// Consommé par le code-behind de <c>MainWindow</c> à chaque événement
    /// <c>SizeChanged</c> (via <see cref="ApplySize"/>) et <c>LocationChanged</c>
    /// (via <see cref="ApplyPosition"/>).
    /// </para>
    /// <para>
    /// Objectif : centraliser le calcul de la marge verticale ajustée et le routage
    /// vers les deux opérations atomiques publiques d'écriture du contrat
    /// <see cref="ISE_Window"/>, en parité une-à-une avec les deux événements WPF
    /// émis par la fenêtre principale. Aucune écriture directe dans les propriétés
    /// <c>{ get; }</c> du contrat n'est tolérée (R-4.14.16 du 0231 ; SE-09 du 0232-SE-RS).
    /// </para>
    /// <para>
    /// Régime d'accès au Setting : R-2.5.6 applicable au sous-dossier
    /// <c>Presentation</c> — l'injection de <see cref="ISE_Window"/> par constructeur
    /// est admise et effective. <see cref="IS_ExClassifier"/> est injecté au titre
    /// de service transversal d'utilité du tableau de §4.7.4 du 0230, pour la
    /// requalification terminale des exceptions non prévues conformément à R-4.7.25.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Calculer la marge verticale ajustée à partir de la hauteur courante et de la hauteur minimale.</description></item>
    ///   <item><description>Relayer atomiquement la nouvelle taille de fenêtre vers <see cref="ISE_Window.UpdateWindowDimensions"/>.</description></item>
    ///   <item><description>Relayer atomiquement la nouvelle position de fenêtre vers <see cref="ISE_Window.UpdateWindowPosition"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune logique métier propre à l'application.</description></item>
    ///   <item><description>Aucun abonnement aux événements WPF ; ce rôle relève du code-behind de <c>MainWindow</c>.</description></item>
    ///   <item><description>Aucun accès direct aux contrôles XAML.</description></item>
    ///   <item><description>Aucun accès aux données ni à un Repository.</description></item>
    ///   <item><description>Aucune écriture directe dans les propriétés <c>{ get; }</c> de <see cref="ISE_Window"/> : toute mutation passe par les opérations atomiques publiques du contrat.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_Window"/>
    /// <seealso cref="ISE_Window"/>
    /// <seealso cref="IS_ExClassifier"/>
    public class SR_Window : IS_Window
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Diviseur appliqué à l'écart entre la hauteur courante et la hauteur minimale
        /// pour produire la marge verticale ajustée. Conservé dans le Service au titre
        /// de la non-responsabilité de calcul portée par les Settings (§4.14.7 du 0230).
        /// </summary>
        private const int _marginDivisor = 13;

        // --- CallChain ---
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_Window _seWindow;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_Window"/>.
        /// </summary>
        /// <param name="seWindow">Setting transverse exposant l'état partagé des fenêtres ; injecté au titre de R-2.5.6 (sous-dossier <c>Presentation</c> hors <c>Business/</c>).</param>
        /// <param name="classifier">Service transversal d'utilité de requalification terminale des exceptions non prévues, conformément à §4.7.4 du 0230 et R-4.7.25.</param>
        /// <exception cref="ArgumentNullException">Levée lorsque <paramref name="seWindow"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_Window(ISE_Window seWindow, IS_ExClassifier classifier)
        {
            _seWindow = seWindow ?? throw new ArgumentNullException(nameof(seWindow));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Applique atomiquement la nouvelle taille (largeur, hauteur) de la fenêtre
        /// principale dans le Setting <see cref="ISE_Window"/>, en y associant la marge
        /// verticale ajustée dérivée des dimensions courantes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le code-behind de <c>MainWindow</c> à chaque événement
        /// <c>SizeChanged</c>. La méthode calcule la marge verticale ajustée par appel
        /// à <see cref="ComputeAdjustedMargin"/> puis délègue l'écriture à
        /// <see cref="ISE_Window.UpdateWindowDimensions"/> qui assure la cohérence des
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
        /// <para>
        /// Formule de calcul de la marge ajustée : <c>(height - MainWindowMinHeight) / 13</c>,
        /// ramenée à zéro lorsque la hauteur est inférieure ou égale à la hauteur minimale,
        /// afin d'éviter toute marge négative.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant, enrichie localement au format <c>{caller} &gt; {_callee} &gt; {nameof(ApplySize)}</c>.</param>
        /// <param name="width">Largeur courante de la fenêtre, en pixels.</param>
        /// <param name="height">Hauteur courante de la fenêtre, en pixels.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée au code <c>BU_ER_01</c> lorsque <paramref name="caller"/> est <see langword="null"/> ou vide.</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors du relais vers <see cref="ISE_Window"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée lorsque l'annulation coopérative est sollicitée via <paramref name="ct"/>.</exception>
        public void ApplySize(string caller, int width, int height, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ApplySize)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le paramètre '{nameof(caller)}' est null ou vide.");

                ct.ThrowIfCancellationRequested();

                int marginAdjusted = ComputeAdjustedMargin(height, _seWindow.MainWindowMinHeight);
                _seWindow.UpdateWindowDimensions(width, height, marginAdjusted);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Applique atomiquement la nouvelle position (top, left) de la fenêtre principale
        /// dans le Setting <see cref="ISE_Window"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le code-behind de <c>MainWindow</c> à chaque événement
        /// <c>LocationChanged</c>. La méthode délègue l'écriture à
        /// <see cref="ISE_Window.UpdateWindowPosition"/>.
        /// </para>
        /// <para>
        /// Dénomination dérogatoire au préfixe par défaut <c>Execute</c> (R-4.2.12) :
        /// même justification que <see cref="ApplySize"/> — le verbe <c>Apply</c> exprime
        /// la sémantique d'application d'une mesure WPF observée. Dérogation tracée au
        /// titre de SR20 cas Concept du 0232-SR.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant, enrichie localement au format <c>{caller} &gt; {_callee} &gt; {nameof(ApplyPosition)}</c>.</param>
        /// <param name="top">Position verticale (Y) du bord supérieur de la fenêtre, en pixels.</param>
        /// <param name="left">Position horizontale (X) du bord gauche de la fenêtre, en pixels.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée au code <c>BU_ER_01</c> lorsque <paramref name="caller"/> est <see langword="null"/> ou vide.</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors du relais vers <see cref="ISE_Window"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée lorsque l'annulation coopérative est sollicitée via <paramref name="ct"/>.</exception>
        public void ApplyPosition(string caller, double top, double left, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ApplyPosition)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le paramètre '{nameof(caller)}' est null ou vide.");

                ct.ThrowIfCancellationRequested();

                _seWindow.UpdateWindowPosition(top, left);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Calcule la marge verticale ajustée à partir de la hauteur courante et de la
        /// hauteur minimale de référence.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Méthode conservée dans le Service au titre de la non-responsabilité de calcul
        /// portée par les Settings (§4.14.7 du 0230). La formule appliquée est
        /// <c>(height - minHeight) / 13</c>, ramenée à zéro lorsque la hauteur est
        /// inférieure ou égale à la hauteur minimale, afin d'éviter toute marge négative.
        /// </para>
        /// </remarks>
        /// <param name="height">Hauteur courante de la fenêtre, en pixels.</param>
        /// <param name="minHeight">Hauteur minimale de référence, en pixels.</param>
        /// <returns>La marge ajustée, en pixels (toujours positive ou nulle).</returns>
        private static int ComputeAdjustedMargin(int height, int minHeight)
        {
            int delta = height - minHeight;
            return delta > 0 ? delta / _marginDivisor : 0;
        }

        #endregion
    }
}