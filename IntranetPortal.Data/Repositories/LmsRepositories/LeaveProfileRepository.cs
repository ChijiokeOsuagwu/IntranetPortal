using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Base.Repositories.LmsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.LmsRepositories
{
    public class LeaveProfileRepository : ILeaveProfileRepository
    {
        public IConfiguration _config { get; }
        public LeaveProfileRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //====== Leave Profiles Action Methods =======//
        #region Leave Profiles Action Methods

        public async Task<bool> AddAsync(LeaveProfile leaveProfile)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_lvs_pfls(lvs_pfl_nm, ");
            sb.Append("lvs_pfl_ds) VALUES (@lvs_pfl_nm, @lvs_pfl_ds); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_nm = cmd.Parameters.Add("@lvs_pfl_nm", NpgsqlDbType.Text);
                    var lvs_pfl_ds = cmd.Parameters.Add("@lvs_pfl_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pfl_nm.Value = leaveProfile.Name;
                    lvs_pfl_ds.Value = leaveProfile.Description ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int rows = 0;
            string query = "DELETE FROM public.lms_lvs_pfls WHERE (lvs_pfl_id = @lvs_pfl_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    lvs_pfl_id.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> EditAsync(LeaveProfile leaveProfile)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lms_lvs_pfls SET lvs_pfl_nm=@lvs_pfl_nm, ");
            sb.Append("lvs_pfl_ds=@lvs_pfl_ds WHERE (lvs_pfl_id=@lvs_pfl_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlDbType.Integer);
                    var lvs_pfl_nm = cmd.Parameters.Add("@lvs_pfl_nm", NpgsqlDbType.Text);
                    var lvs_pfl_ds = cmd.Parameters.Add("@lvs_pfl_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pfl_id.Value = leaveProfile.Id;
                    lvs_pfl_nm.Value = leaveProfile.Name;
                    lvs_pfl_ds.Value = leaveProfile.Description ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<LeaveProfile> GetByIdAsync(int id)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (id < 1) { return null; }
            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds ");
            sb.Append("FROM public.lms_lvs_pfls ");
            sb.Append("WHERE (lvs_pfl_id = @lvs_pfl_id);");
            query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_pfl_id.Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            leaveProfile.Id = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"];
                            leaveProfile.Name = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString();
                            leaveProfile.Description = reader["lvs_pfl_ds"] == DBNull.Value ? string.Empty : reader["lvs_pfl_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return leaveProfile;
        }

        public async Task<LeaveProfile> GetByNameAsync(string profileName)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(profileName)) { return null; }
            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds ");
            sb.Append("FROM public.lms_lvs_pfls ");
            sb.Append("WHERE LOWER(lvs_pfl_nm) = LOWER(@lvs_pfl_nm);");
            query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_nm = cmd.Parameters.Add("@lvs_pfl_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_pfl_nm.Value = profileName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            leaveProfile.Id = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"];
                            leaveProfile.Name = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString();
                            leaveProfile.Description = reader["lvs_pfl_ds"] == DBNull.Value ? string.Empty : reader["lvs_pfl_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return leaveProfile;
        }

        public async Task<List<LeaveProfile>> GetAllAsync()
        {
            List<LeaveProfile> leaveProfiles = new List<LeaveProfile>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds ");
            sb.Append("FROM public.lms_lvs_pfls ");
            sb.Append("ORDER BY lvs_pfl_nm; ");
            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfiles.Add(new LeaveProfile()
                        {
                            Id = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
                            Name = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
                            Description = reader["lvs_pfl_ds"] == DBNull.Value ? string.Empty : reader["lvs_pfl_ds"].ToString()
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfiles;
        }

        #endregion

    }
}
