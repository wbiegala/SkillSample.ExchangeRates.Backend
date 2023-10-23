using Microsoft.Extensions.DependencyInjection;
using SkillSample.ExchangeRates.Backend.Infrastructure.Time;

namespace SkillSample.ExchangeRates.Backend.Infrastructure
{
    public static class InfrastructureInstaller
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ITimeService, TimeService>();

            return services;
        }
    }
}
