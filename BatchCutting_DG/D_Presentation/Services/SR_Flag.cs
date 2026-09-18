using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Settings;

namespace BatchCutting_DG.D_Presentation.Services
{

    public class SR_Flag : IS_Flag
    {
        public Uri GetAppFlagUri() =>  SE_Flags.AppFlagUri;

        public void SetAppFlagUri(string languageCode)
        {
                SE_Flags.AppFlagUri = GetFlagUri(languageCode);
        }

        public Uri GetFlagUri(string languageCode)
        {
            if (!string.IsNullOrWhiteSpace(languageCode) &&
                SE_Flags.ReferenceFlag.TryGetValue(languageCode.ToUpperInvariant(), out var uri))
            {
                return uri;
            }

            return SE_Flags.DefaultFlagUri;
        }
    }
}