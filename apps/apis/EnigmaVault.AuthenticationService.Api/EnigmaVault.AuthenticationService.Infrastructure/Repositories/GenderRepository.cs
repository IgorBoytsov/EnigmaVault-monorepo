using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Infrastructure.Repositories
{
    public class GenderRepository(UsersDBContext context) : IGenderRepository
    {
        private readonly UsersDBContext _context = context;

        /*--Get-------------------------------------------------------------------------------------------*/

        public async IAsyncEnumerable<GenderDomain?> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var entitiesStream = _context.Genders.AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken);

            await foreach (var genderEntity in entitiesStream)
            {
                var domain = GenderDomain.Reconstitute(genderEntity.IdGender, genderEntity.GenderName);

                if (domain != null)
                    yield return domain;
            }
        }
    }
}