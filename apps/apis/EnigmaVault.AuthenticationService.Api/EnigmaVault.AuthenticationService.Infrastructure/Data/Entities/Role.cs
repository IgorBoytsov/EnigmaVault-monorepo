using System;
using System.Collections.Generic;

namespace EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;

public partial class Role
{
    public int IdRole { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
