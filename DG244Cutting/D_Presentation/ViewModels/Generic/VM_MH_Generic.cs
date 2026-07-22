using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using System.Windows.Input;

namespace DG244Cutting.D_Presentation.ViewModels.Generic
{
    /// <summary>
    /// Classe de base commune à tous les ViewModels associés à une
    /// vue de type UserControl de menu horizontal de l'application
    /// DG244Cutting, socle propre à la famille VM_MH de la couche
    /// <c>D_Presentation</c>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Classe abstraite fille directe de
    /// <see cref="VM_Generic"/>, à parité ontologique avec
    /// <see cref="VM_Page_Generic"/> au sein de la hiérarchie des
    /// socles ViewModel. Toutes deux dérivent à parité de
    /// <see cref="VM_Generic"/> qui factorise les mécaniques
    /// transverses communes (implémentation INPC, CallChain, filet
    /// de sécurité applicatif, mécanique multilingue). La présente
    /// classe résout l'aiguillage propre à la famille des menus
    /// horizontaux : portage de la dépendance
    /// <see cref="IU_Navigation"/> au titre de l'EA-05, des cinq
    /// commandes transverses standards, de la mécanique d'état
    /// d'occupation observable (<see cref="IsProcessing"/> avec les
    /// helpers <see cref="BeginProcessing"/> et
    /// <see cref="EndProcessing"/>), et du hook de chargement
    /// asynchrone post-montage <see cref="LoadAsync"/> à parité
    /// avec <see cref="VM_Page_Generic"/>. Réside dans
    /// <c>D_Presentation/ViewModels/Generic</c>. Le statut
    /// <c>abstract</c> interdit l'instanciation directe et impose
    /// la dérivation systématique par les VM_MH concrets de
    /// l'application, conformément à §4.15.8 du 0230.</para>
    ///
    /// <para>Rationnel du renommage : La présente classe résulte
    /// d'un refactoring qui a substitué son héritage utilitaire
    /// historique à <see cref="VM_Page_Generic"/> par un héritage
    /// direct à <see cref="VM_Generic"/>. Cette refonte a entraîné
    /// le retrait du mot <c>Page</c> du nom de la classe
    /// (anciennement <c>VM_MH_Page_Generic</c>), ce mot retrouvant
    /// dans le nommage du projet sa portée stricte de composant
    /// WPF <see cref="System.Windows.Controls.Page"/>. Un ViewModel
    /// de menu horizontal n'est pas un ViewModel de page : il s'en
    /// distingue par sa vue associée (<c>UserControl</c> pour un
    /// menu horizontal, <c>Page</c> WPF pour une <c>VM_Page</c>) et
    /// par ses responsabilités propres (commandes transverses, état
    /// d'occupation). La parenté à <see cref="VM_Page_Generic"/>
    /// relevait d'une factorisation purement technique sans
    /// parenté ontologique ; elle est désormais résorbée par le
    /// partage d'un ancêtre commun explicite
    /// <see cref="VM_Generic"/>.</para>
    ///
    /// <para>Objectif : Porter au socle de la famille VM_MH les
    /// éléments transverses à tous les menus horizontaux de
    /// l'application — la dépendance <see cref="IU_Navigation"/>
    /// au titre de l'EA-05, les cinq commandes transverses
    /// standards (<see cref="MenuCommand"/>,
    /// <see cref="ReduceCommand"/>, <see cref="HomeCommand"/>,
    /// <see cref="PreviousCommand"/>, <see cref="RefreshCommand"/>),
    /// la propriété observable <see cref="IsProcessing"/> avec son
    /// setter privé, les helpers <see cref="BeginProcessing"/> et
    /// <see cref="EndProcessing"/> en <c>protected virtual</c>,
    /// les cinq handlers privés correspondants, et le hook
    /// <see cref="LoadAsync"/> en <c>public virtual</c> ajouté
    /// par parité de structure avec
    /// <see cref="VM_Page_Generic"/>.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Composer les cinq commandes
    ///   transverses standards (<see cref="MenuCommand"/>,
    ///   <see cref="ReduceCommand"/>, <see cref="HomeCommand"/>,
    ///   <see cref="PreviousCommand"/>, <see cref="RefreshCommand"/>)
    ///   via <see cref="UT_RelayCommandArg0Async"/> câblé sur les
    ///   cinq handlers privés correspondants. La composition est
    ///   unique et centralisée ; aucun ViewModel dérivé ne
    ///   recompose ces commandes.</description></item>
    ///   <item><description>Déléguer chacune des cinq commandes
    ///   à <see cref="IU_Navigation"/> via le filet de sécurité
    ///   <c>ExecuteSafeAsync</c> hérité de
    ///   <see cref="VM_Generic"/>, avec construction de la
    ///   CallChain initiale via <c>BuildFirstCallChain</c> et
    ///   propagation systématique au UseCase.</description></item>
    ///   <item><description>Maintenir l'état observable
    ///   <see cref="IsProcessing"/> et émettre la notification
    ///   INPC à chaque modification, via le helper
    ///   <c>SetProperty</c> hérité de
    ///   <see cref="VM_Generic"/>.</description></item>
    ///   <item><description>Garantir l'anti-réentrance des cinq
    ///   commandes transverses via leur prédicat <c>CanExecute</c>
    ///   câblé sur <c>!IsProcessing</c> : un clic répété rapide
    ///   sur un bouton transverse pendant qu'une navigation est en
    ///   cours ne déclenche pas une seconde invocation
    ///   concurrente.</description></item>
    ///   <item><description>Exposer aux dérivés les helpers
    ///   <see cref="BeginProcessing"/> et
    ///   <see cref="EndProcessing"/> qui pilotent l'état
    ///   <see cref="IsProcessing"/>. Les deux méthodes sont
    ///   déclarées <c>protected virtual</c>, autorisant un dérivé
    ///   à les surcharger pour enrichir le comportement (par
    ///   exemple, incrémenter un compteur d'occupation
    ///   imbriquée).</description></item>
    ///   <item><description>Encadrer chaque handler privé interne
    ///   par le pattern <c>BeginProcessing</c> / <c>try</c> /
    ///   <c>finally</c> / <c>EndProcessing</c>, garantissant que
    ///   <see cref="IsProcessing"/> est systématiquement remis à
    ///   <see langword="false"/> à l'issue du traitement, y
    ///   compris en cas d'exception capturée par le filet
    ///   <c>ExecuteSafeAsync</c> hérité.</description></item>
    ///   <item><description>Déclarer au socle de la famille VM_MH
    ///   le hook <see cref="LoadAsync"/> en
    ///   <c>public virtual</c>, avec implémentation par défaut
    ///   retournant <see cref="Task.CompletedTask"/>, à parité
    ///   nominale et structurelle avec
    ///   <see cref="VM_Page_Generic.LoadAsync"/> (§4.15.8 du
    ///   0230).</description></item>
    ///   <item><description>Porter au socle de la famille VM_MH les
    ///   quatre libellés transverses des boutons standards
    ///   (<see cref="Label_MH_Menu"/>, <see cref="Label_MH_Home"/>,
    ///   <see cref="Label_MH_Previous"/>,
    ///   <see cref="Label_MH_Refresh"/>) via une surcharge
    ///   nominative de <see cref="LoadLabels"/> héritée de
    ///   <see cref="VM_Generic"/>. La surcharge résout les quatre
    ///   clés legacy <c>MH_Ti_01</c>, <c>MH_Ti_04</c>,
    ///   <c>MH_Ti_02</c>, <c>MH_Ti_03</c> via
    ///   <see cref="IS_Dictionary"/> hérité de
    ///   <see cref="VM_Generic"/>. Cette factorisation est le
    ///   pendant côté libellés de la factorisation déjà réalisée
    ///   pour les cinq commandes et pour l'état d'occupation
    ///   observable, et permet à tout VM_MH concret minimal
    ///   d'hériter directement les quatre libellés transverses
    ///   sans surcharge propre (§4.15.8 du 0230).</description></item>
    ///   <item><description>Déléguer intégralement à
    ///   <see cref="VM_Generic"/> les responsabilités transverses
    ///   non spécifiques à la famille VM_MH (INPC, CallChain via
    ///   <c>BuildFirstCallChain</c>, filet
    ///   <c>ExecuteSafeAsync</c>, orchestration multilingue via
    ///   <c>InitializeLabels</c> et l'abonnement INPC à
    ///   <c>AppCultureCode</c>) par héritage strict.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne décide ni ne déclenche aucune
    ///   navigation directement : la décision de navigation est
    ///   portée exclusivement par <see cref="IU_Navigation"/>,
    ///   conformément à R-4.12.2. Les cinq handlers privés
    ///   délèguent simplement à <see cref="IU_Navigation"/> via
    ///   <c>ExecuteSafeAsync</c>.</description></item>
    ///   <item><description>N'invoque pas
    ///   <c>InitializeLabels()</c> en son constructeur, bien que la
    ///   présente classe porte désormais quatre libellés transverses
    ///   propres et leur surcharge de <see cref="LoadLabels"/>.
    ///   Son statut <c>abstract</c> et la règle R-4.11.8 du 0231
    ///   réservent cette invocation au constructeur du
    ///   <c>VM_MHxx</c> concret final, en dernière instruction,
    ///   pour prévenir l'écueil classique d'invocation virtuelle
    ///   dans un constructeur de classe de base avec dépendances
    ///   dérivées non encore initialisées. L'invocation effective
    ///   d'<c>InitializeLabels()</c> par le dérivé concret
    ///   déclenche la résolution dynamique de
    ///   <see cref="LoadLabels"/> qui aboutit — par dispatching
    ///   virtuel — sur l'override porté par la présente classe (et
    ///   éventuellement sur un override additionnel propre au
    ///   dérivé qui appellera <c>base.LoadLabels(callChain)</c>
    ///   en première instruction conformément à §3.14.4 du
    ///   0230).</description></item>
    ///   <item><description>Ne déclare aucune commande
    ///   contextuelle au menu : les commandes propres à un menu
    ///   horizontal spécifique (boutons d'action contextuels d'une
    ///   page particulière) sont déclarées et instanciées par les
    ///   ViewModels dérivés concrets.</description></item>
    ///   <item><description>N'expose pas <see cref="IU_Navigation"/>
    ///   aux dérivés : la dépendance est <c>private</c> dans la
    ///   présente classe, ce qui cantonne l'exception
    ///   architecturale d'accès direct à
    ///   <see cref="IU_Navigation"/> par un ViewModel (cf. §4.15.8
    ///   du 0230) au seul niveau de la classe de
    ///   base.</description></item>
    ///   <item><description>Ne réimplémente pas
    ///   <c>INotifyPropertyChanged</c> ni les helpers
    ///   <c>OnPropertyChanged</c> et <c>SetProperty</c> : ils sont
    ///   hérités de <see cref="VM_Generic"/>.</description></item>
    /// </list>
    ///
    /// <para>Statut de troisième exemple canonique :</para>
    ///
    /// <para>La présente classe constitue le troisième exemple
    /// canonique d'une séquence de cinq destinée à servir de
    /// matière première au futur 0232 de la famille ViewModels de
    /// présentation (à inscrire au 0230 dans un fil de maintenance
    /// documentaire ultérieur). Elle suit
    /// <see cref="VM_Generic"/> (premier exemple) et
    /// <see cref="VM_Page_Generic"/> (deuxième exemple). Le statut
    /// d'étalon doctrinal couvre l'héritage strict à
    /// <see cref="VM_Generic"/>, le constructeur à quatre
    /// paramètres avec délégation des trois premiers à
    /// <c>base(...)</c> et affectation propre du quatrième, la
    /// structure des sept régions, la composition canonique des
    /// cinq commandes transverses standards, la mécanique d'état
    /// d'occupation observable, et la signature canonique du hook
    /// <see cref="LoadAsync"/> reproduite à l'identique de
    /// <see cref="VM_Page_Generic.LoadAsync"/>.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à sept
    /// régions, conformément à §4.4.2 (cinq régions standard) et
    /// à §4.4.3 du 0230 (deux extensions présentes : Propriétés
    /// publiques car la classe expose des propriétés publiques
    /// propres, Méthodes protégées au titre de R-4.4.10 du 0231
    /// car la classe expose des méthodes <c>protected</c>
    /// propres) :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   champ <c>_isProcessing</c> et les quatre champs de
    ///   sauvegarde des libellés transverses
    ///   (<c>_label_mh_menu</c>, <c>_label_mh_home</c>,
    ///   <c>_label_mh_previous</c>,
    ///   <c>_label_mh_refresh</c>).</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   champ <see cref="_navigation"/>
    ///   (<see cref="IU_Navigation"/>, EA-05).</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c> :
    ///   <see cref="IsProcessing"/>, les cinq propriétés
    ///   <see cref="ICommand"/> (<see cref="MenuCommand"/>,
    ///   <see cref="ReduceCommand"/>, <see cref="HomeCommand"/>,
    ///   <see cref="PreviousCommand"/>, <see cref="RefreshCommand"/>),
    ///   et les quatre propriétés observables de libellés transverses
    ///   (<see cref="Label_MH_Menu"/>,
    ///   <see cref="Label_MH_Home"/>,
    ///   <see cref="Label_MH_Previous"/>,
    ///   <see cref="Label_MH_Refresh"/>).</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur <c>protected</c> à quatre
    ///   paramètres.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   contient <see cref="LoadAsync"/>
    ///   (<c>public virtual</c>, à parité avec
    ///   <see cref="VM_Page_Generic.LoadAsync"/>).</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   contient l'override <see cref="LoadLabels"/>
    ///   (<c>protected override</c>, surcharge première de
    ///   <c>VM_Generic.LoadLabels</c> au socle de la famille MH
    ///   pour l'alimentation des quatre libellés transverses), et
    ///   les deux helpers <see cref="BeginProcessing"/> et
    ///   <see cref="EndProcessing"/>
    ///   (<c>protected virtual</c>).</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   contient les cinq handlers privés
    ///   <see cref="ExecuteMenuAsync"/>,
    ///   <see cref="ExecuteReduceAsync"/>,
    ///   <see cref="ExecuteHomeAsync"/>,
    ///   <see cref="ExecutePreviousAsync"/>,
    ///   <see cref="ExecuteRefreshAsync"/>.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Événements / Délégués /
    /// Indexeurs ===</c> n'est pas présente : l'événement
    /// <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
    /// est porté par <see cref="VM_Generic"/> au titre d'INPC et
    /// n'est pas redéclaré ici.</para>
    ///
    /// <para>Exception architecturale documentée — EA-05 (accès
    /// direct à <see cref="IU_Navigation"/> par un ViewModel) :</para>
    ///
    /// <para>Cette classe injecte directement
    /// <see cref="IU_Navigation"/>, alors que R-4.12.2 réserve les
    /// décisions de navigation aux UseCases et que la doctrine
    /// générale déconseille l'accès direct des ViewModels aux
    /// UseCases de navigation (ils passent normalement par
    /// <c>IS_UseCaseInvoker</c> au titre de l'EA-11). Cette
    /// exception est délibérée et strictement cantonnée aux cinq
    /// commandes transverses standards du menu horizontal : leur
    /// sémantique purement navigationnelle (sans décision métier
    /// propre) justifie la délégation directe à
    /// <see cref="IU_Navigation"/> sans intermédiation par un
    /// UseCase métier ni par <c>IS_UseCaseInvoker</c>. La
    /// dépendance est stockée en <c>private</c> pour interdire
    /// toute propagation de cette dérogation aux dérivés. Cf.
    /// §4.15.8 du 0230 pour la formalisation complète de cette
    /// exception architecturale.</para>
    /// </remarks>
    public abstract class VM_MH_Generic : VM_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Champ support de la propriété observable
        /// <see cref="IsProcessing"/>, initialisé à
        /// <see langword="false"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Modifié exclusivement via
        /// <see cref="BeginProcessing"/> et
        /// <see cref="EndProcessing"/>, qui utilisent le helper
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>
        /// pour déclencher la notification INPC.</para>
        /// </remarks>
        private bool _isProcessing;

