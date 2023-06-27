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
    public class AssetEquipmentGroupRepository : IAssetEquipmentGroupRepository
    {
        public IConfiguration _config { get; }
        public AssetEquipmentGroupRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<AssetEquipmentGroup> GetByIdAsync(int assetEquipmentGroupId)
        {
            AssetEquipmentGroup assetEquipmentGroup = new AssetEquipmentGroup();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.id, e.eqmt_grp_id, e.asm_asset_id, e.added_time, e.added_by, ");
            sb.Append("g.eqmt_grp_nm, a.asst_no, a.asst_nm, a.asst_ds, a.typ_id, t.typ_nm, ");
            sb.Append("a.cnd_sts, a.cur_loc FROM public.bam_eqmgrp_ass e ");
            sb.Append("INNER JOIN public.bam_eqmt_grps g ON e.eqmt_grp_id = g.eqmt_grp_id ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON e.asm_asset_id = a.asst_id  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON a.typ_id = t.typ_id ");
            sb.Append("WHERE e.eqmt_grp_id = @eqmt_grp_id");
            //sb.Append("ORDER BY t.typ_nm; ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_id = cmd.Parameters.Add("@eqmt_grp_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    eqmt_grp_id.Value = assetEquipmentGroupId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetEquipmentGroup.AssetEquipmentGroupID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"];
                            assetEquipmentGroup.EquipmentGroupID = reader["eqmt_grp_id"] == DBNull.Value ? 0 : (int)reader["eqmt_grp_id"];
                            assetEquipmentGroup.EquipmentGroupName = reader["eqmt_grp_nm"] == DBNull.Value ? string.Empty : reader["eqmt_grp_nm"].ToString();
                            assetEquipmentGroup.AssetID = reader["asm_asset_id"] == DBNull.Value ? string.Empty : reader["asm_asset_id"].ToString();
                            assetEquipmentGroup.AssetNo = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString();
                            assetEquipmentGroup.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                            assetEquipmentGroup.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                            assetEquipmentGroup.AddedBy = reader["added_by"] == DBNull.Value ? string.Empty : reader["added_by"].ToString();
                            assetEquipmentGroup.AddedTime = reader["added_time"] == DBNull.Value ? string.Empty : reader["added_time"].ToString();
                            assetEquipmentGroup.AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"];
                            assetEquipmentGroup.AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();
                            assetEquipmentGroup.ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"];
                            assetEquipmentGroup.CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetEquipmentGroup;
        }

        public async Task<IList<AssetEquipmentGroup>> GetByEquipmentGroupIdAsync(int equipmentGroupId)
        {
            IList<AssetEquipmentGroup> assetEquipmentGroups = new List<AssetEquipmentGroup>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.id, e.eqmt_grp_id, e.asm_asset_id, e.added_time, e.added_by, ");
            sb.Append("g.eqmt_grp_nm, a.asst_no, a.asst_nm, a.asst_ds, a.typ_id, t.typ_nm, ");
            sb.Append("a.cnd_sts, a.cur_loc FROM public.bam_eqmgrp_ass e ");
            sb.Append("INNER JOIN public.bam_eqmt_grps g ON e.eqmt_grp_id = g.eqmt_grp_id ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON e.asm_asset_id = a.asst_id  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON a.typ_id = t.typ_id ");
            sb.Append("WHERE (e.eqmt_grp_id = @eqmt_grp_id) ORDER BY t.typ_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_id = cmd.Parameters.Add("@eqmt_grp_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    eqmt_grp_id.Value = equipmentGroupId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetEquipmentGroups.Add(new AssetEquipmentGroup
                            {
                                AssetEquipmentGroupID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                EquipmentGroupID = reader["eqmt_grp_id"] == DBNull.Value ? 0 : (int)reader["eqmt_grp_id"],
                                EquipmentGroupName = reader["eqmt_grp_nm"] == DBNull.Value ? string.Empty : reader["eqmt_grp_nm"].ToString(),
                                AssetID = reader["asm_asset_id"] == DBNull.Value ? string.Empty : reader["asm_asset_id"].ToString(),
                                AssetNo = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AddedBy = reader["added_by"] == DBNull.Value ? string.Empty : reader["added_by"].ToString(),
                                AddedTime = reader["added_time"] == DBNull.Value ? string.Empty : reader["added_time"].ToString(),
                                AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                                CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
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
            return assetEquipmentGroups;
        }

        public async Task<IList<AssetEquipmentGroup>> GetByEquipmentIdAndEquipmentGroupIdAsync(int equipmentGroupId, string equipmentId)
        {
            IList<AssetEquipmentGroup> assetEquipmentGroups = new List<AssetEquipmentGroup>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.id, e.eqmt_grp_id, e.asm_asset_id, e.added_time, e.added_by, ");
            sb.Append("g.eqmt_grp_nm, a.asst_no, a.asst_nm, a.asst_ds, a.typ_id, t.typ_nm, ");
            sb.Append("a.cnd_sts, a.cur_loc FROM public.bam_eqmgrp_ass e ");
            sb.Append("INNER JOIN public.bam_eqmt_grps g ON e.eqmt_grp_id = g.eqmt_grp_id ");
            sb.Append("INNER JOIN public.asm_stt_asst a ON e.asm_asset_id = a.asst_id  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON a.typ_id = t.typ_id ");
            sb.Append("WHERE (e.eqmt_grp_id = @eqmt_grp_id) AND (e.asm_asset_id = @asm_asset_id) ");
            sb.Append("ORDER BY t.typ_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_id = cmd.Parameters.Add("@eqmt_grp_id", NpgsqlDbType.Integer);
                    var asm_asset_id = cmd.Parameters.Add("@asm_asset_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    eqmt_grp_id.Value = equipmentGroupId;
                    asm_asset_id.Value = equipmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetEquipmentGroups.Add(new AssetEquipmentGroup
                            {
                                AssetEquipmentGroupID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                EquipmentGroupID = reader["eqmt_grp_id"] == DBNull.Value ? 0 : (int)reader["eqmt_grp_id"],
                                EquipmentGroupName = reader["eqmt_grp_nm"] == DBNull.Value ? string.Empty : reader["eqmt_grp_nm"].ToString(),
                                AssetID = reader["asm_asset_id"] == DBNull.Value ? string.Empty : reader["asm_asset_id"].ToString(),
                                AssetNo = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AddedBy = reader["added_by"] == DBNull.Value ? string.Empty : reader["added_by"].ToString(),
                                AddedTime = reader["added_time"] == DBNull.Value ? string.Empty : reader["added_time"].ToString(),
                                AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),
                                ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                                CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
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
            return assetEquipmentGroups;
        }

        public async Task<bool> AddAsync(AssetEquipmentGroup assetEquipmentGroup)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_eqmgrp_ass(eqmt_grp_id, asm_asset_id, added_time, ");
            sb.Append("added_by) VALUES (@eqmt_grp_id, @asm_asset_id, @added_time, @added_by); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eqmt_grp_id = cmd.Parameters.Add("@eqmt_grp_id", NpgsqlDbType.Integer);
                    var asm_asset_id = cmd.Parameters.Add("@asm_asset_id", NpgsqlDbType.Text);
                    var added_time = cmd.Parameters.Add("@added_time", NpgsqlDbType.Text);
                    var added_by = cmd.Parameters.Add("@added_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    eqmt_grp_id.Value = assetEquipmentGroup.EquipmentGroupID;
                    asm_asset_id.Value = assetEquipmentGroup.AssetID;
                    added_time.Value = assetEquipmentGroup.AddedTime ?? (object)DBNull.Value;
                    added_by.Value = assetEquipmentGroup.AddedBy ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int assetEquipmentGroupId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_eqmgrp_ass WHERE (id = @id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    id.Value = assetEquipmentGroupId;
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
    }
}
