using MyApp.Application;
using MyApp.Core;
using MyApp.Infrastructure;

namespace MyApp.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddApplicationDI(configuration).AddInfrastructureDI().AddCoreDI(configuration);

            return services;
        }
    }
}
