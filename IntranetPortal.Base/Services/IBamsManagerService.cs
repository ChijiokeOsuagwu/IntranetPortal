using IntranetPortal.Base.Models.BamsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IBamsManagerService
    {
        #region Assignment Event Action Methods
        Task<IList<AssignmentEvent>> GetOpenAssignmentsAsync();
        Task<IList<AssignmentEvent>> GetAllAssignmentsAsync();
        Task<AssignmentEvent> GetAssignmentEventByIdAsync(int assignmentEventId);
        Task<IList<AssignmentEvent>> GetAssignmentsByYearAndMonthAsync(int? startYear, int? startMonth = null);
        Task<IList<AssignmentEvent>> SearchAssignmentEventsByCustomerNameAsync(string customerName);
        Task<bool> CreateAssignmentEventAsync(AssignmentEvent assignmentEvent);
        Task<bool> DeleteAssignmentEventAsync(int assignmentEventId);
        Task<bool> UpdateAssignmentEventAsync(AssignmentEvent assignmentEvent);
        #endregion

        #region Assignment Deployment Action Methods
        Task<IList<AssignmentDeployment>> GetAssignmentDeploymentsByAssignmentEventIdAsync(int assignmentEventId);
        Task<AssignmentDeployment> GetAssignmentDeploymentByIdAsync(int assignmentDeploymentId);
        Task<bool> CreateAssignmentDeploymentAsync(AssignmentDeployment assignmentDeployment);
        Task<bool> UpdateAssignmentDeploymentAsync(AssignmentDeployment assignmentDeployment);
        Task<bool> DeleteAssignmentDeploymentAsync(int assignmentDeploymentId);
        #endregion

        #region Assignment Settings Action Methods
        Task<IList<AssignmentEventType>> GetAssignmentEventTypesAsync();
        Task<IList<AssignmentStatus>> GetOnlyAssignmentStatusAsync();
        Task<IList<AssignmentStatus>> GetOnlyDeploymentStatusAsync();
        #endregion

        #region Deployment Team Members Action Methods
        Task<IList<DeploymentTeamMember>> GetDeploymentTeamMembersByAssignmentEventIdAsync(int assignmentEventId);
        Task<IList<DeploymentTeamMember>> GetDeploymentTeamMembersByAssignmentEventIdAndPersonIdAsync(int assignmentEventId, string personId);
        Task<IList<DeploymentTeamMember>> GetDeploymentTeamMembersByDeploymentIdAsync(int deploymentId);
        Task<DeploymentTeamMember> GetDeploymentTeamMembersByIdAsync(int deploymentTeamMemberId);
        Task<bool> CreateDeploymentTeamMemberAsync(DeploymentTeamMember deploymentTeamMember);
        Task<bool> UpdateDeploymentTeamMemberAsync(DeploymentTeamMember deploymentTeamMember);
        Task<bool> DeleteDeploymentTeamMemberAsync(int deploymentTeamMemberId);
        Task<bool> DeleteDeploymentTeamMembersByDeploymentIdAsync(int deploymentId);

        #endregion

        #region Deployment Equipments Action Methods
        Task<DeploymentEquipment> GetDeploymentEquipmentByIdAsync(int deploymentEquipmentId);
        Task<IList<DeploymentEquipment>> GetDeploymentEquipmentByAssignmentEventIdAsync(int assignmentEventId);
        Task<IList<DeploymentEquipment>> GetDeploymentEquipmentsByAssignmentEventIdAndAssetIdAsync(int assignmentEventId, string assetId);
        Task<IList<DeploymentEquipment>> GetDeploymentEquipmentsByDeploymentIdAsync(int deploymentId);
        Task<bool> CreateDeploymentEquipmentAsync(DeploymentEquipment deploymentEquipment);
        Task<bool> UpdateDeploymentEquipmentAsync(DeploymentEquipment deploymentEquipment);
        Task<bool> DeleteDeploymentEquipmentAsync(int deploymentEquipmentId);
        Task<bool> DeleteDeploymentEquipmentByDeploymentIdAsync(int deploymentId);
        #endregion

        #region Assignment Extensions Action Methods
        Task<IList<AssignmentExtension>> GetAssignmentExtensionsByAssignmentEventIdAsync(int assignmentEventId);
        Task<AssignmentExtension> GetAssignmentExtensionByIdAsync(int assignmentExtensionId);
        Task<bool> CreateAssignmentExtensionAsync(AssignmentExtension assignmentExtension);
        Task<bool> DeleteAssignmentExtensionAsync(int assignmentExtensionId);
        Task<DateTime?> GetAssignmentEventClosingTime(int assignmentEventId);
        #endregion

        #region Assignment Updates Action Methods
        Task<IList<AssignmentUpdates>> GetAssignmentUpdatesByAssignmentEventIdAsync(int assignmentEventId);
        Task<AssignmentUpdates> GetAssignmentUpdateByIdAsync(int assignmentUpdateId);
        Task<bool> CreateAssignmentUpdateAsync(AssignmentUpdates assignmentUpdate);
        Task<bool> EditAssignmentUpdateAsync(AssignmentUpdates assignmentUpdate);
        Task<bool> DeleteAssignmentUpdateAsync(int assignmentUpdateId);
        #endregion

        #region Equipment Group Action Methods
        Task<EquipmentGroup> GetEquipmentGroupByIdAsync(int equipmentGroupId);
        Task<IList<EquipmentGroup>> GetAllEquipmentGroupsAsync();
        Task<bool> CreateEquipmentGroupAsync(EquipmentGroup equipmentGroup);
        Task<bool> UpdateEquipmentGroupAsync(EquipmentGroup equipmentGroup);
        Task<bool> DeleteEquipmentGroupAsync(int equipmentGroupId);
        #endregion

        #region Asset Equipment Groups Action Methods
        Task<IList<AssetEquipmentGroup>> GetAssetEquipmentGroupsByEquipmentGroupIdAsync(int equipmentGroupId);
        Task<IList<AssetEquipmentGroup>> GetAssetEquipmentGroupsByEquipmentGroupIdAndEquipmentIdAsync(int equipmentGroupId, string equipmentId);
        Task<AssetEquipmentGroup> GetAssetEquipmentGroupByIdAsync(int assetEquipmentGroupId);
        Task<bool> CreateAssetEquipmentGroupAsync(AssetEquipmentGroup assetEquipmentGroup);
        Task<bool> DeleteAssetEquipmentGroupAsync(int assetEquipmentGroupId);
        #endregion
    }
}
