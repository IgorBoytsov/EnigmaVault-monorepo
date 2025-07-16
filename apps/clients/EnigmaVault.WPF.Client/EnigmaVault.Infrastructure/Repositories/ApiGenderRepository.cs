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
    public class ApiGenderRepository : IGenderRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiGenderRepository> _logger;
        private readonly IApiRequestHandler _apiRequestHandler;

        public ApiGenderRepository(HttpClient httpClient,
                                   ILogger<ApiGenderRepository> logger,
                                   IApiRequestHandler apiRequestHandler)
        {
            _httpClient = httpClient;
            _apiRequestHandler = apiRequestHandler;
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<Result<List<GenderDto>>> GetGendersAsync()
        {
            const string operationName = "получение списка гендеров";
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                const string endpoint = "api/genders/get-all-stream";
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var genders = await response.Content.ReadFromJsonAsync<List<GenderDto>>(_jsonSerializerOptions);

                    var resultList = genders ?? new List<GenderDto>();

                    _logger.LogInformation("Запрос на получение гендеров успешно обработан. Получено {Count} записей.", resultList.Count);

                    return Result<List<GenderDto>>.Success(resultList);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<List<GenderDto>>(response, operationName);

            }, operationName);
        }
    }
}