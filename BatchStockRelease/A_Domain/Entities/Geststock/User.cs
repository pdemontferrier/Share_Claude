using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class User
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Initial { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public string TelPro { get; set; } = null!;

    public string TelFixePro { get; set; } = null!;

    public string? TelPerso { get; set; }

    public string MailPro { get; set; } = null!;

    public string? MailPerso { get; set; }

    public int TypeContrat { get; set; }

    public sbyte Acces { get; set; }

    public sbyte Exist { get; set; }

    public sbyte MdpReset { get; set; }

    public int? IdSecteur { get; set; }

    public int? LicenceE3 { get; set; }

    public int Societe { get; set; }

    public DateOnly? DateEntree { get; set; }

    public string? Adresse { get; set; }

    public int? CodePostal { get; set; }

    public string? Ville { get; set; }

    public string? Pays { get; set; }

    /// <summary>
    /// Pourcentage de production 
    /// </summary>
    public int? Prcp { get; set; }

    public string? MatriculePleiade { get; set; }

    public string? LoginWindows { get; set; }

    public virtual ICollection<UserAppMessage> UserAppMessages { get; set; } = new List<UserAppMessage>();

    public virtual ICollection<UserAppPageDroit> UserAppPageDroits { get; set; } = new List<UserAppPageDroit>();
}
