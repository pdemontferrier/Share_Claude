using System;
using System.Collections.Generic;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using BatchCutting_DG.C_Infrastructure.Settings;

namespace BatchCutting_DG.C_Infrastructure.Persistence.GestStock;

public partial class GestStockContext : DbContext
{
    public GestStockContext()
    {
    }

    public GestStockContext(DbContextOptions<GestStockContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArticleInterne> ArticleInternes { get; set; }

    public virtual DbSet<ChutesMagasin> ChutesMagasins { get; set; }

    public virtual DbSet<CommandeClient> CommandeClients { get; set; }

    public virtual DbSet<CommandeClientAction> CommandeClientActions { get; set; }

    public virtual DbSet<CommandeClientActionType> CommandeClientActionTypes { get; set; }

    public virtual DbSet<CommandeClientModification> CommandeClientModifications { get; set; }

    public virtual DbSet<CommandeClientStatut> CommandeClientStatuts { get; set; }

    public virtual DbSet<CouleursFinition> CouleursFinitions { get; set; }

    public virtual DbSet<CouleursRal> CouleursRals { get; set; }

    public virtual DbSet<CouleursRalFinition> CouleursRalFinitions { get; set; }

    public virtual DbSet<DecoupeBarre> DecoupeBarres { get; set; }

    public virtual DbSet<DecoupeDetail> DecoupeDetails { get; set; }

    public virtual DbSet<DecoupeLot> DecoupeLots { get; set; }

    public virtual DbSet<DecoupeMachine> DecoupeMachines { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAppEventStore> UserAppEventStores { get; set; }

    public virtual DbSet<UserAppMessage> UserAppMessages { get; set; }

    public virtual DbSet<UserAppPage> UserAppPages { get; set; }

    public virtual DbSet<UserAppPageDroit> UserAppPageDroits { get; set; }

    public virtual DbSet<UserDroit> UserDroits { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    public virtual DbSet<UserSessionCommand> UserSessionCommands { get; set; }

    public virtual DbSet<VieApplication> VieApplications { get; set; }

    public virtual DbSet<VieChuteMagasinReference> VieChuteMagasinReferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseMySql(SE_GestStock.Credential, Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.3.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_520_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<ArticleInterne>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("article_interne");

            entity.HasIndex(e => new { e.Reference, e.Couleur }, "article_interne_unique_reference_couleur").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Alertes)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("alertes");
            entity.Property(e => e.ArticleUbc)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("article_ubc");
            entity.Property(e => e.CalculStockMin)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("calcul_stock_min");
            entity.Property(e => e.CategorieL1)
                .HasMaxLength(50)
                .HasComment("Catégorie de fournisseur")
                .HasColumnName("categorie_L1");
            entity.Property(e => e.CategorieL2)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Actif'")
                .HasComment("Catégorie si Actif, Attente, Suspendu")
                .HasColumnName("categorie_L2");
            entity.Property(e => e.CoefStocSecu)
                .HasDefaultValueSql("'1.2'")
                .HasComment("CHECK (coef_stoc_secu>= 1)")
                .HasColumnName("coef_stoc_secu");
            entity.Property(e => e.CoefTauxChute)
                .HasDefaultValueSql("'1'")
                .HasColumnName("coef_taux_chute");
            entity.Property(e => e.ConsoHebdo)
                .HasColumnType("double(10,2)")
                .HasColumnName("conso_hebdo");
            entity.Property(e => e.ControlCheck).HasColumnName("control_check");
            entity.Property(e => e.Couleur)
                .HasMaxLength(20)
                .HasColumnName("couleur");
            entity.Property(e => e.DateConsoHebdoAuto).HasColumnName("date_conso_hebdo_auto");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("date_creation");
            entity.Property(e => e.DateDpa)
                .HasColumnType("datetime")
                .HasColumnName("date_dpa");
            entity.Property(e => e.Depreciation)
                .HasDefaultValueSql("'0'")
                .HasColumnName("depreciation");
            entity.Property(e => e.DernierPrix)
                .HasDefaultValueSql("'0.000000'")
                .HasColumnType("double(10,6)")
                .HasColumnName("dernier_prix");
            entity.Property(e => e.Designation)
                .HasMaxLength(300)
                .HasColumnName("designation");
            entity.Property(e => e.DesignationL1)
                .HasMaxLength(100)
                .HasComment("Désignation agrégée")
                .HasColumnName("designation_L1");
            entity.Property(e => e.Existant)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("existant");
            entity.Property(e => e.GereEnStock)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("gere_en_stock");
            entity.Property(e => e.GestionChute)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("gestion_chute");
            entity.Property(e => e.IdArticleInterneVitrageType)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("id_article_interne_vitrage_type");
            entity.Property(e => e.IdComposeType)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("id_compose_type");
            entity.Property(e => e.IdCouleurFinition)
                .HasMaxLength(3)
                .HasDefaultValueSql("'G'")
                .HasColumnName("id_couleur_finition");
            entity.Property(e => e.IdCouleurRal)
                .HasDefaultValueSql("'9999'")
                .HasColumnType("int(4)")
                .HasColumnName("id_couleur_ral");
            entity.Property(e => e.IdCouleurRalFinition)
                .HasMaxLength(15)
                .HasDefaultValueSql("'9999 G'")
                .HasColumnName("id_couleur_ral_finition");
            entity.Property(e => e.IdDernierFournisseur)
                .HasColumnType("int(11)")
                .HasColumnName("id_dernier_fournisseur");
            entity.Property(e => e.IdEmplacementChutes)
                .HasColumnType("int(11)")
                .HasColumnName("id_emplacement_chutes");
            entity.Property(e => e.IdFuturFournisseur)
                .HasColumnType("int(11)")
                .HasColumnName("id_futur_fournisseur");
            entity.Property(e => e.IdPickingListes)
                .HasDefaultValueSql("'9'")
                .HasColumnType("int(7)")
                .HasColumnName("id_picking_listes");
            entity.Property(e => e.InventaireCoefEmplacement)
                .HasDefaultValueSql("'1'")
                .HasColumnName("inventaire_coef_emplacement");
            entity.Property(e => e.InventaireEnCours)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("inventaire_en_cours");
            entity.Property(e => e.InventaireExempt)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("inventaire_exempt");
            entity.Property(e => e.InventaireForce)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("inventaire_force");
            entity.Property(e => e.Poids)
                .HasDefaultValueSql("'0'")
                .HasColumnName("poids");
            entity.Property(e => e.PrixMainOeuvre)
                .HasDefaultValueSql("'0'")
                .HasColumnName("prix_main_oeuvre");
            entity.Property(e => e.QteMiniPickingAuto)
                .HasDefaultValueSql("-1")
                .HasColumnType("int(11)")
                .HasColumnName("qte_mini_picking_auto");
            entity.Property(e => e.RapportPoids)
                .HasDefaultValueSql("'1'")
                .HasColumnName("rapport_poids");
            entity.Property(e => e.RapportResaStock)
                .HasDefaultValueSql("'1'")
                .HasColumnName("rapport_resa_stock");
            entity.Property(e => e.RapportStock1Stock2)
                .HasDefaultValueSql("'0'")
                .HasColumnName("rapport_stock1_stock2");
            entity.Property(e => e.RapportStockTarif)
                .HasDefaultValueSql("'1'")
                .HasColumnName("rapport_stock_tarif");
            entity.Property(e => e.RefElusoft)
                .HasMaxLength(50)
                .HasColumnName("ref_elusoft");
            entity.Property(e => e.Reference)
                .HasMaxLength(100)
                .HasColumnName("reference");
            entity.Property(e => e.StockMin)
                .HasComment("CHECK (stock_min>= 0)")
                .HasColumnType("int(6)")
                .HasColumnName("stock_min");
            entity.Property(e => e.StockSecurite)
                .HasComment("CHECK (stock_securite>= 1)")
                .HasColumnType("int(11)")
                .HasColumnName("stock_securite");
            entity.Property(e => e.UnitePoids)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("unite_poids");
            entity.Property(e => e.UniteReservation)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("unite_reservation");
            entity.Property(e => e.UniteStock)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("unite_stock");
            entity.Property(e => e.UniteStock2)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("unite_stock2");
            entity.Property(e => e.UniteTarif)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("unite_tarif");
            entity.Property(e => e.Valorised)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("valorised");
        });

        modelBuilder.Entity<ChutesMagasin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chutes_magasin");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AttenteIntegration).HasColumnName("attente_integration");
            entity.Property(e => e.CodeBarre)
                .HasMaxLength(15)
                .HasColumnName("code_barre");
            entity.Property(e => e.DateIntegration)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("date_integration");
            entity.Property(e => e.Emplacement)
                .HasColumnType("int(11)")
                .HasColumnName("emplacement");
            entity.Property(e => e.Enregistrement)
                .HasColumnType("datetime")
                .HasColumnName("enregistrement");
            entity.Property(e => e.IdArticleInterne)
                .HasColumnType("int(11)")
                .HasColumnName("id_article_interne");
            entity.Property(e => e.IdOperateur)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_operateur");
            entity.Property(e => e.IdType)
                .HasColumnType("int(11)")
                .HasColumnName("id_type");
            entity.Property(e => e.Largeur)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("largeur");
            entity.Property(e => e.Longueur)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("longueur");
            entity.Property(e => e.Prix)
                .HasDefaultValueSql("'0'")
                .HasColumnName("prix");
            entity.Property(e => e.Reserve)
                .HasMaxLength(20)
                .HasDefaultValueSql("'0'")
                .HasColumnName("reserve");
            entity.Property(e => e.Scan)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("scan");
        });

        modelBuilder.Entity<CommandeClient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("commande_client");

            entity.HasIndex(e => e.IdDecoupeLot, "commande_client_id_decoupe_lot_IDX");

            entity.HasIndex(e => e.NumProjet, "commande_client_num_projet_IDX");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AffectationLot).HasColumnName("affectation_lot");
            entity.Property(e => e.Altitude)
                .HasColumnType("int(11)")
                .HasColumnName("altitude");
            entity.Property(e => e.Annulation).HasColumnName("annulation");
            entity.Property(e => e.CmdParent)
                .HasColumnType("int(11)")
                .HasColumnName("cmd_parent");
            entity.Property(e => e.CommandeUbc)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("commande_ubc");
            entity.Property(e => e.Couleur)
                .HasMaxLength(45)
                .HasColumnName("couleur");
            entity.Property(e => e.CoutMateriel).HasColumnName("cout_materiel");
            entity.Property(e => e.DateAcces).HasColumnName("date_acces");
            entity.Property(e => e.DateConfirmation).HasColumnName("date_confirmation");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("date_creation");
            entity.Property(e => e.DateFabrication).HasColumnName("date_fabrication");
            entity.Property(e => e.DateFacturation).HasColumnName("date_facturation");
            entity.Property(e => e.DatePremierDossierFab).HasColumnName("date_premier_dossier_fab");
            entity.Property(e => e.DatePremiereConfirme).HasColumnName("date_premiere_confirme");
            entity.Property(e => e.DateSignature).HasColumnName("date_signature");
            entity.Property(e => e.DateValidation).HasColumnName("date_validation");
            entity.Property(e => e.DelaiSupp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("delai_supp");
            entity.Property(e => e.DesignationProjet)
                .HasMaxLength(1000)
                .HasColumnName("designation_projet");
            entity.Property(e => e.DetailStructure)
                .HasColumnType("text")
                .HasColumnName("detail_structure");
            entity.Property(e => e.Divers)
                .HasColumnType("text")
                .HasColumnName("divers");
            entity.Property(e => e.EcheanceSemaineFabrication)
                .HasColumnType("int(7)")
                .HasColumnName("echeance_semaine_fabrication");
            entity.Property(e => e.ExportCarnetGarantie)
                .HasDefaultValueSql("-1")
                .HasColumnType("tinyint(4)")
                .HasColumnName("export_carnet_garantie");
            entity.Property(e => e.FichierConso)
                .HasMaxLength(300)
                .HasColumnName("fichier_Conso");
            entity.Property(e => e.FichierDebitComplet)
                .HasMaxLength(100)
                .HasColumnName("fichier_Debit_Complet");
            entity.Property(e => e.Financement)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("financement");
            entity.Property(e => e.IdCommandeParent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_commande_parent");
            entity.Property(e => e.IdCommercial)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_commercial");
            entity.Property(e => e.IdConcession)
                .HasColumnType("smallint(6)")
                .HasColumnName("id_concession");
            entity.Property(e => e.IdCouleurFinition)
                .HasMaxLength(3)
                .HasColumnName("id_couleur_finition");
            entity.Property(e => e.IdCouleurRal)
                .HasColumnType("int(4)")
                .HasColumnName("id_couleur_ral");
            entity.Property(e => e.IdCouleurRalFinition)
                .HasMaxLength(15)
                .HasColumnName("id_couleur_ral_finition");
            entity.Property(e => e.IdDecoupeLot)
                .HasColumnType("int(11)")
                .HasColumnName("id_decoupe_lot");
            entity.Property(e => e.IdTournee)
                .HasColumnType("int(11)")
                .HasColumnName("id_tournee");
            entity.Property(e => e.IdTypeSecondaire)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_type_secondaire");
            entity.Property(e => e.IdTypeTertiaire)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_type_tertiaire");
            entity.Property(e => e.ImportColisage)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("import_colisage");
            entity.Property(e => e.ImportControle).HasColumnName("import_controle");
            entity.Property(e => e.ImportCover).HasColumnName("import_cover");
            entity.Property(e => e.ImportDebitComplet).HasColumnName("import_Debit_Complet");
            entity.Property(e => e.ImportValide).HasColumnName("import_valide");
            entity.Property(e => e.LienDossier1)
                .HasMaxLength(300)
                .HasColumnName("lien_dossier_1");
            entity.Property(e => e.LienDossier2)
                .HasMaxLength(300)
                .HasColumnName("lien_dossier_2");
            entity.Property(e => e.ListePicking).HasColumnName("liste_picking");
            entity.Property(e => e.LivraisonSouhaitee).HasColumnName("livraison_souhaitee");
            entity.Property(e => e.Marge).HasColumnName("marge");
            entity.Property(e => e.Modele)
                .HasMaxLength(45)
                .HasColumnName("modele");
            entity.Property(e => e.ModeleCategorie)
                .HasMaxLength(50)
                .HasColumnName("modele_categorie");
            entity.Property(e => e.ModeleType)
                .HasMaxLength(50)
                .HasColumnName("modele_type");
            entity.Property(e => e.NomProjet)
                .HasMaxLength(100)
                .HasColumnName("nom_projet");
            entity.Property(e => e.NumAx)
                .HasMaxLength(10)
                .HasColumnName("Num_Ax");
            entity.Property(e => e.NumLancement1)
                .HasColumnType("int(4)")
                .HasColumnName("num_lancement_1");
            entity.Property(e => e.NumLancement2)
                .HasColumnType("int(4)")
                .HasColumnName("num_lancement_2");
            entity.Property(e => e.NumProjet)
                .HasMaxLength(9)
                .HasColumnName("num_projet");
            entity.Property(e => e.NumServeur)
                .HasDefaultValueSql("'1900'")
                .HasColumnType("int(4)")
                .HasColumnName("num_serveur");
            entity.Property(e => e.OrigineSav)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("origine_sav");
            entity.Property(e => e.PrePlanif)
                .HasColumnType("tinyint(4)")
                .HasColumnName("pre_planif");
            entity.Property(e => e.PrixHt).HasColumnName("prix_ht");
            entity.Property(e => e.PrixPartCom).HasColumnName("prix_part_com");
            entity.Property(e => e.PrixPrestation).HasColumnName("prix_prestation");
            entity.Property(e => e.PrixTransport).HasColumnName("prix_transport");
            entity.Property(e => e.RemiseTotale).HasColumnName("remise_totale");
            entity.Property(e => e.Reserve)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("reserve");
            entity.Property(e => e.ServeurArchive)
                .HasColumnType("tinyint(4)")
                .HasColumnName("serveur_archive");
            entity.Property(e => e.Statut)
                .HasDefaultValueSql("'19'")
                .HasColumnType("int(11)")
                .HasColumnName("statut");
            entity.Property(e => e.StatutDecoupeLot)
                .HasDefaultValueSql("'Attente'")
                .HasColumnType("enum('Attente','Affecté','Encours','Terminé')")
                .HasColumnName("statut_decoupe_lot");
            entity.Property(e => e.TypeCommande)
                .HasColumnType("int(11)")
                .HasColumnName("type_commande");
            entity.Property(e => e.Updated).HasColumnName("updated");
        });

        modelBuilder.Entity<CommandeClientAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("commande_client_action");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.BonusMalus)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(2)")
                .HasColumnName("bonus_Malus");
            entity.Property(e => e.DateAction)
                .HasColumnType("datetime")
                .HasColumnName("date_action");
            entity.Property(e => e.IdAction)
                .HasColumnType("int(11)")
                .HasColumnName("id_action");
            entity.Property(e => e.IdCmdClient)
                .HasColumnType("int(11)")
                .HasColumnName("id_cmd_client");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_user");
            entity.Property(e => e.TempsEstime)
                .HasComment("Le temps passé est définie en seconde")
                .HasColumnType("int(111)")
                .HasColumnName("tempsEstime");
        });

        modelBuilder.Entity<CommandeClientActionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("commande_client_action_type");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(45)
                .HasColumnName("action");
            entity.Property(e => e.AdvAff)
                .HasColumnType("int(2)")
                .HasColumnName("ADV_aff");
            entity.Property(e => e.Controle)
                .HasMaxLength(45)
                .HasColumnName("controle");
            entity.Property(e => e.IdCcStatut)
                .HasColumnType("int(11)")
                .HasColumnName("id_cc_statut");
            entity.Property(e => e.IsModif)
                .HasColumnType("int(11)")
                .HasColumnName("isModif");
        });

        modelBuilder.Entity<CommandeClientModification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("commande_client_modification");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.DateModification)
                .HasColumnType("datetime")
                .HasColumnName("date_modification");
            entity.Property(e => e.NouvelleValeur)
                .HasColumnType("text")
                .HasColumnName("nouvelle_valeur");
            entity.Property(e => e.NumProjet)
                .HasMaxLength(12)
                .HasColumnName("num_projet");
            entity.Property(e => e.TypeModif)
                .HasMaxLength(100)
                .HasColumnName("type_modif");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<CommandeClientStatut>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("commande_client_statut");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Valeur, "valeur_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Affichage)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("affichage");
            entity.Property(e => e.CaPrevisionnelReste)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("CA_previsionnel_reste");
            entity.Property(e => e.Confirmable)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(2)");
            entity.Property(e => e.Couleur)
                .HasMaxLength(50)
                .HasColumnName("couleur");
            entity.Property(e => e.Extraction)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("extraction");
            entity.Property(e => e.FinirReservation)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("finir_reservation");
            entity.Property(e => e.FinirTacheOuverte)
                .HasDefaultValueSql("'0'")
                .HasColumnType("smallint(6)")
                .HasColumnName("finir_tache_ouverte");
            entity.Property(e => e.IdStatutAx)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_statut_ax");
            entity.Property(e => e.MailClient)
                .HasColumnType("tinyint(4)")
                .HasColumnName("mail_client");
            entity.Property(e => e.MailTexte)
                .HasColumnType("text")
                .HasColumnName("mail_texte");
            entity.Property(e => e.Nom)
                .HasMaxLength(75)
                .HasColumnName("nom");
            entity.Property(e => e.Pointage)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("pointage");
            entity.Property(e => e.RecalculMarge)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("recalcul_marge");
            entity.Property(e => e.ResetPlanif)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("reset_planif");
            entity.Property(e => e.TabCaFacturable)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("tab_ca_facturable");
            entity.Property(e => e.TabCaFacture)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("tab_ca_facture");
            entity.Property(e => e.TabCaPreviAttente)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("tab_ca_previ_attente");
            entity.Property(e => e.TabCaPreviConsolide)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("tab_ca_previ_consolide");
            entity.Property(e => e.TabIgnore)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("tab_ignore");
            entity.Property(e => e.Valeur)
                .HasColumnType("int(11)")
                .HasColumnName("valeur");
            entity.Property(e => e.VerifImport)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("verif_import");
        });

        modelBuilder.Entity<CouleursFinition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("couleurs_finition");

            entity.Property(e => e.Id)
                .HasMaxLength(3)
                .HasDefaultValueSql("'G'")
                .HasColumnName("id");
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .HasColumnName("designation");
        });

        modelBuilder.Entity<CouleursRal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("couleurs_ral");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(4)")
                .HasColumnName("id");
            entity.Property(e => e.CodeCmjn)
                .HasMaxLength(100)
                .HasColumnName("code_cmjn");
            entity.Property(e => e.CodeHexa)
                .HasMaxLength(100)
                .HasColumnName("code_hexa");
            entity.Property(e => e.CodeRvb)
                .HasMaxLength(100)
                .HasColumnName("code_rvb");
            entity.Property(e => e.DesignationAnglais)
                .HasMaxLength(100)
                .HasColumnName("designation_anglais");
            entity.Property(e => e.DesignationFrancais)
                .HasMaxLength(100)
                .HasColumnName("designation_francais");
            entity.Property(e => e.ReferenceBase)
                .HasMaxLength(300)
                .HasColumnName("reference_base");
        });

        modelBuilder.Entity<CouleursRalFinition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("couleurs_ral_finition");

            entity.HasIndex(e => e.IdCouleurRalInt, "couleurs_ral_base_FK1");

            entity.HasIndex(e => e.IdCouleurFinitionInt, "couleurs_ral_base_FK2");

            entity.Property(e => e.Id)
                .HasMaxLength(15)
                .HasColumnName("id");
            entity.Property(e => e.IdCouleurFinitionExt)
                .HasMaxLength(3)
                .HasColumnName("id_couleur_finition_ext");
            entity.Property(e => e.IdCouleurFinitionInt)
                .HasMaxLength(3)
                .HasColumnName("id_couleur_finition_int");
            entity.Property(e => e.IdCouleurRalExt)
                .HasColumnType("int(4)")
                .HasColumnName("id_couleur_ral_ext");
            entity.Property(e => e.IdCouleurRalInt)
                .HasColumnType("int(4)")
                .HasColumnName("id_couleur_ral_int");

            entity.HasOne(d => d.IdCouleurFinitionIntNavigation).WithMany(p => p.CouleursRalFinitions)
                .HasForeignKey(d => d.IdCouleurFinitionInt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("couleurs_ral_base_FK2");

            entity.HasOne(d => d.IdCouleurRalIntNavigation).WithMany(p => p.CouleursRalFinitions)
                .HasForeignKey(d => d.IdCouleurRalInt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("couleurs_ral_base_FK1");
        });

        modelBuilder.Entity<DecoupeBarre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("decoupe_barre");

            entity.HasIndex(e => e.IdArticleInterne, "decoupe_barre_id_article_interne_IDX");

            entity.HasIndex(e => e.IdDecoupeLot, "decoupe_barre_id_decoupe_lot_IDX");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.ApproAdresseDesignation)
                .HasMaxLength(45)
                .HasColumnName("appro_adresse_designation");
            entity.Property(e => e.ApproAdressePriorite)
                .HasColumnType("int(11)")
                .HasColumnName("appro_adresse_priorite");
            entity.Property(e => e.ApproAllocation).HasColumnName("appro_allocation");
            entity.Property(e => e.ApproChariotDesignation)
                .HasMaxLength(20)
                .HasColumnName("appro_chariot_designation");
            entity.Property(e => e.ApproCodeBarre)
                .HasMaxLength(15)
                .HasColumnName("appro_code_barre");
            entity.Property(e => e.ApproConteneur)
                .HasMaxLength(45)
                .HasColumnName("appro_conteneur");
            entity.Property(e => e.ApproDateDebut)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("appro_date_debut");
            entity.Property(e => e.ApproDateFin)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("appro_date_fin");
            entity.Property(e => e.ApproEmplacement)
                .HasColumnType("int(11)")
                .HasColumnName("appro_emplacement");
            entity.Property(e => e.ApproEmplacementDesignation)
                .HasMaxLength(45)
                .HasColumnName("appro_emplacement_designation");
            entity.Property(e => e.ApproIdChariot)
                .HasColumnType("int(11)")
                .HasColumnName("appro_id_chariot");
            entity.Property(e => e.ApproInactif).HasColumnName("appro_inactif");
            entity.Property(e => e.ApproOrigine)
                .HasDefaultValueSql("'nd'")
                .HasColumnType("enum('neuf','chute','nd')")
                .HasColumnName("appro_origine");
            entity.Property(e => e.ApproPosteMachineId)
                .HasMaxLength(30)
                .HasColumnName("appro_poste_machine_id");
            entity.Property(e => e.ApproPosteMachineIp)
                .HasMaxLength(30)
                .HasColumnName("appro_poste_machine_ip");
            entity.Property(e => e.ApproRupture).HasColumnName("appro_rupture");
            entity.Property(e => e.ApproSortieFaite).HasColumnName("appro_sortie_faite");
            entity.Property(e => e.ApproSortieForce).HasColumnName("appro_sortie_force");
            entity.Property(e => e.ApproSortieSupp).HasColumnName("appro_sortie_supp");
            entity.Property(e => e.ApproTypeContenant)
                .HasMaxLength(45)
                .HasColumnName("appro_type_contenant");
            entity.Property(e => e.ApproTypeConteneur)
                .HasMaxLength(45)
                .HasColumnName("appro_type_conteneur");
            entity.Property(e => e.ApproUtilisateurErp)
                .HasColumnType("int(11)")
                .HasColumnName("appro_utilisateur_ERP");
            entity.Property(e => e.ApproUtilisateurPoste)
                .HasMaxLength(30)
                .HasColumnName("appro_utilisateur_poste");
            entity.Property(e => e.ApproZoneDesignation)
                .HasMaxLength(45)
                .HasColumnName("appro_zone_designation");
            entity.Property(e => e.ApproZonePriorite)
                .HasColumnType("int(11)")
                .HasColumnName("appro_zone_priorite");
            entity.Property(e => e.Categorie1)
                .HasColumnType("int(2)")
                .HasColumnName("categorie1");
            entity.Property(e => e.Categorie2)
                .HasColumnType("int(2)")
                .HasColumnName("categorie2");
            entity.Property(e => e.Categorie3)
                .HasColumnType("int(2)")
                .HasColumnName("categorie3");
            entity.Property(e => e.Categorie4)
                .HasMaxLength(20)
                .HasColumnName("categorie4");
            entity.Property(e => e.DecoupeCodeBarreChute)
                .HasMaxLength(15)
                .HasColumnName("decoupe_code_barre_chute");
            entity.Property(e => e.DecoupeCommentaires)
                .HasMaxLength(1000)
                .HasColumnName("decoupe_commentaires");
            entity.Property(e => e.DecoupeDateDebut)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("decoupe_date_debut");
            entity.Property(e => e.DecoupeDateFin)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("decoupe_date_fin");
            entity.Property(e => e.DecoupeEmpSc)
                .HasMaxLength(10)
                .HasDefaultValueSql("'nd'")
                .HasColumnName("decoupe_emp_SC");
            entity.Property(e => e.DecoupeFaite).HasColumnName("decoupe_faite");
            entity.Property(e => e.DecoupeGestionChutes).HasColumnName("decoupe_gestion_chutes");
            entity.Property(e => e.DecoupeIdEmpSc)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("decoupe_id_emp_SC");
            entity.Property(e => e.DecoupeLongueurChuteFinale)
                .HasColumnType("int(11)")
                .HasColumnName("decoupe_longueur_chute_finale");
            entity.Property(e => e.DecoupeLongueurReste)
                .HasPrecision(7, 3)
                .HasColumnName("decoupe_longueur_reste");
            entity.Property(e => e.DecoupeNombre)
                .HasColumnType("int(4)")
                .HasColumnName("decoupe_nombre");
            entity.Property(e => e.DecoupePosteMachineId)
                .HasMaxLength(30)
                .HasColumnName("decoupe_poste_machine_id");
            entity.Property(e => e.DecoupePosteMachineIp)
                .HasMaxLength(30)
                .HasColumnName("decoupe_poste_machine_ip");
            entity.Property(e => e.DecoupeTypeReste)
                .HasDefaultValueSql("'nd'")
                .HasColumnType("enum('dechet','chute','nd')")
                .HasColumnName("decoupe_type_reste");
            entity.Property(e => e.DecoupeUtilisateurErp)
                .HasColumnType("int(11)")
                .HasColumnName("decoupe_utilisateur_ERP");
            entity.Property(e => e.DecoupeUtilisateurPoste)
                .HasMaxLength(30)
                .HasColumnName("decoupe_utilisateur_poste");
            entity.Property(e => e.DernierPrixDateMaj)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("dernier_prix_date_maj");
            entity.Property(e => e.DernierPrixMm)
                .HasDefaultValueSql("'0.000000'")
                .HasColumnType("double(10,6)")
                .HasColumnName("dernier_prix_mm");
            entity.Property(e => e.IdArticleInterne)
                .HasColumnType("int(11)")
                .HasColumnName("id_article_interne");
            entity.Property(e => e.IdDecoupeLot)
                .HasColumnType("int(11)")
                .HasColumnName("id_decoupe_lot");
            entity.Property(e => e.IdStock)
                .HasColumnType("int(11)")
                .HasColumnName("id_stock");
            entity.Property(e => e.LongueurBarre)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_barre");
            entity.Property(e => e.LongueurChuteMini)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_chute_mini");
            entity.Property(e => e.OrdreTri)
                .HasColumnType("int(3)")
                .HasColumnName("ordre_tri");
        });

        modelBuilder.Entity<DecoupeDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("decoupe_detail");

            entity.HasIndex(e => e.IdArticleInterne, "decoupe_detail_id_article_interne_IDX");

            entity.HasIndex(e => e.IdCommandeClient, "decoupe_detail_id_commande_client_IDX");

            entity.HasIndex(e => e.IdDecoupeLot, "decoupe_detail_id_decoupe_lot_IDX");

            entity.HasIndex(e => e.NumLigne, "decoupe_detail_num_ligne_IDX");

            entity.HasIndex(e => e.NumProjet, "decoupe_detail_num_projet_IDX");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AdvPosteMachineId)
                .HasMaxLength(100)
                .HasColumnName("adv_poste_machine_id");
            entity.Property(e => e.AdvPosteMachineIp)
                .HasMaxLength(100)
                .HasColumnName("adv_poste_machine_ip");
            entity.Property(e => e.AdvUtilisateurErp)
                .HasDefaultValueSql("'26'")
                .HasColumnType("int(11)")
                .HasColumnName("adv_utilisateur_ERP");
            entity.Property(e => e.AdvUtilisateurPoste)
                .HasMaxLength(100)
                .HasColumnName("adv_utilisateur_poste");
            entity.Property(e => e.ApproCompoChute).HasColumnName("appro_compo_chute");
            entity.Property(e => e.ApproCompoNeuf).HasColumnName("appro_compo_neuf");
            entity.Property(e => e.ApproComposant).HasColumnName("appro_composant");
            entity.Property(e => e.ApproComposeInactif).HasColumnName("appro_compose_inactif");
            entity.Property(e => e.ApproOptimBarreChute).HasColumnName("appro_optim_barre_chute");
            entity.Property(e => e.ApproOptimBarreNeuve).HasColumnName("appro_optim_barre_neuve");
            entity.Property(e => e.Categorie1)
                .HasColumnType("int(2)")
                .HasColumnName("categorie1");
            entity.Property(e => e.Categorie2)
                .HasColumnType("int(2)")
                .HasColumnName("categorie2");
            entity.Property(e => e.Categorie3)
                .HasColumnType("int(2)")
                .HasColumnName("categorie3");
            entity.Property(e => e.Categorie4)
                .HasMaxLength(20)
                .HasColumnName("categorie4");
            entity.Property(e => e.Commentaires)
                .HasMaxLength(3000)
                .HasColumnName("commentaires");
            entity.Property(e => e.Couleur)
                .HasMaxLength(20)
                .HasColumnName("couleur");
            entity.Property(e => e.DecoupeAFaire).HasColumnName("decoupe_a_faire");
            entity.Property(e => e.DecoupeBarreIndex)
                .HasColumnType("int(4)")
                .HasColumnName("decoupe_barre_index");
            entity.Property(e => e.DecoupeCommentaires)
                .HasMaxLength(1000)
                .HasColumnName("decoupe_commentaires");
            entity.Property(e => e.DecoupeDateDebut)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("decoupe_date_debut");
            entity.Property(e => e.DecoupeDateFin)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("decoupe_date_fin");
            entity.Property(e => e.DecoupeFaite).HasColumnName("decoupe_faite");
            entity.Property(e => e.DecoupeLongueurReste)
                .HasPrecision(7, 3)
                .HasColumnName("decoupe_longueur_reste");
            entity.Property(e => e.DecoupePosteMachineId)
                .HasMaxLength(30)
                .HasColumnName("decoupe_poste_machine_id");
            entity.Property(e => e.DecoupePosteMachineIp)
                .HasMaxLength(30)
                .HasColumnName("decoupe_poste_machine_ip");
            entity.Property(e => e.DecoupeUtilisateurErp)
                .HasColumnType("int(11)")
                .HasColumnName("decoupe_utilisateur_ERP");
            entity.Property(e => e.DecoupeUtilisateurPoste)
                .HasMaxLength(30)
                .HasColumnName("decoupe_utilisateur_poste");
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .HasColumnName("designation");
            entity.Property(e => e.IdArticleCompose)
                .HasColumnType("int(11)")
                .HasColumnName("id_article_compose");
            entity.Property(e => e.IdArticleInterne)
                .HasColumnType("int(11)")
                .HasColumnName("id_article_interne");
            entity.Property(e => e.IdCommandeClient)
                .HasColumnType("int(11)")
                .HasColumnName("id_commande_client");
            entity.Property(e => e.IdDecoupeBarre)
                .HasColumnType("int(11)")
                .HasColumnName("id_decoupe_barre");
            entity.Property(e => e.IdDecoupeLot)
                .HasColumnType("int(11)")
                .HasColumnName("id_decoupe_lot");
            entity.Property(e => e.IdImport)
                .HasColumnType("int(11)")
                .HasColumnName("id_import");
            entity.Property(e => e.IdPrimaire)
                .HasColumnType("int(11)")
                .HasColumnName("id_primaire");
            entity.Property(e => e.Inactif).HasColumnName("inactif");
            entity.Property(e => e.Inclinaison1)
                .HasPrecision(5, 2)
                .HasColumnName("inclinaison_1");
            entity.Property(e => e.Inclinaison2)
                .HasPrecision(5, 2)
                .HasColumnName("inclinaison_2");
            entity.Property(e => e.IndiceDecoupe)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(1)")
                .HasColumnName("indice_decoupe");
            entity.Property(e => e.LongueurBarre)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_barre");
            entity.Property(e => e.LongueurChuteMini)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_chute_mini");
            entity.Property(e => e.LongueurDecoupe)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_decoupe");
            entity.Property(e => e.LongueurOptim)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_optim");
            entity.Property(e => e.MessageElumatec)
                .HasMaxLength(240)
                .HasColumnName("message_elumatec");
            entity.Property(e => e.NomProjet)
                .HasMaxLength(30)
                .HasColumnName("nom_projet");
            entity.Property(e => e.NumLigne)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("num_ligne");
            entity.Property(e => e.NumProjet)
                .HasMaxLength(9)
                .HasColumnName("num_projet");
            entity.Property(e => e.OrdreTri)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(3)")
                .HasColumnName("ordre_tri");
            entity.Property(e => e.Pivot1)
                .HasPrecision(5, 2)
                .HasColumnName("pivot_1");
            entity.Property(e => e.Pivot2)
                .HasPrecision(5, 2)
                .HasColumnName("pivot_2");
            entity.Property(e => e.Position)
                .HasMaxLength(20)
                .HasColumnName("position");
            entity.Property(e => e.Quantite)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(3)")
                .HasColumnName("quantite");
            entity.Property(e => e.RefCommentaires)
                .HasMaxLength(1)
                .HasColumnName("ref_commentaires");
            entity.Property(e => e.Reference)
                .HasMaxLength(100)
                .HasColumnName("reference");
            entity.Property(e => e.ReferenceVue)
                .HasMaxLength(100)
                .HasColumnName("reference_vue");
            entity.Property(e => e.Source)
                .HasMaxLength(1000)
                .HasColumnName("source");
            entity.Property(e => e.Structure)
                .HasMaxLength(20)
                .HasColumnName("structure");
            entity.Property(e => e.TypeHistorique)
                .HasMaxLength(20)
                .HasColumnName("type_historique");
            entity.Property(e => e.ValidationLigne).HasColumnName("validation_ligne");
        });

        modelBuilder.Entity<DecoupeLot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("decoupe_lot");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.ApproChariotDesignation)
                .HasMaxLength(20)
                .HasColumnName("appro_chariot_designation");
            entity.Property(e => e.ApproChute).HasColumnName("appro_chute");
            entity.Property(e => e.ApproChuteDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("appro_chute_date");
            entity.Property(e => e.ApproChutePosteId)
                .HasMaxLength(30)
                .HasColumnName("appro_chute_poste_id");
            entity.Property(e => e.ApproChutePosteIp)
                .HasMaxLength(30)
                .HasColumnName("appro_chute_poste_ip");
            entity.Property(e => e.ApproChuteUtilisateurErp)
                .HasColumnType("int(11)")
                .HasColumnName("appro_chute_utilisateur_ERP");
            entity.Property(e => e.ApproChuteUtilisateurPoste)
                .HasMaxLength(30)
                .HasColumnName("appro_chute_utilisateur_poste");
            entity.Property(e => e.ApproIdChariot)
                .HasColumnType("int(11)")
                .HasColumnName("appro_id_chariot");
            entity.Property(e => e.ApproNeuf).HasColumnName("appro_neuf");
            entity.Property(e => e.ApproNeufDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("appro_neuf_date");
            entity.Property(e => e.ApproNeufPosteId)
                .HasMaxLength(30)
                .HasColumnName("appro_neuf_poste_id");
            entity.Property(e => e.ApproNeufPosteIp)
                .HasMaxLength(30)
                .HasColumnName("appro_neuf_poste_ip");
            entity.Property(e => e.ApproNeufUtilisateurErp)
                .HasColumnType("int(11)")
                .HasColumnName("appro_neuf_utilisateur_ERP");
            entity.Property(e => e.ApproNeufUtilisateurPoste)
                .HasMaxLength(30)
                .HasColumnName("appro_neuf_utilisateur_poste");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("creation_date");
            entity.Property(e => e.CreationPosteId)
                .HasMaxLength(30)
                .HasColumnName("creation_poste_id");
            entity.Property(e => e.CreationPosteIp)
                .HasMaxLength(30)
                .HasColumnName("creation_poste_ip");
            entity.Property(e => e.CreationUtilisateurErp)
                .HasColumnType("int(11)")
                .HasColumnName("creation_utilisateur_ERP");
            entity.Property(e => e.CreationUtilisateurPoste)
                .HasMaxLength(30)
                .HasColumnName("creation_utilisateur_poste");
            entity.Property(e => e.DecoupeDg).HasColumnName("decoupe_DG");
            entity.Property(e => e.DecoupeDgDateDebut)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("decoupe_DG_date_debut");
            entity.Property(e => e.DecoupeDgDateFin)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("decoupe_DG_date_fin");
            entity.Property(e => e.DecoupeDgEncours).HasColumnName("decoupe_DG_encours");
            entity.Property(e => e.Designation)
                .HasMaxLength(60)
                .HasColumnName("designation");
            entity.Property(e => e.IdCouleur)
                .HasMaxLength(15)
                .HasColumnName("id_couleur");
            entity.Property(e => e.IdEcheance)
                .HasColumnType("int(6)")
                .HasColumnName("id_echeance");
            entity.Property(e => e.Inactif).HasColumnName("inactif");
            entity.Property(e => e.NbProject)
                .HasColumnType("int(3)")
                .HasColumnName("nb_project");
            entity.Property(e => e.OptimChute).HasColumnName("optim_chute");
            entity.Property(e => e.OptimNeuf).HasColumnName("optim_neuf");
            entity.Property(e => e.ValidDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("valid_date");
            entity.Property(e => e.ValidFaite).HasColumnName("valid_faite");
            entity.Property(e => e.ValidPosteId)
                .HasMaxLength(30)
                .HasColumnName("valid_poste_id");
            entity.Property(e => e.ValidPosteIp)
                .HasMaxLength(30)
                .HasColumnName("valid_poste_ip");
            entity.Property(e => e.ValidUtilisateurErp)
                .HasColumnType("int(11)")
                .HasColumnName("valid_utilisateur_ERP");
            entity.Property(e => e.ValidUtilisateurPoste)
                .HasMaxLength(30)
                .HasColumnName("valid_utilisateur_poste");
        });

        modelBuilder.Entity<DecoupeMachine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("decoupe_machine");

            entity.HasIndex(e => e.IdMachine, "id_machine").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AdresseIpConsole)
                .HasMaxLength(20)
                .HasColumnName("adresse_ip_Console");
            entity.Property(e => e.AdresseIpImp)
                .HasMaxLength(20)
                .HasColumnName("adresse_ip_IMP");
            entity.Property(e => e.AdresseIpPc)
                .HasMaxLength(20)
                .HasColumnName("adresse_ip_PC");
            entity.Property(e => e.DescriptionMachine)
                .HasMaxLength(300)
                .HasDefaultValueSql("'A Completer'")
                .HasColumnName("description_machine");
            entity.Property(e => e.DesignationConsole)
                .HasMaxLength(50)
                .HasColumnName("designation_Console");
            entity.Property(e => e.DesignationImp)
                .HasMaxLength(50)
                .HasColumnName("designation_IMP");
            entity.Property(e => e.DesignationPc)
                .HasMaxLength(50)
                .HasColumnName("designation_PC");
            entity.Property(e => e.IdMachine)
                .HasMaxLength(20)
                .HasDefaultValueSql("'DG244_xx'")
                .HasColumnName("id_machine");
            entity.Property(e => e.PortComConsole)
                .HasMaxLength(4)
                .HasColumnName("port_COM_Console");
            entity.Property(e => e.PortComPc)
                .HasMaxLength(4)
                .HasColumnName("port_COM_PC");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.Acces)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(2)");
            entity.Property(e => e.Adresse).HasMaxLength(200);
            entity.Property(e => e.Birthday).HasDefaultValueSql("'1900-01-01'");
            entity.Property(e => e.CodePostal)
                .HasColumnType("int(11)")
                .HasColumnName("Code_postal");
            entity.Property(e => e.DateEntree)
                .HasDefaultValueSql("'1900-01-01'")
                .HasColumnName("Date_Entree");
            entity.Property(e => e.Exist)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(2)");
            entity.Property(e => e.IdSecteur)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_secteur");
            entity.Property(e => e.Initial).HasMaxLength(5);
            entity.Property(e => e.LicenceE3)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("licence_e3");
            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.LoginWindows)
                .HasMaxLength(20)
                .HasColumnName("login_windows");
            entity.Property(e => e.MailPerso)
                .HasMaxLength(200)
                .HasDefaultValueSql("'someone@something.com'")
                .HasColumnName("Mail_perso");
            entity.Property(e => e.MailPro)
                .HasMaxLength(200)
                .HasColumnName("Mail_pro");
            entity.Property(e => e.MatriculePleiade)
                .HasMaxLength(100)
                .HasColumnName("matricule_pleiade");
            entity.Property(e => e.MdpReset)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(2)")
                .HasColumnName("mdp_reset");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(200)
                .HasColumnName("Mot_de_passe");
            entity.Property(e => e.Nom).HasMaxLength(100);
            entity.Property(e => e.Pays).HasMaxLength(100);
            entity.Property(e => e.Prcp)
                .HasDefaultValueSql("'100'")
                .HasComment("Pourcentage de production ")
                .HasColumnType("int(3)")
                .HasColumnName("PRCP");
            entity.Property(e => e.Prenom).HasMaxLength(100);
            entity.Property(e => e.Societe).HasColumnType("int(11)");
            entity.Property(e => e.TelFixePro)
                .HasMaxLength(14)
                .HasColumnName("Tel_fixe_pro");
            entity.Property(e => e.TelPerso)
                .HasMaxLength(14)
                .HasDefaultValueSql("'00 00 00 00 00'")
                .HasColumnName("Tel_perso");
            entity.Property(e => e.TelPro)
                .HasMaxLength(14)
                .HasColumnName("Tel_pro");
            entity.Property(e => e.TypeContrat)
                .HasColumnType("int(11)")
                .HasColumnName("Type_contrat");
            entity.Property(e => e.Ville).HasMaxLength(100);
        });

        modelBuilder.Entity<UserAppEventStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_app_event_store");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AppCallChain)
                .HasMaxLength(1000)
                .HasColumnName("app_call_chain");
            entity.Property(e => e.AppCommandMethod)
                .HasMaxLength(1000)
                .HasColumnName("app_command_method");
            entity.Property(e => e.AppHandlerCommand)
                .HasMaxLength(1000)
                .HasColumnName("app_handler_command");
            entity.Property(e => e.AppId)
                .HasColumnType("int(11)")
                .HasColumnName("app_id");
            entity.Property(e => e.AppUserId)
                .HasColumnType("int(11)")
                .HasColumnName("app_user_id");
            entity.Property(e => e.Data)
                .HasMaxLength(4000)
                .HasColumnName("data");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(30)
                .HasColumnName("device_id");
            entity.Property(e => e.DeviceIp)
                .HasMaxLength(30)
                .HasColumnName("device_ip");
            entity.Property(e => e.DeviceUser)
                .HasMaxLength(30)
                .HasColumnName("device_user");
            entity.Property(e => e.TableDesignation)
                .HasMaxLength(100)
                .HasColumnName("table_designation");
            entity.Property(e => e.TableId)
                .HasColumnType("int(11)")
                .HasColumnName("table_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<UserAppMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_app_message");

            entity.HasIndex(e => e.IdAppRecepient, "user_app_message_FK__applications_recipient");

            entity.HasIndex(e => e.IdAppSender, "user_app_message_FK__applications_sender");

            entity.HasIndex(e => e.IdUserSender, "user_app_message_FK_user");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(2000)
                .HasColumnName("content");
            entity.Property(e => e.IdAppRecepient)
                .HasColumnType("int(11)")
                .HasColumnName("id_app_recepient");
            entity.Property(e => e.IdAppSender)
                .HasColumnType("int(11)")
                .HasColumnName("id_app_sender");
            entity.Property(e => e.IdUserSender)
                .HasColumnType("int(11)")
                .HasColumnName("id_user_sender");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("sent_date");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");

            entity.HasOne(d => d.IdUserSenderNavigation).WithMany(p => p.UserAppMessages)
                .HasForeignKey(d => d.IdUserSender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_app_message_FK_user");
        });

        modelBuilder.Entity<UserAppPage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_app_page");

            entity.HasIndex(e => e.Page, "NewTable_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Page)
                .HasMaxLength(10)
                .HasColumnName("page");
        });

        modelBuilder.Entity<UserAppPageDroit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_app_page_droit");

            entity.HasIndex(e => e.IdApp, "user_app_right__applications_FK");

            entity.HasIndex(e => new { e.IdUser, e.IdApp, e.Page }, "user_app_right_unique").IsUnique();

            entity.HasIndex(e => e.Page, "user_app_right_user_app_page_FK");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.IdApp)
                .HasColumnType("int(11)")
                .HasColumnName("id_app");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_user");
            entity.Property(e => e.Page)
                .HasMaxLength(10)
                .HasColumnName("page");
            entity.Property(e => e.UserCanAccess).HasColumnName("user_can_access");
            entity.Property(e => e.UserCanAdmin).HasColumnName("user_can_admin");
            entity.Property(e => e.UserCanControl).HasColumnName("user_can_control");
            entity.Property(e => e.UserCanCreate).HasColumnName("user_can_create");
            entity.Property(e => e.UserCanDelete).HasColumnName("user_can_delete");
            entity.Property(e => e.UserCanMonitor).HasColumnName("user_can_monitor");
            entity.Property(e => e.UserCanRead).HasColumnName("user_can_read");
            entity.Property(e => e.UserCanSupervise).HasColumnName("user_can_supervise");
            entity.Property(e => e.UserCanUpdate).HasColumnName("user_can_update");
            entity.Property(e => e.UserCanValidate).HasColumnName("user_can_validate");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserAppPageDroits)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_app_right_user_FK");

            entity.HasOne(d => d.PageNavigation).WithMany(p => p.UserAppPageDroits)
                .HasPrincipalKey(p => p.Page)
                .HasForeignKey(d => d.Page)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_app_right_user_app_page_FK");
        });

        modelBuilder.Entity<UserDroit>(entity =>
        {
            entity.HasKey(e => e.IdTab).HasName("PRIMARY");

            entity.ToTable("user_droit");

            entity.Property(e => e.IdTab)
                .HasColumnType("int(11)")
                .HasColumnName("id_tab");
            entity.Property(e => e.IdAction)
                .HasColumnType("int(11)")
                .HasColumnName("id_action");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_user");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_session");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Connected).HasColumnName("connected");
            entity.Property(e => e.ConnectionDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("connection_date");
            entity.Property(e => e.DeviceId)
                .HasMaxLength(30)
                .HasColumnName("device_id");
            entity.Property(e => e.DeviceIp)
                .HasMaxLength(30)
                .HasColumnName("device_ip");
            entity.Property(e => e.DeviceUser)
                .HasMaxLength(30)
                .HasColumnName("device_user");
            entity.Property(e => e.DisconnectionDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("disconnection_date");
            entity.Property(e => e.IdApplication)
                .HasColumnType("int(11)")
                .HasColumnName("id_application");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_user");
        });

        modelBuilder.Entity<UserSessionCommand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_session_command");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CommandDate)
                .HasDefaultValueSql("'1970-01-01 00:00:00'")
                .HasColumnType("datetime")
                .HasColumnName("command_date");
            entity.Property(e => e.CommandType)
                .HasMaxLength(30)
                .HasColumnName("command_type");
            entity.Property(e => e.IdAppTarget)
                .HasColumnType("int(11)")
                .HasColumnName("id_app_target");
            entity.Property(e => e.IdUserIssuer)
                .HasColumnType("int(11)")
                .HasColumnName("id_user_issuer");
            entity.Property(e => e.IdUserTarget)
                .HasColumnType("int(11)")
                .HasColumnName("id_user_target");
        });

        modelBuilder.Entity<VieApplication>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vie_application");

            entity.Property(e => e.Accessible).HasColumnName("accessible");
            entity.Property(e => e.Fonction)
                .HasMaxLength(50)
                .HasColumnName("fonction");
            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.IdAppliParent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("id_appli_parent");
            entity.Property(e => e.Image)
                .HasMaxLength(100)
                .HasColumnName("image");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Ordre)
                .HasColumnType("mediumint(9)")
                .HasColumnName("ordre");
            entity.Property(e => e.PleinEcran)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("plein_ecran");
            entity.Property(e => e.TjsVisible)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("tjs_visible");
        });

        modelBuilder.Entity<VieChuteMagasinReference>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vie_chute_magasin_reference");

            entity.Property(e => e.Categorie1)
                .HasDefaultValueSql("'14'")
                .HasColumnType("int(2)")
                .HasColumnName("categorie1");
            entity.Property(e => e.Categorie2)
                .HasDefaultValueSql("'9'")
                .HasColumnType("int(2)")
                .HasColumnName("categorie2");
            entity.Property(e => e.Categorie3)
                .HasDefaultValueSql("'5'")
                .HasColumnType("int(2)")
                .HasColumnName("categorie3");
            entity.Property(e => e.Categorie4)
                .HasMaxLength(20)
                .HasDefaultValueSql("'NA'")
                .HasColumnName("categorie4");
            entity.Property(e => e.CodeBarre)
                .HasMaxLength(15)
                .HasColumnName("code_barre");
            entity.Property(e => e.Emplacement)
                .HasColumnType("int(11)")
                .HasColumnName("emplacement");
            entity.Property(e => e.EmplacementDesignation)
                .HasMaxLength(45)
                .HasColumnName("emplacement_designation");
            entity.Property(e => e.IdArticleInterne)
                .HasColumnType("int(11)")
                .HasColumnName("id_article_interne");
            entity.Property(e => e.IdChuteMagasin)
                .HasColumnType("int(11)")
                .HasColumnName("id_chute_magasin");
            entity.Property(e => e.LongueurBarre)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_barre");
            entity.Property(e => e.LongueurChuteMini)
                .HasPrecision(7, 3)
                .HasColumnName("longueur_chute_mini");
            entity.Property(e => e.OrdreTri)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(3)")
                .HasColumnName("ordre_tri");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
