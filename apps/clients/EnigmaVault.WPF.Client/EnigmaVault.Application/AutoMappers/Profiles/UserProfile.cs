using AutoMapper;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.AutoMappers.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, UserDomain>()
                .ConstructUsing(dto => UserDomain.Reconstitute(
                    dto.IdUser,
                    dto.Login,
                    dto.UserName,
                    dto.Email,
                    dto.Phone,
                    dto.IdGender,
                    dto.IdCountry));

            CreateMap<UserDomain, UserDto>();
        }
    }
}