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
        private readonly IAssginAuctioneerAndPublicAuctionRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SetAuctionUpdateableFalse _setAuctionUpdateableFalse;
        private readonly GenerateNumericalOrder _generateNumericalOrder;

        public AssginAuctioneerAndPublicAuctionHandler(
            IAssginAuctioneerAndPublicAuctionRepository repository,
            IHttpContextAccessor httpContextAccessor,
            SetAuctionUpdateableFalse setAuctionUpdateableFalse,
            GenerateNumericalOrder generateNumericalOrder
        )
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _setAuctionUpdateableFalse = setAuctionUpdateableFalse;
            _generateNumericalOrder = generateNumericalOrder;
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
                await _repository.CheckAuctioneerAssignedToAnotherAuctionAsync(
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
            var result = await _repository.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                request.AuctionId,
                request.Auctioneer,
                userId
            );
            if (result.Item1)
            {
                var delaySetAuctionUpdateableFalse = DateTime.Parse(result.Item2) - DateTime.Now;
                if (delaySetAuctionUpdateableFalse > TimeSpan.Zero)
                {
                    BackgroundJob.Schedule<SetAuctionUpdateableFalse>(
                        job => job.SetAuctionUpdateableFalseAsync(request.AuctionId),
                        delaySetAuctionUpdateableFalse
                    );
                }
                else
                {
                    await _setAuctionUpdateableFalse.SetAuctionUpdateableFalseAsync(
                        request.AuctionId
                    );
                }
                //var delayGenarateNumbericalOrder = DateTime.Parse(result.Item3) - DateTime.Now;
                //if (delayGenarateNumbericalOrder > TimeSpan.Zero)
                //{
                //    BackgroundJob.Schedule<GenerateNumericalOrder>(
                //        job => job.GenerateNumericalOrderAsync(request.AuctionId),
                //        delayGenarateNumbericalOrder
                //    );
                //}
                //else
                //{
                //    await _generateNumericalOrder.GenerateNumericalOrderAsync(request.AuctionId);
                //}
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
