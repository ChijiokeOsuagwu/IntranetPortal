﻿using IntranetPortal.Base.Models.AssetManagerModels;
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
    public class AssetRepository : IAssetRepository
    {
        public IConfiguration _config { get; }
        public AssetRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //======== Assets Action Methods =========//
        #region Assets Read Action Methods
        public async Task<Asset> GetByIdAsync(string assetId)
        {
            Asset asset = new Asset();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE (s.asst_id = @asst_id) AND (s.is_del = false);");
            string query = sb.ToString();

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
                        asset.AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString();
                        asset.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                        asset.AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString();
                        asset.CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString();
                        asset.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                        asset.UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString();
                        asset.ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString();
                        asset.CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString();
                        asset.NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"];
                        asset.ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString();
                        asset.PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"];
                        asset.PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"];
                        asset.ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString();
                        asset.BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"];
                        asset.BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                        asset.AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"];
                        asset.AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString();
                        asset.AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"];
                        asset.AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString();
                        asset.AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"];
                        asset.AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                        asset.AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"];
                        asset.AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"];
                        asset.AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString();
                        asset.ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString();
                        asset.ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString();
                        asset.CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString();
                        asset.CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString();
                        asset.ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"];
                        asset.BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"];
                        asset.BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString();
                    }
            }
            await conn.CloseAsync();
            return asset;
        }
        public async Task<Asset> GetByNameAsync(string assetName)
        {
            Asset asset = new Asset();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE (s.asst_nm = @asst_nm) AND (s.is_del = false);");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asst_nm = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                asst_nm.Value = assetName;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        asset.AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString();
                        asset.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                        asset.AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString();
                        asset.CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString();
                        asset.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                        asset.UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString();
                        asset.ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString();
                        asset.CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString();
                        asset.NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"];
                        asset.ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString();
                        asset.PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"];
                        asset.PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"];
                        asset.ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString();
                        asset.BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"];
                        asset.BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                        asset.AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"];
                        asset.AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString();
                        asset.AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"];
                        asset.AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString();
                        asset.AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"];
                        asset.AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                        asset.AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"];
                        asset.AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"];
                        asset.AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString();
                        asset.ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString();
                        asset.ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString();
                        asset.CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString();
                        asset.CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString();
                        asset.ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"];
                        asset.BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"];
                        asset.BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString();
                    }
            }
            await conn.CloseAsync();
            return asset;
        }
        public async Task<IList<Asset>> SearchByNameAsync(string assetName, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.is_del = false) AND (LOWER(asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_name = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                asset_name.Value = assetName;
                usr_id.Value = userId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByAssetTypeIdAsync(int assetTypeId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.typ_id = @typ_id) AND (s.is_del = false) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                asset_type_id.Value = assetTypeId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByClassIdAsync(int assetClassId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.clss_id = @clss_id OR s.clss_id IS NULL) AND (s.is_del = false)");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                clss_id.Value = assetClassId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByCategoryIdAsync(int assetCategoryId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.ctg_id = @ctg_id) AND (s.is_del = false) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                ctg_id.Value = assetCategoryId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByDivisionIdAsync(int assetDivisionId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE (s.dvsn_id = @dvsn_id) AND (s.is_del = false)");
            sb.Append("AND s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var dvsn_id = cmd.Parameters.Add("@dvsn_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                dvsn_id.Value = assetDivisionId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByAssetGroupIdAsync(int assetGroupId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.grp_id = @grp_id) AND (s.is_del = false) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                grp_id.Value = assetGroupId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetAllAsync(string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.is_del = false) ORDER BY s.asst_nm; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_id.Value = userId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByAssetConditionAsync(int assetCondition, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.is_del = false) AND (s.cnd_sts = @cnd_sts) ");
            sb.Append("ORDER BY s.asst_nm; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                cnd_sts.Value = assetCondition;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }


        //====== Search Action Methods By BinLocations ================//
        public async Task<IList<Asset>> GetByBinLocationIdnAssetTypeIdAsync(int binLocationId, int assetTypeId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.typ_id = @typ_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bnloc_id = @bnloc_id) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                asset_type_id.Value = assetTypeId;
                bnloc_id.Value = binLocationId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBinLocationIdnAssetTypeIdnAssetConditionAsync(int binLocationId, int assetTypeId, int assetCondition, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.typ_id = @typ_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bnloc_id = @bnloc_id) AND (s.cnd_sts = @cnd_sts) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                asset_type_id.Value = assetTypeId;
                bnloc_id.Value = binLocationId;
                cnd_sts.Value = assetCondition;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBinLocationIdnAssetGroupIdAsync(int binLocationId, int assetGroupId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.grp_id = @grp_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bnloc_id = @bnloc_id) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                grp_id.Value = assetGroupId;
                bnloc_id.Value = binLocationId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBinLocationIdnAssetGroupIdnAssetConditionAsync(int binLocationId, int assetGroupId, int assetCondition, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.grp_id = @grp_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bnloc_id = @bnloc_id) AND (s.cnd_sts = @cnd_sts) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                grp_id.Value = assetGroupId;
                bnloc_id.Value = binLocationId;
                cnd_sts.Value = assetCondition;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBinLocationIdAsync(int binLocationId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.bnloc_id = @bnloc_id) ");
            sb.Append("AND (s.is_del = false) ORDER BY s.asst_nm; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                bnloc_id.Value = binLocationId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBinLocationIdnAssetConditionAsync(int binLocationId, int assetCondition, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.bnloc_id = @bnloc_id) AND (s.cnd_sts = @cnd_sts) ");
            sb.Append("AND (s.is_del = false) ORDER BY s.asst_nm; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                bnloc_id.Value = binLocationId;
                cnd_sts.Value = assetCondition;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }

        //===== Search Action Methods By BaseLocation ================//
        public async Task<IList<Asset>> GetByBaseLocationIdnAssetTypeIdAsync(int baseLocationId, int assetTypeId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.typ_id = @typ_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bloc_id = @bloc_id) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                asset_type_id.Value = assetTypeId;
                bloc_id.Value = baseLocationId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBaseLocationIdnAssetTypeIdnAssetConditionAsync(int baseLocationId, int assetTypeId, int assetCondition, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.typ_id = @typ_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bloc_id = @bloc_id) AND (s.cnd_sts = @cnd_sts) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var asset_type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                asset_type_id.Value = assetTypeId;
                bloc_id.Value = baseLocationId;
                cnd_sts.Value = assetCondition;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBaseLocationIdnAssetGroupIdAsync(int baseLocationId, int assetGroupId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.grp_id = @grp_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bloc_id = @bloc_id) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                grp_id.Value = assetGroupId;
                bloc_id.Value = baseLocationId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBaseLocationIdnAssetGroupIdnAssetConditionAsync(int baseLocationId, int assetGroupId, int assetCondition, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.grp_id = @grp_id) AND (s.is_del = false) ");
            sb.Append("AND (s.bloc_id = @bloc_id) AND (s.cnd_sts = @cnd_sts) ");
            sb.Append("ORDER BY s.asst_nm;");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                grp_id.Value = assetGroupId;
                bloc_id.Value = baseLocationId;
                cnd_sts.Value = assetCondition;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBaseLocationIdAsync(int baseLocationId, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.bloc_id = @bloc_id) ");
            sb.Append("AND (s.is_del = false) ORDER BY s.asst_nm; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                bloc_id.Value = baseLocationId;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }
        public async Task<IList<Asset>> GetByBaseLocationIdnAssetConditionAsync(int baseLocationId, int assetCondition, string userId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.cnd_sts, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname, dvsn_id, ");
            sb.Append("s.bnloc_id, b.bnloc_nm, s.clss_id, k.clss_nm, s.grp_id, g.grp_nm, ");
            sb.Append("CASE s.cnd_sts WHEN 0 THEN 'In Good Condition' ");
            sb.Append("WHEN 1 THEN 'Requires Repairs' ");
            sb.Append("WHEN 2 THEN 'Faulty Beyond Repair' ");
            sb.Append("WHEN 3 THEN 'End of Life Reached' END cnd_ds ");
            sb.Append("FROM public.asm_stt_asst s  ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("LEFT JOIN public.asm_stt_clss k ON k.clss_id = s.clss_id ");
            sb.Append("LEFT JOIN public.asm_stt_grps g ON g.grp_id = s.grp_id ");
            sb.Append("LEFT JOIN public.asm_stt_bnlcs b ON s.bnloc_id = b.bnloc_id ");
            sb.Append("WHERE s.dvsn_id IN (SELECT asst_dvsn_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("AND s.bloc_id IN (SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE ntt_typ=1 AND usr_acct_id = @usr_id) ");
            sb.Append("AND (s.bloc_id = @bloc_id) AND (s.cnd_sts = @cnd_sts) ");
            sb.Append("AND (s.is_del = false) ORDER BY s.asst_nm; ");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                usr_id.Value = userId;
                bloc_id.Value = baseLocationId;
                cnd_sts.Value = assetCondition;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        assetList.Add(new Asset()
                        {
                            AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                            AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                            AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                            CustomField = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
                            AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                            UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                            ConditionDescription = reader["cnd_ds"] == DBNull.Value ? string.Empty : reader["cnd_ds"].ToString(),
                            CurrentLocation = reader["cur_loc"] == DBNull.Value ? string.Empty : reader["cur_loc"].ToString(),
                            NoOfConfirmedBooking = reader["no_bkd"] == DBNull.Value ? 0 : (long)reader["no_bkd"],
                            ParentAssetID = reader["mstr_id"] == DBNull.Value ? string.Empty : reader["mstr_id"].ToString(),
                            PurchaseDate = reader["pur_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["pur_dt"],
                            PurchaseAmount = reader["pur_amt"] == DBNull.Value ? 0.00M : (decimal)reader["pur_amt"],
                            ImagePath = reader["img_pth"] == DBNull.Value ? string.Empty : reader["img_pth"].ToString(),
                            BaseLocationID = reader["bloc_id"] == DBNull.Value ? 0 : (int)reader["bloc_id"],
                            BaseLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            AssetTypeID = reader["typ_id"] == DBNull.Value ? 0 : (int)reader["typ_id"],
                            AssetTypeName = reader["typ_nm"] == DBNull.Value ? String.Empty : reader["typ_nm"].ToString(),
                            AssetClassID = reader["clss_id"] == DBNull.Value ? 0 : (int)reader["clss_id"],
                            AssetClassName = reader["clss_nm"] == DBNull.Value ? String.Empty : reader["clss_nm"].ToString(),
                            AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                            AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                            AssetDivisionID = reader["dvsn_id"] == DBNull.Value ? (int?)null : (int)reader["dvsn_id"],
                            AssetGroupID = reader["grp_id"] == DBNull.Value ? (int?)null : (int)reader["grp_id"],
                            AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),
                            ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                            ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                            CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                            CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
                            ConditionStatus = reader["cnd_sts"] == DBNull.Value ? AssetCondition.InGoodCondition : (AssetCondition)reader["cnd_sts"],
                            BinLocationID = reader["bnloc_id"] == DBNull.Value ? (int?)null : (int)reader["bnloc_id"],
                            BinLocationName = reader["bnloc_nm"] == DBNull.Value ? string.Empty : reader["bnloc_nm"].ToString(),
                        });
                    }
            }
            await conn.CloseAsync();
            return assetList;
        }

        #endregion

        #region Assets Write Action Methods
        public async Task<bool> AddAsync(Asset asset)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_stt_asst(asst_id, asst_no, asst_nm, asst_ds,  ");
            sb.Append("typ_id, ctg_id, usg_stts, asst_cndt, cur_loc, mstr_id, pur_dt, pur_amt, ");
            sb.Append("img_pth, asst_mb, asst_md, asst_cd, asst_cb, bloc_id, cnd_sts, is_del, ");
            sb.Append("bnloc_id, clss_id, dvsn_id, grp_id) VALUES (@asst_id, @asst_no, @asst_nm, ");
            sb.Append("@asst_ds, @typ_id, @ctg_id, @usg_stts, @asst_cndt, @cur_loc, ");
            sb.Append("@mstr_id, @pur_dt, @pur_amt, @img_pth, @asst_mb, @asst_md, @asst_cd,");
            sb.Append("@asst_cb, @bloc_id, @cnd_sts, default, @bnloc_id, @clss_id, @dvsn_id, ");
            sb.Append("@grp_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                var asst_no = cmd.Parameters.Add("@asst_no", NpgsqlDbType.Text);
                var asst_nm = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                var asst_ds = cmd.Parameters.Add("@asst_ds", NpgsqlDbType.Text);
                var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                var dvsn_id = cmd.Parameters.Add("@dvsn_id", NpgsqlDbType.Integer);
                var usg_stts = cmd.Parameters.Add("@usg_stts", NpgsqlDbType.Text);
                var asst_cndt = cmd.Parameters.Add("@asst_cndt", NpgsqlDbType.Text);
                var cur_loc = cmd.Parameters.Add("@cur_loc", NpgsqlDbType.Text);
                var mstr_id = cmd.Parameters.Add("@mstr_id", NpgsqlDbType.Text);
                var pur_dt = cmd.Parameters.Add("@pur_dt", NpgsqlDbType.TimestampTz);
                var pur_amt = cmd.Parameters.Add("@pur_amt", NpgsqlDbType.Numeric);
                var img_pth = cmd.Parameters.Add("@img_pth", NpgsqlDbType.Text);
                var asst_mb = cmd.Parameters.Add("@asst_mb", NpgsqlDbType.Text);
                var asst_md = cmd.Parameters.Add("@asst_md", NpgsqlDbType.Text);
                var asst_cb = cmd.Parameters.Add("@asst_cb", NpgsqlDbType.Text);
                var asst_cd = cmd.Parameters.Add("@asst_cd", NpgsqlDbType.Text);
                var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                asst_id.Value = asset.AssetID ?? Guid.NewGuid().ToString();
                asst_no.Value = asset.AssetNumber ?? (object)DBNull.Value;
                asst_nm.Value = asset.AssetName;
                asst_ds.Value = asset.AssetDescription ?? (object)DBNull.Value;
                typ_id.Value = asset.AssetTypeID;
                ctg_id.Value = asset.AssetCategoryID;
                dvsn_id.Value = asset.AssetDivisionID ?? (object)DBNull.Value;
                usg_stts.Value = asset.UsageStatus;
                asst_cndt.Value = asset.CustomField ?? (object)DBNull.Value;
                cur_loc.Value = asset.CurrentLocation ?? (object)DBNull.Value;
                mstr_id.Value = asset.ParentAssetID ?? (object)DBNull.Value;
                pur_dt.Value = asset.PurchaseDate ?? (object)DBNull.Value;
                pur_amt.Value = asset.PurchaseAmount ?? 0.00M;
                img_pth.Value = asset.ImagePath ?? (object)DBNull.Value;
                asst_mb.Value = asset.ModifiedBy ?? (object)DBNull.Value;
                asst_md.Value = asset.ModifiedDate ?? (object)DBNull.Value;
                asst_cb.Value = asset.CreatedBy ?? (object)DBNull.Value;
                asst_cd.Value = asset.CreatedDate ?? (object)DBNull.Value;
                bloc_id.Value = asset.BaseLocationID;
                cnd_sts.Value = (int)asset.ConditionStatus;
                bnloc_id.Value = asset.BinLocationID;
                clss_id.Value = asset.AssetClassID;
                grp_id.Value = asset.AssetGroupID;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditAsync(Asset asset)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET asst_no=@asst_no, asst_nm=@asst_nm, ");
            sb.Append("asst_ds=@asst_ds, typ_id=@typ_id, ctg_id=@ctg_id, mstr_id=@mstr_id, ");
            sb.Append("pur_dt=@pur_dt, pur_amt=@pur_amt, img_pth=@img_pth, asst_mb=@asst_mb, ");
            sb.Append("asst_md=@asst_md, cnd_sts=@cnd_sts, asst_cndt=@asst_cndt, ");
            sb.Append("bnloc_id=@bnloc_id, clss_id=@clss_id, grp_id = @grp_id ");
            sb.Append("WHERE (asst_id=@asst_id);");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                var asst_no = cmd.Parameters.Add("@asst_no", NpgsqlDbType.Text);
                var asst_nm = cmd.Parameters.Add("@asst_nm", NpgsqlDbType.Text);
                var asst_ds = cmd.Parameters.Add("@asst_ds", NpgsqlDbType.Text);
                var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var ctg_id = cmd.Parameters.Add("@ctg_id", NpgsqlDbType.Integer);
                var mstr_id = cmd.Parameters.Add("@mstr_id", NpgsqlDbType.Text);
                var pur_dt = cmd.Parameters.Add("@pur_dt", NpgsqlDbType.TimestampTz);
                var pur_amt = cmd.Parameters.Add("@pur_amt", NpgsqlDbType.Numeric);
                var img_pth = cmd.Parameters.Add("@img_pth", NpgsqlDbType.Text);
                var asst_mb = cmd.Parameters.Add("@asst_mb", NpgsqlDbType.Text);
                var asst_md = cmd.Parameters.Add("@asst_md", NpgsqlDbType.Text);
                var cnd_sts = cmd.Parameters.Add("@cnd_sts", NpgsqlDbType.Integer);
                var asst_cndt = cmd.Parameters.Add("@asst_cndt", NpgsqlDbType.Text);
                var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                var clss_id = cmd.Parameters.Add("@clss_id", NpgsqlDbType.Integer);
                var grp_id = cmd.Parameters.Add("@grp_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                asst_id.Value = asset.AssetID;
                asst_no.Value = asset.AssetNumber ?? (object)DBNull.Value;
                asst_nm.Value = asset.AssetName;
                asst_ds.Value = asset.AssetDescription ?? (object)DBNull.Value;
                typ_id.Value = asset.AssetTypeID;
                ctg_id.Value = asset.AssetCategoryID;
                mstr_id.Value = asset.ParentAssetID ?? (object)DBNull.Value;
                pur_dt.Value = asset.PurchaseDate ?? (object)DBNull.Value;
                pur_amt.Value = asset.PurchaseAmount ?? (object)DBNull.Value;
                img_pth.Value = asset.ImagePath ?? (object)DBNull.Value;
                asst_mb.Value = asset.ModifiedBy ?? (object)DBNull.Value;
                asst_md.Value = asset.ModifiedDate ?? (object)DBNull.Value;
                cnd_sts.Value = (int)asset.ConditionStatus;
                asst_cndt.Value = asset.CustomField ?? (object)DBNull.Value;
                bnloc_id.Value = asset.BinLocationID ?? (object)DBNull.Value;
                clss_id.Value = asset.AssetClassID ?? (object)DBNull.Value;
                grp_id.Value = asset.AssetGroupID ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateUsageStatusAsync(string assetId, string newStatus, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET usg_stts = @usg_stts, asst_mb = @asst_mb, ");
            sb.Append("asst_md = @asst_md WHERE (asst_id = @asst_id );");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    var usg_stts = cmd.Parameters.Add("@usg_stts", NpgsqlDbType.Text);
                    var asst_mb = cmd.Parameters.Add("@asst_mb", NpgsqlDbType.Text);
                    var asst_md = cmd.Parameters.Add("@asst_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asst_id.Value = assetId;
                    usg_stts.Value = newStatus;
                    asst_mb.Value = modifiedBy ?? (object)DBNull.Value;
                    asst_md.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} UTC";

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

        public async Task<bool> UpdateAssetConditionAsync(string assetId, string assetCondition, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET asst_cndt = @asst_cndt, asst_mb = @asst_mb,");
            sb.Append("asst_md = @asst_md  WHERE (asst_id = @asst_id );");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    var asst_cndt = cmd.Parameters.Add("@asst_cndt", NpgsqlDbType.Text);
                    var asst_mb = cmd.Parameters.Add("@asst_mb", NpgsqlDbType.Text);
                    var asst_md = cmd.Parameters.Add("@asst_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asst_id.Value = assetId;
                    asst_cndt.Value = assetCondition;
                    asst_mb.Value = modifiedBy ?? (object)DBNull.Value;
                    asst_md.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} UTC";
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

        public async Task<bool> UpdateCurrentLocationAsync(string assetId, string currentLocation, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET cur_loc = @cur_loc, asst_mb = @asst_mb, ");
            sb.Append("asst_md = @asst_md WHERE (asst_id = @asst_id );");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    var cur_loc = cmd.Parameters.Add("@cur_loc", NpgsqlDbType.Text);
                    var asst_mb = cmd.Parameters.Add("@asst_mb", NpgsqlDbType.Text);
                    var asst_md = cmd.Parameters.Add("@asst_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asst_id.Value = assetId;
                    cur_loc.Value = currentLocation;
                    asst_mb.Value = modifiedBy ?? (object)DBNull.Value;
                    asst_md.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} UTC";

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

        public async Task<bool> UpdateBaseLocationAsync(string assetId, int baseLocationId, string currentLocation, int? binLocationId, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET bloc_id = @bloc_id, cur_loc = @cur_loc, asst_mb = @asst_mb, ");
            sb.Append("asst_md = @asst_md, bnloc_id=@bnloc_id WHERE (asst_id = @asst_id );");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    var bloc_id = cmd.Parameters.Add("@bloc_id", NpgsqlDbType.Integer);
                    var cur_loc = cmd.Parameters.Add("@cur_loc", NpgsqlDbType.Text);
                    var asst_mb = cmd.Parameters.Add("@asst_mb", NpgsqlDbType.Text);
                    var asst_md = cmd.Parameters.Add("@asst_md", NpgsqlDbType.Text);
                    var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    asst_id.Value = assetId;
                    bloc_id.Value = baseLocationId;
                    cur_loc.Value = currentLocation;
                    asst_mb.Value = modifiedBy ?? (object)DBNull.Value;
                    asst_md.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} UTC";
                    bnloc_id.Value = binLocationId ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(string assetId, string deletedBy)
        {
            int rows = 0;
            string deletedTime = $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET is_del=TRUE, asst_mb=@asst_mb, asst_md=@asst_md ");
            sb.Append("WHERE (asst_id=@asst_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    var asst_mb = cmd.Parameters.Add("@asst_mb", NpgsqlDbType.Text);
                    var asst_md = cmd.Parameters.Add("@asst_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asst_id.Value = assetId;
                    asst_mb.Value = deletedBy ?? (object)DBNull.Value;
                    asst_md.Value = deletedTime ?? (object)DBNull.Value;

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

        public async Task<bool> DeletePermanentlyAsync(string assetId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DELETE FROM public.asm_ass_rsvs WHERE (asset_id = @asst_id); ");
            sb.AppendLine("DELETE FROM public.bam_dpmt_eqmt WHERE usg_id IN (SELECT usg_id FROM asm_ass_usg WHERE asset_id = @asst_id);");
            sb.AppendLine("DELETE FROM public.asm_ass_usg WHERE (asset_id = @asst_id); ");
            sb.AppendLine("DELETE FROM public.asm_mtnc_hst WHERE (asm_asst_id = @asst_id); ");
            sb.AppendLine("DELETE FROM public.asm_incdt_hst WHERE (asm_asst_id = @asst_id); ");
            sb.AppendLine("DELETE FROM public.asm_mvt_hst WHERE (asm_asst_id = @asst_id); ");
            sb.AppendLine("DELETE FROM public.bam_dpmt_eqmt WHERE(eqmt_asset_id = @asst_id); ");
            sb.AppendLine("DELETE FROM public.asm_stt_asst WHERE (asst_id = @asst_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var asst_id = cmd.Parameters.Add("@asst_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    asst_id.Value = assetId;
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
