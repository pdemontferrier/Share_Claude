using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.B_UseCases.Settings.BusinessLogic
{
    public class SE_UseCases
    {
        // Référence Lot sélectionné
        public static int DecoupeLotId { get; set; }
        public static string? DecoupeLotDesignation = string.Empty;
        public static string? DecoupeLotCouleur = string.Empty;
        public static int DecoupeLotEcheance { get; set; }

        // Chemin de base pour la validation d'un lot
        public static string? DirectoryPathBatch = string.Empty;
        public static string? DirectoryPathBatchFile = string.Empty;
        public static string? BatchFileName1 = string.Empty;
        public static readonly string BatchFileName2 = "_Liste_Debit";
        public static readonly string BatchFileSheet1 = "decoupe_detail";
        public static readonly string BatchFileSheet2 = "decoupe_barre";

        // Référence Barre de découpe
        public static int DecoupeBarreId { get; set; }
        public static int DecoupeBarreLongueurChuteFinale { get; set; }
        public static string? DecoupeBarreDecoupeCommentaires = string.Empty;
        public static int DecoupeBarreIdChariot { get; set; }
        public static string? DecoupeBarreChariotDesignation = string.Empty;
        public static int DecoupeBarreIdStock { get; set; }
        public static int DecoupeBarreStockQuantite { get; set; }

        // Référence aux détails de la découpe
        public static int DecoupeDetailId { get; set; }
        public static string? DecoupeDetailReferenceVue = string.Empty;
        public static string? DecoupeDetailDecoupeCommentaires = string.Empty;

        // Référence Article Interne de la barre à découper
        public static int ArticleInterneId { get; set; }

        // Référence à l'approvisionnement de barre à découper
        public static int DecoupeBarreNewBarId { get; set; }
        public static int DecoupeNewBarreBarIndex { get; set; }
        public static readonly decimal DecoupeBarreSpaceBetweenCuts = 30;
        public static decimal DecoupeBarreRemainingBarLength = 0;

        // Référence aux détails de l'état du stock
        public static VieStockQuantiteEmplacement? StockQuantiteEmplacement;

        // Liste des actions CommandeClients
        public static readonly int ActionProjectImport = 23;
        public static readonly int ActionProjectControl = 24;
        public static readonly int ActionProjectValidation = 25;
        public static readonly int ActionBatchValidation = 26;
        public static readonly int ActionBarDropStockRelease = 27;
        public static readonly int ActionBarNewStockRelease = 28;
        public static readonly int ActionBarCutDG244_01 = 29;
        public static readonly int ActionBarCutDG244_02 = 30;
        public static readonly int ActionBarCutDG244_03 = 31;

        // Liste des statuts secondaires des CommandeClients
        public static readonly int StatutSecondaireApproChute = 1;
        public static readonly int StatutSecondaireApproNeuf = 5;
        public static readonly int StatutSecondaireDecoupeDG244_01 = 2;
        public static readonly int StatutSecondaireDecoupeDG244_02 = 6;
        public static readonly int StatutSecondaireDecoupeDG244_03 = 7;

    }
}