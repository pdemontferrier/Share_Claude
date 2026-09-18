using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserDroit
{
    public int IdTab { get; set; }

    public int IdUser { get; set; }

    public int IdAction { get; set; }
}
