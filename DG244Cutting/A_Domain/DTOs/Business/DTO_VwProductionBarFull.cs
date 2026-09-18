namespace DG244Cutting.A_Domain.DTOs.Business
{
    /// <summary>
    /// Objet de transport de la composition en barres optimisées d'une série de
    /// production, à raison d'une instance par barre, partagé par les écrans de
    /// consultation et de validation des barres.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter la composition en barres optimisées d'une série de
    /// production, à raison d'une instance par barre, depuis la projection SQL portée
    /// par le couple <c>IR_VwProductionBarFull</c> / <c>CR_VwProductionBarFull</c> sur
    /// la vue <c>vw_ProductionBar_Full</c>, vers trois destinations : le quatrième
    /// onglet de la Page11, qui liste en consultation les barres d'une série achevée ;
    /// le premier onglet de la Page20, qui présente à l'opérateur la barre que
    /// l'application vient de retenir et l'emplacement où en prendre la matière ; le
    /// quatrième onglet de la Page20, qui liste les barres de la série en rupture de
    /// stock. Vingt et un champs projetés sur les quatre-vingt-deux colonnes de la vue :
    /// dix-neuf champs d'affichage - les seize colonnes du tableau de la Page11, dont
    /// l'ordre relatif est conservé, et trois champs servant le premier onglet de la
    /// Page20 -, et deux champs de service non affichés dédiés à l'identification, à la
    /// vérification de cohérence du lot et à l'ordonnancement. L'absence de suffixe de
    /// destination marque la projection de référence de cette vue.</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucune
    /// référence à EF Core.</para>
    /// <para>Invariant : les types transportés et leur caractère nullable recopient
    /// fidèlement ceux de la vue source, sans rien y ajouter ni en retrancher ; les
    /// préfixes de nom de propriété (AR, AI, PB, PS, CSL) conservent la trace de la table
    /// d'origine de chaque colonne au sein de la jointure.</para>
    /// </remarks>
    public class DTO_VwProductionBarFull
    {
        /// <summary>Code alphanumérique unique de la référence article.</summary>
        public string ARReference { get; set; } = null!;

        /// <summary>Désignation de la référence article.</summary>
        public string ARDesignation { get; set; } = null!;

        /// <summary>Identifiant de couleur, teinte RAL et finition de l'article interne.</summary>
        public string? AIIdColorRalFinish { get; set; }

        /// <summary>Catégorie métier principale de la référence article.</summary>
        public string? ARFamilyCategoryPrincipal { get; set; }

        /// <summary>Longueur totale de la barre en millimètres.</summary>
        public int PBBarLength { get; set; }

        /// <summary>Hauteur de profilé de la barre en millimètres.</summary>
        public decimal? ARBarHeightMm { get; set; }

        /// <summary>Largeur de profilé de la barre en millimètres.</summary>
        public decimal? ARBarWidthMm { get; set; }

        /// <summary>Ordre de tri d'affichage du profil. Également premier critère d'ordonnancement du tableau.</summary>
        public short ARSortOrder { get; set; }

        /// <summary>Nombre de découpes affectées à la barre.</summary>
        public int PBCutPieceCount { get; set; }

        /// <summary>Longueur de reste calculée en millimètres, préliminaire avant validation.</summary>
        public int? PBResidueLength { get; set; }

        /// <summary>Barre neuve (vrai) ou chute réutilisée (faux). Également deuxième critère d'ordonnancement du tableau.</summary>
        public bool PBIsNewBar { get; set; }

        /// <summary>Emplacement réel où la chute d'origine de la barre a été rangée, tel qu'enregistré lors de sa qualification en Page22 ou de sa saisie manuelle en Page30. Valeur absente sur une barre neuve, la vue ne joignant alors aucune chute. À distinguer des emplacements candidats du futur résidu portés par la vue (<c>CSLScrapLocationVertical</c>, <c>CSLScrapLocationHorizontal</c>), qui désignent où ranger le résidu et non où prendre la barre.</summary>
        public string? CSLScrapLocationSource { get; set; }

        /// <summary>Barre acceptée physiquement par l'opérateur.</summary>
        public bool PBIsValidated { get; set; }

        /// <summary>Barre effectivement utilisée pour des découpes.</summary>
        public bool PBIsUsed { get; set; }

        /// <summary>Barre en rupture de stock.</summary>
        public bool PBIsOutOfStock { get; set; }

        /// <summary>Barre refusée : le refus marque l'enregistrement comme logiquement supprimé.</summary>
        public bool PBIsDeleted { get; set; }

        /// <summary>Motif du refus de la barre.</summary>
        public string? PBRejectionReason { get; set; }

        /// <summary>Clé technique de la série de production. Champ de service non affiché : support de la vérification de cohérence du lot reçu, dont toutes les lignes portent la même série. À distinguer de <c>PSIdSerialNumber</c>, numéro métier de la série.</summary>
        public int PSId { get; set; }

        /// <summary>Numéro métier de la série de production, sous lequel l'atelier la désigne sur les documents comme dans les échanges entre postes. À distinguer de <c>PSId</c>, qui en est la clé technique.</summary>
        public int PSIdSerialNumber { get; set; }

        /// <summary>Désignation de la série de production : libellé du chantier ou de la commande, que le numéro de série seul ne porte pas.</summary>
        public string PSDescription { get; set; } = null!;

        /// <summary>Clé technique de la barre de production. Champ de service non affiché : identification de chaque ligne du tableau et troisième critère d'ordonnancement.</summary>
        public int PBId { get; set; }
    }
}