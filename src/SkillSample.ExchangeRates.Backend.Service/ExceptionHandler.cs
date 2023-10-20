using SkillSample.ExchangeRates.Backend.Contract;
using System.Text.Json;

namespace SkillSample.ExchangeRates.Backend.Service
{
    public class ExceptionHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                var dto = MapException(ex);

                context.Response.StatusCode = 500;
                context.Response.Headers.Add("Content-Type", "application/json");
                await context.Response.WriteAsync(JsonSerializer.Serialize(dto, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
            }
        }

        private static ExceptionResponseDto MapException(Exception ex)
        {
            return new ExceptionResponseDto
            {
                Type = ex.GetType().FullName ?? ex.GetType().Name,
                Message = ex.Message
            };
        }
    }
}
