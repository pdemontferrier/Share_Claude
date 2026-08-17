using System.Windows.Input;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Components.HorizontalMenus
{
    /// <summary>
    /// ViewModel du menu horizontal associé à la
    /// <see cref="DG244Cutting.D_Presentation.ViewModels.Pages.VM_Page10"/>
    /// de l'application DG244Cutting, exposant à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Components.HorizontalMenus.MH10"/>
    /// les quatre commandes transverses standards héritées du socle
    /// <see cref="VM_MH_Generic"/>, augmentées d'une commande propre
    /// de navigation contextuelle vers la page de gestion du stock de
    /// chutes et du libellé multilingue du bouton correspondant.
    /// </summary>
    /// <remarks>
    /// <para>Contexte :</para>
    ///
    /// <para>La Page10 est le tableau de bord de production de
    /// l'application : elle présente à l'opérateur les séries de
    /// découpe réparties par état (en retard, à réaliser, en cours,
    /// réalisées, à venir) et constitue le point d'entrée du parcours
    /// nominal de production. Le menu horizontal qui la surmonte
    /// expose les quatre boutons transverses standards du socle. Le
    /// parcours nominal de production alimente et consomme
    /// automatiquement le stock de chutes réutilisables, sans
    /// intervention manuelle ; l'atelier a néanmoins besoin de pouvoir
    /// agir sur ce stock hors de ce flux. Le présent ViewModel porte
    /// le point d'entrée de cette action.</para>
    ///
    /// <para>Objectif :</para>
    ///
    /// <para>Offrir à l'opérateur, depuis le tableau de bord, un accès
    /// direct et permanent à la page de gestion du stock de chutes,
    /// indépendamment de toute série sélectionnée et hors du parcours
    /// de production. L'accès est matérialisé par un bouton du menu
    /// horizontal, dont le présent ViewModel expose la commande et le
    /// libellé multilingue. L'action est purement navigationnelle :
    /// elle ne modifie aucun état métier.</para>
    ///
    /// <para>Responsabilités :</para>
    ///
    /// <list type="bullet">
    ///   <item><description>Exposer au binding XAML la commande
    ///   <see cref="CuttingScrapStockCommand"/> de navigation
    ///   contextuelle vers la page de gestion du stock de chutes,
    ///   composée au constructeur et câblée sur le handler privé
    ///   <see cref="ExecuteCuttingScrapStockAsync"/> avec garde
    ///   d'anti-réentrance sur
    ///   <see cref="VM_MH_Generic.IsProcessing"/>.</description></item>
    ///   <item><description>Exposer au binding XAML la propriété
    ///   observable <see cref="Label_MH_CuttingScrapStock"/>, libellé
    ///   du bouton dans la culture active.</description></item>
    ///   <item><description>Alimenter ce libellé propre par surcharge
    ///   de <see cref="VM_MH_Generic.LoadLabels"/>, en préservant
    ///   l'alimentation des quatre libellés transverses portés par le
    ///   socle.</description></item>
    ///   <item><description>Déclencher, au constructeur et en dernière
    ///   instruction, l'orchestration multilingue héritée via
    ///   <see cref="VM_Generic.InitializeLabels"/>.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    ///
    /// <list type="bullet">
    ///   <item><description>Ne modifie aucun état métier : ni l'état
    ///   de sélection de série, ni aucune entité, ni aucun indicateur
    ///   de série. Aucun Repository, aucun Command Handler, aucun
    ///   Query Handler, aucune transactionnalité n'est
    ///   mobilisé.</description></item>
    ///   <item><description>Ne redéclare aucun membre hérité : ni les
    ///   cinq commandes transverses standards, ni
    ///   <see cref="VM_MH_Generic.IsProcessing"/>, ni les quatre
    ///   propriétés observables de libellés transverses, ni le champ
    ///   <c>_navigation</c>, qui est le champ <c>protected
    ///   readonly</c> hérité du socle.</description></item>
    ///   <item><description>Ne conditionne pas la visibilité du bouton
    ///   correspondant : ce conditionnement est porté côté Vue par
    ///   l'override d'<c>ApplyNavigationRules</c> de <c>MH10</c>,
    ///   auquel la présente commande est indissociablement liée au
    ///   titre de R-4.13.14 du 0231.</description></item>
    ///   <item><description>Ne compose aucun message d'erreur propre :
    ///   le traitement terminal des erreurs est intégralement délégué
    ///   au filet <c>ExecuteSafeAsync</c> hérité de
    ///   <see cref="VM_Generic"/>.</description></item>
    ///   <item><description>N'implémente aucune cérémonie multilingue
    ///   locale : aucun abonnement propre à l'état applicatif, aucun
    ///   marshalling, aucune logique de repli locale, la mécanique
    ///   étant intégralement factorisée par
    ///   <see cref="VM_Generic"/>.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales :</para>
    ///
    /// <para>Le handler <see cref="ExecuteCuttingScrapStockAsync"/>
    /// invoque directement <see cref="IU_Navigation"/> par le champ
    /// <c>protected readonly</c> <c>_navigation</c> hérité de
    /// <see cref="VM_MH_Generic"/>, au titre de l'EA-05 (accès direct
    /// à <see cref="IU_Navigation"/> par le couple générique de la
    /// famille MH et ses dérivés). Cette consommation déroge à
    /// R-4.12.2 et à I-4.12.2 du 0231, qui réservent aux UseCases la
    /// décision de navigation. Le périmètre de l'EA-05 ouvre aux
    /// dérivés la surface d'écriture du contrat, au premier chef
    /// <see cref="IU_Navigation.NavigateToPageAsync"/> pour la
    /// navigation contextuelle vers une page déterminée. Aucune
    /// médiation par <c>IS_UseCaseInvoker</c> n'est mobilisée :
    /// <c>UC_Navigation</c> est enregistré Singleton et ne consomme
    /// aucune dépendance scoped, ce qui rend sans objet la fonction
    /// propre de cette médiation (R-4.10.12 du 0231, principe P4-bis
    /// de §4.10.10 du 0230). Aucune redéclaration ni injection propre
    /// de <see cref="IU_Navigation"/> n'est opérée : le point
    /// d'injection demeure unique et localisé au socle.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq
    /// régions standard (§4.4.2 du 0230) augmentée de deux extensions
    /// §4.4.3 : <c>=== Propriétés publiques ===</c>, présente car la
    /// classe expose des propriétés publiques propres et placée avant
    /// le constructeur conformément à R-4.4.9 du 0231 ; et
    /// <c>=== Méthodes protégées ===</c>, présente au titre de
    /// R-4.4.10 du 0231 car la classe expose une méthode
    /// <c>protected</c> propre, et insérée entre la région Méthodes
    /// publiques et la région Méthodes privées. L'extension
    /// <c>=== Événements / Délégués / Indexeurs ===</c> n'est pas
    /// présente : <see cref="VM_MH10"/> n'expose aucun événement
    /// propre, l'événement <c>PropertyChanged</c> étant porté par
    /// <see cref="VM_Generic"/> au titre d'INPC et hérité par
    /// transitivité. Soit sept régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   champ de stockage
    ///   <c>_label_mh_cuttingscrapstock</c> de la propriété
    ///   observable de libellé propre.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>. Aucune
    ///   dépendance propre n'est injectée par le présent
    ///   ViewModel ; les quatre dépendances du constructeur sont
    ///   intégralement déléguées à <c>base(...)</c>, et la
    ///   consommation d'<see cref="IU_Navigation"/> s'opère par le
    ///   champ <c>protected readonly</c> hérité du socle au titre
    ///   d'EA-05.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c> :
    ///   <see cref="CuttingScrapStockCommand"/> et
    ///   <see cref="Label_MH_CuttingScrapStock"/>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur <c>public</c> à quatre paramètres, délégation
    ///   intégrale à <see cref="VM_MH_Generic"/> via
    ///   <c>base(...)</c> sans rétention locale, composition de
    ///   <see cref="CuttingScrapStockCommand"/>, et invocation
    ///   d'<see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction du corps pour déclencher l'alimentation des
    ///   quatre libellés transverses hérités du socle et du libellé
    ///   propre (R-4.11.8 du 0231).</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>. Aucun
    ///   override de <see cref="VM_MH_Generic.LoadAsync"/>, le
    ///   présent ViewModel n'ayant aucune donnée à charger au
    ///   montage du menu horizontal.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   override propre de
    ///   <see cref="VM_MH_Generic.LoadLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   handler <see cref="ExecuteCuttingScrapStockAsync"/> de la
    ///   commande de navigation contextuelle.</description></item>
    /// </list>
    /// </remarks>
    public class VM_MH10 : VM_MH_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Champ support de la propriété observable
        /// <see cref="Label_MH_CuttingScrapStock"/>, initialisé à la
        /// chaîne vide. Alimenté par l'override propre de
        /// <see cref="LoadLabels"/> via la résolution de la clé
        /// <c>MH_Ti_23</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Modifié exclusivement par
        /// <see cref="LoadLabels"/> au premier appel et à chaque
        /// changement d'<c>AppCultureCode</c>, via le helper
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/>
        /// pour déclencher la notification INPC. Jamais
        /// <see langword="null"/>.</para>
        /// </remarks>
        private string _label_mh_cuttingscrapstock = string.Empty;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Commande de navigation contextuelle ouvrant la page de
        /// gestion du stock de chutes (Page30).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Commande de la troisième catégorie de
        /// R-3.13.5 du 0231 — bouton de navigation contextuelle,
        /// conduisant vers une page déterminée sans déclencher aucun
        /// traitement métier. À parité de nature avec les cinq
        /// commandes transverses du socle
        /// <see cref="VM_MH_Generic"/>, dont elle ne masque ni ne
        /// redéclare aucun membre. Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur et jamais réaffectée, câblée sur le handler
        /// privé <see cref="ExecuteCuttingScrapStockAsync"/> avec un
        /// prédicat <c>CanExecute</c> fixé à la négation
        /// d'<see cref="VM_MH_Generic.IsProcessing"/>, garantissant
        /// l'anti-réentrance à l'identique des commandes du
        /// socle.</para>
        /// <para>Effets observables : navigation de la Page10 vers la
        /// Page30 et empilement du contexte de retour, permettant le
        /// retour au tableau de bord par le bouton
        /// <c>MH_Previous</c> du menu horizontal de la page de
        /// destination. Aucun effet sur l'état métier de
        /// l'application.</para>
        /// <para>Couple indissociable : La présente commande forme
        /// avec le conditionnement de visibilité du bouton
        /// <c>MH_CuttingScrapStock</c>, porté par l'override
        /// d'<c>ApplyNavigationRules</c> de <c>MH10</c>, un couple
        /// indissociable au titre de R-4.13.14 du 0231. Le nom de
        /// page consommé de part et d'autre est strictement
        /// identique. L'exposition de l'une sans l'autre constitue
        /// une non-conformité à I-4.13.14.</para>
        /// <para>Cas d'échec métier : Aucun cas propre. Le droit
        /// d'accès est traité en amont par le masquage du bouton
        /// côté Vue ; toute exception levée à l'exécution est
        /// capturée, journalisée et notifiée par le filet
        /// <c>ExecuteSafeAsync</c> hérité de
        /// <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public ICommand CuttingScrapStockCommand { get; }

        /// <summary>
        /// Libellé multilingue propre du bouton d'accès au stock de
        /// chutes, bindé sur le <c>TextBlock</c> du bouton
        /// <c>MH_CuttingScrapStock</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété observable alimentée par la
        /// résolution de la clé <c>MH_Ti_23</c> au travers de
        /// l'override propre de <see cref="LoadLabels"/>. Le setter
        /// est <c>private</c> : la propriété n'est mutée qu'en
        /// interne, via <c>SetProperty</c> hérité de
        /// <see cref="VM_Generic"/> pour émettre la notification
        /// <c>PropertyChanged</c>.</para>
        /// <para>Valeur : Chaîne résolue du dictionnaire actif, ou
        /// chaîne de repli si la clé est absente de la culture
        /// courante. Chaîne vide avant le premier appel à
        /// <see cref="LoadLabels"/> orchestré par
        /// <see cref="VM_Generic.InitializeLabels"/> ; jamais
        /// <see langword="null"/>.</para>
        /// <para>Rechargement : La valeur est reconstruite à chaque
        /// changement de culture active, par la mécanique multilingue
        /// factorisée par <see cref="VM_Generic"/>.</para>
        /// </remarks>
        public string Label_MH_CuttingScrapStock
        {
            get => _label_mh_cuttingscrapstock;
            private set => SetProperty(ref _label_mh_cuttingscrapstock, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_MH10"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte :</para>
        ///
        /// <para>Constructeur <c>public</c> à quatre paramètres,
        /// reproduisant strictement la signature du constructeur
        /// <c>protected</c> de <see cref="VM_MH_Generic"/>. Le
        /// présent ViewModel n'injecte aucune dépendance propre en
        /// sus des quatre dépendances de base : la consommation
        /// d'<see cref="IU_Navigation"/> par le handler de la
        /// commande de navigation contextuelle s'opère par le champ
        /// <c>protected readonly</c> hérité du socle au titre
        /// d'EA-05, sans rétention locale.</para>
        ///
        /// <para>Séquence d'initialisation :</para>
        ///
        /// <list type="number">
        ///   <item><description>Délégation intégrale à
        ///   <see cref="VM_MH_Generic"/> via
        ///   <c>base(dictionary, logAndNotify, app, navigation)</c>,
        ///   qui prend en charge l'affectation des quatre
        ///   dépendances, les gardes
        ///   <see cref="ArgumentNullException"/> correspondantes et
        ///   la composition des cinq commandes transverses
        ///   standards.</description></item>
        ///   <item><description>Composition de
        ///   <see cref="CuttingScrapStockCommand"/> via
        ///   <see cref="UT_RelayCommandArg0Async"/>, câblée sur
        ///   <see cref="ExecuteCuttingScrapStockAsync"/> avec le
        ///   prédicat d'anti-réentrance.</description></item>
        ///   <item><description>Invocation de
        ///   <see cref="VM_Generic.InitializeLabels"/> en DERNIÈRE
        ///   instruction (R-4.11.8 du 0231), déclenchant le premier
        ///   appel synchrone à <see cref="LoadLabels"/> par
        ///   dispatching virtuel — soit l'alimentation des quatre
        ///   libellés transverses du socle puis celle de
        ///   <see cref="Label_MH_CuttingScrapStock"/> — ainsi que le
        ///   branchement de l'abonnement INPC à la culture
        ///   active.</description></item>
        /// </list>
        ///
        /// <para>Filet de sécurité :</para>
        ///
        /// <para>La validation non-nulle des quatre paramètres est
        /// intégralement portée par la chaîne <c>base(...)</c> ;
        /// aucune garde locale redondante n'est ajoutée. Le présent
        /// constructeur ne retenant aucune référence propre, il
        /// n'introduit aucun point de défaillance
        /// supplémentaire.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c> et consommé par l'override propre de
        /// <see cref="LoadLabels"/> au travers du champ
        /// <c>protected readonly</c> <c>_dictionary</c> hérité de
        /// <see cref="VM_Generic"/>. Injecté en Singleton par le
        /// conteneur DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_MH_Generic"/> via <c>base(...)</c>.
        /// Mobilisé uniquement par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, non utilisé
        /// directement par le présent ViewModel. Injecté en
        /// Singleton par le conteneur DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par <see cref="VM_Generic"/>. Le
        /// présent dérivé ne stocke pas cette dépendance ni n'y
        /// accède directement, conformément à I-4.11.11 du 0231.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="navigation">UseCase de navigation, transmis
        /// à <see cref="VM_MH_Generic"/> via <c>base(...)</c> sans
        /// rétention locale. Consommé par les cinq handlers privés
        /// hérités du socle de la famille VM_MH et par le handler
        /// propre <see cref="ExecuteCuttingScrapStockAsync"/>, au
        /// titre de l'EA-05 ; l'accès du présent dérivé s'opère par
        /// le champ <c>protected readonly</c> <c>_navigation</c>
        /// hérité, sans redéclaration ni injection propre. Injecté en
        /// Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée par la
        /// chaîne <c>base(...)</c> si l'un des quatre paramètres est
        /// <see langword="null"/>.</exception>
        public VM_MH10(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IU_Navigation navigation)
            : base(dictionary, logAndNotify, app, navigation)
        {
            CuttingScrapStockCommand = new UT_RelayCommandArg0Async(
                ExecuteCuttingScrapStockAsync, () => !IsProcessing);

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Alimente les libellés multilingues du menu horizontal :
        /// les quatre libellés transverses portés par le socle, puis
        /// le libellé propre
        /// <see cref="Label_MH_CuttingScrapStock"/>.
        /// </summary>
        /// <param name="callChain">CallChain courante propagée par la
        /// mécanique multilingue héritée — par
        /// <see cref="VM_Generic.InitializeLabels"/> au premier appel,
        /// puis à chaque changement de culture active —, transmise
        /// telle quelle à <c>base.LoadLabels</c> et à
        /// <see cref="IS_Dictionary.GetText"/>.</param>
        /// <remarks>
        /// <para>Contexte : Surcharge propre de
        /// <see cref="VM_MH_Generic.LoadLabels"/>. L'appel à
        /// <c>base.LoadLabels(callChain)</c> est IMPÉRATIVEMENT la
        /// première instruction du corps, afin de préserver
        /// l'alimentation des quatre libellés transverses du socle :
        /// leur omission constituerait une non-conformité au contrat
        /// de la mécanique multilingue de la famille MH, dont le
        /// socle porte du traitement à préserver, à la différence de
        /// l'implémentation par défaut de
        /// <see cref="VM_Generic"/>.</para>
        /// <para>Résolution : Le libellé propre est résolu depuis la
        /// clé <c>MH_Ti_23</c>. Une affectation par ligne, sans
        /// boucle dynamique. Aucune affectation locale des quatre
        /// libellés transverses n'est opérée.</para>
        /// <para>Filet de sécurité : Aucun <c>try</c>/<c>catch</c>
        /// local n'est posé et aucune logique de repli locale n'est
        /// admise. Le filet est porté exclusivement par le service de
        /// dictionnaire, qui journalise en interne toute anomalie et
        /// résout par une valeur de repli sans propager d'exception
        /// au présent ViewModel (R-4.11.8 du 0231).</para>
        /// </remarks>
        protected override void LoadLabels(string callChain)
        {
            base.LoadLabels(callChain);

            Label_MH_CuttingScrapStock = _dictionary.GetText(callChain, "MH_Ti_23");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de la commande
        /// <see cref="CuttingScrapStockCommand"/> : déclenche la
        /// navigation vers la page de gestion du stock de chutes
        /// (Page30), sur le patron strictement identique aux cinq
        /// handlers transverses du socle.
        /// </summary>
        /// <returns>Une tâche représentant l'exécution asynchrone de
        /// la navigation.</returns>
        /// <remarks>
        /// <para>Contexte : Encadre l'invocation de
        /// <see cref="IU_Navigation.NavigateToPageAsync"/> par le
        /// pattern <c>BeginProcessing</c> / <c>try</c> /
        /// <c>finally</c> / <c>EndProcessing</c>, garantissant la
        /// remise à <see langword="false"/> de
        /// <see cref="VM_MH_Generic.IsProcessing"/> en toute
        /// circonstance, et par le filet de sécurité hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> alimenté par une
        /// CallChain initiale construite via
        /// <c>BuildFirstCallChain</c>. Le bloc <c>try</c>/
        /// <c>finally</c> ne capture rien : aucun
        /// <c>try</c>/<c>catch</c> local n'est posé en sus du filet
        /// hérité, et le traitement terminal des erreurs lui est
        /// intégralement délégué.</para>
        /// <para>Consommation d'<see cref="IU_Navigation"/> : Opérée
        /// par le champ <c>protected readonly</c> <c>_navigation</c>
        /// hérité de <see cref="VM_MH_Generic"/> au titre de l'EA-05,
        /// sans rétention locale, sans injection propre, sans
        /// médiation par <c>IS_UseCaseInvoker</c> et sans UseCase
        /// métier intermédiaire — la commande étant purement
        /// navigationnelle.</para>
        /// <para>Jeton d'annulation :
        /// <see cref="CancellationToken.None"/> est passé
        /// explicitement en argument, tant au filet qu'à l'opération
        /// de navigation. Le contrat de commande WPF exposé par
        /// <see cref="UT_RelayCommandArg0Async"/> ne véhicule aucun
        /// jeton, de sorte qu'aucune annulation coopérative n'est
        /// disponible au présent handler.</para>
        /// <para>Nom de page : Le littéral <c>"Page30"</c> est
        /// strictement identique à celui sur lequel <c>MH10</c>
        /// conditionne la visibilité du bouton correspondant, au
        /// titre de R-4.13.14 du 0231.</para>
        /// </remarks>
        private async Task ExecuteCuttingScrapStockAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.NavigateToPageAsync(
                        callChain, "Page30", CancellationToken.None);
                }, CancellationToken.None);
            }
            finally
            {
                EndProcessing();
            }
        }

        #endregion
    }
}