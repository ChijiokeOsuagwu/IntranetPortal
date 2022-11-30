using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Repositories.BamsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.BamsRepositories
{
    public class AssignmentExtensionRepository : IAssignmentExtensionRepository
    {
        public IConfiguration _config { get; }
        public AssignmentExtensionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Assignment Extension Action Methods ==============================//
        #region Assignment Extension Action Methods

        public async Task<AssignmentExtension> GetByIdAsync(int Id)
        {
            AssignmentExtension assignmentExtension = new AssignmentExtension();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT x.assgn_extsn_id, x.xtns_reason, x.xtns_type, x.from_time, x.to_time, ");
            sb.Append("x.assignmt_id, x.createdby, x.createdtime, a.assgnmt_tl, a.status_id, s.stts_nm ");
            sb.Append("FROM public.bam_assgn_xts x INNER JOIN bam_assgnmts a ON x.assignmt_id = a.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_stt_stts s ON a.status_id = s.stts_id ");
            sb.Append("WHERE (x.assgn_extsn_id = @extsn_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var extsn_id = cmd.Parameters.Add("@extsn_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    extsn_id.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentExtension.AssignmentExtensionID = reader["assgn_extsn_id"] == DBNull.Value ? (int?)null : (int)reader["assgn_extsn_id"];
                            assignmentExtension.ExtensionType = reader["xtns_type"] == DBNull.Value ? string.Empty : reader["xtns_type"].ToString();
                            assignmentExtension.AssignmentEventID = reader["assignmt_id"] == DBNull.Value ? (int?)null : (int)reader["assignmt_id"];
                            assignmentExtension.AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString();
                            assignmentExtension.ExtensionReason = reader["xtns_reason"] == DBNull.Value ? string.Empty : reader["xtns_reason"].ToString();
                            assignmentExtension.FromTime = reader["from_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["from_time"];
                            assignmentExtension.ToTime = reader["to_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["to_time"];
                            assignmentExtension.StatusID = reader["status_id"] == DBNull.Value ? 0 : (int)reader["status_id"];
                            assignmentExtension.StatusDescription = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString();
                            assignmentExtension.CreatedBy = reader["createdby"] == DBNull.Value ? string.Empty : reader["createdby"].ToString();
                            assignmentExtension.CreatedTime = reader["createdtime"] == DBNull.Value ? string.Empty : reader["createdtime"].ToString();
                            assignmentExtension.FromTimeFormatted = reader["from_time"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["from_time"]).ToLongDateString()} {((DateTime)reader["from_time"]).ToLongTimeString()}";
                            assignmentExtension.ToTimeFormatted = reader["to_time"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["to_time"]).ToLongDateString()} {((DateTime)reader["to_time"]).ToLongTimeString()}";
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assignmentExtension;
        }

        public async Task<IList<AssignmentExtension>> GetByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<AssignmentExtension> assignmentExtensions = new List<AssignmentExtension>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT x.assgn_extsn_id, x.xtns_reason, x.xtns_type, x.from_time, x.to_time, ");
            sb.Append("x.assignmt_id, x.createdby, x.createdtime, a.assgnmt_tl, a.status_id, s.stts_nm ");
            sb.Append("FROM public.bam_assgn_xts x INNER JOIN bam_assgnmts a ON x.assignmt_id = a.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_stt_stts s ON a.status_id = s.stts_id ");
            sb.Append("WHERE (x.assignmt_id = @assignmt_id) ORDER BY x.assgn_extsn_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assignmt_id = cmd.Parameters.Add("@assignmt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assignmt_id.Value = assignmentEventId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentExtensions.Add(new AssignmentExtension
                            {
                                AssignmentExtensionID = reader["assgn_extsn_id"] == DBNull.Value ? (int?)null : (int)reader["assgn_extsn_id"],
                                AssignmentEventID = reader["assignmt_id"] == DBNull.Value ? (int?)null : (int)reader["assignmt_id"],
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                ExtensionReason = reader["xtns_reason"] == DBNull.Value ? string.Empty : reader["xtns_reason"].ToString(),
                                ExtensionType = reader["xtns_type"] == DBNull.Value ? string.Empty : reader["xtns_type"].ToString(),
                                FromTime = reader["from_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["from_time"],
                                ToTime = reader["to_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["to_time"],
                                StatusID = reader["status_id"] == DBNull.Value ? 0 : (int)reader["status_id"],
                                StatusDescription = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString(),
                                CreatedBy = reader["createdby"] == DBNull.Value ? string.Empty : reader["createdby"].ToString(),
                                CreatedTime = reader["createdtime"] == DBNull.Value ? string.Empty : reader["createdtime"].ToString(),
                                FromTimeFormatted = reader["from_time"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["from_time"]).ToLongDateString()} {((DateTime)reader["from_time"]).ToLongTimeString()}",
                                ToTimeFormatted = reader["to_time"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["to_time"]).ToLongDateString()} {((DateTime)reader["to_time"]).ToLongTimeString()}",
                            }) ;
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assignmentExtensions;
        }

        public async Task<bool> AddAsync(AssignmentExtension assignmentExtension)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_assgn_xts(xtns_reason, xtns_type, from_time, ");
            sb.Append("to_time, assignmt_id, createdby, createdtime) VALUES (@xtns_reason, ");
            sb.Append("@xtns_type, @from_time, @to_time, @assignmt_id, @createdby, @createdtime); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var xtns_reason = cmd.Parameters.Add("@xtns_reason", NpgsqlDbType.Text);
                    var xtns_type = cmd.Parameters.Add("@xtns_type", NpgsqlDbType.Text);
                    var assignmt_id = cmd.Parameters.Add("@assignmt_id", NpgsqlDbType.Integer);
                    var from_time = cmd.Parameters.Add("@from_time", NpgsqlDbType.TimestampTz);
                    var to_time = cmd.Parameters.Add("@to_time", NpgsqlDbType.TimestampTz);
                    var createdby = cmd.Parameters.Add("@createdby", NpgsqlDbType.Text);
                    var createdtime = cmd.Parameters.Add("@createdtime", NpgsqlDbType.Text);
                    cmd.Prepare();
                    xtns_reason.Value = assignmentExtension.ExtensionReason;
                    xtns_type.Value = assignmentExtension.ExtensionType;
                    assignmt_id.Value = assignmentExtension.AssignmentEventID;
                    from_time.Value = assignmentExtension.FromTime ?? DateTime.Now;
                    to_time.Value = assignmentExtension.ToTime ?? DateTime.Now;
                    createdby.Value = assignmentExtension.CreatedBy ?? (object)DBNull.Value;
                    createdtime.Value = assignmentExtension.CreatedTime ?? (object)DBNull.Value;
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

        public async Task<bool> DeleteAsync(int assignmentExtensionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_assgn_xts WHERE (assgn_extsn_id = @assgn_extsn_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_extsn_id = cmd.Parameters.Add("@assgn_extsn_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    assgn_extsn_id.Value = assignmentExtensionId;
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

        #endregion
    }
}
