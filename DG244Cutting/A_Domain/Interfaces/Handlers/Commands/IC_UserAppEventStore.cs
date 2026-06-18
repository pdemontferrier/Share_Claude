using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Commands
{
    /// <summary>
    /// Contrat du Command Handler dédié à la persistance des enregistrements de l'Event Store applicatif.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et constitue le contrat d'écriture
    /// dans la table <c>UserAppEventStore</c>. Son implémentation concrète <c>CH_UserAppEventStore</c>
    /// réside dans B_UseCases/Handlers/Commands/.
    /// </para>
    /// <para>
    /// Positionnement dans la mécanique Event Store : cette interface est appelée exclusivement par
    /// <c>CH_Generic&lt;T&gt;</c>, via sa méthode privée de journalisation. Elle n'est jamais
    /// appelée directement depuis un Service métier, un UseCase ou un ViewModel.
    /// </para>
    /// <para>
    /// Séparation des paramètres : outre la CallChain, les paramètres <paramref name="handlerCommand"/>
    /// et <paramref name="commandMethod"/> sont maintenus comme champs distincts afin de permettre
    /// des requêtes SQL directes sur la table Event Store sans nécessiter d'analyse syntaxique de
    /// la CallChain (ex. : <c>WHERE AppHandlerCommand = 'CH_Order'</c>).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Déclarer le contrat d'écriture d'un enregistrement Event Store contextualisé.</description></item>
    ///   <item><description>Garantir la propagation de la CallChain depuis le Command Handler métier appelant.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne construit pas le contexte applicatif : ce rôle appartient à l'implémentation via <c>IQ_AppContext</c>.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas : ces responsabilités appartiennent au UseCase.</description></item>
    /// </list>
    /// </remarks>
    public interface IC_UserAppEventStore
    {
        /// <summary>
        /// Inscrit un enregistrement Event Store contextualisé dans le DbContext partagé.
        /// </summary>
        /// <remarks>
        /// L'enregistrement produit est automatiquement enrichi avec le contexte applicatif courant
        /// (identité utilisateur, poste, adresse IP, horodatage) via <c>IQ_AppContext</c> dans
        /// l'implémentation. L'écriture s'effectue dans la même transaction que la mutation métier
        /// qu'elle accompagne, garantissant la solidarité transactionnelle définie en section 3.8
        /// du référentiel normatif.
        /// </remarks>
        /// <param name="caller">
        /// CallChain complète construite jusqu'au Command Handler métier ayant déclenché la mutation.
        /// Cette valeur est stockée telle quelle dans le champ <c>AppCallChain</c> de l'entité,
        /// conformément à l'exemple du référentiel normatif (section 3.7).
        /// </param>
        /// <param name="tableDesignation">
        /// Nom de l'entité ou de la table métier concernée par la mutation tracée.
        /// Ne doit pas être <see langword="null"/> ni vide.
        /// </param>
        /// <param name="tableId">
        /// Identifiant de l'enregistrement métier modifié. La valeur <c>0</c> est acceptée
        /// lorsque l'identifiant n'est pas disponible (ex. : entité sans propriété <c>Id</c> entière).
        /// </param>
        /// <param name="data">
        /// Sérialisation JSON complète de l'état de l'entité au moment de la mutation.
        /// Ne doit pas être <see langword="null"/> ni vide.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="tableDesignation"/> ou <paramref name="data"/> est nul ou vide.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'inscription dans le change tracker.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task HandleAddAsync(
            string caller,
            string tableDesignation,
            int tableId,
            string data,
            CancellationToken ct = default);
    }
}