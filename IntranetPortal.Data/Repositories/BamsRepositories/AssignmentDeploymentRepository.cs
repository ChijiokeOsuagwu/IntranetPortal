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
    public class AssignmentDeploymentRepository : IAssignmentDeploymentRepository
    {
        public IConfiguration _config { get; }
        public AssignmentDeploymentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== AssignmentDeployment Action Methods =====================================================//
        #region AssignmentDeployment Action Methods

        public async Task<AssignmentDeployment> GetByIdAsync(int Id)
        {
            AssignmentDeployment assignmentDeployment = new AssignmentDeployment();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.dpmt_id, d.assgmnt_id, d.departure_time, d.lead_nm, ");
            sb.Append("d.lead_phn, d.status_id, d.arrival_time, d.mdb, d.mdt, d.ctb, ");
            sb.Append("d.ctt, d.dpmt_tl, d.station_id, a.assgnmt_tl, a.assgnmt_ds, ");
            sb.Append("l.locname, s.stts_nm FROM public.bam_dpmnts d ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON d.assgmnt_id = a.assgnmt_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = d.station_id ");
            sb.Append("INNER JOIN public.bam_stt_stts s ON s.stts_id = d.status_id ");
            sb.Append("WHERE (d.dpmt_id = @dpmt_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_id = cmd.Parameters.Add("@dpmt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    dpmt_id.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentDeployment.DeploymentID = reader["dpmt_id"] == DBNull.Value ? 0 : (int)reader["dpmt_id"];
                            assignmentDeployment.AssignmentEventID = reader["assgmnt_id"] == DBNull.Value ? 0 : (int)reader["assgmnt_id"];
                            assignmentDeployment.TeamLeadName = reader["lead_nm"] == DBNull.Value ? string.Empty : reader["lead_nm"].ToString();
                            assignmentDeployment.TeamLeadPhone = reader["lead_phn"] == DBNull.Value ? string.Empty : reader["lead_phn"].ToString();
                            assignmentDeployment.StatusID = reader["status_id"] == DBNull.Value ? (int?)null : (int)reader["status_id"];
                            assignmentDeployment.StatusDescription = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString();
                            assignmentDeployment.StationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            assignmentDeployment.StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            assignmentDeployment.DepartureTime = reader["departure_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["departure_time"];
                            assignmentDeployment.ArrivalTime = reader["arrival_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["arrival_time"];
                            assignmentDeployment.AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString();
                            assignmentDeployment.AssignmentEventDescription = reader["assgnmt_ds"] == DBNull.Value ? string.Empty : reader["assgnmt_ds"].ToString();
                            assignmentDeployment.DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString();
                            assignmentDeployment.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            assignmentDeployment.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            assignmentDeployment.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            assignmentDeployment.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assignmentDeployment;
        }

        public async Task<IList<AssignmentDeployment>> GetByAssignmentIdAsync(int assignmentEventId)
        {
            IList<AssignmentDeployment> assignmentDeployments = new List<AssignmentDeployment>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.dpmt_id, d.assgmnt_id, d.departure_time, d.lead_nm, ");
            sb.Append("d.lead_phn, d.status_id, d.arrival_time, d.mdb, d.mdt, d.ctb, ");
            sb.Append("d.ctt, d.dpmt_tl, d.station_id, a.assgnmt_tl, a.assgnmt_ds, ");
            sb.Append("l.locname, s.stts_nm FROM public.bam_dpmnts d ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON d.assgmnt_id = a.assgnmt_id ");
            sb.Append("INNER JOIN public.gst_locs l ON l.locqk = d.station_id ");
            sb.Append("INNER JOIN public.bam_stt_stts s ON s.stts_id = d.status_id ");
            sb.Append("WHERE (d.assgmnt_id = @assgmnt_id) ");
            sb.Append($"ORDER BY dpmt_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgmnt_id = cmd.Parameters.Add("assgmnt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assgmnt_id.Value = assignmentEventId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentDeployments.Add(new AssignmentDeployment
                            {
                                DeploymentID = reader["dpmt_id"] == DBNull.Value ? 0 : (int)reader["dpmt_id"],
                                AssignmentEventID = reader["assgmnt_id"] == DBNull.Value ? 0 : (int)reader["assgmnt_id"],
                                TeamLeadName = reader["lead_nm"] == DBNull.Value ? string.Empty : reader["lead_nm"].ToString(),
                                TeamLeadPhone = reader["lead_phn"] == DBNull.Value ? string.Empty : reader["lead_phn"].ToString(),
                                StatusID = reader["status_id"] == DBNull.Value ? (int?)null : (int)reader["status_id"],
                                StatusDescription = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString(),
                                StationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                DepartureTime = reader["departure_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["departure_time"],
                                ArrivalTime = reader["arrival_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["arrival_time"],
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                AssignmentEventDescription = reader["assgnmt_ds"] == DBNull.Value ? string.Empty : reader["assgnmt_ds"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString()
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
            return assignmentDeployments;
        }

        #endregion

        //============== AssignmentDeployment CRUD Action Methods =================================================//
        #region AssignmentDeployment CRUD Action Methods
        public async Task<bool> AddAsync(AssignmentDeployment assignmentDeployment)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_dpmnts(assgmnt_id, departure_time, lead_nm, ");
            sb.Append("lead_phn, mdb, mdt, ctb, ctt, dpmt_tl, status_id, arrival_time, station_id) ");
            sb.Append("VALUES (@assgmnt_id, @departure_time, @lead_nm, @lead_phn, @mdb, ");
            sb.Append("@mdt, @ctb, @ctt, @dpmt_tl, @status_id, @arrival_time, @station_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgmnt_id = cmd.Parameters.Add("@assgmnt_id", NpgsqlDbType.Integer);
                    var departure_time = cmd.Parameters.Add("@departure_time", NpgsqlDbType.Timestamp);
                    var lead_nm = cmd.Parameters.Add("@lead_nm", NpgsqlDbType.Text);
                    var lead_phn = cmd.Parameters.Add("@lead_phn", NpgsqlDbType.Text);
                    var arrival_time = cmd.Parameters.Add("@arrival_time", NpgsqlDbType.Timestamp);
                    var dpmt_tl = cmd.Parameters.Add("@dpmt_tl", NpgsqlDbType.Text);
                    var status_id = cmd.Parameters.Add("@status_id", NpgsqlDbType.Integer);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assgmnt_id.Value = assignmentDeployment.AssignmentEventID;
                    departure_time.Value = assignmentDeployment.DepartureTime ?? (object)DBNull.Value;
                    lead_nm.Value = assignmentDeployment.TeamLeadName ?? (object)DBNull.Value;
                    lead_phn.Value = assignmentDeployment.TeamLeadPhone ?? (object)DBNull.Value;
                    arrival_time.Value = assignmentDeployment.ArrivalTime ?? (object)DBNull.Value;
                    dpmt_tl.Value = assignmentDeployment.DeploymentTitle;
                    status_id.Value = assignmentDeployment.StatusID ?? (object)DBNull.Value;
                    station_id.Value = assignmentDeployment.StationID;
                    mdb.Value = assignmentDeployment.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentDeployment.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = assignmentDeployment.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = assignmentDeployment.CreatedTime ?? (object)DBNull.Value;
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

        public async Task<bool> EditAsync(AssignmentDeployment assignmentDeployment)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bam_dpmnts SET assgmnt_id = @assgmnt_id, ");
            sb.Append("departure_time = @departure_time, lead_nm = @lead_nm, lead_phn = @lead_phn, ");
            sb.Append("mdb = @mdb, mdt = @mdt, dpmt_tl = @dpmt_tl, station_id = @station_id, ");
            sb.Append("status_id = @status_id, arrival_time = @arrival_time ");
            sb.Append("WHERE (dpmt_id = @dpmt_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_id = cmd.Parameters.Add("@dpmt_id", NpgsqlDbType.Integer);
                    var assgmnt_id = cmd.Parameters.Add("@assgmnt_id", NpgsqlDbType.Integer);
                    var departure_time = cmd.Parameters.Add("@departure_time", NpgsqlDbType.Timestamp);
                    var lead_nm = cmd.Parameters.Add("@lead_nm", NpgsqlDbType.Text);
                    var lead_phn = cmd.Parameters.Add("@lead_phn", NpgsqlDbType.Text);
                    var arrival_time = cmd.Parameters.Add("@arrival_time", NpgsqlDbType.Timestamp);
                    var dpmt_tl = cmd.Parameters.Add("@dpmt_tl", NpgsqlDbType.Text);
                    var status_id = cmd.Parameters.Add("@status_id", NpgsqlDbType.Integer);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    cmd.Prepare();
                    dpmt_id.Value = assignmentDeployment.DeploymentID;
                    assgmnt_id.Value = assignmentDeployment.AssignmentEventID;
                    departure_time.Value = assignmentDeployment.DepartureTime ?? (object)DBNull.Value;
                    lead_nm.Value = assignmentDeployment.TeamLeadName ?? (object)DBNull.Value;
                    lead_phn.Value = assignmentDeployment.TeamLeadPhone ?? (object)DBNull.Value;
                    arrival_time.Value = assignmentDeployment.ArrivalTime ?? (object)DBNull.Value;
                    dpmt_tl.Value = assignmentDeployment.DeploymentTitle;
                    status_id.Value = assignmentDeployment.StatusID ?? (object)DBNull.Value;
                    station_id.Value = assignmentDeployment.StationID;
                    mdb.Value = assignmentDeployment.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentDeployment.ModifiedTime ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int assignmentDeploymentId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_dpmnts WHERE (dpmt_id = @dpmt_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_id = cmd.Parameters.Add("@dpmt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    dpmt_id.Value = assignmentDeploymentId;
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
