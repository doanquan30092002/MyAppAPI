using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.DTOs.SearchUserAttendance;

namespace MyApp.Application.CQRS.SearchUserAttendance.Queries
{
    public class SearchUserAttendanceResponse
    {
        public string Message { get; set; }
        public string AuctionName { get; set; }
        public int? NumericalOrder { get; set; }
    }
}
