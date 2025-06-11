using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Models.Request;
using EnigmaVault.Infrastructure.Models.Response;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnigmaVault.Infrastructure.Repositories
{
    public class ApiUserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiUserRepository> _logger;

        public ApiUserRepository(HttpClient httpClient, 
                                 ILogger<ApiUserRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<Result<(UserDomain? User, CryptoParameters CryptoParameters)>> AuthenticationAsync(string login, string password)
        {
            string authenticationEndpoint = "api/users/authenticate";

            var authenticationRequest = new AuthRequest
            {
                Login = login,
                Password = password
            };
            
            try
            {
                _logger.LogInformation("Попытка отправки запроса {authenticationRequest} отправлен на EndPoint {authenticationEndpoint}", authenticationRequest, authenticationEndpoint);

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(authenticationEndpoint, authenticationRequest, _jsonSerializerOptions);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Запрос успешно получен");
                    AuthResponse? authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonSerializerOptions);

                    _logger.LogInformation("Попытка реконструкции результата {Content}", response.Content);
                    var user = UserDomain.Reconstitute(authResponse!.IdUser, authResponse.Login, authResponse.UserName, authResponse.Email, authResponse.Phone, authResponse.IdGender, authResponse.IdCountry);
                    _logger.LogInformation("Успешная аутентификация. Результат реконструкции {@user}", user);

                    return Result<ValueTuple<UserDomain?, CryptoParameters>>.Success((user, authResponse.CryptoParameters));
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Ошибка аутентификация: {errorContent}", errorContent);
                    return Result<ValueTuple<UserDomain?, CryptoParameters>>.Failure(new Error(ErrorCode.ApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical(ex, "Сетевая ошибка при аутентификации");
                return Result<ValueTuple<UserDomain?, CryptoParameters>>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                _logger.LogCritical(ex, "JSON ошибка при аутентификации");
                return Result<ValueTuple<UserDomain?, CryptoParameters>>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex) 
            {
                _logger.LogCritical(ex, "Непредвиденная ошибка при аутентификации");
                return Result<ValueTuple<UserDomain?, CryptoParameters>>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }

        public async Task<Result<(string Login, string Password)?>> RegistrationAsync(string login, string password, string userName, string email, string? phone, int idGender, int idCountry)
        {
            string registrationEndpoint = "api/users/register";

            var registerRequest = new RegisterRequest
            {
                Login = login,
                Password = password,
                UserName = userName,
                Email = email,
                Phone = phone,
                IdGender = idGender,
                IdCountry = idCountry
            };

            try
            {
                _logger.LogInformation("Попытка отправки запроса {registerRequest} отправлен на EndPoint {registrationEndpoint}", registerRequest, registrationEndpoint);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(registrationEndpoint, registerRequest, _jsonSerializerOptions);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    _logger.LogInformation("Запрос успешно выполнен");
                    return Result<ValueTuple<string, string>?>.Success((login, password));
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Ошибка регистрации: {errorContent}", errorContent);
                    return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.RegisterApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical(ex, "Сетевая ошибка при регистрации");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                _logger.LogCritical(ex, "JSON ошибка при аутентификации");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Непредвиденная ошибка при регистрации");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }

        public async Task<Result<(string Login, string NewPassword)?>> RecoveryAccessAsync(string login, string email, string newPassword)
        {
            string recoveryAccessEndpoint = "api/users/recovery-access";

            var recoveryAccessRequest = new RecoveryAccessRequest
            {
                Login = login,
                Email = email,
                NewPassword = newPassword
            };

            try
            {
                _logger.LogInformation("Попытка отправки запроса {recoveryAccessRequest} отправлен на EndPoint {recoveryAccessEndpoint}", recoveryAccessRequest, recoveryAccessEndpoint);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(recoveryAccessEndpoint, recoveryAccessRequest, _jsonSerializerOptions);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("Запрос успешно выполнен");
                    return Result<ValueTuple<string, string>?>.Success((login, newPassword));
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Ошибка восстановление доступа: {errorContent}", errorContent);
                    return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.RecoveryApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical(ex, "Сетевая ошибка при регистрации");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                _logger.LogCritical(ex, "JSON ошибка при восстановление доступа к учетной записи");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Непредвиденная ошибка при восстановление доступа к учетной записи");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }
    }
}