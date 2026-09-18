using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic;

namespace BatchStockRelease.B_UseCases.UseCases.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// UseCase orchestrateur responsable du processus complet de démarrage de l’application.
    /// Il regroupe et pilote les vérifications techniques et contextuelles nécessaires avant
    /// le chargement de la fenêtre principale.
    /// </para>
    /// 
    /// <b>Contexte :</b>
    /// <para>
    /// Ce UseCase est appelé lors du lancement initial de l’application WPF, depuis la méthode
    /// <see cref="App.Application_Startup"/> de la classe <see cref="App"/>.
    /// Il fait partie du cycle d’amorçage global, qui vise à garantir la stabilité, la connectivité
    /// et la validité de la session utilisateur avant d’afficher l’interface principale.
    /// </para>
    /// 
    /// <b>Objectif :</b>
    /// <para>
    /// S’assurer que toutes les conditions nécessaires à l’exécution sont réunies :
    /// connexion base de données, disponibilité applicative, cohérence de session, et paramétrage
    /// linguistique.
    /// Si toutes les conditions sont réunies, il met à jour le contexte global (titre, utilisateur)
    /// et autorise le lancement du <see cref="MainWindow"/>.
    /// </para>
    /// 
    /// <b>Utilisateurs cibles :</b> 
    /// <para>
    /// Magasiniers et opérateurs responsables de la préparation des barres avant découpe.
    /// </para>
    /// 
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Configurer la langue courante à partir du code culture système.</description></item>
    /// <item><description>Mettre à jour le titre et les informations de contexte utilisateur.</description></item>
    /// <item><description>Tester la connexion à la base de données.</description></item>
    /// <item><description>Vérifier l’absence de sessions actives sur d’autres postes.</description></item>
    /// <item><description>Contrôler l’accessibilité de l’application (verrou administrateur).</description></item>
    /// <item><description>Notifier et journaliser toute erreur rencontrée via <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public class UC_Application_OnStart : IU_Application_OnStart
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du UseCase pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_Language _language;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_StartupContextUpdater _startupContextUpdater;
        private readonly IS_StartupDatabaseConnectivity _databaseConnectivity;
        private readonly IS_UserSession_Integrity _sessionIntegrity;
        private readonly IS_ApplicationAvailability _applicationAvailability;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du UseCase <see cref="UC_Application_OnStart"/>.
        /// </summary>
        public UC_Application_OnStart(
            IS_Language language,
            IS_LogAndNotify logAndNotify,
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser,
            IS_StartupContextUpdater startupContextUpdater,
            IS_StartupDatabaseConnectivity databaseConnectivity,
            IS_UserSession_Integrity sessionIntegrity,
            IS_ApplicationAvailability applicationAvailability)
        {
            _callee = GetType().Name;

            _language = language;
            _logAndNotify = logAndNotify;
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
            _startupContextUpdater = startupContextUpdater;
            _databaseConnectivity = databaseConnectivity;
            _sessionIntegrity = sessionIntegrity;
            _applicationAvailability = applicationAvailability;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Exécute le scénario complet d’initialisation de l’application.
        /// </para>
        /// 
        /// <b>Contexte :</b>
        /// <para>
        /// Méthode appelée par la procédure de démarrage (<see cref="App.Application_Startup"/>)
        /// afin de vérifier la disponibilité de l’environnement et de préparer le contexte utilisateur.
        /// </para>
        /// 
        /// <b>Objectif :</b>
        /// <para>
        /// Garantir que l’application peut être ouverte en toute sécurité et dans un état stable.
        /// </para>
        /// 
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Configurer la langue courante à partir du code culture système.</description></item>
        /// <item><description>Tester la connexion à la base de données.</description></item>
        /// <item><description>Vérifier l’absence de sessions actives sur d’autres postes.</description></item>
        /// <item><description>Contrôler l’accessibilité de l’application (verrou administrateur).</description></item>
        /// <item><description>Mettre à jour le titre et les informations de contexte utilisateur.</description></item>
        /// <item><description>Notifier et journaliser toute erreur rencontrée via <see cref="IS_LogAndNotify"/>.</description></item>
        /// </list>
        /// <param name="cultureCode">Code de la culture courante (ex. "fr-FR").</param>
        /// <param name="caller">Chaîne d’appelante utilisée pour la traçabilité (CallChain).</param>
        /// <returns><c>true</c> si l’initialisation réussit, sinon <c>false</c>.</returns>
        /// <exception cref="Ex_Business">Si une règle métier est violée.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur d’accès aux ressources est détectée.</exception>
        /// </summary>
        public async Task<bool> ExecuteAsync(string cultureCode, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1️ : Configuration de la langue
                _language.Execute(cultureCode);

                // Étape 2 : Mise à jour du contexte de démarrage
                await _startupContextUpdater.ExecuteAsync(callChain);

                // Étape 3 : Vérification de la connectivité à la base de données
                bool dbOk = await _databaseConnectivity.TestConnectionAsync(callChain);
                if (!dbOk)
                {
                    return false;
                }

                // Étape 4 : Vérification d'une session utilisateur active
                bool sessionConflict = await _sessionIntegrity.HasActiveSessionOnAnotherDeviceAsync(
                    _settingsApp.GetAppId(),
                    _settingsUser.GetAppUserId(), 
                    _settingsUser.GetAppDeviceId(), 
                    callChain);
                if (sessionConflict)
                {
                    // Si il y a un conflit cela implique que le poste est déjà connecté.
                    await _logAndNotify.ExecuteAsync("No_EC_18", new Ex_Business(callChain, "STAR_02", "No_Er_Bu_09"));
                    return false;
                }

                // Étape 5 : Vérification de la disponibilité applicative
                bool appAvailable = await _applicationAvailability.IsAppAccessibleAsync(_settingsApp.GetAppId(), callChain);
                if (!appAvailable)
                {
                    return false;
                }

                // Étape 6 : Démarrage autorisé
                return true;
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
                return false;
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
                return false;
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
                return false;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}