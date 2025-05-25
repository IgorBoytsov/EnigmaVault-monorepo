using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Application.Implementations.UseCases;
using EnigmaVault.AuthenticationService.Application.Mappers;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using Moq;
using System.Runtime.CompilerServices;

namespace MyApplication.Tests
{
    internal class GetAllCountryStreamingUseCaseTests
    {
        private Mock<ICountryRepository> _countryRepositoryMock;
        private GetAllCountryStreamingUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _countryRepositoryMock = new Mock<ICountryRepository>();
            _useCase = new GetAllCountryStreamingUseCase(_countryRepositoryMock.Object);
        }

        private static async IAsyncEnumerable<T> CreateAsyncEnumerable<T>(IEnumerable<T> items, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield(); // Имитируем асинхронность, чтобы проверка токена имела шанс сработать
                yield return item;
            }
        }

        private static async IAsyncEnumerable<CountryDomain> CreateAsyncEnumerableThrowingException(IEnumerable<CountryDomain> itemsToYieldFirst, string errorMessage, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in itemsToYieldFirst)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return item;
            }
            throw new InvalidOperationException(errorMessage);
        }

        private static List<CountryDomain> CreateSampleCountryDomains()
        {
            return new List<CountryDomain>
            {
                CountryDomain.Reconstitute(1, "Country A"),
                CountryDomain.Reconstitute(2, "Country B"),
                CountryDomain.Reconstitute(3, "Country C"),
                CountryDomain.Reconstitute(4, "Country D"),
            };
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenRepositoryReturnsCountries_ShouldReturnMappedDtos()
        {
            var domainCountries = CreateSampleCountryDomains();

            _countryRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerable(domainCountries, ct));

            var expectedDtos = domainCountries.Select(d => d.ToDto()).ToList();
            var resultDtos = new List<CountryDto?>();

            await foreach (var dto in _useCase.GetAllStreamingAsync())
                resultDtos.Add(dto);

            Assert.That(resultDtos.Count, Is.EqualTo(expectedDtos.Count));
            for (int i = 0; i < resultDtos.Count; i++)
            {
                Assert.That(resultDtos[i]?.IdCountry, Is.EqualTo(expectedDtos[i]?.IdCountry));
                Assert.That(resultDtos[i]?.CountryName, Is.EqualTo(expectedDtos[i]?.CountryName));
            }

            _countryRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenRepositoryReturnsEmptyStream_ShouldReturnEmptyStream()
        {
            var emptyDomainCountries = new List<CountryDomain>();

            _countryRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerable(emptyDomainCountries, ct));

            var resultDtos = new List<CountryDto?>();

            await foreach (var dto in _useCase.GetAllStreamingAsync())
                resultDtos.Add(dto);

            Assert.That(resultDtos, Is.Empty);
            _countryRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenCancellationRequested_ShouldStopStreamingAndPropagateCancellation()
        {
            var cts = new CancellationTokenSource();
            var domainCountries = CreateSampleCountryDomains();

            _countryRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerable(domainCountries, ct));

            var resultDtos = new List<CountryDto?>();

            // Act & Assert
            // Ожидаем, что OperationCanceledException будет выброшено, когда мы попытаемся прочитать из потока
            // после отмены.
            var exception = Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var dto in _useCase.GetAllStreamingAsync(cts.Token))
                {
                    resultDtos.Add(dto);
                    if (resultDtos.Count == 1)
                    {
                        cts.Cancel();
                    }
                }
            });

            Assert.That(resultDtos.Count, Is.EqualTo(1));
            Assert.That(resultDtos[0]?.IdCountry, Is.EqualTo(domainCountries[0].IdCountry));

            _countryRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(cts.Token), Times.Once);
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenRepositoryStreamThrowsException_ShouldPropagateException()
        {
            var domainCountriesToYieldFirst = CreateSampleCountryDomains();
            var errorMessage = "БД потеряла связь.";

            _countryRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerableThrowingException(domainCountriesToYieldFirst, errorMessage, ct));

            var resultDtos = new List<CountryDto?>();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await foreach (var dto in _useCase.GetAllStreamingAsync())
                {
                    resultDtos.Add(dto);
                }
            });

            Assert.That(ex.Message, Is.EqualTo(errorMessage));
            Assert.That(resultDtos.Count, Is.EqualTo(domainCountriesToYieldFirst.Count));
            _countryRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}