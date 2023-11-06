using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.UseCases;
using SkillSample.ExchangeRates.Backend.Infrastructure;
using SkillSample.ExchangeRates.Backend.NBP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure();

        services.AddNbpIntegration(cfg =>
        {
            cfg.UseCurrencyTable(Environment.GetEnvironmentVariable("NBP_ExchangeTable"));
            cfg.UseApiAddress(Environment.GetEnvironmentVariable("NBP_ApiAddress"));
        });

        services.AddDbContext<ExchangeRatesDbContext>((ctx, options) =>
        {
            var connStr = context.Configuration.GetConnectionString("DbConnectionString");
            options.UseSqlServer(connStr);
        });

        services.AddUseCases();
    }).Build();

host.Run();