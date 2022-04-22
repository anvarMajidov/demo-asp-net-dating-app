using api.Data;
using api.Interfaces;
using api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionStr = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionStr));
            return services;
        }
    }
}
