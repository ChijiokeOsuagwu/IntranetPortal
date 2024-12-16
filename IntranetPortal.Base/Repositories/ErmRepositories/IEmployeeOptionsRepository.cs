using IntranetPortal.Base.Models.EmployeeRecordModels;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ErmRepositories
{
    public interface IEmployeeOptionsRepository
    {
        Task<bool> EditAsync(EmployeeOptions e);
        Task<EmployeeOptions> GetAllAsync(string id);
    }
}