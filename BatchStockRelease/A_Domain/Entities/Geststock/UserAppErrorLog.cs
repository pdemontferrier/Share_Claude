using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserAppErrorLog
{
    public int Id { get; set; }

    public DateTime? Timestamp { get; set; }

    public int? AppId { get; set; }

    public string? AppCallChain { get; set; }

    public string? AppErrorId { get; set; }

    public string? AppErrorMessage { get; set; }

    public string? AppErrorException { get; set; }

    public int? AppUserId { get; set; }

    public string? DeviceUser { get; set; }

    public string? DeviceId { get; set; }

    public string? DeviceIp { get; set; }
}
