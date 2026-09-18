using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;

namespace BatchCutting_DG.B_UseCases.Services.BusinessLogic
{
    public class SR_DecoupeMachineIdentification : IS_CuttingMachineIdentification
    {
        private readonly IQ_DecoupeMachine _cuttingMachine;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;
        private readonly IS_Application _application;
        private readonly IS_UserSession _userSession;

        public SR_DecoupeMachineIdentification(IQ_DecoupeMachine cuttingMachine, IS_Settings settings,
                                                    IS_Notification notification, IS_Application application,
                                                    IS_UserSession userSession)
        {
            _cuttingMachine = cuttingMachine;
            _settings = settings;
            _notification = notification;
            _application = application;
            _userSession = userSession;
        }

        public async Task ExecuteAsync()
        {
            string currentIp = _settings.GetCRDeviceIP(); // Récupérer l'adresse IP du poste

            if (string.IsNullOrEmpty(currentIp))
            {
                _notification.Information("No_In_07");
                return;
            }

            var machine = await _cuttingMachine.HandleGetByDeviceIpAddressAsync(currentIp);

            if (machine != null)
            {
                _settings.SetCuttingMachine(machine.IdMachine);
                if (!string.IsNullOrEmpty(machine.DesignationImp))
                {
                    _settings.SetLabelPrinter(machine.DesignationImp);
                }

                if (!string.IsNullOrEmpty(machine.PortComConsole))
                {
                    _settings.SetLocalSerialPort(machine.PortComConsole);
                }

                _notification.Success("No_Su_11", Environment.NewLine + 
                                        Environment.NewLine + $"Id : {machine.IdMachine}" +
                                        Environment.NewLine + $"Printer : {_settings.GetLabelPrinter()}" +
                                        Environment.NewLine + $"PortCOM : {_settings.GetLocalSerialPort()}");

            }
            else
            {
                _notification.Warning("No_Wa_12", Environment.NewLine + Environment.NewLine + $"IP : {currentIp}");

                // Effectuer la fermeture de session
                // Vérifier si l'utilisateur encours est identifié
                if (_settings.GetAppUserID() > 0)
                {
                    // Deconnecter l'utilisateur
                    await _userSession.CloseUserSessionAsync(_settings.GetSessionId());
                }

                _application.ShutdownApplicationForceClose("No_Wa_13");
            }
        }
    }
}