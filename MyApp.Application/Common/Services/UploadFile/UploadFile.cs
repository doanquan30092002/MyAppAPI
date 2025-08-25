using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.Common.Services.UploadFile
{
    public class UploadFile : IUploadFile
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _settings;

        public UploadFile(IAmazonS3 s3Client, S3Settings settings)
        {
            _s3Client = s3Client;
            _settings = settings;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            using var stream = file.OpenReadStream();
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = _settings.BucketName,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead,
                PartSize = 5 * 1024 * 1024,
                StorageClass = S3StorageClass.Standard,
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            var fileUrl = $"{_settings.Endpoint}/{_settings.BucketName}/{fileName}";
            return fileUrl;
        }
    }
}
