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
    public class TaskEvaluationDetailRepository : ITaskEvaluationDetailRepository
    {
        public IConfiguration _config { get; }
        public TaskEvaluationDetailRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Task Evaluation Detail Read Action Methods
        public async Task<List<TaskEvaluationDetail>> GetByTaskEvaluationHeaderIdAndTaskItemIdAsync(int taskEvaluationHeaderId, long taskItemId)
        {
            List<TaskEvaluationDetail> taskEvaluationDetails = new List<TaskEvaluationDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.eval_dtl_id, d.eval_hdr_id, d.tsk_lst_id, ");
            sb.Append("d.tsk_itm_id, d.eval_dt, d.percent_completion, ");
            sb.Append("d.quality_rating, d.eval_comments, t.tsk_itm_no, ");
            sb.Append("t.tsk_itm_ds, t.deliverables, t.crt_by, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_eval_dtl d ");
            sb.Append("INNER JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = d.tsk_itm_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = d.tsk_lst_id ");
            sb.Append("WHERE (d.eval_hdr_id = @eval_hdr_id) ");
            sb.Append("AND (d.tsk_itm_id = @tsk_itm_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Integer);
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                await cmd.PrepareAsync();
                eval_hdr_id.Value = taskEvaluationHeaderId;
                tsk_itm_id.Value = taskItemId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        taskEvaluationDetails.Add(new TaskEvaluationDetail
                        {
                            TaskEvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                            TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                            TaskItemNo = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString(),
                            TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString(),
                            TaskItemDeliverable = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),

                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            PercentageCompletion = reader["percent_completion"] == DBNull.Value ? 0 : (int)reader["percent_completion"],
                            QualityRating = reader["quality_rating"] == DBNull.Value ? 0 : (int)reader["quality_rating"],
                            EvaluatorsComment = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),
                            TaskOwnerName = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),

                        });
                    }
            }

            await conn.CloseAsync();
            return taskEvaluationDetails;
        }

        public async Task<List<TaskEvaluationDetail>> GetByTaskEvaluationHeaderIdAsync(int taskEvaluationHeaderId)
        {
            List<TaskEvaluationDetail> taskEvaluationDetails = new List<TaskEvaluationDetail>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.eval_dtl_id, d.eval_hdr_id, d.tsk_lst_id, ");
            sb.Append("d.tsk_itm_id, d.eval_dt, d.percent_completion, ");
            sb.Append("d.quality_rating, d.eval_comments, t.tsk_itm_no, ");
            sb.Append("t.tsk_itm_ds, t.deliverables, t.crt_by, l.tsk_lst_nm ");
            sb.Append("FROM public.wkm_eval_dtl d ");
            sb.Append("INNER JOIN public.wkm_tsk_itms t ON t.tsk_itm_id = d.tsk_itm_id ");
            sb.Append("INNER JOIN public.wkm_tsk_lst l ON l.tsk_lst_id = d.tsk_lst_id ");
            sb.Append("WHERE (d.eval_hdr_id = @eval_hdr_id) ");
            sb.Append("ORDER BY d.tsk_itm_id; ");

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
                        taskEvaluationDetails.Add(new TaskEvaluationDetail
                        {
                            TaskEvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                            TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (int)reader["eval_hdr_id"],
                            TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                            TaskListName = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                            TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                            TaskItemNo = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString(),
                            TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString(),
                            TaskItemDeliverable = reader["deliverables"] == DBNull.Value ? string.Empty : reader["deliverables"].ToString(),

                            EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                            PercentageCompletion = reader["percent_completion"] == DBNull.Value ? 0 : (int)reader["percent_completion"],
                            QualityRating = reader["quality_rating"] == DBNull.Value ? 0 : (int)reader["quality_rating"],
                            EvaluatorsComment = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),
                            TaskOwnerName = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),

                        });
                    }
            }

            await conn.CloseAsync();
            return taskEvaluationDetails;
        }


        #endregion

        #region Task Evaluation Detail Write Action Methods
        public async Task<bool> AddAsync(TaskEvaluationDetail taskEvaluationDetail)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_eval_dtl(eval_hdr_id, tsk_lst_id, ");
            sb.Append("tsk_itm_id, eval_dt, percent_completion, quality_rating, ");
            sb.Append("eval_comments) VALUES (@eval_hdr_id, @tsk_lst_id, ");
            sb.Append("@tsk_itm_id, @eval_dt, @percent_completion, ");
            sb.Append("@quality_rating, @eval_comments); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Integer);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.TimestampTz);
                var percent_completion = cmd.Parameters.Add("@percent_completion", NpgsqlDbType.Integer);
                var quality_rating = cmd.Parameters.Add("@quality_rating", NpgsqlDbType.Integer);
                var eval_comments = cmd.Parameters.Add("@eval_comments", NpgsqlDbType.Text);

                cmd.Prepare();

                eval_hdr_id.Value = taskEvaluationDetail.TaskEvaluationHeaderId;
                tsk_lst_id.Value = taskEvaluationDetail.TaskListId;
                tsk_itm_id.Value = taskEvaluationDetail.TaskItemId;
                eval_dt.Value = taskEvaluationDetail.EvaluationDate ?? DateTime.UtcNow;
                percent_completion.Value = taskEvaluationDetail.PercentageCompletion;
                quality_rating.Value = taskEvaluationDetail.QualityRating;
                eval_comments.Value = taskEvaluationDetail.EvaluatorsComment ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(TaskEvaluationDetail taskEvaluationDetail)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_eval_dtl SET eval_hdr_id=@eval_hdr_id, ");
            sb.Append("tsk_lst_id=@tsk_lst_id, tsk_itm_id=@tsk_itm_id, ");
            sb.Append("eval_dt=@eval_dt, percent_completion=@percent_completion, ");
            sb.Append("quality_rating=@quality_rating, eval_comments=@eval_comments ");
            sb.Append("WHERE (eval_dtl_id=@eval_dtl_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var eval_dtl_id = cmd.Parameters.Add("@eval_dtl_id", NpgsqlDbType.Bigint);
                var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Integer);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.TimestampTz);
                var percent_completion = cmd.Parameters.Add("@percent_completion", NpgsqlDbType.Integer);
                var quality_rating = cmd.Parameters.Add("@quality_rating", NpgsqlDbType.Integer);
                var eval_comments = cmd.Parameters.Add("@eval_comments", NpgsqlDbType.Text);

                cmd.Prepare();

                eval_dtl_id.Value = taskEvaluationDetail.TaskEvaluationDetailId;
                eval_hdr_id.Value = taskEvaluationDetail.TaskEvaluationHeaderId;
                tsk_lst_id.Value = taskEvaluationDetail.TaskListId;
                tsk_itm_id.Value = taskEvaluationDetail.TaskItemId;
                eval_dt.Value = taskEvaluationDetail.EvaluationDate ?? DateTime.UtcNow;
                percent_completion.Value = taskEvaluationDetail.PercentageCompletion;
                quality_rating.Value = taskEvaluationDetail.QualityRating;
                eval_comments.Value = taskEvaluationDetail.EvaluatorsComment ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

    }
}
