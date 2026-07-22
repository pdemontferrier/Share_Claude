using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.B_UseCases.Settings.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    public class SR_Settings_UseCase : IS_Settings_UseCase
    {
        // BatchStockRelease.B_UseCases.Settings.BusinessLogic
        public int GetDecoupeLotId() => SE_UseCases.DecoupeLotId;
        public void SetDecoupeLotId(int decoupeLotId) => SE_UseCases.DecoupeLotId = decoupeLotId;
        public string? GetDecoupeLotDesignation() => SE_UseCases.DecoupeLotDesignation;
        public void SetDecoupeLotDesignation(string? decoupeLotDesignation) => SE_UseCases.DecoupeLotDesignation = decoupeLotDesignation;
        public string? GetDecoupeLotCouleur() => SE_UseCases.DecoupeLotCouleur;
        public void SetDecoupeLotCouleur(string? decoupeLotCouleur) => SE_UseCases.DecoupeLotCouleur = decoupeLotCouleur;
        public int GetDecoupeLotEcheance() => SE_UseCases.DecoupeLotEcheance;
        public void SetDecoupeLotEcheance(int decoupeLotEcheance) => SE_UseCases.DecoupeLotEcheance = decoupeLotEcheance;

        public string? GetDirectoryPathBatch() => SE_UseCases.DirectoryPathBatch;
        public void SetDirectoryPathBatch(string? directoryPathBatch) => SE_UseCases.DirectoryPathBatch = directoryPathBatch;
        public string? GetDirectoryPathBatchFile() => SE_UseCases.DirectoryPathBatchFile;
        public void SetDirectoryPathBatchFile(string? directoryPathBatchFile) => SE_UseCases.DirectoryPathBatchFile = directoryPathBatchFile;
        public string? GetBatchFileName1() => SE_UseCases.BatchFileName1;
        public void SetBatchFileName1(string? batchFileName1) => SE_UseCases.BatchFileName1 = batchFileName1;
        public string? GetBatchFileName2() => SE_UseCases.BatchFileName2;
        public string? GetBatchFileSheet1() => SE_UseCases.BatchFileSheet1;
        public string? GetBatchFileSheet2() => SE_UseCases.BatchFileSheet2;


        public int GetDecoupeBarreId() => SE_UseCases.DecoupeBarreId;
        public void SetDecoupeBarreId(int decoupeBarreId) => SE_UseCases.DecoupeBarreId = decoupeBarreId;
        public int GetDecoupeBarreLongueurChuteFinale() => SE_UseCases.DecoupeBarreLongueurChuteFinale;
        public void SetDecoupeBarreLongueurChuteFinale(int decoupeBarreLongueurChuteFinale) => SE_UseCases.DecoupeBarreLongueurChuteFinale = decoupeBarreLongueurChuteFinale;
        public string? GetDecoupeBarreDecoupeCommentaires() => SE_UseCases.DecoupeBarreDecoupeCommentaires;
        public void SetDecoupeBarreDecoupeCommentaires(string? decoupeBarreDecoupeCommentaires) => SE_UseCases.DecoupeBarreDecoupeCommentaires = decoupeBarreDecoupeCommentaires;
        public int GetDecoupeBarreIdChariot() => SE_UseCases.DecoupeBarreIdChariot;
        public void SetDecoupeBarreIdChariot(int decoupeBarreIdChariot) => SE_UseCases.DecoupeBarreIdChariot = decoupeBarreIdChariot;
        public string? GetDecoupeBarreChariotDesignation() => SE_UseCases.DecoupeBarreChariotDesignation;
        public void SetDecoupeBarreChariotDesignation(string? decoupeBarreChariotDesignation) => SE_UseCases.DecoupeBarreChariotDesignation = decoupeBarreChariotDesignation;
        public int GetDecoupeBarreIdStock() => SE_UseCases.DecoupeBarreIdStock;
        public void SetDecoupeBarreIdStock(int decoupeBarreIdStock) => SE_UseCases.DecoupeBarreIdStock = decoupeBarreIdStock;
        public int GetDecoupeBarreStockQuantite() => SE_UseCases.DecoupeBarreStockQuantite;
        public void SetDecoupeBarreStockQuantite(int decoupeBarreStockQuantite) => SE_UseCases.DecoupeBarreStockQuantite = decoupeBarreStockQuantite;

        public int GetDecoupeBarreNewBarId() => SE_UseCases.DecoupeBarreNewBarId;
        public void SetDecoupeBarreNewBarId(int decoupeBarreNewBarId) => SE_UseCases.DecoupeBarreNewBarId = decoupeBarreNewBarId;
        public int GetDecoupeBarreNewBarIndex() => SE_UseCases.DecoupeNewBarreBarIndex;
        public void SetDecoupeBarreNewBarIndex(int decoupeBarreNewBarIndex) => SE_UseCases.DecoupeNewBarreBarIndex = decoupeBarreNewBarIndex;
        public decimal DecoupeBarreGetSpaceBetweenCuts() => SE_UseCases.DecoupeBarreSpaceBetweenCuts;
        public decimal GetDecoupeBarreRemainingBarLength() => SE_UseCases.DecoupeBarreRemainingBarLength;
        public void SetDecoupeBarreRemainingBarLength(decimal decoupeBarreRemainingBarLength) => SE_UseCases.DecoupeBarreRemainingBarLength = decoupeBarreRemainingBarLength;


        public int GetDecoupeDetailId() => SE_UseCases.DecoupeDetailId;
        public void SetDecoupeDetailId(int decoupeDetailId) => SE_UseCases.DecoupeDetailId = decoupeDetailId;
        public string? GetDecoupeDetailReferenceVue() => SE_UseCases.DecoupeDetailReferenceVue;
        public void SetDecoupeDetailReferenceVue(string? decoupeDetailReferenceVue) => SE_UseCases.DecoupeDetailReferenceVue = decoupeDetailReferenceVue;
        public string? GetDecoupeDetailDecoupeCommentaires() => SE_UseCases.DecoupeDetailDecoupeCommentaires;
        public void SetDecoupeDetailDecoupeCommentaires(string? decoupeDetailDecoupeCommentaires) => SE_UseCases.DecoupeDetailDecoupeCommentaires = decoupeDetailDecoupeCommentaires;


        public int GetArticleInterneId() => SE_UseCases.ArticleInterneId;
        public void SetArticleInterneId(int articleInterneId) => SE_UseCases.ArticleInterneId = articleInterneId;


        public int GetActionProjectImport() => SE_UseCases.ActionProjectImport;
        public int GetActionProjectControl() => SE_UseCases.ActionProjectControl;
        public int GetActionProjectValidation() => SE_UseCases.ActionProjectValidation;
        public int GetActionBatchValidation() => SE_UseCases.ActionBatchValidation;
        public int GetActionBarDropStockRelease() => SE_UseCases.ActionBarDropStockRelease;
        public int GetActionBarNewStockRelease() => SE_UseCases.ActionBarNewStockRelease;
        public int GetActionBarCutDG244_01() => SE_UseCases.ActionBarCutDG244_01;
        public int GetActionBarCutDG244_02() => SE_UseCases.ActionBarCutDG244_02;
        public int GetActionBarCutDG244_03() => SE_UseCases.ActionBarCutDG244_03;


        public int GetStatutSecondaireApproChute() => SE_UseCases.StatutSecondaireApproChute;
        public int GetStatutSecondaireApproNeuf() => SE_UseCases.StatutSecondaireApproNeuf;
        public int GetStatutSecondaireDecoupeDG244_01() => SE_UseCases.StatutSecondaireDecoupeDG244_01;
        public int GetStatutSecondaireDecoupeDG244_02() => SE_UseCases.StatutSecondaireDecoupeDG244_02;
        public int GetStatutSecondaireDecoupeDG244_03() => SE_UseCases.StatutSecondaireDecoupeDG244_03;


        public VieStockQuantiteEmplacement? GetStockQuantiteEmplacement() => SE_UseCases.StockQuantiteEmplacement;
        public void SetStockQuantiteEmplacement(VieStockQuantiteEmplacement? stockQuantiteEmplacement) => SE_UseCases.StockQuantiteEmplacement = stockQuantiteEmplacement;
    }
}