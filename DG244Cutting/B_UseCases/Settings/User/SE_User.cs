using System.ComponentModel;
using System.Runtime.CompilerServices;
using DG244Cutting.A_Domain.Entities.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;

namespace DG244Cutting.B_UseCases.Settings.User
{
    /// <summary>
    /// Représente l'état utilisateur courant partagé par l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant Singleton transversal injectable via <see cref="ISE_User"/>,
    /// enregistré dans <c>E_Miscellaneous/CompositionRoot/SR_ConteneurDI.cs</c>.</para>
    /// <para>Objectif : Centraliser un état utilisateur partagé, observable et cohérent,
    /// sans porter de logique métier ni de logique technique d'acquisition. Les propriétés dont
    /// l'évolution doit rester cohérente (contexte poste, droits d'accès) sont exposées en
    /// lecture seule et leur écriture est canalisée par les méthodes publiques dédiées.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Stocker l'identité de l'utilisateur courant.</description></item>
    /// <item><description>Stocker le contexte du poste courant.</description></item>
    /// <item><description>Stocker l'état de session, de fermeture et d'accès applicatif.</description></item>
    /// <item><description>Stocker les droits d'accès aux pages déjà calculés par des composants externes.</description></item>
    /// <item><description>Notifier les observateurs lors des changements d'état via <see cref="INotifyPropertyChanged"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne pas charger les droits depuis une source externe.</description></item>
    /// <item><description>Ne pas résoudre directement les informations réseau ou système du poste.</description></item>
    /// <item><description>Ne pas orchestrer un flux applicatif.</description></item>
    /// </list>
    /// </remarks>
    public class SE_User : ISE_User
    {
        #region === Propriétés privées ===

        // --- Identité utilisateur ---
        private int _appUserId;
        private string _appUserFullName = string.Empty;

        // --- Contexte poste ---
        private string _appDeviceId = string.Empty;
        private string _appDeviceIP = string.Empty;
        private string _appDeviceUser = string.Empty;

        // --- État session et accès ---
        private int _appSessionId;
        private int _userAttempt;
        private string _closeCommandType = string.Empty;
        private bool _forceClose;
        private int _selectedSessionId;
        private string _selectedSessionFullName = string.Empty;
        private bool _canUserAccessApp;

        // --- Droits d'accès aux pages ---
        private readonly Dictionary<string, PageRights> _pagesUserRights = new();

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        // --- Identité utilisateur ---

        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur applicatif courant.
        /// </summary>
        public int AppUserId
        {
            get => _appUserId;
            set => SetField(ref _appUserId, value);
        }

        /// <summary>
        /// Obtient ou définit le nom complet de l'utilisateur courant.
        /// </summary>
        public string AppUserFullName
        {
            get => _appUserFullName;
            set => SetField(ref _appUserFullName, NormalizeString(value));
        }

        // --- Contexte poste (lecture seule, écriture via SetDeviceContext) ---

        /// <summary>
        /// Obtient l'identifiant du poste courant.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="SetDeviceContext"/>.
        /// </remarks>
        public string AppDeviceId => _appDeviceId;

        /// <summary>
        /// Obtient l'adresse IP du poste courant.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="SetDeviceContext"/>.
        /// </remarks>
        public string AppDeviceIP => _appDeviceIP;

        /// <summary>
        /// Obtient le nom du compte système du poste courant.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="SetDeviceContext"/>.
        /// </remarks>
        public string AppDeviceUser => _appDeviceUser;

        // --- État session et accès ---

        /// <summary>
        /// Obtient ou définit l'identifiant de la session applicative courante.
        /// </summary>
        public int AppSessionId
        {
            get => _appSessionId;
            set => SetField(ref _appSessionId, value);
        }

        /// <summary>
        /// Obtient ou définit le nombre de tentatives de connexion.
        /// </summary>
        public int UserAttempt
        {
            get => _userAttempt;
            set => SetField(ref _userAttempt, value);
        }

