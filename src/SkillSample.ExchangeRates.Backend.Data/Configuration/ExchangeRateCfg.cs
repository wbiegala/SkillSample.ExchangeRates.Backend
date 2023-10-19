using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSample.ExchangeRates.Backend.Domain.Model;

namespace SkillSample.ExchangeRates.Backend.Data.Configuration
{
    internal class ExchangeRateCfg : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.HasKey(er => er.Id);

            builder.Ignore(er => er.Events);

            builder.Property(er => er.Table)
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(er => er.TableNumber)
                .IsRequired()
                .HasMaxLength(18);

            builder.Property(er => er.TradingDate)
                .IsRequired();

            builder.Property(er => er.EffectiveDate)
                .IsRequired();

            builder.HasMany(er => er.Rates)
                .WithOne()
                .HasForeignKey(dr => dr.ExchangeRateId);
        }
    }
}
