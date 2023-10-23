using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.UseCases;
using SkillSample.ExchangeRates.Backend.Infrastructure;
using SkillSample.ExchangeRates.Backend.NBP;
using Microsoft.EntityFrameworkCore;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddInfrastructure();

        services.AddNbpIntegration(cfg =>
        {
            cfg.UseCurrencyTable(Environment.GetEnvironmentVariable("NBP_ExchangeTable"));
            cfg.UseApiAddress(Environment.GetEnvironmentVariable("NBP_ApiAddress"));
        });

        services.AddDbContext<ExchangeRatesDbContext>((ctx, options) =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("DbConnectionString"));
        });

        services.AddUseCases();
    }).Build();

host.Run();