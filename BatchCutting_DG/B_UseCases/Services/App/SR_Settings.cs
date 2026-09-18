using System.ComponentModel;
using CommonResources.Settings;
using CommonResources.Utilities;
using BatchCutting_DG.A_Domain.AppEntities;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.B_UseCases.Settings;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.D_Presentation.Settings;

namespace BatchCutting_DG.B_UseCases.Services.App
{
    public class SR_Settings : IS_Settings
    {
        // CommonResources.Settings
        public string GetApplicationTitle() => CR_CommonSettings.ApplicationTitle;
        public void SetApplicationTitle(string appTitle) => CR_CommonSettings.ApplicationTitle = appTitle;
        public string GetCredentialsGeststock_prod() => CR_DataBaseSettings.CredentialsGeststock_prod;
        public string GetCredentialsGeststock_dev() => CR_DataBaseSettings.CredentialsGeststock_dev;


        // BatchCutting_DG.D_Presentation.Services
        public Uri GetDicEn() => SE_Dictionary.Dic_en;
        public void SetDicEn(Uri value) => SE_Dictionary.Dic_en = value;

        public Uri GetDicFr() => SE_Dictionary.Dic_fr;
        public void SetDicFr(Uri value) => SE_Dictionary.Dic_fr = value;

        public Uri GetDicDe() => SE_Dictionary.Dic_de;
        public void SetDicDe(Uri value) => SE_Dictionary.Dic_de = value;

        public Uri GetDicEs() => SE_Dictionary.Dic_es;
        public void SetDicEs(Uri value) => SE_Dictionary.Dic_es = value;

        public Uri GetDicIt() => SE_Dictionary.Dic_it;
        public void SetDicIt(Uri value) => SE_Dictionary.Dic_it = value;

        public Uri GetDicPt() => SE_Dictionary.Dic_pt;
        public void SetDicPt(Uri value) => SE_Dictionary.Dic_pt = value;

        public string GetAppCultureCode() => SE_Dictionary.AppCultureCode;
        public void SetAppCultureCode(string value) => SE_Dictionary.AppCultureCode = value;


        //BatchCutting_DG.B_UseCases.Settings.DeviceInfo
        public string GetCRDeviceUser() => DeviceInfo.DeviceUser;
        public string GetCRDeviceID() => DeviceInfo.DeviceID;
        public string GetCRDeviceIP() => DeviceInfo.DeviceIP;


        // BatchCutting_DG.B_UseCases.Settings.AppSettings
        public int GetAppID() => SE_App.AppID;
        public int GetAppAccess() => SE_App.AppAccess;
        public Uri GetCommonRessourcesPath() => SE_App.CommonRessourcesPath;
        public DateTime GetAppDate() => SE_App.AppDate;
        public DateTime GetAppDateTime() => SE_App.AppDateTime;
        public int GetShowDialogWindowDelay() => SE_App.ShowDialogWindowDelay;
        public int GetCloseCommandDelay() => SE_App.CloseCommandDelay;
        public int GetMessageCheckDelay() => SE_App.MessageCheckDelay;
        public int GetMessageNotificationDelay() => SE_App.MessageNotificationDelay;
        

        // BatchCutting_DG.B_UseCases.Settings.UserSettings
        public int GetAppUserID() => SE_User.AppUserID;
        public void SetAppUserID(int userID) => SE_User.AppUserID = userID;

        public string GetAppUserFullName() => SE_User.AppUserFullName;
        public void SetAppUserFullName(string fullName)
        {
            if (SE_User.AppUserFullName != fullName)
            {
                SE_User.AppUserFullName = fullName;
                SE_User.OnPropertyChanged(nameof(SE_User.AppUserFullName));
            }
        }

        public string GetAppDeviceUser() => SE_User.AppDeviceUser;
        public void SetAppDeviceUser() => SE_User.AppDeviceUser = DeviceInfo.DeviceUser;

        public string GetAppDeviceID() => SE_User.AppDeviceID;
        public void SetAppDeviceID() => SE_User.AppDeviceID = DeviceInfo.DeviceID;

        public string GetAppDeviceIP() => SE_User.AppDeviceIP;
        public void SetAppDeviceIP() => SE_User.AppDeviceIP = DeviceInfo.DeviceIP;

        public int GetUserAttempt() => SE_User.UserAttempt;
        public void SetUserAttempt(int attempt) => SE_User.UserAttempt = attempt;
        public void IncrementUserAttempt()
        {
            SE_User.UserAttempt++;
        }

        public string GetCloseCommandType() => SE_User.CloseCommandType;

        public bool GetForceClose() => SE_User.ForceClose;
        public void SetForceClose(bool forceClose) => SE_User.ForceClose = forceClose;

        public int GetSessionId() => SE_User.SessionId;
        public void SetSessionId(int sessionId) => SE_User.SessionId = sessionId;

        public int GetSelectedSessionId() => SE_User.SelectedSessionId;
        public void SetSelectedSessionId(int selectedSessionId) => SE_User.SelectedSessionId = selectedSessionId;

        public string GetSelectedSessionFullName() => SE_User.SelectedSessionFullName;
        public void SetSelectedSessionFullName(string fullName) => SE_User.SelectedSessionFullName = fullName;

        public bool GetCanUserAccessApp() => SE_User.CanUserAccessApp;
        public void SetCanUserAccessApp(bool value) => SE_User.CanUserAccessApp = value;


