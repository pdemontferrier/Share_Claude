using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de recalcul de l'indicateur de rupture de stock d'une série de
    /// production, à partir de l'état courant de ses barres.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la déclaration de
    /// rupture de stock d'une barre de production et de sa libération, qui en délègue l'exécution
    /// au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionSeries_SetBarOutOfStockFlag"/>
    /// résidant en <c>B_UseCases/Services/Business</c>. Le service est appelé à l'intérieur de la
    /// transaction ouverte par son appelant, sur le contexte de données partagé.
    /// </para>
    /// <para>
    /// Objectif : une barre optimisée et présentée à l'opérateur peut se révéler indisponible,
    /// neuve comme de chute. Elle est alors déclarée en rupture de stock : elle n'est ni supprimée
    /// ni écartée, mais mise de côté jusqu'à ce que la matière redevienne disponible. Le tableau de
    /// bord des séries signale, sans ouverture de chaque série, celles qui comportent au moins une
    /// barre en rupture ; il lit pour cela l'indicateur <c>IsBarOutOfStock</c> de la série. Le
    /// contrat concentre en un point unique l'écriture de cet indicateur : le service en est le seul
    /// écrivain et ne reçoit jamais la valeur à poser, qu'il déduit lui-même de l'état des barres.
    /// </para>
    /// <para>
    /// Règle de calcul : l'indicateur vaut <see langword="true"/> si et seulement si la série
    /// comporte au moins une barre en rupture de stock non supprimée logiquement. Aucune distinction
    /// n'est faite entre barre neuve et barre de chute ; une barre refusée, donc supprimée
    /// logiquement, est sortie du circuit et n'est pas prise en compte. Le recalcul est symétrique :
    /// la déclaration d'une rupture fait passer la série à <see langword="true"/> ; la libération
    /// d'une barre ne la remet à <see langword="false"/> que si aucune autre barre n'est encore en
    /// rupture.
    /// </para>
    /// <para>
    /// Le recalcul est porté par un service, et non par un UseCase, afin d'être exécuté dans la
    /// transaction de l'appelant : un UseCase ouvre sa propre transaction et ne peut être invoqué
    /// à l'intérieur de celle d'un autre UseCase. Chaque appelant conserve ainsi un enregistrement
    /// unique de l'ensemble de ses modifications.
    /// </para>
    /// <para>
    /// Évaluation de l'état des barres : l'appelant modifie les barres dans le contexte partagé sans
    /// les enregistrer avant l'appel, et une requête adressée à la base lirait leur état enregistré
    /// antérieur. L'état des barres est donc évalué en mémoire, sur les instances suivies par le
    /// contexte partagé, modifications non enregistrées comprises ; la base n'est interrogée que sur
    /// l'identifiant de série.
    /// </para>
    /// <para>
    /// Conditions d'usage et limites : l'appel intervient après modification des barres sur leurs
    /// instances suivies. Une barre ajoutée au contexte sans avoir été enregistrée n'est pas prise en
    /// compte ; une barre en cours de suppression physique serait encore prise en compte. Ces deux
    /// situations sont étrangères à la déclaration et à la libération d'une rupture, le refus d'une
    /// barre passant par sa suppression logique, évaluée en mémoire.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de recalcul de l'indicateur de rupture de stock d'une série de production désignée.</description></item>
    /// <item><description>Garantir que l'indicateur est déduit de l'état courant des barres de la série et jamais reçu de l'appelant.</description></item>
    /// <item><description>Garantir la symétrie du recalcul : les deux sens de variation sont écrits dès qu'ils changent.</description></item>
    /// <item><description>Garantir que l'indicateur de rupture de stock est la seule modification portée sur la série.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne déclare ni ne libère la rupture d'une barre : la modification des barres relève de l'appelant, qui l'opère avant l'appel.</description></item>
    /// <item><description>Ne modifie aucune barre et aucun autre indicateur d'avancement de la série.</description></item>
    /// <item><description>Ne restitue aucune indication de changement à l'appelant.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.ProductionSeries"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.ProductionBar"/>
    public interface IS_ProductionSeries_SetBarOutOfStockFlag
    {
        // --- Groupe 1 : Recalcul de l'indicateur de rupture de stock d'une série ---

        /// <summary>
        /// Recalcule l'indicateur de rupture de stock de la série de production désignée à partir de
        /// l'état courant de ses barres, puis confie sa mise à jour à l'écriture générique s'il a
        /// changé, sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, après modification des barres concernées sur leurs instances suivies. La série
        /// est lue avec suivi des changements, puis les barres de la série sont lues avec suivi des
        /// changements sur le seul critère de l'identifiant de série ; leur état de rupture et de
        /// suppression logique est évalué en mémoire, modifications non enregistrées comprises. Les
        /// barres de la série qui n'étaient pas encore suivies le deviennent, à l'état inchangé.
        /// </para>
        /// <para>
        /// Écriture conditionnelle : la série n'est confiée au Command Handler générique
        /// <c>IC_Generic&lt;ProductionSeries&gt;</c> que si la valeur recalculée diffère de la valeur
        /// courante. Un appel sur un état inchangé ne laisse ainsi aucune trace : ni date de mise à
        /// jour, ni événement technique. Lorsque la valeur change, dans un sens comme dans l'autre, le
        /// Command Handler positionne la date de mise à jour et inscrit un événement technique ; la
        /// série reste suivie dans le contexte partagé et n'est enregistrée qu'à la validation de la
        /// transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : maintenir exact, en toutes circonstances, le signalement des séries comportant
        /// au moins une barre en rupture de stock.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier la précondition structurelle de l'argument : identifiant de série strictement positif.</description></item>
        /// <item><description>Vérifier que la série désignée existe et n'est pas supprimée logiquement, avant toute lecture de ses barres.</description></item>
        /// <item><description>Déterminer si la série comporte au moins une barre en rupture de stock non supprimée logiquement.</description></item>
        /// <item><description>Positionner <c>IsBarOutOfStock</c> à la valeur recalculée et déléguer la mise à jour au Command Handler générique, uniquement si cette valeur diffère de la valeur courante.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ de la série, en particulier aucun indicateur d'approvisionnement, de début ou de fin de découpe, ni aucune barre.</description></item>
        /// <item><description>N'interroge jamais la base sur l'état de rupture ou de suppression logique des barres.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionSeries">
        /// Identifiant de la série de production dont l'indicateur de rupture de stock est recalculé.
        /// Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé aux Query Handlers et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si la série désignée est introuvable ;
        /// avec le code <c>BU_ER_04</c> si la série désignée est supprimée logiquement.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la série ou de ses barres, ou lors de la délégation de la mise à jour de la série.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionSeries,
            CancellationToken ct = default);
    }
}