using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Service métier responsable de la mise à jour de l'état connecté/déconnecté d'une session utilisateur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/Services/User</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé par le UseCase d'ouverture
    /// ou de fermeture de programme via son interface <see cref="IS_UserAppSession_Update"/> ;
    /// il est le consommateur direct unique de <see cref="IC_Generic{T}"/> pour l'entité
    /// <see cref="UserAppSession"/> dans le cadre de la mise à jour, selon le patron normatif
    /// de consommation par injection (§4.15.3).
    /// </para>
    /// <para>
    /// Objectif : porter l'action métier unitaire de mise à jour d'une session, en
    /// appliquant la logique conditionnelle de positionnement des dates selon l'état
    /// connecté/déconnecté à partir du <see cref="DTO_AppContext"/> reçu, puis en
    /// déléguant la mutation au Command Handler générique, sans exposer la logique de
    /// persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Reporter les informations device du contexte sur la session.</description></item>
    /// <item><description>Positionner le statut et la date de connexion ou de déconnexion selon l'état demandé.</description></item>
    /// <item><description>Déléguer l'écriture au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à l'appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction : ce rôle appartient au UseCase orchestrateur.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> ni <c>IS_AppContext</c> : le contexte applicatif lui est fourni par argument depuis le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Command Handler.</description></item>
    /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique.</description></item>
    /// <item><description>Ne journalise ni ne notifie : ces rôles relèvent du pipeline terminal du UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_UserAppSession_Update"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_UserAppSession_Update : IS_UserAppSession_Update
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
        /// Initialise une nouvelle instance de <see cref="SR_UserAppSession_Update"/> avec ses dépendances.
        /// </summary>
        /// <param name="commandHandler">Command Handler générique consommé pour l'écriture de la session.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public SR_UserAppSession_Update(
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
        /// Met à jour une session <see cref="UserAppSession"/> existante avec l'état
        /// connecté/déconnecté, les informations device et la date correspondante issus
        /// du contexte applicatif fourni.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée à l'ouverture (état connecté) ou à la fermeture (état
        /// déconnecté) du programme par le UseCase orchestrateur, à l'intérieur de la
        /// transaction qu'il a ouverte. La mutation effective est déléguée au Command
        /// Handler générique, qui positionne les champs d'audit et inscrit l'enregistrement
        /// Event Store associé.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="session"/> et <paramref name="appContext"/>.</description></item>
        /// <item><description>Reporter les informations device et positionner le statut de connexion.</description></item>
        /// <item><description>Positionner conditionnellement la date de connexion ou de déconnexion selon <paramref name="isConnected"/>.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> ni ne pilote la transaction.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="session">Session existante à mettre à jour. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="isConnected">
        /// Nouvel état de la session : <see langword="true"/> positionne la date de connexion
        /// et annule la date de déconnexion ; <see langword="false"/> positionne la date de
        /// déconnexion.
        /// </param>
        /// <param name="appContext">
        /// Contexte applicatif courant fournissant les informations device et l'horodatage
        /// applicatif. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="session"/> ou <paramref name="appContext"/> est <see langword="null"/>.</exception>
        /// <exception cref="Ex_Infrastructure">Levée si l'écriture en base échoue lors de la délégation au Command Handler.</exception>
        public async Task ExecuteAsync(
            string caller,
            UserAppSession session,
            bool isConnected,
            DTO_AppContext appContext,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (session is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "La session à mettre à jour est obligatoire.");

                if (appContext is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le contexte applicatif est obligatoire pour la mise à jour d'une session.");

                ct.ThrowIfCancellationRequested();

                session.DeviceUser = appContext.AppDeviceUser;
                session.DeviceId = appContext.AppDeviceId;
                session.DeviceIp = appContext.AppDeviceIP;

                session.IsConnected = isConnected;

                if (isConnected)
                {
                    session.ConnectionDate = appContext.AppDateTime;
                    session.DisconnectionDate = null;
                }
                else
                {
                    session.DisconnectionDate = appContext.AppDateTime;
                }

                await _commandHandler.HandleUpdateAsync(callChain, session, ct);
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