using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IAssignmentDeploymentRepository
    {
        Task<AssignmentDeployment> GetByIdAsync(int Id);
        Task<IList<AssignmentDeployment>> GetByAssignmentIdAsync(int assignmentEventId);
        Task<bool> AddAsync(AssignmentDeployment assignmentDeployment);
        Task<bool> EditAsync(AssignmentDeployment assignmentDeployment);
        Task<bool> DeleteAsync(int assignmentDeploymentId);
    }
}
