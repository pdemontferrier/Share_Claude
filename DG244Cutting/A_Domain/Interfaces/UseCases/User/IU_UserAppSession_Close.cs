using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase de fermeture (déconnexion) d'une session applicative utilisateur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le pipeline de fermeture de l'application console, qui en délègue l'exécution
    /// au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAppSession_Close"/>
    /// résidant en <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de session / authentification). Il est le pendant symétrique de
    /// <see cref="IU_UserAppSession_Open"/>.
    /// </para>
    /// <para>
    /// Objectif : orchestrer le scénario complet de fermeture de session — chargement de
    /// la session cible, contrôle d'appartenance utilisateur, passage à l'état déconnecté —
    /// sous une transaction unique et un traitement terminal des erreurs conforme à la
    /// section 4.7.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario de fermeture de session.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine de mutation : celle-ci est déléguée au Service spécialisé.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de A_Domain.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAppSession_Close
    {
        // --- Groupe 1 : Fermeture de session ---

        /// <summary>
        /// Ferme la session applicative identifiée de l'utilisateur courant en la passant
        /// à l'état déconnecté, après contrôle d'appartenance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté à la fermeture du programme console. L'identité de
        /// l'utilisateur courant est lue depuis le contexte applicatif courant. Le UseCase
        /// ouvre la transaction, charge la session via le Query Handler, contrôle son
        /// appartenance à l'utilisateur courant, délègue la mise à jour à l'état
        /// déconnecté au Service spécialisé, puis valide la transaction. L'absence de
        /// session correspondante est un cas non bloquant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Charger la session cible par identifiant.</description></item>
        /// <item><description>Contrôler l'appartenance de la session à l'utilisateur courant.</description></item>
        /// <item><description>Déléguer la mise à jour à l'état déconnecté au Service spécialisé.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="sessionId">Identifiant de la session à fermer. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant utilisateur lu du contexte applicatif courant ou
        /// <paramref name="sessionId"/> n'est pas strictement positif, ou si la session
        /// chargée n'appartient pas à l'utilisateur courant.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors de la lecture ou de l'écriture.</exception>
        Task ExecuteAsync(string caller, int sessionId, CancellationToken ct = default);
    }
}