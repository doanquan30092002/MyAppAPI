using MediatR;
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

        public Task<SearchUserAttendanceResponseDTO> Handle(
            SearchUserAttendanceRequest request,
            CancellationToken cancellationToken
        )
        {
            var response = _attendanceRepository.SearchUserAttendanceAsync(
                request.AuctionId,
                request.CitizenIdentification
            );
            return response;
        }
    }
}
