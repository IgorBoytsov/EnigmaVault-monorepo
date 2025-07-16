using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.DTOs;
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

        public async IAsyncEnumerable<CountryDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var entitiesStream = _context.Countries.AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken);

            await foreach (var countryEntity in entitiesStream)
            {
                var country = new CountryDto(countryEntity.IdCountry, countryEntity.CountryName);

                if (country != null)
                    yield return country;
            }
        }
    }
}