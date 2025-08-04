using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.SearchUserAttendance.Queries;

namespace MyApp.Application.Interfaces.SearchUserAttendance
{
    public interface ISearchUserAttendanceRepository
    {
        Task<SearchUserAttendanceResponseDTO> SearchUserAttendanceAsync(
            Guid auctionId,
            string citizenIdentification
        );
    }
}