        /// <summary>
        /// Obtient ou définit le type de commande de fermeture demandé.
        /// </summary>
        public string CloseCommandType
        {
            get => _closeCommandType;
            set => SetField(ref _closeCommandType, NormalizeString(value));
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si une fermeture forcée est demandée.
        /// </summary>
        public bool ForceClose
        {
            get => _forceClose;
            set => SetField(ref _forceClose, value);
        }

        /// <summary>
        /// Obtient ou définit l'identifiant de la session sélectionnée.
        /// </summary>
        public int SelectedSessionId
        {
            get => _selectedSessionId;
            set => SetField(ref _selectedSessionId, value);
        }

        /// <summary>
        /// Obtient ou définit le nom complet associé à la session sélectionnée.
        /// </summary>
        public string SelectedSessionFullName
        {
            get => _selectedSessionFullName;
            set => SetField(ref _selectedSessionFullName, NormalizeString(value));
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'utilisateur peut accéder à l'application.
        /// </summary>
        public bool CanUserAccessApp
        {
            get => _canUserAccessApp;
            set => SetField(ref _canUserAccessApp, value);
        }

        // --- Droits d'accès aux pages (lecture seule, écriture via méthodes dédiées) ---

        /// <summary>
        /// Obtient les droits d'accès aux pages actuellement stockés.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Cette propriété est consultée par les composants qui doivent lire l'état courant des droits.</para>
        /// <para>Objectif : Retourner une vue en lecture seule de l'état interne, sans allocation défensive à chaque accès.</para>
        /// <para>Écriture exclusive via <see cref="SetPageAccessRight"/> et <see cref="ClearPageAccessRights"/>.</para>
        /// </remarks>
        public IReadOnlyDictionary<string, PageRights> PagesUserRights => _pagesUserRights;

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        /// <summary>
        /// Émis lors d'une modification effective d'une propriété mutable.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Déclenché par le helper <see cref="SetField{T}"/> pour les setters
        /// de propriétés scalaires et pour les opérations atomiques multi-propriétés
        /// (<see cref="SetDeviceContext"/>, <see cref="Reset"/>), et directement par
        /// <see cref="SetPageAccessRight"/> et <see cref="ClearPageAccessRights"/> lors des mutations
        /// du dictionnaire <see cref="PagesUserRights"/> (pour lesquelles <see cref="SetField{T}"/>
        /// n'est pas applicable, le champ support n'étant pas réaffecté).</para>
        /// <para>Objectif : Notifier les observateurs d'un changement d'état observable
        /// conformément au pattern <see cref="INotifyPropertyChanged"/>.</para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SE_User"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelé par le conteneur d'injection lors de la création du Singleton.</para>
        /// <para>Objectif : Garantir un état initial vide et cohérent, sans acquérir de données techniques ni métier.</para>
        /// </remarks>
        public SE_User()
        {
            Reset();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Définit de manière atomique le contexte du poste courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par un composant externe chargé de résoudre les informations techniques du poste.</para>
        /// <para>Objectif : Garantir la cohérence des trois composantes liées (identifiant, adresse IP, compte système)
        /// en invoquant directement <see cref="SetField{T}"/> sur chaque champ support, ce qui filtre les valeurs
        /// identiques et notifie uniquement les propriétés réellement modifiées, conformément à l'exemple canonique
        /// cité en §4.14.7 (« Usage du helper dans les opérations atomiques »).</para>
        /// </remarks>
        /// <param name="deviceId">Identifiant du poste.</param>
        /// <param name="deviceIP">Adresse IP du poste.</param>
        /// <param name="deviceUser">Compte système du poste.</param>
        public void SetDeviceContext(string deviceId, string deviceIP, string deviceUser)
        {
            SetField(ref _appDeviceId, NormalizeString(deviceId), nameof(AppDeviceId));
            SetField(ref _appDeviceIP, NormalizeString(deviceIP), nameof(AppDeviceIP));
            SetField(ref _appDeviceUser, NormalizeString(deviceUser), nameof(AppDeviceUser));
        }

        /// <summary>
        /// Incrémente le compteur de tentatives de connexion.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée après un échec d'authentification.</para>
        /// <para>Objectif : Faire évoluer le compteur via un point d'entrée unique et cohérent ;
        /// l'écriture passe par le setter public de <see cref="UserAttempt"/>, qui invoque <see cref="SetField{T}"/>.</para>
        /// </remarks>
        public void IncrementUserAttempt()
        {
            UserAttempt++;
        }

        /// <summary>
        /// Définit les droits d'accès d'une page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée après calcul ou chargement externe des droits applicables.</para>
        /// <para>Objectif : Mettre à jour de manière atomique les droits d'une page sans exposer l'état interne mutable.
        /// La mutation du dictionnaire support est suivie d'une émission directe de <see cref="PropertyChanged"/>
        /// (le helper <see cref="SetField{T}"/> ne s'applique pas, le champ support n'étant pas réaffecté).</para>
        /// </remarks>
        /// <param name="pageCode">Code de la page concernée.</param>
        /// <param name="pageRights">Droits à appliquer à la page.</param>
        /// <exception cref="ArgumentException">Levée lorsque <paramref name="pageCode"/> est vide ou blanc.</exception>
        /// <exception cref="ArgumentNullException">Levée lorsque <paramref name="pageRights"/> est <see langword="null"/>.</exception>
        public void SetPageAccessRight(string pageCode, PageRights pageRights)
        {
            if (string.IsNullOrWhiteSpace(pageCode))
            {
                throw new ArgumentException("The page code cannot be null, empty, or whitespace.", nameof(pageCode));
            }

            if (pageRights is null)
            {
                throw new ArgumentNullException(nameof(pageRights));
            }

            _pagesUserRights[pageCode.Trim()] = ClonePageRights(pageRights);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PagesUserRights)));
        }

