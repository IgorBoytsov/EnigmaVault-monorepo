using EnigmaVault.Domain.Results;

namespace EnigmaVault.Infrastructure.Services.Abstractions
{
    public interface IApiRequestHandler
    {
        /// <summary>
        /// Выполняет вызов API и обрабатывает общие исключения.
        /// </summary>
        public Task<Result<T>> ExecuteApiCallAsync<T>(Func<Task<Result<T>>> apiCallLogic, string operationName);

        /// <summary>
        /// Перегрузка для методов, возвращающих не-generic Result.
        /// </summary>
        public Task<Result> ExecuteApiCallAsync(Func<Task<Result>> apiCallLogic, string operationName);

        /// <summary>
        /// Обрабатывает неуспешный HTTP-ответ, преобразуя его в Result.Failure.
        /// </summary>
        public Task<Result<T>> HandleApiErrorAsync<T>(HttpResponseMessage response, string operationName);

        /// <summary>
        /// Перегрузка для не-generic Result.
        /// </summary>
        public Task<Result> HandleApiErrorAsync(HttpResponseMessage response, string operationName);
    }
}