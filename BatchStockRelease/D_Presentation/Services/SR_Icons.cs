using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.Settings;

namespace BatchStockRelease.D_Presentation.Services
{
    /// <summary>
    /// <b>SR_Icons</b>
    /// <para>
    /// Service applicatif chargé de fournir les <see cref="Uri"/> des icônes utilisées dans l'application BatchStockRelease.  
    /// Chaque méthode retourne l'URI d'une icône spécifique, centralisée dans le fichier de configuration <see cref="SE_Icons"/>.
    /// </para>
    /// <para>
    /// Ce service implémente <see cref="IS_Icons"/> et permet d'accéder aux icônes de manière standardisée depuis les vues et ViewModels.  
    /// En séparant la logique d’accès aux ressources de l’interface graphique, il favorise la maintenabilité et la réutilisabilité.
    /// </para>
    /// </summary>
    public class SR_Icons : IS_Icons
    {
        #region === Icons Emails ===

        /// <summary>
        /// Retourne l'icône utilisée pour les emails standards.
        /// </summary>
        public Uri GetEmailIcon_Source() => SE_Icons.IconEmail_Source;

        /// <summary>
        /// Retourne l'icône utilisée pour les emails non lus.
        /// </summary>
        public Uri GetEmailNotReadIcon_Source() => SE_Icons.IconEmailNotRead_Source;

        /// <summary>
        /// Retourne l'icône bleue utilisée pour le bouton de fermeture de l’application.
        /// </summary>
        public Uri GetAppCloseBlue_Source() => SE_Icons.AppCloseBlue_Source;

        #endregion

        #region === Menu Horizontal Buttons Source ===

        /// <summary>Retourne l'icône du bouton Menu horizontal.</summary>
        public Uri GetMH_Menu_Source() => SE_Icons.MH_Menu_Source;

        /// <summary>Retourne l'icône du bouton Mettre à jour.</summary>
        public Uri GetMH_Update_Source() => SE_Icons.MH_Update_Source;

        /// <summary>Retourne l'icône du bouton Retour arrière.</summary>
        public Uri GetMH_ReturnBack_Source() => SE_Icons.MH_ReturnBack_Source;

        /// <summary>Retourne l'icône du bouton Rafraîchir.</summary>
        public Uri GetMH_Refresh_Source() => SE_Icons.MH_Refresh_Source;

        /// <summary>Retourne l'icône du bouton Importer.</summary>
        public Uri GetMH_Import_Source() => SE_Icons.MH_Import_Source;

        /// <summary>Retourne l'icône du bouton Enregistrer.</summary>
        public Uri GetMH_Save_Source() => SE_Icons.MH_Save_Source;

        /// <summary>Retourne l'icône du bouton Sauvegarder &amp; Mettre à jour.</summary>
        public Uri GetMH_SaveUpdate_Source() => SE_Icons.MH_SaveUpdate_Source;

        /// <summary>Retourne l'icône du bouton Ajouter.</summary>
        public Uri GetMH_Add_Source() => SE_Icons.MH_Add_Source;

        /// <summary>Retourne l'icône du bouton Supprimer.</summary>
        public Uri GetMH_Delete_Source() => SE_Icons.MH_Delete_Source;

        /// <summary>Retourne l'icône du bouton Dupliquer.</summary>
        public Uri GetMH_Duplicate_Source() => SE_Icons.MH_Duplicate_Source;

        /// <summary>Retourne l'icône du bouton Précédente.</summary>
        public Uri GetMH_Previous_Source() => SE_Icons.MH_Previous_Source;

        /// <summary>Retourne l'icône du bouton Détails.</summary>
        public Uri GetMH_Details_Source() => SE_Icons.MH_Details_Source;

        /// <summary>Retourne l'icône du bouton Historique ou Logs.</summary>
        public Uri GetMH_Logs_Source() => SE_Icons.MH_Logs_Source;

        /// <summary>Retourne l'icône du bouton Activer.</summary>
        public Uri GetMH_Active_Source() => SE_Icons.MH_Active_Source;

