using System;
using System.Collections.Generic;

namespace EnigmaVault.SecretService.Infrastructure.Data.Entities;

public partial class Secret
{
    public int IdSecret { get; set; }

    public int IdUser { get; set; }

    public byte[] EncryptedData { get; set; } = null!;

    public byte[] Nonce { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public string? Url { get; set; }

    public string? Notes { get; set; }

    public int SchemaVersion { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime DateUpdate { get; set; }

    public bool IsFavorite { get; set; }
}
