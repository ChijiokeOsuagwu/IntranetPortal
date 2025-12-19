using IntranetPortal.Base.Models.LeaveModels;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.LeaveRepositories
{
    public class LeaveRepository
    {
        public IConfiguration _config { get; }
        public LeaveRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Leave Types Action Methods

        public async Task<bool> AddLeaveTypeAsync(LeaveType leaveType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_typs(lvs_typ_cd, ");
            sb.Append("lvs_typ_nm, lvs_typ_ds) VALUES (@lvs_typ_cd, ");
            sb.Append("@lvs_typ_nm, @lvs_typ_ds); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_typ_nm = cmd.Parameters.Add("@lvs_typ_nm", NpgsqlDbType.Text);
                    var lvs_typ_ds = cmd.Parameters.Add("@lvs_typ_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_typ_cd.Value = leaveType.Code;
                    lvs_typ_nm.Value = leaveType.Name;
                    lvs_typ_ds.Value = leaveType.Description ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> DeleteLeaveTypeAsync(string code)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_typs WHERE (lvs_typ_cd = @lvs_typ_cd);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_typ_cd.Value = code;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> EditLeaveTypeAsync(LeaveType leaveType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_typs SET lvs_typ_nm=@lvs_typ_nm, ");
            sb.Append("lvs_typ_ds=@lvs_typ_ds WHERE (lvs_typ_cd=@lvs_typ_cd); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_typ_nm = cmd.Parameters.Add("@lvs_typ_nm", NpgsqlDbType.Text);
                    var lvs_typ_ds = cmd.Parameters.Add("@lvs_typ_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_typ_cd.Value = leaveType.Code;
                    lvs_typ_nm.Value = leaveType.Name;
                    lvs_typ_ds.Value = leaveType.Description ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<LeaveType> GetLeaveTypeByCodeAsync(string code)
        {
            LeaveType leaveType = new LeaveType();
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(code)) { return null; }
            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds, lvs_typ_sy ");
            sb.Append("FROM public.lvm_lvs_typs ");
            sb.Append("WHERE (lvs_typ_cd = @lvs_typ_cd);");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_typ_cd.Value = code;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            leaveType.Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString();
                            leaveType.Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                            leaveType.Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString();
                            leaveType.IsSystem = reader["lvs_typ_sy"] == DBNull.Value ? false : (bool)reader["lvs_typ_sy"];
                        }
                }
                await conn.CloseAsync();
            }
            return leaveType;
        }

        public async Task<LeaveType> GetLeaveTypeByNameAsync(string name)
        {
            LeaveType leaveType = new LeaveType();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(name)) { return null; }
            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds, lvs_typ_sy ");
            sb.Append("FROM public.lvm_lvs_typs ");
            sb.Append("WHERE LOWER(lvs_typ_nm) = LOWER(@lvs_typ_nm);");
            query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_typ_nm = cmd.Parameters.Add("@lvs_typ_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_typ_nm.Value = name;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            leaveType.Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString();
                            leaveType.Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                            leaveType.Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString();
                            leaveType.IsSystem = reader["lvs_typ_sy"] == DBNull.Value ? false : (bool)reader["lvs_typ_sy"];
                        }
                }
                await conn.CloseAsync();
            }
            return leaveType;
        }

        public async Task<List<LeaveType>> GetAllLeaveTypesAsync()
        {
            List<LeaveType> leaveTypes = new List<LeaveType>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds, lvs_typ_sy ");
            sb.Append("FROM public.lvm_lvs_typs ");
            sb.Append("ORDER BY lvs_typ_nm; ");
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
                        leaveTypes.Add(new LeaveType()
                        {
                            Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString(),
                            Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString(),
                            IsSystem = reader["lvs_typ_sy"] == DBNull.Value ? false : (bool)reader["lvs_typ_sy"]
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveTypes;
        }

        public async Task<List<LeaveType>> GetAllLeaveTypesExcludingSystemAsync()
        {
            List<LeaveType> leaveTypes = new List<LeaveType>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_typ_cd, lvs_typ_nm, lvs_typ_ds, lvs_typ_sy ");
            sb.Append("FROM public.lvm_lvs_typs WHERE lvs_typ_sy = false ");
            sb.Append("ORDER BY lvs_typ_nm; ");
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
                        leaveTypes.Add(new LeaveType()
                        {
                            Code = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : (reader["lvs_typ_cd"]).ToString(),
                            Name = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Description = reader["lvs_typ_ds"] == DBNull.Value ? string.Empty : reader["lvs_typ_ds"].ToString(),
                            IsSystem = reader["lvs_typ_sy"] == DBNull.Value ? false : (bool)reader["lvs_typ_sy"]
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveTypes;
        }
        #endregion

        #region Leave Profiles Action Methods

        public async Task<bool> AddLeaveProfileAsync(LeaveProfile leaveProfile)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_pfls(lvs_pfl_nm, ");
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

        public async Task<bool> DeleteLeaveProfileAsync(int id)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_pfls WHERE (lvs_pfl_id = @lvs_pfl_id);";
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

        public async Task<bool> EditLeaveProfileAsync(LeaveProfile leaveProfile)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_pfls SET lvs_pfl_nm=@lvs_pfl_nm, ");
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

        public async Task<LeaveProfile> GetLeaveProfileByIdAsync(int id)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (id < 1) { return null; }
            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds ");
            sb.Append("FROM public.lvm_lvs_pfls ");
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

        public async Task<LeaveProfile> GetLeaveProfileByNameAsync(string profileName)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(profileName)) { return null; }
            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds ");
            sb.Append("FROM public.lvm_lvs_pfls ");
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

        public async Task<List<LeaveProfile>> GetAllLeaveProfilesAsync()
        {
            List<LeaveProfile> leaveProfiles = new List<LeaveProfile>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds ");
            sb.Append("FROM public.lvm_lvs_pfls ");
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

        #region Leave Profile Details Action Methods

        #region Leave Profile Details Read Action Methods
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileIdAsync(int profileId)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.is_yrly, d.cancarryover, d.is_mntz, d.lvs_dur, ");
            sb.Append("d.dur_typ, d.carryover_end_mn, d.lvs_dur_ds,  ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("CASE WHEN d.carryover_end_mn = 1 THEN 'January' ");
            sb.Append("WHEN d.carryover_end_mn = 2 THEN 'February' ");
            sb.Append("WHEN d.carryover_end_mn = 3 THEN 'March' ");
            sb.Append("WHEN d.carryover_end_mn = 4 THEN 'April' ");
            sb.Append("WHEN d.carryover_end_mn = 5 THEN 'May' ");
            sb.Append("WHEN d.carryover_end_mn = 6 THEN 'June' ");
            sb.Append("WHEN d.carryover_end_mn = 7 THEN 'July' ");
            sb.Append("WHEN d.carryover_end_mn = 8 THEN 'August' ");
            sb.Append("WHEN d.carryover_end_mn = 9 THEN 'September' ");
            sb.Append("WHEN d.carryover_end_mn = 10 THEN 'October' ");
            sb.Append("WHEN d.carryover_end_mn = 11 THEN 'November' ");
            sb.Append("WHEN d.carryover_end_mn = 12 THEN 'December' ");
            sb.Append("END as carryover_end_mn_name, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE (d.lvs_pfl_id = @lvs_pfl_id) ");
            sb.Append("ORDER BY d.lvs_typ_cd; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlTypes.NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_pfl_id.Value = profileId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfileDetails.Add(new LeaveProfileDetail()
                        {
                            Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
                            ProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
                            ProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
                            DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
                            DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
                            IsYearly = reader["is_yrly"] == DBNull.Value ? false : (bool)reader["is_yrly"],
                            CanBeCarriedOver = reader["cancarryover"] == DBNull.Value ? false : (bool)reader["cancarryover"],
                            CanBeMonetized = reader["is_mntz"] == DBNull.Value ? false : (bool)reader["is_mntz"],
                            CarryOverEndMonth = reader["carryover_end_mn"] == DBNull.Value ? 0 : (int)reader["carryover_end_mn"],
                            CarryOverEndMonthName = reader["carryover_end_mn_name"] == DBNull.Value ? string.Empty : reader["carryover_end_mn_name"].ToString(),
                            DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailByIdAsync(int leaveProfileDetailId)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.is_yrly, d.cancarryover, d.is_mntz, d.lvs_dur, ");
            sb.Append("d.dur_typ, d.carryover_end_mn, d.lvs_dur_ds,  ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("CASE WHEN d.carryover_end_mn = 1 THEN 'January' ");
            sb.Append("WHEN d.carryover_end_mn = 2 THEN 'February' ");
            sb.Append("WHEN d.carryover_end_mn = 3 THEN 'March' ");
            sb.Append("WHEN d.carryover_end_mn = 4 THEN 'April' ");
            sb.Append("WHEN d.carryover_end_mn = 5 THEN 'May' ");
            sb.Append("WHEN d.carryover_end_mn = 6 THEN 'June' ");
            sb.Append("WHEN d.carryover_end_mn = 7 THEN 'July' ");
            sb.Append("WHEN d.carryover_end_mn = 8 THEN 'August' ");
            sb.Append("WHEN d.carryover_end_mn = 9 THEN 'September' ");
            sb.Append("WHEN d.carryover_end_mn = 10 THEN 'October' ");
            sb.Append("WHEN d.carryover_end_mn = 11 THEN 'November' ");
            sb.Append("WHEN d.carryover_end_mn = 12 THEN 'December' ");
            sb.Append("END as carryover_end_mn_name, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE (d.pfl_dtl_id = @pfl_dtl_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var pfl_dtl_id = cmd.Parameters.Add("@pfl_dtl_id", NpgsqlTypes.NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    pfl_dtl_id.Value = leaveProfileDetailId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfileDetails.Add(new LeaveProfileDetail()
                        {
                            Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
                            ProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
                            ProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
                            DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
                            DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
                            IsYearly = reader["is_yrly"] == DBNull.Value ? false : (bool)reader["is_yrly"],
                            CanBeCarriedOver = reader["cancarryover"] == DBNull.Value ? false : (bool)reader["cancarryover"],
                            CanBeMonetized = reader["is_mntz"] == DBNull.Value ? false : (bool)reader["is_mntz"],
                            CarryOverEndMonth = reader["carryover_end_mn"] == DBNull.Value ? 0 : (int)reader["carryover_end_mn"],
                            CarryOverEndMonthName = reader["carryover_end_mn_name"] == DBNull.Value ? string.Empty : reader["carryover_end_mn_name"].ToString(),
                            DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileIdnLeaveTypeAsync(int profileId, string leaveTypeCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.is_yrly, d.cancarryover, d.is_mntz, d.lvs_dur, ");
            sb.Append("d.dur_typ, d.carryover_end_mn, d.lvs_dur_ds,  ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("CASE WHEN d.carryover_end_mn = 1 THEN 'January' ");
            sb.Append("WHEN d.carryover_end_mn = 2 THEN 'February' ");
            sb.Append("WHEN d.carryover_end_mn = 3 THEN 'March' ");
            sb.Append("WHEN d.carryover_end_mn = 4 THEN 'April' ");
            sb.Append("WHEN d.carryover_end_mn = 5 THEN 'May' ");
            sb.Append("WHEN d.carryover_end_mn = 6 THEN 'June' ");
            sb.Append("WHEN d.carryover_end_mn = 7 THEN 'July' ");
            sb.Append("WHEN d.carryover_end_mn = 8 THEN 'August' ");
            sb.Append("WHEN d.carryover_end_mn = 9 THEN 'September' ");
            sb.Append("WHEN d.carryover_end_mn = 10 THEN 'October' ");
            sb.Append("WHEN d.carryover_end_mn = 11 THEN 'November' ");
            sb.Append("WHEN d.carryover_end_mn = 12 THEN 'December' ");
            sb.Append("END as carryover_end_mn_name, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE (d.lvs_pfl_id = @lvs_pfl_id) ");
            sb.Append("AND (d.lvs_typ_cd = @lvs_typ_cd); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_pfl_id.Value = profileId;
                    lvs_typ_cd.Value = leaveTypeCode;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfileDetails.Add(new LeaveProfileDetail()
                        {
                            Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
                            ProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
                            ProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
                            DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
                            DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
                            IsYearly = reader["is_yrly"] == DBNull.Value ? false : (bool)reader["is_yrly"],
                            CanBeCarriedOver = reader["cancarryover"] == DBNull.Value ? false : (bool)reader["cancarryover"],
                            CanBeMonetized = reader["is_mntz"] == DBNull.Value ? false : (bool)reader["is_mntz"],
                            CarryOverEndMonth = reader["carryover_end_mn"] == DBNull.Value ? 0 : (int)reader["carryover_end_mn"],
                            CarryOverEndMonthName = reader["carryover_end_mn_name"] == DBNull.Value ? string.Empty : reader["carryover_end_mn_name"].ToString(),
                            DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByEmployeeIdnLeaveTypeAsync(string employeeId)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.is_yrly, d.cancarryover, d.is_mntz, d.lvs_dur, ");
            sb.Append("d.dur_typ, d.carryover_end_mn, d.lvs_dur_ds,  ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("CASE WHEN d.carryover_end_mn = 1 THEN 'January' ");
            sb.Append("WHEN d.carryover_end_mn = 2 THEN 'February' ");
            sb.Append("WHEN d.carryover_end_mn = 3 THEN 'March' ");
            sb.Append("WHEN d.carryover_end_mn = 4 THEN 'April' ");
            sb.Append("WHEN d.carryover_end_mn = 5 THEN 'May' ");
            sb.Append("WHEN d.carryover_end_mn = 6 THEN 'June' ");
            sb.Append("WHEN d.carryover_end_mn = 7 THEN 'July' ");
            sb.Append("WHEN d.carryover_end_mn = 8 THEN 'August' ");
            sb.Append("WHEN d.carryover_end_mn = 9 THEN 'September' ");
            sb.Append("WHEN d.carryover_end_mn = 10 THEN 'October' ");
            sb.Append("WHEN d.carryover_end_mn = 11 THEN 'November' ");
            sb.Append("WHEN d.carryover_end_mn = 12 THEN 'December' ");
            sb.Append("END as carryover_end_mn_name, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE d.lvs_pfl_id = (SELECT lvs_pfl_id FROM public.erm_emp_inf ");
            sb.Append("WHERE emp_id = @emp_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfileDetails.Add(new LeaveProfileDetail()
                        {
                            Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
                            ProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
                            ProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
                            DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
                            DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
                            IsYearly = reader["is_yrly"] == DBNull.Value ? false : (bool)reader["is_yrly"],
                            CanBeCarriedOver = reader["cancarryover"] == DBNull.Value ? false : (bool)reader["cancarryover"],
                            CanBeMonetized = reader["is_mntz"] == DBNull.Value ? false : (bool)reader["is_mntz"],
                            CarryOverEndMonth = reader["carryover_end_mn"] == DBNull.Value ? 0 : (int)reader["carryover_end_mn"],
                            CarryOverEndMonthName = reader["carryover_end_mn_name"] == DBNull.Value ? string.Empty : reader["carryover_end_mn_name"].ToString(),
                            DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails;
        }
        public async Task<LeaveProfileDetail> GetLeaveProfileDetailByEmployeeIdnLeaveTypeAsync(string employeeId, string leaveTypeCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.is_yrly, d.cancarryover, d.is_mntz, d.lvs_dur, ");
            sb.Append("d.dur_typ, d.carryover_end_mn, d.lvs_dur_ds,  ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("CASE WHEN d.carryover_end_mn = 1 THEN 'January' ");
            sb.Append("WHEN d.carryover_end_mn = 2 THEN 'February' ");
            sb.Append("WHEN d.carryover_end_mn = 3 THEN 'March' ");
            sb.Append("WHEN d.carryover_end_mn = 4 THEN 'April' ");
            sb.Append("WHEN d.carryover_end_mn = 5 THEN 'May' ");
            sb.Append("WHEN d.carryover_end_mn = 6 THEN 'June' ");
            sb.Append("WHEN d.carryover_end_mn = 7 THEN 'July' ");
            sb.Append("WHEN d.carryover_end_mn = 8 THEN 'August' ");
            sb.Append("WHEN d.carryover_end_mn = 9 THEN 'September' ");
            sb.Append("WHEN d.carryover_end_mn = 10 THEN 'October' ");
            sb.Append("WHEN d.carryover_end_mn = 11 THEN 'November' ");
            sb.Append("WHEN d.carryover_end_mn = 12 THEN 'December' ");
            sb.Append("END as carryover_end_mn_name, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE d.lvs_pfl_id = (SELECT lvs_pfl_id FROM public.erm_emp_inf ");
            sb.Append("WHERE emp_id = @emp_id) ");
            sb.Append("AND (d.lvs_typ_cd = @lvs_typ_cd); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    lvs_typ_cd.Value = leaveTypeCode;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfileDetails.Add(new LeaveProfileDetail()
                        {
                            Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
                            ProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
                            ProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
                            DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
                            DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
                            IsYearly = reader["is_yrly"] == DBNull.Value ? false : (bool)reader["is_yrly"],
                            CanBeCarriedOver = reader["cancarryover"] == DBNull.Value ? false : (bool)reader["cancarryover"],
                            CanBeMonetized = reader["is_mntz"] == DBNull.Value ? false : (bool)reader["is_mntz"],
                            CarryOverEndMonth = reader["carryover_end_mn"] == DBNull.Value ? 0 : (int)reader["carryover_end_mn"],
                            CarryOverEndMonthName = reader["carryover_end_mn_name"] == DBNull.Value ? string.Empty : reader["carryover_end_mn_name"].ToString(),
                            DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails[0];
        }
        #endregion

        #region Leave Profile Details Write Action Methods
        public async Task<bool> AddLeaveProfileDetailAsync(LeaveProfileDetail leaveProfileDetail)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_pfdt(lvs_pfl_id, ");
            sb.Append("lvs_typ_cd, is_yrly, cancarryover, is_mntz, ");
            sb.Append("lvs_dur, dur_typ, carryover_end_mn, lvs_dur_ds) ");
            sb.Append("VALUES (@lvs_pfl_id, @lvs_typ_cd, @is_yrly, ");
            sb.Append("@cancarryover, @is_mntz, @lvs_dur, @dur_typ, ");
            sb.Append("@carry_over_end_mn, @lvs_dur_ds); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var is_yrly = cmd.Parameters.Add("@is_yrly", NpgsqlDbType.Boolean);
                    var cancarryover = cmd.Parameters.Add("@cancarryover", NpgsqlDbType.Boolean);
                    var is_mntz = cmd.Parameters.Add("@is_mntz", NpgsqlDbType.Boolean);
                    var lvs_dur = cmd.Parameters.Add("@lvs_dur", NpgsqlDbType.Integer);
                    var dur_typ = cmd.Parameters.Add("@dur_typ", NpgsqlDbType.Integer);
                    var carryover_end_mn = cmd.Parameters.Add("@carryover_end_mn", NpgsqlDbType.Integer);
                    var lvs_dur_ds = cmd.Parameters.Add("@lvs_dur_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pfl_id.Value = leaveProfileDetail.ProfileId;
                    lvs_typ_cd.Value = leaveProfileDetail.LeaveTypeCode;
                    is_yrly.Value = leaveProfileDetail.IsYearly;
                    cancarryover.Value = leaveProfileDetail.CanBeCarriedOver;
                    is_mntz.Value = leaveProfileDetail.CanBeMonetized;
                    lvs_dur.Value = leaveProfileDetail.Duration;
                    dur_typ.Value = leaveProfileDetail.DurationTypeId;
                    carryover_end_mn.Value = leaveProfileDetail.CarryOverEndMonth;
                    lvs_dur_ds.Value = leaveProfileDetail.DurationDescription;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> DeleteLeaveProfileDetailAsync(int leaveProfileDetailId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_pfdt WHERE (pfl_dtl_id = @pfl_dtl_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pfl_dtl_id = cmd.Parameters.Add("@pfl_dtl_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pfl_dtl_id.Value = leaveProfileDetailId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> EditLeaveProfileDetailAsync(LeaveProfileDetail leaveProfileDetail)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_pfdt SET lvs_typ_cd=@lvs_typ_cd, ");
            sb.Append("is_yrly=@is_yrly, cancarryover=@cancarryover, ");
            sb.Append("is_mntz=@is_mntz, lvs_dur=@lvs_dur, dur_typ=@dur_typ, ");
            sb.Append("carryover_end_mn=@carryover_end_mn, lvs_dur_ds=@lvs_dur_ds ");
            sb.Append("WHERE (pfl_dtl_id=@pfl_dtl_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pfl_dtl_id = cmd.Parameters.Add("@pfl_dtl_id", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var is_yrly = cmd.Parameters.Add("@is_yrly", NpgsqlDbType.Boolean);
                    var cancarryover = cmd.Parameters.Add("@cancarryover", NpgsqlDbType.Boolean);
                    var is_mntz = cmd.Parameters.Add("@is_mntz", NpgsqlDbType.Boolean);
                    var lvs_dur = cmd.Parameters.Add("@lvs_dur", NpgsqlDbType.Integer);
                    var dur_typ = cmd.Parameters.Add("@dur_typ", NpgsqlDbType.Integer);
                    var carryover_end_mn = cmd.Parameters.Add("@carryover_end_mn", NpgsqlDbType.Integer);
                    var lvs_dur_ds = cmd.Parameters.Add("@lvs_dur_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    pfl_dtl_id.Value = leaveProfileDetail.Id;
                    lvs_typ_cd.Value = leaveProfileDetail.LeaveTypeCode;
                    is_yrly.Value = leaveProfileDetail.IsYearly;
                    cancarryover.Value = leaveProfileDetail.CanBeCarriedOver;
                    is_mntz.Value = leaveProfileDetail.CanBeMonetized;
                    lvs_dur.Value = leaveProfileDetail.Duration;
                    dur_typ.Value = leaveProfileDetail.DurationTypeId;
                    carryover_end_mn.Value = leaveProfileDetail.CarryOverEndMonth;
                    lvs_dur_ds.Value = leaveProfileDetail.DurationDescription;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        #endregion

        #region Profile Details Utility Action Methods
        public async Task<LeaveDuration> GetLeaveDurationByProfileIdnLeaveTypeAsync(int profileId, string leaveTypeCode)
        {
            LeaveDuration leaveDuration = new LeaveDuration();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_dur, act_lvs_dur_typ, lvs_dur_ds ");
            sb.Append("FROM public.lvm_lvs_pfdt ");
            sb.Append("WHERE lvs_pfl_id = @lvs_pfl_id ");
            sb.Append("AND lvs_typ_cd = @lvs_typ_cd; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlTypes.NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlTypes.NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_pfl_id.Value = profileId;
                    lvs_typ_cd.Value = leaveTypeCode;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveDuration.Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"];
                        leaveDuration.DurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"];
                        leaveDuration.DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString();
                    }
                }
                await conn.CloseAsync();
            }
            return leaveDuration;
        }
        #endregion

        #endregion
    }
}
