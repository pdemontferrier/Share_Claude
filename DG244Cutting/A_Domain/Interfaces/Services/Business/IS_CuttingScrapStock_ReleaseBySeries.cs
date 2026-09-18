using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de libération, à la clôture d'une série de production, de
    /// l'ensemble des chutes du stock que cette série avait réservées sans les consommer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de clôture de la série
    /// de production, qui l'invoque après avoir marqué la série achevée, à l'intérieur de la
    /// transaction qu'il a ouverte. Elle en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_CuttingScrapStock_ReleaseBySeries"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'application consomme en priorité les chutes réutilisables avant de mobiliser
    /// une barre neuve, afin de limiter les pertes de matière. Le stock de chutes est partagé par
    /// toutes les séries de production ; une chute retenue par une optimisation est réservée au
    /// profit de la série consommatrice et cesse d'être candidate aux optimisations suivantes,
    /// tout en demeurant physiquement en stock. Une chute réservée dont la barre n'a été ni
    /// validée ni refusée n'a jamais été consommée : sans libération, elle resterait invisible au
    /// stock indéfiniment. La clôture de la série est le seul moment où il est certain que la
    /// série ne consommera plus rien ; le contrat exprime le besoin de rendre, à cet instant et en
    /// une seule opération, toutes ces chutes au stock disponible.
    /// </para>
    /// <para>
    /// La libération porte sur la série et non sur une chute désignée : sont concernées toutes les
    /// chutes non supprimées logiquement dont le champ <c>ReservedFor</c> porte l'identifiant de la
    /// série sous sa forme textuelle, y compris celles en attente d'intégration. Les chutes
    /// supprimées logiquement, consommées ou écartées, conservent leur réservation, seule trace de
    /// la série qui les a mobilisées, et ne sont pas concernées.
    /// </para>
    /// <para>
    /// L'information de réservation est tenue pour transitoire : la libération efface, sur chaque
    /// chute rendue au stock, l'indication de la série qui l'avait retenue, et cette perte est
    /// assumée.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de libération en bloc des chutes réservées par une série de production désignée.</description></item>
    /// <item><description>Garantir que la libération retourne les chutes au stock sans jamais les en faire sortir.</description></item>
    /// <item><description>Garantir que la libération est la seule modification portée sur les chutes concernées.</description></item>
    /// <item><description>Garantir que les chutes supprimées logiquement conservent leur réservation.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne consomme aucune chute et n'en supprime aucune logiquement.</description></item>
    /// <item><description>Ne vérifie ni l'existence ni l'état de la série : l'appel intervient après le marquage de la série achevée.</description></item>
    /// <item><description>Ne modifie aucun autre champ que la réservation ; la date de mise à jour relève de l'écriture générique.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : la clôture est journalisée une seule fois, sur la série.</description></item>
    /// <item><description>N'impose aucune dépendance aux Settings ni aux Repositories : l'accès aux données transite par les handlers génériques.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Services.Business.IS_CuttingScrapStock_Reserve"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.CuttingScrapStock"/>
    public interface IS_CuttingScrapStock_ReleaseBySeries
    {
        // --- Groupe 1 : Libération des chutes réservées par une série ---

        /// <summary>
        /// Rend au stock disponible, en une seule opération, toutes les chutes que la série de
        /// production désignée avait réservées sans les consommer, puis confie leur mise à jour à
        /// l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de clôture de la série, à l'intérieur de
        /// la transaction qu'il a ouverte et après le marquage de la série achevée. Les chutes
        /// concernées sont lues avec suivi des changements, puis leur mise à jour est déléguée en
        /// un appel unique au Command Handler générique <c>IC_Generic&lt;CuttingScrapStock&gt;</c>,
        /// qui positionne la date de mise à jour et inscrit un événement technique par chute ; les
        /// chutes restent suivies dans le contexte partagé et ne sont enregistrées qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : réintégrer dans les recherches de disponibilité les chutes physiquement
        /// présentes en atelier que la série close ne consommera plus, afin qu'elles redeviennent
        /// candidates aux optimisations des séries suivantes.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier la précondition structurelle de l'argument : identifiant de série strictement positif.</description></item>
        /// <item><description>Sélectionner les chutes non supprimées logiquement dont la réservation est égale à l'identifiant de la série converti en chaîne, sans autre critère ; une réservation d'un autre format, composée de blancs notamment, n'est pas sélectionnée.</description></item>
        /// <item><description>Admettre une sélection vide comme cas nominal, sans aucune écriture.</description></item>
        /// <item><description>Remettre à <see langword="null"/> le champ <c>ReservedFor</c> de chaque chute sélectionnée, toutes les chutes sélectionnées étant libérées ensemble.</description></item>
        /// <item><description>Déléguer la mise à jour de l'ensemble des chutes libérées au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni l'indicateur de suppression logique, ni les indicateurs d'attente d'intégration et d'inventaire, ni la longueur, le code-barres, l'article ou l'emplacement de la chute.</description></item>
        /// <item><description>Ne restitue pas le nombre de chutes libérées.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie ni l'existence ni l'état de la série.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionSeries">
        /// Identifiant de la série de production close. Doit être strictement positif ; son
        /// existence et son état ne sont pas vérifiés.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif. L'absence de chute réservée par la série n'est pas un échec.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture des chutes ou de la délégation de leur mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionSeries,
            CancellationToken ct = default);
    }
}