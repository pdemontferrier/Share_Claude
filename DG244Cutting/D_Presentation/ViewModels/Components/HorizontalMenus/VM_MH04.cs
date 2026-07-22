using System.Windows.Input;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.ViewModels;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Components.HorizontalMenus
{
    /// <summary>
    /// ViewModel du menu horizontal associé à la
    /// <see cref="DG244Cutting.D_Presentation.ViewModels.Pages.VM_Page04"/>
    /// de l'application DG244Cutting, exposant à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Components.HorizontalMenus.MH04"/>
    /// les cinq commandes transverses standards héritées du socle
    /// <see cref="VM_MH_Generic"/>, augmentées de quatre commandes
    /// d'action métier propres — <see cref="NewCommand"/>,
    /// <see cref="AddCommand"/>, <see cref="ModifyCommand"/> et
    /// <see cref="SaveCommand"/> — relayées vers le contrat de page
    /// <see cref="IV_Page04"/> injecté, et de quatre libellés
    /// multilingues propres <see cref="Label_MH_New"/>,
    /// <see cref="Label_MH_Add"/>, <see cref="Label_MH_Modify"/> et
    /// <see cref="Label_MH_Save"/> alimentant les boutons correspondants.
    /// </summary>
    /// <remarks>
    /// <para>Contexte :</para>
    ///
    /// <para>ViewModel du menu horizontal de la page d'administration
    /// des comptes utilisateurs (Page04). Il complète le socle
    /// transverse par quatre points d'action métier pilotant la page
    /// courante : créer un nouvel enregistrement, ajouter, passer en
    /// édition, et enregistrer. Le pilotage n'est pas réalisé par un
    /// UseCase métier médiatisé, mais par relais vers le contrat de
    /// ViewModel de page <see cref="IV_Page04"/>.</para>
    ///
    /// <para>Objectif :</para>
    ///
    /// <para>Offrir à l'opérateur quatre boutons d'action dont la
    /// visibilité est conditionnée par ses droits sur la page courante
    /// (axe porté par la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Components.HorizontalMenus.MH04"/>
    /// via son override d'<c>ApplySecurityRules</c>) et dont
    /// l'activation est conditionnée par l'état d'édition de la page
    /// (axe porté ici par la garde <c>CanExecute</c> de chaque
    /// commande, composée de <see cref="VM_MH_Generic.IsProcessing"/>
    /// et de la garde correspondante du contrat
    /// <see cref="IV_Page04"/>). Ces deux axes sont orthogonaux : un
    /// bouton peut être visible mais désactivé.</para>
    ///
    /// <para>Responsabilités :</para>
    ///
    /// <para>Exposer les quatre commandes d'action
    /// <see cref="NewCommand"/>, <see cref="AddCommand"/>,
    /// <see cref="ModifyCommand"/>, <see cref="SaveCommand"/> et les
    /// quatre libellés observables associés, en sus des membres
    /// transverses hérités. Relayer chaque action vers le contrat
    /// <see cref="IV_Page04"/> dans le filet de sécurité hérité
    /// <see cref="VM_Generic.ExecuteSafeAsync"/> et sous garde
    /// d'anti-réentrance <see cref="VM_MH_Generic.IsProcessing"/>.
    /// Alimenter les libellés propres par surcharge nominative de
    /// <see cref="LoadLabels"/>.</para>
    ///
    /// <para>Non-responsabilités :</para>
    ///
    /// <para>N'effectue aucun traitement métier propre : les quatre
    /// commandes sont de purs relais vers <see cref="IV_Page04"/>, la
    /// logique métier étant portée par l'implémenteur du contrat. Ne
    /// consomme aucun UseCase métier via <c>IS_UseCaseInvoker</c>
    /// (EA-11 non mobilisée). Ne déclenche aucune navigation.
    /// N'évalue ni ne résout les droits d'accès, délégués à la vue et
    /// à <see cref="IU_Navigation"/>.</para>
    ///
    /// <para>Note sur les exceptions architecturales :</para>
    ///
    /// <para>Le pilotage de Page04 passe par le contrat
    /// <see cref="IV_Page04"/> (préfixe <c>IV_</c>, porté par
    /// A_Domain), dispositif de découplage par inversion de dépendance
    /// sur le modèle d'<c>IS_Navigation</c> : aucune référence concrète
    /// entre le présent ViewModel et le ViewModel de page. Le contrat
    /// <see cref="IV_Page04"/> étant un <c>IV_</c> et non un
    /// <c>IU_</c>/<c>IQ_</c>, l'interdiction I-4.10.10 (injection
    /// directe d'un UseCase/Query dans un VM_MHxx) ne s'applique pas
    /// et aucune EA-11 n'est mobilisée. L'EA-05 (consommation directe
    /// d'<see cref="IU_Navigation"/>) reste strictement cantonnée au
    /// socle <see cref="VM_MH_Generic"/> pour ses cinq commandes
    /// transverses et n'est pas étendue ici : les quatre dépendances
    /// de base (<see cref="IS_Dictionary"/>,
    /// <see cref="IU_LogAndNotify"/>, <see cref="ISE_App"/>,
    /// <see cref="IU_Navigation"/>) sont intégralement déléguées à
    /// <c>base(...)</c> sans rétention locale, seule la dépendance
    /// propre <see cref="IV_Page04"/> étant retenue.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2 du 0230), augmentée de deux extensions §4.4.3 :
    /// l'extension <c>=== Propriétés publiques ===</c> qui porte les
    /// huit membres exposés propres (quatre commandes et quatre
    /// libellés), et l'extension <c>=== Méthodes protégées ===</c>,
    /// rendue OBLIGATOIRE par la présence de la surcharge
    /// <c>protected override</c> <see cref="LoadLabels"/> (R-4.4.10,
    /// I-4.4.5 du 0231 ; §4.4.3 du 0230 : une méthode <c>protected</c>
    /// sous forme <c>override</c> relève de cette région et non de la
    /// région Méthodes publiques). La région Méthodes publiques est
    /// présente mais vide (marqueur <c>// A compléter</c>), le
    /// composant n'exposant aucune méthode <c>public</c> propre.
    /// L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente. Soit sept régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   porte les quatre champs backing des libellés propres
    ///   (<c>_label_mh_new</c>, <c>_label_mh_add</c>,
    ///   <c>_label_mh_modify</c>, <c>_label_mh_save</c>).</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   porte la dépendance propre <c>_page04</c>
    ///   (<see cref="IV_Page04"/>), seule dépendance retenue
    ///   localement.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c>
    ///   (extension §4.4.3) : porte les quatre commandes d'action et
    ///   les quatre libellés observables.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur <c>public</c> à cinq paramètres (les quatre du
    ///   socle puis <see cref="IV_Page04"/>), délégation à
    ///   <see cref="VM_MH_Generic"/> via <c>base(...)</c>, garde
    ///   non-nulle du cinquième paramètre, composition des quatre
    ///   commandes, et invocation d'
    ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction (R-4.11.8 du 0231).</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>. Aucun
    ///   override de <see cref="VM_MH_Generic.LoadAsync"/>, le
    ///   composant n'ayant pas de donnée métier à charger au
    ///   montage.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c>
    ///   (extension §4.4.3) : porte la surcharge nominative
    ///   <see cref="LoadLabels"/> qui alimente les quatre libellés
    ///   propres après appel de <c>base.LoadLabels(callChain)</c> en
    ///   première instruction.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   porte les quatre handlers <see cref="ExecuteNewAsync"/>,
    ///   <see cref="ExecuteAddAsync"/>, <see cref="ExecuteModifyAsync"/>
    ///   et <see cref="ExecuteSaveAsync"/> des commandes d'action
    ///   propres.</description></item>
    /// </list>
    /// </remarks>
    public class VM_MH04 : VM_MH_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Champ de stockage du libellé propre <see cref="Label_MH_New"/>,
        /// initialisé à la chaîne vide et alimenté par la surcharge
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// <c>MH_Ti_05</c>. Les mutations passent par <c>SetProperty</c>
        /// hérité de <see cref="VM_Generic"/> pour émettre la
        /// notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_new = string.Empty;

        /// <summary>
        /// Champ de stockage du libellé propre <see cref="Label_MH_Add"/>,
        /// initialisé à la chaîne vide et alimenté par la surcharge
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// <c>MH_Ti_06</c>. Les mutations passent par <c>SetProperty</c>
        /// hérité de <see cref="VM_Generic"/> pour émettre la
        /// notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_add = string.Empty;

        /// <summary>
        /// Champ de stockage du libellé propre
        /// <see cref="Label_MH_Modify"/>, initialisé à la chaîne vide et
        /// alimenté par la surcharge <see cref="LoadLabels"/> via la
        /// résolution de la clé <c>MH_Ti_08</c>. Les mutations passent
        /// par <c>SetProperty</c> hérité de <see cref="VM_Generic"/> pour
        /// émettre la notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_modify = string.Empty;

        /// <summary>
        /// Champ de stockage du libellé propre <see cref="Label_MH_Save"/>,
        /// initialisé à la chaîne vide et alimenté par la surcharge
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// <c>MH_Ti_09</c>. Les mutations passent par <c>SetProperty</c>
        /// hérité de <see cref="VM_Generic"/> pour émettre la
        /// notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_save = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Contrat de ViewModel de la page d'administration (Page04),
        /// vers lequel les quatre commandes d'action propres sont
        /// relayées. Dépendance propre injectée en cinquième paramètre
        /// du constructeur et retenue localement, sur le modèle de
        /// découplage par contrat partagé A_Domain
        /// (inversion de dépendance de type <c>IS_Navigation</c>).
        /// Consommée exclusivement au travers du filet de sécurité
        /// hérité <see cref="VM_Generic.ExecuteSafeAsync"/> ; ses gardes
        /// (<see cref="IV_Page04.CanEnterCreate"/>,
        /// <see cref="IV_Page04.CanAdd"/>,
        /// <see cref="IV_Page04.CanEnterEdit"/>,
        /// <see cref="IV_Page04.CanSave"/>) sont lues par les gardes
        /// <c>CanExecute</c> des commandes.
        /// </summary>
        private readonly IV_Page04 _page04;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Commande d'action « Nouveau » relayée vers
        /// <see cref="IV_Page04.EnterCreate"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteNewAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing &amp;&amp; _page04.CanEnterCreate</c>
        /// (anti-réentrance combinée à l'état d'édition de la page).
        /// La réévaluation de la garde est assurée par le canal natif
        /// <see cref="System.Windows.Input.CommandManager.RequerySuggested"/>
        /// d'<see cref="UT_RelayCommandArg0Async"/>.</para>
        /// <para>Sortie : relais vers <see cref="IV_Page04.EnterCreate"/>.
        /// Aucun cas d'échec métier propre : le traitement terminal des
        /// erreurs est délégué au filet
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>.</para>
        /// </remarks>
        public ICommand NewCommand { get; }

        /// <summary>
        /// Commande d'action « Ajouter » relayée vers
        /// <see cref="IV_Page04.AddAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteAddAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing &amp;&amp; _page04.CanAdd</c>. Réévaluation
        /// par <see cref="System.Windows.Input.CommandManager.RequerySuggested"/>.</para>
        /// <para>Sortie : relais vers <see cref="IV_Page04.AddAsync"/>,
        /// dans le filet <see cref="VM_Generic.ExecuteSafeAsync"/>.</para>
        /// </remarks>
        public ICommand AddCommand { get; }

        /// <summary>
        /// Commande d'action « Modifier » relayée vers
        /// <see cref="IV_Page04.EnterEdit"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteModifyAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing &amp;&amp; _page04.CanEnterEdit</c>.
        /// Réévaluation par
        /// <see cref="System.Windows.Input.CommandManager.RequerySuggested"/>.</para>
        /// <para>Sortie : relais vers <see cref="IV_Page04.EnterEdit"/>,
        /// dans le filet <see cref="VM_Generic.ExecuteSafeAsync"/>.</para>
        /// </remarks>
        public ICommand ModifyCommand { get; }

        /// <summary>
        /// Commande d'action « Enregistrer » relayée vers
        /// <see cref="IV_Page04.SaveAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteSaveAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing &amp;&amp; _page04.CanSave</c>. Réévaluation
        /// par <see cref="System.Windows.Input.CommandManager.RequerySuggested"/>.</para>
        /// <para>Sortie : relais vers <see cref="IV_Page04.SaveAsync"/>,
        /// dans le filet <see cref="VM_Generic.ExecuteSafeAsync"/>.</para>
        /// </remarks>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Libellé multilingue propre du bouton « Nouveau », bindé sur
        /// le <c>TextBlock</c> du bouton <c>MH_New</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_05</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Le setter est <c>private</c> :
        /// la propriété n'est mutée qu'en interne, via <c>SetProperty</c>
        /// hérité de <see cref="VM_Generic"/> pour émettre la
        /// notification <c>PropertyChanged</c>.</para>
        /// </remarks>
        public string Label_MH_New
        {
            get => _label_mh_new;
            private set => SetProperty(ref _label_mh_new, value);
        }

        /// <summary>
        /// Libellé multilingue propre du bouton « Ajouter », bindé sur
        /// le <c>TextBlock</c> du bouton <c>MH_Add</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_06</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Setter <c>private</c>, mutation via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_Add
        {
            get => _label_mh_add;
            private set => SetProperty(ref _label_mh_add, value);
        }

        /// <summary>
        /// Libellé multilingue propre du bouton « Modifier », bindé sur
        /// le <c>TextBlock</c> du bouton <c>MH_Modify</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_08</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Setter <c>private</c>, mutation via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_Modify
        {
            get => _label_mh_modify;
            private set => SetProperty(ref _label_mh_modify, value);
        }

        /// <summary>
        /// Libellé multilingue propre du bouton « Enregistrer », bindé
        /// sur le <c>TextBlock</c> du bouton <c>MH_Save</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_09</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Setter <c>private</c>, mutation via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_Save
        {
            get => _label_mh_save;
            private set => SetProperty(ref _label_mh_save, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_MH04"/>.
        /// </summary>
        /// <remarks>
        /// <para>Séquence d'initialisation :</para>
        ///
        /// <para>Délègue d'abord à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c> (validation non-nulle des quatre dépendances
        /// de base, composition des cinq commandes transverses). Garde
        /// ensuite la non-nullité de <paramref name="page04"/> et le
        /// retient dans le champ propre <c>_page04</c>. Compose les
        /// quatre commandes d'action propres (chaque garde
        /// <c>CanExecute</c> combinant <c>!IsProcessing</c> et la garde
        /// correspondante de <see cref="IV_Page04"/>). Invoque enfin
        /// <see cref="VM_Generic.InitializeLabels"/> en DERNIÈRE
        /// instruction (R-4.11.8 du 0231) afin de déclencher
        /// l'alimentation des libellés — les quatre transverses via
        /// <c>base.LoadLabels</c> et les quatre propres via la surcharge
        /// <see cref="LoadLabels"/>.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c>. Injecté en Singleton par le conteneur
        /// DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à <see cref="VM_MH_Generic"/>
        /// via <c>base(...)</c>. Mobilisé uniquement par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, non utilisé
        /// directement par le présent ViewModel. Injecté en Singleton
        /// par le conteneur DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par <see cref="VM_Generic"/>. Le
        /// présent dérivé ne stocke pas cette dépendance ni n'y accède
        /// directement, conformément à I-4.11.11 du 0231. Injecté en
        /// Singleton par le conteneur DI.</param>
        /// <param name="navigation">UseCase de navigation, transmis à
        /// <see cref="VM_MH_Generic"/> via <c>base(...)</c> pour ses
        /// cinq commandes transverses (EA-05). Le présent dérivé ne le
        /// retient pas localement et ne le consomme pas directement.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="page04">Contrat de ViewModel de la page
        /// d'administration (Page04), retenu dans le champ propre
        /// <c>_page04</c> et cible des relais des quatre commandes
        /// d'action. Résolu par inversion de dépendance A_Domain
        /// (renvoi <see cref="IV_Page04"/> vers l'implémenteur concret,
        /// porté par le conteneur DI).</param>
        /// <exception cref="ArgumentNullException">Levée par la chaîne
        /// <c>base(...)</c> si l'un des quatre paramètres de base est
        /// <see langword="null"/>, ou par la garde locale si
        /// <paramref name="page04"/> est <see langword="null"/>.</exception>
        public VM_MH04(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IU_Navigation navigation,
            IV_Page04 page04)
            : base(dictionary, logAndNotify, app, navigation)
        {
            _page04 = page04 ?? throw new ArgumentNullException(nameof(page04));

            NewCommand = new UT_RelayCommandArg0Async(
                ExecuteNewAsync, () => !IsProcessing && _page04.CanEnterCreate);
            AddCommand = new UT_RelayCommandArg0Async(
                ExecuteAddAsync, () => !IsProcessing && _page04.CanAdd);
            ModifyCommand = new UT_RelayCommandArg0Async(
                ExecuteModifyAsync, () => !IsProcessing && _page04.CanEnterEdit);
            SaveCommand = new UT_RelayCommandArg0Async(
                ExecuteSaveAsync, () => !IsProcessing && _page04.CanSave);

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Alimente les libellés multilingues du menu horizontal : les
        /// quatre libellés transverses du socle puis les quatre libellés
        /// d'action propres.
        /// </summary>
        /// <param name="callChain">CallChain courante propagée par la
        /// mécanique multilingue héritée
        /// (<see cref="VM_Generic.InitializeLabels"/> au premier appel,
        /// puis à chaque changement de langue), transmise telle quelle à
        /// <c>base.LoadLabels</c> et à
        /// <see cref="IS_Dictionary.GetText"/>.</param>
        /// <remarks>
        /// <para>Contexte : Surcharge nominative de
        /// <see cref="VM_MH_Generic.LoadLabels"/>. L'appel à
        /// <c>base.LoadLabels(callChain)</c> est IMPÉRATIVEMENT la
        /// première instruction du corps, afin de préserver
        /// l'alimentation des quatre libellés transverses du socle
        /// (<c>MH_Ti_01</c> à <c>MH_Ti_04</c>) : leur omission
        /// constituerait une non-conformité au contrat de la mécanique
        /// multilingue de la famille MH (§5.4.6 du 0232-MH-VM ;
        /// §4.15.8 du 0230). Les quatre libellés propres sont ensuite
        /// résolus depuis les clés <c>MH_Ti_05</c> (Nouveau),
        /// <c>MH_Ti_06</c> (Ajouter), <c>MH_Ti_08</c> (Modifier) et
        /// <c>MH_Ti_09</c> (Enregistrer).</para>
        /// </remarks>
        protected override void LoadLabels(string callChain)
        {
            base.LoadLabels(callChain);

            Label_MH_New = _dictionary.GetText(callChain, "MH_Ti_05");
            Label_MH_Add = _dictionary.GetText(callChain, "MH_Ti_06");
            Label_MH_Modify = _dictionary.GetText(callChain, "MH_Ti_08");
            Label_MH_Save = _dictionary.GetText(callChain, "MH_Ti_09");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de la commande <see cref="NewCommand"/> : relaie
        /// l'action « Nouveau » vers <see cref="IV_Page04.EnterCreate"/>,
        /// sur le patron strictement identique aux cinq handlers
        /// transverses du socle.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution du
        /// relais.</returns>
        /// <remarks>
        /// <para>Contexte : Encadre l'invocation par le pattern
        /// <c>BeginProcessing</c> / <c>try</c> / <c>finally</c> /
        /// <c>EndProcessing</c> (remise à <see langword="false"/> de
        /// <see cref="VM_MH_Generic.IsProcessing"/>) et par le filet de
        /// sécurité hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// alimenté par une CallChain initiale construite via
        /// <c>BuildFirstCallChain</c>. Le jeton d'annulation transmis au
        /// relais est <see cref="System.Threading.CancellationToken.None"/>
        /// (explicite). Le traitement terminal des erreurs est délégué au
        /// filet ; aucun cas d'échec métier propre n'est traité
        /// ici.</para>
        /// </remarks>
        private async Task ExecuteNewAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _page04.EnterCreate(callChain, CancellationToken.None);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="AddCommand"/> : relaie
        /// l'action « Ajouter » vers <see cref="IV_Page04.AddAsync"/>.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution du
        /// relais.</returns>
        /// <remarks>
        /// <para>Contexte : Même patron que <see cref="ExecuteNewAsync"/>
        /// (<c>BeginProcessing</c> / <c>try</c> / <c>ExecuteSafeAsync</c>
        /// / <c>finally</c> / <c>EndProcessing</c>), avec jeton
        /// <see cref="System.Threading.CancellationToken.None"/>
        /// explicite. Aucun cas d'échec métier propre.</para>
        /// </remarks>
        private async Task ExecuteAddAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _page04.AddAsync(callChain, CancellationToken.None);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="ModifyCommand"/> : relaie
        /// l'action « Modifier » vers <see cref="IV_Page04.EnterEdit"/>.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution du
        /// relais.</returns>
        /// <remarks>
        /// <para>Contexte : Même patron que <see cref="ExecuteNewAsync"/>,
        /// avec jeton
        /// <see cref="System.Threading.CancellationToken.None"/>
        /// explicite. Aucun cas d'échec métier propre.</para>
        /// </remarks>
        private async Task ExecuteModifyAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _page04.EnterEdit(callChain, CancellationToken.None);
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="SaveCommand"/> : relaie
        /// l'action « Enregistrer » vers <see cref="IV_Page04.SaveAsync"/>.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution du
        /// relais.</returns>
        /// <remarks>
        /// <para>Contexte : Même patron que <see cref="ExecuteNewAsync"/>,
        /// avec jeton
        /// <see cref="System.Threading.CancellationToken.None"/>
        /// explicite. Aucun cas d'échec métier propre.</para>
        /// </remarks>
        private async Task ExecuteSaveAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _page04.SaveAsync(callChain, CancellationToken.None);
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