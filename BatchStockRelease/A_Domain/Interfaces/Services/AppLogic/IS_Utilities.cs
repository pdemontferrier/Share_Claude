namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    public interface IS_Utilities
    {
        string GetCrypte_md5(string chaine);

        Uri GetVueUri(string? referenceVue);

    }
}
