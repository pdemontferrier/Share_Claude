using DG244Cutting.A_Domain.Common.Enums;

namespace DG244Cutting.A_Domain.DTOs.Business
{
    /// <summary>
    /// Objet de transport d'un élément de série de production affichable dans le
    /// tableau de bord Page10.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter un élément de série de production affichable
    /// pour le tableau de bord Page10, son statut de classement et son indicateur
    /// de retard étant déjà calculés en amont par la brique de lecture de la Page10
    /// (Query Handler de projection de l'entité <c>ProductionSeries</c>).</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucun
    /// calcul de statut ni d'indicateur de retard, aucun recalcul de clé de tri,
    /// aucune référence à EF Core.</para>
    /// <para>Invariant : une instance ne représente qu'une série ayant franchi le
    /// socle d'admission (<c>IsImported AND IsProductionValidated</c>) ; la
    /// sentinelle <c>NotValidated</c> de <c>En_ProductionSeriesStatus</c> n'est
    /// jamais portée par un DTO affiché.</para>
    /// </remarks>
    public class DTO_ProductionSeriesItem
    {
        /// <summary>Clé technique de la série ; alimente le routage au clic (<c>SE_UseCase.SelectSeries(Id)</c>).</summary>
        public int Id { get; set; }

        /// <summary>Numéro de série AX présenté à l'opérateur.</summary>
        public int IdSerialNumber { get; set; }

        /// <summary>Description de la série.</summary>
        public string? Description { get; set; }

        /// <summary>Date de début de production ; frontière ToDo/Upcoming.</summary>
        public DateTime? ProductionStartDate { get; set; }

        /// <summary>Date de fin de production ; frontière ToDo/Overdue, base de l'indicateur de retard, clé de tri primaire des cinq listes.</summary>
        public DateTime? ProductionEndDate { get; set; }

        /// <summary>Code couleur du jour de fin de production, pilotant la couleur de fond de la ligne (recopié tel quel de l'entité).</summary>
        public short ProductionEndDay { get; set; }

        /// <summary>Clé semaine-jour au format "NN-n" (semaine ISO sur deux chiffres zéro-paddés, jour de semaine lundi=1 sur un chiffre), recalculée en amont depuis <c>ProductionStartDate</c> ; critère de tri secondaire.</summary>
        public string? WeekDayKey { get; set; }

        /// <summary>Statut de classement de la série (l'une des cinq valeurs réelles ; la sentinelle <c>NotValidated</c> n'est jamais portée par un DTO affiché).</summary>
        public En_ProductionSeriesStatus Status { get; set; }

        /// <summary>Indicateur de retard : série commencée ayant dépassé son échéance de fin (commencé AND <c>AppDate &gt;= ProductionEndDate</c>) ; pilote la mise en gras de la ligne, sans définir de statut.</summary>
        public bool IsLate { get; set; }

        /// <summary>Série optimisée pour la découpe sur barres de chutes ; consommé par Page11 à son ouverture.</summary>
        public bool IsDropBarOptimized { get; set; }

        /// <summary>Barres de chutes approvisionnées ; état d'approvisionnement, composante de « commencé ».</summary>
        public bool IsDropBarSupplied { get; set; }

        /// <summary>Série optimisée pour la découpe sur barres neuves ; consommé par Page12 à son ouverture.</summary>
        public bool IsNewBarOptimized { get; set; }

        /// <summary>Barres neuves approvisionnées ; état d'approvisionnement, discriminant du routage Page12/Page20.</summary>
        public bool IsNewBarSupplied { get; set; }
    }
}