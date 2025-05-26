using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;
using EnigmaVault.AuthenticationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyInfrastructure.Tests
{
    internal class GenderRepositoryTests : IDisposable
    {
        private readonly UsersDBContext _context;
        private readonly IGenderRepository _genderRepository;

        public GenderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UsersDBContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            _context = new UsersDBContext(options);
            _context.Database.EnsureCreated();

            _genderRepository = new GenderRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static List<Gender> CreateSampleCollectionGenders()
        {
            return new List<Gender>
            {
                new() { GenderName = "Не указан" },
                new() { GenderName = "Муж" },
                new() { GenderName = "Жен" },
            };
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        [Test]
        public async Task GetAllGendersStreamingAsync_ShouldReturnCollectionGenders()
        {
            var addingGenders = CreateSampleCollectionGenders();

            await _context.Genders.AddRangeAsync(addingGenders);
            await _context.SaveChangesAsync();

            var gettingGenders = new List<GenderDomain?>();

            await foreach (var country in _genderRepository.GetAllStreamingAsync())
                gettingGenders.Add(country);

            for (var i = 0; i < gettingGenders.Count; i++)
                TestContext.Out.WriteLine($"Гендер #{i + 1}: {gettingGenders[i].GenderName}");

            Assert.That(gettingGenders.Any(), Is.True);
            Assert.That(gettingGenders.Count, Is.EqualTo(addingGenders.Count));
            Assert.That(gettingGenders.All(g => g != null), Is.True);
            Assert.That(gettingGenders.Select(g => g!.GenderName), Is.EquivalentTo(addingGenders.Select(ag => ag.GenderName)));
        }

        [Test]
        public async Task GetAllStreamingAsync_ShouldBeCancellable()
        {
            var addingGenders = CreateSampleCollectionGenders();

            await _context.Genders.AddRangeAsync(addingGenders);
            await _context.SaveChangesAsync();

            var cts = new CancellationTokenSource();
            var processedGenders = new List<GenderDomain?>();
            int itemsToProcessBeforeCancel = 2;

            var exception = Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var genderDomain in _genderRepository.GetAllStreamingAsync(cts.Token))
                {
                    processedGenders.Add(genderDomain);
                    TestContext.Out.WriteLine($"В процессе: {genderDomain?.GenderName}");
                    if (processedGenders.Count >= itemsToProcessBeforeCancel)
                    {
                        TestContext.Out.WriteLine($"Аннулирование токена после обработки  {processedGenders.Count} элементов...");
                        cts.Cancel();
                    }
                }
            });

            Assert.That(exception, Is.Not.Null, "Ожидалось исключение OperationCanceledException.");
            Assert.That(processedGenders.Count, Is.EqualTo(itemsToProcessBeforeCancel), $"Перед отменой необходимо было обработать только элементы {itemsToProcessBeforeCancel}.");

            TestContext.Out.WriteLine($"Тест завершен. Обработано {processedGenders.Count} предметов.");
            foreach (var country in processedGenders)
                TestContext.Out.WriteLine($"Элемент в процессе: {country?.GenderName}");

            var expectedProcessedNames = addingGenders.Take(itemsToProcessBeforeCancel).Select(g => g.GenderName);
            var actualProcessedNames = processedGenders.Select(g => g?.GenderName);
            Assert.That(actualProcessedNames, Is.EquivalentTo(expectedProcessedNames));
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenCancelledBeforeFirstItem_ShouldThrowAndProcessZeroItems()
        {
            var addingGenders = CreateSampleCollectionGenders();

            await _context.Genders.AddRangeAsync(addingGenders);
            await _context.SaveChangesAsync();

            var cts = new CancellationTokenSource();
            var processedCountries = new List<GenderDomain?>();

            cts.Cancel();

            var exception = Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var countryDomain in _genderRepository.GetAllStreamingAsync(cts.Token))
                    processedCountries.Add(countryDomain);
            });

            Assert.That(exception, Is.Not.Null, "Ожидаем OperationCanceledException.");
            Assert.That(processedCountries.Count, Is.EqualTo(0), "Должно было быть обработано ноль элементов.");
        }
    }
}