using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserApplication
{
    public int Id { get; set; }

    public int? IdUtilisateur { get; set; }

    public int? IdApp { get; set; }

    public int AccesLevel { get; set; }
}
