using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;

namespace BatchCutting_DG.B_UseCases.Services.UserLogic
{
    public class SR_UserSession : IS_UserSession
    {
        private readonly string ServiceName;
        private readonly IQ_UserSession _qhUserSession;
        private readonly IC_UserSession _chUserSession;
        private readonly IS_Notification _notification;
        private readonly IS_Settings _settings;

        public SR_UserSession(IC_UserSession usCommand, IQ_UserSession usQuery,
                                    IS_Notification notification, IS_Settings settings)
        {
            ServiceName = GetType().Name;
            _chUserSession = usCommand;
            _qhUserSession = usQuery;
            _notification = notification;
            _settings = settings;
        }


        public async Task OpenUserSessionAsync(int userId, int appId, string sessionText)
        {
            try
            {
                // Vérifier les sessions existantes via le repository
                var existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(userId, appId);

                if (existingSessions.Any())
                {
                    // Mettre à jour la session existante
                    // La valeur de IsConnected est normalement true, cependant false est la valeur par défaut pour l'instant
                    await _chUserSession.HandleUpdateUserSessionAsync(existingSessions.First(), false, ServiceName, nameof(OpenUserSessionAsync));

                    // Supprimer les sessions supplémentaires si nécessaire
                    await _chUserSession.HandleDeleteAdditionalSessions(existingSessions.Skip(1), ServiceName, nameof(OpenUserSessionAsync));
                }
                else
                {
                    // Créer une nouvelle session
                    await _chUserSession.HandleCreateNewUserSessionAsync(ServiceName, nameof(OpenUserSessionAsync));
                }

                // Mettre à jour SessionId
                existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(userId, appId);
                if (existingSessions.Any())
                {
                    // Met à jour la session existante
                    var entity = existingSessions.First();
                    _settings.SetSessionId(entity.Id);
                }
            }
            catch (Exception ex) { _notification.Error("No_Er_04", ex.Message); }
        }


        public async Task CloseUserSessionAsync(int sessionId)
        {
            try
            {
                if (sessionId > 0)
                {
                    // Charge la session de l'utilisateur
                    var existingSession = await _qhUserSession.HandleGetByIdAsync(sessionId);
                    if (existingSession != null)
                    {
                        // Mettre à jour la session existante
                        await _chUserSession.HandleUpdateUserSessionAsync(existingSession, false, ServiceName, nameof(CloseUserSessionAsync));
                    }
                }
            }
            catch (Exception ex) { _notification.Error("No_Er_04", ex.Message); }
        }
    }
}