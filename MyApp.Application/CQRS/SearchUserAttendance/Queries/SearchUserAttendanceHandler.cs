using MediatR;
using MyApp.Application.Interfaces.SearchUserAttendance;

namespace MyApp.Application.CQRS.SearchUserAttendance.Queries
{
    public class SearchUserAttendanceHandler
        : IRequestHandler<SearchUserAttendanceRequest, SearchUserAttendanceResponse>
    {
        private readonly ISearchUserAttendanceRepository _attendanceRepository;

        public SearchUserAttendanceHandler(ISearchUserAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public Task<SearchUserAttendanceResponse> Handle(
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
