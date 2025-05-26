namespace EnigmaVault.AuthenticationService.Domain.DomainModels
{
    public class GenderDomain
    {
        private GenderDomain() { }

        public int IdGender { get; private set; }

        public string GenderName { get; private set; } = null!;

        public static GenderDomain Reconstitute(int id, string genderName)
        {
            return new GenderDomain
            {
                IdGender = id,
                GenderName = genderName,
            };
        }
    }
}
