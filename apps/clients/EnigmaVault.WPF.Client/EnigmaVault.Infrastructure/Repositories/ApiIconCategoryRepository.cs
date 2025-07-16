using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnigmaVault.Infrastructure.Repositories
{
    public class ApiIconCategoryRepository : IIconCategoryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiIconRepository> _logger;
        private readonly IApiRequestHandler _apiRequestHandler;

        public ApiIconCategoryRepository(HttpClient httpClient,
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

        /*--Get-------------------------------------------------------------------------------------------*/

        public async Task<Result<List<IconCategoryDto>?>> GetAllAsync()
        {
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string getSecretEndpoint = $"api/icon-category/get-all";

                HttpResponseMessage response = await _httpClient.GetAsync(getSecretEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    List<IconCategoryDto>? secretsResponse = await response.Content.ReadFromJsonAsync<List<IconCategoryDto>>(_jsonSerializerOptions);

                    if (secretsResponse == null)
                        return Result<List<IconCategoryDto>?>.Failure(new Error(ErrorCode.EmptyValue, "API вернуло успешный статус, но данные списка не были получены (null)."));

                    return Result<List<IconCategoryDto>?>.Success(secretsResponse);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<List<IconCategoryDto>?>(response, "получение иконок");
            }, "получение иконок");
        }
    }
}
