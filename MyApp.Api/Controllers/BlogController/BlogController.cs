using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.Blog.ChangeStatusBlog;
using MyApp.Application.CQRS.Blog.CreateBlog;
using MyApp.Application.CQRS.Blog.GetBlogDetail;
using MyApp.Application.CQRS.Blog.GetListBlog;
using MyApp.Application.CQRS.Blog.UpdateBlog;

namespace MyApp.Api.Controllers.BlogController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Create-Blog")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> CreateBlog([FromForm] CreateBlogRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("Get-List-Blog")]
        public async Task<ActionResult> GetListBlog([FromQuery] GetListBlogRequest request)
        {
            var response = await _mediator.Send(request);
            if (response.Blogs.Count == 0 || !response.Blogs.Any())
            {
                return Ok(
                    new ApiResponse<GetListBlogResponse>
                    {
                        Code = 404,
                        Message = Message.GET_BLOGS_NOT_FOUND,
                        Data = null,
                    }
                );
            }
            return Ok(
                new ApiResponse<GetListBlogResponse>
                {
                    Code = 200,
                    Message = Message.GET_BLOGS_SUCCESS,
                    Data = response,
                }
            );
        }

        [HttpGet]
        [Route("Get-Blog-Detail")]
        public async Task<ActionResult> GetBlogDetail([FromQuery] GetBlogDetailRequest request)
        {
            var response = await _mediator.Send(request);
            if (response == null)
            {
                return Ok(
                    new ApiResponse<GetBlogDetailResponse>
                    {
                        Code = 404,
                        Message = Message.GET_BLOG_NOT_FOUND,
                        Data = null,
                    }
                );
            }
            return Ok(
                new ApiResponse<GetBlogDetailResponse>
                {
                    Code = 200,
                    Message = Message.GET_BLOG_SUCCESS,
                    Data = response,
                }
            );
        }

        [HttpPost]
        [Route("Update-Blog")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> UpdateBlog([FromForm] UpdateBlogRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpPost]
        [Route("Change-Status-Blog")]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<ActionResult> ChangeStatusBlog([FromBody] ChangeStatusBlogRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}
