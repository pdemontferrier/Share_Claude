using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier d'inscription, sur une barre de production, de son acceptation
    /// physique par l'opérateur, avec ou sans signalement de zones défectueuses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la validation de
    /// barre, UC_BarValidation, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionBar_Validate"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier approvisionne chaque barre juste avant de la couper. L'application
    /// désigne à l'opérateur la barre à prendre, chute du stock ou barre neuve ; l'opérateur en
    /// constate l'état puis l'accepte, et ce geste engage la matière. Le contrat exprime
    /// l'inscription de cette acceptation sur la barre, sous l'une de deux formes gouvernées par un
    /// discriminant unique, la présence d'une première zone défectueuse.
    /// </para>
    /// <para>
    /// Sans défaut, le placement provisoire calculé par l'optimisation devient définitif à
    /// l'identique : la barre est validée et scellée en une seule écriture. Avec une ou deux zones
    /// défectueuses, repérées en millimètres depuis le début de la barre, la barre est validée et
    /// ses défauts enregistrés, mais elle n'est pas scellée : son plan provisoire n'est plus
    /// valable, elle devient la barre de référence de la ré-optimisation, et son scellement relève
    /// du service qui inscrira le plan recomposé. Le marqueur de placement provisoire
    /// <c>IsOptimizedTemp</c> demeure en toute hypothèse inchangé pour toute la vie de la barre.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de validation d'une barre de production désignée, avec ou sans zones défectueuses.</description></item>
    /// <item><description>Garantir que seule une barre ni validée, ni épuisée, ni en rupture de stock, ni refusée ou supprimée logiquement peut être validée.</description></item>
    /// <item><description>Garantir que les zones défectueuses signalées sont complètes, ordonnées, non chevauchantes et comprises dans la longueur de la barre.</description></item>
    /// <item><description>Garantir que seuls l'indicateur de validation et, selon le cas, l'indicateur de scellement ou les bornes des zones défectueuses sont modifiés.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne recompose pas le plan de coupe d'une barre à défauts et ne la scelle pas : ces rôles relèvent de la ré-optimisation et du service qui inscrira le plan recomposé.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie : la traçabilité de la validation relève d'un service dédié appelé par le UseCase.</description></item>
    /// <item><description>Ne consomme pas la chute source éventuelle de la barre et ne traite pas le reliquat.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.ProductionBar"/>
    public interface IS_ProductionBar_Validate
    {
        // --- Groupe 1 : Validation d'une barre de production ---

        /// <summary>
        /// Inscrit sur la barre de production désignée l'acceptation physique de l'opérateur, avec
        /// ou sans signalement de zones défectueuses, puis confie sa mise à jour à l'écriture
        /// générique sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte. La barre est lue avec suivi des changements, puis sa mise à jour est déléguée au
        /// Command Handler générique <c>IC_Generic&lt;ProductionBar&gt;</c>, qui positionne la date
        /// de mise à jour et inscrit un événement technique ; la barre reste suivie dans le contexte
        /// partagé et n'est enregistrée qu'à la validation de la transaction par l'appelant. Un
        /// échec survenant avant la modification laisse la barre intacte.
        /// </para>
        /// <para>
        /// Objectif : faire passer la barre à l'état validé ; sans défaut, sceller à l'identique son
        /// plan provisoire ; avec défauts, enregistrer les zones signalées et laisser la barre non
        /// scellée en attente de recomposition.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier les préconditions structurelles des arguments : identifiant de barre strictement positif ; zones défectueuses complètes, la seconde n'étant admise qu'avec la première ; bornes positives ou nulles ; bornes ordonnées selon début 1 &lt; fin 1 &lt;= début 2 &lt; fin 2, deux zones contiguës étant admises.</description></item>
        /// <item><description>Vérifier que la barre désignée existe et que son état permet la validation : ni déjà validée, ni épuisée, ni en rupture de stock, ni refusée ou supprimée logiquement.</description></item>
        /// <item><description>Sur le chemin avec défauts, vérifier que chaque borne renseignée n'excède pas la longueur de la barre.</description></item>
        /// <item><description>Sans défaut : positionner l'indicateur de validation et l'indicateur de scellement.</description></item>
        /// <item><description>Avec défauts : positionner l'indicateur de validation et inscrire les bornes des zones reçues, la seconde zone pouvant rester vide ; l'indicateur de scellement est laissé inchangé.</description></item>
        /// <item><description>Déléguer la mise à jour de la barre au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni le marqueur de placement provisoire, ni les champs de découpe et de reliquat, ni les indicateurs d'utilisation, de rupture de stock et de suppression logique, ni le motif de refus, ni les rattachements, la longueur, l'origine et la date de création de la barre ; sans défaut, les bornes de zones défectueuses ne sont pas modifiées.</description></item>
        /// <item><description>Ne contrôle ni l'indicateur de scellement ni le marqueur de placement provisoire.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">Identifiant de la barre de production acceptée par l'opérateur. Doit être strictement positif.</param>
        /// <param name="defectStart1">
        /// Début de la première zone défectueuse, en millimètres depuis le début de la barre, ou
        /// <see langword="null"/> en l'absence de défaut. Renseigné conjointement avec
        /// <paramref name="defectEnd1"/> ; sa présence discrimine la validation avec défauts de la
        /// validation sans défaut.
        /// </param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, en millimètres ; renseignée conjointement avec <paramref name="defectStart1"/>.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, en millimètres ; facultatif, renseigné conjointement avec <paramref name="defectEnd2"/> et seulement si la première zone l'est.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, en millimètres ; renseignée conjointement avec <paramref name="defectStart2"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif, si une borne renseignée est négative, ou, sur le chemin avec
        /// défauts, si une borne renseignée excède la longueur de la barre ; avec le code
        /// <c>BU_ER_03</c> si une zone est incomplète, si la seconde zone est renseignée sans la
        /// première, si les bornes sont désordonnées ou chevauchantes, ou si la barre désignée est
        /// introuvable ; avec le code <c>BU_ER_04</c>, en un échec unique citant chaque condition
        /// violée, si la barre est déjà validée, épuisée, en rupture de stock, ou refusée ou
        /// supprimée logiquement.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la barre ou de la délégation de sa mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionBar,
            int? defectStart1,
            int? defectEnd1,
            int? defectStart2,
            int? defectEnd2,
            CancellationToken ct = default);
    }
}