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
    public class AssetRepository : IAssetRepository
    {
        public IConfiguration _config { get; }
        public AssetRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Asset Action Methods =====================================================//
        #region Asset Action Methods

        public async Task<Asset> GetByIdAsync(string assetId)
        {
            Asset asset = new Asset();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd,");
            sb.Append("s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname FROM public.asm_stt_asst s ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk WHERE (s.asst_id = @asst_id);");
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
                            asset.AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString();
                            asset.AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString();
                            asset.AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString();
                            asset.AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString();
                            asset.UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString();
                            asset.Condition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString();
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
                            asset.AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"];
                            asset.AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString();
                            asset.ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString();
                            asset.ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString();
                            asset.CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString();
                            asset.CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return asset;
        }

        public async Task<IList<Asset>> GetByNameAsync(string assetName)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, ");
            sb.Append("s.cur_loc, s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.asst_cd, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd, ");
            sb.Append("s.asst_cb, s.bloc_id, t.typ_nm, c.asst_ctgs_nm, l.locname FROM public.asm_stt_asst s ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append($"WHERE(LOWER(asst_nm) LIKE '%'||LOWER(@asst_nm)||'%') ORDER BY asst_nm;");
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
                            assetList.Add(new Asset()
                            {
                                AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                                Condition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
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
                                AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                                ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                                CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                                CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
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
            return assetList;
        }

        public async Task<IList<Asset>> GetByAssetTypeIdAsync(int assetTypeId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, s.cur_loc, ");
            sb.Append("s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd, ");
            sb.Append("c.asst_ctgs_nm, l.locname FROM public.asm_stt_asst s ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id  ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk WHERE (s.typ_id = @typ_id) ");
            sb.Append("ORDER BY c.asst_ctgs_nm, t.typ_nm, asst_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_type_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_type_id.Value = assetTypeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetList.Add(new Asset()
                            {
                                AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                                Condition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
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
                                AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                                ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                                CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                                CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
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
            return assetList;
        }

        public async Task<IList<Asset>> GetByCategoryIdAsync(int assetCategoryId)
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, s.cur_loc, ");
            sb.Append("s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd, ");
            sb.Append("c.asst_ctgs_nm, l.locname FROM public.asm_stt_asst s ");
            sb.Append(" INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk WHERE (ctg_id = @ctg_id) ");
            sb.Append("ORDER BY typ_nm, asst_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_category_id = cmd.Parameters.Add("@asst_category_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    asset_category_id.Value = assetCategoryId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetList.Add(new Asset()
                            {
                                AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                                Condition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
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
                                AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                                ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                                CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                                CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
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
            return assetList;
        }

        public async Task<IList<Asset>> GetAllAsync()
        {
            List<Asset> assetList = new List<Asset>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT s.asst_id, s.asst_no, s.asst_nm, s.asst_ds, s.typ_id, s.ctg_id, s.usg_stts, s.asst_cndt, s.cur_loc, ");
            sb.Append("s.mstr_id, s.pur_dt, s.pur_amt, s.img_pth, s.asst_mb, s.asst_md, s.asst_cd, s.asst_cb, s.bloc_id, t.typ_nm, ");
            sb.Append("(SELECT COUNT (ass_rsv_id) FROM public.asm_ass_rsvs WHERE asset_id = asst_id AND rsv_stt = 'Confirmed') AS no_bkd, ");
            sb.Append("c.asst_ctgs_nm, l.locname FROM public.asm_stt_asst s ");
            sb.Append("INNER JOIN public.asm_stt_typs t ON s.typ_id = t.typ_id ");
            sb.Append("INNER JOIN public.asm_stt_ctgs c ON t.ctg_id = c.asst_ctgs_id ");
            sb.Append("INNER JOIN public.gst_locs l ON s.bloc_id = l.locqk ");
            sb.Append("ORDER BY c.asst_ctgs_nm, t.typ_nm, s.asst_nm;");
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
                            assetList.Add(new Asset()
                            {
                                AssetID = reader["asst_id"] == DBNull.Value ? string.Empty : reader["asst_id"].ToString(),
                                AssetName = reader["asst_nm"] == DBNull.Value ? string.Empty : reader["asst_nm"].ToString(),
                                AssetNumber = reader["asst_no"] == DBNull.Value ? string.Empty : reader["asst_no"].ToString(),
                                AssetDescription = reader["asst_ds"] == DBNull.Value ? string.Empty : reader["asst_ds"].ToString(),
                                UsageStatus = reader["usg_stts"] == DBNull.Value ? string.Empty : reader["usg_stts"].ToString(),
                                Condition = reader["asst_cndt"] == DBNull.Value ? string.Empty : reader["asst_cndt"].ToString(),
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
                                AssetCategoryID = reader["ctg_id"] == DBNull.Value ? 0 : (int)reader["ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? String.Empty : reader["asst_ctgs_nm"].ToString(),
                                ModifiedBy = reader["asst_mb"] == DBNull.Value ? string.Empty : reader["asst_mb"].ToString(),
                                ModifiedDate = reader["asst_md"] == DBNull.Value ? string.Empty : reader["asst_md"].ToString(),
                                CreatedBy = reader["asst_cb"] == DBNull.Value ? string.Empty : reader["asst_cb"].ToString(),
                                CreatedDate = reader["asst_cd"] == DBNull.Value ? string.Empty : reader["asst_cd"].ToString(),
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
            return assetList;
        }

        public async Task<bool> AddAsync(Asset asset)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_stt_asst(asst_id, asst_no, asst_nm, asst_ds, typ_id, ctg_id, usg_stts, asst_cndt, cur_loc, ");
            sb.Append("mstr_id, pur_dt, pur_amt, img_pth, asst_mb, asst_md, asst_cd, asst_cb, bloc_id) ");
            sb.Append("VALUES (@asst_id, @asst_no, @asst_nm, @asst_ds, @typ_id, @ctg_id, @usg_stts, @asst_cndt, @cur_loc, ");
            sb.Append("@mstr_id, @pur_dt, @pur_amt, @img_pth, @asst_mb, @asst_md, @asst_cd, @asst_cb, @bloc_id);");
            string query = sb.ToString();
            try
            {
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
                    cmd.Prepare();
                    asst_id.Value = asset.AssetID ?? Guid.NewGuid().ToString();
                    asst_no.Value = asset.AssetNumber ?? (object)DBNull.Value;
                    asst_nm.Value = asset.AssetName;
                    asst_ds.Value = asset.AssetDescription ?? (object)DBNull.Value;
                    typ_id.Value = asset.AssetTypeID;
                    ctg_id.Value = asset.AssetCategoryID;
                    usg_stts.Value = asset.UsageStatus;
                    asst_cndt.Value = asset.Condition;
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

        public async Task<bool> EditAsync(Asset asset)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET asst_no=@asst_no, asst_nm=@asst_nm, asst_ds=@asst_ds, typ_id=@typ_id, ");
            sb.Append("ctg_id=@ctg_id, mstr_id=@mstr_id, pur_dt=@pur_dt, pur_amt=@pur_amt, img_pth=@img_pth, asst_mb=@asst_mb, ");
            sb.Append("asst_md=asst_md WHERE (asst_id=@asst_id); ");
            string query = sb.ToString();
            try
            {
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
                    cmd.Prepare();
                    asst_id.Value = asset.AssetID;
                    asst_no.Value = asset.AssetNumber;
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

        public async Task<bool> UpdateBaseLocationAsync(string assetId, int baseLocationId, string currentLocation, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_asst SET bloc_id = @bloc_id, cur_loc = @cur_loc, asst_mb = @asst_mb, ");
            sb.Append("asst_md = @asst_md WHERE (asst_id = @asst_id );");
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
                    cmd.Prepare();
                    asst_id.Value = assetId;
                    bloc_id.Value = baseLocationId;
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

        public async Task<bool> DeleteAsync(string assetId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_stt_asst WHERE (asst_id = @asst_id);";
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
