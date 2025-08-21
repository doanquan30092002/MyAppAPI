using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IGetUserInfoRepository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.GetUserInfo.Queries
{
    public class GetUserInfoQuery : IRequest<GetUserInfoResponse>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserInfoHandler : IRequestHandler<GetUserInfoQuery, GetUserInfoResponse>
    {
        private readonly IGetUserInfoRepository _getUserInfoRepository;

        public GetUserInfoHandler(IGetUserInfoRepository getUserInfoRepository)
        {
            _getUserInfoRepository =
                getUserInfoRepository
                ?? throw new ArgumentNullException(nameof(getUserInfoRepository));
        }

        public async Task<GetUserInfoResponse> Handle(
            GetUserInfoQuery request,
            CancellationToken cancellationToken
        )
        {
            var user = await _getUserInfoRepository.GetUserInfoAsync(
                request.UserId,
                cancellationToken
            );
            return user ?? new GetUserInfoResponse();
        }
    }
}
