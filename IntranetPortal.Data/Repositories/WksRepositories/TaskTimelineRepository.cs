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
    public class TaskTimelineRepository : ITaskTimelineRepository
    {
        public IConfiguration _config { get; }
        public TaskTimelineRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region TaskTimeline Write Action Methods
        public async Task<bool> AddAsync(TaskTimelineChange taskTimelineChange)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_tsk_tml(tsk_itm_id, ");
            sb.Append("prev_start_dt, prev_end_dt, new_start_dt, ");
            sb.Append("new_end_dt, diff_in_days, mod_by, mod_dt, ");
            sb.Append("tsk_lst_id) VALUES (@tsk_itm_id, @prev_start_dt, ");
            sb.Append("@prev_end_dt, @new_start_dt, @new_end_dt, ");
            sb.Append("@diff_in_days, @mod_by, @mod_dt, @tsk_lst_id);");

            string query = sb.ToString();
            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var prev_start_dt = cmd.Parameters.Add("@prev_start_dt", NpgsqlDbType.TimestampTz);
                var prev_end_dt = cmd.Parameters.Add("@prev_end_dt", NpgsqlDbType.TimestampTz);
                var new_start_dt = cmd.Parameters.Add("@new_start_dt", NpgsqlDbType.TimestampTz);
                var new_end_dt = cmd.Parameters.Add("@new_end_dt", NpgsqlDbType.TimestampTz);
                var diff_in_days = cmd.Parameters.Add("@diff_in_days", NpgsqlDbType.Numeric);
                var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);

                cmd.Prepare();

                tsk_itm_id.Value = taskTimelineChange.TaskItemId;
                tsk_lst_id.Value = taskTimelineChange.TaskListId;
                prev_start_dt.Value = taskTimelineChange.PreviousStartDate ?? (object)DBNull.Value;
                prev_end_dt.Value = taskTimelineChange.PreviousEndDate ?? (object)DBNull.Value;
                new_start_dt.Value = taskTimelineChange.NewStartDate ?? (object)DBNull.Value;
                new_end_dt.Value = taskTimelineChange.NewEndDate ?? (object)DBNull.Value;
                diff_in_days.Value = taskTimelineChange.DifferentInDays;
                mod_by.Value = taskTimelineChange.ModifiedBy ?? (object)DBNull.Value;
                mod_dt.Value = taskTimelineChange.ModifiedTime ?? $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()} GMT";

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        #region TaskTimeline Read Action Methods
        public async Task<List<TaskTimelineChange>> GetByTaskItemIdAsync(long taskItemId)
        {
            List<TaskTimelineChange> taskTimelineChanges = new List<TaskTimelineChange>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tml_chg_id, tsk_itm_id, prev_start_dt, ");
            sb.Append("prev_end_dt, new_start_dt, new_end_dt, diff_in_days, ");
            sb.Append("mod_by, mod_dt, tsk_lst_id FROM public.wkm_tsk_tml ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id) ");
            sb.Append("ORDER BY tml_chg_id DESC; ");

            string query = sb.ToString();
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
                        taskTimelineChanges.Add(new TaskTimelineChange
                        {
                            TimelineChangeId = reader["tml_chg_id"] == DBNull.Value ? 0 : (long)reader["tml_chg_id"],
                            TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            PreviousStartDate = reader["prev_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_start_dt"],
                            PreviousEndDate = reader["prev_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_end_dt"],
                            NewStartDate = reader["new_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_start_dt"],
                            NewEndDate = reader["new_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_end_dt"],
                            DifferentInDays = reader["diff_in_days"] == DBNull.Value ? 0.00M : (decimal)reader["diff_in_days"],
                            ModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                            ModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return taskTimelineChanges;
        }

        public async Task<TaskTimelineChange> GetByIdAsync(long taskTimelineChangeId)
        {
            TaskTimelineChange taskTimelineChange = new TaskTimelineChange();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tml_chg_id, tsk_itm_id, prev_start_dt, ");
            sb.Append("prev_end_dt, new_start_dt, new_end_dt, diff_in_days, ");
            sb.Append("mod_by, mod_dt, tsk_lst_id FROM public.wkm_tsk_tml ");
            sb.Append("WHERE (tml_chg_id = @tml_chg_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var tml_chg_id = cmd.Parameters.Add("@tml_chg_id", NpgsqlDbType.Bigint);
                await cmd.PrepareAsync();
                tml_chg_id.Value = taskTimelineChangeId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskTimelineChange.TimelineChangeId = reader["tml_chg_id"] == DBNull.Value ? 0 : (long)reader["tml_chg_id"];
                        taskTimelineChange.TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"];
                        taskTimelineChange.TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"];
                        taskTimelineChange.PreviousStartDate = reader["prev_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_start_dt"];
                        taskTimelineChange.PreviousEndDate = reader["prev_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_end_dt"];
                        taskTimelineChange.NewStartDate = reader["new_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_start_dt"];
                        taskTimelineChange.NewEndDate = reader["new_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_end_dt"];
                        taskTimelineChange.DifferentInDays = reader["diff_in_days"] == DBNull.Value ? 0.00M : (decimal)reader["diff_in_days"];
                        taskTimelineChange.ModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString();
                        taskTimelineChange.ModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString();
                    }
            }
            await conn.CloseAsync();
            return taskTimelineChange;
        }

        #endregion
    }
}
