using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using LeitTemporImport.B_UseCases.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;


namespace LeitTemporImport.B_UseCases.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// QueryHandler (QH) dédié à l’entité <see cref="UserApp"/>. Il encapsule les requêtes de lecture
    /// spécifiques en s’appuyant sur le repository <see cref="IR_UserApp"/>, conformément au modèle CQRS.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases et services applicatifs nécessitant des lectures sur la table UserApp,
    /// sans accéder directement au DbContext.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir des points d’entrée de requêtes traçables (CallChain) et robustes (reclassification d’exceptions).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher un utilisateur par login Windows.</description></item>
    /// </list>
    /// </summary>
    public class QH_UserApp : QH_Generic<UserApp>, IQ_UserApp
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IR_UserApp _repositorySpecifique;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le QueryHandler UserApp.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires aux requêtes UserApp.</para>
        /// </summary>
        /// <param name="repository">Repository spécifique <see cref="IR_UserApp"/>.</param>
        public QH_UserApp(IR_UserApp repository)
            : base(repository)
        {
            _callee = GetType().Name;
            _repositorySpecifique = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne un utilisateur applicatif à partir de son login Windows.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour identifier l’utilisateur courant sur un poste (device user).</para>
        /// <para>Objectif</para>
        /// <para>Fournir une requête CQRS dédiée, traçable et robuste.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="windowsLogin"/>.</description></item>
        /// <item><description>Interroger le repository <see cref="IR_UserApp"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="windowsLogin">Login Windows à rechercher.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="UserApp"/> ou null.</returns>
        public async Task<UserApp?> HandleGetByWindowsLoginAsync(string caller, string windowsLogin, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByWindowsLoginAsync)}";

            try
            {
                if (string.IsNullOrWhiteSpace(windowsLogin))
                    throw new ArgumentException("windowsLogin is required.", nameof(windowsLogin));

                return await _repositorySpecifique.GetByWindowsLoginAsync(callChain, windowsLogin, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}