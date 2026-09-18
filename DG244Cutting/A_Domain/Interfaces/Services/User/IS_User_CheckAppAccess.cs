using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Description :
    /// <para>
    /// Interface du service métier responsable de la vérification de l’accès
    /// d’un utilisateur à une application donnée.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette interface est utilisée dans la couche Domain pour définir le contrat
    /// fonctionnel d’un service de contrôle d’accès applicatif, sans dépendre
    /// d’une implémentation technique particulière. Elle est consommée par les
    /// UseCases chargés de piloter les scénarios d’authentification, de démarrage
    /// ou de validation des droits utilisateur.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Définir une opération métier unitaire permettant de déterminer si un
    /// utilisateur identifié possède un droit d’accès à l’application ciblée,
    /// dans le respect des conventions de traçabilité et de robustesse du projet.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// UseCases de la couche Application nécessitant de contrôler l’accès
    /// utilisateur à l’application.
    /// </para>
    ///
    /// Tâches / Actions :
    /// <list type="bullet">
    /// <item><description>Définir le contrat de vérification d’accès applicatif utilisateur.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c>.</description></item>
    /// <item><description>Retourner un résultat booléen simple et exploitable par le UseCase appelant.</description></item>
    /// </list>
    /// </summary>
    public interface IS_User_CheckAppAccess
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>
        /// Vérifie si un utilisateur possède un droit d’accès sur une application donnée.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode est appelée depuis un UseCase de contrôle d’accès ou
        /// d’initialisation applicative. Elle représente une action métier atomique,
        /// limitée au contrôle du lien d’autorisation entre un utilisateur et
        /// une application.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner <c>true</c> si l’utilisateur est autorisé à accéder à
        /// l’application, sinon <c>false</c>.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Recevoir l’identifiant utilisateur et l’identifiant application.</description></item>
        /// <item><description>Recevoir la CallChain amont.</description></item>
        /// <item><description>Retourner le résultat du contrôle d’accès.</description></item>
        /// </list>
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur à contrôler.</param>
        /// <param name="appId">Identifiant de l’application à contrôler.</param>
        /// <param name="caller">CallChain amont transmise par l’appelant.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// <c>true</c> si l’utilisateur possède un accès à l’application ; sinon <c>false</c>.
        /// </returns>
        /// <exception cref="Ex_Business">Levée si une erreur métier est détectée lors du contrôle d’accès.</exception>
        /// <exception cref="Ex_Infrastructure">Levée si une erreur technique survient lors de l’accès aux données.</exception>
        Task<bool> ExecuteAsync(int userId, int appId, string caller, CancellationToken ct = default);

        #endregion
    }
}