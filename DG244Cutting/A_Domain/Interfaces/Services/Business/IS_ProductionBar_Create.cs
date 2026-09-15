using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de création d'une barre de production provisoire à partir
    /// du résultat réussi d'une optimisation de découpe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de l'optimisation
    /// de barre, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionBar_Create"/> résidant
    /// en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'application approvisionne la matière à la demande et n'optimise qu'une
    /// barre à la fois. Lorsque le moteur d'optimisation a déterminé quelle matière mobiliser
    /// (barre neuve ou chute du stock) et quelles découpes y réaliser, cette décision doit
    /// devenir une barre de production que l'opérateur consulte, puis valide, refuse ou
    /// déclare en rupture de stock. Le contrat exprime ce besoin de matérialisation et
    /// concentre en un point unique la correspondance entre le vocabulaire du résultat de
    /// calcul et celui du modèle de données, de sorte que l'orchestrateur reste sans
    /// connaissance des champs de <see cref="ProductionBar"/>.
    /// </para>
    /// <para>
    /// La barre créée est provisoire : marquée comme placement provisoire
    /// (<c>IsOptimizedTemp</c>), elle n'est ni validée ni scellée, et son plan de coupe peut
    /// encore être défait par un refus ou recomposé par une validation avec défauts. Ce
    /// marqueur n'est jamais retiré au cours de la vie de la barre ; ce sont les états de
    /// validation, d'utilisation, de rupture de stock ou de suppression logique qui la font
    /// ensuite progresser, puis sortir du circuit.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de création d'une barre de production provisoire.</description></item>
    /// <item><description>Exposer en retour l'instance créée, afin que l'orchestrateur puisse y rattacher les découpes une fois l'identifiant attribué par la persistance.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif et l'attribution de l'identifiant relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne porte aucune règle de calcul : le choix des découpes, la longueur physique du résidu et sa qualification sont repris tels que déterminés par le moteur d'optimisation.</description></item>
    /// <item><description>Ne rattache pas les découpes à la barre et ne réserve pas la chute source : ces opérations relèvent d'autres actions orchestrées par le UseCase.</description></item>
    /// <item><description>Ne renseigne ni les défauts, ni le reliquat final validé, ni la valorisation du reste, ni le motif de refus, ni les indicateurs d'approvisionnement de la série : ces informations sont posées aux étapes ultérieures du cycle de vie de la barre.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie : la traçabilité métier de la barre commence à sa validation.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DTO_CuttingOptimizationResult"/>
    public interface IS_ProductionBar_Create
    {
        // --- Groupe 1 : Création d'une barre de production provisoire ---

        /// <summary>
        /// Construit la barre de production provisoire correspondant à un résultat
        /// d'optimisation réussi, pour une série et un article interne donnés, puis la
        /// confie à l'écriture générique sans la persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de l'optimisation de barre, à
        /// l'intérieur de la transaction qu'il a ouverte. La mutation est déléguée au Command
        /// Handler générique <c>IC_Generic&lt;ProductionBar&gt;</c>, qui positionne la date de
        /// création et inscrit l'événement associé ; l'entité rejoint ainsi le suivi du
        /// contexte partagé et n'est enregistrée qu'à la validation de la transaction par
        /// l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier les préconditions structurelles des arguments et la cohérence du résultat reçu : issue de succès, au moins une découpe, longueur de barre strictement positive, origine du contenant cohérente avec sa chute source.</description></item>
        /// <item><description>Reporter sur la barre la série, l'article interne, l'origine du contenant, la chute source, la longueur de barre, le nombre de découpes (cardinal de la liste des découpes retenues) et le résidu physique.</description></item>
        /// <item><description>Recopier sans transformation la qualification du résidu, dont la polarité est : <see langword="true"/> pour une chute réutilisable, <see langword="false"/> pour un déchet ; toute inversion conduirait à jeter des chutes et à ranger des déchets en stock.</description></item>
        /// <item><description>Marquer la barre comme placement provisoire, seul indicateur d'état positionné ; tous les autres états de cycle de vie restent à <see langword="false"/>.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne recalcule ni le résidu, ni sa qualification, et ne contrôle aucune de ces deux valeurs.</description></item>
        /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne relit pas l'entité après écriture.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="result">
        /// Résultat de l'invocation du moteur d'optimisation de découpe décrivant la barre à
        /// matérialiser. Ne doit pas être <see langword="null"/> ; son issue doit être un
        /// succès et sa liste d'identifiants de découpes doit être renseignée et non vide.
        /// </param>
        /// <param name="idProductionSeries">
        /// Identifiant de la série de production à laquelle la barre est rattachée. Doit être
        /// strictement positif.
        /// </param>
        /// <param name="idArticleInternal">
        /// Identifiant de l'article interne dont la barre est constituée. Doit être strictement
        /// positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Instance de <see cref="ProductionBar"/> construite et confiée au Command Handler,
        /// jamais <see langword="null"/>. Son identifiant vaut <c>0</c> au retour ; il est
        /// attribué par la base lors de l'enregistrement déclenché par l'appelant, qui conserve
        /// la référence pour y rattacher ensuite les découpes.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="result"/> ou sa liste
        /// d'identifiants de découpes est <see langword="null"/> ; avec le code <c>BU_ER_02</c>
        /// si <paramref name="idProductionSeries"/> ou <paramref name="idArticleInternal"/> n'est
        /// pas strictement positif, si la liste des découpes est vide ou si la longueur de barre
        /// n'est pas strictement positive ; avec le code <c>BU_ER_03</c> si l'issue du résultat
        /// n'est pas un succès, si une barre neuve porte une chute source, ou si une barre de
        /// chute ne porte pas de chute source d'identifiant strictement positif.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la délégation au Command Handler.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task<ProductionBar> ExecuteAsync(
            string caller,
            DTO_CuttingOptimizationResult result,
            int idProductionSeries,
            int idArticleInternal,
            CancellationToken ct = default);
    }
}