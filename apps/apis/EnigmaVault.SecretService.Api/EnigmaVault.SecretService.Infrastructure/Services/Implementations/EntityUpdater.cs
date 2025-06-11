using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using EnigmaVault.SecretService.Infrastructure.Data;
using EnigmaVault.SecretService.Infrastructure.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EnigmaVault.SecretService.Infrastructure.Services.Implementations
{
    internal class EntityUpdater(SecretDBContext context) : IEntityUpdater
    {
        private readonly SecretDBContext _context = context;

        public async Task<Result> UpdatePropertyAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate, 
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls) where TEntity : class
        {
            try
            {
                var countUpdatedRows = await _context.Set<TEntity>()
                    .Where(predicate)
                    .ExecuteUpdateAsync(setPropertyCalls);

                if (countUpdatedRows > 0)
                    return Result.Success();
                else
                    return Result.Failure(new Error(ErrorCode.NotFound, "Запись для обновления не найдена."));
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(ErrorCode.SaveError, "Произошла ошибка при сохранении изменений в базе данных."));
            }
        }

        public async Task<Result<TResult>> UpdateAndGetAsync<TEntity, TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls,
            Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            try
            {
                var countUpdatedRows = await _context.Set<TEntity>()
                    .Where(predicate)
                    .ExecuteUpdateAsync(setPropertyCalls);

                if (countUpdatedRows > 0)
                {
                    var result = await _context.Set<TEntity>()
                        .AsNoTracking()
                        .Where(predicate)
                        .Select(selector)
                        .FirstOrDefaultAsync();

                    if (result is not null && !result.Equals(default(TResult)))
                        return Result<TResult>.Success(result);

                    return Result<TResult>.Failure(new Error(ErrorCode.NotFound, "Запись была обновлена, но не удалось получить ее проекцию после обновления."));
                }

                return Result<TResult>.Failure(new Error(ErrorCode.NotFound, "Запись для обновления не найдена."));
            }
            catch (Exception ex)
            {
                return Result<TResult>.Failure(new Error(ErrorCode.SaveError, "Произошла ошибка при сохранении изменений в базе данных."));
            }
        }
    }
}