using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetUserInfo.Queries;

namespace MyApp.Application.Interfaces.IGetUserInfoRepository
{
    public interface IGetUserInfoRepository
    {
        Task<GetUserInfoResponse> GetUserInfoAsync(
            Guid userId,
            CancellationToken cancellationToken
        );
    }
}
