namespace EnigmaVault.Domain.DomainModels
{
    public class FolderDomain
    {
        private FolderDomain() { }

        public int IdFolder { get; private set; }

        public int IdUser { get; private set; }

        public string FolderName { get; private set; } = null!;

        public static FolderDomain Create(int idUser, string name)
        {
            return new FolderDomain
            {
                IdUser = idUser,
                FolderName = name,
            };
        }

        public static FolderDomain Reconstruct(int idFolder, int idUser, string name)
        {
            return new FolderDomain
            {
                IdFolder = idFolder,
                IdUser = idUser,
                FolderName = name,
            };
        }

    }
}