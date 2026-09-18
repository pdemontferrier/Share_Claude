using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de rattachement provisoire, à une barre de production, des
    /// découpes retenues par le moteur d'optimisation, dans l'ordre de coupe calculé.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de l'optimisation de
    /// barre et par le UseCase orchestrateur de la validation de barre lorsqu'une
    /// ré-optimisation est requise, qui en délèguent l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionCutPiece_AssignToBar"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'application approvisionne chaque barre juste avant sa coupe. Lorsqu'elle a
    /// décidé quelles pièces couper sur une barre, cette décision doit être inscrite sur les
    /// découpes elles-mêmes : chacune quitte le vivier des pièces à optimiser, se rattache à la
    /// barre et reçoit son rang dans le plan de coupe, rang qui fixe l'ordre dans lequel
    /// l'opérateur réalisera les découpes. Le contrat exprime ce besoin d'inscription et
    /// concentre en un point unique la traduction de l'ordre calculé en positions de coupe
    /// persistantes.
    /// </para>
    /// <para>
    /// L'ordre de la liste d'identifiants reçue est porteur de sens : le rang d'une découpe dans
    /// cette liste, compté à partir de 1, est sa position de coupe dans la barre. Aucune autre
    /// donnée ne transporte cette position ; réordonner la liste casserait le plan de coupe sans
    /// produire aucune erreur visible, la barre étant alors découpée dans un ordre différent de
    /// celui calculé et les longueurs ne tombant plus juste.
    /// </para>
    /// <para>
    /// Le placement inscrit est provisoire : il marque les découpes comme placées à titre
    /// provisoire sans les sceller, le scellement du plan de coupe relevant de la validation de
    /// la barre, et un refus de la barre libérant ces mêmes découpes en défaisant exactement ce
    /// marquage. Une découpe précédemment refusée sur une autre barre, revenue au vivier avec son
    /// marqueur de refus, retrouve par ce rattachement un statut neutre.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de rattachement provisoire d'un ensemble ordonné de découpes à une barre de production déjà persistée.</description></item>
    /// <item><description>Garantir que la position de coupe attribuée à chaque découpe est son rang dans la liste reçue, sans aucun réordonnancement.</description></item>
    /// <item><description>Garantir un traitement intégral : toutes les découpes sont marquées ou aucune ne l'est.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne crée pas la barre de production et ne vérifie pas son existence : l'intégrité référentielle entre la découpe et la barre est garantie par la persistance déclenchée par l'appelant.</description></item>
    /// <item><description>Ne choisit pas les découpes et ne calcule pas leur ordre : l'un et l'autre sont repris tels que déterminés par le moteur d'optimisation.</description></item>
    /// <item><description>Ne scelle pas le plan de coupe, ne pose pas l'approvisionnement de la barre, ne valide pas la réalisation des découpes et ne défait pas un rattachement : ces opérations relèvent d'étapes ultérieures du cycle de vie.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie : la traçabilité métier commence à la validation de la barre.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.DTOs.Business.DTO_CuttingOptimizationResult.PieceIds"/>
    public interface IS_ProductionCutPiece_AssignToBar
    {
        // --- Groupe 1 : Rattachement provisoire des découpes à une barre de production ---

        /// <summary>
        /// Rattache à une barre de production les découpes désignées, en les marquant comme
        /// provisoirement placées et en leur attribuant leur position de coupe selon leur rang
        /// dans la liste reçue, puis confie leur mise à jour à l'écriture générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, après l'enregistrement intermédiaire qui a rendu disponible l'identifiant de la
        /// barre. Les découpes sont chargées en une lecture unique avec suivi des changements, puis
        /// leur mise à jour est déléguée au Command Handler générique
        /// <c>IC_Generic&lt;ProductionCutPiece&gt;</c>, qui positionne la date de mise à jour et
        /// inscrit un événement par découpe ; les découpes restent suivies dans le contexte partagé
        /// et ne sont enregistrées qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier les préconditions structurelles des arguments : liste renseignée et non vide, identifiants strictement positifs et sans doublon, identifiant de barre strictement positif.</description></item>
        /// <item><description>Vérifier que chaque découpe désignée existe et que son état permet le rattachement : non déjà rattachée à une barre, non réalisée, non supprimée logiquement.</description></item>
        /// <item><description>Attribuer à chaque découpe, comme position de coupe, son rang dans la liste reçue compté à partir de 1, l'appariement entre identifiant et découpe chargée se faisant par identifiant et jamais par rang dans la liste chargée.</description></item>
        /// <item><description>Marquer chaque découpe comme provisoirement placée, lever son marqueur de refus et la rattacher à la barre ; aucun autre champ n'est modifié.</description></item>
        /// <item><description>Déléguer la mise à jour de l'ensemble des découpes au Command Handler générique, en une seule opération.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni l'indicateur d'optimisation scellée, ni l'indicateur d'approvisionnement de la barre, ni l'indicateur de réalisation, ni l'indicateur de suppression logique, ni les champs d'audit.</description></item>
        /// <item><description>Ne trie ni ne réordonne jamais la liste reçue, y compris à titre interne.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie pas l'existence de la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="pieceIds">
        /// Identifiants des découpes retenues, dans l'ordre de coupe : le rang de chaque identifiant,
        /// compté à partir de 1, devient la position de coupe de la découpe correspondante. Ne doit
        /// pas être <see langword="null"/> ni vide ; chaque identifiant doit être strictement positif
        /// et n'apparaître qu'une seule fois.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production de rattachement, attribué par la persistance. Doit
        /// être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="pieceIds"/> est
        /// <see langword="null"/> ; avec le code <c>BU_ER_02</c> si <paramref name="pieceIds"/> est
        /// vide, si <paramref name="idProductionBar"/> n'est pas strictement positif ou si un
        /// identifiant de découpe n'est pas strictement positif ; avec le code <c>BU_ER_03</c> si un
        /// identifiant de découpe figure plusieurs fois dans la liste, ou si une ou plusieurs
        /// découpes désignées sont introuvables ; avec le code <c>BU_ER_04</c>, en un échec unique
        /// citant chaque découpe concernée, si une ou plusieurs découpes sont déjà rattachées à une
        /// barre, déjà réalisées ou supprimées logiquement.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture des découpes ou de la délégation de leur mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            IReadOnlyList<int> pieceIds,
            int idProductionBar,
            CancellationToken ct = default);
    }
}