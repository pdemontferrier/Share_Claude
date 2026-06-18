namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Référentiel statique des URI d'accès aux ressources iconographiques de l'application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Composant RS_ (Référentiel Statique) défini dans
    /// <c>D_Presentation/Settings</c>. Il agrège l'ensemble des URI <c>pack://</c>
    /// pointant d'une part vers les fichiers .png stockés sous
    /// </para>
    /// <para>
    /// Objectif : Centraliser la résolution des chemins de ressources
    /// iconographiques afin que tout composant consommateur référence une icône via
    /// un identifiant typé stable, sans coder en dur de chemin <c>pack://</c>. Cette
    /// centralisation garantit que tout déplacement, ajout ou suppression d'une
    /// ressource est répercuté en un seul point.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer un ensemble immuable d'URI absolues vers les icônes et logos de l'application.</description></item>
    ///   <item><description>Encapsuler la construction des URI <c>pack://</c> derrière des bases nommées (<see cref="IconsBase"/>) afin d'éviter toute duplication de chaîne dans les déclarations publiques.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune logique de stylisation : la mise en forme des contrôles consommateurs relève de <c>IS_ControlStyler</c>.</description></item>
    ///   <item><description>Ne porte aucun état mutable ni aucune préférence utilisateur : tout choix d'affichage dynamique relève d'un Setting (<c>SE_*</c>).</description></item>
    ///   <item><description>Ne participe pas à la CallChain : un référentiel statique n'orchestre aucun flux et ne propage aucune trace.</description></item>
    /// </list>
    /// <para>
    /// Nature « Référentiel Statique » : conformément à la section 2.7.5 du
    /// référentiel, un RS_ contient des données stables au runtime, non persistées
    /// en base, et résolues en compilation lorsque c'est techniquement possible.
    /// Les URI exposés ici sont construits par concaténation à partir de constantes
    /// <c>const string</c>, ce qui supprime tout problème d'ordre d'initialisation
    /// statique.
    /// </para>
    /// </remarks>
    internal static class RS_Icons
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Préfixe <c>pack://</c> pointant vers le dossier <c>Resources/Icons</c>
        /// de l'assembly courante <c>DG244Cutting</c>. Constante compilée afin de
        /// supprimer toute dépendance à l'ordre d'initialisation des champs
        /// statiques.
        /// </summary>
        private const string IconsBase = "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Icons/";

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        // --------- Application ---------

        /// <summary>URI de l'icône bleue utilisée pour le bouton de fermeture de l’application.</summary>
        public static readonly Uri AppCloseBlue_Source = new(IconsBase + "RE_systemShutdownBlue.png", UriKind.Absolute);


        /// <summary>URI de l'icône de déconnexion à la base de données.</summary>
        public static readonly Uri AppDisconnected_Source = new(IconsBase + "RE_systemShutdownRed.png", UriKind.Absolute);

        // --------- Emails ---------

        /// <summary>URI de l'icône standard pour les emails.</summary>
        public static readonly Uri IconEmail_Source = new(IconsBase + "RE_email.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour les emails non lus.</summary>
        public static readonly Uri IconEmailNotRead_Source = new(IconsBase + "RE_email_received.png", UriKind.Absolute);


        // --------- Menu Horizontal ---------

        /// <summary>URI de l'icône du bouton Menu horizontal.</summary>
        public static readonly Uri MH_Menu_Source = new(IconsBase + "RE_menu.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Mettre à jour.</summary>
        public static readonly Uri MH_Update_Source = new(IconsBase + "RE_folderDownload.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Retourner à la page précédente.</summary>
        public static readonly Uri MH_Previous_Source = new(IconsBase + "RE_returnBack.png", UriKind.Absolute);

        /// <summary>URI de l'icône de rafraîchissement (commande Refresh du menu horizontal).</summary>
        public static readonly Uri MH_Refresh_Source = new(IconsBase + "RE_refresh.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Importer.</summary>
        public static readonly Uri MH_Import_Source = new(IconsBase + "RE_folderDownload.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Enregistrer.</summary>
        public static readonly Uri MH_Save_Source = new(IconsBase + "RE_save.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Sauvegarder et Mettre à jour.</summary>
        public static readonly Uri MH_SaveUpdate_Source = new(IconsBase + "RE_saveUpdate.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Ajouter.</summary>
        public static readonly Uri MH_Add_Source = new(IconsBase + "RE_listAdd.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Supprimer.</summary>
        public static readonly Uri MH_Delete_Source = new(IconsBase + "RE_editDelete.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Dupliquer.</summary>
        public static readonly Uri MH_Duplicate_Source = new(IconsBase + "RE_editCopy.png", UriKind.Absolute);

        /// <summary>URI de l'icône de Détails.</summary>
        public static readonly Uri MH_Details_Source = new(IconsBase + "RE_zoomIn.png", UriKind.Absolute);

        /// <summary>URI de l'icône des Logs / Historique.</summary>
        public static readonly Uri MH_Logs_Source = new(IconsBase + "RE_history.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Activer.</summary>
        public static readonly Uri MH_Active_Source = new(IconsBase + "RE_softwareUpdateAvailable.png", UriKind.Absolute);

        /// <summary>URI de l'icône pour Valider.</summary>
        public static readonly Uri MH_Validate_Source = new(IconsBase + "RE_checked.png", UriKind.Absolute);

        /// <summary>URI de l'icône Utilisateur.</summary>
        public static readonly Uri MH_User_Source = new(IconsBase + "RE_user.png", UriKind.Absolute);

        /// <summary>URI de l'icône Accueil.</summary>
        public static readonly Uri MH_Home_Source = new(IconsBase + "RE_homeFull.png", UriKind.Absolute);

        /// <summary>URI de l'icône Administrateur.</summary>
        public static readonly Uri MH_Admin_Source = new(IconsBase + "RE_parameters.png", UriKind.Absolute);

        /// <summary>URI de l'icône d’avertissement orange (triangle).</summary>
        public static readonly Uri MH_WarningTriangleOrange_Source = new(IconsBase + "RE_warningTriangleOrange.png", UriKind.Absolute);

        /// <summary>URI de l'icône d’avertissement rouge (triangle).</summary>
        public static readonly Uri MH_WarningTriangleRed_Source = new(IconsBase + "RE_warningTriangleRed.png", UriKind.Absolute);


        // --------- Source générale ---------

        /// <summary>URI de l'icône d'attribution / d'affectation.</summary>
        public static readonly Uri IconAssign_Source = new(IconsBase + "RE_assign.png", UriKind.Absolute);

        /// <summary>URI de l'icône de case à cocher cochée.</summary>
        public static readonly Uri IconBoxChecked_Source = new(IconsBase + "RE_boxChecked.png", UriKind.Absolute);

        /// <summary>URI de l'icône de case à cocher vide.</summary>
        public static readonly Uri IconBoxEmpty_Source = new(IconsBase + "RE_boxEmpty.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'annulation cerclée.</summary>
        public static readonly Uri IconCancelCircle_Source = new(IconsBase + "RE_cancelCircle.png", UriKind.Absolute);

        /// <summary>URI de l'icône de validation simple.</summary>
        public static readonly Uri IconChecked_Source = new(IconsBase + "RE_checked.png", UriKind.Absolute);

        /// <summary>URI de l'icône de validation cerclée.</summary>
        public static readonly Uri IconCheckedCircle_Source = new(IconsBase + "RE_checkedCircle.png", UriKind.Absolute);

        /// <summary>URI de l'icône de tableau de bord.</summary>
        public static readonly Uri IconDashboard_Source = new(IconsBase + "RE_dashboard.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'édition de document.</summary>
        public static readonly Uri IconDocumentEdit_Source = new(IconsBase + "RE_documentEdit.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'impression de document.</summary>
        public static readonly Uri IconDocumentPrint_Source = new(IconsBase + "RE_documentPrint.png", UriKind.Absolute);

        /// <summary>URI de l'icône monétaire.</summary>
        public static readonly Uri IconDollarRound_Source = new(IconsBase + "RE_dollarRound.png", UriKind.Absolute);

        /// <summary>URI de l'icône de copie d'élément.</summary>
        public static readonly Uri IconEditCopy_Source = new(IconsBase + "RE_editCopy.png", UriKind.Absolute);

        /// <summary>URI de l'icône de suppression d'élément.</summary>
        public static readonly Uri IconEditDelete_Source = new(IconsBase + "RE_editDelete.png", UriKind.Absolute);

        /// <summary>URI de l'icône de recherche en édition.</summary>
        public static readonly Uri IconEditFind_Source = new(IconsBase + "RE_editFind.png", UriKind.Absolute);

        /// <summary>URI de l'icône de collage d'élément.</summary>
        public static readonly Uri IconEditPaste_Source = new(IconsBase + "RE_editPaste.png", UriKind.Absolute);

        /// <summary>URI de l'icône de recherche.</summary>
        public static readonly Uri IconFind_Source = new(IconsBase + "RE_find.png", UriKind.Absolute);

        /// <summary>URI de l'icône de téléchargement vers un dossier.</summary>
        public static readonly Uri IconFolderDownload_Source = new(IconsBase + "RE_folderDownload.png", UriKind.Absolute);

        /// <summary>URI de l'icône de retour à l'accueil (variante simple).</summary>
        public static readonly Uri IconGoHome_Source = new(IconsBase + "RE_goHome.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'historique.</summary>
        public static readonly Uri IconHistory_Source = new(IconsBase + "RE_history.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'accueil (variante pleine).</summary>
        public static readonly Uri IconHomeFull_Source = new(IconsBase + "RE_homeFull.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'import.</summary>
        public static readonly Uri IconImport_Source = new(IconsBase + "RE_import.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'information.</summary>
        public static readonly Uri IconInfo_Source = new(IconsBase + "RE_info.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'information cerclée pleine.</summary>
        public static readonly Uri IconInfoCircleFull_Source = new(IconsBase + "RE_infoCircleFull.png", UriKind.Absolute);

        /// <summary>URI de l'icône de lancement (variante générique).</summary>
        public static readonly Uri IconLaunch_Source = new(IconsBase + "RE_launch.png", UriKind.Absolute);

        /// <summary>URI de l'icône de lancement d'exécutable.</summary>
        public static readonly Uri IconLaunchExe_Source = new(IconsBase + "RE_launch_exe.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'ajout à une liste.</summary>
        public static readonly Uri IconListAdd_Source = new(IconsBase + "RE_listAdd.png", UriKind.Absolute);

        /// <summary>URI de l'icône de menu (bascule réduit / développé du menu horizontal).</summary>
        public static readonly Uri IconMenu_Source = new(IconsBase + "RE_menu.png", UriKind.Absolute);

        /// <summary>URI de l'icône de page précédente (navigation arrière).</summary>
        public static readonly Uri IconPagePrevious_Source = new(IconsBase + "RE_pagePrevious.png", UriKind.Absolute);

        /// <summary>URI de l'icône de paramètres.</summary>
        public static readonly Uri IconParameters_Source = new(IconsBase + "RE_parameters.png", UriKind.Absolute);

        /// <summary>URI de l'icône de pin couché.</summary>
        public static readonly Uri IconPinLying_Source = new(IconsBase + "RE_pinLying.png", UriKind.Absolute);

        /// <summary>URI de l'icône de pin debout.</summary>
        public static readonly Uri IconPinStanding_Source = new(IconsBase + "RE_pinStanding.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'ajout cerclée.</summary>
        public static readonly Uri IconPlusCircle_Source = new(IconsBase + "RE_plusCircle.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'ajout cerclée pleine.</summary>
        public static readonly Uri IconPlusCircleFull_Source = new(IconsBase + "RE_plusCircleFull.png", UriKind.Absolute);

        /// <summary>URI de l'icône de préférences système avancées.</summary>
        public static readonly Uri IconPreferencesSystemDetails_Source = new(IconsBase + "RE_preferencesSystemDetails.png", UriKind.Absolute);

        /// <summary>URI de l'icône de rafraîchissement (commande Refresh du menu horizontal).</summary>
        public static readonly Uri IconRefresh_Source = new(IconsBase + "RE_refresh.png", UriKind.Absolute);

        /// <summary>URI de l'icône de retour arrière.</summary>
        public static readonly Uri IconReturnBack_Source = new(IconsBase + "RE_returnBack.png", UriKind.Absolute);

        /// <summary>URI de l'icône de sauvegarde simple.</summary>
        public static readonly Uri IconSave_Source = new(IconsBase + "RE_save.png", UriKind.Absolute);

        /// <summary>URI de l'icône de sauvegarde de données.</summary>
        public static readonly Uri IconSaveData_Source = new(IconsBase + "RE_saveData.png", UriKind.Absolute);

        /// <summary>URI de l'icône de mise à jour de sauvegarde.</summary>
        public static readonly Uri IconSaveUpdate_Source = new(IconsBase + "RE_saveUpdate.png", UriKind.Absolute);

        /// <summary>URI de l'icône de retour rapide arrière.</summary>
        public static readonly Uri IconSeekBackward_Source = new(IconsBase + "RE_seekBackward.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'avance rapide.</summary>
        public static readonly Uri IconSeekForward_Source = new(IconsBase + "RE_seekForward.png", UriKind.Absolute);

        /// <summary>URI de l'icône de mise à jour logicielle disponible.</summary>
        public static readonly Uri IconSoftwareUpdateAvailable_Source = new(IconsBase + "RE_softwareUpdateAvailable.png", UriKind.Absolute);

        /// <summary>URI de l'icône de mise à jour logicielle urgente.</summary>
        public static readonly Uri IconSoftwareUpdateUrgent_Source = new(IconsBase + "RE_softwareUpdateUrgent.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'extinction système (état standard).</summary>
        public static readonly Uri IconSystemShutdownBlue_Source = new(IconsBase + "RE_systemShutdownBlue.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'extinction système (état nominal).</summary>
        public static readonly Uri IconSystemShutdownGreen_Source = new(IconsBase + "RE_systemShutdownGreen.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'extinction système (état d'alerte).</summary>
        public static readonly Uri IconSystemShutdownRed_Source = new(IconsBase + "RE_systemShutdownRed.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'éditeur de texte.</summary>
        public static readonly Uri IconTextEditor_Source = new(IconsBase + "RE_textEditor.png", UriKind.Absolute);

        /// <summary>URI de l'icône utilisateur.</summary>
        public static readonly Uri IconUser_Source = new(IconsBase + "RE_user.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'avertissement (orange).</summary>
        public static readonly Uri IconWarningTriangleOrange_Source = new(IconsBase + "RE_warningTriangleOrange.png", UriKind.Absolute);

        /// <summary>URI de l'icône d'avertissement (rouge).</summary>
        public static readonly Uri IconWarningTriangleRed_Source = new(IconsBase + "RE_warningTriangleRed.png", UriKind.Absolute);

        /// <summary>URI de l'icône de fermeture de fenêtre.</summary>
        public static readonly Uri IconWindowClose_Source = new(IconsBase + "RE_windowClose.png", UriKind.Absolute);

        /// <summary>URI de l'icône de maximisation de fenêtre.</summary>
        public static readonly Uri IconWindowMaximize_Source = new(IconsBase + "RE_windowMaximize.png", UriKind.Absolute);

        /// <summary>URI de l'icône de minimisation de fenêtre.</summary>
        public static readonly Uri IconWindowMinimiz_Source = new(IconsBase + "RE_windowMinimize.png", UriKind.Absolute);

        /// <summary>URI de l'icône de restauration de fenêtre.</summary>
        public static readonly Uri IconWindowRestore_Source = new(IconsBase + "RE_windowRestore.png", UriKind.Absolute);

        /// <summary>URI de l'icône de zoom avant.</summary>
        public static readonly Uri IconZoomIn_Source = new(IconsBase + "RE_zoomIn.png", UriKind.Absolute);

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}