        // BatchCutting_DG.B_UseCases.Settings.UseCasesSettings
        public string GetCuttingMachine() => SE_UseCases.CuttingMachine;
        public void SetCuttingMachine(string cuttingMachineDesignation) => SE_UseCases.CuttingMachine = cuttingMachineDesignation;

        public string GetLabelPrinter() => SE_UseCases.LabelPrinter;
        public void SetLabelPrinter(string labelPrinter) => SE_UseCases.LabelPrinter = labelPrinter;

        public string GetLocalSerialPort() => SE_UseCases.LocalSerialPort;
        public void SetLocalSerialPort(string localSerialPort) => SE_UseCases.LocalSerialPort = localSerialPort;

        public int GetDecoupeLotId() => SE_UseCases.DecoupeLotId;
        public void SetDecoupeLotId(int decoupeLotId) => SE_UseCases.DecoupeLotId = decoupeLotId;

        public List<DTO_DecoupeLotWithCut>? GetDecoupeLotWithCut() => SE_UseCases.DecoupeLotWithCut;
        public void SetDecoupeLotWithCut(List<DTO_DecoupeLotWithCut>? decoupeLotWithCut) => SE_UseCases.DecoupeLotWithCut = decoupeLotWithCut;

        public int GetDecoupeBarreId() => SE_UseCases.DecoupeBarreId;
        public void SetDecoupeBarreId(int decoupeBarreId) => SE_UseCases.DecoupeBarreId = decoupeBarreId;

        public int GetDecoupeBarreLongueurChuteFinale() => SE_UseCases.DecoupeBarreLongueurChuteFinale;
        public void SetDecoupeBarreLongueurChuteFinale(int decoupeBarreLongueurChuteFinale) => SE_UseCases.DecoupeBarreLongueurChuteFinale = decoupeBarreLongueurChuteFinale;

        public DTO_DecoupeBarreWithCut? GetDecoupeBarreWithCut() => SE_UseCases.DecoupeBarreWithCut;
        public void SetDecoupeBarreWithCut(DTO_DecoupeBarreWithCut? decoupeBarreWithCut) => SE_UseCases.DecoupeBarreWithCut = decoupeBarreWithCut;

        public string? GetDecoupeBarreDecoupeCommentaires() => SE_UseCases.DecoupeBarreDecoupeCommentaires;
        public void SetDecoupeBarreDecoupeCommentaires(string? decoupeBarreDecoupeCommentaires) => SE_UseCases.DecoupeBarreDecoupeCommentaires = decoupeBarreDecoupeCommentaires;

        public int GetDecoupeDetailId() => SE_UseCases.DecoupeDetailId;
        public void SetDecoupeDetailId(int decoupeDetailId) => SE_UseCases.DecoupeDetailId = decoupeDetailId;

        public string? GetDecoupeDetailReferenceVue() => SE_UseCases.DecoupeDetailReferenceVue;
        public void SetDecoupeDetailReferenceVue(string? decoupeDetailReferenceVue) => SE_UseCases.DecoupeDetailReferenceVue = decoupeDetailReferenceVue;

        public DTO_DecoupeDetailWithCut? GetDecoupeDetailWithCut() => SE_UseCases.DecoupeDetailWithCut;
        public void SetDecoupeDetailWithCut(DTO_DecoupeDetailWithCut? decoupeDetailWithCut) => SE_UseCases.DecoupeDetailWithCut = decoupeDetailWithCut;

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

        public int GetStatutSecondaireDecoupeDG244_01() => SE_UseCases.StatutSecondaireDecoupeDG244_01;
        public int GetStatutSecondaireDecoupeDG244_02() => SE_UseCases.StatutSecondaireDecoupeDG244_02;
        public int GetStatutSecondaireDecoupeDG244_03() => SE_UseCases.StatutSecondaireDecoupeDG244_03;

        public int GetNewBarId() => SE_UseCases.NewBarId;
        public void SetNewBarId(int newBarId) => SE_UseCases.NewBarId = newBarId;

        public int GetNewBarIndex() => SE_UseCases.NewBarIndex;
        public void SetNewBarIndex(int newBarIndex) => SE_UseCases.NewBarIndex = newBarIndex;

        public decimal GetSpaceBetweenCuts() => SE_UseCases.SpaceBetweenCuts;

        public decimal GetRemainingBarLength() => SE_UseCases.RemainingBarLength;
        public void SetRemainingBarLength(decimal remainingBarLength) => SE_UseCases.RemainingBarLength = remainingBarLength;






        // Relayer les abonnements à PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add => SE_User.PropertyChanged += value;
            remove => SE_User.PropertyChanged -= value;
        }

        // Méthode pour déclencher l'événement
        public void UpdateUserSetting<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                SE_User.OnPropertyChanged(propertyName);
            }
        }


        // Droit d'action de l'utilsateur
        public void InitializeUserDefaultAccesses() => SE_User.InitializeDefaultPageAccessRights();
        public List<KeyValuePair<string, PageRights>> GetAllPagesUserRights()
        {
            return SE_User.PagesUserRights.ToList();
        }

        public void SetPageAccessRights(List<UserAppPageDroit> pageAccessRights) => SE_User.SetUserPageAccessRights(pageAccessRights);
        public PageRights? GetPageRights(string pageName) => SE_User.GetPageRights(pageName);


        // Acces base de données
        public string GetCredentialsGeststock()
        {
            return SE_App.Environment == "Prod"
                ? CR_DataBaseSettings.CredentialsGeststock_prod
                : CR_DataBaseSettings.CredentialsGeststock_dev;
        }
    }
}