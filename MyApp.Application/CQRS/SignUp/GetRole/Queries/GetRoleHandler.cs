using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.SignUp.GetRole.Queries
{
    // Query (Request)
    public class GetRoleQuery : IRequest<List<Role>> { }

    // Handler
    public class GetRoleHandler : IRequestHandler<GetRoleQuery, List<Role>>
    {
        private readonly IGetRoleRepository _getRoleRepository;

        public GetRoleHandler(IGetRoleRepository getRoleRepository)
        {
            _getRoleRepository =
                getRoleRepository ?? throw new ArgumentNullException(nameof(getRoleRepository));
        }

        public async Task<List<Role>> Handle(
            GetRoleQuery request,
            CancellationToken cancellationToken
        )
        {
            var roles = await _getRoleRepository.GetAllRolesAsync();
            return roles?.ToList() ?? new List<Role>();
        }
    }
}
