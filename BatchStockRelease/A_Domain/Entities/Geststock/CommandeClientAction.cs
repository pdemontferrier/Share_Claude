using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CommandeClientAction
{
    public int Id { get; set; }

    public int IdCmdClient { get; set; }

    public int IdUser { get; set; }

    public int IdAction { get; set; }

    public DateTime? DateAction { get; set; }

    /// <summary>
    /// Le temps passé est définie en seconde
    /// </summary>
    public int TempsEstime { get; set; }

    public int? BonusMalus { get; set; }
}
