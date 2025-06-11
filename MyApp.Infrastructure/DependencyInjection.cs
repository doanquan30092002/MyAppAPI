using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApp.Application.Interfaces.SearchUserAttendance;
using MyApp.Core.Options;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.SearchUserAttendance;

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

            services.AddScoped<ISearchUserAttendanceRepository, SearchUserAttendanceRepository>();

            return services;
        }
    }
}
