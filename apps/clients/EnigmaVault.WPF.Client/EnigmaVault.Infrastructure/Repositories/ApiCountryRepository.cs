using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Models.Response;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnigmaVault.Infrastructure.Repositories
{
    public class ApiCountryRepository : ICountryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiCountryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;

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

                //TODO: Добавить логер
                if (response.IsSuccessStatusCode)
                {
                    List<CountryResponse>? countriesResponse = await response.Content.ReadFromJsonAsync<List<CountryResponse>>(_jsonSerializerOptions);

                    if (countriesResponse != null)
                        return Result<List<CountryDomain>?>.Success(countriesResponse.Select(g => CountryDomain.Reconstitute(g.IdCountry, g.CountryName)).ToList());
                    else
                        return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.EmptyValue, "API вернуло успешный статус, но данные списка не были получены (возможно, null)."));
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.AuthApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"JSON ошибка при получение списка стран: {ex.Message}. Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
                return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"Непредвиденная ошибка при получение списка стран: {ex}");
                return Result<List<CountryDomain>?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }
    }
}