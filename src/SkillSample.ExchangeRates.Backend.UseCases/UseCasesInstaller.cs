using Microsoft.Extensions.DependencyInjection;

namespace SkillSample.ExchangeRates.Backend.UseCases
{
    public static class UseCasesInstaller
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(UseCasesInstaller).Assembly);
            });

            return services;
        }
    }
}
