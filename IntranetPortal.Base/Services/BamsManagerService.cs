using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Repositories.BamsRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class BamsManagerService : IBamsManagerService
    {
        private readonly IAssignmentEventRepository _assignmentEventRepository;
        private readonly IAssignmentDeploymentRepository _assignmentDeploymentRepository;
        private readonly IBamsSettingsRepository _bamsSettingsRepository;
        private readonly IDeploymentTeamMemberRepository _deploymentTeamMemberRepository;
        private readonly IDeploymentEquipmentRepository _deploymentEquipmentRepository;
        private readonly IAssignmentExtensionRepository _assignmentExtensionRepository;
        private readonly IAssignmentUpdatesRepository _assignmentUpdatesRepository;
        private readonly IEquipmentGroupsRepository _equipmentGroupsRepository;
        private readonly IAssetEquipmentGroupRepository _assetEquipmentGroupRepository;

        public BamsManagerService(IAssignmentEventRepository assignmentEventRepository,
                                    IAssignmentDeploymentRepository assignmentDeploymentRepository,
                                    IBamsSettingsRepository bamsSettingsRepository,
                                    IDeploymentTeamMemberRepository deploymentTeamMemberRepository,
                                    IDeploymentEquipmentRepository deploymentEquipmentRepository,
                                    IAssignmentExtensionRepository assignmentExtensionRepository,
                                    IAssignmentUpdatesRepository assignmentUpdatesRepository,
                                    IEquipmentGroupsRepository equipmentGroupsRepository,
                                    IAssetEquipmentGroupRepository assetEquipmentGroupRepository)
        {
            _assignmentEventRepository = assignmentEventRepository;
            _assignmentDeploymentRepository = assignmentDeploymentRepository;
            _bamsSettingsRepository = bamsSettingsRepository;
            _deploymentTeamMemberRepository = deploymentTeamMemberRepository;
            _deploymentEquipmentRepository = deploymentEquipmentRepository;
            _assignmentExtensionRepository = assignmentExtensionRepository;
            _assignmentUpdatesRepository = assignmentUpdatesRepository;
            _equipmentGroupsRepository = equipmentGroupsRepository;
            _assetEquipmentGroupRepository = assetEquipmentGroupRepository;
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

        public async Task<IList<AssignmentEvent>> GetAssignmentsByYearAndMonthAsync(int? startYear, int? startMonth = null)
        {
            IList<AssignmentEvent> assignmentEvents = new List<AssignmentEvent>();
            try
            {
                if(startMonth < 1 && startYear > 0)
                {
                    var entities = await _assignmentEventRepository.GetByYearAsync(startYear.Value);
                    if (entities != null && entities.Count > 0) { assignmentEvents = entities; }
                }
                else if(startMonth > 0 && startYear > 0 )
                {
                    var entities = await _assignmentEventRepository.GetByYearAndMonthAsync(startYear.Value,startMonth.Value);
                    if (entities != null && entities.Count > 0) { assignmentEvents = entities; }
                }
                else if(startMonth > 0 && startYear < 1)
                {
                    startYear = DateTime.Today.Year;
                    var entities = await _assignmentEventRepository.GetByYearAndMonthAsync(startYear.Value, startMonth.Value);
                    if (entities != null && entities.Count > 0) { assignmentEvents = entities; }
                }
                else
                {
                    var entities = await _assignmentEventRepository.GetAllAsync();
                    if (entities != null && entities.Count > 0) { assignmentEvents = entities; }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentEvents;
        }

        public async Task<IList<AssignmentEvent>> GetAllAssignmentsAsync()
        {
            IList<AssignmentEvent> assignmentEvents = new List<AssignmentEvent>();
            try
            {
                var entities = await _assignmentEventRepository.GetAllAsync();
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

        //============================ Assignment settings Action Methods =============================//
        #region Assignment Settings Action Methods
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

        public async Task<IList<AssignmentStatus>> GetOnlyDeploymentStatusAsync()
        {
            IList<AssignmentStatus> assignmentStatusList = new List<AssignmentStatus>();
            try
            {
                var entities = await _bamsSettingsRepository.GetAssignmentStatusByTypeAsync("D");
                if (entities != null && entities.Count > 0) { assignmentStatusList = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentStatusList;
        }


        #endregion

        //============================ Assignment Deployments Action Methods =============================//
        #region Assignment Deployments Action Methods
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

        //============================ Deployment Team Members Action Methods =============================//
        #region Deployment Team Members Action Methods

        public async Task<DeploymentTeamMember> GetDeploymentTeamMembersByIdAsync(int deploymentTeamMemberId)
        {
            DeploymentTeamMember member = new DeploymentTeamMember();
            try
            {
                var entities = await _deploymentTeamMemberRepository.GetByIdAsync(deploymentTeamMemberId);
                if (entities != null && entities.Count > 0) { member = entities.ToList().FirstOrDefault(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }
        public async Task<IList<DeploymentTeamMember>> GetDeploymentTeamMembersByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<DeploymentTeamMember> members = new List<DeploymentTeamMember>();
            try
            {
                var entities = await _deploymentTeamMemberRepository.GetByAssignmentEventIdAsync(assignmentEventId);
                if (entities != null && entities.Count > 0) { members = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return members;
        }

        public async Task<IList<DeploymentTeamMember>> GetDeploymentTeamMembersByAssignmentEventIdAndPersonIdAsync(int assignmentEventId, string personId)
        {
            IList<DeploymentTeamMember> members = new List<DeploymentTeamMember>();
            try
            {
                var entities = await _deploymentTeamMemberRepository.GetByAssignmentIdAndPersonIdAsync(assignmentEventId, personId);
                if (entities != null && entities.Count > 0) { members = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return members;
        }
        public async Task<IList<DeploymentTeamMember>> GetDeploymentTeamMembersByDeploymentIdAsync(int deploymentId)
        {
            IList<DeploymentTeamMember> members = new List<DeploymentTeamMember>();
            try
            {
                var entities = await _deploymentTeamMemberRepository.GetByDeploymentIdAsync(deploymentId);
                if (entities != null && entities.Count > 0) { members = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return members;
        }
        public async Task<bool> CreateDeploymentTeamMemberAsync(DeploymentTeamMember deploymentTeamMember)
        {
            if (deploymentTeamMember == null) { throw new ArgumentNullException(nameof(deploymentTeamMember), "Required parameter [Deployment Team Member] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentTeamMemberRepository.AddAsync(deploymentTeamMember);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> UpdateDeploymentTeamMemberAsync(DeploymentTeamMember deploymentTeamMember)
        {
            if (deploymentTeamMember == null) { throw new ArgumentNullException(nameof(deploymentTeamMember), "Required parameter [Deployment Team Member] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentTeamMemberRepository.EditAsync(deploymentTeamMember);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> DeleteDeploymentTeamMemberAsync(int deploymentTeamMemberId)
        {
            if (deploymentTeamMemberId < 1) { throw new ArgumentNullException(nameof(deploymentTeamMemberId), "Required parameter [DeploymentTeamMemberID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentTeamMemberRepository.DeleteAsync(deploymentTeamMemberId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteDeploymentTeamMembersByDeploymentIdAsync(int deploymentId)
        {
            if (deploymentId < 1) { throw new ArgumentNullException(nameof(deploymentId), "Required parameter [DeploymentID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentTeamMemberRepository.DeleteByDeploymentIdAsync(deploymentId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion

        //============================ Deployment Equipments Action Methods =============================//
        #region Deployment Equipments Action Methods
        public async Task<DeploymentEquipment> GetDeploymentEquipmentByIdAsync(int deploymentEquipmentId)
        {
            DeploymentEquipment equipment = new DeploymentEquipment();
            try
            {
                var entities = await _deploymentEquipmentRepository.GetByIdAsync(deploymentEquipmentId);
                if (entities != null && entities.Count > 0) { equipment = entities.ToList().FirstOrDefault(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return equipment;
        }
        public async Task<IList<DeploymentEquipment>> GetDeploymentEquipmentByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            try
            {
                var entities = await _deploymentEquipmentRepository.GetByAssignmentEventIdAsync(assignmentEventId);
                if (entities != null && entities.Count > 0) { equipments = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return equipments;
        }
        public async Task<IList<DeploymentEquipment>> GetDeploymentEquipmentsByAssignmentEventIdAndAssetIdAsync(int assignmentEventId, string assetId)
        {
            IList<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            try
            {
                var entities = await _deploymentEquipmentRepository.GetByAssignmentIdAndAssetIdAsync(assignmentEventId, assetId);
                if (entities != null && entities.Count > 0) { equipments = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return equipments;
        }
        public async Task<IList<DeploymentEquipment>> GetDeploymentEquipmentsByDeploymentIdAsync(int deploymentId)
        {
            IList<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            try
            {
                var entities = await _deploymentEquipmentRepository.GetByDeploymentIdAsync(deploymentId);
                if (entities != null && entities.Count > 0) { equipments = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return equipments;
        }
        public async Task<bool> CreateDeploymentEquipmentAsync(DeploymentEquipment deploymentEquipment)
        {
            if (deploymentEquipment == null) { throw new ArgumentNullException(nameof(deploymentEquipment), "Required parameter [Deployment Equipment] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentEquipmentRepository.AddAsync(deploymentEquipment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> UpdateDeploymentEquipmentAsync(DeploymentEquipment deploymentEquipment)
        {
            if (deploymentEquipment == null) { throw new ArgumentNullException(nameof(deploymentEquipment), "Required parameter [Deployment Equipment] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentEquipmentRepository.EditAsync(deploymentEquipment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> DeleteDeploymentEquipmentAsync(int deploymentEquipmentId)
        {
            if (deploymentEquipmentId < 1) { throw new ArgumentNullException(nameof(deploymentEquipmentId), "Required parameter [DeploymentEquipmentID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentEquipmentRepository.DeleteAsync(deploymentEquipmentId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> DeleteDeploymentEquipmentByDeploymentIdAsync(int deploymentId)
        {
            if (deploymentId < 1) { throw new ArgumentNullException(nameof(deploymentId), "Required parameter [DeploymentID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _deploymentEquipmentRepository.DeleteByDeploymentIdAsync(deploymentId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion

        //============================ Assignment Extension Action Methods ===============================//
        #region AssignmentExtension Action Methods
        public async Task<IList<AssignmentExtension>> GetAssignmentExtensionsByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<AssignmentExtension> assignmentExtensions = new List<AssignmentExtension>();
            try
            {
                var entities = await _assignmentExtensionRepository.GetByAssignmentEventIdAsync(assignmentEventId);
                if (entities != null && entities.Count > 0) { assignmentExtensions = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentExtensions;
        }

        public async Task<AssignmentExtension> GetAssignmentExtensionByIdAsync(int assignmentExtensionId)
        {
            AssignmentExtension assignmentExtension = new AssignmentExtension();
            try
            {
                var entity = await _assignmentExtensionRepository.GetByIdAsync(assignmentExtensionId);
                if (entity != null) { assignmentExtension = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentExtension;
        }

        public async Task<bool> CreateAssignmentExtensionAsync(AssignmentExtension assignmentExtension)
        {
            if (assignmentExtension == null) { throw new ArgumentNullException(nameof(assignmentExtension), "Required parameter [AssignmentExtension] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentExtensionRepository.AddAsync(assignmentExtension);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
       
        public async Task<bool> DeleteAssignmentExtensionAsync(int assignmentExtensionId)
        {
            if (assignmentExtensionId < 1) { throw new ArgumentNullException(nameof(assignmentExtensionId), "Required parameter [AssignmentExtensionID] cannot be null."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentExtensionRepository.DeleteAsync(assignmentExtensionId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<DateTime?> GetAssignmentEventClosingTime(int assignmentEventId)
        {
            var assignmentExtensionList = await _assignmentExtensionRepository.GetByAssignmentEventIdAsync(assignmentEventId);
            AssignmentExtension assignmentExtension = assignmentExtensionList.ToList().FirstOrDefault();
            if(assignmentExtension != null && assignmentExtension.AssignmentExtensionID > 0)
            {
                return assignmentExtension.ToTime;
            }
            else
            {
                AssignmentEvent assignmentEvent = await _assignmentEventRepository.GetByIdAsync(assignmentEventId);
                if(assignmentEvent != null && assignmentEvent.ID > 0)
                {
                    return assignmentEvent.EndTime;
                }
            }
            return null;
        }

        #endregion

        //============================ Assignment Updates Action Methods =================================//
        #region Assignment Updates Action Methods
        public async Task<IList<AssignmentUpdates>> GetAssignmentUpdatesByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<AssignmentUpdates> assignmentUpdates = new List<AssignmentUpdates>();
            try
            {
                var entities = await _assignmentUpdatesRepository.GetByAssignmentEventIdAsync(assignmentEventId);
                if (entities != null && entities.Count > 0) { assignmentUpdates = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentUpdates;
        }

        public async Task<AssignmentUpdates> GetAssignmentUpdateByIdAsync(int assignmentUpdateId)
        {
            AssignmentUpdates assignmentUpdate = new AssignmentUpdates();
            try
            {
                var entity = await _assignmentUpdatesRepository.GetByIdAsync(assignmentUpdateId);
                if (entity != null) { assignmentUpdate = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assignmentUpdate;
        }

        public async Task<bool> CreateAssignmentUpdateAsync(AssignmentUpdates assignmentUpdate)
        {
            if (assignmentUpdate == null) { throw new ArgumentNullException(nameof(assignmentUpdate), "Required parameter [AssignmentExtension] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentUpdatesRepository.AddAsync(assignmentUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> EditAssignmentUpdateAsync(AssignmentUpdates assignmentUpdate)
        {
            if (assignmentUpdate == null) { throw new ArgumentNullException(nameof(assignmentUpdate), "Required parameter [AssignmentExtension] is missing. The request cannot be processed."); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentUpdatesRepository.UpdateAsync(assignmentUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssignmentUpdateAsync(int assignmentUpdateId)
        {
            if (assignmentUpdateId < 1) { throw new ArgumentNullException(nameof(assignmentUpdateId)); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assignmentUpdatesRepository.DeleteAsync(assignmentUpdateId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion

        //============================ Equipment Groups Action Methods =============================//
        #region Equipment Groups Action Methods
        public async Task<EquipmentGroup> GetEquipmentGroupByIdAsync(int equipmentGroupId)
        {
            EquipmentGroup equipmentGroup = new EquipmentGroup();
            try
            {
                var entities = await _equipmentGroupsRepository.GetByIdAsync(equipmentGroupId);
                if (entities != null) { equipmentGroup = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return equipmentGroup;
        }
        public async Task<IList<EquipmentGroup>> GetAllEquipmentGroupsAsync()
        {
            IList<EquipmentGroup> equipmentGroups = new List<EquipmentGroup>();
            try
            {
                var entities = await _equipmentGroupsRepository.GetAllAsync();
                if (entities != null && entities.Count > 0) { equipmentGroups = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return equipmentGroups;
        }
        public async Task<bool> CreateEquipmentGroupAsync(EquipmentGroup equipmentGroup)
        {
            if (equipmentGroup == null) { throw new ArgumentNullException(nameof(equipmentGroup)); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _equipmentGroupsRepository.AddAsync(equipmentGroup);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> UpdateEquipmentGroupAsync(EquipmentGroup equipmentGroup)
        {
            if (equipmentGroup == null) { throw new ArgumentNullException(nameof(equipmentGroup)); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _equipmentGroupsRepository.EditAsync(equipmentGroup);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }
        public async Task<bool> DeleteEquipmentGroupAsync(int equipmentGroupId)
        {
            if (equipmentGroupId < 1) { throw new ArgumentNullException(nameof(equipmentGroupId)); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _equipmentGroupsRepository.DeleteAsync(equipmentGroupId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion

        //============================ Asset Equipment Groups Action Methods =================================//
        #region Asset Equipment Groups Action Methods
        public async Task<IList<AssetEquipmentGroup>> GetAssetEquipmentGroupsByEquipmentGroupIdAsync(int equipmentGroupId)
        {
            IList<AssetEquipmentGroup> assetEquipmentGroups = new List<AssetEquipmentGroup>();
            try
            {
                var entities = await _assetEquipmentGroupRepository.GetByEquipmentGroupIdAsync(equipmentGroupId);
                if (entities != null && entities.Count > 0) { assetEquipmentGroups = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetEquipmentGroups;
        }

        public async Task<IList<AssetEquipmentGroup>> GetAssetEquipmentGroupsByEquipmentGroupIdAndEquipmentIdAsync(int equipmentGroupId, string equipmentId)
        {
            IList<AssetEquipmentGroup> assetEquipmentGroups = new List<AssetEquipmentGroup>();
            try
            {
                var entities = await _assetEquipmentGroupRepository.GetByEquipmentIdAndEquipmentGroupIdAsync(equipmentGroupId, equipmentId);
                if (entities != null && entities.Count > 0) { assetEquipmentGroups = entities; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetEquipmentGroups;
        }


        public async Task<AssetEquipmentGroup> GetAssetEquipmentGroupByIdAsync(int assetEquipmentGroupId)
        {
            AssetEquipmentGroup assetEquipmentGroup = new AssetEquipmentGroup();
            try
            {
                var entity = await _assetEquipmentGroupRepository.GetByIdAsync(assetEquipmentGroupId);
                if (entity != null) { assetEquipmentGroup = entity; }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return assetEquipmentGroup;
        }

        public async Task<bool> CreateAssetEquipmentGroupAsync(AssetEquipmentGroup assetEquipmentGroup)
        {
            if (assetEquipmentGroup == null) { throw new ArgumentNullException(nameof(assetEquipmentGroup)); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetEquipmentGroupRepository.AddAsync(assetEquipmentGroup);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        public async Task<bool> DeleteAssetEquipmentGroupAsync(int assetEquipmentGroupId)
        {
            if (assetEquipmentGroupId < 1) { throw new ArgumentNullException(nameof(assetEquipmentGroupId)); }
            bool IsSuccessful = false;
            try
            {
                IsSuccessful = await _assetEquipmentGroupRepository.DeleteAsync(assetEquipmentGroupId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return IsSuccessful;
        }

        #endregion

    }
}
