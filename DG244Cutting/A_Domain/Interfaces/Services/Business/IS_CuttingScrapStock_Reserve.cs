using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de réservation, au profit d'une série de production, d'une chute
    /// du stock retenue par le moteur d'optimisation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de l'optimisation de
    /// barre, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_CuttingScrapStock_Reserve"/>
    /// résidant en <c>B_UseCases/Services/Business</c>. Elle n'est invoquée que pour une barre de
    /// production issue d'une chute : une barre neuve ne consomme aucun stock géré par
    /// l'application.
    /// </para>
    /// <para>
    /// Objectif : l'application consomme en priorité les chutes réutilisables avant de mobiliser
    /// une barre neuve, afin de limiter les pertes de matière. Le stock de chutes est partagé par
    /// toutes les séries de production ; dès qu'une chute est retenue pour recevoir des découpes,
    /// elle doit cesser d'être candidate aux optimisations suivantes, faute de quoi deux séries
    /// pourraient prétendre à la même matière physique et l'opérateur découvrirait l'incohérence
    /// devant un emplacement de rangement vide. Le contrat exprime ce besoin de réservation et
    /// concentre en un point unique l'inscription de la série consommatrice sur la chute.
    /// </para>
    /// <para>
    /// Une chute est disponible lorsqu'elle n'est pas supprimée logiquement, n'est pas en attente
    /// d'intégration et ne porte aucune réservation, c'est-à-dire lorsque son champ
    /// <c>ReservedFor</c> est nul ou vide. La réservation renseigne ce champ avec l'identifiant de
    /// la série consommatrice et retire ainsi la chute des recherches de disponibilité.
    /// </para>
    /// <para>
    /// La réservation n'est pas une consommation : la chute demeure en stock et son devenir suit
    /// celui de la barre qui la porte. La validation de la barre consomme la chute, son refus
    /// l'écarte comme défectueuse, sa déclaration en rupture la laisse réservée en attente. La
    /// réservation conserve la trace de la série consommatrice et n'est libérée qu'à la clôture de
    /// la série.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de réservation d'une chute désignée au profit d'une série de production désignée.</description></item>
    /// <item><description>Garantir que seule une chute disponible, selon le prédicat de disponibilité appliqué à la lecture du stock, peut être réservée.</description></item>
    /// <item><description>Garantir que la réservation est la seule modification portée sur la chute.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne consomme pas la chute : sa suppression logique relève de la validation de la barre.</description></item>
    /// <item><description>Ne libère aucune réservation : la libération relève de la clôture de la série.</description></item>
    /// <item><description>Ne vérifie ni l'existence de la série ni la cohérence d'article entre la chute et la série : le choix de la chute est repris tel que déterminé par le moteur d'optimisation, alimenté par le seul stock de l'article traité.</description></item>
    /// <item><description>Ne distingue pas l'origine de la barre : l'appel n'intervient que pour une barre issue d'une chute.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie : les mouvements de stock ne relèvent pas du cycle de vie des produits.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.CuttingScrapStock"/>
    public interface IS_CuttingScrapStock_Reserve
    {
        // --- Groupe 1 : Réservation d'une chute au profit d'une série de production ---

        /// <summary>
        /// Réserve la chute désignée au profit de la série de production désignée, en inscrivant
        /// l'identifiant de la série sur la chute, puis confie sa mise à jour à l'écriture générique
        /// sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, pour une barre de production issue d'une chute exclusivement. La chute est lue
        /// avec suivi des changements, puis sa mise à jour est déléguée au Command Handler générique
        /// <c>IC_Generic&lt;CuttingScrapStock&gt;</c>, qui positionne la date de mise à jour et
        /// inscrit un événement technique ; la chute reste suivie dans le contexte partagé et n'est
        /// enregistrée qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : retirer des recherches de disponibilité la chute retenue par l'optimisation, en
        /// la rattachant à la série qui la consommera, afin qu'aucune autre série ne puisse y
        /// prétendre.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier les préconditions structurelles des arguments : identifiant de chute et identifiant de série strictement positifs.</description></item>
        /// <item><description>Vérifier que la chute désignée existe.</description></item>
        /// <item><description>Vérifier que l'état de la chute permet la réservation : non supprimée logiquement, non en attente d'intégration, non déjà réservée ; une réservation nulle ou vide est tenue pour libre, une réservation composée de blancs ne l'est pas.</description></item>
        /// <item><description>Refuser toute réservation existante, y compris celle qui porterait déjà la série appelante : aucune réservation n'est écrasée.</description></item>
        /// <item><description>Inscrire l'identifiant de la série, converti en chaîne, dans le champ <c>ReservedFor</c> de la chute ; aucun autre champ n'est modifié.</description></item>
        /// <item><description>Déléguer la mise à jour de la chute au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni l'indicateur de suppression logique, ni les indicateurs d'attente d'intégration et d'inventaire, ni la longueur, le code-barres ou l'article de la chute, ni les champs d'audit.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie pas l'existence de la série.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idCuttingScrapStock">
        /// Identifiant de la chute retenue par le moteur d'optimisation. Doit être strictement positif.
        /// </param>
        /// <param name="idProductionSeries">
        /// Identifiant de la série de production consommatrice. Doit être strictement positif ; son
        /// existence n'est pas vérifiée.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idCuttingScrapStock"/> ou
        /// <paramref name="idProductionSeries"/> n'est pas strictement positif ; avec le code
        /// <c>BU_ER_03</c> si la chute désignée est introuvable ; avec le code <c>BU_ER_04</c>, en un
        /// échec unique citant chaque condition violée, si la chute est supprimée logiquement, en
        /// attente d'intégration ou déjà réservée.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la chute ou de la délégation de sa mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idCuttingScrapStock,
            int idProductionSeries,
            CancellationToken ct = default);
    }
}