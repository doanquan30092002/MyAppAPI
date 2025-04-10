using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces;

namespace MyApp.Application.CQRS.TestFilterSortPage.Queries.GetAllUser
{
    public class GetAllUserHandler : IRequestHandler<UserRequest, UserResponse>
    {
        private readonly IUserRepository userRepository;

        public GetAllUserHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<UserResponse> Handle(
            UserRequest request,
            CancellationToken cancellationToken
        )
        {
            var response = await userRepository.GetAllUser(request);
            return response;
        }
    }
}
