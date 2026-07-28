using LeitTemporImport.A_Domain.Interfaces.Settings.App;
using LeitTemporImport.B_UseCases.Settings.App;
using System;
using System.ComponentModel;

namespace LeitTemporImport.B_UseCases.Services.App
{
    /// <summary>
    /// Service de gestion des paramètres applicatifs généraux.
    /// <para>
    /// Ce service agit comme une façade entre les constantes définies dans <see cref="SE_App"/>
    /// et les paramètres partagés contenus dans <see cref="CR_CommonSettings"/> et <see cref="CR_DataBaseSettings"/>.
    /// Il centralise les informations globales nécessaires à l’application, telles que :
    /// </para>
    /// <list type="bullet">
    /// <item><description>les constantes d’initialisation et de configuration,</description></item>
    /// <item><description>les chemins communs et répertoires partagés,</description></item>
    /// <item><description>les délais de rafraîchissement et de notifications,</description></item>
    /// <item><description>les états applicatifs (connexion, messages non lus),</description></item>
    /// <item><description>et les paramètres d’accès aux bases de données.</description></item>
    /// </list>
    /// </summary>
    public class SR_Settings_App : ISE_App
    {

        #region === Informations générales de l’application ===

        /// <summary>Retourne l’identifiant unique de l’application.</summary>
        public int GetAppId() => SE_App.AppId;

        /// <summary>Retourne la date du jour.</summary>
        public DateTime GetAppDate() => SE_App.AppDate;

        /// <summary>Retourne la date et l’heure actuelles.</summary>
        public DateTime GetAppDateTime() => SE_App.AppDateTime;

        /// <summary>Retourne le chemin du dossier des logs d’erreurs.</summary>
        public string GetErrorLogFolder() => SE_App.ErrorLogFolder;

        /// <summary>Retourne le nom du fichier CSV de logs d’erreurs.</summary>
        public string GetErrorLogFileName() => SE_App.ErrorLogFileName;

        #endregion

    }
}