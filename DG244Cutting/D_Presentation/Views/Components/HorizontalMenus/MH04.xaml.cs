using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.Settings;
using DG244Cutting.D_Presentation.ViewModels.Components.HorizontalMenus;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Components.HorizontalMenus
{
    /// <summary>
    /// Vue WPF du menu horizontal <c>MH04</c> de l'application
    /// DG244Cutting, associé à la
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page04"/>,
    /// exposant les cinq boutons transverses standards du socle
    /// <see cref="VM_MH_Generic"/> (dont <c>MH_Menu</c> bindé sur
    /// <c>ReduceCommand</c>), augmentés de quatre boutons d'action
    /// métier propres <c>MH_New</c>, <c>MH_Add</c>, <c>MH_Modify</c>,
    /// <c>MH_Save</c> bindés respectivement sur <c>NewCommand</c>,
    /// <c>AddCommand</c>, <c>ModifyCommand</c> et <c>SaveCommand</c>,
    /// dont la stylisation et la visibilité conditionnée sont portées
    /// par les overrides propres <see cref="ApplyLayout"/> et
    /// <see cref="ApplySecurityRules"/>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte :</para>
    ///
    /// <para>Code-behind de la vue du menu horizontal de la page
    /// d'administration des comptes utilisateurs (Page04). Il ajoute
    /// au socle <see cref="MH_Generic"/> la prise en charge visuelle
    /// des quatre boutons d'action métier de la page.</para>
    ///
    /// <para>Objectif :</para>
    ///
    /// <para>Styliser les quatre boutons d'action à parité avec les
    /// boutons transverses et en conditionner l'affichage aux droits
    /// granulaires de l'utilisateur courant sur la page courante, sans
    /// altérer le comportement du socle appliqué aux boutons
    /// transverses.</para>
    ///
    /// <para>Responsabilités :</para>
    ///
    /// <para>Résoudre l'instance Singleton du ViewModel associé et
    /// l'affecter au
    /// <see cref="System.Windows.FrameworkElement.DataContext"/>.
    /// Étendre le socle par deux overrides de points d'extension :
    /// <see cref="ApplyLayout"/> (stylisation des quatre boutons
    /// d'action) et <see cref="ApplySecurityRules"/> (visibilité
    /// conditionnée des quatre boutons d'action selon les droits de
    /// création et de modification).</para>
    ///
    /// <para>Non-responsabilités :</para>
    ///
    /// <para>Ne porte aucune logique métier ni aucun accès aux
    /// données : l'évaluation des droits est déléguée à
    /// <see cref="IU_Navigation"/> et le relais des actions au
    /// ViewModel. Ne surcharge aucun des quatre autres points
    /// d'extension du socle (<c>OnResized</c>,
    /// <c>ApplyNavigationRules</c>, <c>OnLoadedAsync</c>,
    /// <c>OnUnloadedAsync</c>).</para>
    ///
    /// <para>Note sur les exceptions architecturales :</para>
    ///
    /// <para>Consomme <see cref="IU_Navigation"/> (EA-05) via le champ
    /// <c>protected</c> <c>_navigation</c> hérité de
    /// <see cref="MH_Generic"/> pour l'évaluation des règles de
    /// visibilité, et <see cref="IS_ControlStyler"/> via le champ
    /// <c>protected</c> <c>_controlStyler</c> hérité pour la
    /// stylisation. Aucune EA supplémentaire propre. La lecture des
    /// prédicats <see cref="IU_Navigation.CurrentPageName"/>,
    /// <see cref="IU_Navigation.CanCreate"/> et
    /// <see cref="IU_Navigation.CanUpdate"/> relève de l'état
    /// d'affichage (visibilité), non d'une décision de naviguer
    /// (R-4.12.19 du 0231).</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2 du 0230), augmentée de l'extension §4.4.3
    /// <c>=== Méthodes protégées ===</c> — insérée entre la région
    /// Méthodes publiques et la région Méthodes privées (R-4.4.10 du
    /// 0231) — qui porte les deux overrides de points d'extension
    /// <see cref="ApplyLayout"/> et <see cref="ApplySecurityRules"/>.
    /// L'extension Propriétés publiques n'est pas présente.
    /// L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente. Soit six régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   porte la référence <c>_viewModel</c> au ViewModel Singleton
    ///   associé.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur sans paramètre (EA-06) résolvant le ViewModel via
    ///   <c>App.ServiceProvider.GetRequiredService</c>, initialisant les
    ///   composants XAML et affectant le
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c>
    ///   (extension §4.4.3) : porte les deux overrides
    ///   <see cref="ApplyLayout"/> et
    ///   <see cref="ApplySecurityRules"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>.</description></item>
    /// </list>
    /// </remarks>
    public partial class MH04 : MH_Generic
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Instance Singleton du ViewModel associé à la présente vue,
        /// résolue au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> et affectée à
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// pour alimenter les bindings WPF déclarés par
        /// <c>MH04.xaml</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Référence conservée à des fins de
        /// lisibilité ; l'alimentation des bindings passe par
        /// l'affectation de cette instance au
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// au constructeur.</para>
        /// </remarks>
        private readonly VM_MH04 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="MH04"/>.
        /// </summary>
        /// <remarks>
        /// <para>Séquence d'initialisation :</para>
        ///
        /// <para>Résout l'instance Singleton du ViewModel
        /// <see cref="VM_MH04"/> via
        /// <c>App.ServiceProvider.GetRequiredService</c> (EA-06),
        /// initialise les composants XAML par
        /// <c>InitializeComponent</c>, puis affecte le ViewModel au
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// pour alimenter les bindings de <c>MH04.xaml</c>. Les
        /// overrides <see cref="ApplyLayout"/> et
        /// <see cref="ApplySecurityRules"/> sont invoqués
        /// ultérieurement par les handlers de chargement du socle
        /// <see cref="MH_Generic"/>.</para>
        ///
        /// <para>Filet de sécurité :</para>
        ///
        /// <para>La résolution du ViewModel via
        /// <c>GetRequiredService</c> lève une exception si le service
        /// n'est pas enregistré, garantissant l'échec explicite en cas
        /// de mauvaise configuration du conteneur DI.</para>
        /// </remarks>
        public MH04()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_MH04>();

            InitializeComponent();

            DataContext = _viewModel;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Applique la stylisation invariante du menu horizontal :
        /// délègue au socle pour les boutons transverses, puis stylise
        /// les quatre boutons d'action métier <c>MH_New</c>,
        /// <c>MH_Add</c>, <c>MH_Modify</c> et <c>MH_Save</c>.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// de chargement du socle et propagée à
        /// <c>base.ApplyLayout</c>.</param>
        /// <remarks>
        /// <para>Contexte : Override du point d'extension synchrone
        /// <see cref="MH_Generic.ApplyLayout"/>. L'appel à
        /// <c>base.ApplyLayout(callChain)</c> est IMPÉRATIVEMENT la
        /// première instruction (R-3.14.7 du 0231), afin de préserver
        /// la stylisation des boutons transverses portée par le socle.
        /// La stylisation propre résout, pour chacun des quatre boutons
        /// d'action, le bouton, son icône et son libellé par le patron
        /// <c>Find&lt;T&gt;</c> avec garde <c>is</c> sans null-forgiving
        /// (R-4.15.25 du 0231), puis délègue à
        /// <see cref="IS_ControlStyler.StyleHorizontalMenuButton"/> avec
        /// l'icône correspondante de <c>RS_Icons</c>
        /// (<c>MH_New_Source</c>, <c>MH_Add_Source</c>,
        /// <c>MH_Modify_Source</c>, <c>MH_Save_Source</c>), à parité
        /// stricte avec l'appel appliqué par le socle aux boutons
        /// transverses.</para>
        /// </remarks>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Button>("MH_New") is Button newButton
                && Find<Image>("MH_New_Icon") is Image newIcon
                && Find<TextBlock>("MH_New_Text") is TextBlock newText)
            {
                _controlStyler.StyleHorizontalMenuButton(
                    newButton, newIcon, newText, RS_Icons.MH_New_Source);
            }

            if (Find<Button>("MH_Add") is Button addButton
                && Find<Image>("MH_Add_Icon") is Image addIcon
                && Find<TextBlock>("MH_Add_Text") is TextBlock addText)
            {
                _controlStyler.StyleHorizontalMenuButton(
                    addButton, addIcon, addText, RS_Icons.MH_Add_Source);
            }

            if (Find<Button>("MH_Modify") is Button modifyButton
                && Find<Image>("MH_Modify_Icon") is Image modifyIcon
                && Find<TextBlock>("MH_Modify_Text") is TextBlock modifyText)
            {
                _controlStyler.StyleHorizontalMenuButton(
                    modifyButton, modifyIcon, modifyText, RS_Icons.MH_Modify_Source);
            }

            if (Find<Button>("MH_Save") is Button saveButton
                && Find<Image>("MH_Save_Icon") is Image saveIcon
                && Find<TextBlock>("MH_Save_Text") is TextBlock saveText)
            {
                _controlStyler.StyleHorizontalMenuButton(
                    saveButton, saveIcon, saveText, RS_Icons.MH_Save_Source);
            }
        }

        /// <summary>
        /// Applique les règles de sécurité du menu horizontal :
        /// conditionne la visibilité des quatre boutons d'action aux
        /// droits granulaires de l'utilisateur courant sur la page
        /// courante.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// du socle et propagée à
        /// <c>base.ApplySecurityRules</c>.</param>
        /// <remarks>
        /// <para>Contexte : Override du point d'extension synchrone
        /// <see cref="MH_Generic.ApplySecurityRules"/>. L'appel à
        /// <c>base.ApplySecurityRules(callChain)</c> (implémentation
        /// socle vide) est placé en première instruction par respect du
        /// patron d'override (item MH-25, §5.4.7 du 0232-MH-VM). La
        /// visibilité est ensuite fixée par
        /// <see cref="MH_Generic.SetButtonVisibility"/> : les boutons
        /// <c>MH_New</c> et <c>MH_Add</c> sont conditionnés au droit de
        /// création (<see cref="IU_Navigation.CanCreate"/>), les boutons
        /// <c>MH_Modify</c> et <c>MH_Save</c> au droit de modification
        /// (<see cref="IU_Navigation.CanUpdate"/>), sur la page courante
        /// lue via <see cref="IU_Navigation.CurrentPageName"/>. Le droit
        /// d'accès à la page (<c>CanAccess</c>) est déjà acquis en amont
        /// par la navigation ; la visibilité ne teste donc que la
        /// création et la modification. Cet axe (visibilité par droits)
        /// est distinct et orthogonal de l'axe d'activation (état
        /// d'édition, porté par les gardes <c>CanExecute</c> des
        /// commandes du ViewModel) : un bouton peut être visible mais
        /// désactivé (§3.11 du 0230).</para>
        /// </remarks>
        protected override void ApplySecurityRules(string callChain)
        {
            base.ApplySecurityRules(callChain);

            string page = _navigation.CurrentPageName;
            bool canCreate = _navigation.CanCreate(page);
            bool canUpdate = _navigation.CanUpdate(page);

            SetButtonVisibility("MH_New", canCreate);
            SetButtonVisibility("MH_Add", canCreate);
            SetButtonVisibility("MH_Modify", canUpdate);
            SetButtonVisibility("MH_Save", canUpdate);
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}