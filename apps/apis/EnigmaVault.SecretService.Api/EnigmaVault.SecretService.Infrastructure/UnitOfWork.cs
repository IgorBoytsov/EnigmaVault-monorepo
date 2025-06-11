using EnigmaVault.SecretService.Application.Abstractions;
using EnigmaVault.SecretService.Infrastructure.Data;

namespace EnigmaVault.SecretService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SecretDBContext _context;

        public UnitOfWork(SecretDBContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    }
}