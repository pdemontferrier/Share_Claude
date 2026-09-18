using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.B_UseCases.Settings
{
    public class SE_UseCases
    {
        // Machine de découpe DG
        public static string CuttingMachine = string.Empty;

        // Imprimante locale
        public static string LabelPrinter = string.Empty;

        // Port COM local PC
        public static string LocalSerialPort = string.Empty;

        // Référence Lot sélectionné
        public static int DecoupeLotId { get; set; }
        public static List<DTO_DecoupeLotWithCut>? DecoupeLotWithCut { get; set; }

        // Référence Barre de découpe
        public static int DecoupeBarreId { get; set; }
        public static int DecoupeBarreLongueurChuteFinale { get; set; }
        public static DTO_DecoupeBarreWithCut? DecoupeBarreWithCut { get; set; }
        public static string? DecoupeBarreDecoupeCommentaires = string.Empty;

        // Référence aux détails de la découpe
        public static int DecoupeDetailId { get; set; }
        public static string? DecoupeDetailReferenceVue = string.Empty;
        public static DTO_DecoupeDetailWithCut? DecoupeDetailWithCut { get; set; }
        public static string? DecoupeDetailDecoupeCommentaires = string.Empty;

        // Référence Article Interne de la barre à découper
        public static int ArticleInterneId { get; set; }

        // Référence à l'approvisionnement de barre à découper
        public static int NewBarId;
        public static int NewBarIndex;
        public static readonly decimal SpaceBetweenCuts = 30;
        public static decimal RemainingBarLength = 0;

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
        public static readonly int StatutSecondaireDecoupeDG244_01 = 2;
        public static readonly int StatutSecondaireDecoupeDG244_02 = 6;
        public static readonly int StatutSecondaireDecoupeDG244_03 = 7;

    }
}