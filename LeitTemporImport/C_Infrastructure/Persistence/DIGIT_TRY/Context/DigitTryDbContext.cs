using System;
using System.Collections.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Context;

public partial class DigitTryDbContext : DbContext
{
    public DigitTryDbContext(DbContextOptions<DigitTryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppList> AppLists { get; set; }

    public virtual DbSet<ArticleCategory1> ArticleCategory1s { get; set; }

    public virtual DbSet<ArticleCategory2> ArticleCategory2s { get; set; }

    public virtual DbSet<ArticleCategory3> ArticleCategory3s { get; set; }

    public virtual DbSet<ArticleIdentificationType> ArticleIdentificationTypes { get; set; }

    public virtual DbSet<ArticleInternal> ArticleInternals { get; set; }

    public virtual DbSet<ArticleInternalConsumption> ArticleInternalConsumptions { get; set; }

    public virtual DbSet<ArticleInternalConsumptionReason> ArticleInternalConsumptionReasons { get; set; }

    public virtual DbSet<ArticleReference> ArticleReferences { get; set; }

    public virtual DbSet<ArticleStorageUnit> ArticleStorageUnits { get; set; }

    public virtual DbSet<ColorFinish> ColorFinishes { get; set; }

    public virtual DbSet<ColorRal> ColorRals { get; set; }

    public virtual DbSet<ColorRalFinish> ColorRalFinishes { get; set; }

    public virtual DbSet<CustomerOrder> CustomerOrders { get; set; }

    public virtual DbSet<CuttingMachine> CuttingMachines { get; set; }

    public virtual DbSet<CuttingScrapArchive> CuttingScrapArchives { get; set; }

    public virtual DbSet<CuttingScrapLocation> CuttingScrapLocations { get; set; }

    public virtual DbSet<CuttingScrapStock> CuttingScrapStocks { get; set; }

    public virtual DbSet<LUUXASeries> LUUXASERIEs { get; set; }

    public virtual DbSet<LifecycleAction> LifecycleActions { get; set; }

    public virtual DbSet<LifecycleActionSource> LifecycleActionSources { get; set; }

    public virtual DbSet<LifecycleActionType> LifecycleActionTypes { get; set; }

    public virtual DbSet<ProductionChassi> ProductionChasses { get; set; }

    public virtual DbSet<ProductionColorLabelType> ProductionColorLabelTypes { get; set; }

    public virtual DbSet<ProductionCutPiece> ProductionCutPieces { get; set; }

    public virtual DbSet<ProductionFrameSash> ProductionFrameSashes { get; set; }

    public virtual DbSet<ProductionSeries> ProductionSeries { get; set; }

    public virtual DbSet<SidePosition> SidePositions { get; set; }

    public virtual DbSet<SpatialPosition> SpatialPositions { get; set; }

    public virtual DbSet<StockBin> StockBins { get; set; }

    public virtual DbSet<StockBinItem> StockBinItems { get; set; }

    public virtual DbSet<StockBinType> StockBinTypes { get; set; }

    public virtual DbSet<StockChariot> StockChariots { get; set; }

    public virtual DbSet<StockSupportType> StockSupportTypes { get; set; }

    public virtual DbSet<StockZone> StockZones { get; set; }

    public virtual DbSet<StockZoneAddress> StockZoneAddresses { get; set; }

    public virtual DbSet<Tempor_Import> Tempor_Imports { get; set; }

    public virtual DbSet<UserApp> UserApps { get; set; }

    public virtual DbSet<UserAppAccess> UserAppAccesses { get; set; }

    public virtual DbSet<UserAppErrorLog> UserAppErrorLogs { get; set; }

    public virtual DbSet<UserAppEventStore> UserAppEventStores { get; set; }

    public virtual DbSet<UserAppMessage> UserAppMessages { get; set; }

    public virtual DbSet<UserAppPage> UserAppPages { get; set; }

    public virtual DbSet<UserAppPageRight> UserAppPageRights { get; set; }

    public virtual DbSet<UserAppSession> UserAppSessions { get; set; }

    public virtual DbSet<UserAppSessionCommand> UserAppSessionCommands { get; set; }

    public virtual DbSet<vw_ArticleInternalDetail> vw_ArticleInternalDetails { get; set; }

    public virtual DbSet<vw_ProductionCutPiece_Control_Coherence> vw_ProductionCutPiece_Control_Coherences { get; set; }

    public virtual DbSet<vw_ProductionCutPiece_Full> vw_ProductionCutPiece_Fulls { get; set; }

    public virtual DbSet<vw_Source_ArticleInternal_Missing> vw_Source_ArticleInternal_Missings { get; set; }

    public virtual DbSet<vw_Source_ArticleReference_Missing> vw_Source_ArticleReference_Missings { get; set; }

    public virtual DbSet<vw_Source_ColorRalFinish_Missing> vw_Source_ColorRalFinish_Missings { get; set; }

    public virtual DbSet<vw_Source_CustomerOrder_Missing> vw_Source_CustomerOrder_Missings { get; set; }

    public virtual DbSet<vw_Source_ProductionChassis_Missing> vw_Source_ProductionChassis_Missings { get; set; }

    public virtual DbSet<vw_Source_ProductionCutPiece_Missing> vw_Source_ProductionCutPiece_Missings { get; set; }

    public virtual DbSet<vw_Source_ProductionFrameSash_Missing> vw_Source_ProductionFrameSash_Missings { get; set; }

    public virtual DbSet<vw_Source_ProductionSeries> vw_Source_ProductionSeries { get; set; }

    public virtual DbSet<vw_Source_SpatialPosition_Missing> vw_Source_SpatialPosition_Missings { get; set; }

    public virtual DbSet<vw_StockBinItemDetail> vw_StockBinItemDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppList>(entity =>
        {
            entity.ToTable("AppList", tb => tb.HasComment("Liste des applications disponibles dans le système. Utilisée pour la gestion des droits, des sessions et de la traçabilité applicative."));

            entity.HasIndex(e => e.Designation, "IX_AppList_Designation");

            entity.Property(e => e.Id).HasComment("Identifiant unique de l’application.");
            entity.Property(e => e.Accessible).HasComment("Indique si l’application est activée et visible pour l’utilisateur.");
            entity.Property(e => e.Comments)
                .HasMaxLength(1000)
                .HasComment("Commentaire interne ou description de l’usage.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure de création de l’enregistrement.");
            entity.Property(e => e.Designation)
                .HasMaxLength(200)
                .HasComment("Désignation fonctionnelle de l’application.");
            entity.Property(e => e.IsDeleted).HasComment("Suppression logique (1 = supprimé, 0 = actif).");
            entity.Property(e => e.UpdatedAt).HasComment("Date et heure de dernière modification.");
        });

        modelBuilder.Entity<ArticleCategory1>(entity =>
        {
            entity.ToTable("ArticleCategory1", tb => tb.HasComment("First-level article classification reference."));

            entity.HasIndex(e => e.Designation, "UQ_ArticleCategory1_Designation").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Designation).HasMaxLength(100);
        });

