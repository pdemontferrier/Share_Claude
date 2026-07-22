using System.ComponentModel;
using DG244Cutting.A_Domain.DTOs.Business;

namespace DG244Cutting.A_Domain.Interfaces.Settings.Business
{
    /// <summary>
    /// Définit le contrat de l'état des sélections métier courantes partagé par l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Contrat Singleton défini dans <c>A_Domain</c>, consommé par
    /// injection de dépendances dans les composants autorisés (services de relais, ViewModels).</para>
    /// <para>Objectif : Exposer l'état des sélections métier courantes via une abstraction découplée
    /// de l'implémentation <see cref="DG244Cutting.B_UseCases.Settings.Business.SE_UseCase"/>. Cet état
    /// matérialise le troisième niveau du relais <see cref="INotifyPropertyChanged"/> à trois niveaux
    /// (Settings → Services → ViewModels) : les propriétés observables qu'il expose sont destinées à
    /// être relayées vers les ViewModels des pages aval. Les identifiants dont l'évolution doit rester
    /// cohérente au sein de la cascade sont exposés en lecture seule par le contrat ; leur écriture est
    /// canalisée par les opérations atomiques dédiées.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Exposer l'état complet de la série sélectionnée sous forme de DTO en lecture seule.</description></item>
    /// <item><description>Exposer l'identifiant de la série sélectionnée.</description></item>
    /// <item><description>Exposer l'identifiant de la barre sélectionnée au sein de la série courante.</description></item>
    /// <item><description>Exposer l'identifiant de la découpe sélectionnée au sein de la barre courante.</description></item>
    /// <item><description>Exposer les opérations atomiques d'écriture qui garantissent l'invalidation en cascade des niveaux aval.</description></item>
    /// <item><description>Permettre la notification de changements d'état via <see cref="INotifyPropertyChanged"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne pas charger de données depuis une base ou un service externe.</description></item>
    /// <item><description>Ne pas contenir de logique métier complexe ni de validation de positivité des identifiants.</description></item>
    /// <item><description>Ne pas garantir la cohérence amont de la cascade au-delà de l'invalidation mécanique des niveaux aval.</description></item>
    /// </list>
    /// </remarks>
    public interface ISE_UseCase : INotifyPropertyChanged
    {
        // --- Groupe 1 : Cascade de sélection métier (lecture seule via le contrat) ---

        /// <summary>
        /// Obtient l'état complet de la série sélectionnée, ou <see langword="null"/> lorsqu'aucune série n'est sélectionnée.
        /// </summary>
        /// <remarks>
        /// La valeur <see langword="null"/> est la sentinelle « aucune série sélectionnée », cohérente avec la
        /// sentinelle <c>0</c> des identifiants. Cette propriété porte le DTO complet de la série afin que les pages
        /// aval le lisent directement depuis l'état partagé sans re-lecture en base. Écriture exclusive via
        /// <see cref="SelectSeries"/> (positionnement) et <see cref="Reset"/> (remise à <see langword="null"/>) ;
        /// lorsqu'elle n'est pas nulle, <see cref="IdSeriesSelected"/> en dérive et lui reste cohérent.
        /// </remarks>
        DTO_ProductionSeriesItem? SelectedSeries { get; }

        /// <summary>
        /// Obtient l'identifiant de la série sélectionnée.
        /// </summary>
        /// <remarks>
        /// La valeur <c>0</c> est la sentinelle conventionnelle « aucune sélection ». Cet identifiant est dérivé de
        /// <see cref="SelectedSeries"/> et lui reste cohérent (<c>SelectedSeries.Id</c> lorsque la série est sélectionnée,
        /// <c>0</c> sinon). Écriture exclusive via <see cref="SelectSeries"/>.
        /// </remarks>
        int IdSeriesSelected { get; }

        /// <summary>
        /// Obtient l'identifiant de la barre sélectionnée au sein de la série courante.
        /// </summary>
        /// <remarks>
        /// La valeur <c>0</c> est la sentinelle conventionnelle « aucune sélection ».
        /// Écriture exclusive via <see cref="SelectSeries"/> (réinitialisation) et <see cref="SelectBar"/> (positionnement).
        /// </remarks>
        int IdBarSelected { get; }

        /// <summary>
        /// Obtient ou définit l'identifiant de la découpe sélectionnée au sein de la barre courante.
        /// </summary>
        /// <remarks>
        /// La valeur <c>0</c> est la sentinelle conventionnelle « aucune sélection ». En bout de cascade,
        /// cette propriété n'invalide aucun état aval : elle dispose d'un accès en écriture direct et est
        /// en outre réinitialisée à <c>0</c> par <see cref="SelectSeries"/> et <see cref="SelectBar"/>.
        /// </remarks>
        int IdCuttingSelected { get; set; }

        // --- Groupe 2 : Opérations atomiques de cascade ---

        /// <summary>
        /// Positionne la série sélectionnée en tête de cascade et invalide les deux niveaux aval.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de la sélection d'une série par un consommateur amont, qui dispose du
        /// DTO complet de la série au moment du clic.</para>
        /// <para>Objectif : Positionner atomiquement <see cref="SelectedSeries"/> et <see cref="IdSeriesSelected"/>
        /// (dérivé de <c>series.Id</c>), puis invalider mécaniquement la barre et la découpe héritées de la série
        /// précédente (ramenées à <c>0</c>). Les champs sont écrits en filtrant les valeurs identiques : seules les
        /// propriétés effectivement modifiées émettent une notification.</para>
        /// <para>Le paramètre est non nullable : la désélection n'est pas réalisée par cette méthode mais
        /// exclusivement par <see cref="Reset"/>.</para>
        /// </remarks>
        /// <param name="series">DTO de la série à sélectionner ; son identifiant <c>Id</c> alimente <see cref="IdSeriesSelected"/>.</param>
        void SelectSeries(DTO_ProductionSeriesItem series);

        /// <summary>
        /// Positionne la barre sélectionnée au sein de la série courante et invalide la découpe héritée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de la sélection d'une barre par un consommateur amont.</para>
        /// <para>Objectif : Positionner la barre sélectionnée et invalider mécaniquement la découpe héritée
        /// de la barre précédente, sans toucher à la série courante. Les deux champs concernés sont écrits
        /// atomiquement en filtrant les valeurs identiques. La valeur <c>0</c> est admise en entrée ; aucune
        /// validation de positivité ni garde de cohérence amont n'est réalisée, la cohérence de la cascade
        /// reposant sur l'ordre d'appel des consommateurs.</para>
        /// </remarks>
        /// <param name="idBar">Identifiant de la barre à sélectionner (<c>0</c> pour désélectionner).</param>
        void SelectBar(int idBar);

        /// <summary>
        /// Réinitialise globalement la cascade de sélection.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le constructeur (état initial garanti) et lors des changements de
        /// contexte (retour sur la page racine, déconnexion, redémarrage logique).</para>
        /// <para>Objectif : Ramener <see cref="SelectedSeries"/> à <see langword="null"/> et les trois identifiants
        /// à la sentinelle <c>0</c>, garantissant une cascade trivialement cohérente. Seuls les champs dont la valeur
        /// n'est pas déjà nulle émettent une notification.</para>
        /// </remarks>
        void Reset();
    }
}