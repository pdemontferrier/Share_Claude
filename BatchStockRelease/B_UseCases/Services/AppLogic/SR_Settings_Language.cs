using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.B_UseCases.Settings.AppLogic;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// Service de gestion des paramètres linguistiques de l'application.
    /// Gère les chemins des dictionnaires et la culture active.
    /// </summary>
    public class SR_Settings_Language : IS_Settings_Language
    {
        /// <summary>
        /// Retourne l'URI du dictionnaire anglais.
        /// </summary>
        public Uri GetDicEn() => SE_Dictionary.Dic_en;
        public void SetDicEn(Uri value) => SE_Dictionary.Dic_en = value;

        /// <summary>
        /// Retourne l'URI du dictionnaire français.
        /// </summary>
        public Uri GetDicFr() => SE_Dictionary.Dic_fr;
        public void SetDicFr(Uri value) => SE_Dictionary.Dic_fr = value;

        /// <summary>
        /// Retourne l'URI du dictionnaire allemand.
        /// </summary>
        public Uri GetDicDe() => SE_Dictionary.Dic_de;
        public void SetDicDe(Uri value) => SE_Dictionary.Dic_de = value;

        /// <summary>
        /// Retourne l'URI du dictionnaire espagnol.
        /// </summary>
        public Uri GetDicEs() => SE_Dictionary.Dic_es;
        public void SetDicEs(Uri value) => SE_Dictionary.Dic_es = value;

        /// <summary>
        /// Retourne l'URI du dictionnaire italien.
        /// </summary>
        public Uri GetDicIt() => SE_Dictionary.Dic_it;
        public void SetDicIt(Uri value) => SE_Dictionary.Dic_it = value;

        /// <summary>
        /// Retourne l'URI du dictionnaire portugais.
        /// </summary>
        public Uri GetDicPt() => SE_Dictionary.Dic_pt;
        public void SetDicPt(Uri value) => SE_Dictionary.Dic_pt = value;

        /// <summary>
        /// Obtient ou définit le code culture de l'application (ex : "fr-FR").
        /// </summary>
        public string GetAppCultureCode() => SE_Dictionary.AppCultureCode;
        public void SetAppCultureCode(string value) => SE_Dictionary.AppCultureCode = value;
    }
}
