using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Base.Repositories.PmsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.PmsRepositories
{
    public class ReviewMessageRepository : IReviewMessageRepository
    {
        public IConfiguration _config { get; }
        public ReviewMessageRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<ReviewMessage>> GetByReviewHeaderIdAsync(int reviewHeaderId)
        {
            List<ReviewMessage> reviewMessageList = new List<ReviewMessage>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_msg_id, m.rvw_hdr_id, m.frm_emp_id, ");
            sb.Append("m.msg_ds, m.msg_dt, m.is_cl, m.dt_cl, e.fullname AS frm_emp_nm, ");
            sb.Append("UPPER(e.sex) AS frm_emp_sex  ");
            sb.Append("FROM public.pmsrvwmsgs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = m.frm_emp_id ");
            sb.Append("WHERE (m.rvw_hdr_id = @rvw_hdr_id) ");
            sb.Append("ORDER BY m.rvw_msg_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_hdr_id.Value = reviewHeaderId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewMessageList.Add(new ReviewMessage()
                    {
                        ReviewMessageId = reader["rvw_msg_id"] == DBNull.Value ? 0 : (int)reader["rvw_msg_id"],
                        ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"],
                        FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                        FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                        FromEmployeeSex = reader["frm_emp_sex"] == DBNull.Value ? string.Empty : reader["frm_emp_sex"].ToString(),
                        MessageBody = reader["msg_ds"] == DBNull.Value ? string.Empty : reader["msg_ds"].ToString(),
                        MessageIsCancelled = reader["is_cl"] == DBNull.Value ? false : (bool)reader["is_cl"],
                        MessageTime = reader["msg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["msg_dt"],
                        TimeCancelled = reader["dt_cl"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_cl"],
                    });
                }
            }
            await conn.CloseAsync();
            return reviewMessageList;
        }

        public async Task<ReviewMessage> GetByIdAsync(int reviewMessageId)
        {
            ReviewMessage reviewMessage = new ReviewMessage();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.rvw_msg_id, m.rvw_hdr_id, m.frm_emp_id, ");
            sb.Append("m.msg_ds, m.msg_dt, m.is_cl, m.dt_cl, e.fullname AS frm_emp_nm, ");
            sb.Append("UPPER(e.sex) AS frm_emp_sex ");
            sb.Append("FROM public.pmsrvwmsgs m ");
            sb.Append("INNER JOIN public.pmsrvwhdrs h ON h.rvw_hdr_id = m.rvw_hdr_id ");
            sb.Append("INNER JOIN public.gst_prsns e ON e.id = m.frm_emp_id ");
            sb.Append("WHERE (m.rvw_msg_id = @rvw_msg_id); ");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var rvw_msg_id = cmd.Parameters.Add("@rvw_msg_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                rvw_msg_id.Value = reviewMessageId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    reviewMessage.ReviewMessageId = reader["rvw_msg_id"] == DBNull.Value ? 0 : (int)reader["rvw_msg_id"];
                    reviewMessage.ReviewHeaderId = reader["rvw_hdr_id"] == DBNull.Value ? 0 : (int)reader["rvw_hdr_id"];
                    reviewMessage.FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString();
                    reviewMessage.FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString();
                    reviewMessage.FromEmployeeSex = reader["frm_emp_sex"] == DBNull.Value ? string.Empty : reader["frm_emp_sex"].ToString();
                    reviewMessage.MessageBody = reader["msg_ds"] == DBNull.Value ? string.Empty : reader["msg_ds"].ToString();
                    reviewMessage.MessageIsCancelled = reader["is_cl"] == DBNull.Value ? false : (bool)reader["is_cl"];
                    reviewMessage.MessageTime = reader["msg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["msg_dt"];
                    reviewMessage.TimeCancelled = reader["dt_cl"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_cl"];
                }
            }
            await conn.CloseAsync();
            return reviewMessage;
        }

        public async Task<bool> AddAsync(ReviewMessage reviewMessage)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pmsrvwmsgs(rvw_hdr_id, frm_emp_id, ");
            sb.Append("msg_ds, msg_dt, is_cl, dt_cl) VALUES (@rvw_hdr_id, ");
            sb.Append("@frm_emp_id, @msg_ds, @msg_dt, @is_cl, @dt_cl); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_hdr_id = cmd.Parameters.Add("@rvw_hdr_id", NpgsqlDbType.Integer);
                    var frm_emp_id = cmd.Parameters.Add("@frm_emp_id", NpgsqlDbType.Text);
                    var msg_ds = cmd.Parameters.Add("@msg_ds", NpgsqlDbType.Text);
                    var msg_dt = cmd.Parameters.Add("@msg_dt", NpgsqlDbType.TimestampTz);
                    var is_cl = cmd.Parameters.Add("@is_cl", NpgsqlDbType.Boolean);
                    var dt_cl = cmd.Parameters.Add("@dt_cl", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    rvw_hdr_id.Value = reviewMessage.ReviewHeaderId;
                    frm_emp_id.Value = reviewMessage.FromEmployeeId;
                    msg_ds.Value = reviewMessage.MessageBody;
                    msg_dt.Value = reviewMessage.MessageTime ?? DateTime.UtcNow;
                    dt_cl.Value = reviewMessage.TimeCancelled ?? (object)DBNull.Value;
                    is_cl.Value = reviewMessage.MessageIsCancelled;

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

        public async Task<bool> UpdateAsync(ReviewMessage reviewMessage)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pmsrvwmsgs	SET msg_ds=@msg_ds, ");
            sb.Append("msg_dt=@msg_dt WHERE (rvw_msg_id = @rvw_msg_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var msg_ds = cmd.Parameters.Add("@msg_ds", NpgsqlDbType.Text);
                    var msg_dt = cmd.Parameters.Add("@msg_dt", NpgsqlDbType.TimestampTz);
                    var rvw_msg_id = cmd.Parameters.Add("@rvw_msg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_msg_id.Value = reviewMessage.ReviewMessageId;
                    msg_ds.Value = reviewMessage.MessageBody;
                    msg_dt.Value = reviewMessage.MessageTime ?? DateTime.UtcNow;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int reviewMessageId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.pmsrvwmsgs WHERE (rvw_msg_id = @rvw_msg_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rvw_msg_id = cmd.Parameters.Add("@rvw_msg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    rvw_msg_id.Value = reviewMessageId;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return rows > 0;
        }
    }
}