        /// <summary>Retourne l'icône du bouton Valider.</summary>
        public Uri GetMH_Validate_Source() => SE_Icons.MH_Validate_Source;

        /// <summary>Retourne l'icône du bouton Utilisateur.</summary>
        public Uri GetMH_User_Source() => SE_Icons.MH_User_Source;

        /// <summary>Retourne l'icône du bouton Accueil.</summary>
        public Uri GetMH_Home_Source() => SE_Icons.MH_Home_Source;

        /// <summary>Retourne l'icône du bouton Administrateur.</summary>
        public Uri GetMH_Admin_Source() => SE_Icons.MH_Admin_Source;

        /// <summary>Retourne l'icône d'avertissement orange (triangle).</summary>
        public Uri GetMH_WarningTriangleOrange_Source() => SE_Icons.MH_WarningTriangleOrange_Source;

        /// <summary>Retourne l'icône d'avertissement rouge (triangle).</summary>
        public Uri GetMH_WarningTriangleRed_Source() => SE_Icons.MH_WarningTriangleRed_Source;

        #endregion

        #region === Menu Vertical Buttons Source ===

        /// <summary>Retourne l'icône du premier bouton du menu vertical.</summary>
        public Uri GetMV1_Source() => SE_Icons.MV1_Source;

        /// <summary>Retourne l'icône du deuxième bouton du menu vertical.</summary>
        public Uri GetMV2_Source() => SE_Icons.MV2_Source;

        /// <summary>Retourne l'icône du troisième bouton du menu vertical.</summary>
        public Uri GetMV3_Source() => SE_Icons.MV3_Source;

        /// <summary>Retourne l'icône du quatrième bouton du menu vertical.</summary>
        public Uri GetMV4_Source() => SE_Icons.MV4_Source;

        /// <summary>Retourne l'icône du cinquième bouton du menu vertical.</summary>
        public Uri GetMV5_Source() => SE_Icons.MV5_Source;

        /// <summary>Retourne l'icône du sixième bouton du menu vertical.</summary>
        public Uri GetMV6_Source() => SE_Icons.MV6_Source;

        /// <summary>Retourne l'icône du septième bouton du menu vertical.</summary>
        public Uri GetMV7_Source() => SE_Icons.MV7_Source;

        /// <summary>Retourne l'icône du bouton utilisateur du menu vertical.</summary>
        public Uri GetMVU_Source() => SE_Icons.MVU_Source;

        #endregion

        #region === Logos ===

        /// <summary>Retourne le logo en couleur.</summary>
        public Uri GetLogo_Col_Source() => SE_Icons.Logo_Col_Source;

        /// <summary>Retourne le logo en noir et blanc.</summary>
        public Uri GetLogo_BW_Source() => SE_Icons.Logo_BW_Source;

        /// <summary>Retourne le logo en blanc sur fond noir.</summary>
        public Uri GetLogo_WB_Source() => SE_Icons.Logo_WB_Source;

        #endregion

        #region === Logos pour impression ===

        /// <summary>Retourne le logo couleur pour impression.</summary>
        public Uri GetPrint_Logo_Col_Source() => SE_Icons.Print_Logo_Col_Source;

        /// <summary>Retourne le logo noir et blanc pour impression.</summary>
        public Uri GetPrint_Logo_BW_Source() => SE_Icons.Print_Logo_BW_Source;

        /// <summary>Retourne le logo blanc sur fond noir pour impression.</summary>
        public Uri GetPrint_Logo_WB_Source() => SE_Icons.Print_Logo_WB_Source;

        #endregion
    }
}


/*


using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.Settings;

namespace BatchStockRelease.D_Presentation.Services
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
        public Uri GetMH_SaveUpdate_Source() => SE_Icons.MH_SaveUpdate_Source;
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
        public Uri GetMH_WarningTriangleOrange_Source() => SE_Icons.MH_WarningTriangleOrange_Source;
        public Uri GetMH_WarningTriangleRed_Source() => SE_Icons.MH_WarningTriangleRed_Source;


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
}*/