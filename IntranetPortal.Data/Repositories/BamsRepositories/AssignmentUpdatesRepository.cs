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
    public class AssignmentUpdatesRepository : IAssignmentUpdatesRepository
    {
        public IConfiguration _config { get; }
        public AssignmentUpdatesRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Assignment Updates Action Methods ==============================//
        #region Assignment Updates Action Methods

        public async Task<AssignmentUpdates> GetByIdAsync(int Id)
        {
            AssignmentUpdates assignmentUpdates = new AssignmentUpdates();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.assgn_upd_id, u.upd_time, u.upd_desc, u.upd_by, u.assgn_id, ");
            sb.Append("u.upd_typ, u.upd_sts, u.upd_isc, u.cnc_by, u.cnc_time, a.assgnmt_tl ");
            sb.Append("FROM public.bam_assgn_upd u ");
            sb.Append("INNER JOIN bam_assgnmts a ON u.assgn_id = a.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_stt_stts s ON a.status_id = s.stts_id ");
            sb.Append("WHERE (assgn_upd_id = @assgn_upd_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_upd_id = cmd.Parameters.Add("@assgn_upd_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assgn_upd_id.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentUpdates.AssignmentUpdateID = reader["assgn_upd_id"] == DBNull.Value ? 0 : (int)reader["assgn_upd_id"];
                            assignmentUpdates.UpdateType = reader["upd_typ"] == DBNull.Value ? AssignmentUpdateType.Information : (AssignmentUpdateType)reader["upd_typ"];
                            assignmentUpdates.AssignmentEventID = reader["assgn_id"] == DBNull.Value ? 0 : (int)reader["assgn_id"];
                            assignmentUpdates.AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString();
                            assignmentUpdates.UpdateStatus = reader["upd_sts"] == DBNull.Value ? AssignmentUpdateStatus.New : (AssignmentUpdateStatus)reader["upd_sts"];
                            assignmentUpdates.UpdateDescription = reader["upd_desc"] == DBNull.Value ? string.Empty : reader["upd_desc"].ToString();
                            assignmentUpdates.UpdateBy = reader["upd_by"] == DBNull.Value ? string.Empty : reader["upd_by"].ToString();
                            assignmentUpdates.UpdateTime = reader["upd_time"] == DBNull.Value ? string.Empty : reader["upd_time"].ToString();
                            assignmentUpdates.IsCancelled = reader["upd_isc"] == DBNull.Value ? false : (bool)reader["upd_isc"];
                            assignmentUpdates.CancelledBy = reader["cnc_by"] == DBNull.Value ? string.Empty : reader["cnc_by"].ToString();
                            assignmentUpdates.CancelledTime = reader["cnc_time"] == DBNull.Value ? string.Empty : reader["cnc_time"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assignmentUpdates;
        }

        public async Task<IList<AssignmentUpdates>> GetByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<AssignmentUpdates> assignmentUpdates = new List<AssignmentUpdates>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.assgn_upd_id, u.upd_time, u.upd_desc, u.upd_by, u.assgn_id, ");
            sb.Append("u.upd_typ, u.upd_sts, u.upd_isc, u.cnc_by, u.cnc_time, a.assgnmt_tl ");
            sb.Append("FROM public.bam_assgn_upd u ");
            sb.Append("INNER JOIN bam_assgnmts a ON u.assgn_id = a.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_stt_stts s ON a.status_id = s.stts_id ");
            sb.Append("WHERE (u.assgn_id = @assgn_id) ORDER BY u.assgn_upd_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentEventId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentUpdates.Add(new AssignmentUpdates
                            {
                                AssignmentUpdateID = reader["assgn_upd_id"] == DBNull.Value ? 0 : (int)reader["assgn_upd_id"],
                                UpdateType = reader["upd_typ"] == DBNull.Value ? AssignmentUpdateType.Information : (AssignmentUpdateType)reader["upd_typ"],
                                AssignmentEventID = reader["assgn_id"] == DBNull.Value ? 0 : (int)reader["assgn_id"],
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                UpdateStatus = reader["upd_sts"] == DBNull.Value ? AssignmentUpdateStatus.New : (AssignmentUpdateStatus)reader["upd_sts"],
                                UpdateDescription = reader["upd_desc"] == DBNull.Value ? string.Empty : reader["upd_desc"].ToString(),
                                UpdateBy = reader["upd_by"] == DBNull.Value ? string.Empty : reader["upd_by"].ToString(),
                                UpdateTime = reader["upd_time"] == DBNull.Value ? string.Empty : reader["upd_time"].ToString(),
                                IsCancelled = reader["upd_isc"] == DBNull.Value ? false : (bool)reader["upd_isc"],
                                CancelledBy = reader["cnc_by"] == DBNull.Value ? string.Empty : reader["cnc_by"].ToString(),
                                CancelledTime = reader["cnc_time"] == DBNull.Value ? string.Empty : reader["cnc_time"].ToString(),
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
            return assignmentUpdates;
        }

        public async Task<bool> AddAsync(AssignmentUpdates assignmentUpdate)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_assgn_upd(upd_time, upd_desc, upd_by, assgn_id, upd_typ, ");
            sb.Append("upd_sts) VALUES (@upd_time, @upd_desc, @upd_by, @assgn_id, @upd_typ, @upd_sts); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var upd_time = cmd.Parameters.Add("@upd_time", NpgsqlDbType.Text);
                    var upd_desc = cmd.Parameters.Add("@upd_desc", NpgsqlDbType.Text);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Integer);
                    var upd_typ = cmd.Parameters.Add("@upd_typ", NpgsqlDbType.Integer);
                    var upd_sts = cmd.Parameters.Add("@upd_sts", NpgsqlDbType.Integer);
                    var upd_by = cmd.Parameters.Add("@upd_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    upd_desc.Value = assignmentUpdate.UpdateDescription;
                    upd_typ.Value = (int)assignmentUpdate.UpdateType;
                    upd_sts.Value = (int)assignmentUpdate.UpdateStatus;
                    assgn_id.Value = assignmentUpdate.AssignmentEventID;
                    upd_by.Value = assignmentUpdate.UpdateBy ?? (object)DBNull.Value;
                    upd_time.Value = assignmentUpdate.UpdateTime ?? (object)DBNull.Value;
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

        public async Task<bool> UpdateAsync(AssignmentUpdates assignmentUpdate)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bam_assgn_upd	SET upd_time = @upd_time, upd_desc = @upd_desc, ");
            sb.Append("upd_by = @upd_by, assgn_id = @assgn_id, upd_typ = @upd_typ, upd_sts = @upd_sts ");
            sb.Append("upd_isc = @upd_isc, cnc_by = @cnc_by, cnc_time = @cnc_time ");
            sb.Append("WHERE (assgn_upd_id = @assgn_upd_id);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var upd_time = cmd.Parameters.Add("@upd_time", NpgsqlDbType.Text);
                    var upd_desc = cmd.Parameters.Add("@upd_desc", NpgsqlDbType.Text);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Integer);
                    var upd_typ = cmd.Parameters.Add("@upd_typ", NpgsqlDbType.Integer);
                    var upd_sts = cmd.Parameters.Add("@upd_sts", NpgsqlDbType.Integer);
                    var upd_by = cmd.Parameters.Add("@upd_by", NpgsqlDbType.Text);
                    var upd_isc = cmd.Parameters.Add("@upd_isc", NpgsqlDbType.Boolean);
                    var cnc_by = cmd.Parameters.Add("@cnc_by", NpgsqlDbType.Text);
                    var cnc_time = cmd.Parameters.Add("@cnc_time", NpgsqlDbType.Text);

                    cmd.Prepare();
                    upd_desc.Value = assignmentUpdate.UpdateDescription;
                    upd_typ.Value = assignmentUpdate.UpdateType;
                    upd_sts.Value = assignmentUpdate.UpdateStatus;
                    assgn_id.Value = assignmentUpdate.AssignmentEventID;
                    upd_by.Value = assignmentUpdate.UpdateBy ?? (object)DBNull.Value;
                    upd_time.Value = assignmentUpdate.UpdateTime ?? (object)DBNull.Value;
                    upd_isc.Value = assignmentUpdate.IsCancelled;
                    cnc_by.Value = assignmentUpdate.CancelledBy ?? (object)DBNull.Value;
                    cnc_time.Value = assignmentUpdate.CancelledTime ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int assignmentUpdateId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_assgn_upd WHERE (assgn_upd_id = @assgn_upd_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_upd_id = cmd.Parameters.Add("@assgn_upd_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    assgn_upd_id.Value = assignmentUpdateId;
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
