using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class commande_client_action
{
    public int id { get; set; }

    public int id_cmd_client { get; set; }

    public int id_user { get; set; }

    public int id_action { get; set; }

    public DateTime? date_action { get; set; }

    public int tempsEstime { get; set; }

    public short? bonus_Malus { get; set; }

    public virtual commande_client id_cmd_clientNavigation { get; set; } = null!;
}
