using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.GlobalSettingsRepositories
{
    public class LocationRepository : ILocationRepository
    {
        public IConfiguration _config { get; }
        public LocationRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============= Locations Action Methods =====================================//
        #region Location Action Methods

        public async Task<bool> AddLocationAsync(Location location)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.gst_locs(locname, loctype, lochq1, lochq2, locmb, locmd, loccb, loccd, locctr, locst) ");
            sb.Append($"VALUES (@locname, @loctype, @lochq1, @lochq2, @locmb, @locmd, @locmb, @locmd, @locctr, @locst);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var locationName = cmd.Parameters.Add("@locname", NpgsqlDbType.Text);
                    var locationType = cmd.Parameters.Add("@loctype", NpgsqlDbType.Text);
                    var locationHeadID1 = cmd.Parameters.Add("@lochq1", NpgsqlDbType.Text);
                    var locationHeadID2 = cmd.Parameters.Add("@lochq2", NpgsqlDbType.Text);
                    var locationModifiedBy = cmd.Parameters.Add("@locmb", NpgsqlDbType.Text);
                    var locationModifiedDate = cmd.Parameters.Add("@locmd", NpgsqlDbType.Text);
                    var locationCountry = cmd.Parameters.Add("@locctr", NpgsqlDbType.Text);
                    var locationState = cmd.Parameters.Add("@locst", NpgsqlDbType.Text);
                    cmd.Prepare();
                    locationName.Value = location.LocationName;
                    locationType.Value = location.LocationType;
                    locationHeadID1.Value = location.LocationHeadID1 ?? (object)DBNull.Value;
                    locationHeadID2.Value = location.LocationHeadID2 ?? (object)DBNull.Value;
                    locationCountry.Value = location.LocationCountry ?? (object)DBNull.Value;
                    locationState.Value = location.LocationState ?? (object)DBNull.Value;
                    locationModifiedBy.Value = location.ModifiedBy ?? (object)DBNull.Value;
                    locationModifiedDate.Value = location.ModifiedDate ?? (object)DBNull.Value;
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

        public async Task<bool> DeleteLocationAsync(int Id)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_locs WHERE (locqk = @locqk);";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var locationId = cmd.Parameters.Add("@locqk", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    locationId.Value = Id;
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

        public async Task<bool> EditLocationAsync(Location location)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.gst_locs 	SET locname=@locname, loctype=@loctype, lochq1=@lochq1, lochq2=@lochq2, ");
            sb.Append($"locmb=@locmb, locmd=@locmd, locctr=@locctr, locst=@locst WHERE (locqk = @locqk);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var locationId = cmd.Parameters.Add("@locqk", NpgsqlDbType.Integer);
                    var locationName = cmd.Parameters.Add("@locname", NpgsqlDbType.Text);
                    var locationType = cmd.Parameters.Add("@loctype", NpgsqlDbType.Text);
                    var locationHeadID1 = cmd.Parameters.Add("@lochq1", NpgsqlDbType.Text);
                    var locationHeadID2 = cmd.Parameters.Add("@lochq2", NpgsqlDbType.Text);
                    var locationModifiedBy = cmd.Parameters.Add("@locmb", NpgsqlDbType.Text);
                    var locationModifiedDate = cmd.Parameters.Add("@locmd", NpgsqlDbType.Text);
                    var locationCountry = cmd.Parameters.Add("@locctr", NpgsqlDbType.Text);
                    var locationState = cmd.Parameters.Add("@locst", NpgsqlDbType.Text);
                    cmd.Prepare();
                    locationId.Value = location.LocationID;
                    locationName.Value = location.LocationName;
                    locationType.Value = location.LocationType;
                    locationHeadID1.Value = location.LocationHeadID1 ?? (object)DBNull.Value;
                    locationHeadID2.Value = location.LocationHeadID2 ?? (object)DBNull.Value;
                    locationModifiedBy.Value = location.ModifiedBy ?? (object)DBNull.Value;
                    locationModifiedDate.Value = location.ModifiedDate ?? (object)DBNull.Value;
                    locationCountry.Value = location.LocationCountry ?? (object)DBNull.Value;
                    locationState.Value = location.LocationState ?? (object)DBNull.Value;
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

        public async Task<Location> GetLocationByIdAsync(int Id)
        {
            Location location = new Location();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (Id < 1) { return null; }
            sb.Append($"SELECT locqk, locname, loctype, lochq1, lochq2, locmb, locmd, loccb, loccd, ");
            sb.Append($"locctr, locst	FROM public.gst_locs WHERE (locqk = @locqk) ORDER BY locname LIMIT 1; ");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var locationId = cmd.Parameters.Add("@locqk", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    locationId.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            location.LocationID = reader["locqk"] == DBNull.Value ? 0 : (int)(reader["locqk"]);
                            location.LocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString();
                            location.LocationType = reader["loctype"] == DBNull.Value ? String.Empty : reader["loctype"].ToString();
                            location.LocationHeadID1 = reader["lochq1"] == DBNull.Value ? String.Empty : reader["lochq1"].ToString();
                            location.LocationHeadID2 = reader["lochq2"] == DBNull.Value ? String.Empty : reader["lochq2"].ToString();
                            location.LocationCountry = reader["locctr"] == DBNull.Value ? String.Empty : reader["locctr"].ToString();
                            location.LocationState = reader["locst"] == DBNull.Value ? String.Empty : reader["locst"].ToString();
                            location.ModifiedBy = reader["locmb"] == DBNull.Value ? string.Empty : reader["locmb"].ToString();
                            location.ModifiedDate = reader["locmd"] == DBNull.Value ? string.Empty : reader["locmd"].ToString();
                            location.CreatedBy = reader["loccb"] == DBNull.Value ? string.Empty : reader["loccb"].ToString();
                            location.CreatedDate = reader["loccd"] == DBNull.Value ? string.Empty : reader["loccd"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return location;
        }

        public async Task<IList<Location>> GetLocationsAsync()
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT locqk, locname, loctype, lochq1, lochq2, locmb, locmd, loccb, loccd, locctr, locst	");
            sb.Append($"FROM public.gst_locs ORDER BY locname; ");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
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
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return locationList;
        }

        public async Task<IList<Location>> GetOnlyStationsAsync()
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT locqk, locname, loctype, lochq1, lochq2, locmb, locmd, loccb, loccd, locctr, locst	");
            sb.Append($"FROM public.gst_locs WHERE(loctype = 'Station') ORDER BY locname; ");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        locationList.Add(new Location()
                        {
                            LocationID = reader["locqk"] == DBNull.Value ? 0 : (int)reader["locqk"],
                            LocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? String.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? String.Empty : reader["locst"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? String.Empty : reader["loctype"].ToString(),
                            LocationHeadID1 = reader["lochq1"] == DBNull.Value ? String.Empty : reader["lochq1"].ToString(),
                            LocationHeadID2 = reader["lochq2"] == DBNull.Value ? String.Empty : reader["lochq2"].ToString(),
                            ModifiedBy = reader["locmb"] == DBNull.Value ? string.Empty : reader["locmb"].ToString(),
                            ModifiedDate = reader["locmd"] == DBNull.Value ? string.Empty : reader["locmd"].ToString(),
                            CreatedBy = reader["loccb"] == DBNull.Value ? string.Empty : reader["loccb"].ToString(),
                            CreatedDate = reader["loccd"] == DBNull.Value ? string.Empty : reader["loccd"].ToString(),
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
            return locationList;
        }

        public async Task<IList<Location>> GetOnlyBureausAsync()
        {
            List<Location> locationList = new List<Location>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT locqk, locname, loctype, lochq1, lochq2, locmb, locmd, loccb, loccd, locctr, locst	");
            sb.Append($"FROM public.gst_locs WHERE(loctype = 'Bureau') ORDER BY locname; ");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        locationList.Add(new Location()
                        {
                            LocationID = reader["locqk"] == DBNull.Value ? 0 : (int)reader["locqk"],
                            LocationName = reader["locname"] == DBNull.Value ? String.Empty : reader["locname"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? String.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? String.Empty : reader["locst"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? String.Empty : reader["loctype"].ToString(),
                            LocationHeadID1 = reader["lochq1"] == DBNull.Value ? String.Empty : reader["lochq1"].ToString(),
                            LocationHeadID2 = reader["lochq2"] == DBNull.Value ? String.Empty : reader["lochq2"].ToString(),
                            ModifiedBy = reader["locmb"] == DBNull.Value ? string.Empty : reader["locmb"].ToString(),
                            ModifiedDate = reader["locmd"] == DBNull.Value ? string.Empty : reader["locmd"].ToString(),
                            CreatedBy = reader["loccb"] == DBNull.Value ? string.Empty : reader["loccb"].ToString(),
                            CreatedDate = reader["loccd"] == DBNull.Value ? string.Empty : reader["loccd"].ToString(),
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
            return locationList;
        }

        #endregion

        //=========== States Action Methods ==========================================//
        #region States Action Methods
        public async Task<IList<State>> GetStatesAsync()
        {
            List<State> statesList = new List<State>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = $"SELECT stts_cd, stts_nm, stts_rg, stts_ct	FROM public.gst_loc_stts ORDER BY stts_nm;";
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        statesList.Add(new State()
                        {
                            Code = reader["stts_cd"] == DBNull.Value ? string.Empty : reader["stts_cd"].ToString(),
                            Name = reader["stts_nm"] == DBNull.Value ? String.Empty : reader["stts_nm"].ToString(),
                            Region = reader["stts_rg"] == DBNull.Value ? String.Empty : reader["stts_rg"].ToString(),
                            Country = reader["stts_ct"] == DBNull.Value ? String.Empty : reader["stts_ct"].ToString(),
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
            return statesList;
        }

        public async Task<State> GetStateByNameAsync(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                throw new ArgumentNullException(nameof(stateName), "The required parameter [State Name] is missing or has an invalid value.");
            }

            State state = new State();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            string query = string.Empty;
            sb.Append($"SELECT stts_cd, stts_nm, stts_rg, stts_ct FROM public.gst_loc_stts "); 
            sb.Append($"WHERE (LOWER(stts_nm) = LOWER(@stts_nm)) ORDER BY stts_nm;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var stts_nm = cmd.Parameters.Add("@stts_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    stts_nm.Value = stateName ?? (object)DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            state.Code = reader["stts_cd"] == DBNull.Value ? string.Empty : reader["stts_cd"].ToString();
                            state.Name = reader["stts_nm"] == DBNull.Value ? String.Empty : reader["stts_nm"].ToString();
                            state.Region = reader["stts_rg"] == DBNull.Value ? String.Empty : reader["stts_rg"].ToString();
                            state.Country = reader["stts_ct"] == DBNull.Value ? String.Empty : reader["stts_ct"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return state;
        }


        public async Task<IList<Country>> GetCountriesAsync()
        {
            List<Country> countriesList = new List<Country>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = $"SELECT ctr_qk, ctr_nm FROM public.gst_loc_ctr ORDER BY ctr_nm ASC;";
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        countriesList.Add(new Country()
                        {
                            CountryID = reader["ctr_qk"] == DBNull.Value ? 0 : (int)reader["ctr_qk"],
                            CountryName = reader["ctr_nm"] == DBNull.Value ? String.Empty : reader["ctr_nm"].ToString(),
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
            return countriesList;
        }

        #endregion
    }
}
