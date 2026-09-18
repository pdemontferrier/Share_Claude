using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de détachement, sur le chemin de validation d'une barre de
    /// production avec défauts, de l'ensemble des découpes rattachées à cette barre, qui
    /// redeviennent matière à optimiser sans être marquées comme refusées.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la validation de
    /// barre (<c>UC_BarValidation</c>), sur le seul chemin de validation avec défauts, après la
    /// validation de la barre et avant la recomposition de son plan de coupe. Ce UseCase en
    /// délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionCutPiece_DetachFromBar"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : une barre de production peut présenter jusqu'à deux zones défectueuses, que
    /// l'opérateur mesure et déclare au moment de la valider. Ces zones consomment de la matière :
    /// le plan de coupe provisoire établi avant leur déclaration n'est plus réalisable. Il doit
    /// être entièrement défait avant que le moteur d'optimisation ne le recompose sur la matière
    /// réellement exploitable. Le contrat exprime ce besoin de démontage : chaque découpe
    /// rattachée à la barre perd son rattachement et sa position de coupe, quitte l'état de
    /// placement provisoire et redevient matière à optimiser. Une partie de ces découpes sera
    /// reprise par la recomposition, souvent aux mêmes positions.
    /// </para>
    /// <para>
    /// Le détachement ne laisse aucune trace de refus : rien n'est arrivé aux découpes, qui
    /// figuraient seulement sur un plan devenu caduc. Il se distingue en cela du détachement
    /// consécutif au refus d'une barre (<c>IS_ProductionCutPiece_DetachAndRefuse</c>), qui libère
    /// les mêmes champs et marque en outre chaque découpe comme refusée. Les deux contrats ne
    /// diffèrent que par ce marqueur et ne sont pas interchangeables : employer l'un pour l'autre
    /// fausserait la traçabilité des refus, ou en inventerait un.
    /// </para>
    /// <para>
    /// Les découpes détachées sont sélectionnées par leur rattachement à la barre et non par une
    /// liste fournie par l'appelant : toutes les découpes rattachées sont détachées, de sorte
    /// qu'aucun reliquat du plan caduc ne subsiste. Le démontage défait le rattachement
    /// provisoire posé par <see cref="IS_ProductionCutPiece_AssignToBar"/>. Le moteur
    /// d'optimisation lisant le vivier des pièces sur l'état persistant, l'enregistrement du
    /// détachement doit précéder la recomposition ; cet enregistrement relève de l'appelant.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de détachement de toutes les découpes rattachées à une barre de production désignée par son identifiant.</description></item>
    /// <item><description>Garantir l'absence de toute trace de refus : le marqueur de refus des découpes n'est jamais modifié.</description></item>
    /// <item><description>Garantir un traitement intégral : toutes les découpes rattachées sont détachées ou aucune ne l'est.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif, préalable à la recomposition du plan, relève du UseCase appelant.</description></item>
    /// <item><description>Ne vérifie ni l'existence ni l'état de la barre : sa validation, préalable au détachement, relève d'une autre action orchestrée par le UseCase.</description></item>
    /// <item><description>Ne recompose pas le plan de coupe et ne rattache aucune découpe : ces opérations relèvent du moteur d'optimisation et d'autres actions du cycle de vie.</description></item>
    /// <item><description>Ne marque aucune découpe comme refusée : le détachement consécutif au refus d'une barre relève d'un contrat distinct.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="IS_ProductionCutPiece_AssignToBar"/>
    public interface IS_ProductionCutPiece_DetachFromBar
    {
        // --- Groupe 1 : Détachement des découpes d'une barre de production ---

        /// <summary>
        /// Défait le plan de coupe provisoire de la barre de production désignée en libérant
        /// toutes ses découpes rattachées vers le vivier des pièces à optimiser, sans les marquer
        /// comme refusées, puis confie leur mise à jour à l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de la validation de barre, sur le chemin
        /// avec défauts, à l'intérieur de la transaction qu'il a ouverte, après la validation de la
        /// barre et avant la recomposition du plan. Les découpes sont chargées en une lecture
        /// unique avec suivi des changements, sélectionnées par leur seul rattachement à la barre,
        /// puis leur mise à jour est déléguée en une seule opération au Command Handler générique
        /// <c>IC_Generic&lt;ProductionCutPiece&gt;</c>, qui positionne la date de mise à jour et
        /// inscrit un événement par découpe ; l'enregistrement effectif relève de l'appelant.
        /// </para>
        /// <para>
        /// Une découpe n'est détachable que si elle n'est pas encore scellée, n'a pas été réalisée
        /// et n'est pas supprimée logiquement. Une découpe scellée sur la barre signalerait une
        /// inversion de la séquence de validation. L'opération n'est pas idempotente : un second
        /// appel sur la même barre échoue, aucune découpe n'y étant plus rattachée.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif.</description></item>
        /// <item><description>Vérifier qu'au moins une découpe est rattachée à la barre.</description></item>
        /// <item><description>Vérifier que l'état de chaque découpe rattachée permet son détachement, et rejeter en un échec unique l'ensemble des découpes qui ne le permettent pas.</description></item>
        /// <item><description>Sur chaque découpe, lever le marqueur de placement provisoire, supprimer le rattachement à la barre et effacer la position de coupe ; aucun autre champ n'est modifié.</description></item>
        /// <item><description>Déléguer la mise à jour de l'ensemble des découpes au Command Handler générique, en une seule opération.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni le marqueur de refus, ni les marqueurs de placement définitif, d'approvisionnement de la barre, de réalisation, de rupture de stock ou de suppression logique, ni les horodatages de coupe, ni les champs d'audit.</description></item>
        /// <item><description>Ne détache aucune découpe tant que tous les contrôles ne sont pas passés.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie ni l'existence ni l'état de la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production dont le plan de coupe provisoire est à défaire ;
        /// seul critère de sélection des découpes. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si aucune découpe n'est rattachée à la
        /// barre désignée ; avec le code <c>BU_ER_04</c>, en un échec unique citant la barre, chaque
        /// découpe concernée et chaque condition violée, si une ou plusieurs découpes rattachées
        /// sont déjà scellées, déjà réalisées ou supprimées logiquement.
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