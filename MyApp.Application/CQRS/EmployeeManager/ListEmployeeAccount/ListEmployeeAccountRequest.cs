using MediatR;

namespace MyApp.Application.CQRS.EmployeeManager.ListEmployeeAccount
{
    public class ListEmployeeAccountRequest : IRequest<ListEmployeeAccountResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Seach { get; set; } //  giá trị cần search email or username

        //1:	Admin
        //2:	Customer
        //3:	Staff
        //4:	Auctioneer
        //5:	Director
        //6:	Manager
        public int? RoleId { get; set; }
    }
}
