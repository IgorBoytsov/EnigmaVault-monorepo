using System;
using System.Collections.Generic;

namespace EnigmaVault.SecretService.Infrastructure.Data.Entities;

public partial class IconCategory
{
    public int IdCategory { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Icon> Icons { get; set; } = new List<Icon>();
}
