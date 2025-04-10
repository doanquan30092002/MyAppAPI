using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.TestUploadFile.Commands
{
    public class ImageUploadHandler : IRequestHandler<ImageUploadRequest, ImageUploadResponse>
    {
        private readonly IImageRepository imageRepository;

        public ImageUploadHandler(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        public async Task<ImageUploadResponse> Handle(
            ImageUploadRequest request,
            CancellationToken cancellationToken
        )
        {
            var validationResult = ValidateFileUpload(request);
            if (!validationResult.Success)
            {
                return validationResult;
            }
            var imageDomainModel = new Image
            {
                File = request.File,
                FileExtension = Path.GetExtension(request.File.FileName),
                FileSizeInBytes = request.File.Length,
                FileName = request.File.FileName,
                FileDescription = request.FileDescription,
            };
            await imageRepository.Upload(imageDomainModel);
            return new ImageUploadResponse { Success = true, Image = imageDomainModel };
        }

        private ImageUploadResponse ValidateFileUpload(ImageUploadRequest imageUploadRequest)
        {
            var allowExtension = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowExtension.Contains(Path.GetExtension(imageUploadRequest.File.FileName)))
            {
                return new ImageUploadResponse
                {
                    Success = false,
                    ErrorMessage = "Chỉ cho phép các định dạng: .jpg, .jpeg, .png",
                };
            }
            if (imageUploadRequest.File.Length > 10485760)
            {
                return new ImageUploadResponse
                {
                    Success = false,
                    ErrorMessage = "filesize > 10MB",
                };
            }
            return new ImageUploadResponse { Success = true };
        }
    }
}
