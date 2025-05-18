using System;
using System.Collections.Generic;

namespace EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;

public partial class Country
{
    public int IdCountry { get; set; }

    public string CountryName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
