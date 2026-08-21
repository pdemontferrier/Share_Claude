using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Business;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page de consultation détaillée d'une série de
    /// production <c>Page11</c> de l'application DG244Cutting, exposant à
    /// la vue les libellés multilingues des cinq onglets de la page, les
    /// sept intitulés de la fiche de synthèse du premier onglet et les
    /// sept caractéristiques de la série désignée par le contexte de
    /// sélection applicatif, relues en base à l'entrée sur la page via un
    /// Query Handler générique invoqué selon l'EA-11.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>. La
    /// page est atteinte par un opérateur d'atelier de découpe depuis le
    /// tableau de bord des séries de production <c>Page10</c>, à
    /// l'ouverture d'une série, pour disposer d'un point de vue complet
    /// sur cette série et son avancement. Elle est strictement en
    /// lecture : elle n'écrit dans aucune entité et ne porte aucune
    /// action métier. La série concernée n'est pas passée en paramètre :
    /// elle est désignée par le contexte de sélection applicatif
    /// <see cref="ISE_UseCase"/>, alimenté au clic sur la série dans le
    /// tableau de bord. Aucune saisie n'est attendue de l'opérateur ; la
    /// sortie de page relève des boutons transverses du menu horizontal
    /// <c>MH11</c>, hors périmètre du présent ViewModel.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/> :</para>
    /// <list type="bullet">
    ///   <item><description>12 propriétés observables
    ///   <c>Label_P11_NN</c> liées aux clés homonymes du dictionnaire
    ///   actif : <see cref="Label_P11_01"/> à <see cref="Label_P11_05"/>
    ///   pour les cinq en-têtes d'onglets (série, commandes, châssis,
    ///   barres, découpes), <see cref="Label_P11_06"/> à
    ///   <see cref="Label_P11_09"/> et <see cref="Label_P11_46"/> à
    ///   <see cref="Label_P11_48"/> pour les sept intitulés de la fiche
    ///   de synthèse du premier onglet. Toutes ces propriétés sont
    ///   alimentées par la mécanique multilingue factorisée par
    ///   <see cref="VM_Generic"/> : premier chargement au constructeur
    ///   via <see cref="VM_Generic.InitializeLabels"/>, rechargement
    ///   automatique à tout changement de langue dynamique par le
    ///   handler interne d'abonnement INPC à
    ///   <see cref="ISE_App.AppCultureCode"/> de l'ancêtre commun,
    ///   conformément à §4.11.5 du 0230 et à R-4.11.9 du
    ///   0231.</description></item>
    ///   <item><description>7 propriétés observables de données
    ///   caractérisant la série de production :
    ///   <see cref="IdSerialNumber"/> et <see cref="Description"/> pour
    ///   son identité, <see cref="ProductionStartDate"/> et
    ///   <see cref="ProductionEndDate"/> pour son calendrier de
    ///   production, <see cref="IsCuttingStarted"/>,
    ///   <see cref="IsCuttingCompleted"/> et
    ///   <see cref="IsBarOutOfStock"/> pour son état d'avancement. Ces
    ///   propriétés ne sont pas des libellés multilingues : leur
    ///   alimentation est portée par <see cref="LoadAsync"/>, qui les
    ///   projette depuis l'enregistrement de série relu en base.
    ///   Les trois derniers sont les drapeaux que le parcours de
    ///   production positionne au fil de son déroulement ; ils sont
    ///   rendus côté Vue sous forme de cases à cocher
    ///   désactivées.</description></item>
    /// </list>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer les 12 propriétés observables
    ///   <c>Label_P11_NN</c> et les 7 propriétés observables de données
    ///   en accès public en lecture, écriture privée via le helper
    ///   hérité <c>SetProperty&lt;T&gt;</c>.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Generic.LoadLabels"/> pour résoudre les 12 clés
    ///   <c>P11_01</c> à <c>P11_05</c>, <c>P11_06</c> à <c>P11_09</c> et
    ///   <c>P11_46</c> à <c>P11_48</c> via
    ///   <see cref="VM_Generic._dictionary"/> hérité et affecter les
    ///   valeurs résolues aux 12 propriétés <c>Label_P11_NN</c>,
    ///   conformément à R-4.11.8 du 0231.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Page_Generic.LoadAsync"/> pour lire l'identifiant
    ///   de série retenu dans <see cref="ISE_UseCase.IdSeriesSelected"/>,
    ///   relire l'enregistrement correspondant de la table des séries de
    ///   production sans suivi de modification par invocation du Query
    ///   Handler générique <see cref="IQ_Generic{T}"/> de
    ///   <see cref="ProductionSeries"/> via
    ///   <see cref="IS_UseCaseInvoker"/>, et alimenter les 7 propriétés
    ///   de données, en encapsulation par le filet hérité
    ///   <see cref="VM_Generic.ExecuteSafeAsync"/> (§4.7.3 du 0230). Le
    ///   hook est invoqué depuis le code-behind de <c>Page11</c> au point
    ///   d'extension <c>OnLoadedAsync</c> exposé par
    ///   <c>Page_Generic</c>.</description></item>
    ///   <item><description>Déléguer à <see cref="VM_Generic"/> la
    ///   cérémonie multilingue complète (premier chargement, abonnement
    ///   INPC filtré sur <see cref="ISE_App.AppCultureCode"/>,
    ///   marshalling Dispatcher défensif, rechargement) par l'unique
    ///   appel à <see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction du constructeur, conformément à I-4.11.11 et
    ///   R-4.11.8 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Relecture en base plutôt que réutilisation du DTO en
    /// mémoire : Le contexte de sélection applicatif expose, outre
    /// l'identifiant <see cref="ISE_UseCase.IdSeriesSelected"/>, l'objet
    /// de transfert complet de la série sélectionnée
    /// (<see cref="ISE_UseCase.SelectedSeries"/>) déjà présent en
    /// mémoire. Le présent ViewModel ne le consomme pas et relit
    /// l'enregistrement en base : cette relecture garantit la fraîcheur
    /// de l'information quel que soit le point d'appel de la page, et
    /// permet son rafraîchissement effectif par le bouton de
    /// rechargement manuel du menu horizontal — l'objet de transfert en
    /// mémoire étant, lui, figé au moment de la sélection.</para>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune écriture, aucune commande, aucun
    ///   contrôle de saisie. La page est un rendu visuel en lecture
    ///   seule ; aucun <c>ICommand</c> n'est exposé et la lecture est
    ///   effectuée sans suivi de modification.</description></item>
    ///   <item><description>Aucune logique métier ni règle
    ///   décisionnelle : les trois indicateurs d'avancement sont
    ///   restitués tels que positionnés en base par le parcours de
    ///   production, sans recalcul ni interprétation
    ///   locale.</description></item>
    ///   <item><description>Aucune vérification défensive de
    ///   l'identifiant de série. La page n'est atteinte qu'après
    ///   sélection d'une série dans le tableau de bord ;
    ///   <see cref="ISE_UseCase.IdSeriesSelected"/> est supposé porter un
    ///   identifiant valide et le cas d'un identifiant absent à
    ///   l'ouverture n'est pas envisagé.</description></item>
    ///   <item><description>Aucune décision de navigation : la règle
    ///   R-4.12.2 du 0231 réserve la décision de navigation aux
    ///   UseCases. <see cref="VM_Page11"/> n'injecte ni
    ///   <c>IU_Navigation</c> ni <c>IS_Navigation</c>. La sortie de la
    ///   page est portée par les commandes transverses du menu
    ///   horizontal, hors périmètre du présent
    ///   ViewModel.</description></item>
    ///   <item><description>Aucune consommation directe d'un contrat
    ///   <c>IU_</c> ou <c>IQ_</c>, conformément à I-4.10.10 du 0231 : le
    ///   Query Handler générique de <see cref="ProductionSeries"/> est
    ///   invoqué exclusivement via <see cref="IS_UseCaseInvoker"/> au
    ///   titre d'EA-11.</description></item>
    ///   <item><description>Aucune initiative transactionnelle
    ///   (I-4.10.1 du 0231) : la lecture est pure et n'ouvre, ne valide
    ///   ni n'annule aucune transaction ; aucune
    ///   <c>ExecutionStrategy</c> n'est mobilisée.</description></item>
    ///   <item><description>Aucun désabonnement explicite ni aucune
    ///   cérémonie multilingue locale : l'abonnement INPC à
    ///   <see cref="ISE_App"/> est branché par
    ///   <see cref="VM_Generic.InitializeLabels"/> et porté par le
    ///   handler interne de l'ancêtre commun, conformément à I-4.11.11
    ///   du 0231 ; aucun désabonnement n'est requis du
    ///   dérivé.</description></item>
    ///   <item><description>Aucun champ propre ni handler propre lié à
    ///   <see cref="ISE_App"/> : l'encapsulation de la dépendance est
    ///   intégralement portée par <see cref="VM_Generic"/> en champ
    ///   <c>private</c> non hérité (I-4.11.11 du 0231) ; le présent
    ///   dérivé n'accède jamais directement à
    ///   <see cref="ISE_App"/>.</description></item>
    ///   <item><description>Aucune logique locale de repli en cas de clé
    ///   absente du dictionnaire ni try/catch local dans
    ///   <see cref="LoadLabels"/> : la logique de repli est portée
    ///   exclusivement par <c>SR_Dictionary</c> conformément à R-4.11.6
    ///   et R-4.11.10 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : Aucune exception
    /// architecturale propre n'est portée par <see cref="VM_Page11"/>.
    /// L'injection de <see cref="ISE_App"/> au constructeur de la base
    /// relève exclusivement de la mécanique multilingue factorisée par
    /// <see cref="VM_Generic"/> (§4.15.5 du 0230, R-4.11.9 du 0231) et
    /// n'est pas une dérogation propre au présent dérivé. L'injection
    /// directe de <see cref="IU_LogAndNotify"/> par le ViewModel relève
    /// de l'exception architecturale propre du socle
    /// <see cref="VM_Generic"/> (EA-01, §4.15.5 du 0230), héritée et non
    /// re-déclarée à ce niveau ; elle est mobilisée par
    /// <see cref="LoadAsync"/> qui encapsule son invocation par le filet
    /// hérité <see cref="VM_Generic.ExecuteSafeAsync"/>. L'injection de
    /// <see cref="ISE_UseCase"/> est nominale au titre de la règle
    /// d'accès aux Settings (consommation par injection de l'interface
    /// <c>ISE_</c> au constructeur du composant consommateur).
    /// L'injection de <see cref="IS_UseCaseInvoker"/> est nominale au
    /// titre du mode d'invocation depuis <c>D_Presentation</c> posé en
    /// §4.10.10 du 0230 : les ViewModels invoquent les contrats
    /// <c>IU_</c> et <c>IQ_</c> via <see cref="IS_UseCaseInvoker"/> qui
    /// matérialise un <c>IServiceScope</c> distinct à chaque invocation.
    /// EA-11 est portée exclusivement par <c>SR_UseCaseInvoker</c> ; le
    /// présent ViewModel en est consommateur et non porteur.</para>
    ///
    /// <para>Absence de propriété de nom de page : Le présent ViewModel
    /// n'expose aucune propriété <c>PageName</c>. La page ne porte pas de
    /// titre propre, le <c>TabControl</c> étant placé directement dans la
    /// <c>Grid</c> de page ; l'identification visuelle est portée par les
    /// cinq en-têtes d'onglets. La clé <c>P11_00</c> subsiste au
    /// dictionnaire sans consommateur.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par deux extensions (R-4.4.10 du
    /// 0231) : l'extension <c>=== Propriétés publiques ===</c> au titre
    /// des 19 propriétés observables exposées, et l'extension
    /// <c>=== Méthodes protégées ===</c> au titre de l'override
    /// <see cref="LoadLabels"/>. Soit sept régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   19 champs supports des propriétés observables (12 champs
    ///   supports de libellés <c>_label_p11_NN</c> et 7 champs supports
    ///   de données <c>_idSerialNumber</c>, <c>_description</c>,
    ///   <c>_productionStartDate</c>, <c>_productionEndDate</c>,
    ///   <c>_isCuttingStarted</c>, <c>_isCuttingCompleted</c>,
    ///   <c>_isBarOutOfStock</c>).</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   2 champs <c>private readonly</c> stockant les dépendances
    ///   propres au dérivé, affectés au constructeur après les gardes
    ///   <see cref="ArgumentNullException"/> : <c>_useCaseInvoker</c>
    ///   (<see cref="IS_UseCaseInvoker"/>) et <c>_seUseCase</c>
    ///   (<see cref="ISE_UseCase"/>).</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c>
    ///   (extension §4.4.3) : 19 propriétés observables exposées en accès
    ///   public en lecture, écriture privée via
    ///   <c>SetProperty&lt;T&gt;</c>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   <c>public</c> à cinq paramètres, délégation à
    ///   <see cref="VM_Page_Generic"/> via
    ///   <c>base(dictionary, logAndNotify, app)</c>, gardes
    ///   <see cref="ArgumentNullException"/> locales sur les deux
    ///   dépendances propres, invocation finale de
    ///   <see cref="VM_Generic.InitializeLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> : override
    ///   <see cref="LoadAsync"/> selon le patron normatif §4.15.6 du 0230
    ///   à trois constituants
    ///   (<see cref="VM_Generic.BuildFirstCallChain"/> interne,
    ///   <see cref="VM_Generic.ExecuteSafeAsync"/>, propagation du
    ///   <see cref="System.Threading.CancellationToken"/>).</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c>
    ///   (extension §4.4.3) : override <see cref="LoadLabels"/> peuplant
    ///   les 12 propriétés <c>Label_P11_NN</c> via
    ///   <see cref="VM_Generic._dictionary"/>, une affectation par ligne
    ///   dans l'ordre numérique croissant des clés, sans appel à
    ///   <c>base.LoadLabels(callChain)</c>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente : <see cref="VM_Page11"/> n'expose aucun
    /// événement propre, l'événement <c>PropertyChanged</c> étant porté
    /// par <see cref="VM_Generic"/> au titre d'INPC et hérité par
    /// transitivité.</para>
    /// </remarks>
    public class VM_Page11 : VM_Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>Champ support de <see cref="Label_P11_01"/> (clé <c>P11_01</c>).</summary>
        private string _label_p11_01 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_02"/> (clé <c>P11_02</c>).</summary>
        private string _label_p11_02 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_03"/> (clé <c>P11_03</c>).</summary>
        private string _label_p11_03 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_04"/> (clé <c>P11_04</c>).</summary>
        private string _label_p11_04 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_05"/> (clé <c>P11_05</c>).</summary>
        private string _label_p11_05 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_06"/> (clé <c>P11_06</c>).</summary>
        private string _label_p11_06 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_07"/> (clé <c>P11_07</c>).</summary>
        private string _label_p11_07 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_08"/> (clé <c>P11_08</c>).</summary>
        private string _label_p11_08 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_09"/> (clé <c>P11_09</c>).</summary>
        private string _label_p11_09 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_46"/> (clé <c>P11_46</c>).</summary>
        private string _label_p11_46 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_47"/> (clé <c>P11_47</c>).</summary>
        private string _label_p11_47 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P11_48"/> (clé <c>P11_48</c>).</summary>
        private string _label_p11_48 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="IdSerialNumber"/>, initialisé à
        /// <c>0</c> et écrasé par <see cref="LoadAsync"/> lorsque
        /// l'enregistrement de série est trouvé.
        /// </summary>
        private int _idSerialNumber = 0;

        /// <summary>
        /// Champ support de <see cref="Description"/>, initialisé à
        /// <see langword="null"/> et écrasé par <see cref="LoadAsync"/>
        /// lorsque l'enregistrement de série est trouvé.
        /// </summary>
        private string? _description = null;

        /// <summary>
        /// Champ support de <see cref="ProductionStartDate"/>, initialisé
        /// à <see langword="null"/> et écrasé par
        /// <see cref="LoadAsync"/> lorsque l'enregistrement de série est
        /// trouvé.
        /// </summary>
        private DateTime? _productionStartDate = null;

        /// <summary>
        /// Champ support de <see cref="ProductionEndDate"/>, initialisé à
        /// <see langword="null"/> et écrasé par <see cref="LoadAsync"/>
        /// lorsque l'enregistrement de série est trouvé.
        /// </summary>
        private DateTime? _productionEndDate = null;

        /// <summary>
        /// Champ support de <see cref="IsCuttingStarted"/>, initialisé à
        /// <see langword="false"/> et écrasé par
        /// <see cref="LoadAsync"/> lorsque l'enregistrement de série est
        /// trouvé.
        /// </summary>
        private bool _isCuttingStarted = false;

        /// <summary>
        /// Champ support de <see cref="IsCuttingCompleted"/>, initialisé
        /// à <see langword="false"/> et écrasé par
        /// <see cref="LoadAsync"/> lorsque l'enregistrement de série est
        /// trouvé.
        /// </summary>
        private bool _isCuttingCompleted = false;

        /// <summary>
        /// Champ support de <see cref="IsBarOutOfStock"/>, initialisé à
        /// <see langword="false"/> et écrasé par
        /// <see cref="LoadAsync"/> lorsque l'enregistrement de série est
        /// trouvé.
        /// </summary>
        private bool _isBarOutOfStock = false;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Composant Singleton porteur de l'exception architecturale
        /// EA-11 (§4.10.10 et §4.15.10 du 0230, §17.4 du 0231), unique
        /// voie d'invocation des UseCases (<c>IU_</c>) et Query Handlers
        /// (<c>IQ_</c>) depuis un composant de <c>D_Presentation</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur, conformément au mode d'invocation depuis
        /// <c>D_Presentation</c> posé en §4.10.10 du 0230. À chaque
        /// invocation, <see cref="IS_UseCaseInvoker"/> matérialise un
        /// <c>IServiceScope</c> distinct, y résout le composant cible et
        /// l'exécute via le délégué fourni, puis dispose le scope. Le
        /// présent ViewModel est consommateur de
        /// <see cref="IS_UseCaseInvoker"/> et non porteur d'EA-11 : EA-11
        /// est portée exclusivement par <c>SR_UseCaseInvoker</c>.</para>
        /// <para>Mode d'invocation strict : Le passage par
        /// <see cref="IS_UseCaseInvoker"/> est imposé par la lecture
        /// stricte du §4.10.10 du 0230, qui pose l'interdiction
        /// structurelle de l'injection directe d'un contrat <c>IU_</c> ou
        /// <c>IQ_</c> dans un composant de <c>D_Presentation</c>,
        /// indépendamment de toute question de captive dependency.
        /// Conformité I-4.10.10 du 0231. Le Query Handler générique
        /// <see cref="IQ_Generic{T}"/> de
        /// <see cref="ProductionSeries"/> est invoqué dans
        /// <see cref="LoadAsync"/> via cette voie unique.</para>
        /// </remarks>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        /// <summary>
        /// Setting Singleton portant la cascade de sélection métier de
        /// l'application, dont l'identifiant de la série de production
        /// retenue par l'opérateur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur, conformément à la règle d'accès aux Settings qui
        /// impose la consommation par injection de l'interface
        /// <c>ISE_</c> au constructeur du composant consommateur. La
        /// consommation par le présent ViewModel se limite à la lecture
        /// de <see cref="ISE_UseCase.IdSeriesSelected"/> au sein de
        /// <see cref="LoadAsync"/> : le ViewModel ne mute jamais la
        /// cascade de sélection, les opérations atomiques
        /// <see cref="ISE_UseCase.SelectSeries"/>,
        /// <see cref="ISE_UseCase.SelectBar"/> et
        /// <see cref="ISE_UseCase.Reset"/> relevant des composants qui
        /// pilotent la sélection en amont.</para>
        /// <para>Le contexte de sélection est alimenté au clic sur la
        /// série dans le tableau de bord <c>Page10</c>, préalablement à
        /// la navigation vers <c>Page11</c> ; l'identifiant lu est la clé
        /// primaire de l'enregistrement de <see cref="ProductionSeries"/>
        /// correspondant.</para>
        /// </remarks>
        private readonly ISE_UseCase _seUseCase;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>Libellé multilingue associé à la clé <c>P11_01</c>, en-tête du premier onglet de la page (Série).</summary>
        public string Label_P11_01
        {
            get => _label_p11_01;
            private set => SetProperty(ref _label_p11_01, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_02</c>, en-tête du deuxième onglet de la page (Commandes).</summary>
        public string Label_P11_02
        {
            get => _label_p11_02;
            private set => SetProperty(ref _label_p11_02, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_03</c>, en-tête du troisième onglet de la page (Châssis).</summary>
        public string Label_P11_03
        {
            get => _label_p11_03;
            private set => SetProperty(ref _label_p11_03, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_04</c>, en-tête du quatrième onglet de la page (Barres).</summary>
        public string Label_P11_04
        {
            get => _label_p11_04;
            private set => SetProperty(ref _label_p11_04, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_05</c>, en-tête du cinquième onglet de la page (Découpes).</summary>
        public string Label_P11_05
        {
            get => _label_p11_05;
            private set => SetProperty(ref _label_p11_05, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_06</c>, intitulé du numéro de série dans la fiche de synthèse du premier onglet.</summary>
        public string Label_P11_06
        {
            get => _label_p11_06;
            private set => SetProperty(ref _label_p11_06, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_07</c>, intitulé de la désignation de la série dans la fiche de synthèse du premier onglet.</summary>
        public string Label_P11_07
        {
            get => _label_p11_07;
            private set => SetProperty(ref _label_p11_07, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_08</c>, intitulé de la date de début de production dans la fiche de synthèse du premier onglet.</summary>
        public string Label_P11_08
        {
            get => _label_p11_08;
            private set => SetProperty(ref _label_p11_08, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_09</c>, intitulé de la date de fin de production dans la fiche de synthèse du premier onglet.</summary>
        public string Label_P11_09
        {
            get => _label_p11_09;
            private set => SetProperty(ref _label_p11_09, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_46</c>, intitulé de l'indicateur de découpe commencée dans la fiche de synthèse du premier onglet.</summary>
        public string Label_P11_46
        {
            get => _label_p11_46;
            private set => SetProperty(ref _label_p11_46, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_47</c>, intitulé de l'indicateur de découpe terminée dans la fiche de synthèse du premier onglet.</summary>
        public string Label_P11_47
        {
            get => _label_p11_47;
            private set => SetProperty(ref _label_p11_47, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P11_48</c>, intitulé de l'indicateur de rupture de stock dans la fiche de synthèse du premier onglet.</summary>
        public string Label_P11_48
        {
            get => _label_p11_48;
            private set => SetProperty(ref _label_p11_48, value);
        }

        /// <summary>
        /// Numéro de la série de production consultée, projeté depuis le
        /// champ <c>IdSerialNumber</c> de l'enregistrement de
        /// <see cref="ProductionSeries"/> relu par
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Entier portant le numéro de série tel qu'enregistré en base,
        /// ou <c>0</c> avant le premier appel à <see cref="LoadAsync"/>
        /// ou lorsque l'enregistrement n'a pas été trouvé.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>
        /// sur le <c>TextBlock</c> de donnée de la première ligne de la
        /// fiche de synthèse. L'accesseur en écriture est privé : la
        /// valeur n'est modifiable que par <see cref="LoadAsync"/>. Cette
        /// propriété n'est pas affectée par <see cref="LoadLabels"/> et
        /// n'est pas rechargée au changement de langue, le numéro de
        /// série ne dépendant pas de la langue active.</para>
        /// <para>Le numéro de série exposé est distinct de la clé
        /// primaire de l'enregistrement : cette dernière, portée par
        /// <see cref="ISE_UseCase.IdSeriesSelected"/>, sert exclusivement
        /// à la relecture et n'est pas affichée.</para>
        /// </remarks>
        public int IdSerialNumber
        {
            get => _idSerialNumber;
            private set => SetProperty(ref _idSerialNumber, value);
        }

        /// <summary>
        /// Désignation de la série de production consultée, projetée
        /// depuis le champ <c>Description</c> de l'enregistrement de
        /// <see cref="ProductionSeries"/> relu par
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Chaîne portant la désignation de la série, ou
        /// <see langword="null"/> avant le premier appel à
        /// <see cref="LoadAsync"/> ou lorsque l'enregistrement n'a pas été
        /// trouvé.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>
        /// sur le <c>TextBlock</c> de donnée de la deuxième ligne de la
        /// fiche de synthèse. L'accesseur en écriture est privé : la
        /// valeur n'est modifiable que par
        /// <see cref="LoadAsync"/>.</para>
        /// <para>Type nullable assumé : L'entité
        /// <see cref="ProductionSeries"/> déclare son champ
        /// <c>Description</c> non nullable (<c>null!</c>), la colonne
        /// étant obligatoire en base. La propriété est néanmoins exposée
        /// en <see cref="string"/> nullable parce que le champ support
        /// s'initialise à <see langword="null"/> et n'est écrasé que si
        /// l'enregistrement est trouvé : le cas d'échec silencieux impose
        /// un état nullable défini, la fiche devant s'afficher vide sans
        /// valeur de substitution arbitraire.</para>
        /// </remarks>
        public string? Description
        {
            get => _description;
            private set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// Date de début de production de la série consultée, projetée
        /// depuis le champ <c>ProductionStartDate</c> de l'enregistrement
        /// de <see cref="ProductionSeries"/> relu par
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Date de début de production, ou <see langword="null"/> avant le
        /// premier appel à <see cref="LoadAsync"/>, lorsque
        /// l'enregistrement n'a pas été trouvé, ou lorsque la donnée est
        /// elle-même absente en base.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>
        /// sur le <c>TextBlock</c> de donnée de la troisième ligne de la
        /// fiche de synthèse, au travers du convertisseur
        /// <c>UT_DateFormatConverter</c> déclaré en ressource de page et
        /// consommé en liaison unidirectionnelle. Le formatage
        /// d'affichage relève intégralement de la vue ; le présent
        /// ViewModel expose la valeur typée telle que lue en
        /// base.</para>
        /// </remarks>
        public DateTime? ProductionStartDate
        {
            get => _productionStartDate;
            private set => SetProperty(ref _productionStartDate, value);
        }

        /// <summary>
        /// Date de fin de production de la série consultée, projetée
        /// depuis le champ <c>ProductionEndDate</c> de l'enregistrement
        /// de <see cref="ProductionSeries"/> relu par
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// Date de fin de production, ou <see langword="null"/> avant le
        /// premier appel à <see cref="LoadAsync"/>, lorsque
        /// l'enregistrement n'a pas été trouvé, ou lorsque la donnée est
        /// elle-même absente en base.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>
        /// sur le <c>TextBlock</c> de donnée de la quatrième ligne de la
        /// fiche de synthèse, au travers du convertisseur
        /// <c>UT_DateFormatConverter</c> déclaré en ressource de page et
        /// consommé en liaison unidirectionnelle. Le formatage
        /// d'affichage relève intégralement de la vue ; le présent
        /// ViewModel expose la valeur typée telle que lue en
        /// base.</para>
        /// </remarks>
        public DateTime? ProductionEndDate
        {
            get => _productionEndDate;
            private set => SetProperty(ref _productionEndDate, value);
        }

        /// <summary>
        /// Indicateur de découpe commencée pour la série consultée,
        /// projeté depuis le champ <c>IsCuttingStarted</c> de
        /// l'enregistrement de <see cref="ProductionSeries"/> relu par
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// <see langword="true"/> lorsque la découpe de la série a
        /// commencé ; <see langword="false"/> avant le premier appel à
        /// <see cref="LoadAsync"/>, lorsque l'enregistrement n'a pas été
        /// trouvé, ou lorsque la découpe n'a pas commencé.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>
        /// sur la case à cocher désactivée de la cinquième ligne de la
        /// fiche de synthèse. Le drapeau est positionné en base par le
        /// parcours de production ; le présent ViewModel le restitue sans
        /// interprétation ni recalcul, et la case à cocher est désactivée
        /// côté Vue, la page étant strictement en lecture.</para>
        /// </remarks>
        public bool IsCuttingStarted
        {
            get => _isCuttingStarted;
            private set => SetProperty(ref _isCuttingStarted, value);
        }

        /// <summary>
        /// Indicateur de découpe terminée pour la série consultée,
        /// projeté depuis le champ <c>IsCuttingCompleted</c> de
        /// l'enregistrement de <see cref="ProductionSeries"/> relu par
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// <see langword="true"/> lorsque la découpe de la série est
        /// terminée ; <see langword="false"/> avant le premier appel à
        /// <see cref="LoadAsync"/>, lorsque l'enregistrement n'a pas été
        /// trouvé, ou lorsque la découpe n'est pas terminée.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>
        /// sur la case à cocher désactivée de la sixième ligne de la
        /// fiche de synthèse. Le drapeau est positionné en base par le
        /// parcours de production ; le présent ViewModel le restitue sans
        /// interprétation ni recalcul, et la case à cocher est désactivée
        /// côté Vue, la page étant strictement en lecture.</para>
        /// </remarks>
        public bool IsCuttingCompleted
        {
            get => _isCuttingCompleted;
            private set => SetProperty(ref _isCuttingCompleted, value);
        }

        /// <summary>
        /// Indicateur de rupture de stock de barres pour la série
        /// consultée, projeté depuis le champ <c>IsBarOutOfStock</c> de
        /// l'enregistrement de <see cref="ProductionSeries"/> relu par
        /// <see cref="LoadAsync"/>.
        /// </summary>
        /// <value>
        /// <see langword="true"/> lorsqu'une rupture de stock de barres
        /// est en cours sur la série ; <see langword="false"/> avant le
        /// premier appel à <see cref="LoadAsync"/>, lorsque
        /// l'enregistrement n'a pas été trouvé, ou en l'absence de
        /// rupture.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page11"/>
        /// sur la case à cocher désactivée de la septième ligne de la
        /// fiche de synthèse. Le drapeau est positionné en base par le
        /// parcours de production ; le présent ViewModel le restitue sans
        /// interprétation ni recalcul, et la case à cocher est désactivée
        /// côté Vue, la page étant strictement en lecture.</para>
        /// </remarks>
        public bool IsBarOutOfStock
        {
            get => _isBarOutOfStock;
            private set => SetProperty(ref _isBarOutOfStock, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Page11"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur DI lors
        /// de la résolution du Singleton <see cref="VM_Page11"/> par la
        /// vue <c>Page11</c> via
        /// <c>App.ServiceProvider.GetRequiredService</c> dans son propre
        /// constructeur (EA-02 Service Locator de
        /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>,
        /// étendue aux dérivés directs pour la résolution de leur
        /// ViewModel — cf. §4.15.7 et §4.15.11 du 0230).</para>
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
        ///   <paramref name="logAndNotify"/> en champs <c>protected</c>
        ///   (<see cref="VM_Generic._dictionary"/>,
        ///   <see cref="VM_Generic._logAndNotify"/>) accessibles aux
        ///   dérivés, stocke <paramref name="app"/> en champ
        ///   <c>private</c> non hérité (encapsulation de la mécanique
        ///   multilingue, conformément à I-4.11.11 du 0231), et
        ///   initialise le champ <c>_callee</c> via
        ///   <c>GetType().Name</c>.</description></item>
        ///   <item><description>Gardes
        ///   <see cref="ArgumentNullException"/> locales sur les deux
        ///   dépendances propres au dérivé
        ///   (<paramref name="useCaseInvoker"/>,
        ///   <paramref name="seUseCase"/>) et affectation aux champs
        ///   <c>_useCaseInvoker</c> et <c>_seUseCase</c>. Ces gardes sont
        ///   portées localement, la classe de base n'imposant pas ces
        ///   dépendances.</description></item>
        ///   <item><description>Appel à
        ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
        ///   instruction du corps. Ce hook explicite orchestre la
        ///   séquence normative en trois temps : construction d'une
        ///   CallChain initiale via
        ///   <see cref="VM_Generic.BuildFirstCallChain"/>, premier appel
        ///   synchrone à l'override <see cref="LoadLabels"/> peuplant les
        ///   12 propriétés <c>Label_P11_NN</c> avant le premier binding
        ///   WPF de la vue, et branchement de l'abonnement INPC interne à
        ///   <see cref="ISE_App"/> pour la prise en compte du changement
        ///   de langue dynamique (R-4.11.8 et R-4.11.9 du
        ///   0231).</description></item>
        /// </list>
        ///
        /// <para>Ordre des paramètres : Les trois dépendances de base
        /// occupent les trois premières positions dans l'ordre imposé par
        /// la signature de <see cref="VM_Page_Generic"/> ; les deux
        /// dépendances propres suivent, dans l'ordre
        /// <see cref="IS_UseCaseInvoker"/> puis
        /// <see cref="ISE_UseCase"/>.</para>
        ///
        /// <para>Règle d'invocation d'<c>InitializeLabels</c> (R-4.11.8
        /// du 0231) : L'appel à
        /// <see cref="VM_Generic.InitializeLabels"/> est exclusivement
        /// effectué dans le constructeur du ViewModel dérivé concret
        /// final, en dernière instruction, après l'affectation de toutes
        /// les dépendances propres. Cette règle prévient l'écueil
        /// classique de l'invocation virtuelle dans le constructeur d'une
        /// classe de base avec dépendances dérivées non encore
        /// initialisées.</para>
        ///
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le constructeur
        /// au-delà des gardes <see cref="ArgumentNullException"/>. Une
        /// levée de garde traduit une défaillance de configuration du
        /// conteneur DI et doit faire échouer l'instanciation
        /// immédiatement.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c>. Mobilisé par <see cref="LoadLabels"/> pour
        /// la résolution des 12 clés de la page. Injecté en Singleton par
        /// le conteneur DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_Page_Generic"/> via <c>base(...)</c>. Mobilisé
        /// par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> au sein de
        /// <see cref="LoadAsync"/>. Injecté en Singleton par le conteneur
        /// DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par l'ancêtre commun (abonnement INPC
        /// interne à <see cref="ISE_App.AppCultureCode"/>). Le présent
        /// dérivé ne stocke pas cette dépendance ni n'y accède
        /// directement, conformément à I-4.11.11 du 0231. Injecté en
        /// Singleton par le conteneur DI.</param>
        /// <param name="useCaseInvoker">Composant Singleton porteur
        /// d'EA-11, unique voie d'invocation du Query Handler générique
        /// <see cref="IQ_Generic{T}"/> de
        /// <see cref="ProductionSeries"/> depuis le présent ViewModel.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="seUseCase">Setting Singleton portant la cascade
        /// de sélection métier, dont l'identifiant de la série de
        /// production retenue. Mobilisé en lecture seule par
        /// <see cref="LoadAsync"/>. Injecté en Singleton par le conteneur
        /// DI.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="useCaseInvoker"/> ou
        /// <paramref name="seUseCase"/> est <see langword="null"/>. Les
        /// gardes sur <paramref name="dictionary"/>,
        /// <paramref name="logAndNotify"/> et <paramref name="app"/> sont
        /// portées par <see cref="VM_Generic"/> via la chaîne
        /// <c>base(...)</c>.</exception>
        public VM_Page11(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IS_UseCaseInvoker useCaseInvoker,
            ISE_UseCase seUseCase)
            : base(dictionary, logAndNotify, app)
        {
            _useCaseInvoker = useCaseInvoker
                ?? throw new ArgumentNullException(nameof(useCaseInvoker));
            _seUseCase = seUseCase
                ?? throw new ArgumentNullException(nameof(seUseCase));

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Redéfinit le hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> pour alimenter les
        /// sept caractéristiques de la fiche de synthèse à partir de
        /// l'enregistrement de série désigné par le contexte de sélection
        /// applicatif, relu sans suivi de modification par invocation du
        /// Query Handler générique <see cref="IQ_Generic{T}"/> de
        /// <see cref="ProductionSeries"/> via
        /// <see cref="IS_UseCaseInvoker"/> (EA-11).
        /// </summary>
        /// <param name="caller">CallChain construite par l'orchestrateur
        /// appelant côté <c>Page_Generic</c> au format normatif
        /// <c>{_callee} &gt; OnLoadedHandler &gt; OnLoadedAsync</c> et
        /// propagée telle quelle par le code-behind via
        /// <c>_viewModel.LoadAsync(caller, ct)</c>. Le paramètre est reçu
        /// par contrat du hook au socle
        /// <see cref="VM_Page_Generic"/> mais n'est pas consommé par le
        /// corps du présent override : une CallChain interne distincte
        /// est construite via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> et consommée par
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> et par le délégué
        /// d'invocation du Query Handler, conformément au patron de
        /// surcharge normatif §4.15.6 du 0230.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé par le
        /// code-behind appelant. Propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, à
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}(System.Func{TUseCase, System.Threading.CancellationToken, System.Threading.Tasks.Task{TResult}}, System.Threading.CancellationToken)"/>
        /// et, par le délégué, au Query Handler
        /// <see cref="IQ_Generic{T}.HandleGetByIdAsNoTrackingAsync"/>.
        /// Valeur par défaut : <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement des sept propriétés de données de la fiche de
        /// synthèse.</returns>
        /// <remarks>
        /// <para>Contexte : Override du hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> déclaré
        /// <c>public virtual</c> au socle conformément à §4.15.6 du 0230.
        /// Invoquée depuis le code-behind de <c>Page11</c> au point
        /// d'extension <c>OnLoadedAsync</c> exposé par
        /// <c>Page_Generic</c> (§4.15.7 du 0230). Méthode strictement
        /// disjointe de <see cref="LoadLabels"/> : libellés synchrones au
        /// constructeur d'un côté, données fonctionnelles asynchrones au
        /// <c>Loaded</c> de la page de l'autre.</para>
        ///
        /// <para>Objectif : Alimenter en trois temps coordonnés les sept
        /// propriétés de données :</para>
        /// <list type="number">
        ///   <item><description>Lecture de l'identifiant de série retenu
        ///   dans <see cref="ISE_UseCase.IdSeriesSelected"/>, stocké en
        ///   variable locale consommée par le temps
        ///   suivant.</description></item>
        ///   <item><description>Relecture de l'enregistrement
        ///   correspondant de la table des séries de production par
        ///   invocation du Query Handler générique
        ///   <see cref="IQ_Generic{T}"/> de
        ///   <see cref="ProductionSeries"/> via
        ///   <see cref="IS_UseCaseInvoker"/>, méthode
        ///   <see cref="IQ_Generic{T}.HandleGetByIdAsNoTrackingAsync"/>,
        ///   produisant l'entité correspondante non suivie par EF Core
        ///   (lecture pure, aucune mutation subséquente de l'entité
        ///   retournée) ou <see langword="null"/> en l'absence
        ///   d'enregistrement.</description></item>
        ///   <item><description>Alimentation des sept propriétés
        ///   <see cref="IdSerialNumber"/>, <see cref="Description"/>,
        ///   <see cref="ProductionStartDate"/>,
        ///   <see cref="ProductionEndDate"/>,
        ///   <see cref="IsCuttingStarted"/>,
        ///   <see cref="IsCuttingCompleted"/> et
        ///   <see cref="IsBarOutOfStock"/> par projection directe des
        ///   champs homonymes de l'entité relue, chacune émettant sa
        ///   notification INPC par le helper hérité
        ///   <c>SetProperty&lt;T&gt;</c>.</description></item>
        /// </list>
        ///
        /// <para>Précondition non vérifiée :
        /// <see cref="ISE_UseCase.IdSeriesSelected"/> est supposé porter
        /// un identifiant valide. Aucune vérification défensive n'est
        /// produite : la page n'est atteinte qu'après sélection d'une
        /// série dans le tableau de bord, laquelle alimente le contexte
        /// de sélection préalablement à la navigation.</para>
        ///
        /// <para>Cas d'échec métier — enregistrement introuvable : Si la
        /// relecture ne rend aucun enregistrement (série supprimée entre
        /// la sélection et l'ouverture, identifiant devenu invalide,
        /// incohérence de base), le chargement s'interrompt
        /// silencieusement par sortie anticipée de la lambda. Les sept
        /// propriétés conservent leurs valeurs d'initialisation et la
        /// fiche s'affiche vide. Aucune notification n'est émise, aucune
        /// exception n'est levée : le cas est une issue fonctionnelle
        /// admise, non une anomalie.</para>
        ///
        /// <para>Patron de surcharge normatif (§4.15.6 du 0230) :
        /// L'override construit une CallChain interne
        /// (<c>innerCallChain</c>) via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> hérité, plutôt
        /// que de consommer la CallChain reçue en paramètre. Le paramètre
        /// <paramref name="caller"/> reçu du hook est utile à des fins de
        /// traçabilité amont, mais la CallChain consommée par le filet et
        /// par le délégué d'invocation est celle reconstruite localement,
        /// garantissant que le format normatif
        /// <c>{_callee} &gt; LoadAsync</c> est appliqué pour l'opération
        /// elle-même.</para>
        ///
        /// <para>Idempotence : La méthode est ré-appelable à chaque
        /// entrée sur la page et à chaque rechargement manuel déclenché
        /// depuis le menu horizontal, sans flag de mémoire d'état. Chaque
        /// appel produit une nouvelle relecture complète de
        /// l'enregistrement et une nouvelle alimentation des sept
        /// propriétés — coût négligeable, la lecture portant sur un
        /// enregistrement unique par clé primaire.</para>
        ///
        /// <para>Filet de sécurité : L'invocation est encapsulée par le
        /// filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/> (§4.7.3
        /// du 0230). Aucun try/catch local n'est posé : les défaillances
        /// métier (<c>Ex_Business</c>) et infrastructure
        /// (<c>Ex_Infrastructure</c>) éventuellement levées par le Query
        /// Handler sont absorbées par
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> selon le pipeline
        /// canonique et traitées terminalement par
        /// <see cref="IU_LogAndNotify"/>. En cas de défaillance, les sept
        /// propriétés restent dans leur état antérieur, l'alimentation
        /// étant postérieure à l'invocation dans le corps de la
        /// lambda.</para>
        ///
        /// <para>Invariants : Aucune écriture en base ; lecture sans
        /// suivi de modification ; aucune transactionnalité et aucune
        /// <c>ExecutionStrategy</c> ; jeton d'annulation propagé de bout
        /// en chaîne.</para>
        ///
        /// <para>Mode d'invocation : Conformément à §4.10.10 du 0230,
        /// l'invocation du Query Handler générique
        /// <see cref="IQ_Generic{T}"/> de
        /// <see cref="ProductionSeries"/> est portée par
        /// <see cref="IS_UseCaseInvoker"/> qui matérialise un
        /// <c>IServiceScope</c> distinct pour l'invocation, y résout
        /// l'implémentation du contrat
        /// (<c>QH_Generic&lt;ProductionSeries&gt;</c>) et l'exécute via
        /// le délégué fourni, puis dispose le scope. Le présent ViewModel
        /// n'injecte pas directement le contrat
        /// <see cref="IQ_Generic{T}"/>, conformément à I-4.10.10 du
        /// 0231.</para>
        ///
        /// <para>Retours signalables : Aucun.
        /// <see cref="LoadAsync"/> ne signale rien à son appelant, le
        /// traitement terminal des erreurs étant intégralement porté par
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> (EA-01).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">Propagée
        /// silencieusement à l'appelant sur signal d'annulation
        /// coopérative par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, conformément à
        /// §4.7.3 du 0230. Aucune journalisation ni
        /// notification.</exception>
        public override async Task LoadAsync(
            string caller,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                int idSeries = _seUseCase.IdSeriesSelected;

                var series = await _useCaseInvoker
                    .InvokeAsync<IQ_Generic<ProductionSeries>, ProductionSeries?>(
                        (handler, innerCt) => handler.HandleGetByIdAsNoTrackingAsync(
                            innerCallChain,
                            idSeries,
                            innerCt),
                        ct);

                if (series is null)
                {
                    return;
                }

                IdSerialNumber = series.IdSerialNumber;
                Description = series.Description;
                ProductionStartDate = series.ProductionStartDate;
                ProductionEndDate = series.ProductionEndDate;
                IsCuttingStarted = series.IsCuttingStarted;
                IsCuttingCompleted = series.IsCuttingCompleted;
                IsBarOutOfStock = series.IsBarOutOfStock;
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> pour charger les douze
        /// libellés multilingues affichés par la page <c>Page11</c> —
        /// cinq en-têtes d'onglets et sept intitulés de la fiche de
        /// synthèse — depuis le dictionnaire de langue actif et les
        /// affecter aux propriétés observables
        /// <c>Label_P11_NN</c> correspondantes.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissant le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> conformément à R-4.11.8 du
        /// 0231. Invoquée par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur pour
        /// le premier chargement, puis par le handler interne
        /// d'abonnement INPC de <see cref="VM_Generic"/> à chaque
        /// changement de langue dynamique notifié par
        /// <see cref="ISE_App.AppCultureCode"/>, avec marshalling
        /// Dispatcher défensif vers le thread UI.</para>
        ///
        /// <para>Objectif : Garantir que les douze propriétés
        /// <c>Label_P11_NN</c> sont synchronisées avec la langue active
        /// du dictionnaire, tant au moment de l'instanciation du
        /// ViewModel que lors de tout changement ultérieur de langue
        /// dynamique au cours de la session. Les douze clés sont
        /// résolues par une affectation par ligne, dans l'ordre
        /// numérique croissant — <c>P11_01</c> à <c>P11_05</c> pour les
        /// en-têtes d'onglets, <c>P11_06</c> à <c>P11_09</c> pour les
        /// quatre premiers intitulés de la fiche, <c>P11_46</c> à
        /// <c>P11_48</c> pour les trois intitulés d'indicateurs — sans
        /// boucle dynamique.</para>
        ///
        /// <para>Absence d'appel à <c>base.LoadLabels(callChain)</c> :
        /// L'implémentation par défaut de
        /// <see cref="VM_Generic.LoadLabels"/> ne porte aucun traitement.
        /// L'appel n'apporterait qu'un bruit inutile et est délibérément
        /// omis, conformément à la pratique standard d'override lorsque
        /// la base ne porte aucun traitement.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local n'est posé. Le
        /// filet est porté exclusivement par <c>SR_Dictionary</c>
        /// conformément à R-4.11.6 et R-4.11.10 du 0231 — toute anomalie
        /// (clé absente, erreur inattendue) est journalisée en interne
        /// par <c>SR_Dictionary</c> et résolue par la valeur de repli
        /// <c>[P11_NN] not found</c>, sans interruption ni propagation
        /// d'exception au présent ViewModel. L'unique exception
        /// susceptible d'être propagée serait
        /// <see cref="OperationCanceledException"/>, structurellement
        /// impossible ici puisque
        /// <see cref="IS_Dictionary.GetText"/> est invoquée sans
        /// <see cref="System.Threading.CancellationToken"/> explicite
        /// (paramètre optionnel par défaut <c>default</c>, équivalent à
        /// <see cref="System.Threading.CancellationToken.None"/>).</para>
        /// </remarks>
        /// <param name="caller">CallChain construite par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// (premier chargement) ou par le handler interne d'abonnement
        /// INPC de <see cref="VM_Generic"/> au changement de langue
        /// dynamique (rechargement), enrichie localement au format
        /// <c>{caller} &gt; LoadLabels</c> et transmise au service de
        /// dictionnaire pour traçabilité.</param>
        protected override void LoadLabels(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadLabels)}";

            Label_P11_01 = _dictionary.GetText(callChain, "P11_01");
            Label_P11_02 = _dictionary.GetText(callChain, "P11_02");
            Label_P11_03 = _dictionary.GetText(callChain, "P11_03");
            Label_P11_04 = _dictionary.GetText(callChain, "P11_04");
            Label_P11_05 = _dictionary.GetText(callChain, "P11_05");
            Label_P11_06 = _dictionary.GetText(callChain, "P11_06");
            Label_P11_07 = _dictionary.GetText(callChain, "P11_07");
            Label_P11_08 = _dictionary.GetText(callChain, "P11_08");
            Label_P11_09 = _dictionary.GetText(callChain, "P11_09");
            Label_P11_46 = _dictionary.GetText(callChain, "P11_46");
            Label_P11_47 = _dictionary.GetText(callChain, "P11_47");
            Label_P11_48 = _dictionary.GetText(callChain, "P11_48");
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}