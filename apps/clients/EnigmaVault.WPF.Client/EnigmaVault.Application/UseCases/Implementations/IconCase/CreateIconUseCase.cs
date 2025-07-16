using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.IconCase;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.IconCase
{
    public class CreateIconUseCase(IIconRepository iconRepository, IMapper mapper) : ICreateIconUseCase
    {
        private readonly IIconRepository _iconRepository = iconRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<IconDto>> CreateAsync(int? idUser, string svgCode, string iconName)
        {
            var domain = IconDomain.Create(idUser, svgCode, iconName, false);

            var result = await _iconRepository.CreateAsync(domain);

            if (!result.IsSuccess)
                return Result<IconDto>.Failure(new Error(ErrorCode.ApiError, result.Errors.ToString()));

            var dto = _mapper.Map<IconDto>(result.Value);

            return Result<IconDto>.Success(dto);
        }
    }
}