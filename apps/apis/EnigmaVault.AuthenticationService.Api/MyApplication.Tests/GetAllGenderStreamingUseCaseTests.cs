using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Application.Implementations.UseCases;
using EnigmaVault.AuthenticationService.Application.Mappers;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using Moq;
using System.Runtime.CompilerServices;

namespace MyApplication.Tests
{
    public class GetAllGenderStreamingUseCaseTests
    {
        private Mock<IGenderRepository> _genderRepositoryMock;
        private GetAllGenderStreamingUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _genderRepositoryMock = new Mock<IGenderRepository>();
            _useCase = new GetAllGenderStreamingUseCase(_genderRepositoryMock.Object);
        }

        private static async IAsyncEnumerable<T> CreateAsyncEnumerable<T>(IEnumerable<T> items, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return item;
            }
        }

        private static async IAsyncEnumerable<GenderDomain> CreateAsyncEnumerableThrowingException(IEnumerable<GenderDomain> itemsToYieldFirst, string errorMessage, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in itemsToYieldFirst)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return item;
            }
            throw new InvalidOperationException(errorMessage);
        }

        private static List<GenderDomain> CreateSampleGenderDomains()
        {
            return new List<GenderDomain>
            {
                GenderDomain.Reconstitute(1, "Не учитывать"),
                GenderDomain.Reconstitute(2, "Муж"),
                GenderDomain.Reconstitute(3, "Жен"),
            };
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenRepositoryReturnsGenders_ShouldReturnMappedDtos()
        {
            var domainGenders = CreateSampleGenderDomains();

            _genderRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerable(domainGenders, ct));

            var expectedDtos = domainGenders.Select(d => d.ToDto()).ToList();
            var resultDtos = new List<GenderDto?>();

            await foreach (var dto in _useCase.GetAllStreamingAsync())
                resultDtos.Add(dto);

            Assert.That(resultDtos.Count, Is.EqualTo(expectedDtos.Count));
            for (int i = 0; i < resultDtos.Count; i++)
            {
                Assert.That(resultDtos[i]?.IdGender, Is.EqualTo(expectedDtos[i]?.IdGender));
                Assert.That(resultDtos[i]?.GenderName, Is.EqualTo(expectedDtos[i]?.GenderName));
            }

            _genderRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenRepositoryReturnsEmptyStream_ShouldReturnEmptyStream()
        {
            var emptyDomainGenders = new List<GenderDomain>();

            _genderRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerable(emptyDomainGenders, ct));

            var resultDtos = new List<GenderDto?>();

            await foreach (var dto in _useCase.GetAllStreamingAsync())
                resultDtos.Add(dto);

            Assert.That(resultDtos, Is.Empty);
            _genderRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenCancellationRequested_ShouldStopStreamingAndPropagateCancellation()
        {
            var cts = new CancellationTokenSource();
            var domainGenders = CreateSampleGenderDomains();

            _genderRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerable(domainGenders, ct));

            var resultDtos = new List<GenderDto?>();

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
            Assert.That(resultDtos[0]?.IdGender, Is.EqualTo(domainGenders[0].IdGender));

            _genderRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(cts.Token), Times.Once);
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenRepositoryStreamThrowsException_ShouldPropagateException()
        {
            var domainGendersToYieldFirst = CreateSampleGenderDomains();
            var errorMessage = "БД потеряла связь.";

            _genderRepositoryMock.Setup(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken ct) => CreateAsyncEnumerableThrowingException(domainGendersToYieldFirst, errorMessage, ct));

            var resultDtos = new List<GenderDto?>();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await foreach (var dto in _useCase.GetAllStreamingAsync())
                {
                    resultDtos.Add(dto);
                }
            });

            Assert.That(ex.Message, Is.EqualTo(errorMessage));
            Assert.That(resultDtos.Count, Is.EqualTo(domainGendersToYieldFirst.Count));
            _genderRepositoryMock.Verify(repo => repo.GetAllStreamingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}