using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Repositories.SecurityRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.SecurityRepositories
{
    public class LocationPermissionRepository : ILocationPermissionRepository
    {
        public IConfiguration _config { get; }
        public LocationPermissionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Location Permission Action Methods =====================================================//
        #region Location Permission Read Action Methods

        public async Task<LocationPermission> GetByIdAsync(int locationPermissionId)
        {
            LocationPermission locationPermission = new LocationPermission();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT loc_pms_id, usr_acct_id, ntt_typ, loc_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = loc_id) as loc_nm ");
            sb.Append("FROM public.sct_loc_pms ");
            sb.Append("WHERE loc_pms_id = @loc_pms_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var loc_pms_id = cmd.Parameters.Add("@loc_pms_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                loc_pms_id.Value = locationPermissionId;
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        locationPermission.LocationPermissionId = reader["loc_pms_id"] == DBNull.Value ? 0 : (int)reader["loc_pms_id"];
                        locationPermission.UserId = reader["usr_acct_id"] == DBNull.Value ? string.Empty : reader["usr_acct_id"].ToString();
                        locationPermission.LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                        locationPermission.PermissionTypeId = reader["ntt_typ"] == DBNull.Value ? 0 : (int)reader["ntt_typ"];
                        locationPermission.LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString();
                    }
            }
            return locationPermission;
        }

        public async Task<IList<LocationPermission>> GetByUserIdAsync(string userId)
        {
            List<LocationPermission> locationPermissionList = new List<LocationPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT loc_pms_id, usr_acct_id, ntt_typ, loc_id, ");
            sb.Append("(SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = loc_id) as loc_nm ");
            sb.Append("FROM public.sct_loc_pms ");
            sb.Append("WHERE LOWER(usr_acct_id) = LOWER(@usr_acct_id); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_acct_id = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_acct_id.Value = userId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    locationPermissionList.Add(new LocationPermission()
                    {
                        LocationPermissionId = reader["loc_pms_id"] == DBNull.Value ? 0 : (int)reader["loc_pms_id"],
                        UserId = reader["usr_acct_id"] == DBNull.Value ? string.Empty : reader["usr_acct_id"].ToString(),
                        LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                        LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),
                        PermissionTypeId = reader["ntt_typ"] == DBNull.Value ? 0 : (int)reader["ntt_typ"],
                    });
                }
            }
            await conn.CloseAsync();
            return locationPermissionList;
        }

        public async Task<IList<Location>> GetUnGrantedLocationsByUserIdAsync(string userId)
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT locname, loctype, lochq1, lochq2, locmb, ");
            sb.Append("locmd, loccb, loccd, locctr, locst, locqk ");
            sb.Append("FROM public.gst_locs WHERE locqk NOT IN ");
            sb.Append("(SELECT loc_id FROM public.sct_loc_pms ");
            sb.Append("WHERE LOWER(usr_acct_id) = LOWER(@usr_acct_id)); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var usr_acct_id = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                usr_acct_id.Value = userId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    locationList.Add(new Location()
                    {
                        LocationID = reader["locqk"] == DBNull.Value ? 0 : (int)reader["locqk"],
                        LocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
                        LocationCountry = reader["locctr"] == DBNull.Value ? String.Empty : reader["locctr"].ToString(),
                        LocationType = reader["loctype"] == DBNull.Value ? String.Empty : reader["loctype"].ToString(),
                        LocationHeadID1 = reader["lochq1"] == DBNull.Value ? String.Empty : reader["lochq1"].ToString(),
                        LocationHeadID2 = reader["lochq2"] == DBNull.Value ? String.Empty : reader["lochq2"].ToString(),
                        LocationState = reader["locst"] == DBNull.Value ? String.Empty : reader["locst"].ToString(),
                        ModifiedBy = reader["locmb"] == DBNull.Value ? string.Empty : reader["locmb"].ToString(),
                        ModifiedDate = reader["locmd"] == DBNull.Value ? string.Empty : reader["locmd"].ToString(),
                        CreatedBy = reader["loccb"] == DBNull.Value ? string.Empty : reader["loccb"].ToString(),
                        CreatedDate = reader["loccd"] == DBNull.Value ? string.Empty : reader["loccd"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return locationList;
        }

        #endregion

        #region Location Permission Write Action Methods

        public async Task<bool> AddAsync(LocationPermission locationPermission)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.sct_loc_pms(usr_acct_id, ntt_typ, ");
            sb.Append("loc_id) VALUES (@usr_acct_id, @ntt_typ, @loc_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_acct_id = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                var ntt_typ = cmd.Parameters.Add("@ntt_typ", NpgsqlDbType.Integer);
                var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                usr_acct_id.Value = locationPermission.UserId;
                ntt_typ.Value = locationPermission.PermissionTypeId;
                loc_id.Value = locationPermission.LocationId;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int locationPermissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.sct_loc_pms  ");
            sb.Append("WHERE (loc_pms_id=@loc_pms_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var loc_pms_id = cmd.Parameters.Add("@loc_pms_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                loc_pms_id.Value = locationPermissionId;
                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion
    }
}
