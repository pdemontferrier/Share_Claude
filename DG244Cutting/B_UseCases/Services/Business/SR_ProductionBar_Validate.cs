using System.Globalization;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de l'inscription, sur une barre de production, de son acceptation
    /// physique par l'opérateur, avec ou sans signalement de zones défectueuses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionBar_Validate"/>, par le UseCase orchestrateur de la validation de
    /// barre, UC_BarValidation. Il consomme directement <see cref="IQ_Generic{T}"/> pour la
    /// lecture préalable de la barre et <see cref="IC_Generic{T}"/> pour sa mise à jour, sur
    /// l'entité <see cref="ProductionBar"/>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier approvisionne chaque barre juste avant de la couper. L'opérateur prend
    /// la barre que l'application lui désigne, chute du stock ou barre neuve, en constate l'état
    /// puis l'accepte ; ce geste engage la matière. Le service inscrit cette acceptation sur la
    /// barre et délègue ensuite la mise à jour au Command Handler générique, sans exposer la
    /// logique de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Le geste prend deux formes, gouvernées par un discriminant unique : la présence d'une
    /// première zone défectueuse. Sans défaut, le placement provisoire calculé par l'optimisation
    /// devient définitif à l'identique : la barre est validée et scellée en une seule écriture.
    /// Avec défauts, l'opérateur signale une ou deux zones, repérées en millimètres depuis le début
    /// de la barre ; la barre est validée et ses défauts enregistrés, mais elle n'est pas scellée,
    /// car le plan provisoire n'est plus valable. Elle devient la barre de référence de la
    /// ré-optimisation, et son scellement relève du service qui inscrira le plan recomposé.
    /// </para>
    /// <para>
    /// Les zones défectueuses obéissent aux mêmes contraintes que celles appliquées par le moteur
    /// d'optimisation lors de la recomposition : bornes comprises entre zéro et la longueur de la
    /// barre, ordonnées selon début 1 &lt; fin 1 &lt;= début 2 &lt; fin 2. Deux zones contiguës
    /// sont admises, et un défaut peut commencer en tête de barre. Le marqueur de placement
    /// provisoire <c>IsOptimizedTemp</c> reste positionné pour toute la vie de la barre ; il n'est
    /// ni lu ni écrit par le service.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier les préconditions structurelles des arguments, y compris la complétude et l'ordonnancement des zones défectueuses.</description></item>
    /// <item><description>Charger la barre désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence de la barre et la compatibilité de son état avec la validation.</description></item>
    /// <item><description>Sur le chemin avec défauts, vérifier que les bornes signalées n'excèdent pas la longueur de la barre.</description></item>
    /// <item><description>Positionner l'indicateur de validation, puis l'indicateur de scellement sans défaut ou les bornes des zones défectueuses avec défauts.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne recompose pas le plan de coupe d'une barre à défauts et ne la scelle pas.</description></item>
    /// <item><description>Ne modifie aucun autre champ que l'indicateur de validation, l'indicateur de scellement et les bornes des zones défectueuses ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c>, n'appelle aucun autre service métier et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : cette inscription relève d'un service dédié appelé par le UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionBar_Validate"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionBar_Validate : IS_ProductionBar_Validate
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
        /// Command Handler générique auquel est déléguée la mise à jour de la barre validée.
        /// </summary>
        private readonly IC_Generic<ProductionBar> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionBar_Validate"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la barre lue reste ainsi suivie par le contexte qui
        /// l'enregistrera, et une seconde lecture par le UseCase restitue la même instance.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la mise à jour de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionBar_Validate(
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
        /// Inscrit sur la barre de production désignée l'acceptation physique de l'opérateur, avec
        /// ou sans signalement de zones défectueuses, puis confie sa mise à jour au Command Handler
        /// générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte. La barre est lue avec suivi des changements ; la même instance est contrôlée,
        /// modifiée puis transmise au Command Handler générique, qui positionne la date de mise à
        /// jour et inscrit un événement technique. L'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant ; un échec survenant avant la modification
        /// laisse la barre intacte.
        /// </para>
        /// <para>
        /// Objectif : faire passer la barre à l'état validé. La présence de la première zone
        /// défectueuse est le discriminant unique : en son absence, le plan provisoire est scellé à
        /// l'identique ; en sa présence, les zones signalées sont enregistrées et la barre reste non
        /// scellée en attente de recomposition.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier en séquence, la première violation interrompant l'exécution : identifiant de barre strictement positif ; première zone complète ; seconde zone complète ; seconde zone accompagnée de la première ; bornes non négatives ; première zone ordonnée ; seconde zone ordonnée et non antérieure à la fin de la première.</description></item>
        /// <item><description>Charger la barre désignée en lecture suivie, puis vérifier qu'elle a été trouvée.</description></item>
        /// <item><description>Vérifier l'état de la barre et rejeter en un échec unique l'ensemble des conditions violées, dans l'ordre : déjà validée, épuisée, en rupture de stock, refusée ou supprimée logiquement, le motif de refus étant cité lorsqu'il est renseigné.</description></item>
        /// <item><description>Sur le chemin avec défauts, vérifier que chaque borne renseignée n'excède pas la longueur de la barre.</description></item>
        /// <item><description>Positionner <c>IsValidated</c> ; sans défaut, positionner <c>IsOptimized</c> ; avec défauts, inscrire les quatre bornes telles que reçues, la seconde zone pouvant rester vide.</description></item>
        /// <item><description>Transmettre au Command Handler générique l'instance chargée et modifiée.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Avec défauts, laisse <c>IsOptimized</c> inchangé ; sans défaut, laisse les bornes de zones défectueuses inchangées.</description></item>
        /// <item><description>Ne lit ni n'écrit <c>IsOptimizedTemp</c> et ne contrôle pas <c>IsOptimized</c>.</description></item>
        /// <item><description>Ne modifie ni les champs de découpe et de reliquat, ni les indicateurs d'utilisation, de rupture de stock et de suppression logique, ni le motif de refus, ni les rattachements, la longueur, l'origine et la date de création de la barre.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production acceptée par l'opérateur. Doit être strictement positif.</param>
        /// <param name="defectStart1">Début de la première zone défectueuse, en millimètres depuis le début de la barre, ou <see langword="null"/> en l'absence de défaut ; sa présence discrimine les deux chemins.</param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, en millimètres ; renseignée conjointement avec <paramref name="defectStart1"/>.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, en millimètres ; facultatif, renseigné conjointement avec <paramref name="defectEnd2"/> et seulement si la première zone l'est.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, en millimètres ; renseignée conjointement avec <paramref name="defectStart2"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif (paramètre nommé et valeur reçue cités), si une borne renseignée est
        /// négative (deux zones citées) ou, sur le chemin avec défauts, si une borne renseignée
        /// excède la longueur de la barre (barre, longueur, zones et bornes fautives citées) ; avec
        /// le code <c>BU_ER_03</c> si une zone est incomplète ou si la seconde est renseignée sans la
        /// première (bornes reçues citées), si la première zone est désordonnée (zone citée), si la
        /// seconde zone est désordonnée ou antérieure à la fin de la première (deux zones citées),
        /// ou si la barre désignée est introuvable (identifiant cité) ; avec le code
        /// <c>BU_ER_04</c>, en un échec unique, si la barre est déjà validée, épuisée, en rupture de
        /// stock, ou refusée ou supprimée logiquement (barre et chaque condition violée citées).
        /// Remonte également sans interception toute <see cref="Ex_Business"/> levée par le Query
        /// Handler ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la barre ou la délégation de sa mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionBar,
            int? defectStart1,
            int? defectEnd1,
            int? defectStart2,
            int? defectEnd2,
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

                // P2 - Première zone complète ou absente.
                if (defectStart1.HasValue != defectEnd1.HasValue)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La première zone défectueuse est incomplète : une seule de ses deux bornes est renseignée (début : {FormatBound(defectStart1)} ; fin : {FormatBound(defectEnd1)}).");

                // P3 - Seconde zone complète ou absente.
                if (defectStart2.HasValue != defectEnd2.HasValue)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La seconde zone défectueuse est incomplète : une seule de ses deux bornes est renseignée (début : {FormatBound(defectStart2)} ; fin : {FormatBound(defectEnd2)}).");

                // P4 - Seconde zone admise uniquement en présence de la première.
                if (defectStart2.HasValue && !defectStart1.HasValue)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La seconde zone défectueuse est renseignée sans première zone ({DescribeZones(defectStart1, defectEnd1, defectStart2, defectEnd2)}).");

                // P5 - Bornes renseignées positives ou nulles. Une comparaison relevée sur une
                // borne absente vaut false : seules les bornes renseignées sont contrôlées.
                if (defectStart1 < 0 || defectEnd1 < 0 || defectStart2 < 0 || defectEnd2 < 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"Une borne de zone défectueuse est négative ({DescribeZones(defectStart1, defectEnd1, defectStart2, defectEnd2)}).");

                // P6 - Première zone ordonnée. Après P2, les deux bornes sont présentes ou absentes
                // ensemble ; la comparaison relevée vaut false en l'absence de zone.
                if (defectStart1 >= defectEnd1)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Les bornes de la première zone défectueuse sont désordonnées ({FormatBound(defectStart1)} >= {FormatBound(defectEnd1)} mm).");

                // P7 - Seconde zone ordonnée et non antérieure à la fin de la première ; deux zones
                // contiguës sont admises. Après P3 et P4, une seconde zone présente est complète et
                // accompagnée d'une première zone complète.
                if (defectStart2.HasValue
                    && (defectStart2 >= defectEnd2 || defectStart2 < defectEnd1))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La seconde zone défectueuse est désordonnée ou chevauche la première ({DescribeZones(defectStart1, defectEnd1, defectStart2, defectEnd2)}).");

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
                        $"La barre de production désignée est introuvable ; identifiant introuvable : {idProductionBar}.");

                // C2 - État compatible avec la validation, toutes les conditions violées étant
                // citées en un échec unique.
                string? violation = DescribeStateViolation(bar);
                if (violation is not null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état de la barre de production {bar.Id} ne permet pas sa validation : {violation}.");

                // Discriminant unique entre les deux chemins.
                bool hasDefects = defectStart1.HasValue;

                // C3 - Chemin avec défauts : bornes comprises dans la longueur de la barre.
                if (hasDefects)
                {
                    string? beyondLength = DescribeBoundsBeyondLength(
                        bar.BarLength,
                        defectStart1,
                        defectEnd1,
                        defectStart2,
                        defectEnd2);

                    if (beyondLength is not null)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_02,
                            $"Une borne de zone défectueuse excède la longueur de la barre de production {bar.Id} ({bar.BarLength} mm) : {beyondLength} ({DescribeZones(defectStart1, defectEnd1, defectStart2, defectEnd2)}).");
                }

                // M - Modification de l'instance suivie. IsOptimizedTemp n'est jamais touché.
                bar.IsValidated = true;

                if (hasDefects)
                {
                    // Barre de référence de la ré-optimisation : défauts enregistrés, IsOptimized
                    // inchangé.
                    bar.DefectStart1 = defectStart1;
                    bar.DefectEnd1 = defectEnd1;
                    bar.DefectStart2 = defectStart2;
                    bar.DefectEnd2 = defectEnd2;
                }
                else
                {
                    // Plan provisoire scellé à l'identique.
                    bar.IsOptimized = true;
                }

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
        /// Décrit les conditions d'état qui interdisent la validation d'une barre de production, ou
        /// indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée sur la barre chargée, avant toute modification. Une barre est
        /// validable si elle n'est ni déjà validée, ni épuisée, ni en rupture de stock, ni refusée
        /// ou supprimée logiquement. Toutes les conditions violées sont citées ensemble, dans cet
        /// ordre, afin que l'échec renseigne complètement l'appelant ; le motif de refus est cité
        /// lorsqu'il est renseigné. Ni l'indicateur de scellement ni le marqueur de placement
        /// provisoire ne sont contrôlés.
        /// </para>
        /// </remarks>
        /// <param name="bar">Barre chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Énumération des conditions violées, prête à être citée dans le message d'échec ;
        /// <see langword="null"/> si l'état de la barre permet sa validation.
        /// </returns>
        private static string? DescribeStateViolation(ProductionBar bar)
        {
            List<string> conditions = new();

            if (bar.IsValidated)
                conditions.Add("déjà validée");

            if (bar.IsUsed)
                conditions.Add("épuisée");

            if (bar.IsOutOfStock)
                conditions.Add("en rupture de stock");

            if (bar.IsDeleted)
                conditions.Add(string.IsNullOrEmpty(bar.RejectionReason)
                    ? "refusée ou supprimée logiquement"
                    : $"refusée ou supprimée logiquement (motif : « {bar.RejectionReason} »)");

            return conditions.Count == 0
                ? null
                : string.Join(", ", conditions);
        }

        /// <summary>
        /// Décrit les bornes de zones défectueuses renseignées qui excèdent la longueur de la barre,
        /// ou indique qu'aucune ne l'excède.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée sur le chemin avec défauts, après chargement de la barre, les bornes
        /// ayant déjà été reconnues complètes, positives ou nulles et ordonnées. Une borne égale à la
        /// longueur de la barre est admise ; une borne absente n'est pas contrôlée.
        /// </para>
        /// </remarks>
        /// <param name="barLength">Longueur de la barre chargée, en millimètres.</param>
        /// <param name="defectStart1">Début de la première zone défectueuse, en millimètres.</param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, en millimètres.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, en millimètres, ou <see langword="null"/>.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, en millimètres, ou <see langword="null"/>.</param>
        /// <returns>
        /// Énumération des bornes fautives avec leur valeur, prête à être citée dans le message
        /// d'échec ; <see langword="null"/> si toutes les bornes renseignées sont comprises dans la
        /// longueur de la barre.
        /// </returns>
        private static string? DescribeBoundsBeyondLength(
            int barLength,
            int? defectStart1,
            int? defectEnd1,
            int? defectStart2,
            int? defectEnd2)
        {
            List<string> bounds = new();

            if (defectStart1 > barLength)
                bounds.Add($"début de zone 1 ({FormatBound(defectStart1)} mm)");

            if (defectEnd1 > barLength)
                bounds.Add($"fin de zone 1 ({FormatBound(defectEnd1)} mm)");

            if (defectStart2 > barLength)
                bounds.Add($"début de zone 2 ({FormatBound(defectStart2)} mm)");

            if (defectEnd2 > barLength)
                bounds.Add($"fin de zone 2 ({FormatBound(defectEnd2)} mm)");

            return bounds.Count == 0
                ? null
                : string.Join(", ", bounds);
        }

        /// <summary>
        /// Restitue les bornes des deux zones défectueuses reçues sous une forme lisible, destinée
        /// aux messages d'échec.
        /// </summary>
        /// <param name="defectStart1">Début de la première zone défectueuse, ou <see langword="null"/>.</param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, ou <see langword="null"/>.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, ou <see langword="null"/>.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, ou <see langword="null"/>.</param>
        /// <returns>Description des deux zones, une borne absente étant représentée par un tiret.</returns>
        private static string DescribeZones(
            int? defectStart1,
            int? defectEnd1,
            int? defectStart2,
            int? defectEnd2)
        {
            return $"zone 1 : {FormatBound(defectStart1)}-{FormatBound(defectEnd1)} mm ; "
                + $"zone 2 : {FormatBound(defectStart2)}-{FormatBound(defectEnd2)} mm";
        }

        /// <summary>
        /// Restitue une borne de zone défectueuse sous forme textuelle indépendante de la culture.
        /// </summary>
        /// <param name="bound">Borne à restituer, ou <see langword="null"/>.</param>
        /// <returns>Valeur de la borne, ou un tiret si elle est absente.</returns>
        private static string FormatBound(int? bound)
        {
            return bound?.ToString(CultureInfo.InvariantCulture) ?? "-";
        }

        #endregion
    }
}