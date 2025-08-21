using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.Interfaces.IUnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        void BeginTransaction();

        Task CommitAsync();

        Task RollbackAsync();
    }
}