        /// <summary>
        /// Vide les droits d'accès aux pages.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée avant l'application d'un nouveau jeu de droits calculé par un composant externe.</para>
        /// <para>Objectif : Éliminer tout état de droit obsolète avant ré-alimentation.
        /// La mutation du dictionnaire support est suivie d'une émission directe de <see cref="PropertyChanged"/>
        /// (le helper <see cref="SetField{T}"/> ne s'applique pas, le champ support n'étant pas réaffecté).</para>
        /// </remarks>
        public void ClearPageAccessRights()
        {
            _pagesUserRights.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PagesUserRights)));
        }

        /// <summary>
        /// Obtient les droits d'accès d'une page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lorsqu'un composant doit consulter les droits actuellement stockés pour une page.</para>
        /// <para>Objectif : Retourner une copie défensive des droits afin de préserver l'intégrité de l'état interne.</para>
        /// </remarks>
        /// <param name="pageCode">Code de la page recherchée.</param>
        /// <returns>Une copie des droits de la page, ou <see langword="null"/> si la page n'est pas connue.</returns>
        public PageRights? GetPageRights(string pageCode)
        {
            if (string.IsNullOrWhiteSpace(pageCode))
            {
                return null;
            }

            return _pagesUserRights.TryGetValue(pageCode.Trim(), out PageRights? pageRights)
                ? ClonePageRights(pageRights)
                : null;
        }

        /// <summary>
        /// Réinitialise l'état utilisateur partagé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée à l'initialisation depuis le constructeur, ou pour
        /// remettre à zéro le contexte utilisateur lors d'une déconnexion ou d'un redémarrage logique.</para>
        /// <para>Objectif : Restaurer un état neutre, cohérent et reproductible. Les propriétés scalaires
        /// sont réinitialisées par invocation directe de <see cref="SetField{T}"/> avec <c>nameof</c>, conformément
        /// au style atomique strict de R-4.14.15 ; les composantes sous régime atomique (contexte poste, droits
        /// d'accès) sont déléguées à leurs opérations dédiées <see cref="SetDeviceContext"/> et
        /// <see cref="ClearPageAccessRights"/>. Seules les propriétés dont la valeur change effectivement émettent
        /// une notification.</para>
        /// </remarks>
        public void Reset()
        {
            SetField(ref _appUserId, 0, nameof(AppUserId));
            SetField(ref _appUserFullName, string.Empty, nameof(AppUserFullName));

            SetDeviceContext(string.Empty, string.Empty, string.Empty);

            SetField(ref _appSessionId, 0, nameof(AppSessionId));
            SetField(ref _userAttempt, 0, nameof(UserAttempt));
            SetField(ref _closeCommandType, string.Empty, nameof(CloseCommandType));
            SetField(ref _forceClose, false, nameof(ForceClose));
            SetField(ref _selectedSessionId, 0, nameof(SelectedSessionId));
            SetField(ref _selectedSessionFullName, string.Empty, nameof(SelectedSessionFullName));
            SetField(ref _canUserAccessApp, false, nameof(CanUserAccessApp));

            ClearPageAccessRights();
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Met à jour un champ de stockage et déclenche la notification <see cref="PropertyChanged"/>
        /// si la valeur a changé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisée par tous les setters publics de propriétés mutables (résolution
        /// automatique du nom de propriété via <see cref="CallerMemberNameAttribute"/>) et par les opérations
        /// atomiques multi-propriétés <see cref="SetDeviceContext"/> et <see cref="Reset"/> (passage explicite
        /// du nom de propriété via <c>nameof</c>).</para>
        /// <para>Objectif : Centraliser le triptyque canonique « comparaison de valeur, écriture du champ
        /// support, émission de la notification <see cref="PropertyChanged"/> » conformément à la signature
        /// canonique invariante définie en §4.14.7 (clause « Signature canonique du helper SetField »).</para>
        /// </remarks>
        /// <typeparam name="T">Type de la valeur stockée.</typeparam>
        /// <param name="field">Référence au champ de stockage privé.</param>
        /// <param name="value">Nouvelle valeur à appliquer.</param>
        /// <param name="propertyName">Nom de la propriété appelante, résolu automatiquement par
        /// <see cref="CallerMemberNameAttribute"/> pour les setters, ou passé explicitement via <c>nameof</c>
        /// pour les opérations atomiques.</param>
        /// <returns><see langword="true"/> si la valeur a effectivement changé ; <see langword="false"/> sinon.</returns>
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        /// <summary>
        /// Normalise une valeur de chaîne en supprimant les espaces de bordure et en convertissant
        /// <see langword="null"/> en chaîne vide.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisée dans les setters des propriétés string et dans
        /// <see cref="SetDeviceContext"/> pour garantir une valeur valide et cohérente.</para>
        /// <para>Objectif : Prévenir toute assignation de valeur nulle ou contenant des
        /// espaces parasites sur les propriétés string exposées.</para>
        /// </remarks>
        /// <param name="value">Valeur à normaliser.</param>
        /// <returns>Valeur normalisée, ou <see cref="string.Empty"/> si la valeur fournie est <see langword="null"/>.</returns>
        private static string NormalizeString(string? value)
            => value?.Trim() ?? string.Empty;

        /// <summary>
        /// Clone un objet <see cref="PageRights"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de l'exposition ou du stockage des droits de page.</para>
        /// <para>Objectif : Empêcher toute modification externe directe de l'état interne mutable.</para>
        /// </remarks>
        /// <param name="source">Objet source à cloner.</param>
        /// <returns>Nouvelle instance contenant les mêmes valeurs.</returns>
        /// <exception cref="ArgumentNullException">Levée lorsque <paramref name="source"/> est <see langword="null"/>.</exception>
        private static PageRights ClonePageRights(PageRights source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new PageRights
            {
                CanAccess = source.CanAccess,
                CanCreate = source.CanCreate,
                CanRead = source.CanRead,
                CanUpdate = source.CanUpdate,
                CanDelete = source.CanDelete,
                CanControl = source.CanControl,
                CanValidate = source.CanValidate,
                CanSupervise = source.CanSupervise,
                CanMonitor = source.CanMonitor,
                CanAdmin = source.CanAdmin
            };
        }

        #endregion
    }
}