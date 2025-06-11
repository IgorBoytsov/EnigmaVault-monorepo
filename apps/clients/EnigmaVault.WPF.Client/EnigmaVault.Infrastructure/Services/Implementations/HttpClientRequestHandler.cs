using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using EnigmaVault.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EnigmaVault.Infrastructure.Services.Implementations
{
    public class HttpClientRequestHandler(ILogger<HttpClientRequestHandler> logger) : IApiRequestHandler
    {
        private readonly ILogger<HttpClientRequestHandler> _logger = logger;

        public async Task<Result<T>> ExecuteApiCallAsync<T>(Func<Task<Result<T>>> apiCallLogic, string operationName)
        {
            try
            {
                return await apiCallLogic();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical(ex, "Сетевая ошибка при выполнении операции: {OperationName}", operationName);
                return Result<T>.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                _logger.LogCritical(ex, "Ошибка десериализации JSON при выполнении операции: {OperationName}", operationName);
                return Result<T>.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Непредвиденная ошибка при выполнении операции: {OperationName}", operationName);
                return Result<T>.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }

        public async Task<Result> ExecuteApiCallAsync(Func<Task<Result>> apiCallLogic, string operationName)
        {
            try
            {
                return await apiCallLogic();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical(ex, "Сетевая ошибка при выполнении операции: {OperationName}", operationName);
                return Result.Failure(new Error(ErrorCode.NetworkError, $"Сетевая ошибка: {ex.Message}"));
            }
            catch (JsonException ex)
            {
                _logger.LogCritical(ex, "Ошибка десериализации JSON при выполнении операции: {OperationName}", operationName);
                return Result.Failure(new Error(ErrorCode.InvalidResponseFormat, $"Ошибка формата ответа от API: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Непредвиденная ошибка при выполнении операции: {OperationName}", operationName);
                return Result.Failure(new Error(ErrorCode.Unknown, $"Произошла непредвиденная ошибка: {ex.Message}"));
            }
        }

        public async Task<Result<T>> HandleApiErrorAsync<T>(HttpResponseMessage response, string operationName)
        {
            string errorContent = await response.Content.ReadAsStringAsync();

            _logger.LogError("Ошибка API при выполнении операции '{OperationName}': {StatusCode}. Ответ: {ErrorContent}", operationName, response.StatusCode, errorContent);

            return Result<T>.Failure(new Error(ErrorCode.ApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
        }

        public async Task<Result> HandleApiErrorAsync(HttpResponseMessage response, string operationName)
        {
            string errorContent = await response.Content.ReadAsStringAsync();

            _logger.LogError("Ошибка API при выполнении операции '{OperationName}': {StatusCode}. Ответ: {ErrorContent}", operationName, response.StatusCode, errorContent);

            return Result.Failure(new Error(ErrorCode.ApiError, $"Ошибка API: {response.StatusCode}. {errorContent}"));
        }
    }
}
