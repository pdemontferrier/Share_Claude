using System.ComponentModel;
using System.Runtime.CompilerServices;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Composant Singleton de présentation centralisant l'état du drapeau de langue affiché.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant Singleton de présentation injectable via <see cref="ISE_Flag"/>,
    /// enregistré dans le Composition Root. Consommé exclusivement par les composants de présentation
    /// liés à la langue active (ViewModels, contrôles visuels).</para>
    /// <para>Objectif : Centraliser l'état du drapeau affiché, exposer le référentiel
    /// de référence des drapeaux disponibles, et notifier les consommateurs de tout changement
    /// via <see cref="INotifyPropertyChanged"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Stocker et exposer l'URI du drapeau actuellement affiché</item>
    /// <item>Exposer le référentiel des drapeaux indexé par code pays ISO 3166-1 alpha-2</item>
    /// <item>Résoudre un drapeau à partir d'un code pays avec repli explicite</item>
    /// <item>Notifier les consommateurs lors de tout changement de propriété</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier ni orchestration applicative</item>
    /// <item>Aucun accès aux données ou services externes</item>
    /// <item>Aucune propagation de CallChain</item>
    /// </list>
    /// </remarks>
    public class SE_Flag : ISE_Flag
    {
        #region === Propriétés privées ===

        private Uri _appFlagUri;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient l'URI du drapeau par défaut utilisé en cas de repli.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Valeur stable, définie au démarrage, utilisée lorsque
        /// le code pays n'est pas reconnu dans le référentiel.</para>
        /// <para>Objectif : Garantir un affichage de repli cohérent (drapeau France).</para>
        /// </remarks>
        public Uri DefaultFlagUri { get; } = RS_Flags.DefaultFlag_Source;

        /// <summary>
        /// Obtient ou définit l'URI du drapeau actuellement affiché dans l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété centrale consommée par les ViewModels et composants
        /// visuels liés à la langue active de l'application.</para>
        /// <para>Objectif : Permettre la lecture et la mise à jour centralisée du drapeau
        /// courant. Toute modification d'une valeur distincte déclenche une notification
        /// <see cref="PropertyChanged"/>.</para>
        /// </remarks>
        public Uri AppFlagUri
        {
            get => _appFlagUri;
            set => SetField(ref _appFlagUri, value);
        }

        /// <summary>
        /// Obtient le référentiel des drapeaux disponibles, indexé par code pays ISO 3166-1 alpha-2.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisé par <see cref="GetFlagUriOrDefault"/> et exposé aux
        /// consommateurs nécessitant l'accès au référentiel complet.</para>
        /// <para>Objectif : Fournir un accès centralisé en lecture seule au dictionnaire
        /// de drapeaux sans exposer l'implémentation interne.</para>
        /// </remarks>
        public IReadOnlyDictionary<string, Uri> ReferenceFlags => RS_Flags.ReferenceFlag;

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        /// <summary>
        /// Déclenché lorsqu'une propriété observable du setting est modifiée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Émis par le helper <see cref="SetField{T}"/> à chaque
        /// modification effective d'une propriété mutable, conformément au pattern
        /// INotifyPropertyChanged.</para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SE_Flag"/> avec les valeurs par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instanciée exclusivement par le conteneur DI au démarrage de l'application.</para>
        /// <para>Objectif : Garantir un état initial cohérent — drapeau courant initialisé
        /// au drapeau par défaut.</para>
        /// </remarks>
        public SE_Flag()
        {
            _appFlagUri = RS_Flags.DefaultFlag_Source;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne l'URI du drapeau correspondant au code pays donné, ou le drapeau par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par les services de langue lors du changement de langue
        /// ou de l'initialisation de l'interface.</para>
        /// <para>Objectif : Fournir une résolution centralisée avec repli explicite sur
        /// <see cref="DefaultFlagUri"/> si le code n'est pas reconnu.</para>
        /// </remarks>
        /// <param name="countryCode">Code pays ISO 3166-1 alpha-2 à résoudre (ex : "FR", "DE").
        /// Une valeur vide, blanche ou non trouvée entraîne le retour du drapeau par défaut.</param>
        /// <returns>URI du drapeau correspondant, ou <see cref="DefaultFlagUri"/> si non trouvé.</returns>
        public Uri GetFlagUriOrDefault(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return DefaultFlagUri;

            return ReferenceFlags.TryGetValue(countryCode, out Uri? uri)
                ? uri
                : DefaultFlagUri;
        }

        /// <summary>
        /// Réinitialise le drapeau courant à la valeur par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée à l'initialisation de l'application ou en cas de repli
        /// après une résolution invalide.</para>
        /// <para>Objectif : Restaurer un état d'affichage stable et cohérent.</para>
        /// </remarks>
        public void ResetToDefault()
        {
            AppFlagUri = DefaultFlagUri;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Met à jour un champ de stockage et déclenche la notification <see cref="PropertyChanged"/>
        /// si la valeur a effectivement changé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisée par tous les setters publics et opérations atomiques
        /// modifiant des propriétés mutables de la classe.</para>
        /// <para>Objectif : Centraliser la logique INPC, supprimer les notifications redondantes
        /// et exposer à l'appelant l'information du changement effectif.</para>
        /// </remarks>
        /// <typeparam name="T">Type de la valeur stockée.</typeparam>
        /// <param name="field">Référence au champ de stockage privé.</param>
        /// <param name="value">Nouvelle valeur à appliquer.</param>
        /// <param name="propertyName">Nom de la propriété appelante, résolu automatiquement
        /// par <see cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>
        /// <see langword="true"/> si la valeur a effectivement changé et la notification a été émise ;
        /// <see langword="false"/> si la valeur est inchangée et aucune notification n'a été émise.
        /// </returns>
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        #endregion
    }
}