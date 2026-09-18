using System.ComponentModel;
using BatchCutting_DG.A_Domain.AppEntities;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Entities.GestStock;

namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Settings
    {
        // CommonResources.Settings
        string GetApplicationTitle();
        void SetApplicationTitle(string appTitle);
        string GetCredentialsGeststock_prod();
        string GetCredentialsGeststock_dev();

        // BatchCutting_DG.D_Presentation.Services
        Uri GetDicEn();
        void SetDicEn(Uri value);

        Uri GetDicFr();
        void SetDicFr(Uri value);

        Uri GetDicDe();
        void SetDicDe(Uri value);

        Uri GetDicEs();
        void SetDicEs(Uri value);

        Uri GetDicIt();
        void SetDicIt(Uri value);

        Uri GetDicPt();
        void SetDicPt(Uri value);

        string GetAppCultureCode();
        void SetAppCultureCode(string value);

        //BatchCutting_DG.B_UseCases.Settings.DeviceInfo
        string GetCRDeviceID();
        string GetCRDeviceIP();
        string GetCRDeviceUser();

        // BatchCutting_DG.B_UseCases.Settings.AppSettings
        int GetAppID();
        int GetAppAccess();
        Uri GetCommonRessourcesPath();
        DateTime GetAppDate();
        DateTime GetAppDateTime();
        int GetShowDialogWindowDelay();
        int GetCloseCommandDelay();
        int GetMessageCheckDelay();
        int GetMessageNotificationDelay();

        // BatchCutting_DG.B_UseCases.Settings.UserSettings
        int GetAppUserID();
        void SetAppUserID(int userID);

        string GetAppUserFullName();
        void SetAppUserFullName(string fullName);

        string GetAppDeviceUser();
        void SetAppDeviceUser();

        string GetAppDeviceID();
        void SetAppDeviceID();

        string GetAppDeviceIP();
        void SetAppDeviceIP();

        int GetUserAttempt();
        void SetUserAttempt(int attempt);
        void IncrementUserAttempt();

        string GetCloseCommandType();
        bool GetForceClose();
        void SetForceClose(bool forceClose);

        int GetSessionId();
        void SetSessionId(int sessionId);

        int GetSelectedSessionId();
        void SetSelectedSessionId(int selectedSessionId);

        string GetSelectedSessionFullName();
        void SetSelectedSessionFullName(string fullName);

        bool GetCanUserAccessApp();
        void SetCanUserAccessApp(bool value);


        // BatchCutting_DG.B_UseCases.Settings.UseCasesSettings
        string GetCuttingMachine();
        void SetCuttingMachine(string cuttingMachineDesignation);

        string GetLabelPrinter();
        void SetLabelPrinter(string labelPrinterDesignation);

        string GetLocalSerialPort();
        void SetLocalSerialPort(string localSerialPort);

        int GetDecoupeLotId();
        void SetDecoupeLotId(int decoupeLotId);

        List<DTO_DecoupeLotWithCut>? GetDecoupeLotWithCut();
        void SetDecoupeLotWithCut(List<DTO_DecoupeLotWithCut>? decoupeLotWithCut);

        int GetDecoupeBarreId();
        void SetDecoupeBarreId(int decoupeBarreId);

        int GetDecoupeBarreLongueurChuteFinale();
        void SetDecoupeBarreLongueurChuteFinale(int decoupeBarreLongueurChuteFinale);

        DTO_DecoupeBarreWithCut? GetDecoupeBarreWithCut();
        void SetDecoupeBarreWithCut(DTO_DecoupeBarreWithCut? decoupeBarreWithCut);

        string? GetDecoupeBarreDecoupeCommentaires();
        void SetDecoupeBarreDecoupeCommentaires(string? decoupeBarreDecoupeCommentaires);

        int GetDecoupeDetailId();
        void SetDecoupeDetailId(int decoupeDetailId);

        string? GetDecoupeDetailReferenceVue();
        void SetDecoupeDetailReferenceVue(string? decoupeDetailReferenceVue);

        DTO_DecoupeDetailWithCut? GetDecoupeDetailWithCut();
        void SetDecoupeDetailWithCut(DTO_DecoupeDetailWithCut? decoupeDetailWithCut);

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

        int GetStatutSecondaireDecoupeDG244_01();
        int GetStatutSecondaireDecoupeDG244_02();
        int GetStatutSecondaireDecoupeDG244_03();

        int GetNewBarId();
        void SetNewBarId(int newBarId);

        int GetNewBarIndex();
        void SetNewBarIndex(int newBarIndex);

        decimal GetSpaceBetweenCuts();

        decimal GetRemainingBarLength();
        void SetRemainingBarLength(decimal remainingBarLength);



        // Relayer les abonnements à PropertyChanged
        event PropertyChangedEventHandler? PropertyChanged;

        // Droit d'action de l'utilsateur
        void InitializeUserDefaultAccesses();
        List<KeyValuePair<string, PageRights>> GetAllPagesUserRights();
        void SetPageAccessRights(List<UserAppPageDroit> pageAccessRights);
        public PageRights? GetPageRights(string pageName);


        public string GetCredentialsGeststock();
    }
}
