namespace MyApp.Application.CQRS.EmployeeManager.ListEmployeeAccount
{
    public class ListEmployeeAccountResponse
    {
        public List<EmployeeAccountResponse> EmployeeAccounts { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class EmployeeAccountResponse
    {
        public Guid AccountId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public Guid UserId { get; set; }
        public string CitizenIdentification { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Nationality { get; set; }
        public bool Gender { get; set; }
        public DateTime ValidDate { get; set; }
        public string OriginLocation { get; set; }
        public string RecentLocation { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssueBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