        /// <summary>
        /// Champ support de la propriété observable
        /// <see cref="Label_MH_Menu"/>, initialisé à la chaîne
        /// vide. Alimenté par l'override de
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// legacy <c>MH_Ti_01</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Modifié exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>
        /// pour déclencher la notification INPC.</para>
        /// </remarks>
        private string _label_mh_menu = string.Empty;

        /// <summary>
        /// Champ support de la propriété observable
        /// <see cref="Label_MH_Home"/>, initialisé à la chaîne
        /// vide. Alimenté par l'override de
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// legacy <c>MH_Ti_04</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Modifié exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>
        /// pour déclencher la notification INPC.</para>
        /// </remarks>
        private string _label_mh_home = string.Empty;

        /// <summary>
        /// Champ support de la propriété observable
        /// <see cref="Label_MH_Previous"/>, initialisé à la chaîne
        /// vide. Alimenté par l'override de
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// legacy <c>MH_Ti_02</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Modifié exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>
        /// pour déclencher la notification INPC.</para>
        /// </remarks>
        private string _label_mh_previous = string.Empty;

        /// <summary>
        /// Champ support de la propriété observable
        /// <see cref="Label_MH_Refresh"/>, initialisé à la chaîne
        /// vide. Alimenté par l'override de
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// legacy <c>MH_Ti_03</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Modifié exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>
        /// pour déclencher la notification INPC.</para>
        /// </remarks>
        private string _label_mh_refresh = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// UseCase de navigation, consommé exclusivement par les
        /// cinq handlers privés de la présente classe pour
        /// déléguer les commandes transverses standards.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton injecté par le conteneur DI
        /// au constructeur, au titre de l'exception architecturale
        /// EA-05 (accès direct à <see cref="IU_Navigation"/> par un
        /// ViewModel) documentée en remarque de classe. La
        /// visibilité <c>private</c> est délibérée et impérative :
        /// elle cantonne l'EA-05 au seul niveau de la présente
        /// classe de base et interdit à tout ViewModel dérivé
        /// d'accéder directement à <see cref="IU_Navigation"/>.
        /// Tout besoin d'un dérivé d'invoquer une opération de
        /// navigation passe par les cinq commandes transverses
        /// exposées via les propriétés publiques, ou par un
        /// UseCase métier propre.</para>
        /// </remarks>
        private readonly IU_Navigation _navigation;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient une valeur indiquant si une commande transverse
        /// du menu horizontal est en cours d'exécution.
        /// </summary>
        /// <value>
        /// <see langword="true"/> entre l'invocation de
        /// <see cref="BeginProcessing"/> et celle de
        /// <see cref="EndProcessing"/> ; <see langword="false"/>
        /// sinon.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété observable bindable consommée
        /// par les vues pour afficher un indicateur visuel
        /// d'occupation (curseur d'attente, désactivation visuelle
        /// de contrôles). Consommée également en interne par les
        /// prédicats <c>CanExecute</c> des cinq commandes
        /// transverses pour garantir l'anti-réentrance.</para>
        /// <para>Mutation : L'accesseur en écriture est
        /// <c>private</c>. La propriété ne peut être modifiée qu'au
        /// travers des deux helpers protégés
        /// <see cref="BeginProcessing"/> et
        /// <see cref="EndProcessing"/>, qui sont les seuls points
        /// d'écriture admis.</para>
        /// </remarks>
        public bool IsProcessing
        {
            get => _isProcessing;
            private set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>
        /// Commande transverse de bascule vers le menu horizontal
        /// déployé spécifique à la page courante.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur le bouton <c>MH_Menu</c> du
        /// menu horizontal, côté <c>MH_Reduce</c> (menu réduit
        /// commun). Pendant symétrique de
        /// <see cref="ReduceCommand"/> côté <c>MHNN</c> (menus
        /// horizontaux déployés des pages). Délègue à
        /// <see cref="IU_Navigation.ExpendHorizontalMenuAsync"/>.</para>
        /// </remarks>
        public ICommand MenuCommand { get; }

        /// <summary>
        /// Commande transverse de réduction du menu horizontal
        /// déployé vers le composant de menu réduit commun.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur le bouton <c>MH_Menu</c> du
        /// menu horizontal, côté <c>MHNN</c> (menus horizontaux
        /// déployés des pages). Pendant symétrique de
        /// <see cref="MenuCommand"/> côté <c>MH_Reduce</c> (menu
        /// réduit commun). Délègue à
        /// <see cref="IU_Navigation.ReduceHorizontalMenuAsync"/>.</para>
        /// </remarks>
        public ICommand ReduceCommand { get; }

        /// <summary>
        /// Commande transverse de navigation vers la page d'accueil
        /// applicative.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur le bouton <c>MH_Home</c> du
        /// menu horizontal. Délègue à
        /// <see cref="IU_Navigation.NavigateToDefaultAsync"/>.</para>
        /// </remarks>
        public ICommand HomeCommand { get; }

        /// <summary>
        /// Commande transverse de navigation vers la page
        /// précédemment visitée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur le bouton <c>MH_Previous</c>
        /// du menu horizontal. Délègue à
        /// <see cref="IU_Navigation.NavigateToPreviousPageAsync"/>.</para>
        /// </remarks>
        public ICommand PreviousCommand { get; }

        /// <summary>
        /// Commande transverse de rafraîchissement de la page
        /// courante.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur le bouton <c>MH_Refresh</c>
        /// du menu horizontal. Délègue à
        /// <see cref="IU_Navigation.RefreshCurrentPageAsync"/>.</para>
        /// </remarks>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Libellé multilingue transverse du bouton
        /// <c>MH_Menu</c>, alimenté par
        /// <see cref="LoadLabels"/> à partir de la clé legacy
        /// <c>MH_Ti_01</c> via <see cref="IS_Dictionary"/> hérité
        /// de <see cref="VM_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable bindée par la Vue
        /// sur le <c>TextBlock</c> du bouton <c>MH_Menu</c> (cf.
        /// <c>MH_Generic</c> §4.15.9 du 0230 et son contrat XAML).
        /// Setter privé : mutation exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de
        /// <see cref="VM_Generic"/>.</para>
        /// <para>Valeur initiale : Chaîne vide avant le premier
        /// appel à <see cref="LoadLabels"/> orchestré par
        /// <c>VM_Generic.InitializeLabels</c>, qui est invoqué en
        /// dernière instruction du constructeur de tout
        /// <c>VM_MHxx</c> concret final conformément à
        /// R-4.11.8 du 0231.</para>
        /// </remarks>
        public string Label_MH_Menu
        {
            get => _label_mh_menu;
            private set => SetProperty(ref _label_mh_menu, value);
        }

        /// <summary>
        /// Libellé multilingue transverse du bouton
        /// <c>MH_Home</c>, alimenté par
        /// <see cref="LoadLabels"/> à partir de la clé legacy
        /// <c>MH_Ti_04</c> via <see cref="IS_Dictionary"/> hérité
        /// de <see cref="VM_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable bindée par la Vue
        /// sur le <c>TextBlock</c> du bouton <c>MH_Home</c>.
        /// Setter privé : mutation exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de
        /// <see cref="VM_Generic"/>.</para>
        /// <para>Valeur initiale : Chaîne vide avant le premier
        /// appel à <see cref="LoadLabels"/> orchestré par
        /// <c>VM_Generic.InitializeLabels</c>.</para>
        /// </remarks>
        public string Label_MH_Home
        {
            get => _label_mh_home;
            private set => SetProperty(ref _label_mh_home, value);
        }

        /// <summary>
        /// Libellé multilingue transverse du bouton
        /// <c>MH_Previous</c>, alimenté par
        /// <see cref="LoadLabels"/> à partir de la clé legacy
        /// <c>MH_Ti_02</c> via <see cref="IS_Dictionary"/> hérité
        /// de <see cref="VM_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable bindée par la Vue
        /// sur le <c>TextBlock</c> du bouton <c>MH_Previous</c>.
        /// Setter privé : mutation exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de
        /// <see cref="VM_Generic"/>.</para>
        /// <para>Valeur initiale : Chaîne vide avant le premier
        /// appel à <see cref="LoadLabels"/> orchestré par
        /// <c>VM_Generic.InitializeLabels</c>.</para>
        /// </remarks>
        public string Label_MH_Previous
        {
            get => _label_mh_previous;
            private set => SetProperty(ref _label_mh_previous, value);
        }

        /// <summary>
        /// Libellé multilingue transverse du bouton
        /// <c>MH_Refresh</c>, alimenté par
        /// <see cref="LoadLabels"/> à partir de la clé legacy
        /// <c>MH_Ti_03</c> via <see cref="IS_Dictionary"/> hérité
        /// de <see cref="VM_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable bindée par la Vue
        /// sur le <c>TextBlock</c> du bouton <c>MH_Refresh</c>.
        /// Setter privé : mutation exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de
        /// <see cref="VM_Generic"/>.</para>
        /// <para>Valeur initiale : Chaîne vide avant le premier
        /// appel à <see cref="LoadLabels"/> orchestré par
        /// <c>VM_Generic.InitializeLabels</c>.</para>
        /// </remarks>
        public string Label_MH_Refresh
        {
            get => _label_mh_refresh;
            private set => SetProperty(ref _label_mh_refresh, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de
        /// <see cref="VM_MH_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur <c>protected</c>
        /// (instanciation directe interdite, dérivation
        /// uniquement). La classe étant <c>abstract</c>,
        /// le constructeur n'est invoqué que par les constructeurs
        /// des ViewModels de menus horizontaux dérivés via
        /// <c>base(...)</c>.</para>
        /// <para>Séquence d'initialisation (R-4.4.7 du 0231) :</para>
        /// <list type="number">
        ///   <item><description>Délégation à
        ///   <see cref="VM_Generic"/> via
        ///   <c>base(dictionary, logAndNotify, app)</c> en première
        ///   instruction, qui prend en charge l'affectation des
        ///   trois dépendances héritées, l'initialisation de
        ///   <c>_callee</c> via <c>GetType().Name</c>, et les
        ///   gardes <see cref="ArgumentNullException"/> sur les
        ///   trois paramètres hérités.</description></item>
        ///   <item><description>Garde nullité et affectation de la
        ///   dépendance propre <see cref="_navigation"/>.</description></item>
        ///   <item><description>Composition des cinq commandes
        ///   transverses standards via
        ///   <see cref="UT_RelayCommandArg0Async"/>, chacune câblée
        ///   sur son handler privé correspondant et associée au
        ///   prédicat <c>CanExecute</c> <c>!IsProcessing</c> pour
        ///   l'anti-réentrance.</description></item>
        /// </list>
        /// <para>Ce que le constructeur ne fait pas : Il n'appelle
        /// pas <c>InitializeLabels()</c> héritée de
        /// <see cref="VM_Generic"/>, car la présente classe n'a
        /// aucun libellé propre à charger. Les ViewModels dérivés
        /// concrets qui auraient des libellés multilingues à
        /// exposer (par exemple, des tooltips sur les boutons
        /// transverses) override <c>LoadLabels</c> et appellent
        /// <c>InitializeLabels()</c> en dernière instruction de
        /// leur propre constructeur, après l'affectation de leurs
        /// dépendances propres, conformément à R-4.11.8 du 0231.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_Generic"/>.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_Generic"/> au titre de l'EA-01. Injecté en
        /// Singleton par le conteneur DI.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_Generic"/> pour la
        /// mécanique multilingue factorisée. Injecté en Singleton
        /// par le conteneur DI.</param>
        /// <param name="navigation">UseCase de navigation, consommé
        /// exclusivement par les cinq handlers privés de la
        /// présente classe. Injecté en Singleton par le conteneur
        /// DI au titre de l'EA-05.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="navigation"/> est <see langword="null"/>.
        /// Les gardes sur les trois autres paramètres sont portées
        /// par <see cref="VM_Generic"/> via <c>base(...)</c>.</exception>
        protected VM_MH_Generic(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IU_Navigation navigation)
            : base(dictionary, logAndNotify, app)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            MenuCommand = new UT_RelayCommandArg0Async(ExecuteMenuAsync, () => !IsProcessing);
            ReduceCommand = new UT_RelayCommandArg0Async(ExecuteReduceAsync, () => !IsProcessing);
            HomeCommand = new UT_RelayCommandArg0Async(ExecuteHomeAsync, () => !IsProcessing);
            PreviousCommand = new UT_RelayCommandArg0Async(ExecutePreviousAsync, () => !IsProcessing);
            RefreshCommand = new UT_RelayCommandArg0Async(ExecuteRefreshAsync, () => !IsProcessing);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Hook asynchrone de chargement post-montage propre à la
        /// famille VM_MH, point d'arrivée canonique invoqué par
        /// le code-behind du UserControl de menu horizontal
        /// associé à la fin de la séquence de chargement WPF.
        /// </summary>
        /// <param name="callChain">CallChain de l'opération
        /// englobante, propagée à l'override par l'orchestrateur
        /// appelant pour la traçabilité de l'exécution.</param>
        /// <param name="ct">Token d'annulation coopérative propagé
        /// à l'override.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// du chargement. L'implémentation par défaut retourne
        /// <see cref="Task.CompletedTask"/>.</returns>
        /// <remarks>
        /// <para>Contexte : Méthode <c>public virtual</c>
        /// déclarée au niveau de <see cref="VM_MH_Generic"/> à
        /// parité avec <see cref="VM_Page_Generic.LoadAsync"/>,
        /// pour override par les VM_MH concrets ayant
        /// effectivement un état asynchrone à initialiser au
        /// montage de leur menu horizontal. La visibilité publique
        /// est imposée par §4.15.8 du 0230 pour permettre
        /// l'invocation directe depuis le code-behind du
        /// UserControl de menu horizontal associé via le patron de
        /// surcharge OnLoadedAsync de §4.15.9.</para>
        ///
        /// <para>Caractère marginal de l'override côté famille
        /// VM_MH : Un menu horizontal typique n'a pas de
        /// chargement asynchrone à effectuer au montage : toutes
        /// ses informations transverses (par exemple un indicateur
        /// d'état de connexion, un compteur de notifications, un
        /// libellé d'utilisateur courant) sont déjà disponibles
        /// via des Settings observables auxquels le ViewModel se
        /// binde directement par INPC. L'override de
        /// <see cref="LoadAsync"/> est donc attendu marginalement
        /// côté famille VM_MH, contrairement à la famille
        /// <see cref="VM_Page_Generic"/> où il est attendu
        /// systématiquement pour le chargement des données métier
        /// de la page. L'override reste néanmoins possible pour
        /// les cas où un menu horizontal aurait effectivement un
        /// état spécifique à initialiser de façon asynchrone
        /// post-montage.</para>
        ///
        /// <para>Point d'arrivée canonique : Le hook est invoqué
        /// systématiquement depuis
        /// <c>MH_Generic.OnLoadedAsync</c>, point d'extension
        /// asynchrone exposé par le socle de la famille MH côté
        /// code-behind WPF (cf. §4.15.9 du 0230). L'invocation côté
        /// <c>MH_Generic</c> résout le ViewModel courant via son
        /// <c>DataContext</c> et appelle
        /// <see cref="LoadAsync"/> avec la CallChain construite
        /// localement par le handler WPF. L'implémentation par
        /// défaut <see cref="Task.CompletedTask"/> absorbe sans
        /// coût le cas dominant où aucun override n'est défini.</para>
        ///
        /// <para>Parité nominale avec
        /// <see cref="VM_Page_Generic.LoadAsync"/> : La signature
        /// est strictement identique (nom <c>LoadAsync</c> sans
        /// suffixe <c>Data</c>, paramètre <c>callChain</c>,
        /// paramètre <c>ct</c> avec valeur par défaut). Cette
        /// parité formelle entre les deux familles dérivées de
        /// <see cref="VM_Generic"/> est exigée par §4.15.8 du
        /// 0230. Le nom retenu est <c>LoadAsync</c> (et non
        /// <c>LoadDataAsync</c>) parce qu'un ViewModel — qu'il
        /// pilote une <c>Page</c> ou un menu horizontal — peut
        /// charger d'autres choses que des données métier stricto
        /// sensu (un état UI, des préférences utilisateur, un
        /// calcul cosmétique préalable) ; le nom <c>LoadAsync</c>
        /// est sobre et exact.</para>
        ///
        /// <para>Implémentation par défaut : Retourne
        /// <see cref="Task.CompletedTask"/>. Les VM_MH qui n'ont
        /// pas de chargement asynchrone post-montage n'ont pas à
        /// override cette méthode.</para>
        ///
        /// <para>Annulation coopérative : Le paramètre
        /// <paramref name="ct"/> est propagé à l'override par
        /// l'orchestrateur appelant ; les overrides qui invoquent
        /// à leur tour des UseCases asynchrones doivent propager
        /// <paramref name="ct"/> à l'invocation, conformément à
        /// §4.7.3 (annulation coopérative). Une
        /// <see cref="OperationCanceledException"/> levée par
        /// l'override est capturée et remontée silencieusement
        /// par le filet <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>.</para>
        ///
        /// <para>Patron de surcharge normatif :</para>
        /// <example>
        /// <code>
        /// public override async Task LoadAsync(
        ///     string callChain,
        ///     CancellationToken ct = default)
        /// {
        ///     string innerCallChain = BuildFirstCallChain();
        ///     await ExecuteSafeAsync(innerCallChain, async () =>
        ///     {
        ///         // Appel asynchrone (UseCase métier propre,
        ///         // initialisation d'un état UI, etc.) sous
        ///         // protection du filet à cinq captures hérité.
        ///     }, ct);
        /// }
        /// </code>
        /// </example>
        ///
        /// <para>Le patron ci-dessus illustre la consommation
        /// canonique des deux helpers hérités de
        /// <see cref="VM_Generic"/> : <c>BuildFirstCallChain</c>
        /// pour la construction de la CallChain initiale et
        /// <c>ExecuteSafeAsync</c> pour le filet de sécurité
        /// applicatif à cinq captures. Il est strictement
        /// identique au patron documentaire de
        /// <see cref="VM_Page_Generic.LoadAsync"/>, au titre de
        /// la parité de structure entre les deux familles
        /// dérivées.</para>
        /// </remarks>
        public virtual Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
            => Task.CompletedTask;

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Surcharge nominative au socle de la famille VM_MH de
        /// <c>VM_Generic.LoadLabels</c>, alimentant les quatre
        /// propriétés observables de libellés transverses
        /// <see cref="Label_MH_Menu"/>,
        /// <see cref="Label_MH_Home"/>,
        /// <see cref="Label_MH_Previous"/> et
        /// <see cref="Label_MH_Refresh"/> par résolution des
        /// quatre clés legacy
        /// <c>MH_Ti_01</c>, <c>MH_Ti_04</c>, <c>MH_Ti_02</c>,
        /// <c>MH_Ti_03</c> via <see cref="IS_Dictionary"/> hérité
        /// de <see cref="VM_Generic"/>.
        /// </summary>
        /// <param name="callChain">CallChain de l'opération
        /// englobante, propagée à
        /// <c>IS_Dictionary.GetText</c> pour la traçabilité de
        /// l'exécution.</param>
        /// <remarks>
        /// <para>Contexte : Surcharge première de
        /// <c>VM_Generic.LoadLabels</c> au niveau du socle de la
        /// famille VM_MH. Invoquée par
        /// <c>VM_Generic.InitializeLabels</c> au premier appel
        /// (déclenché par le constructeur du <c>VM_MHxx</c>
        /// concret final, conformément à R-4.11.8 du 0231) et à
        /// chaque changement d'<c>AppCultureCode</c> via
        /// l'abonnement INPC porté par
        /// <see cref="VM_Generic"/>.</para>
        ///
        /// <para>Absence d'appel à
        /// <c>base.LoadLabels(callChain)</c> : L'implémentation
        /// par défaut de <c>VM_Generic.LoadLabels</c> ne porte
        /// aucun traitement. L'appel à
        /// <c>base.LoadLabels(callChain)</c> n'apporterait qu'un
        /// bruit inutile et est délibérément omis, conformément à
        /// la pratique standard d'override lorsque la base ne
        /// porte aucun traitement, alignée sur le patron de
        /// <c>VM_Page98.LoadLabels</c> et de
        /// <c>VM_Page99.LoadLabels</c> documenté à §4.15.6 du
        /// 0230.</para>
        ///
        /// <para>Patron de surcharge à un VM_MHxx exposant des
        /// libellés propres : Un dérivé concret qui exposerait
        /// des libellés propres en sus des quatre libellés
        /// transverses surcharge à son tour
        /// <see cref="LoadLabels"/> et appelle obligatoirement
        /// <c>base.LoadLabels(callChain)</c> en première
        /// instruction du corps, afin de préserver l'alimentation
        /// des quatre <c>Label_MH_*</c> portée par la présente
        /// classe. L'omission de cet appel aurait pour
        /// conséquence des libellés transverses non chargés
        /// (chaînes vides), ce qui constituerait une régression
        /// visuelle directe. Cette obligation d'appel à
        /// <c>base.LoadLabels(callChain)</c> est la conséquence
        /// directe du fait que la base
        /// <see cref="VM_MH_Generic"/>.<see cref="LoadLabels"/>
        /// porte désormais du traitement à préserver,
        /// conformément à §3.14.4 du 0230.</para>
        ///
        /// <para>Asymétrie doctrinale avec la famille Page
        /// (assumée) : Côté Page, <c>LoadLabels</c> est
        /// surchargée pour la première fois au niveau du concret
        /// <c>VM_PageXX</c> (la base
        /// <c>VM_Generic.LoadLabels</c> est vide, l'appel à
        /// <c>base.LoadLabels(callChain)</c> est délibérément
        /// omis dans <c>VM_Page98.LoadLabels</c> et
        /// <c>VM_Page99.LoadLabels</c>). Côté MH,
        /// <c>LoadLabels</c> est surchargée pour la première fois
        /// au niveau du générique <see cref="VM_MH_Generic"/>
        /// (chargement des quatre libellés transverses) ; un
        /// éventuel <c>VM_MHxx</c> riche surchargeant à son tour
        /// <c>LoadLabels</c> doit appeler
        /// <c>base.LoadLabels(callChain)</c> en première
        /// instruction. Cette asymétrie est inévitable et
        /// doctrinalement justifiée par le rôle de factorisation
        /// transverse confié au socle de la famille MH ; elle se
        /// manifeste exclusivement au niveau interne du pattern
        /// d'override (cf. §4.15.8 du 0230).</para>
        ///
        /// <para>Clés legacy MH_Ti_NN : Les quatre clés legacy
        /// sont conservées intactes au dictionnaire pour préserver
        /// le référentiel multilingue existant ; le nommage
        /// fonctionnel des propriétés (<c>Label_MH_Menu</c>
        /// plutôt que <c>Label_MH_01</c>, etc.) est en revanche
        /// substitué à la nomenclature numérique afin de favoriser
        /// l'auto-documentation des bindings XAML et la cohérence
        /// avec la nomenclature des commandes
        /// (<c>MenuCommand</c>, <c>HomeCommand</c>,
        /// <c>PreviousCommand</c>, <c>RefreshCommand</c>).</para>
        ///
        /// <para>Filet de sécurité : Aucun <c>try</c>/<c>catch</c>
        /// local n'est posé. Le filet est porté exclusivement par
        /// <c>SR_Dictionary</c> conformément à R-4.11.6 et
        /// R-4.11.10 du 0231 — toute anomalie (clé absente, erreur
        /// inattendue) est journalisée en interne par
        /// <c>SR_Dictionary</c> et résolue par une valeur de repli
        /// <c>[MH_Ti_NN] not found</c>, sans interruption ni
        /// propagation d'exception au présent
        /// ViewModel.</para>
        /// </remarks>
        protected override void LoadLabels(string callChain)
        {
            Label_MH_Menu = _dictionary.GetText(callChain, "MH_Ti_01");
            Label_MH_Previous = _dictionary.GetText(callChain, "MH_Ti_02");
            Label_MH_Refresh = _dictionary.GetText(callChain, "MH_Ti_03");
            Label_MH_Home = _dictionary.GetText(callChain, "MH_Ti_04");
        }

        /// <summary>
        /// Marque le début d'une opération transverse en
        /// positionnant <see cref="IsProcessing"/> à
        /// <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée en première
        /// instruction de chacun des cinq handlers privés
        /// internes (<see cref="ExecuteMenuAsync"/>,
        /// <see cref="ExecuteReduceAsync"/>,
        /// <see cref="ExecuteHomeAsync"/>,
        /// <see cref="ExecutePreviousAsync"/>,
        /// <see cref="ExecuteRefreshAsync"/>), encadrée par un
        /// <c>try</c>/<c>finally</c> garantissant l'appel
        /// symétrique à <see cref="EndProcessing"/> en
        /// clôture.</para>
        /// <para>Caractère <c>virtual</c> : Les dérivés peuvent
        /// surcharger ce helper pour enrichir le comportement,
        /// par exemple pour incrémenter un compteur d'occupation
        /// imbriquée ou émettre une notification applicative
        /// supplémentaire. Tout override doit appeler
        /// <c>base.BeginProcessing()</c> en première instruction
        /// pour préserver la mécanique de base.</para>
        /// </remarks>
        protected virtual void BeginProcessing()
        {
            IsProcessing = true;
        }

        /// <summary>
        /// Marque la fin d'une opération transverse en
        /// repositionnant <see cref="IsProcessing"/> à
        /// <see langword="false"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée dans le bloc
        /// <c>finally</c> de chacun des cinq handlers privés
        /// internes, garantissant que <see cref="IsProcessing"/>
        /// est systématiquement remis à <see langword="false"/>
        /// à l'issue du traitement, y compris en cas d'exception
        /// capturée par <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>.</para>
        /// <para>Caractère <c>virtual</c> : Voir
        /// <see cref="BeginProcessing"/>. Tout override doit
        /// appeler <c>base.EndProcessing()</c> en dernière
        /// instruction pour préserver la mécanique de base.</para>
        /// </remarks>
        protected virtual void EndProcessing()
        {
            IsProcessing = false;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de la commande <see cref="MenuCommand"/>.
        /// Délègue à
        /// <see cref="IU_Navigation.ExpendHorizontalMenuAsync"/>
        /// via le filet <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>, encadré par
        /// <see cref="BeginProcessing"/> /
        /// <see cref="EndProcessing"/>.
        /// </summary>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// du déploiement du menu horizontal.</returns>
        /// <remarks>
        /// <para>Pattern uniforme : Le corps de ce handler est
        /// strictement identique à celui des quatre autres
        /// handlers (<see cref="ExecuteReduceAsync"/>,
        /// <see cref="ExecuteHomeAsync"/>,
        /// <see cref="ExecutePreviousAsync"/>,
        /// <see cref="ExecuteRefreshAsync"/>), seul change
        /// l'appel à la méthode de <see cref="IU_Navigation"/>.
        /// La répétition littérale est délibérée et préférée à
        /// une factorisation par délégué <c>Func&lt;...&gt;</c>,
        /// qui obscurcirait la CallChain lue dans les logs
        /// (chaque handler doit apparaître nominalement dans la
        /// CallChain pour la traçabilité).</para>
        /// </remarks>
        private async Task ExecuteMenuAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.ExpendHorizontalMenuAsync(callChain);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="ReduceCommand"/>.
        /// Délègue à
        /// <see cref="IU_Navigation.ReduceHorizontalMenuAsync"/>
        /// via le filet <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>.
        /// </summary>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// de la réduction du menu horizontal.</returns>
        private async Task ExecuteReduceAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.ReduceHorizontalMenuAsync(callChain);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="HomeCommand"/>.
        /// Délègue à
        /// <see cref="IU_Navigation.NavigateToDefaultAsync"/> via
        /// le filet <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>.
        /// </summary>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// de la navigation vers la page d'accueil.</returns>
        private async Task ExecuteHomeAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.NavigateToDefaultAsync(callChain);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="PreviousCommand"/>.
        /// Délègue à
        /// <see cref="IU_Navigation.NavigateToPreviousPageAsync"/>
        /// via le filet <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>.
        /// </summary>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// de la navigation arrière.</returns>
        private async Task ExecutePreviousAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.NavigateToPreviousPageAsync(callChain);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="RefreshCommand"/>.
        /// Délègue à
        /// <see cref="IU_Navigation.RefreshCurrentPageAsync"/> via
        /// le filet <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>.
        /// </summary>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// du rafraîchissement de la page courante.</returns>
        private async Task ExecuteRefreshAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.RefreshCurrentPageAsync(callChain);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        #endregion
    }
}