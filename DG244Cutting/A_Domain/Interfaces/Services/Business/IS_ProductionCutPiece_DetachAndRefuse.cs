using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de libération, sur une barre de production refusée par
    /// l'opérateur, de l'ensemble des découpes qui y étaient provisoirement placées, avec
    /// inscription sur chacune de la trace du refus de coupe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur du refus de barre, qui
    /// en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionCutPiece_DetachAndRefuse"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'application approvisionne chaque barre juste avant sa coupe. Il arrive que
    /// l'opérateur, allant chercher la barre désignée, la trouve inutilisable - abîmée, déformée,
    /// hors cote, ou chute introuvable à son emplacement - et la refuse ; tout ce que l'optimisation
    /// avait posé sur cette barre doit alors être défait. Le contrat exprime le versant « découpes »
    /// de cette annulation : les découpes provisoirement placées sur la barre perdent leur
    /// rattachement et leur position de coupe, et retournent au vivier des pièces à optimiser, d'où
    /// une optimisation ultérieure pourra les reprendre sur une autre barre.
    /// </para>
    /// <para>
    /// Chaque découpe libérée reçoit le marqueur de refus de coupe, qui conserve la trace du fait
    /// que la matière qui lui était destinée a été écartée. Ce marqueur est une trace et non un
    /// blocage : il n'exclut pas la découpe du vivier, et le rattachement provisoire à une nouvelle
    /// barre le lève. Les découpes libérées ne reçoivent pas le marqueur de placement définitif :
    /// n'ayant jamais été scellées, elles redeviennent matière à optimiser, alors que ce marqueur
    /// les ferait sortir durablement du vivier.
    /// </para>
    /// <para>
    /// Les découpes concernées sont sélectionnées par leur rattachement à la barre et non par une
    /// liste fournie par l'appelant : toutes les découpes rattachées sont libérées, sans exception.
    /// Une barre dont le plan de coupe a été scellé n'est plus refusable, de sorte que toute découpe
    /// déjà scellée est rejetée. Le contrat est le symétrique du scellement : il défait le placement
    /// provisoire posé par le rattachement, là où le scellement le consacre.
    /// </para>
    /// <para>
    /// La libération exprimée ne diffère de celle du service jumeau
    /// <c>IS_ProductionCutPiece_DetachFromBar</c>, qui sert le chemin de validation avec défauts où
    /// le plan de coupe est simplement refait, que par la pose du marqueur de refus de coupe. Les
    /// deux contrats sont délibérément distincts, afin que la sémantique de la libération ne dépende
    /// d'aucune valeur d'appel. L'emploi de l'un pour l'autre ne produirait aucune erreur visible,
    /// mais fausserait l'analyse des refus de coupe.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de libération, sur refus, de toutes les découpes rattachées à une barre de production désignée par son identifiant.</description></item>
    /// <item><description>Garantir l'inscription de la trace du refus de coupe sur chaque découpe libérée, sans exception.</description></item>
    /// <item><description>Garantir le retour des découpes libérées au vivier des pièces à optimiser, sans jamais les déclarer scellées.</description></item>
    /// <item><description>Garantir un traitement intégral : toutes les découpes rattachées sont libérées ou aucune ne l'est.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne vérifie ni l'existence ni l'état de la barre, et n'enregistre pas le motif du refus : la mise à l'écart de la barre relève d'une autre action orchestrée par le UseCase.</description></item>
    /// <item><description>Ne statue pas sur le devenir de la chute dont la barre refusée est éventuellement issue.</description></item>
    /// <item><description>Ne choisit aucune découpe et ne recalcule aucun plan de coupe : la reprise des découpes libérées relève d'une optimisation ultérieure.</description></item>
    /// <item><description>Ne traite pas le chemin de validation avec défauts, servi par le service jumeau.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : le refus est tracé une seule fois, sur la barre, par le UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="IS_ProductionCutPiece_AssignToBar"/>
    /// <seealso cref="IS_ProductionCutPiece_SealToBar"/>
    public interface IS_ProductionCutPiece_DetachAndRefuse
    {
        // --- Groupe 1 : Libération sur refus des découpes d'une barre de production ---

        /// <summary>
        /// Libère toutes les découpes provisoirement placées sur la barre de production refusée
        /// désignée, en inscrivant sur chacune la trace du refus de coupe, puis confie leur mise à
        /// jour à l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur du refus de barre, à l'intérieur de la
        /// transaction qu'il a ouverte et après la mise à l'écart de la barre. Les découpes sont
        /// chargées en une lecture unique avec suivi des changements, sélectionnées par leur seul
        /// rattachement à la barre, puis leur mise à jour est déléguée en une seule opération au
        /// Command Handler générique <c>IC_Generic&lt;ProductionCutPiece&gt;</c>, qui positionne la
        /// date de mise à jour et inscrit un événement par découpe ; l'enregistrement effectif
        /// n'intervient qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Une découpe n'est libérable que si elle est placée provisoirement, non encore scellée,
        /// non réalisée, non supprimée logiquement, non en rupture de stock de barre, et pourvue
        /// d'une position de coupe. L'opération n'est pas idempotente : un second appel sur la même
        /// barre échoue, aucune découpe n'y étant plus rattachée.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif.</description></item>
        /// <item><description>Vérifier qu'au moins une découpe est rattachée à la barre.</description></item>
        /// <item><description>Vérifier que l'état de chaque découpe rattachée permet sa libération, et rejeter en un échec unique l'ensemble des découpes qui ne le permettent pas.</description></item>
        /// <item><description>Sur chaque découpe, positionner le marqueur de refus de coupe, lever le marqueur de placement provisoire, et effacer le rattachement à la barre et la position de coupe ; aucun autre champ n'est modifié.</description></item>
        /// <item><description>Déléguer la mise à jour de l'ensemble des découpes au Command Handler générique, en une seule opération.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni le marqueur de placement définitif, ni les marqueurs d'approvisionnement de la barre, de rupture de stock, de réalisation ou de suppression logique, ni les horodatages de coupe, ni les champs d'audit.</description></item>
        /// <item><description>Ne marque aucune découpe tant que tous les contrôles ne sont pas passés.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie ni l'existence ni l'état de la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production refusée dont les découpes rattachées sont à
        /// libérer. Doit être strictement positif.
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