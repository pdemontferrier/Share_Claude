using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserSession
{
    public int Id { get; set; }

    public int IdApplication { get; set; }

    public int IdUser { get; set; }

    public string? DeviceUser { get; set; }

    public string? DeviceId { get; set; }

    public string? DeviceIp { get; set; }

    public bool Connected { get; set; }

    public DateTime ConnectionDate { get; set; }

    public DateTime DisconnectionDate { get; set; }
}
