using Microsoft.EntityFrameworkCore;
using SkillSample.ExchangeRates.Backend.Domain.Model;

namespace SkillSample.ExchangeRates.Backend.Data
{
    public class ExchangeRatesDbContext : DbContext
    {
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<DailyRate> DailyRates { get; set; }

        public ExchangeRatesDbContext() { }
        public ExchangeRatesDbContext(DbContextOptions<ExchangeRatesDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ExchangeRatesDbContext).Assembly);
        }
    }
}
