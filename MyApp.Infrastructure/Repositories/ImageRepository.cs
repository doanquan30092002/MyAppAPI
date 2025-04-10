using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext context;

        public ImageRepository(
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            AppDbContext context
        )
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }

        public async Task<Image> Upload(Image image)
        {
            var localFilePath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "Images",
                $"{image.FileName}{image.FileExtension}"
            );
            //Upload Image to local path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);
            //http://localhost:1234/images/image.jpg
            // httpContextAccessor get http://localhost:1234/images/image.jpg
            var urlFilePath =
                $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;

            // add Image to Images Table
            await context.Images.AddAsync(image);
            await context.SaveChangesAsync();
            return image;
        }
    }
}
