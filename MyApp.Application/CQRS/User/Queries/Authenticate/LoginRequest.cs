using System.ComponentModel.DataAnnotations;
using MediatR;

namespace MyApp.Application.CQRS.User.Queries.Authenticate
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
