using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase d'authentification d'un utilisateur par identifiant de connexion et mot de passe,
    /// en repli de la page de connexion.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Implémentation du contrat <see cref="IU_UserAuthenticate"/> résidant en
    /// <c>B_UseCases/UseCases/User/</c> conformément à la deuxième obligation contractuelle de
    /// §4.14.2 amendée et à la note [b] du tableau de §2.8 du 0230. Consommé par le ViewModel de la
    /// page de connexion <c>VM_Page00</c> (chaîne (1) d'écriture VM → UC) à la validation du
    /// formulaire, en repli lorsque l'identification automatique par contexte poste (login Windows)
    /// n'a pas abouti au démarrage. Le composant est le pendant, en repli manuel login/mot de passe,
    /// de l'identification automatique portée par <see cref="IU_UserIdentify"/>.</para>
    /// <para>Objectif : Orchestrer l'authentification manuelle — validation du couple login/mot de
    /// passe puis, en cas de succès, établissement de l'identité applicative de l'utilisateur,
    /// ouverture de sa session applicative et application de ses droits de pages — et signaler par
    /// valeur booléenne le verdict d'authentification au ViewModel appelant, sur lequel celui-ci
    /// branche (navigation vers la page applicative si <see langword="true"/>, incrément du compteur
    /// de tentatives si <see langword="false"/>).</para>
    /// <para>Nature transactionnelle (item UC14) : UseCase non transactionnel par construction.
    /// Aucune mutation EF Core n'est portée — la lecture par login s'opère via le Query Handler sans
    /// tracking, la comparaison d'empreinte est un calcul en mémoire, et le positionnement de
    /// <c>ISE_User.AppUserId</c> est une écriture de Setting (hors <c>DbContext</c> partagé). Les
    /// écritures persistantes sont déléguées aux sous-UseCases consommés, chacun portant sa propre
    /// transaction sans imbrication (I-4.10.3). Aucun <c>BeginTransactionAsync</c>,
    /// <c>SaveChangesAsync</c>, <c>CommitAsync</c> ni <c>RollbackAsync</c>.</para>
    /// <para>Chaîne UC → UC normalisée (R-4.14.21, item UC22) : le UseCase consomme deux UseCases en
    /// sous-séquence — <see cref="IU_UserAppSession_Open"/> puis
    /// <see cref="IU_UserAppPageRight_Apply"/> — par injection de leurs interfaces au constructeur.
    /// Les trois conditions doctrinales conjointes sont vérifiées par construction : retour signalable
    /// <c>Task&lt;bool&gt;</c> exploité par valeur, indépendance transactionnelle conforme à I-4.10.3
    /// (les deux UseCases consommés portent leur propre périmètre transactionnel, le présent UseCase
    /// et <see cref="IU_UserAppPageRight_Apply"/> étant non transactionnels par construction), et
    /// traitement terminal propre via le pipeline <c>IU_LogAndNotify</c> interne à chaque UseCase
    /// consommé. Patron de propagation directe par valeur : le retour <see langword="false"/> d'un
    /// sous-UseCase est propagé directement au <c>return false</c> de <see cref="ExecuteAsync"/>,
    /// précédé d'une remise à zéro ciblée de <c>ISE_User.AppUserId</c> pour garantir la cohérence
    /// d'état — jamais <c>Reset()</c>, qui écraserait le compteur de tentatives géré par le ViewModel.
    /// Ce patron évite en outre la double notification utilisateur, la notification d'échec étant déjà
    /// portée par le pipeline interne du sous-UseCase.</para>
    /// <para>Gestion terminale des erreurs : les issues métier attendues — login inconnu ou compte
    /// inactif ou supprimé (indistinctement), mot de passe erroné, échec d'ouverture de session,
    /// échec d'application des droits — sont signalées par <c>return false</c> direct dans le
    /// <c>try</c>, sans levée d'exception ni notification. Les anomalies applicatives typées sont
    /// captées par la cascade de trois blocs <c>catch</c> (<see cref="Ex_Business"/> → clé
    /// <c>"No_EC_01"</c>, <see cref="Ex_Infrastructure"/> → clé <c>"No_EC_02"</c>,
    /// <see cref="Ex_Unclassified"/> → clé <c>"No_EC_03"</c>) déléguant à <c>IU_LogAndNotify</c> puis
    /// retournant <see langword="false"/>, complétée d'un bloc
    /// <c>catch (OperationCanceledException) { throw; }</c> propageant l'annulation coopérative sans
    /// requalification (R-4.6.13). Aucun bloc <c>catch (Exception ex)</c> terminal (item UC12). La
    /// garde de saisie vide (<see cref="Ex_Business.ErrorCodes.BU_ER_02"/>) est captée par le bloc
    /// <c>catch (Ex_Business)</c>.</para>
    /// <para>Particularité du retour <c>Task&lt;bool&gt;</c> au regard de R-4.14.21 (item UC21) : le
    /// retour signalable n'est pas la marque d'une consommation en sous-séquence par un orchestrateur
    /// UseCase amont — le consommateur amont effectif est le ViewModel <c>VM_Page00</c>, situé hors de
    /// la famille UseCases. Item UC21 marqué ➖.</para>
    /// </remarks>
    /// <seealso cref="IU_UserAuthenticate"/>
    /// <seealso cref="IU_UserIdentify"/>
    /// <seealso cref="IU_UserAppSession_Open"/>
    /// <seealso cref="IU_UserAppPageRight_Apply"/>
    public class UC_UserAuthenticate : IU_UserAuthenticate
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du composant courant, résolu dynamiquement par <c>GetType().Name</c> pour la
        /// construction du segment local de la CallChain (§4.5 du 0230 ; R-4.5.5).
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserApp _userAppQuery;
        private readonly IS_Hashing _hashing;
        private readonly ISE_User _settingsUser;
        private readonly IU_UserAppSession_Open _userAppSessionOpen;
        private readonly IU_UserAppPageRight_Apply _userAppPageRightApply;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du UseCase d'authentification manuelle.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur appelé par le conteneur d'injection de dépendances
        /// (<c>SR_ConteneurDI</c>) lors de la résolution du contrat <see cref="IU_UserAuthenticate"/>
        /// par le ViewModel de la page de connexion.</para>
        /// <para>Objectif : Câbler les six dépendances nécessaires à l'orchestration du scénario
        /// d'authentification (lecture par login, hachage, contexte utilisateur partagé, ouverture de
        /// session et application des droits en sous-séquence, pipeline terminal d'erreurs).</para>
        /// </remarks>
        /// <param name="userAppQuery">Query Handler de l'entité <c>UserApp</c> (résolution par login).</param>
        /// <param name="hashing">Service de calcul d'empreinte du mot de passe saisi.</param>
        /// <param name="settingsUser">Setting Singleton de contexte utilisateur partagé (identité courante).</param>
        /// <param name="userAppSessionOpen">UseCase orchestré en sous-séquence pour l'ouverture de la session applicative (R-4.14.21).</param>
        /// <param name="userAppPageRightApply">UseCase orchestré en sous-séquence pour l'application des droits de pages (R-4.14.21).</param>
        /// <param name="logAndNotify">Pipeline terminal d'erreurs invoqué dans les blocs catch typés.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserAuthenticate(
            IQ_UserApp userAppQuery,
            IS_Hashing hashing,
            ISE_User settingsUser,
            IU_UserAppSession_Open userAppSessionOpen,
            IU_UserAppPageRight_Apply userAppPageRightApply,
            IU_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;

            _userAppQuery = userAppQuery ?? throw new ArgumentNullException(nameof(userAppQuery));
            _hashing = hashing ?? throw new ArgumentNullException(nameof(hashing));
            _settingsUser = settingsUser ?? throw new ArgumentNullException(nameof(settingsUser));
            _userAppSessionOpen = userAppSessionOpen ?? throw new ArgumentNullException(nameof(userAppSessionOpen));
            _userAppPageRightApply = userAppPageRightApply ?? throw new ArgumentNullException(nameof(userAppPageRightApply));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <inheritdoc/>
        public async Task<bool> ExecuteAsync(string caller, string login, string password, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Précondition structurelle sur les arguments métier, validée avant la vérification
                // d'annulation coopérative (ordre normatif validation → ct → opérations principales,
                // §4.7 du 0230 ; R-4.7.25).
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        "Le login ou le mot de passe saisi est null, vide ou composé uniquement d'espaces blancs.");

                ct.ThrowIfCancellationRequested();

                // Résolution de l'utilisateur actif et non supprimé associé au login. Un retour null
                // (login inconnu, compte inactif ou supprimé, indistinctement) est une issue métier
                // attendue : sortie par return false sans notification.
                UserApp? user = await _userAppQuery.HandleGetByLoginAsync(callChain, login, ct);

                if (user is null)
                    return false;

                // Comparaison ordinale de l'empreinte du mot de passe saisi à l'empreinte stockée
                // (empreinte hexadécimale minuscule ; le mot de passe n'est jamais comparé en clair).
                string passwordHash = _hashing.Hash(password);

                if (!string.Equals(passwordHash, user.PasswordHash, StringComparison.Ordinal))
                    return false;

                // Établissement de l'identité applicative courante avant l'ouverture de session et
                // l'application des droits : ces deux sous-scénarios lisent l'identité depuis l'état
                // applicatif partagé, sans paramètre d'identifiant.
                _settingsUser.AppUserId = user.Id;
                if (user.Id > 0)
                {
                    string fullName = $"{user.FirstName} {user.LastName}";
                    if (!string.IsNullOrWhiteSpace(fullName))
                        _settingsUser.AppUserFullName = fullName;
                }

                // Sous-séquence 1 — ouverture de session (chaîne UC → UC normalisée, R-4.14.21).
                // Propagation directe par valeur du retour signalable ; sur échec, remise à zéro
                // ciblée de l'identité (jamais Reset(), qui écraserait le compteur de tentatives).
                bool sessionOpened = await _userAppSessionOpen.ExecuteAsync(callChain, ct);

                if (!sessionOpened)
                {
                    _settingsUser.AppUserId = 0;
                    return false;
                }

                // Sous-séquence 2 — application des droits de pages (chaîne UC → UC normalisée,
                // R-4.14.21). Même patron de propagation directe et de remise à zéro ciblée sur échec.
                bool pageRightApplied = await _userAppPageRightApply.ExecuteAsync(callChain, ct);

                if (!pageRightApplied)
                {
                    _settingsUser.AppUserId = 0;
                    return false;
                }

                return true;
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); return false; }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); return false; }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); return false; }
            catch (OperationCanceledException) { throw; }
        }

        #endregion

        #region === Méthodes privées ===

        // Aucune méthode privée : la séquence d'authentification est portée intégralement par
        // ExecuteAsync (orchestration linéaire courte).

        #endregion
    }
}