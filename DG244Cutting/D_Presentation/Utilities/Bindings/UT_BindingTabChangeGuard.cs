using System;
using System.Windows;
using System.Windows.Controls;

namespace DG244Cutting.D_Presentation.Utilities.Bindings
{
    /// <summary>
    /// Utilitaire de binding WPF exposant une propriété attachée qui conditionne et,
    /// le cas échéant, annule le changement d'onglet d'un TabControl au moyen d'une
    /// garde fournie par le ViewModel consommateur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : WPF n'offre aucun événement de changement d'onglet annulable sur le
    /// TabControl - l'événement SelectionChanged est notifié après coup et ne permet
    /// pas de s'opposer à la bascule. Conditionner une sortie d'onglet impose donc un
    /// contournement : intercepter la sélection déjà survenue et, si elle doit être
    /// refusée, rétablir la sélection précédente. Le canal idiomatique de ce
    /// contournement est la propriété attachée, seul moyen d'exposer ce comportement
    /// au XAML sans logique en code-behind.
    /// </para>
    /// <para>
    /// Objectif : Offrir aux pages d'édition un point d'ancrage XAML unique pour
    /// conditionner ou annuler une bascule d'onglet. L'interception et l'annulation
    /// sont écrites une seule fois dans l'utilitaire et réutilisées par toute page ;
    /// la politique effective (condition de sortie, confirmation) est intégralement
    /// déléguée à une garde fournie par le ViewModel, l'utilitaire restant agnostique
    /// de toute condition de page.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer la propriété attachée Guard portant la garde de sortie, de signature (index source, index cible) vers autorisé.</description></item>
    ///   <item><description>Abonner et désabonner le gestionnaire de changement de sélection sur le TabControl selon que la garde est posée ou retirée.</description></item>
    ///   <item><description>Ne traiter que les changements de sélection émis par le TabControl gardé lui-même, en ignorant ceux remontés par des sélecteurs descendants.</description></item>
    ///   <item><description>Invoquer la garde et annuler la bascule refusée par rétablissement de la sélection précédente, sous drapeau anti-réentrance.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune politique ni condition de page : aucune référence à IS_Notification, à une entité ou à une quelconque notion de verrou ; toute décision vit dans la garde fournie.</description></item>
    ///   <item><description>Ne participe à aucune mécanique transverse du référentiel (CallChain, annulation coopérative, capture d'erreurs, log, Event Store, transactionnalité, multilingue, navigation).</description></item>
    ///   <item><description>Ne dépend d'aucun service injecté et n'est pas enregistrée dans le conteneur DI (SR_ConteneurDI).</description></item>
    /// </list>
    /// <para>Usage type :</para>
    /// <code>
    /// &lt;TabControl bindings:UT_BindingTabChangeGuard.Guard="{Binding LeaveGuard}" ... /&gt;
    /// </code>
    /// <para>
    /// où le préfixe d'alias bindings est déclaré par
    /// xmlns:bindings="clr-namespace:DG244Cutting.D_Presentation.Utilities.Bindings",
    /// et où LeaveGuard est un Func&lt;int, int, bool&gt; exposé par le ViewModel de la
    /// page consommatrice.
    /// </para>
    /// <para>
    /// Statut hors familles canoniques : La classe UT_BindingTabChangeGuard relève de
    /// la famille UT_ (Utility WPF) au sens de 2.7 du 0230 (Famille 6 - Utilitaires).
    /// Cette famille n'est pas couverte par la règle de parité interface/implémentation
    /// posée en 2.7.4 du 0230 et indexée par R-2.7.6 du 0231 ; aucun contrat IS_ ou
    /// autre n'est attendu.
    /// </para>
    /// <para>
    /// Exceptions architecturales : La classe ne porte aucune exception architecturale
    /// propre. Aucune ligne EA-NN ne lui est associée à l'inventaire de 4.15.10 du 0230.
    /// </para>
    /// </remarks>
    public static class UT_BindingTabChangeGuard
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Propriété attachée publique portant la garde de sortie d'onglet, invoquée à
        /// chaque tentative de bascule pour autoriser ou annuler le changement de
        /// sélection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : DependencyProperty attachée enregistrée sous le nom logique
        /// "Guard", de type Func&lt;int, int, bool&gt;, de type propriétaire
        /// UT_BindingTabChangeGuard et de valeur par défaut null. Sa métadonnée déclare
        /// le callback OnGuardChanged, déclenché à chaque changement de garde sur
        /// l'élément porteur.
        /// </para>
        /// <para>
        /// Objectif : Servir de point d'ancrage déclaratif en XAML pour la politique de
        /// sortie d'onglet. La garde reçoit l'index de l'onglet source et l'index de
        /// l'onglet cible et retourne true pour autoriser la bascule, false pour
        /// l'annuler. L'utilitaire ne connaît pas le contenu de cette politique : toute
        /// condition, toute confirmation vit dans la garde fournie par le ViewModel
        /// consommateur.
        /// </para>
        /// </remarks>
        public static readonly DependencyProperty GuardProperty =
            DependencyProperty.RegisterAttached(
                "Guard",
                typeof(Func<int, int, bool>),
                typeof(UT_BindingTabChangeGuard),
                new PropertyMetadata(null, OnGuardChanged));

        /// <summary>
        /// Lit la garde de sortie d'onglet actuellement attachée à l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <returns>La garde attachée à l'élément, ou <see langword="null"/> si aucune garde n'y est posée.</returns>
        public static Func<int, int, bool>? GetGuard(DependencyObject dp) =>
            (Func<int, int, bool>?)dp.GetValue(GuardProperty);

