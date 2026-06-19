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
    /// <item><description>Exposer un retour signalable booléen permettant au UseCase orchestrant amont (le cas échéant) de constater le succès ou l'échec du sous-scénario sans propagation d'exception, conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2.</description></item>
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
        /// <para>
        /// Retour : <see langword="true"/> si la fermeture de session a abouti (transaction
        /// validée — soit après passage effectif à l'état déconnecté, soit après constat
        /// non bloquant d'absence de session correspondante) ; <see langword="false"/> si
        /// une exception applicative typée (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) a été captée et
        /// traitée terminalement par <c>IU_LogAndNotify</c>. Le retour booléen permet à un
        /// éventuel UseCase orchestrant amont de constater l'issue du sous-scénario,
        /// conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2.
        /// L'annulation coopérative (<see cref="OperationCanceledException"/>) n'est pas
        /// signalée par ce retour : elle est propagée à l'appelant selon le mécanisme
        /// normatif de §4.6.
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
        /// <returns>
        /// <see langword="true"/> si la fermeture de session a abouti ; <see langword="false"/>
        /// si une exception applicative typée a été captée et traitée terminalement.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// Les exceptions applicatives typées (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles sont captées et signalées
        /// par le retour <see langword="false"/>.
        /// </exception>
        Task<bool> ExecuteAsync(string caller, int sessionId, CancellationToken ct = default);
    }
}