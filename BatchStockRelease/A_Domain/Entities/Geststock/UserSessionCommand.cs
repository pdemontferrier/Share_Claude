using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserSessionCommand
{
    public int Id { get; set; }

    public int IdUserTarget { get; set; }

    public int IdAppTarget { get; set; }

    public int IdUserIssuer { get; set; }

    public string? CommandType { get; set; }

    public DateTime CommandDate { get; set; }
}
