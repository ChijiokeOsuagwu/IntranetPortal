using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.SecurityModels;
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

    public class AssetBinLocationRepository : IAssetBinLocationRepository
    {
        public IConfiguration _config { get; }
        public AssetBinLocationRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== AssetBinLocation Action Methods ==========================//
        #region AssetBinLocation Action Methods

        public async Task<AssetBinLocation> GetByIdAsync(int assetBinLocationId)
        {
            AssetBinLocation assetBinLocation = new AssetBinLocation();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bnloc_id, bnloc_nm, bnloc_ds, loc_id, l.locname ");
            sb.Append("FROM public.asm_stt_bnlcs b INNER JOIN gst_locs l ON b.loc_id = l.locqk ");
            sb.Append("WHERE (bnloc_id = @bnloc_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    bnloc_id.Value = assetBinLocationId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetBinLocation.AssetBinLocationID = reader["bnloc_id"] == DBNull.Value ? 0 : (int)reader["bnloc_id"];
                            assetBinLocation.AssetBinLocationName = reader["bnloc_nm"] == DBNull.Value ? String.Empty : reader["bnloc_nm"].ToString();
                            assetBinLocation.AssetBinLocationDescription = reader["bnloc_ds"] == DBNull.Value ? String.Empty : reader["bnloc_ds"].ToString();

                            assetBinLocation.AssetLocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                            assetBinLocation.AssetLocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assetBinLocation;
        }

        public async Task<IList<AssetBinLocation>> SearchByNameAsync(string assetBinLocationName, string userId)
        {
            List<AssetBinLocation> assetBinLocationList = new List<AssetBinLocation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.bnloc_id, b.bnloc_nm, b.bnloc_ds, b.loc_id, l.locname ");
            sb.Append("FROM public.asm_stt_bnlcs b INNER JOIN gst_locs l ON b.loc_id = l.locqk ");
            sb.Append($"WHERE(LOWER(bnloc_nm) LIKE '%'||LOWER(@bnloc_nm)||'%') ");
            sb.Append("AND loc_id IN (SELECT loc_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("ORDER BY bnloc_nm; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bnloc_nm = cmd.Parameters.Add("@bnloc_nm", NpgsqlDbType.Text);
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bnloc_nm.Value = assetBinLocationName;
                    usr_id.Value = userId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetBinLocationList.Add(new AssetBinLocation()
                            {
                                AssetBinLocationID = reader["bnloc_id"] == DBNull.Value ? 0 : (int)reader["bnloc_id"],
                                AssetBinLocationName = reader["bnloc_nm"] == DBNull.Value ? String.Empty : reader["bnloc_nm"].ToString(),
                                AssetBinLocationDescription = reader["bnloc_ds"] == DBNull.Value ? String.Empty : reader["bnloc_ds"].ToString(),

                                AssetLocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                AssetLocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
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
            return assetBinLocationList;
        }

        public async Task<IList<AssetBinLocation>> GetByNameAsync(string assetBinLocationName)
        {
            List<AssetBinLocation> assetBinLocationList = new List<AssetBinLocation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bnloc_id, bnloc_nm, bnloc_ds, loc_id, l.locname ");
            sb.Append("FROM public.asm_stt_bnlcs b INNER JOIN gst_locs l ON b.loc_id = l.locqk ");
            sb.Append($"WHERE(LOWER(bnloc_nm) = LOWER(@bnloc_nm)) ");
            sb.Append("ORDER BY bnloc_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bnloc_nm = cmd.Parameters.Add("@bnloc_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bnloc_nm.Value = assetBinLocationName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetBinLocationList.Add(new AssetBinLocation()
                            {
                                AssetBinLocationID = reader["bnloc_id"] == DBNull.Value ? 0 : (int)reader["bnloc_id"],
                                AssetBinLocationName = reader["bnloc_nm"] == DBNull.Value ? String.Empty : reader["bnloc_nm"].ToString(),
                                AssetBinLocationDescription = reader["bnloc_ds"] == DBNull.Value ? String.Empty : reader["bnloc_ds"].ToString(),
                                AssetLocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                AssetLocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
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
            return assetBinLocationList;
        }

        public async Task<IList<AssetBinLocation>> GetByNameAsync(string assetBinLocationName, string userId)
        {
            List<AssetBinLocation> assetBinLocationList = new List<AssetBinLocation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bnloc_id, bnloc_nm, bnloc_ds, loc_id, l.locname ");
            sb.Append("FROM public.asm_stt_bnlcs b INNER JOIN gst_locs l ON b.loc_id = l.locqk ");
            sb.Append($"WHERE(LOWER(bnloc_nm) = LOWER(@bnloc_nm)) ");
            sb.Append("AND loc_id IN (SELECT loc_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("ORDER BY bnloc_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bnloc_nm = cmd.Parameters.Add("@bnloc_nm", NpgsqlDbType.Text);
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bnloc_nm.Value = assetBinLocationName;
                    usr_id.Value = userId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetBinLocationList.Add(new AssetBinLocation()
                            {
                                AssetBinLocationID = reader["bnloc_id"] == DBNull.Value ? 0 : (int)reader["bnloc_id"],
                                AssetBinLocationName = reader["bnloc_nm"] == DBNull.Value ? String.Empty : reader["bnloc_nm"].ToString(),
                                AssetBinLocationDescription = reader["bnloc_ds"] == DBNull.Value ? String.Empty : reader["bnloc_ds"].ToString(),
                                AssetLocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                AssetLocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
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
            return assetBinLocationList;
        }

        public async Task<IList<AssetBinLocation>> GetByLocationIdAsync(int locationId, string userId)
        {
            List<AssetBinLocation> assetBinLocationList = new List<AssetBinLocation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bnloc_id, bnloc_nm, bnloc_ds, loc_id, l.locname ");
            sb.Append("FROM public.asm_stt_bnlcs b INNER JOIN gst_locs l ON ");
            sb.Append("b.loc_id = l.locqk WHERE (loc_id = @loc_id) ");
            sb.Append("AND loc_id IN (SELECT loc_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ORDER BY bnloc_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    usr_id.Value = userId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assetBinLocationList.Add(new AssetBinLocation()
                            {
                                AssetBinLocationID = reader["bnloc_id"] == DBNull.Value ? 0 : (int)reader["bnloc_id"],
                                AssetBinLocationName = reader["bnloc_nm"] == DBNull.Value ? String.Empty : reader["bnloc_nm"].ToString(),
                                AssetBinLocationDescription = reader["bnloc_ds"] == DBNull.Value ? String.Empty : reader["bnloc_ds"].ToString(),

                                AssetLocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                AssetLocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
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
            return assetBinLocationList;
        }

        public async Task<IList<AssetBinLocation>> GetAllAsync(string userId)
        {
            List<AssetBinLocation> assetBinLocationList = new List<AssetBinLocation>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.bnloc_id, b.bnloc_nm, b.bnloc_ds, b.loc_id, l.locname ");
            sb.Append("FROM public.asm_stt_bnlcs b INNER JOIN gst_locs l ON b.loc_id = l.locqk ");
            sb.Append("WHERE b.loc_id IN (SELECT loc_id FROM public.sct_ntt_pms ");
            sb.Append("WHERE ntt_typ=0 AND usr_acct_id = @usr_id) ");
            sb.Append("ORDER BY l.locname, b.bnloc_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    usr_id.Value = userId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        assetBinLocationList.Add(new AssetBinLocation()
                        {
                            AssetBinLocationID = reader["bnloc_id"] == DBNull.Value ? 0 : (int)reader["bnloc_id"],
                            AssetBinLocationName = reader["bnloc_nm"] == DBNull.Value ? String.Empty : reader["bnloc_nm"].ToString(),
                            AssetBinLocationDescription = reader["bnloc_ds"] == DBNull.Value ? String.Empty : reader["bnloc_ds"].ToString(),

                            AssetLocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            AssetLocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
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
            return assetBinLocationList;
        }
        #endregion

        #region Asset Bin Location Write Action Methods
        public async Task<bool> AddAsync(AssetBinLocation assetBinLocation)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.asm_stt_bnlcs(bnloc_nm, bnloc_ds, loc_id) ");
            sb.Append("VALUES (@bnloc_nm, @bnloc_ds, @loc_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bnloc_nm = cmd.Parameters.Add("@bnloc_nm", NpgsqlDbType.Text);
                    var bnloc_ds = cmd.Parameters.Add("@bnloc_ds", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    bnloc_nm.Value = assetBinLocation.AssetBinLocationName;
                    bnloc_ds.Value = assetBinLocation.AssetBinLocationDescription ?? (object)DBNull.Value;
                    loc_id.Value = assetBinLocation.AssetLocationID;

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

        public async Task<bool> EditAsync(AssetBinLocation assetBinLocation)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.asm_stt_bnlcs SET bnloc_nm=@bnloc_nm, bnloc_ds=@bnloc_ds, ");
            sb.Append(" loc_id=@loc_id WHERE (bnloc_id = @bnloc_id);");
            string query = sb.ToString(); try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                    var bnloc_nm = cmd.Parameters.Add("@bnloc_nm", NpgsqlDbType.Text);
                    var bnloc_ds = cmd.Parameters.Add("@bnloc_ds", NpgsqlDbType.Text);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    bnloc_id.Value = assetBinLocation.AssetBinLocationID;
                    bnloc_nm.Value = assetBinLocation.AssetBinLocationName;
                    bnloc_ds.Value = assetBinLocation.AssetBinLocationDescription ?? (object)DBNull.Value;
                    loc_id.Value = assetBinLocation.AssetLocationID;

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

        public async Task<bool> DeleteAsync(int assetBinLocationId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.asm_stt_bnlcs WHERE (bnloc_id = @bnloc_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var bnloc_id = cmd.Parameters.Add("@bnloc_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    bnloc_id.Value = assetBinLocationId;
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
