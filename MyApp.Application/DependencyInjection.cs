using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Common.Services.JwtHelper;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.CQRS.ForgotPassword.Service;
using MyApp.Application.Interfaces.IJwtHelper;
using MyApp.Application.Interfaces.IUnitOfWork;

namespace MyApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                cfg.NotificationPublisher = new TaskWhenAllPublisher();
            });

            services.AddScoped<IOTPService, SmsOTPService>();
            services.AddSingleton<HttpClient>();

            services.Configure<S3Settings>(configuration.GetSection("CloudFly"));
            services.AddSingleton(sp =>
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<S3Settings>>().Value
            );

            services.AddSingleton<IAmazonS3>(sp =>
            {
                var settings = sp.GetRequiredService<S3Settings>();
                var awsConfig = new AmazonS3Config
                {
                    ServiceURL = settings.Endpoint,
                    ForcePathStyle = true,
                    SignatureVersion = "2",
                };
                return new AmazonS3Client(settings.AccessKey, settings.SecretKey, awsConfig);
            });

            services.AddScoped<IUploadFile, UploadFile>();

            services.AddScoped<IJwtHelper, JwtHelper>();

            return services;
        }
    }
}
