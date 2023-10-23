using Microsoft.Extensions.DependencyInjection;
using SkillSample.ExchangeRates.Backend.NBP.ExchangeRates;

namespace SkillSample.ExchangeRates.Backend.NBP
{
    public static class NbpIntegrationInstaller
    {
        public static IServiceCollection AddNbpIntegration(this IServiceCollection services, Action<NbpIntegrationConfiguration> configure)
        {
            var configuration = new NbpIntegrationConfiguration();
            configure(configuration);
            services.AddSingleton(configuration);

            services.AddHttpClient(NbpIntegrationConfiguration.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(configuration.ApiAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddScoped<IExchangeRatesProvider, ExchangeRatesProvider>();

            return services;
        }
    }
}