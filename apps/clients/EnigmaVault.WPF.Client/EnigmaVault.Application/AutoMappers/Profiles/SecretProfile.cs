using AutoMapper;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.AutoMappers.Profiles
{
    public class SecretProfile : Profile
    {
        public SecretProfile()
        {
            CreateMap<SecretDomain, EncryptedSecret>();

            //CreateMap<SecretDomain, EncryptedSecret>()
            //    .ForMember(
            //        dest => dest.DateUpdate,
            //        opt => opt.MapFrom(src => src.DateUpdate!.Value.ToLocalTime())
            //    );
        }
    }
}