using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Repositories.AtsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.AtsRepositories
{
    public class AssignmentRepository:IAssignmentRepository
    {
        public IConfiguration _config { get; }
        public AssignmentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }


        #region Assignment Repository

        #region Assignment Read Action Methods

        public async Task<List<Assignment>> GetAssignmentsByClientIdAsync(string clientId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<Assignment> listOfAssignments = new List<Assignment>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-2);
            DateTime to_date = toDate ?? DateTime.Now.AddMonths(1);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.assgn_id, a.assgn_tl, a.assgn_ds, a.evnt_typ_id, ");
            sb.Append("a.start_time, a.end_time, a.station_id, a.assigned_to_id, ");
            sb.Append("a.evnt_venue, a.evnt_state, a.bzns_id, a.liaison_nm, a.liaison_phn, ");
            sb.Append("a.approval_status, a.progress_status, a.ispd, a.islv, a.isus, a.ispr, ");
            sb.Append("a.ctb, a.ctt, a.due_date, a.assigned_by_id, a.evnt_ctr, ");
            sb.Append("a.confirm_status, a.coverage_status, ");
            sb.Append("(SELECT evnt_typ_ds FROM public.ats_evnt_typs WHERE evnt_typ_id = a.evnt_typ_id) as evnt_typ_nm, ");
            sb.Append("(SELECT locname  FROM public.gst_locs WHERE locqk = a.station_id) as station_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_to_id) as assigned_to_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_by_id) as assigned_by_nm ");
            sb.Append("FROM public.ats_assg_inf a ");
            sb.Append("WHERE (a.bzns_id = @bzns_id) ");
            sb.Append("AND (a.start_time >= @dt_frm) ");
            sb.Append("AND (a.end_time <= @dt_to) ");
            sb.Append("ORDER BY a.start_time DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    bzns_id.Value = clientId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            listOfAssignments.Add(new Assignment
                            {
                                Id = reader["assgn_id"] == DBNull.Value ? 0 : (long)reader["assgn_id"],
                                Title = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                Description = reader["assgn_ds"] == DBNull.Value ? string.Empty : reader["assgn_ds"].ToString(),
                                EventTypeId = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                EventStartTime = reader["start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_time"],
                                EventEndTime = reader["end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_time"],
                                StationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                AssignedToId = reader["assigned_to_id"] == DBNull.Value ? string.Empty : reader["assigned_to_id"].ToString(),
                                EventVenue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString(),
                                EventState = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString(),
                                ClientId = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                ContactPerson = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString(),
                                ContactPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString(),
                                ApprovalStatus = reader["approval_status"] == DBNull.Value ? string.Empty : reader["approval_status"].ToString(),
                                ProgressStatus = reader["progress_status"] == DBNull.Value ? string.Empty : reader["progress_status"].ToString(),
                                IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"],
                                IsLive = reader["islv"] == DBNull.Value ? false : (bool)reader["islv"],
                                IsUsed = reader["isus"] == DBNull.Value ? false : (bool)reader["isus"],
                                IsPriority = reader["ispr"] == DBNull.Value ? false : (bool)reader["ispr"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                                ReportDueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["due_date"],
                                AssignedById = reader["assigned_by_id"] == DBNull.Value ? string.Empty : reader["assigned_by_id"].ToString(),
                                EventCountry = reader["evnt_ctr"] == DBNull.Value ? string.Empty : reader["evnt_ctr"].ToString(),
                                EventTypeTitle = reader["evnt_typ_nm"] == DBNull.Value ? string.Empty : reader["evnt_typ_nm"].ToString(),
                                StationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                AssignedToName = reader["assigned_to_nm"] == DBNull.Value ? string.Empty : reader["assigned_to_nm"].ToString(),
                                AssignedByName = reader["assigned_by_nm"] == DBNull.Value ? string.Empty : reader["assigned_by_nm"].ToString(),
                                ConfirmationStatus = reader["confirm_status"] == DBNull.Value ? string.Empty : reader["confirm_status"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return listOfAssignments;
        }


        /*
       public async Task<Assignment> GetAssignmentByIdAsync(long assignmentId)
       {
           Assignment assignment = new Assignment();
           StringBuilder sb = new StringBuilder();
           sb.Append("SELECT assgn_id, assgn_tl, assgn_ds, evnt_typ_id, start_time, ");
           sb.Append("end_time, station_id, assigned_to_id, evnt_venue, evnt_state, ");
           sb.Append("bzns_id, liaison_nm, liaison_phn, approval_status, ");
           sb.Append("progress_status, ispd, islv, isus, ispr, ctb, ctt, due_date, ");
           sb.Append("assigned_by_id, evnt_ctr, ");
           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = assigned_to_id) as assigned_to_nm, ");
           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = assigned_by_id) as assigned_by_nm, ");
           sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = station_id) as station_nm, ");
           sb.Append("(SELECT bzns_name FROM public.gst_bzns WHERE bzns_id = bzns_id) as bzns_nm ");
           sb.Append("FROM public.ats_assg_inf a ");
           sb.Append("WHERE (assgn_id=@assgn_id);");

           string query = sb.ToString();
           using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
           {
               await conn.OpenAsync();
               // Retrieve all rows
               using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
               {
                   var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                   await cmd.PrepareAsync();
                   assgn_id.Value = assignmentId;
                   using (var reader = await cmd.ExecuteReaderAsync())
                       while (await reader.ReadAsync())
                       {
                           task.Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"];
                           task.Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString();
                           task.Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString();
                           task.MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString();
                           task.WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"];
                           task.WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString();
                           task.MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"];
                           task.LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString();
                           task.LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString();
                           task.LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"];
                           task.TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString();
                           task.TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString();
                           //task.AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString();
                           //task.AssignedToName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString();
                           task.AssignedByEmployeeId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString();
                           task.AssignedByEmployeeName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString();
                           task.AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"];
                           task.StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"];
                           task.StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString();
                           task.Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"];

                           task.ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"];
                           task.ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"];
                           task.ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString();

                           task.ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"];
                           task.ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"];
                           task.ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString();

                           task.ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"];
                           task.ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString();
                           task.ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"];
                           task.ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"];
                           task.ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"];
                           task.ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"];

                           task.IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"];
                           task.CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString();
                           task.CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"];

                           task.IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"];
                           task.ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString();
                           task.ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"];

                           task.UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"];
                           task.UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString();
                           task.DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"];
                           task.DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString();
                           task.LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                           task.LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString();

                           task.CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"];
                           task.CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString();
                           task.CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"];
                           task.IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"];

                           task.CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"];
                           task.CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString();
                           task.LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"];
                           task.LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString();

                           task.IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"];
                           task.AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"];
                       }
               }
               await conn.CloseAsync();
           }
           return assignment;
       }

       public async Task<List<TaskItem>> GetTaskItemsByFolderIdAsync(long folderId)
       {
           List<TaskItem> taskList = new List<TaskItem>();
           StringBuilder sb = new StringBuilder();
           sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
           sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
           sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
           sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
           sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
           sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
           sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
           sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
           sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
           sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
           sb.Append("WHEN 2 THEN 'Approved for Execution' ");
           sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
           sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
           sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");

           sb.Append("CASE t.prgs_stts WHEN 0 THEN 'Not Yet Started' ");
           sb.Append("WHEN 1 THEN 'In Progress' ");
           sb.Append("WHEN 2 THEN 'Completed' ");
           sb.Append("WHEN 3 THEN 'On Hold' END AS prgs_stts_ds, ");

           sb.Append("CASE t.apprv_stts WHEN 0 THEN 'Pending' ");
           sb.Append("WHEN 1 THEN 'Approved' ");
           sb.Append("WHEN 2 THEN 'Declined' END AS apprv_stts_ds, ");

           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
           sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
           sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
           sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
           sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm ");
           sb.Append("FROM public.wsp_tsk_itms t ");
           sb.Append("WHERE (t.wki_fdr_id=@wki_fdr_id) ");
           sb.Append("ORDER BY t.tsk_itm_id;");
           string query = sb.ToString();
           using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
           {
               await conn.OpenAsync();
               // Retrieve all rows
               using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
               {
                   var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                   await cmd.PrepareAsync();
                   wki_fdr_id.Value = folderId;
                   using (var reader = await cmd.ExecuteReaderAsync())
                       while (await reader.ReadAsync())
                       {
                           taskList.Add(new TaskItem
                           {
                               Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                               Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                               Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                               MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                               WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                               WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                               MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                               LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                               LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                               LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                               TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                               TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                               AssignedByEmployeeId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                               AssignedByEmployeeName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                               AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                               StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                               StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                               Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],

                               ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                               ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                               ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString(),

                               ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                               ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                               ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString(),

                               ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                               ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                               ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                               ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                               ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                               ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                               IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                               CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                               CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                               IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                               ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                               ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                               UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                               UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                               DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                               DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                               LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                               LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                               CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                               CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                               CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                               IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                               CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                               CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                               LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                               LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                               IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                               AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                           });
                       }
               }
               await conn.CloseAsync();
           }
           return taskList;
       }
       public async Task<List<TaskItem>> GetTaskItemsByOwnerIdnDescriptionnFolderIdAsync(string ownerId, string taskDescription, long? folderId)
       {
           List<TaskItem> taskList = new List<TaskItem>();
           StringBuilder sb = new StringBuilder();
           sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
           sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
           sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
           sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
           sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
           sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
           sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
           sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
           sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
           sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
           sb.Append("WHEN 2 THEN 'Approved for Execution' ");
           sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
           sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
           sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");

           sb.Append("CASE t.prgs_stts WHEN 0 THEN 'Not Yet Started' ");
           sb.Append("WHEN 1 THEN 'In Progress' ");
           sb.Append("WHEN 2 THEN 'Completed' ");
           sb.Append("WHEN 3 THEN 'On Hold' END AS prgs_stts_ds, ");

           sb.Append("CASE t.apprv_stts WHEN 0 THEN 'Pending' ");
           sb.Append("WHEN 1 THEN 'Approved' ");
           sb.Append("WHEN 2 THEN 'Declined' END AS apprv_stts_ds, ");

           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
           sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
           sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
           sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
           sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm ");
           sb.Append("FROM public.wsp_tsk_itms t ");
           sb.Append("WHERE (t.tsk_owner_id = @tsk_owner_id) ");
           sb.Append("AND LOWER(t.tsk_itm_ds) = LOWER(@tsk_itm_ds)  ");
           sb.Append("AND (t.wki_fdr_id=@wki_fdr_id OR t.wki_fdr_id IS NULL) ");
           sb.Append("ORDER BY t.tsk_itm_id;");
           string query = sb.ToString();
           using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
           {
               await conn.OpenAsync();
               // Retrieve all rows
               using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
               {
                   var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                   var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                   var tsk_itm_ds = cmd.Parameters.Add("@tsk_itm_ds", NpgsqlDbType.Text);
                   await cmd.PrepareAsync();
                   tsk_owner_id.Value = ownerId;
                   wki_fdr_id.Value = folderId;
                   tsk_itm_ds.Value = taskDescription;
                   using (var reader = await cmd.ExecuteReaderAsync())
                       while (await reader.ReadAsync())
                       {
                           taskList.Add(new TaskItem
                           {
                               Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                               Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                               Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                               MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                               WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                               WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                               MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                               LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                               LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                               LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                               TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                               TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                               AssignedByEmployeeId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                               AssignedByEmployeeName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                               AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                               StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                               StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                               Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],

                               ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                               ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                               ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString(),

                               ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                               ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                               ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString(),

                               ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                               ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                               ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                               ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                               ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                               ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                               IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                               CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                               CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                               IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                               ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                               ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                               UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                               UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                               DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                               DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                               LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                               LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                               CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                               CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                               CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                               IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                               CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                               CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                               LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                               LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                               IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                               AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                           });
                       }
               }
               await conn.CloseAsync();
           }
           return taskList;
       }
       public async Task<long> GetTaskItemsCountByFolderIdAsync(long folderId)
       {
           long _totalCount = 0;
           StringBuilder sb = new StringBuilder();
           sb.Append("SELECT COUNT(tsk_itm_id) as total ");
           sb.Append("FROM public.wsp_tsk_itms ");
           sb.Append("WHERE (wki_fdr_id=@wki_fdr_id); ");
           string query = sb.ToString();
           using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
           {
               await conn.OpenAsync();
               // Retrieve all rows
               using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
               {
                   var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                   await cmd.PrepareAsync();
                   wki_fdr_id.Value = folderId;
                   var obj = await cmd.ExecuteScalarAsync();
                   _totalCount = Convert.ToInt64(obj);
               }
               await conn.CloseAsync();
           }
           return _totalCount;
       }

       //========= Pending Task Items ==================//
       public async Task<List<TaskItem>> GetTaskItemsPendingByOwnerIdAsync(string ownerId)
       {
           List<TaskItem> taskList = new List<TaskItem>();
           StringBuilder sb = new StringBuilder();
           sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
           sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
           sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
           sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
           sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
           sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
           sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
           sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
           sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
           sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
           sb.Append("WHEN 2 THEN 'Approved for Execution' ");
           sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
           sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
           sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");
           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
           sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
           sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
           sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
           sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
           sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm ");
           sb.Append("FROM public.wsp_tsk_itms t ");
           sb.Append("WHERE (t.tsk_owner_id=@tsk_owner_id) AND (t.wki_fdr_id IS NULL) ");
           sb.Append("ORDER BY t.tsk_itm_id;");
           string query = sb.ToString();
           using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
           {
               await conn.OpenAsync();
               // Retrieve all rows
               using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
               {
                   var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                   await cmd.PrepareAsync();
                   tsk_owner_id.Value = ownerId;
                   using (var reader = await cmd.ExecuteReaderAsync())
                       while (await reader.ReadAsync())
                       {
                           taskList.Add(new TaskItem
                           {
                               Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                               Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                               Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                               MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                               WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                               WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                               MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                               LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                               LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                               LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                               TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                               TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                               AssignedByEmployeeId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                               AssignedByEmployeeName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                               AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                               StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                               StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                               Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],
                               ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                               ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                               ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                               ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                               ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                               ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                               ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                               ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                               ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                               ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                               IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                               CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                               CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                               IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                               ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                               ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                               UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                               UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                               DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                               DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                               LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                               LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                               CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                               CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                               CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                               IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                               CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                               CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                               LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                               LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                               IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                               AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                           });
                       }
               }
               await conn.CloseAsync();
           }
           return taskList;
       }
       public async Task<long> GetTaskItemsPendingCountByOwnerIdAsync(string ownerId)
       {
           long item_count = 0;
           StringBuilder sb = new StringBuilder();
           sb.Append("SELECT COUNT(tsk_itm_id) as total ");
           sb.Append("FROM public.wsp_tsk_itms t ");
           sb.Append("WHERE (t.tsk_owner_id=@tsk_owner_id) AND (t.wki_fdr_id IS NULL); ");
           string query = sb.ToString();
           using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
           {
               await conn.OpenAsync();
               // Retrieve all rows
               using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
               {
                   var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                   await cmd.PrepareAsync();
                   tsk_owner_id.Value = ownerId;
                   var obj = await cmd.ExecuteScalarAsync();
                   item_count = (long)obj;
               }
               await conn.CloseAsync();
           }
           return item_count;
       }
       */
        #endregion

        #region Task Items Write Action Methods
        public async Task<long> AddAssignmentAsync(Assignment assignment)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_inf(assgn_tl, assgn_ds, evnt_typ_id, ");
            sb.Append("start_time, end_time, station_id, assigned_to_id, evnt_venue, ");
            sb.Append("evnt_state, evnt_bzns_id, liaison_nm, liaison_phn, approval_status, ");
            sb.Append("progress_status, ispd, islv, isus, ispr, ctb, ctt, due_date, ");
            sb.Append("assigned_by_id, evnt_ctr) VALUES (@assgn_tl, @assgn_ds, @evnt_typ_id, ");
            sb.Append("@start_time, @end_time, @station_id, @assigned_to_id, @evnt_venue, ");
            sb.Append("@evnt_state, @evnt_bzns_id, @liaison_nm, @liaison_phn, @approval_status, ");
            sb.Append("@progress_status, @ispd, @islv, @isus, @ispr, @ctb, @ctt, @due_date, ");
            sb.Append("@assigned_by_id, @evnt_ctr) RETURNING assgn_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_tl = cmd.Parameters.Add("@assgn_tl", NpgsqlDbType.Text);
                    var assgn_ds = cmd.Parameters.Add("@assgn_ds", NpgsqlDbType.Text);
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    var start_time = cmd.Parameters.Add("@start_time", NpgsqlDbType.Timestamp);
                    var end_time = cmd.Parameters.Add("@end_time", NpgsqlDbType.Timestamp);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var assigned_to_id = cmd.Parameters.Add("@assigned_to_id", NpgsqlDbType.Text);
                    var assigned_by_id = cmd.Parameters.Add("@assigned_by_id", NpgsqlDbType.Text);
                    var evnt_venue = cmd.Parameters.Add("@evnt_venue", NpgsqlDbType.Text);
                    var evnt_state = cmd.Parameters.Add("@evnt_state", NpgsqlDbType.Text);
                    var evnt_ctr = cmd.Parameters.Add("@evnt_ctr", NpgsqlDbType.Text);
                    var evnt_bzns_id = cmd.Parameters.Add("@evnt_bzns_id", NpgsqlDbType.Text);
                    var liaison_nm = cmd.Parameters.Add("@liaison_nm", NpgsqlDbType.Text);
                    var liaison_phn = cmd.Parameters.Add("@liaison_phn", NpgsqlDbType.Text);
                    var approval_status = cmd.Parameters.Add("@approval_status", NpgsqlDbType.Text);
                    var progress_status = cmd.Parameters.Add("@progress_status", NpgsqlDbType.Text);
                    var ispd = cmd.Parameters.Add("@ispd", NpgsqlDbType.Boolean);
                    var islv = cmd.Parameters.Add("@islv", NpgsqlDbType.Boolean);
                    var isus = cmd.Parameters.Add("@isus", NpgsqlDbType.Boolean);
                    var ispr = cmd.Parameters.Add("@ispr", NpgsqlDbType.Boolean);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Timestamp);
                    var due_date = cmd.Parameters.Add("@due_date", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assgn_tl.Value = assignment.Title;
                    assgn_ds.Value = assignment.Description ?? (object)DBNull.Value; 
                    evnt_typ_id.Value = assignment.EventTypeId;
                    start_time.Value = assignment.EventStartTime ?? (object)DBNull.Value;
                    end_time.Value = assignment.EventEndTime ?? (object)DBNull.Value;
                    station_id.Value = assignment.StationId ?? (object)DBNull.Value;
                    assigned_to_id.Value = assignment.AssignedToId ?? (object)DBNull.Value;
                    assigned_by_id.Value = assignment.AssignedById ?? (object)DBNull.Value;
                    evnt_venue.Value = assignment.EventVenue ?? (object)DBNull.Value;
                    evnt_state.Value = assignment.EventState ?? (object)DBNull.Value;
                    evnt_ctr.Value = assignment.EventCountry ?? (object)DBNull.Value;
                    evnt_bzns_id.Value = assignment.ClientId;
                    liaison_nm.Value = assignment.ContactPerson ?? (object)DBNull.Value;
                    liaison_phn.Value = assignment.ContactPhone ?? (object)DBNull.Value;
                    approval_status.Value = assignment.ApprovalStatus ?? (object)DBNull.Value;
                    progress_status.Value = assignment.ProgressStatus ?? (object)DBNull.Value;
                    ispd.Value = assignment.IsPaid;
                    islv.Value = assignment.IsLive;
                    isus.Value = assignment.IsUsed;
                    ispr.Value = assignment.IsPriority;
                    ctb.Value = assignment.CreatedBy;
                    ctt.Value = assignment.CreatedTime ?? DateTime.Now; 
                    due_date.Value = assignment.ReportDueDate ?? (object)DBNull.Value;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        /*
        public async Task<bool> UpdateTaskItemAsync(TaskItem task)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET tsk_itm_ds=@tsk_itm_ds,  ");
            sb.Append("tsk_itm_inf=@tsk_itm_inf, prj_no=@prj_no, prg_no=@prg_no, ");
            sb.Append("prg_dt=@prg_dt, exp_start_dt=@exp_start_dt, ");
            sb.Append("exp_due_dt=@exp_due_dt, mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_ds = cmd.Parameters.Add("@tsk_itm_ds", NpgsqlDbType.Text);
                    var tsk_itm_inf = cmd.Parameters.Add("@tsk_itm_inf", NpgsqlDbType.Text);
                    var prj_no = cmd.Parameters.Add("@prj_no", NpgsqlDbType.Text);
                    var prg_no = cmd.Parameters.Add("@prg_no", NpgsqlDbType.Text);
                    var prg_dt = cmd.Parameters.Add("@prg_dt", NpgsqlDbType.Timestamp);
                    var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.Timestamp);
                    var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.Timestamp);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_itm_ds.Value = task.Description;
                    tsk_itm_inf.Value = task.MoreInformation ?? (object)DBNull.Value;
                    prj_no.Value = task.LinkProjectNumber ?? (object)DBNull.Value;
                    prg_no.Value = task.LinkProgramCode ?? (object)DBNull.Value;
                    prg_dt.Value = task.LinkProgramDate ?? (object)DBNull.Value;
                    exp_start_dt.Value = task.ExpectedStartTime ?? (object)DBNull.Value;
                    exp_due_dt.Value = task.ExpectedDueTime ?? (object)DBNull.Value;
                    mod_dt.Value = task.LastModifiedTime ?? DateTime.Now;
                    mod_by.Value = task.LastModifiedBy ?? (object)DBNull.Value;
                    tsk_itm_id.Value = task.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteTaskItemAsync(long taskItemId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wsp_wki_hst WHERE tsk_itm_id = @tsk_itm_id; ");
            sb.Append("DELETE FROM public.wsp_wki_nts WHERE tsk_id = @tsk_itm_id; ");
            sb.Append("DELETE FROM public.wsp_eval_rtns WHERE (tsk_itm_id = @tsk_itm_id); ");
            sb.Append("DELETE FROM public.wsp_eval_dtl WHERE (tsk_itm_id = @tsk_itm_id); ");
            sb.Append("DELETE FROM public.wsp_tsk_tml WHERE (tsk_itm_id = @tsk_itm_id); ");
            sb.Append("DELETE FROM public.wsp_tsk_delg WHERE (tsk_itm_id = @tsk_itm_id); ");
            sb.Append("DELETE FROM public.wsp_tsk_itms WHERE (tsk_itm_id = @tsk_itm_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        */
        #endregion

        #endregion

        #region Assignment Settings

        #region Assignment EventType

        //===== Write Action Methods =====//
        public async Task<int> AddAssignmentEventTypeAsync(AssignmentEventType eventType)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_evnt_typs(evnt_typ_ds) VALUES (@evnt_typ_ds) ");
            sb.Append("RETURNING evnt_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_ds = cmd.Parameters.Add("@evnt_typ_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    evnt_typ_ds.Value = eventType.Description;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentEventTypeAsync(AssignmentEventType eventType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_evnt_typs SET evnt_typ_ds=@evnt_typ_ds ");
            sb.Append("WHERE (evnt_typ_id=@evnt_typ_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_ds = cmd.Parameters.Add("@evnt_typ_ds", NpgsqlDbType.Text);
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    evnt_typ_ds.Value = eventType.Description;
                    evnt_typ_id.Value = eventType.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentEventTypeAsync(int assignmentEventTypeId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_evnt_typs WHERE (evnt_typ_id=@evnt_typ_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    evnt_typ_id.Value = assignmentEventTypeId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        //===== Read Action Methods ====//
        public async Task<AssignmentEventType> GetAssignmentEventTypeByIdAsync(int assignmentEventTypeId)
        {
            AssignmentEventType assignmentEventType = new AssignmentEventType();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT evnt_typ_id, evnt_typ_ds ");
            sb.Append("FROM public.ats_evnt_typs ");
            sb.Append("WHERE (evnt_typ_id=@evnt_typ_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    evnt_typ_id.Value = assignmentEventTypeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEventType.Id = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"];
                            assignmentEventType.Description = reader["evnt_typ_ds"] == DBNull.Value ? "" : reader["evnt_typ_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEventType;
        }
        public async Task<List<AssignmentEventType>> GetAssignmentEventTypesByDescriptionAsync(string description)
        {
            List<AssignmentEventType> assignmentEventTypeList = new List<AssignmentEventType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT evnt_typ_id, evnt_typ_ds ");
            sb.Append("FROM public.ats_evnt_typs ");
            sb.Append("WHERE LOWER(evnt_typ_ds) = LOWER(@evnt_typ_ds) ");
            sb.Append("ORDER BY evnt_typ_ds;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_ds = cmd.Parameters.Add("@evnt_typ_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    evnt_typ_ds.Value = description;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEventTypeList.Add(new AssignmentEventType
                            {
                                Id = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                Description = reader["evnt_typ_ds"] == DBNull.Value ? "" : reader["evnt_typ_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEventTypeList;
        }




        public async Task<List<AssignmentEventType>> GetAssignmentEventTypesAsync()
        {
            List<AssignmentEventType> assignmentEventTypeList = new List<AssignmentEventType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT evnt_typ_id, evnt_typ_ds ");
            sb.Append("FROM public.ats_evnt_typs ");
            sb.Append("ORDER BY evnt_typ_ds;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEventTypeList.Add(new AssignmentEventType
                            {
                                Id = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                Description = reader["evnt_typ_ds"] == DBNull.Value ? "" : reader["evnt_typ_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEventTypeList;
        }
        #endregion

        #region Assignment Role

        //===== Write Action Methods =====//
        public async Task<int> AddAssignmentRoleAsync(AssignmentRole role)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_rls(assg_rls_ds) ");
            sb.Append("VALUES (@assg_rls_ds) RETURNING assg_rls_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_ds = cmd.Parameters.Add("@assg_rls_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assg_rls_ds.Value = role.Description;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentEventTypeAsync(AssignmentRole role)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_rls SET assg_rls_ds=@assg_rls_ds ");
            sb.Append("WHERE (assg_rls_id=@assg_rls_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_ds = cmd.Parameters.Add("@assg_rls_ds", NpgsqlDbType.Text);
                    var assg_rls_id = cmd.Parameters.Add("@assg_rls_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    assg_rls_ds.Value = role.Description;
                    assg_rls_id.Value = role.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentRoleAsync(int assignmentRoleId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_rls WHERE (assg_rls_id=@assg_rls_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_id = cmd.Parameters.Add("@assg_rls_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    assg_rls_id.Value = assignmentRoleId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        //===== Read Action Methods ====//
        public async Task<AssignmentRole> GetAssignmentRoleByIdAsync(int assignmentRoleId)
        {
            AssignmentRole assignmentRole = new AssignmentRole();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_rls_id, assg_rls_ds ");
            sb.Append("FROM public.ats_assg_rls ");
            sb.Append("WHERE (assg_rls_id=@assg_rls_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_id = cmd.Parameters.Add("@assg_rls_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assg_rls_id.Value = assignmentRoleId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentRole.Id = reader["assg_rls_id"] == DBNull.Value ? 0 : (int)reader["assg_rls_id"];
                            assignmentRole.Description = reader["assg_rls_ds"] == DBNull.Value ? "" : reader["assg_rls_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentRole;
        }
        public async Task<List<AssignmentRole>> GetAssignmentRolesByDescriptionAsync(string assignmentRoleDescription)
        {
            List<AssignmentRole> assignmentRoleList = new List<AssignmentRole>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_rls_id, assg_rls_ds ");
            sb.Append("FROM public.ats_assg_rls ");
            sb.Append("WHERE (assg_rls_ds=@assg_rls_ds) ");
            sb.Append("ORDER BY assg_rls_ds;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_ds = cmd.Parameters.Add("@assg_rls_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assg_rls_ds.Value = assignmentRoleDescription;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentRoleList.Add(new AssignmentRole
                            {
                                Id = reader["assg_rls_id"] == DBNull.Value ? 0 : (int)reader["assg_rls_id"],
                                Description = reader["assg_rls_ds"] == DBNull.Value ? "" : reader["assg_rls_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentRoleList;
        }

        public async Task<List<AssignmentRole>> GetAssignmentRolesAsync()
        {
            List<AssignmentRole> assignmentRoleList = new List<AssignmentRole>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_rls_id, assg_rls_ds ");
            sb.Append("FROM public.ats_assg_rls ");
            sb.Append("ORDER BY assg_rls_ds;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentRoleList.Add(new AssignmentRole
                            {
                                Id = reader["assg_rls_id"] == DBNull.Value ? 0 : (int)reader["assg_rls_id"],
                                Description = reader["assg_rls_ds"] == DBNull.Value ? "" : reader["assg_rls_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentRoleList;
        }

        #endregion

        #region Assignment Note Action Methods
        public async Task<List<AssignmentNote>> GetAssignmentNotesByAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentNote> assignmentNotes = new List<AssignmentNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT nts_id, nts_tm, nts_ds, nts_by, ");
            sb.Append("assgn_id, is_ccl, ccl_by, ccl_dt ");
            sb.Append("FROM public.ats_assg_nts ");
            sb.Append("WHERE (assgn_id = @assgn_id) ");
            sb.Append("ORDER BY nts_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentNotes.Add(new AssignmentNote
                            {
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0 : (long)reader["assgn_id"],
                                NoteId = reader["nts_id"] == DBNull.Value ? 0 : (long)reader["nts_id"],
                                NoteTime = reader["nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["nts_tm"],
                                NoteContent = reader["nts_ds"] == DBNull.Value ? string.Empty : reader["nts_ds"].ToString(),
                                NoteWrittenBy = reader["nts_by"] == DBNull.Value ? string.Empty : reader["nts_by"].ToString(),
                                IsCancelled = reader["is_ccl"] == DBNull.Value ? false : (bool)reader["is_ccl"],
                                CancelledBy = reader["ccl_by"] == DBNull.Value ? string.Empty : reader["ccl_by"].ToString(),
                                CancelledOn = reader["ccl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ccl_dt"],
                            });
                        }
                }
            }
            return assignmentNotes;
        }
        public async Task<bool> AddNoteAsync(AssignmentNote n)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_nts( nts_tm, ");
            sb.Append("nts_ds, nts_by, assgn_id) ");
            sb.Append("VALUES (@nts_tm, @nts_ds, @nts_by, @assgn_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_tm = cmd.Parameters.Add("@nts_tm", NpgsqlDbType.Timestamp);
                    var nts_ds = cmd.Parameters.Add("@nts_ds", NpgsqlDbType.Text);
                    var nts_by = cmd.Parameters.Add("@nts_by", NpgsqlDbType.Text);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    nts_tm.Value = n.NoteTime;
                    nts_ds.Value = n.NoteContent;
                    nts_by.Value = n.NoteWrittenBy ?? (object)DBNull.Value;
                    assgn_id.Value = n.AssignmentId ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> CancelAssignmentNoteAsync(long assignmentNoteId, string cancelledBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_nts SET is_ccl=true, ccl_dt=@ccl_dt, ");
            sb.Append("ccl_by=@ccl_by WHERE (nts_id=@nts_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    var ccl_dt = cmd.Parameters.Add("@ccl_dt", NpgsqlDbType.Timestamp);
                    var ccl_by = cmd.Parameters.Add("@ccl_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    nts_id.Value = assignmentNoteId;
                    ccl_dt.Value = DateTime.Now;
                    ccl_by.Value = cancelledBy;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentNoteAsync(long assignmentNoteId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_nts WHERE (nts_id=@nts_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    nts_id.Value = assignmentNoteId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #region Assignment History Action Methods
        public async Task<List<AssignmentHistory>> GetAssignmentHistoryByAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentHistory> assignmentHistory = new List<AssignmentHistory>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_hst_id, assg_hst_tm, assg_hst_ds, ");
            sb.Append("assg_hst_by, assgn_id  FROM public.ats_assg_hst ");
            sb.Append("WHERE (assgn_id = @assgn_id) ");
            sb.Append("ORDER BY assg_hst_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentHistory.Add(new AssignmentHistory
                            {
                                Id = reader["assg_hst_id"] == DBNull.Value ? 0L : (long)reader["assg_hst_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                ActivityTime = reader["assg_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assg_hst_tm"],
                                ActivityDescription = reader["assg_hst_ds"] == DBNull.Value ? string.Empty : reader["assg_hst_ds"].ToString(),
                                ActivityBy = reader["assg_hst_by"] == DBNull.Value ? string.Empty : reader["assg_hst_by"].ToString(),
                            });
                        }
                }
            }
            return assignmentHistory;
        }
        public async Task<bool> AddAssignmentHistoryAsync(AssignmentHistory assignmentHistory)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_hst(assg_hst_tm, ");
            sb.Append("assg_hst_ds, assg_hst_by, assgn_id) VALUES ");
            sb.Append("(@assg_hst_tm, @assg_hst_ds, @assg_hst_by, @assgn_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_hst_tm = cmd.Parameters.Add("@assg_hst_tm", NpgsqlDbType.Timestamp);
                    var assg_hst_ds = cmd.Parameters.Add("@assg_hst_ds", NpgsqlDbType.Text);
                    var assg_hst_by = cmd.Parameters.Add("@assg_hst_by", NpgsqlDbType.Text);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assg_hst_tm.Value = assignmentHistory.ActivityTime;
                    assg_hst_ds.Value = assignmentHistory.ActivityDescription;
                    assg_hst_by.Value = assignmentHistory.ActivityBy; 
                    assgn_id.Value = assignmentHistory.AssignmentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentHistoryAsync(long assignmentHistoryId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_hst WHERE (assg_hst_id = @assg_hst_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_hst_id = cmd.Parameters.Add("@assg_hst_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assg_hst_id.Value = assignmentHistoryId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #endregion

    }
}
