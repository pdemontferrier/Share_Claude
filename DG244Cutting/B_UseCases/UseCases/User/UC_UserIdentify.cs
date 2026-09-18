using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// Description :
    /// <para>
    /// UseCase applicatif responsable de l’identification de l’utilisateur courant
    /// et de la vérification de son autorisation d’accès à l’application.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Ce UseCase est appelé au démarrage de l’application WPF ou lors d’un
    /// scénario nécessitant de valider l’identité applicative de l’utilisateur
    /// connecté sur le poste Windows. Il orchestre les services de contrôle
    /// de l’utilisateur et de contrôle d’accès applicatif.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Retourner l’identifiant de l’utilisateur applicatif si celui-ci est
    /// reconnu et autorisé à accéder à l’application ; sinon retourner <c>0</c>.
    /// Toute anomalie est gérée via le mécanisme centralisé
    /// <see cref="IU_LogAndNotify"/>.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// ViewModels et mécanismes applicatifs de démarrage nécessitant un contrôle
    /// d’identité et d’autorisation utilisateur.
    /// </para>
    ///
    /// Tâches / Actions :
    /// <list type="bullet">
    /// <item><description>Récupérer l’identifiant utilisateur du poste via <see cref="ISE_User"/>.</description></item>
    /// <item><description>Récupérer l’identifiant de l’application via <see cref="ISE_App"/>.</description></item>
    /// <item><description>Vérifier si l’utilisateur du poste correspond à un utilisateur applicatif connu.</description></item>
    /// <item><description>Vérifier si cet utilisateur possède un accès à l’application.</description></item>
    /// <item><description>Retourner l’identifiant utilisateur si le contrôle est valide.</description></item>
    /// <item><description>Centraliser les erreurs via <see cref="IU_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public class UC_UserIdentify : IU_UserIdentify
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_User _seUser;
        private readonly ISE_App _seApp;
        private readonly IS_User_CheckDeviceUser _checkDeviceUser;
        private readonly IS_User_CheckAppAccess _checkAppAccess;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance du UseCase <see cref="UC_UserIdentify"/>.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette classe est instanciée via l’injection de dépendances dans la couche
        /// Application. Elle reçoit les services et settings nécessaires à
        /// l’identification de l’utilisateur et à la vérification de l’accès
        /// applicatif.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Préparer le UseCase à exécuter un scénario complet de contrôle
        /// d’identité utilisateur et d’autorisation d’accès.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Initialiser la variable <c>_callee</c>.</description></item>
        /// <item><description>Recevoir et stocker les dépendances nécessaires.</description></item>
        /// </list>
        /// </summary>
        /// <param name="seUser">Settings utilisateur du poste.</param>
        /// <param name="seApp">Settings applicatifs.</param>
        /// <param name="checkDeviceUser">Service de vérification de l’utilisateur du poste.</param>
        /// <param name="checkAppAccess">Service de vérification d’accès utilisateur / application.</param>
        /// <param name="logAndNotify">Service centralisé de log et notification.</param>
        /// <exception cref="ArgumentNullException">Levée si une dépendance obligatoire est nulle.</exception>
        public UC_UserIdentify(
            ISE_User seUser,
            ISE_App seApp,
            IS_User_CheckDeviceUser checkDeviceUser,
            IS_User_CheckAppAccess checkAppAccess,
            IU_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;

            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _seApp = seApp ?? throw new ArgumentNullException(nameof(seApp));
            _checkDeviceUser = checkDeviceUser ?? throw new ArgumentNullException(nameof(checkDeviceUser));
            _checkAppAccess = checkAppAccess ?? throw new ArgumentNullException(nameof(checkAppAccess));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>
        /// Identifie l’utilisateur applicatif courant et vérifie son autorisation
        /// d’accès à l’application.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode est appelée depuis un ViewModel ou un mécanisme de démarrage
        /// afin de contrôler que l’utilisateur Windows du poste est bien reconnu
        /// dans le système applicatif et qu’il dispose des droits d’accès
        /// nécessaires.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner l’identifiant utilisateur si le contrôle est valide ; sinon
        /// retourner <c>0</c>. Toute erreur est centralisée via
        /// <see cref="IU_LogAndNotify"/>.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Construire la CallChain du UseCase.</description></item>
        /// <item><description>Récupérer le nom utilisateur du poste.</description></item>
        /// <item><description>Vérifier l’existence de l’utilisateur applicatif correspondant.</description></item>
        /// <item><description>Vérifier l’autorisation d’accès à l’application.</description></item>
        /// <item><description>Retourner l’identifiant utilisateur si tout est conforme.</description></item>
        /// <item><description>Gérer les erreurs métier et techniques via <see cref="IU_LogAndNotify"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// Identifiant utilisateur si l’utilisateur est reconnu et autorisé ; sinon <c>0</c>.
        /// </returns>
        public async Task<int> ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                string? deviceUser = _seUser.AppDeviceUser;

                if (string.IsNullOrWhiteSpace(deviceUser))
                {
                    throw new Ex_Business(
                        callChain: callChain,
                        errorId: "AUTH_01",
                        errorException: "No_Er_Bu_01");
                }

                int appId = _seApp.AppId;

                int userId = await _checkDeviceUser.ExecuteAsync(deviceUser, callChain, ct);

                if (userId <= 0)
                {
                    throw new Ex_Business(
                        callChain: callChain,
                        errorId: "AUTH_02",
                        errorException: "No_Er_Bu_02");
                }

                bool hasAccess = await _checkAppAccess.ExecuteAsync(userId, appId, callChain, ct);

                if (!hasAccess)
                {
                    throw new Ex_Business(
                        callChain: callChain,
                        errorId: "AUTH_03",
                        errorException: "No_Er_Bu_03");
                }

                return userId;
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_18", ex, true, ct);
                return 0;
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_19", ex, true, ct);
                return 0;
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_20", ex, true, ct);
                return 0;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}