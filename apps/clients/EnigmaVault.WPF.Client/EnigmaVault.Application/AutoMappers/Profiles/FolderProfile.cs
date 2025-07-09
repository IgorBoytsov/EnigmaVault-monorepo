using AutoMapper;
using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.AutoMappers.Profiles
{
    public class FolderProfile : Profile
    {
        public FolderProfile()
        {
            CreateMap<FolderDto, FolderDomain>().ConstructUsing(dto => FolderDomain.Reconstruct(dto.IdFolder, dto.IdUser, dto.FolderName));

            CreateMap<FolderDomain, FolderDto>();
        }
    }
}