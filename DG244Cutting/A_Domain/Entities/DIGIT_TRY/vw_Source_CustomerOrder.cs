using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_CustomerOrder
{
    public int? IdOrder { get; set; }

    public int IdProductionSeries { get; set; }

    public int PartialSeriesIndex { get; set; }

    public int? ProjectNumber { get; set; }

    public string? ProjectDesignation { get; set; }

    public string? ManufacturingSite { get; set; }

    public string? ManufacturingPlant { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public DateOnly? ShippingDate { get; set; }

    public DateOnly? ProductionStartDate { get; set; }

    public int? ProductionStartWeek { get; set; }

    public DateOnly? ProductionEndDate { get; set; }

    public int? ProductionEndWeek { get; set; }

    public int? ProductionEndWeekday { get; set; }

    public string? ProductionEndTourId { get; set; }

    public string? OrderSponsor { get; set; }

    public string? MainSalesPointCode { get; set; }

    public string? MainSalesPoint { get; set; }

    public string? MainSalesPointName { get; set; }

    public string? MainSalesPointAddress { get; set; }

    public string? SecondarySalesPointName { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerProjectName { get; set; }

    public string? CustomerProjectDesignation { get; set; }

    public string? CustomerStreet { get; set; }

    public string? CustomerCity { get; set; }

    public string? CustomerZipCode { get; set; }

    public string? CustomerCountry { get; set; }

    public string? DeliveryPosition { get; set; }

    public string? QuaiZone { get; set; }
}
