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
    /// QueryHandler (QH) dédié à l’entité <see cref="ProductionSeries"/>. Il encapsule les requêtes de lecture
    /// spécifiques en s’appuyant sur le repository <see cref="IR_ProductionSeries"/>, conformément au modèle CQRS.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import afin de déterminer si une série de production a déjà été importée
    /// et d’appliquer les règles de traitement associées (ex : suppression du fichier MDB si déjà importé).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir des points d’entrée de requêtes traçables (CallChain) et robustes (reclassification d’exceptions)
    /// pour les lectures sur ProductionSeries.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lire une série par IdSerialNumber.</description></item>
    /// <item><description>Lire le statut IsImported d’une série.</description></item>
    /// </list>
    /// </summary>
    public class QH_ProductionSeries : QH_Generic<ProductionSeries>, IQ_ProductionSeries
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IR_ProductionSeries _repositorySpecifique;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le QueryHandler ProductionSeries.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires aux requêtes ProductionSeries.</para>
        /// <param name="repository">Repository spécifique <see cref="IR_ProductionSeries"/>.</param>
        /// </summary>
        public QH_ProductionSeries(IR_ProductionSeries repository)
            : base(repository)
        {
            _callee = GetType().Name;
            _repositorySpecifique = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la série de production correspondant à un numéro de série donné.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lors des contrôles d’existence et des validations avant traitement d’import.</para>
        /// <para>Objectif</para>
        /// <para>Fournir une requête CQRS dédiée, traçable et robuste.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider l’argument <paramref name="idSerialNumber"/>.</description></item>
        /// <item><description>Interroger le repository <see cref="IR_ProductionSeries"/>.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="ProductionSeries"/> ou null.</returns>
        /// </summary>
        public async Task<ProductionSeries?> HandleGetByIdSerialNumberAsync(
            string caller,
            int idSerialNumber,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByIdSerialNumberAsync)}";

            try
            {
                if (idSerialNumber <= 0)
                    throw new ArgumentOutOfRangeException(nameof(idSerialNumber), "idSerialNumber must be > 0.");

                return await _repositorySpecifique.GetByIdSerialNumberAsync(callChain, idSerialNumber, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le statut d’import (IsImported) associé à un numéro de série donné.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour déterminer si une série a déjà été importée.</para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture optimisée du statut IsImported, traçable et robuste.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider l’argument <paramref name="idSerialNumber"/>.</description></item>
        /// <item><description>Interroger le repository <see cref="IR_ProductionSeries"/>.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Bool? : true/false si trouvé, null sinon.</returns>
        /// </summary>
        public async Task<bool?> HandleGetIsImportedAsync(
            string caller,
            int idSerialNumber,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetIsImportedAsync)}";

            try
            {
                if (idSerialNumber <= 0)
                    throw new ArgumentOutOfRangeException(nameof(idSerialNumber), "idSerialNumber must be > 0.");

                return await _repositorySpecifique.GetIsImportedAsync(callChain, idSerialNumber, ct);
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
