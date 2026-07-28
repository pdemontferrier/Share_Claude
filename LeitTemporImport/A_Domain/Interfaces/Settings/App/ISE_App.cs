using System;
using System.ComponentModel;

namespace LeitTemporImport.A_Domain.Interfaces.Settings.App
{
    /// <summary>
    /// Interface de service des paramètres applicatifs généraux.
    /// <para>
    /// Cette interface définit les membres publics du service <see cref="B_UseCases.Services.App.SR_Settings_App"/>.
    /// Elle centralise les méthodes d’accès aux paramètres d’application, aux ressources partagées,
    /// et aux états globaux de l’environnement d’exécution.
    /// </para>
    /// <para>
    /// Elle inclut également les événements de notification liés à la surveillance de la connexion
    /// à la base de données et les informations de monitoring associées.
    /// </para>
    /// </summary>
    public interface ISE_App
    {
        #region === Informations générales de l’application ===

        int GetAppId();
        DateTime GetAppDate();
        DateTime GetAppDateTime();
        string GetErrorLogFolder();
        string GetErrorLogFileName();

        #endregion
    }
}