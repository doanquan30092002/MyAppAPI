using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Services.ExportWord.ExportAuctionDocuments;
using MyApp.Application.Interfaces.ISupportRegisterDocuments;

namespace MyApp.Application.CQRS.AuctionDocuments.SupportRegisterDocuments.Command
{
    public class ExportWordAuctionDocumentHandler
        : IRequestHandler<ExportWordAuctionDocumentCommand, byte[]>
    {
        private readonly ISupportRegisterDocuments _supportRegisterDocuments;
        private readonly IExportAuctionDocuments _exportAuctionDocuments;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExportWordAuctionDocumentHandler(
            ISupportRegisterDocuments supportRegisterDocuments,
            IExportAuctionDocuments exportAuctionDocuments,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _supportRegisterDocuments = supportRegisterDocuments;
            _exportAuctionDocuments = exportAuctionDocuments;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<byte[]> Handle(
            ExportWordAuctionDocumentCommand request,
            CancellationToken cancellationToken
        )
        {
            var document = await _supportRegisterDocuments.GetAuctionDocumentByIdAsync(
                request.AuctionDocumentId
            );

            if (document == null)
                throw new KeyNotFoundException("Không tìm thấy hồ sơ đấu giá phù hợp.");

            IFormFile templateFile = request.TemplateFile;

            if (templateFile == null)
            {
                var defaultTemplatePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "wwwroot",
                    "Templates",
                    "phieu-dang-ky.docx"
                );
                if (!File.Exists(defaultTemplatePath))
                    throw new FileNotFoundException(
                        "Không tìm thấy file template Word mặc định.",
                        defaultTemplatePath
                    );

                await using var fs = new FileStream(
                    defaultTemplatePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read
                );

                templateFile = new FormFile(
                    fs,
                    0,
                    fs.Length,
                    "phieu-dang-ky",
                    "phieu-dang-ky.docx"
                );
            }

            var exportResult = await _exportAuctionDocuments.ExportAuctionDocumentToWordFile(
                document,
                templateFile
            );

            exportResult.Stream.Position = 0;
            return exportResult.Stream.ToArray();
        }
    }
}
