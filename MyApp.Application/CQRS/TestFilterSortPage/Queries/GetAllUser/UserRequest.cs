using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.TestFilterSortPage.Queries.GetAllUser
{
    public class UserRequest : IRequest<UserResponse>
    {
        public string? FilterOn { get; set; }
        public string? FilterQuery { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 2;
    }
}
