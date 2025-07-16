using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Domain.Constants;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Dtos.Response;
using EnigmaVault.Infrastructure.Models.Request;
using EnigmaVault.Infrastructure.Models.Response;
using EnigmaVault.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnigmaVault.Infrastructure.Repositories
{
    public class ApiSecretRepository : ISecretRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiSecretRepository> _logger;
        private readonly IApiRequestHandler _apiRequestHandler;

        public ApiSecretRepository(HttpClient httpClient,
                                   ILogger<ApiSecretRepository> logger,
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

        public async Task<Result<SecretDomain?>> CreateAsync(SecretDomain secret)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string secretEndpoint = "api/secrets/create";

                var secretCommand = new SecretRequest()
                {
                    IdUser = secret.IdUser,
                    EncryptedData = secret.EncryptedData,
                    Nonce = secret.Nonce,
                    ServiceName = secret.ServiceName,
                    Url = secret.Url,
                    Notes = secret.Notes,
                    SchemaVersion = secret.SchemaVersion,
                    IsFavorite = secret.IsFavorite,
                };

                _logger.LogInformation("Попытка отправки секрета {@secretCommand} для его внесение в БД на EndPoint {secretEndpoint}", secretCommand, secretEndpoint);

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(secretEndpoint, secretCommand, _jsonSerializerOptions);
               
                if (response.IsSuccessStatusCode)
                {
                    SecretResponse? secretResponse = await response.Content.ReadFromJsonAsync<SecretResponse>(_jsonSerializerOptions);

                    var secretDomain = SecretDomain.Reconstruct(
                        secretResponse!.IdSecret, secretResponse.IdFolder,
                        secretResponse.EncryptedData, secretResponse.Nonce,
                        secretResponse.ServiceName, secretResponse.Url, secretResponse.Notes, secretResponse.SvgIcon, secretResponse.SchemaVersion,
                        secretResponse.DateAdded, secretResponse.DateUpdate, secretResponse.IsFavorite);

                    _logger.LogInformation("Секрет успешно создан: {@secretDomain}", secretDomain);

                    return Result<SecretDomain?>.Success(secretDomain);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<SecretDomain?>(response, "создание секрета");
            }, "создание секрета");
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async Task<Result<List<SecretDomain>?>> GetAllAsync(int userId)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string getSecretEndpoint = $"api/secrets/get-all?UserId={userId}";

                _logger.LogInformation("Начало обработки информации из запроса к API");

                HttpResponseMessage response = await _httpClient.GetAsync(getSecretEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    List<SecretResponse>? secretsResponse = await response.Content.ReadFromJsonAsync<List<SecretResponse>>(_jsonSerializerOptions);

                    if (secretsResponse == null)
                    {
                        _logger.LogWarning("API вернуло успешный статус, но данные списка не были получены (null).");
                        return Result<List<SecretDomain>?>.Failure(new Error(ErrorCode.EmptyValue, "API вернуло успешный статус, но данные списка не были получены (null)."));
                    }

                    List<SecretDomain> secrets = secretsResponse.Select(s => SecretDomain.Reconstruct(
                        s.IdSecret, s.IdFolder, s.EncryptedData, s.Nonce, s.ServiceName, s.Url, s.Notes, s.SvgIcon, s.SchemaVersion,
                        s.DateAdded, s.DateUpdate, s.IsFavorite)).ToList();

                    _logger.LogInformation("Список из {Count} секретов успешно получен.", secrets.Count);
                    return Result<List<SecretDomain>?>.Success(secrets);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<List<SecretDomain>?>(response, "получение списка секретов");
            }, "получение списка секретов");
        }

        /*--Update----------------------------------------------------------------------------------------*/    

        public async Task<Result<DateTime>> UpdateAsync(SecretDomain secret)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync<DateTime>(async () =>
            {
                string updateSecretEndpoint = $"api/secrets/{secret.IdSecret}/all-secret";

                var secretPayload = new
                {
                    ServiceName = secret.ServiceName,
                    Url = secret.Url,
                    IsFavorite = secret.IsFavorite,
                    Note = secret.Notes,
                    EncryptedData = secret.EncryptedData,
                    Nonce = secret.Nonce,
                    SchemaVersion = secret.SchemaVersion,
                };

                HttpResponseMessage response = await _httpClient.PutAsJsonAsync(updateSecretEndpoint, secretPayload);

                if (response.IsSuccessStatusCode)
                {
                    var responseDto = await response.Content.ReadFromJsonAsync<UpdateSecretResponse>();
                    if (responseDto == null)
                        return Result<DateTime>.Failure(new Error(ErrorCode.InvalidResponseFormat, "API вернул успешный статус, но тело ответа пустое."));

                    return Result<DateTime>.Success(responseDto.DateUpdate);
                }
                else
                {
                    return await _apiRequestHandler.HandleApiErrorAsync<DateTime>(response, "обновление метаданных секрета");
                }

            }, "обновление метаданных секрета");
        }

        public async Task<Result<DateTime>> UpdateMetaDataAsync(int idSecret, string serviceName, string? url)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync<DateTime>(async () =>
            {
                string updateMetadataEndpoint = $"api/secrets/{idSecret}/meta-data";
                var metaDataPayload = new 
                { 
                    ServiceName = serviceName, 
                    Url = url 
                };

                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(updateMetadataEndpoint, metaDataPayload);

                if (response.IsSuccessStatusCode)
                {
                    var responseDto = await response.Content.ReadFromJsonAsync<UpdateSecretResponse>();
                    if (responseDto == null)
                        return Result<DateTime>.Failure(new Error(ErrorCode.InvalidResponseFormat, "API вернул успешный статус, но тело ответа пустое."));

                    return Result<DateTime>.Success(responseDto.DateUpdate);
                }
                else
                {
                    return await _apiRequestHandler.HandleApiErrorAsync<DateTime>(response, "обновление метаданных секрета");
                }

            }, "обновление метаданных секрета");
        }

        public async Task<Result<DateTime>> UpdateEncryptedDataAsync(int idSecret, string encryptedData, string nonce)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync<DateTime>(async () =>
            {
                string endpoint = $"api/secrets/{idSecret}/encrypted-data";

                var payload = new 
                { 
                    EncryptedData = encryptedData, 
                    Nonce = nonce,
                    SchemaVersion = SchemaVersions.CURENT_SCHEMA_VERSION,
                };

                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(endpoint, payload);

                if (response.IsSuccessStatusCode)
                {
                    var responseDto = await response.Content.ReadFromJsonAsync<UpdateSecretResponse>();
                    if (responseDto == null)
                        return Result<DateTime>.Failure(new Error(ErrorCode.InvalidResponseFormat, "API вернул успешный статус, но тело ответа пустое."));

                    return Result<DateTime>.Success(responseDto.DateUpdate);
                }
                else
                {
                    return await _apiRequestHandler.HandleApiErrorAsync<DateTime>(response, "обновление метаданных секрета");
                }

            }, "обновление зашифрованных данных секрета");
        }

        public async Task<Result<DateTime>> UpdateFavoriteAsync(int idSecret, bool isFavorite)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync<DateTime>(async () =>
            {
                string endpoint = $"api/secrets/{idSecret}/favorite";

                var payload = new 
                { 
                    IsFavorite = isFavorite 
                };

                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(endpoint, payload);

                if (response.IsSuccessStatusCode)
                {
                    var responseDto = await response.Content.ReadFromJsonAsync<UpdateSecretResponse>();
                    if (responseDto == null)
                        return Result<DateTime>.Failure(new Error(ErrorCode.InvalidResponseFormat, "API вернул успешный статус, но тело ответа пустое."));

                    return Result<DateTime>.Success(responseDto.DateUpdate);
                }
                else
                {
                    return await _apiRequestHandler.HandleApiErrorAsync<DateTime>(response, "обновление метаданных секрета");
                }

            }, "обновление статуса 'избранное'");
        }

        public async Task<Result<DateTime>> UpdateNoteAsync(int idSecret, string? note)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string endpoint = $"api/secrets/{idSecret}/note";

                var payload = new 
                {
                    Note = note 
                };

                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(endpoint, payload);

                if (response.IsSuccessStatusCode)
                {
                    var responseDto = await response.Content.ReadFromJsonAsync<UpdateSecretResponse>();
                    if (responseDto == null)
                        return Result<DateTime>.Failure(new Error(ErrorCode.InvalidResponseFormat, "API вернул успешный статус, но тело ответа пустое."));

                    return Result<DateTime>.Success(responseDto.DateUpdate);
                }
                else
                {
                    return await _apiRequestHandler.HandleApiErrorAsync<DateTime>(response, "обновление метаданных секрета");
                }

            }, "обновление заметки");
        }

        public async Task<Result> UpdateIconAsync(int idSecret, string svgCode)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync( async () =>
            {
                string endpoint = $"api/secrets/{idSecret}/icon";

                var payload = new
                {
                    SvgIcon = svgCode
                };

                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(endpoint , payload);

                if (response.IsSuccessStatusCode)
                    return Result.Success();
                else
                    return await _apiRequestHandler.HandleApiErrorAsync(response, "обновление иконка у секрета");
            }, "обновление иконка у секрета");
        }

        public async Task<Result> UpdateFolderAsync(int idSecret, int? idFolder)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string endpoint = $"api/secrets/{idSecret}/folder";

                var payload = new
                {
                    IdFolder = idFolder
                };

                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(endpoint, payload);

                if (response.StatusCode == HttpStatusCode.OK)
                    return Result.Success();
                else
                    return await _apiRequestHandler.HandleApiErrorAsync(response, "обновление папки, в которой лежит секрет");

            }, "обновление папки, в которой лежит секрет");
        }

        /*--Delete----------------------------------------------------------------------------------------*/

        public async Task<Result<int?>> DeleteAsync(int idSecret)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string endpoint = $"api/secrets/{idSecret}";

                HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("Секрет с ID {IdSecret} успешно удален.", idSecret);
                    return Result<int?>.Success(idSecret);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<int?>(response, "удаление секрета");

            }, "удаление секрета");
        }

        /*--Exist-----------------------------------------------------------------------------------------*/

    }
}