namespace EnigmaVault.SecretService.Api.Dtos.Requests.Folders
{
    public sealed record CreateFolderRequest(int UserId, string FolderName);
}