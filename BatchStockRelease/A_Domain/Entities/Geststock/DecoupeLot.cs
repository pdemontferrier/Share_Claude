using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class DecoupeLot
{
    public int Id { get; set; }

    public int IdEcheance { get; set; }

    public string IdCouleur { get; set; } = null!;

    public string? Designation { get; set; }

    public bool Inactif { get; set; }

    public int NbProject { get; set; }

    public string? CreationUtilisateurPoste { get; set; }

    public int CreationUtilisateurErp { get; set; }

    public string? CreationPosteId { get; set; }

    public string? CreationPosteIp { get; set; }

    public DateTime? CreationDate { get; set; }

    public bool ValidFaite { get; set; }

    public string? ValidUtilisateurPoste { get; set; }

    public int ValidUtilisateurErp { get; set; }

    public string? ValidPosteId { get; set; }

    public string? ValidPosteIp { get; set; }

    public DateTime? ValidDate { get; set; }

    public int ApproIdChariot { get; set; }

    public string? ApproChariotDesignation { get; set; }

    public bool OptimChute { get; set; }

    public bool ApproChute { get; set; }

    public string? ApproChuteUtilisateurPoste { get; set; }

    public int ApproChuteUtilisateurErp { get; set; }

    public string? ApproChutePosteId { get; set; }

    public string? ApproChutePosteIp { get; set; }

    public DateTime? ApproChuteDate { get; set; }

    public bool OptimNeuf { get; set; }

    public bool ApproNeuf { get; set; }

    public string? ApproNeufUtilisateurPoste { get; set; }

    public int ApproNeufUtilisateurErp { get; set; }

    public string? ApproNeufPosteId { get; set; }

    public string? ApproNeufPosteIp { get; set; }

    public DateTime? ApproNeufDate { get; set; }

    public bool DecoupeDg { get; set; }

    public bool DecoupeDgEncours { get; set; }

    public DateTime? DecoupeDgDateDebut { get; set; }

    public DateTime? DecoupeDgDateFin { get; set; }
}