        modelBuilder.Entity<ArticleCategory2>(entity =>
        {
            entity.ToTable("ArticleCategory2", tb => tb.HasComment("Second-level article classification reference."));

            entity.HasIndex(e => e.Designation, "UQ_ArticleCategory2_Designation").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Designation).HasMaxLength(100);
        });

        modelBuilder.Entity<ArticleCategory3>(entity =>
        {
            entity.ToTable("ArticleCategory3", tb => tb.HasComment("Third-level article classification reference."));

            entity.HasIndex(e => e.Designation, "UQ_ArticleCategory3_Designation").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Designation).HasMaxLength(100);
        });

        modelBuilder.Entity<ArticleIdentificationType>(entity =>
        {
            entity.ToTable("ArticleIdentificationType", tb => tb.HasComment("Table defining how each article is physically identified (piece, bar, box, etc.)."));

            entity.HasIndex(e => e.Code, "UQ_ArticleIdentificationType_Code").IsUnique();

            entity.HasIndex(e => e.Designation, "UQ_ArticleIdentificationType_Designation").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Designation).HasMaxLength(100);
        });

        modelBuilder.Entity<ArticleInternal>(entity =>
        {
            entity.ToTable("ArticleInternal", tb => tb.HasComment("Articles internes : variante physique/couleur d’une référence, utilisée pour le stock et la production."));

            entity.HasIndex(e => e.IdColorRalFinish, "IX_ArticleInternal_IdColorRalFinish").HasFillFactor(100);

            entity.HasIndex(e => new { e.IdArticleReference, e.IdColorRalFinish }, "UQ_ArticleInternal_RefColor").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant interne unique de l’article interne (PK).");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.IdArticleReference).HasComment("Identifiant de la référence article (FK ArticleReference.Id).");
            entity.Property(e => e.IdColorRalFinish)
                .HasMaxLength(50)
                .HasComment("Identifiant couleur/finition (FK ColorRalFinish.Id).");
            entity.Property(e => e.IsDeleted).HasComment("Date de dernière mise à jour de l’enregistrement.");
            entity.Property(e => e.ManageScraps).HasComment("Indique si l’article interne est géré avec suivi des chutes.");
            entity.Property(e => e.StandardBarLengthMm)
                .HasDefaultValue(0.0)
                .HasComment("Longueur de stockage de référence de l’article, exprimée en millimètres.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de l’enregistrement.");

            entity.HasOne(d => d.IdArticleReferenceNavigation).WithMany(p => p.ArticleInternals)
                .HasForeignKey(d => d.IdArticleReference)
                .HasConstraintName("FK_ArticleInternal_ArticleReference");

            entity.HasOne(d => d.IdColorRalFinishNavigation).WithMany(p => p.ArticleInternals)
                .HasForeignKey(d => d.IdColorRalFinish)
                .HasConstraintName("FK_ArticleInternal_ColorRalFinish");
        });

        modelBuilder.Entity<ArticleInternalConsumption>(entity =>
        {
            entity.ToTable("ArticleInternalConsumption", tb => tb.HasComment("Table enregistrant toutes les consommations d’articles internes réalisées dans l’atelier."));

            entity.Property(e => e.Id).HasComment("Identifiant unique de la consommation.");
            entity.Property(e => e.ConsumptionDate)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date et heure de la consommation (UTC).");
            entity.Property(e => e.ContainerName)
                .HasMaxLength(50)
                .HasComment("Nom du conteneur depuis lequel la consommation a été réalisée.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date de création de la ligne (UTC).");
            entity.Property(e => e.IdArticleInternal).HasComment("Identifiant de l’article interne consommé (FK → ArticleInternal.Id).");
            entity.Property(e => e.IdConsumptionReason).HasComment("Motif de la consommation (FK → ArticleInternalConsumptionReason.Id).");
            entity.Property(e => e.IdUserApp).HasComment("Utilisateur ayant effectué ou déclaré la consommation (FK → UserApp.Id).");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la ligne est supprimée logiquement.");
            entity.Property(e => e.LocationName)
                .HasMaxLength(50)
                .HasComment("Nom de l’adresse ou emplacement concerné (si disponible).");
            entity.Property(e => e.ManualConsumptionNumber)
                .HasMaxLength(20)
                .HasComment("Numéro de consommation manuelle (optionnel).");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(20)
                .HasComment("Numéro de commande associé (si existant).");
            entity.Property(e => e.Quantity).HasComment("Quantité consommée lors de l’opération.");
            entity.Property(e => e.Reason)
                .HasMaxLength(100)
                .HasComment("Désignation lisible du motif (copie à des fins d’audit).");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de la ligne (UTC).");

            entity.HasOne(d => d.IdArticleInternalNavigation).WithMany(p => p.ArticleInternalConsumptions)
                .HasForeignKey(d => d.IdArticleInternal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumption_ArticleInternal");

            entity.HasOne(d => d.IdConsumptionReasonNavigation).WithMany(p => p.ArticleInternalConsumptions)
                .HasForeignKey(d => d.IdConsumptionReason)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumption_Reason");

            entity.HasOne(d => d.IdUserAppNavigation).WithMany(p => p.ArticleInternalConsumptions)
                .HasForeignKey(d => d.IdUserApp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumption_UserApp");
        });

        modelBuilder.Entity<ArticleInternalConsumptionReason>(entity =>
        {
            entity.ToTable("ArticleInternalConsumptionReason", tb => tb.HasComment("Table de référence contenant les motifs de consommation des articles internes."));

            entity.HasIndex(e => e.Designation, "UQ_ArticleInternalConsumptionReason_Designation").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique du motif de consommation.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date et heure de création de l’enregistrement, générée automatiquement par le système.");
            entity.Property(e => e.Designation)
                .HasMaxLength(50)
                .HasComment("Désignation textuelle du motif de consommation (exemples : \"Inventaire\", \"Sortie atelier\", \"Correction stock\"). Doit être unique.");
            entity.Property(e => e.IsDeleted).HasComment("Indicateur de suppression logique (0 = actif, 1 = supprimé).");
            entity.Property(e => e.UpdatedAt).HasComment("Date et heure de la dernière mise à jour de l’enregistrement.");
        });

        modelBuilder.Entity<ArticleReference>(entity =>
        {
            entity.ToTable("ArticleReference", tb => tb.HasComment("Définit les références articles."));

            entity.HasIndex(e => e.Reference, "IX_ArticleReference_Active_Reference").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Reference, "UQ_ArticleReference_Reference").IsUnique();

            entity.Property(e => e.Id).HasComment("Clé primaire de la référence article.");
            entity.Property(e => e.CodeArticle)
                .HasMaxLength(100)
                .HasComment("Code article principal issu du champ Tempor_Import.Feld_41.");
            entity.Property(e => e.CodeArticleCuttingMachine)
                .HasMaxLength(100)
                .HasComment("Code article spécifique à la machine de découpe, issu du champ Tempor_Import.Feld_10_330.");
            entity.Property(e => e.CodeArticleReference)
                .HasMaxLength(100)
                .HasComment("Code article de référence issu du champ Tempor_Import.Feld_4.");
            entity.Property(e => e.CodeFamily)
                .HasMaxLength(100)
                .HasComment("Code famille de la référence article, issu du champ Tempor_Import.Feld_16.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure de création de l’enregistrement.");
            entity.Property(e => e.Designation)
                .HasMaxLength(300)
                .HasComment("Désignation lisible et descriptive de la référence article issu du champ Tempor_Import.Feld_10_100.");
            entity.Property(e => e.FamilyCategory)
                .HasMaxLength(100)
                .HasComment("Catégorie famille de la référence article, issue du champ Tempor_Import.Feld_40.");
            entity.Property(e => e.IdArticleCategory1)
                .HasDefaultValue((short)1)
                .HasComment("Clé étrangère : catégorie principale de l’article (Catégorie 1).");
            entity.Property(e => e.IdArticleCategory2)
                .HasDefaultValue((short)1)
                .HasComment("Clé étrangère : catégorie secondaire de l’article (Catégorie 2).");
            entity.Property(e => e.IdArticleCategory3)
                .HasDefaultValue((short)1)
                .HasComment("Clé étrangère : catégorie tertiaire de l’article (Catégorie 3).");
            entity.Property(e => e.IdArticleIdentificationType)
                .HasDefaultValue((short)1)
                .HasComment("Clé étrangère : type d’identification (pièce, barre, carton…).");
            entity.Property(e => e.IdArticleStorageUnit).HasComment("Clé étrangère : unité de stockage (mètre, pièce, carton…).");
            entity.Property(e => e.IdCuttingMachine)
                .HasDefaultValue(1)
                .HasComment("Clé étrangère : machine de découpe associée à la référence.");
            entity.Property(e => e.IdScrapLocationHorizontal)
                .HasDefaultValue(1)
                .HasComment("Clé étrangère : emplacement horizontal par défaut des chutes.");
            entity.Property(e => e.IdScrapLocationVertical)
                .HasDefaultValue(1)
                .HasComment("Clé étrangère : emplacement vertical par défaut des chutes.");
            entity.Property(e => e.IdSupplier).HasComment("Clé étrangère optionnelle : fournisseur associé à la référence.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la référence est supprimée logiquement (0=active, 1=supprimée).");
            entity.Property(e => e.ManageScraps).HasComment("Indique si la référence participe à la gestion des chutes.");
            entity.Property(e => e.MaxVerticalLength)
                .HasDefaultValue(3000)
                .HasComment("Longueur verticale maximale supportée par le stockage.");
            entity.Property(e => e.MinScrapLength)
                .HasDefaultValue(1000m)
                .HasComment("Longueur minimale de chute réutilisable pour la découpe.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Reference)
                .HasMaxLength(100)
                .HasComment("Code alphanumérique unique identifiant la référence article issu du champ Tempor_Import.CodeNat.");
            entity.Property(e => e.SortOrder)
                .HasDefaultValue((short)1)
                .HasComment("Ordre de tri utilisé pour l’affichage et les traitements.");
            entity.Property(e => e.StandardBarLengthMm)
                .HasComment("Longueur standard de stockage de l’article, en millimètres issu du champ Tempor_Import.Wert_40.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.UpdatedAt).HasComment("Date et heure de la dernière mise à jour.");

            entity.HasOne(d => d.IdArticleCategory1Navigation).WithMany(p => p.ArticleReferences)
                .HasForeignKey(d => d.IdArticleCategory1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleReference_ArticleCategory1");

            entity.HasOne(d => d.IdArticleCategory2Navigation).WithMany(p => p.ArticleReferences)
                .HasForeignKey(d => d.IdArticleCategory2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleReference_ArticleCategory2");

            entity.HasOne(d => d.IdArticleCategory3Navigation).WithMany(p => p.ArticleReferences)
                .HasForeignKey(d => d.IdArticleCategory3)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleReference_ArticleCategory3");

            entity.HasOne(d => d.IdArticleIdentificationTypeNavigation).WithMany(p => p.ArticleReferences)
                .HasForeignKey(d => d.IdArticleIdentificationType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleReference_ArticleIdentificationType");

            entity.HasOne(d => d.IdArticleStorageUnitNavigation).WithMany(p => p.ArticleReferences)
                .HasForeignKey(d => d.IdArticleStorageUnit)
                .HasConstraintName("FK_ArticleReference_StorageUnit");

            entity.HasOne(d => d.IdCuttingMachineNavigation).WithMany(p => p.ArticleReferences)
                .HasForeignKey(d => d.IdCuttingMachine)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleReference_CuttingMachine");

            entity.HasOne(d => d.IdScrapLocationHorizontalNavigation).WithMany(p => p.ArticleReferenceIdScrapLocationHorizontalNavigations)
                .HasForeignKey(d => d.IdScrapLocationHorizontal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleReference_ScrapLocationH");

            entity.HasOne(d => d.IdScrapLocationVerticalNavigation).WithMany(p => p.ArticleReferenceIdScrapLocationVerticalNavigations)
                .HasForeignKey(d => d.IdScrapLocationVertical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleReference_ScrapLocationV");
        });

        modelBuilder.Entity<ArticleStorageUnit>(entity =>
        {
            entity.ToTable("ArticleStorageUnit", tb => tb.HasComment("Defines the physical storage measurement units for articles (piece, meter, kg, etc.). IDs preserved from GestStock for compatibility."));

            entity.HasIndex(e => e.Code, "UQ_ArticleStorageUnit_Code").IsUnique();

            entity.HasIndex(e => e.Designation, "UQ_ArticleStorageUnit_Designation").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Designation).HasMaxLength(100);
        });

        modelBuilder.Entity<ColorFinish>(entity =>
        {
            entity.ToTable("ColorFinish", tb => tb.HasComment("Stores available finish types for aluminium profiles (mat, glossy, structured, anodized, etc.)."));

            entity.HasIndex(e => e.Designation, "IX_ColorFinish_Designation");

            entity.HasIndex(e => e.Designation, "UQ_ColorFinish_Designation").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(3)
                .HasDefaultValue("G")
                .HasComment("Unique code identifying the surface finish (e.g., G for Glossy, M for Mat).");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .HasComment("Name or description of the surface finish.");
        });

        modelBuilder.Entity<ColorRal>(entity =>
        {
            entity.ToTable("ColorRal", tb => tb.HasComment("Stores RAL color references including designations and various color code formats (Hex, RGB, CMYK)."));

            entity.HasIndex(e => e.DesignationFr, "IX_ColorRal_DesignationFr");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BaseReference).HasMaxLength(300);
            entity.Property(e => e.CmykCode).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DesignationEn)
                .HasMaxLength(100)
                .HasComment("English color name as per the RAL standard.");
            entity.Property(e => e.DesignationFr)
                .HasMaxLength(100)
                .HasComment("French color name as per the RAL standard.");
            entity.Property(e => e.HexCode).HasMaxLength(100);
            entity.Property(e => e.RgbCode).HasMaxLength(100);
        });

        modelBuilder.Entity<ColorRalFinish>(entity =>
        {
            entity.ToTable("ColorRalFinish", tb => tb.HasComment("Associates RAL color references with surface finish types (internal and external)."));

            entity.HasIndex(e => e.IdExternalRal, "IX_ColorRalFinish_ExternalRal");

            entity.HasIndex(e => e.IdInternalRal, "IX_ColorRalFinish_InternalRal");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IdExternalFinish)
                .HasMaxLength(3)
                .HasDefaultValue("I");
            entity.Property(e => e.IdExternalRal).HasDefaultValue(9999);
            entity.Property(e => e.IdInternalFinish)
                .HasMaxLength(3)
                .HasDefaultValue("I")
                .HasComment("Finish reference for the internal face (mat, glossy, structured, etc.).");
            entity.Property(e => e.IdInternalRal)
                .HasDefaultValue(9999)
                .HasComment("RAL color reference for the internal face.");

            entity.HasOne(d => d.IdExternalFinishNavigation).WithMany(p => p.ColorRalFinishIdExternalFinishNavigations)
                .HasForeignKey(d => d.IdExternalFinish)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ColorRalFinish_ExternalFinish");

            entity.HasOne(d => d.IdExternalRalNavigation).WithMany(p => p.ColorRalFinishIdExternalRalNavigations)
                .HasForeignKey(d => d.IdExternalRal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ColorRalFinish_ExternalRal");

            entity.HasOne(d => d.IdInternalFinishNavigation).WithMany(p => p.ColorRalFinishIdInternalFinishNavigations)
                .HasForeignKey(d => d.IdInternalFinish)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ColorRalFinish_InternalFinish");

            entity.HasOne(d => d.IdInternalRalNavigation).WithMany(p => p.ColorRalFinishIdInternalRalNavigations)
                .HasForeignKey(d => d.IdInternalRal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ColorRalFinish_InternalRal");
        });

        modelBuilder.Entity<CustomerOrder>(entity =>
        {
            entity.ToTable("CustomerOrder", tb => tb.HasComment("Commande client issue de Tempor_Import (Leitxx.mdb)"));

            entity.HasIndex(e => e.IdProductionSeries, "IX_CustomerOrder_IdProductionSeries");

            entity.HasIndex(e => e.PartialSeriesIndex, "IX_CustomerOrder_PartialSeriesIndex");

            entity.HasIndex(e => new { e.IdProductionSeries, e.PartialSeriesIndex }, "IX_CustomerOrder_Series_Partial");

            entity.HasIndex(e => e.IdOrder, "UX_CustomerOrder_IdOrder")
                .IsUnique()
                .HasFillFactor(100);

            entity.Property(e => e.Id).HasComment("Clé technique interne.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création SQL.");
            entity.Property(e => e.CustomerCity)
                .HasMaxLength(100)
                .HasComment("Ville du chantier. Source : Tempor_Import.Feld_10_086.");
            entity.Property(e => e.CustomerCountry)
                .HasMaxLength(50)
                .HasComment("Pays du chantier. Source : Tempor_Import.Feld_10_084.");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(200)
                .HasComment("Nom du client final. Source : Tempor_Import.Feld_10_163.");
            entity.Property(e => e.CustomerProjectDesignation)
                .HasMaxLength(200)
                .HasComment("Désignation projet. Source : Tempor_Import.Feld_10_002.");
            entity.Property(e => e.CustomerProjectName)
                .HasMaxLength(200)
                .HasComment("Nom du chantier. Source : Tempor_Import.Feld_10_083.");
            entity.Property(e => e.CustomerStreet)
                .HasMaxLength(200)
                .HasComment("Rue du chantier. Source : Tempor_Import.Feld_10_087.");
            entity.Property(e => e.CustomerZipCode)
                .HasMaxLength(20)
                .HasComment("Code postal du chantier. Source : Tempor_Import.Feld_10_085.");
            entity.Property(e => e.DeliveryDate).HasComment("Date de livraison. Source : Tempor_Import.Feld_10_072.");
            entity.Property(e => e.DeliveryPosition)
                .HasMaxLength(100)
                .HasComment("Position de livraison. Source : Tempor_Import.Feld_10_118.");
            entity.Property(e => e.IdOrder).HasComment("Numéro commande Tryba. Source : Tempor_Import.Aunummer.");
            entity.Property(e => e.IdProductionSeries).HasComment("FK série de production. Source : Tempor_Import.SerieNr.");
            entity.Property(e => e.IsDeleted).HasComment("Suppression logique.");
            entity.Property(e => e.LookCustomerOrderId)
                .HasMaxLength(100)
                .HasComment("Identifiant Look3E pour la Commande Client. Source : Tempor_Import.Feld_10_205.");
            entity.Property(e => e.MainSalesPoint)
                .HasMaxLength(50)
                .HasComment("Code client principal. Source : Tempor_Import.Feld_10_273.");
            entity.Property(e => e.MainSalesPointAddress)
                .HasMaxLength(200)
                .HasComment("Adresse client principal. Source : Tempor_Import.Feld_10_274.");
            entity.Property(e => e.MainSalesPointCode)
                .HasMaxLength(50)
                .HasComment("Numéro client principal. Source : Tempor_Import.Feld_10_326.");
            entity.Property(e => e.MainSalesPointName)
                .HasMaxLength(200)
                .HasComment("Point de vente principal. Source : Tempor_Import.Feld_10_110.");
            entity.Property(e => e.ManufacturingPlant)
                .HasMaxLength(50)
                .HasComment("Usine de fabrication. Source : Tempor_Import.Feld_10_081.");
            entity.Property(e => e.ManufacturingSite)
                .HasMaxLength(50)
                .HasComment("Site de fabrication. Source : Tempor_Import.Feld_10_073.");
            entity.Property(e => e.OrderSponsor)
                .HasMaxLength(200)
                .HasComment("Commanditaire de la commande. Source : Tempor_Import.Feld_10_299.");
            entity.Property(e => e.PartialSeriesIndex).HasComment("Index série partielle. Source : Tempor_Import.TeilserienIndex.");
            entity.Property(e => e.ProductionEndDate).HasComment("Fin production. Source : Tempor_Import.Feld_10_212.");
            entity.Property(e => e.ProductionEndTourId)
                .HasMaxLength(50)
                .HasComment("Tournée fin production. Source : Tempor_Import.Feld_10_053.");
            entity.Property(e => e.ProductionEndWeek).HasComment("Semaine fin production (AAWW ex 2503=2025 sem 03). Source : Tempor_Import.Feld_10_054.");
            entity.Property(e => e.ProductionEndWeekday).HasComment("Jour semaine fin production. Source : Tempor_Import.Feld_10_545.");
            entity.Property(e => e.ProductionStartDate).HasComment("Début production. Source : Tempor_Import.Feld_10_082.");
            entity.Property(e => e.ProductionStartWeek).HasComment("Semaine début production (AAWW ex 2503=2025 sem 03). Source : Tempor_Import.Feld_10_243.");
            entity.Property(e => e.ProjectDesignation)
                .HasMaxLength(100)
                .HasComment("Sous-série + gamme + couleur. Source : Tempor_Import.Feld_10_288.");
            entity.Property(e => e.ProjectNumber).HasComment("Numéro de projet. Source : Tempor_Import.Feld_10_171.");
            entity.Property(e => e.QuaiZone)
                .HasMaxLength(100)
                .HasComment("Zone de quai. Source : Tempor_Import.Feld_10_184.");
            entity.Property(e => e.SecondarySalesPointName)
                .HasMaxLength(200)
                .HasComment("Point de vente secondaire. Source : Tempor_Import.Feld_10_024.");
            entity.Property(e => e.ShippingDate).HasComment("Date d’expédition. Source : Tempor_Import.Feld_10_213.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de mise à jour SQL.");

            entity.HasOne(d => d.IdProductionSeriesNavigation).WithMany(p => p.CustomerOrders)
                .HasForeignKey(d => d.IdProductionSeries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerOrder_ProductionSeries");
        });

        modelBuilder.Entity<CuttingMachine>(entity =>
        {
            entity.ToTable("CuttingMachine", tb => tb.HasComment("Table storing configuration data for each cutting machine in the aluminium workshop."));

            entity.HasIndex(e => e.MachineCode, "UQ_CuttingMachine_MachineCode").IsUnique();

            entity.Property(e => e.ConsoleComPort).HasMaxLength(4);
            entity.Property(e => e.ConsoleIpAddress).HasMaxLength(20);
            entity.Property(e => e.ConsoleName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Designation)
                .HasMaxLength(300)
                .HasDefaultValue("To be completed");
            entity.Property(e => e.MachineCode)
                .HasMaxLength(20)
                .HasDefaultValue("DG244_xx")
                .HasComment("Unique code identifying the cutting machine (e.g., DG244_01).");
            entity.Property(e => e.PcComPort).HasMaxLength(4);
            entity.Property(e => e.PcIpAddress).HasMaxLength(20);
            entity.Property(e => e.PcName).HasMaxLength(50);
            entity.Property(e => e.PrinterIpAddress).HasMaxLength(20);
            entity.Property(e => e.PrinterName).HasMaxLength(50);
        });

        modelBuilder.Entity<CuttingScrapArchive>(entity =>
        {
            entity.ToTable("CuttingScrapArchive", tb => tb.HasComment("Historique complet des chutes issues des opérations de découpe."));

            entity.Property(e => e.Id).HasComment("Identifiant unique de la ligne dans l’archive des chutes.");
            entity.Property(e => e.Barcode)
                .HasMaxLength(30)
                .HasComment("Code-barres associé à la chute pour identification automatique.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure de création de l’enregistrement dans le système.");
            entity.Property(e => e.EntryDate).HasComment("Date et heure d’entrée de la chute dans le stock.");
            entity.Property(e => e.ExitDate).HasComment("Date et heure de sortie ou de consommation de la chute.");
            entity.Property(e => e.IdArticleInternal).HasComment("Référence vers l’article interne correspondant à la chute enregistrée.");
            entity.Property(e => e.IdCutType).HasComment("Type de chute (ex : barre, panneau). Utilisé si plusieurs types de chutes doivent être différenciés.");
            entity.Property(e => e.IdOperatorEntry).HasComment("Utilisateur (UserApp) ayant enregistré l’entrée de la chute dans le stock.");
            entity.Property(e => e.IdOperatorExit).HasComment("Utilisateur (UserApp) ayant consommé ou sorti la chute du stock.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la ligne est supprimée logiquement (soft delete). 0 = actif, 1 = supprimé.");
            entity.Property(e => e.IsScannedOnEntry).HasComment("Indique si la chute a été scannée lors de son entrée dans le stock (0 = non, 1 = oui).");
            entity.Property(e => e.IsScannedOnExit).HasComment("Indique si la sortie ou consommation de la chute a été réalisée via un scan.");
            entity.Property(e => e.LengthMm).HasComment("Longueur de la chute en millimètres.");
            entity.Property(e => e.Price).HasComment("Valeur estimée de la chute, utilisée pour le calcul d’inventaire ou d’analyse de coûts.");
            entity.Property(e => e.ReservedFor)
                .HasMaxLength(50)
                .HasComment("Indication de réservation de la chute (ex : numéro de lot, commande, projet).");
            entity.Property(e => e.UpdatedAt).HasComment("Dernière date de mise à jour de la ligne.");
            entity.Property(e => e.WidthMm).HasComment("Largeur de la chute (si applicable). Peut rester NULL pour les barres.");

            entity.HasOne(d => d.IdArticleInternalNavigation).WithMany(p => p.CuttingScrapArchives)
                .HasForeignKey(d => d.IdArticleInternal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CuttingScrapArchive_ArticleInternal");

            entity.HasOne(d => d.IdOperatorEntryNavigation).WithMany(p => p.CuttingScrapArchiveIdOperatorEntryNavigations)
                .HasForeignKey(d => d.IdOperatorEntry)
                .HasConstraintName("FK_CuttingScrapArchive_OperatorEntry");

            entity.HasOne(d => d.IdOperatorExitNavigation).WithMany(p => p.CuttingScrapArchiveIdOperatorExitNavigations)
                .HasForeignKey(d => d.IdOperatorExit)
                .HasConstraintName("FK_CuttingScrapArchive_OperatorExit");
        });

        modelBuilder.Entity<CuttingScrapLocation>(entity =>
        {
            entity.ToTable("CuttingScrapLocation", tb => tb.HasComment("Defines storage locations for cutting scraps (horizontal or vertical)."));

            entity.HasIndex(e => e.OrderIndex, "IX_CuttingScrapLocation_OrderIndex").HasFillFactor(100);

            entity.HasIndex(e => e.Designation, "UQ_CuttingScrapLocation_Designation").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Designation).HasMaxLength(45);
            entity.Property(e => e.IsHorizontal).HasComment("Indicates whether the scrap location is horizontal (1) or not (0).");
            entity.Property(e => e.IsVertical).HasComment("Indicates whether the scrap location is vertical (1) or not (0).");
            entity.Property(e => e.MaxQuantity).HasDefaultValue(1);
        });

        modelBuilder.Entity<CuttingScrapStock>(entity =>
        {
            entity.ToTable("CuttingScrapStock", tb => tb.HasComment("Représente le stock actif des chutes disponibles dans l’atelier aluminium."));

            entity.HasIndex(e => e.Barcode, "UQ_CuttingScrapStock_Barcode").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique de la chute en stock.");
            entity.Property(e => e.Barcode)
                .HasMaxLength(30)
                .HasComment("Code-barres unique permettant l’identification de la chute.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.EntryDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date d’entrée de la chute en stock.");
            entity.Property(e => e.IdArticleInternal).HasComment("Référence vers l’article interne dont provient la chute.");
            entity.Property(e => e.IdOperator).HasComment("Utilisateur ayant réalisé l’action d’enregistrement ou de modification.");
            entity.Property(e => e.IdStockBin).HasComment("Identifie l’emplacement physique (StockBin) où est rangée la chute.");
            entity.Property(e => e.IntegrationDate).HasComment("Date à laquelle la chute a été intégrée dans le système.");
            entity.Property(e => e.InventoryDate).HasComment("Date de la dernière opération d’inventaire concernant cette chute.");
            entity.Property(e => e.IsDeleted).HasComment("Marque la chute comme supprimée (soft delete).");
            entity.Property(e => e.IsInInventory).HasComment("Indique si la chute est actuellement comptabilisée dans l’inventaire.");
            entity.Property(e => e.IsScanned).HasComment("Indique si la chute a été scannée lors de sa prise en charge.");
            entity.Property(e => e.LengthMm).HasComment("Longueur de la chute (en millimètres).");
            entity.Property(e => e.Origin)
                .HasMaxLength(100)
                .HasComment("Origine déclarée de la chute (poste, machine, opération).");
            entity.Property(e => e.Price).HasComment("Dernier prix connu associé à cette chute.");
            entity.Property(e => e.ReservedFor)
                .HasMaxLength(20)
                .HasComment("Numéro de lot ou de projet ayant réservé cette chute.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière modification de l’enregistrement.");
            entity.Property(e => e.WaitForIntegration).HasComment("Indique si la chute est en attente d’intégration dans le système.");
            entity.Property(e => e.WidthMm).HasComment("Largeur de la chute (en millimètres).");

            entity.HasOne(d => d.IdArticleInternalNavigation).WithMany(p => p.CuttingScrapStocks)
                .HasForeignKey(d => d.IdArticleInternal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CuttingScrapStock_ArticleInternal");

            entity.HasOne(d => d.IdOperatorNavigation).WithMany(p => p.CuttingScrapStocks)
                .HasForeignKey(d => d.IdOperator)
                .HasConstraintName("FK_CuttingScrapStock_Operator");

            entity.HasOne(d => d.IdStockBinNavigation).WithMany(p => p.CuttingScrapStocks)
                .HasForeignKey(d => d.IdStockBin)
                .HasConstraintName("FK_CuttingScrapStock_StockBin");
        });

        modelBuilder.Entity<LUUXASeries>(entity =>
        {
            entity.HasKey(e => e.SERIALNOSTR);

            entity.ToTable("LUUXASERIES");

            entity.Property(e => e.SERIALNOSTR).HasMaxLength(11);
            entity.Property(e => e.ATWIN_PRODUCTIONENDDATE).HasColumnType("datetime");
            entity.Property(e => e.CREATEDDATETIME).HasColumnType("datetime");
            entity.Property(e => e.EEEA_SERIALDESCRIPTION).HasMaxLength(50);
            entity.Property(e => e.EEEA_SERIALPLANDATE).HasColumnType("datetime");
        });

        modelBuilder.Entity<LifecycleAction>(entity =>
        {
            entity.ToTable("LifecycleAction", tb => tb.HasComment("Historique des actions liées au cycle de vie d’une série de production."));

            entity.HasIndex(e => new { e.IdLifecycleActionSource, e.IdSource, e.ActionTimestamp }, "IX_LifecycleAction_Source")
                .IsDescending(false, false, true)
                .HasFillFactor(100);

            entity.HasIndex(e => new { e.IdLifecycleActionType, e.ActionTimestamp }, "IX_LifecycleAction_Type").IsDescending(false, true);

            entity.Property(e => e.Id).HasComment("Clé technique interne autoincrémentée.");
            entity.Property(e => e.ActionTimestamp)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Horodatage exact de l’action de cycle de vie. Différent de CreatedAt : représente le moment réel où l’événement a été produit.");
            entity.Property(e => e.Comments)
                .HasMaxLength(500)
                .HasComment("Commentaire libre associé à l’action du cycle de vie.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Horodatage de création de la ligne dans la base. Souvent identique à ActionTimestamp mais peut différer selon les sources.");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(30)
                .HasComment("Identifiant ou nom du poste client. Correspond généralement au nom réseau du terminal.");
            entity.Property(e => e.DeviceIp)
                .HasMaxLength(30)
                .HasComment("Adresse IP du poste client ayant généré l’action. Permet une traçabilité réseau en cas de diagnostic ou d’audit.");
            entity.Property(e => e.DeviceUser)
                .HasMaxLength(30)
                .HasComment("Nom utilisateur du poste client (session Windows ou nom OS).");
            entity.Property(e => e.IdApplication).HasComment("Identifiant de l’application ayant généré l’action (FK → AppList.Id).");
            entity.Property(e => e.IdLifecycleActionSource).HasComment("Identifiant de la source métier concernée par l’action (référentiel LifecycleActionSource).");
            entity.Property(e => e.IdLifecycleActionType).HasComment("Identifiant du type d’action du cycle de vie (référentiel LifecycleActionType).");
            entity.Property(e => e.IdSource).HasComment("Identifiant technique de l’entité métier concernée par l’action, selon la source déclarée.");
            entity.Property(e => e.IdUser).HasComment("Identifiant de l’utilisateur ayant déclenché l’action (FK → UserApp.Id).");
            entity.Property(e => e.IsDeleted).HasComment("Suppression logique (soft delete). 0 = actif, 1 = supprimé. Permet de conserver l’historique complet des actions.");
            entity.Property(e => e.UpdatedAt).HasComment("Horodatage de la dernière mise à jour. Resté NULL si la ligne n’a jamais été modifiée après création.");

            entity.HasOne(d => d.IdApplicationNavigation).WithMany(p => p.LifecycleActions)
                .HasForeignKey(d => d.IdApplication)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LifecycleAction_Application");

            entity.HasOne(d => d.IdLifecycleActionSourceNavigation).WithMany(p => p.LifecycleActions)
                .HasForeignKey(d => d.IdLifecycleActionSource)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LifecycleAction_ActionSource");

            entity.HasOne(d => d.IdLifecycleActionTypeNavigation).WithMany(p => p.LifecycleActions)
                .HasForeignKey(d => d.IdLifecycleActionType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LifecycleAction_ActionType");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.LifecycleActions)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LifecycleAction_User");
        });

        modelBuilder.Entity<LifecycleActionSource>(entity =>
        {
            entity.ToTable("LifecycleActionSource", tb => tb.HasComment("Référentiel des entités sources concernées par les actions du cycle de vie."));

            entity.HasIndex(e => e.Code, "UQ_LifecycleActionSource_Code").IsUnique();

            entity.HasIndex(e => e.SourceName, "UQ_LifecycleActionSource_SourceName").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant interne unique de la source d’action.");
            entity.Property(e => e.Code)
                .HasMaxLength(4)
                .HasComment("Code court identifiant la source métier de l’action.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de la source d’action.");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasComment("Description du périmètre fonctionnel de la source.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la source d’action est supprimée logiquement.");
            entity.Property(e => e.SourceName)
                .HasMaxLength(200)
                .HasComment("Nom logique de la source métier (table associée).");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de la source d’action.");
        });

        modelBuilder.Entity<LifecycleActionType>(entity =>
        {
            entity.ToTable("LifecycleActionType", tb => tb.HasComment("Référentiel des types d’action du cycle de vie (import, validation, découpe, expédition, etc.)."));

            entity.HasIndex(e => e.Code, "UQ_LifecycleActionType_Code").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant interne unique du type d’action.");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasComment("Code unique représentant le type d’action.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création du type d’action.");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasComment("Description du rôle et du contexte du type d’action.");
            entity.Property(e => e.Designation)
                .HasMaxLength(200)
                .HasComment("Désignation lisible du type d’action.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si le type d’action est supprimé logiquement.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour du type d’action.");
        });

        modelBuilder.Entity<ProductionChassi>(entity =>
        {
            entity.ToTable("ProductionChassis", tb => tb.HasComment("Châssis de production rattaché à une commande, issu d’agrégations (group by) depuis Tempor_Import."));

            entity.HasIndex(e => new { e.IdCustomerOrder, e.BarcodeId }, "IX_ProductionChassis_IdCustomerOrder_BarcodeId").HasFillFactor(100);

            entity.HasIndex(e => new { e.IdCustomerOrder, e.OrderPosition }, "IX_ProductionChassis_IdCustomerOrder_OrderPosition");

            entity.HasIndex(e => new { e.IdCustomerOrder, e.PartialSeriesIndex }, "IX_ProductionChassis_IdCustomerOrder_PartialSeriesIndex");

            entity.HasIndex(e => e.BarcodeId, "UQ_ProductionChassis_BarcodeId").IsUnique();

            entity.HasIndex(e => new { e.IdCustomerOrder, e.OrderPosition, e.PartialSeriesIndex, e.BarcodeId }, "UQ_ProductionChassis_CustomerOrder_Position_PartialSeries").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant technique du châssis (PK).");
            entity.Property(e => e.BarcodeId)
                .HasMaxLength(50)
                .HasComment("Identifiant code-barres chassis. Source : Tempor_Import.Feld_10_059.");
            entity.Property(e => e.CapacityZone)
                .HasMaxLength(100)
                .HasComment("Zone de capacité. Source : Tempor_Import.Feld_10_074.");
            entity.Property(e => e.ColorNameIntExt)
                .HasMaxLength(100)
                .HasComment("Couleur intérieur/extérieur. Source : Tempor_Import.Feld_10_011.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création (système).");
            entity.Property(e => e.CustomerPosition)
                .HasMaxLength(100)
                .HasComment("Position client. Source : Tempor_Import.Feld_10_041.");
            entity.Property(e => e.ElementHeight).HasComment("Hauteur élément. Source : Tempor_Import.Feld_10_032.");
            entity.Property(e => e.ElementWidth).HasComment("Largeur élément. Source : Tempor_Import.Feld_10_031.");
            entity.Property(e => e.FrameHeightIncludingRV).HasComment("Hauteur cadre incluant RV. Source : Tempor_Import.Feld_10_078.");
            entity.Property(e => e.FrameWidthIncludingRV).HasComment("Largeur cadre incluant RV. Source : Tempor_Import.Feld_10_077.");
            entity.Property(e => e.HeightWithCorrectionAndMiterTip).HasComment("Hauteur avec correction et coupe à la pointe. Source : Tempor_Import.Feld_10_246.");
            entity.Property(e => e.IdCustomerOrder).HasComment("FK CustomerOrder. Source : Tempor_Import.Aunummer.");
            entity.Property(e => e.IsDeleted).HasComment("Suppression logique (soft delete).");
            entity.Property(e => e.LookChassisId)
                .HasMaxLength(100)
                .HasComment("Identifiant Look3E pour le Chassis. Source : Tempor_Import.Feld_10_513.");
            entity.Property(e => e.OpeningTypeAbbreviation)
                .HasMaxLength(100)
                .HasComment("Abréviation type d’ouverture. Source : Tempor_Import.Feld_10_034.");
            entity.Property(e => e.OpeningTypeText)
                .HasMaxLength(200)
                .HasComment("Texte type d’ouverture. Source : Tempor_Import.Feld_10_013.");
            entity.Property(e => e.OrderPosition).HasComment("Position du châssis dans la commande. Source : Tempor_Import.Pos.");
            entity.Property(e => e.OuterHeightIncludingRV).HasComment("Hauteur extérieure incluant RV. Source : Tempor_Import.Feld_10_080.");
            entity.Property(e => e.OuterWidthIncludingRV).HasComment("Largeur extérieure incluant RV. Source : Tempor_Import.Feld_10_079.");
            entity.Property(e => e.PartialSeriesIndex).HasComment("Index de sous-série. Source : Tempor_Import.TeilserienIndex.");
            entity.Property(e => e.ProductFamily)
                .HasMaxLength(100)
                .HasComment("Famille produit. Source : Tempor_Import.Feld_10_048.");
            entity.Property(e => e.Quantity).HasComment("Quantité. Source : Tempor_Import.Wert_11.");
            entity.Property(e => e.SashDimensionsLeftRight)
                .HasMaxLength(100)
                .HasComment("Dimensions vantaux G/D. Source : Tempor_Import.Feld_10_012.");
            entity.Property(e => e.SashPreset)
                .HasMaxLength(100)
                .HasComment("Ventaux prédéfinis. Source : Tempor_Import.Feld_10_282.");
            entity.Property(e => e.SeriesPosition).HasComment("Position dans la série. Source : Tempor_Import.Feld_10_030.");
            entity.Property(e => e.SlidingType)
                .HasMaxLength(200)
                .HasComment("Type de coulissant. Source : Tempor_Import.Feld_10_233.");
            entity.Property(e => e.SlidingTypeDetailed)
                .HasMaxLength(200)
                .HasComment("Type de coulissant détaillé. Source : Tempor_Import.Feld_10_234.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour (système).");
            entity.Property(e => e.WidthWithCorrectionAndMiterTip)
                .HasComment("Largeur avec correction et coupe à la pointe. Source : Tempor_Import.Feld_10_245.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.WindowSystemCode)
                .HasMaxLength(100)
                .HasComment("Code système fenêtre. Source : Tempor_Import.Feld_10_019.");
            entity.Property(e => e.WindowText)
                .HasMaxLength(1000)
                .HasComment("Texte de fenêtre. Source : Tempor_Import.Feld_10_113.");

            entity.HasOne(d => d.IdCustomerOrderNavigation).WithMany(p => p.ProductionChassis)
                .HasForeignKey(d => d.IdCustomerOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionChassis_CustomerOrder");
        });

        modelBuilder.Entity<ProductionColorLabelType>(entity =>
        {
            entity.ToTable("ProductionColorLabelType", tb => tb.HasComment("Table ENUM des couleurs de production utilisées pour qualifier létat dune série en fonction du jour de fin de production."));

            entity.HasIndex(e => e.Label, "UQ_ProductionColorLabelType_Label").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant ENUM : 1=Bleu, 2=Orange, 3=Jaune, 4=Rouge, 5=Rose.");
            entity.Property(e => e.Label)
                .HasMaxLength(20)
                .HasComment("Libellé textuel de la couleur de production.");
        });

        modelBuilder.Entity<ProductionCutPiece>(entity =>
        {
            entity.ToTable("ProductionCutPiece", tb => tb.HasComment("Pièces à découper issues de Tempor_Import, rattachées à un composant de châssis (cadre ou ouvrant)."));

            entity.HasIndex(e => e.IdArticleInternal, "IX_ProductionCutPiece_IdArticleInternal").HasFillFactor(100);

            entity.HasIndex(e => e.IdProductionBar, "IX_ProductionCutPiece_IdProductionBar").HasFillFactor(100);

            entity.HasIndex(e => e.IsBarSupplied, "IX_ProductionCutPiece_IsBarSupplied").HasFillFactor(100);

            entity.HasIndex(e => e.IsComment, "IX_ProductionCutPiece_IsComment").HasFillFactor(100);

            entity.HasIndex(e => e.IsCut, "IX_ProductionCutPiece_IsCut").HasFillFactor(100);

            entity.HasIndex(e => e.IsOptimized, "IX_ProductionCutPiece_IsOptimized").HasFillFactor(100);

            entity.HasIndex(e => e.IdSpatialPosition, "IX_ProductionCutPiece_SpatialPositionInChassis").HasFillFactor(100);

            entity.HasIndex(e => e.CutBarcode, "UQ_ProductionCutPiece_CutBarcode").IsUnique();

            entity.HasIndex(e => e.LookCutPieceId, "UQ_ProductionCutPiece_LookCutPieceId").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant technique de la pièce à découper (PK).");
            entity.Property(e => e.ArticleCounter).HasComment("Compteur article Leitxx. Source : Tempor_Import.Feld_10_302.");
            entity.Property(e => e.AssemblyCode)
                .HasMaxLength(100)
                .HasComment("Code de montage. Source : Tempor_Import.Feld_10_009.");
            entity.Property(e => e.AssociatedArticleReferenceLeft)
                .HasMaxLength(100)
                .HasComment("Référence article associée à gauche. Source : Tempor_Import.Feld_10_347.");
            entity.Property(e => e.AssociatedArticleReferenceRight)
                .HasMaxLength(100)
                .HasComment("Référence article associée à droite. Source : Tempor_Import.Feld_10_346.");
            entity.Property(e => e.BarColorCodeInOut)
                .HasMaxLength(100)
                .HasComment("Code couleur barre intérieur/extérieur. Source : Tempor_Import.Feld_8.");
            entity.Property(e => e.BarFamilyCode).HasComment("Code famille de barre. Source : Tempor_Import.Wert_2.");
            entity.Property(e => e.BarHeight)
                .HasComment("Hauteur de la barre. Source : Tempor_Import.Wert_41.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.BarLength).HasComment("Longueur brute de la barre. Source : Tempor_Import.Wert_21.");
            entity.Property(e => e.BarProductCodeToPrint)
                .HasMaxLength(100)
                .HasComment("Code produit à imprimer sur la barre. Source : Tempor_Import.CodeNat.");
            entity.Property(e => e.BarProductFamilyName)
                .HasMaxLength(100)
                .HasComment("Désignation famille de barre. Source : Tempor_Import.Feld_40.");
            entity.Property(e => e.BarReference)
                .HasMaxLength(100)
                .HasComment("Référence de la barre. Source : Tempor_Import.Feld_9.");
            entity.Property(e => e.BarWidth)
                .HasComment("Largeur de la barre. Source : Tempor_Import.Wert_42.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ChassisPieceNumber).HasComment("Numéro de pièce dans le châssis. Source : Tempor_Import.Ref.");
            entity.Property(e => e.ComponentPieceNumber).HasComment("Numéro de pièce dans le composant. Source : Tempor_Import.Feld_10_114.");
            entity.Property(e => e.ConnectionProfileCode)
                .HasMaxLength(100)
                .HasComment("Code profil de liaison. Source : Tempor_Import.Feld_10_022.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.CustomerOrderLineNumber).HasComment("Numéro de ligne de commande client. Source : Tempor_Import.Feld_10_227.");
            entity.Property(e => e.CustomerOrderLineNumber2).HasComment("Numéro de ligne commande client (variante). Source : Tempor_Import.Feld_10_239.");
            entity.Property(e => e.CutBarcode)
                .HasMaxLength(100)
                .HasComment("Code-barre de la pièce. Source : Tempor_Import.Feld_10_165.");
            entity.Property(e => e.CutDimension)
                .HasComment("Dimension de coupe. Source : Tempor_Import.Wert_6")
                .HasColumnType("decimal(5, 1)");
            entity.Property(e => e.CutFinishedAt).HasComment("Date et heure de validation de la découpe.");
            entity.Property(e => e.CutInclinationLeft).HasComment("Inclinaison de coupe gauche. Source : Tempor_Import.Feld_10_104.");
            entity.Property(e => e.CutInclinationRight).HasComment("Inclinaison de coupe droite. Source : Tempor_Import.Feld_10_105.");
            entity.Property(e => e.CutPivotLeft).HasComment("Pivot de coupe gauche. Source : Tempor_Import.Feld_10_111.");
            entity.Property(e => e.CutPivotRight).HasComment("Pivot de coupe droite. Source : Tempor_Import.Feld_10_112.");
            entity.Property(e => e.CutStartedAt).HasComment("Date et heure de début de la découpe.");
            entity.Property(e => e.DaylightLengthWithAngleAndCorrection).HasComment("Longueur jour avec angle et correction. Source : Tempor_Import.Feld_10_267.");
            entity.Property(e => e.DiePitch).HasComment("Pas de filière. Source : Tempor_Import.Wert_37.");
            entity.Property(e => e.DrainageCodeUsedForCalculation)
                .HasMaxLength(100)
                .HasComment("Code d’évacuation pour calcul. Source : Tempor_Import.Feld_10_181.");
            entity.Property(e => e.ElementCounter).HasComment("Compteur d’élément. Source : Tempor_Import.Feld_10_057.");
            entity.Property(e => e.FinishingCutLength).HasComment("Longueur de coupe finition. Source : Tempor_Import.Wert_33.");
            entity.Property(e => e.FrameFieldNumber).HasComment("Numéro de champ du cadre. Source : Tempor_Import.Feld_10_058.");
            entity.Property(e => e.IdArticleInternal).HasComment("Identifiant de l’article interne associé à la pièce à découper.");
            entity.Property(e => e.IdProductionBar).HasComment("Identifiant de la barre de production associée à la découpe.");
            entity.Property(e => e.IdProductionFrameSash).HasComment("Identifiant du composant châssis (cadre ou ouvrant) parent.");
            entity.Property(e => e.IdSpatialPosition).HasComment("Identifiant de la position spatiale dans le châssis. Source : Tempor_Import.Feld_6.");
            entity.Property(e => e.IsBarSupplied).HasComment("Indique si la barre nécessaire à la découpe a été approvisionnée.");
            entity.Property(e => e.IsComment).HasComment("Indique si un commentaire actif est associé à la découpe.");
            entity.Property(e => e.IsCut).HasComment("Indique si la découpe a été réalisée.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est supprimé logiquement.");
            entity.Property(e => e.IsOptimized).HasComment("Indique si la découpe a été sélectionnée par le processus d’optimisation.");
            entity.Property(e => e.LookChassisId)
                .HasMaxLength(100)
                .HasComment("Identifiant Look3E pour le Chassis. Source : Tempor_Import.Feld_10_513.");
            entity.Property(e => e.LookCustomerOrderId)
                .HasMaxLength(100)
                .HasComment("Identifiant Look3E pour la Commande Client. Source. Source : Tempor_Import.Feld_10_205.");
            entity.Property(e => e.LookCutPieceId)
                .HasMaxLength(100)
                .HasComment("Identifiant Look3E pour la pièce à découper. Source : Tempor_Import.Feld_23.");
            entity.Property(e => e.MachineCode)
                .HasMaxLength(100)
                .HasComment("Code machine de découpe. Source : Tempor_Import.Feld_10_021.");
            entity.Property(e => e.OptimizationMinLength).HasComment("Longueur minimale pour optimisation. Source : Tempor_Import.Wert_38.");
            entity.Property(e => e.PartialSeriesSequentialPieceNumber).HasComment("Numéro séquentiel dans la série partielle. Source : Tempor_Import.Feld_10_277.");
            entity.Property(e => e.PositionPieceNumber).HasComment("Numéro de pièce dans la position. Source : Tempor_Import.Feld_10_006.");
            entity.Property(e => e.ProfileCodeToPrint)
                .HasMaxLength(100)
                .HasComment("Code profil à imprimer. Source : Tempor_Import.Feld_10_027.");
            entity.Property(e => e.ProfileColorCodeInOut)
                .HasMaxLength(100)
                .HasComment("Code couleur profil intérieur/extérieur. Source : Tempor_Import.Feld_10_026.");
            entity.Property(e => e.ProfileColorInside)
                .HasMaxLength(100)
                .HasComment("Couleur intérieure du profil. Source : Tempor_Import.Feld_10_088.");
            entity.Property(e => e.ProfileColorOutside)
                .HasMaxLength(100)
                .HasComment("Couleur extérieure du profil. Source : Tempor_Import.Feld_10_089.");
            entity.Property(e => e.ProfileHeight)
                .HasComment("Hauteur du profil. Source : Tempor_Import.Feld_10_075.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ProfileLength)
                .HasComment("Longueur du profil. Source : Tempor_Import.Feld_10_010.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ProfileLengthIncludingFOD)
                .HasComment("Longueur du profil avec FOD. Source : Tempor_Import.Feld_10_565.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ProfileName)
                .HasMaxLength(100)
                .HasComment("Nom du profil. Source : Tempor_Import.Feld_10_100.");
            entity.Property(e => e.ProfileNumber)
                .HasMaxLength(100)
                .HasComment("Numéro de profil. Source : Tempor_Import.Feld_10_066.");
            entity.Property(e => e.ProfileNumberForMachine)
                .HasMaxLength(100)
                .HasComment("Numéro de profil machine. Source : Tempor_Import.Feld_10_330.");
            entity.Property(e => e.ProfileWidth)
                .HasComment("Largeur du profil. Source : Tempor_Import.Feld_10_051.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.RemainingUntilLength).HasComment("Longueur restante avant seuil. Source : Tempor_Import.Wert_36.");
            entity.Property(e => e.SawCutLength).HasComment("Longueur de coupe scie. Source : Tempor_Import.Wert_34.");
            entity.Property(e => e.ScrapMaxLength).HasComment("Longueur maximale de chute autorisée. Source : Tempor_Import.Wert_39.");
            entity.Property(e => e.SequentialPieceNumber).HasComment("Numéro séquentiel de pièce. Source : Tempor_Import.Feld_10_036.");
            entity.Property(e => e.SideIndex).HasComment("Indice côté (0 bas,1 gauche,2 haut,3 droite). Source : Tempor_Import.Feld_10_020.");
            entity.Property(e => e.TotalElementsCount).HasComment("Nombre total d’éléments. Source : Tempor_Import.Feld_10_144.");
            entity.Property(e => e.TotalQuantityForPosition).HasComment("Quantité totale pour la position. Source : Tempor_Import.Feld_10_133.");
            entity.Property(e => e.TrolleyLevel).HasComment("Niveau du chariot. Source : Tempor_Import.Etage.");
            entity.Property(e => e.TrolleyNumber).HasComment("Numéro de chariot. Source : Tempor_Import.Wagen.");
            entity.Property(e => e.TrolleySlot).HasComment("Emplacement du chariot. Source : Tempor_Import.Fach.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de l’enregistrement.");
            entity.Property(e => e.WindowCounter).HasComment("Compteur de fenêtre. Source : Tempor_Import.Feld_10_052.");

            entity.HasOne(d => d.IdArticleInternalNavigation).WithMany(p => p.ProductionCutPieces)
                .HasForeignKey(d => d.IdArticleInternal)
                .HasConstraintName("FK_ProductionCutPiece_ArticleInternal");

            entity.HasOne(d => d.IdProductionFrameSashNavigation).WithMany(p => p.ProductionCutPieces)
                .HasForeignKey(d => d.IdProductionFrameSash)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionCutPiece_ProductionFrameSash");

            entity.HasOne(d => d.IdSpatialPositionNavigation).WithMany(p => p.ProductionCutPieces)
                .HasForeignKey(d => d.IdSpatialPosition)
                .HasConstraintName("FK_ProductionCutPiece_SpatialPosition");
        });

        modelBuilder.Entity<ProductionFrameSash>(entity =>
        {
            entity.ToTable("ProductionFrameSash", tb => tb.HasComment("Cadres et ouvrants de production rattachés à un châssis, issus de Tempor_Import (données Leitxx)."));

            entity.HasIndex(e => e.IdProductionChassis, "IX_ProductionFrameSash_IdProductionChassis").HasFillFactor(100);

            entity.HasIndex(e => new { e.IdProductionChassis, e.ComponentNumber }, "UQ_ProductionFrameSash_Chassis_Component").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant technique du cadre/ouvrant (PK).");
            entity.Property(e => e.AdjacentFramePartToSash)
                .HasMaxLength(100)
                .HasComment("Partie de cadre adjacente à l’ouvrant. Source : Tempor_Import.Feld_10_224.");
            entity.Property(e => e.BeadSystemInnerSeal)
                .HasMaxLength(100)
                .HasComment("Joint intérieur issu du système de parcloses. Source : Tempor_Import.Feld_10_294.");
            entity.Property(e => e.BeadsHeight)
                .HasComment("Hauteur des parcloses. Source : Tempor_Import.Feld_10_056.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.BeadsWidth)
                .HasComment("Largeur des parcloses. Source : Tempor_Import.Feld_10_055.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ComponentNumber).HasComment("Numéro du composant dans le chassis. Source : Tempor_Import.Wert_14.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.CremoneType1)
                .HasMaxLength(100)
                .HasComment("Ferrage type crémone 1. Source : Tempor_Import.Feld_10_262.");
            entity.Property(e => e.DisplayColorInside)
                .HasMaxLength(100)
                .HasComment("Couleur d’affichage intérieure. Source : Tempor_Import.Feld_10_585.");
            entity.Property(e => e.DisplayColorOutside)
                .HasMaxLength(100)
                .HasComment("Couleur d’affichage extérieure. Source : Tempor_Import.Feld_10_586.");
            entity.Property(e => e.FrameSashHeight)
                .HasComment("Hauteur cadre/ouvrant. Source : Tempor_Import.Feld_10_039.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameSashHeightTenths)
                .HasComment("Hauteur cadre/ouvrant en dixièmes. Source : Tempor_Import.Feld_10_230.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameSashWidth)
                .HasComment("Largeur cadre/ouvrant. Source : Tempor_Import.Feld_10_038.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameSashWidthTenths)
                .HasComment("Largeur cadre/ouvrant en dixièmes. Source : Tempor_Import.Feld_10_229.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameThresholdCounterProfile)
                .HasMaxLength(100)
                .HasComment("Contre-profil cadre/seuil. Source : Tempor_Import.Feld_10_221.");
            entity.Property(e => e.GlazingAssignment)
                .HasMaxLength(1000)
                .HasComment("Affectation du vitrage. Source : Tempor_Import.Feld_10_148.");
            entity.Property(e => e.GlazingBeadsPerSashFrame)
                .HasMaxLength(100)
                .HasComment("Parcloses par ouvrant/cadre. Source : Tempor_Import.Feld_10_137.");
            entity.Property(e => e.GlazingCode)
                .HasMaxLength(100)
                .HasComment("Code vitrage. Source : Tempor_Import.Feld_10_560.");
            entity.Property(e => e.GlazingDimensions)
                .HasMaxLength(100)
                .HasComment("Dimensions du vitrage. Source : Tempor_Import.Feld_10_134.");
            entity.Property(e => e.GlazingSealText)
                .HasMaxLength(100)
                .HasComment("Texte pour le joint de vitrage. Source : Tempor_Import.Feld_10_017.");
            entity.Property(e => e.GlazingText)
                .HasMaxLength(100)
                .HasComment("Texte vitrage. Source : Tempor_Import.Feld_10_018.");
            entity.Property(e => e.HandlePosition)
                .HasMaxLength(100)
                .HasComment("Position de la poignée. Source : Tempor_Import.Feld_10_161.");
            entity.Property(e => e.HardwareRabbetHeightTenths)
                .HasComment("Hauteur feuillure de ferrure en dixièmes. Source : Tempor_Import.Feld_10_232.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.HardwareRabbetWidthTenths)
                .HasComment("Largeur feuillure de ferrure en dixièmes. Source : Tempor_Import.Feld_10_231.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.HardwareSystemCode)
                .HasMaxLength(100)
                .HasComment("Code du système de ferrures. Source : Tempor_Import.Feld_10_023.");
            entity.Property(e => e.HardwareSystemText)
                .HasMaxLength(100)
                .HasComment("Texte pour le système de ferrures. Source : Tempor_Import.Feld_10_014.");
            entity.Property(e => e.IdProductionChassis).HasComment("Identifiant technique du châssis parent (FK ProductionChassis).");
            entity.Property(e => e.InnerSealSashFrame)
                .HasMaxLength(100)
                .HasComment("Joint ouvrant/cadre intérieur. Source : Tempor_Import.Feld_10_150.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est supprimé logiquement.");
            entity.Property(e => e.MechanismCode)
                .HasMaxLength(100)
                .HasComment("Code de mécanisme (boîtier/entraînement). Source : Tempor_Import.Feld_10_215.");
            entity.Property(e => e.OpeningDirectionIndicator)
                .HasMaxLength(100)
                .HasComment("Indicateur de sens d’ouverture. Source : Tempor_Import.Feld_10_045.");
            entity.Property(e => e.OpeningTypeText)
                .HasMaxLength(1000)
                .HasComment("Type d’ouverture (texte). Source : Tempor_Import.Feld_10_043.");
            entity.Property(e => e.PositionDataSealColor)
                .HasMaxLength(100)
                .HasComment("Couleur du joint issue des données de position. Source : Tempor_Import.Feld_10_489.");
            entity.Property(e => e.ReinforcementCode)
                .HasMaxLength(100)
                .HasComment("Code de renfort. Source : Tempor_Import.Feld_10_015.");
            entity.Property(e => e.ReinforcementLength).HasComment("Longueur du renfort. Source : Tempor_Import.Feld_10_016.");
            entity.Property(e => e.ReinforcementLength1NoGrid).HasComment("Longueur de renfort 1 sans trame. Source : Tempor_Import.Feld_10_583.");
            entity.Property(e => e.ReinforcementLength2NoGrid).HasComment("Longueur de renfort 2 sans trame. Source : Tempor_Import.Feld_10_584.");
            entity.Property(e => e.SashHardwareIndicator)
                .HasMaxLength(100)
                .HasComment("Indicateur de ferrure d’ouvrant sinon global. Source : Tempor_Import.Feld_10_187.");
            entity.Property(e => e.Seal)
                .HasMaxLength(100)
                .HasComment("Joint. Source : Tempor_Import.Feld_10_061.");
            entity.Property(e => e.SealColor)
                .HasMaxLength(100)
                .HasComment("Couleur du joint. Source : Tempor_Import.Feld_10_062.");
            entity.Property(e => e.SealSystem)
                .HasMaxLength(100)
                .HasComment("Système de joint. Source : Tempor_Import.Feld_10_067.");
            entity.Property(e => e.SealVariantCode)
                .HasMaxLength(100)
                .HasComment("Code de variante de joint. Source : Tempor_Import.Feld_10_563.");
            entity.Property(e => e.SealVariantText)
                .HasMaxLength(100)
                .HasComment("Texte de variante de joint. Source : Tempor_Import.Feld_10_564.");
            entity.Property(e => e.SpecialOpeningTypeCode)
                .HasMaxLength(100)
                .HasComment("Code type d’ouverture spécifique. Source : Tempor_Import.Feld_10_125.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de l’enregistrement.");

            entity.HasOne(d => d.IdProductionChassisNavigation).WithMany(p => p.ProductionFrameSashes)
                .HasForeignKey(d => d.IdProductionChassis)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionFrameSash_ProductionChassis");
        });

        modelBuilder.Entity<ProductionSeries>(entity =>
        {
            entity.ToTable(tb => tb.HasComment("Table contenant les informations des séries de production issues d’Axapta."));

            entity.HasIndex(e => e.IdSerialNumber, "UQ_ProductionSeries_IdSerial").IsUnique();

            entity.Property(e => e.Id).HasComment("Clé technique interne (IDENTITY). N’existe pas dans AX.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de la ligne dans le système local. N’existe pas dans AX.");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasComment("Description de la série. Correspond au champ AX: EEEA_SERIALDESCRIPTION.");
            entity.Property(e => e.IdRec).HasComment("Identifiant unique AX (RECID). Permet d’assurer le lien avec la ligne AX originale.");
            entity.Property(e => e.IdSerialNumber).HasComment("Numéro de série AX. Correspond au champ AX: SERIALNOSTR.");
            entity.Property(e => e.IsCuttingCompleted).HasComment("Indique si l’ensemble des découpes de la série ont été réalisées. False = non découpée, True = découpée.");
            entity.Property(e => e.IsCuttingStarted).HasComment("Indique si une des découpes de la série a été réalisée. False = non commencée, True = commencée.");
            entity.Property(e => e.IsDeleted).HasComment("Indicateur de suppression logique (soft delete). N’existe pas dans AX.");
            entity.Property(e => e.IsDropBarSupplied).HasComment("Indique si la série a reçu l’approvisionnement en barres de chutes (stock de chutes). False = non approvisionnée, True = approvisionnée.");
            entity.Property(e => e.IsImported).HasComment("Indique si les données métier associées à la série ont été importées depuis un fichier Leitxx.mdb. False = non importée, True = importée.");
            entity.Property(e => e.IsNewBarSupplied).HasComment("Indique si la série a reçu l’approvisionnement en barres neuves. False = non approvisionnée, True = approvisionnée.");
            entity.Property(e => e.IsProductionValidated).HasComment("Indique si la série a été validée pour lancement. False = à valider, True = validée.");
            entity.Property(e => e.ProductionEndDate).HasComment("Date de fin de production. Correspond au champ AX: ATWIN_PRODUCTIONENDDATE.");
            entity.Property(e => e.ProductionEndDay).HasComment("Code couleur de l’étiquette, calculé depuis le jour de fin de production. Référence ProductionColorLabelType. 0 = Violet si date absente.");
            entity.Property(e => e.ProductionStartDate).HasComment("Date de début de la production. Correspond au champ AX: EEEA_SERIALPLANDATE.");
            entity.Property(e => e.RecVersion).HasComment("Version du record dans AX. Correspond au champ AX: RECVERSION (utilisé pour le contrôle de concurrence dans AX).");
            entity.Property(e => e.SerieCreatedAt).HasComment("Date de création initiale de la série. Correspond au champ AX: CREATEDDATETIME.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière modification dans le système local. N’existe pas dans AX.");

            entity.HasOne(d => d.ProductionEndDayNavigation).WithMany(p => p.ProductionSeries)
                .HasForeignKey(d => d.ProductionEndDay)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionSeries_ColorLabelType");
        });

        modelBuilder.Entity<SidePosition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SidePosi__3214EC07A3E9EB45");

            entity.ToTable("SidePosition", tb => tb.HasComment("Referentiel des positions laterales normalisees associant SideIndex et SpacePositionIndex."));

            entity.HasIndex(e => new { e.SideIndex, e.SpacePositionIndex }, "UQ_SidePosition").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant technique unique de la position laterale normalisee.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de creation de l'enregistrement.");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasComment("Designation lisible de la position laterale pour affichage et reporting.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la position spatiale est supprimee logiquement.");
            entity.Property(e => e.SideIndex).HasComment("Indice lateral technique utilise pour le parcours des pieces dans le chassis.");
            entity.Property(e => e.SpacePositionIndex).HasComment("Indice spatial global utilise pour l’ordonnancement circulaire des positions.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de derniere mise a jour de l'enregistrement.");
        });

        modelBuilder.Entity<SpatialPosition>(entity =>
        {
            entity.ToTable("SpatialPosition", tb => tb.HasComment("Referentiel de correspondance des positions spatiales issues de Tempor_Import.Feld_6."));

            entity.HasIndex(e => e.Position, "IX_SpatialPosition_Position").HasFillFactor(100);

            entity.HasIndex(e => e.Code, "UQ_SpatialPosition_Code").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant technique unique de la position spatiale.");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasComment("Code source issu de Tempor_Import.Feld_6.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de creation de l'enregistrement.");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasComment("Description fonctionnelle lisible de la position ou du type de piece.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la position spatiale est supprimee logiquement.");
            entity.Property(e => e.Position)
                .HasMaxLength(20)
                .HasComment("Position spatiale normalisee (Haut, Bas, Gauche, Droite, Horizontal, Vertical, Croisillon, NA).");
            entity.Property(e => e.UpdatedAt).HasComment("Date de derniere mise a jour de l'enregistrement.");
        });

        modelBuilder.Entity<StockBin>(entity =>
        {
            entity.ToTable("StockBin", tb => tb.HasComment("Contenant physique (bac, boîte, plateau…) utilisé dans les zones de stockage."));

            entity.HasIndex(e => e.Designation, "UQ_StockBin_Designation").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique du bac (clé primaire).");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.CurrentItems).HasComment("Nombre actuel de contenants présents dans le bac.");
            entity.Property(e => e.Designation)
                .HasMaxLength(50)
                .HasComment("Désignation unique du bac (ex. : C45, A12).");
            entity.Property(e => e.IdStockBinType).HasComment("Type de bac (boîte, plateau, panier, cassette…).");
            entity.Property(e => e.IdStockSupportType).HasComment("Type de support présent dans l’adresse (rack, sol, structure spéciale…).");
            entity.Property(e => e.IdStockZoneAddress).HasComment("Identifiant de l’adresse de zone dans laquelle se trouve le bac.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est supprimé logiquement (1 = supprimé).");
            entity.Property(e => e.IsMovable).HasComment("Indique si le bac est déplaçable (TRUE = mobile).");
            entity.Property(e => e.MaxItems).HasComment("Capacité maximale théorique du bac (en nombre d’unités).");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de l’enregistrement.");

            entity.HasOne(d => d.IdStockBinTypeNavigation).WithMany(p => p.StockBins)
                .HasForeignKey(d => d.IdStockBinType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockBin_StockBinType");

            entity.HasOne(d => d.IdStockSupportTypeNavigation).WithMany(p => p.StockBins)
                .HasForeignKey(d => d.IdStockSupportType)
                .HasConstraintName("FK_StockBin_StockSupportType");

            entity.HasOne(d => d.IdStockZoneAddressNavigation).WithMany(p => p.StockBins)
                .HasForeignKey(d => d.IdStockZoneAddress)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockBin_StockZoneAddress");
        });

        modelBuilder.Entity<StockBinItem>(entity =>
        {
            entity.ToTable("StockBinItem", tb => tb.HasComment("Représente les quantités d’articles neufs stockées dans un emplacement physique (StockBin)."));

            entity.HasIndex(e => e.IdArticleInternal, "IX_StockBinItem_Article");

            entity.HasIndex(e => e.IdStockBin, "IX_StockBinItem_StockBin");

            entity.HasIndex(e => new { e.IdArticleInternal, e.IdStockBin }, "UQ_StockBinItem").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique du StockBinItem.");
            entity.Property(e => e.AccessibleDate)
                .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .HasComment("Date à partir de laquelle la quantité est disponible pour l’utilisation ou la préparation.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure de création de l’enregistrement.");
            entity.Property(e => e.IdArticleInternal).HasComment("Référence vers l’article interne stocké (clé étrangère vers ArticleInternal).");
            entity.Property(e => e.IdStockBin).HasComment("Référence vers le bac de stockage physique (clé étrangère vers StockBin).");
            entity.Property(e => e.InventoryDate)
                .HasDefaultValue(new DateOnly(1970, 1, 1))
                .HasComment("Date du dernier inventaire effectué pour ce bac et cet article.");
            entity.Property(e => e.IsAccessible)
                .HasDefaultValue(true)
                .HasComment("Indique si le stock est actuellement accessible (1 = oui, 0 = non).");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est logiquement supprimé (soft delete).");
            entity.Property(e => e.Quantity).HasComment("Quantité disponible dans le bac pour cet article interne.");
            entity.Property(e => e.UpdatedAt).HasComment("Date et heure de la dernière mise à jour de l’enregistrement.");

            entity.HasOne(d => d.IdArticleInternalNavigation).WithMany(p => p.StockBinItems)
                .HasForeignKey(d => d.IdArticleInternal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockBinItem_ArticleInternal");

            entity.HasOne(d => d.IdStockBinNavigation).WithMany(p => p.StockBinItems)
                .HasForeignKey(d => d.IdStockBin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockBinItem_StockBin");
        });

        modelBuilder.Entity<StockBinType>(entity =>
        {
            entity.ToTable("StockBinType", tb => tb.HasComment("Table définissant les types de bacs (bins) utilisés pour stocker ou contenir des articles."));

            entity.HasIndex(e => e.Designation, "UQ_StockBinType_Designation").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique du type de bac.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasComment("Description optionnelle du type de bac.");
            entity.Property(e => e.Designation)
                .HasMaxLength(50)
                .HasComment("Désignation du type de bac (ex: Bac plastique, Tiroir, Panier filaire).");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est supprimé logiquement.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière modification de l’enregistrement.");
        });

        modelBuilder.Entity<StockChariot>(entity =>
        {
            entity.ToTable("StockChariot", tb => tb.HasComment("Représente les chariots physiques utilisés pour la préparation et le déplacement des articles en atelier."));

            entity.HasIndex(e => e.Designation, "UQ_StockChariot_Designation").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique du chariot.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.Designation)
                .HasMaxLength(50)
                .HasComment("Désignation lisible du chariot (ex : Chariot 1).");
            entity.Property(e => e.IsDeleted).HasComment("Indique si le chariot est supprimé logiquement.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de l’enregistrement.");
        });

        modelBuilder.Entity<StockSupportType>(entity =>
        {
            entity.ToTable("StockSupportType", tb => tb.HasComment("Type de support physique permettant de stocker des articles, des conteneurs ou tout type de matériel dans l’entrepôt."));

            entity.HasIndex(e => e.Designation, "UQ_StockSupportType_Designation").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .HasComment("Désignation du type de support (rack, étagère, casier, palette, box au sol, etc.).");
        });

        modelBuilder.Entity<StockZone>(entity =>
        {
            entity.ToTable("StockZone", tb => tb.HasComment("Définit les zones de stockage physiques de l’atelier aluminium."));

            entity.HasIndex(e => e.Designation, "UQ_StockZone_Designation").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique de la zone de stockage.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date et heure de création de la zone dans le système.");
            entity.Property(e => e.Designation)
                .HasMaxLength(50)
                .HasComment("Désignation lisible de la zone (ex : ZONE A, TAMPON, etc.).");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la zone est supprimée logiquement.");
            entity.Property(e => e.Priority).HasComment("Priorité de picking associée à la zone. Les zones à priorité faible sont traitées en premier.");
            entity.Property(e => e.UpdatedAt).HasComment("Date et heure de dernière modification.");
        });

        modelBuilder.Entity<StockZoneAddress>(entity =>
        {
            entity.ToTable("StockZoneAddress", tb => tb.HasComment("Liste des adresses internes appartenant à une zone de stockage."));

            entity.HasIndex(e => e.Designation, "UQ_StockZoneAddress_Designation").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique de l’adresse interne de zone.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date et heure de création de l’enregistrement.");
            entity.Property(e => e.Designation)
                .HasMaxLength(50)
                .HasComment("Désignation humaine et lisible de l’adresse dans la zone (ex : Z1-A01, Z2-C05).");
            entity.Property(e => e.IdStockZone).HasComment("Référence à la zone de stockage à laquelle appartient cette adresse interne.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’adresse est supprimée logiquement.");
            entity.Property(e => e.Priority).HasComment("Ordre de priorité pour le picking ou les opérations logistiques à l’intérieur de la zone.");
            entity.Property(e => e.UpdatedAt).HasComment("Dernière date de mise à jour de l’enregistrement.");

            entity.HasOne(d => d.IdStockZoneNavigation).WithMany(p => p.StockZoneAddresses)
                .HasForeignKey(d => d.IdStockZone)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockZoneAddress_StockZone");
        });

        modelBuilder.Entity<Tempor_Import>(entity =>
        {
            entity.ToTable("Tempor_Import", tb => tb.HasComment("Table intermédiaire d’import des données issues de la base Leit.mdb avant répartition vers les tables métiers"));

            entity.Property(e => e.Aualpha).HasMaxLength(100);
            entity.Property(e => e.CodeNat).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Feld_1).HasMaxLength(100);
            entity.Property(e => e.Feld_10_001).HasComment("Numéro Commande Tryba");
            entity.Property(e => e.Feld_10_002)
                .HasMaxLength(100)
                .HasComment("Désignation Projet");
            entity.Property(e => e.Feld_10_003).HasComment("Numéro Série");
            entity.Property(e => e.Feld_10_004).HasComment("Position");
            entity.Property(e => e.Feld_10_005).HasComment("Numéro de chariot");
            entity.Property(e => e.Feld_10_006).HasComment("Numéro de pièce dans position");
            entity.Property(e => e.Feld_10_007)
                .HasMaxLength(100)
                .HasComment("Information complémentaire");
            entity.Property(e => e.Feld_10_008)
                .HasMaxLength(100)
                .HasComment("Texte pour montage / drainage");
            entity.Property(e => e.Feld_10_009)
                .HasMaxLength(100)
                .HasComment("Code de montage");
            entity.Property(e => e.Feld_10_010)
                .HasComment("Longueur du profil")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_011)
                .HasMaxLength(100)
                .HasComment("Désignation Couleur Int/Ext");
            entity.Property(e => e.Feld_10_012)
                .HasMaxLength(100)
                .HasComment("Dimension Vantaux G/D");
            entity.Property(e => e.Feld_10_013)
                .HasMaxLength(200)
                .HasComment("Texte pour le type d’ouverture");
            entity.Property(e => e.Feld_10_014)
                .HasMaxLength(100)
                .HasComment("Texte pour le système de ferrures");
            entity.Property(e => e.Feld_10_015)
                .HasMaxLength(100)
                .HasComment("Code de renfort");
            entity.Property(e => e.Feld_10_016).HasComment("Longueur du renfort");
            entity.Property(e => e.Feld_10_017)
                .HasMaxLength(100)
                .HasComment("Texte pour le joint de vitrage");
            entity.Property(e => e.Feld_10_018)
                .HasMaxLength(100)
                .HasComment("Texte pour le vitrage");
            entity.Property(e => e.Feld_10_019)
                .HasMaxLength(100)
                .HasComment("Code du système de fenêtre");
            entity.Property(e => e.Feld_10_020).HasComment("Position / emplacement 0=Bas,1=Gauche,2=Haut,3=Droite");
            entity.Property(e => e.Feld_10_021)
                .HasMaxLength(100)
                .HasComment("Code machine");
            entity.Property(e => e.Feld_10_022)
                .HasMaxLength(100)
                .HasComment("Code du profil de raccord");
            entity.Property(e => e.Feld_10_023)
                .HasMaxLength(100)
                .HasComment("Code du système de ferrures");
            entity.Property(e => e.Feld_10_024)
                .HasMaxLength(100)
                .HasComment("Point de vente (secondaire)");
            entity.Property(e => e.Feld_10_026)
                .HasMaxLength(100)
                .HasComment("Code couleur");
            entity.Property(e => e.Feld_10_027)
                .HasMaxLength(100)
                .HasComment("Code du profil");
            entity.Property(e => e.Feld_10_028).HasComment("Numéro de chariot");
            entity.Property(e => e.Feld_10_029).HasComment("Numéro d’étage");
            entity.Property(e => e.Feld_10_030).HasComment("Position du chassis dans la série");
            entity.Property(e => e.Feld_10_031).HasComment("Largeur de l’élément");
            entity.Property(e => e.Feld_10_032).HasComment("Hauteur de l’élément");
            entity.Property(e => e.Feld_10_033)
                .HasMaxLength(100)
                .HasComment("Partie alphanumérique");
            entity.Property(e => e.Feld_10_034)
                .HasMaxLength(100)
                .HasComment("Abréviation du type d’ouverture");
            entity.Property(e => e.Feld_10_035)
                .HasMaxLength(100)
                .HasComment("Texte pour l’ornement");
            entity.Property(e => e.Feld_10_036).HasComment("Numéro de pièce séquentiel");
            entity.Property(e => e.Feld_10_037)
                .HasComment("Longueur du profil")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_038)
                .HasComment("Largeur cadre / ouvrant")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_039)
                .HasComment("Hauteur cadre / ouvrant")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_040).HasComment("Numéro d’ouvrant");
            entity.Property(e => e.Feld_10_041)
                .HasMaxLength(100)
                .HasComment("Position client");
            entity.Property(e => e.Feld_10_042)
                .HasMaxLength(100)
                .HasComment("Reste / chutes");
            entity.Property(e => e.Feld_10_043)
                .HasMaxLength(1000)
                .HasComment("Texte pour le type d’ouverture");
            entity.Property(e => e.Feld_10_044)
                .HasMaxLength(100)
                .HasComment("Code-barres déterminé à partir de {Barcode*}");
            entity.Property(e => e.Feld_10_045).HasComment("Indicateur du sens d’ouverture");
            entity.Property(e => e.Feld_10_046)
                .HasMaxLength(100)
                .HasComment("Code de la traverse de protection (jet d’eau)");
            entity.Property(e => e.Feld_10_047).HasComment("Longueur de la traverse de protection");
            entity.Property(e => e.Feld_10_048)
                .HasMaxLength(100)
                .HasComment("Chassis Famille produit");
            entity.Property(e => e.Feld_10_049).HasComment("Indicateur du type d’ouverture");
            entity.Property(e => e.Feld_10_050)
                .HasMaxLength(100)
                .HasComment("Perçage de montage");
            entity.Property(e => e.Feld_10_051)
                .HasComment("Largeur du profil")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_052).HasComment("Compteur de fenêtres");
            entity.Property(e => e.Feld_10_053)
                .HasMaxLength(100)
                .HasComment("Date Production Fin - Identifiant de tournée");
            entity.Property(e => e.Feld_10_054).HasComment("Date Production Fin - Numéro de Semaine AASS");
            entity.Property(e => e.Feld_10_055)
                .HasComment("Largeur des parcloses")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_056)
                .HasComment("Hauteur des parcloses")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_057).HasComment("Compteur d’éléments");
            entity.Property(e => e.Feld_10_058).HasComment("Numéro de champ de cadre");
            entity.Property(e => e.Feld_10_059)
                .HasMaxLength(100)
                .HasComment("Identifiant code-barres du chassis");
            entity.Property(e => e.Feld_10_060)
                .HasMaxLength(100)
                .HasComment("Information complémentaire issue de la saisie de position (champ multiligne)");
            entity.Property(e => e.Feld_10_061)
                .HasMaxLength(100)
                .HasComment("Joint");
            entity.Property(e => e.Feld_10_062)
                .HasMaxLength(100)
                .HasComment("Couleur du joint");
            entity.Property(e => e.Feld_10_063)
                .HasMaxLength(100)
                .HasComment("Indicateur de pilotage");
            entity.Property(e => e.Feld_10_064).HasComment("Longueur de coupe en bout 1");
            entity.Property(e => e.Feld_10_065).HasComment("Longueur de coupe en bout 2");
            entity.Property(e => e.Feld_10_066)
                .HasMaxLength(100)
                .HasComment("Numéro de profil");
            entity.Property(e => e.Feld_10_067)
                .HasMaxLength(100)
                .HasComment("Système de joint");
            entity.Property(e => e.Feld_10_068)
                .HasComment("Poids total [kg]")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_069).HasComment("Dimension accessoire 4 pour la liste 71");
            entity.Property(e => e.Feld_10_070).HasComment("Dimension accessoire 5 pour la liste 71");
            entity.Property(e => e.Feld_10_071).HasComment("Dimension accessoire 6 pour la liste 71");
            entity.Property(e => e.Feld_10_072)
                .HasMaxLength(100)
                .HasComment("Date Livraison");
            entity.Property(e => e.Feld_10_073)
                .HasMaxLength(100)
                .HasComment("Site de fabrication");
            entity.Property(e => e.Feld_10_074)
                .HasMaxLength(100)
                .HasComment("Zone de capacité");
            entity.Property(e => e.Feld_10_075)
                .HasComment("Hauteur du profil")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_076)
                .HasComment("Cote de jour")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_077).HasComment("Largeur de l’élément cadre incluant RV");
            entity.Property(e => e.Feld_10_078).HasComment("Hauteur de l’élément cadre incluant RV");
            entity.Property(e => e.Feld_10_079).HasComment("Largeur de la dimension extérieure de l’élément incluant RV");
            entity.Property(e => e.Feld_10_080).HasComment("Hauteur de la dimension extérieure de l’élément incluant RV");
            entity.Property(e => e.Feld_10_081)
                .HasMaxLength(100)
                .HasComment("Usine de fabrication");
            entity.Property(e => e.Feld_10_082)
                .HasMaxLength(100)
                .HasComment("Date Production Début");
            entity.Property(e => e.Feld_10_083)
                .HasMaxLength(100)
                .HasComment("Nom du chantier");
            entity.Property(e => e.Feld_10_084)
                .HasMaxLength(100)
                .HasComment("Pays du chantier");
            entity.Property(e => e.Feld_10_085).HasComment("Code postal du chantier");
            entity.Property(e => e.Feld_10_086)
                .HasMaxLength(100)
                .HasComment("Ville du chantier");
            entity.Property(e => e.Feld_10_087)
                .HasMaxLength(200)
                .HasComment("Rue du chantier");
            entity.Property(e => e.Feld_10_088)
                .HasMaxLength(100)
                .HasComment("Couleur intérieure du profil");
            entity.Property(e => e.Feld_10_089)
                .HasMaxLength(100)
                .HasComment("Couleur extérieure du profil");
            entity.Property(e => e.Feld_10_090)
                .HasMaxLength(100)
                .HasComment("Code du profil aluminium");
            entity.Property(e => e.Feld_10_091)
                .HasMaxLength(100)
                .HasComment("Code machine aluminium");
            entity.Property(e => e.Feld_10_092)
                .HasMaxLength(100)
                .HasComment("Couleur intérieure du profil aluminium");
            entity.Property(e => e.Feld_10_093)
                .HasMaxLength(100)
                .HasComment("Couleur extérieure du profil aluminium");
            entity.Property(e => e.Feld_10_094).HasComment("Longueur de la pièce aluminium");
            entity.Property(e => e.Feld_10_095).HasComment("Largeur de la pièce aluminium");
            entity.Property(e => e.Feld_10_096).HasComment("Épaisseur de la pièce aluminium");
            entity.Property(e => e.Feld_10_097).HasComment("Angle en bout 1 de la pièce aluminium");
            entity.Property(e => e.Feld_10_098).HasComment("Angle en bout 2 de la pièce aluminium");
            entity.Property(e => e.Feld_10_099).HasComment("Machine en ligne");
            entity.Property(e => e.Feld_10_100)
                .HasMaxLength(100)
                .HasComment("Désignation du profil");
            entity.Property(e => e.Feld_10_101).HasComment("Quantité de pièces pour la position");
            entity.Property(e => e.Feld_10_102)
                .HasMaxLength(100)
                .HasComment("Abréviation");
            entity.Property(e => e.Feld_10_103).HasComment("Cote de poignée");
            entity.Property(e => e.Feld_10_104).HasComment("Découpe Inclinaison Gauche");
            entity.Property(e => e.Feld_10_105).HasComment("Découpe Inclinaison Droite");
            entity.Property(e => e.Feld_10_106).HasComment("Longueur de coupe aluminium 1");
            entity.Property(e => e.Feld_10_107).HasComment("Longueur de coupe aluminium 2");
            entity.Property(e => e.Feld_10_108).HasComment("Longueur de débit avec corrections de contre-dépouille");
            entity.Property(e => e.Feld_10_109)
                .HasMaxLength(100)
                .HasComment("Couleur aluminium");
            entity.Property(e => e.Feld_10_110)
                .HasMaxLength(100)
                .HasComment("Point de vente (principal)");
            entity.Property(e => e.Feld_10_111).HasComment("Découpe Pivot Gauche");
            entity.Property(e => e.Feld_10_112).HasComment("Découpe Pivot Droit");
            entity.Property(e => e.Feld_10_113)
                .HasMaxLength(1000)
                .HasComment("Texte de fenêtre");
            entity.Property(e => e.Feld_10_114).HasComment("Position dans l’espace : 1=Bas,2=Haut,3=Gauche,4=Droite,5=Bas,6=Haut,7=Gauche,8=Droite");
            entity.Property(e => e.Feld_10_115).HasComment("Hauteur de l’ouvrant aluminium");
            entity.Property(e => e.Feld_10_116).HasComment("Largeur de l’ouvrant aluminium");
            entity.Property(e => e.Feld_10_117)
                .HasMaxLength(100)
                .HasComment("Pièces aluminium à l’intérieur de l’ouvrant");
            entity.Property(e => e.Feld_10_118).HasComment("Position de livraison");
            entity.Property(e => e.Feld_10_119).HasComment("Angle intérieur");
            entity.Property(e => e.Feld_10_120).HasComment("Angle intérieur / 2");
            entity.Property(e => e.Feld_10_121).HasComment("Composition extérieure");
            entity.Property(e => e.Feld_10_122).HasComment("Angle en bout 1");
            entity.Property(e => e.Feld_10_123).HasComment("Angle en bout 2");
            entity.Property(e => e.Feld_10_124)
                .HasMaxLength(100)
                .HasComment("Profil de battement d’ouvrant Oui / Non");
            entity.Property(e => e.Feld_10_125)
                .HasMaxLength(100)
                .HasComment("Code pour type d’ouverture spécial");
            entity.Property(e => e.Feld_10_126)
                .HasMaxLength(100)
                .HasComment("Numéro de programme de soudure");
            entity.Property(e => e.Feld_10_127)
                .HasComment("Largeur de feuillure de l’ouvrant")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_128).HasComment("Hauteur de feuillure de l’ouvrant");
            entity.Property(e => e.Feld_10_129)
                .HasMaxLength(100)
                .HasComment("Cote de pose du volet roulant");
            entity.Property(e => e.Feld_10_130).HasComment("Espace réservé pour information de paire");
            entity.Property(e => e.Feld_10_131).HasComment("Cote de poignée du cadre");
            entity.Property(e => e.Feld_10_132).HasComment("Cote de poignée manuelle");
            entity.Property(e => e.Feld_10_133).HasComment("Quantité totale pour la position");
            entity.Property(e => e.Feld_10_134)
                .HasMaxLength(100)
                .HasComment("Dimensions du vitrage");
            entity.Property(e => e.Feld_10_135)
                .HasMaxLength(100)
                .HasComment("Cotes de division sous forme de chaîne avec codes (Cote1/Code1/Cote2/Code2)");
            entity.Property(e => e.Feld_10_136)
                .HasMaxLength(100)
                .HasComment("Dimension Vantaux G/D");
            entity.Property(e => e.Feld_10_137)
                .HasMaxLength(100)
                .HasComment("Parcloses par ouvrant / cadre (cadre uniquement pour vitrage fixe)");
            entity.Property(e => e.Feld_10_138)
                .HasMaxLength(100)
                .HasComment("Profils de battement pour la partie d’ouvrant actuelle");
            entity.Property(e => e.Feld_10_139)
                .HasMaxLength(100)
                .HasComment("Accessoires selon identifiant d’impression");
            entity.Property(e => e.Feld_10_140)
                .HasMaxLength(1000)
                .HasComment("Accessoires selon identifiant d’impression");
            entity.Property(e => e.Feld_10_141)
                .HasMaxLength(1000)
                .HasComment("Accessoires selon identifiant d’impression");
            entity.Property(e => e.Feld_10_142)
                .HasMaxLength(1000)
                .HasComment("Accessoires selon identifiant d’impression");
            entity.Property(e => e.Feld_10_143)
                .HasMaxLength(1000)
                .HasComment("Accessoires selon identifiant d’impression");
            entity.Property(e => e.Feld_10_144).HasComment("Nombre total d’éléments");
            entity.Property(e => e.Feld_10_145)
                .HasMaxLength(100)
                .HasComment("Joint de cadre 1");
            entity.Property(e => e.Feld_10_146)
                .HasMaxLength(100)
                .HasComment("Joint de cadre 2");
            entity.Property(e => e.Feld_10_147)
                .HasMaxLength(100)
                .HasComment("Raccords éventuels, baguettes avant");
            entity.Property(e => e.Feld_10_148)
                .HasMaxLength(1000)
                .HasComment("Affectation du vitrage");
            entity.Property(e => e.Feld_10_149)
                .HasMaxLength(100)
                .HasComment("Profil adaptateur");
            entity.Property(e => e.Feld_10_150)
                .HasMaxLength(100)
                .HasComment("Joint ouvrant / cadre intérieur");
            entity.Property(e => e.Feld_10_151)
                .HasMaxLength(100)
                .HasComment("Joint ouvrant / cadre extérieur");
            entity.Property(e => e.Feld_10_152)
                .HasMaxLength(100)
                .HasComment("Joint de battement 1");
            entity.Property(e => e.Feld_10_153)
                .HasMaxLength(100)
                .HasComment("Joint de battement 2");
            entity.Property(e => e.Feld_10_154)
                .HasMaxLength(100)
                .HasComment("Joint de battement 3");
            entity.Property(e => e.Feld_10_155)
                .HasMaxLength(100)
                .HasComment("Fournisseur de profil");
            entity.Property(e => e.Feld_10_156)
                .HasComment("Largeur incluant correction de largeur")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_157).HasComment("Hauteur incluant correction de largeur");
            entity.Property(e => e.Feld_10_158).HasComment("Recoupe / rognage");
            entity.Property(e => e.Feld_10_159).HasComment("Correction de largeur issue des données de référence");
            entity.Property(e => e.Feld_10_160).HasComment("Feuillure du cadre");
            entity.Property(e => e.Feld_10_161)
                .HasMaxLength(100)
                .HasComment("Position de la poignée");
            entity.Property(e => e.Feld_10_162).HasComment("Nombre total d’unités de fenêtres");
            entity.Property(e => e.Feld_10_163)
                .HasMaxLength(100)
                .HasComment("Nom du client (Final)");
            entity.Property(e => e.Feld_10_164).HasComment("Ligne issue de D_Teilserien");
            entity.Property(e => e.Feld_10_165)
                .HasMaxLength(100)
                .HasComment("Code-barres avant conversion");
            entity.Property(e => e.Feld_10_166)
                .HasComment("Longueur sans surcote de soudure")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_167)
                .HasMaxLength(100)
                .HasComment("Indicateur d’armature");
            entity.Property(e => e.Feld_10_168)
                .HasMaxLength(100)
                .HasComment("Indicateur de zone de fabrication pour séries partielles");
            entity.Property(e => e.Feld_10_169).HasComment("Catégorie Produit (24=Coulissant) + Sous-Série 2 et 3");
            entity.Property(e => e.Feld_10_170)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_171).HasComment("Numéro Commande Tryba Mayenne");
            entity.Property(e => e.Feld_10_172).HasComment("Indicateur du type de pièce");
            entity.Property(e => e.Feld_10_173)
                .HasMaxLength(100)
                .HasComment("Pièce rapportée / élément additionnel");
            entity.Property(e => e.Feld_10_174)
                .HasMaxLength(100)
                .HasComment("Fraisage pour logement de poignée sur battement pour ouvrant principal");
            entity.Property(e => e.Feld_10_175)
                .HasMaxLength(100)
                .HasComment("Contre-profil 1 pour petits-bois");
            entity.Property(e => e.Feld_10_176)
                .HasMaxLength(100)
                .HasComment("Contre-profil 2 pour petits-bois");
            entity.Property(e => e.Feld_10_177)
                .HasMaxLength(100)
                .HasComment("Indicateur de couleur 1");
            entity.Property(e => e.Feld_10_178)
                .HasMaxLength(100)
                .HasComment("Information de laque / finition");
            entity.Property(e => e.Feld_10_179).HasComment("Nombre d’évacuations niveau 0");
            entity.Property(e => e.Feld_10_180).HasComment("Nombre d’évacuations niveau 1");
            entity.Property(e => e.Feld_10_181)
                .HasMaxLength(100)
                .HasComment("Code d’évacuation utilisé lors du calcul");
            entity.Property(e => e.Feld_10_182).HasComment("Éjection de l’ouvrant en cas de largeur d’ouverture");
            entity.Property(e => e.Feld_10_183).HasComment("Position de la pièce");
            entity.Property(e => e.Feld_10_184)
                .HasMaxLength(100)
                .HasComment("Zone de quai");
            entity.Property(e => e.Feld_10_185)
                .HasMaxLength(100)
                .HasComment("Nom de l’intermédiaire");
            entity.Property(e => e.Feld_10_186)
                .HasMaxLength(100)
                .HasComment("Indicateur de joint");
            entity.Property(e => e.Feld_10_187)
                .HasMaxLength(100)
                .HasComment("Indicateur de ferrure de l’ouvrant issue du ferrage associé sinon global");
            entity.Property(e => e.Feld_10_188)
                .HasMaxLength(100)
                .HasComment("Marquage de l’ouvrant");
            entity.Property(e => e.Feld_10_189)
                .HasMaxLength(100)
                .HasComment("Code de coupe des lames (variante de coupe des lames de volet roulant)");
            entity.Property(e => e.Feld_10_190)
                .HasMaxLength(100)
                .HasComment("Numéro Wegoma");
            entity.Property(e => e.Feld_10_191)
                .HasMaxLength(100)
                .HasComment("Numéro Staude");
            entity.Property(e => e.Feld_10_192)
                .HasMaxLength(100)
                .HasComment("Chariot du groupe machine");
            entity.Property(e => e.Feld_10_193)
                .HasMaxLength(100)
                .HasComment("Casier du groupe machine");
            entity.Property(e => e.Feld_10_194)
                .HasMaxLength(100)
                .HasComment("Numéro Urban");
            entity.Property(e => e.Feld_10_195)
                .HasMaxLength(100)
                .HasComment("Numéro Dubüs");
            entity.Property(e => e.Feld_10_196).HasComment("Usinage en bout 0");
            entity.Property(e => e.Feld_10_197).HasComment("Usinage en bout 1");
            entity.Property(e => e.Feld_10_198)
                .HasMaxLength(100)
                .HasComment("Société intermédiaire");
            entity.Property(e => e.Feld_10_199).HasComment("Ouvrant de battement ou contre-ouvrant de battement");
            entity.Property(e => e.Feld_10_200)
                .HasMaxLength(100)
                .HasComment("Joints extérieurs");
            entity.Property(e => e.Feld_10_201)
                .HasMaxLength(100)
                .HasComment("Éléments de fenêtre adjacents");
            entity.Property(e => e.Feld_10_202)
                .HasMaxLength(100)
                .HasComment("Éléments de fenêtre adjacents intérieurs");
            entity.Property(e => e.Feld_10_203)
                .HasMaxLength(100)
                .HasComment("Traverse de protection WS1 issue des données de position, uniquement pour cadre bas");
            entity.Property(e => e.Feld_10_204)
                .HasMaxLength(100)
                .HasComment("Technicien");
            entity.Property(e => e.Feld_10_205)
                .HasMaxLength(100)
                .HasComment("Identifiant Look3E pour la Commande Client");
            entity.Property(e => e.Feld_10_206)
                .HasMaxLength(100)
                .HasComment("Champ libre (0)");
            entity.Property(e => e.Feld_10_207)
                .HasMaxLength(100)
                .HasComment("Projet de construction");
            entity.Property(e => e.Feld_10_208)
                .HasMaxLength(100)
                .HasComment("Espace réservé pour numéro de machine");
            entity.Property(e => e.Feld_10_209)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_210).HasComment("Numéro de pièce client séquentiel indépendant du pairage");
            entity.Property(e => e.Feld_10_211).HasComment("Numéro séquentiel de bloc de soudure");
            entity.Property(e => e.Feld_10_212)
                .HasMaxLength(100)
                .HasComment("Date Production Fin");
            entity.Property(e => e.Feld_10_213)
                .HasMaxLength(100)
                .HasComment("Date Expédition");
            entity.Property(e => e.Feld_10_214)
                .HasMaxLength(100)
                .HasComment("Cotes de division sous forme de chaîne depuis la gauche / le bas");
            entity.Property(e => e.Feld_10_215)
                .HasMaxLength(100)
                .HasComment("Code de mécanisme (boîtier / entraînement)");
            entity.Property(e => e.Feld_10_216).HasComment("Indicateur d’ouverture");
            entity.Property(e => e.Feld_10_217).HasComment("Position pour élargissements");
            entity.Property(e => e.Feld_10_218)
                .HasMaxLength(100)
                .HasComment("Espace réservé pour profondeur d’insertion");
            entity.Property(e => e.Feld_10_219).HasComment("Repère d’appui pour bois de calage");
            entity.Property(e => e.Feld_10_220).HasComment("Catégorie Produit (24=Coulissant) + Sous-Série 2 et 3");
            entity.Property(e => e.Feld_10_221)
                .HasMaxLength(100)
                .HasComment("Contre-profil cadre / seuil");
            entity.Property(e => e.Feld_10_222)
                .HasMaxLength(100)
                .HasComment("Espace réservé pour perçages de ventilation");
            entity.Property(e => e.Feld_10_223).HasComment("Plan de l’ouvrant");
            entity.Property(e => e.Feld_10_224)
                .HasMaxLength(100)
                .HasComment("Partie de cadre adjacente à l’ouvrant");
            entity.Property(e => e.Feld_10_225)
                .HasMaxLength(100)
                .HasComment("Plans d’ouvrants adjacents");
            entity.Property(e => e.Feld_10_226)
                .HasMaxLength(100)
                .HasComment("Profils d’ouvrant adjacents");
            entity.Property(e => e.Feld_10_227).HasComment("Numéro de ligne dans la commande client");
            entity.Property(e => e.Feld_10_228)
                .HasMaxLength(100)
                .HasComment("Indicateur issu de la classe de position");
            entity.Property(e => e.Feld_10_229)
                .HasComment("Largeur cadre / ouvrant en dixièmes")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_230)
                .HasComment("Hauteur cadre / ouvrant en dixièmes")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_231)
                .HasComment("Largeur de feuillure de ferrure en dixièmes")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_232)
                .HasComment("Hauteur de feuillure de ferrure en dixièmes")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_233)
                .HasMaxLength(200)
                .HasComment("Coulissant Type");
            entity.Property(e => e.Feld_10_234)
                .HasMaxLength(200)
                .HasComment("Coulissant Type détaillé");
            entity.Property(e => e.Feld_10_235)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_236)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_237)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_238)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_239).HasComment("Numéro de ligne dans la commande client");
            entity.Property(e => e.Feld_10_240)
                .HasMaxLength(100)
                .HasComment("Système aluminium");
            entity.Property(e => e.Feld_10_241)
                .HasMaxLength(100)
                .HasComment("Code-barres avant conversion");
            entity.Property(e => e.Feld_10_242)
                .HasMaxLength(100)
                .HasComment("Date Production Début");
            entity.Property(e => e.Feld_10_243).HasComment("Date Production Début - Numéro de Semaine AASS");
            entity.Property(e => e.Feld_10_244)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_245)
                .HasComment("Largeur avec correction de largeur et à la pointe, uniquement pour fenêtre standard")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_246).HasComment("Hauteur avec correction de largeur et à la pointe, uniquement pour fenêtre standard");
            entity.Property(e => e.Feld_10_247)
                .HasMaxLength(100)
                .HasComment("Éléments intérieurs du cadre");
            entity.Property(e => e.Feld_10_248).HasComment("Feuillure de vitrage de la pièce");
            entity.Property(e => e.Feld_10_249).HasComment("Référence au gabarit acier");
            entity.Property(e => e.Feld_10_250).HasComment("Côté de vissage acier");
            entity.Property(e => e.Feld_10_251)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_252)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_253)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_254)
                .HasMaxLength(1000)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_255)
                .HasMaxLength(100)
                .HasComment("Code de surface");
            entity.Property(e => e.Feld_10_256)
                .HasMaxLength(100)
                .HasComment("Texte de surface");
            entity.Property(e => e.Feld_10_257).HasComment("Un seul vitrage (True=un seul vitrage, False=plusieurs vitrages)");
            entity.Property(e => e.Feld_10_258).HasComment("Champs adjacents uniquement traverse / meneau");
            entity.Property(e => e.Feld_10_259)
                .HasMaxLength(100)
                .HasComment("Indicateur de fabrication uniquement pour le cadre");
            entity.Property(e => e.Feld_10_260)
                .HasMaxLength(100)
                .HasComment("Champs d’ouvrant adjacents à la pièce et au seuil (de / à)");
            entity.Property(e => e.Feld_10_261)
                .HasMaxLength(100)
                .HasComment("Champs d’ouvrant adjacents à la pièce et aux accessoires supérieurs (de / à)");
            entity.Property(e => e.Feld_10_262)
                .HasMaxLength(100)
                .HasComment("Ferrage Type Crémone 1");
            entity.Property(e => e.Feld_10_263).HasComment("Correction de largeur des éléments adjacents côté 0");
            entity.Property(e => e.Feld_10_264).HasComment("Correction de largeur des éléments adjacents côté 1");
            entity.Property(e => e.Feld_10_265).HasComment("Correction de largeur");
            entity.Property(e => e.Feld_10_266)
                .HasMaxLength(100)
                .HasComment("Chariot de répartition");
            entity.Property(e => e.Feld_10_267).HasComment("Longueur = cote de jour avec angle sur largeur de base + correction de largeur de base");
            entity.Property(e => e.Feld_10_268)
                .HasMaxLength(500)
                .HasComment("Code et cote de mortaisage des petits-bois de l’ouvrant");
            entity.Property(e => e.Feld_10_269)
                .HasMaxLength(500)
                .HasComment("Code de surface intérieure");
            entity.Property(e => e.Feld_10_270)
                .HasMaxLength(500)
                .HasComment("Texte de surface intérieure");
            entity.Property(e => e.Feld_10_271).HasComment("Surface manuelle");
            entity.Property(e => e.Feld_10_272).HasComment("Surface intérieure manuelle");
            entity.Property(e => e.Feld_10_273)
                .HasMaxLength(100)
                .HasComment("Code client");
            entity.Property(e => e.Feld_10_274)
                .HasMaxLength(200)
                .HasComment("Point de Vente Tryba Adresse Rue");
            entity.Property(e => e.Feld_10_275)
                .HasMaxLength(100)
                .HasComment("Accessoire en bout 1");
            entity.Property(e => e.Feld_10_276)
                .HasMaxLength(100)
                .HasComment("Accessoire en bout 2");
            entity.Property(e => e.Feld_10_277).HasComment("Numéro séquentiel de pièce de série partielle");
            entity.Property(e => e.Feld_10_278).HasComment("Rue du client");
            entity.Property(e => e.Feld_10_279)
                .HasMaxLength(100)
                .HasComment("Information de série");
            entity.Property(e => e.Feld_10_280).HasComment("Code-barres avec complément de côté");
            entity.Property(e => e.Feld_10_281).HasComment("Code-barres avec complément de côté");
            entity.Property(e => e.Feld_10_282)
                .HasMaxLength(100)
                .HasComment("Ventaux Pré-défini");
            entity.Property(e => e.Feld_10_283)
                .HasMaxLength(500)
                .HasComment("Information de coordonnées");
            entity.Property(e => e.Feld_10_284)
                .HasMaxLength(100)
                .HasComment("Couleur du joint");
            entity.Property(e => e.Feld_10_285)
                .HasMaxLength(100)
                .HasComment("Numéro de construction");
            entity.Property(e => e.Feld_10_286)
                .HasMaxLength(100)
                .HasComment("Groupe de numéros de construction");
            entity.Property(e => e.Feld_10_287)
                .HasMaxLength(100)
                .HasComment("Information de numéro de construction");
            entity.Property(e => e.Feld_10_288)
                .HasMaxLength(100)
                .HasComment("Sous-Série + Gamme + Couleur");
            entity.Property(e => e.Feld_10_289)
                .HasMaxLength(100)
                .HasComment("Angle 1 de la baguette de battement");
            entity.Property(e => e.Feld_10_290)
                .HasMaxLength(100)
                .HasComment("Angle 2 de la baguette de battement");
            entity.Property(e => e.Feld_10_291)
                .HasMaxLength(100)
                .HasComment("Angle 2 de la baguette de battement");
            entity.Property(e => e.Feld_10_292)
                .HasMaxLength(100)
                .HasComment("Numéro de casier restant");
            entity.Property(e => e.Feld_10_293).HasComment("Nombre de champs de cadre");
            entity.Property(e => e.Feld_10_294)
                .HasMaxLength(100)
                .HasComment("Joint intérieur issu du système de parcloses");
            entity.Property(e => e.Feld_10_295)
                .HasMaxLength(100)
                .HasComment("Indicateur Fen");
            entity.Property(e => e.Feld_10_296)
                .HasMaxLength(100)
                .HasComment("Indicateur ACTH");
            entity.Property(e => e.Feld_10_297)
                .HasMaxLength(100)
                .HasComment("Indicateur volet roulant");
            entity.Property(e => e.Feld_10_298)
                .HasMaxLength(100)
                .HasComment("Numéro d’étiquette CSTB");
            entity.Property(e => e.Feld_10_299).HasComment("Commanditaire de la commande");
            entity.Property(e => e.Feld_10_300).HasComment("Position informatique du commanditaire");
            entity.Property(e => e.Feld_10_301)
                .HasMaxLength(100)
                .HasComment("Système de battement (stulp)");
            entity.Property(e => e.Feld_10_302).HasComment("Article Compteur");
            entity.Property(e => e.Feld_10_303).HasComment("Compteur de fenêtres Ferro");
            entity.Property(e => e.Feld_10_304).HasComment("Côté de battement Ferro");
            entity.Property(e => e.Feld_10_305).HasComment("Position de la poignée");
            entity.Property(e => e.Feld_10_306)
                .HasMaxLength(100)
                .HasComment("Chariot - liste 1");
            entity.Property(e => e.Feld_10_307)
                .HasMaxLength(1000)
                .HasComment("Chariot des éléments de ferrure sur l’ouvrant");
            entity.Property(e => e.Feld_10_308).HasComment("Position du battement");
            entity.Property(e => e.Feld_10_309)
                .HasMaxLength(100)
                .HasComment("Groupe chariot (liste 1)");
            entity.Property(e => e.Feld_10_310).HasComment("Bloc chariot (liste 1)");
            entity.Property(e => e.Feld_10_311).HasComment("Numéro de chariot (liste 1)");
            entity.Property(e => e.Feld_10_312).HasComment("Casier de chariot (liste 1)");
            entity.Property(e => e.Feld_10_313).HasComment("Numéro de pièce chariot (liste 1)");
            entity.Property(e => e.Feld_10_314)
                .HasMaxLength(100)
                .HasComment("Répartition gauche/bas TA");
            entity.Property(e => e.Feld_10_315).HasComment("Code en ligne - variante 3");
            entity.Property(e => e.Feld_10_316)
                .HasMaxLength(100)
                .HasComment("Groupe matière 2");
            entity.Property(e => e.Feld_10_317)
                .HasMaxLength(100)
                .HasComment("Numéro de commande");
            entity.Property(e => e.Feld_10_318).HasComment("Marquage couleur extérieure du profil aluminium");
            entity.Property(e => e.Feld_10_319)
                .HasMaxLength(100)
                .HasComment("Casier");
            entity.Property(e => e.Feld_10_320)
                .HasMaxLength(100)
                .HasComment("Étage");
            entity.Property(e => e.Feld_10_321)
                .HasMaxLength(100)
                .HasComment("Chariot");
            entity.Property(e => e.Feld_10_322)
                .HasMaxLength(100)
                .HasComment("Espace réservé");
            entity.Property(e => e.Feld_10_323).HasComment("Pièce dans le casier");
            entity.Property(e => e.Feld_10_324)
                .HasMaxLength(100)
                .HasComment("Gestionnaire / chargé de dossier");
            entity.Property(e => e.Feld_10_325)
                .HasMaxLength(100)
                .HasComment("Gestionnaire / chargé de dossier");
            entity.Property(e => e.Feld_10_326)
                .HasMaxLength(100)
                .HasComment("Numéro client");
            entity.Property(e => e.Feld_10_327)
                .HasMaxLength(100)
                .HasComment("Article Code Barre");
            entity.Property(e => e.Feld_10_328)
                .HasMaxLength(100)
                .HasComment("Article Couleur Int/Ext 1");
            entity.Property(e => e.Feld_10_329)
                .HasMaxLength(100)
                .HasComment("Groupe matière 2 des éléments de ferrure d’ouvrant");
            entity.Property(e => e.Feld_10_330)
                .HasMaxLength(100)
                .HasComment("Article Référence");
            entity.Property(e => e.Feld_10_331).HasComment("Largeur de seuil");
            entity.Property(e => e.Feld_10_332).HasComment("Motif binaire Rotox");
            entity.Property(e => e.Feld_10_333).HasComment("Motif binaire Rotox");
            entity.Property(e => e.Feld_10_334).HasComment("Indicateur acier issu du profil PVC");
            entity.Property(e => e.Feld_10_335)
                .HasMaxLength(100)
                .HasComment("Information pour montage en rainure");
            entity.Property(e => e.Feld_10_336)
                .HasMaxLength(100)
                .HasComment("Chariot L1 par position / élément");
            entity.Property(e => e.Feld_10_337)
                .HasMaxLength(100)
                .HasComment("Groupe chariot (liste 1)");
            entity.Property(e => e.Feld_10_338).HasComment("Bloc chariot (liste 1)");
            entity.Property(e => e.Feld_10_339).HasComment("Numéro de chariot (liste 1)");
            entity.Property(e => e.Feld_10_340).HasComment("Casier de chariot (liste 1)");
            entity.Property(e => e.Feld_10_341).HasComment("Numéro de pièce chariot (liste 1)");
            entity.Property(e => e.Feld_10_342)
                .HasMaxLength(100)
                .HasComment("Information de renfort");
            entity.Property(e => e.Feld_10_343).HasComment("Mode de position");
            entity.Property(e => e.Feld_10_344).HasComment("Mode de position");
            entity.Property(e => e.Feld_10_345)
                .HasMaxLength(100)
                .HasComment("Numéro de commande acier");
            entity.Property(e => e.Feld_10_346)
                .HasMaxLength(100)
                .HasComment("Article associé - référence à droite");
            entity.Property(e => e.Feld_10_347)
                .HasMaxLength(100)
                .HasComment("Article associé - référence à gauche");
            entity.Property(e => e.Feld_10_348).HasComment("Numéro de commande variante");
            entity.Property(e => e.Feld_10_349).HasComment("Battement secondaire ou ouvrant standard");
            entity.Property(e => e.Feld_10_350)
                .HasMaxLength(100)
                .HasComment("Système de battement (stulp)");
            entity.Property(e => e.Feld_10_351).HasComment("Usinage aluminium en bout 1");
            entity.Property(e => e.Feld_10_352).HasComment("Usinage aluminium en bout 1");
            entity.Property(e => e.Feld_10_353).HasComment("Forme spéciale");
            entity.Property(e => e.Feld_10_354).HasComment("Palier d’angle et compas");
            entity.Property(e => e.Feld_10_355)
                .HasMaxLength(100)
                .HasComment("Épaisseur du joint extérieur ouvrant / cadre");
            entity.Property(e => e.Feld_10_356)
                .HasMaxLength(100)
                .HasComment("Article Code barre Extension");
            entity.Property(e => e.Feld_10_357).HasComment("Emplacement F_T");
            entity.Property(e => e.Feld_10_358)
                .HasMaxLength(100)
                .HasComment("Éléments intérieurs du cadre 2");
            entity.Property(e => e.Feld_10_359)
                .HasMaxLength(100)
                .HasComment("Espace réservé – marqueur machine 2");
            entity.Property(e => e.Feld_10_360)
                .HasComment("Surcote de correction issue de la liaison de contre-profil côté 1")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_361)
                .HasComment("Surcote de correction issue de la liaison de contre-profil côté 2")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_362)
                .HasMaxLength(100)
                .HasComment("Espace réservé pour casier de parcloses en cas de blocage");
            entity.Property(e => e.Feld_10_363)
                .HasMaxLength(100)
                .HasComment("Espace réservé pour numéro de date");
            entity.Property(e => e.Feld_10_364).HasComment("Correction de longueur pour acier côté 1");
            entity.Property(e => e.Feld_10_365).HasComment("Correction de longueur pour acier côté 2");
            entity.Property(e => e.Feld_10_366).HasComment("Correction acier dépendante de l’angle côté 1");
            entity.Property(e => e.Feld_10_367).HasComment("Correction acier dépendante de l’angle côté 2");
            entity.Property(e => e.Feld_10_368)
                .HasMaxLength(100)
                .HasComment("Numéro de commande du job d’impression");
            entity.Property(e => e.Feld_10_369)
                .HasMaxLength(100)
                .HasComment("Article d’achat");
            entity.Property(e => e.Feld_10_370)
                .HasMaxLength(100)
                .HasComment("Variante d’achat");
            entity.Property(e => e.Feld_10_371).HasComment("Correction adjacente");
            entity.Property(e => e.Feld_10_372).HasComment("Correction adjacente");
            entity.Property(e => e.Feld_10_373).HasComment("Correction absolue");
            entity.Property(e => e.Feld_10_374).HasComment("Correction issue de la liaison de contre-profil");
            entity.Property(e => e.Feld_10_375).HasComment("Correction issue de la liaison de contre-profil");
            entity.Property(e => e.Feld_10_376).HasComment("Épaisseur du vitrage");
            entity.Property(e => e.Feld_10_377).HasComment("Poids du vitrage");
            entity.Property(e => e.Feld_10_378).HasComment("Indicateur DIN de l’ouvrant (RQN 069954)");
            entity.Property(e => e.Feld_10_379)
                .HasMaxLength(100)
                .HasComment("Code-barres déterminé à partir de {Glas_Barcode*}");
            entity.Property(e => e.Feld_10_380)
                .HasMaxLength(100)
                .HasComment("Adresse du fournisseur de surface");
            entity.Property(e => e.Feld_10_381)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur extérieur 1");
            entity.Property(e => e.Feld_10_382)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur extérieur 2");
            entity.Property(e => e.Feld_10_383)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur extérieur 3");
            entity.Property(e => e.Feld_10_384)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur extérieur 4");
            entity.Property(e => e.Feld_10_385)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur intérieur 1");
            entity.Property(e => e.Feld_10_386)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur intérieur 2");
            entity.Property(e => e.Feld_10_387)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur intérieur 3");
            entity.Property(e => e.Feld_10_388)
                .HasMaxLength(100)
                .HasComment("Marquage surface - indicateur intérieur 4");
            entity.Property(e => e.Feld_10_389)
                .HasComment("Cote de réglage")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_390)
                .HasMaxLength(100)
                .HasComment("Groupe d’optimisation des variantes");
            entity.Property(e => e.Feld_10_391)
                .HasComment("Cote résiduelle")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_392).HasComment("Usine de fabrication issue des données de position");
            entity.Property(e => e.Feld_10_393)
                .HasComment("Poids par mètre linéaire")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_394)
                .HasMaxLength(100)
                .HasComment("Code de pièce");
            entity.Property(e => e.Feld_10_395).HasComment("Série partielle");
            entity.Property(e => e.Feld_10_396)
                .HasMaxLength(100)
                .HasComment("Code-barres");
            entity.Property(e => e.Feld_10_397)
                .HasMaxLength(100)
                .HasComment("Code chariot converti");
            entity.Property(e => e.Feld_10_398)
                .HasMaxLength(100)
                .HasComment("Valeur d’allongement");
            entity.Property(e => e.Feld_10_399)
                .HasMaxLength(100)
                .HasComment("Indicateur A");
            entity.Property(e => e.Feld_10_400)
                .HasMaxLength(100)
                .HasComment("Indicateur B");
            entity.Property(e => e.Feld_10_401)
                .HasMaxLength(100)
                .HasComment("Indicateur C");
            entity.Property(e => e.Feld_10_402)
                .HasMaxLength(100)
                .HasComment("Indicateur D");
            entity.Property(e => e.Feld_10_403)
                .HasMaxLength(100)
                .HasComment("Image DXF");
            entity.Property(e => e.Feld_10_404)
                .HasComment("Découpe Dimension Base")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_405)
                .HasMaxLength(100)
                .HasComment("Indicateur de résolution de ferrure 1");
            entity.Property(e => e.Feld_10_406)
                .HasMaxLength(100)
                .HasComment("Indicateur de résolution de ferrure 2");
            entity.Property(e => e.Feld_10_407)
                .HasMaxLength(100)
                .HasComment("Indicateur de résolution de ferrure 3");
            entity.Property(e => e.Feld_10_408)
                .HasMaxLength(100)
                .HasComment("Indicateur de résolution de ferrure 4");
            entity.Property(e => e.Feld_10_409)
                .HasMaxLength(100)
                .HasComment("Marqueur de rail final");
            entity.Property(e => e.Feld_10_410)
                .HasMaxLength(100)
                .HasComment("Nombre de pièces non percées");
            entity.Property(e => e.Feld_10_411)
                .HasMaxLength(100)
                .HasComment("Type de profil de renfort");
            entity.Property(e => e.Feld_10_412)
                .HasMaxLength(100)
                .HasComment("Variante de profil de renfort");
            entity.Property(e => e.Feld_10_413)
                .HasMaxLength(100)
                .HasComment("Surface du profil de renfort");
            entity.Property(e => e.Feld_10_414)
                .HasMaxLength(100)
                .HasComment("Surface intérieure du profil de renfort");
            entity.Property(e => e.Feld_10_415)
                .HasMaxLength(100)
                .HasComment("Numéro de programme du profil de renfort");
            entity.Property(e => e.Feld_10_416)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_417)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_418)
                .HasMaxLength(100)
                .HasComment("Type de rail de suspension");
            entity.Property(e => e.Feld_10_419)
                .HasMaxLength(100)
                .HasComment("Variante de rail de suspension");
            entity.Property(e => e.Feld_10_420)
                .HasMaxLength(100)
                .HasComment("Surface du rail de suspension");
            entity.Property(e => e.Feld_10_421)
                .HasMaxLength(100)
                .HasComment("Surface intérieure du rail de suspension");
            entity.Property(e => e.Feld_10_422)
                .HasMaxLength(100)
                .HasComment("Numéro de programme du rail de suspension");
            entity.Property(e => e.Feld_10_423)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_424)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_425)
                .HasMaxLength(100)
                .HasComment("Type de rail final");
            entity.Property(e => e.Feld_10_426)
                .HasMaxLength(100)
                .HasComment("Variante du rail final");
            entity.Property(e => e.Feld_10_427)
                .HasMaxLength(100)
                .HasComment("Surface du rail final");
            entity.Property(e => e.Feld_10_428)
                .HasMaxLength(100)
                .HasComment("Surface intérieure du rail final");
            entity.Property(e => e.Feld_10_429)
                .HasMaxLength(100)
                .HasComment("Numéro de programme du rail final");
            entity.Property(e => e.Feld_10_430)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_431)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_432)
                .HasMaxLength(100)
                .HasComment("Type de profil perforé");
            entity.Property(e => e.Feld_10_433)
                .HasMaxLength(100)
                .HasComment("Variante de profil perforé");
            entity.Property(e => e.Feld_10_434)
                .HasMaxLength(100)
                .HasComment("Surface du profil perforé");
            entity.Property(e => e.Feld_10_435)
                .HasMaxLength(100)
                .HasComment("Surface intérieure du profil non perforé");
            entity.Property(e => e.Feld_10_436)
                .HasMaxLength(100)
                .HasComment("Numéro de programme du profil perforé");
            entity.Property(e => e.Feld_10_437)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_438)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_439)
                .HasMaxLength(100)
                .HasComment("Type de profil non perforé");
            entity.Property(e => e.Feld_10_440)
                .HasMaxLength(100)
                .HasComment("Variante de profil non perforé");
            entity.Property(e => e.Feld_10_441)
                .HasMaxLength(100)
                .HasComment("Surface du profil non perforé");
            entity.Property(e => e.Feld_10_442)
                .HasMaxLength(100)
                .HasComment("Surface intérieure du profil non perforé");
            entity.Property(e => e.Feld_10_443)
                .HasMaxLength(100)
                .HasComment("Numéro de programme du profil non perforé");
            entity.Property(e => e.Feld_10_444)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_445)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_446)
                .HasMaxLength(100)
                .HasComment("Type de butée");
            entity.Property(e => e.Feld_10_447)
                .HasMaxLength(100)
                .HasComment("Variante de butée");
            entity.Property(e => e.Feld_10_448)
                .HasMaxLength(100)
                .HasComment("Surface de la butée");
            entity.Property(e => e.Feld_10_449)
                .HasMaxLength(100)
                .HasComment("Surface intérieure de la butée");
            entity.Property(e => e.Feld_10_450)
                .HasMaxLength(100)
                .HasComment("Numéro de programme de la butée");
            entity.Property(e => e.Feld_10_451)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_452)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_453)
                .HasMaxLength(100)
                .HasComment("Type de caisson");
            entity.Property(e => e.Feld_10_454)
                .HasMaxLength(100)
                .HasComment("Type d’entraînement");
            entity.Property(e => e.Feld_10_455)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_456)
                .HasMaxLength(100)
                .HasComment("Réserve");
            entity.Property(e => e.Feld_10_457).HasComment("Vue des données de position");
            entity.Property(e => e.Feld_10_458)
                .HasMaxLength(100)
                .HasComment("Indicateur de longueur");
            entity.Property(e => e.Feld_10_459).HasComment("Correction de soudure 2");
            entity.Property(e => e.Feld_10_460).HasComment("Forme spéciale cintrée – rayon extérieur");
            entity.Property(e => e.Feld_10_461).HasComment("Forme spéciale cintrée – rayon intérieur");
            entity.Property(e => e.Feld_10_462)
                .HasMaxLength(100)
                .HasComment("Article Couleur Int/Ext 2");
            entity.Property(e => e.Feld_10_463).HasComment("Indicateur de variante 1");
            entity.Property(e => e.Feld_10_464).HasComment("Indicateur de variante 1");
            entity.Property(e => e.Feld_10_465)
                .HasMaxLength(100)
                .HasComment("Indicateur de variante 1");
            entity.Property(e => e.Feld_10_466)
                .HasMaxLength(100)
                .HasComment("Lieu de déchargement issu de l’en-tête de commande");
            entity.Property(e => e.Feld_10_467).HasComment("Angle de coupe aluminium 0");
            entity.Property(e => e.Feld_10_468).HasComment("Angle de coupe aluminium 1");
            entity.Property(e => e.Feld_10_469)
                .HasMaxLength(100)
                .HasComment("Cotes de remplissage – structure verticale");
            entity.Property(e => e.Feld_10_470)
                .HasMaxLength(100)
                .HasComment("Numéro de pièce par barre (doit être défini en machine)");
            entity.Property(e => e.Feld_10_471)
                .HasMaxLength(100)
                .HasComment("Information de paire (doit être définie en machine)");
            entity.Property(e => e.Feld_10_472)
                .HasMaxLength(100)
                .HasComment("Pièce aluminium issue de la pièce PVC");
            entity.Property(e => e.Feld_10_473)
                .HasMaxLength(100)
                .HasComment("Code acier {M_StahlCode}");
            entity.Property(e => e.Feld_10_474)
                .HasMaxLength(100)
                .HasComment("Désignation acier {M_StahlBez}");
            entity.Property(e => e.Feld_10_475)
                .HasMaxLength(100)
                .HasComment("Numéro de chambre acier {M_StahlKNr}");
            entity.Property(e => e.Feld_10_476)
                .HasMaxLength(100)
                .HasComment("Désignation de chambre acier {M_StahlKBez}");
            entity.Property(e => e.Feld_10_477)
                .HasMaxLength(100)
                .HasComment("Angle de coupe acier 1 {M_StahlKW1}");
            entity.Property(e => e.Feld_10_478)
                .HasMaxLength(100)
                .HasComment("Angle de coupe acier 2 {M_StahlKW2}");
            entity.Property(e => e.Feld_10_479)
                .HasMaxLength(100)
                .HasComment("Montage acier {M_StahlKMont}");
            entity.Property(e => e.Feld_10_480)
                .HasComment("Longueur de feuillure intérieure uniquement pour la pièce propre")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_481).HasComment("Perte de matière à la soudure - face en bout 1");
            entity.Property(e => e.Feld_10_482).HasComment("Perte de matière à la soudure - face en bout 2");
            entity.Property(e => e.Feld_10_483)
                .HasComment("Cote de l’arête intérieure")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_484).HasComment("Pièce mortaisée avec encoche sur la pièce");
            entity.Property(e => e.Feld_10_485)
                .HasMaxLength(100)
                .HasComment("Indicateur 1 issu du profil");
            entity.Property(e => e.Feld_10_486)
                .HasMaxLength(100)
                .HasComment("Indicateur 2 issu du profil");
            entity.Property(e => e.Feld_10_487).HasComment("Indicateur 3 issu du profil");
            entity.Property(e => e.Feld_10_488)
                .HasMaxLength(100)
                .HasComment("Indicateur 4 issu du profil");
            entity.Property(e => e.Feld_10_489)
                .HasMaxLength(100)
                .HasComment("Couleur du joint issue des données de position");
            entity.Property(e => e.Feld_10_490)
                .HasMaxLength(100)
                .HasComment("Espace réservé pour information machine");
            entity.Property(e => e.Feld_10_491)
                .HasMaxLength(1000)
                .HasComment("Éléments intérieurs verticaux");
            entity.Property(e => e.Feld_10_492)
                .HasMaxLength(100)
                .HasComment("Éléments intérieurs horizontaux");
            entity.Property(e => e.Feld_10_493).HasComment("Cote résiduelle extérieure");
            entity.Property(e => e.Feld_10_494)
                .HasMaxLength(100)
                .HasComment("Perçages de seuil");
            entity.Property(e => e.Feld_10_495)
                .HasMaxLength(100)
                .HasComment("Indicateur cadre/ouvrant 0 (A)");
            entity.Property(e => e.Feld_10_496)
                .HasMaxLength(100)
                .HasComment("Indicateur cadre/ouvrant 1 (B)");
            entity.Property(e => e.Feld_10_497)
                .HasMaxLength(100)
                .HasComment("Indicateur cadre/ouvrant 2 (C)");
            entity.Property(e => e.Feld_10_498)
                .HasMaxLength(100)
                .HasComment("Indicateur cadre/ouvrant 3 (D)");
            entity.Property(e => e.Feld_10_499)
                .HasComment("Longueur intérieure côté extérieur pour pièce intérieure (Emmegi Hefesta)")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_500)
                .HasComment("Longueur d’arête côté intérieur")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_501)
                .HasComment("Longueur d’arête côté extérieur")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_502)
                .HasMaxLength(100)
                .HasComment("Division au niveau du petit-bois");
            entity.Property(e => e.Feld_10_503)
                .HasMaxLength(100)
                .HasComment("Remplissage - tracé de structure calculé");
            entity.Property(e => e.Feld_10_504).HasComment("Indicateur lu depuis la classe de position");
            entity.Property(e => e.Feld_10_507)
                .HasMaxLength(100)
                .HasComment("Indicateur de résolution de ferrure 2");
            entity.Property(e => e.Feld_10_508)
                .HasMaxLength(100)
                .HasComment("Indicateur de résolution de ferrure 3");
            entity.Property(e => e.Feld_10_509)
                .HasMaxLength(100)
                .HasComment("Indicateur de résolution de ferrure 4");
            entity.Property(e => e.Feld_10_510)
                .HasMaxLength(100)
                .HasComment("Cote de pose, élément adjacent - bout 0");
            entity.Property(e => e.Feld_10_511)
                .HasMaxLength(100)
                .HasComment("Cote de pose, élément adjacent - bout 1");
            entity.Property(e => e.Feld_10_512).HasComment("Vitrage le plus épais");
            entity.Property(e => e.Feld_10_513)
                .HasMaxLength(100)
                .HasComment("Identifiant Look3E pour le Chassis");
            entity.Property(e => e.Feld_10_514).HasComment("Non défini");
            entity.Property(e => e.Feld_10_515)
                .HasMaxLength(100)
                .HasComment("Indicateur d’extrémité issu de la table");
            entity.Property(e => e.Feld_10_516)
                .HasMaxLength(100)
                .HasComment("Indicateur d’extrémité issu de la table");
            entity.Property(e => e.Feld_10_517)
                .HasMaxLength(100)
                .HasComment("Longueur acier {Stahl.Laenge} (RQN 084076)");
            entity.Property(e => e.Feld_10_518)
                .HasMaxLength(100)
                .HasComment("Code de règle spéciale (appliqué sur le petit-bois)");
            entity.Property(e => e.Feld_10_519)
                .HasMaxLength(100)
                .HasComment("Code de règle spéciale adjacente (appliqué sur l’ouvrant lié à la première traverse soudée)");
            entity.Property(e => e.Feld_10_520)
                .HasMaxLength(100)
                .HasComment("Texte de règle spéciale adjacente (appliqué sur l’ouvrant ; texte fixe déclenché par l’entrée 1, valeur « Soude »)");
            entity.Property(e => e.Feld_10_521)
                .HasMaxLength(100)
                .HasComment("Code Z via identifiant d’impression");
            entity.Property(e => e.Feld_10_522)
                .HasMaxLength(100)
                .HasComment("Usinages issus des éléments de volet roulant pour Graule");
            entity.Property(e => e.Feld_10_523)
                .HasMaxLength(100)
                .HasComment("Code-barres du cadre associé (code-barres du cadre supérieur)");
            entity.Property(e => e.Feld_10_524)
                .HasMaxLength(100)
                .HasComment("Code-barres de l’ouvrant associé (code-barres de l’ouvrant inférieur)");
            entity.Property(e => e.Feld_10_525)
                .HasMaxLength(100)
                .HasComment("Code du profil perforé pour le tablier");
            entity.Property(e => e.Feld_10_526).HasComment("Quantité de profils perforés pour le tablier");
            entity.Property(e => e.Feld_10_527)
                .HasMaxLength(100)
                .HasComment("Code du profil non perforé pour le tablier");
            entity.Property(e => e.Feld_10_528).HasComment("Quantité de profils non perforés pour le tablier");
            entity.Property(e => e.Feld_10_529).HasComment("Tablier verrouillé");
            entity.Property(e => e.Feld_10_530).HasComment("Quantité de profils non perforés / non perforés pour le tablier");
            entity.Property(e => e.Feld_10_531)
                .HasMaxLength(100)
                .HasComment("Numéro de document");
            entity.Property(e => e.Feld_10_532)
                .HasMaxLength(100)
                .HasComment("Cotes supplémentaires issues des accessoires");
            entity.Property(e => e.Feld_10_533)
                .HasMaxLength(100)
                .HasComment("Indicateur issu de la table E");
            entity.Property(e => e.Feld_10_534)
                .HasMaxLength(100)
                .HasComment("Division du tablier 1 (Stmeseder)");
            entity.Property(e => e.Feld_10_535)
                .HasMaxLength(100)
                .HasComment("Division du tablier 2 (Stmeseder)");
            entity.Property(e => e.Feld_10_536)
                .HasMaxLength(100)
                .HasComment("Division du tablier 3 (Stmeseder)");
            entity.Property(e => e.Feld_10_537)
                .HasMaxLength(100)
                .HasComment("Division du tablier 4 (Stmeseder)");
            entity.Property(e => e.Feld_10_538)
                .HasMaxLength(100)
                .HasComment("Division du tablier 5 (Stmeseder)");
            entity.Property(e => e.Feld_10_539)
                .HasComment("Découpe Dimension fond de feuillure")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_540)
                .HasMaxLength(100)
                .HasComment("Réserve Stmeseder");
            entity.Property(e => e.Feld_10_541).HasComment("Index de montage");
            entity.Property(e => e.Feld_10_542)
                .HasMaxLength(100)
                .HasComment("Désignation de montage");
            entity.Property(e => e.Feld_10_543)
                .HasMaxLength(100)
                .HasComment("Identifiant de document suivant");
            entity.Property(e => e.Feld_10_544)
                .HasMaxLength(100)
                .HasComment("Date Production Fin");
            entity.Property(e => e.Feld_10_545).HasComment("Jour de la semaine de la date de fin de production");
            entity.Property(e => e.Feld_10_546).HasComment("Allongement de la pièce à l’avant en position NC");
            entity.Property(e => e.Feld_10_547).HasComment("Allongement de la pièce à l’arrière en position NC");
            entity.Property(e => e.Feld_10_548)
                .HasMaxLength(100)
                .HasComment("Entraînement de tablier 1 (Stmeseder)");
            entity.Property(e => e.Feld_10_549)
                .HasMaxLength(100)
                .HasComment("Sortie de l’entraînement de tablier 1 (Stmeseder)");
            entity.Property(e => e.Feld_10_550)
                .HasMaxLength(100)
                .HasComment("Réserve Stmeseder");
            entity.Property(e => e.Feld_10_551)
                .HasMaxLength(100)
                .HasComment("Sortie maximale de l’entraînement de tablier (Stmeseder)");
            entity.Property(e => e.Feld_10_552)
                .HasMaxLength(100)
                .HasComment("Entraînement maximal du tablier (Stmeseder)");
            entity.Property(e => e.Feld_10_553)
                .HasMaxLength(100)
                .HasComment("Données d’entraînement (Stmeseder)");
            entity.Property(e => e.Feld_10_554)
                .HasMaxLength(100)
                .HasComment("Données du tablier (Stmeseder)");
            entity.Property(e => e.Feld_10_555)
                .HasMaxLength(100)
                .HasComment("Bois de calage (Stmeseder)");
            entity.Property(e => e.Feld_10_556)
                .HasMaxLength(100)
                .HasComment("Variante de tablier");
            entity.Property(e => e.Feld_10_557)
                .HasMaxLength(100)
                .HasComment("Type d’entraînement du tablier");
            entity.Property(e => e.Feld_10_558)
                .HasMaxLength(100)
                .HasComment("Sortie du tablier");
            entity.Property(e => e.Feld_10_559)
                .HasMaxLength(100)
                .HasComment("Mode de commande du tablier");
            entity.Property(e => e.Feld_10_560)
                .HasMaxLength(100)
                .HasComment("Code vitrage");
            entity.Property(e => e.Feld_10_561)
                .HasMaxLength(100)
                .HasComment("Découpe (Oui / Non)");
            entity.Property(e => e.Feld_10_562)
                .HasMaxLength(100)
                .HasComment("Information de découpe / encoche");
            entity.Property(e => e.Feld_10_563)
                .HasMaxLength(100)
                .HasComment("Code de variante de joint");
            entity.Property(e => e.Feld_10_564)
                .HasMaxLength(100)
                .HasComment("Texte de variante de joint");
            entity.Property(e => e.Feld_10_565)
                .HasComment("Longueur du profil incluant FOD")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Feld_10_566)
                .HasMaxLength(100)
                .HasComment("Indicateur de variante d’article 1");
            entity.Property(e => e.Feld_10_567)
                .HasMaxLength(100)
                .HasComment("Indicateur de variante d’article 2");
            entity.Property(e => e.Feld_10_568)
                .HasMaxLength(100)
                .HasComment("Indicateur de variante d’article 3");
            entity.Property(e => e.Feld_10_569)
                .HasMaxLength(100)
                .HasComment("Indicateur de variante d’article 4");
            entity.Property(e => e.Feld_10_570)
                .HasMaxLength(100)
                .HasComment("Indicateur issu de la table A (converti)");
            entity.Property(e => e.Feld_10_571)
                .HasMaxLength(100)
                .HasComment("Indicateur issu de la table B (converti)");
            entity.Property(e => e.Feld_10_572)
                .HasMaxLength(100)
                .HasComment("Indicateur issu de la table C (converti)");
            entity.Property(e => e.Feld_10_573)
                .HasMaxLength(100)
                .HasComment("Indicateur issu de la table D (converti)");
            entity.Property(e => e.Feld_10_574)
                .HasMaxLength(100)
                .HasComment("Indicateur issu de la table E (converti)");
            entity.Property(e => e.Feld_10_575)
                .HasMaxLength(100)
                .HasComment("Article d’achat");
            entity.Property(e => e.Feld_10_576)
                .HasMaxLength(100)
                .HasComment("Variante d’achat");
            entity.Property(e => e.Feld_10_577)
                .HasMaxLength(100)
                .HasComment("Référence article");
            entity.Property(e => e.Feld_10_578).HasComment("Compteur de pièces de fenêtre");
            entity.Property(e => e.Feld_10_579)
                .HasMaxLength(100)
                .HasComment("Image bitmap");
            entity.Property(e => e.Feld_10_580)
                .HasMaxLength(100)
                .HasComment("Code bitmap pour la face d’appui des accessoires");
            entity.Property(e => e.Feld_10_581)
                .HasMaxLength(100)
                .HasComment("Code fabricant issu de l’article P");
            entity.Property(e => e.Feld_10_582)
                .HasMaxLength(100)
                .HasComment("Numéro fabricant issu de l’article P");
            entity.Property(e => e.Feld_10_583).HasComment("Longueur de renfort 1 sans trame");
            entity.Property(e => e.Feld_10_584).HasComment("Longueur de renfort 2 sans trame");
            entity.Property(e => e.Feld_10_585)
                .HasMaxLength(100)
                .HasComment("Couleur d’affichage intérieure");
            entity.Property(e => e.Feld_10_586)
                .HasMaxLength(100)
                .HasComment("Couleur d’affichage extérieure");
            entity.Property(e => e.Feld_10_587)
                .HasMaxLength(100)
                .HasComment("Chemin de l’image par position");
            entity.Property(e => e.Feld_10_588)
                .HasMaxLength(100)
                .HasComment("Nom du fichier image par position");
            entity.Property(e => e.Feld_10_590).HasMaxLength(100);
            entity.Property(e => e.Feld_10_591).HasMaxLength(100);
            entity.Property(e => e.Feld_10_592).HasMaxLength(100);
            entity.Property(e => e.Feld_10_593).HasMaxLength(100);
            entity.Property(e => e.Feld_10_594).HasMaxLength(100);
            entity.Property(e => e.Feld_10_595).HasMaxLength(100);
            entity.Property(e => e.Feld_10_596).HasMaxLength(100);
            entity.Property(e => e.Feld_10_598).HasMaxLength(100);
            entity.Property(e => e.Feld_10_599).HasMaxLength(100);
            entity.Property(e => e.Feld_10_600).HasMaxLength(100);
            entity.Property(e => e.Feld_10_601).HasMaxLength(100);
            entity.Property(e => e.Feld_10_603).HasMaxLength(100);
            entity.Property(e => e.Feld_13).HasMaxLength(200);
            entity.Property(e => e.Feld_16).HasMaxLength(100);
            entity.Property(e => e.Feld_18).HasMaxLength(100);
            entity.Property(e => e.Feld_19).HasMaxLength(100);
            entity.Property(e => e.Feld_2).HasMaxLength(100);
            entity.Property(e => e.Feld_23).HasMaxLength(100);
            entity.Property(e => e.Feld_25).HasMaxLength(100);
            entity.Property(e => e.Feld_27).HasMaxLength(100);
            entity.Property(e => e.Feld_28).HasMaxLength(100);
            entity.Property(e => e.Feld_29).HasMaxLength(100);
            entity.Property(e => e.Feld_3).HasMaxLength(100);
            entity.Property(e => e.Feld_30).HasMaxLength(100);
            entity.Property(e => e.Feld_31).HasMaxLength(100);
            entity.Property(e => e.Feld_32).HasMaxLength(100);
            entity.Property(e => e.Feld_33).HasMaxLength(100);
            entity.Property(e => e.Feld_34).HasMaxLength(100);
            entity.Property(e => e.Feld_36).HasMaxLength(100);
            entity.Property(e => e.Feld_38).HasMaxLength(100);
            entity.Property(e => e.Feld_39).HasMaxLength(100);
            entity.Property(e => e.Feld_4).HasMaxLength(100);
            entity.Property(e => e.Feld_40).HasMaxLength(100);
            entity.Property(e => e.Feld_41).HasMaxLength(100);
            entity.Property(e => e.Feld_42).HasMaxLength(100);
            entity.Property(e => e.Feld_43).HasMaxLength(100);
            entity.Property(e => e.Feld_44).HasMaxLength(100);
            entity.Property(e => e.Feld_46).HasMaxLength(100);
            entity.Property(e => e.Feld_5).HasMaxLength(100);
            entity.Property(e => e.Feld_6).HasMaxLength(100);
            entity.Property(e => e.Feld_7).HasMaxLength(100);
            entity.Property(e => e.Feld_8).HasMaxLength(100);
            entity.Property(e => e.Feld_9).HasMaxLength(100);
            entity.Property(e => e.Memo1).HasMaxLength(100);
            entity.Property(e => e.Memo2).HasMaxLength(100);
            entity.Property(e => e.Memo3).HasMaxLength(100);
            entity.Property(e => e.Memo4).HasMaxLength(100);
            entity.Property(e => e.OberflNat).HasMaxLength(100);
            entity.Property(e => e.Oberfl_INat).HasMaxLength(100);
            entity.Property(e => e.Reihenfolge1).HasMaxLength(100);
            entity.Property(e => e.Reihenfolge2).HasMaxLength(100);
            entity.Property(e => e.Sortier1).HasMaxLength(100);
            entity.Property(e => e.Sortier2).HasMaxLength(100);
            entity.Property(e => e.Sortier3).HasMaxLength(100);
            entity.Property(e => e.TeilSerienBlock).HasMaxLength(100);
            entity.Property(e => e.VarianteNat).HasMaxLength(100);
            entity.Property(e => e.Wert_41).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Wert_42).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Wert_6).HasColumnType("decimal(5, 1)");
        });

        modelBuilder.Entity<UserApp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User");

            entity.ToTable("UserApp", tb => tb.HasComment("Table des utilisateurs des systèmes digitaux. Contient les informations d’identité, de contact, d’emploi, de gestion et de sécurité."));

            entity.HasIndex(e => e.LastName, "IX_User_LastName");

            entity.HasIndex(e => e.Login, "UQ_User_Login").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique de l’utilisateur.");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasComment("Adresse postale professionnelle de l’utilisateur.");
            entity.Property(e => e.Birthday)
                .HasDefaultValue(new DateOnly(1900, 1, 1))
                .HasComment("Date de naissance de l’utilisateur.");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasComment("Ville de l’adresse professionnelle.");
            entity.Property(e => e.CompanyId).HasComment("Identifiant de la société employant l’utilisateur.");
            entity.Property(e => e.ContractType).HasComment("Type de contrat de l’utilisateur (CDD, CDI, intérim…).");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasComment("Pays de l’adresse professionnelle.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.EmailPersonal)
                .HasMaxLength(200)
                .HasDefaultValue("someone@something.com")
                .HasComment("Adresse e-mail personnelle de l’utilisateur.");
            entity.Property(e => e.EmailProfessional)
                .HasMaxLength(200)
                .HasComment("Adresse e-mail professionnelle de l’utilisateur.");
            entity.Property(e => e.EntryDate)
                .HasDefaultValue(new DateOnly(1900, 1, 1))
                .HasComment("Date d’entrée de l’utilisateur dans l’entreprise.");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasComment("Prénom de l’utilisateur.");
            entity.Property(e => e.Initials)
                .HasMaxLength(5)
                .HasComment("Initiales de l’utilisateur.");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasComment("Indique si le compte utilisateur est actif dans les systèmes digitaux.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est supprimé logiquement (soft delete).");
            entity.Property(e => e.IsResetRequired).HasComment("Indique si l’utilisateur doit réinitialiser son mot de passe lors de sa prochaine connexion.");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasComment("Nom de famille de l’utilisateur.");
            entity.Property(e => e.Login)
                .HasMaxLength(100)
                .HasComment("Identifiant de connexion unique de l’utilisateur.");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(200)
                .HasComment("Mot de passe hashé de l’utilisateur. Ne doit jamais contenir le mot de passe en clair.");
            entity.Property(e => e.PhonePersonal)
                .HasMaxLength(14)
                .HasDefaultValue("00 00 00 00 00")
                .HasComment("Téléphone personnel de l’utilisateur.");
            entity.Property(e => e.PhonePro)
                .HasMaxLength(14)
                .HasComment("Téléphone professionnel de l’utilisateur.");
            entity.Property(e => e.PhoneProFixed)
                .HasMaxLength(14)
                .HasComment("Téléphone fixe professionnel de l’utilisateur.");
            entity.Property(e => e.PleiadeNumber)
                .HasMaxLength(100)
                .HasComment("Matricule de l’utilisateur dans le système RH Pléiade.");
            entity.Property(e => e.PostalCode).HasComment("Code postal de l’adresse professionnelle.");
            entity.Property(e => e.ProductivityRate)
                .HasDefaultValue((short)100)
                .HasComment("Pourcentage de productivité utilisé pour les calculs internes.");
            entity.Property(e => e.SectorId)
                .HasDefaultValue(0)
                .HasComment("Identifiant du secteur d’activité auquel appartient l’utilisateur.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de l’enregistrement.");
            entity.Property(e => e.WindowsLogin)
                .HasMaxLength(20)
                .HasComment("Login Windows permettant l’authentification AD.");
        });

        modelBuilder.Entity<UserAppAccess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserApplication");

            entity.ToTable("UserAppAccess", tb => tb.HasComment("Associe un utilisateur à une application, avec un niveau d’accès configurable. Gère les autorisations globales par application."));

            entity.HasIndex(e => new { e.IdUser, e.IdApplication }, "IX_UserApplication_User_App");

            entity.HasIndex(e => new { e.IdUser, e.IdApplication }, "UQ_UserApplication_User_Application").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique de l’enregistrement (clé primaire).");
            entity.Property(e => e.AccessLevel)
                .HasDefaultValue((short)1)
                .HasComment("Niveau d’accès de l’utilisateur à l’application (1 = accès standard, valeurs supérieures = droits étendus selon la politique interne).");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure de création de l’enregistrement.");
            entity.Property(e => e.IdApplication).HasComment("Référence vers l’application à laquelle l’utilisateur est associé.");
            entity.Property(e => e.IdUser).HasComment("Référence vers l’utilisateur auquel les droits applicatifs sont attribués.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est supprimé logiquement (1 = supprimé). Permet une gestion d’historisation sans suppression physique.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière modification de l’enregistrement (mise à jour automatique via EF Core).");

            entity.HasOne(d => d.IdApplicationNavigation).WithMany(p => p.UserAppAccesses)
                .HasForeignKey(d => d.IdApplication)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserApplication_Application");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserAppAccesses)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserApplication_User");
        });

        modelBuilder.Entity<UserAppErrorLog>(entity =>
        {
            entity.ToTable("UserAppErrorLog", tb => tb.HasComment("Journal des erreurs applicatives générées par les services et applications du système. Stocke les messages, la chaîne d'appel, les informations techniques."));

            entity.HasIndex(e => new { e.IdApplication, e.ErrorTimestamp }, "IX_UserAppErrorLog_App_Timestamp");

            entity.Property(e => e.Id).HasComment("Identifiant unique du journal d'erreur.");
            entity.Property(e => e.CallChain)
                .HasMaxLength(1000)
                .HasComment("Chaîne complète des appels (services, use cases, handlers) ayant conduit à l'erreur.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de l'enregistrement dans le journal d'erreur.");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(30)
                .HasComment("Identifiant unique de l'appareil (PC, terminal, machine).");
            entity.Property(e => e.DeviceIp)
                .HasMaxLength(30)
                .HasComment("Adresse IP de l'appareil ayant généré l'erreur.");
            entity.Property(e => e.DeviceUser)
                .HasMaxLength(30)
                .HasComment("Nom utilisateur sur l'appareil local où l'erreur s'est produite.");
            entity.Property(e => e.ErrorCode)
                .HasMaxLength(1000)
                .HasComment("Code d'erreur fonctionnel ou technique associé à l'incident.");
            entity.Property(e => e.ErrorException)
                .HasMaxLength(4000)
                .HasComment("Détails complets de l'exception (.NET stacktrace ou message interne).");
            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(1000)
                .HasComment("Message d'erreur lisible par l'utilisateur ou le développeur.");
            entity.Property(e => e.ErrorTimestamp)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure où l'erreur a été enregistrée (horodatage système).");
            entity.Property(e => e.IdApplication).HasComment("Référence vers l'application source de l'erreur (FK vers AppList).");
            entity.Property(e => e.IdUser).HasComment("Utilisateur connecté lors de l'erreur (FK vers User). Peut être NULL si l'erreur survient avant authentification.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l'enregistrement est marqué comme supprimé (soft delete).");
            entity.Property(e => e.UpdatedAt).HasComment("Date de la dernière mise à jour de l'enregistrement.");

            entity.HasOne(d => d.IdApplicationNavigation).WithMany(p => p.UserAppErrorLogs)
                .HasForeignKey(d => d.IdApplication)
                .HasConstraintName("FK_UserAppErrorLog_Application");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserAppErrorLogs)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_UserAppErrorLog_User");
        });

        modelBuilder.Entity<UserAppEventStore>(entity =>
        {
            entity.ToTable("UserAppEventStore", tb => tb.HasComment("Table technique d’Event Store : historisation automatique des écritures en base sous forme de snapshots JSON. Chaque enregistrement capture l’état d’une entité persistée (TableDesignation/TableId/Data) et le contexte applicatif (AppId, AppUserId, Device*), avec traçabilité par callChain complète (AppCallChain)."));

            entity.HasIndex(e => e.AppId, "IX_UserAppEventStore_AppId");

            entity.HasIndex(e => e.AppUserId, "IX_UserAppEventStore_AppUserId");

            entity.HasIndex(e => e.Timestamp, "IX_UserAppEventStore_Timestamp");

            entity.Property(e => e.Id).HasComment("Clé primaire technique (IDENTITY) de l’enregistrement Event Store.");
            entity.Property(e => e.AppCallChain)
                .HasMaxLength(1000)
                .HasComment("Chaîne d’appel complète (callChain), permettant la traçabilité bout-en-bout de l’action.");
            entity.Property(e => e.AppId).HasComment("Identifiant de l’application à l’origine de l’écriture (FK vers dbo.AppList).");
            entity.Property(e => e.AppUserId).HasComment("Utilisateur applicatif à l’origine de l’écriture (FK vers dbo.UserApp).");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création technique de l’enregistrement (sysdatetime()).");
            entity.Property(e => e.Data).HasComment("Snapshot JSON de l’entité (état sérialisé) tel qu’enregistré lors de l’opération d’écriture.");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(30)
                .HasComment("Identifiant du poste/terminal (contexte device) ayant déclenché l’écriture.");
            entity.Property(e => e.DeviceIp)
                .HasMaxLength(30)
                .HasComment("Adresse IP du poste/terminal (contexte device) ayant déclenché l’écriture.");
            entity.Property(e => e.DeviceUser)
                .HasMaxLength(30)
                .HasComment("Identifiant utilisateur côté poste/terminal (contexte device) ayant déclenché l’écriture.");
            entity.Property(e => e.IsDeleted).HasComment("Flag de suppression logique standard Projet 104 (0 = actif, 1 = supprimé). En pratique l’Event Store reste généralement non supprimé.");
            entity.Property(e => e.TableDesignation)
                .HasMaxLength(100)
                .HasComment("Désignation de la table/entité concernée (généralement typeof(T).Name côté application).");
            entity.Property(e => e.TableId).HasComment("Identifiant de l’enregistrement de la table/entité concernée. Renseigné après persistance (Id > 0).");
            entity.Property(e => e.Timestamp)
                .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .HasComment("Horodatage applicatif de l’événement (DTO_AppContext.AppDateTime) permettant de tracer l’instant logique de l’écriture.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour technique de l’enregistrement (NULL si jamais mis à jour).");

            entity.HasOne(d => d.App).WithMany(p => p.UserAppEventStores)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_UserAppEventStore_AppList");

            entity.HasOne(d => d.AppUser).WithMany(p => p.UserAppEventStores)
                .HasForeignKey(d => d.AppUserId)
                .HasConstraintName("FK_UserAppEventStore_User");
        });

        modelBuilder.Entity<UserAppMessage>(entity =>
        {
            entity.ToTable("UserAppMessage", tb => tb.HasComment("Représente les messages échangés entre applications et utilisateurs."));

            entity.HasIndex(e => e.IdApplicationRecipient, "IX_UserAppMessage_ApplicationRecipient");

            entity.HasIndex(e => e.IdApplicationSender, "IX_UserAppMessage_ApplicationSender");

            entity.HasIndex(e => e.IdUserSender, "IX_UserAppMessage_UserSender");

            entity.Property(e => e.Id).HasComment("Identifiant unique du message.");
            entity.Property(e => e.Content)
                .HasMaxLength(2000)
                .HasComment("Contenu textuel du message. Peut inclure des détails techniques, des instructions ou des informations contextuelles.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure de création de l’enregistrement (métadonnée technique).");
            entity.Property(e => e.IdApplicationRecipient).HasComment("Identifiant de l’application destinataire du message. Référence AppList(Id).");
            entity.Property(e => e.IdApplicationSender).HasComment("Identifiant de l’application à l’origine du message. Référence AppList(Id).");
            entity.Property(e => e.IdUserSender).HasComment("Identifiant de l’utilisateur ayant envoyé le message. Référence User(Id).");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement a été marqué comme supprimé sans suppression physique. 0 = actif, 1 = supprimé.");
            entity.Property(e => e.IsRead).HasComment("Indique si le message a été marqué comme lu par le destinataire. 0 = non lu, 1 = lu.");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date et heure d’envoi du message. Correspond également à la date d’émission.");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasComment("Sujet du message. Court texte descriptif utilisé comme titre de notification ou d’alerte.");
            entity.Property(e => e.UpdatedAt).HasComment("Date et heure de dernière mise à jour de l’enregistrement. Null tant qu’aucune modification n’est effectuée.");

            entity.HasOne(d => d.IdApplicationRecipientNavigation).WithMany(p => p.UserAppMessageIdApplicationRecipientNavigations)
                .HasForeignKey(d => d.IdApplicationRecipient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppMessage_ApplicationRecipient");

            entity.HasOne(d => d.IdApplicationSenderNavigation).WithMany(p => p.UserAppMessageIdApplicationSenderNavigations)
                .HasForeignKey(d => d.IdApplicationSender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppMessage_ApplicationSender");

            entity.HasOne(d => d.IdUserSenderNavigation).WithMany(p => p.UserAppMessages)
                .HasForeignKey(d => d.IdUserSender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppMessage_UserSender");
        });

        modelBuilder.Entity<UserAppPage>(entity =>
        {
            entity.ToTable("UserAppPage", tb => tb.HasComment("Référence les pages applicatives disponibles dans une application. Sert de base à la gestion des droits d’accès par page."));

            entity.HasIndex(e => e.PageCode, "UQ_UserAppPage_PageCode").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique de la page (clé primaire). Reprend l’ID d’origine issu de la base source.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date et heure de création de l’enregistrement. Gérée automatiquement par SQL Server.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement a été supprimé logiquement (1 = supprimé). Permet un archivage sans suppression réelle.");
            entity.Property(e => e.PageCode)
                .HasMaxLength(10)
                .HasComment("Code unique identifiant fonctionnellement une page applicative (exemple : P10_HOME, P20_STOCK).");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de la ligne. Mise à jour automatiquement via EF Core lors des modifications.");
        });

        modelBuilder.Entity<UserAppPageRight>(entity =>
        {
            entity.ToTable("UserAppPageRight", tb => tb.HasComment("Table enregistrant les droits d’’accès par utilisateur, application et page."));

            entity.HasIndex(e => new { e.IdUser, e.IdApplication, e.PageCode }, "UQ_UserAppPageRight").IsUnique();

            entity.Property(e => e.Id).HasComment("Identifiant unique interne.");
            entity.Property(e => e.CanAccess).HasComment("L’utilisateur peut accéder à la page.");
            entity.Property(e => e.CanAdmin).HasComment("L’utilisateur dispose des droits administratifs.");
            entity.Property(e => e.CanControl).HasComment("L’utilisateur peut effectuer des actions de contrôle.");
            entity.Property(e => e.CanCreate).HasComment("L’utilisateur peut créer des données.");
            entity.Property(e => e.CanDelete).HasComment("L’utilisateur peut supprimer des données.");
            entity.Property(e => e.CanMonitor).HasComment("L’utilisateur peut suivre en temps réel.");
            entity.Property(e => e.CanRead).HasComment("L’utilisateur peut lire les données.");
            entity.Property(e => e.CanSupervise).HasComment("L’utilisateur peut superviser les opérations.");
            entity.Property(e => e.CanUpdate).HasComment("L’utilisateur peut modifier les données.");
            entity.Property(e => e.CanValidate).HasComment("L’utilisateur peut valider des actions.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de la ligne.");
            entity.Property(e => e.IdApplication).HasComment("Identifiant de l’application cible.");
            entity.Property(e => e.IdUser).HasComment("Identifiant de l’utilisateur concerné.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la ligne est supprimée logiquement.");
            entity.Property(e => e.PageCode)
                .HasMaxLength(10)
                .HasComment("Code fonctionnel de la page. Exception du projet : utilisé comme clé étrangère plutôt que l’Id technique.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour.");

            entity.HasOne(d => d.IdApplicationNavigation).WithMany(p => p.UserAppPageRights)
                .HasForeignKey(d => d.IdApplication)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppPageRight_Application");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserAppPageRights)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppPageRight_User");

            entity.HasOne(d => d.PageCodeNavigation).WithMany(p => p.UserAppPageRights)
                .HasPrincipalKey(p => p.PageCode)
                .HasForeignKey(d => d.PageCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppPageRight_PageCode");
        });

        modelBuilder.Entity<UserAppSession>(entity =>
        {
            entity.ToTable("UserAppSession", tb => tb.HasComment("Table enregistrant les sessions utilisateur pour les applications internes."));

            entity.HasIndex(e => e.IdApplication, "IX_UserAppSession_IdApplication");

            entity.HasIndex(e => e.IdUser, "IX_UserAppSession_IdUser");

            entity.HasIndex(e => e.IsConnected, "IX_UserAppSession_IsConnected");

            entity.Property(e => e.Id).HasComment("Identifiant unique de la session.");
            entity.Property(e => e.ConnectionDate)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date et heure de connexion de la session.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasComment("Date de création de l’enregistrement.");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(30)
                .HasComment("Identifiant unique de l’appareil (machine, terminal).");
            entity.Property(e => e.DeviceIp)
                .HasMaxLength(30)
                .HasComment("Adresse IP de l’appareil client.");
            entity.Property(e => e.DeviceUser)
                .HasMaxLength(30)
                .HasComment("Nom d’utilisateur de l’appareil client.");
            entity.Property(e => e.DisconnectionDate).HasComment("Date et heure de déconnexion de la session (NULL si toujours connectée).");
            entity.Property(e => e.IdApplication).HasComment("Clé étrangère vers AppList. Identifie l’application concernée par la session.");
            entity.Property(e => e.IdUser).HasComment("Clé étrangère vers User. Identifie l’utilisateur connecté.");
            entity.Property(e => e.IsConnected).HasComment("État actuel de la session (1 = connectée, 0 = déconnectée).");
            entity.Property(e => e.IsDeleted).HasComment("Indique si la session est supprimée logiquement (soft delete).");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour de la session.");

            entity.HasOne(d => d.IdApplicationNavigation).WithMany(p => p.UserAppSessions)
                .HasForeignKey(d => d.IdApplication)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppSession_AppList");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserAppSessions)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppSession_User");
        });

        modelBuilder.Entity<UserAppSessionCommand>(entity =>
        {
            entity.ToTable("UserAppSessionCommand", tb => tb.HasComment("Représente les commandes envoyées d’une application ou d’un utilisateur vers un autre utilisateur."));

            entity.HasIndex(e => e.IdApplicationTarget, "IX_UserAppSessionCommand_IdApplicationTarget");

            entity.HasIndex(e => e.IdUserIssuer, "IX_UserAppSessionCommand_IdUserIssuer");

            entity.HasIndex(e => e.IdUserTarget, "IX_UserAppSessionCommand_IdUserTarget");

            entity.Property(e => e.Id).HasComment("Identifiant unique de la commande (clé primaire).");
            entity.Property(e => e.CommandDate)
                .HasDefaultValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .HasComment("Date et heure à laquelle la commande a été générée. Sert de base au traitement FIFO des commandes.");
            entity.Property(e => e.CommandType)
                .HasMaxLength(30)
                .HasComment("Type de commande envoyée (ex : RefreshData, LogoutUser, LockSession, NotifyWarning). Définit l’action à exécuter côté client.");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasComment("Date de création de l’enregistrement. Renseignée automatiquement par SQL Server via sysdatetime().");
            entity.Property(e => e.IdApplicationTarget).HasComment("Application destinataire de la commande (ex : BatchCutting, BatchStockRelease). Permet de router la commande vers l’application correcte.");
            entity.Property(e => e.IdUserIssuer).HasComment("Utilisateur émetteur de la commande. Référence la table User. Permet de tracer l’origine d’une commande ou action distante.");
            entity.Property(e => e.IdUserTarget).HasComment("Utilisateur destinataire de la commande. Référence la table User. Identifie celui qui reçoit l’ordre ou la notification.");
            entity.Property(e => e.IsDeleted).HasComment("Indique si l’enregistrement est supprimé logiquement. 0 = actif, 1 = supprimé. Utilisé pour éviter les suppressions physiques.");
            entity.Property(e => e.UpdatedAt).HasComment("Date de dernière mise à jour. Gérée côté EF Core ou via triggers si nécessaire.");

            entity.HasOne(d => d.IdApplicationTargetNavigation).WithMany(p => p.UserAppSessionCommands)
                .HasForeignKey(d => d.IdApplicationTarget)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppSessionCommand_AppList");

            entity.HasOne(d => d.IdUserIssuerNavigation).WithMany(p => p.UserAppSessionCommandIdUserIssuerNavigations)
                .HasForeignKey(d => d.IdUserIssuer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppSessionCommand_UserIssuer");

            entity.HasOne(d => d.IdUserTargetNavigation).WithMany(p => p.UserAppSessionCommandIdUserTargetNavigations)
                .HasForeignKey(d => d.IdUserTarget)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAppSessionCommand_UserTarget");
        });

        modelBuilder.Entity<vw_ArticleInternalDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ArticleInternalDetails");

            entity.Property(e => e.CategoryLevel1)
                .HasMaxLength(100)
                .HasComment("Catégorie article niveau 1.");
            entity.Property(e => e.CategoryLevel2)
                .HasMaxLength(100)
                .HasComment("Catégorie article niveau 2.");
            entity.Property(e => e.CategoryLevel3)
                .HasMaxLength(100)
                .HasComment("Catégorie article niveau 3.");
            entity.Property(e => e.CuttingMachineCode)
                .HasMaxLength(20)
                .HasComment("Identifiant de la machine de coupe utilisée pour traiter cet article (ex : DG244).");
            entity.Property(e => e.Designation)
                .HasMaxLength(300)
                .HasComment("Désignation commerciale de la référence article.");
            entity.Property(e => e.Id).HasComment("Identifiant unique de l’article interne.");
            entity.Property(e => e.IdColorRalFinish)
                .HasMaxLength(50)
                .HasComment("Identifiant du couple RAL + finition utilisé par l’article interne.");
            entity.Property(e => e.IdScrapLocationHorizontal).HasComment("Identifiant du casier horizontal où les chutes sont rangées.");
            entity.Property(e => e.IdScrapLocationVertical).HasComment("Identifiant du casier vertical où les chutes sont rangées.");
            entity.Property(e => e.IdentificationType)
                .HasMaxLength(100)
                .HasComment("Type d’identification : pièce, barre, boîte, etc.");
            entity.Property(e => e.ManageScraps).HasComment("Indique si cet article doit être géré dans la logique des chutes.");
            entity.Property(e => e.MinScrapLength)
                .HasComment("Longueur minimale considérée comme une chute réutilisable.")
                .HasColumnType("decimal(23, 11)");
            entity.Property(e => e.Reference)
                .HasMaxLength(100)
                .HasComment("Référence principale de l’article (famille produit).");
            entity.Property(e => e.ScrapLocationHorizontal)
                .HasMaxLength(45)
                .HasComment("Désignation de l’emplacement horizontal où ranger les chutes.");
            entity.Property(e => e.ScrapLocationVertical)
                .HasMaxLength(45)
                .HasComment("Désignation de l’emplacement vertical où ranger les chutes.");
            entity.Property(e => e.SortOrder).HasComment("Ordre de tri conseillé pour les listes d’articles.");
            entity.Property(e => e.StorageUnitCode)
                .HasMaxLength(20)
                .HasComment("Code de l’unité de stockage (ex : ML, PC, KG…).");
            entity.Property(e => e.StorageUnitDesignation)
                .HasMaxLength(100)
                .HasComment("Désignation complète de l’unité de stockage.");
        });

        modelBuilder.Entity<vw_ProductionCutPiece_Control_Coherence>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ProductionCutPiece_Control_Coherence");

            entity.Property(e => e.ArticleInternalStatus)
                .HasMaxLength(14)
                .IsUnicode(false);
            entity.Property(e => e.ArticleReferenceCode).HasMaxLength(100);
            entity.Property(e => e.ArticleReferenceStatus)
                .HasMaxLength(14)
                .IsUnicode(false);
            entity.Property(e => e.ChassisStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ColorRalFinishId).HasMaxLength(50);
            entity.Property(e => e.ColorStatus)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.CutBarcode).HasMaxLength(100);
            entity.Property(e => e.FrameSashStatus)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.LookCutPieceId).HasMaxLength(100);
            entity.Property(e => e.ProductionChassisBarcodeId).HasMaxLength(50);
            entity.Property(e => e.SourceBarcodeId).HasMaxLength(100);
            entity.Property(e => e.SourceIdColorRalFinish).HasMaxLength(100);
            entity.Property(e => e.SourceReference).HasMaxLength(100);
            entity.Property(e => e.SourceSpatialPositionCode).HasMaxLength(100);
            entity.Property(e => e.SpatialPositionCode).HasMaxLength(50);
            entity.Property(e => e.SpatialPositionStatus)
                .HasMaxLength(19)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vw_ProductionCutPiece_Full>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ProductionCutPiece_Full");

            entity.Property(e => e.COCustomerCity).HasMaxLength(100);
            entity.Property(e => e.COCustomerCountry).HasMaxLength(50);
            entity.Property(e => e.COCustomerName).HasMaxLength(200);
            entity.Property(e => e.COCustomerProjectDesignation).HasMaxLength(200);
            entity.Property(e => e.COCustomerProjectName).HasMaxLength(200);
            entity.Property(e => e.COCustomerStreet).HasMaxLength(200);
            entity.Property(e => e.COCustomerZipCode).HasMaxLength(20);
            entity.Property(e => e.CODeliveryPosition).HasMaxLength(100);
            entity.Property(e => e.COMainSalesPoint).HasMaxLength(50);
            entity.Property(e => e.COMainSalesPointAddress).HasMaxLength(200);
            entity.Property(e => e.COMainSalesPointCode).HasMaxLength(50);
            entity.Property(e => e.COMainSalesPointName).HasMaxLength(200);
            entity.Property(e => e.COManufacturingPlant).HasMaxLength(50);
            entity.Property(e => e.COManufacturingSite).HasMaxLength(50);
            entity.Property(e => e.COOrderSponsor).HasMaxLength(200);
            entity.Property(e => e.COProductionEndTourId).HasMaxLength(50);
            entity.Property(e => e.COProjectDesignation).HasMaxLength(100);
            entity.Property(e => e.COQuaiZone).HasMaxLength(100);
            entity.Property(e => e.COSecondarySalesPointName).HasMaxLength(200);
            entity.Property(e => e.PCBarcodeId).HasMaxLength(50);
            entity.Property(e => e.PCCapacityZone).HasMaxLength(100);
            entity.Property(e => e.PCColorNameIntExt).HasMaxLength(100);
            entity.Property(e => e.PCCustomerPosition).HasMaxLength(100);
            entity.Property(e => e.PCOpeningTypeAbbreviation).HasMaxLength(100);
            entity.Property(e => e.PCOpeningTypeText).HasMaxLength(200);
            entity.Property(e => e.PCPAssemblyCode).HasMaxLength(100);
            entity.Property(e => e.PCPAssociatedArticleReferenceLeft).HasMaxLength(100);
            entity.Property(e => e.PCPAssociatedArticleReferenceRight).HasMaxLength(100);
            entity.Property(e => e.PCPBarColorCodeInOut).HasMaxLength(100);
            entity.Property(e => e.PCPBarHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PCPBarProductCodeToPrint).HasMaxLength(100);
            entity.Property(e => e.PCPBarProductFamilyName).HasMaxLength(100);
            entity.Property(e => e.PCPBarReference).HasMaxLength(100);
            entity.Property(e => e.PCPBarWidth).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PCPConnectionProfileCode).HasMaxLength(100);
            entity.Property(e => e.PCPCutBarcode).HasMaxLength(100);
            entity.Property(e => e.PCPCutDimension).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.PCPDrainageCodeUsedForCalculation).HasMaxLength(100);
            entity.Property(e => e.PCPLookChassisId).HasMaxLength(100);
            entity.Property(e => e.PCPLookCustomerOrderId).HasMaxLength(100);
            entity.Property(e => e.PCPLookCutPieceId).HasMaxLength(100);
            entity.Property(e => e.PCPMachineCode).HasMaxLength(100);
            entity.Property(e => e.PCPProfileCodeToPrint).HasMaxLength(100);
            entity.Property(e => e.PCPProfileColorCodeInOut).HasMaxLength(100);
            entity.Property(e => e.PCPProfileColorInside).HasMaxLength(100);
            entity.Property(e => e.PCPProfileColorOutside).HasMaxLength(100);
            entity.Property(e => e.PCPProfileHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PCPProfileLength).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PCPProfileLengthIncludingFOD).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PCPProfileName).HasMaxLength(100);
            entity.Property(e => e.PCPProfileNumber).HasMaxLength(100);
            entity.Property(e => e.PCPProfileNumberForMachine).HasMaxLength(100);
            entity.Property(e => e.PCPProfileWidth).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PCProductFamily).HasMaxLength(100);
            entity.Property(e => e.PCSashDimensionsLeftRight).HasMaxLength(100);
            entity.Property(e => e.PCSashPreset).HasMaxLength(100);
            entity.Property(e => e.PCSlidingType).HasMaxLength(200);
            entity.Property(e => e.PCSlidingTypeDetailed).HasMaxLength(200);
            entity.Property(e => e.PCWidthWithCorrectionAndMiterTip).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PCWindowSystemCode).HasMaxLength(100);
            entity.Property(e => e.PCWindowText).HasMaxLength(1000);
            entity.Property(e => e.PFSAdjacentFramePartToSash).HasMaxLength(100);
            entity.Property(e => e.PFSBeadSystemInnerSeal).HasMaxLength(100);
            entity.Property(e => e.PFSBeadsHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSBeadsWidth).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSCremoneType1).HasMaxLength(100);
            entity.Property(e => e.PFSDisplayColorInside).HasMaxLength(100);
            entity.Property(e => e.PFSDisplayColorOutside).HasMaxLength(100);
            entity.Property(e => e.PFSFrameSashHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSFrameSashHeightTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSFrameSashWidth).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSFrameSashWidthTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSFrameThresholdCounterProfile).HasMaxLength(100);
            entity.Property(e => e.PFSGlazingAssignment).HasMaxLength(1000);
            entity.Property(e => e.PFSGlazingBeadsPerSashFrame).HasMaxLength(100);
            entity.Property(e => e.PFSGlazingCode).HasMaxLength(100);
            entity.Property(e => e.PFSGlazingDimensions).HasMaxLength(100);
            entity.Property(e => e.PFSGlazingSealText).HasMaxLength(100);
            entity.Property(e => e.PFSGlazingText).HasMaxLength(100);
            entity.Property(e => e.PFSHandlePosition).HasMaxLength(100);
            entity.Property(e => e.PFSHardwareRabbetHeightTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSHardwareRabbetWidthTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.PFSHardwareSystemCode).HasMaxLength(100);
            entity.Property(e => e.PFSHardwareSystemText).HasMaxLength(100);
            entity.Property(e => e.PFSInnerSealSashFrame).HasMaxLength(100);
            entity.Property(e => e.PFSMechanismCode).HasMaxLength(100);
            entity.Property(e => e.PFSOpeningDirectionIndicator).HasMaxLength(100);
            entity.Property(e => e.PFSOpeningTypeText).HasMaxLength(1000);
            entity.Property(e => e.PFSPositionDataSealColor).HasMaxLength(100);
            entity.Property(e => e.PFSReinforcementCode).HasMaxLength(100);
            entity.Property(e => e.PFSSashHardwareIndicator).HasMaxLength(100);
            entity.Property(e => e.PFSSeal).HasMaxLength(100);
            entity.Property(e => e.PFSSealColor).HasMaxLength(100);
            entity.Property(e => e.PFSSealSystem).HasMaxLength(100);
            entity.Property(e => e.PFSSealVariantCode).HasMaxLength(100);
            entity.Property(e => e.PFSSealVariantText).HasMaxLength(100);
            entity.Property(e => e.PFSSpecialOpeningTypeCode).HasMaxLength(100);
            entity.Property(e => e.PSDescription).HasMaxLength(200);
            entity.Property(e => e.SP1SideIndexDescription).HasMaxLength(100);
            entity.Property(e => e.SPCode).HasMaxLength(50);
            entity.Property(e => e.SPDescription).HasMaxLength(200);
            entity.Property(e => e.SPPosition).HasMaxLength(20);
        });

        modelBuilder.Entity<vw_Source_ArticleInternal_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_ArticleInternal_Missing");

            entity.Property(e => e.ArticleReferenceCode).HasMaxLength(100);
            entity.Property(e => e.ColorCodeInOut).HasMaxLength(100);
            entity.Property(e => e.IdColorRalFinish).HasMaxLength(50);
            entity.Property(e => e.StandardBarLengthMm).HasColumnType("decimal(23, 11)");
        });

        modelBuilder.Entity<vw_Source_ArticleReference_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_ArticleReference_Missing");

            entity.Property(e => e.CodeArticle).HasMaxLength(100);
            entity.Property(e => e.CodeArticleCuttingMachine).HasMaxLength(100);
            entity.Property(e => e.CodeArticleReference).HasMaxLength(100);
            entity.Property(e => e.CodeFamily).HasMaxLength(100);
            entity.Property(e => e.Designation)
                .HasMaxLength(300)
                .HasComment("Désignation article issue de Tempor_Import.Feld_10_100.");
            entity.Property(e => e.FamilyCategory).HasMaxLength(100);
            entity.Property(e => e.Reference)
                .HasMaxLength(100)
                .HasComment("Référence article issue de Tempor_Import.Feld_10_066.");
            entity.Property(e => e.StandardBarLengthMm).HasColumnType("decimal(23, 11)");
        });

        modelBuilder.Entity<vw_Source_ColorRalFinish_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_ColorRalFinish_Missing");

            entity.Property(e => e.Id).HasMaxLength(100);
            entity.Property(e => e.IdExternalFinish).HasMaxLength(3);
            entity.Property(e => e.IdInternalFinish).HasMaxLength(3);
        });

        modelBuilder.Entity<vw_Source_CustomerOrder_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_CustomerOrder_Missing");

            entity.Property(e => e.CustomerCity).HasMaxLength(100);
            entity.Property(e => e.CustomerCountry).HasMaxLength(50);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.CustomerProjectDesignation).HasMaxLength(200);
            entity.Property(e => e.CustomerProjectName).HasMaxLength(200);
            entity.Property(e => e.CustomerStreet).HasMaxLength(200);
            entity.Property(e => e.CustomerZipCode).HasMaxLength(20);
            entity.Property(e => e.DeliveryPosition).HasMaxLength(100);
            entity.Property(e => e.LookCustomerOrderId).HasMaxLength(100);
            entity.Property(e => e.MainSalesPoint).HasMaxLength(50);
            entity.Property(e => e.MainSalesPointAddress).HasMaxLength(200);
            entity.Property(e => e.MainSalesPointCode).HasMaxLength(50);
            entity.Property(e => e.MainSalesPointName).HasMaxLength(200);
            entity.Property(e => e.ManufacturingPlant).HasMaxLength(50);
            entity.Property(e => e.ManufacturingSite).HasMaxLength(50);
            entity.Property(e => e.OrderSponsor).HasMaxLength(200);
            entity.Property(e => e.ProductionEndTourId).HasMaxLength(50);
            entity.Property(e => e.ProjectDesignation).HasMaxLength(100);
            entity.Property(e => e.QuaiZone).HasMaxLength(100);
            entity.Property(e => e.SecondarySalesPointName).HasMaxLength(200);
        });

        modelBuilder.Entity<vw_Source_ProductionChassis_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_ProductionChassis_Missing");

            entity.Property(e => e.BarcodeId).HasMaxLength(100);
            entity.Property(e => e.CapacityZone).HasMaxLength(100);
            entity.Property(e => e.ColorNameIntExt).HasMaxLength(100);
            entity.Property(e => e.CustomerPosition).HasMaxLength(100);
            entity.Property(e => e.LookChassisId).HasMaxLength(100);
            entity.Property(e => e.OpeningTypeAbbreviation).HasMaxLength(100);
            entity.Property(e => e.OpeningTypeText).HasMaxLength(200);
            entity.Property(e => e.ProductFamily).HasMaxLength(100);
            entity.Property(e => e.SashDimensionsLeftRight).HasMaxLength(100);
            entity.Property(e => e.SashPreset).HasMaxLength(100);
            entity.Property(e => e.SlidingType).HasMaxLength(200);
            entity.Property(e => e.SlidingTypeDetailed).HasMaxLength(200);
            entity.Property(e => e.WidthWithCorrectionAndMiterTip).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.WindowSystemCode).HasMaxLength(100);
            entity.Property(e => e.WindowText).HasMaxLength(1000);
        });

        modelBuilder.Entity<vw_Source_ProductionCutPiece_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_ProductionCutPiece_Missing");

            entity.Property(e => e.AssemblyCode).HasMaxLength(100);
            entity.Property(e => e.AssociatedArticleReferenceLeft).HasMaxLength(100);
            entity.Property(e => e.AssociatedArticleReferenceRight).HasMaxLength(100);
            entity.Property(e => e.BarColorCodeInOut).HasMaxLength(100);
            entity.Property(e => e.BarHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.BarProductCodeToPrint).HasMaxLength(100);
            entity.Property(e => e.BarProductFamilyName).HasMaxLength(100);
            entity.Property(e => e.BarReference).HasMaxLength(100);
            entity.Property(e => e.BarWidth).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ConnectionProfileCode).HasMaxLength(100);
            entity.Property(e => e.CutBarcode).HasMaxLength(100);
            entity.Property(e => e.CutDimension).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.DrainageCodeUsedForCalculation).HasMaxLength(100);
            entity.Property(e => e.LookChassisId).HasMaxLength(100);
            entity.Property(e => e.LookCustomerOrderId).HasMaxLength(100);
            entity.Property(e => e.LookCutPieceId).HasMaxLength(100);
            entity.Property(e => e.MachineCode).HasMaxLength(100);
            entity.Property(e => e.ProfileCodeToPrint).HasMaxLength(100);
            entity.Property(e => e.ProfileColorCodeInOut).HasMaxLength(100);
            entity.Property(e => e.ProfileColorInside).HasMaxLength(100);
            entity.Property(e => e.ProfileColorOutside).HasMaxLength(100);
            entity.Property(e => e.ProfileHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ProfileLength).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ProfileLengthIncludingFOD).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.ProfileName).HasMaxLength(100);
            entity.Property(e => e.ProfileNumber).HasMaxLength(100);
            entity.Property(e => e.ProfileNumberForMachine).HasMaxLength(100);
            entity.Property(e => e.ProfileWidth).HasColumnType("decimal(23, 11)");
        });

        modelBuilder.Entity<vw_Source_ProductionFrameSash_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_ProductionFrameSash_Missing");

            entity.Property(e => e.AdjacentFramePartToSash).HasMaxLength(100);
            entity.Property(e => e.BeadSystemInnerSeal).HasMaxLength(100);
            entity.Property(e => e.BeadsHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.BeadsWidth).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.CremoneType1).HasMaxLength(100);
            entity.Property(e => e.DisplayColorInside).HasMaxLength(100);
            entity.Property(e => e.DisplayColorOutside).HasMaxLength(100);
            entity.Property(e => e.FrameSashHeight).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameSashHeightTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameSashWidth).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameSashWidthTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.FrameThresholdCounterProfile).HasMaxLength(100);
            entity.Property(e => e.GlazingAssignment).HasMaxLength(1000);
            entity.Property(e => e.GlazingBeadsPerSashFrame).HasMaxLength(100);
            entity.Property(e => e.GlazingCode).HasMaxLength(100);
            entity.Property(e => e.GlazingDimensions).HasMaxLength(100);
            entity.Property(e => e.GlazingSealText).HasMaxLength(100);
            entity.Property(e => e.GlazingText).HasMaxLength(100);
            entity.Property(e => e.HandlePosition).HasMaxLength(100);
            entity.Property(e => e.HardwareRabbetHeightTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.HardwareRabbetWidthTenths).HasColumnType("decimal(23, 11)");
            entity.Property(e => e.HardwareSystemCode).HasMaxLength(100);
            entity.Property(e => e.HardwareSystemText).HasMaxLength(100);
            entity.Property(e => e.InnerSealSashFrame).HasMaxLength(100);
            entity.Property(e => e.MechanismCode).HasMaxLength(100);
            entity.Property(e => e.OpeningTypeText).HasMaxLength(1000);
            entity.Property(e => e.PositionDataSealColor).HasMaxLength(100);
            entity.Property(e => e.ReinforcementCode).HasMaxLength(100);
            entity.Property(e => e.SashHardwareIndicator).HasMaxLength(100);
            entity.Property(e => e.Seal).HasMaxLength(100);
            entity.Property(e => e.SealColor).HasMaxLength(100);
            entity.Property(e => e.SealSystem).HasMaxLength(100);
            entity.Property(e => e.SealVariantCode).HasMaxLength(100);
            entity.Property(e => e.SealVariantText).HasMaxLength(100);
            entity.Property(e => e.SpecialOpeningTypeCode).HasMaxLength(100);
        });

        modelBuilder.Entity<vw_Source_ProductionSeries>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_ProductionSeries");

            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<vw_Source_SpatialPosition_Missing>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Source_SpatialPosition_Missing");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Position).HasMaxLength(20);
        });

        modelBuilder.Entity<vw_StockBinItemDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StockBinItemDetails");

            entity.Property(e => e.AccessibleDate).HasComment("Date à partir de laquelle l’article est considéré comme accessible en production.");
            entity.Property(e => e.AddressDesignation)
                .HasMaxLength(50)
                .HasComment("Désignation de l’adresse physique du bac.");
            entity.Property(e => e.AddressSortOrder).HasComment("Ordre de tri de l’adresse dans la zone.");
            entity.Property(e => e.CategoryLevel1)
                .HasMaxLength(100)
                .HasComment("Catégorie de niveau 1 de l’article.");
            entity.Property(e => e.CategoryLevel2)
                .HasMaxLength(100)
                .HasComment("Catégorie de niveau 2 de l’article.");
            entity.Property(e => e.CategoryLevel3)
                .HasMaxLength(100)
                .HasComment("Catégorie de niveau 3 de l’article.");
            entity.Property(e => e.CuttingMachine)
                .HasMaxLength(20)
                .HasComment("Code machine de découpe associée à l’article, si applicable.");
            entity.Property(e => e.Designation)
                .HasMaxLength(300)
                .HasComment("Désignation commerciale de l’article (ArticleReference).");
            entity.Property(e => e.Id).HasComment("Identifiant unique du StockBinItem.");
            entity.Property(e => e.IdArticleInternal).HasComment("Identifiant de l’enregistrement ArticleInternal auquel appartient l’article en stock.");
            entity.Property(e => e.IdColorRalFinish)
                .HasMaxLength(50)
                .HasComment("Identifiant de la finition RAL associée à l’article (intérieur / extérieur).");
            entity.Property(e => e.IdStockBin).HasComment("Identifiant du bac où l’article est stocké.");
            entity.Property(e => e.IdentificationType)
                .HasMaxLength(100)
                .HasComment("Type d’identification de l’article (pièce, barre, carton…).");
            entity.Property(e => e.InventoryDate).HasComment("Date du dernier inventaire physique pour cet article dans ce bac.");
            entity.Property(e => e.IsAccessible).HasComment("Indique si l’article est accessible pour la production (1 = oui, 0 = non).");
            entity.Property(e => e.Quantity).HasComment("Quantité disponible dans le StockBin.");
            entity.Property(e => e.Reference)
                .HasMaxLength(100)
                .HasComment("Référence commerciale de l’article (provenant de ArticleReference).");
            entity.Property(e => e.StockBinDesignation)
                .HasMaxLength(50)
                .HasComment("Désignation textuelle du bac de stockage.");
            entity.Property(e => e.StockBinType)
                .HasMaxLength(50)
                .HasComment("Type du bac (petit bac, casier, boîte…).");
            entity.Property(e => e.StorageCode)
                .HasMaxLength(20)
                .HasComment("Code de l’unité de stockage (ML, PC, CT…).");
            entity.Property(e => e.StorageUnit)
                .HasMaxLength(100)
                .HasComment("Désignation textuelle de l’unité de stockage.");
            entity.Property(e => e.SupportTypeDesignation)
                .HasMaxLength(100)
                .HasComment("Désignation du support physique du bac (rack, étagère, palette…).");
            entity.Property(e => e.ZoneDesignation)
                .HasMaxLength(50)
                .HasComment("Désignation de la zone de stockage où se trouve le bac.");
            entity.Property(e => e.ZoneSortOrder).HasComment("Priorité de tri de la zone.");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
