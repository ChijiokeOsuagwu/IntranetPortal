using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.BaseRepositories
{
    public class UtilityRepository : IUtilityRepository
    {
        public IConfiguration _config { get; }
        public UtilityRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //=============== Database Connectivity Test Action Method  ===============================//
        #region Database Connectivity Test Action Methods
        public async Task<bool> CheckDatabaseConnectionAsync()
        {
            bool IsOpen = false;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            try
            {
                await conn.OpenAsync();
                IsOpen = true;
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                IsOpen = false;
            }
            return IsOpen;
        }

        #endregion

        //=============== Auto Number Action Methods ============================================//
        #region Auto Number Action Methods
        public async Task<string> GetAutoNumberAsync(string numberType)
        {
            string codeNumber = string.Empty;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            if (String.IsNullOrEmpty(numberType)) { throw new ArgumentNullException(nameof(numberType), "The required parameter [Number Type] is missing or has an invalid value."); }
            string query = $"SELECT COALESCE(prefix,'') || LPAD(next_no::text, no_length, '0') AS code_no FROM gst_auto_no WHERE (no_type = @no_type)";
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var noType = cmd.Parameters.Add("@no_type", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    noType.Value = numberType;
                    var result = await cmd.ExecuteScalarAsync();
                    codeNumber = result.ToString();
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return codeNumber;
        }

        public async Task<bool> IncrementAutoNumberAsync(string numberType)
        {
            if (String.IsNullOrEmpty(numberType)) { throw new ArgumentNullException(nameof(numberType), "The required parameter [Number Type] is missing or has an invalid value."); }
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.gst_auto_no SET next_no=next_no + 1	WHERE no_type=@no_type;";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var no_type = cmd.Parameters.Add("@no_type", NpgsqlDbType.Text);
                    cmd.Prepare();
                    no_type.Value = numberType;
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

        public async Task<int> GetNumberCount(AutoNumberType type, int day, int month, int year)
        {
            int recordCount = -1;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(cdn_rgr_id) FROM public.gst_cdn_rgr ");
            sb.Append("WHERE(cdn_typ = @cdn_typ AND dd = @dd AND mm = @mm AND yy = @yy);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var cdn_typ = cmd.Parameters.Add("@cdn_typ", NpgsqlDbType.Integer);
                    var dd = cmd.Parameters.Add("@dd", NpgsqlDbType.Integer);
                    var mm = cmd.Parameters.Add("@mm", NpgsqlDbType.Integer);
                    var yy = cmd.Parameters.Add("@yy", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    cdn_typ.Value = Convert.ToInt64(type);
                    dd.Value = day;
                    mm.Value = month;
                    yy.Value = year;
                    var result = await cmd.ExecuteScalarAsync();
                    recordCount = Convert.ToInt32(result);
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return recordCount;
        }

        public async Task<bool> AddCodeNumberRecord(AutoNumberType type, int day, int month, int year)
        {
            int recordCount = -1;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_cdn_rgr(cdn_typ, dd, mm, yy, mdt)  ");
            sb.Append("VALUES (@cdn_typ, @dd, @mm, @yy, @mdt);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var cdn_typ = cmd.Parameters.Add("@cdn_typ", NpgsqlDbType.Integer);
                    var dd = cmd.Parameters.Add("@dd", NpgsqlDbType.Integer);
                    var mm = cmd.Parameters.Add("@mm", NpgsqlDbType.Integer);
                    var yy = cmd.Parameters.Add("@yy", NpgsqlDbType.Integer);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    cdn_typ.Value = (int)type;
                    dd.Value = day;
                    mm.Value = month;
                    yy.Value = year;
                    mdt.Value = $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";
                    var result = await cmd.ExecuteNonQueryAsync();
                    recordCount = (int)result;
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return recordCount > 0;
        }

        #endregion

        //=============== Messages Action Methods ================================================//
        #region Messages Action Methods
        public async Task<List<Message>> GetMessagesByReceipientIdAsync(string recipientId)
        {
            List<Message> messageList = new List<Message>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.msg_id, m.msg_time, m.msg_from, m.msg_sbj, ");
            sb.Append("m.msg_bdy, m.msg_url, r.msg_rcpt_id, r.rcpt_id, ");
            sb.Append("r.is_rd, r.time_rd, r.is_del, r.time_del, ");
            sb.Append("r.msg_id, p.fullname FROM public.gst_msgs m  ");
            sb.Append("INNER JOIN public.gst_msgrs r ON m.msg_id = r.msg_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON r.rcpt_id = p.id ");
            sb.Append("WHERE (r.rcpt_id = @rcpt_id OR r.rcpt_id IS NULL) ");
            sb.Append("AND (r.is_del = false) ");
            sb.Append("ORDER BY r.msg_rcpt_id DESC;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rcpt_id = cmd.Parameters.Add("@rcpt_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    rcpt_id.Value = recipientId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        messageList.Add(new Message()
                        {
                            MessageID = reader["msg_id"] == DBNull.Value ? string.Empty : (reader["msg_id"]).ToString(),
                            SentTime = reader["msg_time"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["msg_time"]),
                            SentBy = reader["msg_from"] == DBNull.Value ? string.Empty : (reader["msg_from"]).ToString(),
                            ActionUrl = reader["msg_url"] == DBNull.Value ? string.Empty : reader["msg_url"].ToString(),
                            DeletedTime = reader["time_del"] == DBNull.Value ? string.Empty : reader["time_del"].ToString(),
                            IsDeleted = reader["is_del"] == DBNull.Value ? false : (bool)reader["is_del"],
                            IsRead = reader["is_rd"] == DBNull.Value ? false : (bool)reader["is_rd"],
                            MessageBody = reader["msg_bdy"] == DBNull.Value ? string.Empty : reader["msg_bdy"].ToString(),
                            MessageDetailID = reader["msg_rcpt_id"] == DBNull.Value ? 0 : (long)reader["msg_rcpt_id"],
                            ReadTime = reader["time_rd"] == DBNull.Value ? string.Empty : reader["time_rd"].ToString(),
                            RecipientID = reader["rcpt_id"] == DBNull.Value ? string.Empty : reader["rcpt_id"].ToString(),
                            RecipientName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Subject = reader["msg_sbj"] == DBNull.Value ? string.Empty : reader["msg_sbj"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return messageList;
        }

        public async Task<Message> GetMessageByMessageDetailIdAsync(int messageDetailId)
        {
            Message message = new Message();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.msg_id, m.msg_time, m.msg_from, m.msg_sbj, m.msg_bdy, m.msg_url, r.msg_rcpt_id, r.rcpt_id, ");
            sb.Append("r.is_rd, r.time_rd, r.is_del, r.time_del, r.msg_id, p.fullname FROM public.gst_msgs m ");
            sb.Append("INNER JOIN public.gst_msgrs r ON m.msg_id = r.msg_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON r.rcpt_id = p.id ");
            sb.Append("WHERE (r.msg_rcpt_id = @msg_rcpt_id);");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var msg_rcpt_id = cmd.Parameters.Add("@msg_rcpt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    msg_rcpt_id.Value = messageDetailId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        message.MessageID = reader["msg_id"] == DBNull.Value ? string.Empty : (reader["msg_id"]).ToString();
                        message.SentTime = reader["msg_time"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["msg_time"]);
                        message.SentBy = reader["msg_from"] == DBNull.Value ? string.Empty : (reader["msg_from"]).ToString();
                        message.ActionUrl = reader["msg_url"] == DBNull.Value ? string.Empty : reader["msg_url"].ToString();
                        message.DeletedTime = reader["time_del"] == DBNull.Value ? string.Empty : reader["time_del"].ToString();
                        message.IsDeleted = reader["is_del"] == DBNull.Value ? false : (bool)reader["is_del"];
                        message.IsRead = reader["is_rd"] == DBNull.Value ? false : (bool)reader["is_rd"];
                        message.MessageBody = reader["msg_bdy"] == DBNull.Value ? string.Empty : reader["msg_bdy"].ToString();
                        message.MessageDetailID = reader["msg_rcpt_id"] == DBNull.Value ? 0 : (int)reader["msg_rcpt_id"];
                        message.ReadTime = reader["time_rd"] == DBNull.Value ? string.Empty : reader["time_rd"].ToString();
                        message.RecipientID = reader["rcpt_id"] == DBNull.Value ? string.Empty : reader["rcpt_id"].ToString();
                        message.RecipientName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString();
                        message.Subject = reader["msg_sbj"] == DBNull.Value ? string.Empty : reader["msg_sbj"].ToString();
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return message;
        }

        public async Task<Message> GetMessageByMessageIdAsync(string messageId)
        {
            Message message = new Message();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.msg_id, m.msg_time, m.msg_from, m.msg_sbj, m.msg_bdy, m.msg_url, r.msg_rcpt_id, r.rcpt_id, ");
            sb.Append("r.is_rd, r.time_rd, r.is_del, r.time_del, r.msg_id, p.fullname FROM public.gst_msgs m ");
            sb.Append("INNER JOIN public.gst_msgrs r ON m.msg_id = r.msg_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON r.rcpt_id = p.id ");
            sb.Append("WHERE (LOWER(m.msg_id) = LOWER(@msg_id));");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var msg_id = cmd.Parameters.Add("@msg_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    msg_id.Value = messageId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        message.MessageID = reader["msg_id"] == DBNull.Value ? string.Empty : (reader["msg_id"]).ToString();
                        message.SentTime = reader["msg_time"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["msg_time"]);
                        message.SentBy = reader["msg_from"] == DBNull.Value ? string.Empty : (reader["msg_from"]).ToString();
                        message.ActionUrl = reader["msg_url"] == DBNull.Value ? string.Empty : reader["msg_url"].ToString();
                        message.DeletedTime = reader["time_del"] == DBNull.Value ? string.Empty : reader["time_del"].ToString();
                        message.IsDeleted = reader["is_del"] == DBNull.Value ? false : (bool)reader["is_del"];
                        message.IsRead = reader["is_rd"] == DBNull.Value ? false : (bool)reader["is_rd"];
                        message.MessageBody = reader["msg_bdy"] == DBNull.Value ? string.Empty : reader["msg_bdy"].ToString();
                        message.MessageDetailID = reader["msg_rcpt_id"] == DBNull.Value ? 0 : (int)reader["msg_rcpt_id"];
                        message.ReadTime = reader["time_rd"] == DBNull.Value ? string.Empty : reader["time_rd"].ToString();
                        message.RecipientID = reader["rcpt_id"] == DBNull.Value ? string.Empty : reader["rcpt_id"].ToString();
                        message.RecipientName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString();
                        message.Subject = reader["msg_sbj"] == DBNull.Value ? string.Empty : reader["msg_sbj"].ToString();
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return message;
        }

        public async Task<bool> AddMessageAsync(Message message)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_msgs(msg_id, msg_time, msg_from, msg_sbj, msg_bdy, msg_url) ");
            sb.Append("VALUES (@msg_id, @msg_time, @msg_from, @msg_sbj, @msg_bdy, @msg_url);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var messageId = cmd.Parameters.Add("@msg_id", NpgsqlDbType.Text);
                    var sentTime = cmd.Parameters.Add("@msg_time", NpgsqlDbType.TimestampTz);
                    var sentBy = cmd.Parameters.Add("@msg_from", NpgsqlDbType.Text);
                    var messageSubject = cmd.Parameters.Add("@msg_sbj", NpgsqlDbType.Text);
                    var messageBody = cmd.Parameters.Add("@msg_bdy", NpgsqlDbType.Text);
                    var actionUrl = cmd.Parameters.Add("@msg_url", NpgsqlDbType.Text);
                    cmd.Prepare();
                    messageId.Value = message.MessageID ?? Guid.NewGuid().ToString();
                    sentTime.Value = message.SentTime ?? DateTime.UtcNow; //$"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value; ;
                    sentBy.Value = message.SentBy;
                    messageSubject.Value = message.Subject;
                    messageBody.Value = message.MessageBody;
                    actionUrl.Value = message.ActionUrl ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync();}
            return rows > 0;
        }

        public async Task<bool> AddMessageDetailAsync(MessageDetail messageDetail)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_msgrs(rcpt_id, is_rd, time_rd, is_del, time_del, msg_id) ");
            sb.Append("VALUES (@rcpt_id, @is_rd, @time_rd, @is_del, @time_del, @msg_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var messageId = cmd.Parameters.Add("@msg_id", NpgsqlDbType.Text);
                    var recipientId = cmd.Parameters.Add("@rcpt_id", NpgsqlDbType.Text);
                    var isRead = cmd.Parameters.Add("@is_rd", NpgsqlDbType.Boolean);
                    var timeRead = cmd.Parameters.Add("@time_rd", NpgsqlDbType.Text);
                    var isDeleted = cmd.Parameters.Add("@is_del", NpgsqlDbType.Boolean);
                    var timeDeleted = cmd.Parameters.Add("@time_del", NpgsqlDbType.Text);
                    cmd.Prepare();
                    messageId.Value = messageDetail.MessageID;
                    recipientId.Value = messageDetail.RecipientID;
                    isRead.Value = messageDetail.IsRead;
                    timeRead.Value = messageDetail.ReadTime ?? (object)DBNull.Value;
                    isDeleted.Value = messageDetail.IsDeleted;
                    timeDeleted.Value = messageDetail.DeletedTime ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally {await conn.CloseAsync(); }
            return rows > 0;
        }

        public async Task<bool> UpdateMessageReadStatusAsync(int messageDetailId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_msgrs SET is_rd = true, time_rd = @time_rd  WHERE (msg_rcpt_id=@msg_rcpt_id)");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var msg_rcpt_id = cmd.Parameters.Add("@msg_rcpt_id", NpgsqlDbType.Integer);
                    var timeRead = cmd.Parameters.Add("@time_rd", NpgsqlDbType.Text);
                    cmd.Prepare();
                    msg_rcpt_id.Value = messageDetailId;
                    timeRead.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value; ;

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

        //======= Delete Messages Permanently ==============////
        public async Task<bool> DeleteReadMessageDetailByRecipientIdAsync(string recipientId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("DELETE FROM public.gst_msgrs ");
            sb.Append("WHERE (rcpt_id = @rcpt_id) AND (is_rd = true);");
   
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var rcpt_id = cmd.Parameters.Add("@rcpt_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    rcpt_id.Value = recipientId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { await conn.CloseAsync();}

            return rows > 0;
        }

        public async Task<bool> DeleteMessageDetailByMessageDetailIdAsync(int messageDetailId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.gst_msgrs ");
            sb.Append("WHERE (msg_rcpt_id = @msg_rcpt_id)");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var msg_rcpt_id = cmd.Parameters.Add("@msg_rcpt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    msg_rcpt_id.Value = messageDetailId;

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

        public async Task<bool> DeleteMessageByMessageIdAsync(string messageId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.gst_msgs ");
            sb.Append("WHERE (msg_id = @msg_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var msg_id = cmd.Parameters.Add("@msg_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    msg_id.Value = messageId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception)
            {
                rows = 1;
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }


        #endregion

        //=============== Activity History Action methods ====================//
        #region Activity History Action Methods
        public async Task<bool> InsertActivityHistoryAsync(ActivityHistory activityHistory)
        {
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.sct_act_lgg(act_dt_tm, act_usr_fn, act_dtl, act_src_ip, act_src_tz, act_src_mn, act_src) ");
            sb.Append($"VALUES (@act_dt_tm, @act_usr_fn, @act_dtl, @act_src_ip, @act_src_tz, @act_src_mn, @act_src);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var activityTime = cmd.Parameters.Add("@act_dt_tm", NpgsqlDbType.TimestampTz);
                    var activityUserFullName = cmd.Parameters.Add("@act_usr_fn", NpgsqlDbType.Text);
                    var activityDetails = cmd.Parameters.Add("@act_dtl", NpgsqlDbType.Text);
                    var activitySourceIP = cmd.Parameters.Add("@act_src_ip", NpgsqlDbType.Text);
                    var activitySourceTimeZone = cmd.Parameters.Add("@act_src_tz", NpgsqlDbType.Text);
                    var activitySourceMachineName = cmd.Parameters.Add("@act_src_mn", NpgsqlDbType.Text);
                    var activitySource = cmd.Parameters.Add("@act_src", NpgsqlDbType.Text);
                    cmd.Prepare();
                    activityTime.Value = activityHistory.ActivityTime ?? (object)DBNull.Value;
                    activityUserFullName.Value = activityHistory.ActivityUserFullName ?? (object)DBNull.Value;
                    activityDetails.Value = activityHistory.ActivityDetails ?? (object)DBNull.Value;
                    activitySourceIP.Value = activityHistory.ActivitySourceIP ?? (object)DBNull.Value;
                    activitySourceTimeZone.Value = activityHistory.ActivityTimeZone ?? (object)DBNull.Value;
                    activitySourceMachineName.Value = activityHistory.ActivityMachineName ?? (object)DBNull.Value;
                    activitySource.Value = activityHistory.ActivitySource ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                return false; ;
            }
            return rows > 0;
        }
        #endregion

        //=============== Entity Activity History Action methods =============//
        #region Entity Activity History Action Methods
        public async Task<bool> InsertTaskItemActivityHistoryAsync(TaskItemActivityHistory taskItemActivityHistory)
        {
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.utl_ntt_hst(actv_dsc, ");
            sb.Append($"actv_dt, actv_by, tsk_itm_id) ");
            sb.Append($"VALUES (@actv_dsc, @actv_dt, @actv_by, ");
            sb.Append($"@tsk_itm_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var actv_dt = cmd.Parameters.Add("@actv_dt", NpgsqlDbType.TimestampTz);
                var actv_dsc = cmd.Parameters.Add("@actv_dsc", NpgsqlDbType.Text);
                var actv_by = cmd.Parameters.Add("@actv_by", NpgsqlDbType.Text);
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                cmd.Prepare();
                actv_dt.Value = taskItemActivityHistory.ActivityTime ?? DateTime.UtcNow;
                actv_dsc.Value = taskItemActivityHistory.ActivityDescription ?? (object)DBNull.Value;
                actv_by.Value = taskItemActivityHistory.ActivityBy ?? (object)DBNull.Value;
                tsk_itm_id.Value = taskItemActivityHistory.TaskItemId.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> InsertTaskListActivityHistoryAsync(TaskListActivityHistory taskListActivityHistory)
        {
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.utl_ntt_hst(actv_dsc, ");
            sb.Append($"actv_dt, actv_by, tsk_lst_id) ");
            sb.Append($"VALUES (@actv_dsc, @actv_dt, @actv_by, ");
            sb.Append($"@tsk_lst_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var actv_dt = cmd.Parameters.Add("@actv_dt", NpgsqlDbType.TimestampTz);
                var actv_dsc = cmd.Parameters.Add("@actv_dsc", NpgsqlDbType.Text);
                var actv_by = cmd.Parameters.Add("@actv_by", NpgsqlDbType.Text);
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                actv_dt.Value = taskListActivityHistory.ActivityTime ?? DateTime.UtcNow;
                actv_dsc.Value = taskListActivityHistory.ActivityDescription ?? (object)DBNull.Value;
                actv_by.Value = taskListActivityHistory.ActivityBy ?? (object)DBNull.Value;
                tsk_lst_id.Value = taskListActivityHistory.TaskListId.Value;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<List<TaskItemActivityHistory>> GetTaskItemActivityHistoryByTaskItemIdAsync(long taskItemId)
        {
            List<TaskItemActivityHistory> history = new List<TaskItemActivityHistory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT actv_hst_id, actv_dsc, actv_dt, actv_by, ");
            sb.Append("tsk_itm_id FROM public.utl_ntt_hst ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id) ");
            sb.Append("ORDER BY actv_hst_id DESC;");
            query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                await cmd.PrepareAsync();
                tsk_itm_id.Value = taskItemId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    history.Add(new TaskItemActivityHistory()
                    {
                        ActivityBy = reader["actv_by"] == DBNull.Value ? string.Empty : reader["actv_by"].ToString(),
                        ActivityDescription = reader["actv_dsc"] == DBNull.Value ? string.Empty : reader["actv_dsc"].ToString(),
                        ActivityHistoryId = reader["actv_hst_id"] == DBNull.Value ? 0 : (long)reader["actv_hst_id"],
                        ActivityTime = reader["actv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["actv_dt"],
                        TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? (long?)null : (long)reader["tsk_itm_id"],
                    });
                }
            }
            await conn.CloseAsync();
            return history;
        }

        #endregion

        //=============== Applications Action Methods ========================//
        #region Applications Action Methods
        public async Task<List<SystemApplication>> GetApplicationsAsync()
        {
            List<SystemApplication> applicationsList = new List<SystemApplication>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT app_cd, app_ds FROM public.sct_usr_apps WHERE app_cd != 'SYS';");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        applicationsList.Add(new SystemApplication()
                        {
                            ApplicationID = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : (reader["app_ds"]).ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return applicationsList;
        }
        #endregion

        //=============== Industry Types Action Methods =======================//
        #region Industry Types Action Methods
        public async Task<List<IndustryType>> GetIndustryTypesAsync()
        {
            List<IndustryType> industryTypes = new List<IndustryType>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT id, name FROM public.gst_lst_inds ORDER BY name;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        industryTypes.Add(new IndustryType()
                        {
                            IndustryTypeID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                            IndustryTypeName = reader["name"] == DBNull.Value ? string.Empty : (reader["name"]).ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return industryTypes;
        }
        #endregion
    }
}
