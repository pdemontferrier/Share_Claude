using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic
{
    public interface IS_LabelPrinter
    {
        bool PrintBarCutLabel(DTO_DecoupeDetailWithCut decoupeDetailWithCut);
        bool PrintBarDropLabel(DTO_DecoupeBarreWithCut decoupeBarreWithCut);
        bool PrintBarWasteLabel();
    }
}