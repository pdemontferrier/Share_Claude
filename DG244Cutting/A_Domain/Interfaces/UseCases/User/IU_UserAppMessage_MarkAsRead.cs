using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase de marquage comme lu d'un message applicatif existant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.2 amendée, 1ère obligation contractuelle). Elle est
    /// consommée par injection de dépendances par un ViewModel en chaîne (1) directe -
    /// typiquement un VM de page de consultation des messages applicatifs - qui en
    /// délègue l'exécution au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAppMessage_MarkAsRead"/>
    /// résidant en <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (messagerie applicative interne, entité <c>UserAppMessage</c>).
    /// </para>
    /// <para>
    /// Objectif : orchestrer, sous une transaction unique et un traitement terminal des
    /// erreurs conforme à §4.7, le scénario de marquage comme lu d'un message applicatif
    /// identifié par son entier - lecture préalable de l'entité, positionnement de
    /// l'indicateur de lecture, persistance unifiée - sans exposer aux couches
    /// supérieures les détails de délégation aux Services et Handlers en aval.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario de marquage de lecture d'un message applicatif identifié.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine de chargement et de mutation : celle-ci est déléguée au Service spécialisé <c>IS_UserAppMessage_MarkAsRead</c>.</description></item>
    /// <item><description>Ne décrit ni la création, ni l'envoi, ni la suppression du message : ces actions relèvent de contrats distincts.</description></item>
    /// <item><description>N'expose aucun type technique de persistance (EF Core, DbContext, IQueryable), conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAppMessage_MarkAsRead
    {
        // --- Groupe 1 : Marquage de lecture ---

        /// <summary>
        /// Orchestre le marquage comme lu par son destinataire du message applicatif
        /// identifié par son entier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelé depuis un ViewModel en chaîne (1) directe après interaction
        /// utilisateur. Le UseCase ouvre la transaction, délègue l'action métier unitaire
        /// au Service <c>IS_UserAppMessage_MarkAsRead</c> (qui charge l'entité ciblée,
        /// positionne <c>IsRead</c> à <see langword="true"/> et délègue la mise à jour au
        /// Command Handler générique), persiste la mutation solidairement avec
        /// l'enregistrement Event Store via <c>SaveChangesAsync</c>, puis valide la
        /// transaction. Toute exception applicative typée remontée par le Service est
        /// traitée terminalement par <c>IU_LogAndNotify</c> et n'est jamais propagée à
        /// l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ouvrir la transaction d'écriture et la valider ou l'annuler selon l'issue.</description></item>
        /// <item><description>Déléguer l'action métier unitaire au Service <c>IS_UserAppMessage_MarkAsRead</c>.</description></item>
        /// <item><description>Persister la mutation et l'enregistrement Event Store via un appel unique à <c>SaveChangesAsync</c>.</description></item>
        /// <item><description>Déléguer le traitement terminal des erreurs à <c>IU_LogAndNotify</c>.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne réalise aucun accès EF Core métier (lecture ou écriture) ni aucune requalification d'exception : ces rôles relèvent du Service et des Handlers en aval.</description></item>
        /// <item><description>N'appelle jamais directement un Command Handler ni un Repository.</description></item>
        /// <item><description>Ne positionne aucun champ d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) : ce rôle est centralisé dans le Command Handler générique.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format
        /// normatif de §4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="messageId">
        /// Identifiant du message applicatif à marquer comme lu. Doit être strictement
        /// positif ; toute valeur non conforme entraîne une <see cref="Ex_Business"/>
        /// (code <c>BU_ER_02</c>) levée par le Service en aval, captée terminalement par
        /// le UseCase sans propagation à l'appelant.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée,
        /// conformément à §4.6. Les exceptions applicatives typées
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles sont captées
        /// dans les trois blocs <c>catch</c> typés du UseCase et traitées terminalement
        /// par <c>IU_LogAndNotify</c>.
        /// </exception>
        Task ExecuteAsync(string caller, int messageId, CancellationToken ct = default);
    }
}