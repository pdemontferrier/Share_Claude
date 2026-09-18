using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de blocage et de déblocage de l'ensemble des découpes rattachées
    /// à une barre de production, selon que la matière de cette barre est en rupture de stock ou
    /// redevenue disponible.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la rupture de stock
    /// de barre, <c>UC_ProductionBar_SetOutOfStock</c>, qui en délègue l'exécution au service
    /// concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionCutPiece_SetBarOutOfStock"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier approvisionne chaque barre juste avant de la couper. Lorsque la
    /// matière d'une barre neuve n'est pas physiquement présente en atelier, la barre est déclarée
    /// en rupture de stock : elle n'est ni supprimée ni écartée, mais mise en attente jusqu'à ce
    /// que la matière redevienne disponible, moment où elle est libérée. Le contrat exprime le
    /// versant « découpes » de cette mise en attente, dans les deux sens : la déclaration pose le
    /// marqueur de blocage sur les découpes rattachées à la barre, la libération le retire.
    /// </para>
    /// <para>
    /// Ce marqueur compte parmi les critères qui excluent une découpe du vivier des pièces à
    /// optimiser, et maintient cette exclusion indépendamment du marqueur de placement provisoire ;
    /// il constitue également un motif de refus du scellement du plan de coupe. La caractéristique
    /// essentielle de l'opération est ce qu'elle préserve : les découpes restent rattachées à la
    /// barre, avec leur position de coupe, et leur marqueur de placement provisoire n'est pas
    /// modifié. Le plan de coupe demeure intact, simplement gelé ; c'est ce maintien du
    /// rattachement qui permet de retrouver les découpes de la barre et de les restaurer sans
    /// recalcul lorsque la matière revient, à la différence du refus ou du détachement, qui
    /// défont le plan.
    /// </para>
    /// <para>
    /// Les découpes concernées sont sélectionnées par leur seul rattachement à la barre et non par
    /// une liste fournie par l'appelant. Les deux sens partagent l'intégralité de leurs contrôles,
    /// et seules les découpes dont le marqueur diffère de la valeur demandée sont effectivement
    /// mises à jour.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de pose ou de retrait du marqueur de blocage sur toutes les découpes rattachées à une barre de production désignée par son identifiant.</description></item>
    /// <item><description>Garantir que le plan de coupe reste intact : ni le rattachement, ni la position de coupe, ni le marqueur de placement provisoire d'aucune découpe ne sont modifiés.</description></item>
    /// <item><description>Garantir un traitement en bloc : tout échec de contrôle interrompt l'opération avant qu'aucune découpe ne soit marquée.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne contrôle ni l'existence ni l'état de la barre, ni l'état courant du marqueur de blocage des découpes : le contrôle d'état relève du service de rupture de stock de la barre, appelé auparavant dans la même transaction.</description></item>
    /// <item><description>Ne défait pas le plan de coupe et ne recalcule aucun ordre de coupe.</description></item>
    /// <item><description>Ne recalcule pas l'indicateur de rupture de stock de la série : ce rôle relève d'un service dédié appelé par le UseCase.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : la déclaration ou la libération est tracée une seule fois, sur la barre, par le UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="IS_ProductionBar_SetOutOfStock"/>
    /// <seealso cref="IS_ProductionSeries_SetBarOutOfStockFlag"/>
    /// <seealso cref="IS_ProductionCutPiece_SealToBar"/>
    public interface IS_ProductionCutPiece_SetBarOutOfStock
    {
        // --- Groupe 1 : Blocage des découpes d'une barre de production en rupture de stock ---

        /// <summary>
        /// Pose ou retire le marqueur de blocage sur les découpes rattachées à la barre de
        /// production désignée, sans toucher au plan de coupe, puis confie leur mise à jour à
        /// l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de la rupture de stock de barre, à
        /// l'intérieur de la transaction qu'il a ouverte, après le basculement de la barre et avant
        /// le recalcul de l'indicateur de rupture de la série. Les découpes sont chargées en une
        /// lecture unique avec suivi des changements, sélectionnées par leur seul rattachement à la
        /// barre, puis la mise à jour des découpes à modifier est déléguée en une seule opération
        /// au Command Handler générique <c>IC_Generic&lt;ProductionCutPiece&gt;</c>, qui positionne
        /// la date de mise à jour et inscrit un événement par découpe transmise ; les découpes
        /// restent suivies dans le contexte partagé et ne sont enregistrées qu'à la validation de
        /// la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : bloquer les découpes d'une barre dont la matière est absente de l'atelier
        /// (déclaration), ou les débloquer lorsque la matière est redevenue disponible
        /// (libération), en conservant leur place dans le plan de coupe.
        /// </para>
        /// <para>
        /// Une découpe réalisée ou supprimée logiquement interdit l'opération, dans les deux sens et
        /// quelle que soit la valeur courante de son marqueur. Seules les découpes dont le marqueur
        /// diffère de la valeur demandée sont mises à jour ; lorsque toutes portent déjà cette
        /// valeur, l'opération aboutit sans aucune écriture.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif.</description></item>
        /// <item><description>Vérifier qu'au moins une découpe est rattachée à la barre.</description></item>
        /// <item><description>Vérifier qu'aucune découpe rattachée n'est réalisée ni supprimée logiquement, et rejeter en un échec unique l'ensemble des découpes qui ne le permettent pas.</description></item>
        /// <item><description>Écrire la valeur demandée dans le marqueur de blocage des seules découpes dont le marqueur en diffère ; aucun autre champ n'est modifié.</description></item>
        /// <item><description>Déléguer la mise à jour des découpes modifiées au Command Handler générique, en une seule opération.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni le rattachement, ni la position de coupe, ni les marqueurs de placement provisoire et définitif, d'approvisionnement, de refus, de réalisation et de suppression logique, ni les horodatages de coupe, ni les champs d'audit.</description></item>
        /// <item><description>Ne marque aucune découpe tant que tous les contrôles ne sont pas passés.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne contrôle ni l'état de la barre ni l'état courant du marqueur des découpes.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production dont les découpes rattachées sont à bloquer ou à
        /// débloquer. Doit être strictement positif.
        /// </param>
        /// <param name="isBarOutOfStock">
        /// Valeur à écrire dans le marqueur de blocage des découpes : <see langword="true"/> pour la
        /// déclaration de rupture de stock, <see langword="false"/> pour sa libération.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si aucune découpe n'est rattachée à la
        /// barre désignée ; avec le code <c>BU_ER_04</c>, en un échec unique citant la barre, le
        /// sens de l'opération, chaque découpe concernée et chaque condition violée, si une ou
        /// plusieurs découpes rattachées sont déjà réalisées ou supprimées logiquement.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture des découpes ou de la délégation de leur mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionBar,
            bool isBarOutOfStock,
            CancellationToken ct = default);
    }
}