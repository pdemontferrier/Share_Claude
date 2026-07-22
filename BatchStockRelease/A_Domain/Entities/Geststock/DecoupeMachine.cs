using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class DecoupeMachine
{
    public int Id { get; set; }

    public string IdMachine { get; set; } = null!;

    public string? DescriptionMachine { get; set; }

    public string? DesignationConsole { get; set; }

    public string? AdresseIpConsole { get; set; }

    public string? PortComConsole { get; set; }

    public string? DesignationPc { get; set; }

    public string? AdresseIpPc { get; set; }

    public string? PortComPc { get; set; }

    public string? DesignationImp { get; set; }

    public string? AdresseIpImp { get; set; }
}
