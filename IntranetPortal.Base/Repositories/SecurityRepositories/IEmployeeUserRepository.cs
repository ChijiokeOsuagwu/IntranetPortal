using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SecurityRepositories
{
    public interface IEmployeeUserRepository
    {
        Task<IList<EmployeeUser>> GetAllAsync();
        Task<IList<EmployeeUser>> GetByNameAsync(string fullName);
    }
}
