using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.BamsRepositories
{
    public interface IDeploymentEquipmentRepository
    {
        Task<bool> AddAsync(DeploymentEquipment deploymentEquipment);

        Task<bool> EditAsync(DeploymentEquipment deploymentEquipment);

        Task<bool> DeleteAsync(int deploymentEquipmentId);

        Task<bool> DeleteByDeploymentIdAsync(int deploymentId);

        Task<IList<DeploymentEquipment>> GetByIdAsync(int deploymentEquipmentId);

        Task<IList<DeploymentEquipment>> GetByDeploymentIdAsync(int assignmentDeploymentId);

        Task<IList<DeploymentEquipment>> GetByAssignmentEventIdAsync(int assignmentEventId);
        Task<IList<DeploymentEquipment>> GetByAssignmentIdAndAssetIdAsync(int assignmentEventId, string assetId);

    }
}