        /// <summary>
        /// Attache ou retire la garde de sortie d'onglet sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <param name="value">Garde à attacher, ou <see langword="null"/> pour retirer la garde en place.</param>
        public static void SetGuard(DependencyObject dp, Func<int, int, bool>? value) =>
            dp.SetValue(GuardProperty, value);

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Propriété attachée privée servant de drapeau anti-réentrance pendant le
        /// rétablissement d'une sélection refusée, afin de prévenir la boucle de
        /// changement de sélection déclenchée par le rétablissement lui-même.
        /// </summary>
        /// <remarks>
        /// DependencyProperty attachée enregistrée sous le nom logique
        /// "RevertingSelection", de type bool, de valeur par défaut false et sans
        /// callback. Positionnée à true le temps du rétablissement de l'onglet source
        /// (affectation de SelectedIndex), elle inhibe le traitement du SelectionChanged
        /// consécutif à ce rétablissement. Portée par élément, conformément à la nature
        /// sans état de la classe.
        /// </remarks>
        private static readonly DependencyProperty RevertingSelectionProperty =
            DependencyProperty.RegisterAttached(
                "RevertingSelection",
                typeof(bool),
                typeof(UT_BindingTabChangeGuard),
                new PropertyMetadata(false));

        /// <summary>
        /// Lit la valeur du drapeau anti-réentrance RevertingSelection sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <returns><see langword="true"/> si un rétablissement est en cours sur l'élément ; <see langword="false"/> sinon.</returns>
        private static bool GetRevertingSelection(DependencyObject dp) =>
            (bool)dp.GetValue(RevertingSelectionProperty);

        /// <summary>
        /// Affecte la valeur du drapeau anti-réentrance RevertingSelection sur l'élément cible.
        /// </summary>
        /// <param name="dp">Objet de dépendance porteur de la propriété attachée.</param>
        /// <param name="value">Nouvel état du drapeau à appliquer.</param>
        private static void SetRevertingSelection(DependencyObject dp, bool value) =>
            dp.SetValue(RevertingSelectionProperty, value);

        /// <summary>
        /// Callback déclenché à chaque changement de la propriété attachée Guard,
        /// abonnant ou désabonnant le gestionnaire de changement de sélection sur le
        /// TabControl cible selon que la nouvelle garde est posée ou retirée.
        /// </summary>
        /// <remarks>
        /// Le gestionnaire OnSelectionChanged est systématiquement désabonné avant tout
        /// réabonnement, garantissant l'idempotence de l'abonnement y compris lors d'une
        /// transition d'une garde vers une autre. Le réabonnement n'a lieu que si la
        /// nouvelle garde est non nulle ; une garde retirée (null) laisse le TabControl
        /// désabonné. Si le porteur n'est pas un TabControl, le callback est sans effet.
        /// </remarks>
        /// <param name="dp">Élément WPF dont la propriété a changé, attendu être un TabControl.</param>
        /// <param name="e">Données de l'événement contenant l'ancienne et la nouvelle garde.</param>
        private static void OnGuardChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is TabControl tabControl)
            {
                tabControl.SelectionChanged -= OnSelectionChanged;

                if (e.NewValue is not null)
                    tabControl.SelectionChanged += OnSelectionChanged;
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement SelectionChanged du TabControl gardé, invoquant
        /// la garde attachée et annulant la bascule d'onglet par rétablissement de la
        /// sélection précédente lorsque la garde la refuse.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le gestionnaire ne traite que les événements émis par le TabControl gardé
        /// lui-même : un SelectionChanged remonté (bubblé) par un sélecteur descendant
        /// résidant dans un onglet est ignoré par comparaison de e.OriginalSource au
        /// TabControl. Le traitement est également inhibé pendant un rétablissement en
        /// cours (drapeau RevertingSelection), ce qui neutralise la réentrance provoquée
        /// par le rétablissement lui-même.
        /// </para>
        /// <para>
        /// L'index source est résolu depuis l'item retiré (e.RemovedItems), l'index
        /// cible depuis SelectedIndex ; la garde est invoquée avec ce couple. Lorsqu'elle
        /// refuse la bascule, la sélection précédente est rétablie sous drapeau
        /// anti-réentrance. Si l'index source ne peut être déterminé (aucun item retiré,
        /// item introuvable dans Items), le gestionnaire s'abstient de tout veto
        /// (comportement fail-open) sans lever ni capturer d'exception.
        /// </para>
        /// </remarks>
        /// <param name="sender">Instance du TabControl gardé, source de l'événement.</param>
        /// <param name="e">Arguments de l'événement de changement de sélection, porteurs des items retirés et ajoutés.</param>
        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not TabControl tabControl)
                return;

            if (!ReferenceEquals(e.OriginalSource, tabControl))
                return;

            if (GetRevertingSelection(tabControl))
                return;

            if (e.RemovedItems.Count == 0)
                return;

            int from = tabControl.Items.IndexOf(e.RemovedItems[0]);
            if (from < 0)
                return;

            int to = tabControl.SelectedIndex;

            Func<int, int, bool>? guard = GetGuard(tabControl);
            if (guard is null)
                return;

            if (!guard(from, to))
            {
                SetRevertingSelection(tabControl, true);
                tabControl.SelectedIndex = from;
                SetRevertingSelection(tabControl, false);
            }
        }

        #endregion
    }
}