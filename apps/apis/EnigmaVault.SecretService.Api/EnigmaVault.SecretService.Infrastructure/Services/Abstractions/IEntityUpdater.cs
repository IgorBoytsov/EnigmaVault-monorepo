using EnigmaVault.SecretService.Domain.Results;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EnigmaVault.SecretService.Infrastructure.Services.Abstractions
{
    internal interface IEntityUpdater
    {
        Task<Result> UpdatePropertyAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls) where TEntity : class;
        Task<Result<TResult>> UpdateAndGetAsync<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls, Expression<Func<TEntity, TResult>> selector) where TEntity : class;
    }
}