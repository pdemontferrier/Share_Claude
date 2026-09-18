using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Services.User;
using LeitTemporImport.A_Domain.Interfaces.Settings.User;
using LeitTemporImport.A_Domain.Interfaces.UseCases.User;

namespace LeitTemporImport.B_UseCases.UseCases.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// UseCase d’identification et de contrôle d’ouverture de l’application console.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Exécuté au démarrage (Program) avant les imports.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir que l’utilisateur du poste est reconnu. En cas d’échec, journaliser et
    /// retourner <c>0</c> afin que l’application interrompe proprement le flux.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Application console d’import (projet 104).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lire le login Windows via <c>IS_Settings_User</c>.</description></item>
    /// <item><description>Vérifier l’utilisateur via <c>IS_User_CheckAppDeviceUser</c>.</description></item>
    /// <item><description>Journaliser une erreur métier si l’utilisateur n’est pas reconnu.</description></item>
    /// <item><description>Retourner l’identifiant utilisateur ou <c>0</c>.</description></item>
    /// </list>
    /// </summary>
    public class UC_UserIdentify : IU_UserIdentify
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du UseCase pour la traçabilité.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_User _settingsUser;
        private readonly IS_User_CheckAppDeviceUser _checkAppDeviceUser;
        private readonly IS_ErrorLogger _errorLogger;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le UseCase d’identification utilisateur.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI.</para>
        /// <para>Objectif</para>
        /// <para>Valider les dépendances et initialiser la traçabilité.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider les dépendances non nulles.</description></item>
        /// </list>
        /// <param name="settingsUser">Accès aux informations utilisateur du poste.</param>
        /// <param name="checkAppDeviceUser">Service de vérification de l’utilisateur applicatif.</param>
        /// <param name="errorLogger">Service de journalisation centralisée.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est nulle.</exception>
        /// </summary>
        public UC_UserIdentify(
            ISE_User settingsUser,
            IS_User_CheckAppDeviceUser checkAppDeviceUser,
            IS_ErrorLogger errorLogger)
        {
            _callee = GetType().Name;

            _settingsUser = settingsUser ?? throw new ArgumentNullException(nameof(settingsUser));
            _checkAppDeviceUser = checkAppDeviceUser ?? throw new ArgumentNullException(nameof(checkAppDeviceUser));
            _errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Identifie l’utilisateur applicatif au démarrage.</para>
        /// <para>Contexte</para>
        /// <para>Appelé par Program avant tout traitement.</para>
        /// <para>Objectif</para>
        /// <para>Retourner l’IdUser si reconnu, sinon <c>0</c> après journalisation.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Lire le login Windows.</description></item>
        /// <item><description>Vérifier l’utilisateur.</description></item>
        /// <item><description>Logger et retourner <c>0</c> en cas d’échec.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Id utilisateur si identifié, sinon 0.</returns>
        /// </summary>
        public async Task<int> ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                string? deviceUser = _settingsUser.GetAppDeviceUser();
                Console.WriteLine($"WindowsLogin input = '{deviceUser}' (len={deviceUser?.Length})");

                // 1) Validation d'entrée : deviceUser absent / blanc
                if (string.IsNullOrWhiteSpace(deviceUser))
                {
                    var ex = new Ex_Business(
                        callChain: callChain,
                        errorId: "No_AUTH_00",
                        errorException:
                            "Ouverture refusée : l'identifiant utilisateur du poste (AppDeviceUser) est vide ou non configuré.");

                    await _errorLogger.ExecuteAsync(callChain, ex, ct);
                    return 0;
                }

                // 2) Contrôle métier
                int userId = await _checkAppDeviceUser.ExecuteAsync(callChain, deviceUser, ct);

                if (userId == 0)
                {
                    var ex = new Ex_Business(
                        callChain: callChain,
                        errorId: "No_AUTH_01",
                        errorException: $"Ouverture refusée, utilisateur '{deviceUser}' non identifié sur ce poste.");

                    await _errorLogger.ExecuteAsync(callChain, ex, ct);
                    return 0;
                }

                Console.WriteLine($"Utilisateur connecté : {userId}");
                return userId;
            }
            catch (Exception ex)
            {
                // Console batch : on journalise et on retourne 0 pour interrompre proprement.
                await _errorLogger.ExecuteAsync(callChain, ex, ct);
                return 0;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}