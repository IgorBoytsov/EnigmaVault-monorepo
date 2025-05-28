using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Models.Request;
using EnigmaVault.Infrastructure.Models.Response;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnigmaVault.Infrastructure.Repositories
{
    public class ApiUserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiUserRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<Result<UserDomain?>> AuthenticationAsync(string login, string password)
        {
            string authenticationEndpoint = "api/users/authenticate";

            var authenticationRequest = new AuthRequest
            {
                Login = login,
                Password = password
            };

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(authenticationEndpoint, authenticationRequest, _jsonSerializerOptions);

                if (response.IsSuccessStatusCode)
                {
                    AuthResponse? authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonSerializerOptions);

                    var user = UserDomain.Reconstitute(authResponse.IdUser, authResponse.Login, authResponse.UserName, authResponse.Email, authResponse.Phone, authResponse.IdGender, authResponse.IdCountry);

                    return Result<UserDomain?>.Success(user);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    return Result<UserDomain?>.Failure(new Error(ErrorCode.AuthApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                return Result<UserDomain?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"JSON ошибка при аутентификации: {ex.Message}. Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
                return Result<UserDomain?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex) 
            {
                //TODO: Добавить логер
                Debug.WriteLine($"Непредвиденная ошибка при аутентификации: {ex}");
                return Result<UserDomain?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
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
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(registrationEndpoint, registerRequest, _jsonSerializerOptions);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return Result<ValueTuple<string, string>?>.Success((login, password));
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.RegisterApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"JSON ошибка при регистрации: {ex.Message}. Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"Непредвиденная ошибка при регистрации: {ex}");
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
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(recoveryAccessEndpoint, recoveryAccessRequest, _jsonSerializerOptions);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Result<ValueTuple<string, string>?>.Success((login, newPassword));
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.RecoveryApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
                }
            }
            catch (HttpRequestException ex)
            {
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"JSON ошибка при восстановление пароля: {ex.Message}. Path: {ex.Path}, LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                //TODO: Добавить логер
                Debug.WriteLine($"Непредвиденная ошибка восстановление пароля: {ex}");
                return Result<ValueTuple<string, string>?>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }
    }
}