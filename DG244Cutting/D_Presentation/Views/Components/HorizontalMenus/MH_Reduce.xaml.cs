using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.D_Presentation.Settings;
using DG244Cutting.D_Presentation.ViewModels.Components.HorizontalMenus;

namespace DG244Cutting.D_Presentation.Views.Components.HorizontalMenus
{
    /// <summary>
    /// Vue WPF du composant partagé <c>MH_Reduce</c> de l'application
    /// DG244Cutting, état réduit du menu horizontal exposant un unique
    /// bouton <c>MH_Menu</c> dont l'action déclenche la bascule vers le
    /// composant <c>MHxx</c> de la page courante.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille MH de la couche
    /// <c>D_Presentation</c>, héritant directement de
    /// <see cref="UserControl"/>. Cette posture d'héritage est
    /// doctrinalement distincte de celle de la famille MHxx (héritage
    /// <c>MH_Generic</c>) et résulte de la dichotomie entre les deux
    /// états du menu horizontal : l'état réduit (le présent composant)
    /// ne porte qu'un unique bouton sur une zone visuelle étroite et ne
    /// mobilise pas le cycle de vie WPF élargi
    /// (<c>ApplyLayout</c>, <c>ApplyNavigationRules</c>,
    /// <c>ApplySecurityRules</c>, <c>OnLoadedAsync</c>,
    /// <c>OnUnloadedAsync</c>) du socle <c>MH_Generic</c>. La dichotomie
    /// est explicitement portée par le contrat
    /// <see cref="IS_ControlStyler"/> qui expose deux méthodes
    /// spécialisées : <c>StyleMH_Reduce(Border, Button, Uri)</c> pour
    /// l'état réduit et <c>StyleHorizontalMenuGrid(Grid, ColumnDefinition,
    /// ColumnDefinition, Border, double)</c> pour l'état déployé.</para>
    /// <para>Le composant est instancié par le framework WPF de
    /// navigation via <c>Activator.CreateInstance</c>, hors conteneur DI
    /// (I-4.13.10 du 0231). Le constructeur sans paramètre est imposé par
    /// cette contrainte et résout ses deux dépendances
    /// (<see cref="IS_ControlStyler"/> pour la stylisation et
    /// <see cref="VM_MH_Reduce"/> pour le <c>DataContext</c>) via
    /// <c>App.ServiceProvider.GetRequiredService</c>. L'URI <c>pack://</c>
    /// du composant est exposée par
    /// <c>ISE_Navigation.MH_Reduce_Source</c>.</para>
    /// <para>Acception doctrinale : Dans le modèle DG244Cutting, le
    /// composant <c>MH_Reduce</c> expose un unique bouton <c>MH_Menu</c>
    /// dont l'action déclenche la bascule vers le composant
    /// <c>MHxx</c> de la page courante. Les quatre boutons transverses
    /// (Menu, Home, Previous, Refresh) sont portés par les classes
    /// <c>MHxx</c> dérivées de <c>MH_Generic</c>, et non par
    /// <c>MH_Reduce</c>.</para>
    /// <para>Objectif : Constituer la vue WPF permanente du composant
    /// partagé de l'état réduit du menu horizontal, résoudre les deux
    /// dépendances <see cref="IS_ControlStyler"/> et
    /// <see cref="VM_MH_Reduce"/> via le ServiceProvider, affecter le
    /// ViewModel au <see cref="FrameworkElement.DataContext"/> pour
    /// activer les bindings WPF de la propriété observable
    /// <see cref="VM_MH_Generic.Label_MH_Menu"/> héritée par
    /// <c>VM_MH_Reduce</c> et de la commande
    /// <c>MenuCommand</c> héritée par <c>VM_MH_Reduce</c> de
    /// <c>VM_MH_Generic</c>, et appliquer au chargement la
    /// stylisation visuelle complète via
    /// <see cref="IS_ControlStyler.StyleMH_Reduce"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Servir de vue WPF — la composition XAML est
    ///   portée par <c>MH_Reduce.xaml</c> et se limite à un <c>Grid</c>
    ///   conteneur sans colonnes contenant une <c>Border</c> et un
    ///   <c>Button</c> unique alignés à droite.</description></item>
    ///   <item><description>Résoudre <see cref="IS_ControlStyler"/> et
    ///   <see cref="VM_MH_Reduce"/> via
    ///   <c>App.ServiceProvider.GetRequiredService</c> dans le
    ///   constructeur sans paramètre, conformément à la convention de
    ///   plateforme <c>App.ServiceProvider</c> (§4.15.11 du 0230) et au
    ///   titre de l'exception architecturale propre déclarée
    ///   ci-dessous.</description></item>
    ///   <item><description>Affecter
    ///   <see cref="FrameworkElement.DataContext"/> à l'instance de
    ///   <see cref="VM_MH_Reduce"/> pour alimenter les bindings WPF de la
    ///   propriété <see cref="VM_MH_Generic.Label_MH_Menu"/> héritée par
    ///   <c>VM_MH_Reduce</c> et de la
    ///   commande <c>MenuCommand</c> héritée de
    ///   <c>VM_MH_Generic</c>.</description></item>
    ///   <item><description>S'abonner à l'événement
    ///   <see cref="FrameworkElement.Loaded"/> du composant dans le
    ///   constructeur, vers le handler privé <see cref="OnLoaded"/>.</description></item>
    ///   <item><description>Appliquer au chargement, depuis
    ///   <see cref="OnLoaded"/>, la stylisation visuelle complète
    ///   (couleurs, polices, dimensions, bordures, icône) du composant
    ///   via <c>StyleMH_Reduce(MH_Border, MH_Menu,
    ///   RS_Icons.MH_Menu_Source)</c>, sous filet de sécurité ultime
    ///   local conformément à §4.15.9 du 0230.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Porter de la logique applicative propre — le
    ///   composant est un relais visuel pur entre
    ///   <see cref="VM_MH_Reduce"/> et le rendu WPF. Aucune décision de
    ///   navigation, aucun chargement de données, aucune commande
    ///   utilisateur propre.</description></item>
    ///   <item><description>Charger les libellés — le chargement est
    ///   intégralement porté par <c>VM_MH_Generic</c> via sa surcharge
    ///   nominative de <c>LoadLabels</c>, déclenchée au constructeur
    ///   de <see cref="VM_MH_Reduce"/> par l'invocation de
    ///   <c>InitializeLabels()</c> en dernière instruction, puis à la
    ///   réception des notifications INPC de
    ///   <c>ISE_App.AppCultureCode</c> (R-4.11.9 du 0231). Toute
    ///   tentative de chargement de libellé depuis le présent code-behind
    ///   constituerait une non-conformité à I-4.11.10 du
    ///   0231.</description></item>
    ///   <item><description>Mobiliser le cycle de vie WPF élargi
    ///   (<c>ApplyLayout</c>, <c>ApplyNavigationRules</c>,
    ///   <c>ApplySecurityRules</c>, <c>OnLoadedAsync</c>,
    ///   <c>OnUnloadedAsync</c>) — ces points d'extension sont
    ///   l'apanage de <c>MH_Generic</c> et de ses dérivés
    ///   <c>MHxx</c>. Le présent composant, par sa nature d'état
    ///   réduit minimal, n'en a pas besoin et n'hérite pas du
    ///   socle.</description></item>
    ///   <item><description>Traiter les exceptions applicatives au-delà
    ///   du filet de sécurité ultime local — la capture et le traitement
    ///   des exceptions typées sont assurés par le filet
    ///   <c>ExecuteSafeAsync</c> de <c>VM_Generic</c> côté ViewModel.
    ///   Le filet local de <see cref="OnLoaded"/> n'est qu'un rempart
    ///   ultime contre une défaillance inattendue de la stylisation
    ///   pendant le chargement.</description></item>
    /// </list>
    /// <para>Note sur le statut canonique : Le présent code-behind porte
    /// statut d'étalon doctrinal pour la famille MH dans sa variante (1)
    /// à héritage <see cref="UserControl"/> direct. Les conventions
    /// documentaires et structurelles appliquées (héritage
    /// <see cref="UserControl"/> direct, exception architecturale propre
    /// EA-08 Service Locator, résolution des dépendances dans le
    /// constructeur sans paramètre, abonnement <c>Loaded</c> direct vers
    /// un handler privé, filet de sécurité ultime local conformément à
    /// §4.15.9 du 0230, densité de documentation XML) sont destinées à
    /// servir de matière première à la rédaction ultérieure du §5 du
    /// 0230 et du 0232 dédié à la famille MH. Ces documents ne sont pas
    /// publiés à la date du présent fil ; leur rédaction relèvera de
    /// fils de maintenance documentaire distincts non encore ouverts.
    /// La seconde variante doctrinale de la famille (« composant à
    /// héritage <c>MH_Generic</c> ») est attendue avec la première
    /// production d'un <c>MHxx</c>. Les rubriques de documentation XML
    /// du présent fichier sont conformes à l'usage de la couche Vue dans
    /// le projet, sans usage de balises HTML conformément à I-4.3.2 du
    /// 0231.</para>
    /// <para>Note sur les exceptions architecturales : Le présent
    /// composant porte une exception architecturale propre, EA-08
    /// (Service Locator <c>App.ServiceProvider</c> en couche Vue), à
    /// parité doctrinale stricte avec <c>MH_Generic</c>. Le constructeur
    /// sans paramètre, imposé par l'instanciation du composant par
    /// <c>Activator.CreateInstance</c> du framework WPF de navigation au
    /// sein de <c>IU_Navigation.ReduceHorizontalMenuAsync</c>, interdit
    /// toute injection paramétrée. Les deux dépendances
    /// <see cref="IS_ControlStyler"/> et <see cref="VM_MH_Reduce"/> sont
    /// donc résolues par appel à la propriété statique publique
    /// <c>App.ServiceProvider.GetRequiredService&lt;T&gt;()</c>. Le
    /// périmètre d'application de l'EA est strict : le Service Locator
    /// n'est utilisé que dans le constructeur de <see cref="MH_Reduce"/>,
    /// et exclusivement pour la résolution de ces deux dépendances. Cf.
    /// §4.15.10 (note de fin, description faisant autorité d'EA-08) et
    /// §4.15.11 du 0230 (convention de plateforme App.ServiceProvider)
    /// pour la formalisation complète de
    /// cette exception. La signature du handler <see cref="OnLoaded"/>
    /// est <c>void</c> synchrone : aucune dérogation à l'usage
    /// <c>async void</c> n'est mobilisée par le présent composant.</para>
    /// </remarks>
    public partial class MH_Reduce : UserControl
    {
        #region === Propriétés privées ===

