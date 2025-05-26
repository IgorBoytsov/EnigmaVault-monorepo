using EnigmaVault.AuthenticationService.Api.Controllers;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using Moq;

namespace MyApi.Tests
{
    public class GendersControllerTests
    {
        private Mock<IGetAllGenderStreamingUseCase> _mockUseCase;
        private GendersController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockUseCase = new Mock<IGetAllGenderStreamingUseCase>();
            _controller = new GendersController(_mockUseCase.Object);
        }

        private async IAsyncEnumerable<GenderDto?> GetEmptyAsyncEnumerable()
        {
            await Task.CompletedTask;
            yield break;
        }

        [Test]
        public void GetGendersStream_CallsUseCase_AndReturnsItsResult()
        {
            var cancellationToken = new CancellationTokenSource().Token;
            var expectedStream = GetEmptyAsyncEnumerable();

            _mockUseCase.Setup(uc => uc.GetAllStreamingAsync(cancellationToken)).Returns(expectedStream);

            var resultStream = _controller.GetCountriesStream(cancellationToken);

            Assert.That(resultStream, Is.SameAs(expectedStream));
            _mockUseCase.Verify(uc => uc.GetAllStreamingAsync(cancellationToken), Times.Once);
        }
    }
}