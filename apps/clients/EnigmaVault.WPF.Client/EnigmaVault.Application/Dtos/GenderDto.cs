namespace EnigmaVault.Application.Dtos
{
    public sealed record GenderDto(int IdGender, string GenderName)
    {
        public static readonly GenderDto Empty = new GenderDto(0, string.Empty);
    }
}