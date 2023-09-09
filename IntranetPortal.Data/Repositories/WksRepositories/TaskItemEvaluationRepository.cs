using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Repositories.WksRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.WksRepositories
{
    public class TaskItemEvaluationRepository: ITaskItemEvaluationRepository
    {
        public IConfiguration _config { get; }
        public TaskItemEvaluationRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<TaskItemEvaluation>> GetByTaskListIdAndEvaluatorIdAsync(int taskListId, string evaluatorId)
        {
            List<TaskItemEvaluation> taskItemEvaluations = new List<TaskItemEvaluation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.tsk_itm_id, i.tsk_itm_no, i.tsk_itm_ds, i.tsk_lst_id, ");
            sb.Append("i.mst_tsk_id, i.prj_no, i.prg_no, i.prg_dt, i.tsk_owner_id, ");
            sb.Append("i.assgnd_emp_id, i.assigned_dt, i.tsk_itm_stts, i.prgs_stts, ");
            sb.Append("i.apprv_stts, i.approved_dt, i.approved_by, i.exp_start_dt, ");
            sb.Append("i.act_start_dt, i.exp_due_dt, i.act_due_dt, i.deliverables, ");
            sb.Append("i.is_cancelled, i.cancelled_dt, i.cancelled_by, i.unit_id, ");
            sb.Append("i.dept_id, i.loc_id, i.completion_is_confirmed, l.tsk_lst_nm, ");
            sb.Append("i.completion_confirmed_by, i.completion_confirmed_on, ");
            sb.Append("i.mod_by, i.mod_dt, i.crt_by, i.crt_dt, p1.fullname as tsk_owner_nm, ");
            sb.Append("p2.fullname as assgnd_emp_nm, case i.prgs_stts ");
            sb.Append("when 0 then 'Not Started' when 1 then 'In Progress' ");
            sb.Append("when 2 then 'Completed' when 3 then 'On Hold' end prgs_stts_ds, ");
            sb.Append("case i.tsk_itm_stts when 0 then 'Open' when 1 then 'Closed' ");
            sb.Append("end tsk_itm_stts_ds, case i.apprv_stts when 0 then 'Pending' ");
            sb.Append("when 1 then 'Approved' when 2 then 'Declined' end apprv_stts_ds, ");
            sb.Append("t.tsk_itm_ds as mst_tsk_ds, d.eval_dtl_id, d.eval_hdr_id, ");
            sb.Append("d.eval_dt, d.percent_completion, d.quality_rating, d.eval_comments, ");
            sb.Append("h.eval_emp_id FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_eval_dtl d ON d.tsk_itm_id = i.tsk_itm_id ");
            sb.Append("LEFT JOIN public.wkm_eval_hdr h ON (h.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("AND LOWER(h.eval_emp_id) = LOWER(@eval_emp_id)) ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                tsk_lst_id.Value = taskListId;
                eval_emp_id.Value = evaluatorId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskItemEvaluations.Add(new TaskItemEvaluation
                        {
                            Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                            Number = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString(),
                            Description = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString(),
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? (long?)null : (long)reader["mst_tsk_id"],
                            MasterTaskDescription = reader["mst_tsk_ds"] == DBNull.Value ? string.Empty : reader["mst_tsk_ds"].ToString(),
                            LinkProjectNumber = reader["prj_no"] == DBNull.Value ? string.Empty : reader["prj_no"].ToString(),
                            LinkProgramCode = reader["prg_no"] == DBNull.Value ? string.Empty : reader["prg_no"].ToString(),
                            LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                            TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                            TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                            AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? string.Empty : reader["assgnd_emp_id"].ToString(),
                            AssignedToName = reader["assgnd_emp_nm"] == DBNull.Value ? string.Empty : reader["assgnd_emp_nm"].ToString(),
                            AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                            TaskStatus = reader["tsk_itm_stts"] == DBNull.Value ? WorkItemStatus.Open : (WorkItemStatus)reader["tsk_itm_stts"],
                            TaskStatusDescription = reader["tsk_itm_stts_ds"] == DBNull.Value ? "Open" : reader["tsk_itm_stts_ds"].ToString(),
                            ProgressStatus = reader["prgs_stts"] == DBNull.Value ? WorkItemProgressStatus.NotStarted : (WorkItemProgressStatus)reader["prgs_stts"],
                            ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? "Not Started" : reader["prgs_stts_ds"].ToString(),
                            TaskApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                            ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? "Pending" : reader["apprv_stts_ds"].ToString(),
                            TimeApproved = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                            ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                            ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                            ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                            ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                            ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],
                            Deliverable = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),
                            IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                            TimeCancelled = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],
                            CancelledBy = reader["cancelled_by"] == DBNull.Value ? string.Empty : reader["cancelled_by"].ToString(),
                            UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                            DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                            LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                            CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                            CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? string.Empty : reader["completion_confirmed_by"].ToString(),
                            CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                            LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                            LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                            CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                            CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString(),

                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            EvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                            EvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            PercentageCompletion = reader["percent_completion"] == DBNull.Value ? 0 : (int)reader["percent_completion"],
                            QualityRating = reader["quality_rating"] == DBNull.Value ? 0 : (int)reader["quality_rating"],
                            EvaluatorComments = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return taskItemEvaluations;
        }


    }
}
