using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Models.Request;
using EnigmaVault.Infrastructure.Models.Response;
using EnigmaVault.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnigmaVault.Infrastructure.Repositories
{
    public class ApiFolderRepository : IFolderRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiFolderRepository> _logger;
        private readonly IApiRequestHandler _apiRequestHandler;

        public ApiFolderRepository(HttpClient httpClient, 
                                   ILogger<ApiFolderRepository> logger,
                                   IApiRequestHandler apiRequestHandler)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiRequestHandler = apiRequestHandler;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        /*--Create----------------------------------------------------------------------------------------*/

        public async Task<Result<FolderDomain>> CreateAsync(FolderDomain folder)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string folderEndpoint = "api/folders/create";

                var folderRequest = new FolderRequest()
                {
                    UserId = folder.IdUser,
                    FolderName = folder.FolderName,
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(folderEndpoint, folderRequest, _jsonSerializerOptions);

                if (response.IsSuccessStatusCode)
                {
                    FolderResponse? folderResponse = await response.Content.ReadFromJsonAsync<FolderResponse>(_jsonSerializerOptions);

                    var folderDomain = FolderDomain.Reconstruct(folderResponse.IdFolder, folderResponse.IdUser, folderResponse.FolderName);

                    return Result<FolderDomain>.Success(folderDomain);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<FolderDomain>(response, "создание папки");
            }, "создание папки");
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async Task<Result<List<FolderDto>>> GetAllAsync(int userId)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string getFoldersEndpoint = $"api/folders/get-all?UserId={userId}";

                HttpResponseMessage response = await _httpClient.GetAsync(getFoldersEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    List<FolderResponse>? foldersResponse = await response.Content.ReadFromJsonAsync<List<FolderResponse>>(_jsonSerializerOptions);

                    if (foldersResponse == null)
                    {
                        return Result<List<FolderDto>>.Failure(new Error(ErrorCode.EmptyValue, "API вернуло успешный статус, но данные списка не были получены (возможно, null)."));
                    }

                    List<FolderDto> folders = foldersResponse.Select(f => new FolderDto
                    {
                        IdFolder = f.IdFolder,
                        IdUser = f.IdUser,
                        FolderName = f.FolderName,
                    })
                    .ToList();

                    return Result<List<FolderDto>>.Success(folders);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<List<FolderDto>>(response, "получение папок для секретов");
            }, "получение папок для секретов");
        }

        /*--Update----------------------------------------------------------------------------------------*/

        public async Task<Result> UpdateFolderNameAsync(int idFolder, string newName)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string endpoint = $"api/folders/{idFolder}/name";

                var payload = new
                {
                    Name = newName
                };

                HttpResponseMessage response = await _httpClient.PutAsJsonAsync(endpoint, payload);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Result.Success();
                }
                else
                {
                    return await _apiRequestHandler.HandleApiErrorAsync(response, "обновление названия папки");
                }

            }, "обновление названия папки");
        }

        /*--Delete----------------------------------------------------------------------------------------*/

        public async Task<Result> DeleteAsync(int idFolder)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string endpoint = $"api/folders/{idFolder}";

                HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                {
                    return Result.Success();
                }

                return await _apiRequestHandler.HandleApiErrorAsync(response, "удаление секрета");

            }, "удаление секрета");
        }

        /*--Exist-----------------------------------------------------------------------------------------*/
    }
}
