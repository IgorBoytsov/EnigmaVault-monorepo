using System;
using System.Collections.Generic;

namespace EnigmaVault.SecretService.Infrastructure.Data.Entities;

public partial class Icon
{
    public int IdIcon { get; set; }

    public int? IdUser { get; set; }

    public string SvgCode { get; set; } = null!;

    public string IconName { get; set; } = null!;

    public bool IsCommon { get; set; }

    public int IdIconCategory { get; set; }

    public virtual IconCategory IdIconCategoryNavigation { get; set; } = null!;
}
