using System;
using System.Collections.Generic;

namespace EnigmaVault.SecretService.Infrastructure.Data.Entities;

public partial class Folder
{
    public int IdFolder { get; set; }

    public int IdUser { get; set; }

    public string FolderName { get; set; } = null!;

    public virtual ICollection<Secret> Secrets { get; set; } = new List<Secret>();
}
