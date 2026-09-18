using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier d'inscription, sur une barre de production validée avec
    /// défauts, du plan de coupe recomposé par le moteur d'optimisation, et de scellement
    /// de cette barre.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la validation
    /// de barre (<c>UC_BarValidation</c>), sur le seul chemin de validation avec défauts, qui
    /// en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionBar_UpdateOptimization"/>
    /// résidant en <c>B_UseCases/Services/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : une barre de production peut présenter jusqu'à deux zones défectueuses, que
    /// l'opérateur mesure et déclare au moment de la valider. Ces zones consomment de la
    /// matière et rendent irréalisable le plan de coupe provisoire établi avant leur
    /// déclaration. Ce plan est alors défait, puis recomposé sur la matière réellement
    /// exploitable par <see cref="IS_CuttingOptimizer.OptimizeWithDefects"/>. Le contrat
    /// exprime le besoin d'inscrire sur la barre le résultat de cette recomposition et de la
    /// sceller : le plan est dès lors établi, il n'est plus remis en cause, et la découpe peut
    /// commencer. Il s'agit de la dernière écriture portée sur la barre dans le chemin de
    /// validation avec défauts.
    /// </para>
    /// <para>
    /// L'opération réunit deux natures d'information indissociables : la description du plan
    /// recomposé (nombre de découpes placées, longueur du reste de matière, qualification de ce
    /// reste en chute réutilisable ou en déchet) et le scellement de la barre. Leur réunion est
    /// délibérée : les dissocier ferait apparaître un état transitoire, dans lequel la barre
    /// porterait un plan recomposé sans être scellée, qu'aucun traitement ne sait interpréter.
    /// </para>
    /// <para>
    /// Sur le chemin de validation sans défaut, le plan provisoire devient définitif à
    /// l'identique et la barre est scellée par le service de validation, sans recours au
    /// présent contrat. Lorsque la recomposition ne place aucune découpe, le présent contrat
    /// n'est pas sollicité : la barre relève alors du service de refus
    /// (<c>IS_ProductionBar_Reject</c>).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire d'inscription du plan recomposé et de scellement d'une barre de production désignée par son identifiant.</description></item>
    /// <item><description>Garantir le caractère indissociable de cette écriture : plan recomposé et scellement sont confiés ensemble à l'écriture générique, ou ne le sont pas.</description></item>
    /// <item><description>Garantir le respect de la séquence de validation, en refusant toute barre dont l'état ne correspond pas à une barre validée et non encore scellée.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne porte aucune règle de calcul : le nombre de découpes, la longueur du reste et sa qualification sont repris tels que déterminés par le moteur d'optimisation, sans recalcul ni contrôle de bornes.</description></item>
    /// <item><description>Ne valide pas la barre et n'enregistre pas ses défauts : ces informations sont posées préalablement par le service de validation (<c>IS_ProductionBar_Validate</c>) et constituent l'entrée du calcul, non sa sortie.</description></item>
    /// <item><description>Ne détache ni ne rattache aucune découpe : ces opérations relèvent d'autres actions orchestrées par le UseCase (<c>IS_ProductionCutPiece_DetachFromBar</c>, <c>IS_ProductionCutPiece_AssignAndSealToBar</c>).</description></item>
    /// <item><description>Ne qualifie ni ne range le reliquat de la barre : sa validation, son emplacement et son code-barre relèvent de la fin de barre.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie et ne notifie pas : la traçabilité métier relève d'un service dédié.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DTO_CuttingOptimizationResult"/>
    public interface IS_ProductionBar_UpdateOptimization
    {
        // --- Groupe 1 : Inscription du plan recomposé et scellement d'une barre ---

        /// <summary>
        /// Inscrit sur une barre de production validée le plan de coupe recomposé par le moteur
        /// d'optimisation, puis scelle la barre, en une écriture indissociable confiée à
        /// l'écriture générique sans la persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de la validation de barre, sur le
        /// chemin avec défauts, à l'intérieur de la transaction qu'il a ouverte, après la
        /// validation de la barre, le détachement du plan provisoire, la recomposition du plan et
        /// le rattachement des découpes du nouveau plan. La barre est lue avec suivi des
        /// changements ; la même instance est contrôlée, modifiée puis confiée au Command Handler
        /// générique <c>IC_Generic&lt;ProductionBar&gt;</c>, qui positionne la date de mise à
        /// jour et inscrit l'événement associé. L'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : établir définitivement le plan de coupe de la barre sur la matière
        /// réellement exploitable, afin que la découpe puisse commencer.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier les préconditions structurelles des arguments et la nature du résultat reçu : identifiant de barre strictement positif, résultat renseigné, issue de succès, liste des découpes renseignée et non vide.</description></item>
        /// <item><description>Vérifier que la barre désignée existe.</description></item>
        /// <item><description>Vérifier que la barre est validée, non encore scellée, et n'est ni supprimée logiquement, ni refusée, ni épuisée, ni en rupture de stock ; toute condition violée est citée dans un échec unique. Ce contrôle détecte toute inversion dans la séquence de validation.</description></item>
        /// <item><description>Vérifier que le résultat a été calculé pour cette barre : origine du contenant, chute source et longueur de barre identiques à celles de la barre ; tout écart est cité dans un échec unique.</description></item>
        /// <item><description>Reporter sur la barre le nombre de découpes (cardinal de la liste des découpes retenues) et la longueur du reste, toujours renseignée y compris lorsqu'elle vaut zéro.</description></item>
        /// <item><description>Recopier sans transformation la qualification du reste, dont la polarité est : <see langword="true"/> pour une chute réutilisable, <see langword="false"/> pour un déchet ; toute inversion conduirait à jeter des chutes et à ranger des déchets en stock.</description></item>
        /// <item><description>Sceller la barre, seul indicateur d'état positionné.</description></item>
        /// <item><description>Déléguer l'écriture de la barre modifiée au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie ni l'état de validation, ni les zones défectueuses, ni le marqueur de placement provisoire, qui demeure positionné pendant toute la vie de la barre.</description></item>
        /// <item><description>Ne modifie ni l'origine du contenant, ni la chute source, ni la longueur de barre, qui ne sont lues qu'à des fins de contrôle de cohérence.</description></item>
        /// <item><description>Ne modifie ni l'état d'utilisation, ni le reliquat validé, ni l'emplacement ou le code-barre de chute, ni le motif de refus, ni l'état de suppression logique, ni aucun autre champ de la barre.</description></item>
        /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne relit pas l'entité après écriture.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production dont le plan recomposé est inscrit puis scellé.
        /// Doit être strictement positif.
        /// </param>
        /// <param name="result">
        /// Résultat de la recomposition du plan de coupe par le moteur d'optimisation. Ne doit
        /// pas être <see langword="null"/> ; son issue doit être
        /// <see cref="En_CuttingOptimizationOutcome.Success"/>, sa liste d'identifiants de
        /// découpes doit être renseignée et non vide, et son contenant (origine, chute source,
        /// longueur) doit être celui de la barre désignée.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Tâche représentant l'opération asynchrone, sans valeur de retour. L'aboutissement se
        /// constate par l'absence d'exception ; la barre modifiée est alors suivie par le
        /// contexte partagé et enregistrée à la validation de la transaction par l'appelant.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="result"/> ou sa liste
        /// d'identifiants de découpes est <see langword="null"/> ; avec le code <c>BU_ER_02</c>
        /// si <paramref name="idProductionBar"/> n'est pas strictement positif ou si la liste des
        /// découpes est vide ; avec le code <c>BU_ER_03</c> si l'issue du résultat n'est pas un
        /// succès, si la barre désignée est introuvable, ou si l'origine du contenant, la chute
        /// source ou la longueur de barre du résultat diffère de celle de la barre (tous les
        /// écarts étant cités en un échec unique) ; avec le code <c>BU_ER_04</c>, en un échec
        /// unique citant toutes les conditions violées, si la barre est supprimée logiquement,
        /// porte un motif de refus, n'est pas validée, est déjà scellée, est épuisée ou est en
        /// rupture de stock.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la barre ou de la délégation de sa mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionBar,
            DTO_CuttingOptimizationResult result,
            CancellationToken ct = default);
    }
}