namespace EnigmaVault.SecretService.Application.Features.Folders
{
    public class FolderDto
    {
        public int IdFolder { get; set; }

        public int IdUser { get; set; }

        public string FolderName { get; set; } = null!;
    }
}