using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;
using MyApp.Application.Interfaces.AuctionDocumentRegisted;
using MyApp.Application.Interfaces.Blog;
using MyApp.Application.Interfaces.ChangeStatusAuctionRound;
using MyApp.Application.Interfaces.DetailAuctionAsset;
using MyApp.Application.Interfaces.DetailAuctionDocument;
using MyApp.Application.Interfaces.EmployeeManager;
using MyApp.Application.Interfaces.GenarateNumbericalOrder;
using MyApp.Application.Interfaces.GetAuctioneers;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionCategoriesRepository;
using MyApp.Application.Interfaces.IAuctionDocuments;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.ICreateAuctionRoundRepository;
using MyApp.Application.Interfaces.IExcelRepository;
using MyApp.Application.Interfaces.IFindHighestPriceAndFlag;
using MyApp.Application.Interfaces.IForgetPasswordRepository;
using MyApp.Application.Interfaces.IGetAuctionByIdRepository;
using MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository;
using MyApp.Application.Interfaces.IGetBusinessOverviewRepository;
using MyApp.Application.Interfaces.IGetListAssetInfostatisticsRepository;
using MyApp.Application.Interfaces.IGetListAuctionRoundRepository;
using MyApp.Application.Interfaces.IGetListBidders;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;
using MyApp.Application.Interfaces.IGetListEnteredPricesRepository;
using MyApp.Application.Interfaces.IGetListRepository;
using MyApp.Application.Interfaces.IGetStatisticOverviewRepository;
using MyApp.Application.Interfaces.IGetUserInfoRepository;
using MyApp.Application.Interfaces.IListUserWinnerRepository;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Application.Interfaces.INotificationsRepository;
using MyApp.Application.Interfaces.IPaymentDeposit;
using MyApp.Application.Interfaces.IRefundRepository;
using MyApp.Application.Interfaces.ISaveListPricesRepository;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Application.Interfaces.IUpdateDepositStatus;
using MyApp.Application.Interfaces.IUpdateWinnerFlagRepository;
using MyApp.Application.Interfaces.ListAuctionAsset;
using MyApp.Application.Interfaces.ListAuctionRegisted;
using MyApp.Application.Interfaces.ListCustomer;
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
using MyApp.Infrastructure.Repositories.AuctionDocumentsRepository;
using MyApp.Infrastructure.Repositories.AuctionRepository;
using MyApp.Infrastructure.Repositories.Blog;
using MyApp.Infrastructure.Repositories.ChangeStatusAuctionRound;
using MyApp.Infrastructure.Repositories.CreateAuctionRoundRepository;
using MyApp.Infrastructure.Repositories.DetailAuctionAsset;
using MyApp.Infrastructure.Repositories.DetailAuctionDocument;
using MyApp.Infrastructure.Repositories.EmployeeManager;
using MyApp.Infrastructure.Repositories.ExcelRepository;
using MyApp.Infrastructure.Repositories.FindHighestPriceAndFlagRepository;
using MyApp.Infrastructure.Repositories.ForgetPassRepository;
using MyApp.Infrastructure.Repositories.GenarateNumbericalOrder;
using MyApp.Infrastructure.Repositories.GetAuctionByIdRepository;
using MyApp.Infrastructure.Repositories.GetAuctioneers;
using MyApp.Infrastructure.Repositories.GetAuctionRoundStatisticsRepository;
using MyApp.Infrastructure.Repositories.GetBusinessOverviewRepository;
using MyApp.Infrastructure.Repositories.GetListAssetInfoStatisticsRepository;
using MyApp.Infrastructure.Repositories.GetListAuctionDocumentsRepository;
using MyApp.Infrastructure.Repositories.GetListAuctionRepository;
using MyApp.Infrastructure.Repositories.GetListAuctionRoundRepository;
using MyApp.Infrastructure.Repositories.GetListBiddersRepository;
using MyApp.Infrastructure.Repositories.GetListEnteredPricesRepository;
using MyApp.Infrastructure.Repositories.GetListUserWinnerRepository;
using MyApp.Infrastructure.Repositories.GetStatisticOverviewRepository;
using MyApp.Infrastructure.Repositories.GetUserInfoRepository;
using MyApp.Infrastructure.Repositories.ListAuctionAsset;
using MyApp.Infrastructure.Repositories.ListAuctionRegisted;
using MyApp.Infrastructure.Repositories.ListCustomer;
using MyApp.Infrastructure.Repositories.LoginUserRepository;
using MyApp.Infrastructure.Repositories.NotificationsRepository;
using MyApp.Infrastructure.Repositories.PaymentDepositRepository;
using MyApp.Infrastructure.Repositories.ReceiveAuctionRegistrationForm;
using MyApp.Infrastructure.Repositories.RefundRepository;
using MyApp.Infrastructure.Repositories.RegisterAuctionDocumentRepository;
using MyApp.Infrastructure.Repositories.SaveListPricesRepository;
using MyApp.Infrastructure.Repositories.SearchUserAttendance;
using MyApp.Infrastructure.Repositories.SignUpRepository;
using MyApp.Infrastructure.Repositories.SupportRegisterDocuments;
using MyApp.Infrastructure.Repositories.UpdateAccountRepository;
using MyApp.Infrastructure.Repositories.UpdateDepositStatusRepository;
using MyApp.Infrastructure.Repositories.UpdateExpiredProfile;
using MyApp.Infrastructure.Repositories.UpdateWinnerFlagRepository;
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
            services.AddScoped<ICreateAuctionRoundRepository, CreateAuctionRoundRepository>();
            services.AddScoped<IGetListAuctionRoundRepository, GetListAuctionRoundRepository>();
            services.AddScoped<ISaveListPricesRepository, SaveListPricesRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            services.AddScoped<IFindHighestPriceAndFlag, FindHighestPriceAndFlagRepository>();
            services.AddScoped<IGetListEnteredPricesRepository, GetListEnteredPricesRepository>();
            services.AddScoped<IUpdateWinnerFlagRepository, UpdateWinnerFlagRepository>();
            services.AddScoped<
                IChangeStatusAuctionRoundRepository,
                ChangeStatusAuctionRoundRepository
            >();
            services.AddScoped<IListUserWinnerRepository, GetListUserWinnerRepository>();

            services.AddScoped<IAuctionDocuments, AuctionDocumentsRepository>();
            services.AddScoped<IEmployeeManagerRepository, EmployeeManagerRepository>();
            services.AddScoped<
                IGetAuctionRoundStatisticsRepository,
                GetAuctionRoundStatisticsRepository
            >();
            services.AddScoped<IListCustomerRepository, ListCustomerRepository>();
            services.AddScoped<IListAuctionAssetRepository, ListAuctionAssetRepository>();
            services.AddScoped<IDetailAuctionAssetRepository, DetailAuctionAssetRepository>();
            services.AddScoped<
                IGetListAssetInfostatisticsRepository,
                GetListAssetInfoStatisticsRepository
            >();
            services.AddScoped<IGetBusinessOverviewRepository, GetBusinessOverviewRepository>();
            services.AddScoped<IGetStatisticOverviewRepository, GetStatisticOverviewRepository>();
            services.AddScoped<IGetListBiddersRepository, GetListBiddersRepository>();
            return services;
        }
    }
}
