using System.Security.Claims;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;
using MyApp.Application.JobBackgroud.AuctionJob;

namespace MyApp.Application.CQRS.AssginAuctioneerAndPublicAuction.Command
{
    public class AssginAuctioneerAndPublicAuctionHandler
        : IRequestHandler<
            AssginAuctioneerAndPublicAuctionRequest,
            AssginAuctioneerAndPublicAuctionResponse
        >
    {
        private readonly IAssginAuctioneerAndPublicAuctionRepository _assginAuctioneerAndPublicAuctionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SetAuctionUpdateable _setAuctionUpdateableFalse;

        public AssginAuctioneerAndPublicAuctionHandler(
            IAssginAuctioneerAndPublicAuctionRepository assginAuctioneerAndPublicAuctionRepository,
            IHttpContextAccessor httpContextAccessor,
            SetAuctionUpdateable setAuctionUpdateableFalse
        )
        {
            _assginAuctioneerAndPublicAuctionRepository =
                assginAuctioneerAndPublicAuctionRepository;
            _httpContextAccessor = httpContextAccessor;
            _setAuctionUpdateableFalse = setAuctionUpdateableFalse;
        }

        public async Task<AssginAuctioneerAndPublicAuctionResponse> Handle(
            AssginAuctioneerAndPublicAuctionRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            // check 1 auctioneer cannot be assigned to 2 auctions at the same time
            var checkAuctioneerAssigned =
                await _assginAuctioneerAndPublicAuctionRepository.CheckAuctioneerAssignedToAnotherAuctionAsync(
                    request.Auctioneer,
                    request.AuctionId
                );
            if (checkAuctioneerAssigned)
            {
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 400,
                    Message = Message.AUCTIONEER_ASSIGNED_ANOTHER_AUCTION,
                };
            }
            // assign auctioneer to auction and public auction
            var result =
                await _assginAuctioneerAndPublicAuctionRepository.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                    request.AuctionId,
                    request.Auctioneer,
                    userId
                );
            if (result.Item1)
            {
                var delay = DateTime.Parse(result.Item2) - DateTime.Now;
                if (delay > TimeSpan.Zero)
                {
                    BackgroundJob.Schedule<SetAuctionUpdateable>(
                        job => job.SetAuctionUpdateableAsync(request.AuctionId, true),
                        delay
                    );
                }
                else
                {
                    await _setAuctionUpdateableFalse.SetAuctionUpdateableAsync(
                        request.AuctionId,
                        true
                    );
                }
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 200,
                    Message = Message.ASSGIN_AUCTIONEER_AND_PUBLIC_AUCTION_SUCCESS,
                };
            }
            else
            {
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 500,
                    Message = Message.SYSTEM_ERROR,
                };
            }
        }
    }
}
