using Microsoft.EntityFrameworkCore;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ExchangeRatesDbContext>((ctx, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext"));
});

builder.Services.AddUseCases();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
