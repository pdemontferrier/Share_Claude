using CommonResources.Settings;

namespace BatchStockRelease.D_Presentation.Settings
{
    /// <summary>
    /// <b>SE_Icons</b>
    /// <para>
    /// Classe statique de configuration centralisant les chemins d’accès (<see cref="Uri"/>) aux icônes et logos utilisés dans l’application BatchStockRelease.  
    /// </para>
    /// <para>
    /// Les icônes sont principalement importées depuis le projet <b>CommonRessources</b> via la classe <see cref="CR_IconsSettings"/>,  
    /// tandis que les logos spécifiques à BatchStockRelease sont définis dans le répertoire local :
    /// <c>D_Presentation/Ressources/Logo/</c>.
    /// </para>
    /// <para>
    /// Cette classe est utilisée par <see cref="BatchStockRelease.D_Presentation.Services.SR_Icons"/> pour fournir les icônes aux vues et ViewModels.
    /// </para>
    /// </summary>
    public static class SE_Icons
    {
        #region === Icons Emails ===

        /// <summary>Icône standard pour les emails.</summary>
        public static readonly Uri IconEmail_Source = CR_IconsSettings.IconEmail;

        /// <summary>Icône pour les emails non lus.</summary>
        public static readonly Uri IconEmailNotRead_Source = CR_IconsSettings.IconEmailNotRead;

        /// <summary>Icône bleue utilisée pour le bouton de fermeture de l’application.</summary>
        public static readonly Uri AppCloseBlue_Source = CR_IconsSettings.IconSystemShutdownBlue;

        #endregion

        #region === Menu Horizontal Buttons ===

        /// <summary>Icône du bouton Menu horizontal.</summary>
        public static readonly Uri MH_Menu_Source = CR_IconsSettings.IconMenu;

        /// <summary>Icône du bouton Mettre à jour.</summary>
        public static readonly Uri MH_Update_Source = CR_IconsSettings.IconFolderDownload;

        /// <summary>Icône du bouton Retour arrière.</summary>
        public static readonly Uri MH_ReturnBack_Source = CR_IconsSettings.IconReturnBack;

        /// <summary>Icône du bouton Rafraîchir.</summary>
        public static readonly Uri MH_Refresh_Source = CR_IconsSettings.IconRefresh;

        /// <summary>Icône du bouton Importer.</summary>
        public static readonly Uri MH_Import_Source = CR_IconsSettings.IconFolderDownload;

        /// <summary>Icône du bouton Enregistrer.</summary>
        public static readonly Uri MH_Save_Source = CR_IconsSettings.IconSave;

        /// <summary>Icône du bouton Sauvegarder et Mettre à jour.</summary>
        public static readonly Uri MH_SaveUpdate_Source = CR_IconsSettings.IconSaveUpdate;

        /// <summary>Icône du bouton Ajouter.</summary>
        public static readonly Uri MH_Add_Source = CR_IconsSettings.IconListAdd;

        /// <summary>Icône du bouton Supprimer.</summary>
        public static readonly Uri MH_Delete_Source = CR_IconsSettings.IconEditDelete;

        /// <summary>Icône du bouton Dupliquer.</summary>
        public static readonly Uri MH_Duplicate_Source = CR_IconsSettings.IconEditCopy;

        /// <summary>Icône du bouton Page précédente.</summary>
        public static readonly Uri MH_Previous_Source = CR_IconsSettings.IconPagePrevious;

        /// <summary>Icône du bouton Détails.</summary>
        public static readonly Uri MH_Details_Source = CR_IconsSettings.IconZoomIn;

        /// <summary>Icône du bouton Logs / Historique.</summary>
        public static readonly Uri MH_Logs_Source = CR_IconsSettings.IconHistory;

        /// <summary>Icône du bouton Activer.</summary>
        public static readonly Uri MH_Active_Source = CR_IconsSettings.IconSoftwareUpdateAvailable;

        /// <summary>Icône du bouton Valider.</summary>
        public static readonly Uri MH_Validate_Source = CR_IconsSettings.IconChecked;

