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
    public class TaskSubmissionRepository : ITaskSubmissionRepository
    {
        public IConfiguration _config { get; }
        public TaskSubmissionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Task Submission Write Actions
        public async Task<bool> AddAsync(TaskSubmission taskSubmission)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_tsk_sbms(tsk_lst_id, ");
            sb.Append("frm_emp_id, to_emp_id, sbm_typ_id, sbm_dt, ");
            sb.Append("sbm_msg, is_xtn, dt_xtn) VALUES (@tsk_lst_id, ");
            sb.Append("@frm_emp_id, @to_emp_id, @sbm_typ_id, @sbm_dt, ");
            sb.Append("@sbm_msg, @is_xtn, @dt_xtn);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var frm_emp_id = cmd.Parameters.Add("@frm_emp_id", NpgsqlDbType.Text);
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var sbm_typ_id = cmd.Parameters.Add("@sbm_typ_id", NpgsqlDbType.Integer);
                var sbm_dt = cmd.Parameters.Add("@sbm_dt", NpgsqlDbType.TimestampTz);
                var sbm_msg = cmd.Parameters.Add("@sbm_msg", NpgsqlDbType.Text);
                var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);

                cmd.Prepare();

                tsk_lst_id.Value = taskSubmission.TaskListId;
                frm_emp_id.Value = taskSubmission.FromEmployeeId;
                to_emp_id.Value = taskSubmission.ToEmployeeId;
                sbm_typ_id.Value = (int)taskSubmission.SubmissionType;
                sbm_dt.Value = taskSubmission.DateSubmitted;
                sbm_msg.Value = taskSubmission.Comment ?? (object)DBNull.Value;
                is_xtn.Value = taskSubmission.IsActioned;
                dt_xtn.Value = taskSubmission.DateActioned ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(long taskSubmissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_sbms ");
            sb.Append("SET is_xtn=@is_xtn, dt_xtn=@dt_xtn ");
            sb.Append("WHERE wkm_tsk_sbm_id = @wkm_tsk_sbm_id; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wkm_tsk_sbm_id = cmd.Parameters.Add("@wkm_tsk_sbm_id", NpgsqlDbType.Bigint);
                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    wkm_tsk_sbm_id.Value = taskSubmissionId;
                    is_xtn.Value = true;
                    dt_xtn.Value = DateTime.UtcNow;

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

        public async Task<bool> DeleteAsync(long taskSubmissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wkm_tsk_sbms ");
            sb.Append("WHERE (wkm_tsk_sbm_id = @wkm_tsk_sbm_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wkm_tsk_sbm_id = cmd.Parameters.Add("@wkm_tsk_sbm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    wkm_tsk_sbm_id.Value = taskSubmissionId;

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

        #region Task Submission Read Action Methods

        public async Task<TaskSubmission> GetByIdAsync(long taskSubmissionId)
        {
            TaskSubmission taskSubmission = new TaskSubmission();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE s.wkm_tsk_sbm_id = @wkm_tsk_sbm_id; ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var wkm_tsk_sbm_id = cmd.Parameters.Add("@wkm_tsk_sbm_id", NpgsqlDbType.Bigint);
                await cmd.PrepareAsync();
                wkm_tsk_sbm_id.Value = taskSubmissionId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmission.TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"];
                        taskSubmission.TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"];
                        taskSubmission.TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString();
                        taskSubmission.FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString();
                        taskSubmission.FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString();
                        taskSubmission.ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString();
                        taskSubmission.ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString();
                        taskSubmission.SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"];
                        taskSubmission.DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"];
                        taskSubmission.Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString();
                        taskSubmission.IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"];
                        taskSubmission.DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"];
                    }
            }
            await conn.CloseAsync();
            return taskSubmission;
        }

        public async Task<List<TaskSubmission>> GetByToEmployeeIdAsync(string toEmployeeId)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                to_emp_id.Value = toEmployeeId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmissions.Add(new TaskSubmission
                        {
                            TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                            DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskSubmissions;
        }

        public async Task<List<TaskSubmission>> GetByToEmployeeIdAndTaskListIdAsync(string toEmployeeId, int taskListId)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND s.tsk_lst_id = @tsk_lst_id;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                to_emp_id.Value = toEmployeeId;
                tsk_lst_id.Value = taskListId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmissions.Add(new TaskSubmission
                        {
                            TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                            DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskSubmissions;
        }

        public async Task<List<TaskSubmission>> GetByToEmployeeIdAndSubmittedYearAsync(string toEmployeeId, int submittedYear)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("ORDER BY s.wkm_tsk_sbm_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                to_emp_id.Value = toEmployeeId;
                dt_yr.Value = submittedYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmissions.Add(new TaskSubmission
                        {
                            TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                            DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskSubmissions;
        }

        public async Task<List<TaskSubmission>> GetByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, int submittedYear, int submittedMonth)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("AND date_part('month', s.sbm_dt) = @dt_mm ");
            sb.Append("ORDER BY s.wkm_tsk_sbm_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                var dt_mm = cmd.Parameters.Add("@dt_mm", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                to_emp_id.Value = toEmployeeId;
                dt_yr.Value = submittedYear;
                dt_mm.Value = submittedMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmissions.Add(new TaskSubmission
                        {
                            TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                            DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskSubmissions;
        }

        //============= From Employee Name =============================//
        public async Task<List<TaskSubmission>> GetByToEmployeeIdAndFromEmployeeNameAsync(string toEmployeeId, string fromEmployeeName)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND LOWER(p1.fullname) = LOWER(@frm_emp_nm); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                to_emp_id.Value = toEmployeeId;
                frm_emp_nm.Value = fromEmployeeName;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmissions.Add(new TaskSubmission
                        {
                            TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                            DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskSubmissions;
        }

        public async Task<List<TaskSubmission>> GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(string toEmployeeId, string fromEmployeeName, int submittedYear)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("AND LOWER(p1.fullname) = LOWER(@frm_emp_nm) ");
            sb.Append("ORDER BY s.wkm_tsk_sbm_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                to_emp_id.Value = toEmployeeId;
                dt_yr.Value = submittedYear;
                frm_emp_nm.Value = fromEmployeeName;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmissions.Add(new TaskSubmission
                        {
                            TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                            DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskSubmissions;
        }

        public async Task<List<TaskSubmission>> GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, string fromEmployeeName, int submittedYear, int submittedMonth)
        {
            List<TaskSubmission> taskSubmissions = new List<TaskSubmission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.wkm_tsk_sbm_id, s.tsk_lst_id, s.frm_emp_id, ");
            sb.Append("s.to_emp_id, s.sbm_typ_id, s.sbm_dt, s.sbm_msg, ");
            sb.Append("s.is_xtn, s.dt_xtn, p1.fullname as frm_emp_nm, ");
            sb.Append("p2.fullname as to_emp_nm, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_tsk_sbms s  ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = s.frm_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = s.to_emp_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = s.tsk_lst_id ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("AND date_part('month', s.sbm_dt) = @dt_mm ");
            sb.Append("AND LOWER(p1.fullname) = LOWER(@frm_emp_nm) ");
            sb.Append("ORDER BY s.wkm_tsk_sbm_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                var dt_mm = cmd.Parameters.Add("@dt_mm", NpgsqlDbType.Integer);
                var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                to_emp_id.Value = toEmployeeId;
                dt_yr.Value = submittedYear;
                dt_mm.Value = submittedMonth;
                frm_emp_nm.Value = fromEmployeeName;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskSubmissions.Add(new TaskSubmission
                        {
                            TaskSubmissionId = reader["wkm_tsk_sbm_id"] == DBNull.Value ? 0 : (long)reader["wkm_tsk_sbm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                            DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskSubmissions;
        }

        #endregion
    }
}
