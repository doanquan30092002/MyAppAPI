using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.Blog.GetBlogDetail;
using MyApp.Application.CQRS.Blog.GetListBlog;
using MyApp.Application.Interfaces.Blog;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.Blog
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext context;

        public BlogRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> ChangeStatusBlogAsync(
            Guid blogId,
            int status,
            string? note,
            string? userIdStr
        )
        {
            var blogExist = await context.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId);
            if (blogExist == null)
            {
                return false;
            }
            blogExist.Status = status;
            blogExist.Note = note ?? blogExist.Note;
            blogExist.UpdatedAt = DateTime.Now;
            blogExist.UpdatedBy = string.IsNullOrEmpty(userIdStr)
                ? Guid.Empty
                : Guid.Parse(userIdStr);
            context.Blogs.Update(blogExist);
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateBlogAsync(
            string title,
            string content,
            string thumbnailUrl,
            string? userIdStr
        )
        {
            try
            {
                var blog = new Blogs
                {
                    BlogId = Guid.NewGuid(),
                    Title = title,
                    Content = content,
                    ThumbnailUrl = thumbnailUrl,
                    CreatedAt = DateTime.Now,
                    CreatedBy = Guid.Parse(userIdStr),
                    Status = 0,
                };
                await context.Blogs.AddAsync(blog);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBlogAsync(Guid blogId)
        {
            var blog = await context.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId);
            if (blog == null)
            {
                return false;
            }
            try
            {
                context.Blogs.Remove(blog);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<GetBlogDetailResponse?> GetBlogDetailAsync(Guid blogId)
        {
            var query = await context
                .Blogs.Where(b => b.BlogId == blogId)
                .Select(b => new GetBlogDetailResponse
                {
                    BlogId = b.BlogId,
                    Title = b.Title,
                    Content = b.Content,
                    ThumbnailUrl = b.ThumbnailUrl,
                    CreatedAt = b.CreatedAt,
                    CreatedBy = b.CreatedBy,
                    UpdatedAt = b.UpdatedAt ?? default,
                    UpdatedBy = b.UpdatedBy ?? Guid.Empty,
                    Status = b.Status,
                    Note = b.Note,
                })
                .FirstOrDefaultAsync();
            return query;
        }

        public Task<List<BlogsResponse>> GetListBlogAsync(
            int pageNumber,
            int pageSize,
            string? searchTitle,
            int? status = null,
            Guid? userId = null
        )
        {
            var query = context.Blogs.AsQueryable();
            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(b => b.Title.Contains(searchTitle));
            }
            if (status != null)
            {
                if (status != 4)
                {
                    query = query.Where(b => b.Status == status);
                }
                else
                {
                    query = query.Where(b => b.Status == 1 || b.Status == 2 || b.Status == 3);
                }
            }
            if (userId.HasValue)
            {
                query = query.Where(b => b.CreatedBy == userId.Value);
            }
            return query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BlogsResponse
                {
                    BlogId = b.BlogId,
                    Title = b.Title,
                    Content = b.Content,
                    ThumbnailUrl = b.ThumbnailUrl,
                    CreatedAt = b.CreatedAt,
                    CreatedBy = b.CreatedBy,
                    UpdatedAt = b.UpdatedAt ?? default,
                    UpdatedBy = b.UpdatedBy ?? Guid.Empty,
                    Status = b.Status,
                    Note = b.Note,
                })
                .ToListAsync();
        }

        public Task<int> GetTotalCountAsync(
            string? searchTitle,
            int? status = null,
            Guid? userId = null
        )
        {
            var query = context.Blogs.AsQueryable();
            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(b => b.Title.Contains(searchTitle));
            }
            if (status != null)
            {
                if (status != 4)
                {
                    query = query.Where(b => b.Status == status);
                }
                else
                {
                    query = query.Where(b => b.Status == 2 || b.Status == 3);
                }
            }
            if (userId.HasValue)
            {
                query = query.Where(b => b.CreatedBy == userId.Value);
            }
            return query.CountAsync();
        }

        public async Task<bool> UpdateBlogAsync(
            Guid blogId,
            string? title,
            string? content,
            string? thumbnailUrl,
            string? userIdStr
        )
        {
            var blog = await context.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId);
            if (blog == null)
            {
                return false;
            }
            try
            {
                blog.Title = title ?? blog.Title;
                blog.Content = content ?? blog.Content;
                blog.ThumbnailUrl = thumbnailUrl ?? blog.ThumbnailUrl;
                blog.UpdatedAt = DateTime.Now;
                blog.UpdatedBy = string.IsNullOrEmpty(userIdStr)
                    ? Guid.Empty
                    : Guid.Parse(userIdStr);
                context.Blogs.Update(blog);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
