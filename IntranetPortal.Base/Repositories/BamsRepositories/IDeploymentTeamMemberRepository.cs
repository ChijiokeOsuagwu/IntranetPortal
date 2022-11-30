using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IDeploymentTeamMemberRepository
    {
        Task<bool> AddAsync(DeploymentTeamMember deploymentTeamMember);

        Task<bool> EditAsync(DeploymentTeamMember deploymentTeamMember);

        Task<bool> DeleteAsync(int deploymentTeamMemberId);

        Task<bool> DeleteByDeploymentIdAsync(int deploymentId);

        Task<IList<DeploymentTeamMember>> GetByIdAsync(int deploymentTeamMemberId);

        Task<IList<DeploymentTeamMember>> GetByDeploymentIdAsync(int assignmentDeploymentId);

        Task<IList<DeploymentTeamMember>> GetByAssignmentEventIdAsync(int assignmentEventId);
        Task<IList<DeploymentTeamMember>> GetByAssignmentIdAndPersonIdAsync(int assignmentEventId, string personId);
    }
}
