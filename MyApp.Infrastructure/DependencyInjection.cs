using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IForgetPasswordRepository;
using MyApp.Application.Interfaces.IGetAuctionByIdRepository;
using MyApp.Application.Interfaces.IGetListRepository;
using MyApp.Application.Interfaces.IGetUserInfoRepository;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Application.Interfaces.INofiticationsRepository;
using MyApp.Application.Interfaces.IPaymentDeposit;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Application.Interfaces.RegisterAuctionDocument;
using MyApp.Application.Interfaces.SearchUserAttendance;
using MyApp.Application.Interfaces.UpdateAccountRepository;
using MyApp.Application.Interfaces.UpdateExpiredProfile;
using MyApp.Core.Entities;
using MyApp.Core.Options;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.ImplementUnitOfWork;
using MyApp.Infrastructure.Repositories.AuctionAssetsImplement;
using MyApp.Infrastructure.Repositories.AuctionCategoriesRepository;
using MyApp.Infrastructure.Repositories.AuctionRepository;
using MyApp.Infrastructure.Repositories.ExcelRepository;
using MyApp.Infrastructure.Repositories.ForgetPassRepository;
using MyApp.Infrastructure.Repositories.GetAuctionByIdRepository;
using MyApp.Infrastructure.Repositories.GetListAuctionRepository;
using MyApp.Infrastructure.Repositories.GetUserInfoRepository;
using MyApp.Infrastructure.Repositories.LoginUserRepository;
using MyApp.Infrastructure.Repositories.NotificationsRepository;
using MyApp.Infrastructure.Repositories.PaymentDepositRepository;
using MyApp.Infrastructure.Repositories.RegisterAuctionDocumentRepository;
using MyApp.Infrastructure.Repositories.SearchUserAttendance;
using MyApp.Infrastructure.Repositories.SignUpRepository;
using MyApp.Infrastructure.Repositories.SupportRegisterDocuments;
using MyApp.Infrastructure.Repositories.UpdateAccountRepository;
using MyApp.Infrastructure.Repositories.UpdateExpiredProfile;

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
            services.AddScoped<IUpdateAccountRepository, UpdateAccountRepository>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IExcelRepository, ExcelRepository>();

            services.AddScoped<IAuctionCategoriesRepository, AuctionCategoriesRepository>();

            services.AddScoped<ISearchUserAttendanceRepository, SearchUserAttendanceRepository>();
            services.AddScoped<ISignUpRepository, SignUpRepository>();

            services.AddScoped<IAuctionAssetsRepository, AuctionAssetsImplement>();

            services.AddScoped<IForgetPasswordRepository, ForgetPassRepository>();
            services.AddScoped<IGetAuctionByIdRepository, GetAuctionByIdRepository>();
            services.AddScoped<IGetListRepository, GetListAuctionRepository>();
            services.AddScoped<IGetRoleRepository, GetRoleRepository>();
            services.AddScoped<IGetUserInfoRepository, GetUserInfoRepository>();

            services.AddTransient<ISupportRegisterDocuments, SupportRegisterDocumentsRepository>();

            services.AddScoped<IPaymentDeposit, PaymentDepositRepository>();
            services.AddScoped<IUpdateExpiredProfileRepository, UpdateExpiredProfileRepository>();
            services.AddScoped<
                IRegisterAuctionDocumentRepository,
                RegisterAuctionDocumentRepository
            >();

            services.AddScoped<INotificationRepository, NotificationsImplement>();

            return services;
        }
    }
}
