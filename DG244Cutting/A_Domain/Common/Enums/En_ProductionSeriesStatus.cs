namespace DG244Cutting.A_Domain.Common.Enums
{
    /// <summary>
    /// Statut d’une série de production au regard de son avancement et de son
    /// échéance, en vue de sa répartition dans l’une des cinq listes du tableau
    /// de bord Page10.
    /// </summary>
    /// <remarks>
    /// <see cref="NotValidated"/> est le défaut conservateur du type
    /// (<c>default(En_ProductionSeriesStatus) == NotValidated</c>) et une garde
    /// défensive ; il n’est jamais porté par un DTO affiché, les séries hors
    /// socle d’admission (<c>IsImported AND IsProductionValidated</c>) étant
    /// exclues en amont par le filtre de la requête de lecture. La valeur
    /// entière 1..5 encode l’ordre d’affichage des cinq listes à l’écran (ToDo,
    /// Overdue, InProgress, Completed, Upcoming) et NON l’ordre de priorité de
    /// classement, ce dernier relevant des prédicats mutuellement exclusifs de
    /// la logique de répartition (hors périmètre de ce fil).
    /// </remarks>
    public enum En_ProductionSeriesStatus
    {
        /// <summary>
        /// Sentinelle — série ne franchissant pas le socle d’admission ; défaut
        /// du type et garde, jamais affichée.
        /// </summary>
        NotValidated = 0,

        /// <summary>
        /// Série admise, non terminée, non commencée, échéance = jour courant
        /// (ou fenêtre imminente) ; rang d’affichage 1.
        /// </summary>
        ToDo = 1,

        /// <summary>
        /// Série admise, non terminée, non commencée, échéance dépassée ; rang
        /// d’affichage 2.
        /// </summary>
        Overdue = 2,

        /// <summary>
        /// Série admise, avancement engagé (approvisionnement chutes,
        /// approvisionnement barres neuves, ou découpe démarrée), découpe non
        /// terminée ; rang d’affichage 3.
        /// </summary>
        InProgress = 3,

        /// <summary>
        /// Série admise, découpe terminée ; rang d’affichage 4.
        /// </summary>
        Completed = 4,

        /// <summary>
        /// Série admise, non terminée, non commencée, échéance à venir ; rang
        /// d’affichage 5.
        /// </summary>
        Upcoming = 5
    }
}