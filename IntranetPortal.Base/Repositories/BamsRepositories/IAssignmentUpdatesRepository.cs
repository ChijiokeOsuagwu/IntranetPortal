using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IAssignmentUpdatesRepository
    {
        Task<AssignmentUpdates> GetByIdAsync(int Id);
        Task<IList<AssignmentUpdates>> GetByAssignmentEventIdAsync(int assignmentEventId);
        Task<bool> AddAsync(AssignmentUpdates assignmentUpdate);
        Task<bool> UpdateAsync(AssignmentUpdates assignmentUpdate);
        Task<bool> DeleteAsync(int assignmentUpdateId);
    }
}
