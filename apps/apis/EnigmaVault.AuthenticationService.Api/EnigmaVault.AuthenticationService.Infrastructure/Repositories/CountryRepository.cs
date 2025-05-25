using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Infrastructure.Repositories
{
    public class CountryRepository(UsersDBContext context) : ICountryRepository
    {
        private readonly UsersDBContext _context = context;

        /*--Get-------------------------------------------------------------------------------------------*/

        public async IAsyncEnumerable<CountryDomain?> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var entitiesStream = _context.Countries.AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken);

            await foreach (var countryEntity in entitiesStream)
            {
                var domain = CountryDomain.Reconstitute(countryEntity.IdCountry, countryEntity.CountryName);

                if (domain != null)
                    yield return domain;
            }
        }
    }
}