using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase d'initialisation systématique des droits de pages au moindre
    /// privilège et d'application conditionnelle des droits de l'utilisateur courant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à la première
    /// obligation contractuelle de §4.14.2 amendée du 0230 (placement des contrats). Elle
    /// est consommée par injection de dépendances par un orchestrateur amont relevant du
    /// cycle de session - typiquement la séquence de démarrage applicatif au sens de §3.10
    /// du 0230 (orchestrateur prévu : <c>UC_Application_OnStart</c>), ainsi que par le
    /// pipeline d'authentification ou de réinitialisation du contexte utilisateur - qui en
    /// délègue l'exécution au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAppPageRight_Apply"/>
    /// résidant en <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de session / authentification).
    /// </para>
    /// <para>
    /// Objectif : orchestrer la constitution de l'état des droits de pages dans le contexte
    /// utilisateur partagé selon une logique en deux temps - initialisation systématique au
    /// moindre privilège de l'ensemble des pages applicatives connues indépendamment de la
    /// présence d'un utilisateur identifié dans le contexte applicatif, puis application
    /// conditionnelle des droits spécifiques de l'utilisateur courant chargés via le Query
    /// Handler lorsqu'un utilisateur est effectivement identifié. Le traitement terminal
    /// des erreurs est conforme à la section 4.7 du 0230. Le scénario est en lecture seule
    /// côté base de données ; son seul effet est l'écriture du contexte utilisateur
    /// partagé, qui ne constitue pas une mutation persistée et n'ouvre donc pas de
    /// transaction (§4.10).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario d'initialisation des droits de pages au moindre privilège et d'application conditionnelle des droits utilisateur.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// <item><description>Exposer un retour signalable booléen permettant au UseCase orchestrant amont de constater le succès ou l'échec du sous-scénario sans propagation d'exception, conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2 du 0230 indexée par R-4.14.21.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne décide pas du moment d'appel : celui-ci est fixé par l'orchestrateur amont (séquence de démarrage applicatif, pipeline d'authentification ou réinitialisation du contexte).</description></item>
    /// <item><description>Ne décide pas de la présence ou non d'un utilisateur identifié : la conditionnalité d'application des droits utilisateur est portée par la lecture du contexte applicatif courant.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de A_Domain.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAppPageRight_Apply
    {
        // --- Groupe 1 : Application des droits de pages ---

        /// <summary>
        /// Initialise systématiquement les droits de pages au moindre privilège dans le
        /// contexte utilisateur partagé, puis applique conditionnellement les droits
        /// spécifiques de l'utilisateur courant lorsqu'un utilisateur est identifié dans
        /// le contexte applicatif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : invocable au démarrage applicatif avant toute authentification, ainsi
        /// qu'après authentification ou lors d'une réinitialisation du contexte utilisateur.
        /// L'identité de l'application et, le cas échéant, de l'utilisateur sont lues depuis
        /// le contexte applicatif courant.
        /// </para>
        /// <para>
        /// Sémantique fonctionnelle en deux temps : (1) initialisation systématique au
        /// moindre privilège - le UseCase initialise les droits par défaut sur l'ensemble
        /// des pages applicatives connues, à l'exception des pages système exemptées du
        /// contrôle, indépendamment de la présence d'un utilisateur identifié dans le
        /// contexte applicatif ; (2) application conditionnelle des droits utilisateur -
        /// le UseCase ne procède au chargement et à l'application des droits spécifiques de
        /// l'utilisateur courant que lorsque le contexte applicatif fournit un identifiant
        /// utilisateur strictement positif (<c>AppUserId &gt; 0</c>). En l'absence d'un tel
        /// identifiant, le contexte utilisateur partagé est laissé en état initialisé au
        /// moindre privilège.
        /// </para>
        /// <para>
        /// Sémantique du retour : <see langword="true"/> couvre les deux variantes nominales
        /// du scénario - avec utilisateur identifié, l'initialisation par défaut a été
        /// effectuée puis les droits utilisateur ont été chargés et appliqués (le maintien
        /// des droits par défaut en cas de jeu vide retourné par le Query Handler est une
        /// variante admise de cette branche) ; sans utilisateur identifié, seule
        /// l'initialisation par défaut a été effectuée. <see langword="false"/> couvre les
        /// quatre cas d'échec applicatif capté terminalement par <c>IU_LogAndNotify</c>
        /// conformément à §4.7.4 du 0230 - précondition structurelle <c>AppId &lt;= 0</c>
        /// (<c>Ex_Business</c> code <c>BU_ER_02</c>), absence de page applicative non
        /// supprimée dans le référentiel <c>UserAppPage</c> empêchant l'initialisation par
        /// défaut (<c>Ex_Business</c> code <c>BU_ER_04</c>), défaillance d'infrastructure
        /// remontée par les Query Handlers, par le Service de contexte applicatif ou par
        /// les Settings consommés (<c>Ex_Infrastructure</c>), défaillance applicative non
        /// classifiée (<c>Ex_Unclassified</c>). Le retour booléen permet à un UseCase
        /// orchestrant amont de constater l'issue du sous-scénario, conformément à la
        /// clause de chaîne d'appel UseCase → UseCase de §4.14.2 amendée du 0230 indexée
        /// par R-4.14.21. L'annulation coopérative (<see cref="OperationCanceledException"/>)
        /// n'est pas signalée par ce retour : elle est propagée à l'appelant selon le
        /// mécanisme normatif de §4.6 du 0230.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle d'identifiant application strictement positif.</description></item>
        /// <item><description>Initialiser systématiquement les droits de pages par défaut au moindre privilège sur l'ensemble des pages applicatives connues, à l'exception des pages système exemptées du contrôle, indépendamment de la présence d'un utilisateur identifié.</description></item>
        /// <item><description>Charger conditionnellement les droits spécifiques de l'utilisateur via le Query Handler lorsqu'un identifiant utilisateur strictement positif est présent dans le contexte applicatif.</description></item>
        /// <item><description>Appliquer conditionnellement les droits chargés dans le contexte utilisateur partagé.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5 du 0230. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si le scénario a abouti à son objectif - avec utilisateur
        /// identifié, initialisation par défaut effectuée puis chargement et application
        /// des droits utilisateur ; sans utilisateur identifié, seule l'initialisation par
        /// défaut effectuée. <see langword="false"/> si une exception applicative typée
        /// (<see cref="Ex_Business"/> code <c>BU_ER_02</c> sur précondition
        /// <c>AppId &lt;= 0</c> ou code <c>BU_ER_04</c> sur référentiel <c>UserAppPage</c>
        /// vide, <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) a été
        /// captée et traitée terminalement par <c>IU_LogAndNotify</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément
        /// à §4.6 du 0230. Les exceptions applicatives typées (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) ne sont jamais
        /// propagées : elles sont captées et signalées par le retour <see langword="false"/>.
        /// </exception>
        Task<bool> ExecuteAsync(string caller, CancellationToken ct = default);
    }
}