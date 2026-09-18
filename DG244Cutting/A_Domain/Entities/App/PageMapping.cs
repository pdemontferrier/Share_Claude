
namespace DG244Cutting.A_Domain.Entities.App
{
    /// <summary>
    /// Représente la description complète d'une page de navigation.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette entité est définie dans <c>A_Domain</c> afin d'être utilisable
    /// par toutes les couches sans créer de dépendances croisées.</para>
    /// <para>Objectif : Regrouper en un enregistrement immuable les informations structurelles
    /// d'une page : son nom logique, l'URI de sa vue, l'URI de son menu horizontal et le nom
    /// de la région de menu vertical associée.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer les quatre attributs définissant une page de navigation.</item>
    ///   <item>Garantir l'immutabilité de ces attributs après construction.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucun comportement métier ni aucune logique d'exécution.</item>
    ///   <item>Ne connaît pas les droits associés à la page — ce rôle appartient à <see cref="DG244Cutting.A_Domain.Interfaces.Settings.User.ISE_User"/>.</item>
    /// </list>
    /// </remarks>
    /// <param name="PageName">Nom logique de la page (ex. <c>"Page10"</c>).</param>
    /// <param name="PageUri">URI relative de la vue XAML de la page.</param>
    /// <param name="MHUri">URI relative du menu horizontal associé à cette page.</param>
    /// <param name="MVName">Nom de la région de menu vertical associée (ex. <c>"MV1"</c>).</param>
    public readonly record struct PageMapping(
        string PageName,
        Uri PageUri,
        Uri MHUri,
        string MVName
    );
}