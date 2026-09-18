using DG244Cutting.A_Domain.Entities.App;

namespace DG244Cutting.A_Domain.Interfaces.Settings.Presentation
{
    /// <summary>
    /// Contrat du référentiel central de navigation WPF.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette interface est définie dans <c>A_Domain</c> afin d'être
    /// accessible aux UseCases de <c>B_UseCases</c> et aux services de <c>D_Presentation</c>
    /// sans dépendance vers les couches supérieures. Son implémentation concrète
    /// <see cref="D_Presentation.Settings.SE_Navigation"/> réside dans
    /// <c>D_Presentation/Settings/</c>.</para>
    /// <para>Objectif : Exposer le contrat d'accès au référentiel central de navigation
    /// — pages connues de l'application, URIs de vues et de menus horizontaux, page par défaut,
    /// page de connexion, page de repli, URI du menu horizontal réduit commun — de manière
    /// stateless et immuable, indépendamment de tout état d'exécution et de toute couche
    /// supérieure à <c>A_Domain</c>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer le dictionnaire complet des pages connues de l'application.</item>
    ///   <item>Fournir les URIs de page et de menu horizontal par nom logique de page.</item>
    ///   <item>Exposer la page par défaut et l'URI du menu réduit commun.</item>
    ///   <item>Exposer les noms logiques de la page de connexion et de la page de repli.</item>
    ///   <item>Servir de source d'autorité pour l'existence d'une page dans le système.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucun état d'exécution (page courante, historique) — ce rôle
    ///   appartient à <see cref="UseCases.App.IU_Navigation"/>.</item>
    ///   <item>Ne vérifie pas les droits utilisateur — ce rôle appartient à
    ///   <see cref="User.ISE_User"/>.</item>
    ///   <item>Ne décide pas de naviguer — ce rôle appartient à
    ///   <see cref="UseCases.App.IU_Navigation"/>.</item>
    /// </list>
    /// </remarks>
    public interface ISE_Navigation
    {
        // --- Groupe 1 : Référentiel des pages ---

        /// <summary>
        /// Obtient le dictionnaire complet des pages connues de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Ce dictionnaire est consulté par <c>UC_Navigation</c> pour
        /// vérifier l'existence d'une page avant toute tentative de navigation.</para>
        /// <para>Objectif : Retourner une vue en lecture seule du référentiel interne
        /// afin d'interdire toute modification externe.</para>
        /// </remarks>
        IReadOnlyDictionary<string, PageMapping> PageMappings { get; }

        // --- Groupe 2 : Accès par nom logique ---

        /// <summary>
        /// Tente d'obtenir le <see cref="PageMapping"/> associé à un nom logique de page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>UC_Navigation</c> avant toute décision de navigation,
        /// pour valider l'existence de la page cible dans le référentiel.</para>
        /// <para>Objectif : Éviter de propager un nom de page inconnu vers les couches
        /// de présentation ou de service.</para>
        /// </remarks>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page10"</c>).</param>
        /// <param name="mapping">
        /// Lorsque la méthode retourne <see langword="true"/>, contient le <see cref="PageMapping"/>
        /// correspondant ; sinon, contient la valeur par défaut du type.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la page est connue du référentiel ;
        /// <see langword="false"/> sinon.
        /// </returns>
        bool TryGetPageMapping(string pageName, out PageMapping mapping);

        // --- Groupe 3 : Valeurs par défaut et constantes ---

        /// <summary>
        /// Obtient le nom logique de la page par défaut de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisé par <c>UC_Navigation</c> lors de l'initialisation
        /// de l'application ou lors d'un repli vers l'état initial.</para>
        /// </remarks>
        string DefaultPageName { get; }

        /// <summary>
        /// Obtient le nom logique de la page de connexion, exemptée du contrôle
        /// des droits d'accès.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisé par <c>UC_Navigation</c> pour identifier
        /// la page sur laquelle aucune vérification de droits n'est appliquée, et
        /// éviter ainsi un blocage de la navigation initiale avant authentification.</para>
        /// </remarks>
        string LoginPageName { get; }

        /// <summary>
        /// Obtient le nom logique de la page de repli, vers laquelle toute navigation
        /// non autorisée est silencieusement redirigée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisé par <c>UC_Navigation</c> lorsqu'une tentative
        /// de navigation vers une page pour laquelle l'utilisateur ne dispose pas des
        /// droits requis est interceptée, pour rediriger silencieusement vers la page
        /// de repli sans propager d'erreur à l'interface.</para>
        /// </remarks>
        string FallbackPageName { get; }

        /// <summary>
        /// Obtient le <see cref="PageMapping"/> de la page par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Raccourci sémantique évitant un appel à
        /// <see cref="TryGetPageMapping"/> pour la page d'accueil.</para>
        /// </remarks>
        PageMapping DefaultPageMapping { get; }

        /// <summary>
        /// Obtient l'URI du composant de menu horizontal réduit, commun à toutes les pages
        /// qui n'ont pas de menu horizontal propre.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisée par <c>SR_Navigation</c> lors de la transition
        /// vers une nouvelle page (réduction temporaire du menu) et par <c>UC_Navigation</c>
        /// lors de la navigation vers <c>Page00</c>.</para>
        /// </remarks>
        Uri MH_Reduce_Source { get; }
    }
}