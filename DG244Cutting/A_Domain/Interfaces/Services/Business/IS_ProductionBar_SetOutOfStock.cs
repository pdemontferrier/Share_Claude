using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de déclaration et de libération de la rupture de stock d'une
    /// barre de production neuve.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la rupture de stock
    /// de barre, UC_ProductionBar_SetOutOfStock, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionBar_SetOutOfStock"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier approvisionne chaque barre juste avant de la couper. Il arrive que la
    /// barre neuve désignée par l'application soit absente de l'atelier, faute de livraison du
    /// fournisseur. À la différence du refus, qui écarte une matière défectueuse ou introuvable,
    /// la rupture de stock met la barre en attente : la matière n'est pas condamnée, elle est
    /// seulement indisponible. La rupture ne concerne que les barres neuves ; une chute
    /// introuvable relève du refus.
    /// </para>
    /// <para>
    /// Le contrat exprime ce basculement dans les deux sens, gouvernés par la valeur à écrire dans
    /// l'indicateur de rupture de stock. La déclaration pose l'indicateur : la barre cesse d'être
    /// considérée comme en cours de traitement et une autre barre ou une autre référence peut être
    /// proposée, tandis que les découpes qu'elle porte lui restent rattachées. La libération retire
    /// l'indicateur lorsque la matière est redevenue disponible : les découpes retrouvent aussitôt
    /// leur place dans le plan de coupe et la barre redevient éligible aux recherches. Les deux
    /// sens partagent l'ensemble de leurs contrôles, à l'exception de celui qui porte sur l'état
    /// courant de l'indicateur. Le plan de coupe de la barre est conservé intact dans les deux cas,
    /// et le marqueur de placement provisoire <c>IsOptimizedTemp</c> n'est ni lu ni écrit.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de pose ou de retrait de l'indicateur de rupture de stock d'une barre de production désignée.</description></item>
    /// <item><description>Garantir que seule une barre neuve, ni validée, ni épuisée, ni refusée ou supprimée logiquement, et dont l'indicateur de rupture diffère de la valeur demandée, peut basculer.</description></item>
    /// <item><description>Garantir que seul l'indicateur de rupture de stock est modifié, le plan de coupe de la barre étant préservé.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne marque ni ne libère les découpes rattachées à la barre et ne recalcule pas l'indicateur de rupture de la série : ces rôles relèvent de services dédiés appelés par le UseCase.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie : la traçabilité de la rupture relève d'un service dédié appelé par le UseCase.</description></item>
    /// <item><description>Ne refuse pas la barre et ne propose pas de barre de remplacement.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.ProductionBar"/>
    public interface IS_ProductionBar_SetOutOfStock
    {
        // --- Groupe 1 : Rupture de stock d'une barre de production ---

        /// <summary>
        /// Pose ou retire l'indicateur de rupture de stock de la barre de production neuve
        /// désignée, puis confie sa mise à jour à l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte. La barre est lue avec suivi des changements, puis sa mise à jour est déléguée au
        /// Command Handler générique <c>IC_Generic&lt;ProductionBar&gt;</c>, qui positionne la date
        /// de mise à jour et inscrit un événement technique ; la barre reste suivie dans le contexte
        /// partagé et n'est enregistrée qu'à la validation de la transaction par l'appelant. Un
        /// échec survenant avant la modification laisse la barre intacte.
        /// </para>
        /// <para>
        /// Objectif : mettre en attente une barre neuve dont la matière est absente de l'atelier
        /// (déclaration), ou la rendre de nouveau éligible lorsque la matière est redevenue
        /// disponible (libération), sans toucher à son plan de coupe.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier la précondition structurelle de l'argument : identifiant de barre strictement positif.</description></item>
        /// <item><description>Vérifier que la barre désignée existe et que son état permet l'opération demandée : ni déjà validée, ni épuisée, indicateur de rupture différent de la valeur demandée, ni refusée ou supprimée logiquement, et barre neuve.</description></item>
        /// <item><description>Écrire dans l'indicateur de rupture de stock la valeur reçue.</description></item>
        /// <item><description>Déléguer la mise à jour de la barre au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni le marqueur de placement provisoire, ni les indicateurs de validation et de scellement, ni le nombre de découpes et les champs de reliquat, ni les indicateurs d'utilisation et de suppression logique, ni le motif de refus, ni les bornes de zones défectueuses, ni la chute source, les rattachements, la longueur, l'origine et la date de création de la barre.</description></item>
        /// <item><description>Ne contrôle ni l'indicateur de scellement ni le marqueur de placement provisoire.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">Identifiant de la barre de production neuve concernée. Doit être strictement positif.</param>
        /// <param name="isOutOfStock">
        /// Valeur à écrire dans l'indicateur de rupture de stock : <see langword="true"/> pour la
        /// déclaration de rupture, <see langword="false"/> pour sa libération.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si la barre désignée est
        /// introuvable ; avec le code <c>BU_ER_04</c>, en un échec unique citant chaque condition
        /// violée, si la barre est déjà validée, épuisée, déjà en rupture de stock lors d'une
        /// déclaration ou non en rupture de stock lors d'une libération, refusée ou supprimée
        /// logiquement, ou issue d'une chute.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la barre ou de la délégation de sa mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionBar,
            bool isOutOfStock,
            CancellationToken ct = default);
    }
}