using CommonResources.Settings;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;

namespace BatchCutting_DG.B_UseCases.Services.App
{
    public class SR_ReferenceVue : IS_ReferenceVue
    {
        public Uri GetVueUri(string? referenceVue)
        {
            string refVue = referenceVue ?? string.Empty;
            return CR_VuesSettings.GetVueUri(refVue);
        }
    }
}