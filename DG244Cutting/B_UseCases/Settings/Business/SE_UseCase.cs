using System.ComponentModel;
using System.Runtime.CompilerServices;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Interfaces.Settings.Business;

namespace DG244Cutting.B_UseCases.Settings.Business
{
    /// <summary>
    /// Représente l'état des sélections métier courantes partagé par l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant Singleton transversal injectable via <see cref="ISE_UseCase"/>,
    /// enregistré dans <c>E_Miscellaneous/CompositionRoot/SR_ConteneurDI.cs</c>.</para>
    /// <para>Objectif : Centraliser un état de sélection métier partagé, observable et cohérent,
    /// sans porter de logique métier ni de logique technique d'acquisition. Cet état matérialise le
    /// troisième niveau du relais <see cref="INotifyPropertyChanged"/> à trois niveaux
    /// (Settings → Services → ViewModels). Les identifiants dont l'évolution doit rester cohérente au
    /// sein de la cascade sont exposés en lecture seule et leur écriture est canalisée par les opérations
    /// atomiques dédiées, qui garantissent l'invalidation mécanique des niveaux aval.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Stocker l'état complet de la série sélectionnée sous forme de DTO, exposé en lecture seule.</description></item>
    /// <item><description>Stocker l'identifiant de la série sélectionnée.</description></item>
    /// <item><description>Stocker l'identifiant de la barre sélectionnée au sein de la série courante.</description></item>
    /// <item><description>Stocker l'identifiant de la découpe sélectionnée au sein de la barre courante.</description></item>
    /// <item><description>Garantir l'invalidation en cascade des niveaux aval lors des mutations amont.</description></item>
    /// <item><description>Notifier les observateurs lors des changements d'état via <see cref="INotifyPropertyChanged"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne pas charger de données depuis une source externe.</description></item>
    /// <item><description>Ne pas porter de logique métier ni de validation de positivité des identifiants.</description></item>
    /// <item><description>Ne pas garantir la cohérence amont de la cascade au-delà de l'invalidation mécanique des niveaux aval.</description></item>
    /// </list>
    /// </remarks>
    public class SE_UseCase : ISE_UseCase
    {
        #region === Propriétés privées ===

        // --- Cascade de sélection métier ---
        private DTO_ProductionSeriesItem? _selectedSeries;
        private int _idSeriesSelected;
        private int _idBarSelected;
        private int _idCuttingSelected;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        // --- Cascade de sélection métier (lecture seule, écriture via opérations atomiques) ---

        /// <summary>
        /// Obtient l'état complet de la série sélectionnée, ou <see langword="null"/> lorsqu'aucune série n'est sélectionnée.
        /// </summary>
        /// <remarks>
        /// La valeur <see langword="null"/> est la sentinelle « aucune série sélectionnée », cohérente avec la
        /// sentinelle <c>0</c> des identifiants. Écriture exclusive via <see cref="SelectSeries"/> (positionnement)
        /// et <see cref="Reset"/> (remise à <see langword="null"/>).
        /// </remarks>
        public DTO_ProductionSeriesItem? SelectedSeries => _selectedSeries;

        /// <summary>
        /// Obtient l'identifiant de la série sélectionnée.
        /// </summary>
        /// <remarks>
        /// La valeur <c>0</c> est la sentinelle conventionnelle « aucune sélection ». Cet identifiant est dérivé de
        /// <see cref="SelectedSeries"/> et lui reste cohérent. Écriture exclusive via <see cref="SelectSeries"/>.
        /// </remarks>
        public int IdSeriesSelected => _idSeriesSelected;

        /// <summary>
        /// Obtient l'identifiant de la barre sélectionnée au sein de la série courante.
        /// </summary>
        /// <remarks>
        /// La valeur <c>0</c> est la sentinelle conventionnelle « aucune sélection ».
        /// Écriture exclusive via <see cref="SelectSeries"/> (réinitialisation) et <see cref="SelectBar"/> (positionnement).
        /// </remarks>
        public int IdBarSelected => _idBarSelected;

