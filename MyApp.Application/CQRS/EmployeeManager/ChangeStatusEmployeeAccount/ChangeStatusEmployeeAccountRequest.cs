using MediatR;

namespace MyApp.Application.CQRS.EmployeeManager.ChangeStatusEmployeeAccount
{
    public class ChangeStatusEmployeeAccountRequest : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public bool IsActive { get; set; }
    }
}
