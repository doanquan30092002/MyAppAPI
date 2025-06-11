using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Application.Interfaces.SearchUserAttendance;
using MyApp.Core.Options;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.ImplementUnitOfWork;
using MyApp.Infrastructure.Repositories.AuctionCategoriesRepository;
using MyApp.Infrastructure.Repositories.AuctionRepository;
using MyApp.Infrastructure.Repositories.ExcelRepository;
using MyApp.Infrastructure.Repositories.LoginUserRepository;
using MyApp.Infrastructure.Repositories.SearchUserAttendance;
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ILoginUserRepository, LoginUserRepository>();
            //services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IExcelRepository, ExcelRepository>();

            services.AddScoped<IAuctionCategoriesRepository, AuctionCategoriesRepository>();

            services.AddScoped<ISearchUserAttendanceRepository, SearchUserAttendanceRepository>();
            services.AddScoped<ISignUpRepository, SignUpRepository>();

            return services;
        }
    }
}
