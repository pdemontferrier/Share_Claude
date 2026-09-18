using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de l'inscription, sur une barre de production validée avec
    /// défauts, du plan de coupe recomposé par le moteur d'optimisation, et du scellement de
    /// cette barre.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside
    /// en <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionBar_UpdateOptimization"/>, par le UseCase orchestrateur de la
    /// validation de barre (<c>UC_BarValidation</c>), sur le seul chemin de validation avec
    /// défauts. Il consomme directement <see cref="IQ_Generic{T}"/> pour la lecture préalable
    /// de la barre et <see cref="IC_Generic{T}"/> pour sa mise à jour, sur l'entité
    /// <see cref="ProductionBar"/>.
    /// </para>
    /// <para>
    /// Objectif : les zones défectueuses déclarées par l'opérateur à la validation d'une barre
    /// consomment de la matière et rendent irréalisable le plan de coupe provisoire. Une fois ce
    /// plan défait puis recomposé par <see cref="IS_CuttingOptimizer.OptimizeWithDefects"/> sur
    /// la matière réellement exploitable, le service inscrit sur la barre la description du
    /// nouveau plan (nombre de découpes, longueur du reste, qualification du reste) et scelle la
    /// barre. Le plan est dès lors établi et la découpe peut commencer. Le service délègue la
    /// mise à jour au Command Handler générique, sans exposer la logique de persistance ni
    /// assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Écriture indissociable : la description du plan recomposé et le scellement sont
    /// positionnés sur la même instance et confiés ensemble au Command Handler, après réussite
    /// de l'ensemble des contrôles. Un échec survenant avant la modification laisse la barre
    /// intacte ; aucun état intermédiaire, dans lequel la barre porterait un plan recomposé
    /// sans être scellée, ne peut être produit.
    /// </para>
    /// <para>
    /// Contrôle de séquence : le service est le seul point où une inversion dans la séquence de
    /// validation peut être détectée. Il n'accepte qu'une barre validée, non encore scellée, et
    /// qui n'est ni supprimée logiquement, ni refusée, ni épuisée, ni en rupture de stock. Il
    /// vérifie en outre que le résultat reçu a été calculé pour cette barre, en comparant
    /// l'origine du contenant, la chute source et la longueur de barre ; les contrôles propres
    /// au résultat seul (longueur positive, cohérence entre origine et chute source) sont
    /// impliqués par cette égalité et ne sont pas repris.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier les préconditions structurelles des arguments et la nature du résultat d'optimisation reçu.</description></item>
    /// <item><description>Charger la barre désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence de la barre, la compatibilité de son état avec le scellement et la cohérence du résultat avec la barre.</description></item>
    /// <item><description>Reporter sur la barre le nombre de découpes, la longueur du reste et sa qualification, puis la sceller.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne porte aucune règle de calcul : le nombre de découpes, la longueur du reste et sa qualification sont repris tels que déterminés par le moteur d'optimisation, sans recalcul ni contrôle de bornes.</description></item>
    /// <item><description>Ne valide pas la barre, n'enregistre pas ses défauts, ne détache ni ne rattache aucune découpe, et ne qualifie pas le reliquat en fin de barre.</description></item>
    /// <item><description>Ne modifie aucun autre champ que le nombre de découpes, la longueur du reste, sa qualification et l'indicateur de scellement ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionBar_UpdateOptimization"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionBar_UpdateOptimization : IS_ProductionBar_UpdateOptimization
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du type concret, utilisé comme segment propre dans les CallChains construites
        /// par le service.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Query Handler générique auquel est déléguée la lecture suivie de la barre désignée.
        /// </summary>
        private readonly IQ_Generic<ProductionBar> _queryHandler;

        /// <summary>
        /// Command Handler générique auquel est déléguée la mise à jour de la barre scellée.
        /// </summary>
        private readonly IC_Generic<ProductionBar> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionBar_UpdateOptimization"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la barre lue reste ainsi suivie par le contexte qui
        /// l'enregistrera.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la mise à jour de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionBar_UpdateOptimization(
            IQ_Generic<ProductionBar> queryHandler,
            IC_Generic<ProductionBar> commandHandler,
            IS_ExClassifier classifier)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Inscrit sur la barre de production désignée le plan de coupe recomposé par le moteur
        /// d'optimisation, scelle la barre, puis confie sa mise à jour au Command Handler
        /// générique sans la persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de la validation de barre, sur le chemin
        /// avec défauts, à l'intérieur de la transaction qu'il a ouverte. La barre est lue avec suivi
        /// des changements ; la même instance est contrôlée, modifiée puis transmise au Command
        /// Handler générique, qui positionne la date de mise à jour et inscrit l'événement associé.
        /// L'enregistrement effectif n'intervient qu'à la validation de la transaction par
        /// l'appelant ; un échec survenant avant la modification laisse la barre intacte.
        /// </para>
        /// <para>
        /// Objectif : établir définitivement le plan de coupe de la barre sur la matière réellement
        /// exploitable, en une écriture indissociable du plan recomposé et du scellement.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier, dans l'ordre : identifiant de barre strictement positif, présence du résultat, issue de succès, présence puis non-vacuité de la liste des découpes.</description></item>
        /// <item><description>Charger la barre désignée en lecture suivie, puis vérifier qu'elle a été trouvée.</description></item>
        /// <item><description>Vérifier l'état de la barre et rejeter en un échec unique l'ensemble des conditions violées, dans l'ordre : supprimée logiquement, porteuse d'un motif de refus, non validée, déjà scellée, épuisée, en rupture de stock.</description></item>
        /// <item><description>Vérifier la cohérence du résultat avec la barre et rejeter en un échec unique l'ensemble des écarts, dans l'ordre : origine du contenant, chute source, longueur de barre.</description></item>
        /// <item><description>Positionner le nombre de découpes au cardinal de la liste reçue, la longueur du reste à la valeur reçue (y compris zéro), la qualification du reste à la valeur reçue sans transformation, et l'indicateur de scellement à <see langword="true"/>.</description></item>
        /// <item><description>Transmettre au Command Handler générique l'instance chargée et modifiée.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ de la barre ; le marqueur de placement provisoire, l'état de validation et les zones défectueuses restent inchangés.</description></item>
        /// <item><description>Ne contrôle ni ne recalcule la longueur ni la qualification du reste.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne relit pas l'entité après écriture.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production à mettre à jour et à sceller. Doit être strictement positif.</param>
        /// <param name="result">
        /// Résultat de la recomposition du plan de coupe par le moteur d'optimisation. Ne doit pas
        /// être <see langword="null"/> ; son issue doit être
        /// <see cref="En_CuttingOptimizationOutcome.Success"/>, sa liste d'identifiants de découpes
        /// doit être renseignée et non vide, et son contenant doit être celui de la barre désignée.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant l'opération asynchrone, sans valeur de retour.</returns>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="result"/> ou sa liste
        /// d'identifiants de découpes est <see langword="null"/> ; avec le code <c>BU_ER_02</c> si
        /// <paramref name="idProductionBar"/> n'est pas strictement positif (valeur reçue citée) ou
        /// si la liste des découpes est vide ; avec le code <c>BU_ER_03</c> si l'issue du résultat
        /// n'est pas un succès (valeur reçue citée), si la barre désignée est introuvable
        /// (identifiant cité), ou si le contenant du résultat diffère de la barre (barre et chaque
        /// écart cités, en un échec unique) ; avec le code <c>BU_ER_04</c>, en un échec unique, si
        /// l'état de la barre ne permet pas son scellement (barre et chaque condition violée
        /// citées). Remonte également sans interception toute <see cref="Ex_Business"/> levée par
        /// le Query Handler ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la barre ou la délégation de sa mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionBar,
            DTO_CuttingOptimizationResult result,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P1 - Identifiant de barre strictement positif.
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre de production (idProductionBar) doit être strictement positif ; valeur reçue : {idProductionBar}.");

                // P2 - Résultat d'optimisation obligatoire.
                if (result is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le résultat d'optimisation (result) est obligatoire pour l'inscription du plan recomposé d'une barre de production.");

                // P3 - Seule une issue de succès décrit un plan recomposé à inscrire.
                if (result.Outcome != En_CuttingOptimizationOutcome.Success)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"L'issue du résultat d'optimisation (result.Outcome) doit être Success ; valeur reçue : {result.Outcome}.");

                // P4 - Liste des découpes obligatoire.
                if (result.PieceIds is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "La liste des identifiants de découpes (result.PieceIds) est obligatoire.");

                // P5 - Un plan recomposé porte toujours au moins une découpe.
                if (result.PieceIds.Count == 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        "La liste des identifiants de découpes (result.PieceIds) doit contenir au moins un élément.");

                ct.ThrowIfCancellationRequested();

                // L - Lecture SUIVIE : l'instance chargée est celle que le contexte partagé
                // enregistrera. Aucune variante AsNoTracking.
                ProductionBar? bar = await _queryHandler.HandleGetByIdAsync(
                    callChain,
                    idProductionBar,
                    ct);

                // C1 - La barre désignée doit exister.
                if (bar is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La barre de production désignée est introuvable, signe d'une incohérence entre l'orchestration de la validation et l'état persistant ; identifiant introuvable : {idProductionBar}.");

                // C2 - État compatible avec le scellement, toutes les conditions violées étant
                // citées en un échec unique ; garantit l'ordre des étapes de la validation.
                string? stateViolation = DescribeStateViolation(bar);
                if (stateViolation is not null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état de la barre de production {idProductionBar} ne permet ni l'inscription du plan recomposé ni son scellement : {stateViolation}.");

                // C3 - Le résultat doit avoir été calculé pour cette barre, tous les écarts étant
                // cités en un échec unique.
                string? resultMismatch = DescribeResultMismatch(result, bar);
                if (resultMismatch is not null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Le résultat d'optimisation reçu ne correspond pas à la barre de production {idProductionBar} : {resultMismatch}.");

                // M - Écriture indissociable du plan recomposé et du scellement, sur l'instance
                // chargée et sur elle seule. Aucun recalcul.
                bar.CutPieceCount = result.PieceIds.Count;
                bar.ResidueLength = result.ResidueLength;

                // Polarité : true = chute réutilisable, false = déchet. Recopie sans transformation.
                bar.ResidueIsScrap = result.ResidueIsScrap;

                // Scellement : seul indicateur d'état positionné ; IsOptimizedTemp reste inchangé.
                bar.IsOptimized = true;

                // D - Délégation de la même instance que celle chargée ; UpdatedAt et événement
                // Event Store relèvent du Command Handler générique.
                await _commandHandler.HandleUpdateAsync(callChain, bar, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Décrit les conditions d'état qui interdisent l'inscription du plan recomposé et le
        /// scellement d'une barre de production, ou indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée sur la barre chargée, avant toute modification. Une barre est
        /// scellable si elle n'est pas supprimée logiquement, ne porte aucun motif de refus, a été
        /// validée, n'est pas encore scellée, n'est pas épuisée et n'est pas en rupture de stock.
        /// Un motif de refus nul, vide ou composé uniquement de blancs est tenu pour absent. Toutes
        /// les conditions violées sont citées ensemble, dans cet ordre, afin que l'échec renseigne
        /// complètement l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="bar">Barre de production chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Énumération des conditions violées, prête à être citée dans le message d'échec ;
        /// <see langword="null"/> si l'état de la barre permet son scellement.
        /// </returns>
        private static string? DescribeStateViolation(ProductionBar bar)
        {
            List<string> conditions = new();

            if (bar.IsDeleted)
                conditions.Add("supprimée logiquement");

            if (!string.IsNullOrWhiteSpace(bar.RejectionReason))
                conditions.Add($"porteuse d'un motif de refus (motif existant : « {bar.RejectionReason} »)");

            if (!bar.IsValidated)
                conditions.Add("non validée");

            if (bar.IsOptimized)
                conditions.Add("déjà scellée");

            if (bar.IsUsed)
                conditions.Add("épuisée");

            if (bar.IsOutOfStock)
                conditions.Add("en rupture de stock");

            return conditions.Count == 0
                ? null
                : string.Join(", ", conditions);
        }

        /// <summary>
        /// Décrit les écarts entre le contenant porté par un résultat d'optimisation et la barre de
        /// production à laquelle il est destiné, ou indique qu'aucun ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée sur le résultat reçu et la barre chargée, avant toute modification. Le
        /// résultat d'une recomposition recopie le contenant de la barre pour laquelle il a été
        /// calculé ; un écart sur l'origine du contenant, la chute source ou la longueur de barre
        /// signale qu'un résultat destiné à une autre barre a été transmis. Tous les écarts sont
        /// cités ensemble, dans cet ordre, avec la valeur du résultat et celle de la barre.
        /// </para>
        /// </remarks>
        /// <param name="result">Résultat d'optimisation reçu. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="bar">Barre de production chargée. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Énumération des écarts constatés, prête à être citée dans le message d'échec ;
        /// <see langword="null"/> si le résultat est cohérent avec la barre.
        /// </returns>
        private static string? DescribeResultMismatch(DTO_CuttingOptimizationResult result, ProductionBar bar)
        {
            List<string> mismatches = new();

            if (result.IsNewBar != bar.IsNewBar)
                mismatches.Add($"origine du contenant divergente (result.IsNewBar = {result.IsNewBar}, barre : {bar.IsNewBar})");

            if (result.IdSourceScrap != bar.IdSourceScrap)
                mismatches.Add($"chute source divergente (result.IdSourceScrap = {FormatNullable(result.IdSourceScrap)}, barre : {FormatNullable(bar.IdSourceScrap)})");

            if (result.BarLength != bar.BarLength)
                mismatches.Add($"longueur de barre divergente (result.BarLength = {result.BarLength}, barre : {bar.BarLength})");

            return mismatches.Count == 0
                ? null
                : string.Join(", ", mismatches);
        }

        /// <summary>
        /// Restitue la représentation textuelle d'un identifiant facultatif, la valeur absente étant
        /// rendue par le littéral <c>null</c>.
        /// </summary>
        /// <param name="value">Identifiant facultatif à restituer.</param>
        /// <returns>Représentation textuelle de <paramref name="value"/>, ou <c>null</c> si elle est absente.</returns>
        private static string FormatNullable(int? value)
            => value?.ToString() ?? "null";

        #endregion
    }
}