        /// <summary>Icône du bouton Utilisateur.</summary>
        public static readonly Uri MH_User_Source = CR_IconsSettings.IconUser;

        /// <summary>Icône du bouton Accueil.</summary>
        public static readonly Uri MH_Home_Source = CR_IconsSettings.IconHomeFull;

        /// <summary>Icône du bouton Administrateur.</summary>
        public static readonly Uri MH_Admin_Source = CR_IconsSettings.IconParameters;

        /// <summary>Icône d’avertissement orange (triangle).</summary>
        public static readonly Uri MH_WarningTriangleOrange_Source = CR_IconsSettings.IconWarningTriangleOrange;

        /// <summary>Icône d’avertissement rouge (triangle).</summary>
        public static readonly Uri MH_WarningTriangleRed_Source = CR_IconsSettings.IconWarningTriangleRed;

        #endregion

        #region === Menu Vertical Buttons ===

        /// <summary>Icône du premier bouton du menu vertical.</summary>
        public static readonly Uri MV1_Source = CR_IconsSettings.IconHomeFull;

        /// <summary>Icône du deuxième bouton du menu vertical.</summary>
        public static readonly Uri MV2_Source = CR_IconsSettings.IconDollarRound;

        /// <summary>Icône du troisième bouton du menu vertical.</summary>
        public static readonly Uri MV3_Source = CR_IconsSettings.IconInfoCircleFull;

        /// <summary>Icône du quatrième bouton du menu vertical.</summary>
        public static readonly Uri MV4_Source = CR_IconsSettings.IconPlusCircleFull;

        /// <summary>Icône du cinquième bouton du menu vertical.</summary>
        public static readonly Uri MV5_Source = CR_IconsSettings.IconAssign;

        /// <summary>Icône du sixième bouton du menu vertical.</summary>
        public static readonly Uri MV6_Source = CR_IconsSettings.IconLaunch;

        /// <summary>Icône du septième bouton du menu vertical.</summary>
        public static readonly Uri MV7_Source = CR_IconsSettings.IconDashboard;

        /// <summary>Icône du bouton Utilisateur du menu vertical.</summary>
        public static readonly Uri MVU_Source = CR_IconsSettings.IconUser;

        #endregion

        #region === Logos ===

        /// <summary>Logo couleur de l’application.</summary>
        public static readonly Uri Logo_Col_Source = CR_IconsSettings.Logo;

        /// <summary>Logo noir et blanc.</summary>
        public static readonly Uri Logo_BW_Source = CR_IconsSettings.Logo_BW;

        /// <summary>Logo blanc sur fond noir.</summary>
        public static readonly Uri Logo_WB_Source = CR_IconsSettings.Logo_WB;

        #endregion

        #region === Logos pour impression ===

        /// <summary>
        /// Nom du projet courant, utilisé pour construire dynamiquement le chemin d’accès des logos d’impression.  
        /// Cette propriété évite l’usage de références en dur (ex. <c>"BatchCutting_DG"</c>).
        /// </summary>
        private static readonly string ProjectName = typeof(SE_Icons).Assembly.GetName().Name ?? "BatchStockRelease";

        /// <summary>Logo couleur utilisé pour les impressions.</summary>
        public static readonly Uri Print_Logo_Col_Source =
            new Uri($"/{ProjectName};component/D_Presentation/Ressources/Logo/RE_Logo.png", UriKind.Relative);

        /// <summary>Logo noir et blanc utilisé pour les impressions.</summary>
        public static readonly Uri Print_Logo_BW_Source =
            new Uri($"/{ProjectName};component/D_Presentation/Ressources/Logo/RE_Logo_BW.png", UriKind.Relative);

        /// <summary>Logo blanc sur fond noir utilisé pour les impressions.</summary>
        public static readonly Uri Print_Logo_WB_Source =
            new Uri($"/{ProjectName};component/D_Presentation/Ressources/Logo/RE_Logo_WB.png", UriKind.Relative);

        #endregion
    }
}