using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserAppMessage
{
    public int Id { get; set; }

    public int IdAppSender { get; set; }

    public int IdUserSender { get; set; }

    public int IdAppRecepient { get; set; }

    public DateTime SentDate { get; set; }

    public string Subject { get; set; } = null!;

    public string? Content { get; set; }

    public bool IsRead { get; set; }

    public virtual User IdUserSenderNavigation { get; set; } = null!;
}
