
namespace DG244Cutting.A_Domain.DTOs.Business
{
    /// <summary>
    /// Objet de transport du vivier des découpes restant à réaliser pour un couple
    /// série de production / article interne, à raison d'une instance par découpe,
    /// consommé par le moteur d'optimisation de la Page20.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter, à raison d'une instance par découpe, la projection
    /// SQL portée par le couple <c>IR_VwProductionCutPieceFull</c> /
    /// <c>CR_VwProductionCutPieceFull</c> sur la vue <c>vw_ProductionCutPiece_Full</c>,
    /// à destination du moteur d'optimisation de la Page20 et de l'affichage du profilé
    /// en cours de traitement. La matière transportée relève de trois natures : les
    /// données de découpe, matière à placer sur une barre ; les paramètres de coupe,
    /// qui gouvernent la consommation matière ; les caractéristiques d'article, qui
    /// déterminent le contenant de repli et la qualification du résidu. S'y ajoutent
    /// des champs d'identification, étrangers au calcul, qui ordonnent la recherche de
    /// la prochaine référence à traiter et présentent à l'opérateur le profilé en cours
    /// de traitement. Treize champs projetés sur les deux cent trente-trois colonnes de
    /// la vue. Le suffixe <c>_P20</c> marque la destination, la même vue alimentant
    /// d'autres écrans avec d'autres sélections de colonnes.</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucune
    /// référence à EF Core.</para>
    /// <para>Invariant : les types transportés et leur caractère nullable recopient
    /// fidèlement ceux de la vue source, sans rien y ajouter ni en retrancher.
    /// L'arrondi en millimètres entiers et le traitement des valeurs absentes relèvent
    /// du consommateur, non du transport. Les préfixes de nom de propriété (PCP, AR,
    /// AI) conservent la trace de la table d'origine de chaque colonne au sein de la
    /// jointure.</para>
    /// </remarks>
    public class DTO_VwProductionCutPieceFull_P20
    {
        // Données de découpe

        /// <summary>Clé technique de la pièce à découper. Identifie chaque ligne du vivier et fonde son ordonnancement déterministe.</summary>
        public int PCPId { get; set; }

        /// <summary>Longueur de la découpe, matière à placer sur une barre.</summary>
        public decimal? PCPCutDimension { get; set; }

        // Paramètres de coupe

        /// <summary>Épaisseur du trait de scie consommée entre deux découpes consécutives. Valeur portée par chaque ligne mais invariante pour un article interne donné.</summary>
        public int? PCPSawCutLength { get; set; }

        /// <summary>Longueur de la coupe de propreté en tête de barre. Valeur portée par chaque ligne mais invariante pour un article interne donné.</summary>
        public int? PCPFinishingCutLength { get; set; }

        // Caractéristiques d'article

        /// <summary>Identifiant de l'article interne de la découpe. Clé de compatibilité avec le stock de chutes.</summary>
        public int? PCPIdArticleInternal { get; set; }

        /// <summary>Longueur d'une barre neuve de l'article interne, en millimètres. Contenant de repli de l'optimisation.</summary>
        public double? AIStandardBarLengthMm { get; set; }

        /// <summary>Longueur minimale à partir de laquelle un résidu est qualifié de chute réutilisable.</summary>
        public decimal? ARMinScrapLength { get; set; }

        /// <summary>Indique si l'article interne est géré avec suivi des chutes. À défaut, tout résidu est un déchet.</summary>
        public bool? AIManageScraps { get; set; }

        /// <summary>Ordre d'affichage de la référence article.</summary>
        public short? ARSortOrder { get; set; }

        // Identification et affichage

        /// <summary>Couple référence et couleur, concaténé par la vue et séparé par une barre verticale. Ordonne la recherche de la prochaine référence à traiter.</summary>
        public string PCPReferenceColor { get; set; } = null!;

        /// <summary>Référence de la barre d'origine de la découpe, affichée en Page20.</summary>
        public string? PCPBarReference { get; set; }

        /// <summary>Code couleur intérieur et extérieur de la barre, affiché en Page20.</summary>
        public string? PCPBarColorCodeInOut { get; set; }

        /// <summary>Désignation du profilé dont la découpe est issue, affichée en Page20.</summary>
        public string? PCPProfileName { get; set; }
    }
}