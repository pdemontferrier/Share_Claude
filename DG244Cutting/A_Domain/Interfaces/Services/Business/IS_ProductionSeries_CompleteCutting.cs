using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de clôture des découpes d'une série de production, qui marque la
    /// série comme ayant achevé l'ensemble de ses découpes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la clôture d'une
    /// série de production, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionSeries_CompleteCutting"/>
    /// résidant en <c>B_UseCases/Services/Business</c>. Le service est appelé à l'intérieur de la
    /// transaction ouverte par son appelant, sur le contexte de données partagé.
    /// </para>
    /// <para>
    /// Objectif : une série de production regroupe des découpes issues des commandes clients, que
    /// le parcours de découpe traite barre après barre jusqu'à ce qu'il ne reste plus rien à
    /// couper. Le contrat inscrit ce point d'arrivée en posant l'indicateur
    /// <c>IsCuttingCompleted</c> de la série. Cet indicateur conditionne le classement de la série
    /// parmi les séries terminées au tableau de bord, son ouverture en consultation seule plutôt
    /// qu'en production, et son exclusion de la reprise du parcours de découpe.
    /// </para>
    /// <para>
    /// Répartition des responsabilités : le contrat n'établit pas la condition de clôture. Le
    /// constat qu'aucune découpe de la série ne reste à couper relève de l'appelant, qui l'établit
    /// avant l'appel ; le service écrit sur la série sans interroger ni ses découpes ni ses barres.
    /// La journalisation de la clôture dans le cycle de vie de la série, la libération des chutes
    /// réservées pour elle et l'enregistrement de l'ensemble relèvent également de l'appelant, dans
    /// la même transaction.
    /// </para>
    /// <para>
    /// Caractère définitif : le service ne pose jamais que la valeur <see langword="true"/> et
    /// n'offre aucun retour arrière. Il rejette la clôture d'une série déjà clôturée : un second
    /// appel traduit une erreur d'orchestration et conduirait l'appelant à journaliser une seconde
    /// fois la même clôture.
    /// </para>
    /// <para>
    /// La clôture est portée par un service, et non par un UseCase, afin d'être exécutée dans la
    /// transaction de l'appelant : un UseCase ouvre sa propre transaction et ne peut être invoqué à
    /// l'intérieur de celle d'un autre UseCase. L'appelant conserve ainsi un enregistrement unique
    /// de l'ensemble de ses modifications.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de clôture des découpes d'une série de production désignée.</description></item>
    /// <item><description>Garantir que l'indicateur de clôture des découpes est la seule modification portée sur la série.</description></item>
    /// <item><description>Garantir le caractère définitif de la clôture : seule la valeur <see langword="true"/> est posée, et une série déjà clôturée est rejetée.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>N'établit pas la condition de clôture et n'interroge ni les découpes ni les barres de la série.</description></item>
    /// <item><description>Ne contrôle pas l'engagement préalable de la série : ni l'approvisionnement en barres de chutes ou en barres neuves, ni le début des découpes ne sont vérifiés.</description></item>
    /// <item><description>Ne modifie aucun autre indicateur d'avancement, aucune date de planification ni aucune caractéristique de la série.</description></item>
    /// <item><description>Ne libère aucune chute réservée pour la série.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.ProductionSeries"/>
    public interface IS_ProductionSeries_CompleteCutting
    {
        // --- Groupe 1 : Clôture des découpes d'une série ---

        /// <summary>
        /// Marque comme achevées les découpes de la série de production désignée en posant son
        /// indicateur de clôture, puis confie sa mise à jour à l'écriture générique, sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, après qu'il a établi qu'aucune découpe de la série ne reste à couper. La série
        /// est lue avec suivi des changements, puis contrôlée avant toute modification. Un échec
        /// survenant avant la modification laisse la série intacte.
        /// </para>
        /// <para>
        /// Écriture : une fois les contrôles passés, l'indicateur <c>IsCuttingCompleted</c> passe
        /// de <see langword="false"/> à <see langword="true"/> et la série est confiée au Command
        /// Handler générique <c>IC_Generic&lt;ProductionSeries&gt;</c>, qui positionne la date de
        /// mise à jour et inscrit un événement technique. Le rejet d'une série déjà clôturée
        /// garantit que chaque écriture correspond à un changement réel. La série reste suivie dans
        /// le contexte partagé et n'est enregistrée qu'à la validation de la transaction par
        /// l'appelant.
        /// </para>
        /// <para>
        /// Objectif : inscrire sur la série le point d'arrivée de son parcours de découpe.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier la précondition structurelle de l'argument : identifiant de série strictement positif.</description></item>
        /// <item><description>Vérifier, dans l'ordre, que la série désignée existe, qu'elle n'est pas supprimée logiquement et qu'elle n'est pas déjà clôturée.</description></item>
        /// <item><description>Positionner <c>IsCuttingCompleted</c> à <see langword="true"/> et déléguer la mise à jour au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ de la série, en particulier aucun indicateur d'approvisionnement, d'optimisation, de validation, de rupture de stock ou de début de découpe, ni aucune date de planification ; l'horodatage de la clôture relève de l'action de cycle de vie inscrite par l'appelant.</description></item>
        /// <item><description>Ne vérifie pas qu'aucune découpe ne reste à couper.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionSeries">
        /// Identifiant de la série de production dont les découpes sont marquées comme achevées.
        /// Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si la série désignée est introuvable ;
        /// avec le code <c>BU_ER_04</c> si la série désignée est supprimée logiquement ou si ses
        /// découpes sont déjà marquées comme achevées.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la série ou lors de la délégation de sa mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionSeries,
            CancellationToken ct = default);
    }
}