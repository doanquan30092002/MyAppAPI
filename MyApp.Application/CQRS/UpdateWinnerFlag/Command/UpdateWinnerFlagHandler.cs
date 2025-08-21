using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.UpdateStatusDeposit.Command;
using MyApp.Application.Interfaces.IUpdateWinnerFlagRepository;

namespace MyApp.Application.CQRS.UpdateFlagWinner.Command
{
    public class UpdateWinnerFlagHandler
        : IRequestHandler<UpdateWinnerFlagRequest, UpdateWinnerFlagResponse>
    {
        private readonly IUpdateWinnerFlagRepository _updateWinnerFlagRepository;

        public UpdateWinnerFlagHandler(IUpdateWinnerFlagRepository updateWinnerFlagRepository)
        {
            _updateWinnerFlagRepository = updateWinnerFlagRepository;
        }

        public async Task<UpdateWinnerFlagResponse> Handle(
            UpdateWinnerFlagRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                bool updateResult = await _updateWinnerFlagRepository.UpdateWinnerFlagAsync(
                    request.AuctionRoundPriceId
                );

                return new UpdateWinnerFlagResponse { StatusUpdate = updateResult };
            }
            catch (Exception)
            {
                return new UpdateWinnerFlagResponse { StatusUpdate = false };
            }
        }
    }
}
