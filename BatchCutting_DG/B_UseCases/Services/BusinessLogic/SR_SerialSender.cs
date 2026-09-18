using System.IO.Ports;
using System.Text;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchCutting_DG.B_UseCases.Services.BusinessLogic
{
    public class SR_SerialSender : IS_SerialSender
    {
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;

        public SR_SerialSender(IS_Settings settings, IS_Notification notification)
        {
            _settings = settings;
            _notification = notification;
        }

        public async Task SendMessageAsync(string decoupeDetailMessageElumatec)
        {
            try
            {
                using var serialPort = new SerialPort(_settings.GetLocalSerialPort(), 9600, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    ReadTimeout = 2000,
                    WriteTimeout = 500,
                    Encoding = Encoding.ASCII
                };

                // Charger le message
                string message = decoupeDetailMessageElumatec ?? string.Empty;
                if (!string.IsNullOrEmpty(message))
                {
                    // Nettoyage : suppression de STX (char 0x02) en début et ETX (char 0x03) en fin
                    message = message.Trim((char)0x02, (char)0x03);

                    // Ouvre le port série
                    serialPort.Open();

                    // Convertir la chaîne en ASCII
                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);

                    // Création d'un tableau final avec l'ajout manuel de fin de ligne
                    byte[] finalMessage = new byte[messageBytes.Length + 1];
                    Array.Copy(messageBytes, finalMessage, messageBytes.Length);
                    finalMessage[finalMessage.Length - 1] = 0xD; // Ajout de fin de ligne

                    // Envoyer le message final
                    serialPort.Write(finalMessage, 0, finalMessage.Length);

                    // Terminer l'envoi en fermamnt le port série
                    serialPort.Close();
                }

                await Task.CompletedTask;

            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_17", ex.Message);
            }
        }
    }
}