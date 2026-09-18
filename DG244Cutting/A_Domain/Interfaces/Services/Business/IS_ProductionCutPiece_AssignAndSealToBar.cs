using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de rattachement définitif, à une barre de production validée avec
    /// défauts, des découpes du plan de coupe recomposé par le moteur d'optimisation, dans l'ordre
    /// de coupe calculé.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la validation de
    /// barre (<c>UC_BarValidation</c>), sur le seul chemin de validation avec défauts et lorsque la
    /// recomposition a placé au moins une découpe, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionCutPiece_AssignAndSealToBar"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : une barre de production peut présenter jusqu'à deux zones défectueuses, que
    /// l'opérateur mesure et déclare au moment de la valider. Ces zones consomment de la matière :
    /// le plan de coupe établi avant leur déclaration n'est plus réalisable, et l'application le
    /// recompose sur la matière réellement exploitable. La barre étant déjà acceptée, le plan
    /// recomposé n'a plus vocation à être remis en cause. Le contrat exprime le besoin d'inscrire
    /// ce plan sur les découpes elles-mêmes en une seule écriture : chaque découpe se rattache à la
    /// barre, reçoit son rang dans le plan de coupe et est aussitôt scellée, sans jamais traverser
    /// l'état de placement provisoire.
    /// </para>
    /// <para>
    /// Le contrat réunit deux gestes que le parcours nominal sépare. Dans ce parcours, le placement
    /// précède la décision de l'opérateur et reste provisoire : il est porté par
    /// <see cref="IS_ProductionCutPiece_AssignToBar"/>, puis rendu définitif par le service
    /// homologue de scellement d'un rattachement existant (<c>IS_ProductionCutPiece_SealToBar</c>).
    /// Ici, enchaîner ces deux gestes traverserait inutilement l'état provisoire, imposerait un
    /// enregistrement intermédiaire et inscrirait deux événements par découpe ; le présent contrat
    /// les réalise en une opération unique, réservée au chemin de validation avec défauts.
    /// </para>
    /// <para>
    /// L'ordre de la liste d'identifiants reçue est porteur de sens : le rang d'une découpe dans
    /// cette liste, compté à partir de 1, est sa position de coupe dans la barre. Aucune autre
    /// donnée ne transporte cette position ; réordonner la liste casserait le plan de coupe sans
    /// produire aucune erreur visible, la barre étant alors découpée dans un ordre différent de
    /// celui calculé et les longueurs ne tombant plus juste. Les découpes non retenues par la
    /// recomposition restent au vivier des pièces à optimiser et ne sont pas concernées.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de rattachement définitif d'un ensemble ordonné de découpes à une barre de production validée avec défauts.</description></item>
    /// <item><description>Garantir que la position de coupe attribuée à chaque découpe est son rang dans la liste reçue, sans aucun réordonnancement.</description></item>
    /// <item><description>Garantir que les découpes sont scellées directement, sans être marquées comme provisoirement placées, même transitoirement.</description></item>
    /// <item><description>Garantir un traitement intégral : toutes les découpes sont marquées ou aucune ne l'est.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne valide pas la barre, n'enregistre pas ses défauts, ne détache pas les découpes du plan provisoire et n'inscrit pas le plan recomposé sur la barre : ces opérations relèvent d'autres actions orchestrées par le UseCase.</description></item>
    /// <item><description>Ne lit ni ne vérifie la barre de production : l'intégrité référentielle entre la découpe et la barre est garantie par la persistance déclenchée par l'appelant.</description></item>
    /// <item><description>Ne choisit pas les découpes et ne calcule pas leur ordre : l'un et l'autre sont repris tels que déterminés par le moteur d'optimisation.</description></item>
    /// <item><description>Ne traite pas les découpes non retenues par la recomposition, ni l'issue dans laquelle aucune découpe n'est placée.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie, ne journalise ni ne notifie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionCutPiece_AssignToBar"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.DTOs.Business.DTO_CuttingOptimizationResult.PieceIds"/>
    public interface IS_ProductionCutPiece_AssignAndSealToBar
    {
        // --- Groupe 1 : Rattachement définitif des découpes à une barre de production ---

        /// <summary>
        /// Rattache à une barre de production validée avec défauts les découpes du plan recomposé,
        /// en leur attribuant leur position de coupe selon leur rang dans la liste reçue et en les
        /// scellant directement, puis confie leur mise à jour à l'écriture générique en une seule
        /// opération, sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, après la validation de la barre avec ses défauts, le détachement du plan
        /// provisoire et la recomposition du plan, et avant l'inscription du plan recomposé sur la
        /// barre. Les découpes sont chargées en une lecture unique avec suivi des changements, puis
        /// leur mise à jour est déléguée au Command Handler générique
        /// <c>IC_Generic&lt;ProductionCutPiece&gt;</c>, qui positionne la date de mise à jour et
        /// inscrit un événement par découpe ; les découpes restent suivies dans le contexte partagé
        /// et ne sont enregistrées qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier les préconditions structurelles des arguments : liste renseignée et non vide, identifiant de barre strictement positif, identifiants de découpes strictement positifs et sans doublon.</description></item>
        /// <item><description>Vérifier que chaque découpe désignée existe et que son état permet le rattachement : rattachée à aucune barre, y compris la barre désignée, non réalisée, non supprimée logiquement.</description></item>
        /// <item><description>Attribuer à chaque découpe, comme position de coupe, son rang dans la liste reçue compté à partir de 1, l'appariement entre identifiant et découpe chargée se faisant par identifiant et jamais par rang dans la liste chargée.</description></item>
        /// <item><description>Rattacher chaque découpe à la barre, la marquer comme définitivement placée et non provisoirement placée, lever son marqueur de refus et la marquer comme portée par une barre approvisionnée ; aucun autre champ n'est modifié.</description></item>
        /// <item><description>Déléguer la mise à jour de l'ensemble des découpes au Command Handler générique, en une seule opération.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni l'indicateur de réalisation, ni les dates de début et de fin de coupe, ni l'indicateur de rupture d'approvisionnement, ni l'indicateur de suppression logique, ni les champs d'audit.</description></item>
        /// <item><description>Ne contrôle ni l'indicateur d'optimisation scellée, ni l'indicateur de placement provisoire, ni l'indicateur de rupture d'approvisionnement : le vivier d'optimisation exclut déjà ces états.</description></item>
        /// <item><description>Ne trie ni ne réordonne jamais la liste reçue, y compris à titre interne.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne lit pas la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="pieceIds">
        /// Identifiants des découpes retenues par la recomposition, dans l'ordre de coupe : le rang
        /// de chaque identifiant, compté à partir de 1, devient la position de coupe de la découpe
        /// correspondante. Ne doit pas être <see langword="null"/> ni vide ; chaque identifiant doit
        /// être strictement positif et n'apparaître qu'une seule fois.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production validée avec défauts. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="pieceIds"/> est
        /// <see langword="null"/> ; avec le code <c>BU_ER_02</c> si <paramref name="pieceIds"/> est
        /// vide, si <paramref name="idProductionBar"/> n'est pas strictement positif (valeur reçue
        /// citée) ou si des identifiants de découpes ne sont pas strictement positifs (valeurs
        /// fautives citées) ; avec le code <c>BU_ER_03</c> si des identifiants figurent plusieurs
        /// fois dans la liste, une même découpe ne pouvant occuper deux positions de coupe
        /// (identifiants dupliqués cités), ou si des découpes désignées sont introuvables
        /// (identifiants cités) ; avec le code <c>BU_ER_04</c>, en un échec unique, si des découpes
        /// sont déjà rattachées à une barre quelle qu'elle soit, déjà réalisées ou supprimées
        /// logiquement (chaque découpe et chaque condition violée citées). Remonte également sans
        /// interception toute <see cref="Ex_Business"/> levée par le Query Handler ou le Command Handler.
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