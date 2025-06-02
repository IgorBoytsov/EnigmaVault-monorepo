using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.GanderCase;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.GenderCase
{
    public class GetGendersUseCase(IGenderRepository genderRepository,
                                   IMapper mapper,
                                   ILogger<GetGendersUseCase> logger) : IGetGendersUseCase
    {
        private readonly IGenderRepository _genderRepository = genderRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetGendersUseCase> _logger = logger;

        public async Task<Result<List<GenderDto>?>> GetGendersAsync()
        {
            _logger.LogInformation("GetGendersUseCase начал выполнение _genderRepository.GetGendersAsync()");

            var result = await _genderRepository.GetGendersAsync();

            if (result is not null && result.IsSuccess)
            {
                _logger.LogInformation("_genderRepository.GetGendersAsync() выполнился успешно");
                return Result<List<GenderDto>?>.Success(result.Value!.Select(g => _mapper.Map<GenderDto>(g)).ToList());
            }
            else
            {
                var errors = result!.Errors.ToList();
                _logger.LogInformation("_genderRepository.GetGendersAsync() выполнился с ошибками: {@errors}", errors);
                return Result<List<GenderDto>?>.Failure(errors);
            }
            
        }
    }
}