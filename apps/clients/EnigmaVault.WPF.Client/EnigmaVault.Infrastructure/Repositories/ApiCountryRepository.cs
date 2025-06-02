using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Models.Response;
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

        public ApiCountryRepository(HttpClient httpClient,
                                    ILogger<ApiCountryRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<Result<List<CountryDomain>?>> GetGendersAsync()
        {
            string getCountiesEndpoint = "api/countries/get-all-stream";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(getCountiesEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Попытка получение списка стран с EndPoint {getCountiesEndpoint}", getCountiesEndpoint);

                    List<CountryResponse>? countriesResponse = await response.Content.ReadFromJsonAsync<List<CountryResponse>>(_jsonSerializerOptions);

                    if (countriesResponse != null)
                    {
                        List<CountryDomain> countries = countriesResponse.Select(g => CountryDomain.Reconstitute(g.IdCountry, g.CountryName)).ToList();
                        _logger.LogInformation("Запрос успешно получен {@countries}", countries);
                        return Result<List<CountryDomain>?>.Success(countries);
                    }
                    else
                    {
                        _logger.LogError("API запрос выполнился, но данных не вернул {@response}", response);
                        return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.EmptyValue, "API вернуло успешный статус, но данные списка не были получены (возможно, null)."));
                    }      
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API не выполнил запрос {errorContent}", errorContent);
                    return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.ApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical(ex, "Сетевая ошибка при аутентификации");
                return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                _logger.LogCritical(ex, "JSON ошибка при аутентификации");
                return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Непредвиденная ошибка при получение списка стран");
                return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }
    }
}