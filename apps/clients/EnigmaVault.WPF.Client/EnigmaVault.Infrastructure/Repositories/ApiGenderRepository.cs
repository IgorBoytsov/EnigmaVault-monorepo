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
    public class ApiGenderRepository : IGenderRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiGenderRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;

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
                    List<GenderResponse>? gendersResponse = await response.Content.ReadFromJsonAsync<List<GenderResponse>>(_jsonSerializerOptions);

                    if (gendersResponse != null)
                    {
                        List<GenderDomain>? genders = gendersResponse.Select(g => GenderDomain.Reconstitute(g.IdGender, g.GenderName)).ToList();

                        return Result<List<GenderDomain>?>.Success(genders);
                    }
                    else
                    {
                        //TODO: Добавить логер
                        return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.EmptyValue, "API вернуло успешный статус, но данные списка не были получены (возможно, null)."));
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.AuthApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"JSON ошибка при получение списка гендоров: {ex.Message}. Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
                return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"Непредвиденная ошибка при получение списка гендоров: {ex}");
                return Result<List<GenderDomain>?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }
    }
}