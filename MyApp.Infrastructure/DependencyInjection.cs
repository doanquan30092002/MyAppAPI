using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Core.Options;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.LoginUserRepository;
using MyApp.Infrastructure.Repositories.SignUpRepository;

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

            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ILoginUserRepository, LoginUserRepository>();
            //services.AddScoped<IImageRepository, ImageRepository>();

            services.AddScoped<ISignUpRepository, SignUpRepository>();

            return services;
        }
    }
}
