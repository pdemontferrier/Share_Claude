using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;
using System.Globalization;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier de lecture-projection des séries de production admissibles relevant du
    /// périmètre de découpe piloté, alimentant le tableau de bord Page10.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionSeries_Get"/>, par le tableau de bord des séries, à chaque affichage
    /// de celui-ci. Il délègue la lecture au Query Handler générique
    /// <see cref="IQ_Generic{T}"/> paramétré sur <see cref="vw_ProductionSeries_Full"/> et lit la
    /// date applicative courante via <see cref="IS_AppContext"/>. Il n'accède jamais directement à
    /// l'infrastructure technique ni à un Repository.
    /// </para>
    /// <para>
    /// Objectif : présenter à l'opérateur l'ensemble des séries sur lesquelles il est susceptible
    /// d'intervenir, réparties en cinq statuts de classement - en retard, à faire, en cours,
    /// terminées, à venir - afin qu'il choisisse celle à traiter. La liste restituée est plate,
    /// déjà qualifiée et déjà triée, de sorte que le composant consommateur ne porte ni règle de
    /// classement, ni règle de tri.
    /// </para>
    /// <para>
    /// Socle d'admission : une série est restituée lorsqu'elle est importée, qu'elle porte ses deux
    /// dates de production, et qu'elle comporte au moins une pièce de découpe dans le périmètre
    /// piloté. Les deux premières conditions sont portées par le prédicat applicatif ; la troisième
    /// est garantie par la source de lecture <see cref="vw_ProductionSeries_Full"/>, dont le contenu
    /// est restreint à ces seules séries. Le filtre de suppression logique est porté par la même
    /// source. Aucune de ces deux garanties n'est redoublée côté applicatif.
    /// </para>
    /// <para>
    /// Source de lecture : la vue est une surface de lecture de l'entité fonctionnelle Série de
    /// production ; elle est sans clé et n'expose aucune propriété de navigation. Les écritures
    /// portées sur une série continuent de passer par l'entité et relèvent de composants distincts.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire la CallChain locale du service et honorer le jeton d'annulation.</description></item>
    /// <item><description>Lire une unique fois la date applicative courante via <see cref="IS_AppContext"/>.</description></item>
    /// <item><description>Déléguer la lecture filtrée sans suivi des changements au Query Handler générique via <see cref="IQ_Generic{T}.HandleGetFilteredAsNoTrackingAsync"/>.</description></item>
    /// <item><description>Projeter chaque ligne lue en <see cref="DTO_ProductionSeriesItem"/> en calculant statut, indicateur de retard et clé semaine-jour.</description></item>
    /// <item><description>Trier la liste plate selon les trois critères successifs et la retourner.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne mute aucun état, n'écrit rien et n'appelle jamais <c>SaveChangesAsync</c> ; n'ouvre, ne valide ni n'annule aucune transaction.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// <item><description>Ne détermine pas le périmètre de découpe, qui réside en base dans la définition de la source de lecture.</description></item>
    /// <item><description>Ne projette aucun compteur d'avancement ni libellé de couleur exposés par la source : le contenu de l'objet de transport est stable.</description></item>
    /// <item><description>Ne répartit pas les éléments par statut d'affichage : cette répartition relève du composant consommateur.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionSeries_Get"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IS_AppContext"/>
    public class SR_ProductionSeries_Get : IS_ProductionSeries_Get
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du type concret, utilisé comme segment propre dans les CallChains construites par le
        /// service.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Query Handler générique auquel est déléguée la lecture filtrée, sans suivi des
        /// changements, de la surface de lecture des séries de production.
        /// </summary>
        private readonly IQ_Generic<vw_ProductionSeries_Full> _qh;

        /// <summary>
        /// Service de fourniture du contexte applicatif, source de la date applicative courante qui
        /// détermine le statut de classement de chaque série.
        /// </summary>
        private readonly IS_AppContext _appContext;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionSeries_Get"/> avec ses
        /// dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation. Le Query Handler générique est paramétré sur
        /// <see cref="vw_ProductionSeries_Full"/> et résolu par l'enregistrement en générique ouvert
        /// du composition root ; aucun enregistrement dédié n'est requis. Le nom réel du type est
        /// capté à la construction pour alimenter les CallChains émises par le service.
        /// </para>
        /// </remarks>
        /// <param name="qh">Query Handler générique consommé pour la lecture de la surface de lecture des séries. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="appContext">Service de fourniture du contexte applicatif courant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification terminale des exceptions non prévues. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="qh"/>, <paramref name="appContext"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionSeries_Get(
            IQ_Generic<vw_ProductionSeries_Full> qh,
            IS_AppContext appContext,
            IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _qh = qh ?? throw new ArgumentNullException(nameof(qh));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne l'ensemble des séries de production admissibles, projetées en objets de
        /// transport qualifiés et triés.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Dérogation de nommage : ce Service relève du cas Concept ; le préfixe canonique
        /// <c>Execute</c> est remplacé par le verbe <c>Get</c>, dont la sémantique de
        /// lecture-projection restitue directement l'ensemble des séries admissibles qualifiées.
        /// Trace requise au titre de la doctrine du préfixe des méthodes publiques des Services.
        /// </para>
        /// <para>
        /// Contexte : appelée à chaque affichage du tableau de bord des séries - ouverture de
        /// l'application, retour de parcours de découpe, rafraîchissement explicite. L'opération est
        /// une lecture-projection pure : une seule lecture de la date applicative, un seul appel de
        /// lecture en base, puis projection et tri en mémoire. Aucun aller-retour en base par ligne
        /// n'est émis.
        /// </para>
        /// <para>
        /// Objectif : restituer une liste plate ordonnée sur trois critères successifs - date de fin
        /// de production, puis clé semaine-jour en comparaison ordinale, puis numéro de série - dont
        /// chaque élément porte l'un des cinq statuts réels de
        /// <see cref="En_ProductionSeriesStatus"/> ; la valeur sentinelle <c>NotValidated</c> n'est
        /// jamais retournée.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire la CallChain du service et honorer le jeton d'annulation en entrée de traitement.</description></item>
        /// <item><description>Lire une unique fois la date applicative courante.</description></item>
        /// <item><description>Déléguer la lecture filtrée sans suivi des changements au Query Handler générique.</description></item>
        /// <item><description>Projeter chaque ligne lue en objet de transport qualifié, puis trier et retourner la liste.</description></item>
        /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne porte aucune validation d'argument : aucun identifiant n'est reçu et <paramref name="caller"/> n'est pas contrôlé.</description></item>
        /// <item><description>N'intercepte ni ne requalifie les exceptions typées remontant de l'aval, qui sont relancées telles quelles.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// CallChain amont transmise par l'appelant, jamais consommée autrement que par
        /// concaténation. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Liste plate des séries de production admissibles, triée par date de fin de production,
        /// puis par clé semaine-jour en comparaison ordinale, puis par numéro de série. Retourne une
        /// liste vide si aucune série n'est admissible ; ne retourne jamais <see langword="null"/>.
        /// </returns>
        /// <exception cref="Ex_Business">Levée lorsqu'une erreur métier est détectée en aval lors de la lecture.</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors de l'accès aux données.</exception>
        /// <exception cref="OperationCanceledException">Levée lorsque l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task<List<DTO_ProductionSeriesItem>> GetProductionSeriesAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetProductionSeriesAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                DateTime appDate = _appContext.GetAppContext().AppDate;

                List<vw_ProductionSeries_Full> series = await _qh.HandleGetFilteredAsNoTrackingAsync(
                    callChain,
                    v => v.PSIsImported
                         // Maintien en commentaire délibéré, hors périmètre du présent fil, sur arbitrage du développeur.
                         // && v.PSIsProductionValidated
                         // Les deux tests de nullité ci-dessous garantissent la validité des déréférencements !.Value de la projection.
                         && v.PSProductionStartDate != null
                         && v.PSProductionEndDate != null,
                    ct);

                List<DTO_ProductionSeriesItem> items = new List<DTO_ProductionSeriesItem>(series.Count);

                foreach (vw_ProductionSeries_Full serie in series)
                {
                    items.Add(ProjectToItem(serie, appDate));
                }

                List<DTO_ProductionSeriesItem> sorted = items
                    .OrderBy(i => i.ProductionEndDate)
                    .ThenBy(i => i.WeekDayKey, StringComparer.Ordinal)
                    .ThenBy(i => i.IdSerialNumber)
                    .ToList();

                return sorted;
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Projette une ligne de la surface de lecture des séries en objet de transport, en
        /// calculant les champs dérivés.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée pour chaque ligne matérialisée par la lecture filtrée. La ligne reçue
        /// a franchi le socle d'admission - série importée, deux dates de production renseignées, et
        /// appartenance au périmètre de découpe garantie par la source de lecture. Les deux dates
        /// sont en conséquence traitées comme non nulles, sans garde défensive supplémentaire.
        /// </para>
        /// <para>
        /// Objectif : retourner un objet de transport renseigné, portant un statut réel - jamais la
        /// sentinelle <c>NotValidated</c> - calculé selon la grille de classement figée, ainsi que
        /// l'indicateur de retard et la clé de tri semaine-jour.
        /// </para>
        /// <para>
        /// Règle de démarrage : une série est considérée comme commencée dès lors qu'elle est
        /// approvisionnée en barres de chute ou en barres neuves, ou que sa découpe est démarrée.
        /// Cette qualification alimente à la fois le statut et l'indicateur de retard.
        /// </para>
        /// </remarks>
        /// <param name="serie">Ligne admissible à projeter, dates de production renseignées.</param>
        /// <param name="appDate">Date applicative courante, commune à tous les éléments de la projection.</param>
        /// <returns>Un <see cref="DTO_ProductionSeriesItem"/> renseigné et qualifié.</returns>
        private static DTO_ProductionSeriesItem ProjectToItem(vw_ProductionSeries_Full serie, DateTime appDate)
        {
            DateTime startDate = serie.PSProductionStartDate!.Value;
            DateTime endDate = serie.PSProductionEndDate!.Value;

            bool isStarted = serie.PSIsDropBarSupplied || serie.PSIsNewBarSupplied || serie.PSIsCuttingStarted;

            return new DTO_ProductionSeriesItem
            {
                Id = serie.PSId,
                IdSerialNumber = serie.PSIdSerialNumber,
                Description = serie.PSDescription,
                ProductionStartDate = startDate,
                ProductionEndDate = endDate,
                ProductionEndDay = serie.PSProductionEndDay,
                IsDropBarOptimized = serie.PSIsDropBarOptimized,
                IsDropBarSupplied = serie.PSIsDropBarSupplied,
                IsNewBarOptimized = serie.PSIsNewBarOptimized,
                IsNewBarSupplied = serie.PSIsNewBarSupplied,
                IsBarOutOfStock = serie.PSIsBarOutOfStock,
                Status = ResolveStatus(serie, appDate, isStarted, startDate, endDate),
                IsLate = isStarted && appDate >= endDate,
                WeekDayKey = BuildWeekDayKey(startDate)
            };
        }

        /// <summary>
        /// Détermine le statut de classement d'une série admissible selon la grille ordonnée par
        /// priorité.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée durant la projection, pour chaque ligne admissible. La grille est
        /// appliquée dans un ordre de priorité strict : découpe terminée, puis série commencée, puis
        /// position de la date applicative relative à l'intervalle de production. Le premier test
        /// satisfait emporte la décision.
        /// </para>
        /// <para>
        /// Objectif : retourner l'une des cinq valeurs réelles de
        /// <see cref="En_ProductionSeriesStatus"/> ; la sentinelle <c>NotValidated</c> n'est jamais
        /// retournée, l'admission en amont garantissant que la série est qualifiable.
        /// </para>
        /// </remarks>
        /// <param name="serie">Ligne admissible en cours de projection.</param>
        /// <param name="appDate">Date applicative courante.</param>
        /// <param name="isStarted">Indique si la série est commencée : approvisionnement en chutes, en barres neuves, ou découpe démarrée.</param>
        /// <param name="startDate">Date de début de production, non nulle par admission.</param>
        /// <param name="endDate">Date de fin de production, non nulle par admission.</param>
        /// <returns>Le statut de classement réel de la série.</returns>
        private static En_ProductionSeriesStatus ResolveStatus(
            vw_ProductionSeries_Full serie,
            DateTime appDate,
            bool isStarted,
            DateTime startDate,
            DateTime endDate)
        {
            if (serie.PSIsCuttingCompleted)
            {
                return En_ProductionSeriesStatus.Completed;
            }

            if (isStarted)
            {
                return En_ProductionSeriesStatus.InProgress;
            }

            if (appDate >= endDate)
            {
                return En_ProductionSeriesStatus.Overdue;
            }

            if (appDate >= startDate && appDate < endDate)
            {
                return En_ProductionSeriesStatus.ToDo;
            }

            return En_ProductionSeriesStatus.Upcoming;
        }

        /// <summary>
        /// Construit la clé de tri semaine-jour au format <c>"NN-n"</c> à partir de la date de début
        /// de production.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée durant la projection. La semaine ISO est zéro-paddée sur deux
        /// chiffres ; le jour de semaine est exprimé lundi = 1, dimanche = 7, sur un chiffre, ce qui
        /// impose de reclasser la valeur zéro renvoyée pour le dimanche par la plateforme. Calculée
        /// en mémoire, cette clé ne peut être poussée en base.
        /// </para>
        /// <para>
        /// Objectif : retourner une clé normalisée dont la comparaison ordinale restitue l'ordre
        /// chronologique semaine puis jour, exploitée comme critère de tri secondaire.
        /// </para>
        /// </remarks>
        /// <param name="startDate">Date de début de production, non nulle par admission.</param>
        /// <returns>La clé semaine-jour au format <c>"NN-n"</c>.</returns>
        private static string BuildWeekDayKey(DateTime startDate)
        {
            int isoWeek = ISOWeek.GetWeekOfYear(startDate);

            int isoDay = (int)startDate.DayOfWeek;
            if (isoDay == 0)
            {
                isoDay = 7;
            }

            return $"{isoWeek:D2}-{isoDay}";
        }

        #endregion
    }
}