using System.Text.Json.Serialization;

namespace EnigmaVault.Application.Dtos.Secrets.CryptoService
{
    public class CryptoParameters
    {
        [JsonPropertyName("salt")]
        public string Salt { get; set; }

        [JsonPropertyName("iterations")]
        public int Iterations { get; set; }

        [JsonPropertyName("memorySizeKb")]
        public int MemorySizeKb { get; set; }

        [JsonPropertyName("degreeOfParallelism")]
        public int DegreeOfParallelism { get; set; }
    }
}