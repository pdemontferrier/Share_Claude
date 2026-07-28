using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.UseCases.Business;
using LeitTemporImport.A_Domain.Interfaces.UseCases.User;
using LeitTemporImport.E_Miscellaneous.CompositionRoot;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    #region === Méthodes publiques ===

    private static async Task Main()
    {
        string callChain = $"{nameof(Program)} > {nameof(Main)}";

        using var cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        using var serviceProvider = SR_ConteneurDI.ConfigureServices();
        using var scope = serviceProvider.CreateScope();
        var errorLog = scope.ServiceProvider.GetRequiredService<IS_ErrorLogger>();

        try
        {
            Console.WriteLine($"Import Leit_Tempor commencé à : {DateTime.Now}");

            // Identification de l'utilisateur
            var ucIdentify = scope.ServiceProvider.GetRequiredService<IU_UserIdentify>();
            int userId = await ucIdentify.ExecuteAsync(callChain, ct);

            if (userId == 0)
            {
                Console.WriteLine("Utilisateur non identifié. Import annulé.");
                return;
            }

            // Importation
            var ucImport = scope.ServiceProvider.GetRequiredService<IU_TemporImport>();
            await ucImport.ExecuteAsync(callChain, ct);

            Console.WriteLine($"Import Leit_Tempor terminé à : {DateTime.Now}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Import annulé (CancellationToken).");
        }
        catch (Exception ex)
        {
            // Pas de discrimination Business/Infrastructure ici :
            // le logger normalise déjà.
            try
            {
                await errorLog.ExecuteAsync(callChain, ex, ct);
            }
            catch
            {
                // Ultime fallback : ne pas crasher si DI / logger indisponible
                Console.WriteLine($"Erreur critique : {ex.Message}");
            }

            Console.WriteLine("Import interrompu suite à une erreur. Voir logs.");
        }
    }

    #endregion
}