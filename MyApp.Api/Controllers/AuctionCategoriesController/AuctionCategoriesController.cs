using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.AuctionCategories.Queries;
using MyApp.Core.Entities;

namespace MyApp.Api.Controllers.AuctionCategoeisController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionCategoriesController(IMediator _mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _mediator.Send(new FindAllAuctionCategoriesQuery());
            var response = new ApiResponse<List<AuctionCategory>>
            {
                Code = 200,
                Message = "Lấy danh sách category thành công",
                Data = categories,
            };
            return Ok(response);
        }
    }
}
