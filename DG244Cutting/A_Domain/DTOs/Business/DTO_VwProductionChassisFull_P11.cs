namespace DG244Cutting.A_Domain.DTOs.Business
{
    /// <summary>
    /// Objet de transport de la composition physique d'une série de production, à
    /// raison d'une instance par châssis, affichable dans le troisième onglet de la
    /// Page11.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter la composition physique d'une série de production,
    /// à raison d'une instance par châssis, depuis la projection SQL portée par le
    /// couple <c>IR_VwProductionChassisFull</c> / <c>CR_VwProductionChassisFull</c>
    /// sur la vue <c>vw_ProductionChassis_Full</c>, vers le troisième onglet de la
    /// Page11. Seize champs projetés sur les soixante-seize colonnes de la vue : onze
    /// champs d'affichage dans l'ordre des colonnes du tableau, cinq champs de service
    /// non affichés dédiés à l'identification, à la vérification de cohérence du lot et
    /// à l'ordonnancement. Le suffixe <c>_P11</c> marque la destination, la même vue
    /// devant alimenter d'autres écrans avec d'autres sélections de colonnes.</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucune
    /// référence à EF Core.</para>
    /// <para>Invariant : les types transportés et leur caractère nullable recopient
    /// fidèlement ceux de la vue source, sans rien y ajouter ni en retrancher ; les
    /// préfixes de nom de propriété (PC, PS, CO) conservent la trace de la table
    /// d'origine de chaque colonne au sein de la jointure.</para>
    /// </remarks>
    public class DTO_VwProductionChassisFull_P11
    {
        /// <summary>Position du châssis dans la série - colonne P11_14 « Position série ».</summary>
        public short PCSeriesPosition { get; set; }

        /// <summary>Position du châssis telle qu'exprimée par le client - colonne P11_15 « Position commande ». À ne pas confondre avec <c>PCOrderPosition</c>, non affichée, qui porte le troisième critère d'ordonnancement du tableau.</summary>
        public string? PCCustomerPosition { get; set; }

        /// <summary>Identifiant code-barres du châssis, par lequel l'opérateur retrouve une pièce - colonne P11_16 « Code Barre ».</summary>
        public string PCBarcodeId { get; set; } = null!;

        /// <summary>Quantité de châssis identiques portée par la ligne - colonne P11_17 « Quantité ».</summary>
        public short PCQuantity { get; set; }

        /// <summary>Code du système de profilé du châssis - colonne P11_18 « Famille produit ».</summary>
        public string? PCWindowSystemCode { get; set; }

        /// <summary>Hauteur de l'élément - colonne P11_19 « Hauteur ».</summary>
        public short? PCElementHeight { get; set; }

        /// <summary>Largeur de l'élément - colonne P11_20 « Largeur ».</summary>
        public short? PCElementWidth { get; set; }

        /// <summary>Couleur intérieure et extérieure du châssis - colonne P11_21 « Couleur ».</summary>
        public string? PCColorNameIntExt { get; set; }

        /// <summary>Texte descriptif de la menuiserie - colonne P11_22 « Description 1 ».</summary>
        public string? PCWindowText { get; set; }

        /// <summary>Type de coulissant détaillé - colonne P11_23 « Description 2 ».</summary>
        public string? PCSlidingTypeDetailed { get; set; }

        /// <summary>Libellé du type d'ouverture - colonne P11_24 « Description 3 ».</summary>
        public string? PCOpeningTypeText { get; set; }

        /// <summary>Clé technique de la série de production. Champ de service non affiché : support de la vérification de cohérence du lot reçu, dont toutes les lignes portent la même série.</summary>
        public int PSId { get; set; }

        /// <summary>Clé technique du châssis. Champ de service non affiché : identification de chaque ligne du tableau.</summary>
        public int PCId { get; set; }

        /// <summary>Numéro de commande client. Champ de service non affiché : premier critère d'ordonnancement du tableau.</summary>
        public int COIdOrder { get; set; }

        /// <summary>Index de la série partielle de la commande client. Champ de service non affiché : deuxième critère d'ordonnancement du tableau.</summary>
        public int COPartialSeriesIndex { get; set; }

        /// <summary>Position du châssis dans la commande, telle que portée par la table d'origine du châssis. Champ de service non affiché : troisième critère d'ordonnancement du tableau. À ne pas confondre avec <c>PCCustomerPosition</c>, affichée en colonne P11_15 « Position commande ».</summary>
        public short PCOrderPosition { get; set; }
    }
}