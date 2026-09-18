using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier d'inscription, sur une série de production, de son premier
    /// approvisionnement en barres neuves ou en barres de chute.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par le UseCase orchestrateur de la validation d'une
    /// barre de production, qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionSeries_SetSupplyFlag"/>
    /// résidant en <c>B_UseCases/Services/Business</c>. Le service est appelé à chaque validation de
    /// barre, à l'intérieur de la transaction ouverte par son appelant, sur le contexte de données
    /// partagé.
    /// </para>
    /// <para>
    /// Objectif : l'atelier est approvisionné à la demande, chaque barre étant approvisionnée juste
    /// avant d'être coupée. Le tableau de bord des séries considère qu'une série est en cours dès
    /// qu'elle a fait l'objet d'un premier engagement de matière. L'acceptation par l'opérateur d'une
    /// barre, neuve ou de chute prélevée au stock, constitue cet engagement. Le contrat inscrit ce
    /// fait sur la série au moyen de l'un de ses deux indicateurs d'approvisionnement, choisi selon
    /// l'origine de la barre acceptée : <c>IsNewBarSupplied</c> pour une barre neuve,
    /// <c>IsDropBarSupplied</c> pour une barre de chute.
    /// </para>
    /// <para>
    /// Règles d'inscription : un seul indicateur est visé par appel, et il n'est écrit que s'il
    /// change. Un indicateur déjà posé n'est pas réécrit, ce qui constitue le cas nominal dès la
    /// deuxième barre d'une même origine : une série consommant de nombreuses barres de même origine
    /// ne voit son indicateur écrit qu'une fois. Les deux indicateurs sont indépendants et
    /// cumulatifs : une série consommant des barres neuves et des barres de chute porte les deux, et
    /// poser l'un ne modifie jamais l'autre. Un indicateur posé n'est jamais remis à
    /// <see langword="false"/> : l'approvisionnement est un fait acquis, qu'un refus ultérieur de
    /// barre ne défait pas.
    /// </para>
    /// <para>
    /// L'inscription est portée par un service, et non par un UseCase, afin d'être exécutée dans la
    /// transaction de l'appelant : un UseCase ouvre sa propre transaction et ne peut être invoqué
    /// à l'intérieur de celle d'un autre UseCase. L'appelant conserve ainsi un enregistrement unique
    /// de l'ensemble de ses modifications.
    /// </para>
    /// <para>
    /// Origine de la barre : l'origine est reçue de l'appelant et n'est jamais déduite. Le contrat
    /// ne porte sur aucune barre de production ; il ne lit ni ne modifie aucune d'entre elles.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire d'inscription du premier approvisionnement d'une série de production désignée, pour une origine de barre donnée.</description></item>
    /// <item><description>Garantir l'écriture unique et conditionnelle de l'indicateur visé : un seul champ par appel, et seulement s'il change.</description></item>
    /// <item><description>Garantir l'indépendance des deux indicateurs d'approvisionnement et l'absence de tout retour à <see langword="false"/>.</description></item>
    /// <item><description>Garantir que l'indicateur visé est la seule modification portée sur la série.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni orchestration, ni transaction, ni persistance : l'enregistrement effectif relève du UseCase appelant.</description></item>
    /// <item><description>Ne détermine pas l'origine de la barre et ne lit ni ne modifie aucune barre de production.</description></item>
    /// <item><description>Ne modifie ni l'indicateur de début de découpe, ni l'indicateur de rupture de stock, ni l'indicateur de fin de découpe, ni les dates de production de la série.</description></item>
    /// <item><description>Ne restitue aucune indication de changement à l'appelant.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Entities.DIGIT_TRY.ProductionSeries"/>
    public interface IS_ProductionSeries_SetSupplyFlag
    {
        // --- Groupe 1 : Inscription du premier approvisionnement d'une série ---

        /// <summary>
        /// Inscrit sur la série de production désignée son premier approvisionnement, en barres
        /// neuves ou en barres de chute, par écriture unique et conditionnelle de l'indicateur
        /// correspondant, confiée à l'écriture générique, sans persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur à chaque validation de barre, à
        /// l'intérieur de la transaction qu'il a ouverte. La série est lue avec suivi des
        /// changements ; si elle est déjà suivie par le contexte partagé, c'est cette instance qui est
        /// restituée avec ses valeurs courantes, modifications non enregistrées comprises, et l'état
        /// de l'indicateur visé est évalué sur ces valeurs.
        /// </para>
        /// <para>
        /// Contrôles : l'existence de la série, son absence de suppression logique et son absence de
        /// clôture sont vérifiées à chaque appel, y compris lorsque l'indicateur visé est déjà posé et
        /// qu'aucune écriture ne suit. Un échec survenu avant la modification laisse la série intacte.
        /// </para>
        /// <para>
        /// Écriture conditionnelle : la série n'est confiée au Command Handler générique
        /// <c>IC_Generic&lt;ProductionSeries&gt;</c> que si l'indicateur visé passe de
        /// <see langword="false"/> à <see langword="true"/>. Un appel sur un indicateur déjà posé ne
        /// laisse aucune trace : ni date de mise à jour, ni événement technique. Lorsque l'indicateur
        /// est posé, le Command Handler positionne la date de mise à jour et inscrit un événement
        /// technique ; la série reste suivie dans le contexte partagé et n'est enregistrée qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : faire apparaître la série comme en cours dès son premier engagement de matière.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier la précondition structurelle de l'argument : identifiant de série strictement positif.</description></item>
        /// <item><description>Vérifier que la série désignée existe, n'est pas supprimée logiquement et n'est pas clôturée.</description></item>
        /// <item><description>Sélectionner l'indicateur visé selon l'origine reçue : <c>IsNewBarSupplied</c> pour une barre neuve, <c>IsDropBarSupplied</c> pour une barre de chute.</description></item>
        /// <item><description>Positionner l'indicateur visé à <see langword="true"/> et déléguer la mise à jour au Command Handler générique, uniquement s'il ne l'était pas déjà.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ de la série, en particulier ni l'autre indicateur d'approvisionnement, ni les indicateurs de début de découpe, de rupture de stock et de fin de découpe, ni les dates de production, ni les champs d'audit.</description></item>
        /// <item><description>N'écrit jamais <see langword="false"/> sur un indicateur d'approvisionnement.</description></item>
        /// <item><description>Ne lève aucune exception lorsque l'indicateur visé est déjà posé.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement puis propagée à l'aval.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionSeries">
        /// Identifiant de la série de production dont le premier approvisionnement est inscrit.
        /// Doit être strictement positif.
        /// </param>
        /// <param name="isNewBar">
        /// Origine de la barre acceptée : <see langword="true"/> pour une barre neuve,
        /// <see langword="false"/> pour une barre de chute prélevée au stock.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif ; avec le code <c>BU_ER_03</c> si la série désignée est introuvable ;
        /// avec le code <c>BU_ER_04</c> si la série désignée est supprimée logiquement ou clôturée.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si une défaillance technique survient lors de la lecture de la série ou lors de la délégation de sa mise à jour.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(
            string caller,
            int idProductionSeries,
            bool isNewBar,
            CancellationToken ct = default);
    }
}