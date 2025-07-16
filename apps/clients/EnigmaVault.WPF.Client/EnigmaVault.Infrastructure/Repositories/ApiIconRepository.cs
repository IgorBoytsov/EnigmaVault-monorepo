using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
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
    public class ApiIconRepository : IIconRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiIconRepository> _logger;
        private readonly IApiRequestHandler _apiRequestHandler;

        public ApiIconRepository(HttpClient httpClient,
                                 ILogger<ApiIconRepository> logger,
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

        public async Task<Result<IconDomain?>> CreateAsync(IconDomain icon)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string secretEndpoint = "api/icons/create";

                var iconCommand = new IconRequest(icon.IdUser, icon.SvgCode, false);

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(secretEndpoint, iconCommand, _jsonSerializerOptions);

                if (response.IsSuccessStatusCode)
                {
                    IconResponse? iconResponse = await response.Content.ReadFromJsonAsync<IconResponse>(_jsonSerializerOptions);

                    var iconDomain = IconDomain.Reconstruct(iconResponse!.IdIcon, iconResponse.IdUser, iconResponse.SvgCode, iconResponse.IconName, iconResponse.IdIconCategory, iconResponse.IsCommon);

                    return Result<IconDomain?>.Success(iconDomain);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<IconDomain?>(response, "создание иконки");
            }, "создание иконки");
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async Task<Result<List<IconDto>?>> GetAllAsync(int userId)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string getSecretEndpoint = $"api/icons/get-all?UserId={userId}";

                HttpResponseMessage response = await _httpClient.GetAsync(getSecretEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    List<IconDto>? secretsResponse = await response.Content.ReadFromJsonAsync<List<IconDto>>(_jsonSerializerOptions);

                    if (secretsResponse == null)
                        return Result<List<IconDto>?>.Failure(new Error(ErrorCode.EmptyValue, "API вернуло успешный статус, но данные списка не были получены (null)."));

                    return Result<List<IconDto>?>.Success(secretsResponse);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<List<IconDto>?>(response, "получение иконок");
            }, "получение иконок");
        }

        /*--Update----------------------------------------------------------------------------------------*/

        public async Task<Result> UpdateAsync(int idUser, int idIcon, string name, string? svgCode)
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string endpoint = $"api/icons/{idIcon}/update";

                var payload = new
                {
                    IdUser = idUser,
                    IconName = name,
                    SvgCode = svgCode
                };

                HttpResponseMessage response = await _httpClient.PatchAsJsonAsync(endpoint, payload);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Result.Success();
                }
                else
                {
                    return await _apiRequestHandler.HandleApiErrorAsync(response, "обновление данных иконки");
                }

            }, "обновление данных иконки");
        }
    }
}