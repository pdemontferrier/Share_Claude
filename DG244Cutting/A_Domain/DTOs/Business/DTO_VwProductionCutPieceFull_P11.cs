namespace DG244Cutting.A_Domain.DTOs.Business
{
    /// <summary>
    /// Objet de transport du détail des découpes d'une série de production, à raison
    /// d'une instance par découpe, affichable dans le cinquième onglet de la Page11.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter le détail des découpes d'une série de production,
    /// à raison d'une instance par découpe, depuis la projection SQL portée par le
    /// couple <c>IR_VwProductionCutPieceFull</c> / <c>CR_VwProductionCutPieceFull</c>
    /// sur la vue <c>vw_ProductionCutPiece_Full</c>, vers le cinquième onglet de la
    /// Page11. Une découpe est la pièce de profilé élémentaire à couper : elle porte
    /// l'identification du profilé dont elle est issue, sa géométrie de coupe, ses
    /// dimensions, sa position sur la barre affectée et quatre indicateurs d'état
    /// jalonnant son parcours d'atelier. Vingt champs projetés sur les deux cent
    /// trente-trois colonnes de la vue : seize champs d'affichage dans l'ordre des
    /// colonnes du tableau, quatre champs de service non affichés dédiés à
    /// l'identification, à la vérification de cohérence du lot et à l'ordonnancement.
    /// Le suffixe <c>_P11</c> marque la destination, la même vue devant alimenter
    /// d'autres écrans avec d'autres sélections de colonnes.</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucune
    /// référence à EF Core.</para>
    /// <para>Invariant : les types transportés et leur caractère nullable recopient
    /// fidèlement ceux de la vue source, sans rien y ajouter ni en retrancher ; les
    /// préfixes de nom de propriété (PCP, ACM, AR, PS) conservent la trace de la table
    /// d'origine de chaque colonne au sein de la jointure.</para>
    /// </remarks>
    public class DTO_VwProductionCutPieceFull_P11
    {
        /// <summary>Référence de la barre d'origine de la découpe - colonne P11_25 « Référence ».</summary>
        public string? PCPBarReference { get; set; }

        /// <summary>Nom du profil dont la découpe est issue - colonne P11_11 « Désignation ».</summary>
        public string? PCPProfileName { get; set; }

        /// <summary>Code couleur intérieur et extérieur de la barre - colonne P11_21 « Couleur ».</summary>
        public string? PCPBarColorCodeInOut { get; set; }

        /// <summary>Désignation métier de la catégorie de pièce - colonne P11_26 « Catégorie ». Également premier critère d'ordonnancement du tableau.</summary>
        public string? ACMDescription { get; set; }

        /// <summary>Inclinaison de coupe à gauche - colonne P11_37 « Incli. gauche ».</summary>
        public short? PCPCutInclinationLeft { get; set; }

        /// <summary>Pivot de coupe à gauche - colonne P11_38 « Pivot gauche ».</summary>
        public short? PCPCutPivotLeft { get; set; }

        /// <summary>Longueur de la découpe - colonne P11_39 « Longueur découpe ».</summary>
        public decimal? PCPCutDimension { get; set; }

        /// <summary>Pivot de coupe à droite - colonne P11_40 « Pivot droit ».</summary>
        public short? PCPCutPivotRight { get; set; }

        /// <summary>Inclinaison de coupe à droite - colonne P11_41 « Incli. droite ».</summary>
        public short? PCPCutInclinationRight { get; set; }

        /// <summary>Hauteur de profilé de la découpe - colonne P11_19 « Hauteur ».</summary>
        public decimal? PCPProfileHeight { get; set; }

        /// <summary>Largeur de profilé de la découpe - colonne P11_20 « Largeur ».</summary>
        public decimal? PCPProfileWidth { get; set; }

        /// <summary>Position de la découpe au sein de la barre, ordre de coupe - colonne P11_42 « Pos. découpe ». Également quatrième critère d'ordonnancement du tableau. Une découpe encore au vivier, non affectée à une barre, porte cette valeur à l'état absent : état nominal, non anomalie.</summary>
        public int? PCPCutPositionInBar { get; set; }

        /// <summary>Barre nécessaire à la découpe approvisionnée - colonne P11_42 « Barre appro. ».</summary>
        public bool PCPIsBarSupplied { get; set; }

        /// <summary>Barre nécessaire à la découpe en rupture de stock - colonne P11_34 « Barre en rupture ».</summary>
        public bool PCPIsBarOutOfStock { get; set; }

        /// <summary>Découpe réalisée - colonne P11_43 « Découpe faite ».</summary>
        public bool PCPIsCut { get; set; }

        /// <summary>Découpe refusée - colonne P11_45 « Découpe refusée ».</summary>
        public bool PCPIsCutRefused { get; set; }

        /// <summary>Clé technique de la série de production. Champ de service non affiché : support de la vérification de cohérence du lot reçu, dont toutes les lignes portent la même série.</summary>
        public int PSId { get; set; }

        /// <summary>Clé technique de la pièce à découper. Champ de service non affiché : identification de chaque ligne du tableau.</summary>
        public int PCPId { get; set; }

        /// <summary>Ordre d'affichage de la référence article. Champ de service non affiché : deuxième critère d'ordonnancement du tableau.</summary>
        public short? ARSortOrder { get; set; }

        /// <summary>Identifiant de la barre de production associée à la découpe. Champ de service non affiché : troisième critère d'ordonnancement du tableau. Une découpe encore au vivier, non affectée à une barre, porte cette valeur à l'état absent : état nominal, non anomalie.</summary>
        public int? PCPIdProductionBar { get; set; }
    }
}