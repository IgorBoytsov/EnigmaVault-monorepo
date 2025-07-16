namespace EnigmaVault.Application.Dtos
{
    public sealed record CountryDto(int IdCountry, string CountryName)
    {
        public static readonly CountryDto Empty = new(0, string.Empty);
    }
}