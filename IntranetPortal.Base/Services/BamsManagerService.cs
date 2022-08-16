using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Repositories.BamsRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class BamsManagerService : IBamsManagerService
    {
        private readonly IAssignmentEventRepository _assignmentEventRepository;
        private readonly IAssignmentDeploymentRepository _assignmentDeploymentRepository;
        private readonly IBamsSettingsRepository _bamsSettingsRepository;

        public BamsManagerService(IAssignmentEventRepository assignmentEventRepository,
                                    IAssignmentDeploymentRepository assignmentDeploymentRepository,
                                    IBamsSettingsRepository bamsSettingsRepository)
        {
            _assignmentEventRepository = assignmentEventRepository;
            _assignmentDeploymentRepository = assignmentDeploymentRepository;
            _bamsSettingsRepository = bamsSettingsRepository;
        }

        //============================ Assignment Events Select Action Methods =============================//
        #region Assignment Events Select Action Methods
        public async Task<IList<AssignmentEvent>> GetOpenAssignmentsAsync()
        {
            IList<AssignmentEvent> assignmentEvents = new List<AssignmentEvent>();
            try
            {
                var entities = await _assignmentEventRepository.GetOpenAsync();
                if (entities != null && entities.Count > 0) { assignmentEvents = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentEvents;
        }

        public async Task<AssignmentEvent> GetAssignmentEventByIdAsync(int assignmentEventId)
        {
            AssignmentEvent assignmentEvent = new AssignmentEvent();
            try
            {
                var entity = await _assignmentEventRepository.GetByIdAsync(assignmentEventId);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.Title)) { assignmentEvent = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentEvent;
        }

        public async Task<IList<AssignmentEvent>> SearchAssignmentEventsByCustomerNameAsync(string customerName)
        {
            IList<AssignmentEvent> assignmentEvents = new List<AssignmentEvent>();
            try
            {
                var entities = await _assignmentEventRepository.SearchByCustomerNameAsync(customerName);
                if (entities != null && entities.Count > 0) { assignmentEvents = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentEvents;
        }

        #endregion

        //============================ Assignment Event CRUD Action Methods =============================//
        #region AssignmentEvent CRUD Action Methods
        public async Task<bool> CreateAssignmentEventAsync(AssignmentEvent assignmentEvent)
        {
            if (assignmentEvent == null) { throw new ArgumentNullException(nameof(assignmentEvent), "Required parameter [AssignmentEvent] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentEventRepository.AddAsync(assignmentEvent);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssignmentEventAsync(int assignmentEventId)
        {
            if (assignmentEventId < 1) { throw new ArgumentNullException(nameof(assignmentEventId), "Required parameter [AssignmentEventID] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentEventRepository.DeleteAsync(assignmentEventId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> UpdateAssignmentEventAsync(AssignmentEvent assignmentEvent)
        {
            if (assignmentEvent == null) { throw new ArgumentNullException(nameof(assignmentEvent), "Required parameter [AssignmentEvent] is missing."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentEventRepository.EditAsync(assignmentEvent);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion


        //============================ Assignment Deployments Select Action Methods =============================//
        #region Assignment Deployments Select Action Methods
        public async Task<IList<AssignmentDeployment>> GetAssignmentDeploymentsByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<AssignmentDeployment> assignmentDeployments = new List<AssignmentDeployment>();
            try
            {
                var entities = await _assignmentDeploymentRepository.GetByAssignmentIdAsync(assignmentEventId);
                if (entities != null && entities.Count > 0) { assignmentDeployments = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentDeployments;
        }

        public async Task<AssignmentDeployment> GetAssignmentDeploymentByIdAsync(int assignmentDeploymentId)
        {
            AssignmentDeployment assignmentDeployment = new AssignmentDeployment();
            try
            {
                var entity = await _assignmentDeploymentRepository.GetByIdAsync(assignmentDeploymentId);
                if (entity != null && !string.IsNullOrWhiteSpace(entity.DeploymentTitle)) { assignmentDeployment = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentDeployment;
        }

        #endregion

        //============================ Assignment Deployments CRUD Action Methods =========================// 
        #region Assignment Deployments CRUD Action Methods
        public async Task<bool> CreateAssignmentDeploymentAsync(AssignmentDeployment assignmentDeployment)
        {
            if (assignmentDeployment == null) { throw new ArgumentNullException(nameof(assignmentDeployment), "Required parameter [Assignment Deployment] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentDeploymentRepository.AddAsync(assignmentDeployment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> UpdateAssignmentDeploymentAsync(AssignmentDeployment assignmentDeployment)
        {
            if (assignmentDeployment == null) { throw new ArgumentNullException(nameof(assignmentDeployment), "Required parameter [Assignment Deployment] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentDeploymentRepository.EditAsync(assignmentDeployment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> DeleteAssignmentDeploymentAsync(int assignmentDeploymentId)
        {
            if (assignmentDeploymentId < 1) { throw new ArgumentNullException(nameof(assignmentDeploymentId), "Required parameter [AssignmentDeploymentID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentDeploymentRepository.DeleteAsync(assignmentDeploymentId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion


        //============================ Assignment settings Select Action Methods =============================//
        #region Assignment Settings Select Action Methods
        public async Task<IList<AssignmentEventType>> GetAssignmentEventTypesAsync()
        {
            IList<AssignmentEventType> assignmentEventTypes = new List<AssignmentEventType>();
            try
            {
                var entities = await _bamsSettingsRepository.GetAllAssignmentEventTypesAsync();
                if (entities != null && entities.Count > 0) { assignmentEventTypes = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentEventTypes;
        }

        public async Task<IList<AssignmentStatus>> GetOnlyAssignmentStatusAsync()
        {
            IList<AssignmentStatus> assignmentStatusList = new List<AssignmentStatus>();
            try
            {
                var entities = await _bamsSettingsRepository.GetAssignmentStatusByTypeAsync("A");
                if (entities != null && entities.Count > 0) { assignmentStatusList = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentStatusList;
        }

        #endregion

    }
}
