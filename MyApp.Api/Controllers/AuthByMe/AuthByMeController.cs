using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.CQRS.TestFilterSortPage.Queries.GetAllUser;
using MyApp.Application.CQRS.User.Queries.Authenticate;
using MyApp.Core.Entities;

namespace AuthenAuthorDemo.Controllers.AuthByMe
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthByMeController(IMediator _mediator) : ControllerBase
    {
        //private readonly IRegisterUserService registerUserService;
        //private readonly IUserRepository userRepository;
        //private readonly ITokenRepository tokenRepository;

        //public AuthByMeController(
        //    IRegisterUserService registerUserService,
        //    IUserRepository userRepository,
        //    ITokenRepository tokenRepository
        //)
        //{
        //    this.registerUserService = registerUserService;
        //    this.userRepository = userRepository;
        //    this.tokenRepository = tokenRepository;
        //}

        //[HttpPost]
        //[Route("Register")]
        //public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        //{
        //    var user = await registerUserService.RegisterUser(registerRequestDTO);
        //    if (user != null)
        //    {
        //        return Ok("Register success");
        //    }

        //    return BadRequest("Something went wrong");
        //}

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            var response = await _mediator.Send(loginRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("GetAllUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetAllUser([FromQuery] UserRequest userRequest)
        {
            var response = await _mediator.Send(userRequest);
            return Ok(response);
        }
    }
}
