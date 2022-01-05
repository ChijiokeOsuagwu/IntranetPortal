using IntranetPortal.Base.Models.GlobalSettingsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.GlobalSettingsRepositories
{
    public interface IDepartmentRepository
    {
        Task<bool> AddDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int Id);
        Task<bool> EditDepartmentAsync(Department department);
        Task<Department> GetDepartmentByIdAsync(int Id);
        Task<IList<Department>> GetDepartmentsAsync();
    }
}
