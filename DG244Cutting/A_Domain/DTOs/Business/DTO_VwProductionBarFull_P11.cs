namespace DG244Cutting.A_Domain.DTOs.Business
{
    /// <summary>
    /// Objet de transport de la composition en barres optimisées d'une série de
    /// production, à raison d'une instance par barre, affichable dans le quatrième
    /// onglet de la Page11.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter la composition en barres optimisées d'une série de
    /// production, à raison d'une instance par barre, depuis la projection SQL portée
    /// par le couple <c>IR_VwProductionBarFull</c> / <c>CR_VwProductionBarFull</c> sur
    /// la vue <c>vw_ProductionBar_Full</c>, vers le quatrième onglet de la Page11.
    /// Dix-huit champs projetés sur les quatre-vingt-deux colonnes de la vue : seize
    /// champs d'affichage dans l'ordre des colonnes du tableau, deux champs de service
    /// non affichés dédiés à l'identification, à la vérification de cohérence du lot et
    /// à l'ordonnancement. Le suffixe <c>_P11</c> marque la destination, la même vue
    /// devant alimenter d'autres écrans avec d'autres sélections de colonnes.</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucune
    /// référence à EF Core.</para>
    /// <para>Invariant : les types transportés et leur caractère nullable recopient
    /// fidèlement ceux de la vue source, sans rien y ajouter ni en retrancher ; les
    /// préfixes de nom de propriété (AR, AI, PB, PS) conservent la trace de la table
    /// d'origine de chaque colonne au sein de la jointure.</para>
    /// </remarks>
    public class DTO_VwProductionBarFull_P11
    {
        /// <summary>Code alphanumérique unique de la référence article - colonne P11_25 « Référence ».</summary>
        public string ARReference { get; set; } = null!;

        /// <summary>Désignation de la référence article - colonne P11_11 « Désignation ».</summary>
        public string ARDesignation { get; set; } = null!;

        /// <summary>Identifiant de couleur, teinte RAL et finition de l'article interne - colonne P11_21 « Couleur ».</summary>
        public string? AIIdColorRalFinish { get; set; }

        /// <summary>Catégorie métier principale de la référence article - colonne P11_26 « Catégorie ».</summary>
        public string? ARFamilyCategoryPrincipal { get; set; }

        /// <summary>Longueur totale de la barre en millimètres - colonne P11_27 « Longueur ».</summary>
        public int PBBarLength { get; set; }

        /// <summary>Hauteur de profilé de la barre en millimètres - colonne P11_19 « Hauteur ».</summary>
        public decimal? ARBarHeightMm { get; set; }

        /// <summary>Largeur de profilé de la barre en millimètres - colonne P11_20 « Largeur ».</summary>
        public decimal? ARBarWidthMm { get; set; }

        /// <summary>Ordre de tri d'affichage du profil - colonne P11_28 « Ordre tri ». Également premier critère d'ordonnancement du tableau.</summary>
        public short ARSortOrder { get; set; }

        /// <summary>Nombre de découpes affectées à la barre - colonne P11_29 « NB découpes ».</summary>
        public int PBCutPieceCount { get; set; }

        /// <summary>Longueur de reste calculée en millimètres, préliminaire avant validation - colonne P11_30 « Longueur reste ».</summary>
        public int? PBResidueLength { get; set; }

        /// <summary>Barre neuve (vrai) ou chute réutilisée (faux) - colonne P11_31 « Barre neuve ». Également deuxième critère d'ordonnancement du tableau.</summary>
        public bool PBIsNewBar { get; set; }

        /// <summary>Barre acceptée physiquement par l'opérateur - colonne P11_32 « Barre validée ».</summary>
        public bool PBIsValidated { get; set; }

        /// <summary>Barre effectivement utilisée pour des découpes - colonne P11_33 « Barre utilisée ».</summary>
        public bool PBIsUsed { get; set; }

        /// <summary>Barre en rupture de stock - colonne P11_34 « Barre en rupture ».</summary>
        public bool PBIsOutOfStock { get; set; }

        /// <summary>Barre refusée : le refus marque l'enregistrement comme logiquement supprimé - colonne P11_35 « Barre refusée ».</summary>
        public bool PBIsDeleted { get; set; }

        /// <summary>Motif du refus de la barre - colonne P11_36 « Motif refus ».</summary>
        public string? PBRejectionReason { get; set; }

        /// <summary>Clé technique de la série de production. Champ de service non affiché : support de la vérification de cohérence du lot reçu, dont toutes les lignes portent la même série.</summary>
        public int PSId { get; set; }

        /// <summary>Clé technique de la barre de production. Champ de service non affiché : identification de chaque ligne du tableau et troisième critère d'ordonnancement.</summary>
        public int PBId { get; set; }
    }
}