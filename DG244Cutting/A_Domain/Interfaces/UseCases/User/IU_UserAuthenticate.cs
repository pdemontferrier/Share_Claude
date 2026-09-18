using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase d'authentification d'un utilisateur par identifiant de connexion et
    /// mot de passe, en repli de la page de connexion.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à la première obligation
    /// contractuelle de §4.14.2 amendée du 0230 (placement des contrats). Elle est consommée par
    /// injection de dépendances par le ViewModel de la page de connexion <c>VM_Page00</c>
    /// (chaîne (1) d'écriture VM → UC, médiée par <c>IS_UseCaseInvoker</c>), qui en délègue
    /// l'exécution au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAuthenticate"/> résidant en
    /// <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine <c>User</c> (cycle de
    /// session / authentification) et constitue le pendant, en repli manuel login/mot de passe,
    /// de l'identification automatique par contexte poste portée par
    /// <see cref="IU_UserIdentify"/>.
    /// </para>
    /// <para>
    /// Objectif : orchestrer le scénario d'authentification manuelle — validation du couple
    /// login/mot de passe, puis, en cas de succès, établissement de l'identité applicative de
    /// l'utilisateur, ouverture de sa session applicative et application de ses droits de pages —
    /// en consommant en sous-séquence les UseCases <see cref="IU_UserAppSession_Open"/> et
    /// <see cref="IU_UserAppPageRight_Apply"/> conformément à la chaîne UC → UC normalisée de
    /// §4.14.2 amendée du 0230 indexée par R-4.14.21. Le UseCase retourne au ViewModel appelant un
    /// indicateur booléen de réussite, sur lequel celui-ci branche (navigation vers la page
    /// applicative en cas de succès, incrément du compteur de tentatives en cas d'échec). Le
    /// UseCase est non transactionnel par construction : la comparaison d'empreinte est un calcul
    /// en mémoire, la lecture par login est portée par un Query Handler sans mutation, et les
    /// écritures persistantes sont déléguées aux sous-UseCases consommés, chacun portant sa propre
    /// transaction sans imbrication (I-4.10.3).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario d'authentification manuelle login/mot de passe.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// <item><description>Exposer un retour signalable booléen permettant au consommateur amont (ViewModel de connexion) de brancher sur le succès ou l'échec de l'authentification sans propagation d'exception applicative typée.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte ni la navigation, ni le comptage des tentatives de connexion, ni la fermeture applicative : ces responsabilités relèvent du ViewModel appelant.</description></item>
    /// <item><description>Ne contrôle pas l'accessibilité applicative, réalisée au démarrage et hors du périmètre de ce scénario.</description></item>
    /// <item><description>Ne compare jamais le mot de passe en clair : seule son empreinte est confrontée à l'empreinte stockée.</description></item>
    /// <item><description>Ne porte pas la logique métier fine des sous-scénarios de session et de droits, déléguée aux UseCases consommés en sous-séquence.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de A_Domain.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAuthenticate
    {
        // --- Groupe 1 : Authentification ---

        /// <summary>
        /// Authentifie un utilisateur par son identifiant de connexion et son mot de passe ; en cas
        /// de succès, établit l'identité applicative de l'utilisateur, ouvre sa session applicative
        /// et applique ses droits de pages.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : invoquée par le ViewModel de la page de connexion <c>VM_Page00</c> à la
        /// validation du formulaire, en repli lorsque l'identification automatique par contexte poste
        /// (login Windows) n'a pas abouti au démarrage. La méthode valide le couple login/mot de
        /// passe, puis, sur succès, positionne l'identité courante et enchaîne l'ouverture de session
        /// et l'application des droits de pages en consommant <see cref="IU_UserAppSession_Open"/> et
        /// <see cref="IU_UserAppPageRight_Apply"/> en sous-séquence.
        /// </para>
        /// <para>
        /// Invariants comportementaux : (1) le mot de passe n'est jamais comparé en clair — seule son
        /// empreinte est confrontée à l'empreinte stockée ; (2) l'identité applicative est positionnée
        /// avant l'ouverture de session et l'application des droits, ces deux sous-scénarios lisant
        /// l'identité depuis l'état applicatif partagé ; (3) cohérence d'état sur échec — aucune
        /// identité n'est laissée positionnée pour une authentification qui échoue, le nettoyage étant
        /// strictement ciblé sur l'identifiant utilisateur courant et ne réinitialisant jamais l'état
        /// utilisateur global, afin de préserver le compteur de tentatives géré par le ViewModel.
        /// </para>
        /// <para>
        /// Sémantique du retour : <see langword="true"/> lorsque l'authentification a abouti — couple
        /// login/mot de passe validé, identité établie, session ouverte et droits de pages appliqués.
        /// <see langword="false"/> lorsque l'authentification n'est pas établie, ce qui recouvre deux
        /// familles de situations : (a) issues métier attendues — saisie vide (login ou mot de passe
        /// blanc), login inconnu ou compte inactif ou supprimé (indistinctement), mot de passe erroné,
        /// échec d'ouverture de session signalé par <see cref="IU_UserAppSession_Open"/>, ou échec
        /// d'application des droits signalé par <see cref="IU_UserAppPageRight_Apply"/> ; (b) anomalie
        /// applicative typée (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) captée et traitée terminalement par le pipeline
        /// <c>IU_LogAndNotify</c> interne au UseCase. L'annulation coopérative
        /// (<see cref="OperationCanceledException"/>) n'est pas signalée par ce retour : elle est
        /// propagée à l'appelant selon le mécanisme normatif de §4.6 du 0230.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider la présence effective du login et du mot de passe saisis.</description></item>
        /// <item><description>Résoudre l'utilisateur applicatif actif et non supprimé associé au login.</description></item>
        /// <item><description>Comparer l'empreinte du mot de passe saisi à l'empreinte stockée.</description></item>
        /// <item><description>Positionner l'identité applicative courante puis ouvrir la session et appliquer les droits de pages en sous-séquence.</description></item>
        /// <item><description>Rétablir un état cohérent en cas d'échec de répercussion (ouverture de session ou application des droits).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif de la
        /// section 4.5 du 0230. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="login">Identifiant de connexion saisi par l'utilisateur.</param>
        /// <param name="password">
        /// Mot de passe saisi par l'utilisateur. Jamais persisté ni comparé en clair : seule son
        /// empreinte est confrontée à l'empreinte stockée.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si l'authentification a abouti (identité établie, session ouverte et
        /// droits de pages appliqués) ; <see langword="false"/> en cas d'échec — saisie vide, login
        /// inconnu ou compte inactif ou supprimé, mot de passe erroné, échec d'ouverture de session,
        /// échec d'application des droits, ou anomalie applicative typée captée et traitée
        /// terminalement.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6 du
        /// 0230. Les exceptions applicatives typées (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) ne sont jamais propagées :
        /// elles sont captées et signalées par le retour <see langword="false"/>.
        /// </exception>
        Task<bool> ExecuteAsync(string caller, string login, string password, CancellationToken ct = default);
    }
}