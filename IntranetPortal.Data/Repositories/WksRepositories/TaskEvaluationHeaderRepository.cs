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
    public class TaskEvaluationHeaderRepository : ITaskEvaluationHeaderRepository
    {
        public IConfiguration _config { get; }
        public TaskEvaluationHeaderRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Task Evaluation Header Read by Task OwnerID Action Methods
        //======== Get By Task Owner ID =============================//
        public async Task<List<TaskEvaluationHeader>> GetByTaskOwnerIdAndDueYearAndDueMonthAsync(string taskOwnerId, int dueYear, int dueMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE LOWER(h.owner_emp_id) = LOWER(@owner_emp_id) ");
            sb.Append("AND ((date_part('year',  h.eval_dt) = @dt_yr) ");
            sb.Append("AND (date_part('month',  h.eval_dt) = @dt_mn)) ");
            sb.Append("ORDER BY  h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var dt_mn = cmd.Parameters.Add("@dt_mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    owner_emp_id.Value = taskOwnerId;
                    dt_yr.Value = dueYear;
                    dt_mn.Value = dueMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                                TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                                EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                                TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                                TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                                AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                                TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                                AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskEvaluationHeaders;
        }
        public async Task<List<TaskEvaluationHeader>> GetByTaskOwnerIdAndDueYearAsync(string taskOwnerId, int dueYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE LOWER(h.owner_emp_id) = LOWER(@owner_emp_id) ");
            sb.Append("AND (date_part('year',  h.eval_dt) = @dt_yr) ");
            sb.Append("ORDER BY  h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    owner_emp_id.Value = taskOwnerId;
                    dt_yr.Value = dueYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                                TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                                EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                                TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                                TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                                AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                                TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                                AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskEvaluationHeaders;
        }
        public async Task<List<TaskEvaluationHeader>> GetByTaskOwnerIdAsync(string taskOwnerId)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE LOWER(h.owner_emp_id) = LOWER(@owner_emp_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                owner_emp_id.Value = taskOwnerId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }

            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }
        #endregion

        #region Task Evaluation Header Read by Task List ID Action Methods
        //======== Get By Task List ID =============================//
        public async Task<List<TaskEvaluationHeader>> GetByTaskListIdAndEvaluatorIdAsync(int taskListId, string evaluatorId)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.tsk_lst_id = @tsk_lst_id ");
            sb.Append("AND LOWER(h.eval_emp_id) = LOWER(@eval_emp_id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);

                await cmd.PrepareAsync();

                eval_emp_id.Value = evaluatorId;
                tsk_lst_id.Value = taskListId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }

            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }
        public async Task<List<TaskEvaluationHeader>> GetByIdAsync(int taskEvaluationHeaderId)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.eval_hdr_id = @eval_hdr_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                eval_hdr_id.Value = taskEvaluationHeaderId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }

            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }
        public async Task<TaskEvaluationHeader> GetScoresByTaskListIdAndEvaluatorIdAsync(int taskListId, string evaluatorId)
        {
            TaskEvaluationHeader taskEvaluationHeader = new TaskEvaluationHeader();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.eval_hdr_id, h.eval_emp_id, d.tsk_lst_id,");
            sb.Append("SUM(d.percent_completion) AS sum_percent_completion, ");
            sb.Append("SUM(d.quality_rating) AS sum_quality_rating, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, h.owner_emp_id, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wkm_eval_dtl  ");
            sb.Append("WHERE eval_hdr_id = d.eval_hdr_id  ");
            sb.Append("AND percent_completion = 100) ");
            sb.Append("AS no_items_completed FROM public.wkm_eval_dtl d ");
            sb.Append("INNER JOIN public.wkm_eval_hdr h ON h.eval_hdr_id = d.eval_hdr_id ");
            sb.Append("WHERE d.tsk_lst_id = @tsk_lst_id AND h.eval_emp_id = @eval_emp_id ");
            sb.Append("GROUP BY h.eval_hdr_id, h.eval_emp_id, d.eval_hdr_id, d.tsk_lst_id, ");
            sb.Append("h.owner_emp_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);

                await cmd.PrepareAsync();

                eval_emp_id.Value = evaluatorId;
                tsk_lst_id.Value = taskListId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeader.Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"];
                        taskEvaluationHeader.TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"];
                        taskEvaluationHeader.EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString();
                        taskEvaluationHeader.TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString();
                        taskEvaluationHeader.TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"];
                        taskEvaluationHeader.NoOfCompletedTasks = reader["no_items_completed"] == DBNull.Value ? 0 : (long)reader["no_items_completed"];
                        taskEvaluationHeader.TotalPercentCompletion = reader["sum_percent_completion"] == DBNull.Value ? 0 : (long)reader["sum_percent_completion"];
                        taskEvaluationHeader.TotalQualityRating = reader["sum_quality_rating"] == DBNull.Value ? 0 : (long)reader["sum_quality_rating"];
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeader;
        }

        #endregion

        #region Task Evaluation Header Read by Reports To Employee ID Action Methods
        //============== Get By Reports To Employee ID =============//
        public async Task<List<TaskEvaluationHeader>> GetByReportsToEmployeeIdAndDueYearAndDueMonthAsync(string reportsToEmployeeId, int dueYear, int dueMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE LOWER(h.owner_emp_id) IN (SELECT LOWER(emp_id) FROM ");
            sb.Append("public.erm_emp_rpts WHERE LOWER(rpt_emp_id) = LOWER(@rpt_emp_id) ");
            sb.Append("AND rpt_nds > CURRENT_DATE) ");
            sb.Append("AND ((date_part('year', h.eval_dt) = @dt_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) = @dt_mn)) ");
            sb.Append("ORDER BY  h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var dt_mn = cmd.Parameters.Add("@dt_mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = reportsToEmployeeId;
                    dt_yr.Value = dueYear;
                    dt_mn.Value = dueMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                                TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                                EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                                TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                                TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                                AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                                TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                                AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskEvaluationHeaders;
        }
        public async Task<List<TaskEvaluationHeader>> GetByReportsToEmployeeIdAndDueYearAsync(string reportsToEmployeeId, int dueYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE LOWER(h.owner_emp_id) IN (SELECT LOWER(emp_id) FROM ");
            sb.Append("public.erm_emp_rpts WHERE LOWER(rpt_emp_id) = LOWER(@rpt_emp_id) ");
            sb.Append("AND rpt_nds > CURRENT_DATE) ");
            sb.Append("AND (date_part('year', h.eval_dt) = @dt_yr) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = reportsToEmployeeId;
                    dt_yr.Value = dueYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                                TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                                EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                                TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                                TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                                AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                                TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                                AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync(); }
            return taskEvaluationHeaders;
        }
        public async Task<List<TaskEvaluationHeader>> GetByReportsToEmployeeIdAsync(string reportsToEmployeeId)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE LOWER(h.owner_emp_id) IN (SELECT LOWER(emp_id) FROM ");
            sb.Append("public.erm_emp_rpts WHERE LOWER(rpt_emp_id) = LOWER(@rpt_emp_id) ");
            sb.Append("AND rpt_nds > CURRENT_DATE);");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                rpt_emp_id.Value = reportsToEmployeeId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        #endregion

        #region Task Evaluation Header Read by Location, Department and Unit Action Methods
        //======== Get By Unit ID =============================//

        public async Task<List<TaskEvaluationHeader>> GetByUnitIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_unit_id = @unit_id ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn))");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByUnitIdAndEvaluationYearAsync(int taskOwnerUnitId, int startYear, int endYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_unit_id = @unit_id ");
            sb.Append("AND (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByUnitIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_unit_id = @unit_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn))");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByUnitIdAndLocationIdAndEvaluationYearAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_unit_id = @unit_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr)) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }


        //======== Get By Department ID =============================//
        public async Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_dept_id = @dept_id ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn))");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int startYear, int endYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_dept_id = @dept_id ");
            sb.Append("AND (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_dept_id = @dept_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn))");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByDepartmentIdAndLocationIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_dept_id = @dept_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr)) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }


        //======== Get By Location ID =================================//
        public async Task<List<TaskEvaluationHeader>> GetByLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn))");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByLocationIdAndEvaluationYearAsync(int taskOwnerLocationId, int startYear, int endYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr)) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByEvaluationYearAndEvaluationMonthAsync(int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeader>> GetByEvaluationYearAsync(int startYear, int endYear)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.tsk_lst_id, h.eval_emp_id, ");
            sb.Append("h.owner_emp_id, h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, h.eval_dt, h.total_no_of_tasks, ");
            sb.Append("h.no_completed_tasks, h.no_uncompleted_tasks, ");
            sb.Append("h.total_percent_completion, h.average_percent_completion, ");
            sb.Append("d.deptname, h.total_quality_rating, ");
            sb.Append("h.average_quality_rating, u.unitname, l.locname, ");
            sb.Append("e.current_designation AS evaluator_designation, ");
            sb.Append("p1.fullname AS eval_emp_name, l.loctype, t.tsk_lst_nm, ");
            sb.Append("f.current_designation AS owner_designation, ");
            sb.Append("p2.fullname AS owner_emp_name ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeader
                        {
                            Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                            EvaluatorName = reader["eval_emp_name"] == DBNull.Value ? string.Empty : reader["eval_emp_name"].ToString(),
                            EvaluatorDesignation = reader["evaluator_designation"] == DBNull.Value ? string.Empty : reader["evaluator_designation"].ToString(),
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerDesignation = reader["owner_designation"] == DBNull.Value ? string.Empty : reader["owner_designation"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDeptName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            NoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                            NoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                            TotalPercentCompletion = reader["total_percent_completion"] == DBNull.Value ? 0 : (long)reader["total_percent_completion"],
                            AveragePercentCompletion = reader["average_percent_completion"] == DBNull.Value ? 0.00M : (decimal)reader["average_percent_completion"],
                            TotalQualityRating = reader["total_quality_rating"] == DBNull.Value ? 0 : (long)reader["total_quality_rating"],
                            AverageQualityRating = reader["average_quality_rating"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        #endregion

        #region Task Evaluation Header Summary By Location, Department and Unit Action Methods

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_unit_id = @unit_id ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndEvaluationYearAsync(int taskOwnerUnitId, int startYear, int endYear)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_unit_id = @unit_id ");
            sb.Append("AND (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_unit_id = @unit_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByUnitIdAndLocationIdAndEvaluationYearAsync(int taskOwnerUnitId, int taskOwnerLocationId, int startYear, int endYear)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_unit_id = @unit_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                unit_id.Value = taskOwnerUnitId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }


        //======== Get By Department ID =============================//
        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_dept_id = @dept_id ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int startYear, int endYear)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE h.owner_dept_id = @dept_id ");
            sb.Append("AND (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_dept_id = @dept_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByDepartmentIdAndLocationIdAndEvaluationYearAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, int startYear, int endYear)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_dept_id = @dept_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                dept_id.Value = taskOwnerDepartmentId;
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }


        //======== Get By Location ID =================================//
        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByLocationIdAndEvaluationYearAndEvaluationMonthAsync(int taskOwnerLocationId, int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByLocationIdAndEvaluationYearAsync(int taskOwnerLocationId, int startYear, int endYear)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (h.owner_loc_id = @location_id) ");
            sb.Append("AND ((date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr)) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                location_id.Value = taskOwnerLocationId;
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByEvaluationYearAndEvaluationMonthAsync(int startYear, int endYear, int startMonth, int endMonth)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("AND (date_part('month', h.eval_dt) BETWEEN @start_mn AND @end_mn) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                var start_mn = cmd.Parameters.Add("@start_mn", NpgsqlDbType.Integer);
                var end_mn = cmd.Parameters.Add("@end_mn", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                start_yr.Value = startYear;
                end_yr.Value = endYear;
                start_mn.Value = startMonth;
                end_mn.Value = endMonth;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        public async Task<List<TaskEvaluationHeaderSummary>> GetSummaryByEvaluationYearAsync(int startYear, int endYear)
        {
            List<TaskEvaluationHeaderSummary> taskEvaluationHeaders = new List<TaskEvaluationHeaderSummary>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.owner_emp_id, p2.fullname AS owner_emp_name, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, ");
            sb.Append("h.owner_loc_id, u.unitname, d.deptname, l.locname, ");
            sb.Append("SUM(h.total_no_of_tasks) AS total_tasks, ");
            sb.Append("SUM(h.no_completed_tasks) AS comp_tasks, ");
            sb.Append("SUM(h.no_uncompleted_tasks) AS uncomp_tasks, ");
            sb.Append("AVG(h.average_percent_completion) AS ave_pc_comp, ");
            sb.Append("AVG(h.average_quality_rating) AS ave_qlt_rating ");
            sb.Append("FROM public.wkm_eval_hdr h ");
            sb.Append("INNER JOIN public.wkm_tsk_lst t ON t.tsk_lst_id = h.tsk_lst_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p1 ON p1.id = h.eval_emp_id ");
            sb.Append("INNER JOIN public.erm_emp_inf f ON f.emp_id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_prsns p2 ON p2.id = h.owner_emp_id ");
            sb.Append("INNER JOIN public.gst_units u ON u.unitqk = h.owner_unit_id ");
            sb.Append("INNER JOIN public.gst_depts d ON d.deptqk = h.owner_dept_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = h.owner_loc_id ");
            sb.Append("WHERE (date_part('year', h.eval_dt) BETWEEN @start_yr AND @end_yr) ");
            sb.Append("GROUP BY h.owner_loc_id, l.locname, h.owner_dept_id, d.deptname, ");
            sb.Append("h.owner_unit_id, u.unitname, h.owner_emp_id, p2.fullname ");
            sb.Append("ORDER BY l.locname, d.deptname, u.unitname, p2.fullname; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var start_yr = cmd.Parameters.Add("@start_yr", NpgsqlDbType.Integer);
                var end_yr = cmd.Parameters.Add("@end_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                start_yr.Value = startYear;
                end_yr.Value = endYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationHeaders.Add(new TaskEvaluationHeaderSummary
                        {
                            TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                            TaskOwnerName = reader["owner_emp_name"] == DBNull.Value ? string.Empty : reader["owner_emp_name"].ToString(),
                            TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                            TaskOwnerUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                            TaskOwnerDepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                            TaskOwnerLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNoOfTasks = reader["total_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["total_tasks"],
                            TotalNoOfCompletedTasks = reader["comp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["comp_tasks"],
                            TotalNoOfUncompletedTasks = reader["uncomp_tasks"] == DBNull.Value ? 0.00M : (decimal)reader["uncomp_tasks"],
                            AveragePercentCompletion = reader["ave_pc_comp"] == DBNull.Value ? 0.00M : (decimal)reader["ave_pc_comp"],
                            AverageQualityRating = reader["ave_qlt_rating"] == DBNull.Value ? 0.00M : (decimal)reader["ave_qlt_rating"],
                        });
                    }
            }
            await conn.CloseAsync();
            return taskEvaluationHeaders;
        }

        #endregion

        #region Task Evaluation Header Write Action Methods
        public async Task<int> AddAsync(TaskEvaluationHeader taskEvaluationHeader)
        {
            int id = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_eval_hdr(tsk_lst_id, eval_emp_id, ");
            sb.Append("owner_emp_id, owner_unit_id, owner_dept_id, ");
            sb.Append("owner_loc_id, eval_dt, total_no_of_tasks, ");
            sb.Append("no_completed_tasks, no_uncompleted_tasks, ");
            sb.Append("total_percent_completion, average_percent_completion, ");
            sb.Append("total_quality_rating, average_quality_rating) ");
            sb.Append("VALUES (@tsk_lst_id, @eval_emp_id, ");
            sb.Append("@owner_emp_id, @owner_unit_id, @owner_dept_id, ");
            sb.Append("@owner_loc_id, @eval_dt, @total_no_of_tasks, ");
            sb.Append("@no_completed_tasks, @no_uncompleted_tasks, ");
            sb.Append("@total_percent_completion, @average_percent_completion, ");
            sb.Append("@total_quality_rating, @average_quality_rating) ");
            sb.Append("returning eval_hdr_id;");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                var owner_unit_id = cmd.Parameters.Add("@owner_unit_id", NpgsqlDbType.Integer);
                var owner_dept_id = cmd.Parameters.Add("@owner_dept_id", NpgsqlDbType.Integer);
                var owner_loc_id = cmd.Parameters.Add("@owner_loc_id", NpgsqlDbType.Integer);
                var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.TimestampTz);
                var total_no_of_tasks = cmd.Parameters.Add("@total_no_of_tasks", NpgsqlDbType.Bigint);
                var no_completed_tasks = cmd.Parameters.Add("@no_completed_tasks", NpgsqlDbType.Bigint);
                var no_uncompleted_tasks = cmd.Parameters.Add("@no_uncompleted_tasks", NpgsqlDbType.Bigint);
                var total_percent_completion = cmd.Parameters.Add("@total_percent_completion", NpgsqlDbType.Bigint);
                var average_percent_completion = cmd.Parameters.Add("@average_percent_completion", NpgsqlDbType.Numeric);
                var total_quality_rating = cmd.Parameters.Add("@total_quality_rating", NpgsqlDbType.Bigint);
                var average_quality_rating = cmd.Parameters.Add("@average_quality_rating", NpgsqlDbType.Numeric);

                cmd.Prepare();

                tsk_lst_id.Value = taskEvaluationHeader.TaskListId;
                eval_emp_id.Value = taskEvaluationHeader.EvaluatorId;
                owner_emp_id.Value = taskEvaluationHeader.TaskOwnerId;
                owner_unit_id.Value = taskEvaluationHeader.TaskOwnerUnitId;
                owner_dept_id.Value = taskEvaluationHeader.TaskOwnerDeptId;
                owner_loc_id.Value = taskEvaluationHeader.TaskOwnerLocationId;
                eval_dt.Value = taskEvaluationHeader.EvaluationDate ?? DateTime.UtcNow;
                total_no_of_tasks.Value = taskEvaluationHeader.TotalNumberOfTasks;
                no_completed_tasks.Value = taskEvaluationHeader.NoOfCompletedTasks;
                no_uncompleted_tasks.Value = taskEvaluationHeader.NoOfUncompletedTasks;
                total_percent_completion.Value = taskEvaluationHeader.TotalPercentCompletion;
                average_percent_completion.Value = taskEvaluationHeader.AveragePercentCompletion;
                total_quality_rating.Value = taskEvaluationHeader.TotalQualityRating;
                average_quality_rating.Value = taskEvaluationHeader.AverageQualityRating;

                var result = await cmd.ExecuteScalarAsync();
                id = (int)result;
            }

            await conn.CloseAsync();
            return id;
        }

        public async Task<bool> UpdateAsync(TaskEvaluationHeader taskEvaluationHeader)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_eval_hdr  SET  tsk_lst_id=@tsk_lst_id, ");
            sb.Append("eval_emp_id=@eval_emp_id, owner_emp_id=@owner_emp_id, ");
            sb.Append("owner_unit_id=@owner_unit_id, owner_dept_id=@owner_dept_id, ");
            sb.Append("owner_loc_id=@owner_loc_id, eval_dt=@eval_dt, ");
            sb.Append("total_no_of_tasks=@total_no_of_tasks, ");
            sb.Append("no_completed_tasks=@no_completed_tasks, ");
            sb.Append("no_uncompleted_tasks=@no_uncompleted_tasks, ");
            sb.Append("total_percent_completion=@total_percent_completion, ");
            sb.Append("average_percent_completion=@average_percent_completion, ");
            sb.Append("total_quality_rating=@total_quality_rating, ");
            sb.Append("average_quality_rating=@average_quality_rating ");
            sb.Append("WHERE (eval_hdr_id=@eval_hdr_id);");

            string query = sb.ToString();
            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Integer);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                var owner_unit_id = cmd.Parameters.Add("@owner_unit_id", NpgsqlDbType.Integer);
                var owner_dept_id = cmd.Parameters.Add("@owner_dept_id", NpgsqlDbType.Integer);
                var owner_loc_id = cmd.Parameters.Add("@owner_loc_id", NpgsqlDbType.Integer);
                var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.TimestampTz);
                var total_no_of_tasks = cmd.Parameters.Add("@total_no_of_tasks", NpgsqlDbType.Bigint);
                var no_completed_tasks = cmd.Parameters.Add("@no_completed_tasks", NpgsqlDbType.Bigint);
                var no_uncompleted_tasks = cmd.Parameters.Add("@no_uncompleted_tasks", NpgsqlDbType.Bigint);
                var total_percent_completion = cmd.Parameters.Add("@total_percent_completion", NpgsqlDbType.Bigint);
                var average_percent_completion = cmd.Parameters.Add("@average_percent_completion", NpgsqlDbType.Numeric);
                var total_quality_rating = cmd.Parameters.Add("@total_quality_rating", NpgsqlDbType.Bigint);
                var average_quality_rating = cmd.Parameters.Add("@average_quality_rating", NpgsqlDbType.Numeric);

                cmd.Prepare();

                eval_hdr_id.Value = taskEvaluationHeader.Id;
                tsk_lst_id.Value = taskEvaluationHeader.TaskListId;
                eval_emp_id.Value = taskEvaluationHeader.EvaluatorId;
                owner_emp_id.Value = taskEvaluationHeader.TaskOwnerId;
                owner_unit_id.Value = taskEvaluationHeader.TaskOwnerUnitId;
                owner_dept_id.Value = taskEvaluationHeader.TaskOwnerDeptId;
                owner_loc_id.Value = taskEvaluationHeader.TaskOwnerLocationId;
                eval_dt.Value = taskEvaluationHeader.EvaluationDate ?? DateTime.UtcNow;
                total_no_of_tasks.Value = taskEvaluationHeader.TotalNumberOfTasks;
                no_completed_tasks.Value = taskEvaluationHeader.NoOfCompletedTasks;
                no_uncompleted_tasks.Value = taskEvaluationHeader.NoOfUncompletedTasks;
                total_percent_completion.Value = taskEvaluationHeader.TotalPercentCompletion;
                average_percent_completion.Value = taskEvaluationHeader.AveragePercentCompletion;
                total_quality_rating.Value = taskEvaluationHeader.TotalQualityRating;
                average_quality_rating.Value = taskEvaluationHeader.AverageQualityRating;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion
    }
}
