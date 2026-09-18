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
    /// QueryHandler (QH) dédié à <see cref="UserAppSession"/>. Fournit des requêtes de lecture
    /// nécessaires à la gestion des sessions applicatives (ouverture/fermeture du programme).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases et services pour retrouver les sessions existantes d’un utilisateur
    /// pour une application donnée, et déterminer la session à considérer comme “courante”.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les lectures spécifiques sur <see cref="UserAppSession"/> en s’appuyant sur
    /// <see cref="IR_UserSession"/> et en appliquant la traçabilité (callChain) et la reclassification
    /// des exceptions via <see cref="Ex_Classifier"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases / Services de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Récupérer les sessions par utilisateur et application.</description></item>
    /// <item><description>Déterminer un identifiant de session pertinent pour la suite du traitement.</description></item>
    /// </list>
    /// </summary>
    public class QH_UserSession : QH_Generic<UserAppSession>, IQ_UserSession
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IR_UserSession _repositorySpecifique;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le QueryHandler <see cref="QH_UserSession"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser le repository spécifique utilisé pour les lectures ciblées.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider <paramref name="repository"/>.</description></item>
        /// <item><description>Transmettre le repository au handler générique parent.</description></item>
        /// </list>
        /// </summary>
        /// <param name="repository">Repository spécifique <see cref="UserAppSession"/>.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="repository"/> est null.</exception>
        public QH_UserSession(IR_UserSession repository)
            : base(repository)
        {
            _callee = GetType().Name;
            _repositorySpecifique = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne les sessions <see cref="UserAppSession"/> pour un utilisateur et une application.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour déterminer si une session existe déjà à l’ouverture/fermeture du programme.</para>
        /// <para>Objectif</para>
        /// <para>Déléguer la lecture spécifique au repository et garantir la traçabilité.</para>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="appId">Identifiant de l’application.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Liste de sessions (éventuellement vide).</returns>
        /// <exception cref="Exception">Toute exception est reclassifiée via <see cref="Ex_Classifier"/>.</exception>
        public async Task<List<UserAppSession>> HandleGetByUserIdAppIdAsync( string caller, int userId, int appId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByUserIdAppIdAsync)}";

            try
            {
                return await _repositorySpecifique.GetByUserIdAppIdAsync(callChain, userId, appId, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne un identifiant de session pertinent pour un utilisateur et une application.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé par les UseCases pour choisir une session “courante” (par exemple à mettre à jour
        /// lors de la fermeture du programme).
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Sélectionner la session la plus récente de manière déterministe. Si aucune session n’existe,
        /// retourne 0.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Charger les sessions existantes.</description></item>
        /// <item><description>Sélectionner la session la plus récente (UpdatedAt puis CreatedAt puis Id).</description></item>
        /// <item><description>Retourner son Id, sinon 0.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="appId">Identifiant de l’application.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Id de session, ou 0 si aucune session n’existe.</returns>
        /// <exception cref="Exception">Toute exception est reclassifiée via <see cref="Ex_Classifier"/>.</exception>
        public async Task<int> HandleGetSessionIdAsync( string caller, int userId, int appId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetSessionIdAsync)}";

            try
            {
                var existingSessions = await HandleGetByUserIdAppIdAsync(callChain, userId, appId, ct);

                if (existingSessions.Count == 0)
                    return 0;

                var selected = existingSessions
                    .OrderByDescending(s => s.IsConnected)
                    .ThenByDescending(s => s.UpdatedAt ?? DateTime.MinValue)
                    .ThenByDescending(s => s.CreatedAt)
                    .ThenByDescending(s => s.Id)
                    .First();

                return selected.Id;
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