        // Aucune propriété privée propre exposée par MH_Reduce — le
        // composant ne porte ni backing field d'INPC (la propriété
        // observable Label_MH_Menu est portée par VM_MH_Generic) ni état
        // local au code-behind.

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service de stylisation centralisée des contrôles WPF, résolu
        /// au constructeur via <c>App.ServiceProvider.GetRequiredService</c>
        /// et consommé exclusivement par la méthode privée
        /// <see cref="StyleControls"/> au chargement du composant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Champ stocké en lecture seule, résolu en
        /// première instruction du constructeur au titre de l'EA-08
        /// (Service Locator <c>App.ServiceProvider</c>) documentée dans
        /// le <c>&lt;remarks&gt;</c> de classe. Consommé uniquement par
        /// <see cref="StyleControls"/> via la méthode dédiée
        /// <see cref="IS_ControlStyler.StyleMH_Reduce"/>, distincte de
        /// <c>StyleHorizontalMenuGrid</c> consommée par
        /// <c>MH_Generic</c>.</para>
        /// </remarks>
        private readonly IS_ControlStyler _controlStyler;

        /// <summary>
        /// Instance Singleton du ViewModel associé au présent composant,
        /// résolue au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> et affectée à
        /// <see cref="FrameworkElement.DataContext"/> pour alimenter les
        /// bindings WPF déclarés par <c>MH_Reduce.xaml</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Champ stocké en lecture seule pour exposer le
        /// type concret <see cref="VM_MH_Reduce"/> au code-behind,
        /// distinct du <see cref="FrameworkElement.DataContext"/> typé en
        /// <see cref="object"/>. Aucun usage local n'est nécessaire au-delà
        /// de l'affectation du <c>DataContext</c> dans le constructeur —
        /// le composant n'invoque aucune méthode ni commande du ViewModel
        /// depuis le code-behind, conformément à la séparation MVVM
        /// stricte.</para>
        /// </remarks>
        private readonly VM_MH_Reduce _viewModel;

