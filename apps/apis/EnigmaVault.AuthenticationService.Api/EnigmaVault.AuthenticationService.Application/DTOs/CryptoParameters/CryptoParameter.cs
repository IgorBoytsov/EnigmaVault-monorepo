namespace EnigmaVault.AuthenticationService.Application.DTOs.CryptoParameters
{
    public class CryptoParameter
    {
        public byte[] Salt { get; set; }
        public int DegreeOfParallelism { get; set; }
        public int Iterations { get; set; }
        public int MemorySizeKb { get; set; }
    }
}