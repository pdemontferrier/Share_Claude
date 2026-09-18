using System.Globalization;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using CommonResources.Utilities;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Utilities : IS_Utilities
    {
        // CommonResources.Utilities.EncryptionHelper
        public string GetCrypte_md5(string chaine) => EncryptionHelper.Crypte_md5(chaine);


    }
}
