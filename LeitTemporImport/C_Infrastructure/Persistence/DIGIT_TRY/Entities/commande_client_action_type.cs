using System;
using System.Collections.Generic;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

public partial class commande_client_action_type
{
    public int id { get; set; }

    public string designation { get; set; } = null!;

    public int id_statut_value { get; set; }

    public virtual commande_client_statut id_statut_valueNavigation { get; set; } = null!;
}
