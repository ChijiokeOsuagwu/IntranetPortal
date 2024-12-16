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
    public class TaskItemRepository : ITaskItemRepository
    {
        public IConfiguration _config { get; }
        public TaskItemRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region TaskItem Write Actions
        public async Task<bool> AddAsync(TaskItem taskItem)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_tsk_itms(tsk_itm_no, tsk_itm_ds, ");
            sb.Append("tsk_lst_id, mst_tsk_id, tsk_owner_id, assgnd_emp_id, ");
            sb.Append("assigned_dt, tsk_itm_stts, prgs_stts, apprv_stts, ");
            sb.Append("approved_dt, approved_by, exp_start_dt, act_start_dt, ");
            sb.Append("exp_due_dt, act_due_dt, deliverables, unit_id, dept_id, ");
            sb.Append("loc_id, mod_by, mod_dt, crt_by, crt_dt) ");
            sb.Append("VALUES (@tsk_itm_no, @tsk_itm_ds, @tsk_lst_id, ");
            sb.Append("@mst_tsk_id, @tsk_owner_id, @assgnd_emp_id, @assigned_dt, ");
            sb.Append("@tsk_itm_stts, @prgs_stts, @apprv_stts, @approved_dt,  ");
            sb.Append("@approved_by, @exp_start_dt, @act_start_dt, @exp_due_dt, ");
            sb.Append("@act_due_dt, @deliverables, @unit_id, @dept_id, @loc_id, ");
            sb.Append("@crt_by, @crt_dt, @crt_by, @crt_dt);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_no = cmd.Parameters.Add("@tsk_itm_no", NpgsqlDbType.Text);
                    var tsk_itm_ds = cmd.Parameters.Add("@tsk_itm_ds", NpgsqlDbType.Text);
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var assgnd_emp_id = cmd.Parameters.Add("@assgnd_emp_id", NpgsqlDbType.Text);
                    var assigned_dt = cmd.Parameters.Add("@assigned_dt", NpgsqlDbType.TimestampTz);
                    var mst_tsk_id = cmd.Parameters.Add("@mst_tsk_id", NpgsqlDbType.Bigint);
                    var tsk_itm_stts = cmd.Parameters.Add("@tsk_itm_stts", NpgsqlDbType.Integer);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    var apprv_stts = cmd.Parameters.Add("@apprv_stts", NpgsqlDbType.Integer);
                    var approved_dt = cmd.Parameters.Add("@approved_dt", NpgsqlDbType.TimestampTz);
                    var approved_by = cmd.Parameters.Add("@approved_by", NpgsqlDbType.Text);
                    var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.TimestampTz);
                    var act_start_dt = cmd.Parameters.Add("@act_start_dt", NpgsqlDbType.TimestampTz);
                    var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.TimestampTz);
                    var act_due_dt = cmd.Parameters.Add("@act_due_dt", NpgsqlDbType.TimestampTz);
                    var deliverables = cmd.Parameters.Add("@deliverables", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var crt_by = cmd.Parameters.Add("@crt_by", NpgsqlDbType.Text);
                    var crt_dt = cmd.Parameters.Add("@crt_dt", NpgsqlDbType.Text);

                    cmd.Prepare();

                    tsk_itm_no.Value = taskItem.Number;
                    tsk_itm_ds.Value = taskItem.Description;
                    tsk_lst_id.Value = taskItem.TaskListId;
                    tsk_owner_id.Value = taskItem.TaskOwnerId;
                    assgnd_emp_id.Value = taskItem.AssignedToId;
                    assigned_dt.Value = taskItem.AssignedTime ?? DateTime.UtcNow;
                    mst_tsk_id.Value = taskItem.MasterTaskId ?? (object)DBNull.Value;
                    tsk_itm_stts.Value = (int)taskItem.TaskStatus;
                    prgs_stts.Value = (int)taskItem.ProgressStatus;
                    apprv_stts.Value = (int)taskItem.TaskApprovalStatus;
                    approved_dt.Value = taskItem.TimeApproved ?? (object)DBNull.Value;
                    approved_by.Value = taskItem.ApprovedBy ?? (object)DBNull.Value;
                    exp_start_dt.Value = taskItem.ExpectedStartTime ?? DateTime.UtcNow;
                    act_start_dt.Value = taskItem.ActualStartTime ?? (object)DBNull.Value;
                    exp_due_dt.Value = taskItem.ExpectedDueTime ?? (object)DBNull.Value;
                    act_due_dt.Value = taskItem.ActualDueTime ?? (object)DBNull.Value;
                    deliverables.Value = taskItem.Deliverable ?? (object)DBNull.Value;
                    unit_id.Value = taskItem.UnitId ?? (object)DBNull.Value;
                    dept_id.Value = taskItem.DepartmentId ?? (object)DBNull.Value;
                    loc_id.Value = taskItem.LocationId ?? (object)DBNull.Value;
                    crt_by.Value = taskItem.CreatedBy ?? (object)DBNull.Value;
                    crt_dt.Value = taskItem.CreatedTime ?? $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()} GMT";

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(TaskItem taskItem)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET tsk_itm_ds=@tsk_itm_ds, ");
            sb.Append("deliverables=@deliverables, exp_start_dt=@exp_start_dt, ");
            sb.Append("exp_due_dt=@exp_due_dt, mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_itm_ds = cmd.Parameters.Add("@tsk_itm_ds", NpgsqlDbType.Text);
                var deliverables = cmd.Parameters.Add("@deliverables", NpgsqlDbType.Text);
                var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.TimestampTz);
                var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.TimestampTz);
                var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                cmd.Prepare();
                tsk_itm_ds.Value = taskItem.Description;
                deliverables.Value = taskItem.Deliverable ?? (object)DBNull.Value;
                exp_due_dt.Value = taskItem.ExpectedDueTime;
                exp_start_dt.Value = taskItem.ExpectedStartTime;
                mod_by.Value = taskItem.LastModifiedBy ?? (object)DBNull.Value;
                mod_dt.Value = taskItem.LastModifiedTime ?? $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()} GMT";
                tsk_itm_id.Value = taskItem.Id;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateProgressStatusAsync(long taskItemId, int newProgressStatus, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET prgs_stts=@prgs_stts, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);

                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    prgs_stts.Value = newProgressStatus;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateTaskStatusAsync(long taskItemId, int newTaskStatus, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET tsk_itm_stts=@tsk_itm_stts, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var tsk_itm_stts = cmd.Parameters.Add("@tsk_itm_stts", NpgsqlDbType.Integer);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);

                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    tsk_itm_stts.Value = newTaskStatus;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateTaskStatusByTaskListIdAsync(int taskListId, int newTaskStatus, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET tsk_itm_stts=@tsk_itm_stts, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_lst_id = @tsk_lst_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var tsk_itm_stts = cmd.Parameters.Add("@tsk_itm_stts", NpgsqlDbType.Integer);
                var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);

                cmd.Prepare();
                tsk_lst_id.Value = taskListId;
                tsk_itm_stts.Value = newTaskStatus;
                mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string approvedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET apprv_stts=@apprv_stts, ");
            sb.Append("approved_dt=@approved_dt, approved_by=@approved_by, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var apprv_stts = cmd.Parameters.Add("@apprv_stts", NpgsqlDbType.Integer);
                    var approved_dt = cmd.Parameters.Add("@approved_dt", NpgsqlDbType.TimestampTz);
                    var approved_by = cmd.Parameters.Add("@approved_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);

                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    apprv_stts.Value = (int)newApprovalStatus;
                    approved_dt.Value = DateTime.UtcNow;
                    approved_by.Value = approvedBy;
                    mod_by.Value = approvedBy ?? (object)DBNull.Value;
                    mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateTimelineAsync(long taskItemId, string modifiedBy, DateTime? previousStartDate, DateTime? newStartDate, DateTime? previousEndDate, DateTime? newEndDate)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET exp_start_dt=@exp_start_dt, ");
            sb.Append("exp_due_dt=@exp_due_dt, mod_by=@mod_by, mod_dt=@mod_dt, ");
            sb.Append("apprv_stts=@apprv_stts, approved_dt=@approved_dt, ");
            sb.Append("approved_by=@approved_by WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.TimestampTz);
                var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.TimestampTz);
                var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                var apprv_stts = cmd.Parameters.Add("@apprv_stts", NpgsqlDbType.Integer);
                var approved_dt = cmd.Parameters.Add("@approved_dt", NpgsqlDbType.TimestampTz);
                var approved_by = cmd.Parameters.Add("@approved_by", NpgsqlDbType.Text);
                cmd.Prepare();
                tsk_itm_id.Value = taskItemId;
                exp_start_dt.Value = newStartDate ?? previousStartDate;
                exp_due_dt.Value = newEndDate ?? previousEndDate;
                mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";
                apprv_stts.Value = 0;
                approved_dt.Value = DBNull.Value;
                approved_by.Value = DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateTaskCancelAsync(long taskItemId, string modifiedBy, bool cancelTask)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET is_cancelled=@is_cancelled, ");
            sb.Append("cancelled_dt=@cancelled_dt, cancelled_by=@cancelled_by, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var is_cancelled = cmd.Parameters.Add("@is_cancelled", NpgsqlDbType.Boolean);
                    var cancelled_dt = cmd.Parameters.Add("@cancelled_dt", NpgsqlDbType.TimestampTz);
                    var cancelled_by = cmd.Parameters.Add("@cancelled_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    is_cancelled.Value = cancelTask;
                    if (cancelTask)
                    {
                        cancelled_dt.Value = DateTime.UtcNow;
                        cancelled_by.Value = modifiedBy ?? (object)DBNull.Value;
                    }
                    else
                    {
                        cancelled_dt.Value = (object)DBNull.Value;
                        cancelled_by.Value = (object)DBNull.Value;
                    }

                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long taskItemId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wkm_tsk_itms ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateProgramLinkAsync(long taskItemId, string programCode, DateTime? programDate, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET prg_no=@prg_no, ");
            sb.Append("prg_dt=@prg_dt, mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                var prg_no = cmd.Parameters.Add("@prg_no", NpgsqlDbType.Text);
                var prg_dt = cmd.Parameters.Add("@prg_dt", NpgsqlDbType.TimestampTz);
                var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                cmd.Prepare();
                tsk_itm_id.Value = taskItemId;
                prg_no.Value = programCode;
                prg_dt.Value = programDate;
                mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateProjectLinkAsync(long taskItemId, string projectCode, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET prj_no=@prj_no, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                var prj_no = cmd.Parameters.Add("@prj_no", NpgsqlDbType.Text);
                var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                cmd.Prepare();
                tsk_itm_id.Value = taskItemId;
                prj_no.Value = projectCode;
                mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateTaskOwnerAsync(long taskItemId, int newTaskListId, string newTaskOwnerId, int newUnitId, int newDeptId, int newLocationId, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_itms SET tsk_lst_id=@tsk_lst_id, ");
            sb.Append("tsk_owner_id=@tsk_owner_id, assgnd_emp_id=@tsk_owner_id, ");
            sb.Append("unit_id=@unit_id, dept_id=@dept_id, loc_id=@loc_id, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                cmd.Prepare();
                tsk_itm_id.Value = taskItemId;
                tsk_lst_id.Value = newTaskListId;
                tsk_owner_id.Value = newTaskOwnerId;
                unit_id.Value = newUnitId;
                dept_id.Value = newDeptId;
                loc_id.Value = newLocationId;
                mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        #region TaskItems Read Actions
        public async Task<TaskItem> GetByIdAsync(long taskItemId)
        {
            TaskItem taskItem = new TaskItem();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_itm_id = @tsk_itm_id); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tsk_itm_id.Value = taskItemId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItem.Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"];
                            taskItem.Number = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString();
                            taskItem.Description = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString();
                            taskItem.TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"];
                            taskItem.TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString();
                            taskItem.MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? (long?)null : (long)reader["mst_tsk_id"];
                            taskItem.MasterTaskDescription = reader["mst_tsk_ds"] == DBNull.Value ? string.Empty : reader["mst_tsk_ds"].ToString();
                            taskItem.LinkProjectNumber = reader["prj_no"] == DBNull.Value ? string.Empty : reader["prj_no"].ToString();
                            taskItem.LinkProgramCode = reader["prg_no"] == DBNull.Value ? string.Empty : reader["prg_no"].ToString();
                            taskItem.LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"];
                            taskItem.TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString();
                            taskItem.TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString();
                            taskItem.AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? string.Empty : reader["assgnd_emp_id"].ToString();
                            taskItem.AssignedToName = reader["assgnd_emp_nm"] == DBNull.Value ? string.Empty : reader["assgnd_emp_nm"].ToString();
                            taskItem.AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"];
                            taskItem.TaskStatus = reader["tsk_itm_stts"] == DBNull.Value ? WorkItemStatus.Open : (WorkItemStatus)reader["tsk_itm_stts"];
                            taskItem.TaskStatusDescription = reader["tsk_itm_stts_ds"] == DBNull.Value ? "Open" : reader["tsk_itm_stts_ds"].ToString();
                            taskItem.ProgressStatus = reader["prgs_stts"] == DBNull.Value ? WorkItemProgressStatus.NotStarted : (WorkItemProgressStatus)reader["prgs_stts"];
                            taskItem.ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? "Not Started" : reader["prgs_stts_ds"].ToString();
                            taskItem.TaskApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"];
                            taskItem.ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? "Pending" : reader["apprv_stts_ds"].ToString();
                            taskItem.TimeApproved = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"];
                            taskItem.ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString();
                            taskItem.ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"];
                            taskItem.ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"];
                            taskItem.ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"];
                            taskItem.ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"];
                            taskItem.Deliverable = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString();
                            taskItem.IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"];
                            taskItem.TimeCancelled = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"];
                            taskItem.CancelledBy = reader["cancelled_by"] == DBNull.Value ? string.Empty : reader["cancelled_by"].ToString();
                            taskItem.UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"];
                            taskItem.DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"];
                            taskItem.LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"];
                            taskItem.CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"];
                            taskItem.CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? string.Empty : reader["completion_confirmed_by"].ToString();
                            taskItem.CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"];
                            taskItem.LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString();
                            taskItem.LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString();
                            taskItem.CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString();
                            taskItem.CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return taskItem;
        }

        public async Task<TaskItem> GetByNumberAsync(string taskItemNumber)
        {
            TaskItem taskItem = new TaskItem();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_itm_no = @tsk_itm_no); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_no = cmd.Parameters.Add("@tsk_itm_no", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    tsk_itm_no.Value = taskItemNumber;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItem.Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"];
                            taskItem.Number = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString();
                            taskItem.Description = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString();
                            taskItem.TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"];
                            taskItem.TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString();
                            taskItem.MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? (long?)null : (long)reader["mst_tsk_id"];
                            taskItem.MasterTaskDescription = reader["mst_tsk_ds"] == DBNull.Value ? string.Empty : reader["mst_tsk_ds"].ToString();
                            taskItem.LinkProjectNumber = reader["prj_no"] == DBNull.Value ? string.Empty : reader["prj_no"].ToString();
                            taskItem.LinkProgramCode = reader["prg_no"] == DBNull.Value ? string.Empty : reader["prg_no"].ToString();
                            taskItem.LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"];
                            taskItem.TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString();
                            taskItem.TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString();
                            taskItem.AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? string.Empty : reader["assgnd_emp_id"].ToString();
                            taskItem.AssignedToName = reader["assgnd_emp_nm"] == DBNull.Value ? string.Empty : reader["assgnd_emp_nm"].ToString();
                            taskItem.AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"];
                            taskItem.TaskStatus = reader["tsk_itm_stts"] == DBNull.Value ? WorkItemStatus.Open : (WorkItemStatus)reader["tsk_itm_stts"];
                            taskItem.TaskStatusDescription = reader["tsk_itm_stts_ds"] == DBNull.Value ? "Open" : reader["tsk_itm_stts_ds"].ToString();
                            taskItem.ProgressStatus = reader["prgs_stts"] == DBNull.Value ? WorkItemProgressStatus.NotStarted : (WorkItemProgressStatus)reader["prgs_stts"];
                            taskItem.ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? "Not Started" : reader["prgs_stts_ds"].ToString();
                            taskItem.TaskApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"];
                            taskItem.ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? "Pending" : reader["apprv_stts_ds"].ToString();
                            taskItem.TimeApproved = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"];
                            taskItem.ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString();
                            taskItem.ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"];
                            taskItem.ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"];
                            taskItem.ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"];
                            taskItem.ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"];
                            taskItem.Deliverable = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString();
                            taskItem.IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"];
                            taskItem.TimeCancelled = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"];
                            taskItem.CancelledBy = reader["cancelled_by"] == DBNull.Value ? string.Empty : reader["cancelled_by"].ToString();
                            taskItem.UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"];
                            taskItem.DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"];
                            taskItem.LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"];
                            taskItem.CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"];
                            taskItem.CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? string.Empty : reader["completion_confirmed_by"].ToString();
                            taskItem.CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"];
                            taskItem.LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString();
                            taskItem.LastModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString();
                            taskItem.CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString();
                            taskItem.CreatedTime = reader["crt_dt"] == DBNull.Value ? string.Empty : reader["crt_dt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return taskItem;
        }

        public async Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE LOWER(i.tsk_owner_id) = LOWER(@emp_id) ");
            sb.Append("OR LOWER(i.assgnd_emp_id) = LOWER(@emp_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = empId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, WorkItemStatus taskStatus)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (LOWER(i.tsk_owner_id) = LOWER(@emp_id) ");
            sb.Append("OR LOWER(i.assgnd_emp_id) = LOWER(@emp_id)) ");
            sb.Append("AND (i.tsk_itm_stts = @tsk_itm_stts);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var tsk_itm_stts = cmd.Parameters.Add("@tsk_itm_stts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = empId;
                    tsk_itm_stts.Value = (int)taskStatus;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, int taskListId)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND (LOWER(i.tsk_owner_id) = LOWER(@emp_id) ");
            sb.Append("OR LOWER(i.assgnd_emp_id) = LOWER(@emp_id));");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = empId;
                    tsk_lst_id.Value = taskListId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, int taskListId, WorkItemStatus taskStatus)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND (LOWER(i.tsk_owner_id) = LOWER(@emp_id) ");
            sb.Append("OR LOWER(i.assgnd_emp_id) = LOWER(@emp_id)) ");
            sb.Append("AND (i.tsk_itm_stts = @tsk_itm_stts);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var tsk_itm_stts = cmd.Parameters.Add("@tsk_itm_stts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = empId;
                    tsk_lst_id.Value = taskListId;
                    tsk_itm_stts.Value = (int)taskStatus;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByOwnerIdOrAssignedToIdAsync(string empId, int taskListId, WorkItemStatus taskStatus, WorkItemProgressStatus progressStatus)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND (LOWER(i.tsk_owner_id) = LOWER(@emp_id) ");
            sb.Append("OR LOWER(i.assgnd_emp_id) = LOWER(@emp_id)) ");
            sb.Append("AND (i.tsk_itm_stts = @tsk_itm_stts) ");
            sb.Append("AND (i.prgs_stts = @prgs_stts);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var tsk_itm_stts = cmd.Parameters.Add("@tsk_itm_stts", NpgsqlDbType.Integer);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = empId;
                    tsk_lst_id.Value = taskListId;
                    tsk_itm_stts.Value = (int)taskStatus;
                    prgs_stts.Value = (int)progressStatus;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        //======================= Get By TaskListID ==============================================//

        public async Task<List<TaskItem>> GetByTaskListIdAsync(int taskListId)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            //sb.Append("AND (LOWER(i.tsk_owner_id) = LOWER(@emp_id) ");
            //sb.Append("OR LOWER(i.assgnd_emp_id) = LOWER(@emp_id));");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByTaskListIdAndDescriptionAsync(int taskListId, string taskDescription)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND LOWER(i.tsk_itm_ds) = LOWER(@tsk_ds) ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            //sb.Append("AND (LOWER(i.tsk_owner_id) = LOWER(@emp_id) ");
            //sb.Append("OR LOWER(i.assgnd_emp_id) = LOWER(@emp_id));");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var tsk_ds = cmd.Parameters.Add("@tsk_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    tsk_ds.Value = taskDescription;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByTaskListIdAndProgressStatusAsync(int taskListId, int progressStatus)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) AND (i.prgs_stts = @prgs_stts) ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    prgs_stts.Value = progressStatus;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByTaskListIdAndDueYearAsync(int taskListId, int dueYear)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND date_part('year', i.exp_due_dt) = @dt_yr ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    dt_yr.Value = dueYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByTaskListIdAndDueYearAndProgressStatusAsync(int taskListId, int dueYear, int progressStatus)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND (i.prgs_stts = @prgs_stts) ");
            sb.Append("AND (date_part('year', i.exp_due_dt) = @dt_yr) ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    dt_yr.Value = dueYear;
                    prgs_stts.Value = progressStatus;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByTaskListIdAndDueYearAndDueMonthAsync(int taskListId, int dueYear, int dueMonth)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND ((date_part('year', i.exp_due_dt) = @dt_yr) ");
            sb.Append("AND (date_part('month', i.exp_due_dt) = @dt_mn)) ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var dt_mn = cmd.Parameters.Add("@dt_mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    dt_yr.Value = dueYear;
                    dt_mn.Value = dueMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        public async Task<List<TaskItem>> GetByTaskListIdAndDueYearAndDueMonthAndProgressStatusAsync(int taskListId, int dueYear, int dueMonth, int progressStatus)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
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
            sb.Append("t.tsk_itm_ds as mst_tsk_ds FROM public.wkm_tsk_itms i ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = i.tsk_lst_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = i.tsk_owner_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = i.assgnd_emp_id ");
            sb.Append("LEFT JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = i.mst_tsk_id ");
            sb.Append("WHERE (i.tsk_lst_id = @tsk_lst_id) AND (i.prgs_stts = @prgs_stts) ");
            sb.Append("AND (date_part('year', i.exp_due_dt) = @dt_yr) ");
            sb.Append("AND (date_part('month', i.exp_due_dt) = @dt_mn) ");
            sb.Append("ORDER BY i.tsk_itm_id; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var dt_mn = cmd.Parameters.Add("@dt_mn", NpgsqlDbType.Integer);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    dt_yr.Value = dueYear;
                    dt_mn.Value = dueMonth;
                    prgs_stts.Value = progressStatus;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItems.Add(new TaskItem
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
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskItems;
        }

        #endregion

        #region Task Item Note Action Methods
        public async Task<IList<TaskNote>> GetNotesByTaskItemIdAsync(long taskItemId)
        {
            IList<TaskNote> taskNotes = new List<TaskNote>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ntt_nts_id, ntt_nts_tm, ntt_nts_ds, ");
            sb.Append("ntt_nts_by, tsk_id, tsk_lst_id, is_cnc, dt_cnc ");
            sb.Append("FROM public.gst_ntt_nts ");
            sb.Append("WHERE (tsk_id = @tsk_id ");
            sb.Append("AND tsk_id IS NOT NULL) ");
            sb.Append("ORDER BY ntt_nts_id DESC; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_id = cmd.Parameters.Add("@tsk_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tsk_id.Value = taskItemId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskNotes.Add(new TaskNote
                            {
                                TaskItemId = reader["tsk_id"] == DBNull.Value ? 0 : (long)reader["tsk_id"],
                                NoteId = reader["ntt_nts_id"] == DBNull.Value ? 0 : (long)reader["ntt_nts_id"],
                                NoteTime = reader["ntt_nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ntt_nts_tm"],
                                NoteDescription = reader["ntt_nts_ds"] == DBNull.Value ? string.Empty : reader["ntt_nts_ds"].ToString(),
                                NoteWrittenBy = reader["ntt_nts_by"] == DBNull.Value ? string.Empty : reader["ntt_nts_by"].ToString(),
                                IsCancelled = reader["is_cnc"] == DBNull.Value ? false : (bool)reader["is_cnc"],
                                CancelledOn = reader["dt_cnc"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_cnc"],
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskNotes;
        }
        public async Task<bool> AddNoteAsync(TaskNote taskNote)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_ntt_nts(ntt_nts_tm, ntt_nts_ds, ");
            sb.Append("ntt_nts_by, tsk_id, is_cnc, dt_cnc) ");
            sb.Append("VALUES (@ntt_nts_tm, @ntt_nts_ds, @ntt_nts_by, ");
            sb.Append("@tsk_id, @is_cnc, @dt_cnc); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_id = cmd.Parameters.Add("@tsk_id", NpgsqlDbType.Bigint);
                    var ntt_nts_tm = cmd.Parameters.Add("@ntt_nts_tm", NpgsqlDbType.TimestampTz);
                    var ntt_nts_ds = cmd.Parameters.Add("@ntt_nts_ds", NpgsqlDbType.Text);
                    var ntt_nts_by = cmd.Parameters.Add("@ntt_nts_by", NpgsqlDbType.Text);
                    var is_cnc = cmd.Parameters.Add("@is_cnc", NpgsqlDbType.Boolean);
                    var dt_cnc = cmd.Parameters.Add("@dt_cnc", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    tsk_id.Value = taskNote.TaskItemId;
                    ntt_nts_tm.Value = taskNote.NoteTime;
                    ntt_nts_ds.Value = taskNote.NoteDescription;
                    ntt_nts_by.Value = taskNote.NoteWrittenBy;
                    is_cnc.Value = taskNote.IsCancelled;
                    dt_cnc.Value = taskNote.CancelledOn ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateToIsCancelledAsync(long taskNoteId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_ntt_nts SET is_cnc=true, dt_cnc=@dt_cnc ");
            sb.Append("WHERE (ntt_nts_id = @ntt_nts_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var ntt_nts_id = cmd.Parameters.Add("@ntt_nts_id", NpgsqlDbType.Bigint);
                    var dt_cnc = cmd.Parameters.Add("@dt_cnc", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    ntt_nts_id.Value = taskNoteId;
                    dt_cnc.Value = DateTime.UtcNow;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion
    }
}
