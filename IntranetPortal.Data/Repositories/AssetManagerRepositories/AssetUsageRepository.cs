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
    public class AssetUsageRepository : IAssetUsageRepository
    {
        public IConfiguration _config { get; }
        public AssetUsageRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //====================== Current Asset Usage Methods Starts Here ====================================//
        #region Current Assets Usage Methods
        public async Task<IList<AssetUsage>> GetAllCurrentAsync()
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by,u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id ");
            sb.Append("WHERE (u.start_date >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append("ORDER BY u.usg_id DESC;");
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
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetCurrentByAssetIdAsync(string assetId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id WHERE (u.asset_id = @asset_id) ");
            sb.Append("AND (u.start_date >= CURRENT_TIMESTAMP - '1 year'::interval) ORDER BY  u.usg_id DESC;");
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
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetCurrentByAssetNameAsync(string assetName)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id ");
            sb.Append("WHERE (u.start_date >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append($"AND (LOWER(a.asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY  u.usg_id DESC;");
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
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetCurrentByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id ");
            sb.Append("WHERE (u.asst_typ_id = @asst_typ_id) ");
            sb.Append("AND (u.start_date >= CURRENT_TIMESTAMP - '1 year'::interval) ");
            sb.Append($"ORDER BY u.asset_id, u.usg_id DESC;");
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
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        #endregion

        //====================== All Asset Usage Methods Starts Here ========================================//
        #region All Assets Usage Methods
        public async Task<IList<AssetUsage>> GetAllAsync()
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id ORDER BY u.usg_id DESC;");
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
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        public async Task<AssetUsage> GetByIdAsync(int assetUsageId)
        {
            AssetUsage assetUsage = new AssetUsage();
            if (assetUsageId < 1) { throw new ArgumentNullException(nameof(assetUsageId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id WHERE (u.usg_id = @usg_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var usg_id = cmd.Parameters.Add("@usg_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    usg_id.Value = assetUsageId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetUsage.UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"];
                            assetUsage.AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString();
                            assetUsage.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                            assetUsage.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                            assetUsage.UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"];
                            assetUsage.UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"];
                            assetUsage.Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString();
                            assetUsage.UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString();
                            assetUsage.UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString();
                            assetUsage.CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString();
                            assetUsage.CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString();
                            assetUsage.CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"];
                            assetUsage.CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"];
                            assetUsage.CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString();
                            assetUsage.CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString();
                            assetUsage.CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"];
                            assetUsage.CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString();
                            assetUsage.CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"];
                            assetUsage.CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString();
                            assetUsage.AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"];
                            assetUsage.AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            assetUsage.CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString();
                            assetUsage.ModifiedBy = reader["md_by"] == DBNull.Value ? string.Empty : reader["md_by"].ToString();
                            assetUsage.ModifiedTime = reader["md_dt"] == DBNull.Value ? string.Empty : reader["md_dt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetUsage;
        }

        public async Task<IList<AssetUsage>> GetByAssetIdAsync(string assetId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id WHERE (u.asset_id = @asset_id) ");
            sb.Append("ORDER BY  u.usg_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetCheckedOutByAssetIdAsync(string assetId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetId)) { throw new ArgumentNullException(nameof(assetId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id WHERE (u.asset_id = @asset_id) AND ");
            sb.Append("(u.chk_in_by = '' OR u.chk_in_by = null OR u.chk_in_time = null) ");
            sb.Append("ORDER BY  u.usg_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }


        public async Task<IList<AssetUsage>> GetByAssetNameAsync(string assetName)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id ");
            sb.Append($"WHERE (LOWER(a.asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY  u.usg_id DESC;");
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
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        public async Task<IList<AssetUsage>> GetByAssetTypeIdAsync(int assetTypeId)
        {
            List<AssetUsage> assetUsages = new List<AssetUsage>();
            if (assetTypeId < 1) { throw new ArgumentNullException(nameof(assetTypeId)); }
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usg_id, u.asset_id, u.start_date, u.end_date, u.usg_purpose, u.event_ds, u.chk_out_by, u.from_loc, ");
            sb.Append("u.chk_out_time, u.chk_out_cnd, u.chk_out_to, u.chk_out_cmt, u.chk_in_by, u.chk_in_time, u.chk_in_cnd, ");
            sb.Append("u.chk_in_cmt, u.asst_typ_id, u.event_loc, a.asst_nm, a.asst_ds, t.typ_nm, u.md_by, u.md_dt, u.chk_status ");
            sb.Append("FROM public.asm_ass_usg u INNER JOIN public.asm_stt_asst a ON u.asset_id = a.asst_id ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON u.asst_typ_id = t.typ_id ");
            sb.Append("WHERE (u.asst_typ_id = @asst_typ_id) ORDER BY u.asset_id, u.usg_id DESC;");
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
                            assetUsages.Add(new AssetUsage()
                            {
                                UsageID = reader["usg_id"] == DBNull.Value ? 0 : (int)reader["usg_id"],
                                AssetID = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStartTime = reader["start_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_date"],
                                UsageEndTime = reader["end_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_date"],
                                Purpose = reader["usg_purpose"] == DBNull.Value ? string.Empty : reader["usg_purpose"].ToString(),
                                UsageDescription = reader["event_ds"] == DBNull.Value ? string.Empty : reader["event_ds"].ToString(),
                                UsageLocation = reader["event_loc"] == DBNull.Value ? string.Empty : reader["event_loc"].ToString(),
                                CheckedInBy = reader["chk_in_by"] == DBNull.Value ? string.Empty : reader["chk_in_by"].ToString(),
                                CheckedInComment = reader["chk_in_cmt"] == DBNull.Value ? string.Empty : reader["chk_in_cmt"].ToString(),
                                CheckedInCondition = reader["chk_in_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_in_cnd"],
                                CheckedInTime = reader["chk_in_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_in_time"],
                                CheckedOutBy = reader["chk_out_by"] == DBNull.Value ? string.Empty : reader["chk_out_by"].ToString(),
                                CheckedOutComment = reader["chk_out_cmt"] == DBNull.Value ? string.Empty : reader["chk_out_cmt"].ToString(),
                                CheckedOutTime = reader["chk_out_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["chk_out_time"],
                                CheckedOutTo = reader["chk_out_to"] == DBNull.Value ? string.Empty : reader["chk_out_to"].ToString(),
                                CheckOutCondition = reader["chk_out_cnd"] == DBNull.Value ? AssetCondition.Unspecified : (AssetCondition)reader["chk_out_cnd"],
                                CheckStatus = reader["chk_status"] == DBNull.Value ? string.Empty : reader["chk_status"].ToString(),
                                AssetTypeID = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                CheckedOutFromLocation = reader["from_loc"] == DBNull.Value ? string.Empty : reader["from_loc"].ToString(),
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
            return assetUsages;
        }

        #endregion

        //====================== Asset Usage CRUD Methods Starts Here =======================================//
        #region Asset Usage CRUD Methods
        public async Task<bool> AddCheckOutAsync(AssetUsage assetUsage)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_ass_usg( asset_id, start_date, end_date, usg_purpose, event_ds, chk_out_by, chk_out_time, from_loc, ");
            sb.Append("chk_out_cnd, chk_out_to, chk_out_cmt, chk_in_by, chk_in_time, chk_in_cnd, chk_in_cmt, asst_typ_id, event_loc, md_by, ");
            sb.Append("md_dt, chk_status) VALUES ( @asset_id, @start_date, @end_date, @usg_purpose, @event_ds, @chk_out_by, ");
            sb.Append("@chk_out_time, @from_loc, @chk_out_cnd, @chk_out_to, @chk_out_cmt, @chk_in_by, @chk_in_time, @chk_in_cnd, @chk_in_cmt, ");
            sb.Append("@asst_typ_id, @event_loc, @md_by, @md_dt, @chk_status);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);
                    var start_date = cmd.Parameters.Add("@start_date", NpgsqlDbType.TimestampTz);
                    var end_date = cmd.Parameters.Add("@end_date", NpgsqlDbType.TimestampTz);
                    var usg_purpose = cmd.Parameters.Add("@usg_purpose", NpgsqlDbType.Text);
                    var event_ds = cmd.Parameters.Add("@event_ds", NpgsqlDbType.Text);
                    var chk_out_by = cmd.Parameters.Add("@chk_out_by", NpgsqlDbType.Text);
                    var chk_out_time = cmd.Parameters.Add("@chk_out_time", NpgsqlDbType.TimestampTz);
                    var chk_out_cnd = cmd.Parameters.Add("@chk_out_cnd", NpgsqlDbType.Integer);
                    var chk_out_to = cmd.Parameters.Add("@chk_out_to", NpgsqlDbType.Text);
                    var chk_out_cmt = cmd.Parameters.Add("@chk_out_cmt", NpgsqlDbType.Text);
                    var chk_in_by = cmd.Parameters.Add("@chk_in_by", NpgsqlDbType.Text);
                    var chk_in_time = cmd.Parameters.Add("@chk_in_time", NpgsqlDbType.TimestampTz);
                    var chk_in_cnd = cmd.Parameters.Add("@chk_in_cnd", NpgsqlDbType.Integer);
                    var chk_in_cmt = cmd.Parameters.Add("@chk_in_cmt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var event_loc = cmd.Parameters.Add("@event_loc", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var chk_status = cmd.Parameters.Add("@chk_status", NpgsqlDbType.Text);
                    var from_loc = cmd.Parameters.Add("@from_loc", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asset_id.Value = assetUsage.AssetID;
                    start_date.Value = assetUsage.UsageStartTime ?? (object)DBNull.Value;
                    end_date.Value = assetUsage.UsageEndTime ?? (object)DBNull.Value;
                    usg_purpose.Value = assetUsage.Purpose ?? (object)DBNull.Value;
                    event_ds.Value = assetUsage.UsageDescription ?? (object)DBNull.Value;
                    chk_out_by.Value = assetUsage.CheckedOutBy ?? (object)DBNull.Value;
                    chk_out_time.Value = assetUsage.CheckedOutTime ?? (object)DBNull.Value;
                    chk_out_cnd.Value = (int)assetUsage.CheckOutCondition;
                    chk_out_to.Value = assetUsage.CheckedOutTo ?? (object)DBNull.Value;
                    chk_out_cmt.Value = assetUsage.CheckedOutComment ?? (object)DBNull.Value;
                    chk_in_by.Value = assetUsage.CheckedInBy ?? (object)DBNull.Value;
                    chk_in_time.Value = assetUsage.CheckedInTime ?? (object)DBNull.Value;
                    chk_in_cnd.Value = (int)assetUsage.CheckedInCondition;
                    chk_in_cmt.Value = assetUsage.CheckedInComment ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetUsage.AssetTypeID;
                    event_loc.Value = assetUsage.UsageLocation ?? (object)DBNull.Value;
                    md_by.Value = assetUsage.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetUsage.ModifiedTime ?? (object)DBNull.Value;
                    chk_status.Value = assetUsage.CheckStatus;
                    from_loc.Value = assetUsage.CheckedOutFromLocation;

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

        public async Task<bool> EditAsync(AssetUsage assetUsage)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_ass_usg SET asset_id = @asset_id, start_date = @start_date, end_date = @end_date, ");
            sb.Append("usg_purpose = @usg_purpose, event_ds = @event_ds, chk_out_by = @event_ds, chk_out_time = @chk_out_time, ");
            sb.Append("chk_out_cnd = @chk_out_cnd, chk_out_to = @chk_out_to, chk_out_cmt = @chk_out_cmt, chk_in_by = @chk_out_cmt, ");
            sb.Append("chk_in_time = @chk_in_time, chk_in_cnd = @chk_in_cnd, chk_in_cmt = @chk_in_cmt, asst_typ_id = @asst_typ_id, ");
            sb.Append("event_loc = @event_loc, md_by = @md_by, md_dt = @md_dt, chk_status = @chk_status, from_loc = @from_loc ");
            sb.Append("WHERE (usg_id = @usg_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var usg_id = cmd.Parameters.Add("@usg_id", NpgsqlDbType.Integer);
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);
                    var start_date = cmd.Parameters.Add("@start_date", NpgsqlDbType.TimestampTz);
                    var end_date = cmd.Parameters.Add("@end_date", NpgsqlDbType.TimestampTz);
                    var usg_purpose = cmd.Parameters.Add("@usg_purpose", NpgsqlDbType.Text);
                    var event_ds = cmd.Parameters.Add("@event_ds", NpgsqlDbType.Text);
                    var chk_out_by = cmd.Parameters.Add("@chk_out_by", NpgsqlDbType.Text);
                    var chk_out_time = cmd.Parameters.Add("@chk_out_time", NpgsqlDbType.TimestampTz);
                    var chk_out_cnd = cmd.Parameters.Add("@chk_out_cnd", NpgsqlDbType.Integer);
                    var chk_out_to = cmd.Parameters.Add("@chk_out_to", NpgsqlDbType.Text);
                    var chk_out_cmt = cmd.Parameters.Add("@chk_out_cmt", NpgsqlDbType.Text);
                    var chk_in_by = cmd.Parameters.Add("@chk_in_by", NpgsqlDbType.Text);
                    var chk_in_time = cmd.Parameters.Add("@chk_in_time", NpgsqlDbType.TimestampTz);
                    var chk_in_cnd = cmd.Parameters.Add("@chk_in_cnd", NpgsqlDbType.Integer);
                    var chk_in_cmt = cmd.Parameters.Add("@chk_in_cmt", NpgsqlDbType.Text);
                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var event_loc = cmd.Parameters.Add("@event_loc", NpgsqlDbType.Text);
                    var md_by = cmd.Parameters.Add("@md_by", NpgsqlDbType.Text);
                    var md_dt = cmd.Parameters.Add("@md_dt", NpgsqlDbType.Text);
                    var chk_status = cmd.Parameters.Add("@chk_status", NpgsqlDbType.Text);
                    var from_loc = cmd.Parameters.Add("@from_loc", NpgsqlDbType.Text);
                    cmd.Prepare();
                    usg_id.Value = assetUsage.UsageID;
                    asset_id.Value = assetUsage.AssetID;
                    start_date.Value = assetUsage.UsageStartTime ?? (object)DBNull.Value;
                    usg_purpose.Value = assetUsage.Purpose ?? (object)DBNull.Value;
                    event_ds.Value = assetUsage.UsageDescription ?? (object)DBNull.Value;
                    chk_out_by.Value = assetUsage.CheckedOutBy ?? (object)DBNull.Value;
                    chk_out_time.Value = assetUsage.CheckedOutTime ?? (object)DBNull.Value;
                    chk_out_cnd.Value = (int)assetUsage.CheckOutCondition;
                    chk_out_to.Value = assetUsage.CheckedOutTo ?? (object)DBNull.Value;
                    chk_out_cmt.Value = assetUsage.CheckedOutComment ?? (object)DBNull.Value;
                    chk_in_by.Value = assetUsage.CheckedInBy ?? (object)DBNull.Value;
                    chk_in_time.Value = assetUsage.CheckedInTime ?? (object)DBNull.Value;
                    chk_in_cnd.Value = (int)assetUsage.CheckedInCondition;
                    chk_in_cmt.Value = assetUsage.CheckedInComment ?? (object)DBNull.Value;
                    asst_typ_id.Value = assetUsage.AssetTypeID;
                    event_loc.Value = assetUsage.UsageLocation ?? (object)DBNull.Value;
                    md_by.Value = assetUsage.ModifiedBy ?? (object)DBNull.Value;
                    md_dt.Value = assetUsage.ModifiedTime ?? (object)DBNull.Value;
                    chk_status.Value = assetUsage.CheckStatus;
                    from_loc.Value = assetUsage.CheckedOutFromLocation;

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

        public async Task<bool> DeleteAsync(int assetUsageId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_ass_usg	WHERE (usg_id = @usg_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var usg_id = cmd.Parameters.Add("@usg_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    usg_id.Value = assetUsageId;
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
