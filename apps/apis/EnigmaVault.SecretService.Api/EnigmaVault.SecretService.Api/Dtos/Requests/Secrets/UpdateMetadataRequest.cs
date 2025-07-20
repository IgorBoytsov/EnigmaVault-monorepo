namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public sealed record UpdateMetadataRequest(string ServiceName, string? Url);
}