        #endregion

        #region === Propriétés publiques ===

        // Aucune propriété publique exposée par MH_Reduce hors de celles
        // héritées de System.Windows.Controls.UserControl.

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        // Aucun événement propre exposé par MH_Reduce. L'événement WPF
        // Loaded est consommé par abonnement direct dans le constructeur
        // vers le handler privé OnLoaded ; il n'est pas re-exposé.

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="MH_Reduce"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre imposé par le
        /// framework WPF de navigation, qui instancie le composant via
        /// <c>Activator.CreateInstance</c> au sein de
        /// <c>IU_Navigation.ReduceHorizontalMenuAsync</c>. La résolution
        /// des dépendances ne peut donc se faire par injection paramétrée
        /// et s'effectue par le canal légitime
        /// <c>App.ServiceProvider.GetRequiredService</c>, au titre de la
        /// convention de plateforme documentée en §4.15.11 du 0230 et de
        /// l'EA-08 (Service Locator <c>App.ServiceProvider</c>) propre au
        /// présent composant.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Résolution de
        ///   <see cref="IS_ControlStyler"/> via
        ///   <c>App.ServiceProvider.GetRequiredService</c> et stockage
        ///   dans le champ <see cref="_controlStyler"/>.</description></item>
        ///   <item><description>Résolution de <see cref="VM_MH_Reduce"/>
        ///   via <c>App.ServiceProvider.GetRequiredService</c> et stockage
        ///   dans le champ <see cref="_viewModel"/>. La méthode
        ///   <c>GetRequiredService</c> est utilisée (et non
        ///   <c>GetService</c>), conformément à R-4.15.21 du 0231 et à
        ///   §4.15.11 du
        ///   0230 : toute dépendance non résolue doit faire échouer
        ///   l'instanciation immédiatement par exception explicite plutôt
        ///   que de produire une <see cref="NullReferenceException"/>
        ///   ultérieure.</description></item>
        ///   <item><description>Invocation de <c>InitializeComponent</c>
        ///   pour la composition XAML — étape impérativement préalable à
        ///   toute affectation de
        ///   <see cref="FrameworkElement.DataContext"/>.</description></item>
        ///   <item><description>Affectation de
        ///   <see cref="FrameworkElement.DataContext"/> à
        ///   <see cref="_viewModel"/> pour activer le binding WPF de la
        ///   propriété <see cref="VM_MH_Generic.Label_MH_Menu"/> héritée par
        ///   <c>VM_MH_Reduce</c> et le
        ///   binding de la commande <c>MenuCommand</c> héritée de
        ///   <c>VM_MH_Generic</c>.</description></item>
        ///   <item><description>Abonnement à l'événement
        ///   <see cref="FrameworkElement.Loaded"/> du composant vers le
        ///   handler privé <see cref="OnLoaded"/>.</description></item>
        /// </list>
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le constructeur
        /// au-delà des deux résolutions Service Locator. Une défaillance
        /// de <c>GetRequiredService</c> traduirait une erreur de
        /// configuration du conteneur DI et doit faire échouer
        /// l'instanciation immédiatement. Le filet de sécurité ultime du
        /// chargement est porté localement par <see cref="OnLoaded"/>,
        /// conformément à §4.15.9 du 0230.</para>
        /// </remarks>
        public MH_Reduce()
        {
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_MH_Reduce>();

            InitializeComponent();

            DataContext = _viewModel;

            Loaded += OnLoaded;
        }

