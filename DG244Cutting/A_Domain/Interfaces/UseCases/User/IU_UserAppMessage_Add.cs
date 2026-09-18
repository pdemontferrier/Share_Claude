using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase d'ajout d'un message applicatif à destination d'une application
    /// identifiée par son <c>AppList.Id</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par un ViewModel orchestrant la composition d'un message applicatif (chaîne (1)
    /// directe VM → UC), qui en délègue l'exécution au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAppMessage_Add"/>
    /// résidant en <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de messages applicatifs inter-applications).
    /// </para>
    /// <para>
    /// Objectif : orchestrer le scénario unitaire d'ajout d'un enregistrement
    /// <c>UserAppMessage</c>. Le UseCase lit en premier lieu le contexte applicatif
    /// courant via <c>IS_AppContext.GetAppContext()</c>, ouvre la transaction
    /// (encapsulée dans une stratégie d'exécution en présence d'<c>EnableRetryOnFailure</c>),
    /// délègue l'action métier unitaire au Service <c>IS_UserAppMessage_Add</c> en lui
    /// transmettant le <c>DTO_AppContext</c> obtenu et les trois arguments métier portant
    /// le contenu propre du message (application destinataire, sujet, contenu textuel),
    /// appelle <c>SaveChangesAsync</c> pour persister la mutation et l'enregistrement
    /// Event Store solidairement, puis valide la transaction. Toute exception applicative
    /// typée (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
    /// <see cref="Ex_Unclassified"/>) remontée par le Service est captée terminalement et
    /// traitée par <c>IU_LogAndNotify</c> avec les clés <c>No_EC_01</c> / <c>No_EC_02</c> /
    /// <c>No_EC_03</c> respectivement, sans propagation à l'appelant.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario d'ajout d'un message applicatif.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine : elle est déléguée au Service spécialisé en aval.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// <item><description>Ne reçoit pas le <c>DTO_AppContext</c> en paramètre : le contexte applicatif est lu en interne via <c>IS_AppContext</c> au début du scénario, à l'intérieur du délégué de la stratégie d'exécution, garantissant la rejouabilité atomique en cas de réexécution.</description></item>
    /// <item><description>N'expose pas de retour signalable : le UseCase est consommé en chaîne (1) directe par un ViewModel sans sous-séquence amont prévue (signature <see cref="Task"/> simple, R-4.14.21 sans objet).</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAppMessage_Add
    {
        // --- Groupe 1 : Ajout d'un message applicatif ---

        /// <summary>
        /// Orchestre l'ajout d'un nouvel enregistrement <c>UserAppMessage</c> à destination
        /// de l'application désignée par <paramref name="idApplicationRecipient"/>, avec
        /// le sujet et le contenu fournis.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté typiquement par un ViewModel de page de composition de
        /// message applicatif. Le UseCase ouvre la transaction sous stratégie d'exécution,
        /// lit le contexte applicatif courant via <c>IS_AppContext.GetAppContext()</c> à
        /// l'intérieur du délégué, délègue la mutation au Service métier
        /// <c>IS_UserAppMessage_Add</c> en lui transmettant le <c>DTO_AppContext</c>
        /// obtenu et les trois arguments métier, persiste via <c>SaveChangesAsync</c> et
        /// valide la transaction. Toute exception applicative typée est traitée
        /// terminalement et n'est pas propagée à l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
        /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution (R-4.10.2 amendée).</description></item>
        /// <item><description>Résoudre le contexte applicatif courant via <c>IS_AppContext</c> à l'intérieur du délégué d'exécution.</description></item>
        /// <item><description>Déléguer l'action métier au Service <c>IS_UserAppMessage_Add</c>.</description></item>
        /// <item><description>Déléguer le traitement terminal des erreurs à <c>IU_LogAndNotify</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idApplicationRecipient">
        /// Identifiant de l'application destinataire du message (référence
        /// <c>AppList(Id)</c>). Doit être strictement positif. La validation structurelle
        /// est portée par le Service métier en aval.
        /// </param>
        /// <param name="subject">
        /// Sujet du message. Ne doit être ni <see langword="null"/>, ni vide, ni constitué
        /// exclusivement d'espaces. La validation structurelle est portée par le Service
        /// métier en aval.
        /// </param>
        /// <param name="content">
        /// Contenu textuel du message. Peut être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du scénario.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// Les exceptions applicatives typées (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) ne sont jamais
        /// propagées : elles sont captées et traitées terminalement par
        /// <c>IU_LogAndNotify</c> (clés <c>No_EC_01</c> / <c>No_EC_02</c> / <c>No_EC_03</c>).
        /// </exception>
        Task ExecuteAsync(
            string caller,
            int idApplicationRecipient,
            string subject,
            string? content,
            CancellationToken ct = default);
    }
}