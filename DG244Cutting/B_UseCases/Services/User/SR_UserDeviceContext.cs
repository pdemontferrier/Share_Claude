using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.Settings.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Alimente le contexte poste de l'utilisateur courant à partir de l'environnement local.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Service appartenant à la couche applicative (B_UseCases) et résidant
    /// en <c>B_UseCases/Services/User/</c>. Il implémente le contrat
    /// <see cref="IS_UserDeviceContext"/> défini en <c>A_Domain/Interfaces/Services/User/</c>. Il
    /// est consommé par injection de dépendances par le UseCase orchestrateur d'initialisation
    /// du contexte utilisateur.</para>
    /// <para>Objectif : Résoudre le nom du poste, l'adresse IPv4 et l'utilisateur Windows
    /// courant, puis les injecter dans le paramètre utilisateur partagé <c>ISE_User</c> via
    /// l'opération atomique <c>SetDeviceContext</c>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Lire les informations techniques locales du poste.</description></item>
    /// <item><description>Mettre à jour <c>ISE_User</c> via une opération atomique.</description></item>
    /// <item><description>Encapsuler les détails techniques de résolution du poste.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne charge ni persiste de données applicatives.</description></item>
    /// <item><description>Ne calcule pas de droits utilisateur.</description></item>
    /// <item><description>N'orchestre pas un flux métier complet.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_UserDeviceContext"/>
    /// <seealso cref="IS_ExClassifier"/>
    public sealed class SR_UserDeviceContext : IS_UserDeviceContext
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_User _seUser;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur appelé par le conteneur d'injection de dépendances
        /// lors de la résolution du contrat <see cref="IS_UserDeviceContext"/>.</para>
        /// <para>Objectif : Fournir au service l'accès au paramètre utilisateur partagé à
        /// alimenter et au classifieur d'exceptions transversal pour la requalification terminale
        /// du patron canonique à quatre catch (§4.7, §4.7.4).</para>
        /// </remarks>
        /// <param name="seUser">Paramètre utilisateur partagé (<c>ISE_User</c>) à alimenter via <c>SetDeviceContext</c>.</param>
        /// <param name="classifier">Service de classification des exceptions non prévues (service transversal d'utilité, §4.7.4).</param>
        /// <exception cref="ArgumentNullException">Levée lorsque l'un des paramètres est <see langword="null"/>.</exception>
        public SR_UserDeviceContext(ISE_User seUser, IS_ExClassifier classifier)
        {
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Alimente le contexte poste de l'utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode appelée lorsque l'application doit connaître les
        /// caractéristiques techniques du poste local. Elle est typiquement invoquée par le
        /// UseCase orchestrateur d'initialisation du contexte utilisateur.</para>
        /// <para>Objectif : Résoudre les informations techniques utiles (nom de poste,
        /// adresse IPv4, compte Windows) puis les injecter dans <c>ISE_User</c> via l'opération
        /// atomique <c>SetDeviceContext</c>.</para>
        /// <para>Note d'ordonnancement (R-4.7.25). Aucune précondition structurelle n'est
        /// requise sur les arguments d'entrée : <paramref name="caller"/> est purement une chaîne
        /// de traçabilité, et aucun argument métier n'est attendu. L'ordre canonique
        /// validation → ct → opérations se réduit donc à ct → opérations, le segment validation
        /// étant sans objet pour ce Concept.</para>
        /// </remarks>
        /// <param name="caller">Nom du composant appelant utilisé pour la propagation de la CallChain au format normatif.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        /// <exception cref="Ex_Infrastructure">
        /// Levée en cas de défaillance technique imprévue requalifiée par
        /// <see cref="IS_ExClassifier"/> dans le quatrième catch du patron canonique.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation coopérative est demandée via <paramref name="ct"/> ; propagée
        /// sans requalification (R-4.6.13).
        /// </exception>
        public Task ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Validation -> ct : aucune précondition structurelle (cf. note d'ordonnancement
                // dans la documentation XML) ; vérification d'annulation coopérative.
                ct.ThrowIfCancellationRequested();

                // Opérations principales - résolution puis alimentation atomique du Setting.
                string deviceId = ResolveDeviceId();
                string deviceIp = ResolveIPv4Address();
                string deviceUser = ResolveWindowsUser();

                _seUser.SetDeviceContext(deviceId, deviceIp, deviceUser);

                return Task.CompletedTask;
            }
            catch (Ex_Business) { throw; }                  // déjà typé -> laisser remonter
            catch (Ex_Infrastructure) { throw; }            // déjà typé -> laisser remonter
            catch (OperationCanceledException) { throw; }   // annulation -> jamais requalifier
            catch (Exception ex)
            {
                // Exception non prévue -> requalifier et relancer immédiatement (R-4.7.25).
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Résout l'identifiant du poste courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode appelée durant l'alimentation du contexte poste.</para>
        /// <para>Objectif : Retourner un nom de poste exploitable pour identifier la machine locale.</para>
        /// </remarks>
        /// <returns>Le nom du poste courant, ou une chaîne vide si aucune valeur exploitable n'est disponible.</returns>
        private static string ResolveDeviceId()
        {
            return Environment.MachineName?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Résout l'adresse IPv4 principale du poste courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode appelée durant l'alimentation du contexte poste.</para>
        /// <para>Objectif : Retourner la première adresse IPv4 opérationnelle trouvée sur une interface réseau active.</para>
        /// </remarks>
        /// <returns>L'adresse IPv4 trouvée, ou <c>127.0.0.1</c> si aucune adresse exploitable n'est disponible.</returns>
        private static string ResolveIPv4Address()
        {
            IEnumerable<NetworkInterface> interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni =>
                    ni.OperationalStatus == OperationalStatus.Up &&
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                    !ni.Description.Contains("virtual", StringComparison.OrdinalIgnoreCase) &&
                    !ni.Description.Contains("pseudo", StringComparison.OrdinalIgnoreCase));

            // Passe 1 : interface physique active avec gateway IPv4.
            foreach (NetworkInterface ni in interfaces)
            {
                IPInterfaceProperties ipProps = ni.GetIPProperties();

                bool hasIpv4Gateway = ipProps.GatewayAddresses.Any(g =>
                    g.Address.AddressFamily == AddressFamily.InterNetwork);

                if (!hasIpv4Gateway)
                {
                    continue;
                }

                string? address = ipProps.UnicastAddresses
                    .Where(ip =>
                        ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !ip.Address.ToString().StartsWith("127.", StringComparison.OrdinalIgnoreCase) &&
                        !ip.Address.ToString().StartsWith("169.254.", StringComparison.OrdinalIgnoreCase))
                    .Select(ip => ip.Address.ToString())
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(address))
                {
                    return address;
                }
            }

            // Passe 2 : fallback sur interface physique active sans exiger de gateway.
            foreach (NetworkInterface ni in interfaces)
            {
                IPInterfaceProperties ipProps = ni.GetIPProperties();

                string? address = ipProps.UnicastAddresses
                    .Where(ip =>
                        ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !ip.Address.ToString().StartsWith("127.", StringComparison.OrdinalIgnoreCase) &&
                        !ip.Address.ToString().StartsWith("169.254.", StringComparison.OrdinalIgnoreCase))
                    .Select(ip => ip.Address.ToString())
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(address))
                {
                    return address;
                }
            }

            return "127.0.0.1";
        }

        /// <summary>
        /// Résout l'utilisateur Windows courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode appelée durant l'alimentation du contexte poste.</para>
        /// <para>Objectif : Retourner le nom du compte Windows courant dans un format exploitable par l'application.</para>
        /// </remarks>
        /// <returns>Le nom du compte Windows courant, ou une chaîne vide si aucune valeur exploitable n'est disponible.</returns>
        private static string ResolveWindowsUser()
        {
            var name = WindowsIdentity.GetCurrent()?.Name?.Trim() ?? string.Empty;
            return name.Contains('\\') ? name.Split('\\').Last() : name;
        }

        #endregion
    }
}