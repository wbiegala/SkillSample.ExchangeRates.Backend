using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSample.ExchangeRates.Backend.Domain.Model;

namespace SkillSample.ExchangeRates.Backend.Data.Configuration
{
    internal class DailyRateCfg : IEntityTypeConfiguration<DailyRate>
    {
        public void Configure(EntityTypeBuilder<DailyRate> builder)
        {
            builder.HasKey(dr => dr.Id);

            builder.HasOne(dr => dr.Currency)
                .WithMany()
                .HasForeignKey(dr => dr.CurrencyId);

            builder.Property(dr => dr.Mid)
                .IsRequired();
        }
    }
}
