using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Base.Repositories.LeaveRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.LeaveRepositories
{
    public class LeaveRepository:ILeaveRepository
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
            sb.Append("lvs_pfl_ds, lvs_pfl_cd) VALUES (@lvs_pfl_nm, ");
            sb.Append("@lvs_pfl_ds, @lvs_pfl_cd);  ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_nm = cmd.Parameters.Add("@lvs_pfl_nm", NpgsqlDbType.Text);
                    var lvs_pfl_ds = cmd.Parameters.Add("@lvs_pfl_ds", NpgsqlDbType.Text);
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pfl_nm.Value = leaveProfile.Name;
                    lvs_pfl_ds.Value = leaveProfile.Description ?? (object)DBNull.Value;
                    lvs_pfl_cd.Value = leaveProfile.Code;
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
            sb.Append("lvs_pfl_ds=@lvs_pfl_ds ");
            sb.Append("WHERE (lvs_pfl_id=@lvs_pfl_id); ");
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
            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds, ");
            sb.Append("lvs_pfl_cd FROM public.lvm_lvs_pfls ");
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
                            leaveProfile.Code = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString();
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
            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds, ");
            sb.Append("lvs_pfl_cd FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE LOWER(lvs_pfl_nm)=LOWER(@lvs_pfl_nm);");
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
                            leaveProfile.Code = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString();
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

            sb.Append("SELECT lvs_pfl_id, lvs_pfl_nm, lvs_pfl_ds, ");
            sb.Append("lvs_pfl_cd FROM public.lvm_lvs_pfls ");
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
                            Description = reader["lvs_pfl_ds"] == DBNull.Value ? string.Empty : reader["lvs_pfl_ds"].ToString(),
                            Code = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString()
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
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
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
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
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
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
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
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByEmployeeIdAsync(string employeeId)
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
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
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
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
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
        public async Task<LeaveProfileDetail> GetLeaveProfileDetailByEmployeeNamenLeaveTypeAsync(string employeeName, string leaveTypeCode)
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
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE (d.lvs_typ_cd = @lvs_typ_cd) ");
            sb.Append("AND d.lvs_pfl_id = (SELECT lvs_pfl_cd FROM public.erm_emp_inf ");
            sb.Append("WHERE emp_id = (SELECT id FROM public.gst_prsns  ");
            sb.Append("WHERE fullname = @emp_nm)); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
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

        //public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByEmployeeIdnLeaveTypeAsync(string employeeId, string leaveTypeCode)
        //{
        //    List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
        //    sb.Append("d.is_yrly, d.cancarryover, d.is_mntz, d.lvs_dur, ");
        //    sb.Append("d.dur_typ, d.carryover_end_mn, d.lvs_dur_ds,  ");
        //    sb.Append("p.lvs_pfl_cd, p.lvs_pfl_nm, ");
        //    sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
        //    sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
        //    sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
        //    sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
        //    sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
        //    sb.Append("CASE WHEN d.carryover_end_mn = 1 THEN 'January' ");
        //    sb.Append("WHEN d.carryover_end_mn = 2 THEN 'February' ");
        //    sb.Append("WHEN d.carryover_end_mn = 3 THEN 'March' ");
        //    sb.Append("WHEN d.carryover_end_mn = 4 THEN 'April' ");
        //    sb.Append("WHEN d.carryover_end_mn = 5 THEN 'May' ");
        //    sb.Append("WHEN d.carryover_end_mn = 6 THEN 'June' ");
        //    sb.Append("WHEN d.carryover_end_mn = 7 THEN 'July' ");
        //    sb.Append("WHEN d.carryover_end_mn = 8 THEN 'August' ");
        //    sb.Append("WHEN d.carryover_end_mn = 9 THEN 'September' ");
        //    sb.Append("WHEN d.carryover_end_mn = 10 THEN 'October' ");
        //    sb.Append("WHEN d.carryover_end_mn = 11 THEN 'November' ");
        //    sb.Append("WHEN d.carryover_end_mn = 12 THEN 'December' ");
        //    sb.Append("END as carryover_end_mn_name, ");
        //    sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
        //    sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
        //    sb.Append("FROM public.lvm_lvs_pfdt d ");
        //    sb.Append("INNER JOIN public.lvm_lvs_pfls p ");
        //    sb.Append("ON p.lvs_pfl_id = d.lvs_pfl_id ");
        //    sb.Append("WHERE (d.lvs_typ_cd = @lvs_typ_cd) ");
        //    sb.Append("AND (p.lvs_pfl_cd = (SELECT lvs_pfl_cd ");
        //    sb.Append("FROM public.erm_emp_inf  ");
        //    sb.Append("WHERE emp_id = @lvs_emp_id); ");
        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
        //            var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
        //            await cmd.PrepareAsync();
        //            lvs_emp_id.Value = employeeId;
        //            lvs_typ_cd.Value = leaveTypeCode;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveProfileDetails.Add(new LeaveProfileDetail()
        //                {
        //                    Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
        //                    ProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
        //                    ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
        //                    ProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
        //                    DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
        //                    DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
        //                    IsYearly = reader["is_yrly"] == DBNull.Value ? false : (bool)reader["is_yrly"],
        //                    CanBeCarriedOver = reader["cancarryover"] == DBNull.Value ? false : (bool)reader["cancarryover"],
        //                    CanBeMonetized = reader["is_mntz"] == DBNull.Value ? false : (bool)reader["is_mntz"],
        //                    CarryOverEndMonth = reader["carryover_end_mn"] == DBNull.Value ? 0 : (int)reader["carryover_end_mn"],
        //                    CarryOverEndMonthName = reader["carryover_end_mn_name"] == DBNull.Value ? string.Empty : reader["carryover_end_mn_name"].ToString(),
        //                    DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString(),
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveProfileDetails;
        //}
        //public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByEmployeeNamenLeaveTypeAsync(string employeeName, string leaveTypeCode)
        //{
        //    List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
        //    sb.Append("d.is_yrly, d.cancarryover, d.is_mntz, d.lvs_dur, ");
        //    sb.Append("d.dur_typ, d.carryover_end_mn, d.lvs_dur_ds,  ");
        //    sb.Append("p.lvs_pfl_cd, p.lvs_pfl_nm, ");
        //    sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
        //    sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
        //    sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
        //    sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
        //    sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
        //    sb.Append("CASE WHEN d.carryover_end_mn = 1 THEN 'January' ");
        //    sb.Append("WHEN d.carryover_end_mn = 2 THEN 'February' ");
        //    sb.Append("WHEN d.carryover_end_mn = 3 THEN 'March' ");
        //    sb.Append("WHEN d.carryover_end_mn = 4 THEN 'April' ");
        //    sb.Append("WHEN d.carryover_end_mn = 5 THEN 'May' ");
        //    sb.Append("WHEN d.carryover_end_mn = 6 THEN 'June' ");
        //    sb.Append("WHEN d.carryover_end_mn = 7 THEN 'July' ");
        //    sb.Append("WHEN d.carryover_end_mn = 8 THEN 'August' ");
        //    sb.Append("WHEN d.carryover_end_mn = 9 THEN 'September' ");
        //    sb.Append("WHEN d.carryover_end_mn = 10 THEN 'October' ");
        //    sb.Append("WHEN d.carryover_end_mn = 11 THEN 'November' ");
        //    sb.Append("WHEN d.carryover_end_mn = 12 THEN 'December' ");
        //    sb.Append("END as carryover_end_mn_name, ");
        //    sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
        //    sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
        //    sb.Append("FROM public.lvm_lvs_pfdt d ");
        //    sb.Append("INNER JOIN public.lvm_lvs_pfls p ");
        //    sb.Append("ON p.lvs_pfl_id = d.lvs_pfl_id ");
        //    sb.Append("WHERE (d.lvs_typ_cd = @lvs_typ_cd) ");
        //    sb.Append("AND (p.lvs_pfl_cd = (SELECT lvs_pfl_cd ");
        //    sb.Append("FROM public.erm_emp_inf WHERE emp_id =  ");
        //    sb.Append("(SELECT id FROM public.gst_prsns  ");
        //    sb.Append("WHERE fullname = @lvs_emp_nm)));");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_emp_nm = cmd.Parameters.Add("@lvs_emp_nm", NpgsqlDbType.Text);
        //            var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
        //            await cmd.PrepareAsync();
        //            lvs_emp_nm.Value = employeeName;
        //            lvs_typ_cd.Value = leaveTypeCode;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveProfileDetails.Add(new LeaveProfileDetail()
        //                {
        //                    Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
        //                    ProfileId = reader["lvs_pfl_id"] == DBNull.Value ? 0 : (int)reader["lvs_pfl_id"],
        //                    ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
        //                    ProfileName = reader["lvs_pfl_nm"] == DBNull.Value ? string.Empty : reader["lvs_pfl_nm"].ToString(),
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    Duration = reader["lvs_dur"] == DBNull.Value ? 0 : (int)reader["lvs_dur"],
        //                    DurationTypeId = reader["dur_typ"] == DBNull.Value ? 0 : (int)reader["dur_typ"],
        //                    DurationTypeDescription = reader["dur_typ_ds"] == DBNull.Value ? string.Empty : reader["dur_typ_ds"].ToString(),
        //                    IsYearly = reader["is_yrly"] == DBNull.Value ? false : (bool)reader["is_yrly"],
        //                    CanBeCarriedOver = reader["cancarryover"] == DBNull.Value ? false : (bool)reader["cancarryover"],
        //                    CanBeMonetized = reader["is_mntz"] == DBNull.Value ? false : (bool)reader["is_mntz"],
        //                    CarryOverEndMonth = reader["carryover_end_mn"] == DBNull.Value ? 0 : (int)reader["carryover_end_mn"],
        //                    CarryOverEndMonthName = reader["carryover_end_mn_name"] == DBNull.Value ? string.Empty : reader["carryover_end_mn_name"].ToString(),
        //                    DurationDescription = reader["lvs_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_dur_ds"].ToString(),
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveProfileDetails;
        //}

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
            sb.Append("@carryover_end_mn, @lvs_dur_ds); ");
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

        #region Leave Plans Action Methods

        #region Leave Plan Write Action Methods
        //===  Leave Write Action Methods =======//
        public async Task<long> AddLeavePlanAsync(LeavePlan e)
        {
            long newLeaveId = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO public.lvm_lvs_plns(emp_id, unit_id, ");
            sb.Append("dept_id, loc_id, lvs_yr, lvs_typ_cd, lvs_rsn, ");
            sb.Append("lvs_pln_sdt, lvs_pln_edt, lvs_pln_dur, pln_dur_ds, ");
            sb.Append("pln_rsmptn_dt, pln_dur_typ, lvs_pln_sts) ");
            sb.Append("VALUES (@emp_id, @unit_id, @dept_id, @loc_id, ");
            sb.Append("@lvs_yr, @lvs_typ_cd, @lvs_rsn, @lvs_pln_sdt, ");
            sb.Append("@lvs_pln_edt, @lvs_pln_dur, @pln_dur_ds, "); 
            sb.Append("@pln_rsmptn_dt, @pln_dur_typ, @lvs_pln_sts) ");
            sb.Append(" RETURNING lvs_pln_id;  ");
       
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("emp_id", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);

                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);
                    
                    var lvs_pln_sdt = cmd.Parameters.Add("@lvs_pln_sdt", NpgsqlDbType.Timestamp);
                    var lvs_pln_edt = cmd.Parameters.Add("@lvs_pln_edt", NpgsqlDbType.Timestamp);
                    var lvs_pln_dur = cmd.Parameters.Add("@lvs_pln_dur", NpgsqlDbType.Integer);
                    var pln_dur_typ = cmd.Parameters.Add("@pln_dur_typ", NpgsqlDbType.Integer);
                    var pln_dur_ds = cmd.Parameters.Add("@pln_dur_ds", NpgsqlDbType.Text);
                    var pln_rsmptn_dt = cmd.Parameters.Add("@pln_rsmptn_dt", NpgsqlDbType.Timestamp);
                    var lvs_pln_sts = cmd.Parameters.Add("@lvs_pln_sts", NpgsqlDbType.Integer);

                    cmd.Prepare();

                    emp_id.Value = e.LeaveEmployeeId;
                    unit_id.Value = e.LeaveUnitId;
                    dept_id.Value = e.LeaveDepartmentId;
                    loc_id.Value = e.LeaveLocationId;

                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason ?? (object)DBNull.Value;

                    lvs_pln_sdt.Value = e.LeavePlanStartDate ?? (object)DBNull.Value;
                    lvs_pln_edt.Value = e.LeavePlanEndDate ?? (object)DBNull.Value;
                    lvs_pln_dur.Value = e.LeavePlanDuration;
                    pln_dur_typ.Value = e.LeavePlanDurationTypeId;
                    pln_dur_ds.Value = e.LeavePlanDurationDescription ?? (object)DBNull.Value;
                    pln_rsmptn_dt.Value = e.LeavePlanResumptionDate ?? (object)DBNull.Value;
                    lvs_pln_sts.Value = e.LeavePlanStatusId;

                    var obj = await cmd.ExecuteScalarAsync();
                    newLeaveId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return newLeaveId;
        }
        public async Task<bool> DeleteLeavePlanAsync(long leavePlanId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            //sb.Append("DELETE FROM public.lms_lvs_aprvs WHERE (lvs_inf_id = @lvs_inf_id); ");
            //sb.Append("DELETE FROM public.lms_lvs_docs WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lvm_lvs_logs WHERE (lvs_pln_id = @lvs_pln_id); ");
            sb.Append("DELETE FROM public.lvm_lvs_msgs WHERE (lvs_pln_id = @lvs_pln_id); ");
            //sb.Append("DELETE FROM public.lms_lvs_sbms WHERE (lvs_inf_id = @lvs_inf_id); ");
            //sb.Append("DELETE FROM public.lms_lvs_trnx WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lvm_lvs_plns WHERE (lvs_pln_id = @lvs_pln_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_pln_id.Value = leavePlanId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> EditLeavePlanAsync(LeavePlan e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.lvm_lvs_plns SET lvs_yr=@lvs_yr, ");
            sb.Append("lvs_typ_cd=@lvs_typ_cd, lvs_rsn=@lvs_rsn, ");
            sb.Append("lvs_pln_sdt=@lvs_pln_sdt, lvs_pln_edt=@lvs_pln_edt, ");
            sb.Append("lvs_pln_dur=@lvs_pln_dur, pln_dur_ds=@pln_dur_ds, ");
            sb.Append("pln_rsmptn_dt=@pln_rsmptn_dt, pln_dur_typ=@pln_dur_typ ");
            sb.Append("WHERE (lvs_pln_id=@lvs_pln_id); ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);

                    var lvs_pln_sdt = cmd.Parameters.Add("@lvs_pln_sdt", NpgsqlDbType.Timestamp);
                    var lvs_pln_edt = cmd.Parameters.Add("@lvs_pln_edt", NpgsqlDbType.Timestamp);
                    var lvs_pln_dur = cmd.Parameters.Add("@lvs_pln_dur", NpgsqlDbType.Integer);
                    var pln_dur_typ = cmd.Parameters.Add("@pln_dur_typ", NpgsqlDbType.Integer);
                    var pln_dur_ds = cmd.Parameters.Add("@pln_dur_ds", NpgsqlDbType.Text);
                    var pln_rsmptn_dt = cmd.Parameters.Add("@pln_rsmptn_dt", NpgsqlDbType.Timestamp);

                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);

                    cmd.Prepare();

                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason ?? (object)DBNull.Value;

                    lvs_pln_sdt.Value = e.LeavePlanStartDate ?? (object)DBNull.Value;
                    lvs_pln_edt.Value = e.LeavePlanEndDate ?? (object)DBNull.Value;
                    lvs_pln_dur.Value = e.LeavePlanDuration;
                    pln_dur_typ.Value = e.LeavePlanDurationTypeId;
                    pln_dur_ds.Value = e.LeavePlanDurationDescription ?? (object)DBNull.Value;
                    pln_rsmptn_dt.Value = e.LeavePlanResumptionDate ?? (object)DBNull.Value;

                    lvs_pln_id.Value = e.LeavePlanId;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeavePlanStatusAsync(long leavePlanId, int newStatus)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_plns SET lvs_pln_sts=@lvs_pln_sts ");
            sb.Append("WHERE (lvs_pln_id=@lvs_pln_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    var lvs_pln_sts = cmd.Parameters.Add("@lvs_pln_sts", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    lvs_pln_id.Value = leavePlanId;
                    lvs_pln_sts.Value = newStatus;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        #endregion

        #region Leave Plans Read Action Methods

        #region Leave Plans By LeavePlanId & Employee ID & Name
        public async Task<LeavePlan> GetLeavePlanByIdAsync(long leavePlanId)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_pln_id = @lvs_pln_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_pln_id.Value = leavePlanId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],
                            
                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList[0];
        }
        public async Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId, int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.emp_id = @emp_id AND p.lvs_yr=@lvs_yr) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
                    lvs_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        // By Employee Name
        public async Task<List<LeavePlan>> GetLeavePlansByEmployeeNameAsync(string employeeName)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.emp_id IN (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @emp_nm)) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        public async Task<List<LeavePlan>> GetLeavePlansByEmployeeNameAsync(string employeeName, int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) AND ");
            sb.Append("(p.emp_id IN (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @emp_nm)) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    lvs_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByEmployeeNameAsync(string employeeName, int leaveYear, int leaveMonth)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM p.lvs_pln_sdt) = @sdt_month) AND ");
            sb.Append("(p.emp_id IN (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @emp_nm)) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    lvs_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }


        #endregion

        #region Leave Plans By Location & Unit

        // For Leave Year & Leave Month
        public async Task<List<LeavePlan>> GetLeavePlansByLeaveYearAsync(int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByLeaveYearnLeaveMonthAsync(int leaveYear, int leaveMonth)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM p.lvs_pln_sdt) = @sdt_month) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }


        // For LocationId
        public async Task<List<LeavePlan>> GetLeavePlansByLocationIdAsync(int locationId, int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND (p.loc_id = @loc_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    lvs_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByLocationIdAsync(int locationId, int leaveYear, int leaveMonth)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND (p.loc_id = @loc_id) AND ");
            sb.Append("(EXTRACT(MONTH FROM p.lvs_pln_sdt) = @sdt_month) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    lvs_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        // For UnitId
        public async Task<List<LeavePlan>> GetLeavePlansByUnitIdAsync(int unitId, int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND (p.unit_id = @unit_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    unit_id.Value = unitId;
                    lvs_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByUnitIdAsync(int unitId, int leaveYear, int leaveMonth)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND (p.unit_id = @unit_id) AND ");
            sb.Append("(EXTRACT(MONTH FROM p.lvs_pln_sdt) = @sdt_month) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    unit_id.Value = unitId;
                    lvs_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        // For LocationId & UnitId
        public async Task<List<LeavePlan>> GetLeavePlansByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND (p.loc_id = @loc_id) ");
            sb.Append("AND (p.unit_id = @unit_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    unit_id.Value = unitId;
                    lvs_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear, int leaveMonth)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.lvs_pln_sts, ");
            sb.Append("CASE WHEN p.lvs_pln_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN p.lvs_pln_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN p.lvs_pln_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN p.lvs_pln_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN p.lvs_pln_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN p.lvs_pln_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_pln_sts_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND (p.loc_id = @loc_id) ");
            sb.Append("AND (p.unit_id = @unit_id) AND ");
            sb.Append("(EXTRACT(MONTH FROM p.lvs_pln_sdt) = @sdt_month) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    unit_id.Value = unitId;
                    lvs_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leavePlanList.Add(new LeavePlan()
                        {
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveEmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            LeaveEmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

                            LeaveUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            LeaveUnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
                            LeaveDepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            LeaveDepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
                            LeaveLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LeaveLocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDuration = reader["lvs_pln_dur"] == DBNull.Value ? 0 : (int)reader["lvs_pln_dur"],
                            LeavePlanDurationTypeId = reader["pln_dur_typ"] == DBNull.Value ? 0 : (int)reader["pln_dur_typ"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["pln_rsmptn_dt"],

                            LeavePlanStatusId = reader["lvs_pln_sts"] == DBNull.Value ? 0 : (int)reader["lvs_pln_sts"],
                            LeavePlanStatusDescription = reader["lvs_pln_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_pln_sts_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        #endregion


        #region Leave Plans for Reports & Team Members
        //public async Task<List<EmployeeLeave>> GetByReportingLineIdAsync(string teamLeadId, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.is_pln = @is_pln) ");
        //    sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
        //    sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");
        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            rpt_emp_id.Value = teamLeadId;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearAsync(string teamLeadId, int year, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE v.lvs_yr = @lvs_yr AND v.is_pln = @is_pln ");
        //    sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
        //    sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            rpt_emp_id.Value = teamLeadId;
        //            lvs_yr.Value = year;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearnStatusAsync(string teamLeadId, int year, string leaveStatus, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE v.lvs_yr = @lvs_yr AND v.is_pln = @is_pln ");
        //    sb.Append("AND v.lvs_sts = @lvs_sts ");
        //    sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
        //    sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            rpt_emp_id.Value = teamLeadId;
        //            lvs_yr.Value = year;
        //            lvs_sts.Value = leaveStatus;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearnMonthAsync(string teamLeadId, int year, int month, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE v.is_pln = @is_pln ");
        //    sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
        //    sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
        //    sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
        //    sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            rpt_emp_id.Value = teamLeadId;
        //            lvs_yr.Value = year;
        //            lvs_month.Value = month;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByReportingLineIdnYearnMonthnStatusAsync(string teamLeadId, int year, int month, string leaveStatus, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE v.is_pln = @is_pln AND v.lvs_sts = @lvs_sts ");
        //    sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
        //    sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
        //    sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
        //    sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
        //            var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            rpt_emp_id.Value = teamLeadId;
        //            lvs_yr.Value = year;
        //            lvs_month.Value = month;
        //            lvs_sts.Value = leaveStatus;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByReportingLineIdnStatusAsync(string teamLeadId, string leaveStatus, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE v.is_pln = @is_pln AND v.lvs_sts = @lvs_sts ");
        //    sb.Append("AND v.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
        //    sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");
        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
        //            var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            rpt_emp_id.Value = teamLeadId;
        //            lvs_sts.Value = leaveStatus;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        #endregion

        #region All Employee Leaves By Location, Department, Unit
        //=== All Employee Leaves By Location, Department, Unit etc ========//
        //public async Task<EmployeeLeave> GetByIdAsync(long id)
        //{
        //    EmployeeLeave e = new EmployeeLeave();
        //    string query = string.Empty;
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.lvs_inf_id = @lvs_inf_id); ");

        //    query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_inf_id = cmd.Parameters.Add("@lvs_inf_id", NpgsqlDbType.Bigint);
        //            await cmd.PrepareAsync();
        //            lvs_inf_id.Value = id;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                e.Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"];
        //                e.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
        //                e.EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();

        //                e.UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"];
        //                e.UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString();
        //                e.DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"];
        //                e.DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString();
        //                e.LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
        //                e.LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString();

        //                e.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
        //                e.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
        //                e.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
        //                e.LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString();
        //                e.LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString();
        //                e.IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"];

        //                e.ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"];
        //                e.ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"];
        //                e.ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"];
        //                e.ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"];
        //                e.ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString();

        //                e.ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"];
        //                e.ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"];
        //                e.ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"];
        //                e.ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"];
        //                e.ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString();

        //                e.ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"];
        //                e.ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"];
        //                e.ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"];
        //                e.ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"];
        //                e.ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString();

        //                e.RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"];

        //                e.LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"];
        //                e.LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"];
        //                e.LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString();

        //                e.HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"];
        //                e.HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"];
        //                e.HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString();

        //                e.ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"];
        //                e.ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"];
        //                e.ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"];
        //                e.ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"];
        //                e.ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"];
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return e;
        //}
        //public async Task<List<EmployeeLeave>> GetAllAsync(bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.is_pln = @is_pln) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByYearAsync(int year, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.lvs_yr = @lvs_yr) AND (v.is_pln = @is_pln) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            lvs_yr.Value = year;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByYearnStatusAsync(int year, string leaveStatus, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.lvs_yr = @lvs_yr) AND (v.is_pln = @is_pln) ");
        //    sb.Append("AND v.lvs_sts = @lvs_sts ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            lvs_yr.Value = year;
        //            lvs_sts.Value = leaveStatus;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByYearnMonthAsync(int year, int month, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.is_pln = @is_pln) ");
        //    sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
        //    sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            lvs_yr.Value = year;
        //            lvs_month.Value = month;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByYearnMonthnStatusAsync(int year, int month, string leaveStatus, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_sts = @lvs_sts) ");
        //    sb.Append("AND DATE_PART('Year', v.prp_lvs_sdt) = @lvs_yr ");
        //    sb.Append("AND DATE_PART('Month', v.prp_lvs_sdt) = @lvs_month ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var lvs_month = cmd.Parameters.Add("@lvs_month", NpgsqlDbType.Integer);
        //            var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            lvs_yr.Value = year;
        //            lvs_month.Value = month;
        //            lvs_sts.Value = leaveStatus;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        //public async Task<List<EmployeeLeave>> GetByStatusAsync(string leaveStatus, bool isPlan)
        //{
        //    List<EmployeeLeave> leaveList = new List<EmployeeLeave>();
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("SELECT v.lvs_inf_id, v.emp_id, v.unit_id, v.dept_id, ");
        //    sb.Append("v.loc_id, v.lvs_yr, v.lvs_typ_cd, v.lvs_rsn, v.lvs_sts, ");
        //    sb.Append("v.is_pln, v.prp_lvs_sdt, v.prp_lvs_edt, v.prp_lvs_dur, ");
        //    sb.Append("v.act_lvs_dur, v.prp_dur_ds, v.apv_lvs_sdt, v.apv_lvs_edt,  ");
        //    sb.Append("v.apv_lvs_dur, v.apv_dur_ds, v.act_lvs_sdt, v.act_lvs_edt, ");
        //    sb.Append("v.act_dur_ds, v.lm_rsmptn_dt, v.lm_confm_dt, v.lm_confm_by, ");
        //    sb.Append("v.hr_rsmptn_dt, v.hr_confm_dt, v.hr_confm_by, v.rqs_cls_dt, ");
        //    sb.Append("v.is_lm_aprv, v.is_hd_aprv, v.is_hr_aprv, v.is_xm_aprv, ");
        //    sb.Append("v.is_sm_aprv, v.prp_lvs_dur_typ, v.apv_lvs_dur_typ, v.act_lvs_dur_typ,  ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = v.emp_id) ");
        //    sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lms_lvs_typs ");
        //    sb.Append("WHERE lvs_typ_cd = v.lvs_typ_cd) as lvs_typ_nm, ");
        //    sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = v.unit_id) ");
        //    sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
        //    sb.Append("= v.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
        //    sb.Append("WHERE locqk = v.loc_id) as loc_nm ");
        //    sb.Append("FROM public.lms_lvs_infs v ");
        //    sb.Append("WHERE (v.is_pln = @is_pln) AND (v.lvs_sts = @lvs_sts) ");
        //    sb.Append("ORDER BY v.prp_lvs_sdt; ");

        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
        //            var is_pln = cmd.Parameters.Add("@is_pln", NpgsqlDbType.Boolean);
        //            await cmd.PrepareAsync();
        //            lvs_sts.Value = leaveStatus;
        //            is_pln.Value = isPlan;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveList.Add(new EmployeeLeave()
        //                {
        //                    Id = reader["lvs_inf_id"] == DBNull.Value ? 0L : (long)reader["lvs_inf_id"],
        //                    EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
        //                    EmployeeFullName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),

        //                    UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
        //                    UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),
        //                    DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
        //                    DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),
        //                    LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
        //                    LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

        //                    LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
        //                    LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
        //                    LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
        //                    LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),
        //                    LeaveStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
        //                    IsPlan = reader["is_pln"] == DBNull.Value ? true : (bool)reader["is_pln"],

        //                    ProposedLeaveStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
        //                    ProposedLeaveEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
        //                    ProposedLeaveDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
        //                    ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
        //                    ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),

        //                    ApprovedLeaveStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
        //                    ApprovedLeaveEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
        //                    ApprovedLeaveDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
        //                    ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
        //                    ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),

        //                    ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
        //                    ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
        //                    ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
        //                    ActualDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
        //                    ActualDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

        //                    RequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rqs_cls_dt"],

        //                    LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_rsmptn_dt"],
        //                    LineManagerConfirmResumptionDate = reader["lm_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lm_confm_dt"],
        //                    LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),

        //                    HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_rsmptn_dt"],
        //                    HrConfirmResumptionDate = reader["hr_confm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["hr_confm_dt"],
        //                    HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),

        //                    ApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
        //                    ApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
        //                    ApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
        //                    ApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
        //                    ApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveList;
        //}
        #endregion

        #region Employee Leave Days 
        //public async Task<LeaveDuration> GetUsedLeaveDurationByLeaveYearnEmployeeIdnLeaveTypeAsync(int leaveYear, string employeeId, string leaveTypeCode)
        //{
        //    LeaveDuration leaveDuration = new LeaveDuration();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT act_lvs_dur_typ, act_dur_ds, SUM(act_lvs_dur) as total_duration ");
        //    sb.Append("FROM public.lms_lvs_infs WHERE emp_id = @emp_id ");
        //    sb.Append("AND lvs_typ_cd = @lvs_typ_cd AND lvs_yr = @lvs_yr ");
        //    sb.Append("GROUP BY act_lvs_dur_typ, act_dur_ds; ");
        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
        //            var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
        //            var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
        //            await cmd.PrepareAsync();
        //            lvs_yr.Value = leaveYear;
        //            emp_id.Value = employeeId;
        //            lvs_typ_cd.Value = leaveTypeCode;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                leaveDuration.Duration = reader["total_duration"] == DBNull.Value ? 0 : (int)reader["total_duration"];
        //                leaveDuration.DurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"];
        //                leaveDuration.DurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString();
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return leaveDuration;
        //}

        #endregion

        #endregion

        #region Leave Submission Action Methods
        //==== Leave Plan Submission Read Action Methods
        public async Task<List<LeaveSubmission>> GetLeaveSubmissionsByLeaveSubmissionIdAsync(long leaveSubmissionId)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_sbm_id, lvs_pln_id, lvs_rqst_id, frm_emp_nm,  ");
            sb.Append("to_emp_nm, sbm_purps, sbm_dt, sbm_msg, is_xtn, dt_xtn, ");
            sb.Append("to_emp_rl  FROM public.lvm_lvs_sbms ");
            sb.Append("WHERE (lvs_sbm_id=@lvs_sbm_id) ");
            sb.Append("ORDER BY lvs_sbm_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_sbm_id = cmd.Parameters.Add("@lvs_sbm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_sbm_id.Value = leaveSubmissionId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            LeaveSubmissionId = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0 : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0 : (long)reader["lvs_rqst_id"],
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }

        public async Task<List<LeaveSubmission>> GetLeaveSubmissionsByToEmployeeNameAsync(string toEmployeeName)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_sbm_id, lvs_pln_id, lvs_rqst_id, frm_emp_nm,  ");
            sb.Append("to_emp_nm, sbm_purps, sbm_dt, sbm_msg, is_xtn, dt_xtn, ");
            sb.Append("to_emp_rl  FROM public.lvm_lvs_sbms ");
            sb.Append("WHERE (to_emp_nm=@to_emp_nm) ");
            sb.Append("ORDER BY lvs_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_nm = cmd.Parameters.Add("@to_emp_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    to_emp_nm.Value = toEmployeeName;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            LeaveSubmissionId = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0 : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0 : (long)reader["lvs_rqst_id"],
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }
        public async Task<List<LeaveSubmission>> GetLeaveSubmissionsByYearSubmittedAsync(string toEmployeeName, int yearSubmitted)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_sbm_id, lvs_pln_id, lvs_rqst_id, frm_emp_nm,  ");
            sb.Append("to_emp_nm, sbm_purps, sbm_dt, sbm_msg, is_xtn, dt_xtn, ");
            sb.Append("to_emp_rl  FROM public.lvm_lvs_sbms ");
            sb.Append("WHERE (to_emp_nm=@to_emp_nm) ");
            sb.Append("AND (DATE_PART('Year', sbm_dt) = @yr) ");
            sb.Append("ORDER BY lvs_sbm_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_nm = cmd.Parameters.Add("@to_emp_nm", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    to_emp_nm.Value = toEmployeeName;
                    yr.Value = yearSubmitted;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        submissionList.Add(new LeaveSubmission()
                        {
                            LeaveSubmissionId = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0 : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0 : (long)reader["lvs_rqst_id"],
                            ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                            Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
                            Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                            IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                            TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }

        //public async Task<List<LeaveSubmission>> GetSubmissionsByYearnMonthSubmittedAsync(int yearSubmitted, int monthSubmitted)
        //{
        //    List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
        //    sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
        //    sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
        //    sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
        //    sb.Append("FROM public.lms_lvs_sbms s ");
        //    sb.Append("WHERE DATE_PART('Year', s.sbm_dt) = @yr ");
        //    sb.Append("AND DATE_PART('Month', s.sbm_dt) = @mn ");
        //    sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
        //            var mn = cmd.Parameters.Add("@mn", NpgsqlDbType.Integer);
        //            await cmd.PrepareAsync();
        //            yr.Value = yearSubmitted;
        //            mn.Value = monthSubmitted;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                submissionList.Add(new LeaveSubmission()
        //                {
        //                    Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
        //                    LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
        //                    ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
        //                    ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
        //                    FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
        //                    FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
        //                    TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
        //                    Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
        //                    Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
        //                    IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
        //                    TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
        //                    ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return submissionList;
        //}
        //public async Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdnYearSubmittedAsync(string toEmployeeId, int yearSubmitted)
        //{
        //    List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
        //    sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp-rl, ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
        //    sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
        //    sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
        //    sb.Append("FROM public.lms_lvs_sbms s ");
        //    sb.Append("WHERE s.to_emp_id = @to_emp_id ");
        //    sb.Append("AND DATE_PART('Year', s.sbm_dt) = @yr ");
        //    sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
        //            var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
        //            await cmd.PrepareAsync();
        //            to_emp_id.Value = toEmployeeId;
        //            yr.Value = yearSubmitted;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                submissionList.Add(new LeaveSubmission()
        //                {
        //                    Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
        //                    LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
        //                    ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
        //                    ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
        //                    FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
        //                    FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
        //                    TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
        //                    Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
        //                    Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
        //                    IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
        //                    TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
        //                    ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return submissionList;
        //}
        //public async Task<List<LeaveSubmission>> GetSubmissionsByToEmployeeIdnYearnMonthSubmittedAsync(string toEmployeeId, int yearSubmitted, int monthSubmitted)
        //{
        //    List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT s.lvs_sbm_id, s.lvs_inf_id, s.frm_emp_id, s.to_emp_id, ");
        //    sb.Append("s.sbm_purps, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns p1 ");
        //    sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
        //    sb.Append("(SELECT fullname FROM public.gst_prsns p2 ");
        //    sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm ");
        //    sb.Append("FROM public.lms_lvs_sbms s ");
        //    sb.Append("WHERE s.to_emp_id = @to_emp_id ");
        //    sb.Append("AND DATE_PART('Year', s.sbm_dt) = @yr ");
        //    sb.Append("AND DATE_PART('Month', s.sbm_dt) = @mn ");
        //    sb.Append("ORDER BY s.lvs_sbm_id DESC; ");
        //    string query = sb.ToString();
        //    using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
        //            var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
        //            var mn = cmd.Parameters.Add("@mn", NpgsqlDbType.Integer);
        //            await cmd.PrepareAsync();
        //            to_emp_id.Value = toEmployeeId;
        //            yr.Value = yearSubmitted;
        //            mn.Value = monthSubmitted;
        //            var reader = await cmd.ExecuteReaderAsync();
        //            while (await reader.ReadAsync())
        //            {
        //                submissionList.Add(new LeaveSubmission()
        //                {
        //                    Id = reader["lvs_sbm_id"] == DBNull.Value ? 0 : (long)reader["lvs_sbm_id"],
        //                    LeaveId = reader["lvs_inf_id"] == DBNull.Value ? 0 : (long)reader["lvs_inf_id"],
        //                    ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
        //                    ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
        //                    FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
        //                    FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
        //                    TimeSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
        //                    Purpose = reader["sbm_purps"] == DBNull.Value ? string.Empty : reader["sbm_purps"].ToString(),
        //                    Message = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
        //                    IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
        //                    TimeActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
        //                    ToEmployeeRole = reader["to_emp_rl"] == DBNull.Value ? string.Empty : reader["to_emp_rl"].ToString(),
        //                });
        //            }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    return submissionList;
        //}

        //==== Leave Submission Write Action Methods
        public async Task<bool> AddLeaveSubmissionAsync(LeaveSubmission e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_sbms(lvs_pln_id, lvs_rqst_id, ");
            sb.Append("frm_emp_nm, to_emp_nm, sbm_purps, sbm_dt, sbm_msg, is_xtn, ");
            sb.Append("dt_xtn, to_emp_rl) VALUES (@lvs_pln_id, @lvs_rqst_id, ");
            sb.Append("@frm_emp_nm, @to_emp_nm, @sbm_purps, @sbm_dt, @sbm_msg, ");
            sb.Append("@is_xtn, @dt_xtn, @to_emp_rl); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                    var to_emp_nm = cmd.Parameters.Add("@to_emp_nm", NpgsqlDbType.Text);
                    var sbm_purps = cmd.Parameters.Add("@sbm_purps", NpgsqlDbType.Text);
                    var sbm_dt = cmd.Parameters.Add("@sbm_dt", NpgsqlDbType.TimestampTz);
                    var sbm_msg = cmd.Parameters.Add("@sbm_msg", NpgsqlDbType.Text);
                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);
                    var to_emp_rl = cmd.Parameters.Add("@to_emp_rl", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pln_id.Value = e.LeavePlanId ?? (object)DBNull.Value;
                    lvs_rqst_id.Value = e.LeaveRequestId ?? (object)DBNull.Value;
                    frm_emp_nm.Value = e.FromEmployeeName;
                    to_emp_nm.Value = e.ToEmployeeName;
                    sbm_purps.Value = e.Purpose;
                    sbm_dt.Value = e.TimeSubmitted;
                    sbm_msg.Value = e.Message ?? (object)DBNull.Value;
                    is_xtn.Value = e.IsActioned;
                    dt_xtn.Value = e.TimeActioned ?? (object)DBNull.Value;
                    to_emp_rl.Value = e.ToEmployeeRole;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> DeleteSubmissionAsync(long id)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_sbms WHERE (lvs_sbm_id = @lvs_sbm_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_sbm_id = cmd.Parameters.Add("@lvs_sbm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_sbm_id.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateSubmissionActionStatusAsync(long leaveSubmissionId, DateTime? timeActioned)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_sbms SET is_xtn=true, dt_xtn=@dt_xtn ");
            sb.Append("WHERE lvs_sbm_id = @lvs_sbm_id; ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_sbm_id = cmd.Parameters.Add("@lvs_sbm_id", NpgsqlDbType.Bigint);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    lvs_sbm_id.Value = leaveSubmissionId;
                    dt_xtn.Value = timeActioned;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        #endregion

        #region Leave Approval Action Methods
        public async Task<long> AddLeaveApprovalAsync(LeaveApproval e)
        {
            long _newLeaveApprovalId = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_aprvs(lvs_pln_id, ");
            sb.Append("lvs_rqst_id, aprv_emp_nm, lvs_emp_nm, is_aprvd, ");
            sb.Append("lvs_aprv_dt, lvs_aprv_rmk, lvs_aprv_as) VALUES (");
            sb.Append("@lvs_pln_id, @lvs_rqst_id, @aprv_emp_nm, @lvs_emp_nm, ");
            sb.Append("@is_aprvd, @lvs_aprv_dt, @lvs_aprv_rmk, @lvs_aprv_as) ");
            sb.Append("RETURNING lvs_aprv_id;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var aprv_emp_nm = cmd.Parameters.Add("@aprv_emp_nm", NpgsqlDbType.Text);
                    var lvs_emp_nm = cmd.Parameters.Add("@lvs_emp_nm", NpgsqlDbType.Text);
                    var is_aprvd = cmd.Parameters.Add("@is_aprvd", NpgsqlDbType.Boolean);
                    var lvs_aprv_dt = cmd.Parameters.Add("@lvs_aprv_dt", NpgsqlDbType.TimestampTz);
                    var lvs_aprv_rmk = cmd.Parameters.Add("@lvs_aprv_rmk", NpgsqlDbType.Text);
                    var lvs_aprv_as = cmd.Parameters.Add("@lvs_aprv_as", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pln_id.Value = e.LeavePlanId ?? (object)DBNull.Value;
                    lvs_rqst_id.Value = e.LeaveRequestId ?? (object)DBNull.Value;
                    aprv_emp_nm.Value = e.ApproverName;
                    lvs_emp_nm.Value = e.ApplicantName;
                    is_aprvd.Value = e.IsApproved;
                    lvs_aprv_dt.Value = e.TimeApproved ?? DateTime.Now;
                    lvs_aprv_rmk.Value = e.ApproverComments ?? string.Empty;
                    lvs_aprv_as.Value = e.ApproverRole;

                    var obj = await cmd.ExecuteScalarAsync();
                    _newLeaveApprovalId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return _newLeaveApprovalId;
        }
        public async Task<bool> DeleteApprovalAsync(long leaveApprovalId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_aprvs WHERE (lvs_aprv_id = @lvs_aprv_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_aprv_id = cmd.Parameters.Add("@lvs_aprv_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_aprv_id.Value = leaveApprovalId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<List<LeaveApproval>> GetLeaveApprovalsByLeavePlanIdAsync(long leavePlanId)
        {
            List<LeaveApproval> approvalsList = new List<LeaveApproval>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_aprv_id, lvs_pln_id, lvs_rqst_id, aprv_emp_nm, ");
            sb.Append("lvs_emp_nm, is_aprvd, lvs_aprv_dt, lvs_aprv_rmk, lvs_aprv_as ");
            sb.Append("FROM public.lvm_lvs_aprvs WHERE (lvs_pln_id = @lvs_pln_id) ");
            sb.Append("ORDER BY lvs_aprv_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_pln_id.Value = leavePlanId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        approvalsList.Add(new LeaveApproval()
                        {
                            LeaveApprovalId = reader["lvs_aprv_id"] == DBNull.Value ? 0L : (long)reader["lvs_aprv_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                            ApplicantName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                            TimeApproved = reader["lvs_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lvs_aprv_dt"],
                            ApproverComments = reader["lvs_aprv_rmk"] == DBNull.Value ? string.Empty : reader["lvs_aprv_rmk"].ToString(),
                            ApproverRole = reader["lvs_aprv_as"] == DBNull.Value ? string.Empty : reader["lvs_aprv_as"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return approvalsList;
        }
        public async Task<List<LeaveApproval>> GetLeaveApprovalsByLeaveRequestIdAsync(long leaveRequestId)
        {
            List<LeaveApproval> approvalsList = new List<LeaveApproval>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_aprv_id, lvs_pln_id, lvs_rqst_id, aprv_emp_nm, ");
            sb.Append("lvs_emp_nm, is_aprvd, lvs_aprv_dt, lvs_aprv_rmk, lvs_aprv_as ");
            sb.Append("FROM public.lvm_lvs_aprvs WHERE (lvs_rqst_id = @lvs_rqst_id) ");
            sb.Append("ORDER BY lvs_aprv_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_rqst_id.Value = leaveRequestId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        approvalsList.Add(new LeaveApproval()
                        {
                            LeaveApprovalId = reader["lvs_aprv_id"] == DBNull.Value ? 0L : (long)reader["lvs_aprv_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                            ApplicantName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                            TimeApproved = reader["lvs_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lvs_aprv_dt"],
                            ApproverComments = reader["lvs_aprv_rmk"] == DBNull.Value ? string.Empty : reader["lvs_aprv_rmk"].ToString(),
                            ApproverRole = reader["lvs_aprv_as"] == DBNull.Value ? string.Empty : reader["lvs_aprv_as"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return approvalsList;
        }
        public async Task<LeaveApproval> GetApprovalByIdAsync(long leaveApprovalId)
        {
            List<LeaveApproval> approvalsList = new List<LeaveApproval>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_aprv_id, lvs_pln_id, lvs_rqst_id, aprv_emp_nm, ");
            sb.Append("lvs_emp_nm, is_aprvd, lvs_aprv_dt, lvs_aprv_rmk, lvs_aprv_as ");
            sb.Append("FROM public.lvm_lvs_aprvs WHERE (lvs_aprv_id = @lvs_aprv_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_aprv_id = cmd.Parameters.Add("@lvs_aprv_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_aprv_id.Value = leaveApprovalId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        approvalsList.Add(new LeaveApproval()
                        {
                            LeaveApprovalId = reader["lvs_aprv_id"] == DBNull.Value ? 0L : (long)reader["lvs_aprv_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            ApproverName = reader["aprv_emp_nm"] == DBNull.Value ? string.Empty : reader["aprv_emp_nm"].ToString(),
                            ApplicantName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            IsApproved = reader["is_aprvd"] == DBNull.Value ? false : (bool)reader["is_aprvd"],
                            TimeApproved = reader["lvs_aprv_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lvs_aprv_dt"],
                            ApproverComments = reader["lvs_aprv_rmk"] == DBNull.Value ? string.Empty : reader["lvs_aprv_rmk"].ToString(),
                            ApproverRole = reader["lvs_aprv_as"] == DBNull.Value ? string.Empty : reader["lvs_aprv_as"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return approvalsList[0];
        }

        #endregion
        #endregion

        #region Leave Requests Action Methods

        #region Leave Request Write Action Methods
        //===  Leave Write Action Methods =======//
        public async Task<long> AddLeaveRequestAsync(LeaveRequest r)
        {
            long newLeaveId = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO public.lvm_lvs_rqsts(lvs_emp_id, lvs_unit_id, ");
            sb.Append("lvs_dept_id, lvs_loc_id, lvs_rqst_yr, lvs_typ_cd, lvs_rsn, ");
            sb.Append("lvs_rqst_sts, lvs_rqst_sdt, lvs_rqst_edt, lvs_rqst_dur, ");
            sb.Append("lvs_rqst_dur_ds, act_lvs_sdt, act_lvs_edt, act_lvs_dur, ");
            sb.Append("act_dur_ds, lm_rsmptn_dt, lm_confm_dt, lm_confm_by, ");
            sb.Append("hr_rsmptn_dt, hr_confm_dt, hr_confm_by, rqs_cls_dt, ");
            sb.Append("is_lm_aprv, is_hd_aprv, is_hr_aprv, is_xm_aprv, ");
            sb.Append("is_sm_aprv, lvs_rqst_dur_typ, act_lvs_dur_typ, ");
            sb.Append("rqst_rsmptn_dt) VALUES (@lvs_emp_id, @lvs_unit_id, ");
            sb.Append("@lvs_dept_id, @lvs_loc_id, @lvs_rqst_yr, @lvs_typ_cd, ");
            sb.Append("@lvs_rsn, @lvs_rqst_sts, @lvs_rqst_sdt, ");
            sb.Append("@lvs_rqst_edt, @lvs_rqst_dur, @lvs_rqst_dur_ds, ");
            sb.Append("@act_lvs_sdt, @act_lvs_edt, @act_lvs_dur, @act_dur_ds, ");
            sb.Append("@lm_rsmptn_dt, @lm_confm_dt, @lm_confm_by, @hr_rsmptn_dt,  ");
            sb.Append("@hr_confm_dt, @hr_confm_by, @rqs_cls_dt, @is_lm_aprv, ");
            sb.Append("@is_hd_aprv, @is_hr_aprv, @is_xm_aprv, @is_sm_aprv,   ");
            sb.Append("@lvs_rqst_dur_typ, @act_lvs_dur_typ, @rqst_rsmptn_dt) ");
            sb.Append("RETURNING lvs_rqst_id; ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_dept_id = cmd.Parameters.Add("@lvs_dept_id", NpgsqlDbType.Integer);
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);

                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);

                    var lvs_rqst_sdt = cmd.Parameters.Add("@lvs_rqst_sdt", NpgsqlDbType.Timestamp);
                    var lvs_rqst_edt = cmd.Parameters.Add("@lvs_rqst_edt", NpgsqlDbType.Timestamp);
                    var lvs_rqst_dur = cmd.Parameters.Add("@lvs_rqst_dur", NpgsqlDbType.Integer);
                    var lvs_rqst_dur_ds = cmd.Parameters.Add("@lvs_rqst_dur_ds", NpgsqlDbType.Text);

                    var act_lvs_sdt = cmd.Parameters.Add("@act_lvs_sdt", NpgsqlDbType.Timestamp);
                    var act_lvs_edt = cmd.Parameters.Add("@act_lvs_edt", NpgsqlDbType.Timestamp);
                    var act_lvs_dur = cmd.Parameters.Add("@act_lvs_dur", NpgsqlDbType.Integer);
                    var act_dur_ds = cmd.Parameters.Add("@act_dur_ds", NpgsqlDbType.Text);

                    var lm_rsmptn_dt = cmd.Parameters.Add("@lm_rsmptn_dt", NpgsqlDbType.Timestamp);
                    var lm_confm_dt = cmd.Parameters.Add("@lm_confm_dt", NpgsqlDbType.Timestamp);
                    var lm_confm_by = cmd.Parameters.Add("@lm_confm_by", NpgsqlDbType.Text);

                    var hr_rsmptn_dt = cmd.Parameters.Add("@hr_rsmptn_dt", NpgsqlDbType.Timestamp);
                    var hr_confm_dt = cmd.Parameters.Add("@hr_confm_dt", NpgsqlDbType.Timestamp);
                    var hr_confm_by = cmd.Parameters.Add("@hr_confm_by", NpgsqlDbType.Text);

                    var rqs_cls_dt = cmd.Parameters.Add("@rqs_cls_dt", NpgsqlDbType.Timestamp);
                    
                    var is_lm_aprv = cmd.Parameters.Add("@is_lm_aprv", NpgsqlDbType.Boolean);
                    var is_hd_aprv = cmd.Parameters.Add("@is_hd_aprv", NpgsqlDbType.Boolean);
                    var is_hr_aprv = cmd.Parameters.Add("@is_hr_aprv", NpgsqlDbType.Boolean);
                    var is_xm_aprv = cmd.Parameters.Add("@is_xm_aprv", NpgsqlDbType.Boolean);
                    var is_sm_aprv = cmd.Parameters.Add("@is_sm_aprv", NpgsqlDbType.Boolean);

                    var lvs_rqst_dur_typ = cmd.Parameters.Add("@lvs_rqst_dur_typ", NpgsqlDbType.Integer);
                    var act_lvs_dur_typ = cmd.Parameters.Add("@act_lvs_dur_typ", NpgsqlDbType.Integer);
                    var rqst_rsmptn_dt = cmd.Parameters.Add("@rqst_rsmptn_dt", NpgsqlDbType.Timestamp);

                    cmd.Prepare();

                    lvs_emp_id.Value = r.LeaveEmployeeId;
                    lvs_unit_id.Value = r.UnitId;
                    lvs_dept_id.Value = r.DepartmentId;
                    lvs_loc_id.Value = r.LocationId;

                    lvs_rqst_yr.Value = r.LeaveYear;
                    lvs_typ_cd.Value = r.LeaveTypeCode;
                    lvs_rsn.Value = r.LeaveReason ?? (object)DBNull.Value;
                    lvs_rqst_sts.Value = r.LeaveRequestStatusId;

                    lvs_rqst_sdt.Value = r.RequestedStartDate;
                    lvs_rqst_edt.Value = r.RequestedEndDate;
                    lvs_rqst_dur.Value = r.RequestedDuration;
                    lvs_rqst_dur_ds.Value = r.RequestedDurationDescription;

                    act_lvs_sdt.Value = r.ActualLeaveStartDate ?? (object)DBNull.Value;
                    act_lvs_edt.Value = r.ActualLeaveEndDate ?? (object)DBNull.Value;
                    act_lvs_dur.Value = r.ActualLeaveDuration;
                    act_dur_ds.Value = r.ActualLeaveDurationDescription;

                    lm_rsmptn_dt.Value = r.LineManagersResumptionDate ?? (object)DBNull.Value;
                    lm_confm_dt.Value = r.LineManagerConfirmResumptionTime ?? (object)DBNull.Value;
                    lm_confm_by.Value = r.LineManagerConfirmResumptionBy ?? (object)DBNull.Value;

                    hr_rsmptn_dt.Value = r.HrResumptionDate ?? (object)DBNull.Value;
                    hr_confm_dt.Value = r.HrConfirmResumptionTime ?? (object)DBNull.Value;
                    hr_confm_by.Value = r.HrConfirmResumptionBy ?? (object)DBNull.Value;

                    rqs_cls_dt.Value = r.LeaveRequestCloseDate ?? (object)DBNull.Value;

                    is_lm_aprv.Value = r.IsApprovedByLineManager;
                    is_hd_aprv.Value = r.IsApprovedByHeadOfDepartment;
                    is_hr_aprv.Value = r.IsApprovedByHR;
                    is_sm_aprv.Value = r.IsApprovedByStationManager;
                    is_xm_aprv.Value = r.IsApprovedByExecutiveManagement;

                    lvs_rqst_dur_typ.Value = r.RequestedDurationTypeId;
                    act_lvs_dur_typ.Value = r.ActualLeaveDurationTypeId;
                    rqst_rsmptn_dt.Value = r.RequestedResumptionDate ?? (object)DBNull.Value;
                    
                    var obj = await cmd.ExecuteScalarAsync();
                    newLeaveId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return newLeaveId;
        }
        public async Task<bool> DeleteLeaveRequestAsync(long leaveRequestId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            //sb.Append("DELETE FROM public.lms_lvs_aprvs WHERE (lvs_inf_id = @lvs_inf_id); ");
            //sb.Append("DELETE FROM public.lms_lvs_docs WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lvm_lvs_logs WHERE (lvs_rqs_id = @lvs_rqs_id); ");
            sb.Append("DELETE FROM public.lvm_lvs_msgs WHERE (lvs_rqs_id = @lvs_rqs_id); ");
            //sb.Append("DELETE FROM public.lms_lvs_sbms WHERE (lvs_inf_id = @lvs_inf_id); ");
            //sb.Append("DELETE FROM public.lms_lvs_trnx WHERE (lvs_inf_id = @lvs_inf_id); ");
            sb.Append("DELETE FROM public.lvm_lvs_rqsts WHERE (lvs_rqst_id = @lvs_rqst_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_rqs_id.Value = leaveRequestId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> EditLeaveRequestAsync(LeaveRequest r)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.lvm_lvs_rqsts SET lvs_rqst_yr=@lvs_rqst_yr, ");
            sb.Append("lvs_typ_cd=@lvs_typ_cd, lvs_rsn=@lvs_rsn, ");
            sb.Append("lvs_rqst_sdt=@lvs_rqst_sdt, lvs_rqst_edt=@lvs_rqst_edt, ");
            sb.Append("lvs_rqst_dur=@lvs_rqst_dur, lvs_rqst_dur_ds=@lvs_rqst_dur_ds, ");
            sb.Append("lvs_rqst_dur_typ=@lvs_rqst_dur_typ, rqst_rsmptn_dt=@rqst_rsmptn_dt, ");
            sb.Append("lvs_rqst_sts=@lvs_rqst_sts  ");
            sb.Append("WHERE (lvs_rqst_id = @lvs_rqst_id); ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_rsn = cmd.Parameters.Add("@lvs_rsn", NpgsqlDbType.Text);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);

                    var lvs_rqst_sdt = cmd.Parameters.Add("@lvs_rqst_sdt", NpgsqlDbType.Timestamp);
                    var lvs_rqst_edt = cmd.Parameters.Add("@lvs_rqst_edt", NpgsqlDbType.Timestamp);
                    var lvs_rqst_dur = cmd.Parameters.Add("@lvs_rqst_dur", NpgsqlDbType.Integer);
                    var lvs_rqst_dur_ds = cmd.Parameters.Add("@lvs_rqst_dur_ds", NpgsqlDbType.Text);

                    var lvs_rqst_dur_typ = cmd.Parameters.Add("@lvs_rqst_dur_typ", NpgsqlDbType.Integer);
                    var rqst_rsmptn_dt = cmd.Parameters.Add("@rqst_rsmptn_dt", NpgsqlDbType.Timestamp);

                    cmd.Prepare();

                    lvs_rqst_yr.Value = r.LeaveYear;
                    lvs_typ_cd.Value = r.LeaveTypeCode;
                    lvs_rsn.Value = r.LeaveReason ?? (object)DBNull.Value;
                    lvs_rqst_sts.Value = r.LeaveRequestStatusId;

                    lvs_rqst_sdt.Value = r.RequestedStartDate;
                    lvs_rqst_edt.Value = r.RequestedEndDate;
                    lvs_rqst_dur.Value = r.RequestedDuration;
                    lvs_rqst_dur_ds.Value = r.RequestedDurationDescription;

                    lvs_rqst_dur_typ.Value = r.RequestedDurationTypeId;
                    rqst_rsmptn_dt.Value = r.RequestedResumptionDate ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeaveRequestStatusAsync(long leaveRequestId, int newStatus)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_rqsts SET lvs_rqst_sts=@lvs_rqst_sts ");
            sb.Append("WHERE (lvs_rqst_id=@lvs_rqst_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    lvs_rqst_id.Value = leaveRequestId;
                    lvs_rqst_sts.Value = newStatus;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        #endregion


        #region Leave Requests By LeaveRequestId & Employee Id & Name
        public async Task<LeaveRequest> GetLeaveRequestByIdAsync(long leaveRequestId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, ");
            sb.Append("r.lm_confm_dt, r.lm_confm_by, r.hr_rsmptn_dt, ");
            sb.Append("r.hr_confm_dt, r.hr_confm_by, r.rqs_cls_dt, r.is_lm_aprv, ");
            sb.Append("r.is_hd_aprv, r.is_hr_aprv, r.is_xm_aprv, r.is_sm_aprv, ");
            sb.Append("r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, r.rqst_rsmptn_dt, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_id = @lvs_rqst_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_rqst_id.Value = leaveRequestId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveRequestList.Add(new LeaveRequest()
                        {
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),

                            UnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            UnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),
                            DepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            DepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString(),
                            LocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                            LocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                            LeaveYear = reader["lvs_rqst_yr"] == DBNull.Value ? 2020 : (int)reader["lvs_rqst_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            RequestedStartDate = reader["lvs_rqst_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_sdt"],
                            RequestedEndDate = reader["lvs_rqst_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_edt"],
                            RequestedDuration = reader["lvs_rqst_dur"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur"],
                            RequestedDurationTypeId = reader["lvs_rqst_dur_typ"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur_typ"],
                            RequestedDurationDescription = reader["lvs_rqst_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_dur_ds"].ToString(),
                            RequestedResumptionDate = reader["rqst_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqst_rsmptn_dt"],

                            LeaveRequestStatusId = reader["lvs_rqst_sts"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_sts"],
                            LeaveRequestStatusDescription = reader["lvs_rqst_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_sts_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualLeaveDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualLeaveDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),
                            LineManagerConfirmResumptionTime = reader["lm_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_confm_dt"],

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),
                            HrConfirmResumptionTime = reader["hr_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_confm_dt"],

                            LeaveRequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqs_cls_dt"],
                            
                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList[0];
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId, int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, ");
            sb.Append("r.lm_confm_dt, r.lm_confm_by, r.hr_rsmptn_dt, ");
            sb.Append("r.hr_confm_dt, r.hr_confm_by, r.rqs_cls_dt, r.is_lm_aprv, ");
            sb.Append("r.is_hd_aprv, r.is_hr_aprv, r.is_xm_aprv, r.is_sm_aprv, ");
            sb.Append("r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, r.rqst_rsmptn_dt, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_emp_id = @lvs_emp_id) ");
            sb.Append("AND (r.lvs_rqst_yr = @lvs_rqst_yr) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
                    lvs_rqst_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveRequestList.Add(new LeaveRequest()
                        {
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),

                            UnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            UnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),
                            DepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            DepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString(),
                            LocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                            LocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                            LeaveYear = reader["lvs_rqst_yr"] == DBNull.Value ? 2020 : (int)reader["lvs_rqst_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            RequestedStartDate = reader["lvs_rqst_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_sdt"],
                            RequestedEndDate = reader["lvs_rqst_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_edt"],
                            RequestedDuration = reader["lvs_rqst_dur"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur"],
                            RequestedDurationTypeId = reader["lvs_rqst_dur_typ"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur_typ"],
                            RequestedDurationDescription = reader["lvs_rqst_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_dur_ds"].ToString(),
                            RequestedResumptionDate = reader["rqst_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqst_rsmptn_dt"],

                            LeaveRequestStatusId = reader["lvs_rqst_sts"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_sts"],
                            LeaveRequestStatusDescription = reader["lvs_rqst_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_sts_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualLeaveDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualLeaveDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),
                            LineManagerConfirmResumptionTime = reader["lm_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_confm_dt"],

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),
                            HrConfirmResumptionTime = reader["hr_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_confm_dt"],

                            LeaveRequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqs_cls_dt"],

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        // By Employee Name
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, ");
            sb.Append("r.lm_confm_dt, r.lm_confm_by, r.hr_rsmptn_dt, ");
            sb.Append("r.hr_confm_dt, r.hr_confm_by, r.rqs_cls_dt, r.is_lm_aprv, ");
            sb.Append("r.is_hd_aprv, r.is_hr_aprv, r.is_xm_aprv, r.is_sm_aprv, ");
            sb.Append("r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, r.rqst_rsmptn_dt, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_emp_id IN (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @lvs_emp_nm)) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_nm = cmd.Parameters.Add("@emp_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_nm.Value = employeeName;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveRequestList.Add(new LeaveRequest()
                        {
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),

                            UnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            UnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),
                            DepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            DepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString(),
                            LocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                            LocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                            LeaveYear = reader["lvs_rqst_yr"] == DBNull.Value ? 2020 : (int)reader["lvs_rqst_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            RequestedStartDate = reader["lvs_rqst_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_sdt"],
                            RequestedEndDate = reader["lvs_rqst_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_edt"],
                            RequestedDuration = reader["lvs_rqst_dur"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur"],
                            RequestedDurationTypeId = reader["lvs_rqst_dur_typ"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur_typ"],
                            RequestedDurationDescription = reader["lvs_rqst_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_dur_ds"].ToString(),
                            RequestedResumptionDate = reader["rqst_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqst_rsmptn_dt"],

                            LeaveRequestStatusId = reader["lvs_rqst_sts"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_sts"],
                            LeaveRequestStatusDescription = reader["lvs_rqst_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_sts_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualLeaveDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualLeaveDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),
                            LineManagerConfirmResumptionTime = reader["lm_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_confm_dt"],

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),
                            HrConfirmResumptionTime = reader["hr_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_confm_dt"],

                            LeaveRequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqs_cls_dt"],

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName, int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, ");
            sb.Append("r.lm_confm_dt, r.lm_confm_by, r.hr_rsmptn_dt, ");
            sb.Append("r.hr_confm_dt, r.hr_confm_by, r.rqs_cls_dt, r.is_lm_aprv, ");
            sb.Append("r.is_hd_aprv, r.is_hr_aprv, r.is_xm_aprv, r.is_sm_aprv, ");
            sb.Append("r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, r.rqst_rsmptn_dt, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(r.lvs_emp_id IN (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @lvs_emp_nm)) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_nm = cmd.Parameters.Add("@lvs_emp_nm", NpgsqlDbType.Text);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_nm.Value = employeeName;
                    lvs_rqst_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveRequestList.Add(new LeaveRequest()
                        {
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),

                            UnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            UnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),
                            DepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            DepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString(),
                            LocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                            LocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                            LeaveYear = reader["lvs_rqst_yr"] == DBNull.Value ? 2020 : (int)reader["lvs_rqst_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            RequestedStartDate = reader["lvs_rqst_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_sdt"],
                            RequestedEndDate = reader["lvs_rqst_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_edt"],
                            RequestedDuration = reader["lvs_rqst_dur"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur"],
                            RequestedDurationTypeId = reader["lvs_rqst_dur_typ"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur_typ"],
                            RequestedDurationDescription = reader["lvs_rqst_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_dur_ds"].ToString(),
                            RequestedResumptionDate = reader["rqst_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqst_rsmptn_dt"],

                            LeaveRequestStatusId = reader["lvs_rqst_sts"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_sts"],
                            LeaveRequestStatusDescription = reader["lvs_rqst_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_sts_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualLeaveDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualLeaveDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),
                            LineManagerConfirmResumptionTime = reader["lm_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_confm_dt"],

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),
                            HrConfirmResumptionTime = reader["hr_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_confm_dt"],

                            LeaveRequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqs_cls_dt"],

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName, int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, ");
            sb.Append("r.lm_confm_dt, r.lm_confm_by, r.hr_rsmptn_dt, ");
            sb.Append("r.hr_confm_dt, r.hr_confm_by, r.rqs_cls_dt, r.is_lm_aprv, ");
            sb.Append("r.is_hd_aprv, r.is_hr_aprv, r.is_xm_aprv, r.is_sm_aprv, ");
            sb.Append("r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, r.rqst_rsmptn_dt, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) AND ");
            sb.Append("(r.lvs_emp_id IN (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @lvs_emp_nm)) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_nm = cmd.Parameters.Add("@lvs_emp_nm", NpgsqlDbType.Text);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_nm.Value = employeeName;
                    lvs_rqst_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveRequestList.Add(new LeaveRequest()
                        {
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),

                            UnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            UnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),
                            DepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            DepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString(),
                            LocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                            LocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                            LeaveYear = reader["lvs_rqst_yr"] == DBNull.Value ? 2020 : (int)reader["lvs_rqst_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveReason = reader["lvs_rsn"] == DBNull.Value ? string.Empty : reader["lvs_rsn"].ToString(),

                            RequestedStartDate = reader["lvs_rqst_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_sdt"],
                            RequestedEndDate = reader["lvs_rqst_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lvs_rqst_edt"],
                            RequestedDuration = reader["lvs_rqst_dur"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur"],
                            RequestedDurationTypeId = reader["lvs_rqst_dur_typ"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_dur_typ"],
                            RequestedDurationDescription = reader["lvs_rqst_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_dur_ds"].ToString(),
                            RequestedResumptionDate = reader["rqst_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqst_rsmptn_dt"],

                            LeaveRequestStatusId = reader["lvs_rqst_sts"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_sts"],
                            LeaveRequestStatusDescription = reader["lvs_rqst_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_sts_ds"].ToString(),

                            ActualLeaveStartDate = reader["act_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_sdt"],
                            ActualLeaveEndDate = reader["act_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["act_lvs_edt"],
                            ActualLeaveDuration = reader["act_lvs_dur"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur"],
                            ActualLeaveDurationTypeId = reader["act_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["act_lvs_dur_typ"],
                            ActualLeaveDurationDescription = reader["act_dur_ds"] == DBNull.Value ? string.Empty : reader["act_dur_ds"].ToString(),

                            LineManagersResumptionDate = reader["lm_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_rsmptn_dt"],
                            LineManagerConfirmResumptionBy = reader["lm_confm_by"] == DBNull.Value ? string.Empty : reader["lm_confm_by"].ToString(),
                            LineManagerConfirmResumptionTime = reader["lm_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["lm_confm_dt"],

                            HrResumptionDate = reader["hr_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_rsmptn_dt"],
                            HrConfirmResumptionBy = reader["hr_confm_by"] == DBNull.Value ? string.Empty : reader["hr_confm_by"].ToString(),
                            HrConfirmResumptionTime = reader["hr_confm_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hr_confm_dt"],

                            LeaveRequestCloseDate = reader["rqs_cls_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["rqs_cls_dt"],

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        #endregion


        #endregion

        #region Leave Balances Read Methods
        public async Task<long> GetLeaveDaysUsedByEmployeeIdnLeaveTypeCodenLeaveYearAsync(string employeeId, string leaveTypeCode, int leaveYear)
        {
            long totalCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COALESCE(SUM(no_dys_usd), 0) as total ");
            sb.Append("FROM public.lvm_lvs_trnx ");
            sb.Append("WHERE (lvs_emp_id = @lvs_emp_id) ");
            sb.Append("AND (lvs_typ_cd = @lvs_typ_cd) ");
            sb.Append("AND (lvs_yr = @lvs_yr); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
                    lvs_typ_cd.Value = leaveTypeCode;
                    lvs_yr.Value = leaveYear;
                    var obj = await cmd.ExecuteScalarAsync();
                    totalCount = (long)obj;
                }
                await conn.CloseAsync();
            }
            return totalCount;
        }
        public async Task<long> GetLeaveDaysUsedByEmployeeNamenLeaveTypeCodenLeaveYearAsync(string employeeName, string leaveTypeCode, int leaveYear)
        {
            long totalCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COALESCE(SUM(no_dys_usd), 0) as total ");
            sb.Append("FROM public.lvm_lvs_trnx ");
            sb.Append("WHERE (lvs_typ_cd = @lvs_typ_cd) ");
            sb.Append("AND (lvs_yr = @lvs_yr) ");
            sb.Append("AND (lvs_emp_id = (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @lvs_emp_nm));");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_nm = cmd.Parameters.Add("@lvs_emp_nm", NpgsqlDbType.Text);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_nm.Value = employeeName;
                    lvs_typ_cd.Value = leaveTypeCode;
                    lvs_yr.Value = leaveYear;
                    var obj = await cmd.ExecuteScalarAsync();
                    totalCount = (long)obj;
                }
                await conn.CloseAsync();
            }
            return totalCount;
        }

        #endregion

        #region Leave Activity Log Action Methods
        public async Task<List<LeaveActivityLog>> GetLeaveActivityLogByLeavePlanIdAsync(long leavePlanId)
        {
            List<LeaveActivityLog> activityLogs = new List<LeaveActivityLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT log_id, act_ds, act_dt, lvs_pln_id, lvs_rqs_id ");
            sb.Append("FROM public.lvm_lvs_logs ");
            sb.Append("WHERE (lvs_pln_id = @lvs_pln_id ");
            sb.Append("AND lvs_pln_id IS NOT NULL) ");
            sb.Append("ORDER BY log_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_pln_id.Value = leavePlanId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            activityLogs.Add(new LeaveActivityLog
                            {
                                LeaveActivityLogId = reader["log_id"] == DBNull.Value ? 0 : (long)reader["log_id"],
                                LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0 : (long)reader["lvs_pln_id"],
                                LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0 : (long)reader["lvs_rqs_id"],
                                ActivityTime = reader["act_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_dt"],
                                ActivityDescription = reader["act_ds"] == DBNull.Value ? string.Empty : reader["act_ds"].ToString(),
                            });
                        }
                }
            }
            return activityLogs;
        }
        public async Task<List<LeaveActivityLog>> GetLeaveActivityLogByLeaveRequestIdAsync(long leaveRequestId)
        {
            List<LeaveActivityLog> activityLogs = new List<LeaveActivityLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT log_id, act_ds, act_dt, lvs_pln_id, lvs_rqs_id ");
            sb.Append("FROM public.lvm_lvs_logs ");
            sb.Append("WHERE (lvs_rqs_id = @lvs_rqs_id ");
            sb.Append("AND lvs_rqs_id IS NOT NULL) ");
            sb.Append("ORDER BY log_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_rqs_id.Value = leaveRequestId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            activityLogs.Add(new LeaveActivityLog
                            {
                                LeaveActivityLogId = reader["log_id"] == DBNull.Value ? 0 : (long)reader["log_id"],
                                LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0 : (long)reader["lvs_pln_id"],
                                LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0 : (long)reader["lvs_rqs_id"],
                                ActivityTime = reader["act_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_dt"],
                                ActivityDescription = reader["act_ds"] == DBNull.Value ? string.Empty : reader["act_ds"].ToString(),
                            });
                        }
                }
            }
            return activityLogs;
        }
        public async Task<bool> AddLeaveActivityLogAsync(LeaveActivityLog log)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_logs(act_ds, act_dt, ");
            sb.Append("lvs_pln_id, lvs_rqs_id) ");
            sb.Append("VALUES (@act_ds, @act_dt, @lvs_pln_id, @lvs_rqs_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var act_dt = cmd.Parameters.Add("@act_dt", NpgsqlDbType.Timestamp);
                    var act_ds = cmd.Parameters.Add("@act_ds", NpgsqlDbType.Text);
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    act_dt.Value = log.ActivityTime;
                    act_ds.Value = log.ActivityDescription;
                    lvs_pln_id.Value = log.LeavePlanId ?? (object)DBNull.Value;
                    lvs_rqs_id.Value = log.LeaveRequestId ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteLeaveActivityLogAsync(long leaveActivityLogId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_logs WHERE (log_id = @log_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var log_id = cmd.Parameters.Add("@log_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    log_id.Value = leaveActivityLogId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #region Leave Notes Action Methods
        public async Task<bool> AddNoteAsync(LeaveNote e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_msgs(lvs_pln_id, ");
            sb.Append("lvs_rqs_id, frm_emp_nm, msg_ds, msg_dt) ");
            sb.Append("VALUES (@lvs_pln_id, @lvs_rqs_id, @frm_emp_nm, ");
            sb.Append("@msg_ds, @msg_dt); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);
                    var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                    var msg_ds = cmd.Parameters.Add("@msg_ds", NpgsqlDbType.Text);
                    var msg_dt = cmd.Parameters.Add("@msg_dt", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    lvs_pln_id.Value = e.LeavePlanId ?? (object)DBNull.Value;
                    lvs_rqs_id.Value = e.LeaveRequestId ?? (object)DBNull.Value;
                    frm_emp_nm.Value = e.FromEmployeeName;
                    msg_ds.Value = e.NoteContent ?? (object)DBNull.Value;
                    msg_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<List<LeaveNote>> GetNotesByLeavePlanIdAsync(long leavePlanId)
        {
            List<LeaveNote> notesList = new List<LeaveNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_msg_id, lvs_pln_id, lvs_rqs_id, ");
            sb.Append("frm_emp_nm, msg_ds, msg_dt ");
            sb.Append("FROM public.lvm_lvs_msgs ");
            sb.Append("WHERE lvs_pln_id=@lvs_pln_id ");
            sb.Append("ORDER BY lvs_msg_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_pln_id.Value = leavePlanId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        notesList.Add(new LeaveNote()
                        {
                            Id = reader["lvs_msg_id"] == DBNull.Value ? 0L : (long)reader["lvs_msg_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"],
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            NoteContent = reader["msg_ds"] == DBNull.Value ? string.Empty : reader["msg_ds"].ToString(),
                            TimeAdded = reader["msg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["msg_dt"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return notesList;
        }
        public async Task<List<LeaveNote>> GetNotesByLeaveRequestIdAsync(long leaveRequestId)
        {
            List<LeaveNote> notesList = new List<LeaveNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_msg_id, lvs_pln_id, lvs_rqs_id, ");
            sb.Append("frm_emp_nm, msg_ds, msg_dt ");
            sb.Append("FROM public.lvm_lvs_msgs ");
            sb.Append("WHERE lvs_rqs_id=@lvs_rqs_id ");
            sb.Append("ORDER BY lvs_msg_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_rqs_id.Value = leaveRequestId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        notesList.Add(new LeaveNote()
                        {
                            Id = reader["lvs_msg_id"] == DBNull.Value ? 0L : (long)reader["lvs_msg_id"],
                            LeavePlanId = reader["lvs_pln_id"] == DBNull.Value ? 0L : (long)reader["lvs_pln_id"],
                            LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"],
                            FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                            NoteContent = reader["msg_ds"] == DBNull.Value ? string.Empty : reader["msg_ds"].ToString(),
                            TimeAdded = reader["msg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["msg_dt"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return notesList;
        }

        #endregion

    }
}
