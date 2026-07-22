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
    /// pour le tableau de bord Page10, sous quatre natures de matière : des données
    /// descriptives recopiées de l'entité, des champs dérivés calculés en amont par
    /// la brique de lecture, des jalons d'avancement du flux de traitement, et un
    /// indicateur d'alerte de disponibilité matière.</para>
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
        /// <summary>Clé technique de la série.</summary>
        public int Id { get; set; }

        /// <summary>Numéro de série AX présenté à l'opérateur.</summary>
        public int IdSerialNumber { get; set; }

        /// <summary>Description de la série.</summary>
        public string? Description { get; set; }

        /// <summary>Date de début de production de la série.</summary>
        public DateTime? ProductionStartDate { get; set; }

        /// <summary>Date de fin de production de la série.</summary>
        public DateTime? ProductionEndDate { get; set; }

        /// <summary>Code couleur du jour de fin de production, recopié tel quel de l'entité.</summary>
        public short ProductionEndDay { get; set; }

        /// <summary>Clé semaine-jour au format "NN-n" (semaine ISO sur deux chiffres zéro-paddés, jour de semaine lundi=1 sur un chiffre), recalculée en amont depuis <c>ProductionStartDate</c>.</summary>
        public string? WeekDayKey { get; set; }

        /// <summary>Statut de classement de la série (l'une des cinq valeurs réelles ; la sentinelle <c>NotValidated</c> n'est jamais portée par un DTO affiché).</summary>
        public En_ProductionSeriesStatus Status { get; set; }

        /// <summary>Indicateur de retard : série commencée ayant dépassé son échéance de fin (commencé AND <c>AppDate &gt;= ProductionEndDate</c>).</summary>
        public bool IsLate { get; set; }

        /// <summary>Série optimisée pour la découpe sur barres de chutes.</summary>
        public bool IsDropBarOptimized { get; set; }

        /// <summary>Barres de chutes approvisionnées.</summary>
        public bool IsDropBarSupplied { get; set; }

        /// <summary>Série optimisée pour la découpe sur barres neuves.</summary>
        public bool IsNewBarOptimized { get; set; }

        /// <summary>Barres neuves approvisionnées.</summary>
        public bool IsNewBarSupplied { get; set; }

        /// <summary>Indicateur d'alerte de disponibilité matière en barres neuves, agrégé au niveau série : false = stock disponible, true = rupture signalée sur au moins une référence d'article de la série, sans information sur la ou les références concernées.</summary>
        public bool IsBarOutOfStock { get; set; }
    }
}