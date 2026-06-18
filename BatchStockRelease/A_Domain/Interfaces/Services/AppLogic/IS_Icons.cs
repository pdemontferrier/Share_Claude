
namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    public interface IS_Icons
    {
        // Icons Email
        Uri GetEmailIcon_Source();
        Uri GetEmailNotReadIcon_Source();
        Uri GetAppCloseBlue_Source();

        // Menu Horizontal Buttons Source
        Uri GetMH_Menu_Source();
        Uri GetMH_Update_Source();
        Uri GetMH_ReturnBack_Source();
        Uri GetMH_Refresh_Source();
        Uri GetMH_Import_Source();
        Uri GetMH_Save_Source();
        Uri GetMH_SaveUpdate_Source();
        Uri GetMH_Add_Source();
        Uri GetMH_Delete_Source();
        Uri GetMH_Duplicate_Source();
        Uri GetMH_Previous_Source();
        Uri GetMH_Details_Source();
        Uri GetMH_Logs_Source();
        Uri GetMH_Active_Source();
        Uri GetMH_Validate_Source();
        Uri GetMH_User_Source();
        Uri GetMH_Home_Source();
        Uri GetMH_Admin_Source();
        Uri GetMH_WarningTriangleOrange_Source();
        Uri GetMH_WarningTriangleRed_Source();

        // Menu Vertical Buttons Source
        Uri GetMV1_Source();
        Uri GetMV2_Source();
        Uri GetMV3_Source();
        Uri GetMV4_Source();
        Uri GetMV5_Source();
        Uri GetMV6_Source();
        Uri GetMV7_Source();
        Uri GetMVU_Source();

        // Logo
        Uri GetLogo_Col_Source();
        Uri GetLogo_BW_Source();
        Uri GetLogo_WB_Source();

        // Logos pour impression
        Uri GetPrint_Logo_Col_Source();
        Uri GetPrint_Logo_BW_Source();
        Uri GetPrint_Logo_WB_Source();
    }
}