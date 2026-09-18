using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de retrait définitif du stock, par suppression logique, de la chute
    /// dont est issue une barre de production sortie du circuit de production.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par les UseCases orchestrateurs du traitement d'une
    /// barre de production, validation ou refus, qui en délèguent l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_CuttingScrapStock_Withdraw"/>
    /// résidant en <c>B_UseCases/Services/Business</c>. Elle n'est invoquée que pour une barre de
    /// production issue d'une chute : une barre neuve ne consomme aucun stock géré par
    /// l'application.
    /// </para>
    /// <para>
    /// Objectif : l'application consomme en priorité les chutes réutilisables avant de mobiliser
    /// une barre neuve, et le stock de chutes doit refléter ce qui se trouve physiquement dans les
    /// emplacements de rangement de l'atelier. Une chute portée par une barre qui sort du circuit
    /// de production ne s'y trouve plus, ou ne doit plus être utilisée : elle doit quitter le stock
    /// et ne plus jamais être proposée à l'optimisation. Le contrat exprime ce besoin de retrait du
    /// stock et le concentre en un point unique.
    /// </para>
    /// <para>
    /// Le retrait du stock recouvre trois situations qui convergent exactement sur la chute : la
    /// consommation par la coupe, lorsque la barre est validée sans défaut ; la consommation sans
    /// production, lorsque la barre est validée avec défauts sans qu'aucune pièce n'ait pu y être
    /// placée ; l'écart pour défaut ou absence, lorsque la barre est refusée parce que la chute est
    /// défectueuse ou introuvable à son emplacement. Dans les trois cas la chute est consommée ou
    /// écartée et ne sera jamais remise en stock. Le contrat ignore laquelle de ces situations
    /// l'invoque : la distinction relève exclusivement du UseCase appelant, qui traite la barre et
    /// ses découpes et en porte la traçabilité.
    /// </para>
    /// <para>
    /// Le retrait est une suppression logique : la chute demeure enregistrée, et le prédicat de
    /// disponibilité, qui exclut toute chute supprimée logiquement, la retire des recherches du
    /// stock. Sa réservation est conservée : elle porte la trace de la série de production au
    /// profit de laquelle la chute a été retenue.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de retrait du stock d'une chute désignée.</description></item>
    /// <item><description>Garantir que seule une chute existante et non encore retirée peut être retirée.</description></item>
    /// <item><description>Garantir que la suppression logique est la seule modification portée sur la chute.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne distingue pas la consommation par la coupe, la consommation sans production et l'écart pour défaut ou absence : la distinction relève du UseCase appelant.</description></item>
    /// <item><description>Ne connaît pas la barre de production : il reçoit le seul identifiant de la chute et ne contrôle pas sa cohérence avec la barre, que l'appelant a chargée.</description></item>
    /// <item><description>Ne libère pas la réservation de la chute et ne l'exige pas : une réservation absente, par exemple après une reprise sur incident, n'empêche pas le retrait.</description></item>
    /// <item><description>Ne contrôle pas l'attente d'intégration de la chute : il s'agit d'un état de gestion du stock étranger au retrait, la chute ayant déjà été coupée ou écartée physiquement.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : les mouvements de stock ne relèvent pas du cycle de vie des produits.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.CuttingScrapStock"/>
    public interface IS_CuttingScrapStock_Withdraw
    {
        // --- Groupe 1 : Retrait d'une chute du stock ---

        /// <summary>
        /// Retire du stock, par suppression logique, la chute désignée, dont est issue une barre de
        /// production sortie du circuit de production, et confie cette suppression logique à
        /// l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, lors de la validation ou du refus d'une barre de production issue d'une chute.
        /// La chute est lue avec suivi des changements afin de contrôler son existence et son état,
        /// puis sa suppression logique est déléguée au Command Handler générique
        /// <c>IC_Generic&lt;CuttingScrapStock&gt;</c>, qui positionne l'indicateur de suppression
        /// logique et la date de mise à jour et inscrit un événement technique. La chute n'est
        /// enregistrée qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : faire sortir définitivement du stock une chute consommée ou écartée, afin
        /// qu'elle ne soit plus jamais proposée à l'optimisation.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier la précondition structurelle de l'argument : identifiant de chute strictement positif.</description></item>
        /// <item><description>Vérifier que la chute désignée existe.</description></item>
        /// <item><description>Vérifier que la chute n'est pas déjà supprimée logiquement ; un retrait répété trahit une erreur d'orchestration, telle que deux barres prétendant à la même chute ou un double appel.</description></item>
        /// <item><description>Déléguer la suppression logique de la chute au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>N'affecte lui-même aucun champ de la chute : l'indicateur de suppression logique et la date de mise à jour relèvent du Command Handler générique.</description></item>
        /// <item><description>Laisse intacts la réservation, les indicateurs d'attente d'intégration et d'inventaire, les dimensions, le code-barres, l'article, l'emplacement, la date d'entrée, le prix et l'origine de la chute.</description></item>
        /// <item><description>Ne contrôle ni la réservation, ni l'attente d'intégration, ni la cohérence de la chute avec la barre appelante.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idCuttingScrapStock">
        /// Identifiant de la chute dont est issue la barre de production sortie du circuit. Doit être
        /// strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idCuttingScrapStock"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si la chute désignée est introuvable ;
        /// avec le code <c>BU_ER_04</c> si la chute est déjà supprimée logiquement. Remonte également
        /// sans interception toute <see cref="Ex_Business"/> levée par l'écriture générique,
        /// notamment l'inéligibilité d'une entité à la suppression logique (<c>BU_ER_03</c>), sans
        /// objet pour une chute, qui porte l'indicateur de suppression logique.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la chute ou de la délégation de sa suppression logique.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idCuttingScrapStock,
            CancellationToken ct = default);
    }
}