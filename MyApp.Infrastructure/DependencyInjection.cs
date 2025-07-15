using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;
using MyApp.Application.Interfaces.AuctionDocumentRegisted;
using MyApp.Application.Interfaces.DetailAuctionDocument;
using MyApp.Application.Interfaces.GenarateNumbericalOrder;
using MyApp.Application.Interfaces.GetAuctioneers;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IForgetPasswordRepository;
using MyApp.Application.Interfaces.IGetAuctionByIdRepository;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;
using MyApp.Application.Interfaces.IGetListRepository;
using MyApp.Application.Interfaces.IGetUserInfoRepository;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Application.Interfaces.INotificationsRepository;
using MyApp.Application.Interfaces.IPaymentDeposit;
using MyApp.Application.Interfaces.IRefundRepository;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Application.Interfaces.IUpdateDepositStatus;
using MyApp.Application.Interfaces.ListAuctionRegisted;
using MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;
using MyApp.Application.Interfaces.SearchUserAttendance;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Application.Interfaces.UpdateAccount.Service;
using MyApp.Application.Interfaces.UpdateExpiredProfile;
using MyApp.Application.Interfaces.UserRegisteredAuction;
using MyApp.Core.Options;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.ImplementUnitOfWork;
using MyApp.Infrastructure.Repositories.AssginAuctioneerAndPublicAuction;
using MyApp.Infrastructure.Repositories.AuctionAssetsImplement;
using MyApp.Infrastructure.Repositories.AuctionCategoriesRepository;
using MyApp.Infrastructure.Repositories.AuctionDocumentRegisted;
using MyApp.Infrastructure.Repositories.AuctionRepository;
using MyApp.Infrastructure.Repositories.DetailAuctionDocument;
using MyApp.Infrastructure.Repositories.ExcelRepository;
using MyApp.Infrastructure.Repositories.ForgetPassRepository;
using MyApp.Infrastructure.Repositories.GenarateNumbericalOrder;
using MyApp.Infrastructure.Repositories.GetAuctionByIdRepository;
using MyApp.Infrastructure.Repositories.GetAuctioneers;
using MyApp.Infrastructure.Repositories.GetListAuctionDocumentsRepository;
using MyApp.Infrastructure.Repositories.GetListAuctionRepository;
using MyApp.Infrastructure.Repositories.GetUserInfoRepository;
using MyApp.Infrastructure.Repositories.ListAuctionRegisted;
using MyApp.Infrastructure.Repositories.LoginUserRepository;
using MyApp.Infrastructure.Repositories.NotificationsRepository;
using MyApp.Infrastructure.Repositories.PaymentDepositRepository;
using MyApp.Infrastructure.Repositories.ReceiveAuctionRegistrationForm;
using MyApp.Infrastructure.Repositories.RefundRepository;
using MyApp.Infrastructure.Repositories.RegisterAuctionDocumentRepository;
using MyApp.Infrastructure.Repositories.SearchUserAttendance;
using MyApp.Infrastructure.Repositories.SignUpRepository;
using MyApp.Infrastructure.Repositories.SupportRegisterDocuments;
using MyApp.Infrastructure.Repositories.UpdateAccountRepository;
using MyApp.Infrastructure.Repositories.UpdateDepositStatusRepository;
using MyApp.Infrastructure.Repositories.UpdateExpiredProfile;
using MyApp.Infrastructure.Repositories.UserRegisteredAuction;
using MyApp.Infrastructure.Services.UpdateAccount;

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
            services.AddScoped<IOTPService_1, EmailOTPService_1>();
            services.AddScoped<
                IAssginAuctioneerAndPublicAuctionRepository,
                AssginAuctioneerAndPublicAuctionRepository
            >();
            services.AddScoped<IGetAuctioneersRepository, GetAuctioneersRepository>();
            services.AddScoped<IGetListDocumentsRepository, GetListAuctionDocumentsRepository>();
            services.AddScoped<IUpdateDepositStatus, UpdateDepositStatusRepository>();

            services.AddScoped<INotificationsRepository, NotificationsImplement>();

            services.AddScoped<IRefundRepository, RefundRepository>();
            services.AddScoped<IDetailAuctionDocumentRepository, DetailAuctionDocumentRepository>();
            services.AddScoped<
                IReceiveAuctionRegistrationFormRepository,
                ReceiveAuctionRegistrationFormRepository
            >();
            services.AddScoped<
                IGenarateNumbericalOrderRepository,
                GenarateNumbericalOrderRepository
            >();
            services.AddScoped<IAuctionRegistedRepository, AuctionRegistedRepository>();
            services.AddScoped<
                IAuctionDocumentRegistedRepository,
                AuctionDocumentRegistedRepository
            >();
            services.AddScoped<IUserRegisteredAuctionRepository, UserRegisteredAuctionRepository>();

            return services;
        }
    }
}
