namespace EnigmaVault.Infrastructure.Models.Response
{
    internal class FolderResponse
    {
        public int IdFolder { get; set; }

        public int IdUser { get; set; }

        public string FolderName { get; set; } = null!;
    }
}