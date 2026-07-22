using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ProductionBar_Full
{
    /// <summary>
    /// Source : [ProductionSeries] - Champ [Id] - Clé technique interne (IDENTITY). N’existe pas dans AX.
    /// </summary>
    public int PSId { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IdSerialNumber] - Numéro de série AX. Correspond au champ AX: SERIALNOSTR.
    /// </summary>
    public int PSIdSerialNumber { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IdRec] - Identifiant unique AX (RECID). Lien avec la ligne AX originale.
    /// </summary>
    public long PSIdRec { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [RecVersion] - Version du record dans AX (contrôle de concurrence AX).
    /// </summary>
    public int PSRecVersion { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [Description] - Description de la série. Champ AX: EEEA_SERIALDESCRIPTION.
    /// </summary>
    public string PSDescription { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionStartDate] - Date de début de production. Champ AX: EEEA_SERIALPLANDATE.
    /// </summary>
    public DateTime? PSProductionStartDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDate] - Date de fin de production. Champ AX: ATWIN_PRODUCTIONENDDATE.
    /// </summary>
    public DateTime? PSProductionEndDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDay] - Code couleur étiquette. 0 = Violet si date absente.
    /// </summary>
    public short PSProductionEndDay { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [SerieCreatedAt] - Date de création initiale de la série. Champ AX: CREATEDDATETIME.
    /// </summary>
    public DateTime? PSSerieCreatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsImported] - Données importées depuis un fichier Leitxx.mdb (False/True).
    /// </summary>
    public bool PSIsImported { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsProductionValidated] - Série validée pour lancement (False/True).
    /// </summary>
    public bool PSIsProductionValidated { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarOptimized] - Série optimisée pour découpe sur barres de chutes.
    /// </summary>
    public bool PSIsDropBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarSupplied] - Série approvisionnée en barres de chutes.
    /// </summary>
    public bool PSIsDropBarSupplied { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarOptimized] - Série optimisée pour découpe sur barres neuves.
    /// </summary>
    public bool PSIsNewBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarSupplied] - Série approvisionnée en barres neuves.
    /// </summary>
    public bool PSIsNewBarSupplied { get; set; }

    public bool PSIsBarOutOfStock { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingStarted] - Une des découpes de la série a été réalisée.
    /// </summary>
    public bool PSIsCuttingStarted { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingCompleted] - Ensemble des découpes de la série réalisées.
    /// </summary>
    public bool PSIsCuttingCompleted { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [Id] - Clé primaire de la référence article.
    /// </summary>
    public int ARId { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleCategory1] - Catégorie principale de l’article (Catégorie 1).
    /// </summary>
    public short ARIdArticleCategory1 { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleCategory2] - Catégorie secondaire de l’article (Catégorie 2).
    /// </summary>
    public short ARIdArticleCategory2 { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleCategory3] - Catégorie tertiaire de l’article (Catégorie 3).
    /// </summary>
    public short ARIdArticleCategory3 { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdCuttingMachine] - Machine de découpe associée à la référence.
    /// </summary>
    public int ARIdCuttingMachine { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleIdentificationType] - Type d’identification (pièce, barre, carton…).
    /// </summary>
    public short ARIdArticleIdentificationType { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleStorageUnit] - Unité de stockage (mètre, pièce, carton…).
    /// </summary>
    public int? ARIdArticleStorageUnit { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdScrapLocationHorizontal] - Emplacement horizontal par défaut des chutes.
    /// </summary>
    public int ARIdScrapLocationHorizontal { get; set; }

    /// <summary>
    /// Source : [CuttingScrapLocation] - Champ [Designation] - Désignation de l’emplacement horizontal des chutes.
    /// </summary>
    public string? CSLScrapLocationHorizontal { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdScrapLocationVertical] - Emplacement vertical par défaut des chutes.
    /// </summary>
    public int ARIdScrapLocationVertical { get; set; }

    /// <summary>
    /// Source : [CuttingScrapLocation] - Champ [Designation] - Désignation de l’emplacement vertical des chutes.
    /// </summary>
    public string? CSLScrapLocationVertical { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdSupplier] - Fournisseur associé à la référence (optionnel).
    /// </summary>
    public int? ARIdSupplier { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [Reference] - Code alphanumérique unique. Source Tempor_Import.CodeNat.
    /// </summary>
    public string ARReference { get; set; } = null!;

    /// <summary>
    /// Source : [ArticleReference] - Champ [Designation] - Désignation de la référence. Source Tempor_Import.Feld_10_100.
    /// </summary>
    public string ARDesignation { get; set; } = null!;

    /// <summary>
    /// Source : [ArticleReference] - Champ [FamilyCategory] - Catégorie famille. Source Tempor_Import.Feld_40.
    /// </summary>
    public string? ARFamilyCategory { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeFamily] - Code famille. Source Tempor_Import.Feld_16.
    /// </summary>
    public string? ARCodeFamily { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticleReference] - Code article de référence. Source Tempor_Import.Feld_4.
    /// </summary>
    public string? ARCodeArticleReference { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticle] - Code article principal. Source Tempor_Import.Feld_41.
    /// </summary>
    public string? ARCodeArticle { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticleCuttingMachine] - Code article machine de découpe. Source Feld_10_330.
    /// </summary>
    public string? ARCodeArticleCuttingMachine { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [MinScrapLength] - Longueur minimale de chute réutilisable.
    /// </summary>
    public decimal? ARMinScrapLength { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [MaxVerticalLength] - Longueur verticale maximale supportée par le stockage.
    /// </summary>
    public int ARMaxVerticalLength { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [SortOrder] - Ordre de tri pour l’affichage et les traitements.
    /// </summary>
    public short ARSortOrder { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [Id] - Identifiant interne unique de l’article interne (PK).
    /// </summary>
    public int AIId { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [IdArticleReference] - Identifiant de la référence article (FK ArticleReference.Id).
    /// </summary>
    public int AIIdArticleReference { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [IdColorRalFinish] - Identifiant couleur/finition (FK ColorRalFinish.Id).
    /// </summary>
    public string? AIIdColorRalFinish { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [StandardBarLengthMm] - Longueur de stockage de référence de l’article (mm).
    /// </summary>
    public double? AIStandardBarLengthMm { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [ManageScraps] - Article interne géré avec suivi des chutes.
    /// </summary>
    public bool AIManageScraps { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [Id] - Identifiant technique unique de la barre de production (PK).
    /// </summary>
    public int PBId { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IdProductionSeries] - Rattachement à la série de production (FK ProductionSeries.Id).
    /// </summary>
    public int PBIdProductionSeries { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IdArticleInternal] - Article interne de la barre (FK ArticleInternal.Id).
    /// </summary>
    public int PBIdArticleInternal { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IdSourceScrap] - Chute d’origine si barre réutilisée (FK CuttingScrapStock.Id).
    /// </summary>
    public int? PBIdSourceScrap { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [BarLength] - Longueur totale de la barre en millimètres.
    /// </summary>
    public int PBBarLength { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [DefectStart1] - Début du 1er défaut depuis le début de la barre (mm).
    /// </summary>
    public int? PBDefectStart1 { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [DefectEnd1] - Fin du 1er défaut (mm).
    /// </summary>
    public int? PBDefectEnd1 { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [DefectStart2] - Début du 2e défaut (mm).
    /// </summary>
    public int? PBDefectStart2 { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [DefectEnd2] - Fin du 2e défaut (mm).
    /// </summary>
    public int? PBDefectEnd2 { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [CutPieceCount] - Nombre de découpes réalisées sur la barre.
    /// </summary>
    public int PBCutPieceCount { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ResidueLength] - Reste calculé en mm, préliminaire avant validation.
    /// </summary>
    public int? PBResidueLength { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ResidueIsScrap] - Qualification automatique du reliquat ; par défaut True.
    /// </summary>
    public bool PBResidueIsScrap { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ResidueValidatedLength] - Longueur finale du reliquat validée (Page22, mm).
    /// </summary>
    public int? PBResidueValidatedLength { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ResidueFinalIsScrap] - Statut final validé, peut différer par forçage opérateur.
    /// </summary>
    public bool PBResidueFinalIsScrap { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ScrapBarcode] - Code-barre de la chute générée. Format BS- + Id sur 9 positions.
    /// </summary>
    public string? PBScrapBarcode { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ScrapLocation] - Emplacement littéral de la chute générée.
    /// </summary>
    public string? PBScrapLocation { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ResidueValue] - Valeur du reste sur base de ResidueValidatedLength.
    /// </summary>
    public decimal? PBResidueValue { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [ResidueValueUpdatedAt] - Date de mise à jour de la valeur du reste.
    /// </summary>
    public DateTime? PBResidueValueUpdatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [RejectionReason] - Commentaire de motif de refus de la barre.
    /// </summary>
    public string? PBRejectionReason { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsNewBar] - True = barre neuve, False = chute réutilisée.
    /// </summary>
    public bool PBIsNewBar { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsOptimizedTemp] - Placement provisoire, maintenu True sur la barre de référence.
    /// </summary>
    public bool PBIsOptimizedTemp { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsValidated] - Barre acceptée physiquement par l’opérateur.
    /// </summary>
    public bool PBIsValidated { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsOptimized] - Barre définitivement scellée dans le plan.
    /// </summary>
    public bool PBIsOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsUsed] - Barre utilisée pour des découpes ; True à la validation Page22.
    /// </summary>
    public bool PBIsUsed { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsOutOfStock] - Rupture de stock de barre neuve.
    /// </summary>
    public bool PBIsOutOfStock { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsResidueValidated] - Reliquat confirmé par l’opérateur en Page22.
    /// </summary>
    public bool PBIsResidueValidated { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [CreatedAt] - Date et heure de création de l’enregistrement.
    /// </summary>
    public DateTime PBCreatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [UpdatedAt] - Date et heure de dernière modification.
    /// </summary>
    public DateTime? PBUpdatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionBar] - Champ [IsDeleted] - Suppression logique (1 = supprimé, 0 = actif).
    /// </summary>
    public bool PBIsDeleted { get; set; }
}
