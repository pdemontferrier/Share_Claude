using System.Windows;
using System.Windows.Input;

namespace DG244Cutting.D_Presentation.Utilities.Cursor
{
    /// <summary>
    /// Propriété attachée pilotant le curseur global de l'application en réponse
    /// à un état booléen <c>IsBusy</c> exposé par un ViewModel.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Utilitaire WPF sans état, destiné à être bindé en XAML
    /// sur la propriété observable <c>IsProcessing</c> d'un ViewModel héritant de
    /// <see cref="VM_Page_Generic"/> ou <see cref="VM_MH_Generic"/>. Il remplace
    /// la manipulation directe de <see cref="Mouse.OverrideCursor"/> qui figurait dans
    /// les VM hérités du projet BatchStockRelease.</para>
    /// <para>Objectif : Garantir que la traduction visuelle d'un état de
    /// traitement (curseur d'attente) reste confinée à la couche View, sans coupler
    /// les ViewModels aux types WPF de présentation (<c>Mouse</c>, <c>Cursors</c>).</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Affecter <see cref="Mouse.OverrideCursor"/> à <see cref="Cursors.Wait"/>
    ///   lorsque l'état bindé passe à <see langword="true"/>.</item>
    ///   <item>Restaurer le curseur par défaut (<see langword="null"/>) lorsque l'état
    ///   bindé repasse à <see langword="false"/>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne maintient aucun état propre — la propriété attachée est un simple
    ///   relais entre le binding et l'API WPF.</item>
    ///   <item>Ne porte aucune logique métier ni aucun appel à un UseCase.</item>
    ///   <item>Ne participe pas à la <c>CallChain</c> applicative (utilitaire WPF sans état).</item>
    /// </list>
    /// </remarks>
    public static class UT_GlobalCursor
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
        /// Identifie la propriété attachée <c>IsBusy</c> exploitable en XAML via
        /// <c>cursors:UT_GlobalCursor.IsBusy="{Binding IsProcessing}"</c>.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.RegisterAttached(
                "IsBusy",
                typeof(bool),
                typeof(UT_GlobalCursor),
                new PropertyMetadata(defaultValue: false, propertyChangedCallback: OnIsBusyChanged));

        /// <summary>
        /// Lit la valeur courante de la propriété attachée <c>IsBusy</c> sur l'élément cible.
        /// </summary>
        /// <param name="target">Élément WPF porteur de la propriété attachée.</param>
        /// <returns><see langword="true"/> si l'état d'occupation est actif ; <see langword="false"/> sinon.</returns>
        public static bool GetIsBusy(DependencyObject target) =>
            (bool)target.GetValue(IsBusyProperty);

        /// <summary>
        /// Affecte la valeur de la propriété attachée <c>IsBusy</c> sur l'élément cible.
        /// </summary>
        /// <param name="target">Élément WPF porteur de la propriété attachée.</param>
        /// <param name="value">Nouvel état d'occupation à appliquer.</param>
        public static void SetIsBusy(DependencyObject target, bool value) =>
            target.SetValue(IsBusyProperty, value);

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Callback déclenché à chaque changement de la propriété attachée <c>IsBusy</c>,
        /// répercutant l'état sur <see cref="Mouse.OverrideCursor"/>.
        /// </summary>
        /// <remarks>
        /// <para>Comportement : Affecte <see cref="Cursors.Wait"/> lorsque la
        /// nouvelle valeur est <see langword="true"/>, et restaure le curseur système
        /// par défaut (<see langword="null"/>) sinon.</para>
        /// </remarks>
        /// <param name="d">Élément WPF dont la propriété a changé (non utilisé : l'effet est global).</param>
        /// <param name="e">Données de l'événement contenant l'ancienne et la nouvelle valeur.</param>
        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Mouse.OverrideCursor = (bool)e.NewValue ? Cursors.Wait : null;
        }

        #endregion
    }
}