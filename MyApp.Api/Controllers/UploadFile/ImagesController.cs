using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.TestUploadFile.Commands;
using MyApp.Application.CQRS.User.Queries.Authenticate;

namespace MyApp.Api.Controllers.UploadFile
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("Upload")]
        [AllowAnonymous]
        public async Task<ActionResult<ImageUploadResponse>> Upload(
            [FromForm] ImageUploadRequest imageUploadRequest
        )
        {
            var response = await _mediator.Send(imageUploadRequest);
            return Ok(response);
        }
    }
}
