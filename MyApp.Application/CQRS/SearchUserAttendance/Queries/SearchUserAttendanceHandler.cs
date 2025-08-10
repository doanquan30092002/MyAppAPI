using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.SearchUserAttendance;

namespace MyApp.Application.CQRS.SearchUserAttendance.Queries
{
    public class SearchUserAttendanceHandler
        : IRequestHandler<SearchUserAttendanceRequest, SearchUserAttendanceResponseDTO>
    {
        private readonly ISearchUserAttendanceRepository _attendanceRepository;

        public SearchUserAttendanceHandler(ISearchUserAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<SearchUserAttendanceResponseDTO> Handle(
            SearchUserAttendanceRequest request,
            CancellationToken cancellationToken
        )
        {
            var auctionName = await _attendanceRepository.GetAuctionNameAsync(request.AuctionId);

            if (auctionName == null)
            {
                return new SearchUserAttendanceResponseDTO
                {
                    Message = Message.AUCTION_NOT_EXIST,
                    AuctionName = null,
                    NumericalOrder = null,
                };
            }

            var numericalOrder = await _attendanceRepository.GetNumericalOrderAsync(
                request.AuctionId,
                request.CitizenIdentification
            );

            if (numericalOrder == null)
            {
                return new SearchUserAttendanceResponseDTO
                {
                    Message = Message.NOT_FOUND_NUMERICAL_ORDER,
                    AuctionName = auctionName,
                    NumericalOrder = null,
                };
            }

            return new SearchUserAttendanceResponseDTO
            {
                Message = Message.FOUND_NUMERICAL_ORDER,
                AuctionName = auctionName,
                NumericalOrder = numericalOrder,
            };
        }
    }
}
