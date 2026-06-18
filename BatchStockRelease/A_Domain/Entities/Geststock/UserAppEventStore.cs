using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserAppEventStore
{
    public int Id { get; set; }

    public string? TableDesignation { get; set; }

    public int? TableId { get; set; }

    public DateTime Timestamp { get; set; }

    public string? Data { get; set; }

    public int? AppId { get; set; }

    public string? AppCallChain { get; set; }

    public string? AppHandlerCommand { get; set; }

    public string? AppCommandMethod { get; set; }

    public int? AppUserId { get; set; }

    public string? DeviceUser { get; set; }

    public string? DeviceId { get; set; }

    public string? DeviceIp { get; set; }
}
