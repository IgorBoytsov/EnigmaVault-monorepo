using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.AuthenticationService.Api.Controllers
{
    [ApiController]
    [Route("api/genders")]
    public class GendersController : ControllerBase
    {
        private readonly IGetAllGenderStreamingUseCase _getAllGenderStreamingUseCase;

        public GendersController(IGetAllGenderStreamingUseCase getAllCountryStreamingUseCase)
        {
            _getAllGenderStreamingUseCase = getAllCountryStreamingUseCase;
        }

        /*--Получение списка гендеров---------------------------------------------------------------------*/

        [HttpGet("get-all-stream")]
        public IAsyncEnumerable<GenderDto> GetCountriesStream(CancellationToken cancellationToken) => _getAllGenderStreamingUseCase.GetAllStreamingAsync(cancellationToken);
    }
}