using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.ISignUpRepository
{
    public interface IGetRoleRepository
    {
        Task<List<Role>?> GetAllRolesAsync();
    }
}
