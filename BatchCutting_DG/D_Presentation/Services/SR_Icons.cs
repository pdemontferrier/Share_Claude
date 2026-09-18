using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Settings;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Icons : IS_Icons
    {
        // Icons Emails
        public Uri GetEmailIcon_Source() => SE_Icons.IconEmail_Source;
        public Uri GetEmailNotReadIcon_Source() => SE_Icons.IconEmailNotRead_Source;
        public Uri GetAppCloseBlue_Source() => SE_Icons.AppCloseBlue_Source;


        // Menu Horizontal Buttons Source
        public Uri GetMH_Menu_Source() => SE_Icons.MH_Menu_Source;
        public Uri GetMH_Update_Source() => SE_Icons.MH_Update_Source;
        public Uri GetMH_ReturnBack_Source() => SE_Icons.MH_ReturnBack_Source;
        public Uri GetMH_Refresh_Source() => SE_Icons.MH_Refresh_Source;
        public Uri GetMH_Import_Source() => SE_Icons.MH_Import_Source;
        public Uri GetMH_Save_Source() => SE_Icons.MH_Save_Source;
        public Uri GetMH_Add_Source() => SE_Icons.MH_Add_Source;
        public Uri GetMH_Delete_Source() => SE_Icons.MH_Delete_Source;
        public Uri GetMH_Duplicate_Source() => SE_Icons.MH_Duplicate_Source;
        public Uri GetMH_Previous_Source() => SE_Icons.MH_Previous_Source;
        public Uri GetMH_Details_Source() => SE_Icons.MH_Details_Source;
        public Uri GetMH_Logs_Source() => SE_Icons.MH_Logs_Source;
        public Uri GetMH_Active_Source() => SE_Icons.MH_Active_Source;
        public Uri GetMH_Validate_Source() => SE_Icons.MH_Validate_Source;
        public Uri GetMH_User_Source() => SE_Icons.MH_User_Source;
        public Uri GetMH_Home_Source() => SE_Icons.MH_Home_Source;
        public Uri GetMH_Admin_Source() => SE_Icons.MH_Admin_Source;


        // Menu Vertical Buttons Source
        public Uri GetMV1_Source() => SE_Icons.MV1_Source;
        public Uri GetMV2_Source() => SE_Icons.MV2_Source;
        public Uri GetMV3_Source() => SE_Icons.MV3_Source;
        public Uri GetMV4_Source() => SE_Icons.MV4_Source;
        public Uri GetMV5_Source() => SE_Icons.MV5_Source;
        public Uri GetMV6_Source() => SE_Icons.MV6_Source;
        public Uri GetMV7_Source() => SE_Icons.MV7_Source;
        public Uri GetMVU_Source() => SE_Icons.MVU_Source;

        // Logo
        public Uri GetLogo_Col_Source() => SE_Icons.Logo_Col_Source;
        public Uri GetLogo_BW_Source() => SE_Icons.Logo_BW_Source;
        public Uri GetLogo_WB_Source() => SE_Icons.Logo_WB_Source;

        // Logos pour impression
        public Uri GetPrint_Logo_Col_Source() => SE_Icons.Print_Logo_Col_Source;
        public Uri GetPrint_Logo_BW_Source() => SE_Icons.Print_Logo_BW_Source;
        public Uri GetPrint_Logo_WB_Source() => SE_Icons.Print_Logo_WB_Source;

    }
}