using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase permettant de valider l'intégration d'une chute générée en stock
    /// en mettant à jour l’état <c>AttenteIntegration</c> à <c>false</c>.
    /// </summary>
    public class UC_BarDropStockIntegration : IU_BarDropStockIntegration
    {
        private readonly string _callee;
        private readonly IQ_ChutesMagasin _qhChutesMagasin;
        private readonly IS_ChutesMagasin_UpdateIntegration _chutesMagasin_UpdateIntegration;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Settings_App _settingsApp;

        public UC_BarDropStockIntegration(
            IQ_ChutesMagasin qhChutesMagasin,
            IS_ChutesMagasin_UpdateIntegration chutesMagasin_UpdateIntegration,
            IS_LogAndNotify logAndNotify,
            IS_Settings_App settingsApp)
        {
            _callee = GetType().Name;
            _qhChutesMagasin = qhChutesMagasin;
            _chutesMagasin_UpdateIntegration = chutesMagasin_UpdateIntegration;
            _logAndNotify = logAndNotify;
            _settingsApp = settingsApp;
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(string QrCodeInput, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // Eléments de paramétrage
            DateTime appDateTime = _settingsApp.GetAppDateTime();

            try
            {
                // Étape 1 : Récuprérer l'enregistrement de la table ChutesMagasin
                var chuteMagasin = await _qhChutesMagasin.HandleGetByQrCodeAsync(QrCodeInput);

                // Étape 2 : Tester si chuteMagasin est null
                if (chuteMagasin == null)
                {
                    var ex = new Ex_Business(callChain, $"CM_02 - QrCodeInput : {QrCodeInput}", "No_Er_Bu_57");
                    await _logAndNotify.ExecuteAsync("No_EC_18", ex, false);
                    return 0;
                }

                // Étape 3 : Tester si chuteMagasin.AttenteIntegration = false
                if (!chuteMagasin.AttenteIntegration)
                    return 2;

                // Étape 4 : Mettre à jour chuteMagasin.AttenteIntegration = false
                await _chutesMagasin_UpdateIntegration.ExecuteAsync(chuteMagasin, appDateTime, callChain);
                return 1;
            }
            catch (Ex_Business bex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", bex);
            }
            catch (Ex_Infrastructure iex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", iex);
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
            }

            // Ajouter un return par défaut après le bloc try/catch
            // Il indique qu'une erreur technique a été loggée mais que l’UI peut réagir proprement.
            return -1;
        }
    }
}