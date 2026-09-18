using DG244Cutting.A_Domain.DTOs.App;

namespace DG244Cutting.A_Domain.Interfaces.Services.App
{
    /// <summary>
    /// Contrat du service de fourniture du contexte applicatif courant.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : contrat défini dans A_Domain, consommé par injection de
    /// dépendances par les composants autorisés (UseCases, ViewModels). L'implémentation
    /// <c>SR_AppContext</c> réside en B_UseCases/Services/App.</para>
    /// <para>Objectif : exposer, sous forme d'un objet de transport unique, un
    /// instantané du contexte applicatif et utilisateur courant, agrégé à partir des
    /// Settings applicatif et utilisateur.</para>
    /// <para>Responsabilités : agréger les valeurs de contexte exposées par les Settings
    /// applicatif et utilisateur dans un <c>DTO_AppContext</c>.</para>
    /// <para>Non-responsabilités : aucune logique métier ; aucun accès aux données
    /// (repository, base, service externe) ; aucune orchestration de flux applicatif ;
    /// aucune mutation des Settings.</para>
    /// </remarks>
    public interface IS_AppContext
    {
        // --- Groupe 1 : Accès au contexte applicatif ---

        /// <summary>
        /// Retourne un instantané du contexte applicatif et utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : appelée lorsqu'un composant a besoin du contexte applicatif
        /// courant en entrée de traitement.</para>
        /// <para>Objectif : projeter les valeurs courantes des Settings applicatif et
        /// utilisateur dans un objet de transport sans comportement.</para>
        /// </remarks>
        /// <returns>Un <c>DTO_AppContext</c> renseigné à partir des Settings courants.</returns>
        DTO_AppContext GetAppContext();
    }
}