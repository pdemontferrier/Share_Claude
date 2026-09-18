using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
    /// <see cref="DG244Cutting.D_Presentation.ViewModels.Pages.VM_Page20"/>
    /// de l'application DG244Cutting, exposant à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Components.HorizontalMenus.MH20"/>
    /// les cinq commandes transverses standards héritées du socle
    /// <see cref="VM_MH_Generic"/>, augmentées de trois commandes
    /// d'action propres — <see cref="ValidateCommand"/>,
    /// <see cref="RejectCommand"/> et <see cref="OutOfStockCommand"/> —
    /// relayées vers le contrat de page <see cref="IV_Page20"/>, d'une
    /// commande de navigation contextuelle <see cref="DetailsCommand"/>
    /// conduisant vers la Page11, et des quatre libellés multilingues
    /// propres alimentant les boutons correspondants.
    /// </summary>
    /// <remarks>
    /// <para>Contexte :</para>
    ///
    /// <para>DG244Cutting pilote la découpe des profilés aluminium de
    /// l'atelier sur le centre Elumatec DG244, selon un modèle
    /// d'approvisionnement à la demande : chaque barre est approvisionnée
    /// juste avant d'être coupée. La Page20 est la page de production :
    /// l'application y présente à l'opérateur la barre qu'elle a retenue
    /// pour la série en cours. Le présent ViewModel porte les gestes que
    /// l'opérateur peut accomplir sur cette barre depuis le menu
    /// horizontal développé de la page.</para>
    ///
    /// <para>Objectif :</para>
    ///
    /// <para>Offrir à l'opérateur quatre gestes : valider la barre
    /// présentée (avec ou sans défauts saisis sur la page), la refuser,
    /// la déclarer en rupture de stock, et consulter le détail de la
    /// série en cours sur la Page11, page de consultation en lecture
    /// seule (commandes, châssis, barres, découpes). Les trois premiers
    /// gestes sont des déclencheurs : la page sait ce qu'il faut
    /// contrôler, invoquer, désélectionner et relancer. Déclarer une
    /// barre en rupture immobilisant de la matière et interrompant le
    /// parcours, ce geste est précédé d'une confirmation émise par le
    /// présent ViewModel ; si l'opérateur se ravise, la page n'est pas
    /// sollicitée. Le quatrième geste est une navigation directe, sans
    /// traitement.</para>
    ///
    /// <para>L'activation de chaque bouton d'action traduit la
    /// possibilité structurelle du geste, exposée par les gardes du
    /// contrat <see cref="IV_Page20"/> — elle ne traduit jamais la
    /// cohérence de la saisie, dont le défaut produit un avertissement
    /// côté page et non un bouton désactivé. Aucun droit métier ne
    /// distingue les trois gestes d'action : leurs boutons sont visibles
    /// sans condition. Le bouton de détail n'est rendu visible par la vue
    /// que si l'utilisateur dispose du droit d'accès à la Page11.</para>
    ///
    /// <para>Synchronisation avec l'état de la page (§4.13.5.4 du
    /// 0230) : le présent ViewModel ne porte aucune copie de l'état de
    /// la page. Il lit les gardes
    /// <see cref="IV_Page20.CanValidate"/>,
    /// <see cref="IV_Page20.CanReject"/> et
    /// <see cref="IV_Page20.CanDeclareOutOfStock"/> au moment de
    /// l'évaluation de chaque <c>CanExecute</c>, et s'abonne à la
    /// notification <c>PropertyChanged</c> du contrat pour forcer la
    /// réévaluation des commandes à chaque transition de garde. Ces
    /// transitions surviennent à l'issue de traitements asynchrones de
    /// la page, sans entrée de l'opérateur : le déclenchement passif de
    /// <see cref="CommandManager.RequerySuggested"/> ne suffit donc pas à
    /// les refléter.</para>
    ///
    /// <para>Responsabilités :</para>
    ///
    /// <list type="bullet">
    ///   <item><description>Exposer au binding XAML les trois commandes
    ///   d'action <see cref="ValidateCommand"/>,
    ///   <see cref="RejectCommand"/> et <see cref="OutOfStockCommand"/>
    ///   et la commande de navigation contextuelle
    ///   <see cref="DetailsCommand"/>, en sus des membres transverses
    ///   hérités.</description></item>
    ///   <item><description>Exposer au binding XAML les quatre libellés
    ///   observables associés <see cref="Label_MH_Validate"/>,
    ///   <see cref="Label_MH_Reject"/>, <see cref="Label_MH_Details"/>
    ///   et <see cref="Label_MH_OutOfStock"/>.</description></item>
    ///   <item><description>Relayer chaque geste d'action vers le
    ///   contrat <see cref="IV_Page20"/>, dans le filet de sécurité
    ///   hérité <see cref="VM_Generic.ExecuteSafeAsync"/> et sous garde
    ///   d'anti-réentrance <see cref="VM_MH_Generic.IsProcessing"/>.</description></item>
    ///   <item><description>Obtenir la confirmation de l'opérateur avant
    ///   de relayer la déclaration de rupture de stock.</description></item>
    ///   <item><description>Naviguer vers la Page11 sur demande de
    ///   l'opérateur.</description></item>
    ///   <item><description>Forcer la réévaluation des gardes
    ///   <c>CanExecute</c> à chaque transition de garde notifiée par le
    ///   contrat <see cref="IV_Page20"/>.</description></item>
    ///   <item><description>Alimenter les libellés propres par surcharge
    ///   nominative de <see cref="LoadLabels"/>.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    ///
    /// <list type="bullet">
    ///   <item><description>N'effectue aucun traitement métier propre :
    ///   le contrôle de la saisie des défauts et du motif de refus,
    ///   l'invocation des UseCases de validation, de refus et de
    ///   rupture, la désélection de la barre et la relance de la
    ///   séquence d'entrée de la page relèvent de l'implémenteur du
    ///   contrat <see cref="IV_Page20"/>.</description></item>
    ///   <item><description>Ne calcule aucune garde : les gardes des
    ///   trois commandes d'action sont observées sur le contrat, jamais
    ///   évaluées localement.</description></item>
    ///   <item><description>N'interprète aucun retour : les trois
    ///   déclencheurs du contrat retournent une tâche sans
    ///   valeur.</description></item>
    ///   <item><description>Ne consomme aucun UseCase métier via
    ///   <c>IS_UseCaseInvoker</c>.</description></item>
    ///   <item><description>Ne décide pas de la visibilité des boutons
    ///   et n'évalue aucun droit d'accès : ces responsabilités relèvent
    ///   de la vue et d'<see cref="IU_Navigation"/>.</description></item>
    ///   <item><description>Ne redéfinit aucun des membres transverses
    ///   hérités du socle <see cref="VM_MH_Generic"/>.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales :</para>
    ///
    /// <para>Le pilotage de la Page20 passe par le contrat
    /// <see cref="IV_Page20"/> (préfixe <c>IV_</c>, porté par A_Domain),
    /// dispositif de découplage par inversion de dépendance : aucune
    /// référence concrète n'existe entre le présent ViewModel et le
    /// ViewModel de page. Le contrat étant un <c>IV_</c> et non un
    /// <c>IU_</c> ou un <c>IQ_</c>, l'interdiction I-4.10.9 du 0231 ne
    /// s'applique pas et aucune EA-11 n'est mobilisée. La dépendance
    /// <see cref="IS_Notification"/> est un Service de présentation,
    /// également hors du champ d'I-4.10.9. La commande de navigation
    /// contextuelle <see cref="DetailsCommand"/> consomme
    /// <see cref="IU_Navigation"/> au titre de l'EA-05, par le champ
    /// <c>protected</c> <c>_navigation</c> hérité du socle, sans
    /// injection ni rétention locale ; elle forme avec le
    /// conditionnement de visibilité du bouton <c>MH_Details</c> dans
    /// l'override <c>ApplyNavigationRules</c> de la vue un couple
    /// indissociable (R-4.13.14 du 0231).</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2 du 0230), augmentée de deux régions
    /// supplémentaires : la région d'extension
    /// <c>=== Propriétés publiques ===</c>, qui porte les huit membres
    /// exposés propres (quatre commandes et quatre libellés), et la
    /// région <c>=== Méthodes protégées ===</c>, rendue obligatoire par
    /// la présence de la surcharge <c>protected override</c>
    /// <see cref="LoadLabels"/> (R-4.4.10 du 0231). L'extension
    /// <c>=== Événements / Délégués / Indexeurs ===</c> n'est pas
    /// présente : l'événement <c>PropertyChanged</c> est porté par
    /// <see cref="VM_Generic"/>. Soit sept régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> : porte
    ///   les quatre champs de stockage des libellés propres.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : porte
    ///   les deux dépendances propres <c>_page20</c>
    ///   (<see cref="IV_Page20"/>) et <c>_notification</c>
    ///   (<see cref="IS_Notification"/>), seules dépendances retenues
    ///   localement.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c>
    ///   (extension) : porte les quatre commandes et les quatre
    ///   libellés observables.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   <c>public</c> à six paramètres, délégation des quatre
    ///   dépendances de base à <see cref="VM_MH_Generic"/> via
    ///   <c>base(...)</c>, gardes non nulles des deux dépendances
    ///   propres, composition des quatre commandes, abonnement à la
    ///   notification du contrat de page, et invocation
    ///   d'<see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction (R-4.11.8 du 0231).</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>. Aucun
    ///   override de <see cref="VM_MH_Generic.LoadAsync"/>, le composant
    ///   n'ayant pas de donnée à charger au montage.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> : porte
    ///   la surcharge nominative <see cref="LoadLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : porte les
    ///   quatre handlers <see cref="ExecuteValidateAsync"/>,
    ///   <see cref="ExecuteRejectAsync"/>,
    ///   <see cref="ExecuteOutOfStockAsync"/> et
    ///   <see cref="ExecuteDetailsAsync"/>, ainsi que le relais de
    ///   réévaluation des gardes
    ///   <see cref="OnPage20PropertyChanged"/>.</description></item>
    /// </list>
    /// </remarks>
    public class VM_MH20 : VM_MH_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Champ de stockage du libellé propre
        /// <see cref="Label_MH_Validate"/>, initialisé à la chaîne vide et
        /// alimenté par la surcharge <see cref="LoadLabels"/> via la
        /// résolution de la clé <c>MH_Ti_14</c>. Les mutations passent par
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/> pour
        /// émettre la notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_validate = string.Empty;

        /// <summary>
        /// Champ de stockage du libellé propre
        /// <see cref="Label_MH_Reject"/>, initialisé à la chaîne vide et
        /// alimenté par la surcharge <see cref="LoadLabels"/> via la
        /// résolution de la clé <c>MH_Ti_15</c>. Les mutations passent par
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/> pour
        /// émettre la notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_reject = string.Empty;

        /// <summary>
        /// Champ de stockage du libellé propre
        /// <see cref="Label_MH_Details"/>, initialisé à la chaîne vide et
        /// alimenté par la surcharge <see cref="LoadLabels"/> via la
        /// résolution de la clé <c>MH_Ti_16</c>. Les mutations passent par
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/> pour
        /// émettre la notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_details = string.Empty;

        /// <summary>
        /// Champ de stockage du libellé propre
        /// <see cref="Label_MH_OutOfStock"/>, initialisé à la chaîne vide
        /// et alimenté par la surcharge <see cref="LoadLabels"/> via la
        /// résolution de la clé <c>MH_Ti_24</c>. Les mutations passent par
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/> pour
        /// émettre la notification <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_outofstock = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Contrat de ViewModel de la page de production (Page20), vers
        /// lequel les trois commandes d'action propres sont relayées.
        /// Dépendance propre injectée en cinquième paramètre du
        /// constructeur et retenue localement. Ses trois gardes
        /// (<see cref="IV_Page20.CanValidate"/>,
        /// <see cref="IV_Page20.CanReject"/>,
        /// <see cref="IV_Page20.CanDeclareOutOfStock"/>) sont lues par les
        /// gardes <c>CanExecute</c> des commandes, et sa notification
        /// <c>PropertyChanged</c> est observée par
        /// <see cref="OnPage20PropertyChanged"/>.
        /// </summary>
        private readonly IV_Page20 _page20;

        /// <summary>
        /// Service de notification opérateur, consommé exclusivement par
        /// <see cref="ExecuteOutOfStockAsync"/> pour obtenir la
        /// confirmation de l'opérateur avant la déclaration de rupture de
        /// stock. Dépendance propre injectée en sixième paramètre du
        /// constructeur et retenue localement.
        /// </summary>
        private readonly IS_Notification _notification;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Commande d'action « Valider » relayée vers
        /// <see cref="IV_Page20.ValidateAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteValidateAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing &amp;&amp; _page20.CanValidate</c>
        /// (anti-réentrance combinée à la possibilité structurelle de
        /// valider la barre présentée). La validation couvre la barre sans
        /// défaut comme la barre avec défauts saisis sur la page, la
        /// distinction relevant de l'implémenteur du contrat.</para>
        /// <para>Sortie : relais vers
        /// <see cref="IV_Page20.ValidateAsync"/>, dans le filet
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>. Aucun cas d'échec
        /// métier propre, aucun retour interprété.</para>
        /// </remarks>
        public ICommand ValidateCommand { get; }

        /// <summary>
        /// Commande d'action « Refuser » relayée vers
        /// <see cref="IV_Page20.RejectAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteRejectAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing &amp;&amp; _page20.CanReject</c>. Le contrôle
        /// de la présence du motif de refus relève de l'implémenteur du
        /// contrat et ne conditionne pas la garde.</para>
        /// <para>Sortie : relais vers <see cref="IV_Page20.RejectAsync"/>,
        /// dans le filet <see cref="VM_Generic.ExecuteSafeAsync"/>. Aucun
        /// cas d'échec métier propre, aucun retour interprété.</para>
        /// </remarks>
        public ICommand RejectCommand { get; }

        /// <summary>
        /// Commande d'action « Rupture » : après confirmation de
        /// l'opérateur, relaie la déclaration de rupture de stock vers
        /// <see cref="IV_Page20.DeclareOutOfStockAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteOutOfStockAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing &amp;&amp; _page20.CanDeclareOutOfStock</c>.</para>
        /// <para>Confirmation : la déclaration met la barre en attente et
        /// suspend ses découpes jusqu'à ce que la matière soit de nouveau
        /// disponible. Une confirmation modale (clé <c>No_Qe_01</c>) est
        /// donc demandée à l'opérateur ; en cas de refus, la commande
        /// s'achève sans solliciter la page.</para>
        /// <para>Sortie : relais vers
        /// <see cref="IV_Page20.DeclareOutOfStockAsync"/> en cas de
        /// confirmation, dans le filet
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>. Aucun cas d'échec
        /// métier propre, aucun retour interprété.</para>
        /// </remarks>
        public ICommand OutOfStockCommand { get; }

        /// <summary>
        /// Commande de navigation contextuelle « Détails » conduisant vers
        /// la Page11, page de consultation en lecture seule de la série en
        /// cours.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteDetailsAsync"/>. Garde <c>CanExecute</c> :
        /// <c>!IsProcessing</c>. La commande ne déclenche aucun
        /// traitement : son seul effet est une navigation (§4.13.6.4 du
        /// 0230). La visibilité du bouton correspondant est conditionnée
        /// par la vue au droit d'accès à la Page11 (R-4.13.14 du
        /// 0231).</para>
        /// <para>Sortie : invocation de
        /// <see cref="IU_Navigation.NavigateToPageAsync"/> avec le nom de
        /// page <c>"Page11"</c>, dans le filet
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>. Le contrôle du droit
        /// d'accès demeure interne au UseCase de navigation. Aucun cas
        /// d'échec métier propre.</para>
        /// </remarks>
        public ICommand DetailsCommand { get; }

        /// <summary>
        /// Libellé multilingue propre du bouton « Valider », bindé sur le
        /// <c>TextBlock</c> du bouton <c>MH_Validate</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_14</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Setter <c>private</c>, mutation via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_Validate
        {
            get => _label_mh_validate;
            private set => SetProperty(ref _label_mh_validate, value);
        }

        /// <summary>
        /// Libellé multilingue propre du bouton « Refuser », bindé sur le
        /// <c>TextBlock</c> du bouton <c>MH_Reject</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_15</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Setter <c>private</c>, mutation via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_Reject
        {
            get => _label_mh_reject;
            private set => SetProperty(ref _label_mh_reject, value);
        }

        /// <summary>
        /// Libellé multilingue propre du bouton « Détails », bindé sur le
        /// <c>TextBlock</c> du bouton <c>MH_Details</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_16</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Setter <c>private</c>, mutation via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_Details
        {
            get => _label_mh_details;
            private set => SetProperty(ref _label_mh_details, value);
        }

        /// <summary>
        /// Libellé multilingue propre du bouton « Rupture », bindé sur le
        /// <c>TextBlock</c> du bouton <c>MH_OutOfStock</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_24</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Setter <c>private</c>, mutation via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_OutOfStock
        {
            get => _label_mh_outofstock;
            private set => SetProperty(ref _label_mh_outofstock, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_MH20"/>.
        /// </summary>
        /// <remarks>
        /// <para>Séquence d'initialisation :</para>
        ///
        /// <para>Délègue d'abord à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c> (validation non nulle des quatre dépendances
        /// de base, composition des cinq commandes transverses). Garde
        /// ensuite la non-nullité de <paramref name="page20"/> puis de
        /// <paramref name="notification"/> et les retient dans les champs
        /// propres <c>_page20</c> et <c>_notification</c>. Compose les
        /// trois commandes d'action (chaque garde <c>CanExecute</c>
        /// combinant <c>!IsProcessing</c> et la garde correspondante de
        /// <see cref="IV_Page20"/>) puis la commande de navigation
        /// contextuelle (garde <c>!IsProcessing</c>). S'abonne ensuite à
        /// la notification <c>PropertyChanged</c> du contrat de page.
        /// Invoque enfin <see cref="VM_Generic.InitializeLabels"/> en
        /// DERNIÈRE instruction (R-4.11.8 du 0231) afin de déclencher
        /// l'alimentation des libellés — les quatre transverses via
        /// <c>base.LoadLabels</c> et les quatre propres via la surcharge
        /// <see cref="LoadLabels"/>.</para>
        ///
        /// <para>Durée de vie de l'abonnement :</para>
        ///
        /// <para>L'abonnement à la notification du contrat de page n'est
        /// jamais retiré. Le présent ViewModel et l'implémenteur du
        /// contrat sont enregistrés en Singleton : leurs durées de vie
        /// sont identiques et alignées sur celle du processus, qui assure
        /// la libération.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c>. Injecté en Singleton par le conteneur
        /// DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement terminal
        /// des erreurs, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c>. Mobilisé uniquement par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, non utilisé
        /// directement par le présent ViewModel. Injecté en Singleton par
        /// le conteneur DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif global,
        /// transmis à <see cref="VM_MH_Generic"/> via <c>base(...)</c> pour
        /// l'alimentation de la mécanique multilingue factorisée par
        /// <see cref="VM_Generic"/>. Le présent dérivé ne stocke pas cette
        /// dépendance ni n'y accède directement, conformément à I-4.11.11
        /// du 0231. Injecté en Singleton par le conteneur DI.</param>
        /// <param name="navigation">UseCase de navigation, transmis à
        /// <see cref="VM_MH_Generic"/> via <c>base(...)</c>. Le présent
        /// dérivé ne le retient pas localement : il le consomme pour
        /// <see cref="DetailsCommand"/> par le champ <c>protected</c>
        /// <c>_navigation</c> hérité du socle, au titre de l'EA-05.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="page20">Contrat de ViewModel de la page de
        /// production (Page20), retenu dans le champ propre
        /// <c>_page20</c>, cible des relais des trois commandes d'action et
        /// source de leurs gardes. Résolu par inversion de dépendance
        /// A_Domain vers l'implémenteur concret, porté par le conteneur
        /// DI.</param>
        /// <param name="notification">Service de notification opérateur,
        /// retenu dans le champ propre <c>_notification</c> et consommé
        /// pour la confirmation préalable à la déclaration de rupture de
        /// stock. Injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée par la chaîne
        /// <c>base(...)</c> si l'un des quatre paramètres de base est
        /// <see langword="null"/>, ou par les gardes locales si
        /// <paramref name="page20"/> ou <paramref name="notification"/>
        /// est <see langword="null"/>.</exception>
        public VM_MH20(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IU_Navigation navigation,
            IV_Page20 page20,
            IS_Notification notification)
            : base(dictionary, logAndNotify, app, navigation)
        {
            _page20 = page20 ?? throw new ArgumentNullException(nameof(page20));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));

            ValidateCommand = new UT_RelayCommandArg0Async(
                ExecuteValidateAsync, () => !IsProcessing && _page20.CanValidate);
            RejectCommand = new UT_RelayCommandArg0Async(
                ExecuteRejectAsync, () => !IsProcessing && _page20.CanReject);
            OutOfStockCommand = new UT_RelayCommandArg0Async(
                ExecuteOutOfStockAsync, () => !IsProcessing && _page20.CanDeclareOutOfStock);
            DetailsCommand = new UT_RelayCommandArg0Async(
                ExecuteDetailsAsync, () => !IsProcessing);

            _page20.PropertyChanged += OnPage20PropertyChanged;

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
        /// propres.
        /// </summary>
        /// <param name="caller">CallChain reçue de la mécanique
        /// multilingue héritée — par
        /// <see cref="VM_Generic.InitializeLabels"/> au premier appel, puis
        /// à chaque changement de culture active —, transmise telle quelle
        /// à <c>base.LoadLabels</c> et enrichie localement du segment
        /// <c>LoadLabels</c> pour la résolution des clés via
        /// <see cref="IS_Dictionary.GetText"/>.</param>
        /// <remarks>
        /// <para>Contexte : Surcharge nominative de
        /// <see cref="VM_MH_Generic.LoadLabels"/>. L'appel à
        /// <c>base.LoadLabels(caller)</c> est IMPÉRATIVEMENT la première
        /// instruction fonctionnelle du corps, afin de préserver
        /// l'alimentation des quatre libellés transverses du socle
        /// (<c>MH_Ti_01</c> à <c>MH_Ti_04</c>) : son omission
        /// constituerait une non-conformité au contrat de la mécanique
        /// multilingue de la famille MH (§3.14 et §4.15.8 du 0230). Les
        /// quatre libellés propres sont ensuite résolus dans l'ordre
        /// croissant des clés : <c>MH_Ti_14</c> (Valider),
        /// <c>MH_Ti_15</c> (Refuser), <c>MH_Ti_16</c> (Détails) et
        /// <c>MH_Ti_24</c> (Rupture).</para>
        /// <para>Filet de sécurité : aucun <c>try</c>/<c>catch</c> local ;
        /// la résolution des clés est protégée par
        /// <see cref="IS_Dictionary.GetText"/> (R-4.11.6 et R-4.11.10 du
        /// 0231).</para>
        /// </remarks>
        protected override void LoadLabels(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadLabels)}";

            base.LoadLabels(caller);

            Label_MH_Validate = _dictionary.GetText(callChain, "MH_Ti_14");
            Label_MH_Reject = _dictionary.GetText(callChain, "MH_Ti_15");
            Label_MH_Details = _dictionary.GetText(callChain, "MH_Ti_16");
            Label_MH_OutOfStock = _dictionary.GetText(callChain, "MH_Ti_24");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de la commande <see cref="ValidateCommand"/> : relaie
        /// la validation de la barre présentée vers
        /// <see cref="IV_Page20.ValidateAsync"/>, sur le patron
        /// strictement identique aux cinq handlers transverses du socle.
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
        /// <c>BuildFirstCallChain</c>. Le bloc <c>try</c>/<c>finally</c>
        /// ne capture rien : le traitement terminal des erreurs est
        /// intégralement délégué au filet hérité ; aucun cas d'échec
        /// métier propre n'est traité ici.</para>
        /// <para>Jeton d'annulation :
        /// <see cref="CancellationToken.None"/> est passé explicitement en
        /// argument, tant au filet qu'à l'opération relayée. Le contrat de
        /// commande WPF exposé par <see cref="UT_RelayCommandArg0Async"/>
        /// ne véhicule aucun jeton, de sorte qu'aucune annulation
        /// coopérative n'est disponible au présent handler.</para>
        /// </remarks>
        private async Task ExecuteValidateAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _page20.ValidateAsync(callChain, CancellationToken.None);
                }, CancellationToken.None);
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="RejectCommand"/> : relaie le
        /// refus de la barre présentée vers
        /// <see cref="IV_Page20.RejectAsync"/>.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution du
        /// relais.</returns>
        /// <remarks>
        /// <para>Contexte : Même patron que
        /// <see cref="ExecuteValidateAsync"/>. Aucun cas d'échec métier
        /// propre.</para>
        /// <para>Jeton d'annulation :
        /// <see cref="CancellationToken.None"/> est passé explicitement en
        /// argument, tant au filet qu'à l'opération relayée — cf.
        /// <see cref="ExecuteValidateAsync"/>.</para>
        /// </remarks>
        private async Task ExecuteRejectAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _page20.RejectAsync(callChain, CancellationToken.None);
                }, CancellationToken.None);
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="OutOfStockCommand"/> : demande
        /// la confirmation de l'opérateur, puis relaie la déclaration de
        /// rupture de stock vers
        /// <see cref="IV_Page20.DeclareOutOfStockAsync"/> si elle est
        /// accordée.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution de la
        /// confirmation et, le cas échéant, du relais.</returns>
        /// <remarks>
        /// <para>Contexte : Même patron que
        /// <see cref="ExecuteValidateAsync"/>. La confirmation est émise à
        /// l'intérieur du délégué confié au filet
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, avant l'appel au
        /// contrat : les exceptions typées relancées par
        /// <see cref="IS_Notification.ConfirmationReturn"/> sont ainsi
        /// captées par le filet (R-4.7.18 et R-4.7.19 du 0231), et
        /// <see cref="VM_MH_Generic.IsProcessing"/> demeure positionné
        /// pendant l'affichage de la boîte de dialogue modale.</para>
        /// <para>Abandon : si l'opérateur ne confirme pas, le délégué
        /// s'achève sans solliciter le contrat de page ; aucun état n'est
        /// modifié.</para>
        /// <para>Jeton d'annulation :
        /// <see cref="CancellationToken.None"/> est passé explicitement en
        /// argument au filet, à la confirmation et à l'opération relayée —
        /// cf. <see cref="ExecuteValidateAsync"/>.</para>
        /// </remarks>
        private async Task ExecuteOutOfStockAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    if (!_notification.ConfirmationReturn(callChain, "No_Qe_01", null, CancellationToken.None))
                    {
                        return;
                    }

                    await _page20.DeclareOutOfStockAsync(callChain, CancellationToken.None);
                }, CancellationToken.None);
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Handler de la commande <see cref="DetailsCommand"/> : navigue
        /// vers la Page11.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution de la
        /// navigation.</returns>
        /// <remarks>
        /// <para>Contexte : Même patron que
        /// <see cref="ExecuteValidateAsync"/>. Invoque
        /// <see cref="IU_Navigation.NavigateToPageAsync"/> par le champ
        /// <c>protected</c> <c>_navigation</c> hérité du socle, au titre
        /// de l'EA-05 et de R-4.13.14 du 0231, sans médiation par un
        /// UseCase métier. Le nom de page <c>"Page11"</c> est strictement
        /// identique à celui sur lequel la vue conditionne la visibilité
        /// du bouton <c>MH_Details</c>. Aucun cas d'échec métier
        /// propre.</para>
        /// <para>Jeton d'annulation :
        /// <see cref="CancellationToken.None"/> est passé explicitement en
        /// argument, tant au filet qu'à la navigation — cf.
        /// <see cref="ExecuteValidateAsync"/>.</para>
        /// </remarks>
        private async Task ExecuteDetailsAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.NavigateToPageAsync(callChain, "Page11", CancellationToken.None);
                }, CancellationToken.None);
            }
            finally
            {
                EndProcessing();
            }
        }

        /// <summary>
        /// Relais de réévaluation des gardes : sur transition de l'une des
        /// trois gardes du contrat <see cref="IV_Page20"/>, force le
        /// recalcul des <c>CanExecute</c> de l'ensemble des commandes.
        /// </summary>
        /// <param name="sender">Émetteur de la notification (implémenteur
        /// du contrat de page). Non exploité.</param>
        /// <param name="e">Arguments de la notification, dont le nom de la
        /// propriété modifiée sert de filtre.</param>
        /// <remarks>
        /// <para>Contexte : Les transitions de garde de la page de
        /// production surviennent à l'issue de traitements asynchrones,
        /// sans entrée de l'opérateur ; le déclenchement passif de
        /// <see cref="CommandManager.RequerySuggested"/> ne suffit pas à
        /// les refléter. Le présent relais provoque explicitement la
        /// réévaluation via
        /// <see cref="CommandManager.InvalidateRequerySuggested"/>
        /// (§4.13.5.3 du 0230).</para>
        /// <para>Filtre : seules les notifications portant sur
        /// <see cref="IV_Page20.CanValidate"/>,
        /// <see cref="IV_Page20.CanReject"/> ou
        /// <see cref="IV_Page20.CanDeclareOutOfStock"/> sont traitées ;
        /// toute autre notification est ignorée.</para>
        /// <para>Marshalling : la notification pouvant être émise hors du
        /// thread d'interface, la réévaluation est postée sur le
        /// <see cref="Dispatcher"/> de l'application lorsqu'il existe. En
        /// contexte non-WPF (tests unitaires), l'appel est effectué
        /// directement.</para>
        /// </remarks>
        private void OnPage20PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IV_Page20.CanValidate)
                && e.PropertyName != nameof(IV_Page20.CanReject)
                && e.PropertyName != nameof(IV_Page20.CanDeclareOutOfStock))
            {
                return;
            }

            Dispatcher? dispatcher = Application.Current?.Dispatcher;
            if (dispatcher is null)
            {
                // Contexte non-WPF (tests unitaires) : appel direct.
                CommandManager.InvalidateRequerySuggested();
                return;
            }

            dispatcher.BeginInvoke(new Action(CommandManager.InvalidateRequerySuggested));
        }

        #endregion
    }
}