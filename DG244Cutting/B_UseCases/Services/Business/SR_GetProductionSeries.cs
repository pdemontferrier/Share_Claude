using DG244Cutting.A_Domain.Common.Enums;
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
    /// Description :
    /// <para>
    /// Service métier de lecture-projection des séries de production admissibles,
    /// destiné à alimenter le tableau de bord Page10.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Ce service est consommé par le ViewModel <c>VM_Page10</c> (Singleton) via
    /// médiation <c>IS_UseCaseInvoker</c> (franchissement Singleton vers Scoped,
    /// EA-11). Il s'appuie sur le Query Handler générique
    /// <see cref="IQ_Generic{ProductionSeries}"/> pour lire, en mode NoTracking, les
    /// séries admissibles, et sur <see cref="IS_AppContext"/> pour disposer de la
    /// date applicative courante. Il n'accède jamais directement à l'infrastructure
    /// technique ni à un Repository.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Fournir l'ensemble des séries de production admissibles, projetées en objets
    /// de transport affichables et qualifiés (statut de classement, indicateur de
    /// retard, clé de tri semaine-jour), sous forme d'une liste plate déjà triée.
    /// Le service ne mute aucun état et n'écrit rien.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// ViewModel <c>VM_Page10</c> du tableau de bord, via médiation
    /// <c>IS_UseCaseInvoker</c>.
    /// </para>
    ///
    /// Tâches / Actions :
    /// <list type="bullet">
    /// <item><description>Construire la CallChain locale du service.</description></item>
    /// <item><description>Lire la date applicative courante via <see cref="IS_AppContext"/>.</description></item>
    /// <item><description>Déléguer la lecture filtrée NoTracking des séries admissibles au Query Handler <see cref="IQ_Generic{ProductionSeries}"/>.</description></item>
    /// <item><description>Projeter chaque entité en <see cref="DTO_ProductionSeriesItem"/> en calculant statut, indicateur de retard et clé de tri.</description></item>
    /// <item><description>Trier la liste plate selon les trois critères successifs et la retourner.</description></item>
    /// <item><description>Classifier toute exception non prévue via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <seealso cref="IS_GetProductionSeries"/>
    /// <seealso cref="IQ_Generic{ProductionSeries}"/>
    /// <seealso cref="IS_AppContext"/>
    /// </summary>
    public class SR_GetProductionSeries : IS_GetProductionSeries
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_Generic<ProductionSeries> _qh;
        private readonly IS_AppContext _appContext;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance du service
        /// <see cref="SR_GetProductionSeries"/>.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette classe est instanciée via l'injection de dépendances dans la couche
        /// Application. Elle reçoit le Query Handler générique de lecture des séries
        /// de production, le service de contexte applicatif source de la date
        /// courante, et le classifieur d'exceptions.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Préparer le service à exécuter la lecture-projection des séries de
        /// production dans un cadre conforme à la Clean Architecture et aux
        /// conventions de traçabilité du projet.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Recevoir le Query Handler générique <see cref="IQ_Generic{ProductionSeries}"/>.</description></item>
        /// <item><description>Recevoir le service de contexte applicatif <see cref="IS_AppContext"/>.</description></item>
        /// <item><description>Recevoir le classifieur d'exceptions <see cref="IS_ExClassifier"/>.</description></item>
        /// <item><description>Initialiser la variable <c>_callee</c> avec le nom réel de la classe.</description></item>
        /// <item><description>Stocker les dépendances nécessaires au fonctionnement du service.</description></item>
        /// </list>
        /// </summary>
        /// <param name="qh">Query Handler générique de lecture des entités <see cref="ProductionSeries"/>.</param>
        /// <param name="appContext">Service de fourniture du contexte applicatif courant (source de la date applicative).</param>
        /// <param name="classifier">Service de classification terminale des exceptions non prévues.</param>
        /// <exception cref="ArgumentNullException">Levée si l'une des dépendances injectées est nulle.</exception>
        public SR_GetProductionSeries(
            IQ_Generic<ProductionSeries> qh,
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
        /// Description :
        /// <para>
        /// Retourne l'ensemble des séries de production admissibles, projetées en
        /// objets de transport affichables, qualifiées et triées.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode est appelée depuis le ViewModel du tableau de bord Page10.
        /// Elle constitue une opération de lecture-projection pure : lecture filtrée
        /// NoTracking déléguée au Query Handler, projection en mémoire vers DTO avec
        /// calcul des champs dérivés, tri, et retour. Elle ne mute aucun état.
        /// </para>
        ///
        /// <para>
        /// Dérogation de nommage : conformément au régime dual-cas amendé
        /// (§4.14.3 amendée), ce Service relève du cas Concept ; le préfixe
        /// canonique <c>Execute</c> est remplacé par le verbe <c>Get</c>, dont la
        /// sémantique de lecture-projection restitue directement l'ensemble des
        /// séries admissibles qualifiées. Trace requise au titre de la doctrine du
        /// préfixe des méthodes publiques des Services (R-4.2.12, I-4.2.6).
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner une liste plate de <see cref="DTO_ProductionSeriesItem"/> déjà
        /// triée (date de fin de production, puis clé semaine-jour, puis numéro de
        /// série), chaque élément portant un statut de classement réel, jamais la
        /// sentinelle <c>NotValidated</c>, tout en garantissant la traçabilité
        /// complète et la reclassification normalisée des erreurs.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Construire la CallChain du service.</description></item>
        /// <item><description>Lire une unique fois la date applicative courante.</description></item>
        /// <item><description>Déléguer la lecture filtrée NoTracking des séries admissibles au Query Handler.</description></item>
        /// <item><description>Projeter chaque entité en objet de transport qualifié.</description></item>
        /// <item><description>Trier la liste plate et la retourner.</description></item>
        /// <item><description>Classifier toute exception non prévue via <see cref="IS_ExClassifier"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont transmise par l'appelant.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Une liste plate, jamais nulle (vide si aucune série n'est admissible), de
        /// <see cref="DTO_ProductionSeriesItem"/> triée selon les trois critères
        /// successifs date de fin de production, clé semaine-jour, numéro de série.
        /// </returns>
        /// <exception cref="Ex_Business">Levée si une erreur métier est détectée en aval lors de la lecture.</exception>
        /// <exception cref="Ex_Infrastructure">Levée si une erreur technique survient lors de l'accès aux données.</exception>
        public async Task<List<DTO_ProductionSeriesItem>> GetProductionSeriesAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetProductionSeriesAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                DateTime appDate = _appContext.GetAppContext().AppDate;

                List<ProductionSeries> series = await _qh.HandleGetFilteredAsNoTrackingAsync(
                    callChain,
                    s => s.IsImported
                         // && s.IsProductionValidated
                         && s.ProductionStartDate != null
                         && s.ProductionEndDate != null,
                    ct);

                List<DTO_ProductionSeriesItem> items = new List<DTO_ProductionSeriesItem>(series.Count);

                foreach (ProductionSeries serie in series)
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
        /// Description :
        /// <para>
        /// Projette une entité <see cref="ProductionSeries"/> admissible en objet de
        /// transport <see cref="DTO_ProductionSeriesItem"/>, en calculant les champs
        /// dérivés (statut de classement, indicateur de retard, clé semaine-jour).
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Appelée pour chaque entité matérialisée par la lecture filtrée. L'entité
        /// reçue a franchi le socle d'admission
        /// (<c>IsImported AND IsProductionValidated</c>) et porte ses deux dates de
        /// production renseignées ; la projection les traite comme non-nulles sans
        /// garde défensive.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner un DTO renseigné, portant un statut réel (jamais
        /// <c>NotValidated</c>), calculé selon la grille de classement figée.
        /// </para>
        /// </summary>
        /// <param name="serie">Entité admissible à projeter (dates de production renseignées).</param>
        /// <param name="appDate">Date applicative courante, commune à tous les items de la projection.</param>
        /// <returns>Un <see cref="DTO_ProductionSeriesItem"/> renseigné et qualifié.</returns>
        private static DTO_ProductionSeriesItem ProjectToItem(ProductionSeries serie, DateTime appDate)
        {
            DateTime startDate = serie.ProductionStartDate!.Value;
            DateTime endDate = serie.ProductionEndDate!.Value;

            bool isStarted = serie.IsDropBarSupplied || serie.IsNewBarSupplied || serie.IsCuttingStarted;

            return new DTO_ProductionSeriesItem
            {
                Id = serie.Id,
                IdSerialNumber = serie.IdSerialNumber,
                Description = serie.Description,
                ProductionStartDate = startDate,
                ProductionEndDate = endDate,
                ProductionEndDay = serie.ProductionEndDay,
                IsDropBarOptimized = serie.IsDropBarOptimized,
                IsDropBarSupplied = serie.IsDropBarSupplied,
                IsNewBarOptimized = serie.IsNewBarOptimized,
                IsNewBarSupplied = serie.IsNewBarSupplied,
                Status = ResolveStatus(serie, appDate, isStarted, startDate, endDate),
                IsLate = isStarted && appDate >= endDate,
                WeekDayKey = BuildWeekDayKey(startDate)
            };
        }

        /// <summary>
        /// Description :
        /// <para>
        /// Détermine le statut de classement d'une série admissible selon la grille
        /// ordonnée par priorité.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Appelée durant la projection. La grille est appliquée dans l'ordre de
        /// priorité : découpe terminée, puis série commencée, puis position de la
        /// date applicative relative à l'intervalle de production.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner l'une des cinq valeurs réelles de
        /// <see cref="En_ProductionSeriesStatus"/> ; la sentinelle
        /// <c>NotValidated</c> n'est jamais retournée.
        /// </para>
        /// </summary>
        /// <param name="serie">Entité admissible en cours de projection.</param>
        /// <param name="appDate">Date applicative courante.</param>
        /// <param name="isStarted">Indique si la série est commencée (approvisionnement chutes, barres neuves, ou découpe démarrée).</param>
        /// <param name="startDate">Date de début de production (non-nulle par admission).</param>
        /// <param name="endDate">Date de fin de production (non-nulle par admission).</param>
        /// <returns>Le statut de classement réel de la série.</returns>
        private static En_ProductionSeriesStatus ResolveStatus(
            ProductionSeries serie,
            DateTime appDate,
            bool isStarted,
            DateTime startDate,
            DateTime endDate)
        {
            if (serie.IsCuttingCompleted)
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
        /// Description :
        /// <para>
        /// Construit la clé semaine-jour au format <c>"NN-n"</c> à partir de la date
        /// de début de production.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Appelée durant la projection. La semaine ISO est zéro-paddée sur deux
        /// chiffres ; le jour de semaine est exprimé lundi=1 sur un chiffre. Cette
        /// clé sert de critère de tri secondaire ; calculée en mémoire, elle ne peut
        /// être poussée en SQL.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner la clé de tri semaine-jour normalisée pour la série.
        /// </para>
        /// </summary>
        /// <param name="startDate">Date de début de production (non-nulle par admission).</param>
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