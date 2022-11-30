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
    public class AssetMovementRepository : IAssetMovementRepository
    {
        public IConfiguration _config { get; }
        public AssetMovementRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //=============== All Asset Movement Methods Starts Here ==========================//
        #region All Assets Movement Methods
        public async Task<IList<AssetMovement>> GetAllAsync()
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mvt_hst_id, m.asm_asst_id, m.mvt_fr_loc, m.mvt_to_loc, m.mvt_dt, m.mvt_pps, ");
            sb.Append("m.ass_cndt, m.aprv_by, m.mvd_by, m.commnts, m.lggd_by, m.lggd_dt, m.md_by, m.md_dt, ");
            sb.Append("m.asst_typ_id, m.asst_ctg_id, m.mvt_sts, a.asst_nm, a.asst_ds, a.cnd_sts, t.typ_nm, ");
            sb.Append("c.asst_ctgs_nm FROM public.asm_mvt_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("ORDER BY m.mvt_hst_id;");
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
                            assetMovementList.Add(new AssetMovement()
                            {
                                AssetMovementID = reader["mvt_hst_id"] == DBNull.Value ? 0 : (int)reader["mvt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString(),
                                AssetConditionDescription = reader["ass_cndt"] == DBNull.Value ? string.Empty : reader["ass_cndt"].ToString(),
                                AssetConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["cnd_sts"],
                                MovedFromLocationName = reader["mvt_fr_loc"] == DBNull.Value ? string.Empty : reader["mvt_fr_loc"].ToString(),
                                MovedToLocationName = reader["mvt_to_loc"] == DBNull.Value ? string.Empty : reader["mvt_to_loc"].ToString(),
                                MovedOn = reader["mvt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mvt_dt"],
                                MovementPurpose = reader["mvt_pps"] == DBNull.Value ? string.Empty : reader["mvt_pps"].ToString(),
                                MovementStatus = reader["mvt_sts"] == DBNull.Value ? string.Empty : reader["mvt_sts"].ToString(),
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                SupervisedBy = reader["mvd_by"] == DBNull.Value ? string.Empty : reader["mvd_by"].ToString(),
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
            return assetMovementList;
        }

        public async Task<AssetMovement> GetByIdAsync(int assetMovementId)
        {
            AssetMovement assetMovement = new AssetMovement();
            if (assetMovementId < 1) { throw new ArgumentNullException(nameof(assetMovementId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mvt_hst_id, m.asm_asst_id, m.mvt_fr_loc, m.mvt_to_loc, m.mvt_dt, m.mvt_pps, ");
            sb.Append("m.ass_cndt, m.aprv_by, m.mvd_by, m.commnts, m.lggd_by, m.lggd_dt, m.md_by, m.md_dt, ");
            sb.Append("m.asst_typ_id, m.asst_ctg_id, m.mvt_sts, a.asst_nm, a.asst_ds, a.cnd_sts, t.typ_nm, ");
            sb.Append("c.asst_ctgs_nm FROM public.asm_mvt_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.mvt_hst_id = @mvt_hst_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var mvt_hst_id = cmd.Parameters.Add("@mvt_hst_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    mvt_hst_id.Value = assetMovementId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetMovement.AssetMovementID = reader["mvt_hst_id"] == DBNull.Value ? 0 : (int)reader["mvt_hst_id"];
                            assetMovement.AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString();
                            assetMovement.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                            assetMovement.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                            assetMovement.ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString();
                            assetMovement.AssetConditionDescription = reader["ass_cndt"] == DBNull.Value ? string.Empty : reader["ass_cndt"].ToString();
                            assetMovement.AssetConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["cnd_sts"];
                            assetMovement.MovedFromLocationName = reader["mvt_fr_loc"] == DBNull.Value ? string.Empty : reader["mvt_fr_loc"].ToString();
                            assetMovement.MovedToLocationName = reader["mvt_to_loc"] == DBNull.Value ? string.Empty : reader["mvt_to_loc"].ToString();
                            assetMovement.MovedOn = reader["mvt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mvt_dt"];
                            assetMovement.MovementPurpose = reader["mvt_pps"] == DBNull.Value ? string.Empty : reader["mvt_pps"].ToString();
                            assetMovement.MovementStatus = reader["mvt_sts"] == DBNull.Value ? string.Empty : reader["mvt_sts"].ToString();
                            assetMovement.LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"];
                            assetMovement.SupervisedBy = reader["mvd_by"] == DBNull.Value ? string.Empty : reader["mvd_by"].ToString();
                            assetMovement.Comments = reader["commnts"] == DBNull.Value ? string.Empty : reader["commnts"].ToString();
                            assetMovement.LoggedBy = reader["lggd_by"] == DBNull.Value ? string.Empty : reader["lggd_by"].ToString();
                            assetMovement.AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"];
                            assetMovement.AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            assetMovement.AssetCategoryID = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"];
                            assetMovement.AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString();
                            assetMovement.ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString();
                            assetMovement.ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetMovement;
        }

        public async Task<IList<AssetMovement>> GetByAssetIdAsync(string assetId)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mvt_hst_id, m.asm_asst_id, m.mvt_fr_loc, m.mvt_to_loc, m.mvt_dt, m.mvt_pps, ");
            sb.Append("m.ass_cndt, m.aprv_by, m.mvd_by, m.commnts, m.lggd_by, m.lggd_dt, m.md_by, m.md_dt, ");
            sb.Append("m.asst_typ_id, m.asst_ctg_id, m.mvt_sts, a.asst_nm, a.asst_ds, a.cnd_sts, t.typ_nm, ");
            sb.Append("c.asst_ctgs_nm FROM public.asm_mvt_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.asm_asst_id = @asm_asst_id) ORDER BY m.mvt_hst_id DESC;");
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
                            assetMovementList.Add(new AssetMovement()
                            {
                                AssetMovementID = reader["mvt_hst_id"] == DBNull.Value ? 0 : (int)reader["mvt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString(),
                                AssetConditionDescription = reader["ass_cndt"] == DBNull.Value ? string.Empty : reader["ass_cndt"].ToString(),
                                AssetConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["cnd_sts"],
                                MovedFromLocationName = reader["mvt_fr_loc"] == DBNull.Value ? string.Empty : reader["mvt_fr_loc"].ToString(),
                                MovedToLocationName = reader["mvt_to_loc"] == DBNull.Value ? string.Empty : reader["mvt_to_loc"].ToString(),
                                MovedOn = reader["mvt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mvt_dt"],
                                MovementPurpose = reader["mvt_pps"] == DBNull.Value ? string.Empty : reader["mvt_pps"].ToString(),
                                MovementStatus = reader["mvt_sts"] == DBNull.Value ? string.Empty : reader["mvt_sts"].ToString(),
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                SupervisedBy = reader["mvd_by"] == DBNull.Value ? string.Empty : reader["mvd_by"].ToString(),
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
            return assetMovementList;
        }

        public async Task<IList<AssetMovement>> GetByAssetNameAsync(string assetName)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mvt_hst_id, m.asm_asst_id, m.mvt_fr_loc, m.mvt_to_loc, m.mvt_dt, m.mvt_pps, ");
            sb.Append("m.ass_cndt, m.aprv_by, m.mvd_by, m.commnts, m.lggd_by, m.lggd_dt, m.md_by, m.md_dt, ");
            sb.Append("m.asst_typ_id, m.asst_ctg_id, m.mvt_sts, a.asst_nm, a.asst_ds, a.cnd_sts, t.typ_nm, ");
            sb.Append("c.asst_ctgs_nm FROM public.asm_mvt_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append($"WHERE (LOWER(a.asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY  m.mvt_hst_id DESC;");
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
                            assetMovementList.Add(new AssetMovement()
                            {
                                AssetMovementID = reader["mvt_hst_id"] == DBNull.Value ? 0 : (int)reader["mvt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString(),
                                AssetConditionDescription = reader["ass_cndt"] == DBNull.Value ? string.Empty : reader["ass_cndt"].ToString(),
                                AssetConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["cnd_sts"],
                                MovedFromLocationName = reader["mvt_fr_loc"] == DBNull.Value ? string.Empty : reader["mvt_fr_loc"].ToString(),
                                MovedToLocationName = reader["mvt_to_loc"] == DBNull.Value ? string.Empty : reader["mvt_to_loc"].ToString(),
                                MovedOn = reader["mvt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mvt_dt"],
                                MovementPurpose = reader["mvt_pps"] == DBNull.Value ? string.Empty : reader["mvt_pps"].ToString(),
                                MovementStatus = reader["mvt_sts"] == DBNull.Value ? string.Empty : reader["mvt_sts"].ToString(),
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                SupervisedBy = reader["mvd_by"] == DBNull.Value ? string.Empty : reader["mvd_by"].ToString(),
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
            return assetMovementList;
        }

        public async Task<IList<AssetMovement>> GetByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetMovement> assetMovementList = new List<AssetMovement>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT m.mvt_hst_id, m.asm_asst_id, m.mvt_fr_loc, m.mvt_to_loc, m.mvt_dt, m.mvt_pps, ");
            sb.Append("m.ass_cndt, m.aprv_by, m.mvd_by, m.commnts, m.lggd_by, m.lggd_dt, m.md_by, m.md_dt, ");
            sb.Append("m.asst_typ_id, m.asst_ctg_id, m.mvt_sts, a.asst_nm, a.asst_ds, a.cnd_sts, t.typ_nm, ");
            sb.Append("c.asst_ctgs_nm FROM public.asm_mvt_hst m ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON m.asm_asst_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON m.asst_typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON m.asst_ctg_id = c.asst_ctgs_id ");
            sb.Append("WHERE (m.asst_typ_id = @asst_typ_id) ");
            sb.Append($"ORDER BY m.asm_asst_id, m.mvt_hst_id DESC;");
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
                            assetMovementList.Add(new AssetMovement()
                            {
                                AssetMovementID = reader["mvt_hst_id"] == DBNull.Value ? 0 : (int)reader["mvt_hst_id"],
                                AssetID = reader["asm_asst_id"] == DBNull.Value ? string.Empty : reader["asm_asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString(),
                                AssetConditionDescription = reader["ass_cndt"] == DBNull.Value ? string.Empty : reader["ass_cndt"].ToString(),
                                AssetConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["cnd_sts"],
                                MovedFromLocationName = reader["mvt_fr_loc"] == DBNull.Value ? string.Empty : reader["mvt_fr_loc"].ToString(),
                                MovedToLocationName = reader["mvt_to_loc"] == DBNull.Value ? string.Empty : reader["mvt_to_loc"].ToString(),
                                MovedOn = reader["mvt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mvt_dt"],
                                MovementPurpose = reader["mvt_pps"] == DBNull.Value ? string.Empty : reader["mvt_pps"].ToString(),
                                MovementStatus = reader["mvt_sts"] == DBNull.Value ? string.Empty : reader["mvt_sts"].ToString(),
                                LoggedTime = reader["lggd_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lggd_dt"],
                                SupervisedBy = reader["mvd_by"] == DBNull.Value ? string.Empty : reader["mvd_by"].ToString(),
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
            return assetMovementList;
        }

        #endregion

        //============== Asset Movement CRUD Methods Starts Here ==========================//
        #region Asset Movement CRUD Methods
        public async Task<bool> AddAssetMovementAsync(AssetMovement assetMovement)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_mvt_hst(asm_asst_id, mvt_fr_loc, mvt_to_loc, mvt_dt, ");
            sb.Append("mvt_pps, ass_cndt, aprv_by, mvd_by, commnts, lggd_by, lggd_dt, md_by, ");
            sb.Append("md_dt, asst_typ_id, asst_ctg_id, mvt_sts, cnd_sts) VALUES (@asm_asst_id, @mvt_fr_loc, ");
            sb.Append("@mvt_to_loc, @mvt_dt, @mvt_pps, @ass_cndt, @aprv_by, @mvd_by, @commnts, ");
            sb.Append("@lggd_by, @lggd_dt, @md_by, @md_dt, @asst_typ_id, @asst_ctg_id, @mvt_sts, @cnd_sts); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var mmvt_fr_loc = cmd.Parameters.Add("@mvt_fr_loc", NpgsqlDbType.Text);
                    var mvt_to_loc = cmd.Parameters.Add("@mvt_to_loc", NpgsqlDbType.Text);
                    var mvt_dt = cmd.Parameters.Add("@mvt_dt", NpgsqlDbType.TimestampTz);
                    var mvt_pps = cmd.Parameters.Add("@mvt_pps", NpgsqlDbType.Text);
                    var ass_cndt = cmd.Parameters.Add("@ass_cndt", NpgsqlDbType.Text);
                    var aprv_by = cmd.Parameters.Add("@aprv_by", NpgsqlDbType.Text);
                    var mvd_by = cmd.Parameters.Add("@mvd_by", NpgsqlDbType.Text);
                    var commnts = cmd.Parameters.Add("@commnts", NpgsqlDbType.Text);
                    var lggd_by = cmd.Parameters.Add("@lggd_by", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);
                    var lggd_dt = cmd.Parameters.Add("@lggd_dt", NpgsqlDbType.TimestampTz);
                    var mvt_sts = cmd.Parameters.Add("@mvt_sts", NpgsqlDbType.Text);
                    var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    asm_asst_id.Value = assetMovement.AssetID;
                    mmvt_fr_loc.Value = assetMovement.MovedFromLocationName;
                    mvt_to_loc.Value = assetMovement.MovedToLocationName;
                    mvt_dt.Value = assetMovement.MovedOn ?? DateTime.Now;
                    mvt_pps.Value = assetMovement.MovementPurpose ?? string.Empty;
                    ass_cndt.Value = assetMovement.AssetConditionDescription ?? string.Empty;
                    mvd_by.Value = assetMovement.SupervisedBy ?? string.Empty;
                    aprv_by.Value = assetMovement.ApprovedBy ?? string.Empty;
                    commnts.Value = assetMovement.Comments ?? string.Empty;
                    lggd_by.Value = assetMovement.LoggedBy ?? string.Empty;
                    md_by.Value = assetMovement.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetMovement.ModifiedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetMovement.AssetTypeID;
                    asst_ctg_id.Value = assetMovement.AssetCategoryID;
                    lggd_dt.Value = assetMovement.LoggedTime;
                    mvt_sts.Value = assetMovement.MovementStatus == null || assetMovement.MovementStatus == string.Empty ? "Transferred" : assetMovement.MovementStatus;
                    cnd_sts.Value = (int)assetMovement.AssetConditionStatus;
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

        public async Task<bool> EditAsync(AssetMovement assetMovement)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_mvt_hst SET asm_asst_id = @asm_asst_id, mvt_fr_loc = @mvt_fr_loc, ");
            sb.Append("mvt_to_loc = @mvt_to_loc, mvt_dt = @mvt_dt, mvt_pps = @mvt_pps, ass_cndt = @ass_cndt, ");
            sb.Append("aprv_by = @aprv_by, mvd_by = @mvd_by, commnts = @commnts, lggd_by = @lggd_by, ");
            sb.Append("lggd_dt = @lggd_dt, md_by = @md_by, md_dt = @md_dt, asst_typ_id = @asst_typ_id, ");
            sb.Append("asst_ctg_id = @asst_ctg_id, mvt_sts = @mvt_sts, cnd_sts = @cnd_sts ");
            sb.Append("WHERE mvt_hst_id = @mvt_hst_id;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var mvt_hst_id = cmd.Parameters.Add("@mvt_hst_id", NpgsqlDbType.Integer);
                    var asm_asst_id = cmd.Parameters.Add("@asm_asst_id", NpgsqlDbType.Text);
                    var mmvt_fr_loc = cmd.Parameters.Add("@mvt_fr_loc", NpgsqlDbType.Text);
                    var mvt_to_loc = cmd.Parameters.Add("@mvt_to_loc", NpgsqlDbType.Text);
                    var mvt_dt = cmd.Parameters.Add("@mvt_dt", NpgsqlDbType.TimestampTz);
                    var mvt_pps = cmd.Parameters.Add("@mvt_pps", NpgsqlDbType.Text);
                    var ass_cndt = cmd.Parameters.Add("@ass_cndt", NpgsqlDbType.Text);
                    var aprv_by = cmd.Parameters.Add("@aprv_by", NpgsqlDbType.Text);
                    var mvd_by = cmd.Parameters.Add("@mvd_by", NpgsqlDbType.Text);
                    var commnts = cmd.Parameters.Add("@commnts", NpgsqlDbType.Text);
                    var lggd_by = cmd.Parameters.Add("@lggd_by", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);
                    var lggd_dt = cmd.Parameters.Add("@lggd_dt", NpgsqlDbType.TimestampTz);
                    var mvt_sts = cmd.Parameters.Add("@mvt_sts", NpgsqlDbType.Text);
                    var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    mvt_hst_id.Value = assetMovement.AssetMovementID;
                    asm_asst_id.Value = assetMovement.AssetID;
                    mmvt_fr_loc.Value = assetMovement.MovedFromLocationName;
                    mvt_to_loc.Value = assetMovement.MovedToLocationName;
                    mvt_dt.Value = assetMovement.MovedOn ?? DateTime.Now;
                    mvt_pps.Value = assetMovement.MovementPurpose ?? string.Empty;
                    ass_cndt.Value = assetMovement.AssetConditionDescription ?? string.Empty;
                    mvd_by.Value = assetMovement.SupervisedBy ?? string.Empty;
                    aprv_by.Value = assetMovement.ApprovedBy ?? string.Empty;
                    commnts.Value = assetMovement.Comments ?? string.Empty;
                    lggd_by.Value = assetMovement.LoggedBy ?? string.Empty;
                    md_by.Value = assetMovement.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetMovement.ModifiedTime ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetMovement.AssetTypeID;
                    asst_ctg_id.Value = assetMovement.AssetCategoryID;
                    lggd_dt.Value = assetMovement.LoggedTime;
                    mvt_sts.Value = assetMovement.MovementStatus;
                    cnd_sts.Value = (int)assetMovement.AssetConditionStatus;

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

        public async Task<bool> DeleteAsync(int assetMovementId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_mvt_hst WHERE (mvt_hst_id = @mvt_hst_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var mvt_hst_id = cmd.Parameters.Add("@mvt_hst_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    mvt_hst_id.Value = assetMovementId;
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

