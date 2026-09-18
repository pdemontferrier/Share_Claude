using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Service métier responsable de la suppression physique des sessions utilisateur supplémentaires.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/Services/User</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé par le UseCase orchestrateur
    /// du cycle de session via son interface <see cref="IS_UserAppSession_DeleteExtra"/> ;
    /// il est le consommateur direct unique de <see cref="IC_Generic{T}"/> pour l'entité
    /// <see cref="UserAppSession"/> dans le cadre de la suppression de doublons, selon le
    /// patron normatif de consommation par injection (§4.15.3).
    /// </para>
    /// <para>
    /// Objectif : porter l'action métier unitaire de suppression des sessions
    /// supplémentaires, en filtrant les éléments non éligibles puis en déléguant chaque
    /// suppression physique au Command Handler générique, sans exposer la logique de
    /// persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Parcourir la collection fournie en ignorant les éléments null ou d'identifiant non strictement positif.</description></item>
    /// <item><description>Déléguer chaque suppression physique au Command Handler générique via <see cref="IC_Generic{T}.HandleDeleteAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction : ce rôle appartient au UseCase orchestrateur.</description></item>
    /// <item><description>Ne détermine pas quelles sessions sont supplémentaires : la collection est fournie par le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Command Handler.</description></item>
    /// <item><description>Ne journalise ni ne notifie : ces rôles relèvent du pipeline terminal du UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_UserAppSession_DeleteExtra"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_UserAppSession_DeleteExtra : IS_UserAppSession_DeleteExtra
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IC_Generic<UserAppSession> _commandHandler;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_UserAppSession_DeleteExtra"/> avec ses dépendances.
        /// </summary>
        /// <param name="commandHandler">Command Handler générique consommé pour la suppression des sessions.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public SR_UserAppSession_DeleteExtra(
            IC_Generic<UserAppSession> commandHandler,
            IS_ExClassifier classifier)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Supprime physiquement les sessions <see cref="UserAppSession"/> supplémentaires
        /// fournies afin d'éviter la persistance de doublons.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur du cycle de session, à
        /// l'intérieur de la transaction qu'il a ouverte. Chaque suppression effective est
        /// déléguée au Command Handler générique, qui capture l'image de l'entité et inscrit
        /// l'enregistrement Event Store associé. Les éléments null ou d'identifiant non
        /// strictement positif sont ignorés sans erreur.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="additionalSessions"/>.</description></item>
        /// <item><description>Parcourir la collection en ignorant les éléments non éligibles.</description></item>
        /// <item><description>Déléguer chaque suppression physique au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne sélectionne pas les sessions à supprimer : la collection est constituée par le UseCase.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> ni ne pilote la transaction.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="additionalSessions">
        /// Sessions supplémentaires à supprimer. Ne doit pas être <see langword="null"/> ;
        /// les éléments null ou d'identifiant non strictement positif sont ignorés.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="additionalSessions"/> est <see langword="null"/>.</exception>
        /// <exception cref="Ex_Infrastructure">Levée si une suppression en base échoue lors de la délégation au Command Handler.</exception>
        public async Task ExecuteAsync(
            string caller,
            IEnumerable<UserAppSession> additionalSessions,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (additionalSessions is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "La collection des sessions supplémentaires à supprimer est obligatoire.");

                ct.ThrowIfCancellationRequested();

                foreach (var session in additionalSessions)
                {
                    ct.ThrowIfCancellationRequested();

                    if (session is null) continue;
                    if (session.Id <= 0) continue;

                    await _commandHandler.HandleDeleteAsync(callChain, session.Id, ct);
                }
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}