using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApp.Core.Options;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(
                (provider, options) =>
                {
                    options.UseSqlServer(
                        provider
                            .GetRequiredService<IOptionsSnapshot<ConnectionStringOptions>>()
                            .Value.DefaultConnection
                    );
                }
            );

            //services.AddScoped<ITokenRepository, TokenRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IImageRepository, ImageRepository>();

            return services;
        }
    }
}
