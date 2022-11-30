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
    public class DeploymentEquipmentRepository : IDeploymentEquipmentRepository
    {
        public IConfiguration _config { get; }
        public DeploymentEquipmentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<bool> AddAsync(DeploymentEquipment deploymentEquipment)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_dpmt_eqmt(assgnmt_id, dpmnt_id, eqmt_asset_id, status, ");
            sb.Append("mdb, mdt, ctb, ctt, asst_typ_id, usg_id, prev_loc, prev_sts) VALUES (@assgnmt_id, ");
            sb.Append("@dpmnt_id, @eqmt_asset_id, @status, @mdb, @mdt, @ctb, @ctt, @asst_typ_id, @usg_id, ");
            sb.Append("@prev_loc, @prev_sts);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    var dpmnt_id = cmd.Parameters.Add("@dpmnt_id", NpgsqlDbType.Integer);
                    var eqmt_asset_id = cmd.Parameters.Add("@eqmt_asset_id", NpgsqlDbType.Text);
                    var status = cmd.Parameters.Add("@status", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var usg_id = cmd.Parameters.Add("@usg_id", NpgsqlDbType.Integer);
                    var prev_loc = cmd.Parameters.Add("@prev_loc", NpgsqlDbType.Text);
                    var prev_sts = cmd.Parameters.Add("@prev_sts", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assgnmt_id.Value = deploymentEquipment.AssignmentEventID;
                    dpmnt_id.Value = deploymentEquipment.DeploymentID;
                    eqmt_asset_id.Value = deploymentEquipment.AssetID;
                    status.Value = deploymentEquipment.EquipmentDeploymentStatus ?? (object)DBNull.Value;
                    mdb.Value = deploymentEquipment.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = deploymentEquipment.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = deploymentEquipment.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = deploymentEquipment.CreatedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = deploymentEquipment.AssetTypeID;
                    usg_id.Value = deploymentEquipment.EquipmentUsageID ?? (object)DBNull.Value;
                    prev_loc.Value = deploymentEquipment.PreviousLocation ?? (object)DBNull.Value;
                    prev_sts.Value = deploymentEquipment.PreviousAvailabilityStatus ?? (object)DBNull.Value;
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

        public async Task<bool> EditAsync(DeploymentEquipment deploymentEquipment)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bam_dpmt_eqmt SET assgnmt_id = @assgnmt_id, ");
            sb.Append("dpmnt_id = @dpmnt_id, eqmt_asset_id = @eqmt_asset_id, status = @status, ");
            sb.Append("mdb = @mdb, mdt = @mdt, asst_typ_id = @asst_typ_id, usg_id = @usg_id, ");
            sb.Append("prev_loc = @prev_loc, prev_sts = @prev_sts ");
            sb.Append("WHERE (dpmt_eqmt_id = @dpmt_eqmt_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_eqmt_id = cmd.Parameters.Add("@dpmt_eqmt_id", NpgsqlDbType.Integer);
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    var dpmnt_id = cmd.Parameters.Add("@dpmnt_id", NpgsqlDbType.Integer);
                    var eqmt_asset_id = cmd.Parameters.Add("@eqmt_asset_id", NpgsqlDbType.Text);
                    var status = cmd.Parameters.Add("@status", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var usg_id = cmd.Parameters.Add("@usg_id", NpgsqlDbType.Integer);
                    var prev_loc = cmd.Parameters.Add("@prev_loc", NpgsqlDbType.Text);
                    var prev_sts = cmd.Parameters.Add("@prev_sts", NpgsqlDbType.Text);
                    cmd.Prepare();
                    dpmt_eqmt_id.Value = deploymentEquipment.ID;
                    assgnmt_id.Value = deploymentEquipment.AssignmentEventID;
                    dpmnt_id.Value = deploymentEquipment.DeploymentID;
                    eqmt_asset_id.Value = deploymentEquipment.AssetID ?? (object)DBNull.Value;
                    status.Value = deploymentEquipment.EquipmentDeploymentStatus ?? (object)DBNull.Value;
                    mdb.Value = deploymentEquipment.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = deploymentEquipment.ModifiedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = deploymentEquipment.AssetTypeID;
                    usg_id.Value = deploymentEquipment.EquipmentUsageID ?? (object)DBNull.Value;
                    prev_loc.Value = deploymentEquipment.PreviousLocation ?? (object)DBNull.Value;
                    prev_sts.Value = deploymentEquipment.PreviousAvailabilityStatus ?? (object)DBNull.Value;


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

        public async Task<bool> DeleteAsync(int deploymentEquipmentId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_dpmt_eqmt WHERE (dpmt_eqmt_id = @dpmt_eqmt_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_eqmt_id = cmd.Parameters.Add("@dpmt_eqmt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    dpmt_eqmt_id.Value = deploymentEquipmentId;
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
            string query = $"DELETE FROM public.bam_dpmt_eqmt WHERE (dpmnt_id = @dpmnt_id);";
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

        public async Task<IList<DeploymentEquipment>> GetByIdAsync(int deploymentEquipmentId)
        {
            IList<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.dpmt_eqmt_id, e.assgnmt_id, e.dpmnt_id, e.eqmt_asset_id, ");
            sb.Append("e.status, e.mdb, e.mdt, e.ctb, e.ctt, e.usg_id, e.prev_loc, e.prev_sts, ");
            sb.Append("a.assgnmt_tl, d.dpmt_tl, s.asst_nm, e.asst_typ_id, p.typ_nm ");
            sb.Append("FROM public.bam_dpmt_eqmt e ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = e.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = e.dpmnt_id ");
            sb.Append("INNER JOIN public.asm_stt_asst s ON s.asst_id = e.eqmt_asset_id ");
            sb.Append("INNER JOIN public.asm_stt_typs p ON p.typ_id = e.asst_typ_id ");
            sb.Append("WHERE (e.dpmt_eqmt_id = @dpmt_eqmt_id) ORDER BY e.dpmt_eqmt_id; ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dpmt_eqmt_id = cmd.Parameters.Add("@dpmt_eqmt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    dpmt_eqmt_id.Value = deploymentEquipmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            equipments.Add(new DeploymentEquipment
                            {
                                ID = reader["dpmt_eqmt_id"] == DBNull.Value ? 0 : (int)reader["dpmt_eqmt_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                AssetID = reader["eqmt_asset_id"] == DBNull.Value ? string.Empty : reader["eqmt_asset_id"].ToString(),
                                EquipmentDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                EquipmentUsageID = reader["usg_id"] == DBNull.Value ? (int?)null : (int)reader["usg_id"],
                                PreviousAvailabilityStatus = reader["prev_sts"] == DBNull.Value ? string.Empty : reader["prev_sts"].ToString(),
                                PreviousLocation = reader["prev_loc"] == DBNull.Value ? string.Empty : reader["prev_loc"].ToString()
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
            return equipments;
        }

        public async Task<IList<DeploymentEquipment>> GetByAssignmentIdAndAssetIdAsync(int assignmentEventId, string assetId)
        {
            IList<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.dpmt_eqmt_id, e.assgnmt_id, e.dpmnt_id, e.eqmt_asset_id, ");
            sb.Append("e.status, e.mdb, e.mdt, e.ctb, e.ctt, e.usg_id, e.prev_loc, e.prev_sts, ");
            sb.Append("a.assgnmt_tl, d.dpmt_tl, s.asst_nm, e.asst_typ_id, p.typ_nm ");
            sb.Append("FROM public.bam_dpmt_eqmt e ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = e.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = e.dpmnt_id ");
            sb.Append("INNER JOIN public.asm_stt_asst s ON s.asst_id = e.eqmt_asset_id ");
            sb.Append("INNER JOIN public.asm_stt_typs p ON p.typ_id = e.asst_typ_id ");
            sb.Append("WHERE (e.assgnmt_id = @assgnmt_id AND e.eqmt_asset_id = @eqmt_asset_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    var eqmt_asset_id = cmd.Parameters.Add("@eqmt_asset_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assgnmt_id.Value = assignmentEventId;
                    eqmt_asset_id.Value = assetId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            equipments.Add(new DeploymentEquipment
                            {
                                ID = reader["dpmt_eqmt_id"] == DBNull.Value ? 0 : (int)reader["dpmt_eqmt_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                AssetID = reader["eqmt_asset_id"] == DBNull.Value ? string.Empty : reader["eqmt_asset_id"].ToString(),
                                EquipmentDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                EquipmentUsageID = reader["usg_id"] == DBNull.Value ? (int?)null : (int)reader["usg_id"],
                                PreviousAvailabilityStatus = reader["prev_sts"] == DBNull.Value ? string.Empty : reader["prev_sts"].ToString(),
                                PreviousLocation = reader["prev_loc"] == DBNull.Value ? string.Empty : reader["prev_loc"].ToString()
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
            return equipments;
        }

        public async Task<IList<DeploymentEquipment>> GetByDeploymentIdAsync(int assignmentDeploymentId)
        {
            IList<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.dpmt_eqmt_id, e.assgnmt_id, e.dpmnt_id, e.eqmt_asset_id, ");
            sb.Append("e.status, e.mdb, e.mdt, e.ctb, e.ctt, e.usg_id, e.prev_loc, e.prev_sts, ");
            sb.Append("a.assgnmt_tl, d.dpmt_tl, s.asst_nm, e.asst_typ_id, p.typ_nm ");
            sb.Append("FROM public.bam_dpmt_eqmt e ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = e.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = e.dpmnt_id ");
            sb.Append("INNER JOIN public.asm_stt_asst s ON s.asst_id = e.eqmt_asset_id ");
            sb.Append("INNER JOIN public.asm_stt_typs p ON p.typ_id = e.asst_typ_id ");
            sb.Append("WHERE (e.dpmnt_id = @dpmnt_id) ORDER BY e.dpmt_eqmt_id ASC");
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
                            equipments.Add(new DeploymentEquipment
                            {
                                ID = reader["dpmt_eqmt_id"] == DBNull.Value ? 0 : (int)reader["dpmt_eqmt_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                AssetID = reader["eqmt_asset_id"] == DBNull.Value ? string.Empty : reader["eqmt_asset_id"].ToString(),
                                EquipmentDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                EquipmentUsageID = reader["usg_id"] == DBNull.Value ? (int?)null : (int)reader["usg_id"],
                                PreviousAvailabilityStatus = reader["prev_sts"] == DBNull.Value ? string.Empty : reader["prev_sts"].ToString(),
                                PreviousLocation = reader["prev_loc"] == DBNull.Value ? string.Empty : reader["prev_loc"].ToString()
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
            return equipments;
        }

        public async Task<IList<DeploymentEquipment>> GetByAssignmentEventIdAsync(int assignmentEventId)
        {
            IList<DeploymentEquipment> equipments = new List<DeploymentEquipment>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.dpmt_eqmt_id, e.assgnmt_id, e.dpmnt_id, e.eqmt_asset_id, ");
            sb.Append("e.status, e.mdb, e.mdt, e.ctb, e.ctt, e.usg_id, e.prev_loc, e.prev_sts, ");
            sb.Append("a.assgnmt_tl, d.dpmt_tl, s.asst_nm, e.asst_typ_id, p.typ_nm ");
            sb.Append("FROM public.bam_dpmt_eqmt e ");
            sb.Append("INNER JOIN public.bam_assgnmts a ON a.assgnmt_id = e.assgnmt_id ");
            sb.Append("INNER JOIN public.bam_dpmnts d ON d.dpmt_id = e.dpmnt_id ");
            sb.Append("INNER JOIN public.asm_stt_asst s ON s.asst_id = e.eqmt_asset_id ");
            sb.Append("INNER JOIN public.asm_stt_typs p ON p.typ_id = e.asst_typ_id ");
            sb.Append("WHERE (e.assgnmt_id = @assgnmt_id) ORDER BY e.dpmnt_id, e.dpmt_eqmt_id ASC");
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
                            equipments.Add(new DeploymentEquipment
                            {
                                ID = reader["dpmt_eqmt_id"] == DBNull.Value ? 0 : (int)reader["dpmt_eqmt_id"],
                                AssignmentEventID = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                                DeploymentID = reader["dpmnt_id"] == DBNull.Value ? 0 : (int)reader["dpmnt_id"],
                                AssetID = reader["eqmt_asset_id"] == DBNull.Value ? string.Empty : reader["eqmt_asset_id"].ToString(),
                                EquipmentDeploymentStatus = reader["status"] == DBNull.Value ? string.Empty : reader["status"].ToString(),
                                AssignmentEventTitle = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                DeploymentTitle = reader["dpmt_tl"] == DBNull.Value ? string.Empty : reader["dpmt_tl"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                EquipmentUsageID = reader["usg_id"] == DBNull.Value ? (int?)null : (int)reader["usg_id"],
                                PreviousAvailabilityStatus = reader["prev_sts"] == DBNull.Value ? string.Empty : reader["prev_sts"].ToString(),
                                PreviousLocation = reader["prev_loc"] == DBNull.Value ? string.Empty : reader["prev_loc"].ToString()
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
            return equipments;
        }
    }
}
