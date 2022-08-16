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
        Task<AssignmentEvent> GetAssignmentEventByIdAsync(int assignmentEventId);
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
        #endregion
    }
}
