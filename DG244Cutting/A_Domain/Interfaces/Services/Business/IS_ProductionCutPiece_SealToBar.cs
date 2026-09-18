using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de scellement, sur une barre de production validée sans défaut,
    /// du placement provisoire de l'ensemble des découpes qui lui sont rattachées.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la validation de
    /// barre, sur le chemin sans défaut, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionCutPiece_SealToBar"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'application approvisionne chaque barre juste avant sa coupe. L'optimisation a
    /// placé provisoirement des découpes sur la barre, en leur attribuant une position de coupe ;
    /// tant que ce placement reste provisoire, un refus de la barre renverrait ces découpes au
    /// vivier des pièces à optimiser. Lorsque l'opérateur valide la barre sans défaut, ce
    /// placement doit devenir définitif : le plan de coupe calculé est celui que l'opérateur
    /// réalisera, à l'identique, et la matière de chaque découpe est déclarée approvisionnée et
    /// acceptée. Le contrat exprime ce passage du provisoire au définitif, en une seule opération
    /// portant sur toutes les découpes de la barre.
    /// </para>
    /// <para>
    /// Le scellement se distingue du rattachement provisoire, qui place les découpes sur la barre
    /// sans les engager : il n'attribue ni ne modifie aucun rattachement ni aucune position de
    /// coupe, et se borne à consacrer le plan existant. Les découpes concernées sont sélectionnées
    /// par leur rattachement à la barre et non par une liste fournie par l'appelant : toutes les
    /// découpes rattachées sont scellées, sans exception, de sorte qu'aucune pièce du plan ne peut
    /// être omise. Une découpe scellée quitte durablement le vivier des pièces à optimiser, alors
    /// que la barre, elle, conserve son marqueur de placement provisoire toute sa vie.
    /// </para>
    /// <para>
    /// Le contrat ne concerne que le chemin sans défaut de la validation : lorsque des zones
    /// défectueuses sont signalées, le plan de coupe est défait puis recomposé, et les découpes du
    /// nouveau plan sont scellées par un service distinct
    /// (<c>IS_ProductionCutPiece_AssignAndSealToBar</c>).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de scellement de toutes les découpes rattachées à une barre de production désignée par son identifiant.</description></item>
    /// <item><description>Garantir que le plan de coupe reste intact : ni le rattachement ni la position de coupe d'aucune découpe ne sont modifiés.</description></item>
    /// <item><description>Garantir un traitement intégral : toutes les découpes rattachées sont scellées ou aucune ne l'est.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne vérifie ni l'existence ni l'état de la barre : sa validation, préalable au scellement, relève d'une autre action orchestrée par le UseCase.</description></item>
    /// <item><description>Ne choisit aucune découpe et ne recalcule aucun ordre de coupe : le plan est repris tel que l'optimisation l'a posé.</description></item>
    /// <item><description>Ne traite pas le chemin avec défauts : le démontage et la recomposition du plan relèvent d'autres actions du cycle de vie.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : la validation de la barre est tracée par le UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="IS_ProductionCutPiece_AssignToBar"/>
    /// <seealso cref="IS_ProductionBar_Validate"/>
    public interface IS_ProductionCutPiece_SealToBar
    {
        // --- Groupe 1 : Scellement des découpes d'une barre de production ---

        /// <summary>
        /// Rend définitif le placement provisoire de toutes les découpes rattachées à la barre de
        /// production désignée, en les déclarant scellées et approvisionnées, puis confie leur mise
        /// à jour à l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de la validation de barre, sur le chemin
        /// sans défaut, à l'intérieur de la transaction qu'il a ouverte et après la validation de
        /// la barre. Les découpes sont chargées en une lecture unique avec suivi des changements,
        /// sélectionnées par leur seul rattachement à la barre, puis leur mise à jour est déléguée
        /// en une seule opération au Command Handler générique
        /// <c>IC_Generic&lt;ProductionCutPiece&gt;</c>, qui positionne la date de mise à jour et
        /// inscrit un événement par découpe ; l'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Une découpe n'est scellable que si elle est placée provisoirement, non encore scellée,
        /// non réalisée, non supprimée logiquement, non en rupture de stock de barre, et pourvue
        /// d'une position de coupe. L'opération n'est pas idempotente : un second appel sur une
        /// barre déjà scellée échoue, ses découpes n'étant plus placées provisoirement et étant
        /// déjà scellées.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif.</description></item>
        /// <item><description>Vérifier qu'au moins une découpe est rattachée à la barre.</description></item>
        /// <item><description>Vérifier que l'état de chaque découpe rattachée permet son scellement, et rejeter en un échec unique l'ensemble des découpes qui ne le permettent pas.</description></item>
        /// <item><description>Sur chaque découpe, lever le marqueur de placement provisoire et positionner les marqueurs de placement définitif et d'approvisionnement de la barre ; aucun autre champ n'est modifié.</description></item>
        /// <item><description>Déléguer la mise à jour de l'ensemble des découpes au Command Handler générique, en une seule opération.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni le rattachement, ni la position de coupe, ni les marqueurs de refus, de réalisation, de rupture de stock ou de suppression logique, ni les horodatages de coupe, ni les champs d'audit.</description></item>
        /// <item><description>Ne marque aucune découpe tant que tous les contrôles ne sont pas passés.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie ni l'existence ni l'état de la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production validée dont les découpes rattachées sont à
        /// sceller. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si aucune découpe n'est rattachée à la
        /// barre désignée ; avec le code <c>BU_ER_04</c>, en un échec unique citant chaque découpe
        /// concernée et chaque condition violée, si une ou plusieurs découpes rattachées ne sont pas
        /// placées provisoirement, sont déjà scellées, déjà réalisées, supprimées logiquement, en
        /// rupture de stock de barre ou dépourvues de position de coupe.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture des découpes ou de la délégation de leur mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionBar,
            CancellationToken ct = default);
    }
}