        /// <summary>
        /// Obtient ou définit l'identifiant de la découpe sélectionnée au sein de la barre courante.
        /// </summary>
        /// <remarks>
        /// La valeur <c>0</c> est la sentinelle conventionnelle « aucune sélection ». En bout de cascade,
        /// cette propriété n'invalide aucun état aval : elle dispose d'un accès en écriture direct et est
        /// en outre réinitialisée à <c>0</c> par <see cref="SelectSeries"/> et <see cref="SelectBar"/>.
        /// </remarks>
        public int IdCuttingSelected
        {
            get => _idCuttingSelected;
            set => SetField(ref _idCuttingSelected, value);
        }

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        /// <summary>
        /// Émis lors d'une modification effective d'une propriété observable.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Déclenché par le helper <see cref="SetField{T}"/> pour le setter de
        /// <see cref="IdCuttingSelected"/> et pour les opérations atomiques multi-propriétés
        /// (<see cref="SelectSeries"/>, <see cref="SelectBar"/>, <see cref="Reset"/>).</para>
        /// <para>Objectif : Notifier les observateurs d'un changement d'état observable conformément au
        /// pattern <see cref="INotifyPropertyChanged"/>, en support du relais vers les ViewModels aval.</para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SE_UseCase"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelé par le conteneur d'injection lors de la création du Singleton.</para>
        /// <para>Objectif : Garantir un état initial cohérent (cascade réinitialisée à la sentinelle <c>0</c>),
        /// sans acquérir de données techniques ni métier.</para>
        /// </remarks>
        public SE_UseCase()
        {
            Reset();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Positionne la série sélectionnée en tête de cascade et invalide les deux niveaux aval.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de la sélection d'une série par un consommateur amont, qui dispose du
        /// DTO complet de la série au moment du clic.</para>
        /// <para>Objectif : Positionner atomiquement <see cref="SelectedSeries"/> et <see cref="IdSeriesSelected"/>
        /// (dérivé de <c>series.Id</c>), puis invalider mécaniquement la barre et la découpe héritées de la série
        /// précédente, en invoquant directement <see cref="SetField{T}"/> sur chaque champ support, ce qui filtre
        /// les valeurs identiques et notifie uniquement les propriétés réellement modifiées. La cohérence entre
        /// <see cref="SelectedSeries"/> et <see cref="IdSeriesSelected"/> est garantie structurellement par cette
        /// écriture canalisée. Le paramètre est non nullable ; la désélection passe exclusivement par
        /// <see cref="Reset"/>.</para>
        /// </remarks>
        /// <param name="series">DTO de la série à sélectionner ; son identifiant <c>Id</c> alimente <see cref="IdSeriesSelected"/>.</param>
        public void SelectSeries(DTO_ProductionSeriesItem series)
        {
            SetField(ref _selectedSeries, series, nameof(SelectedSeries));
            SetField(ref _idSeriesSelected, series.Id, nameof(IdSeriesSelected));
            SetField(ref _idBarSelected, 0, nameof(IdBarSelected));
            SetField(ref _idCuttingSelected, 0, nameof(IdCuttingSelected));
        }

        /// <summary>
        /// Positionne la barre sélectionnée au sein de la série courante et invalide la découpe héritée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de la sélection d'une barre par un consommateur amont.</para>
        /// <para>Objectif : Positionner la barre sélectionnée et invalider mécaniquement la découpe héritée
        /// de la barre précédente, sans toucher à la série courante, en invoquant directement
        /// <see cref="SetField{T}"/> sur chaque champ support concerné. La sentinelle <c>0</c> est une entrée
        /// légitime ; aucune validation de positivité ni garde de cohérence amont n'est réalisée, la cohérence
        /// de la cascade reposant sur l'ordre d'appel des consommateurs.</para>
        /// </remarks>
        /// <param name="idBar">Identifiant de la barre à sélectionner (<c>0</c> pour désélectionner).</param>
        public void SelectBar(int idBar)
        {
            SetField(ref _idBarSelected, idBar, nameof(IdBarSelected));
            SetField(ref _idCuttingSelected, 0, nameof(IdCuttingSelected));
        }

        /// <summary>
        /// Réinitialise globalement la cascade de sélection.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée à l'initialisation depuis le constructeur, ou lors d'un changement de
        /// contexte (retour sur la page racine, déconnexion, redémarrage logique).</para>
        /// <para>Objectif : Ramener <see cref="SelectedSeries"/> à <see langword="null"/> et les trois identifiants
        /// à la sentinelle <c>0</c> par invocation directe de <see cref="SetField{T}"/> avec <c>nameof</c>. Seuls les
        /// champs dont la valeur n'est pas déjà nulle émettent une notification.</para>
        /// </remarks>
        public void Reset()
        {
            SetField(ref _selectedSeries, null, nameof(SelectedSeries));
            SetField(ref _idSeriesSelected, 0, nameof(IdSeriesSelected));
            SetField(ref _idBarSelected, 0, nameof(IdBarSelected));
            SetField(ref _idCuttingSelected, 0, nameof(IdCuttingSelected));
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Met à jour un champ de stockage et déclenche la notification <see cref="PropertyChanged"/>
        /// si la valeur a changé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisée par le setter public de <see cref="IdCuttingSelected"/> (résolution
        /// automatique du nom de propriété via <see cref="CallerMemberNameAttribute"/>) et par les opérations
        /// atomiques multi-propriétés <see cref="SelectSeries"/>, <see cref="SelectBar"/> et <see cref="Reset"/>
        /// (passage explicite du nom de propriété via <c>nameof</c>).</para>
        /// <para>Objectif : Centraliser le triptyque canonique « comparaison de valeur, écriture du champ
        /// support, émission de la notification <see cref="PropertyChanged"/> » conformément à la signature
        /// canonique invariante définie en §4.14.7 (clause « Signature canonique du helper SetField »).</para>
        /// </remarks>
        /// <typeparam name="T">Type de la valeur stockée.</typeparam>
        /// <param name="field">Référence au champ de stockage privé.</param>
        /// <param name="value">Nouvelle valeur à appliquer.</param>
        /// <param name="propertyName">Nom de la propriété appelante, résolu automatiquement par
        /// <see cref="CallerMemberNameAttribute"/> pour le setter, ou passé explicitement via <c>nameof</c>
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

        #endregion
    }
}