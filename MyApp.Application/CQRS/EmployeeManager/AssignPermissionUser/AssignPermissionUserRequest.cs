using MediatR;

namespace MyApp.Application.CQRS.EmployeeManager.AssignPermissionUser
{
    public class AssignPermissionUserRequest : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public int RoleId { get; set; }
    }
}
