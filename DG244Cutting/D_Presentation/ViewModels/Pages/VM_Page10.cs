using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Interfaces.Services.Business;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Business;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel du tableau de bord des séries de production
    /// <c>Page10</c> de l'application DG244Cutting, page d'accueil et
    /// point d'entrée du flux de traitement d'une série, exposant à la
    /// vue les séries de production admissibles réparties en cinq listes
    /// selon leur statut de classement (à réaliser, en retard, en cours,
    /// réalisées, à venir) et portant l'action de sélection d'une série
    /// qui mémorise le choix dans l'état métier partagé puis navigue vers
    /// la page adéquate du flux de traitement.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page10"/> et
    /// dérivé de <see cref="VM_Page_Generic"/>. La page constitue le
    /// tableau de bord d'accueil affiché après authentification : elle
    /// présente en cinq colonnes les séries de production admissibles
    /// (ayant franchi le socle d'admission en amont), chaque colonne
    /// correspondant à l'un des cinq statuts réels de
    /// <see cref="En_ProductionSeriesStatus"/>. La sélection d'une série
    /// par clic simple sur une ligne mémorise la série dans l'état métier
    /// partagé puis déclenche la navigation vers la première page du flux
    /// de traitement adéquate au regard de l'état d'approvisionnement et
    /// du statut de la série.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page10"/> :</para>
    /// <list type="bullet">
    ///   <item><description>six propriétés observables multilingues —
    ///   <see cref="PageName"/> (clé <c>P10_00</c>, titre de la page selon
    ///   la convention <c>PXX_00 = nom de page</c>) et
    ///   <see cref="Label_P10_01"/> à <see cref="Label_P10_05"/> (clés
    ///   <c>P10_01</c> à <c>P10_05</c>, en-têtes des cinq colonnes),
    ///   alimentées par la mécanique multilingue factorisée par
    ///   <see cref="VM_Generic"/> : premier chargement au constructeur via
    ///   <see cref="VM_Generic.InitializeLabels"/>, rechargement
    ///   automatique à tout changement de langue dynamique ;</description></item>
    ///   <item><description>cinq collections observables
    ///   <see cref="SeriesToDo"/>, <see cref="SeriesOverdue"/>,
    ///   <see cref="SeriesInProgress"/>, <see cref="SeriesCompleted"/> et
    ///   <see cref="SeriesUpcoming"/>, en correspondance directe avec les
    ///   cinq statuts réels de <see cref="En_ProductionSeriesStatus"/>
    ///   (<c>ToDo=1</c> à <c>Upcoming=5</c>), alimentées à l'entrée sur la
    ///   page par <see cref="LoadAsync"/> à partir d'une liste plate déjà
    ///   triée, l'ordre de tri de la liste plate étant préservé à
    ///   l'intérieur de chaque collection ;</description></item>
    ///   <item><description>la commande <see cref="SelectSeriesCommand"/>
    ///   déclenchée au clic sur une ligne, recevant le
    ///   <see cref="DTO_ProductionSeriesItem"/> de la ligne en
    ///   argument.</description></item>
    /// </list>
    ///
    /// <para>Responsabilités : Le présent ViewModel porte le chargement à
    /// l'activation de la page de la liste plate des séries admissibles et
    /// sa répartition en cinq collections selon le statut, ainsi que
    /// l'orchestration de la sélection d'une série : mémorisation
    /// systématique de la série dans l'état métier partagé
    /// (<see cref="ISE_UseCase.SelectSeries"/>) préalablement à toute
    /// navigation, résolution de la page cible du flux de traitement selon
    /// une règle ordonnée fondée sur l'état d'approvisionnement et le
    /// statut de la série, puis délégation de la navigation à
    /// <see cref="IU_Navigation"/>.</para>
    ///
    /// <para>Non-responsabilités : Le calcul du statut de classement, de
    /// l'indicateur de retard, du code couleur de fin de production et de
    /// la clé de tri ne relèvent pas de ce ViewModel : ces champs sont
    /// déjà calculés en amont et portés par le
    /// <see cref="DTO_ProductionSeriesItem"/>. Le ViewModel ne mute ni ne
    /// persiste aucun état de série ; il se borne à répartir la liste
    /// reçue et à mémoriser la sélection dans l'état partagé. La décision
    /// effective de navigation (validation de page, contrôle de droits,
    /// redirection de repli) appartient au UseCase
    /// <see cref="IU_Navigation"/> ; le présent ViewModel se borne à
    /// résoudre le nom logique de la page cible et à le lui
    /// transmettre.</para>
    ///
    /// <para>Note EA : Ce ViewModel consomme
    /// <see cref="IS_UseCaseInvoker"/> (EA-11) pour franchir la frontière
    /// Singleton vers Scoped, tant pour la lecture des séries
    /// (<see cref="IS_GetProductionSeries"/>, Scoped) que pour la
    /// navigation (<see cref="IU_Navigation"/>), résolues via l'invocateur
    /// dans un scope dédié. Le <see cref="ISE_UseCase"/> (Singleton) est
    /// en revanche injecté en direct. Conformément à la doctrine P4-bis,
    /// la consommation via l'invocateur ne bascule pas la portée du
    /// ViewModel, qui demeure Singleton, aucune dépendance Scoped n'étant
    /// injectée directement.</para>
    ///
    /// <para>Structure : La classe est organisée en sept régions —
    /// Propriétés privées, Dépendances privées, Propriétés publiques,
    /// Constructeur, Méthodes publiques, Méthodes protégées, Méthodes
    /// privées.</para>
    /// </remarks>
    public class VM_Page10 : VM_Page_Generic
    {
        #region === Propriétés privées ===

        private string _pageName = string.Empty;
        private string _labelP10_01 = string.Empty;
        private string _labelP10_02 = string.Empty;
        private string _labelP10_03 = string.Empty;
        private string _labelP10_04 = string.Empty;
        private string _labelP10_05 = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Invocateur générique de UseCases, résolvant chaque composant
        /// dans un scope DI dédié et appliquant le filet transactionnel
        /// commun. Consommé pour la lecture des séries
        /// (<see cref="IS_GetProductionSeries"/>) et la navigation
        /// (<see cref="IU_Navigation"/>) au titre de l'EA-11.
        /// </summary>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        /// <summary>
        /// État métier partagé des sélections courantes, consommé pour
        /// mémoriser la série sélectionnée en tête de cascade
        /// préalablement à la navigation. Injecté en direct (Singleton).
        /// </summary>
        private readonly ISE_UseCase _seUseCase;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient le nom multilingue de la page <c>Page10</c>, miroir du
        /// libellé associé à la clé <c>P10_00</c> dans le dictionnaire de
        /// langue actif, selon la convention <c>PXX_00 = nom de page</c>.
        /// </summary>
        public string PageName
        {
            get => _pageName;
            private set => SetProperty(ref _pageName, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P10_01</c>, en-tête de la colonne des séries à réaliser.</summary>
        public string Label_P10_01
        {
            get => _labelP10_01;
            private set => SetProperty(ref _labelP10_01, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P10_02</c>, en-tête de la colonne des séries en retard.</summary>
        public string Label_P10_02
        {
            get => _labelP10_02;
            private set => SetProperty(ref _labelP10_02, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P10_03</c>, en-tête de la colonne des séries en cours.</summary>
        public string Label_P10_03
        {
            get => _labelP10_03;
            private set => SetProperty(ref _labelP10_03, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P10_04</c>, en-tête de la colonne des séries réalisées.</summary>
        public string Label_P10_04
        {
            get => _labelP10_04;
            private set => SetProperty(ref _labelP10_04, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P10_05</c>, en-tête de la colonne des séries à venir.</summary>
        public string Label_P10_05
        {
            get => _labelP10_05;
            private set => SetProperty(ref _labelP10_05, value);
        }

        /// <summary>
        /// Collection observable des séries à réaliser
        /// (<see cref="En_ProductionSeriesStatus.ToDo"/>), alimentée à
        /// l'entrée sur la page par <see cref="LoadAsync"/>.
        /// </summary>
        public ObservableCollection<DTO_ProductionSeriesItem> SeriesToDo { get; } = new();

        /// <summary>
        /// Collection observable des séries en retard
        /// (<see cref="En_ProductionSeriesStatus.Overdue"/>), alimentée à
        /// l'entrée sur la page par <see cref="LoadAsync"/>.
        /// </summary>
        public ObservableCollection<DTO_ProductionSeriesItem> SeriesOverdue { get; } = new();

        /// <summary>
        /// Collection observable des séries en cours
        /// (<see cref="En_ProductionSeriesStatus.InProgress"/>), alimentée
        /// à l'entrée sur la page par <see cref="LoadAsync"/>.
        /// </summary>
        public ObservableCollection<DTO_ProductionSeriesItem> SeriesInProgress { get; } = new();

        /// <summary>
        /// Collection observable des séries réalisées
        /// (<see cref="En_ProductionSeriesStatus.Completed"/>), alimentée
        /// à l'entrée sur la page par <see cref="LoadAsync"/>.
        /// </summary>
        public ObservableCollection<DTO_ProductionSeriesItem> SeriesCompleted { get; } = new();

        /// <summary>
        /// Collection observable des séries à venir
        /// (<see cref="En_ProductionSeriesStatus.Upcoming"/>), alimentée à
        /// l'entrée sur la page par <see cref="LoadAsync"/>.
        /// </summary>
        public ObservableCollection<DTO_ProductionSeriesItem> SeriesUpcoming { get; } = new();

        /// <summary>
        /// Commande de sélection d'une série, déclenchée au clic simple
        /// sur une ligne de l'une des cinq listes, recevant le
        /// <see cref="DTO_ProductionSeriesItem"/> de la ligne en argument.
        /// Mémorise la série dans l'état métier partagé puis navigue vers
        /// la page adéquate du flux de traitement.
        /// </summary>
        public ICommand SelectSeriesCommand { get; }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du ViewModel de la page
        /// <c>Page10</c>, résout ses dépendances propres, instancie la
        /// commande de sélection et déclenche le premier chargement des
        /// libellés multilingues.
        /// </summary>
        /// <param name="dictionary">Service de dictionnaire multilingue,
        /// relayé à <see cref="VM_Generic"/> par la chaîne
        /// <c>base(...)</c>.</param>
        /// <param name="logAndNotify">Service transversal de journalisation
        /// et de notification, relayé à <see cref="VM_Generic"/> au titre
        /// de l'EA-01.</param>
        /// <param name="app">Réglages applicatifs porteurs de la culture
        /// active, relayés à <see cref="VM_Generic"/> pour l'alimentation
        /// de la mécanique multilingue.</param>
        /// <param name="useCaseInvoker">Invocateur générique de UseCases,
        /// injecté en Singleton par le conteneur DI.</param>
        /// <param name="seUseCase">État métier partagé des sélections
        /// courantes, injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="useCaseInvoker"/> ou <paramref name="seUseCase"/>
        /// est <see langword="null"/>. Les gardes sur
        /// <paramref name="dictionary"/>, <paramref name="logAndNotify"/>
        /// et <paramref name="app"/> sont portées par
        /// <see cref="VM_Generic"/> via la chaîne
        /// <c>base(...)</c>.</exception>
        public VM_Page10(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IS_UseCaseInvoker useCaseInvoker,
            ISE_UseCase seUseCase)
            : base(dictionary, logAndNotify, app)
        {
            _useCaseInvoker = useCaseInvoker ?? throw new ArgumentNullException(nameof(useCaseInvoker));
            _seUseCase = seUseCase ?? throw new ArgumentNullException(nameof(seUseCase));

            SelectSeriesCommand = new UT_RelayCommandArg1Async<DTO_ProductionSeriesItem>(OnSelectSeriesAsync);

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge, à l'entrée sur la page, la liste plate des séries de
        /// production admissibles et la répartit dans les cinq collections
        /// observables selon le statut de classement de chaque série,
        /// l'ordre de tri de la liste plate étant préservé à l'intérieur
        /// de chaque collection.
        /// </summary>
        /// <param name="callChain">Chaîne d'appel de l'invocateur amont.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération de chargement.</returns>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                var list = await _useCaseInvoker
                    .InvokeAsync<IS_GetProductionSeries, List<DTO_ProductionSeriesItem>>(
                        (service, innerCt) => service.GetProductionSeriesAsync(
                            innerCallChain, innerCt),
                        ct);

                SeriesToDo.Clear();
                SeriesOverdue.Clear();
                SeriesInProgress.Clear();
                SeriesCompleted.Clear();
                SeriesUpcoming.Clear();

                foreach (var item in list)
                {
                    switch (item.Status)
                    {
                        case En_ProductionSeriesStatus.ToDo:
                            SeriesToDo.Add(item);
                            break;
                        case En_ProductionSeriesStatus.Overdue:
                            SeriesOverdue.Add(item);
                            break;
                        case En_ProductionSeriesStatus.InProgress:
                            SeriesInProgress.Add(item);
                            break;
                        case En_ProductionSeriesStatus.Completed:
                            SeriesCompleted.Add(item);
                            break;
                        case En_ProductionSeriesStatus.Upcoming:
                            SeriesUpcoming.Add(item);
                            break;
                        default:
                            Debug.WriteLine(
                                $"[VM_Page10.LoadAsync] Statut de série inattendu ignoré : "
                                + $"{item.Status} (série {item.IdSerialNumber}).");
                            break;
                    }
                }
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Recharge les six libellés multilingues de la page — le titre de
        /// page et les cinq en-têtes de colonnes — à partir du
        /// dictionnaire actif. Invoquée au premier chargement et à chaque
        /// changement de langue par la mécanique factorisée de
        /// <see cref="VM_Generic"/>.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de la mécanique amont.</param>
        protected override void LoadLabels(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadLabels)}";

            PageName = _dictionary.GetText(callChain, "P10_00");
            Label_P10_01 = _dictionary.GetText(callChain, "P10_01");
            Label_P10_02 = _dictionary.GetText(callChain, "P10_02");
            Label_P10_03 = _dictionary.GetText(callChain, "P10_03");
            Label_P10_04 = _dictionary.GetText(callChain, "P10_04");
            Label_P10_05 = _dictionary.GetText(callChain, "P10_05");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de la commande de sélection d'une série : mémorise la
        /// série sélectionnée dans l'état métier partagé, résout le nom
        /// logique de la page cible du flux de traitement, puis délègue la
        /// navigation à <see cref="IU_Navigation"/>. La sélection est
        /// toujours mémorisée préalablement à la navigation.
        /// </summary>
        /// <param name="dto">Série sélectionnée, transmise depuis la ligne
        /// cliquée.</param>
        /// <returns>Tâche représentant l'opération de sélection.</returns>
        private async Task OnSelectSeriesAsync(DTO_ProductionSeriesItem dto)
        {
            string callChain = BuildFirstCallChain();

            await ExecuteSafeAsync(callChain, async () =>
            {
                _seUseCase.SelectSeries(dto);

                string page = ResolveTargetPage(dto);

                await _useCaseInvoker
                    .InvokeAsync<IU_Navigation>(
                        (navigation, innerCt) => navigation.NavigateToPageAsync(
                            callChain, page, innerCt),
                        ct: default);
            });
        }

        /// <summary>
        /// Résout le nom logique de la première page du flux de traitement
        /// vers laquelle naviguer au regard de l'état d'approvisionnement
        /// et du statut de la série, selon une règle ordonnée.
        /// </summary>
        /// <remarks>
        /// <para>Règle ordonnée : chutes non approvisionnées →
        /// <c>Page11</c> ; barres neuves non approvisionnées →
        /// <c>Page12</c> ; série réalisée → <c>Page13</c> ; sinon découpe
        /// en cours ou validation de la première barre → <c>Page20</c>.
        /// L'ordre chutes → barres neuves étant garanti métier, le cas
        /// chutes approvisionnées et barres neuves non approvisionnées est
        /// structurellement impossible ; l'ordre des tests est néanmoins
        /// écrit pour rester correct si l'invariant était violé.</para>
        /// </remarks>
        /// <param name="dto">Série dont la page cible est résolue.</param>
        /// <returns>Nom logique de la page cible.</returns>
        private static string ResolveTargetPage(DTO_ProductionSeriesItem dto)
        {
            if (!dto.IsDropBarSupplied) return "Page11";
            if (!dto.IsNewBarSupplied) return "Page12";
            if (dto.Status == En_ProductionSeriesStatus.Completed) return "Page13";
            return "Page20";
        }

        #endregion
    }
}