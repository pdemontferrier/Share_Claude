using System.Collections.ObjectModel;
using System.ComponentModel;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page Utilisateur <c>Page01</c> de l'application
    /// DG244Cutting, exposant à la vue les informations d'identification
    /// de l'utilisateur applicatif et du poste courant, les libellés
    /// multilingues des deux onglets de la page, et la liste des droits
    /// d'accès de l'utilisateur courant aux pages de l'application,
    /// chargés à l'entrée sur la page via un Query Handler générique
    /// invoqué selon l'EA-11.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>.
    /// La page est accessible à tout utilisateur connecté et n'expose
    /// aucune commande utilisateur ; elle présente, dans deux onglets,
    /// les informations d'identification de l'utilisateur applicatif et
    /// du poste courant (Onglet 1) et la liste des droits d'accès
    /// granulaires de l'utilisateur courant aux pages de l'application
    /// (Onglet 2). La sortie s'effectue exclusivement via les boutons
    /// transverses du menu horizontal portés par le couple
    /// <c>VM_MH_Generic</c> / <c>MH_Generic</c>.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/> :</para>
    /// <list type="bullet">
    ///   <item><description>18 propriétés observables
    ///   <c>Label_P01_NN</c> liées aux clés homonymes du dictionnaire
    ///   actif : <see cref="Label_P01_01"/> à <see cref="Label_P01_05"/>
    ///   pour les cinq titres de l'Onglet 1,
    ///   <see cref="Label_P01_10"/> et <see cref="Label_P01_11"/> pour
    ///   les deux en-têtes de TabItem, <see cref="Label_P01_13"/> à
    ///   <see cref="Label_P01_23"/> pour les onze en-têtes de colonnes
    ///   de l'Onglet 2. Toutes ces propriétés sont alimentées par la
    ///   mécanique multilingue factorisée par <see cref="VM_Generic"/> :
    ///   premier chargement au constructeur via
    ///   <see cref="VM_Generic.InitializeLabels"/>, rechargement
    ///   automatique à tout changement de langue dynamique par le
    ///   handler interne d'abonnement INPC à
    ///   <see cref="ISE_App.AppCultureCode"/> de l'ancêtre commun,
    ///   conformément à §4.11.5 du 0230 et à R-4.11.9 du 0231.</description></item>
    ///   <item><description>5 propriétés observables de données
    ///   d'identification : <see cref="UserId"/>,
    ///   <see cref="UserFullName"/>, <see cref="DeviceUser"/>,
    ///   <see cref="DeviceId"/>, <see cref="DeviceIP"/>. Ces propriétés
    ///   ne sont pas des libellés multilingues : leur alimentation est
    ///   portée par <see cref="LoadAsync"/>, qui projette en un seul
    ///   appel les valeurs de l'instantané de contexte applicatif et
    ///   utilisateur retourné par
    ///   <see cref="IS_AppContext.GetAppContext"/>.</description></item>
    ///   <item><description>1 propriété observable de collection
    ///   <see cref="PagesUserRights"/> de type
    ///   <see cref="ObservableCollection{T}"/> de
    ///   <see cref="UserAppPageRight"/>, exposant la liste des droits
    ///   d'accès granulaires de l'utilisateur courant aux pages de
    ///   l'application courante. Cette propriété est alimentée par
    ///   <see cref="LoadAsync"/> par invocation d'un Query Handler
    ///   générique <see cref="IQ_Generic{T}"/> via
    ///   <see cref="IS_UseCaseInvoker"/> (EA-11), filtrée par le
    ///   prédicat triple
    ///   <c>IdUser == AppUserId &amp;&amp; IdApplication == AppId
    ///   &amp;&amp; !IsDeleted</c>, triée par ordre alphabétique
    ///   ascendant de <see cref="UserAppPageRight.PageCode"/>.</description></item>
    /// </list>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer les 18 propriétés observables
    ///   <c>Label_P01_NN</c>, les 5 propriétés observables data et la
    ///   propriété observable <see cref="PagesUserRights"/> en accès
    ///   public en lecture, écriture privée via le helper hérité
    ///   <c>SetProperty&lt;T&gt;</c> (pour les 23 propriétés scalaires)
    ///   ou mutation en place via <c>Clear()</c> / <c>Add(...)</c>
    ///   (pour la collection observable).</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Generic.LoadLabels"/> pour résoudre les 18 clés
    ///   <c>P01_01</c> à <c>P01_05</c>, <c>P01_10</c>, <c>P01_11</c>,
    ///   <c>P01_13</c> à <c>P01_23</c> via
    ///   <see cref="VM_Generic._dictionary"/> hérité et affecter les
    ///   valeurs résolues aux 18 propriétés <c>Label_P01_NN</c>,
    ///   conformément à R-4.11.8 du 0231.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Page_Generic.LoadAsync"/> pour charger les 5
    ///   données d'identification depuis un instantané de
    ///   <see cref="IS_AppContext"/> et la collection
    ///   <see cref="PagesUserRights"/> par invocation d'un Query
    ///   Handler générique <see cref="IQ_Generic{T}"/> de
    ///   <see cref="UserAppPageRight"/> via
    ///   <see cref="IS_UseCaseInvoker"/>, en encapsulation par le filet
    ///   hérité <see cref="VM_Generic.ExecuteSafeAsync"/> (§4.7.3 du
    ///   0230). Le hook est invoqué depuis le code-behind de
    ///   <c>Page01</c> au point d'extension <c>OnLoadedAsync</c> exposé
    ///   par <c>Page_Generic</c>.</description></item>
    ///   <item><description>Déléguer à <see cref="VM_Generic"/> la
    ///   cérémonie multilingue complète (premier chargement,
    ///   abonnement INPC filtré sur
    ///   <see cref="ISE_App.AppCultureCode"/>, marshalling Dispatcher
    ///   défensif, rechargement) par l'unique appel à
    ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction du constructeur, conformément à I-4.11.11 et
    ///   R-4.11.8 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune logique métier ni règle
    ///   décisionnelle. La page est un rendu visuel d'informations
    ///   d'identification et de droits d'accès en lecture seule,
    ///   sans calcul, sans modification de l'état applicatif ni des
    ///   droits affichés (les cases à cocher de la <c>ListView</c>
    ///   sont en lecture seule côté Vue, cf.
    ///   <c>IsEnabled="False"</c>).</description></item>
    ///   <item><description>Aucune décision de navigation : la règle
    ///   R-4.12.2 du 0231 réserve la décision de navigation aux
    ///   UseCases. <see cref="VM_Page01"/> n'injecte ni
    ///   <c>IU_Navigation</c> ni <c>IS_Navigation</c>. La sortie de la
    ///   page est portée par les commandes transverses du menu
    ///   horizontal (Home, Previous), hors périmètre du présent
    ///   ViewModel.</description></item>
    ///   <item><description>Aucune commande utilisateur : la page est
    ///   non interactive ; aucun <c>ICommand</c> n'est exposé.</description></item>
    ///   <item><description>Aucune consommation directe d'un contrat
    ///   <c>IU_</c> ou <c>IQ_</c>, conformément à I-4.10.10 du 0231 :
    ///   le Query Handler générique de
    ///   <see cref="UserAppPageRight"/> est invoqué exclusivement via
    ///   <see cref="IS_UseCaseInvoker"/> au titre d'EA-11.</description></item>
    ///   <item><description>Aucun désabonnement explicite ni aucune
    ///   cérémonie multilingue locale : l'abonnement INPC à
    ///   <see cref="ISE_App"/> est branché par
    ///   <see cref="VM_Generic.InitializeLabels"/> et porté par le
    ///   handler interne de l'ancêtre commun, conformément à I-4.11.11
    ///   du 0231 ; aucun désabonnement n'est requis du dérivé. La
    ///   P4-bis (§4.10.10 du 0230) garantit par ailleurs la
    ///   libération naturelle de l'abonnement à l'arrêt de
    ///   l'application.</description></item>
    ///   <item><description>Aucun champ propre ni handler propre lié à
    ///   <see cref="ISE_App"/> : l'encapsulation de la dépendance est
    ///   intégralement portée par <see cref="VM_Generic"/> en champ
    ///   <c>private</c> non hérité (I-4.11.11 du 0231) ; le présent
    ///   dérivé n'accède jamais directement à
    ///   <see cref="ISE_App"/>.</description></item>
    ///   <item><description>Aucune logique locale de fallback en cas
    ///   de clé absente du dictionnaire ni try/catch local dans
    ///   <see cref="LoadLabels"/> : la logique de repli est portée
    ///   exclusivement par <c>SR_Dictionary</c> conformément à
    ///   R-4.11.6 et R-4.11.10 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : Aucune exception
    /// architecturale propre n'est portée par <see cref="VM_Page01"/>.
    /// L'injection de <see cref="ISE_App"/> au constructeur de la base
    /// relève exclusivement de la mécanique multilingue factorisée par
    /// <see cref="VM_Generic"/> (§4.15.5 du 0230, R-4.11.9 du 0231) et
    /// n'est pas une dérogation propre au présent dérivé : aucune
    /// cérémonie multilingue locale n'est portée par
    /// <see cref="VM_Page01"/>, conformément à I-4.11.11 du 0231.
    /// L'injection directe de <see cref="IU_LogAndNotify"/> par le
    /// ViewModel relève de l'exception architecturale propre du socle
    /// <see cref="VM_Generic"/> (EA-01, §4.15.5 du 0230), héritée et
    /// non re-déclarée à ce niveau ; elle est mobilisée par
    /// <see cref="LoadAsync"/> qui encapsule son invocation par le
    /// filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>.
    /// L'injection de <see cref="IS_AppContext"/> au constructeur est
    /// nominale au titre du Service applicatif sans portée d'EA propre.
    /// L'injection de <see cref="IS_UseCaseInvoker"/> est nominale au
    /// titre du mode d'invocation depuis <c>D_Presentation</c> posé en
    /// §4.10.10 du 0230 : les ViewModels invoquent les contrats
    /// <c>IU_</c> et <c>IQ_</c> via <see cref="IS_UseCaseInvoker"/> qui
    /// matérialise un <c>IServiceScope</c> distinct à chaque
    /// invocation. EA-11 (« Composant créateur de scope DI par
    /// invocation ») est portée exclusivement par
    /// <c>SR_UseCaseInvoker</c> ; le présent ViewModel en est
    /// consommateur et non porteur.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par deux extensions (R-4.4.10 du
    /// 0231) : l'extension <c>=== Propriétés publiques ===</c> au titre
    /// des 24 propriétés observables exposées, et l'extension
    /// <c>=== Méthodes protégées ===</c> au titre de l'override
    /// <see cref="LoadLabels"/>. Soit sept régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   23 champs supports des propriétés observables scalaires (18
    ///   champs supports de libellés <c>_label_p01_NN</c> et 5 champs
    ///   supports data <c>_userId</c>, <c>_userFullName</c>,
    ///   <c>_deviceUser</c>, <c>_deviceId</c>, <c>_deviceIP</c>). La
    ///   propriété <see cref="PagesUserRights"/> est exposée sans champ
    ///   support séparé : sa déclaration combine la propriété
    ///   <c>{ get; }</c> en lecture seule et l'instanciation
    ///   <c>= new();</c> en place, patron idiomatique des collections
    ///   observables exposées en lecture seule et mutées par
    ///   <c>Clear</c>/<c>Add</c>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   2 champs <c>private readonly</c> stockant les dépendances
    ///   propres au dérivé, affectés au constructeur après les gardes
    ///   <see cref="ArgumentNullException"/> : <c>_appContext</c>
    ///   (<see cref="IS_AppContext"/>) et <c>_useCaseInvoker</c>
    ///   (<see cref="IS_UseCaseInvoker"/>).</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c>
    ///   (extension §4.4.3) : 24 propriétés observables exposées en
    ///   accès public en lecture, écriture privée via
    ///   <c>SetProperty&lt;T&gt;</c> pour les 23 propriétés scalaires,
    ///   accès <c>{ get; }</c> en lecture seule avec instanciation en
    ///   place pour la collection observable.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   <c>public</c> à cinq paramètres, délégation à
    ///   <see cref="VM_Page_Generic"/> via
    ///   <c>base(dictionary, logAndNotify, app)</c>, gardes
    ///   <see cref="ArgumentNullException"/> locales sur les deux
    ///   dépendances propres, invocation finale de
    ///   <see cref="VM_Generic.InitializeLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> : override
    ///   <see cref="LoadAsync"/> selon patron normatif §4.15.6 du 0230
    ///   à trois constituants (<see cref="VM_Generic.BuildFirstCallChain"/>
    ///   interne, <see cref="VM_Generic.ExecuteSafeAsync"/>, propagation
    ///   du <see cref="System.Threading.CancellationToken"/>).</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c>
    ///   (extension §4.4.3) : override <see cref="LoadLabels"/>
    ///   peuplant les 18 propriétés <c>Label_P01_NN</c> via
    ///   <see cref="VM_Generic._dictionary"/>, une affectation par
    ///   ligne dans l'ordre numérique croissant des clés, sans appel à
    ///   <c>base.LoadLabels(callChain)</c>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente : <see cref="VM_Page01"/> n'expose aucun
    /// événement propre, l'événement <c>PropertyChanged</c> étant porté
    /// par <see cref="VM_Generic"/> au titre d'INPC et hérité par
    /// transitivité.</para>
    /// </remarks>
    public class VM_Page01 : VM_Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>Champ support de <see cref="Label_P01_01"/> (clé <c>P01_01</c>).</summary>
        private string _label_p01_01 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_02"/> (clé <c>P01_02</c>).</summary>
        private string _label_p01_02 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_03"/> (clé <c>P01_03</c>).</summary>
        private string _label_p01_03 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_04"/> (clé <c>P01_04</c>).</summary>
        private string _label_p01_04 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_05"/> (clé <c>P01_05</c>).</summary>
        private string _label_p01_05 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_10"/> (clé <c>P01_10</c>).</summary>
        private string _label_p01_10 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_11"/> (clé <c>P01_11</c>).</summary>
        private string _label_p01_11 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_13"/> (clé <c>P01_13</c>).</summary>
        private string _label_p01_13 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_14"/> (clé <c>P01_14</c>).</summary>
        private string _label_p01_14 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_15"/> (clé <c>P01_15</c>).</summary>
        private string _label_p01_15 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_16"/> (clé <c>P01_16</c>).</summary>
        private string _label_p01_16 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_17"/> (clé <c>P01_17</c>).</summary>
        private string _label_p01_17 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_18"/> (clé <c>P01_18</c>).</summary>
        private string _label_p01_18 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_19"/> (clé <c>P01_19</c>).</summary>
        private string _label_p01_19 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_20"/> (clé <c>P01_20</c>).</summary>
        private string _label_p01_20 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_21"/> (clé <c>P01_21</c>).</summary>
        private string _label_p01_21 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_22"/> (clé <c>P01_22</c>).</summary>
        private string _label_p01_22 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P01_23"/> (clé <c>P01_23</c>).</summary>
        private string _label_p01_23 = string.Empty;

        /// <summary>Champ support de <see cref="UserId"/>.</summary>
        private int _userId = 0;

        /// <summary>Champ support de <see cref="UserFullName"/>.</summary>
        private string? _userFullName = null;

        /// <summary>Champ support de <see cref="DeviceUser"/>.</summary>
        private string? _deviceUser = null;

        /// <summary>Champ support de <see cref="DeviceId"/>.</summary>
        private string? _deviceId = null;

        /// <summary>Champ support de <see cref="DeviceIP"/>.</summary>
        private string? _deviceIP = null;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service applicatif Singleton fournissant un instantané
        /// (<c>DTO_AppContext</c>) du contexte applicatif et utilisateur
        /// courant, agrégé à partir des Settings applicatif et
        /// utilisateur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur. <see cref="IS_AppContext"/> est un Service
        /// applicatif sans portée d'exception architecturale propre :
        /// il agrège les valeurs courantes des Settings dans un
        /// <c>DTO_AppContext</c> sans logique métier ni accès aux
        /// données. La consommation par le présent ViewModel se fait
        /// exclusivement dans <see cref="LoadAsync"/> par un appel
        /// unique à <see cref="IS_AppContext.GetAppContext"/>
        /// produisant un instantané atomique consommé pour
        /// l'alimentation des cinq propriétés data
        /// (<see cref="UserId"/>, <see cref="UserFullName"/>,
        /// <see cref="DeviceUser"/>, <see cref="DeviceId"/>,
        /// <see cref="DeviceIP"/>) et pour la composante du prédicat
        /// triple du filtrage de la collection
        /// <see cref="PagesUserRights"/>.</para>
        /// </remarks>
        private readonly IS_AppContext _appContext;

        /// <summary>
        /// Composant Singleton porteur de l'exception architecturale
        /// EA-11 (« Composant créateur de scope DI par invocation »,
        /// §4.10.10 et §4.15.10 du 0230, §17.4 du 0231), unique voie
        /// d'invocation des UseCases (<c>IU_</c>) et Query Handlers
        /// (<c>IQ_</c>) depuis un composant de
        /// <c>D_Presentation</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur, conformément au mode d'invocation depuis
        /// <c>D_Presentation</c> posé en §4.10.10 du 0230. À chaque
        /// invocation, <see cref="IS_UseCaseInvoker"/> matérialise un
        /// <c>IServiceScope</c> distinct, y résout le composant cible et
        /// l'exécute via le délégué fourni, puis dispose le scope. Le
        /// présent ViewModel est consommateur de
        /// <see cref="IS_UseCaseInvoker"/> et non porteur d'EA-11 :
        /// EA-11 est portée exclusivement par
        /// <c>SR_UseCaseInvoker</c>.</para>
        /// <para>Mode d'invocation strict : Le passage par
        /// <see cref="IS_UseCaseInvoker"/> est imposé par la lecture
        /// stricte du §4.10.10 du 0230 qui pose l'interdiction
        /// structurelle de l'injection directe d'un contrat <c>IU_</c>
        /// ou <c>IQ_</c> dans un composant de <c>D_Presentation</c>,
        /// indépendamment de toute question de captive dependency.
        /// Conformité I-4.10.10 du 0231. Le Query Handler générique
        /// <see cref="IQ_Generic{T}"/> de <see cref="UserAppPageRight"/>
        /// est invoqué dans <see cref="LoadAsync"/> via cette voie
        /// unique.</para>
        /// </remarks>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>Libellé multilingue associé à la clé <c>P01_01</c>, titre de la première donnée d'identification de l'Onglet 1 (Numéro Identifiant).</summary>
        public string Label_P01_01
        {
            get => _label_p01_01;
            private set => SetProperty(ref _label_p01_01, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_02</c>, titre de la deuxième donnée d'identification de l'Onglet 1 (Nom complet).</summary>
        public string Label_P01_02
        {
            get => _label_p01_02;
            private set => SetProperty(ref _label_p01_02, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_03</c>, titre de la troisième donnée d'identification de l'Onglet 1 (Windows ID).</summary>
        public string Label_P01_03
        {
            get => _label_p01_03;
            private set => SetProperty(ref _label_p01_03, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_04</c>, titre de la quatrième donnée d'identification de l'Onglet 1 (Poste ID).</summary>
        public string Label_P01_04
        {
            get => _label_p01_04;
            private set => SetProperty(ref _label_p01_04, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_05</c>, titre de la cinquième donnée d'identification de l'Onglet 1 (Poste IP).</summary>
        public string Label_P01_05
        {
            get => _label_p01_05;
            private set => SetProperty(ref _label_p01_05, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_10</c>, en-tête du premier TabItem (Onglet Utilisateur).</summary>
        public string Label_P01_10
        {
            get => _label_p01_10;
            private set => SetProperty(ref _label_p01_10, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_11</c>, en-tête du second TabItem (Onglet Droits d'accès).</summary>
        public string Label_P01_11
        {
            get => _label_p01_11;
            private set => SetProperty(ref _label_p01_11, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_13</c>, en-tête de la première colonne de la ListView de l'Onglet 2 (Page).</summary>
        public string Label_P01_13
        {
            get => _label_p01_13;
            private set => SetProperty(ref _label_p01_13, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_14</c>, en-tête de la deuxième colonne de la ListView de l'Onglet 2 (Accès).</summary>
        public string Label_P01_14
        {
            get => _label_p01_14;
            private set => SetProperty(ref _label_p01_14, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_15</c>, en-tête de la troisième colonne de la ListView de l'Onglet 2 (Lecture).</summary>
        public string Label_P01_15
        {
            get => _label_p01_15;
            private set => SetProperty(ref _label_p01_15, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_16</c>, en-tête de la quatrième colonne de la ListView de l'Onglet 2 (Modification).</summary>
        public string Label_P01_16
        {
            get => _label_p01_16;
            private set => SetProperty(ref _label_p01_16, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_17</c>, en-tête de la cinquième colonne de la ListView de l'Onglet 2 (Création).</summary>
        public string Label_P01_17
        {
            get => _label_p01_17;
            private set => SetProperty(ref _label_p01_17, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_18</c>, en-tête de la sixième colonne de la ListView de l'Onglet 2 (Suppression).</summary>
        public string Label_P01_18
        {
            get => _label_p01_18;
            private set => SetProperty(ref _label_p01_18, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_19</c>, en-tête de la septième colonne de la ListView de l'Onglet 2 (Contrôle).</summary>
        public string Label_P01_19
        {
            get => _label_p01_19;
            private set => SetProperty(ref _label_p01_19, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_20</c>, en-tête de la huitième colonne de la ListView de l'Onglet 2 (Validation).</summary>
        public string Label_P01_20
        {
            get => _label_p01_20;
            private set => SetProperty(ref _label_p01_20, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_21</c>, en-tête de la neuvième colonne de la ListView de l'Onglet 2 (Supervision).</summary>
        public string Label_P01_21
        {
            get => _label_p01_21;
            private set => SetProperty(ref _label_p01_21, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_22</c>, en-tête de la dixième colonne de la ListView de l'Onglet 2 (Monitoring).</summary>
        public string Label_P01_22
        {
            get => _label_p01_22;
            private set => SetProperty(ref _label_p01_22, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P01_23</c>, en-tête de la onzième colonne de la ListView de l'Onglet 2 (Administration).</summary>
        public string Label_P01_23
        {
            get => _label_p01_23;
            private set => SetProperty(ref _label_p01_23, value);
        }

        /// <summary>
        /// Identifiant applicatif de l'utilisateur courant, projeté
        /// depuis le champ <c>AppUserId</c> du
        /// <c>DTO_AppContext</c> retourné par
        /// <see cref="IS_AppContext.GetAppContext"/> au moment de
        /// l'appel à <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Entier strictement positif identifiant l'utilisateur
        /// applicatif courant dans l'écosystème ERP, ou <c>0</c> avant
        /// le premier appel à <see cref="LoadAsync"/>.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>
        /// (Onglet 1, <c>TextBlock</c> <c>IDData</c>). L'accesseur en
        /// écriture est privé : la valeur ne peut être modifiée qu'à
        /// travers l'override <see cref="LoadAsync"/>, invoqué par le
        /// code-behind de <c>Page01</c> au point d'extension
        /// <c>OnLoadedAsync</c> exposé par <c>Page_Generic</c> (§4.15.7
        /// du 0230).</para>
        /// <para>Cette propriété n'est pas affectée par
        /// <see cref="LoadLabels"/> (l'identifiant utilisateur n'est
        /// pas un libellé multilingue) et n'est pas rechargée par le
        /// handler interne d'abonnement INPC de
        /// <see cref="VM_Generic"/> (l'identifiant utilisateur ne
        /// dépend pas de la langue active).</para>
        /// </remarks>
        public int UserId
        {
            get => _userId;
            private set => SetProperty(ref _userId, value);
        }

        /// <summary>
        /// Nom complet de l'utilisateur applicatif courant, projeté
        /// depuis le champ <c>AppUserFullName</c> du
        /// <c>DTO_AppContext</c> retourné par
        /// <see cref="IS_AppContext.GetAppContext"/> au moment de
        /// l'appel à <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Chaîne représentant le nom complet de l'utilisateur
        /// applicatif courant, ou <see langword="null"/> avant le
        /// premier appel à <see cref="LoadAsync"/> ou si la donnée
        /// agrégée par le Setting utilisateur amont est elle-même
        /// <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>
        /// (Onglet 1, <c>TextBlock</c> <c>UserFullNameData</c>).
        /// L'accesseur en écriture est privé ; alimentation exclusive
        /// par <see cref="LoadAsync"/>.</para>
        /// </remarks>
        public string? UserFullName
        {
            get => _userFullName;
            private set => SetProperty(ref _userFullName, value);
        }

        /// <summary>
        /// Compte système du poste sur lequel l'application est
        /// exécutée, projeté depuis le champ <c>AppDeviceUser</c> du
        /// <c>DTO_AppContext</c> retourné par
        /// <see cref="IS_AppContext.GetAppContext"/> au moment de
        /// l'appel à <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Chaîne représentant le compte Windows local du poste, ou
        /// <see langword="null"/> avant le premier appel à
        /// <see cref="LoadAsync"/> ou si la donnée agrégée par le
        /// Setting amont est elle-même <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>
        /// (Onglet 1, <c>TextBlock</c> <c>DeviceUserData</c>).
        /// L'accesseur en écriture est privé ; alimentation exclusive
        /// par <see cref="LoadAsync"/>.</para>
        /// </remarks>
        public string? DeviceUser
        {
            get => _deviceUser;
            private set => SetProperty(ref _deviceUser, value);
        }

        /// <summary>
        /// Identifiant du poste sur lequel l'application est exécutée,
        /// projeté depuis le champ <c>AppDeviceId</c> du
        /// <c>DTO_AppContext</c> retourné par
        /// <see cref="IS_AppContext.GetAppContext"/> au moment de
        /// l'appel à <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Chaîne représentant le nom machine du poste, ou
        /// <see langword="null"/> avant le premier appel à
        /// <see cref="LoadAsync"/> ou si la donnée agrégée par le
        /// Setting amont est elle-même <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>
        /// (Onglet 1, <c>TextBlock</c> <c>DeviceIdData</c>).
        /// L'accesseur en écriture est privé ; alimentation exclusive
        /// par <see cref="LoadAsync"/>.</para>
        /// </remarks>
        public string? DeviceId
        {
            get => _deviceId;
            private set => SetProperty(ref _deviceId, value);
        }

        /// <summary>
        /// Adresse IP du poste sur lequel l'application est exécutée,
        /// projetée depuis le champ <c>AppDeviceIP</c> du
        /// <c>DTO_AppContext</c> retourné par
        /// <see cref="IS_AppContext.GetAppContext"/> au moment de
        /// l'appel à <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Chaîne représentant l'adresse IP du poste, ou
        /// <see langword="null"/> avant le premier appel à
        /// <see cref="LoadAsync"/> ou si la donnée agrégée par le
        /// Setting amont est elle-même <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>
        /// (Onglet 1, <c>TextBlock</c> <c>DeviceIPData</c>).
        /// L'accesseur en écriture est privé ; alimentation exclusive
        /// par <see cref="LoadAsync"/>.</para>
        /// </remarks>
        public string? DeviceIP
        {
            get => _deviceIP;
            private set => SetProperty(ref _deviceIP, value);
        }

        /// <summary>
        /// Collection observable des droits d'accès granulaires de
        /// l'utilisateur courant aux pages de l'application courante,
        /// triée par ordre alphabétique ascendant de
        /// <see cref="UserAppPageRight.PageCode"/>.
        /// </summary>
        /// <value>
        /// Collection observable de <see cref="UserAppPageRight"/>
        /// instanciée à la construction du présent ViewModel à une
        /// collection vide, puis alimentée par <see cref="LoadAsync"/>
        /// par <c>Clear()</c> suivi d'autant d'<c>Add(...)</c> que
        /// nécessaire. La référence de collection n'est jamais
        /// réaffectée, conformément au patron idiomatique des
        /// collections observables exposées en lecture seule par les
        /// ViewModels WPF.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>
        /// (Onglet 2, <c>ListView</c> <c>UserAccessListView</c>,
        /// attribut <c>ItemsSource="{Binding PagesUserRights}"</c>).
        /// L'<c>ItemTemplate</c> de la <c>ListView</c> consomme par
        /// binding les propriétés <see cref="UserAppPageRight.PageCode"/>,
        /// <see cref="UserAppPageRight.CanAccess"/>,
        /// <see cref="UserAppPageRight.CanRead"/>,
        /// <see cref="UserAppPageRight.CanUpdate"/>,
        /// <see cref="UserAppPageRight.CanCreate"/>,
        /// <see cref="UserAppPageRight.CanDelete"/>,
        /// <see cref="UserAppPageRight.CanControl"/>,
        /// <see cref="UserAppPageRight.CanValidate"/>,
        /// <see cref="UserAppPageRight.CanSupervise"/>,
        /// <see cref="UserAppPageRight.CanMonitor"/> et
        /// <see cref="UserAppPageRight.CanAdmin"/>.</para>
        /// <para>Exposition directe de l'entité de domaine
        /// (arbitrage Q-VM-1 : A) : la collection expose directement
        /// les entités <see cref="UserAppPageRight"/> retournées par
        /// le Query Handler générique, sans projection en DTO
        /// intermédiaire. Choix doctrinal admissible au présent stade
        /// du projet ; toute évolution vers un DTO de projection
        /// relèverait d'un mode Refactoring distinct.</para>
        /// <para>Dérogation au patron <c>SetProperty&lt;T&gt;</c> de
        /// VM-P9 : La propriété est exposée <c>{ get; }</c> en lecture
        /// seule avec instanciation en place via <c>= new();</c>, sans
        /// champ support séparé ni accesseur en écriture privée. Ce
        /// patron idiomatique des collections observables WPF est
        /// admissible au titre de la portée scalaire de VM-P9 (qui
        /// adresse les propriétés observables scalaires) ; la
        /// notification INPC des éléments ajoutés ou retirés est
        /// portée par <see cref="ObservableCollection{T}"/> elle-même
        /// au titre d'<see cref="INotifyCollectionChanged"/>.</para>
        /// <para>Alimentation : Exclusivement par
        /// <see cref="LoadAsync"/> via <c>Clear()</c> suivi d'autant
        /// d'<c>Add(...)</c> que nécessaire, après tri ascendant des
        /// résultats du Query Handler par
        /// <see cref="UserAppPageRight.PageCode"/>. Cette propriété
        /// n'est pas affectée par <see cref="LoadLabels"/> (les droits
        /// d'accès ne sont pas des libellés multilingues) et n'est pas
        /// rechargée par le handler interne d'abonnement INPC de
        /// <see cref="VM_Generic"/> (les droits d'accès ne dépendent
        /// pas de la langue active).</para>
        /// </remarks>
        public ObservableCollection<UserAppPageRight> PagesUserRights { get; } = new();

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Page01"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur DI
        /// lors de la résolution du Singleton <see cref="VM_Page01"/>
        /// par la vue <c>Page01</c> via
        /// <c>App.ServiceProvider.GetRequiredService</c> dans son propre
        /// constructeur (EA-02 Service Locator de
        /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>,
        /// étendue aux dérivés directs pour la résolution de leur
        /// ViewModel — cf. §4.15.7 du 0230).</para>
        ///
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Délégation à
        ///   <see cref="VM_Page_Generic"/> via
        ///   <c>base(dictionary, logAndNotify, app)</c> en première
        ///   instruction. La chaîne <c>base(...)</c> remonte à
        ///   <see cref="VM_Generic"/> qui applique les trois gardes
        ///   <see cref="ArgumentNullException"/> sur les trois
        ///   paramètres, stocke <paramref name="dictionary"/> et
        ///   <paramref name="logAndNotify"/> en champs
        ///   <c>protected</c> (<see cref="VM_Generic._dictionary"/>,
        ///   <see cref="VM_Generic._logAndNotify"/>) accessibles aux
        ///   dérivés, stocke <paramref name="app"/> en champ
        ///   <c>private</c> non hérité (encapsulation de la mécanique
        ///   multilingue, conformément à I-4.11.11 du 0231), et
        ///   initialise le champ <c>_callee</c> via
        ///   <c>GetType().Name</c>.</description></item>
        ///   <item><description>Gardes
        ///   <see cref="ArgumentNullException"/> locales sur les deux
        ///   dépendances propres au dérivé
        ///   (<paramref name="appContext"/>,
        ///   <paramref name="useCaseInvoker"/>) et affectation aux
        ///   champs <c>_appContext</c> et
        ///   <c>_useCaseInvoker</c>.</description></item>
        ///   <item><description>Appel à
        ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
        ///   instruction du corps. Ce hook explicite orchestre la
        ///   séquence normative en trois temps : construction d'une
        ///   CallChain initiale via
        ///   <see cref="VM_Generic.BuildFirstCallChain"/>, premier
        ///   appel synchrone à l'override <see cref="LoadLabels"/>
        ///   peuplant les 18 propriétés <c>Label_P01_NN</c> avant le
        ///   premier binding WPF de la vue, et branchement de
        ///   l'abonnement INPC interne à <see cref="ISE_App"/> pour la
        ///   prise en compte du changement de langue dynamique
        ///   (R-4.11.8 et R-4.11.9 du 0231).</description></item>
        /// </list>
        ///
        /// <para>Règle d'invocation d'<c>InitializeLabels</c>
        /// (R-4.11.8 du 0231) : L'appel à
        /// <see cref="VM_Generic.InitializeLabels"/> est exclusivement
        /// effectué dans le constructeur du ViewModel dérivé concret
        /// final, en dernière instruction, après l'affectation de
        /// toutes les dépendances propres. Cette règle prévient
        /// l'écueil classique de l'invocation virtuelle dans le
        /// constructeur d'une classe de base avec dépendances dérivées
        /// non encore initialisées.</para>
        ///
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le
        /// constructeur au-delà des gardes
        /// <see cref="ArgumentNullException"/>. Toute défaillance de
        /// l'invocation virtuelle de <see cref="LoadLabels"/> par
        /// <see cref="VM_Generic.InitializeLabels"/> est absorbée par
        /// le filet hérité du socle multilingue ; le constructeur est
        /// donc inconditionnellement terminant en l'absence de
        /// paramètres null.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c>. Injecté en Singleton par le conteneur DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_Page_Generic"/> via <c>base(...)</c>. Mobilisé
        /// par le filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// au sein de <see cref="LoadAsync"/>. Injecté en Singleton par
        /// le conteneur DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par l'ancêtre commun (abonnement
        /// INPC interne à <see cref="ISE_App.AppCultureCode"/>). Le
        /// présent dérivé ne stocke pas cette dépendance ni n'y accède
        /// directement, conformément à I-4.11.11 du 0231. Injecté en
        /// Singleton par le conteneur DI.</param>
        /// <param name="appContext">Service applicatif Singleton
        /// fournissant un instantané (<c>DTO_AppContext</c>) du
        /// contexte applicatif et utilisateur courant. Mobilisé par
        /// <see cref="LoadAsync"/> pour l'alimentation des cinq
        /// propriétés data et pour la composante du prédicat triple
        /// du filtrage de <see cref="PagesUserRights"/>. Injecté en
        /// Singleton par le conteneur DI.</param>
        /// <param name="useCaseInvoker">Composant Singleton porteur
        /// d'EA-11, unique voie d'invocation du Query Handler
        /// générique <see cref="IQ_Generic{T}"/> de
        /// <see cref="UserAppPageRight"/> depuis le présent
        /// ViewModel. Injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="appContext"/> ou
        /// <paramref name="useCaseInvoker"/> est
        /// <see langword="null"/>. Les gardes sur
        /// <paramref name="dictionary"/>,
        /// <paramref name="logAndNotify"/> et <paramref name="app"/>
        /// sont portées par <see cref="VM_Generic"/> via la chaîne
        /// <c>base(...)</c>.</exception>
        public VM_Page01(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IS_AppContext appContext,
            IS_UseCaseInvoker useCaseInvoker)
            : base(dictionary, logAndNotify, app)
        {
            _appContext = appContext
                ?? throw new ArgumentNullException(nameof(appContext));
            _useCaseInvoker = useCaseInvoker
                ?? throw new ArgumentNullException(nameof(useCaseInvoker));

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Redéfinit le hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> pour charger les
        /// cinq données d'identification de l'utilisateur et du poste
        /// depuis un instantané de <see cref="IS_AppContext"/>, puis
        /// la collection <see cref="PagesUserRights"/> par invocation
        /// du Query Handler générique <see cref="IQ_Generic{T}"/> de
        /// <see cref="UserAppPageRight"/> via
        /// <see cref="IS_UseCaseInvoker"/> (EA-11).
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// l'orchestrateur appelant côté <c>Page_Generic</c> au
        /// format normatif
        /// <c>{_callee} &gt; OnLoadedHandler &gt; OnLoadedAsync</c>
        /// et propagée tel quel par le code-behind via
        /// <c>_viewModel.LoadAsync(callChain, ct)</c>. Le paramètre est
        /// reçu par contrat du hook au socle
        /// <see cref="VM_Page_Generic"/> mais n'est pas consommé par le
        /// corps du présent override : une CallChain interne distincte
        /// est construite via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> et consommée
        /// par <see cref="VM_Generic.ExecuteSafeAsync"/> et par le
        /// délégué d'invocation du Query Handler, conformément au
        /// patron de surcharge normatif §4.15.6 du 0230.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé par
        /// le code-behind appelant. Propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, à
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}(System.Func{TUseCase, System.Threading.CancellationToken, System.Threading.Tasks.Task{TResult}}, System.Threading.CancellationToken)"/>
        /// et, par le délégué, au Query Handler
        /// <see cref="IQ_Generic{T}.HandleGetFilteredAsNoTrackingAsync"/>.
        /// Valeur par défaut : <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement des cinq propriétés data et de la collection
        /// <see cref="PagesUserRights"/>.</returns>
        /// <remarks>
        /// <para>Contexte : Override du hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> déclaré
        /// <c>public virtual</c> au socle conformément à §4.15.6 du
        /// 0230. Invoquée depuis le code-behind de <c>Page01</c> au
        /// point d'extension <c>OnLoadedAsync</c> exposé par
        /// <c>Page_Generic</c> (§4.15.7 du 0230). Méthode strictement
        /// disjointe de <see cref="LoadLabels"/> : libellés synchrones
        /// au constructeur d'un côté, données fonctionnelles et
        /// collection observable asynchrones au <c>Loaded</c> de la
        /// page de l'autre.</para>
        ///
        /// <para>Objectif : Alimenter en quatre temps coordonnés
        /// les cinq propriétés data et la collection
        /// <see cref="PagesUserRights"/> :</para>
        /// <list type="number">
        ///   <item><description>Snapshot atomique du contexte
        ///   applicatif et utilisateur courant par un appel unique à
        ///   <see cref="IS_AppContext.GetAppContext"/>, retourné dans
        ///   une variable locale <c>ctx</c> consommée par les trois
        ///   temps suivants. La disciplinte d'appel unique garantit
        ///   la cohérence intra-méthode : les cinq propriétés data et
        ///   la composante du prédicat triple lisent toutes du même
        ///   instantané, sans risque de désynchronisation entre deux
        ///   lectures successives du
        ///   Setting.</description></item>
        ///   <item><description>Alimentation des cinq propriétés
        ///   <see cref="UserId"/>, <see cref="UserFullName"/>,
        ///   <see cref="DeviceUser"/>, <see cref="DeviceId"/> et
        ///   <see cref="DeviceIP"/> par projection directe des champs
        ///   homonymes du <c>DTO_AppContext</c>.</description></item>
        ///   <item><description>Invocation du Query Handler générique
        ///   <see cref="IQ_Generic{T}"/> de
        ///   <see cref="UserAppPageRight"/> via
        ///   <see cref="IS_UseCaseInvoker"/>, méthode
        ///   <see cref="IQ_Generic{T}.HandleGetFilteredAsNoTrackingAsync"/>,
        ///   avec prédicat triple
        ///   <c>r.IdUser == ctx.AppUserId
        ///   &amp;&amp; r.IdApplication == ctx.AppId
        ///   &amp;&amp; !r.IsDeleted</c> (arbitrage Q-VM-2 : C),
        ///   produisant une <c>List&lt;UserAppPageRight&gt;</c> non
        ///   suivie par EF Core (lecture pure, aucune mutation
        ///   subséquente des entités retournées).</description></item>
        ///   <item><description>Tri ascendant de la liste retournée
        ///   par <see cref="UserAppPageRight.PageCode"/>, puis
        ///   alimentation de la collection observable
        ///   <see cref="PagesUserRights"/> par <c>Clear()</c> suivi
        ///   d'autant d'<c>Add(...)</c> que nécessaire. La référence
        ///   de collection n'est jamais réaffectée, conformément au
        ///   patron idiomatique de mutation en place des collections
        ///   observables WPF.</description></item>
        /// </list>
        ///
        /// <para>Patron de surcharge normatif (§4.15.6 du 0230) :
        /// L'override construit une CallChain interne
        /// (<c>innerCallChain</c>) via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> hérité, plutôt
        /// que de consommer la CallChain reçue en paramètre. Le
        /// paramètre <paramref name="callChain"/> reçu du hook est
        /// utile à des fins de traçabilité amont mais la CallChain
        /// consommée par le filet et par le délégué d'invocation est
        /// celle reconstruite localement, garantissant que le format
        /// normatif <c>{_callee} &gt; LoadAsync</c> est appliqué pour
        /// l'opération elle-même.</para>
        ///
        /// <para>Idempotence : La méthode est ré-appelable à chaque
        /// entrée sur la page sans flag de mémoire d'état. Chaque
        /// appel produit une nouvelle invocation complète du Query
        /// Handler et une nouvelle alimentation de
        /// <see cref="PagesUserRights"/> par <c>Clear()</c> + boucle
        /// d'<c>Add(...)</c> — coût négligeable en pratique compte
        /// tenu de la cardinalité attendue des droits d'accès
        /// (typiquement quelques dizaines de lignes au maximum) ;
        /// stricte simplicité du contrat.</para>
        ///
        /// <para>Filet de sécurité : L'invocation est encapsulée par
        /// le filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// (§4.7.3 du 0230). Toute exception applicative typée non
        /// capturée par le pipeline du Query Handler amont est traitée
        /// terminalement par <see cref="IU_LogAndNotify"/> via le
        /// filet hérité. Le présent override ne pose aucun try/catch
        /// local : les défaillances métier
        /// (<c>Ex_Business</c>) et infrastructure
        /// (<c>Ex_Infrastructure</c>) éventuellement levées par le
        /// Query Handler sont absorbées par
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> selon le pipeline
        /// canonique à quatre captures (§4.7.3 du 0230) ; en cas de
        /// défaillance, les propriétés data déjà affectées restent
        /// telles quelles (état partiel admis) et la collection
        /// <see cref="PagesUserRights"/> reste dans son état antérieur
        /// (le <c>Clear()</c> et la boucle d'<c>Add(...)</c> ne sont
        /// pas atteints si l'exception est levée par
        /// l'invocation du Query Handler).</para>
        ///
        /// <para>Propagation du <see cref="System.Threading.CancellationToken"/> :
        /// Le jeton fourni par l'appelant est propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> et, par fermeture,
        /// à
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}(System.Func{TUseCase, System.Threading.CancellationToken, System.Threading.Tasks.Task{TResult}}, System.Threading.CancellationToken)"/>,
        /// puis au délégué d'invocation du Query Handler consommé. Le
        /// jeton effectif passé au Query Handler au sein du délégué
        /// est celui que <see cref="IS_UseCaseInvoker"/> transmet au
        /// délégué (paramètre <c>innerCt</c>), conformément à la
        /// signature canonique de §4.10.10 du 0230.</para>
        ///
        /// <para>Mode d'invocation : Conformément à §4.10.10 du 0230,
        /// l'invocation du Query Handler générique
        /// <see cref="IQ_Generic{T}"/> de
        /// <see cref="UserAppPageRight"/> est portée par
        /// <see cref="IS_UseCaseInvoker"/> qui matérialise un
        /// <c>IServiceScope</c> distinct pour l'invocation, y résout
        /// l'implémentation du contrat (<c>QH_Generic&lt;UserAppPageRight&gt;</c>)
        /// et l'exécute via le délégué fourni, puis dispose le scope.
        /// Le présent ViewModel n'injecte pas directement le contrat
        /// <see cref="IQ_Generic{T}"/>, conformément à I-4.10.10 du
        /// 0231.</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée silencieusement à l'appelant sur signal
        /// d'annulation coopérative par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, conformément à
        /// §4.7.3 du 0230. Aucune journalisation ni notification.
        /// </exception>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                var ctx = _appContext.GetAppContext();

                UserId = ctx.AppUserId;
                UserFullName = ctx.AppUserFullName;
                DeviceUser = ctx.AppDeviceUser;
                DeviceId = ctx.AppDeviceId;
                DeviceIP = ctx.AppDeviceIP;

                var rights = await _useCaseInvoker
                    .InvokeAsync<IQ_Generic<UserAppPageRight>, List<UserAppPageRight>>(
                        (handler, innerCt) => handler.HandleGetFilteredAsNoTrackingAsync(
                            innerCallChain,
                            r => r.IdUser == ctx.AppUserId
                                 && r.IdApplication == ctx.AppId
                                 && !r.IsDeleted,
                            innerCt),
                        ct);

                var sorted = rights.OrderBy(r => r.PageCode).ToList();
                PagesUserRights.Clear();
                foreach (var r in sorted) PagesUserRights.Add(r);
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> pour charger les 18
        /// libellés multilingues affichés par la page <c>Page01</c> à
        /// partir des clés <c>P01_01</c> à <c>P01_05</c>,
        /// <c>P01_10</c>, <c>P01_11</c> et <c>P01_13</c> à
        /// <c>P01_23</c> du dictionnaire de langue actif et les
        /// affecter aux 18 propriétés observables
        /// <see cref="Label_P01_01"/> à <see cref="Label_P01_23"/>.
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// (premier chargement) ou par le handler interne d'abonnement
        /// INPC de <see cref="VM_Generic"/> au changement de langue
        /// dynamique (rechargement), et transmise au service de
        /// dictionnaire pour traçabilité.</param>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissant le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> conformément à R-4.11.8
        /// du 0231. Invoquée par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// pour le premier chargement, puis par le handler interne
        /// d'abonnement INPC de <see cref="VM_Generic"/> à chaque
        /// changement de langue dynamique notifié par
        /// <see cref="ISE_App.AppCultureCode"/>, avec marshalling
        /// Dispatcher défensif vers le thread UI. Ne touche pas aux
        /// cinq propriétés data ni à la collection
        /// <see cref="PagesUserRights"/>, dont les alimentations sont
        /// portées par <see cref="LoadAsync"/>.</para>
        ///
        /// <para>Objectif : Garantir que les 18 propriétés
        /// <c>Label_P01_NN</c> sont synchronisées avec la langue
        /// active du dictionnaire, tant au moment de l'instanciation
        /// du ViewModel que lors de tout changement ultérieur de
        /// langue dynamique au cours de la session.</para>
        ///
        /// <para>Patron strict : Une affectation par ligne, dans
        /// l'ordre numérique croissant des clés (<c>P01_01</c>,
        /// <c>P01_02</c>, …, <c>P01_05</c>, puis <c>P01_10</c>,
        /// <c>P01_11</c>, puis <c>P01_13</c>, …, <c>P01_23</c>), sans
        /// regroupement et sans condition. Aucun raccourci de type
        /// boucle dynamique : la résolution nominative permet une
        /// revue de code aisée et un repérage statique des clés
        /// consommées. Les clés <c>P01_00</c> (nom de la page,
        /// convention transverse) et <c>P01_12</c> (libellé non
        /// consommé par la composition visuelle courante) ne sont
        /// volontairement pas chargées par le présent override : leur
        /// présence dans le dictionnaire est conservée à titre de
        /// matière première pour d'éventuelles évolutions
        /// ultérieures.</para>
        ///
        /// <para>Absence d'appel à <c>base.LoadLabels(callChain)</c> :
        /// L'implémentation par défaut de
        /// <see cref="VM_Generic.LoadLabels"/> ne porte aucun
        /// traitement. L'appel à <c>base.LoadLabels(callChain)</c>
        /// n'apporterait qu'un bruit inutile et est délibérément omis,
        /// conformément à la pratique standard d'override lorsque la
        /// base ne porte aucun traitement, alignée sur le patron de
        /// <c>VM_Page98.LoadLabels</c> et de
        /// <c>VM_Page99.LoadLabels</c>.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local n'est posé.
        /// Le filet est porté exclusivement par <c>SR_Dictionary</c>
        /// conformément à R-4.11.6 et R-4.11.10 du 0231 — toute
        /// anomalie (clé absente, erreur inattendue) est journalisée
        /// en interne par <c>SR_Dictionary</c> et résolue par une
        /// valeur de repli <c>[P01_NN] not found</c>, sans
        /// interruption ni propagation d'exception au présent
        /// ViewModel. L'unique exception susceptible d'être propagée
        /// serait <see cref="OperationCanceledException"/>,
        /// structurellement impossible ici puisque la signature de
        /// <c>IS_Dictionary.GetText</c> est invoquée sans
        /// <see cref="System.Threading.CancellationToken"/> explicite
        /// (paramètre optionnel par défaut <c>default</c>, équivalent
        /// à <see cref="System.Threading.CancellationToken.None"/>).</para>
        /// </remarks>
        protected override void LoadLabels(string callChain)
        {
            Label_P01_01 = _dictionary.GetText(callChain, "P01_01");
            Label_P01_02 = _dictionary.GetText(callChain, "P01_02");
            Label_P01_03 = _dictionary.GetText(callChain, "P01_03");
            Label_P01_04 = _dictionary.GetText(callChain, "P01_04");
            Label_P01_05 = _dictionary.GetText(callChain, "P01_05");
            Label_P01_10 = _dictionary.GetText(callChain, "P01_10");
            Label_P01_11 = _dictionary.GetText(callChain, "P01_11");
            Label_P01_13 = _dictionary.GetText(callChain, "P01_13");
            Label_P01_14 = _dictionary.GetText(callChain, "P01_14");
            Label_P01_15 = _dictionary.GetText(callChain, "P01_15");
            Label_P01_16 = _dictionary.GetText(callChain, "P01_16");
            Label_P01_17 = _dictionary.GetText(callChain, "P01_17");
            Label_P01_18 = _dictionary.GetText(callChain, "P01_18");
            Label_P01_19 = _dictionary.GetText(callChain, "P01_19");
            Label_P01_20 = _dictionary.GetText(callChain, "P01_20");
            Label_P01_21 = _dictionary.GetText(callChain, "P01_21");
            Label_P01_22 = _dictionary.GetText(callChain, "P01_22");
            Label_P01_23 = _dictionary.GetText(callChain, "P01_23");
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}