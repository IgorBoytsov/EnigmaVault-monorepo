namespace EnigmaVault.Domain.DomainModels
{
    public class GenderDomain
    {
        private GenderDomain() { }

        public int IdGender { get; private set; }

        public string GenderName { get; private set; } = null!;

        public static GenderDomain Reconstitute(int idGender, string genderName)
        {
            return new GenderDomain()
            {
                IdGender = idGender,
                GenderName = genderName,
            };
        }
    }
}