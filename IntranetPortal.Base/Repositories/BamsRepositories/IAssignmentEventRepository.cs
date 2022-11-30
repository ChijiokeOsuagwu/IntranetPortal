using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IAssignmentEventRepository
    {
        Task<AssignmentEvent> GetByIdAsync(int Id);
        Task<IList<AssignmentEvent>> SearchByCustomerNameAsync(string customerName);
        Task<IList<AssignmentEvent>> GetOpenAsync();
        Task<IList<AssignmentEvent>> GetAllAsync();
        Task<IList<AssignmentEvent>> GetByYearAsync(int startYear);
        Task<IList<AssignmentEvent>> GetByYearAndMonthAsync(int startYear, int startMonth);
        Task<bool> AddAsync(AssignmentEvent assignmentEvent);
        Task<bool> EditAsync(AssignmentEvent assignmentEvent);
        Task<bool> DeleteAsync(int assignmentEventId);
    }
}
