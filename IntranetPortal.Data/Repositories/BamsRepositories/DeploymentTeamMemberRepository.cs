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
    public class DeploymentTeamMemberRepository : IDeploymentTeamMemberRepository
    {
        public IConfiguration _config { get; }
        public DeploymentTeamMemberRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<bool> AddAsync(DeploymentTeamMember deploymentTeamMember)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_dpmt_tms (assgnmt_id, dpmnt_id, tm_mmb_id, ");
            sb.Append("tm_mmb_rl, status, mdb, mdt, ctb, ctt, unit_nm, station_nm) VALUES ");
            sb.Append("(@assgnmt_id, @dpmnt_id, @tm_mmb_id, @tm_mmb_rl, @status,");
            sb.Append(" @mdb, @mdt, @ctb, @ctt, @unit_nm, @station_nm); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    var dpmnt_id = cmd.Parameters.Add("@dpmnt_id", NpgsqlDbType.Integer);
                    var tm_mmb_id = cmd.Parameters.Add("@tm_mmb_id", NpgsqlDbType.Text);
                    var tm_mmb_rl = cmd.Parameters.Add("@tm_mmb_rl", NpgsqlDbType.Text);
                    var status = cmd.Parameters.Add("@status", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    var unit_nm = cmd.Parameters.Add("@unit_nm", NpgsqlDbType.Text);
                    var station_nm = cmd.Parameters.Add("@station_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assgnmt_id.Value = deploymentTeamMember.AssignmentEventID;
                    dpmnt_id.Value = deploymentTeamMember.DeploymentID;
                    tm_mmb_id.Value = deploymentTeamMember.TeamMemberID ?? (object)DBNull.Value;
                    tm_mmb_rl.Value = deploymentTeamMember.TeamMemberRole ?? (object)DBNull.Value;
                    status.Value = deploymentTeamMember.TeamDeploymentStatus ?? (object)DBNull.Value;
                    mdb.Value = deploymentTeamMember.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = deploymentTeamMember.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = deploymentTeamMember.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = deploymentTeamMember.CreatedTime ?? (object)DBNull.Value;
                    unit_nm.Value = deploymentTeamMember.TeamMemberUnit ?? (object)DBNull.Value;
                    station_nm.Value = deploymentTeamMember.TeamMemberStation ?? (object)DBNull.Value;
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

        public async Task<bool> EditAsync(DeploymentTeamMember deploymentTeamMember)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bam_dpmt_tms SET  assgnmt_id = @assgnmt_id, ");
            sb.Append("dpmnt_id = @dpmnt_id, tm_mmb_id = @tm_mmb_id, tm_mmb_rl = @tm_mmb_rl, ");
            sb.Append("status = @status, mdb = @mdb, mdt = @mdt, unit_nm = @unit_nm,  ");
            sb.Append("station_nm = @station_nm WHERE (dpmt_tm_id = @dpmt_tm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_tm_id = cmd.Parameters.Add("@dpmt_tm_id", NpgsqlDbType.Integer);
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    var dpmnt_id = cmd.Parameters.Add("@dpmnt_id", NpgsqlDbType.Integer);
                    var tm_mmb_id = cmd.Parameters.Add("@tm_mmb_id", NpgsqlDbType.Text);
                    var tm_mmb_rl = cmd.Parameters.Add("@tm_mmb_rl", NpgsqlDbType.Text);
                    var status = cmd.Parameters.Add("@status", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    var unit_nm = cmd.Parameters.Add("@unit_nm", NpgsqlDbType.Text);
                    var station_nm = cmd.Parameters.Add("@station_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    dpmt_tm_id.Value = deploymentTeamMember.DeploymentTeamID;
                    assgnmt_id.Value = deploymentTeamMember.AssignmentEventID;
                    dpmnt_id.Value = deploymentTeamMember.DeploymentID;
                    tm_mmb_id.Value = deploymentTeamMember.TeamMemberID ?? (object)DBNull.Value;
                    tm_mmb_rl.Value = deploymentTeamMember.TeamMemberRole ?? (object)DBNull.Value;
                    status.Value = deploymentTeamMember.TeamDeploymentStatus ?? (object)DBNull.Value;
                    mdb.Value = deploymentTeamMember.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = deploymentTeamMember.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = deploymentTeamMember.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = deploymentTeamMember.CreatedTime ?? (object)DBNull.Value;
                    unit_nm.Value = deploymentTeamMember.TeamMemberUnit ?? (object)DBNull.Value;
                    station_nm.Value = deploymentTeamMember.TeamMemberStation ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int deploymentTeamMemberId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_dpmt_tms WHERE (dpmt_tm_id = @dpmt_tm_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_tm_id = cmd.Parameters.Add("@dpmt_tm_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    dpmt_tm_id.Value = deploymentTeamMemberId;
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

        public async Task<bool> DeleteByDeploymentIdAsync(int deploymentId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_dpmt_tms WHERE (dpmnt_id = @dpmnt_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmnt_id = cmd.Parameters.Add("@dpmnt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    dpmnt_id.Value = deploymentId;
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

        public async Task<IList<DeploymentTeamMember>> GetByIdAsync(int deploymentTeamMemberId)
        {
            IList<DeploymentTeamMember> members = new List<DeploymentTeamMember>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.dpmt_tm_id, t.assgnmt_id, t.dpmnt_id, t.tm_mmb_id, ");
            sb.Append("t.tm_mmb_rl, t.status, a.assgnmt_tl, d.dpmt_tl, p.fullname, ");
            sb.Append("t.mdb, t.mdt, t.ctb, t.ctt, t.unit_nm, t.station_nm ");
            sb.Append("FROM public.bam_dpmt_tms t ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = t.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = t.dpmnt_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = t.tm_mmb_id ");
            sb.Append("WHERE (t.dpmt_tm_id = @dpmt_tm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_tm_id = cmd.Parameters.Add("@dpmt_tm_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    dpmt_tm_id.Value = deploymentTeamMemberId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            members.Add(new DeploymentTeamMember
                            {
                                DeploymentTeamID = reader["dpmt_tm_id"] == DBNull.Value ? 0 : (int)reader["dpmt_tm_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                TeamMemberID = reader["tm_mmb_id"] == DBNull.Value ? string.Empty : reader["tm_mmb_id"].ToString(),
                                TeamMemberRole = reader["tm_mmb_rl"] == DBNull.Value ? string.Empty : reader["tm_mmb_rl"].ToString(),
                                TeamDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                TeamMemberName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                TeamMemberUnit = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                                TeamMemberStation = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString()
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
            return members;
        }

        public async Task<IList<DeploymentTeamMember>> GetByAssignmentIdAndPersonIdAsync(int assignmentEventId, string personId)
        {
            IList<DeploymentTeamMember> members = new List<DeploymentTeamMember>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.dpmt_tm_id, t.assgnmt_id, t.dpmnt_id, t.tm_mmb_id, ");
            sb.Append("t.tm_mmb_rl, t.status, a.assgnmt_tl, d.dpmt_tl, p.fullname, ");
            sb.Append("t.mdb, t.mdt, t.ctb, t.ctt, t.unit_nm, t.station_nm ");
            sb.Append("FROM public.bam_dpmt_tms t ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = t.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = t.dpmnt_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = t.tm_mmb_id ");
            sb.Append("WHERE (t.assgnmt_id = @assgnmt_id AND t.tm_mmb_id = @tm_mmb_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    var tm_mmb_id = cmd.Parameters.Add("@tm_mmb_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assgnmt_id.Value = assignmentEventId;
                    tm_mmb_id.Value = personId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            members.Add(new DeploymentTeamMember
                            {
                                DeploymentTeamID = reader["dpmt_tm_id"] == DBNull.Value ? 0 : (int)reader["dpmt_tm_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                TeamMemberID = reader["tm_mmb_id"] == DBNull.Value ? string.Empty : reader["tm_mmb_id"].ToString(),
                                TeamMemberRole = reader["tm_mmb_rl"] == DBNull.Value ? string.Empty : reader["tm_mmb_rl"].ToString(),
                                TeamDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                TeamMemberName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                TeamMemberUnit = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                                TeamMemberStation = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString()
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
            return members;
        }
        
        public async Task<IList<DeploymentTeamMember>> GetByDeploymentIdAsync(int assignmentDeploymentId)
        {
            IList<DeploymentTeamMember> members = new List<DeploymentTeamMember>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.dpmt_tm_id, t.assgnmt_id, t.dpmnt_id, t.tm_mmb_id, ");
            sb.Append("t.tm_mmb_rl, t.status, a.assgnmt_tl, d.dpmt_tl, p.fullname, ");
            sb.Append("t.mdb, t.mdt, t.ctb, t.ctt, t.unit_nm, t.station_nm ");
            sb.Append("FROM public.bam_dpmt_tms t ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = t.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = t.dpmnt_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = t.tm_mmb_id ");
            sb.Append("WHERE (t.dpmnt_id = @dpmnt_id) ORDER BY t.dpmt_tm_id ASC");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmnt_id = cmd.Parameters.Add("@dpmnt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    dpmnt_id.Value = assignmentDeploymentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            members.Add(new DeploymentTeamMember
                            {
                                DeploymentTeamID = reader["dpmt_tm_id"] == DBNull.Value ? 0 : (int)reader["dpmt_tm_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                TeamMemberID = reader["tm_mmb_id"] == DBNull.Value ? string.Empty : reader["tm_mmb_id"].ToString(),
                                TeamMemberRole = reader["tm_mmb_rl"] == DBNull.Value ? string.Empty : reader["tm_mmb_rl"].ToString(),
                                TeamDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                TeamMemberName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                TeamMemberUnit = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                                TeamMemberStation = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString()
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
            return members;
        }

        public async Task<IList<DeploymentTeamMember>> GetByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<DeploymentTeamMember> members = new List<DeploymentTeamMember>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.dpmt_tm_id, t.assgnmt_id, t.dpmnt_id, t.tm_mmb_id, ");
            sb.Append("t.tm_mmb_rl, t.status, a.assgnmt_tl, d.dpmt_tl, p.fullname, ");
            sb.Append("t.mdb, t.mdt, t.ctb, t.ctt, t.unit_nm, t.station_nm ");
            sb.Append("FROM public.bam_dpmt_tms t  ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = t.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = t.dpmnt_id ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = t.tm_mmb_id ");
            sb.Append("WHERE (t.assgnmt_id = @assgnmt_id) ORDER BY t.dpmnt_id, t.dpmt_tm_id ASC");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assgnmt_id.Value = assignmentEventId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            members.Add(new DeploymentTeamMember
                            {
                                DeploymentTeamID = reader["dpmt_tm_id"] == DBNull.Value ? 0 : (int)reader["dpmt_tm_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                TeamMemberID = reader["tm_mmb_id"] == DBNull.Value ? string.Empty : reader["tm_mmb_id"].ToString(),
                                TeamMemberRole = reader["tm_mmb_rl"] == DBNull.Value ? string.Empty : reader["tm_mmb_rl"].ToString(),
                                TeamDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                TeamMemberName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                TeamMemberUnit = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                                TeamMemberStation = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString()
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
            return members;
        }
    }
}
