using DG244Cutting.A_Domain.Entities.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Contrat du service technique de navigation WPF.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette interface est définie dans <c>A_Domain</c> afin d'être
    /// accessible à <c>UC_Navigation</c> (couche <c>B_UseCases</c>) sans dépendance vers
    /// <c>D_Presentation</c>. Son implémentation concrète
    /// <see cref="DG244Cutting.D_Presentation.Services.SR_Navigation"/> réside dans
    /// <c>D_Presentation/Services/</c> et constitue l'unique point de contact applicatif
    /// avec les frames WPF et le <c>Dispatcher</c>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Naviguer la frame de page active (<c>ActivePage</c>) vers une URI donnée.</item>
    ///   <item>Naviguer la frame de menu horizontal (<c>ActiveHorizontalMenu</c>) vers une URI donnée.</item>
    ///   <item>Réduire le menu horizontal vers le composant de menu réduit commun.</item>
    ///   <item>Rafraîchir la page courante en la réinstanciant depuis son type.</item>
    ///   <item>Garantir la synchronisation avec le thread UI via le <c>Dispatcher</c>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne vérifie pas les droits d'accès — ce rôle appartient à
    ///   <see cref="IU_Navigation"/>.</item>
    ///   <item>Ne décide pas quelle page afficher — ce rôle appartient à
    ///   <see cref="IU_Navigation"/>.</item>
    ///   <item>Ne maintient aucun état de navigation (page courante, historique) — ce rôle
    ///   appartient à <see cref="App.IU_Navigation"/>.</item>
    ///   <item>Ne consulte pas les droits utilisateur — ce rôle appartient à
    ///   <see cref="ISE_User"/>.</item>
    /// </list>
    /// </remarks>
    public interface IS_Navigation
    {
        /// <summary>
        /// Navigue la frame de page active vers la page décrite par le <see cref="PageMapping"/> fourni.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>UC_Navigation</c> après que la décision de
        /// navigation a été prise et les droits validés.</para>
        /// <para>Objectif : Exécuter la navigation WPF de la frame <c>ActivePage</c> vers
        /// l'URI de page contenue dans le mapping. La méthode s'assure que l'appel est exécuté
        /// sur le thread UI via le <c>Dispatcher</c>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="mapping">Description complète de la page cible, incluant son URI de vue XAML.</param>
        void NavigateToPage(string caller, PageMapping mapping);

        /// <summary>
        /// Navigue la frame de menu horizontal active vers l'URI spécifiée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>UC_Navigation</c> pour étendre le menu
        /// horizontal vers la vue associée à la page courante, ou pour toute navigation
        /// explicite vers un composant de menu.</para>
        /// <para>Objectif : Mettre à jour la frame <c>ActiveHorizontalMenu</c> sans
        /// aucune logique décisionnelle. La méthode s'assure que l'appel est exécuté sur
        /// le thread UI via le <c>Dispatcher</c>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="menuUri">URI relative du composant de menu horizontal à afficher.</param>
        void NavigateHorizontalMenu(string caller, Uri menuUri);

        /// <summary>
        /// Réduit le menu horizontal en naviguant vers le composant de menu réduit commun.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>UC_Navigation</c> lors d'une transition de
        /// page, pour réduire visuellement le menu horizontal avant que la nouvelle page soit
        /// affichée, ou lors de la navigation vers <c>Page00</c> qui ne dispose pas de menu
        /// horizontal propre.</para>
        /// <para>Objectif : Raccourci sémantique vers <see cref="NavigateHorizontalMenu"/>
        /// avec l'URI du menu réduit fournie par <c>UC_Navigation</c>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="reduceSource">URI du menu horizontal réduit à charger dans la frame.</param>
        void ReduceHorizontalMenu(string caller, Uri reduceSource);

        /// <summary>
        /// Rafraîchit la page courante en la réinstanciant depuis son type.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>UC_Navigation</c> lorsque les données de la
        /// page active doivent être rechargées sans changer de page.</para>
        /// <para>Objectif : Recréer une nouvelle instance du type de la page actuellement
        /// contenue dans la frame <c>ActivePage</c>, déclenchant ainsi son rechargement complet.
        /// La méthode est sans effet si la frame ne contient aucun contenu.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        void RefreshCurrentPage(string caller);
    }
}