using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class commande_client_statut
{
    public int id { get; set; }

    public int? statut_value { get; set; }

    public string? designation { get; set; }

    public virtual ICollection<commande_client_action_type> commande_client_action_types { get; set; } = new List<commande_client_action_type>();
}
