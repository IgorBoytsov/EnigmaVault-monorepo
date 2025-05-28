namespace EnigmaVault.Domain.DomainModels
{
    public class CountryDomain
    {
        private CountryDomain() { }

        public int IdCountry { get; private set; }

        public string CountryName { get; private set; } = null!;

        public static CountryDomain Reconstitute(int idCountry, string coumtryName)
        {
            return new CountryDomain()
            {
                IdCountry = idCountry,
                CountryName = coumtryName,
            };
        }
    }
}