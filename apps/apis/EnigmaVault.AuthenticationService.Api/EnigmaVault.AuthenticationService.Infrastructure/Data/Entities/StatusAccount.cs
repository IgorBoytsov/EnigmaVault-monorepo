using System;
using System.Collections.Generic;

namespace EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;

public partial class StatusAccount
{
    public int IdStatusAccount { get; set; }

    public string StatusName { get; set; } = null!;

    public string? StatusDescription { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
