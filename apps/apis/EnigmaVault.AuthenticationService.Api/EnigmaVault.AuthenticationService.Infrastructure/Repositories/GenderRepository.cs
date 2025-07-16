using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Infrastructure.Repositories
{
    public class GenderRepository(UsersDBContext context) : IGenderRepository
    {
        private readonly UsersDBContext _context = context;

        /*--Get-------------------------------------------------------------------------------------------*/

        public async IAsyncEnumerable<GenderDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var entitiesStream = _context.Genders.AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken);

            await foreach (var genderEntity in entitiesStream)
            {
                var gender = new GenderDto(genderEntity.IdGender, genderEntity.GenderName);
                yield return gender;
            }
        }
    }
}