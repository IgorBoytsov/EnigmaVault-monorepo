using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Application.UseCases.Abstractions.FolderCase;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.FolderCase
{
    public class CreateFolderUseCase(IFolderRepository folderRepository, IMapper mapper) : ICreateFolderUseCase
    {
        private readonly IFolderRepository _folderRepository = folderRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<FolderDto>> CreateAsync(int idUser, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<FolderDto>.Failure(new Error(ErrorCode.EmptyValue, "Вы не указали название для папки."));

            var domain = FolderDomain.Create(idUser, name);

            if (domain == null)
                return Result<FolderDto>.Failure(new Error(ErrorCode.CreateError, "Ошибка создания доменной модели."));

            var result = await _folderRepository.CreateAsync(domain);

            if (!result.IsSuccess)
                return Result<FolderDto>.Failure(new Error(ErrorCode.ApiError, result.Errors.ToString()));

            var dto = _mapper.Map<FolderDto>(result.Value);

            return Result<FolderDto>.Success(dto);
        }
    }
}