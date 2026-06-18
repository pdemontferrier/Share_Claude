using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// Service d’infrastructure responsable de :
    /// <list type="bullet">
    /// <item>la journalisation des erreurs via <see cref="IS_FileLogger"/>,</item>
    /// <item>la notification utilisateur via <see cref="IS_Notification"/>,</item>
    /// <item>et la traduction des messages via <see cref="IS_Dictionary"/>.</item>
    /// </list>
    /// Ce service isole la logique de présentation des erreurs (notification + traduction)
    /// de la persistance technique des logs.
    /// </summary>
    public class SR_LogAndNotify : IS_LogAndNotify
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_FileLogger _fileLogger;
        private readonly IS_Notification _notification;
        private readonly IS_Dictionary _dictionary;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service de log et de notification.
        /// </summary>
        /// <param name="fileLogger">Service de journalisation des erreurs.</param>
        /// <param name="notification">Service de notification utilisateur.</param>
        /// <param name="dictionary">Service de dictionnaire multilingue.</param>
        public SR_LogAndNotify(IS_FileLogger fileLogger, IS_Notification notification, IS_Dictionary dictionary)
        {
            _callee = GetType().Name;

            _fileLogger = fileLogger;
            _notification = notification;
            _dictionary = dictionary;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Exécute la procédure standard de journalisation et de notification d’une erreur.
        /// </summary>
        /// <param name="firstDictionaryKey">Clé du message principal dans le dictionnaire.</param>
        /// <param name="ex">Exception à traiter (métier ou infrastructure).</param>
        /// <param name="notification">Indique si une notification utilisateur doit être affichée (par défaut <c>true</c>).</param>
        /// <remarks>
        /// Cette méthode :
        /// <list type="number">
        /// <item>Identifie le type d’exception (Business, Infrastructure, Autre),</item>
        /// <item>Construit un message utilisateur à partir du dictionnaire,</item>
        /// <item>Envoie le log explicite au <see cref="IS_FileLogger"/>,</item>
        /// <item>Et notifie l’utilisateur si nécessaire.</item>
        /// </list>
        /// </remarks>
        public async Task ExecuteAsync(string firstDictionaryKey, Exception ex, bool notification = true)
        {

            // Traitement centralisé
            switch (ex)
            {
                case Ex_Business bex:
                    await LogAndNotifyAsync(
                        bex.CallChain ?? "UnknownCallChain",
                        bex.ErrorId ?? "UnknownErrorId",                // errorId
                        _dictionary.GetText(firstDictionaryKey),        // errorMessage
                        !string.IsNullOrEmpty(bex.SecondDictionaryKey)  // errorException
                            ? _dictionary.GetText(bex.SecondDictionaryKey) 
                            : "UnknownException",
                        notification);
                    break;

                case Ex_Infrastructure iex:
                    await LogAndNotifyAsync(
                        iex.CallChain ?? "UnknownCallChain",
                        iex.ErrorId ?? "UnknownErrorId",                // errorId
                        _dictionary.GetText(firstDictionaryKey),        // errorMessage
                        !string.IsNullOrEmpty(iex.SecondDictionaryKey)  // errorException
                            ? _dictionary.GetText(iex.SecondDictionaryKey) 
                            : "UnknownException",
                        notification);
                    break;

                default:
                    await LogAndNotifyAsync(
                        "UnknownCallChain",
                        "UnknownErrorId",                         // errorId
                        _dictionary.GetText(firstDictionaryKey),  // errorMessage
                        ex.Message,                               // errorException
                        notification);
                    break;
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Gère la journalisation et la notification d’une erreur selon le type détecté.
        /// </summary>
        private async Task LogAndNotifyAsync(
            string callChain,
            string errorId,
            string errorMessage,
            string errorException,
            bool notify)
        {

            await _fileLogger.LogErrorAsync(
                callChain,
                errorId,
                errorMessage,
                errorException);

            if (notify)
                _notification.Error(errorMessage, $"{errorId} - {errorException}");
        }

        #endregion
    }
}