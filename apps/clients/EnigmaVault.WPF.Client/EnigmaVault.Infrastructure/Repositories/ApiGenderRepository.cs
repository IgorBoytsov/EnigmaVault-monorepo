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
    public class ApiGenderRepository : IGenderRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiGenderRepository> _logger;

        public ApiGenderRepository(HttpClient httpClient,
                                   ILogger<ApiGenderRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<Result<List<GenderDomain>?>> GetGendersAsync()
        {
            string getGendersEndpoint = "api/genders/get-all-stream";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(getGendersEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Попытка получение списка гендеров с EndPoint {getGendersEndpoint}", getGendersEndpoint);

                    List<GenderResponse>? gendersResponse = await response.Content.ReadFromJsonAsync<List<GenderResponse>>(_jsonSerializerOptions);

                    if (gendersResponse != null)
                    {
                        List<GenderDomain>? genders = gendersResponse.Select(g => GenderDomain.Reconstitute(g.IdGender, g.GenderName)).ToList();
                        _logger.LogInformation("Запрос успешно получен {@genders}", genders);
                        return Result<List<GenderDomain>?>.Success(genders);
                    }
                    else
                    {
                        _logger.LogError("API запрос выполнился, но данных не вернул {@response}", response);
                        return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.EmptyValue, "Запрос выполнился успешно, но данные списка не были получены."));
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API не выполнил запрос {errorContent}", errorContent);
                    return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.ApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical(ex, "Сетевая ошибка при аутентификации");
                return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                _logger.LogCritical(ex, "JSON ошибка при аутентификации");
                return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Непредвиденная ошибка при получение списка гендеров");
                return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }
    }
}