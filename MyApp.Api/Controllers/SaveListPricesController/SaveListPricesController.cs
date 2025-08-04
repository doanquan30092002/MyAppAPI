using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Response;
using MyApp.Application.CQRS.SaveListPrices.Command;

namespace MyApp.Api.Controllers.SaveListPricesController
{
    [Route("api")]
    [ApiController]
    public class SaveListPricesController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Route("SaveListPrices")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<SaveListPricesResponse>>> SaveListPrices(
            [FromBody] SaveListPricesRequest saveListPricesRequest
        )
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(
                    new ApiResponse<SaveListPricesResponse>
                    {
                        Code = 400,
                        Message = Message.SAVE_LIST_PRICES_FAIL,
                        Data = null,
                    }
                );
            }

            var response = await _mediator.Send(saveListPricesRequest);
            return Ok(
                new ApiResponse<SaveListPricesResponse>
                {
                    Code = 200,
                    Message = Message.SAVE_LIST_PRICES_SUCCESS,
                    Data = response,
                }
            );
        }
    }
}