        #endregion

        #region === Méthodes publiques ===

        // Aucune méthode publique exposée par MH_Reduce hors de celles
        // héritées de System.Windows.Controls.UserControl.

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de l'événement
        /// <see cref="FrameworkElement.Loaded"/> du composant, déclenchant
        /// l'application de la stylisation visuelle au premier rendu.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Invoqué une fois par le framework WPF lorsque
        /// le composant est chargé dans la frame
        /// <c>ActiveHorizontalMenu</c>. L'abonnement à
        /// <see cref="FrameworkElement.Loaded"/> est établi en dernière
        /// instruction du constructeur, vers le présent handler.</para>
        /// <para>Comportement : Délègue intégralement à la méthode privée
        /// <see cref="StyleControls"/> l'invocation de
        /// <see cref="IS_ControlStyler.StyleMH_Reduce"/>. Aucune autre
        /// opération n'est conduite ici.</para>
        /// <para>Filet de sécurité ultime : Toute exception non gérée par
        /// <see cref="StyleControls"/> est capturée par le bloc
        /// <c>catch (Exception ex)</c> et tracée via
        /// <see cref="Debug.WriteLine(string)"/> au format normatif
        /// §4.15.9 du 0230. Aucune exception ne remonte au
        /// <c>DispatcherUnhandledException</c> du framework WPF. Le filet
        /// est local au présent composant, sans dépendance à un socle
        /// générique — posture cohérente avec l'héritage
        /// <see cref="UserControl"/> direct (cf. <c>&lt;remarks&gt;</c>
        /// de classe).</para>
        /// <para>Signature <c>void</c> synchrone : la méthode
        /// <see cref="StyleControls"/> qu'elle invoque est elle-même
        /// strictement synchrone, l'application des styles ne déclenche
        /// aucune opération asynchrone. Aucune exception architecturale
        /// liée à <c>async void</c> n'est mobilisée.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (le composant
        /// lui-même), non utilisée.</param>
        /// <param name="e">Données de l'événement
        /// <see cref="RoutedEventArgs"/>, non utilisées.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StyleControls();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[{nameof(MH_Reduce)}.{nameof(OnLoaded)}] " +
                    $"Exception non gérée capturée par le filet de sécurité ultime : " +
                    $"{ex.GetType().Name} — {ex.Message}");
            }
        }

        /// <summary>
        /// Applique la stylisation visuelle complète du composant via la
        /// méthode dédiée <see cref="IS_ControlStyler.StyleMH_Reduce"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée synchrone, invoquée
        /// exclusivement par le handler <see cref="OnLoaded"/> au
        /// premier rendu du composant. Aucun usage en dehors de ce
        /// handler.</para>
        /// <para>Objectif : Appliquer la stylisation visuelle (couleurs,
        /// polices, dimensions, bordures, icône) des deux éléments
        /// stylisables du composant — la <c>Border</c> nommée
        /// <c>MH_Border</c> et le <c>Button</c> nommé <c>MH_Menu</c> —
        /// via la méthode dédiée
        /// <see cref="IS_ControlStyler.StyleMH_Reduce"/>, distincte de
        /// <c>StyleHorizontalMenuGrid</c> consommée par
        /// <c>MH_Generic</c>. L'URI de l'icône passée en troisième
        /// paramètre est lue directement sur le référentiel statique
        /// <see cref="RS_Icons.MH_Menu_Source"/>, sans passer par un
        /// service injecté — cohérent avec la nature
        /// « Référentiel Statique » de <see cref="RS_Icons"/> et avec la
        /// posture adoptée par <c>MH_Generic.StyleCommonButtons</c>
        /// pour les autres icônes de la famille MH.</para>
        /// <para>Filet de sécurité : Aucun try/catch local. Toute
        /// exception levée par <see cref="IS_ControlStyler.StyleMH_Reduce"/>
        /// est capturée par le filet de sécurité ultime du handler
        /// <see cref="OnLoaded"/> qui l'invoque.</para>
        /// </remarks>
        private void StyleControls()
        {
            _controlStyler.StyleMH_Reduce(MH_Border, MH_Menu, RS_Icons.MH_Menu_Source);
        }

        #endregion
    }
}