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
    public class ApiCountryRepository : ICountryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiCountryRepository> _logger;
        private readonly IApiRequestHandler _apiRequestHandler;

        public ApiCountryRepository(HttpClient httpClient,
                                    ILogger<ApiCountryRepository> logger,
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

        public async Task<Result<List<CountryDto>>> GetGendersAsync()
        {
            const string operationName = "получение списка стран";
            return await _apiRequestHandler.ExecuteApiCallAsync(async () =>
            {
                string getCountiesEndpoint = "api/countries/get-all-stream";

                HttpResponseMessage response = await _httpClient.GetAsync(getCountiesEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var countries = await response.Content.ReadFromJsonAsync<List<CountryDto>>(_jsonSerializerOptions);

                    var resultList = countries ?? new List<CountryDto>();

                    return Result<List<CountryDto>>.Success(resultList);
                }

                return await _apiRequestHandler.HandleApiErrorAsync<List<CountryDto>>(response, operationName);
            }, operationName);
        }
    }
}