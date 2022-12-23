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
    public class AssetIncidentRepository : IAssetIncidentRepository
    {
        public IConfiguration _config { get; }
        public AssetIncidentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //=============== Current Asset Incident Methods Starts Here ======================//
        #region Current Assets Incident Methods
        public async Task<IList<AssetIncident>> GetAllCurrentAsync()
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.incdt_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append("ORDER BY i.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetCurrentByAssetIdAsync(string assetId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.incdt_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append("AND (i.asm_asst_id = @asst_id) ORDER BY  i.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetCurrentByAssetNameAsync(string assetName)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.incdt_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append($"AND (LOWER(a.asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY  i.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetCurrentByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.asst_typ_id = @asst_typ_id) ");
            sb.Append("AND (i.incdt_dt >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append($"ORDER BY i.asm_asst_id, i.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        #endregion

        //=============== All Asset Incident Methods Starts Here ==========================//
        #region All Assets Incident Methods
        public async Task<IList<AssetIncident>> GetAllAsync()
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("ORDER BY i.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        public async Task<AssetIncident> GetByIdAsync(int assetIncidentId)
        {
            AssetIncident assetIncident = new AssetIncident();
            if (assetIncidentId < 1) { throw new ArgumentNullException(nameof(assetIncidentId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.incdt_hst_id = @incdt_hst_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var incdt_hst_id = cmd.Parameters.Add("@incdt_hst_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    incdt_hst_id.Value = assetIncidentId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetIncident.AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"];
                            assetIncident.AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString();
                            assetIncident.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                            assetIncident.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                            assetIncident.IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"];
                            assetIncident.LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"];
                            assetIncident.IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString();
                            assetIncident.IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString();
                            assetIncident.AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString();
                            assetIncident.ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString();
                            assetIncident.Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString();
                            assetIncident.Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString();
                            assetIncident.LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString();
                            assetIncident.AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"];
                            assetIncident.AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            assetIncident.AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"];
                            assetIncident.AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString();
                            assetIncident.ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString();
                            assetIncident.ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetIncident;
        }

        public async Task<IList<AssetIncident>> GetByAssetIdAsync(string assetId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
          sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.asm_asst_id = @asm_asst_id) ORDER BY i.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetByAssetIdAndYearAsync(string assetId, int incidentYear)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            if(incidentYear < 1) { throw new ArgumentException(nameof(incidentYear)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.asm_asst_id = @asm_asst_id) ");
            sb.Append("AND ((EXTRACT(YEAR FROM u.start_date))::INTEGER = @yr) ");
            sb.Append("ORDER BY i.incdt_hst_id DESC;");
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
                    yr.Value = incidentYear;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetByAssetIdAndYearAndMonthAsync(string assetId, int incidentYear, int incidentMonth)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            if (incidentYear < 1) { throw new ArgumentException(nameof(incidentYear)); }
            if (incidentMonth < 1) { throw new ArgumentException(nameof(incidentYear)); }

            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.asm_asst_id = @asm_asst_id) ");
            sb.Append("AND ((EXTRACT(YEAR FROM i.incdt_dt))::INTEGER = @yr) ");
            sb.Append("AND ((EXTRACT(MONTH FROM i.incdt_dt))::INTEGER = @mn) ");
            sb.Append("ORDER BY i.incdt_hst_id DESC;");
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
                    yr.Value = incidentYear;
                    mn.Value = incidentMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }


        public async Task<IList<AssetIncident>> GetByAssetNameAsync(string assetName)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append($"WHERE (LOWER(a.asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY  i.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        public async Task<IList<AssetIncident>> GetByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetIncident> assetIncidents = new List<AssetIncident>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT i.incdt_hst_id, i.asm_asst_id, i.incdt_dt, i.incdt_ds, i.asst_cndt, i.action_tkn, i.rcomndatn, i.commnts, ");
            sb.Append("i.lggd_by, i.md_by, i.md_dt, i.asst_typ_id, i.asst_ctg_id, i.lggd_dt, a.asst_nm, a.asst_ds, a.asst_cndt, ");
            sb.Append("t.typ_nm, c.asst_ctgs_nm, i.incdt_tl FROM public.asm_incdt_hst i ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON i.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON i.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON i.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (i.asst_typ_id = @asst_typ_id) ");
            sb.Append($"ORDER BY u.asset_id, u.incdt_hst_id DESC;");
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
                            assetIncidents.Add(new AssetIncident()
                            {
                                AssetIncidentID = reader["incdt_hst_id"] == DBNull.Value ? 0 : (int)reader["incdt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                IncidentTime = reader["incdt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["incdt_dt"],
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                IncidentDescription = reader["incdt_ds"] == DBNull.Value ? string.Empty : reader["incdt_ds"].ToString(),
                                IncidentTitle = reader["incdt_tl"] == DBNull.Value ? string.Empty : reader["incdt_tl"].ToString(),
                                AssetCondition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                                ActionTaken = reader["action_tkn"] == DBNull.Value ? string.Empty : reader["action_tkn"].ToString(),
                                Recommendation = reader["rcomndatn"] == DBNull.Value ? string.Empty : reader["rcomndatn"].ToString(),
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
            return assetIncidents;
        }

        #endregion

        //============== Asset Incident CRUD Methods Starts Here ==========================//
        #region Asset Incident CRUD Methods
        public async Task<bool> AddAssetIncidentAsync(AssetIncident assetIncident)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_incdt_hst(asm_asst_id, incdt_dt, incdt_ds, incdt_tl, asst_cndt, action_tkn, ");
            sb.Append("rcomndatn, commnts, lggd_by, md_by, md_dt, asst_typ_id, asst_ctg_id, lggd_dt) VALUES ");
            sb.Append("(@asm_asst_id, @incdt_dt, @incdt_ds, @incdt_tl, @asst_cndt, @action_tkn, @rcomndatn, @commnts, ");
            sb.Append("@lggd_by, @md_by, @md_dt, @asst_typ_id, @asst_ctg_id, @lggd_dt); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var incdt_dt = cmd.Parameters.Add("@incdt_dt", NpgsqlDbType.TimestampTz);
                    var incdt_ds = cmd.Parameters.Add("@incdt_ds", NpgsqlDbType.Text);
                    var incdt_tl = cmd.Parameters.Add("@incdt_tl", NpgsqlDbType.Text);
                    var asst_cndt = cmd.Parameters.Add("@asst_cndt", NpgsqlDbType.Text);
                    var action_tkn = cmd.Parameters.Add("@action_tkn", NpgsqlDbType.Text);
                    var rcomndatn = cmd.Parameters.Add("@rcomndatn", NpgsqlDbType.Text);
                    var commnts = cmd.Parameters.Add("@commnts", NpgsqlDbType.Text);
                    var lggd_by = cmd.Parameters.Add("@lggd_by", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);
                    var lggd_dt = cmd.Parameters.Add("@lggd_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    asm_asst_id.Value = assetIncident.AssetID;
                    incdt_dt.Value = assetIncident.IncidentTime ?? DateTime.Now;
                    incdt_ds.Value = assetIncident.IncidentDescription ?? string.Empty;
                    incdt_tl.Value = assetIncident.IncidentTitle ?? string.Empty;
                    asst_cndt.Value = assetIncident.AssetCondition ?? string.Empty;
                    action_tkn.Value = assetIncident.ActionTaken ?? string.Empty;
                    rcomndatn.Value = assetIncident.Recommendation ?? string.Empty;
                    commnts.Value = assetIncident.Comments ?? string.Empty;
                    lggd_by.Value = assetIncident.LoggedBy ?? string.Empty;
                    md_by.Value = assetIncident.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetIncident.ModifiedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetIncident.AssetTypeID;
                    asst_ctg_id.Value = assetIncident.AssetCategoryID;
                    lggd_dt.Value = assetIncident.LoggedTime;

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

        public async Task<bool> EditAsync(AssetIncident assetIncident)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_incdt_hst SET asm_asst_id = @asm_asst_id, incdt_dt = @incdt_dt, ");
            sb.Append("incdt_ds = @incdt_ds, asst_cndt = @asst_cndt, action_tkn = @action_tkn, rcomndatn = @rcomndatn, ");
            sb.Append("commnts = @commnts, lggd_by = @lggd_by, md_by = @md_by, md_dt = @md_dt, asst_typ_id = @asst_typ_id, ");
            sb.Append("asst_ctg_id = @asst_ctg_id, lggd_dt = @lggd_dt, incdt_tl = @incdt_tl  WHERE incdt_hst_id = @incdt_hst_id; ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var incdt_hst_id = cmd.Parameters.Add("@incdt_hst_id", NpgsqlDbType.Integer);
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var incdt_dt = cmd.Parameters.Add("@incdt_dt", NpgsqlDbType.TimestampTz);
                    var incdt_ds = cmd.Parameters.Add("@incdt_ds", NpgsqlDbType.Text);
                    var incdt_tl = cmd.Parameters.Add("@incdt_tl", NpgsqlDbType.Text);
                    var asst_cndt = cmd.Parameters.Add("@asst_cndt", NpgsqlDbType.Text);
                    var action_tkn = cmd.Parameters.Add("@action_tkn", NpgsqlDbType.Text);
                    var rcomndatn = cmd.Parameters.Add("@rcomndatn", NpgsqlDbType.Text);
                    var commnts = cmd.Parameters.Add("@commnts", NpgsqlDbType.Text);
                    var lggd_by = cmd.Parameters.Add("@lggd_by", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);
                    var lggd_dt = cmd.Parameters.Add("@lggd_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    incdt_hst_id.Value = assetIncident.AssetIncidentID;
                    asm_asst_id.Value = assetIncident.AssetID;
                    incdt_dt.Value = assetIncident.IncidentTime ?? DateTime.Now;
                    incdt_ds.Value = assetIncident.IncidentDescription ?? string.Empty;
                    incdt_tl.Value = assetIncident.IncidentTitle ?? string.Empty;
                    asst_cndt.Value = assetIncident.AssetCondition ?? string.Empty;
                    action_tkn.Value = assetIncident.ActionTaken ?? string.Empty;
                    rcomndatn.Value = assetIncident.Recommendation ?? string.Empty;
                    commnts.Value = assetIncident.Comments ?? string.Empty;
                    lggd_by.Value = assetIncident.LoggedBy ?? string.Empty;
                    md_by.Value = assetIncident.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetIncident.ModifiedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetIncident.AssetTypeID;
                    asst_ctg_id.Value = assetIncident.AssetCategoryID;
                    lggd_dt.Value = assetIncident.LoggedTime;

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

        public async Task<bool> DeleteAsync(int assetIncidentId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_incdt_hst WHERE (incdt_hst_id = @incdt_hst);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var incdt_hst_id = cmd.Parameters.Add("@incdt_hst_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    incdt_hst_id.Value = assetIncidentId;
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
