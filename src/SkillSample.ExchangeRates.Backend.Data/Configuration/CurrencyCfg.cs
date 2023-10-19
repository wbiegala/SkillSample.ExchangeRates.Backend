using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSample.ExchangeRates.Backend.Domain.Model;

namespace SkillSample.ExchangeRates.Backend.Data.Configuration
{
    internal class CurrencyCfg : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired();

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasIndex(c => c.Code);
        }
    }
}
