using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.User;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase orchestrateur de navigation WPF.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette interface est définie dans <c>A_Domain</c> afin d'être
    /// accessible aux ViewModels et aux code-behinds de <c>D_Presentation</c> sans dépendance
    /// vers <c>B_UseCases</c>. Son implémentation concrète
    /// <see cref="B_UseCases.UseCases.App.UC_Navigation"/> réside dans
    /// <c>B_UseCases/UseCases/App/</c>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer les opérations de navigation accessibles aux ViewModels et aux
    ///   composants orchestrateurs.</item>
    ///   <item>Exposer les opérations de contrôle du menu horizontal (déploiement, réduction)
    ///   accessibles aux ViewModels des composants <c>MH_PageNN</c>.</item>
    ///   <item>Exposer une vue prédicative des droits d'accès et des droits métier de
    ///   l'utilisateur courant pour chaque page applicative, consommée par
    ///   <c>MH_Generic.ApplySecurityRules</c> et par les ViewModels conditionnant
    ///   l'affichage ou l'activation de boutons d'action.</item>
    ///   <item>Exposer l'état runtime courant de la navigation (page active, disponibilité
    ///   du retour arrière).</item>
    ///   <item>Garantir que toute décision de navigation — validation de page, vérification
    ///   de droits, redirection de repli — transite exclusivement par ce contrat.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne décrit aucun détail d'implémentation WPF — ce rôle appartient à
    ///   <see cref="IS_Navigation"/>.</item>
    ///   <item>N'expose pas le référentiel des pages — ce rôle appartient à
    ///   <see cref="ISE_Navigation"/>.</item>
    ///   <item>N'expose pas l'état utilisateur brut — l'accès aux droits passe par les
    ///   prédicats <c>CanXxx</c> définis ici, qui encapsulent la consultation de
    ///   <see cref="ISE_User"/>.</item>
    /// </list>
    /// <para>Note sur la pluralité de méthodes publiques : Ce contrat expose
    /// plusieurs méthodes publiques, conformément à la dérogation admise pour le cas
    /// Concept au sens de la convention de nommage UC_ dual-cas Entité/Concept. La
    /// dérogation au préfixe canonique <c>ExecuteAsync</c> et à l'unicité de la méthode
    /// publique est tracée par le présent <c>&lt;remarks&gt;</c> conformément à §4.3.</para>
    /// </remarks>
    public interface IU_Navigation
    {
        // --- Groupe 1 : État runtime courant ---

        /// <summary>
        /// Obtient le nom logique de la page actuellement affichée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consultée par les ViewModels pour déterminer la page active,
        /// par exemple pour mettre à jour l'état visuel d'un menu ou conditionner une action.</para>
        /// <para>Valeur initiale : Chaîne vide avant la première navigation.</para>
        /// </remarks>
        string CurrentPageName { get; }

        /// <summary>
        /// Obtient une valeur indiquant si une navigation arrière est possible.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisée par les ViewModels pour activer ou désactiver un
        /// bouton de retour arrière.</para>
        /// <para>Valeur : <see langword="true"/> si l'historique de navigation contient
        /// au moins une entrée ; <see langword="false"/> sinon.</para>
        /// </remarks>
        bool CanNavigateBack { get; }

        // --- Groupe 2 : Opérations de navigation ---

        /// <summary>
        /// Navigue vers la page identifiée par son nom logique.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Point d'entrée principal de la navigation applicative.
        /// Appelée par les ViewModels lorsqu'une interaction utilisateur déclenche un
        /// changement de page.</para>
        /// <para>Comportement :</para>
        /// <list type="bullet">
        ///   <item>Si <paramref name="pageName"/> est la page courante, la méthode retourne
        ///   sans action.</item>
        ///   <item>Si la page n'est pas référencée dans
        ///   <see cref="ISE_Navigation"/>, l'erreur est journalisée et
        ///   notifiée via <c>IU_LogAndNotify</c>.</item>
        ///   <item>Si l'utilisateur n'a pas le droit <c>CanAccess</c> pour
        ///   <paramref name="pageName"/>, la navigation est silencieusement redirigée vers la
        ///   page de repli configurée.</item>
        ///   <item>La page courante est poussée dans l'historique avant navigation.</item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant (ViewModel ou
        /// composant système).</param>
        /// <param name="pageName">Nom logique de la page cible (ex. <c>"Page10"</c>).</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        Task NavigateToPageAsync(string caller, string pageName, CancellationToken ct = default);

        /// <summary>
        /// Navigue vers la page précédemment visitée, en dépilant l'historique de navigation.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par les ViewModels lorsque l'utilisateur active une
        /// action de retour arrière.</para>
        /// <para>Comportement :</para>
        /// <list type="bullet">
        ///   <item>Si l'historique est vide, l'erreur est journalisée et notifiée — aucune
        ///   navigation n'est effectuée.</item>
        ///   <item>La navigation arrière ne pousse pas la page courante dans l'historique,
        ///   afin d'éviter les cycles.</item>
        ///   <item>Les droits de la page dépilée sont validés : si l'accès est refusé,
        ///   la redirection vers la page de repli s'applique.</item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        Task NavigateToPreviousPageAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Navigue vers la page par défaut de l'application, ou vers la page de connexion
        /// lorsqu'aucun utilisateur n'est connecté.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de l'initialisation de l'application ou lors
        /// d'une remise à zéro de la navigation (post-authentification, retour à l'accueil).</para>
        /// <para>Comportement : La cible de navigation effective dépend de l'état de
        /// l'utilisateur courant :</para>
        /// <list type="bullet">
        ///   <item>Si aucun utilisateur n'est connecté (<see cref="ISE_User.AppUserId"/>
        ///   inférieur ou égal à zéro), la navigation est dirigée vers
        ///   <see cref="ISE_Navigation.LoginPageName"/> (page de connexion), exemptée du
        ///   contrôle des droits d'accès.</item>
        ///   <item>Sinon, la navigation est dirigée vers
        ///   <see cref="ISE_Navigation.DefaultPageName"/> (page d'accueil applicative),
        ///   soumise au contrôle des droits d'accès via la séquence commune de
        ///   <see cref="NavigateToPageAsync"/>.</item>
        /// </list>
        /// <para>La page courante est poussée dans l'historique si elle diffère de la page
        /// cible effectivement résolue.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        Task NavigateToDefaultAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Rafraîchit la page courante en la réinstanciant depuis son type.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lorsque les données de la page active doivent être
        /// rechargées sans changer de page — par exemple après une opération d'écriture qui
        /// modifie l'état affiché.</para>
        /// <para>Comportement : Délègue à
        /// <see cref="IS_Navigation.RefreshCurrentPage"/>. Si aucune page n'est
        /// actuellement chargée, l'erreur est journalisée.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        Task RefreshCurrentPageAsync(string caller, CancellationToken ct = default);

        // --- Groupe 3 : Gestion de l'historique ---

        /// <summary>
        /// Vide l'historique de navigation.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de la déconnexion d'un utilisateur ou lors
        /// d'une remise à zéro complète du contexte applicatif, afin d'éviter que l'historique
        /// d'une session précédente ne soit accessible à la session suivante.</para>
        /// <para>Comportement : L'historique est effacé. La page courante n'est pas
        /// modifiée. Aucune navigation n'est effectuée.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        void ClearNavigationHistory(string caller);

        // --- Groupe 4 : Contrôle du menu horizontal ---

        /// <summary>
        /// Déploie le menu horizontal vers le composant associé à la page courante.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu réduit lorsque l'opérateur
        /// active explicitement le déploiement du menu horizontal de la page affichée.
        /// Conformément aux règles de navigation, le déploiement du menu n'est jamais
        /// automatique : il résulte exclusivement d'une action utilisateur.</para>
        /// <para>Comportement :</para>
        /// <list type="bullet">
        ///   <item>L'URI du menu horizontal cible est lue depuis le mapping de la page
        ///   courante maintenu en état runtime — elle n'est ni recalculée ni reconsultée
        ///   dans <see cref="ISE_Navigation"/>.</item>
        ///   <item>Si aucune page n'est actuellement chargée (état initial), l'erreur est
        ///   journalisée et notifiée via <c>IU_LogAndNotify</c>.</item>
        ///   <item>L'opération est déléguée à <see cref="IS_Navigation.NavigateHorizontalMenu"/>
        ///   pour exécution sur le thread UI.</item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        Task ExpendHorizontalMenuAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Réduit le menu horizontal vers le composant de menu réduit commun.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal déployé
        /// lorsque l'opérateur active explicitement la réduction. Cette opération est
        /// distincte de la réduction automatique effectuée à chaque navigation par
        /// <c>UC_Navigation</c> en interne — elle constitue un contrôle utilisateur direct.</para>
        /// <para>Comportement : Délègue à
        /// <see cref="IS_Navigation.ReduceHorizontalMenu"/>, qui charge l'URI fournie par
        /// <see cref="ISE_Navigation.MH_Reduce_Source"/> dans la frame
        /// <c>ActiveHorizontalMenu</c>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        Task ReduceHorizontalMenuAsync(string caller, CancellationToken ct = default);

        // --- Groupe 5 : Lecture des droits utilisateur par page ---

        /// <summary>
        /// Indique si l'utilisateur courant peut accéder à la page par défaut de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consulté par <c>MH_Generic.ApplyNavigationRules</c>
        /// pour conditionner la visibilité du bouton <c>MH_Home</c> sans codage en dur du nom
        /// logique de la page d'accueil. Ce prédicat encapsule la lecture de
        /// <see cref="ISE_Navigation.DefaultPageName"/> et l'évaluation du droit
        /// <c>CanAccess</c> sur cette page.</para>
        /// <para>Objectif : Permettre aux vues d'évaluer la disponibilité de la page
        /// d'accueil sans avoir à connaître son nom logique ni à injecter elles-mêmes
        /// <see cref="ISE_Navigation"/>. Le couplage entre référentiel des pages et droits
        /// d'accès reste centralisé dans <see cref="UC_Navigation"/>, conformément à son rôle
        /// de point de jonction normatif entre <see cref="ISE_Navigation"/> et
        /// <see cref="ISE_User"/>.</para>
        /// <para>Comportement : Retourne le résultat de <see cref="CanNavigate"/> évalué
        /// sur le nom de la page par défaut. Retourne <see langword="false"/> si la page par
        /// défaut n'est pas accessible à l'utilisateur courant.</para>
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> si l'utilisateur courant a le droit d'accès à la page par
        /// défaut ; <see langword="false"/> sinon.
        /// </returns>
        bool CanNavigateToDefault();

        /// <summary>
        /// Indique si l'utilisateur courant peut accéder à la page spécifiée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consulté par <c>MH_Generic.ApplySecurityRules</c>
        /// pour conditionner la visibilité d'un bouton qui déclencherait une navigation vers
        /// <paramref name="pageName"/>. Encapsule la lecture du droit <c>CanAccess</c> depuis
        /// <see cref="ISE_User"/>.</para>
        /// <para>Comportement : Retourne <see langword="false"/> si <paramref name="pageName"/>
        /// est null, vide ou inconnu, ou si le droit n'est pas explicitement accordé.</para>
        /// </remarks>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page10"</c>).</param>
        bool CanNavigate(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut créer un enregistrement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        bool CanCreate(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut lire les données de la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        bool CanRead(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut modifier un enregistrement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        bool CanUpdate(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut supprimer un enregistrement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        bool CanDelete(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut exercer un contrôle métier sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page40"</c>).</param>
        bool CanControl(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut valider une opération sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page40"</c>).</param>
        bool CanValidate(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut superviser un traitement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page50"</c>).</param>
        bool CanSupervise(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut consulter les données de monitoring de la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page60"</c>).</param>
        bool CanMonitor(string pageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut administrer les paramètres de la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page90"</c>).</param>
        bool CanAdmin(string pageName);
    }
}