using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SkillSample.ExchangeRates.Backend.Data;

namespace SkillSample.ExchangeRates.Backend.UseCases.UnitTests
{
    /// <summary>
    /// Provides "mocked" DbContext provided by InMemory implementation
    /// </summary>
    internal static class DatabaseMockHelper
    {
        public static ExchangeRatesDbContext GetInMemoryDbContext()
        {
            var context = new ExchangeRatesDbContext(_optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        public static IServiceCollection RegisterInMemoryDbContext(this IServiceCollection services)
        {
            services.AddDbContext<ExchangeRatesDbContext>(opt => opt = _optionsBuilder);

            return services;
        }

        private static DbContextOptionsBuilder<ExchangeRatesDbContext> _optionsBuilder => new DbContextOptionsBuilder<ExchangeRatesDbContext>()
                .UseInMemoryDatabase(databaseName: "ExchangeRatesApp")
                .ConfigureWarnings(warns => warns.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    }
}