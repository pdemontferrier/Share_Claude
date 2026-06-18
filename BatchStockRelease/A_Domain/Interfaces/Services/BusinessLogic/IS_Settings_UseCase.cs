using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    public interface IS_Settings_UseCase
    {
        // BatchStockRelease.B_UseCases.Settings.BusinessLogic
        int GetDecoupeLotId();
        void SetDecoupeLotId(int decoupeLotId);
        string? GetDecoupeLotDesignation();
        void SetDecoupeLotDesignation(string? decoupeLotDesignation);
        string? GetDecoupeLotCouleur();
        void SetDecoupeLotCouleur(string? decoupeLotCouleur);
        int GetDecoupeLotEcheance();
        void SetDecoupeLotEcheance(int decoupeLotEcheance);

        string? GetDirectoryPathBatch();
        void SetDirectoryPathBatch(string? directoryPathBatch);
        string? GetDirectoryPathBatchFile();
        void SetDirectoryPathBatchFile(string? directoryPathBatchFile);
        string? GetBatchFileName1();
        void SetBatchFileName1(string? batchFileName1);
        string? GetBatchFileName2();
        string? GetBatchFileSheet1();
        string? GetBatchFileSheet2();

        int GetDecoupeBarreId();
        void SetDecoupeBarreId(int decoupeBarreId);
        int GetDecoupeBarreLongueurChuteFinale();
        void SetDecoupeBarreLongueurChuteFinale(int decoupeBarreLongueurChuteFinale);
        string? GetDecoupeBarreDecoupeCommentaires();
        void SetDecoupeBarreDecoupeCommentaires(string? decoupeBarreDecoupeCommentaires);
        int GetDecoupeBarreIdChariot();
        void SetDecoupeBarreIdChariot(int decoupeBarreIdChariot);
        string? GetDecoupeBarreChariotDesignation();
        void SetDecoupeBarreChariotDesignation(string? decoupeBarreChariotDesignation);
        int GetDecoupeBarreIdStock();
        void SetDecoupeBarreIdStock(int decoupeBarreIdStock);
        int GetDecoupeBarreStockQuantite();
        void SetDecoupeBarreStockQuantite(int decoupeBarreStockQuantite);

        int GetDecoupeDetailId();
        void SetDecoupeDetailId(int decoupeDetailId);
        string? GetDecoupeDetailReferenceVue();
        void SetDecoupeDetailReferenceVue(string? decoupeDetailReferenceVue);
        string? GetDecoupeDetailDecoupeCommentaires();
        void SetDecoupeDetailDecoupeCommentaires(string? decoupeDetailDecoupeCommentaires);

        int GetArticleInterneId();
        void SetArticleInterneId(int articleInterneId);

        int GetActionProjectImport();
        int GetActionProjectControl();
        int GetActionProjectValidation();
        int GetActionBatchValidation();
        int GetActionBarDropStockRelease();
        int GetActionBarNewStockRelease();
        int GetActionBarCutDG244_01();
        int GetActionBarCutDG244_02();
        int GetActionBarCutDG244_03();

        int GetStatutSecondaireApproChute();
        int GetStatutSecondaireApproNeuf();
        int GetStatutSecondaireDecoupeDG244_01();
        int GetStatutSecondaireDecoupeDG244_02();
        int GetStatutSecondaireDecoupeDG244_03();

        int GetDecoupeBarreNewBarId();
        void SetDecoupeBarreNewBarId(int decoupeBarreNewBarId);

        int GetDecoupeBarreNewBarIndex();
        void SetDecoupeBarreNewBarIndex(int decoupeBarreNewBarIndex);

        decimal DecoupeBarreGetSpaceBetweenCuts();

        decimal GetDecoupeBarreRemainingBarLength();
        void SetDecoupeBarreRemainingBarLength(decimal decoupeBarreRemainingBarLength);


        VieStockQuantiteEmplacement? GetStockQuantiteEmplacement();
        void SetStockQuantiteEmplacement(VieStockQuantiteEmplacement? stockQuantiteEmplacement);

    }
}
