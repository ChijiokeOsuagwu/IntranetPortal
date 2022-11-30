using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IAssignmentExtensionRepository
    {
        Task<AssignmentExtension> GetByIdAsync(int Id);
        Task<IList<AssignmentExtension>> GetByAssignmentEventIdAsync(int assignmentEventId);
        Task<bool> AddAsync(AssignmentExtension assignmentExtension);
        Task<bool> DeleteAsync(int assignmentExtensionId);
    }
}
