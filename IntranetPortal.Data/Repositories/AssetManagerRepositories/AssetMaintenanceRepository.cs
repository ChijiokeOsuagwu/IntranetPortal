using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Repositories.AssetManagerRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.AssetManagerRepositories
{
    public class AssetMaintenanceRepository : IAssetMaintenanceRepository
    {
        public IConfiguration _config { get; }
        public AssetMaintenanceRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //=============== Current Asset Maintenance Methods Starts Here ======================//
        #region Current Assets Maintenance Methods
        public async Task<IList<AssetMaintenance>> GetAllCurrentAsync()
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");   
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.mtnc_st_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append("ORDER BY m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetCurrentByAssetIdAsync(string assetId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.mtnc_st_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append("AND (m.asm_asst_id = @asst_id) ORDER BY  m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetCurrentByAssetNameAsync(string assetName)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.mtnc_st_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append($"AND (LOWER(a.asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY  m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_name = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_name.Value = assetName;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetCurrentByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.mtnc_st_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append("AND (m.asst_typ_id = @asst_typ_id) ");
            sb.Append($"ORDER BY m.asm_asst_id, m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_type_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_type_id.Value = assetTypeId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        #endregion

        //=============== All Asset Maintenance Methods Starts Here ==========================//
        #region All Assets Maintenance Methods
        public async Task<IList<AssetMaintenance>> GetAllAsync()
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("ORDER BY m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        public async Task<AssetMaintenance> GetByIdAsync(int assetMaintenanceId)
        {
            AssetMaintenance assetMaintenance = new AssetMaintenance();
            if (assetMaintenanceId < 1) { throw new ArgumentNullException(nameof(assetMaintenanceId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.mtnc_hst_id = @mtnc_hst_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var mtnc_hst_id = cmd.Parameters.Add("@mtnc_hst_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    mtnc_hst_id.Value = assetMaintenanceId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenance.AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"];
                            assetMaintenance.AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString();
                            assetMaintenance.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                            assetMaintenance.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                            assetMaintenance.StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"];
                            assetMaintenance.EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"];
                            assetMaintenance.LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"];
                            assetMaintenance.MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString();
                            assetMaintenance.IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString();
                            assetMaintenance.SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString();
                            assetMaintenance.PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString();
                            assetMaintenance.FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString();
                            assetMaintenance.MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString();
                            assetMaintenance.SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString();
                            assetMaintenance.Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString();
                            assetMaintenance.LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString();
                            assetMaintenance.AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"];
                            assetMaintenance.AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            assetMaintenance.AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"];
                            assetMaintenance.AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString();
                            assetMaintenance.ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString();
                            assetMaintenance.ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetMaintenance;
        }

        public async Task<IList<AssetMaintenance>> GetByAssetIdAsync(string assetId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.asm_asst_id = @asm_asst_id) ORDER BY m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asm_asst_id.Value = assetId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetByAssetIdAndYearAsync(string assetId, int maintenanceYear)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.asm_asst_id = @asm_asst_id) ");
            sb.Append("AND ((EXTRACT(YEAR FROM m.mtnc_st_dt))::INTEGER = @yr) ");
            sb.Append("ORDER BY m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asm_asst_id.Value = assetId;
                    yr.Value = maintenanceYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetByAssetIdAndYearAndMonthAsync(string assetId, int maintenanceYear, int maintenanceMonth)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.asm_asst_id = @asm_asst_id) ");
            sb.Append("AND ((EXTRACT(YEAR FROM m.mtnc_st_dt))::INTEGER = @yr) ");
            sb.Append("AND ((EXTRACT(MONTH FROM m.mtnc_st_dt))::INTEGER = @mn) ");
            sb.Append("ORDER BY m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    var mn = cmd.Parameters.Add("@mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asm_asst_id.Value = assetId;
                    yr.Value = maintenanceYear;
                    mn.Value = maintenanceMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }


        public async Task<IList<AssetMaintenance>> GetByAssetNameAsync(string assetName)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append($"WHERE (LOWER(a.asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY  m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_name = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_name.Value = assetName;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        public async Task<IList<AssetMaintenance>> GetByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetMaintenance> assetMaintenanceList = new List<AssetMaintenance>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mtnc_hst_id, m.asm_asst_id, m.mtnc_tl, m.mtnc_st_dt, m.mtnc_nd_dt, m.mtnc_iss_ds, ");
            sb.Append("m.mtnc_sln_ds, m.mtnc_prv_cndt, m.mtnc_fnl_cndt, m.mtn_by, sup_by, m.commnts, m.lggd_by, ");
            sb.Append("m.lggd_dt, m.md_by, m.md_dt, m.asst_typ_id, m.asst_ctg_id, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm FROM public.asm_mtnc_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.asst_typ_id = @asst_typ_id) ");
            sb.Append($"ORDER BY m.asm_asst_id, m.mtnc_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_type_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_type_id.Value = assetTypeId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMaintenanceList.Add(new AssetMaintenance()
                            {
                                AssetMaintenanceID = reader["mtnc_hst_id"] == DBNull.Value ? 0 : (int)reader["mtnc_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                StartTime = reader["mtnc_st_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_st_dt"],
                                EndTime = reader["mtnc_nd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mtnc_nd_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                MaintenanceTitle = reader["mtnc_tl"] == DBNull.Value ? string.Empty : reader["mtnc_tl"].ToString(),
                                IssueDescription = reader["mtnc_iss_ds"] == DBNull.Value ? string.Empty : reader["mtnc_iss_ds"].ToString(),
                                SolutionDescription = reader["mtnc_sln_ds"] == DBNull.Value ? string.Empty : reader["mtnc_sln_ds"].ToString(),
                                PreviousCondition = reader["mtnc_prv_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_prv_cndt"].ToString(),
                                FinalCondition = reader["mtnc_fnl_cndt"] == DBNull.Value ? string.Empty : reader["mtnc_fnl_cndt"].ToString(),
                                MaintainedBy = reader["mtn_by"] == DBNull.Value ? string.Empty : reader["mtn_by"].ToString(),
                                SupervisedBy = reader["sup_by"] == DBNull.Value ? string.Empty : reader["sup_by"].ToString(),
                                Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString(),
                                LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString(),
                                ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString(),
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
            return assetMaintenanceList;
        }

        #endregion

        //============== Asset Maintenance CRUD Methods Starts Here ==========================//
        #region Asset Maintenance CRUD Methods
        public async Task<bool> AddAssetMaintenanceAsync(AssetMaintenance assetMaintenance)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_mtnc_hst(asm_asst_id, mtnc_tl, mtnc_st_dt, mtnc_nd_dt, mtnc_iss_ds, ");
            sb.Append("mtnc_sln_ds, mtnc_prv_cndt, mtnc_fnl_cndt, mtn_by, sup_by, commnts, lggd_by, lggd_dt, md_by, ");
            sb.Append("md_dt, asst_typ_id, asst_ctg_id) VALUES (@asm_asst_id, @mtnc_tl, @mtnc_st_dt, @mtnc_nd_dt, ");
            sb.Append("@mtnc_iss_ds, @mtnc_sln_ds, @mtnc_prv_cndt, @mtnc_fnl_cndt, @mtn_by, @sup_by, @commnts, @lggd_by, ");
            sb.Append("@lggd_dt, @md_by, @md_dt, @asst_typ_id, @asst_ctg_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var mtnc_tl = cmd.Parameters.Add("@mtnc_tl", NpgsqlDbType.Text);
                    var mtnc_st_dt = cmd.Parameters.Add("@mtnc_st_dt", NpgsqlDbType.TimestampTz);
                    var mtnc_nd_dt = cmd.Parameters.Add("@mtnc_nd_dt", NpgsqlDbType.TimestampTz);
                    var mtnc_iss_ds = cmd.Parameters.Add("@mtnc_iss_ds", NpgsqlDbType.Text);
                    var mtnc_sln_ds = cmd.Parameters.Add("@mtnc_sln_ds", NpgsqlDbType.Text);
                    var mtnc_prv_cndt = cmd.Parameters.Add("@mtnc_prv_cndt", NpgsqlDbType.Text);
                    var mtnc_fnl_cndt = cmd.Parameters.Add("@mtnc_fnl_cndt", NpgsqlDbType.Text);
                    var mtn_by = cmd.Parameters.Add("@mtn_by", NpgsqlDbType.Text);
                    var sup_by = cmd.Parameters.Add("@sup_by", NpgsqlDbType.Text);
                    var commnts = cmd.Parameters.Add("@commnts", NpgsqlDbType.Text);
                    var lggd_by = cmd.Parameters.Add("@lggd_by", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);
                    var lggd_dt = cmd.Parameters.Add("@lggd_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    asm_asst_id.Value = assetMaintenance.AssetID;
                    mtnc_tl.Value = assetMaintenance.MaintenanceTitle;
                    mtnc_st_dt.Value = assetMaintenance.StartTime ?? DateTime.Now;
                    mtnc_nd_dt.Value = assetMaintenance.EndTime ?? DateTime.Now;
                    mtnc_iss_ds.Value = assetMaintenance.IssueDescription ?? string.Empty;
                    mtnc_sln_ds.Value = assetMaintenance.SolutionDescription ?? string.Empty;
                    mtnc_prv_cndt.Value = assetMaintenance.PreviousCondition ?? string.Empty;
                    mtnc_fnl_cndt.Value = assetMaintenance.FinalCondition ?? string.Empty;
                    mtn_by.Value = assetMaintenance.MaintainedBy ?? string.Empty;
                    sup_by.Value = assetMaintenance.SupervisedBy ?? string.Empty;
                    commnts.Value = assetMaintenance.Comments ?? string.Empty;
                    lggd_by.Value = assetMaintenance.LoggedBy ?? string.Empty;
                    md_by.Value = assetMaintenance.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetMaintenance.ModifiedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetMaintenance.AssetTypeID;
                    asst_ctg_id.Value = assetMaintenance.AssetCategoryID;
                    lggd_dt.Value = assetMaintenance.LoggedTime;

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

        public async Task<bool> EditAsync(AssetMaintenance assetMaintenance)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_mtnc_hst	SET asm_asst_id = @asm_asst_id, mtnc_tl = @mtnc_tl, ");
            sb.Append("mtnc_st_dt = @mtnc_st_dt, mtnc_nd_dt = @mtnc_nd_dt, mtnc_iss_ds = @mtnc_iss_ds, ");
            sb.Append("mtnc_sln_ds = @mtnc_sln_ds, mtnc_prv_cndt = @mtnc_prv_cndt, mtnc_fnl_cndt = @mtnc_fnl_cndt, ");
            sb.Append("mtn_by = @mtn_by, sup_by = @sup_by, commnts = @commnts, lggd_by = @lggd_by, lggd_dt = @lggd_dt, ");
            sb.Append("md_by = @md_by, md_dt = @md_dt, asst_typ_id = @asst_typ_id, asst_ctg_id = @asst_ctg_id ");
            sb.Append("WHERE (mtnc_hst_id = @mtnc_hst_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var mtnc_hst_id = cmd.Parameters.Add("@mtnc_hst_id", NpgsqlDbType.Integer);
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var mtnc_tl = cmd.Parameters.Add("@mtnc_tl", NpgsqlDbType.Text);
                    var mtnc_st_dt = cmd.Parameters.Add("@mtnc_st_dt", NpgsqlDbType.TimestampTz);
                    var mtnc_nd_dt = cmd.Parameters.Add("@mtnc_nd_dt", NpgsqlDbType.TimestampTz);
                    var mtnc_iss_ds = cmd.Parameters.Add("@mtnc_iss_ds", NpgsqlDbType.Text);
                    var mtnc_sln_ds = cmd.Parameters.Add("@mtnc_sln_ds", NpgsqlDbType.Text);
                    var mtnc_prv_cndt = cmd.Parameters.Add("@mtnc_prv_cndt", NpgsqlDbType.Text);
                    var mtnc_fnl_cndt = cmd.Parameters.Add("@mtnc_fnl_cndt", NpgsqlDbType.Text);
                    var mtn_by = cmd.Parameters.Add("@mtn_by", NpgsqlDbType.Text);
                    var sup_by = cmd.Parameters.Add("@sup_by", NpgsqlDbType.Text);
                    var commnts = cmd.Parameters.Add("@commnts", NpgsqlDbType.Text);
                    var lggd_by = cmd.Parameters.Add("@lggd_by", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);
                    var lggd_dt = cmd.Parameters.Add("@lggd_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    mtnc_hst_id.Value = assetMaintenance.AssetMaintenanceID;
                    asm_asst_id.Value = assetMaintenance.AssetID;
                    mtnc_tl.Value = assetMaintenance.MaintenanceTitle;
                    mtnc_st_dt.Value = assetMaintenance.StartTime ?? DateTime.Now;
                    mtnc_nd_dt.Value = assetMaintenance.EndTime ?? DateTime.Now;
                    mtnc_iss_ds.Value = assetMaintenance.IssueDescription ?? string.Empty;
                    mtnc_sln_ds.Value = assetMaintenance.SolutionDescription ?? string.Empty;
                    mtnc_prv_cndt.Value = assetMaintenance.PreviousCondition ?? string.Empty;
                    mtnc_fnl_cndt.Value = assetMaintenance.FinalCondition ?? string.Empty;
                    mtn_by.Value = assetMaintenance.MaintainedBy ?? string.Empty;
                    sup_by.Value = assetMaintenance.SupervisedBy ?? string.Empty;
                    commnts.Value = assetMaintenance.Comments ?? string.Empty;
                    lggd_by.Value = assetMaintenance.LoggedBy ?? string.Empty;
                    md_by.Value = assetMaintenance.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetMaintenance.ModifiedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetMaintenance.AssetTypeID;
                    asst_ctg_id.Value = assetMaintenance.AssetCategoryID;
                    lggd_dt.Value = assetMaintenance.LoggedTime;

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

        public async Task<bool> DeleteAsync(int assetMaintenanceId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_mtnc_hst WHERE (mtnc_hst_id = @mtnc_hst_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var mtnc_hst_id = cmd.Parameters.Add("@mtnc_hst_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    mtnc_hst_id.Value = assetMaintenanceId;
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
