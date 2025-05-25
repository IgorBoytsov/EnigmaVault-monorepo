using EnigmaVault.AuthenticationService.Domain.Constants;

namespace EnigmaVault.AuthenticationService.Domain.DomainModels
{
    public class CountryDomain
    {
        private CountryDomain() { }
        
        public int IdCountry { get; private set; }

        public string CountryName { get; private set; } = null!;

        public static (CountryDomain? Country, List<string>? Errors) Create(string countryName)
        {
            List<string>? errors = null;

            if (string.IsNullOrWhiteSpace(countryName))
            {
                errors ??= [];
                errors.Add("Не было указанно название страны");
            }

            if (countryName.Length > CountryConstants.MaxCountryNameLength)
            {
                errors ??= [];
                errors.Add($"Длинна страны была {countryName.Length} что больше максимально допустимой длинны в {CountryConstants.MaxCountryNameLength}");
            }
               
            if (countryName.Length < CountryConstants.MinCountryNameLength)
            {
                errors ??= [];
                errors.Add($"Длинна страны была меньше или ровна 0 что меньше минимально допустимой длинны в {CountryConstants.MinCountryNameLength}");
            }
 
            if (errors is not null)
                return (null, errors);

            var country = new CountryDomain
            {
                CountryName = countryName,
            };

            return (country, errors);
        }

        public static CountryDomain Reconstitute(int id, string countryName)
        {
            return  new CountryDomain
            {
                IdCountry = id,
                CountryName = countryName,
            };
        }
    }
}