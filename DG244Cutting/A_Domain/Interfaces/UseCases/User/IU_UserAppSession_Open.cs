using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase d'ouverture (ou réactivation) d'une session applicative utilisateur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le pipeline de démarrage de l'application console, qui en délègue l'exécution
    /// au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAppSession_Open"/>
    /// résidant en <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de session / authentification). Il est le pendant symétrique de
    /// <see cref="IU_UserAppSession_Close"/>.
    /// </para>
    /// <para>
    /// Objectif : orchestrer le scénario complet d'ouverture de session — recherche des
    /// sessions existantes, activation de la session principale, suppression des sessions
    /// supplémentaires ou création si aucune n'existe, puis propagation de l'identifiant
    /// de session dans le contexte utilisateur global — sous une transaction unique et un
    /// traitement terminal des erreurs conforme à la section 4.7.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario d'ouverture de session.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// <item><description>Exposer un retour signalable booléen permettant au UseCase orchestrant amont (le cas échéant) de constater le succès ou l'échec du sous-scénario sans propagation d'exception, conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine : celle-ci est déléguée aux Services spécialisés.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de A_Domain.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAppSession_Open
    {
        // --- Groupe 1 : Ouverture de session ---

        /// <summary>
        /// Ouvre ou réactive la session applicative de l'utilisateur courant pour
        /// l'application courante, en garantissant l'unicité de la session active.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté au démarrage du programme console, après identification de
        /// l'utilisateur et de l'application. L'identité de l'utilisateur et de
        /// l'application est lue depuis le contexte applicatif courant. Le UseCase ouvre
        /// la transaction, orchestre la lecture et les mutations via les Services
        /// spécialisés, propage l'identifiant de session retenu dans le contexte
        /// utilisateur, puis valide la transaction.
        /// </para>
        /// <para>
        /// Retour : <see langword="true"/> si l'ouverture de session a abouti (transaction
        /// validée et identifiant de session propagé) ; <see langword="false"/> si une
        /// exception applicative typée (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) a été captée et traitée terminalement par
        /// <c>IU_LogAndNotify</c>. Le retour booléen permet à un éventuel UseCase orchestrant
        /// amont de constater l'issue du sous-scénario, conformément à la clause de chaîne
        /// d'appel UseCase → UseCase de §4.14.2. L'annulation coopérative
        /// (<see cref="OperationCanceledException"/>) n'est pas signalée par ce retour : elle
        /// est propagée à l'appelant selon le mécanisme normatif de §4.6.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Rechercher les sessions existantes du couple (utilisateur, application).</description></item>
        /// <item><description>Activer la session principale et supprimer les sessions supplémentaires, ou créer une session si aucune n'existe.</description></item>
        /// <item><description>Propager l'identifiant de session retenu dans le contexte utilisateur.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si l'ouverture de session a abouti ; <see langword="false"/>
        /// si une exception applicative typée a été captée et traitée terminalement.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// Les exceptions applicatives typées (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles sont captées et signalées
        /// par le retour <see langword="false"/>.
        /// </exception>
        Task<bool> ExecuteAsync(string caller, CancellationToken ct = default);
    }
}