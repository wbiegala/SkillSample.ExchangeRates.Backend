using Microsoft.EntityFrameworkCore;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.UseCases;
using SkillSample.ExchangeRates.Backend.Infrastructure;
using SkillSample.ExchangeRates.Backend.NBP;
using SkillSample.ExchangeRates.Backend.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ExchangeRatesDbContext>((ctx, options) =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("DbConnectionString"));
});

builder.Services.AddNbpIntegration(cfg =>
{
    cfg.UseCurrencyTable("B");
});
builder.Services.AddInfrastructure();
builder.Services.AddUseCases();

builder.Services.AddScoped<ExceptionHandler>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetService<ExchangeRatesDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ExceptionHandler>();

app.MapControllers();

app.Run();
