using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;
using EnigmaVault.AuthenticationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyInfrastructure.Tests
{
    public class CountyRepositoryTest : IDisposable
    {
        private readonly UsersDBContext _context;
        private readonly ICountryRepository _countryRepository;

        public CountyRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<UsersDBContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            _context = new UsersDBContext(options);
            _context.Database.EnsureCreated();

            _countryRepository = new CountryRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private static List<Country> CreateSampleCollectionCountries()
        {
            return new List<Country>
            {
                new() { CountryName = "Россия" },
                new() { CountryName = "США" },
                new() { CountryName = "Китай" },
                new() { CountryName = "Вьетнам" },
                new() { CountryName = "Япония" },
                new() { CountryName = "Австралия" },
            };
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        [Test]
        public async Task GetAllCountiesStreamingAsync_ShouldReturnCollectionCountries()
        {
            var addingCounties = CreateSampleCollectionCountries();

            await _context.Countries.AddRangeAsync(addingCounties);
            await _context.SaveChangesAsync();

            var gettingCountries = new List<CountryDomain?>();

            await foreach (var country in _countryRepository.GetAllStreamingAsync())
                gettingCountries.Add(country);

            for (var i = 0; i < gettingCountries.Count; i++)
                TestContext.Out.WriteLine($"Страна #{i + 1}: {gettingCountries[i].CountryName}");

            Assert.That(gettingCountries.Any(), Is.True);
            Assert.That(gettingCountries.Count, Is.EqualTo(addingCounties.Count));
            Assert.That(gettingCountries.All(c => c != null), Is.True);
            Assert.That(gettingCountries.Select(c => c!.CountryName), Is.EquivalentTo(addingCounties.Select(ac => ac.CountryName)));
        }

        [Test]
        public async Task GetAllStreamingAsync_ShouldBeCancellable()
        {
            var addingCountries = CreateSampleCollectionCountries(); 
            await _context.Countries.AddRangeAsync(addingCountries);
            await _context.SaveChangesAsync();

            var cts = new CancellationTokenSource();
            var processedCountries = new List<CountryDomain?>();
            int itemsToProcessBeforeCancel = 2; 

            var exception = Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var countryDomain in _countryRepository.GetAllStreamingAsync(cts.Token))
                {
                    processedCountries.Add(countryDomain);
                    TestContext.Out.WriteLine($"В процессе: {countryDomain?.CountryName}");
                    if (processedCountries.Count >= itemsToProcessBeforeCancel)
                    {
                        TestContext.Out.WriteLine($"Аннулирование токена после обработки  {processedCountries.Count} элементов...");
                        cts.Cancel();
                    }
                }
            });

            Assert.That(exception, Is.Not.Null, "Ожидалось исключение OperationCanceledException.");
            Assert.That(processedCountries.Count, Is.EqualTo(itemsToProcessBeforeCancel), $"Перед отменой необходимо было обработать только элементы {itemsToProcessBeforeCancel}.");

            TestContext.Out.WriteLine($"Тест завершен. Обработано {processedCountries.Count} предметов.");
            foreach (var country in processedCountries)
                TestContext.Out.WriteLine($"Элемент в процессе: {country?.CountryName}");

            var expectedProcessedNames = addingCountries.Take(itemsToProcessBeforeCancel).Select(c => c.CountryName);
            var actualProcessedNames = processedCountries.Select(c => c?.CountryName);
            Assert.That(actualProcessedNames, Is.EquivalentTo(expectedProcessedNames));
        }

        [Test]
        public async Task GetAllStreamingAsync_WhenCancelledBeforeFirstItem_ShouldThrowAndProcessZeroItems()
        {
            var addingCountries = CreateSampleCollectionCountries();
            await _context.Countries.AddRangeAsync(addingCountries);
            await _context.SaveChangesAsync();

            var cts = new CancellationTokenSource();
            var processedCountries = new List<CountryDomain?>();

            cts.Cancel();

            var exception = Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var countryDomain in _countryRepository.GetAllStreamingAsync(cts.Token))
                    processedCountries.Add(countryDomain);
            });

            Assert.That(exception, Is.Not.Null, "Ожидаем OperationCanceledException.");
            Assert.That(processedCountries.Count, Is.EqualTo(0), "Должно было быть обработано ноль элементов.");
        }
    } 
}
