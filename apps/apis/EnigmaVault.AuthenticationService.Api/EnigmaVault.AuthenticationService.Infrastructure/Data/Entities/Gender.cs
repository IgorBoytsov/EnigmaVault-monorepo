using System;
using System.Collections.Generic;

namespace EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;

public partial class Gender
{
    public int IdGender { get; set; }

    public string GenderName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
