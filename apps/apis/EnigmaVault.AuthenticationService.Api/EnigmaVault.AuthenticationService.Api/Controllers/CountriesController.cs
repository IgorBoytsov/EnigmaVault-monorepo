using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.AuthenticationService.Api.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly IGetAllCountryStreamingUseCase _getAllCountryStreamingUseCase;

        public CountriesController(IGetAllCountryStreamingUseCase getAllCountryStreamingUseCase)
        {
            _getAllCountryStreamingUseCase = getAllCountryStreamingUseCase;
        }

        /*--Получение списка стран------------------------------------------------------------------------*/

        [HttpGet("get-all-stream")]
        public IAsyncEnumerable<CountryDto?> GetCountriesStream(CancellationToken cancellationToken) => _getAllCountryStreamingUseCase.GetAllStreamingAsync(cancellationToken);
    }
}