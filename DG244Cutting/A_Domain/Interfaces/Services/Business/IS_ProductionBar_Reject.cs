using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de mise à l'écart définitive, hors du circuit de production, d'une
    /// barre de production désignée, avec conservation du motif de sa mise à l'écart.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par les UseCases orchestrateurs du traitement d'une
    /// barre de production, qui en délèguent l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionBar_Reject"/> résidant en
    /// <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'approvisionnement de l'atelier s'effectue à la demande, chaque barre étant
    /// désignée par l'application juste avant sa découpe ; il arrive que la barre désignée ne puisse
    /// pas être utilisée. Deux situations conduisent alors à l'écarter. Le refus intervient avant
    /// toute acceptation : barre inutilisable, endommagée au-delà des défauts admissibles,
    /// approvisionnement impossible ou, pour une barre issue d'une chute, chute introuvable à son
    /// emplacement ; le motif est choisi par l'opérateur. La barre-déchet intervient après
    /// acceptation, lorsque la recomposition du plan de coupe n'a pu placer aucune pièce sur la
    /// barre validée ; le motif est fourni par le UseCase. Ces deux situations convergent sur la
    /// barre elle-même, qui est écartée avec un motif : c'est cette seule convergence que le contrat
    /// exprime. Il ignore laquelle des deux situations l'invoque, la différence étant portée par le
    /// motif reçu et par les traitements que le UseCase conduit autour de la mise à l'écart.
    /// </para>
    /// <para>
    /// La mise à l'écart est une suppression logique : une barre écartée n'est plus jamais
    /// présentée à l'opérateur, les recherches du circuit de production l'excluant sur le seul
    /// critère de suppression logique. Aucune barre n'est écartée sans motif. Le placement
    /// définitif dans le plan marque le point de non-retour : une barre scellée dans le plan ne
    /// peut plus être écartée. L'acceptation physique de la barre n'est pas un critère : une barre
    /// refusée n'est pas acceptée, une barre-déchet l'est.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de mise à l'écart d'une barre de production désignée, assortie de son motif.</description></item>
    /// <item><description>Garantir qu'aucune barre n'est écartée sans motif exploitable.</description></item>
    /// <item><description>Garantir que seule une barre active, non épuisée, non en rupture de stock et non scellée dans le plan peut être écartée.</description></item>
    /// <item><description>Garantir que l'inscription du motif et la suppression logique sont les seules modifications portées sur la barre.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne distingue pas le refus de la barre-déchet : la différence relève du motif reçu et du UseCase appelant.</description></item>
    /// <item><description>Ne traite pas les découpes affectées à la barre, ne statue pas sur le devenir de la chute dont la barre est éventuellement issue et n'inscrit aucune action de cycle de vie : ces traitements relèvent d'autres services invoqués par le UseCase.</description></item>
    /// <item><description>Ne contrôle pas le motif au regard d'un référentiel de motifs : le motif est inscrit tel que reçu.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.ProductionBar"/>
    public interface IS_ProductionBar_Reject
    {
        // --- Groupe 1 : Mise à l'écart d'une barre de production ---

        /// <summary>
        /// Écarte définitivement du circuit de production la barre désignée, en inscrivant le motif
        /// reçu sur la barre, puis confie sa suppression logique à l'écriture générique sans
        /// persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, lors du refus d'une barre avant son acceptation ou lors de l'issue en
        /// barre-déchet d'une barre acceptée. La barre est lue avec suivi des changements ; le
        /// motif est inscrit sur cette instance, puis la suppression logique est déléguée au
        /// Command Handler générique <c>IC_Generic&lt;ProductionBar&gt;</c>, qui positionne
        /// l'indicateur de suppression logique et la date de mise à jour et inscrit un événement
        /// technique portant le motif. La barre n'est enregistrée qu'à la validation de la
        /// transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : retirer la barre du circuit de production de manière définitive, en
        /// conservant la raison de sa mise à l'écart.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier les préconditions structurelles des arguments : identifiant de barre strictement positif, motif non nul, non vide, non composé uniquement d'espaces et de longueur au plus égale à la longueur autorisée.</description></item>
        /// <item><description>Vérifier que la barre désignée existe.</description></item>
        /// <item><description>Vérifier que l'état de la barre permet sa mise à l'écart : non déjà supprimée logiquement, non épuisée, non en rupture de stock, non scellée dans le plan.</description></item>
        /// <item><description>Inscrire le motif, tel que reçu, sur la barre.</description></item>
        /// <item><description>Déléguer la suppression logique de la barre au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne tient compte ni de l'acceptation physique de la barre, ni de son placement provisoire, et ne les modifie pas.</description></item>
        /// <item><description>Ne modifie aucun autre champ que le motif ; la suppression logique et la date de mise à jour relèvent du Command Handler générique.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production à écarter. Doit être strictement positif.
        /// </param>
        /// <param name="rejectionReason">
        /// Motif de mise à l'écart. Ne doit être ni <see langword="null"/>, ni vide, ni composé
        /// uniquement d'espaces ; sa longueur ne doit pas excéder 500 caractères. Il est inscrit tel
        /// que reçu, sans transformation.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="rejectionReason"/> est
        /// <see langword="null"/>, vide ou composé uniquement d'espaces ; avec le code
        /// <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas strictement positif ou si
        /// <paramref name="rejectionReason"/> excède la longueur autorisée ; avec le code
        /// <c>BU_ER_03</c> si la barre désignée est introuvable ; avec le code <c>BU_ER_04</c>, en
        /// un échec unique citant chaque condition violée, si la barre est déjà supprimée
        /// logiquement, épuisée, en rupture de stock ou scellée dans le plan.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la barre ou de la délégation de sa suppression logique.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionBar,
            string rejectionReason,
            CancellationToken ct = default);
    }
}