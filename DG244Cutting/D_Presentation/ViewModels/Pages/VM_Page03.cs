using System.Collections.ObjectModel;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page Messagerie applicative <c>Page03</c> de
    /// l'application DG244Cutting, exposant à la vue les deux boîtes
    /// (Reçus, Envoyés) de la messagerie interne adressée à
    /// l'application courante et émise par celle-ci, ainsi qu'une
    /// fenêtre de consultation détaillée du message sélectionné.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page03"/>.
    /// La page est accessible à tout utilisateur connecté et n'expose 
    /// aucune commande de composition de message : la messagerie est 
    /// strictement applicative, l'application étant l'émetteur exclusif 
    /// via d'autres mécanismes hors périmètre du présent ViewModel. 
    /// La sortie s'effectue exclusivement via les boutons transverses
    /// du menu horizontal portés par le couple
    /// <c>VM_MH_Generic</c> / <c>MH_Generic</c>.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page03"/> :</para>
    /// <list type="bullet">
    ///   <item><description>10 propriétés observables
    ///   <c>Label_P03_NN</c> liées aux clés homonymes du
    ///   dictionnaire actif (<see cref="Label_P03_01"/> à
    ///   <see cref="Label_P03_10"/>), couvrant les trois en-têtes
    ///   d'onglets, les trois en-têtes de colonnes communes aux
    ///   deux ListViews et les quatre libellés des titres de
    ///   l'onglet Détail.</description></item>
    ///   <item><description>Une propriété observable contextuelle
    ///   <see cref="DateTitleLabel"/> qui bascule dynamiquement
    ///   entre <see cref="Label_P03_08"/> (« Reçu le : ») et
    ///   <see cref="Label_P03_09"/> (« Envoyé le : ») selon la
    ///   nature — réception ou émission — du message
    ///   sélectionné.</description></item>
    ///   <item><description>Deux collections observables
    ///   <see cref="MessagesReceived"/> et
    ///   <see cref="MessagesSent"/> de type
    ///   <see cref="ObservableCollection{UserAppMessage}"/>,
    ///   alimentées par <see cref="LoadAsync"/> via deux invocations
    ///   consécutives du Query Handler
    ///   <see cref="IQ_UserAppMessage"/>
    ///   (<see cref="IQ_UserAppMessage.HandleGetMessagesReceivedAsync"/>
    ///   puis
    ///   <see cref="IQ_UserAppMessage.HandleGetMessagesSentAsync"/>)
    ///   sous médiation de <see cref="IS_UseCaseInvoker"/>
    ///   (EA-11).</description></item>
    ///   <item><description>Trois propriétés observables de sélection
    ///   (<see cref="SelectedReceivedMessage"/>,
    ///   <see cref="SelectedSentMessage"/>,
    ///   <see cref="SelectedMessage"/>) coordonnées par la logique
    ///   métier portée par les setters, et une propriété booléenne
    ///   <see cref="SelectTabItemMessage"/> pilotant la sélection
    ///   automatique de l'onglet Détail à chaque nouvelle sélection
    ///   non nulle.</description></item>
    /// </list>
    ///
    /// <para>Les dix propriétés <c>Label_P03_NN</c> et la propriété
    /// contextuelle <see cref="DateTitleLabel"/> sont alimentées par
    /// la mécanique multilingue factorisée par
    /// <see cref="VM_Generic"/> : premier chargement au constructeur
    /// via <see cref="VM_Generic.InitializeLabels"/>, rechargement
    /// automatique à tout changement de langue dynamique par le
    /// handler interne d'abonnement INPC à
    /// <see cref="ISE_App.AppCultureCode"/>. La propagation du
    /// rechargement de langue sur <see cref="DateTitleLabel"/> est
    /// assurée par invocation de <see cref="UpdateDateTitleLabel"/>
    /// en clôture de <see cref="LoadLabels"/>.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Charger les libellés multilingues
    ///   (override <see cref="LoadLabels"/>).</description></item>
    ///   <item><description>Charger les deux boîtes de messagerie au
    ///   montage de la page (override
    ///   <see cref="LoadAsync"/>).</description></item>
    ///   <item><description>Coordonner les trois propriétés de
    ///   sélection à chaque changement utilisateur, déclencher le
    ///   recalcul de <see cref="DateTitleLabel"/>, et basculer
    ///   <see cref="SelectTabItemMessage"/> pour la sélection
    ///   automatique de l'onglet Détail.</description></item>
    ///   <item><description>Déclencher en fire-and-forget le
    ///   marquage en lecture du message reçu sélectionné, chaîné
    ///   d'une propagation immédiate de l'état « messages non lus »
    ///   sur <see cref="ISE_App"/> via le UseCase
    ///   <see cref="IU_UserAppMessage_CheckUnread"/>, à chaque
    ///   sélection d'un message reçu non lu.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne décide ni ne déclenche aucune
    ///   navigation : la décision de navigation appartient aux
    ///   UseCases, conformément à R-4.12.2 du 0231 et à I-4.12.1.
    ///   Le présent ViewModel n'injecte ni <c>IS_Navigation</c> ni
    ///   <c>IU_Navigation</c>, conformément à
    ///   I-4.12.2.</description></item>
    ///   <item><description>N'expose aucune commande WPF propre :
    ///   les sorties de page transitent par les commandes
    ///   transverses du menu horizontal
    ///   <c>MH03</c>.</description></item>
    ///   <item><description>N'injecte directement aucun contrat
    ///   <c>IU_*</c> ni <c>IQ_*</c> au constructeur : toutes les
    ///   invocations des trois UseCases et du Query Handler
    ///   transitent par <see cref="IS_UseCaseInvoker"/>,
    ///   conformément à I-4.10.10 du 0231 (EA-11).</description></item>
    ///   <item><description>N'effectue aucune décision de tri ni
    ///   transformation sur les deux collections : le tri
    ///   descendant par <see cref="UserAppMessage.SentAt"/> est
    ///   porté en interne par le Query Handler amont
    ///   (<see cref="IQ_UserAppMessage"/>, contrats
    ///   <see cref="IQ_UserAppMessage.HandleGetMessagesReceivedAsync"/>
    ///   et
    ///   <see cref="IQ_UserAppMessage.HandleGetMessagesSentAsync"/>).</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales mobilisées
    /// (héritées ou propres) :</para>
    /// <list type="bullet">
    ///   <item><description>EA-01 — Injection directe d'un UseCase
    ///   en couche Présentation : héritée de <see cref="VM_Generic"/>
    ///   via l'injection d'<see cref="IU_LogAndNotify"/> consommée
    ///   par le filet <see cref="VM_Generic.ExecuteSafeAsync"/>.
    ///   Aucune autre injection directe de UseCase n'est portée par
    ///   le présent ViewModel.</description></item>
    ///   <item><description>EA-11 — Médiation des UseCases métier
    ///   et Query Handlers via <see cref="IS_UseCaseInvoker"/> :
    ///   propre. Quatre contrats consommés au total
    ///   (<see cref="IQ_UserAppMessage"/> pour deux invocations,
    ///   <see cref="IU_UserAppMessage_MarkAsRead"/>,
    ///   <see cref="IU_UserAppMessage_CheckUnread"/>). Conformément
    ///   à §4.10.10 du 0230, l'invocation matérialise un
    ///   <c>IServiceScope</c> distinct par appel, y résout
    ///   l'implémentation du contrat et l'exécute via le délégué
    ///   fourni, puis dispose le scope.</description></item>
    /// </list>
    ///
    /// <para>Statut typologique : Sous-cas riche de la famille
    /// VM_Page selon le chapeau commun §5.2 du 0232-Page-VM. Quatre
    /// dimensions de pré-qualification :</para>
    /// <list type="bullet">
    ///   <item><description>(i) Override de
    ///   <see cref="LoadAsync"/> : oui, scénario à deux invocations
    ///   consécutives du Query Handler
    ///   <see cref="IQ_UserAppMessage"/>.</description></item>
    ///   <item><description>(ii) Consommation de
    ///   <see cref="IS_UseCaseInvoker"/> : oui, quatre contrats
    ///   consommés.</description></item>
    ///   <item><description>(iii) Overrides côté
    ///   <c>Page03</c> : <c>ApplyLayout</c> oui, <c>OnResized</c>
    ///   oui, <c>OnLoadedAsync</c> oui, <c>OnUnloadedAsync</c>
    ///   non.</description></item>
    ///   <item><description>(iv) Structure XAML : Grid + TabControl
    ///   à trois TabItem (ListView Reçus + ListView Envoyés +
    ///   sous-Grid de détail). Analogue structurel le plus proche :
    ///   <c>Page01</c> enrichi d'un troisième onglet Détail.</description></item>
    /// </list>
    ///
    /// <para>Structure des régions : La classe applique la structure
    /// normative à sept régions, soit cinq régions standard
    /// (§4.4.2 du 0230) et deux extensions au titre de R-4.4.10 du
    /// 0231 :</para>
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   onze champs support des onze propriétés observables de
    ///   libellés (dix <c>_label_p03_NN</c> et
    ///   <c>_dateTitleLabel</c>), trois champs support des
    ///   propriétés observables de sélection
    ///   (<c>_selectedReceivedMessage</c>,
    ///   <c>_selectedSentMessage</c>, <c>_selectedMessage</c>) et
    ///   un champ booléen <c>_selectTabItemMessage</c>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   champs <c>_appContext</c> et
    ///   <c>_useCaseInvoker</c>.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c> :
    ///   extension présente (R-4.4.10). Dix propriétés
    ///   <c>Label_P03_01</c> à <c>Label_P03_10</c>, propriété
    ///   contextuelle <see cref="DateTitleLabel"/>, deux
    ///   collections observables <see cref="MessagesReceived"/> et
    ///   <see cref="MessagesSent"/>, trois propriétés de sélection
    ///   et la propriété booléenne
    ///   <see cref="SelectTabItemMessage"/>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur à cinq paramètres en délégation à
    ///   <see cref="VM_Page_Generic"/> sur les trois paramètres
    ///   socle, avec gardes locales sur les deux dépendances
    ///   propres et invocation finale
    ///   d'<see cref="VM_Generic.InitializeLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   override de
    ///   <see cref="VM_Page_Generic.LoadAsync"/>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   extension présente (R-4.4.10). Override de
    ///   <see cref="VM_Generic.LoadLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   méthodes <see cref="UpdateDateTitleLabel"/>,
    ///   <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/> et
    ///   <see cref="LoadMessagesAsync"/>.</description></item>
    /// </list>
    /// </remarks>
    public class VM_Page03 : VM_Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>Champ support de <see cref="Label_P03_01"/>.</summary>
        private string _label_p03_01 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_02"/>.</summary>
        private string _label_p03_02 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_03"/>.</summary>
        private string _label_p03_03 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_04"/>.</summary>
        private string _label_p03_04 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_05"/>.</summary>
        private string _label_p03_05 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_06"/>.</summary>
        private string _label_p03_06 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_07"/>.</summary>
        private string _label_p03_07 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_08"/>.</summary>
        private string _label_p03_08 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_09"/>.</summary>
        private string _label_p03_09 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P03_10"/>.</summary>
        private string _label_p03_10 = string.Empty;

        /// <summary>Champ support de <see cref="DateTitleLabel"/>.</summary>
        private string _dateTitleLabel = string.Empty;

        /// <summary>Champ support de <see cref="SelectedReceivedMessage"/>.</summary>
        private UserAppMessage? _selectedReceivedMessage;

        /// <summary>Champ support de <see cref="SelectedSentMessage"/>.</summary>
        private UserAppMessage? _selectedSentMessage;

        /// <summary>Champ support de <see cref="SelectedMessage"/>.</summary>
        private UserAppMessage? _selectedMessage;

        /// <summary>Champ support de <see cref="SelectTabItemMessage"/>.</summary>
        private bool _selectTabItemMessage;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service applicatif Singleton fournissant un instantané
        /// (<see cref="DTO_AppContext"/>) du contexte applicatif et
        /// utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton injecté par le conteneur DI au
        /// constructeur. Mobilisé par <see cref="LoadAsync"/> pour
        /// la lecture du champ <see cref="DTO_AppContext.AppId"/>
        /// propagé en paramètre des deux invocations du Query
        /// Handler <see cref="IQ_UserAppMessage"/>. La consommation
        /// est <c>_appContext.GetAppContext()</c> en lecture
        /// d'instantané, conformément au patron canonique de
        /// snapshot atomique en entrée de
        /// <see cref="LoadAsync"/>.</para>
        /// </remarks>
        private readonly IS_AppContext _appContext;

        /// <summary>
        /// Composant Singleton porteur d'EA-11, unique voie
        /// d'invocation des UseCases métier et Query Handlers
        /// consommés par le présent ViewModel.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton injecté par le conteneur DI au
        /// constructeur. Mobilisé par <see cref="LoadAsync"/> pour
        /// deux invocations consécutives du Query Handler
        /// <see cref="IQ_UserAppMessage"/>, et par
        /// <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/> pour
        /// les deux invocations chaînées de
        /// <see cref="IU_UserAppMessage_MarkAsRead"/> et de
        /// <see cref="IU_UserAppMessage_CheckUnread"/>.</para>
        /// <para>Conformément à §4.10.10 du 0230 et à I-4.10.10 du
        /// 0231, le présent ViewModel n'injecte aucun des quatre
        /// contrats consommés (les deux contrats du Query Handler et
        /// les deux UseCases) : la résolution dans un
        /// <c>IServiceScope</c> distinct par invocation est
        /// matérialisée par <see cref="IS_UseCaseInvoker"/>
        /// lui-même.</para>
        /// </remarks>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Libellé multilingue de l'en-tête du TabItem
        /// « Messages reçus », lié à la clé <c>P03_01</c> du
        /// dictionnaire actif.
        /// </summary>
        public string Label_P03_01
        {
            get => _label_p03_01;
            private set => SetProperty(ref _label_p03_01, value);
        }

        /// <summary>
        /// Libellé multilingue de l'en-tête du TabItem
        /// « Messages envoyés », lié à la clé <c>P03_02</c> du
        /// dictionnaire actif.
        /// </summary>
        public string Label_P03_02
        {
            get => _label_p03_02;
            private set => SetProperty(ref _label_p03_02, value);
        }

        /// <summary>
        /// Libellé multilingue de l'en-tête du TabItem
        /// « Détail », lié à la clé <c>P03_03</c> du dictionnaire
        /// actif.
        /// </summary>
        public string Label_P03_03
        {
            get => _label_p03_03;
            private set => SetProperty(ref _label_p03_03, value);
        }

        /// <summary>
        /// Libellé multilingue de l'en-tête de colonne « Date »
        /// commun aux deux ListViews, lié à la clé <c>P03_04</c> du
        /// dictionnaire actif.
        /// </summary>
        public string Label_P03_04
        {
            get => _label_p03_04;
            private set => SetProperty(ref _label_p03_04, value);
        }

        /// <summary>
        /// Libellé multilingue de l'en-tête de colonne « Lu » commun
        /// aux deux ListViews, lié à la clé <c>P03_05</c> du
        /// dictionnaire actif.
        /// </summary>
        public string Label_P03_05
        {
            get => _label_p03_05;
            private set => SetProperty(ref _label_p03_05, value);
        }

        /// <summary>
        /// Libellé multilingue de l'en-tête de colonne « Objet »
        /// commun aux deux ListViews, lié à la clé <c>P03_06</c> du
        /// dictionnaire actif.
        /// </summary>
        public string Label_P03_06
        {
            get => _label_p03_06;
            private set => SetProperty(ref _label_p03_06, value);
        }

        /// <summary>
        /// Libellé multilingue du titre « Objet : » de l'onglet
        /// Détail, lié à la clé <c>P03_07</c> du dictionnaire actif.
        /// </summary>
        public string Label_P03_07
        {
            get => _label_p03_07;
            private set => SetProperty(ref _label_p03_07, value);
        }

        /// <summary>
        /// Libellé multilingue « Reçu le : » de l'onglet Détail, lié
        /// à la clé <c>P03_08</c> du dictionnaire actif. Consommé
        /// par <see cref="UpdateDateTitleLabel"/> en branche
        /// « message reçu sélectionné ».
        /// </summary>
        public string Label_P03_08
        {
            get => _label_p03_08;
            private set => SetProperty(ref _label_p03_08, value);
        }

        /// <summary>
        /// Libellé multilingue « Envoyé le : » de l'onglet Détail,
        /// lié à la clé <c>P03_09</c> du dictionnaire actif.
        /// Consommé par <see cref="UpdateDateTitleLabel"/> en
        /// branche « message envoyé sélectionné ».
        /// </summary>
        public string Label_P03_09
        {
            get => _label_p03_09;
            private set => SetProperty(ref _label_p03_09, value);
        }

        /// <summary>
        /// Libellé multilingue du titre « Contenu : » de l'onglet
        /// Détail, lié à la clé <c>P03_10</c> du dictionnaire actif.
        /// </summary>
        public string Label_P03_10
        {
            get => _label_p03_10;
            private set => SetProperty(ref _label_p03_10, value);
        }

        /// <summary>
        /// Libellé contextuel de la date affichée dans l'onglet
        /// Détail, recalculé dynamiquement selon la nature
        /// — réception ou émission — du message sélectionné.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable contextuelle non
        /// adossée à une clé directe du dictionnaire. Sa valeur est
        /// recalculée par <see cref="UpdateDateTitleLabel"/>,
        /// invoquée depuis trois sites :</para>
        /// <list type="bullet">
        ///   <item><description>Le setter de
        ///   <see cref="SelectedReceivedMessage"/> après affectation
        ///   du champ.</description></item>
        ///   <item><description>Le setter de
        ///   <see cref="SelectedSentMessage"/> après affectation du
        ///   champ.</description></item>
        ///   <item><description>L'override <see cref="LoadLabels"/>
        ///   en dernière instruction, pour propagation du
        ///   rechargement de langue dynamique sur le libellé
        ///   contextuel après réaffectation des deux libellés
        ///   sources <see cref="Label_P03_08"/> et
        ///   <see cref="Label_P03_09"/>.</description></item>
        /// </list>
        /// <para>Règle de calcul : si
        /// <c>_selectedReceivedMessage != null</c> alors
        /// <see cref="DateTitleLabel"/> vaut
        /// <see cref="Label_P03_08"/> (« Reçu le : ») ; sinon si
        /// <c>_selectedSentMessage != null</c> alors
        /// <see cref="DateTitleLabel"/> vaut
        /// <see cref="Label_P03_09"/> (« Envoyé le : ») ; sinon
        /// <see cref="DateTitleLabel"/> vaut
        /// <see cref="string.Empty"/>.</para>
        /// </remarks>
        public string DateTitleLabel
        {
            get => _dateTitleLabel;
            private set => SetProperty(ref _dateTitleLabel, value);
        }

        /// <summary>
        /// Collection observable des messages reçus par
        /// l'application courante (boîte de réception), liée au
        /// <c>ItemsSource</c> de la ListView de l'onglet
        /// « Messages reçus ».
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Collection observable initialisée à vide
        /// par déclaration, jamais réaffectée. Conformément au
        /// patron idiomatique WPF de mutation en place,
        /// l'alimentation s'effectue exclusivement par
        /// <see cref="LoadAsync"/> via <c>Clear()</c> suivi d'autant
        /// d'<c>Add(...)</c> que nécessaire, après réception de la
        /// liste retournée par
        /// <see cref="IQ_UserAppMessage.HandleGetMessagesReceivedAsync"/>.
        /// Le tri descendant par <see cref="UserAppMessage.SentAt"/>
        /// est porté en interne par le Query Handler amont ; aucune
        /// transformation supplémentaire n'est portée par le présent
        /// ViewModel.</para>
        /// </remarks>
        public ObservableCollection<UserAppMessage> MessagesReceived { get; } = new();

        /// <summary>
        /// Collection observable des messages émis par
        /// l'application courante (boîte d'envoi), liée au
        /// <c>ItemsSource</c> de la ListView de l'onglet
        /// « Messages envoyés ».
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Symétrique de
        /// <see cref="MessagesReceived"/>. Alimentation par
        /// <see cref="LoadAsync"/> via <c>Clear()</c> +
        /// boucle <c>Add(...)</c> après réception de la liste
        /// retournée par
        /// <see cref="IQ_UserAppMessage.HandleGetMessagesSentAsync"/>.
        /// Tri descendant par <see cref="UserAppMessage.SentAt"/>
        /// porté en interne par le Query Handler amont.</para>
        /// </remarks>
        public ObservableCollection<UserAppMessage> MessagesSent { get; } = new();

        /// <summary>
        /// Message sélectionné dans la ListView de l'onglet
        /// « Messages reçus », lié au <c>SelectedItem</c> de la
        /// ListView correspondante.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable nullable. Le setter
        /// porte la logique métier de coordination des sélections,
        /// du libellé contextuel et du marquage en lecture.</para>
        /// <para>Comportement du setter (sur affectation
        /// effective) :</para>
        /// <list type="bullet">
        ///   <item><description>Si la nouvelle valeur est non
        ///   nulle : affectation de <see cref="SelectedMessage"/>
        ///   à cette valeur, déclenchant par propagation la
        ///   sélection automatique de l'onglet Détail via le
        ///   setter de <see cref="SelectedMessage"/> ; appel à
        ///   <see cref="UpdateDateTitleLabel"/> pour recalcul du
        ///   libellé contextuel sur la branche
        ///   <see cref="Label_P03_08"/> ; et, si le message n'est
        ///   pas encore lu (<see cref="UserAppMessage.IsRead"/>
        ///   <see langword="false"/>), déclenchement en
        ///   fire-and-forget de
        ///   <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/>
        ///   (arbitrage Q1=A du fil de fabrique : auto-marquage en
        ///   lecture legacy, le setter n'attend pas la terminaison
        ///   de la <see cref="System.Threading.Tasks.Task"/>, la
        ///   réactivité de l'UI est préservée au prix d'une
        ///   cohérence d'erreur dégradée explicitement
        ///   admise).</description></item>
        ///   <item><description>Si la nouvelle valeur est nulle
        ///   (déselection programmée par le code consommateur) :
        ///   aucune action métier subséquente. La propagation à
        ///   <see cref="SelectedMessage"/> n'est pas
        ///   effectuée.</description></item>
        /// </list>
        /// </remarks>
        public UserAppMessage? SelectedReceivedMessage
        {
            get => _selectedReceivedMessage;
            set
            {
                if (SetProperty(ref _selectedReceivedMessage, value) && value != null)
                {
                    SelectedMessage = value;
                    UpdateDateTitleLabel();
                    if (!value.IsRead)
                    {
                        _ = MarkSelectedAsReadAndCheckUnreadAsync(value.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Message sélectionné dans la ListView de l'onglet
        /// « Messages envoyés », lié au <c>SelectedItem</c> de la
        /// ListView correspondante.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Symétrique de
        /// <see cref="SelectedReceivedMessage"/> sans la branche
        /// d'auto-marquage en lecture (un message émis par
        /// l'application n'a pas d'état de lecture côté
        /// destinataire dans le périmètre du présent
        /// ViewModel).</para>
        /// <para>Comportement du setter (sur affectation effective,
        /// valeur non nulle) : affectation de
        /// <see cref="SelectedMessage"/> et appel à
        /// <see cref="UpdateDateTitleLabel"/> pour recalcul du
        /// libellé contextuel sur la branche
        /// <see cref="Label_P03_09"/>.</para>
        /// </remarks>
        public UserAppMessage? SelectedSentMessage
        {
            get => _selectedSentMessage;
            set
            {
                if (SetProperty(ref _selectedSentMessage, value) && value != null)
                {
                    SelectedMessage = value;
                    UpdateDateTitleLabel();
                }
            }
        }

        /// <summary>
        /// Message effectivement affiché dans l'onglet Détail,
        /// agrégat des deux sélections amont
        /// (<see cref="SelectedReceivedMessage"/> ou
        /// <see cref="SelectedSentMessage"/>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable nullable.
        /// Alimentée exclusivement par les setters des deux
        /// propriétés de sélection amont, jamais directement par la
        /// vue. Liée par les bindings du sous-Grid de l'onglet
        /// Détail aux champs <see cref="UserAppMessage.Subject"/>,
        /// <see cref="UserAppMessage.SentAt"/> (via
        /// <c>UT_DateFormatConverter</c>) et
        /// <see cref="UserAppMessage.Content"/>.</para>
        /// <para>Comportement du setter : sur affectation effective,
        /// <see cref="SelectTabItemMessage"/> est aligné sur
        /// <c>value != null</c>, déclenchant la sélection
        /// automatique de l'onglet Détail dès qu'un message est
        /// sélectionné dans l'une des deux ListViews amont.</para>
        /// </remarks>
        public UserAppMessage? SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                if (SetProperty(ref _selectedMessage, value))
                {
                    SelectTabItemMessage = value != null;
                }
            }
        }

        /// <summary>
        /// Pilote la sélection automatique de l'onglet Détail à
        /// chaque nouvelle sélection non nulle dans l'une des deux
        /// ListViews amont, lié à la propriété
        /// <c>IsSelected</c> du TabItem Détail du XAML.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété booléenne observable
        /// alimentée exclusivement par le setter de
        /// <see cref="SelectedMessage"/>. Aucune logique métier
        /// supplémentaire dans le setter.</para>
        /// </remarks>
        public bool SelectTabItemMessage
        {
            get => _selectTabItemMessage;
            set => SetProperty(ref _selectTabItemMessage, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Page03"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur DI
        /// lors de la résolution du Singleton <see cref="VM_Page03"/>
        /// par la vue <c>Page03</c> via
        /// <c>App.ServiceProvider.GetRequiredService</c> dans son
        /// propre constructeur (EA-02 Service Locator de
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
        ///   paramètres socle, stocke <paramref name="dictionary"/>
        ///   et <paramref name="logAndNotify"/> en champs
        ///   <c>protected</c> accessibles aux dérivés, stocke
        ///   <paramref name="app"/> en champ <c>private</c> non
        ///   hérité (encapsulation de la mécanique multilingue,
        ///   conformément à I-4.11.11 du 0231), et initialise le
        ///   champ <c>_callee</c> via
        ///   <c>GetType().Name</c>.</description></item>
        ///   <item><description>Gardes
        ///   <see cref="ArgumentNullException"/> locales sur les
        ///   deux dépendances propres au dérivé
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
        ///   peuplant les onze propriétés multilingues avant le
        ///   premier binding WPF de la vue, et branchement de
        ///   l'abonnement INPC interne à <see cref="ISE_App"/> pour
        ///   la prise en compte du changement de langue
        ///   dynamique (R-4.11.8 et R-4.11.9 du 0231).</description></item>
        /// </list>
        ///
        /// <para>Règle d'invocation d'<c>InitializeLabels</c>
        /// (R-4.11.8 du 0231) : L'appel à
        /// <see cref="VM_Generic.InitializeLabels"/> est
        /// exclusivement effectué dans le constructeur du
        /// ViewModel dérivé concret final, en dernière instruction,
        /// après l'affectation de toutes les dépendances propres.
        /// Cette règle prévient l'écueil classique de l'invocation
        /// virtuelle dans le constructeur d'une classe de base avec
        /// dépendances dérivées non encore initialisées.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_Page_Generic"/>
        /// via <c>base(...)</c>. Injecté en Singleton par le
        /// conteneur DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_Page_Generic"/> via <c>base(...)</c>.
        /// Mobilisé par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> au sein de
        /// <see cref="LoadAsync"/> et de
        /// <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/>.
        /// Injecté en Singleton par le conteneur DI au titre de
        /// l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par l'ancêtre commun (abonnement
        /// INPC interne à <see cref="ISE_App.AppCultureCode"/>). Le
        /// présent dérivé ne stocke pas cette dépendance ni n'y
        /// accède directement, conformément à I-4.11.11 du 0231.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="appContext">Service applicatif Singleton
        /// fournissant un instantané (<see cref="DTO_AppContext"/>)
        /// du contexte applicatif et utilisateur courant. Mobilisé
        /// par <see cref="LoadAsync"/> pour la lecture du champ
        /// <see cref="DTO_AppContext.AppId"/>. Injecté en Singleton
        /// par le conteneur DI.</param>
        /// <param name="useCaseInvoker">Composant Singleton porteur
        /// d'EA-11, unique voie d'invocation des quatre contrats
        /// consommés par le présent ViewModel
        /// (<see cref="IQ_UserAppMessage"/> pour deux invocations
        /// dans <see cref="LoadAsync"/>,
        /// <see cref="IU_UserAppMessage_MarkAsRead"/> et
        /// <see cref="IU_UserAppMessage_CheckUnread"/> dans
        /// <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/>).
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="appContext"/> ou
        /// <paramref name="useCaseInvoker"/> est
        /// <see langword="null"/>. Les gardes sur
        /// <paramref name="dictionary"/>,
        /// <paramref name="logAndNotify"/> et
        /// <paramref name="app"/> sont portées par
        /// <see cref="VM_Generic"/> via la chaîne
        /// <c>base(...)</c>.</exception>
        public VM_Page03(
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
        /// <see cref="VM_Page_Generic.LoadAsync"/> pour alimenter
        /// les deux collections <see cref="MessagesReceived"/> et
        /// <see cref="MessagesSent"/> par deux invocations
        /// consécutives du Query Handler
        /// <see cref="IQ_UserAppMessage"/> via
        /// <see cref="IS_UseCaseInvoker"/> (EA-11).
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// l'orchestrateur appelant côté <c>Page_Generic</c> et
        /// propagée par le code-behind via
        /// <c>_viewModel.LoadAsync(callChain, ct)</c>. Le paramètre
        /// est reçu par contrat du hook au socle
        /// <see cref="VM_Page_Generic"/> mais n'est pas consommé
        /// par le corps du présent override : une CallChain
        /// interne distincte est construite via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> et
        /// consommée par <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// et par les deux délégués d'invocation du Query Handler,
        /// conformément au patron de surcharge normatif §4.15.6 du
        /// 0230.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé
        /// par le code-behind appelant. Propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, à
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}"/>
        /// et, par les délégués, au Query Handler. Valeur par
        /// défaut : <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// du chargement des deux collections
        /// observables.</returns>
        /// <remarks>
        /// <para>Contexte : Override du hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> déclaré
        /// <c>public virtual</c> au socle conformément à §4.15.6
        /// du 0230. Invoquée depuis le code-behind de <c>Page03</c>
        /// au point d'extension <c>OnLoadedAsync</c> exposé par
        /// <c>Page_Generic</c> (§4.15.7 du 0230). Méthode
        /// strictement disjointe de <see cref="LoadLabels"/> :
        /// libellés synchrones au constructeur d'un côté, données
        /// fonctionnelles et collections observables asynchrones
        /// au <c>Loaded</c> de la page de l'autre.</para>
        ///
        /// <para>Objectif : Déléguer à la méthode privée
        /// <see cref="LoadMessagesAsync"/> le chargement des
        /// deux collections observables sous protection du filet
        /// hérité, en propageant la CallChain interne enrichie et
        /// le <see cref="CancellationToken"/> reçu du
        /// code-behind. La séquentialité des deux invocations
        /// consécutives du Query Handler
        /// <see cref="IQ_UserAppMessage"/>, le snapshot atomique
        /// du contexte applicatif via
        /// <see cref="IS_AppContext.GetAppContext"/> et
        /// l'alimentation des deux <see cref="MessagesReceived"/>
        /// et <see cref="MessagesSent"/> par <c>Clear()</c> +
        /// boucle <c>Add(...)</c> sont portés intégralement par
        /// <see cref="LoadMessagesAsync"/> ; la présente méthode
        /// n'orchestre que la construction de la CallChain
        /// initiale, l'ouverture du filet de sécurité et la
        /// délégation.</para>
        ///
        /// <para>Chaînage CallChain (§4.5.1 du 0230) : La CallChain
        /// initiale est construite par
        /// <see cref="VM_Generic.BuildFirstCallChain"/> hérité
        /// (patron point d'entrée, produit
        /// <c>"VM_Page03 &gt; LoadAsync"</c>) et transmise telle
        /// quelle à <see cref="LoadMessagesAsync"/> en argument
        /// <c>caller</c>. La méthode privée applique le patron
        /// méthode privée et enrichit localement la chaîne
        /// (<c>"VM_Page03 &gt; LoadAsync &gt; LoadMessagesAsync"</c>)
        /// avant transmission aux invocations du Query
        /// Handler.</para>
        ///
        /// <para>Ordre de tri : Le tri descendant par
        /// <see cref="UserAppMessage.SentAt"/> est porté en interne
        /// par le Query Handler amont (LINQ-to-Objects
        /// post-matérialisation, §4.14.6 du 0230). Aucune
        /// transformation supplémentaire — tri, projection,
        /// filtrage additionnel — n'est portée par le présent
        /// override, conformément à I-4.14.4 amendée et à la stricte
        /// séparation CQRS.</para>
        ///
        /// <para>Filet de sécurité : L'invocation est encapsulée
        /// par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> (§4.7.3 du
        /// 0230). Toute exception applicative typée non capturée
        /// par le pipeline du Query Handler amont est traitée
        /// terminalement par <see cref="IU_LogAndNotify"/> via le
        /// filet hérité. Le présent override ne pose aucun
        /// try/catch local : les défaillances métier
        /// (<c>Ex_Business</c>) et infrastructure
        /// (<c>Ex_Infrastructure</c>) éventuellement levées par le
        /// Query Handler sont absorbées par
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> selon le
        /// pipeline canonique à cinq captures.</para>
        ///
        /// <para>Idempotence : La méthode est ré-appelable à chaque
        /// entrée sur la page sans flag de mémoire d'état. Chaque
        /// appel produit deux nouvelles invocations complètes du
        /// Query Handler et deux nouvelles alimentations par
        /// <c>Clear()</c> + boucle d'<c>Add(...)</c>.</para>
        ///
        /// <para>Mode d'invocation : Conformément à §4.10.10 du
        /// 0230 et à I-4.10.10 du 0231, les deux invocations du
        /// Query Handler générique <see cref="IQ_UserAppMessage"/>
        /// transitent par <see cref="IS_UseCaseInvoker"/> qui
        /// matérialise un <c>IServiceScope</c> distinct pour
        /// chaque invocation, y résout l'implémentation du contrat
        /// et l'exécute via le délégué fourni, puis dispose le
        /// scope. Le présent ViewModel n'injecte pas directement
        /// le contrat <see cref="IQ_UserAppMessage"/>.</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée silencieusement à l'appelant sur signal
        /// d'annulation coopérative par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, conformément
        /// à §4.7.3 du 0230. Aucune journalisation ni notification.
        /// </exception>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                await LoadMessagesAsync(innerCallChain, ct);
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> pour charger les dix
        /// libellés multilingues affichés par la page <c>Page03</c>
        /// à partir des clés <c>P03_01</c> à <c>P03_10</c> du
        /// dictionnaire de langue actif, et propager le
        /// rechargement sur la propriété contextuelle
        /// <see cref="DateTitleLabel"/>.
        /// </summary>
        /// <param name="caller">CallChain construite par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// (premier chargement) ou par le handler interne
        /// d'abonnement INPC de <see cref="VM_Generic"/> au
        /// changement de langue dynamique (rechargement), et
        /// transmise au service de dictionnaire pour traçabilité.</param>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissant le point
        /// d'extension <see cref="VM_Generic.LoadLabels"/>
        /// conformément à R-4.11.8 du 0231. Invoquée par
        /// <see cref="VM_Generic.InitializeLabels"/> au
        /// constructeur pour le premier chargement, puis par le
        /// handler interne d'abonnement INPC de
        /// <see cref="VM_Generic"/> à chaque changement de langue
        /// dynamique notifié par
        /// <see cref="ISE_App.AppCultureCode"/>, avec marshalling
        /// Dispatcher défensif vers le thread UI. Ne touche pas aux
        /// deux collections observables ni aux quatre propriétés de
        /// sélection, dont les alimentations sont portées par
        /// <see cref="LoadAsync"/> et par les setters des
        /// propriétés de sélection.</para>
        ///
        /// <para>Objectif : Garantir que les dix propriétés
        /// <c>Label_P03_NN</c> sont synchronisées avec la langue
        /// active du dictionnaire, tant au moment de l'instanciation
        /// du ViewModel que lors de tout changement ultérieur de
        /// langue dynamique au cours de la session, et propager ce
        /// rechargement sur le libellé contextuel
        /// <see cref="DateTitleLabel"/>.</para>
        ///
        /// <para>Patron strict : Une affectation par ligne, dans
        /// l'ordre numérique croissant des clés (<c>P03_01</c>,
        /// <c>P03_02</c>, …, <c>P03_10</c>), sans regroupement et
        /// sans condition. Aucun raccourci de type boucle
        /// dynamique : la résolution nominative permet une revue de
        /// code aisée et un repérage statique des clés consommées.
        /// Les clés <c>P03_00</c> (nom de la page, convention
        /// transverse) et <c>P03_11</c> à <c>P03_19</c> (libellés
        /// provision non consommés par la composition visuelle
        /// courante) ne sont volontairement pas chargées par le
        /// présent override : leur présence dans le dictionnaire
        /// est conservée à titre de matière première pour
        /// d'éventuelles évolutions ultérieures (arbitrage Q10=A du
        /// fil de fabrique).</para>
        ///
        /// <para>Propagation au libellé contextuel : Appel final
        /// à <see cref="UpdateDateTitleLabel"/> en clôture de la
        /// méthode. Cet appel est indispensable pour que la
        /// propriété <see cref="DateTitleLabel"/> soit rafraîchie
        /// après réaffectation des deux libellés sources
        /// <see cref="Label_P03_08"/> et <see cref="Label_P03_09"/>
        /// au changement de langue dynamique (arbitrage Q3=A du
        /// fil de fabrique).</para>
        ///
        /// <para>Absence d'appel à <c>base.LoadLabels(caller)</c> :
        /// L'implémentation par défaut de
        /// <see cref="VM_Generic.LoadLabels"/> ne porte aucun
        /// traitement. L'appel à <c>base.LoadLabels(caller)</c>
        /// n'apporterait qu'un bruit inutile et est délibérément
        /// omis, conformément à la pratique standard d'override
        /// lorsque la base ne porte aucun traitement, alignée sur
        /// le patron de <c>VM_Page98.LoadLabels</c>,
        /// <c>VM_Page99.LoadLabels</c> et
        /// <c>VM_Page01.LoadLabels</c>.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local n'est
        /// posé. Le filet est porté exclusivement par
        /// <c>SR_Dictionary</c> conformément à R-4.11.6 et
        /// R-4.11.10 du 0231 — toute anomalie (clé absente, erreur
        /// inattendue) est journalisée en interne par
        /// <c>SR_Dictionary</c> et résolue par une valeur de repli
        /// <c>[P03_NN] not found</c>, sans interruption ni
        /// propagation d'exception au présent ViewModel.</para>
        /// </remarks>
        protected override void LoadLabels(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadLabels)}";

            Label_P03_01 = _dictionary.GetText(callChain, "P03_01");
            Label_P03_02 = _dictionary.GetText(callChain, "P03_02");
            Label_P03_03 = _dictionary.GetText(callChain, "P03_03");
            Label_P03_04 = _dictionary.GetText(callChain, "P03_04");
            Label_P03_05 = _dictionary.GetText(callChain, "P03_05");
            Label_P03_06 = _dictionary.GetText(callChain, "P03_06");
            Label_P03_07 = _dictionary.GetText(callChain, "P03_07");
            Label_P03_08 = _dictionary.GetText(callChain, "P03_08");
            Label_P03_09 = _dictionary.GetText(callChain, "P03_09");
            Label_P03_10 = _dictionary.GetText(callChain, "P03_10");

            UpdateDateTitleLabel();
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Recalcule la valeur de <see cref="DateTitleLabel"/> selon
        /// la nature — réception ou émission — du message
        /// sélectionné.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée invoquée depuis trois
        /// sites distincts (les setters de
        /// <see cref="SelectedReceivedMessage"/> et
        /// <see cref="SelectedSentMessage"/>, et l'override
        /// <see cref="LoadLabels"/> en clôture). Calcul local
        /// synchrone, sans CallChain, sans accès au dictionnaire :
        /// l'opération se limite à un aiguillage à trois branches
        /// sur les deux champs support
        /// <c>_selectedReceivedMessage</c> et
        /// <c>_selectedSentMessage</c> et à une affectation à la
        /// propriété <see cref="DateTitleLabel"/>.</para>
        /// <para>Règle de calcul (arbitrage Q3=A du fil de
        /// fabrique) :</para>
        /// <list type="bullet">
        ///   <item><description>Si
        ///   <c>_selectedReceivedMessage</c> non nul :
        ///   <see cref="DateTitleLabel"/> ←
        ///   <see cref="Label_P03_08"/> (« Reçu le : »).</description></item>
        ///   <item><description>Sinon si
        ///   <c>_selectedSentMessage</c> non nul :
        ///   <see cref="DateTitleLabel"/> ←
        ///   <see cref="Label_P03_09"/> (« Envoyé le : »).</description></item>
        ///   <item><description>Sinon :
        ///   <see cref="DateTitleLabel"/> ←
        ///   <see cref="string.Empty"/>.</description></item>
        /// </list>
        /// <para>Lecture des champs support plutôt que des
        /// propriétés : le calcul lit
        /// <c>_selectedReceivedMessage</c> et
        /// <c>_selectedSentMessage</c> directement, sans passer par
        /// les propriétés publiques. Ce choix prévient toute
        /// invocation récursive du setter d'une propriété de
        /// sélection au cas où une logique métier ultérieure y
        /// serait ajoutée. Conformité au patron d'aiguillage
        /// idiomatique des ViewModels.</para>
        /// </remarks>
        private void UpdateDateTitleLabel()
        {
            string value = _selectedReceivedMessage != null
                ? Label_P03_08
                : _selectedSentMessage != null
                    ? Label_P03_09
                    : string.Empty;

            DateTitleLabel = value;
        }

        /// <summary>
        /// Marque en lecture le message reçu sélectionné, puis
        /// déclenche la propagation de l'état « messages non lus »
        /// sur <see cref="ISE_App"/>, par deux invocations
        /// chaînées sous filet hérité.
        /// </summary>
        /// <param name="messageId">Identifiant
        /// (<see cref="UserAppMessage.Id"/>) du message à marquer en
        /// lecture, capturé par le setter de
        /// <see cref="SelectedReceivedMessage"/> à l'instant de la
        /// sélection.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// du marquage et de la propagation chaînée. La tâche est
        /// systématiquement consommée en mode fire-and-forget par
        /// l'appelant (setter de
        /// <see cref="SelectedReceivedMessage"/>) ; sa terminaison
        /// n'est jamais attendue par le code consommateur.</returns>
        /// <remarks>
        /// <para>Contexte : Méthode privée invoquée exclusivement
        /// par le setter de <see cref="SelectedReceivedMessage"/>
        /// en mode fire-and-forget via l'instruction
        /// <c>_ = MarkSelectedAsReadAndCheckUnreadAsync(value.Id)</c>
        /// (arbitrage Q1=A du fil de fabrique : auto-marquage en
        /// lecture legacy, le setter n'attend pas la terminaison
        /// de la <see cref="System.Threading.Tasks.Task"/>, la
        /// réactivité de l'UI est préservée au prix d'une cohérence
        /// d'erreur dégradée explicitement admise).</para>
        ///
        /// <para>Objectif : Enchaîner deux invocations sous le même
        /// filet de sécurité hérité :</para>
        /// <list type="number">
        ///   <item><description>Invocation 1 :
        ///   <see cref="IU_UserAppMessage_MarkAsRead.ExecuteAsync"/>
        ///   avec <paramref name="messageId"/> en paramètre,
        ///   bascule de <see cref="UserAppMessage.IsRead"/> à
        ///   <see langword="true"/> en
        ///   persistance.</description></item>
        ///   <item><description>Invocation 2 :
        ///   <see cref="IU_UserAppMessage_CheckUnread.ExecuteAsync"/>,
        ///   propagation sur <see cref="ISE_App"/> du basculement
        ///   éventuel de l'état « messages non lus » suite à la
        ///   lecture qui vient d'être opérée (arbitrage Q5=B du fil
        ///   de fabrique : propagation immédiate nécessaire pour
        ///   que l'icône de notification du Banner — Chantier 3,
        ///   hors périmètre du présent fil — bascule sans attendre
        ///   le prochain tick du polling 60s).</description></item>
        /// </list>
        ///
        /// <para>CancellationToken : Le contexte fire-and-forget
        /// depuis un setter de propriété n'a pas accès à un
        /// <see cref="CancellationToken"/> d'appelant. Conformément
        /// à l'arbitrage Q5=B du fil de fabrique,
        /// <see cref="CancellationToken.None"/> est propagé
        /// symétriquement aux deux invocations.</para>
        ///
        /// <para>Filet de sécurité : Les deux invocations sont
        /// encapsulées dans un unique <c>ExecuteSafeAsync</c>
        /// hérité de <see cref="VM_Generic"/>. Toute exception
        /// applicative typée non capturée par le pipeline des
        /// UseCases amont est traitée terminalement par
        /// <see cref="IU_LogAndNotify"/> via le filet hérité.
        /// Conformément au mode fire-and-forget, l'éventuelle
        /// exception est traitée par le filet sans remonter au
        /// setter appelant.</para>
        ///
        /// <para>Mode d'invocation : Conformément à §4.10.10 du
        /// 0230 et à I-4.10.10 du 0231, les deux invocations
        /// transitent par <see cref="IS_UseCaseInvoker"/> qui
        /// matérialise un <c>IServiceScope</c> distinct pour
        /// chaque invocation, y résout l'implémentation du contrat
        /// et l'exécute via le délégué fourni, puis dispose le
        /// scope. Le présent ViewModel n'injecte pas directement
        /// les contrats <see cref="IU_UserAppMessage_MarkAsRead"/>
        /// ni <see cref="IU_UserAppMessage_CheckUnread"/>.</para>
        /// </remarks>
        private async Task MarkSelectedAsReadAndCheckUnreadAsync(int messageId)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                await _useCaseInvoker
                    .InvokeAsync<IU_UserAppMessage_MarkAsRead>(
                        (uc, innerCt) => uc.ExecuteAsync(
                            innerCallChain,
                            messageId,
                            innerCt),
                        CancellationToken.None);

                await _useCaseInvoker
                    .InvokeAsync<IU_UserAppMessage_CheckUnread>(
                        (uc, innerCt) => uc.ExecuteAsync(
                            innerCallChain,
                            innerCt),
                        CancellationToken.None);

                await LoadMessagesAsync(innerCallChain, CancellationToken.None);
            });
        }

        /// <summary>
        /// Charge — ou recharge — en deux invocations consécutives
        /// du Query Handler <see cref="IQ_UserAppMessage"/> via
        /// <see cref="IS_UseCaseInvoker"/> (EA-11) les deux
        /// collections observables <see cref="MessagesReceived"/>
        /// et <see cref="MessagesSent"/>.
        /// </summary>
        /// <param name="caller">CallChain reçue de la méthode
        /// appelante, à enrichir localement selon le patron
        /// méthode privée de §4.5.1 du 0230 avant transmission au
        /// Query Handler.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé
        /// par la méthode appelante. Transmis tel quel aux deux
        /// invocations de
        /// <see cref="IS_UseCaseInvoker"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// du chargement des deux collections
        /// observables.</returns>
        /// <remarks>
        /// <para>Contexte : Méthode privée extraite du corps
        /// initial de <see cref="LoadAsync"/>, consommée à deux
        /// sites symétriques :</para>
        /// <list type="bullet">
        ///   <item><description>Depuis <see cref="LoadAsync"/>
        ///   pour le chargement initial au montage de la page,
        ///   avec propagation du
        ///   <see cref="CancellationToken"/> reçu du
        ///   code-behind.</description></item>
        ///   <item><description>Depuis
        ///   <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/>
        ///   après le chaînage
        ///   <see cref="IU_UserAppMessage_MarkAsRead"/> +
        ///   <see cref="IU_UserAppMessage_CheckUnread"/>, pour
        ///   rafraîchir les deux collections après la mutation
        ///   d'état de lecture, avec propagation de
        ///   <see cref="CancellationToken.None"/> (contexte
        ///   fire-and-forget depuis un setter de propriété, cf.
        ///   <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/>).</description></item>
        /// </list>
        ///
        /// <para>Patron CallChain méthode privée (§4.5.1 du
        /// 0230) : La CallChain reçue en paramètre
        /// <paramref name="caller"/> est enrichie localement par
        /// concaténation avec <c>nameof(LoadMessagesAsync)</c>,
        /// sans inclusion du <c>_callee</c> (la classe a déjà
        /// été identifiée par la méthode publique appelante,
        /// conformément à la justification doctrinale de §4.5.1).
        /// La chaîne enrichie <c>callChain</c> est consommée par
        /// les deux délégués d'invocation du Query Handler.</para>
        ///
        /// <para>Objectif : Alimenter en trois temps coordonnés
        /// les deux collections observables :</para>
        /// <list type="number">
        ///   <item><description>Snapshot atomique du contexte
        ///   applicatif et utilisateur courant par un appel
        ///   unique à
        ///   <see cref="IS_AppContext.GetAppContext"/>, retourné
        ///   dans une variable locale <c>ctx</c> consommée par
        ///   les deux invocations suivantes. La discipline
        ///   d'appel unique garantit la cohérence intra-méthode :
        ///   les deux invocations du Query Handler lisent toutes
        ///   deux du même instantané, sans risque de
        ///   désynchronisation entre deux lectures successives du
        ///   Setting.</description></item>
        ///   <item><description>Invocation 1 :
        ///   <see cref="IQ_UserAppMessage.HandleGetMessagesReceivedAsync"/>
        ///   via <see cref="IS_UseCaseInvoker"/>, avec
        ///   <see cref="DTO_AppContext.AppId"/> en paramètre.
        ///   Alimentation de <see cref="MessagesReceived"/> par
        ///   <c>Clear()</c> + boucle
        ///   <c>Add(...)</c>.</description></item>
        ///   <item><description>Invocation 2 :
        ///   <see cref="IQ_UserAppMessage.HandleGetMessagesSentAsync"/>
        ///   via <see cref="IS_UseCaseInvoker"/>, avec
        ///   <see cref="DTO_AppContext.AppId"/> en paramètre.
        ///   Alimentation de <see cref="MessagesSent"/> par
        ///   <c>Clear()</c> + boucle
        ///   <c>Add(...)</c>.</description></item>
        /// </list>
        ///
        /// <para>Ordre de tri : Le tri descendant par
        /// <see cref="UserAppMessage.SentAt"/> est porté en
        /// interne par le Query Handler amont (LINQ-to-Objects
        /// post-matérialisation, §4.14.6 du 0230). Aucune
        /// transformation supplémentaire — tri, projection,
        /// filtrage additionnel — n'est portée par la présente
        /// méthode, conformément à I-4.14.4 amendée et à la
        /// stricte séparation CQRS.</para>
        ///
        /// <para>Filet de sécurité : Aucun filet local. La
        /// présente méthode privée s'exécute exclusivement sous
        /// le filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// porté par ses deux appelants
        /// (<see cref="LoadAsync"/> et
        /// <see cref="MarkSelectedAsReadAndCheckUnreadAsync"/>).
        /// Toute défaillance métier
        /// (<c>Ex_Business</c>) ou infrastructure
        /// (<c>Ex_Infrastructure</c>) éventuellement levée par
        /// l'une des deux invocations du Query Handler est
        /// absorbée par le filet de l'appelant selon le pipeline
        /// canonique à cinq captures (§4.7.3 du 0230). Cette
        /// économie est délibérée : matérialiser un filet local
        /// produirait une imbrication
        /// <c>ExecuteSafeAsync(ExecuteSafeAsync(...))</c> sans
        /// effet utile sur le pipeline d'erreur, qui reste
        /// nominal par la capture externe la plus proche.</para>
        ///
        /// <para>Idempotence : La méthode est ré-appelable sans
        /// flag de mémoire d'état. Chaque appel produit deux
        /// nouvelles invocations complètes du Query Handler et
        /// deux nouvelles alimentations par <c>Clear()</c> +
        /// boucle d'<c>Add(...)</c>.</para>
        ///
        /// <para>Mode d'invocation : Conformément à §4.10.10 du
        /// 0230 et à I-4.10.10 du 0231, les deux invocations du
        /// Query Handler générique
        /// <see cref="IQ_UserAppMessage"/> transitent par
        /// <see cref="IS_UseCaseInvoker"/> qui matérialise un
        /// <c>IServiceScope</c> distinct pour chaque invocation,
        /// y résout l'implémentation du contrat et l'exécute via
        /// le délégué fourni, puis dispose le scope.</para>
        /// </remarks>
        private async Task LoadMessagesAsync(string caller, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(LoadMessagesAsync)}";

            var ctx = _appContext.GetAppContext();

            var received = await _useCaseInvoker
                .InvokeAsync<IQ_UserAppMessage, List<UserAppMessage>>(
                    (qh, innerCt) => qh.HandleGetMessagesReceivedAsync(
                        callChain,
                        ctx.AppId,
                        innerCt),
                    ct);

            MessagesReceived.Clear();
            foreach (var m in received) MessagesReceived.Add(m);

            var sent = await _useCaseInvoker
                .InvokeAsync<IQ_UserAppMessage, List<UserAppMessage>>(
                    (qh, innerCt) => qh.HandleGetMessagesSentAsync(
                        callChain,
                        ctx.AppId,
                        innerCt),
                    ct);

            MessagesSent.Clear();
            foreach (var m in sent) MessagesSent.Add(m);
        }

        #endregion
    }
}
