namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// Interface pour la gestion des paramètres linguistiques de l'application.
    /// </summary>
    public interface IS_Settings_Language
    {
        Uri GetDicEn();
        void SetDicEn(Uri value);
        Uri GetDicFr();
        void SetDicFr(Uri value);
        Uri GetDicDe();
        void SetDicDe(Uri value);
        Uri GetDicEs();
        void SetDicEs(Uri value);
        Uri GetDicIt();
        void SetDicIt(Uri value);
        Uri GetDicPt();
        void SetDicPt(Uri value);

        string GetAppCultureCode();
        void SetAppCultureCode(string value);
    }
}
