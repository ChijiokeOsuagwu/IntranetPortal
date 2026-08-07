using IntranetPortal.Base.Enums;
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
    public class LeaveRepository : ILeaveRepository
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

        public async Task<bool> DeleteLeaveProfileAsync(string profileCode)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_pfls WHERE (lvs_pfl_cd = @lvs_pfl_cd);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pfl_cd.Value = profileCode;
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
            sb.Append("WHERE (lvs_pfl_cd=@lvs_pfl_cd); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    var lvs_pfl_nm = cmd.Parameters.Add("@lvs_pfl_nm", NpgsqlDbType.Text);
                    var lvs_pfl_ds = cmd.Parameters.Add("@lvs_pfl_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pfl_cd.Value = leaveProfile.Code;
                    lvs_pfl_nm.Value = leaveProfile.Name;
                    lvs_pfl_ds.Value = leaveProfile.Description ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<LeaveProfile> GetLeaveProfileByCodeAsync(string profileCode)
        {
            LeaveProfile leaveProfile = new LeaveProfile();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(profileCode)) { throw new Exception("Required parameter [Profile Code] cannot be null."); }
            sb.Append("SELECT lvs_pfl_cd, lvs_pfl_nm, lvs_pfl_ds, ");
            sb.Append("lvs_pfl_cd FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE (lvs_pfl_cd = @lvs_pfl_cd);");
            query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_pfl_cd.Value = profileCode;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
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
            sb.Append("SELECT lvs_pfl_nm, lvs_pfl_ds, ");
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

            sb.Append("SELECT lvs_pfl_nm, lvs_pfl_ds, ");
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
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileCodeAsync(string profileCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_cd, d.lvs_typ_cd, ");
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
            sb.Append("(SELECT lvs_pfl_nm FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_cd = d.lvs_pfl_cd) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lvm_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE (d.lvs_pfl_cd = @lvs_pfl_cd) ");
            sb.Append("ORDER BY d.lvs_typ_cd; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_pfl_cd.Value = profileCode;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfileDetails.Add(new LeaveProfileDetail()
                        {
                            Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
                            ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
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
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_cd, d.lvs_typ_cd, ");
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
            sb.Append("(SELECT lvs_pfl_nm FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_cd = d.lvs_pfl_cd) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lvm_lvs_typs  ");
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
                            ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
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
        public async Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileCodenLeaveTypeAsync(string profileCode, string leaveTypeCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_cd, d.lvs_typ_cd, ");
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
            sb.Append("(SELECT lvs_pfl_nm FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_cd = d.lvs_pfl_cd) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lvm_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE (d.lvs_pfl_cd = @lvs_pfl_cd) ");
            sb.Append("AND (d.lvs_typ_cd = @lvs_typ_cd); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_pfl_cd.Value = profileCode;
                    lvs_typ_cd.Value = leaveTypeCode;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveProfileDetails.Add(new LeaveProfileDetail()
                        {
                            Id = reader["pfl_dtl_id"] == DBNull.Value ? 0 : (int)reader["pfl_dtl_id"],
                            ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
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
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_cd, d.lvs_typ_cd, ");
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
            sb.Append("(SELECT lvs_pfl_nm FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_cd = d.lvs_pfl_cd) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lvm_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE d.lvs_pfl_cd = (SELECT lvs_pfl_cd FROM public.erm_emp_inf ");
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
                            ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
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
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_cd, d.lvs_typ_cd, ");
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
            sb.Append("(SELECT lvs_pfl_nm FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_cd = d.lvs_pfl_cd) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lvm_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE d.lvs_pfl_cd = (SELECT lvs_pfl_cd FROM public.erm_emp_inf ");
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
                            ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
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
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_cd, d.lvs_typ_cd, ");
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
            sb.Append("(SELECT lvs_pfl_nm FROM public.lvm_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_cd = d.lvs_pfl_cd) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lvm_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_pfdt d ");
            sb.Append("WHERE (d.lvs_typ_cd = @lvs_typ_cd) ");
            sb.Append("AND d.lvs_pfl_cd = (SELECT lvs_pfl_cd FROM public.erm_emp_inf ");
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
                            ProfileCode = reader["lvs_pfl_cd"] == DBNull.Value ? string.Empty : reader["lvs_pfl_cd"].ToString(),
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
            sb.Append("INSERT INTO public.lvm_lvs_pfdt(lvs_pfl_cd, ");
            sb.Append("lvs_typ_cd, is_yrly, cancarryover, is_mntz, ");
            sb.Append("lvs_dur, dur_typ, carryover_end_mn, lvs_dur_ds) ");
            sb.Append("VALUES (@lvs_pfl_cd, @lvs_typ_cd, @is_yrly, ");
            sb.Append("@cancarryover, @is_mntz, @lvs_dur, @dur_typ, ");
            sb.Append("@carryover_end_mn, @lvs_dur_ds); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var is_yrly = cmd.Parameters.Add("@is_yrly", NpgsqlDbType.Boolean);
                    var cancarryover = cmd.Parameters.Add("@cancarryover", NpgsqlDbType.Boolean);
                    var is_mntz = cmd.Parameters.Add("@is_mntz", NpgsqlDbType.Boolean);
                    var lvs_dur = cmd.Parameters.Add("@lvs_dur", NpgsqlDbType.Integer);
                    var dur_typ = cmd.Parameters.Add("@dur_typ", NpgsqlDbType.Integer);
                    var carryover_end_mn = cmd.Parameters.Add("@carryover_end_mn", NpgsqlDbType.Integer);
                    var lvs_dur_ds = cmd.Parameters.Add("@lvs_dur_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pfl_cd.Value = leaveProfileDetail.ProfileCode;
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
        public async Task<LeaveDuration> GetLeaveDurationByProfileCodenLeaveTypeAsync(string profileCode, string leaveTypeCode)
        {
            LeaveDuration leaveDuration = new LeaveDuration();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT lvs_dur, act_lvs_dur_typ, lvs_dur_ds ");
            sb.Append("FROM public.lvm_lvs_pfdt ");
            sb.Append("WHERE lvs_pfl_cd = @lvs_pfl_cd ");
            sb.Append("AND lvs_typ_cd = @lvs_typ_cd; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pfl_cd = cmd.Parameters.Add("@lvs_pfl_cd", NpgsqlDbType.Text);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_pfl_cd.Value = profileCode;
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
            sb.Append("pln_rsmptn_dt, pln_dur_typ, is_closed, is_returned) ");
            sb.Append("VALUES (@emp_id, @unit_id, @dept_id, @loc_id, ");
            sb.Append("@lvs_yr, @lvs_typ_cd, @lvs_rsn, @lvs_pln_sdt, ");
            sb.Append("@lvs_pln_edt, @lvs_pln_dur, @pln_dur_ds, ");
            sb.Append("@pln_rsmptn_dt, @pln_dur_typ, @is_closed, @is_returned) ");
            sb.Append("RETURNING lvs_pln_id;  ");

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

                    var is_closed = cmd.Parameters.Add("@is_closed", NpgsqlDbType.Boolean);
                    var is_returned = cmd.Parameters.Add("@is_returned", NpgsqlDbType.Boolean);

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

                    is_closed.Value = e.LeavePlanIsClosed;
                    is_returned.Value = e.LeavePlanIsReturned;

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
        public async Task<bool> EditLeavePlanReturnStatusAsync(long leavePlanId, bool isReturned)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.lvm_lvs_plns SET is_returned=@is_returned ");
            sb.Append("WHERE (lvs_pln_id=@lvs_pln_id); ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var is_returned = cmd.Parameters.Add("@is_returned", NpgsqlDbType.Boolean);
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    is_returned.Value = isReturned;
                    lvs_pln_id.Value = leavePlanId;

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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList[0];
        }

        // By Employee ID
        public async Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.emp_id = @emp_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

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
                            
                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId, int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId, int leaveYear, int leaveMonth)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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
            sb.Append("AND (p.emp_id = @emp_id) ");
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
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = employeeId;
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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
                            
                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
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
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        #endregion

        #region Leave Plans for Reports & Team Members
        public async Task<List<LeavePlan>> GetLeavePlansByReportingLineIdAsync(string teamLeadId)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND p.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        public async Task<List<LeavePlan>> GetLeavePlansByReportingLineIdAsync(string teamLeadId, int leaveYear)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_yr = @lvs_yr) ");
            sb.Append("AND p.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }
        public async Task<List<LeavePlan>> GetLeavePlansByReportingLineIdAsync(string teamLeadId, int leaveYear, int startMonth)
        {
            List<LeavePlan> leavePlanList = new List<LeavePlan>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, ");
            sb.Append("p.loc_id, p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_pln_sdt, ");
            sb.Append("p.lvs_pln_edt, p.lvs_pln_dur, p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("p.pln_dur_typ, p.is_closed, p.is_returned, ");
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
            sb.Append("AND p.emp_id IN (SELECT r.emp_id FROM public.erm_emp_rpts r  ");
            sb.Append("WHERE r.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY p.lvs_pln_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_yr.Value = leaveYear;
                    sdt_month.Value = startMonth;
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

                            LeavePlanIsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                            LeavePlanIsReturned = reader["is_returned"] == DBNull.Value ? false : (bool)reader["is_returned"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leavePlanList;
        }

        #endregion

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
            sb.Append("rqst_rsmptn_dt, is_rqs_cls, rqs_cls_by, rqs_lvs_allw) ");
            sb.Append("VALUES (@lvs_emp_id, @lvs_unit_id, @lvs_dept_id, @lvs_loc_id, ");
            sb.Append("@lvs_rqst_yr, @lvs_typ_cd, @lvs_rsn, @lvs_rqst_sts, ");
            sb.Append("@lvs_rqst_sdt, @lvs_rqst_edt, @lvs_rqst_dur, @lvs_rqst_dur_ds, ");
            sb.Append("@act_lvs_sdt, @act_lvs_edt, @act_lvs_dur, @act_dur_ds, ");
            sb.Append("@lm_rsmptn_dt, @lm_confm_dt, @lm_confm_by, @hr_rsmptn_dt,  ");
            sb.Append("@hr_confm_dt, @hr_confm_by, @rqs_cls_dt, @is_lm_aprv, ");
            sb.Append("@is_hd_aprv, @is_hr_aprv, @is_xm_aprv, @is_sm_aprv,   ");
            sb.Append("@lvs_rqst_dur_typ, @act_lvs_dur_typ, @rqst_rsmptn_dt, ");
            sb.Append("@is_rqs_cls, @rqs_cls_by, @rqs_lvs_allw) ");
            sb.Append("RETURNING lvs_rqst_id; ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
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

                    var is_rqs_cls = cmd.Parameters.Add("@is_rqs_cls", NpgsqlDbType.Boolean);
                    var rqs_cls_by = cmd.Parameters.Add("@rqs_cls_by", NpgsqlDbType.Text);
                    var rqs_lvs_allw = cmd.Parameters.Add("@rqs_lvs_allw", NpgsqlDbType.Boolean);
                    
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
                    act_dur_ds.Value = r.ActualLeaveDurationDescription ?? (object)DBNull.Value;

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
                    act_lvs_dur_typ.Value = r.ActualLeaveDurationTypeId ?? (object)DBNull.Value;
                    rqst_rsmptn_dt.Value = r.RequestedResumptionDate ?? (object)DBNull.Value;

                    is_rqs_cls.Value = r.IsLeaveRequestClosed;
                    rqs_cls_by.Value = r.LeaveRequestClosedBy ?? (object)DBNull.Value;
                    rqs_lvs_allw.Value = r.RequestLeaveAllowance;

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
            sb.Append("DELETE FROM public.lvm_lvs_rqsts WHERE (lvs_rqst_id = @lvs_rqs_id); ");
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
            sb.Append("lvs_rqst_sts=@lvs_rqst_sts, rqs_lvs_allw=@rqs_lvs_allw  ");
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
                    var rqs_lvs_allw = cmd.Parameters.Add("@rqs_lvs_allw", NpgsqlDbType.Boolean);

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
                    rqs_lvs_allw.Value = r.RequestLeaveAllowance;

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
        public async Task<bool> UpdateLeaveRequestApprovalStatusAsync(long leaveRequestId, ApprovalType approvalType)
        {
            int rows = 0;
            string query = string.Empty;
            switch (approvalType)
            {
                case ApprovalType.LineManager:
                    query = "UPDATE public.lvm_lvs_rqsts SET is_lm_aprv=true WHERE (lvs_rqst_id=@lvs_rqst_id);";
                    break;
                case ApprovalType.HeadofDepartment:
                    query = "UPDATE public.lvm_lvs_rqsts SET is_hd_aprv=true WHERE (lvs_rqst_id=@lvs_rqst_id);";
                    break;
                case ApprovalType.HrDepartment:
                    query = "UPDATE public.lvm_lvs_rqsts SET is_hr_aprv=true WHERE (lvs_rqst_id=@lvs_rqst_id);";
                    break;
                case ApprovalType.StationManager:
                    query = "UPDATE public.lvm_lvs_rqsts SET is_sm_aprv=true WHERE (lvs_rqst_id=@lvs_rqst_id);";
                    break;
                case ApprovalType.ExecutiveManagement:
                    query = "UPDATE public.lvm_lvs_rqsts SET is_xm_aprv=true WHERE (lvs_rqst_id=@lvs_rqst_id);";
                    break;
                default:
                    break;
            }

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_rqst_id.Value = leaveRequestId;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeaveRequestHrConfirmedAsync(long leaveRequestId, string confirmedBy, DateTime confirmedTime)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_rqsts ");
            sb.Append("SET hr_confm_dt=@hr_confm_dt, ");
            sb.Append("hr_confm_by=@hr_confm_by, lvs_rqst_sts=@lvs_rqst_sts ");
            sb.Append("WHERE (lvs_rqst_id = @lvs_rqst_id); ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var hr_confm_by = cmd.Parameters.Add("@hr_confm_by", NpgsqlDbType.Text);
                    var hr_confm_dt = cmd.Parameters.Add("@hr_confm_dt", NpgsqlDbType.Timestamp);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    lvs_rqst_id.Value = leaveRequestId;
                    hr_confm_by.Value = confirmedBy;
                    hr_confm_dt.Value = confirmedTime;
                    lvs_rqst_sts.Value = (int)LeaveStatusEnum.Confirmed;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeaveRequestToClosedAsync(LeaveRequest leaveRequest, string leaveRequestClosedBy)
        {
            int rows = 0;
            int leaveRequestStatus = (int)LeaveStatusEnum.Completed;

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_rqsts ");
            sb.Append("SET is_rqs_cls=@is_rqs_cls, rqs_cls_dt=@rqs_cls_dt, ");
            sb.Append("rqs_cls_by=@rqs_cls_by, lvs_rqst_sts=@lvs_rqst_sts, ");
            sb.Append("act_lvs_sdt=@act_lvs_sdt, act_lvs_edt=@act_lvs_edt, ");
            sb.Append("hr_rsmptn_dt=@hr_rsmptn_dt, act_lvs_dur=@act_lvs_dur, ");
            sb.Append("act_dur_ds=@act_dur_ds, act_lvs_dur_typ=@act_lvs_dur_typ ");
            sb.Append("WHERE (lvs_rqst_id = @lvs_rqst_id); ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var rqs_cls_by = cmd.Parameters.Add("@rqs_cls_by", NpgsqlDbType.Text);
                    var rqs_cls_dt = cmd.Parameters.Add("@rqs_cls_dt", NpgsqlDbType.Timestamp);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    var is_rqs_cls = cmd.Parameters.Add("@is_rqs_cls", NpgsqlDbType.Boolean);

                    var act_lvs_sdt = cmd.Parameters.Add("@act_lvs_sdt", NpgsqlDbType.Timestamp);
                    var act_lvs_edt = cmd.Parameters.Add("@act_lvs_edt", NpgsqlDbType.Timestamp);
                    var hr_rsmptn_dt = cmd.Parameters.Add("@hr_rsmptn_dt", NpgsqlDbType.Timestamp);

                    var act_lvs_dur = cmd.Parameters.Add("@act_lvs_dur", NpgsqlDbType.Integer);
                    var act_dur_ds = cmd.Parameters.Add("@act_dur_ds", NpgsqlDbType.Text);
                    var act_lvs_dur_typ = cmd.Parameters.Add("@act_lvs_dur_typ", NpgsqlDbType.Integer);

                    var rqs_lvs_adj = cmd.Parameters.Add("@rqs_lvs_adj", NpgsqlDbType.Boolean);

                    cmd.Prepare();

                    lvs_rqst_id.Value = leaveRequest.LeaveRequestId;
                    rqs_cls_by.Value = leaveRequestClosedBy;
                    rqs_cls_dt.Value = DateTime.Now;
                    lvs_rqst_sts.Value = leaveRequestStatus;
                    is_rqs_cls.Value = true;

                    act_lvs_sdt.Value = leaveRequest.ActualLeaveStartDate;
                    act_lvs_edt.Value = leaveRequest.ActualLeaveEndDate;
                    hr_rsmptn_dt.Value = leaveRequest.HrResumptionDate;

                    act_lvs_dur.Value = leaveRequest.ActualLeaveDuration;
                    act_dur_ds.Value = leaveRequest.ActualLeaveDurationDescription;
                    act_lvs_dur_typ.Value = leaveRequest.ActualLeaveDurationTypeId;

                    rqs_lvs_adj.Value = leaveRequest.RequestLeaveAdjustment;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeaveRequestAdjustmentRequestAsync(long leaveRequestId, bool requestedAdjustment)
        {
            int rows = 0;
            int leaveRequestStatus = (int)LeaveStatusEnum.Completed;

            string query = @"UPDATE public.lvm_lvs_rqsts SET rqs_lvs_adj=@rqs_lvs_adj WHERE (lvs_rqst_id = @lvs_rqst_id); ";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var rqs_lvs_adj = cmd.Parameters.Add("@rqs_lvs_adj", NpgsqlDbType.Boolean);

                    cmd.Prepare();

                    lvs_rqst_id.Value = leaveRequestId;
                    rqs_lvs_adj.Value = requestedAdjustment;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }


        #endregion

        #region Leave Request Read Action Methods

        #region Leave Requests By LeaveRequestId & Employee Id & Name
        public async Task<LeaveRequest> GetLeaveRequestByIdAsync(long leaveRequestId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList[0];
        }

        // By Employee ID
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_emp_id = @lvs_emp_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId, int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId, int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("AND (r.lvs_emp_id = @lvs_emp_id) ");
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
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdnStatusAsync(string employeeId, int leaveStatus)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_emp_id = @lvs_emp_id) ");
            sb.Append("AND (r.lvs_rqst_sts = @lvs_rqst_sts) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
                    lvs_rqst_sts.Value = leaveStatus;

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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdnStatusAsync(string employeeId, int leaveYear, int leaveStatus)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) ");
            sb.Append("AND (r.lvs_emp_id = @lvs_emp_id) ");
            sb.Append("AND (r.lvs_rqst_sts = @lvs_rqst_sts) ");
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
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
                    lvs_rqst_yr.Value = leaveYear;
                    lvs_rqst_sts.Value = leaveStatus;

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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdnStatusAsync(string employeeId, int leaveYear, int leaveMonth, int leaveStatus)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("AND (r.lvs_emp_id = @lvs_emp_id) ");
            sb.Append("AND (r.lvs_rqst_sts = @lvs_rqst_sts) ");
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
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
                    lvs_rqst_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    lvs_rqst_sts.Value = leaveStatus;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

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
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

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
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

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
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        #endregion

        #region Leave Requests By ReportingLine
        public async Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdnStatusAsync(string teamLeadId, int leaveYear, int leaveMonth, int leaveStatus)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("AND (r.lvs_rqst_sts = @lvs_rqst_sts) ");
            sb.Append("AND r.lvs_emp_id IN (SELECT e.emp_id FROM public.erm_emp_rpts e  ");
            sb.Append("WHERE e.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_rqst_yr.Value = leaveYear;
                    sdt_month.Value = leaveMonth;
                    lvs_rqst_sts.Value = leaveStatus;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdnStatusAsync(string teamLeadId, int leaveYear, int leaveStatus)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr)  ");
            sb.Append("AND (r.lvs_rqst_sts = @lvs_rqst_sts) ");
            sb.Append("AND r.lvs_emp_id IN (SELECT e.emp_id FROM public.erm_emp_rpts e  ");
            sb.Append("WHERE e.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var lvs_rqst_sts = cmd.Parameters.Add("@lvs_rqst_sts", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
                    lvs_rqst_yr.Value = leaveYear;
                    lvs_rqst_sts.Value = leaveStatus;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdAsync(string teamLeadId, int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("AND r.lvs_emp_id IN (SELECT e.emp_id FROM public.erm_emp_rpts e  ");
            sb.Append("WHERE e.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdAsync(string teamLeadId, int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr)  ");
            sb.Append("AND (r.lvs_rqst_sts = @lvs_rqst_sts) ");
            sb.Append("AND r.lvs_emp_id IN (SELECT e.emp_id FROM public.erm_emp_rpts e  ");
            sb.Append("WHERE e.rpt_emp_id = @rpt_emp_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = teamLeadId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        #endregion

        #region Leave Requests By Location & Unit

        // For Leave Year & Leave Month
        public async Task<List<LeaveRequest>> GetLeaveRequestsByLeaveYearAsync(int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByLeaveYearnLeaveMonthAsync(int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");

            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }


        // For LocationId
        public async Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdAsync(int locationId, int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_rqst_yr.Value = leaveYear;
                    lvs_loc_id.Value = locationId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdAsync(int locationId, int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("AND (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_loc_id.Value = locationId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        // For UnitId
        public async Task<List<LeaveRequest>> GetLeaveRequestsByUnitIdAsync(int unitId, int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_rqst_yr.Value = leaveYear;
                    lvs_unit_id.Value = unitId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByUnitIdAsync(int unitId, int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("AND (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        // For LocationId & UnitId
        public async Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_yr = @lvs_rqst_yr) ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_rqst_yr.Value = leaveYear;
                    lvs_unit_id.Value = unitId;
                    lvs_loc_id.Value = locationId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("AND (r.lvs_rqst_yr = @lvs_rqst_yr) AND ");
            sb.Append("(EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @sdt_month) ");
            sb.Append("ORDER BY r.lvs_rqst_sdt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_rqst_yr = cmd.Parameters.Add("@lvs_rqst_yr", NpgsqlDbType.Integer);
                    var sdt_month = cmd.Parameters.Add("@sdt_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
                    lvs_loc_id.Value = locationId;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        #endregion

        #region Leave Requests Due Resumption By Resumption Dates
        // By Resumption Dates
        public async Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearAsync(int leaveResumptionYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE r.lvs_rqst_sts IN (4,5,6) ");
            sb.Append("AND (EXTRACT(YEAR FROM r.rqst_rsmptn_dt) = @resumption_year)  ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var resumption_year = cmd.Parameters.Add("@resumption_year", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    resumption_year.Value = leaveResumptionYear;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthAsync(int leaveResumptionYear, int leaveResumptionMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE r.lvs_rqst_sts IN (4,5,6) ");
            sb.Append("AND (EXTRACT(YEAR FROM r.rqst_rsmptn_dt) = @resumption_year  ");
            sb.Append("AND EXTRACT(MONTH FROM r.rqst_rsmptn_dt) = @resumption_month)  ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var resumption_year = cmd.Parameters.Add("@resumption_year", NpgsqlDbType.Integer);
                    var resumption_month = cmd.Parameters.Add("@resumption_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    resumption_year.Value = leaveResumptionYear;
                    resumption_month.Value = leaveResumptionMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnLocationIdAsync(int leaveResumptionYear, int leaveResumptionMonth, int locationId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE r.lvs_rqst_sts IN (4,5,6) ");
            sb.Append("AND (EXTRACT(YEAR FROM r.rqst_rsmptn_dt) = @resumption_year  ");
            sb.Append("AND EXTRACT(MONTH FROM r.rqst_rsmptn_dt) = @resumption_month)  ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var resumption_year = cmd.Parameters.Add("@resumption_year", NpgsqlDbType.Integer);
                    var resumption_month = cmd.Parameters.Add("@resumption_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_loc_id.Value = locationId;
                    resumption_year.Value = leaveResumptionYear;
                    resumption_month.Value = leaveResumptionMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnUnitIdAsync(int leaveResumptionYear, int leaveResumptionMonth, int unitId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE r.lvs_rqst_sts IN (4,5,6) ");
            sb.Append("AND (EXTRACT(YEAR FROM r.rqst_rsmptn_dt) = @resumption_year  ");
            sb.Append("AND EXTRACT(MONTH FROM r.rqst_rsmptn_dt) = @resumption_month)  ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var resumption_year = cmd.Parameters.Add("@resumption_year", NpgsqlDbType.Integer);
                    var resumption_month = cmd.Parameters.Add("@resumption_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
                    resumption_year.Value = leaveResumptionYear;
                    resumption_month.Value = leaveResumptionMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnLocationIdnUnitIdAsync(int leaveResumptionYear, int leaveResumptionMonth, int locationId, int unitId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE r.lvs_rqst_sts IN (4,5,6) ");
            sb.Append("AND (EXTRACT(YEAR FROM r.rqst_rsmptn_dt) = @resumption_year  ");
            sb.Append("AND EXTRACT(MONTH FROM r.rqst_rsmptn_dt) = @resumption_month)  ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var resumption_year = cmd.Parameters.Add("@resumption_year", NpgsqlDbType.Integer);
                    var resumption_month = cmd.Parameters.Add("@resumption_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
                    lvs_loc_id.Value = locationId;
                    resumption_year.Value = leaveResumptionYear;
                    resumption_month.Value = leaveResumptionMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        #endregion

        #region Approved Leave Requests
        // Approved Leave Requests
        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthAsync(int leaveYear, int leaveMonth)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("AND (EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @leave_month  ");
            sb.Append("OR EXTRACT(MONTH FROM r.lvs_rqst_edt) = @leave_month)  ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    var leave_month = cmd.Parameters.Add("@leave_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    leave_year.Value = leaveYear;
                    leave_month.Value = leaveMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnLocationIdAsync(int leaveYear, int leaveMonth, int locationId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("AND (EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @leave_month  ");
            sb.Append("OR EXTRACT(MONTH FROM r.lvs_rqst_edt) = @leave_month)  ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    var leave_month = cmd.Parameters.Add("@leave_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_loc_id.Value = locationId;
                    leave_year.Value = leaveYear;
                    leave_month.Value = leaveMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnUnitIdAsync(int leaveYear, int leaveMonth, int unitId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("AND (EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @leave_month  ");
            sb.Append("OR EXTRACT(MONTH FROM r.lvs_rqst_edt) = @leave_month)  ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    var leave_month = cmd.Parameters.Add("@leave_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
                    leave_year.Value = leaveYear;
                    leave_month.Value = leaveMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnLocationIdnUnitIdAsync(int leaveYear, int leaveMonth, int locationId, int unitId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("AND (EXTRACT(MONTH FROM r.lvs_rqst_sdt) = @leave_month  ");
            sb.Append("OR EXTRACT(MONTH FROM r.lvs_rqst_edt) = @leave_month)  ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    var leave_month = cmd.Parameters.Add("@leave_month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
                    lvs_loc_id.Value = locationId;
                    leave_year.Value = leaveYear;
                    leave_month.Value = leaveMonth;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLocationIdnUnitIdAsync(int leaveYear, int locationId, int unitId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
                    lvs_loc_id.Value = locationId;
                    leave_year.Value = leaveYear;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLocationIdAsync(int leaveYear, int locationId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("AND (r.lvs_loc_id = @lvs_loc_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_loc_id.Value = locationId;
                    leave_year.Value = leaveYear;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnUnitIdAsync(int leaveYear, int unitId)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("AND (r.lvs_unit_id = @lvs_unit_id) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_unit_id.Value = unitId;
                    leave_year.Value = leaveYear;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }
        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearAsync(int leaveYear)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.lvs_rqst_id, r.lvs_emp_id, r.lvs_unit_id, ");
            sb.Append("r.lvs_dept_id, r.lvs_loc_id, r.lvs_rqst_yr, r.rqs_lvs_adj, ");
            sb.Append("r.lvs_typ_cd, r.lvs_rsn, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Resumption Notified' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Pending Closure' ");
            sb.Append("WHEN r.lvs_rqst_sts = 7 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 8 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur, ");
            sb.Append("r.lvs_rqst_dur_ds, r.act_lvs_sdt, r.act_lvs_edt, ");
            sb.Append("r.act_lvs_dur, r.act_dur_ds, r.lm_rsmptn_dt, r.lm_confm_dt, ");
            sb.Append("r.lm_confm_by, r.hr_rsmptn_dt, r.hr_confm_dt, r.hr_confm_by, ");
            sb.Append("r.rqs_cls_dt, r.is_lm_aprv, r.is_hd_aprv, r.is_hr_aprv, ");
            sb.Append("r.is_xm_aprv, r.is_sm_aprv, r.lvs_rqst_dur_typ, r.act_lvs_dur_typ, ");
            sb.Append("r.rqst_rsmptn_dt, r.is_rqs_cls, r.rqs_cls_by, r.rqs_lvs_allw, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_rqsts r ");
            sb.Append("WHERE (r.lvs_rqst_sts = 3) ");
            sb.Append("AND (r.lvs_rqst_yr = @leave_year) ");
            sb.Append("ORDER BY r.rqst_rsmptn_dt; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var leave_year = cmd.Parameters.Add("@leave_year", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    leave_year.Value = leaveYear;
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
                            IsLeaveRequestClosed = reader["is_rqs_cls"] == DBNull.Value ? false : (bool)reader["is_rqs_cls"],
                            LeaveRequestClosedBy = reader["rqs_cls_by"] == DBNull.Value ? string.Empty : reader["rqs_cls_by"].ToString(),

                            IsApprovedByLineManager = reader["is_lm_aprv"] == DBNull.Value ? false : (bool)reader["is_lm_aprv"],
                            IsApprovedByHeadOfDepartment = reader["is_hd_aprv"] == DBNull.Value ? false : (bool)reader["is_hd_aprv"],
                            IsApprovedByHR = reader["is_hr_aprv"] == DBNull.Value ? false : (bool)reader["is_hr_aprv"],
                            IsApprovedByStationManager = reader["is_sm_aprv"] == DBNull.Value ? false : (bool)reader["is_sm_aprv"],
                            IsApprovedByExecutiveManagement = reader["is_xm_aprv"] == DBNull.Value ? false : (bool)reader["is_xm_aprv"],

                            RequestLeaveAllowance = reader["rqs_lvs_allw"] == DBNull.Value ? false : (bool)reader["rqs_lvs_allw"],
                            RequestLeaveAdjustment = reader["rqs_lvs_adj"] == DBNull.Value ? false : (bool)reader["rqs_lvs_adj"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveRequestList;
        }

        #endregion

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
            sb.Append("to_emp_rl, lvs_doc_typ FROM public.lvm_lvs_sbms ");
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
                            DocumentType = reader["lvs_doc_typ"] == DBNull.Value ? string.Empty : reader["lvs_doc_typ"].ToString(),
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
            sb.Append("to_emp_rl, lvs_doc_typ FROM public.lvm_lvs_sbms ");
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
                            DocumentType = reader["lvs_doc_typ"] == DBNull.Value ? string.Empty : reader["lvs_doc_typ"].ToString(),
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
            sb.Append("to_emp_rl, lvs_doc_typ FROM public.lvm_lvs_sbms ");
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
                            DocumentType = reader["lvs_doc_typ"] == DBNull.Value ? string.Empty : reader["lvs_doc_typ"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }


        //==== Leave Request Submission Read Action Methods
        public async Task<List<LeaveSubmission>> GetLeaveSubmissionsByRolenYearSubmittedAsync(string toEmployeeRole, int yearSubmitted)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.lvs_sbm_id, s.lvs_pln_id, s.lvs_rqst_id, ");
            sb.Append("s.frm_emp_nm, s.to_emp_nm, s.sbm_purps, s.sbm_dt, ");
            sb.Append("s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, lvs_doc_typ, ");

            sb.Append("p.lvs_yr, p.lvs_pln_sdt, p.lvs_pln_edt, ");
            sb.Append("p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as pln_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_pln_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as pln_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as pln_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as pln_loc_nm, ");

            sb.Append("r.lvs_rqst_yr, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur_ds, ");
            sb.Append("r.rqst_rsmptn_dt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");

            sb.Append("FROM public.lvm_lvs_sbms s ");
            sb.Append("LEFT JOIN public.lvm_lvs_plns p ON p.lvs_pln_id = s.lvs_pln_id ");
            sb.Append("LEFT JOIN public.lvm_lvs_rqsts r ON r.lvs_rqst_id = s.lvs_rqst_id ");
            sb.Append("WHERE LOWER(s.to_emp_rl) = LOWER(@to_emp_rl) ");
            sb.Append("AND (DATE_PART('Year', s.sbm_dt) = @yr) ");
            sb.Append("ORDER BY s.lvs_sbm_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_rl = cmd.Parameters.Add("@to_emp_rl", NpgsqlDbType.Text);
                    var yr = cmd.Parameters.Add("@yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    to_emp_rl.Value = toEmployeeRole;
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
                            DocumentType = reader["lvs_doc_typ"] == DBNull.Value ? string.Empty : reader["lvs_doc_typ"].ToString(),


                            LeavePlanYear = reader["lvs_yr"] == DBNull.Value ? 0 : (int)reader["lvs_yr"],
                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? new DateTime(2020, 1, 1) : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? new DateTime(2020, 1, 1) : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? new DateTime(2020, 1, 1) : (DateTime)reader["pln_rsmptn_dt"],
                            LeavePlanEmployeeName = reader["pln_emp_nm"] == DBNull.Value ? string.Empty : reader["pln_emp_nm"].ToString(),
                            LeavePlanTypeName = reader["lvs_pln_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_pln_typ_nm"].ToString(),
                            LeavePlanUnitName = reader["pln_unit_nm"] == DBNull.Value ? string.Empty : reader["pln_unit_nm"].ToString(),
                            LeavePlanLocationName = reader["pln_loc_nm"] == DBNull.Value ? string.Empty : reader["pln_loc_nm"].ToString(),


                            LeaveRequestYear = reader["lvs_rqst_yr"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_yr"],
                            LeaveRequestStartDate = reader["lvs_rqst_sdt"] == DBNull.Value ? new DateTime(2000, 1, 1) : (DateTime)reader["lvs_rqst_sdt"],
                            LeaveRequestEndDate = reader["lvs_rqst_edt"] == DBNull.Value ? new DateTime(2000, 1, 1) : (DateTime)reader["lvs_rqst_edt"],
                            LeaveRequestDurationDescription = reader["lvs_rqst_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_dur_ds"].ToString(),
                            LeaveRequestResumptionDate = reader["rqst_rsmptn_dt"] == DBNull.Value ? new DateTime(2000, 1, 1) : (DateTime)reader["rqst_rsmptn_dt"],
                            LeaveRequestStatusDescription = reader["lvs_rqst_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_sts_ds"].ToString(),
                            LeaveRequestEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            LeaveRequestTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveRequestUnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),
                            LeaveRequestLocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }

        public async Task<List<LeaveSubmission>> GetLeaveSubmissionsByRequestIdnRolenPurposeAsync(long leaveRequestId, string toEmployeeRole, string purpose)
        {
            List<LeaveSubmission> submissionList = new List<LeaveSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.lvs_sbm_id, s.lvs_pln_id, s.lvs_rqst_id, ");
            sb.Append("s.frm_emp_nm, s.to_emp_nm, s.sbm_purps, s.sbm_dt, ");
            sb.Append("s.sbm_msg, s.is_xtn, s.dt_xtn, s.to_emp_rl, lvs_doc_typ, ");

            sb.Append("p.lvs_yr, p.lvs_pln_sdt, p.lvs_pln_edt, ");
            sb.Append("p.pln_dur_ds, p.pln_rsmptn_dt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as pln_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_pln_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as pln_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as pln_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as pln_loc_nm, ");

            sb.Append("r.lvs_rqst_yr, r.lvs_rqst_sts, ");
            sb.Append("CASE WHEN r.lvs_rqst_sts = 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 1 THEN 'Pending Approval' ");
            sb.Append("WHEN r.lvs_rqst_sts = 2 THEN 'Declined' ");
            sb.Append("WHEN r.lvs_rqst_sts = 3 THEN 'Approved' ");
            sb.Append("WHEN r.lvs_rqst_sts = 4 THEN 'Confirmed' ");
            sb.Append("WHEN r.lvs_rqst_sts = 5 THEN 'Cancelled' ");
            sb.Append("WHEN r.lvs_rqst_sts = 6 THEN 'Completed' ");
            sb.Append("ELSE 'Unknown' END AS lvs_rqst_sts_ds, ");
            sb.Append("r.lvs_rqst_sdt, r.lvs_rqst_edt, r.lvs_rqst_dur_ds, ");
            sb.Append("r.rqst_rsmptn_dt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.lvs_emp_id) ");
            sb.Append("as lvs_emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = r.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.lvs_unit_id) ");
            sb.Append("as lvs_unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= r.lvs_dept_id) as lvs_dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = r.lvs_loc_id) as lvs_loc_nm ");

            sb.Append("FROM public.lvm_lvs_sbms s ");
            sb.Append("LEFT JOIN public.lvm_lvs_plns p ON p.lvs_pln_id = s.lvs_pln_id ");
            sb.Append("LEFT JOIN public.lvm_lvs_rqsts r ON r.lvs_rqst_id = s.lvs_rqst_id ");
            sb.Append("WHERE (s.lvs_rqst_id = @lvs_rqst_id) ");
            sb.Append("AND LOWER(s.to_emp_rl) = LOWER(@to_emp_rl) ");
            sb.Append("AND (s.sbm_purps = @sbm_purps) ");
            sb.Append("ORDER BY s.lvs_sbm_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var to_emp_rl = cmd.Parameters.Add("@to_emp_rl", NpgsqlDbType.Text);
                    var sbm_purps = cmd.Parameters.Add("@sbm_purps", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_rqst_id.Value = leaveRequestId;
                    to_emp_rl.Value = toEmployeeRole;
                    sbm_purps.Value = purpose;

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
                            DocumentType = reader["lvs_doc_typ"] == DBNull.Value ? string.Empty : reader["lvs_doc_typ"].ToString(),


                            LeavePlanYear = reader["lvs_yr"] == DBNull.Value ? 0 : (int)reader["lvs_yr"],
                            LeavePlanStartDate = reader["lvs_pln_sdt"] == DBNull.Value ? new DateTime(2020, 1, 1) : (DateTime)reader["lvs_pln_sdt"],
                            LeavePlanEndDate = reader["lvs_pln_edt"] == DBNull.Value ? new DateTime(2020, 1, 1) : (DateTime)reader["lvs_pln_edt"],
                            LeavePlanDurationDescription = reader["pln_dur_ds"] == DBNull.Value ? string.Empty : reader["pln_dur_ds"].ToString(),
                            LeavePlanResumptionDate = reader["pln_rsmptn_dt"] == DBNull.Value ? new DateTime(2020, 1, 1) : (DateTime)reader["pln_rsmptn_dt"],
                            LeavePlanEmployeeName = reader["pln_emp_nm"] == DBNull.Value ? string.Empty : reader["pln_emp_nm"].ToString(),
                            LeavePlanTypeName = reader["lvs_pln_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_pln_typ_nm"].ToString(),
                            LeavePlanUnitName = reader["pln_unit_nm"] == DBNull.Value ? string.Empty : reader["pln_unit_nm"].ToString(),
                            LeavePlanLocationName = reader["pln_loc_nm"] == DBNull.Value ? string.Empty : reader["pln_loc_nm"].ToString(),


                            LeaveRequestYear = reader["lvs_rqst_yr"] == DBNull.Value ? 0 : (int)reader["lvs_rqst_yr"],
                            LeaveRequestStartDate = reader["lvs_rqst_sdt"] == DBNull.Value ? new DateTime(2000, 1, 1) : (DateTime)reader["lvs_rqst_sdt"],
                            LeaveRequestEndDate = reader["lvs_rqst_edt"] == DBNull.Value ? new DateTime(2000, 1, 1) : (DateTime)reader["lvs_rqst_edt"],
                            LeaveRequestDurationDescription = reader["lvs_rqst_dur_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_dur_ds"].ToString(),
                            LeaveRequestResumptionDate = reader["rqst_rsmptn_dt"] == DBNull.Value ? new DateTime(2000, 1, 1) : (DateTime)reader["rqst_rsmptn_dt"],
                            LeaveRequestStatusDescription = reader["lvs_rqst_sts_ds"] == DBNull.Value ? string.Empty : reader["lvs_rqst_sts_ds"].ToString(),
                            LeaveRequestEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            LeaveRequestTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            LeaveRequestUnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),
                            LeaveRequestLocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return submissionList;
        }



        //==== Leave Submission Write Action Methods
        public async Task<bool> AddLeaveSubmissionAsync(LeaveSubmission e)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_sbms(lvs_pln_id, lvs_rqst_id, ");
            sb.Append("frm_emp_nm, to_emp_nm, sbm_purps, sbm_dt, sbm_msg, is_xtn, ");
            sb.Append("dt_xtn, to_emp_rl, lvs_doc_typ) VALUES (@lvs_pln_id, @lvs_rqst_id, ");
            sb.Append("@frm_emp_nm, @to_emp_nm, @sbm_purps, @sbm_dt, @sbm_msg, ");
            sb.Append("@is_xtn, @dt_xtn, @to_emp_rl, @lvs_doc_typ); ");

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
                    var lvs_doc_typ = cmd.Parameters.Add("@lvs_doc_typ", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pln_id.Value = e.LeavePlanId ?? (object)DBNull.Value;
                    lvs_rqst_id.Value = e.LeaveRequestId ?? (object)DBNull.Value;
                    frm_emp_nm.Value = e.FromEmployeeName;
                    to_emp_nm.Value = e.ToEmployeeName ?? (object)DBNull.Value;
                    sbm_purps.Value = e.Purpose ?? (object)DBNull.Value;
                    sbm_dt.Value = e.TimeSubmitted ?? DateTime.UtcNow;
                    sbm_msg.Value = e.Message ?? (object)DBNull.Value;
                    is_xtn.Value = e.IsActioned;
                    dt_xtn.Value = e.TimeActioned ?? (object)DBNull.Value;
                    to_emp_rl.Value = e.ToEmployeeRole;
                    lvs_doc_typ.Value = e.DocumentType ?? (object)DBNull.Value;
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

        #region Leave Resumptions Action Methods
        public async Task<long> AddLeaveResumptionAsync(LeaveResumption e)
        {
            long _newLeaveResumptionId = 0;
            string query = @"INSERT INTO public.lvm_lvs_rsmp(lvs_rqst_id, lvs_emp_nm, aprv_rsmp_dt, 
emp_rsmp_dt, emp_no_xtra_dys, emp_no_dys_rem, emp_rsn, emp_record_dt, ln_mgr_nm, lm_rsmp_dt, lm_no_xtra_dys, 
lm_no_dys_rem, lm_rsn, lm_record_dt, emp_rqs_adj, lm_apv_adj, rqs_adj_typ) VALUES (@lvs_rqst_id, @lvs_emp_nm, 
@aprv_rsmp_dt, @emp_rsmp_dt, @emp_no_xtra_dys, @emp_no_dys_rem, @emp_rsn, @emp_record_dt, @ln_mgr_nm, 
@lm_rsmp_dt, @lm_no_xtra_dys, @lm_no_dys_rem, @lm_rsn, @lm_record_dt, @emp_rqs_adj, @lm_apv_adj, @rqs_adj_typ) 
RETURNING lvs_rsmp_id;";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var lvs_emp_nm = cmd.Parameters.Add("@lvs_emp_nm", NpgsqlDbType.Text);
                    var aprv_rsmp_dt = cmd.Parameters.Add("@aprv_rsmp_dt", NpgsqlDbType.Date);
                    var emp_rsmp_dt = cmd.Parameters.Add("@emp_rsmp_dt", NpgsqlDbType.Date);
                    var emp_no_xtra_dys = cmd.Parameters.Add("@emp_no_xtra_dys", NpgsqlDbType.Integer);
                    var emp_no_dys_rem = cmd.Parameters.Add("@emp_no_dys_rem", NpgsqlDbType.Integer);
                    var emp_rsn = cmd.Parameters.Add("@emp_rsn", NpgsqlDbType.Text);
                    var emp_record_dt = cmd.Parameters.Add("@emp_record_dt", NpgsqlDbType.Timestamp);

                    var ln_mgr_nm = cmd.Parameters.Add("@ln_mgr_nm", NpgsqlDbType.Text);
                    var lm_rsmp_dt = cmd.Parameters.Add("@lm_rsmp_dt", NpgsqlDbType.Date);
                    var lm_no_xtra_dys = cmd.Parameters.Add("@lm_no_xtra_dys", NpgsqlDbType.Integer);
                    var lm_no_dys_rem = cmd.Parameters.Add("@lm_no_dys_rem", NpgsqlDbType.Integer);
                    var lm_rsn = cmd.Parameters.Add("@lm_rsn", NpgsqlDbType.Text);
                    var lm_record_dt = cmd.Parameters.Add("@lm_record_dt", NpgsqlDbType.Timestamp);

                    var emp_rqs_adj = cmd.Parameters.Add("@emp_rqs_adj", NpgsqlDbType.Boolean);
                    var lm_apv_adj = cmd.Parameters.Add("@lm_apv_adj", NpgsqlDbType.Boolean);
                    var rqs_adj_typ = cmd.Parameters.Add("@rqs_adj_typ", NpgsqlDbType.Text);

                    cmd.Prepare();

                    lvs_rqst_id.Value = e.LeaveRequestId;
                    lvs_emp_nm.Value = e.LeaveEmployeeName;
                    aprv_rsmp_dt.Value = e.ApprovedResumptionDate;
                    emp_rsmp_dt.Value = e.ResumptionDateByEmployee;
                    emp_no_xtra_dys.Value = e.NoOfExtraDaysByEmployee;
                    emp_no_dys_rem.Value = e.NoOfUnusedDaysByEmployee;
                    emp_rsn.Value = e.ReasonByEmployee ?? (object)DBNull.Value;
                    emp_record_dt.Value = e.DateRecordedByEmployee;

                    ln_mgr_nm.Value = e.LineManagerName ?? (object)DBNull.Value;
                    lm_rsmp_dt.Value = e.ResumptionDateByLineManager ?? (object)DBNull.Value;
                    lm_no_xtra_dys.Value = e.NoOfExtraDaysByLineManager;
                    lm_no_dys_rem.Value = e.NoOfUnusedDaysByLineManager;
                    lm_rsn.Value = e.ReasonByLineManager ?? (object)DBNull.Value;
                    lm_record_dt.Value = e.DateRecordedByLineManager ?? (object)DBNull.Value;

                    emp_rqs_adj.Value = e.EmployeeRequestAdjustment;
                    lm_apv_adj.Value = e.LineManagerApprovesAdjustment;
                    rqs_adj_typ.Value = e.RequestedAdjustmentType ?? (object)DBNull.Value;

                    var obj = await cmd.ExecuteScalarAsync();
                    _newLeaveResumptionId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return _newLeaveResumptionId;
        }
        public async Task<bool> DeleteLeaveResumptionAsync(long leaveResumptionId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_rsmp WHERE (lvs_rsmp_id = @lvs_rsmp_id); ";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rsmp_id = cmd.Parameters.Add("@lvs_rsmp_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_rsmp_id.Value = leaveResumptionId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeaveResumptionByLineManagerAsync(long leaveResumptionId, string lineManagerName, DateTime resumptionDateByLineManager, int noOfExtraDaysByLineManager, int noOfUnusedLeaveDaysByLineManager, string commentsByLineManager, bool approvesAdjustment)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_rsmp SET ln_mgr_nm=@ln_mgr_nm, lm_rsmp_dt=@lm_rsmp_dt, ");
            sb.Append("lm_no_xtra_dys=@lm_no_xtra_dys, lm_no_dys_rem=@lm_no_dys_rem, lm_rsn=@lm_rsn, ");
            sb.Append("lm_record_dt=@lm_record_dt, lm_apv_adj=@lm_apv_adj ");
            sb.Append("WHERE (lvs_rsmp_id = @lvs_rsmp_id); ");
            string query =  sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var ln_mgr_nm = cmd.Parameters.Add("@ln_mgr_nm", NpgsqlDbType.Text);
                    var lm_rsmp_dt = cmd.Parameters.Add("@lm_rsmp_dt", NpgsqlDbType.Date);
                    var lm_no_xtra_dys = cmd.Parameters.Add("@lm_no_xtra_dys", NpgsqlDbType.Integer);
                    var lm_no_dys_rem = cmd.Parameters.Add("@lm_no_dys_rem", NpgsqlDbType.Integer);
                    var lm_rsn = cmd.Parameters.Add("@lm_rsn", NpgsqlDbType.Text);
                    var lm_record_dt = cmd.Parameters.Add("@lm_record_dt", NpgsqlDbType.Timestamp);
                    var lvs_rsmp_id = cmd.Parameters.Add("@lvs_rsmp_id", NpgsqlDbType.Bigint);
                    var lm_apv_adj = cmd.Parameters.Add("@lm_apv_adj", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    ln_mgr_nm.Value = lineManagerName;
                    lm_rsmp_dt.Value = resumptionDateByLineManager.Date;
                    lm_no_xtra_dys.Value = noOfExtraDaysByLineManager;
                    lm_no_dys_rem.Value = noOfUnusedLeaveDaysByLineManager;
                    lm_rsn.Value = commentsByLineManager ?? (object)DBNull.Value;
                    lm_record_dt.Value = DateTime.UtcNow;
                    lvs_rsmp_id.Value = leaveResumptionId;
                    lm_apv_adj.Value = approvesAdjustment;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<LeaveResumption> GetLeaveResumptionByLeaveRequestIdAsync(long leaveRequestId)
        {
            LeaveResumption  leaveResumption = new LeaveResumption();

            string query = @"SELECT lvs_rsmp_id, lvs_rqst_id, lvs_emp_nm, aprv_rsmp_dt, emp_rsmp_dt, emp_no_xtra_dys, 
emp_no_dys_rem, emp_rsn, emp_record_dt, ln_mgr_nm, lm_rsmp_dt, lm_no_xtra_dys, lm_no_dys_rem, lm_rsn, lm_record_dt, 
emp_rqs_adj, lm_apv_adj, rqs_adj_typ FROM public.lvm_lvs_rsmp WHERE (lvs_rqst_id=@lvs_rqst_id); ";
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
                        leaveResumption.LeaveResumptionId = reader["lvs_rsmp_id"] == DBNull.Value ? 0L : (long)reader["lvs_rsmp_id"];
                        leaveResumption.LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"];
                        leaveResumption.LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString();
                        leaveResumption.ApprovedResumptionDate = reader["aprv_rsmp_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["aprv_rsmp_dt"];
                        leaveResumption.ResumptionDateByEmployee = reader["emp_rsmp_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["emp_rsmp_dt"];
                        leaveResumption.NoOfExtraDaysByEmployee = reader["emp_no_xtra_dys"] == DBNull.Value ? 0 : (int)reader["emp_no_xtra_dys"];
                        leaveResumption.NoOfUnusedDaysByEmployee = reader["emp_no_dys_rem"] == DBNull.Value ? 0 : (int)reader["emp_no_dys_rem"];
                        leaveResumption.ReasonByEmployee = reader["emp_rsn"] == DBNull.Value ? string.Empty : reader["emp_rsn"].ToString();
                        leaveResumption.DateRecordedByEmployee = reader["emp_record_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["emp_record_dt"];

                        leaveResumption.LineManagerName = reader["ln_mgr_nm"] == DBNull.Value ? string.Empty : reader["ln_mgr_nm"].ToString();
                        leaveResumption.ResumptionDateByLineManager = reader["lm_rsmp_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lm_rsmp_dt"];
                        leaveResumption.NoOfExtraDaysByLineManager = reader["lm_no_xtra_dys"] == DBNull.Value ? 0 : (int)reader["lm_no_xtra_dys"];
                        leaveResumption.NoOfUnusedDaysByLineManager = reader["lm_no_dys_rem"] == DBNull.Value ? 0 : (int)reader["lm_no_dys_rem"];
                        leaveResumption.ReasonByLineManager = reader["lm_rsn"] == DBNull.Value ? string.Empty : reader["lm_rsn"].ToString();
                        leaveResumption.DateRecordedByLineManager = reader["lm_record_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lm_record_dt"];

                        leaveResumption.EmployeeRequestAdjustment = reader["emp_rqs_adj"] == DBNull.Value ? false : (bool)reader["emp_rqs_adj"];
                        leaveResumption.LineManagerApprovesAdjustment = reader["lm_apv_adj"] == DBNull.Value ? false : (bool)reader["lm_apv_adj"];
                        leaveResumption.RequestedAdjustmentType = reader["rqs_adj_typ"] == DBNull.Value ? string.Empty : reader["rqs_adj_typ"].ToString();

                    }
                }
                await conn.CloseAsync();
            }
            return leaveResumption;
        }
        public async Task<LeaveResumption> GetLeaveResumptionByLeaveResumptionIdAsync(long leaveResumptionId)
        {
            LeaveResumption leaveResumption = new LeaveResumption();

            string query = @"SELECT lvs_rsmp_id, lvs_rqst_id, lvs_emp_nm, aprv_rsmp_dt, emp_rsmp_dt, emp_no_xtra_dys, 
emp_no_dys_rem, emp_rsn, emp_record_dt, ln_mgr_nm, lm_rsmp_dt, lm_no_xtra_dys, lm_no_dys_rem, lm_rsn, lm_record_dt,
emp_rqs_adj, lm_apv_adj, rqs_adj_typ FROM public.lvm_lvs_rsmp WHERE (lvs_rsmp_id=@lvs_rsmp_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rsmp_id = cmd.Parameters.Add("@lvs_rsmp_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_rsmp_id.Value = leaveResumptionId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        leaveResumption.LeaveResumptionId = reader["lvs_rsmp_id"] == DBNull.Value ? 0L : (long)reader["lvs_rsmp_id"];
                        leaveResumption.LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"];
                        leaveResumption.LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString();
                        leaveResumption.ApprovedResumptionDate = reader["aprv_rsmp_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["aprv_rsmp_dt"];
                        leaveResumption.ResumptionDateByEmployee = reader["emp_rsmp_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["emp_rsmp_dt"];
                        leaveResumption.NoOfExtraDaysByEmployee = reader["emp_no_xtra_dys"] == DBNull.Value ? 0 : (int)reader["emp_no_xtra_dys"];
                        leaveResumption.NoOfUnusedDaysByEmployee = reader["emp_no_dys_rem"] == DBNull.Value ? 0 : (int)reader["emp_no_dys_rem"];
                        leaveResumption.ReasonByEmployee = reader["emp_rsn"] == DBNull.Value ? string.Empty : reader["emp_rsn"].ToString();
                        leaveResumption.DateRecordedByEmployee = reader["emp_record_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["emp_record_dt"];

                        leaveResumption.LineManagerName = reader["ln_mgr_nm"] == DBNull.Value ? string.Empty : reader["ln_mgr_nm"].ToString();
                        leaveResumption.ResumptionDateByLineManager = reader["lm_rsmp_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lm_rsmp_dt"];
                        leaveResumption.NoOfExtraDaysByLineManager = reader["lm_no_xtra_dys"] == DBNull.Value ? 0 : (int)reader["lm_no_xtra_dys"];
                        leaveResumption.NoOfUnusedDaysByLineManager = reader["lm_no_dys_rem"] == DBNull.Value ? 0 : (int)reader["lm_no_dys_rem"];
                        leaveResumption.ReasonByLineManager = reader["lm_rsn"] == DBNull.Value ? string.Empty : reader["lm_rsn"].ToString();
                        leaveResumption.DateRecordedByLineManager = reader["lm_record_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lm_record_dt"];

                        leaveResumption.EmployeeRequestAdjustment = reader["emp_rqs_adj"] == DBNull.Value ? false : (bool)reader["emp_rqs_adj"];
                        leaveResumption.LineManagerApprovesAdjustment = reader["lm_apv_adj"] == DBNull.Value ? false : (bool)reader["lm_apv_adj"];
                        leaveResumption.RequestedAdjustmentType = reader["rqs_adj_typ"] == DBNull.Value ? string.Empty : reader["rqs_adj_typ"].ToString();
                    }
                }
                await conn.CloseAsync();
            }
            return leaveResumption;
        }
        #endregion

        #region Leave Documents Action Methods
        public async Task<long> AddLeaveDocumentAsync(LeaveDocument e)
        {
            long _newLeaveDocumentId = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_docs(doc_ref_pth, ");
            sb.Append("lvs_rqst_id, doc_title, doc_ful_pth, upl_dt, ");
            sb.Append("doc_desc) VALUES (@doc_ref_pth, @lvs_rqst_id, ");
            sb.Append("@doc_title, @doc_ful_pth, @upl_dt, @doc_desc) ");
            sb.Append("RETURNING lvs_doc_id;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqst_id = cmd.Parameters.Add("@lvs_rqst_id", NpgsqlDbType.Bigint);
                    var doc_title = cmd.Parameters.Add("@doc_title", NpgsqlDbType.Text);
                    var doc_desc = cmd.Parameters.Add("@doc_desc", NpgsqlDbType.Text);
                    var upl_dt = cmd.Parameters.Add("@upl_dt", NpgsqlDbType.Timestamp);
                    var doc_ref_pth = cmd.Parameters.Add("@doc_ref_pth", NpgsqlDbType.Text);
                    var doc_ful_pth = cmd.Parameters.Add("@doc_ful_pth", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_rqst_id.Value = e.LeaveRequestId;
                    doc_title.Value = e.DocumentTitle;
                    doc_desc.Value = e.DocumentDescription ?? (object)DBNull.Value;
                    upl_dt.Value = e.TimeUploaded ?? DateTime.Now;
                    doc_ref_pth.Value = e.DocumentReferencePath ?? (object)DBNull.Value;
                    doc_ful_pth.Value = e.DocumentFullPath ?? (object)DBNull.Value;

                    var obj = await cmd.ExecuteScalarAsync();
                    _newLeaveDocumentId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return _newLeaveDocumentId;
        }
        public async Task<bool> DeleteLeaveDocumentAsync(long leaveDocumentId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_docs WHERE (lvs_doc_id = @lvs_doc_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_doc_id = cmd.Parameters.Add("@lvs_doc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_doc_id.Value = leaveDocumentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<LeaveDocument> GetLeaveDocumentByIdAsync(long leaveDocumentId)
        {
            LeaveDocument document = new LeaveDocument();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_doc_id, doc_ref_pth, lvs_rqst_id, ");
            sb.Append("doc_title, doc_ful_pth, upl_dt, doc_desc ");
            sb.Append("FROM public.lvm_lvs_docs ");
            sb.Append("WHERE (lvs_doc_id = @lvs_doc_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_doc_id = cmd.Parameters.Add("@lvs_doc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_doc_id.Value = leaveDocumentId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        document.LeaveDocumentId = reader["lvs_doc_id"] == DBNull.Value ? 0L : (long)reader["lvs_doc_id"];
                        document.LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"];
                        document.DocumentTitle = reader["doc_title"] == DBNull.Value ? string.Empty : reader["doc_title"].ToString();
                        document.DocumentDescription = reader["doc_desc"] == DBNull.Value ? string.Empty : reader["doc_desc"].ToString();
                        document.DocumentReferencePath = reader["doc_ref_pth"] == DBNull.Value ? string.Empty : reader["doc_ref_pth"].ToString();
                        document.DocumentFullPath = reader["doc_ful_pth"] == DBNull.Value ? string.Empty : reader["doc_ful_pth"].ToString();
                        document.TimeUploaded = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"];
                    }
                }
                await conn.CloseAsync();
            }
            return document;
        }
        public async Task<List<LeaveDocument>> GetLeaveDocumentsByLeaveRequestIdAsync(long leaveRequestId)
        {
            List<LeaveDocument> documentsList = new List<LeaveDocument>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lvs_doc_id, doc_ref_pth, lvs_rqst_id, ");
            sb.Append("doc_title, doc_ful_pth, upl_dt, doc_desc ");
            sb.Append("FROM public.lvm_lvs_docs ");
            sb.Append("WHERE (lvs_rqst_id = @lvs_rqst_id) ");
            sb.Append("ORDER BY lvs_doc_id DESC; ");
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
                        documentsList.Add(new LeaveDocument()
                        {
                            LeaveDocumentId = reader["lvs_doc_id"] == DBNull.Value ? 0L : (long)reader["lvs_doc_id"],
                            LeaveRequestId = reader["lvs_rqst_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqst_id"],
                            DocumentTitle = reader["doc_title"] == DBNull.Value ? string.Empty : reader["doc_title"].ToString(),
                            DocumentDescription = reader["doc_desc"] == DBNull.Value ? string.Empty : reader["doc_desc"].ToString(),
                            DocumentReferencePath = reader["doc_ref_pth"] == DBNull.Value ? string.Empty : reader["doc_ref_pth"].ToString(),
                            DocumentFullPath = reader["doc_ful_pth"] == DBNull.Value ? string.Empty : reader["doc_ful_pth"].ToString(),
                            TimeUploaded = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return documentsList;
        }

        #endregion

        #region Leave Adjustment Action Methods
        public async Task<long> AddLeaveAdjustmentAsync(LeaveAdjustment e)
        {
            long _newLeaveAdjustmentId = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_adjs(lvs_emp_id, lvs_yr, ");
            sb.Append("lvs_typ_cd, no_wkg_dys, no_dys_des, lvs_adj_typ, ");
            sb.Append("lvs_adj_jus, lvs_adj_dt, lvs_adj_by, lvs_unit_id, ");
            sb.Append("lvs_dept_id, lvs_loc_id, lvs_rqs_id) ");
            sb.Append("VALUES (@lvs_emp_id, @lvs_yr, @lvs_typ_cd, @no_wkg_dys, ");
            sb.Append("@no_dys_des, @lvs_adj_typ, @lvs_adj_jus, @lvs_adj_dt, ");
            sb.Append("@lvs_adj_by, @lvs_unit_id, @lvs_dept_id, @lvs_loc_id, ");
            sb.Append("@lvs_rqs_id) RETURNING lvs_adj_id;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var no_wkg_dys = cmd.Parameters.Add("@no_wkg_dys", NpgsqlDbType.Integer);
                    var no_dys_des = cmd.Parameters.Add("@no_dys_des", NpgsqlDbType.Text);
                    var lvs_adj_typ = cmd.Parameters.Add("@lvs_adj_typ", NpgsqlDbType.Text);
                    var lvs_adj_jus = cmd.Parameters.Add("@lvs_adj_jus", NpgsqlDbType.Text);
                    var lvs_adj_dt = cmd.Parameters.Add("@lvs_adj_dt", NpgsqlDbType.Timestamp);
                    var lvs_adj_by = cmd.Parameters.Add("@lvs_adj_by", NpgsqlDbType.Text);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_dept_id = cmd.Parameters.Add("@lvs_dept_id", NpgsqlDbType.Integer);
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);

                    cmd.Prepare();

                    lvs_emp_id.Value = e.LeaveEmployeeId;
                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    no_wkg_dys.Value = e.NumberOfDays;
                    no_dys_des.Value = e.DurationDescription;
                    lvs_adj_typ.Value = e.AdjustmentType;
                    lvs_adj_jus.Value = e.AdjustmentJustification ?? (object)DBNull.Value;
                    lvs_adj_dt.Value = e.AdjustmentDate;
                    lvs_adj_by.Value = e.AdjustmentAddedBy;
                    lvs_unit_id.Value = e.LeaveUnitId;
                    lvs_dept_id.Value = e.LeaveDepartmentId;
                    lvs_loc_id.Value = e.LeaveLocationId;
                    lvs_rqs_id.Value = e.LeaveRequestId;

                    var obj = await cmd.ExecuteScalarAsync();
                    _newLeaveAdjustmentId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return _newLeaveAdjustmentId;
        }
        
        public async Task<bool> DeleteLeaveAdjustmentAsync(long leaveAdjustmentId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_adjs WHERE (lvs_adj_id = @lvs_adj_id); ";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_adj_id = cmd.Parameters.Add("@lvs_adj_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_adj_id.Value = leaveAdjustmentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<LeaveAdjustment> GetLeaveAdjustmentByIdAsync(long leaveAdjustmentId)
        {
            LeaveAdjustment adjustment = new LeaveAdjustment();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.lvs_adj_id, a.lvs_emp_id, a.lvs_yr, ");
            sb.Append("a.lvs_typ_cd, a.no_wkg_dys, a.no_dys_des, ");
            sb.Append("a.lvs_adj_typ, a.lvs_adj_jus, a.lvs_adj_dt, ");
            sb.Append("a.lvs_adj_by, a.lvs_unit_id, a.lvs_dept_id, ");
            sb.Append("a.lvs_loc_id, a.lvs_rqs_id, t.lvs_typ_nm ");
            sb.Append("FROM public.lvm_lvs_adjs a ");
            sb.Append("INNER JOIN public.lvm_lvs_typs t ");
            sb.Append("ON a.lvs_typ_cd = t.lvs_typ_cd ");
            sb.Append("WHERE (a.lvs_adj_id = @lvs_adj_id);  ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_adj_id = cmd.Parameters.Add("@lvs_adj_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_adj_id.Value = leaveAdjustmentId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        adjustment.LeaveAdjustmentId = reader["lvs_adj_id"] == DBNull.Value ? 0L : (long)reader["lvs_adj_id"];
                        adjustment.LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"];
                        adjustment.LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString();
                        adjustment.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        adjustment.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
                        adjustment.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                        adjustment.NumberOfDays = reader["no_wkg_dys"] == DBNull.Value ? 0 : (int)reader["no_wkg_dys"];
                        adjustment.DurationDescription = reader["no_dys_des"] == DBNull.Value ? string.Empty : reader["no_dys_des"].ToString();
                        adjustment.AdjustmentType = reader["lvs_adj_typ"] == DBNull.Value ? string.Empty : reader["lvs_adj_typ"].ToString();
                        adjustment.AdjustmentJustification = reader["lvs_adj_jus"] == DBNull.Value ? string.Empty : reader["lvs_adj_jus"].ToString();
                        adjustment.AdjustmentDate = reader["lvs_adj_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lvs_adj_dt"];
                        adjustment.AdjustmentAddedBy = reader["lvs_adj_by"] == DBNull.Value ? string.Empty : reader["lvs_adj_by"].ToString();
                        adjustment.LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"];
                        adjustment.LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"];
                        adjustment.LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"];
                    }
                }
                await conn.CloseAsync();
            }
            return adjustment;
        }
        public async Task<List<LeaveAdjustment>> GetLeaveAdjustmentsByLeaveRequestIdAsync(long leaveRequestId)
        {
            List<LeaveAdjustment> adjustmentsList = new List<LeaveAdjustment>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.lvs_adj_id, a.lvs_emp_id, a.lvs_yr, ");
            sb.Append("a.lvs_typ_cd, a.no_wkg_dys, a.no_dys_des, ");
            sb.Append("a.lvs_adj_typ, a.lvs_adj_jus, a.lvs_adj_dt, ");
            sb.Append("a.lvs_adj_by, a.lvs_unit_id, a.lvs_dept_id, ");
            sb.Append("a.lvs_loc_id, a.lvs_rqs_id, t.lvs_typ_nm ");
            sb.Append("FROM public.lvm_lvs_adjs a ");
            sb.Append("INNER JOIN public.lvm_lvs_typs t ");
            sb.Append("ON a.lvs_typ_cd = t.lvs_typ_cd ");
            sb.Append("WHERE (a.lvs_rqs_id = @lvs_rqs_id) ");
            sb.Append("ORDER BY lvs_adj_id DESC; ");
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
                        adjustmentsList.Add(new LeaveAdjustment()
                        {
                            LeaveAdjustmentId = reader["lvs_adj_id"] == DBNull.Value ? 0L : (long)reader["lvs_adj_id"],
                            LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString(),
                            LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString(),
                            NumberOfDays = reader["no_wkg_dys"] == DBNull.Value ? 0 : (int)reader["no_wkg_dys"],
                            DurationDescription = reader["no_dys_des"] == DBNull.Value ? string.Empty : reader["no_dys_des"].ToString(),
                            AdjustmentType = reader["lvs_adj_typ"] == DBNull.Value ? string.Empty : reader["lvs_adj_typ"].ToString(),
                            AdjustmentJustification = reader["lvs_adj_jus"] == DBNull.Value ? string.Empty : reader["lvs_adj_jus"].ToString(),
                            AdjustmentDate = reader["lvs_adj_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lvs_adj_dt"],
                            AdjustmentAddedBy = reader["lvs_adj_by"] == DBNull.Value ? string.Empty : reader["lvs_adj_by"].ToString(),
                            LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return adjustmentsList;
        }

        #endregion

        #region Leave Allowanace Action Methods
        public async Task<long> AddLeaveAllowanceAsync(LeaveAllowance e)
        {
            long _newLeaveAllowanceId = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_allws(lvs_rqs_id, lvs_emp_id, ");
            sb.Append("lvs_yr, pymnt_yr, pymnt_mn, allw_rqst_dt, rqst_is_aprv, ");
            sb.Append("lvs_unit_id, lvs_dept_id, lvs_loc_id, recorded_dt, ");
            sb.Append("recorded_by) VALUES (@lvs_rqs_id, @lvs_emp_id, @lvs_yr, ");
            sb.Append("@pymnt_yr, @pymnt_mn, @allw_rqst_dt, @rqst_is_aprv, ");
            sb.Append("@lvs_unit_id, @lvs_dept_id, @lvs_loc_id, @recorded_dt, ");
            sb.Append("@recorded_by) RETURNING lvs_allw_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var pymnt_yr = cmd.Parameters.Add("@pymnt_yr", NpgsqlDbType.Integer);
                    var pymnt_mn = cmd.Parameters.Add("@pymnt_mn", NpgsqlDbType.Integer);
                    var allw_rqst_dt = cmd.Parameters.Add("@allw_rqst_dt", NpgsqlDbType.Timestamp);
                    var rqst_is_aprv = cmd.Parameters.Add("@rqst_is_aprv", NpgsqlDbType.Boolean);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_dept_id = cmd.Parameters.Add("@lvs_dept_id", NpgsqlDbType.Integer);
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var recorded_dt = cmd.Parameters.Add("@recorded_dt", NpgsqlDbType.Timestamp);
                    var recorded_by = cmd.Parameters.Add("@recorded_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_rqs_id.Value = e.LeaveRequestId;
                    lvs_emp_id.Value = e.LeaveEmployeeId;
                    lvs_yr.Value = e.LeaveYear;
                    pymnt_yr.Value = e.PaymentYear;
                    pymnt_mn.Value = e.PaymentMonth;
                    allw_rqst_dt.Value = e.RequestedTime;
                    rqst_is_aprv.Value = e.IsApproved;
                    lvs_unit_id.Value = e.LeaveUnitId;
                    lvs_dept_id.Value = e.LeaveDepartmentId;
                    lvs_loc_id.Value = e.LeaveLocationId;
                    recorded_dt.Value = e.RecordedTime;
                    recorded_by.Value = e.RecordedBy;

                    var obj = await cmd.ExecuteScalarAsync();
                    _newLeaveAllowanceId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return _newLeaveAllowanceId;
        }
        public async Task<bool> DeleteLeaveAllowanceAsync(long leaveAllowanceId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_allws WHERE (lvs_allw_id = @lvs_allw_id); ";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_allw_id = cmd.Parameters.Add("@lvs_allw_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_allw_id.Value = leaveAllowanceId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<LeaveAllowance> GetLeaveAllowanceByIdAsync(long leaveAllowanceId)
        {
            LeaveAllowance allowance = new LeaveAllowance();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.lvs_allw_id, a.lvs_rqs_id, a.lvs_emp_id, a.lvs_yr, a.pymnt_yr, ");
            sb.Append("a.pymnt_mn, a.allw_rqst_dt, a.rqst_is_aprv, a.lvs_unit_id, a.lvs_dept_id, ");
            sb.Append("a.lvs_loc_id, a.recorded_dt, a.recorded_by, ");
            sb.Append("SELECT fullname FROM public.gst_prsns WHERE (id=a.lvs_emp_id) as lvs_emp_nm, ");
            sb.Append("SELECT locname FROM public.gst_locs WHERE (locqk=a.lvs_unit_id) as lvs_unit_nm, ");
            sb.Append("SELECT deptname FROM public.gst_depts WHERE (deptqk=a.lvs_dept_id) as lvs_dept_nm, ");
            sb.Append("SELECT locname FROM public.gst_locs WHERE (locqk = a.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_allws a ");
            sb.Append("WHERE (a.lvs_allw_id = @lvs_allw_id);  ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_allw_id = cmd.Parameters.Add("@lvs_allw_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_allw_id.Value = leaveAllowanceId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        allowance.LeaveAllowanceId = reader["lvs_allw_id"] == DBNull.Value ? 0L : (long)reader["lvs_allw_id"];
                        allowance.LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"];
                        allowance.LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString();
                        allowance.LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString();
                        allowance.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        allowance.PaymentYear = reader["pymnt_yr"] == DBNull.Value ? 1900 : (int)reader["pymnt_yr"];
                        allowance.PaymentMonth = reader["pymnt_mn"] == DBNull.Value ? 1 : (int)reader["pymnt_mn"];

                        allowance.RequestedTime = reader["allw_rqst_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["allw_rqst_dt"];
                        allowance.IsApproved = reader["rqst_is_aprv"] == DBNull.Value ? false : (bool)reader["rqst_is_aprv"];

                        allowance.LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"];
                        allowance.LeaveUnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString();


                        allowance.LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"];
                        allowance.LeaveDepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString();

                        allowance.LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"];
                        allowance.LeaveLocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString();

                        allowance.RecordedTime = reader["recorded_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["recorded_dt"];
                        allowance.RecordedBy = reader["recorded_by"] == DBNull.Value ? string.Empty : reader["recorded_by"].ToString();

                    }
                }
                await conn.CloseAsync();
            }
            return allowance;
        }
        public async Task<List<LeaveAllowance>> GetLeaveAllowanceByLeaveRequestIdAsync(long leaveRequestId)
        {
            List<LeaveAllowance> allowancesList = new List<LeaveAllowance>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.lvs_allw_id, a.lvs_rqs_id, a.lvs_emp_id, a.lvs_yr, a.pymnt_yr, ");
            sb.Append("a.pymnt_mn, a.allw_rqst_dt, a.rqst_is_aprv, a.lvs_unit_id, a.lvs_dept_id, ");
            sb.Append("a.lvs_loc_id, a.recorded_dt, a.recorded_by, ");
            sb.Append("SELECT fullname FROM public.gst_prsns WHERE (id=a.lvs_emp_id) as lvs_emp_nm, ");
            sb.Append("SELECT locname FROM public.gst_locs WHERE (locqk=a.lvs_unit_id) as lvs_unit_nm, ");
            sb.Append("SELECT deptname FROM public.gst_depts WHERE (deptqk=a.lvs_dept_id) as lvs_dept_nm, ");
            sb.Append("SELECT locname FROM public.gst_locs WHERE (locqk = a.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_allws a ");
            sb.Append("WHERE (a.lvs_rqs_id = @lvs_rqs_id)  ");
            sb.Append("ORDER BY lvs_allw_id; ");

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
                        allowancesList.Add(new LeaveAllowance()
                        {
                            LeaveAllowanceId = reader["lvs_allw_id"] == DBNull.Value ? 0L : (long)reader["lvs_allw_id"],
                            LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            PaymentYear = reader["pymnt_yr"] == DBNull.Value ? 1900 : (int)reader["pymnt_yr"],
                            PaymentMonth = reader["pymnt_mn"] == DBNull.Value ? 1 : (int)reader["pymnt_mn"],

                            RequestedTime = reader["allw_rqst_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["allw_rqst_dt"],
                            IsApproved = reader["rqst_is_aprv"] == DBNull.Value ? false : (bool)reader["rqst_is_aprv"],

                            LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            LeaveUnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),

                            LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            LeaveDepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString(),

                            LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                            LeaveLocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                            RecordedTime = reader["recorded_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["recorded_dt"],
                            RecordedBy = reader["recorded_by"] == DBNull.Value ? string.Empty : reader["recorded_by"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return allowancesList;
        }
        public async Task<List<LeaveAllowance>> GetLeaveAllowanceByEmployeeIdnLeaveYearAsync(string employeeId, int leaveYear)
        {
            List<LeaveAllowance> allowancesList = new List<LeaveAllowance>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.lvs_allw_id, a.lvs_rqs_id, a.lvs_emp_id, a.lvs_yr, a.pymnt_yr, ");
            sb.Append("a.pymnt_mn, a.allw_rqst_dt, a.rqst_is_aprv, a.lvs_unit_id, a.lvs_dept_id, ");
            sb.Append("a.lvs_loc_id, a.recorded_dt, a.recorded_by, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id=a.lvs_emp_id) as lvs_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk=a.lvs_unit_id) as lvs_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk=a.lvs_dept_id) as lvs_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = a.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_allws a ");
            sb.Append("WHERE (a.lvs_emp_id = @lvs_emp_id)  ");
            sb.Append("AND (a.lvs_yr = @lvs_yr) ");
            sb.Append("ORDER BY a.lvs_allw_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = employeeId;
                    lvs_yr.Value = leaveYear;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        allowancesList.Add(new LeaveAllowance()
                        {
                            LeaveAllowanceId = reader["lvs_allw_id"] == DBNull.Value ? 0L : (long)reader["lvs_allw_id"],
                            LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"],
                            LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString(),
                            LeaveEmployeeName = reader["lvs_emp_nm"] == DBNull.Value ? string.Empty : reader["lvs_emp_nm"].ToString(),
                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],
                            PaymentYear = reader["pymnt_yr"] == DBNull.Value ? 1900 : (int)reader["pymnt_yr"],
                            PaymentMonth = reader["pymnt_mn"] == DBNull.Value ? 1 : (int)reader["pymnt_mn"],

                            RequestedTime = reader["allw_rqst_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["allw_rqst_dt"],
                            IsApproved = reader["rqst_is_aprv"] == DBNull.Value ? false : (bool)reader["rqst_is_aprv"],

                            LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"],
                            LeaveUnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString(),

                            LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"],
                            LeaveDepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString(),

                            LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"],
                            LeaveLocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString(),

                            RecordedTime = reader["recorded_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["recorded_dt"],
                            RecordedBy = reader["recorded_by"] == DBNull.Value ? string.Empty : reader["recorded_by"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return allowancesList;
        }

        #endregion

        #region Leave Transactions Action Methods
        #region Leave Transactions Write Action Methods
        public async Task<long> AddLeaveTransactionAsync(LeaveTransaction t)
        {
            long newTransactionId = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO public.lvm_lvs_trnx(lvs_emp_id, lvs_yr, ");
            sb.Append("lvs_typ_cd, no_dys_usd, no_dys_gvn, lvs_trnx_ds, ");
            sb.Append("lvs_trnx_dt, lvs_trnx_by, lvs_unit_id, lvs_dept_id, ");
            sb.Append("lvs_loc_id, lvs_rqs_id, lvs_adj_id, lvs_opn_blc, ");
            sb.Append("lvs_prv_blc) VALUES (@lvs_emp_id, @lvs_yr, @lvs_typ_cd, ");
            sb.Append("@no_dys_usd, @no_dys_gvn, @lvs_trnx_ds, @lvs_trnx_dt, ");
            sb.Append("@lvs_trnx_by, @lvs_unit_id, @lvs_dept_id, @lvs_loc_id, ");
            sb.Append("@lvs_rqs_id, @lvs_adj_id, @lvs_opn_blc, @lvs_prv_blc) ");
            sb.Append("RETURNING lvs_trnx_id; ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var no_dys_usd = cmd.Parameters.Add("@no_dys_usd", NpgsqlDbType.Integer);
                    var no_dys_gvn = cmd.Parameters.Add("@no_dys_gvn", NpgsqlDbType.Integer);
                    var lvs_trnx_ds = cmd.Parameters.Add("@lvs_trnx_ds", NpgsqlDbType.Text);
                    var lvs_trnx_dt = cmd.Parameters.Add("@lvs_trnx_dt", NpgsqlDbType.Timestamp);
                    var lvs_trnx_by = cmd.Parameters.Add("@lvs_trnx_by", NpgsqlDbType.Text);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_dept_id = cmd.Parameters.Add("@lvs_dept_id", NpgsqlDbType.Integer);
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_rqs_id = cmd.Parameters.Add("@lvs_rqs_id", NpgsqlDbType.Bigint);
                    var lvs_adj_id = cmd.Parameters.Add("@lvs_adj_id", NpgsqlDbType.Bigint);
                    var lvs_opn_blc = cmd.Parameters.Add("@lvs_opn_blc", NpgsqlDbType.Bigint);
                    var lvs_prv_blc = cmd.Parameters.Add("@lvs_prv_blc", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_emp_id.Value = t.LeaveEmployeeId;
                    lvs_yr.Value = t.LeaveYear;
                    lvs_typ_cd.Value = t.LeaveTypeCode;
                    no_dys_usd.Value = t.NumberOfDaysUsed;
                    no_dys_gvn.Value = t.NumberOfDaysGiven;
                    lvs_trnx_ds.Value = t.TransactionDescription ?? (object)DBNull.Value;
                    lvs_trnx_dt.Value = t.TransactionDate ?? DateTime.UtcNow;
                    lvs_trnx_by.Value = t.TransactionRecordedBy ?? "System Service";
                    lvs_unit_id.Value = t.LeaveUnitId;
                    lvs_dept_id.Value = t.LeaveDepartmentId;
                    lvs_loc_id.Value = t.LeaveLocationId;
                    lvs_rqs_id.Value = t.LeaveRequestId ?? (object)DBNull.Value;
                    lvs_adj_id.Value = t.LeaveAdjustmentId ?? (object)DBNull.Value;
                    lvs_opn_blc.Value = t.OpeningBalance;
                    lvs_prv_blc.Value = t.PreviousBalance;

                    var obj = await cmd.ExecuteScalarAsync();
                    newTransactionId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return newTransactionId;
        }
        public async Task<bool> DeleteLeaveTransactionAsync(long leaveTransactionId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_trnx WHERE (lvs_trnx_id = @lvs_trnx_id); ";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_trnx_id = cmd.Parameters.Add("@lvs_trnx_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_trnx_id.Value = leaveTransactionId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> DeleteLeaveTransactionByLeaveAdjustmentIdAsync(long leaveAdjustmentId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_trnx WHERE (lvs_adj_id = @lvs_adj_id); ";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_adj_id = cmd.Parameters.Add("@lvs_adj_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_adj_id.Value = leaveAdjustmentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }


        #endregion
        #region Leave Transactions Read Action Methods
        public async Task<LeaveTransaction> GetLeaveTransactionByIdAsync(long leaveTransactionId)
        {
            LeaveTransaction transaction = new LeaveTransaction();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.lvs_trnx_id, t.lvs_emp_id, t.lvs_yr, t.lvs_typ_cd, t.no_dys_usd, t.no_dys_gvn, ");
            sb.Append("t.lvs_trnx_ds, t.lvs_trnx_dt, t.lvs_trnx_by, t.lvs_unit_id, t.lvs_dept_id, t.lvs_loc_id, ");
            sb.Append("t.lvs_rqs_id, t.lvs_adj_id, t.lvs_opn_blc, t.lvs_prv_blc, t.no_dys_ded, p.lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_trnx t INNER JOIN public.lvm_lvs_typs p ON t.lvs_typ_cd = p.lvs_typ_cd ");
            sb.Append("WHERE (t.lvs_trnx_id = @lvs_trnx_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_trnx_id = cmd.Parameters.Add("@lvs_trnx_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_trnx_id.Value = leaveTransactionId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        transaction.LeaveTransactionId = reader["lvs_trnx_id"] == DBNull.Value ? 0L : (long)reader["lvs_trnx_id"];
                        transaction.LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString();
                        transaction.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        transaction.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
                        transaction.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();

                        transaction.NumberOfDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0 : (int)reader["no_dys_usd"];
                        transaction.NumberOfDaysGiven = reader["no_dys_gvn"] == DBNull.Value ? 0 : (int)reader["no_dys_gvn"];
                        transaction.NumberOfDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0 : (int)reader["no_dys_ded"];
                        transaction.TransactionDescription = reader["lvs_trnx_ds"] == DBNull.Value ? string.Empty : reader["lvs_trnx_ds"].ToString();
                        transaction.TransactionDate = reader["lvs_trnx_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lvs_trnx_dt"];
                        transaction.TransactionRecordedBy = reader["lvs_trnx_by"] == DBNull.Value ? string.Empty : reader["lvs_trnx_by"].ToString();
                        
                        transaction.LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"];
                        transaction.LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"];
                        transaction.LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"];

                        transaction.LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"];
                        transaction.LeaveAdjustmentId = reader["lvs_adj_id"] == DBNull.Value ? 0L : (long)reader["lvs_adj_id"];
                        transaction.OpeningBalance = reader["lvs_opn_blc"] == DBNull.Value ? 0 : (int)reader["lvs_opn_blc"];
                        transaction.PreviousBalance = reader["lvs_prv_blc"] == DBNull.Value ? 0 : (int)reader["lvs_prv_blc"];
                    }
                }
                await conn.CloseAsync();
            }
            return transaction;
        }
        public async Task<LeaveTransaction> GetLeaveTransactionByAdjustmentIdAsync(long leaveAdjustmentId)
        {
            LeaveTransaction transaction = new LeaveTransaction();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.lvs_trnx_id, t.lvs_emp_id, t.lvs_yr, t.lvs_typ_cd, t.no_dys_usd, t.no_dys_gvn, ");
            sb.Append("t.lvs_trnx_ds, t.lvs_trnx_dt, t.lvs_trnx_by, t.lvs_unit_id, t.lvs_dept_id, t.lvs_loc_id, ");
            sb.Append("t.lvs_rqs_id, t.lvs_adj_id, t.lvs_opn_blc, t.lvs_prv_blc, t.no_dys_ded, p.lvs_typ_nm  ");
            sb.Append("FROM public.lvm_lvs_trnx t INNER JOIN public.lvm_lvs_typs p ON t.lvs_typ_cd = p.lvs_typ_cd ");
            sb.Append("WHERE (t.lvs_adj_id = @lvs_adj_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_adj_id = cmd.Parameters.Add("@lvs_adj_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_adj_id.Value = leaveAdjustmentId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        transaction.LeaveTransactionId = reader["lvs_trnx_id"] == DBNull.Value ? 0L : (long)reader["lvs_trnx_id"];
                        transaction.LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString();
                        transaction.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        transaction.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
                        transaction.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();

                        transaction.NumberOfDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0 : (int)reader["no_dys_usd"];
                        transaction.NumberOfDaysGiven = reader["no_dys_gvn"] == DBNull.Value ? 0 : (int)reader["no_dys_gvn"];
                        transaction.NumberOfDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0 : (int)reader["no_dys_ded"];
                        transaction.TransactionDescription = reader["lvs_trnx_ds"] == DBNull.Value ? string.Empty : reader["lvs_trnx_ds"].ToString();
                        transaction.TransactionDate = reader["lvs_trnx_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lvs_trnx_dt"];
                        transaction.TransactionRecordedBy = reader["lvs_trnx_by"] == DBNull.Value ? string.Empty : reader["lvs_trnx_by"].ToString();

                        transaction.LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"];
                        transaction.LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"];
                        transaction.LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"];

                        transaction.LeaveRequestId = reader["lvs_rqs_id"] == DBNull.Value ? 0L : (long)reader["lvs_rqs_id"];
                        transaction.LeaveAdjustmentId = reader["lvs_adj_id"] == DBNull.Value ? 0L : (long)reader["lvs_adj_id"];
                        transaction.OpeningBalance = reader["lvs_opn_blc"] == DBNull.Value ? 0 : (int)reader["lvs_opn_blc"];
                        transaction.PreviousBalance = reader["lvs_prv_blc"] == DBNull.Value ? 0 : (int)reader["lvs_prv_blc"];
                    }
                }
                await conn.CloseAsync();
            }
            return transaction;
        }

        #endregion
        #endregion

        #region Leave Rolling Balances Action Methods
        #region Leave Rolling Balances Write Methods
        public async Task<long> AddLeaveRollingBalanceAsync(LeaveRollingBalance t)
        {
            long newBalanceId = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_blnx(lvs_emp_id, lvs_yr, lvs_typ_cd, no_dys_usd, ");
            sb.Append("no_dys_gvn, no_dys_ded, lvs_blnx_dt, lvs_unit_id, lvs_dept_id, lvs_loc_id, ");
            sb.Append("lvs_opn_blc, lvs_prv_blc, bf_prev_blc, prev_blc_xpr, lvs_trnx_id) ");
            sb.Append("VALUES (@lvs_emp_id, @lvs_yr, @lvs_typ_cd, @no_dys_usd, @no_dys_gvn, @no_dys_ded, ");
            sb.Append("@lvs_blnx_dt, @lvs_unit_id, @lvs_dept_id, @lvs_loc_id, @lvs_opn_blc, @lvs_prv_blc, ");
            sb.Append("@bf_prev_blc, @prev_blc_xpr, @lvs_trnx_id) RETURNING lvs_blnx_id; ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var no_dys_usd = cmd.Parameters.Add("@no_dys_usd", NpgsqlDbType.Integer);
                    var no_dys_gvn = cmd.Parameters.Add("@no_dys_gvn", NpgsqlDbType.Integer);
                    var no_dys_ded = cmd.Parameters.Add("@no_dys_ded", NpgsqlDbType.Integer);
                    var lvs_blnx_dt = cmd.Parameters.Add("@lvs_blnx_dt", NpgsqlDbType.Timestamp);
                    var lvs_unit_id = cmd.Parameters.Add("@lvs_unit_id", NpgsqlDbType.Integer);
                    var lvs_dept_id = cmd.Parameters.Add("@lvs_dept_id", NpgsqlDbType.Integer);
                    var lvs_loc_id = cmd.Parameters.Add("@lvs_loc_id", NpgsqlDbType.Integer);
                    var lvs_opn_blc = cmd.Parameters.Add("@lvs_opn_blc", NpgsqlDbType.Bigint);
                    var lvs_prv_blc = cmd.Parameters.Add("@lvs_prv_blc", NpgsqlDbType.Bigint);
                    var bf_prev_blc = cmd.Parameters.Add("@bf_prev_blc", NpgsqlDbType.Boolean);
                    var prev_blc_xpr = cmd.Parameters.Add("@prev_blc_xpr", NpgsqlDbType.Integer);
                    var lvs_trnx_id = cmd.Parameters.Add("@lvs_trnx_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_emp_id.Value = t.LeaveEmployeeId;
                    lvs_yr.Value = t.LeaveYear;
                    lvs_typ_cd.Value = t.LeaveTypeCode;
                    no_dys_usd.Value = t.LeaveDaysUsed;
                    no_dys_gvn.Value = t.LeaveDaysAdded;
                    no_dys_ded.Value = t.LeaveDaysDeducted;
                    lvs_blnx_dt.Value = t.LeaveBalanceDate;
                    lvs_unit_id.Value = t.LeaveUnitId;
                    lvs_dept_id.Value = t.LeaveDepartmentId;
                    lvs_loc_id.Value = t.LeaveLocationId;
                    lvs_opn_blc.Value = t.AnnualProfileLeaveDays;
                    lvs_prv_blc.Value = t.PreviousYearsLeaveBalance;
                    bf_prev_blc.Value = t.PreviousBalanceCanBeCarriedOver;
                    prev_blc_xpr.Value = t.PreviousBalanceExpiryMonth;
                    lvs_trnx_id.Value = t.LeaveTransactionId ?? (object)DBNull.Value;

                    var obj = await cmd.ExecuteScalarAsync();
                    newBalanceId = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return newBalanceId;
        }
        public async Task<bool> UpdateLeaveRollingBalanceAsync(LeaveRollingBalance t)
        {
            long noOfRowsUpdated = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_blnx SET no_dys_usd=@no_dys_usd, no_dys_gvn=@no_dys_gvn, ");
            sb.Append("no_dys_ded=@no_dys_ded, lvs_blnx_dt=@lvs_blnx_dt, lvs_opn_blc=@lvs_opn_blc, ");
            sb.Append("lvs_prv_blc=@lvs_prv_blc, bf_prev_blc=@bf_prev_blc, prev_blc_xpr=@prev_blc_xpr, ");
            sb.Append("lvs_trnx_id=@lvs_trnx_id ");
            sb.Append("WHERE (lvs_emp_id=@lvs_emp_id AND lvs_yr=@lvs_yr AND lvs_typ_cd=@lvs_typ_cd)  ");

            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var no_dys_usd = cmd.Parameters.Add("@no_dys_usd", NpgsqlDbType.Integer);
                    var no_dys_gvn = cmd.Parameters.Add("@no_dys_gvn", NpgsqlDbType.Integer);
                    var no_dys_ded = cmd.Parameters.Add("@no_dys_ded", NpgsqlDbType.Integer);
                    var lvs_blnx_dt = cmd.Parameters.Add("@lvs_blnx_dt", NpgsqlDbType.Timestamp);
                    var lvs_opn_blc = cmd.Parameters.Add("@lvs_opn_blc", NpgsqlDbType.Bigint);
                    var lvs_prv_blc = cmd.Parameters.Add("@lvs_prv_blc", NpgsqlDbType.Bigint);
                    var bf_prev_blc = cmd.Parameters.Add("@bf_prev_blc", NpgsqlDbType.Boolean);
                    var prev_blc_xpr = cmd.Parameters.Add("@prev_blc_xpr", NpgsqlDbType.Integer);
                    var lvs_trnx_id = cmd.Parameters.Add("@lvs_trnx_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_emp_id.Value = t.LeaveEmployeeId;
                    lvs_yr.Value = t.LeaveYear;
                    lvs_typ_cd.Value = t.LeaveTypeCode;
                    no_dys_usd.Value = t.LeaveDaysUsed;
                    no_dys_gvn.Value = t.LeaveDaysAdded;
                    no_dys_ded.Value = t.LeaveDaysDeducted;
                    lvs_blnx_dt.Value = t.LeaveBalanceDate;
                    lvs_opn_blc.Value = t.AnnualProfileLeaveDays;
                    lvs_prv_blc.Value = t.PreviousYearsLeaveBalance;
                    bf_prev_blc.Value = t.PreviousBalanceCanBeCarriedOver;
                    prev_blc_xpr.Value = t.PreviousBalanceExpiryMonth;
                    lvs_trnx_id.Value = t.LeaveTransactionId ?? (object)DBNull.Value;

                    var obj = await cmd.ExecuteNonQueryAsync();
                    noOfRowsUpdated = (long)obj;
                    await conn.CloseAsync();
                }
            }
            return noOfRowsUpdated > 0;
        }
        public async Task<bool> DeleteLeaveRollingBalanceAsync(long leaveRollingBalanceId)
        {
            int rows = 0;
            string query = "DELETE FROM public.lvm_lvs_blnx WHERE (lvs_blnx_id = @lvs_blnx_id); ";

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_blnx_id = cmd.Parameters.Add("@lvs_blnx_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    lvs_blnx_id.Value = leaveRollingBalanceId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        #endregion

        #region Leave Rolling Balances Read Methods
        public async Task<LeaveRollingBalance> GetLeaveRollingBalanceByEmployeeIdAsync(string leaveEmployeeId, int leaveYear, string leaveTypeCode)
        {
            LeaveRollingBalance rollingBalance = new LeaveRollingBalance();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.lvs_blnx_id, b.lvs_emp_id, b.lvs_yr, b.lvs_typ_cd, b.no_dys_usd, b.no_dys_gvn, ");
            sb.Append("b.no_dys_ded, b.lvs_blnx_dt, b.lvs_unit_id, b.lvs_dept_id, b.lvs_loc_id, b.lvs_opn_blc, ");
            sb.Append("b.lvs_prv_blc, b.bf_prev_blc, b.prev_blc_xpr, b.lvs_trnx_id, t.lvs_typ_nm, ");
            sb.Append("COALESCE(b.lvs_opn_blc, 0) + COALESCE(b.lvs_prv_blc, 0) + COALESCE(b.no_dys_gvn, 0) - ");
            sb.Append("COALESCE(b.no_dys_usd, 0) -  COALESCE(b.no_dys_ded, 0) AS out_bf_xpr, ");
            sb.Append("COALESCE(b.lvs_opn_blc, 0) + COALESCE(b.no_dys_gvn, 0) - COALESCE(b.no_dys_usd, 0) - ");
            sb.Append("COALESCE(b.no_dys_ded, 0) AS out_aft_xpr, ");
            sb.Append("CASE WHEN prev_blc_xpr = 1 THEN 'January' WHEN prev_blc_xpr = 2 THEN 'February' ");
            sb.Append("WHEN prev_blc_xpr = 3 THEN 'March' WHEN prev_blc_xpr = 4 THEN 'April' ");
            sb.Append("WHEN prev_blc_xpr = 5 THEN 'May' WHEN prev_blc_xpr = 6 THEN 'June' ");
            sb.Append("WHEN prev_blc_xpr = 7 THEN 'July' WHEN prev_blc_xpr = 8 THEN 'August' ");
            sb.Append("WHEN prev_blc_xpr = 9 THEN 'September' WHEN prev_blc_xpr = 10 THEN 'October' ");
            sb.Append("WHEN prev_blc_xpr = 11 THEN 'November' WHEN prev_blc_xpr = 12 THEN 'December' ");
            sb.Append("ELSE '' END AS prev_blc_xpr_mn, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = b.lvs_emp_id) as lvs_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = b.lvs_unit_id) as lvs_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = b.lvs_dept_id) as lvs_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_blnx b ");
            sb.Append("INNER JOIN public.lvm_lvs_typs t ON b.lvs_typ_cd = t.lvs_typ_cd ");
            sb.Append("WHERE (b.lvs_emp_id = @lvs_emp_id) AND (b.lvs_yr = @lvs_yr) ");
            sb.Append("AND (b.lvs_typ_cd=@lvs_typ_cd) ");
            sb.Append("ORDER BY b.lvs_blnx_id DESC LIMIT 1;  ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_emp_id = cmd.Parameters.Add("@lvs_emp_id", NpgsqlDbType.Text);
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_emp_id.Value = leaveEmployeeId;
                    lvs_yr.Value = leaveYear;
                    lvs_typ_cd.Value = leaveTypeCode;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        rollingBalance.RollingBalanceId = reader["lvs_blnx_id"] == DBNull.Value ? 0L : (long)reader["lvs_blnx_id"];
                        rollingBalance.LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString();
                        rollingBalance.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        rollingBalance.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
                        rollingBalance.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                        rollingBalance.LeaveDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0L : (long)reader["no_dys_usd"];
                        rollingBalance.LeaveDaysAdded = reader["no_dys_gvn"] == DBNull.Value ? 0L : (long)reader["no_dys_gvn"];
                        rollingBalance.LeaveDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0L : (long)reader["no_dys_ded"];
                        
                        rollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = reader["out_bf_xpr"] == DBNull.Value ? 0L : (long)reader["out_bf_xpr"];
                        rollingBalance.TotalOutstandingLeaveDaysAfterExpiry = reader["out_aft_xpr"] == DBNull.Value ? 0L : (long)reader["out_aft_xpr"];

                        rollingBalance.LeaveBalanceDate = reader["lvs_blnx_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lvs_blnx_dt"];
                        rollingBalance.LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"];
                        rollingBalance.LeaveUnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString();
                        rollingBalance.LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"];
                        rollingBalance.LeaveDepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString();
                        rollingBalance.LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"];
                        rollingBalance.LeaveLocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString();
                        rollingBalance.AnnualProfileLeaveDays = reader["lvs_opn_blc"] == DBNull.Value ? 0L : (long)reader["lvs_opn_blc"];
                        rollingBalance.PreviousYearsLeaveBalance = reader["lvs_prv_blc"] == DBNull.Value ? 0L : (long)reader["lvs_prv_blc"];
                        rollingBalance.PreviousBalanceCanBeCarriedOver = reader["bf_prev_blc"] == DBNull.Value ? false : (bool)reader["bf_prev_blc"];
                        rollingBalance.PreviousBalanceExpiryMonth = reader["prev_blc_xpr"] == DBNull.Value ? 0 : (int)reader["prev_blc_xpr"];
                        rollingBalance.PreviousBalanceExpiryMonthName = reader["prev_blc_xpr_mn"] == DBNull.Value ? string.Empty : reader["prev_blc_xpr_mn"].ToString();
                        rollingBalance.LeaveTransactionId = reader["lvs_trnx_id"] == DBNull.Value ? 0L : (long)reader["lvs_trnx_id"];
                    }
                }
                await conn.CloseAsync();
            }
            return rollingBalance;
        }
        public async Task<LeaveRollingBalance> GetLeaveRollingBalanceByTransactionIdAsync(long leaveTransactionId)
        {
            LeaveRollingBalance rollingBalance = new LeaveRollingBalance();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.lvs_blnx_id, b.lvs_emp_id, b.lvs_yr, b.lvs_typ_cd, b.no_dys_usd, b.no_dys_gvn, ");
            sb.Append("b.no_dys_ded, b.lvs_blnx_dt, b.lvs_unit_id, b.lvs_dept_id, b.lvs_loc_id, b.lvs_opn_blc, ");
            sb.Append("b.lvs_prv_blc, b.bf_prev_blc, b.prev_blc_xpr, b.lvs_trnx_id, t.lvs_typ_nm, ");
            sb.Append("COALESCE(b.lvs_opn_blc, 0) + COALESCE(b.lvs_prv_blc, 0) + COALESCE(b.no_dys_gvn, 0) - ");
            sb.Append("COALESCE(b.no_dys_usd, 0) -  COALESCE(b.no_dys_ded, 0) AS out_bf_xpr, ");
            sb.Append("COALESCE(b.lvs_opn_blc, 0) + COALESCE(b.no_dys_gvn, 0) - COALESCE(b.no_dys_usd, 0) - ");
            sb.Append("COALESCE(b.no_dys_ded, 0) AS out_aft_xpr, ");
            sb.Append("CASE WHEN prev_blc_xpr = 1 THEN 'January' WHEN prev_blc_xpr = 2 THEN 'February' ");
            sb.Append("WHEN prev_blc_xpr = 3 THEN 'March' WHEN prev_blc_xpr = 4 THEN 'April' ");
            sb.Append("WHEN prev_blc_xpr = 5 THEN 'May' WHEN prev_blc_xpr = 6 THEN 'June' ");
            sb.Append("WHEN prev_blc_xpr = 7 THEN 'July' WHEN prev_blc_xpr = 8 THEN 'August' ");
            sb.Append("WHEN prev_blc_xpr = 9 THEN 'September' WHEN prev_blc_xpr = 10 THEN 'October' ");
            sb.Append("WHEN prev_blc_xpr = 11 THEN 'November' WHEN prev_blc_xpr = 12 THEN 'December' ");
            sb.Append("ELSE '' END AS prev_blc_xpr_mn, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = b.lvs_emp_id) as lvs_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = b.lvs_unit_id) as lvs_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = b.lvs_dept_id) as lvs_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = b.lvs_loc_id) as lvs_loc_nm ");
            sb.Append("FROM public.lvm_lvs_blnx b ");
            sb.Append("INNER JOIN public.lvm_lvs_typs t ON b.lvs_typ_cd = t.lvs_typ_cd ");
            sb.Append("WHERE (b.lvs_trnx_id = @lvs_trnx_id) ");
            sb.Append("ORDER BY lvs_blnx_id DESC LIMIT 1; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_trnx_id = cmd.Parameters.Add("@lvs_trnx_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    lvs_trnx_id.Value = leaveTransactionId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        rollingBalance.RollingBalanceId = reader["lvs_blnx_id"] == DBNull.Value ? 0L : (long)reader["lvs_blnx_id"];
                        rollingBalance.LeaveEmployeeId = reader["lvs_emp_id"] == DBNull.Value ? string.Empty : reader["lvs_emp_id"].ToString();
                        rollingBalance.LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"];
                        rollingBalance.LeaveTypeCode = reader["lvs_typ_cd"] == DBNull.Value ? string.Empty : reader["lvs_typ_cd"].ToString();
                        rollingBalance.LeaveTypeName = reader["lvs_typ_nm"] == DBNull.Value ? string.Empty : reader["lvs_typ_nm"].ToString();
                        rollingBalance.LeaveDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0 : (long)reader["no_dys_usd"];
                        rollingBalance.LeaveDaysAdded = reader["no_dys_gvn"] == DBNull.Value ? 0 : (long)reader["no_dys_gvn"];
                        rollingBalance.LeaveDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0 : (long)reader["no_dys_ded"];
                        
                        rollingBalance.TotalOutstandingLeaveDaysBeforeExpiry = reader["out_bf_xpr"] == DBNull.Value ? 0L : (long)reader["out_bf_xpr"];
                        rollingBalance.TotalOutstandingLeaveDaysAfterExpiry = reader["out_aft_xpr"] == DBNull.Value ? 0L : (long)reader["out_aft_xpr"];

                        rollingBalance.LeaveBalanceDate = reader["lvs_blnx_dt"] == DBNull.Value ? new DateTime(1900, 1, 1) : (DateTime)reader["lvs_blnx_dt"];
                        rollingBalance.LeaveUnitId = reader["lvs_unit_id"] == DBNull.Value ? 0 : (int)reader["lvs_unit_id"];
                        rollingBalance.LeaveUnitName = reader["lvs_unit_nm"] == DBNull.Value ? string.Empty : reader["lvs_unit_nm"].ToString();
                        rollingBalance.LeaveDepartmentId = reader["lvs_dept_id"] == DBNull.Value ? 0 : (int)reader["lvs_dept_id"];
                        rollingBalance.LeaveDepartmentName = reader["lvs_dept_nm"] == DBNull.Value ? string.Empty : reader["lvs_dept_nm"].ToString();
                        rollingBalance.LeaveLocationId = reader["lvs_loc_id"] == DBNull.Value ? 0 : (int)reader["lvs_loc_id"];
                        rollingBalance.LeaveLocationName = reader["lvs_loc_nm"] == DBNull.Value ? string.Empty : reader["lvs_loc_nm"].ToString();
                        rollingBalance.AnnualProfileLeaveDays = reader["lvs_opn_blc"] == DBNull.Value ? 0L : (long)reader["lvs_opn_blc"];
                        rollingBalance.PreviousYearsLeaveBalance = reader["lvs_prv_blc"] == DBNull.Value ? 0L : (long)reader["lvs_prv_blc"];
                        rollingBalance.PreviousBalanceCanBeCarriedOver = reader["bf_prev_blc"] == DBNull.Value ? false : (bool)reader["bf_prev_blc"];
                        rollingBalance.PreviousBalanceExpiryMonth = reader["prev_blc_xpr"] == DBNull.Value ? 0 : (int)reader["lvs_opn_blc"];
                        rollingBalance.PreviousBalanceExpiryMonthName = reader["prev_blc_xpr_mn"] == DBNull.Value ? string.Empty : reader["prev_blc_xpr"].ToString();
                        rollingBalance.LeaveTransactionId = reader["lvs_trnx_id"] == DBNull.Value ? 0L : (long)reader["lvs_trnx_id"];
                    }
                }
                await conn.CloseAsync();
            }
            return rollingBalance;
        }

        #endregion
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
            sb.Append("lvs_pln_id, lvs_rqs_id) VALUES (@act_ds, ");
            sb.Append("@act_dt, @lvs_pln_id, @lvs_rqs_id); ");
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


        #region Leave Reports Action Methods

        //Leave Plan Compliance 
        public async Task<List<LeavePlanCompliance>> GetLeavePlanComplianceByUnitsAsync(int leaveYear)
        {
            List<LeavePlanCompliance> leavePlanComplianceList = new List<LeavePlanCompliance>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT u.unitqk, u.unitname, ");
            sb.Append("(SELECT COUNT(emp_id) FROM public.erm_emp_inf WHERE unit_id = u.unitqk ) as total_staff, ");
            sb.Append("(SELECT COUNT(DISTINCT emp_id) FROM public.lvm_lvs_plns WHERE unit_id = u.unitqk AND lvs_yr = @lvs_yr ) as staff_with_plan ");
            sb.Append("FROM public.gst_units u GROUP BY u.unitqk, u.unitname ORDER BY u.unitname;");

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
                        leavePlanComplianceList.Add(new LeavePlanCompliance()
                        {
                            UnitId = reader["unitqk"] == DBNull.Value ? 0 : (int)reader["unitqk"],
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TotalNumberOfStaff = reader["total_staff"] == DBNull.Value ? 0L : (long)reader["total_staff"],
                            NumberWithLeavePlans = reader["staff_with_plan"] == DBNull.Value ? 0L : (long)reader["staff_with_plan"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            if(leavePlanComplianceList != null && leavePlanComplianceList.Count > 0)
            {
                foreach(var l in leavePlanComplianceList)
                {
                    l.NumberWithoutLeavePlans = l.TotalNumberOfStaff - l.NumberWithLeavePlans;
                    if(l.TotalNumberOfStaff != 0)
                    {
                        l.PercentageCompliance = (Convert.ToDecimal(l.NumberWithLeavePlans) / Convert.ToDecimal(l.TotalNumberOfStaff)) * 100;
                        l.PercentageComplianceFormatted = $"{l.PercentageCompliance}%";
                    }
                }
            }
            return leavePlanComplianceList;
        }
        public async Task<List<LeavePlanCompliance>> GetLeavePlanComplianceByDepartmentsAsync(int leaveYear)
        {
            List<LeavePlanCompliance> leavePlanComplianceList = new List<LeavePlanCompliance>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.deptqk, d.deptname, ");
            sb.Append("(SELECT COUNT(emp_id) FROM public.erm_emp_inf WHERE dept_id = d.deptqk ) as total_staff, ");
            sb.Append("(SELECT COUNT(DISTINCT emp_id) FROM public.lvm_lvs_plns WHERE dept_id = d.deptqk AND lvs_yr = @lvs_yr ) as staff_with_plan ");
            sb.Append("FROM public.gst_depts d GROUP BY d.deptqk, d.deptname ORDER BY d.deptname;");

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
                        leavePlanComplianceList.Add(new LeavePlanCompliance()
                        {
                            DepartmentId = reader["deptqk"] == DBNull.Value ? 0 : (int)reader["deptqk"],
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TotalNumberOfStaff = reader["total_staff"] == DBNull.Value ? 0L : (long)reader["total_staff"],
                            NumberWithLeavePlans = reader["staff_with_plan"] == DBNull.Value ? 0L : (long)reader["staff_with_plan"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            if (leavePlanComplianceList != null && leavePlanComplianceList.Count > 0)
            {
                foreach (var l in leavePlanComplianceList)
                {
                    l.NumberWithoutLeavePlans = l.TotalNumberOfStaff - l.NumberWithLeavePlans;
                    if (l.TotalNumberOfStaff != 0)
                    {
                        l.PercentageCompliance = (Convert.ToDecimal(l.NumberWithLeavePlans) / Convert.ToDecimal(l.TotalNumberOfStaff)) * 100;
                        l.PercentageComplianceFormatted = $"{l.PercentageCompliance}%";
                    }
                }
            }
            return leavePlanComplianceList;
        }
        public async Task<List<LeavePlanCompliance>> GetLeavePlanComplianceByLocationsAsync(int leaveYear)
        {
            List<LeavePlanCompliance> leavePlanComplianceList = new List<LeavePlanCompliance>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT l.locqk, l.locname, ");
            sb.Append("(SELECT COUNT(emp_id) FROM public.erm_emp_inf WHERE loc_id = l.locqk ) as total_staff, ");
            sb.Append("(SELECT COUNT(DISTINCT emp_id) FROM public.lvm_lvs_plns WHERE loc_id = l.locqk AND lvs_yr = @lvs_yr ) as staff_with_plan ");
            sb.Append("FROM public.gst_locs l GROUP BY l.locqk, l.locname ORDER BY l.locname;");

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
                        leavePlanComplianceList.Add(new LeavePlanCompliance()
                        {
                            LocationId = reader["locqk"] == DBNull.Value ? 0 : (int)reader["locqk"],
                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNumberOfStaff = reader["total_staff"] == DBNull.Value ? 0L : (long)reader["total_staff"],
                            NumberWithLeavePlans = reader["staff_with_plan"] == DBNull.Value ? 0L : (long)reader["staff_with_plan"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            if (leavePlanComplianceList != null && leavePlanComplianceList.Count > 0)
            {
                foreach (var l in leavePlanComplianceList)
                {
                    l.NumberWithoutLeavePlans = l.TotalNumberOfStaff - l.NumberWithLeavePlans;
                    if (l.TotalNumberOfStaff != 0)
                    {
                        l.PercentageCompliance = (Convert.ToDecimal(l.NumberWithLeavePlans) / Convert.ToDecimal(l.TotalNumberOfStaff)) * 100;
                        l.PercentageComplianceFormatted = $"{l.PercentageCompliance}%";
                    }
                }
            }
            return leavePlanComplianceList;
        }


        //Leave Request Compliance
        public async Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceByLocationsAsync(int leaveYear)
        {
            List<LeaveRequestCompliance> leaveRequestComplianceList = new List<LeaveRequestCompliance>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT l.locqk, l.locname, ");
            sb.Append("(SELECT COUNT(emp_id) FROM public.erm_emp_inf WHERE loc_id = l.locqk ) as total_staff, ");
            sb.Append("(SELECT COUNT(DISTINCT lvs_emp_id) FROM public.lvm_lvs_rqsts WHERE lvs_loc_id = l.locqk AND lvs_rqst_yr = @lvs_yr) as staff_with_request ");
            sb.Append("FROM public.gst_locs l  GROUP BY l.locqk, l.locname  ORDER BY l.locname; ");

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
                        leaveRequestComplianceList.Add(new LeaveRequestCompliance()
                        {
                            LocationId = reader["locqk"] == DBNull.Value ? 0 : (int)reader["locqk"],
                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            TotalNumberOfStaff = reader["total_staff"] == DBNull.Value ? 0L : (long)reader["total_staff"],
                            NumberWithLeaveRequests = reader["staff_with_request"] == DBNull.Value ? 0L : (long)reader["staff_with_request"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            if (leaveRequestComplianceList != null && leaveRequestComplianceList.Count > 0)
            {
                foreach (var l in leaveRequestComplianceList)
                {
                    l.NumberWithoutLeaveRequests = l.TotalNumberOfStaff - l.NumberWithLeaveRequests;
                    if (l.TotalNumberOfStaff != 0)
                    {
                        l.PercentageCompliance = (Convert.ToDecimal(l.NumberWithLeaveRequests) / Convert.ToDecimal(l.TotalNumberOfStaff)) * 100;
                        l.PercentageComplianceFormatted = $"{l.PercentageCompliance}%";
                    }
                }
            }
            return leaveRequestComplianceList;
        }
        public async Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceByDepartmentsAsync(int leaveYear)
        {
            List<LeaveRequestCompliance> leaveRequestComplianceList = new List<LeaveRequestCompliance>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.deptqk, d.deptname, ");
            sb.Append("(SELECT COUNT(emp_id) FROM public.erm_emp_inf WHERE dept_id = d.deptqk ) as total_staff, ");
            sb.Append("(SELECT COUNT(DISTINCT lvs_emp_id) FROM public.lvm_lvs_rqsts WHERE lvs_dept_id = d.deptqk AND lvs_rqst_yr = @lvs_yr) as staff_with_request ");
            sb.Append("FROM public.gst_depts d GROUP BY d.deptqk, d.deptname ORDER BY d.deptname;");

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
                        leaveRequestComplianceList.Add(new LeaveRequestCompliance()
                        {
                            DepartmentId = reader["deptqk"] == DBNull.Value ? 0 : (int)reader["deptqk"],
                            LocationName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            TotalNumberOfStaff = reader["total_staff"] == DBNull.Value ? 0L : (long)reader["total_staff"],
                            NumberWithLeaveRequests = reader["staff_with_request"] == DBNull.Value ? 0L : (long)reader["staff_with_request"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            if (leaveRequestComplianceList != null && leaveRequestComplianceList.Count > 0)
            {
                foreach (var l in leaveRequestComplianceList)
                {
                    l.NumberWithoutLeaveRequests = l.TotalNumberOfStaff - l.NumberWithLeaveRequests;
                    if (l.TotalNumberOfStaff != 0)
                    {
                        l.PercentageCompliance = (Convert.ToDecimal(l.NumberWithLeaveRequests) / Convert.ToDecimal(l.TotalNumberOfStaff)) * 100;
                        l.PercentageComplianceFormatted = $"{l.PercentageCompliance}%";
                    }
                }
            }
            return leaveRequestComplianceList;
        }
        public async Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceByUnitsAsync(int leaveYear)
        {
            List<LeaveRequestCompliance> leaveRequestComplianceList = new List<LeaveRequestCompliance>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT u.unitqk, u.unitname, ");
            sb.Append("(SELECT COUNT(emp_id) FROM public.erm_emp_inf WHERE unit_id = u.unitqk ) as total_staff, ");
            sb.Append("(SELECT COUNT(DISTINCT lvs_emp_id) FROM public.lvm_lvs_rqsts WHERE lvs_unit_id = u.unitqk AND lvs_rqst_yr = @lvs_yr) as staff_with_request ");
            sb.Append("FROM public.gst_units u GROUP BY u.unitqk, u.unitname ORDER BY u.unitname;");

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
                        leaveRequestComplianceList.Add(new LeaveRequestCompliance()
                        {
                            UnitId = reader["unitqk"] == DBNull.Value ? 0 : (int)reader["unitqk"],
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                            TotalNumberOfStaff = reader["total_staff"] == DBNull.Value ? 0L : (long)reader["total_staff"],
                            NumberWithLeaveRequests = reader["staff_with_request"] == DBNull.Value ? 0L : (long)reader["staff_with_request"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            if (leaveRequestComplianceList != null && leaveRequestComplianceList.Count > 0)
            {
                foreach (var l in leaveRequestComplianceList)
                {
                    l.NumberWithoutLeaveRequests = l.TotalNumberOfStaff - l.NumberWithLeaveRequests;
                    if (l.TotalNumberOfStaff != 0)
                    {
                        l.PercentageCompliance = (Convert.ToDecimal(l.NumberWithLeaveRequests) / Convert.ToDecimal(l.TotalNumberOfStaff)) * 100;
                        l.PercentageComplianceFormatted = $"{l.PercentageCompliance}%";
                    }
                }
            }
            return leaveRequestComplianceList;
        }


        //Annual Leave Summary
        public async Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByUnitIdAsync(int leaveYear, int unitId)
        {
            List<AnnualLeaveSummary> annualLeaveSummaryList = new List<AnnualLeaveSummary>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, (SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id) AS emp_nm, ");
            sb.Append("e.emp_no_1, e.official_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = e.dept_id) AS dept_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = e.unit_id) AS unit_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = e.loc_id) AS loc_nm, ");
            sb.Append("(SELECT coy_name FROM public.gst_coys WHERE coy_code = e.coy_id) AS coy_nm, ");
            sb.Append("b.lvs_yr, b.lvs_opn_blc, b.no_dys_usd, b.no_dys_gvn, b.no_dys_ded, b.lvs_prv_blc, ");
            sb.Append("b.no_dys_unusd FROM public.erm_emp_inf e INNER JOIN ");
            sb.Append("(SELECT lvs_emp_id, lvs_yr, lvs_blnx_id, lvs_opn_blc, no_dys_usd, no_dys_gvn, no_dys_ded, ");
            sb.Append("lvs_prv_blc, (COALESCE(lvs_opn_blc,0) + COALESCE(lvs_prv_blc,0) + COALESCE(no_dys_gvn,0) ");
            sb.Append("- COALESCE(no_dys_usd,0) - COALESCE(no_dys_ded,0)) AS no_dys_unusd ");
            sb.Append("FROM public.lvm_lvs_blnx WHERE lvs_typ_cd = 'ANL' AND lvs_yr = @lvs_yr) b ");
            sb.Append("ON b.lvs_emp_id = e.emp_id WHERE (e.is_dx = false) ");
            sb.Append("AND (e.unit_id = @unit_id) ORDER BY emp_nm; ");


            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);

                    await cmd.PrepareAsync();
                    lvs_yr.Value = leaveYear;
                    unit_id.Value = unitId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        annualLeaveSummaryList.Add(new AnnualLeaveSummary()
                        {
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                            EmployeeNumber = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                            OfficialEmail = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),

                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),

                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            CompanyCode = reader["coy_id"] == DBNull.Value ? string.Empty : reader["coy_id"].ToString(),
                            CompanyName = reader["coy_nm"] == DBNull.Value ? string.Empty : reader["coy_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],

                            NumberOfAnnualLeaveDaysDue = reader["lvs_opn_blc"] == DBNull.Value ? 0L : (long)reader["lvs_opn_blc"],
                            NumberOfDaysAdded = reader["no_dys_gvn"] == DBNull.Value ? 0L : (long)reader["no_dys_gvn"],
                            NumberOfDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0L : (long)reader["no_dys_ded"],
                            NumberOfDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0L : (long)reader["no_dys_usd"],
                            NumberOfDaysUnused = reader["no_dys_unusd"] == DBNull.Value ? 0L : (long)reader["no_dys_unusd"],
                            PreviousYearsBalanceBroughtFoward = reader["lvs_prv_blc"] == DBNull.Value ? 0L : (long)reader["lvs_prv_blc"],
                            
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return annualLeaveSummaryList;
        }
        public async Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByDepartmentIdAsync(int leaveYear, int departmentId)
        {
            List<AnnualLeaveSummary> annualLeaveSummaryList = new List<AnnualLeaveSummary>();
            StringBuilder sb = new StringBuilder();


            sb.Append("SELECT e.emp_id, (SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id) AS emp_nm, ");
            sb.Append("e.emp_no_1, e.official_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = e.dept_id) AS dept_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = e.unit_id) AS unit_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = e.loc_id) AS loc_nm, ");
            sb.Append("(SELECT coy_name FROM public.gst_coys WHERE coy_code = e.coy_id) AS coy_nm, ");
            sb.Append("b.lvs_yr, b.lvs_opn_blc, b.no_dys_usd, b.no_dys_gvn, b.no_dys_ded, b.lvs_prv_blc, ");
            sb.Append("b.no_dys_unusd FROM public.erm_emp_inf e INNER JOIN ");
            sb.Append("(SELECT lvs_emp_id, lvs_yr, lvs_blnx_id, lvs_opn_blc, no_dys_usd, no_dys_gvn, no_dys_ded, ");
            sb.Append("lvs_prv_blc, (COALESCE(lvs_opn_blc,0) + COALESCE(lvs_prv_blc,0) + COALESCE(no_dys_gvn,0) ");
            sb.Append("- COALESCE(no_dys_usd,0) - COALESCE(no_dys_ded,0)) AS no_dys_unusd ");
            sb.Append("FROM public.lvm_lvs_blnx WHERE lvs_typ_cd = 'ANL' AND lvs_yr = @lvs_yr) b ");
            sb.Append("ON b.lvs_emp_id = e.emp_id WHERE (e.is_dx = false) ");
            sb.Append("AND (e.dept_id = @dept_id) ORDER BY emp_nm; ");


            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);

                    await cmd.PrepareAsync();
                    lvs_yr.Value = leaveYear;
                    dept_id.Value = departmentId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        annualLeaveSummaryList.Add(new AnnualLeaveSummary()
                        {
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                            EmployeeNumber = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                            OfficialEmail = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),

                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),

                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            CompanyCode = reader["coy_id"] == DBNull.Value ? string.Empty : reader["coy_id"].ToString(),
                            CompanyName = reader["coy_nm"] == DBNull.Value ? string.Empty : reader["coy_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],

                            NumberOfAnnualLeaveDaysDue = reader["lvs_opn_blc"] == DBNull.Value ? 0L : (long)reader["lvs_opn_blc"],
                            NumberOfDaysAdded = reader["no_dys_gvn"] == DBNull.Value ? 0L : (long)reader["no_dys_gvn"],
                            NumberOfDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0L : (long)reader["no_dys_ded"],
                            NumberOfDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0L : (long)reader["no_dys_usd"],
                            NumberOfDaysUnused = reader["no_dys_unusd"] == DBNull.Value ? 0L : (long)reader["no_dys_unusd"],
                            PreviousYearsBalanceBroughtFoward = reader["lvs_prv_blc"] == DBNull.Value ? 0L : (long)reader["lvs_prv_blc"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return annualLeaveSummaryList;
        }
        public async Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByLocationIdAsync(int leaveYear, int locationId)
        {
            List<AnnualLeaveSummary> annualLeaveSummaryList = new List<AnnualLeaveSummary>();
            StringBuilder sb = new StringBuilder();


            sb.Append("SELECT e.emp_id, (SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id) AS emp_nm, ");
            sb.Append("e.emp_no_1, e.official_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = e.dept_id) AS dept_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = e.unit_id) AS unit_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = e.loc_id) AS loc_nm, ");
            sb.Append("(SELECT coy_name FROM public.gst_coys WHERE coy_code = e.coy_id) AS coy_nm, ");
            sb.Append("b.lvs_yr, b.lvs_opn_blc, b.no_dys_usd, b.no_dys_gvn, b.no_dys_ded, b.lvs_prv_blc, ");
            sb.Append("b.no_dys_unusd FROM public.erm_emp_inf e INNER JOIN ");
            sb.Append("(SELECT lvs_emp_id, lvs_yr, lvs_blnx_id, lvs_opn_blc, no_dys_usd, no_dys_gvn, no_dys_ded, ");
            sb.Append("lvs_prv_blc, (COALESCE(lvs_opn_blc,0) + COALESCE(lvs_prv_blc,0) + COALESCE(no_dys_gvn,0) ");
            sb.Append("- COALESCE(no_dys_usd,0) - COALESCE(no_dys_ded,0)) AS no_dys_unusd ");
            sb.Append("FROM public.lvm_lvs_blnx WHERE lvs_typ_cd = 'ANL' AND lvs_yr = @lvs_yr) b ");
            sb.Append("ON b.lvs_emp_id = e.emp_id WHERE (e.is_dx = false) ");
            sb.Append("AND (e.loc_id = @loc_id) ORDER BY emp_nm; ");


            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = leaveYear;
                    loc_id.Value = locationId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        annualLeaveSummaryList.Add(new AnnualLeaveSummary()
                        {
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                            EmployeeNumber = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                            OfficialEmail = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),

                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),

                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            CompanyCode = reader["coy_id"] == DBNull.Value ? string.Empty : reader["coy_id"].ToString(),
                            CompanyName = reader["coy_nm"] == DBNull.Value ? string.Empty : reader["coy_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],

                            NumberOfAnnualLeaveDaysDue = reader["lvs_opn_blc"] == DBNull.Value ? 0L : (long)reader["lvs_opn_blc"],
                            NumberOfDaysAdded = reader["no_dys_gvn"] == DBNull.Value ? 0L : (long)reader["no_dys_gvn"],
                            NumberOfDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0L : (long)reader["no_dys_ded"],
                            NumberOfDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0L : (long)reader["no_dys_usd"],
                            NumberOfDaysUnused = reader["no_dys_unusd"] == DBNull.Value ? 0L : (long)reader["no_dys_unusd"],
                            PreviousYearsBalanceBroughtFoward = reader["lvs_prv_blc"] == DBNull.Value ? 0L : (long)reader["lvs_prv_blc"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return annualLeaveSummaryList;
        }
        public async Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByLocationIdnUnitIdAsync(int leaveYear, int locationId, int unitId)
        {
            List<AnnualLeaveSummary> annualLeaveSummaryList = new List<AnnualLeaveSummary>();
            StringBuilder sb = new StringBuilder();


            sb.Append("SELECT e.emp_id, (SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id) AS emp_nm, ");
            sb.Append("e.emp_no_1, e.official_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = e.dept_id) AS dept_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = e.unit_id) AS unit_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = e.loc_id) AS loc_nm, ");
            sb.Append("(SELECT coy_name FROM public.gst_coys WHERE coy_code = e.coy_id) AS coy_nm, ");
            sb.Append("b.lvs_yr, b.lvs_opn_blc, b.no_dys_usd, b.no_dys_gvn, b.no_dys_ded, b.lvs_prv_blc, ");
            sb.Append("b.no_dys_unusd FROM public.erm_emp_inf e INNER JOIN ");
            sb.Append("(SELECT lvs_emp_id, lvs_yr, lvs_blnx_id, lvs_opn_blc, no_dys_usd, no_dys_gvn, no_dys_ded, ");
            sb.Append("lvs_prv_blc, (COALESCE(lvs_opn_blc,0) + COALESCE(lvs_prv_blc,0) + COALESCE(no_dys_gvn,0) ");
            sb.Append("- COALESCE(no_dys_usd,0) - COALESCE(no_dys_ded,0)) AS no_dys_unusd ");
            sb.Append("FROM public.lvm_lvs_blnx WHERE lvs_typ_cd = 'ANL' AND lvs_yr = @lvs_yr) b ");
            sb.Append("ON b.lvs_emp_id = e.emp_id WHERE e.is_dx = false ");
            sb.Append("AND e.loc_id = @loc_id AND e.unit_id = @unit_id ORDER BY emp_nm; ");


            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = leaveYear;
                    loc_id.Value = locationId;
                    unit_id.Value = unitId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        annualLeaveSummaryList.Add(new AnnualLeaveSummary()
                        {
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                            EmployeeNumber = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                            OfficialEmail = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),

                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),

                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            CompanyCode = reader["coy_id"] == DBNull.Value ? string.Empty : reader["coy_id"].ToString(),
                            CompanyName = reader["coy_nm"] == DBNull.Value ? string.Empty : reader["coy_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],

                            NumberOfAnnualLeaveDaysDue = reader["lvs_opn_blc"] == DBNull.Value ? 0L : (long)reader["lvs_opn_blc"],
                            NumberOfDaysAdded = reader["no_dys_gvn"] == DBNull.Value ? 0L : (long)reader["no_dys_gvn"],
                            NumberOfDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0L : (long)reader["no_dys_ded"],
                            NumberOfDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0L : (long)reader["no_dys_usd"],
                            NumberOfDaysUnused = reader["no_dys_unusd"] == DBNull.Value ? 0L : (long)reader["no_dys_unusd"],
                            PreviousYearsBalanceBroughtFoward = reader["lvs_prv_blc"] == DBNull.Value ? 0L : (long)reader["lvs_prv_blc"],

                        });
                    }
                }
                await conn.CloseAsync();
            }
            return annualLeaveSummaryList;
        }
        public async Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByEmployeeNameAsync(int leaveYear, string employeeName)
        {
            List<AnnualLeaveSummary> annualLeaveSummaryList = new List<AnnualLeaveSummary>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.emp_id, (SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id) AS emp_nm, ");
            sb.Append("e.emp_no_1, e.official_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = e.dept_id) AS dept_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = e.unit_id) AS unit_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = e.loc_id) AS loc_nm, ");
            sb.Append("(SELECT coy_name FROM public.gst_coys WHERE coy_code = e.coy_id) AS coy_nm, ");
            sb.Append("b.lvs_yr, b.lvs_opn_blc, b.no_dys_usd, b.no_dys_gvn, b.no_dys_ded, b.lvs_prv_blc, ");
            sb.Append("b.no_dys_unusd FROM public.erm_emp_inf e INNER JOIN ");
            sb.Append("(SELECT lvs_emp_id, lvs_yr, lvs_blnx_id, lvs_opn_blc, no_dys_usd, no_dys_gvn, no_dys_ded, ");
            sb.Append("lvs_prv_blc, (COALESCE(lvs_opn_blc,0) + COALESCE(lvs_prv_blc,0) + COALESCE(no_dys_gvn,0) ");
            sb.Append("- COALESCE(no_dys_usd,0) - COALESCE(no_dys_ded,0)) AS no_dys_unusd ");
            sb.Append("FROM public.lvm_lvs_blnx WHERE lvs_typ_cd = 'ANL' AND lvs_yr = @lvs_yr) b ");
            sb.Append("ON b.lvs_emp_id = e.emp_id WHERE (e.is_dx = false) ");
            sb.Append("AND e.emp_id = (SELECT id FROM public.gst_prsns WHERE fullname = @emp_name); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_yr = cmd.Parameters.Add("@lvs_yr", NpgsqlDbType.Integer);
                    var emp_name = cmd.Parameters.Add("@emp_name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lvs_yr.Value = leaveYear;
                    emp_name.Value = employeeName;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        annualLeaveSummaryList.Add(new AnnualLeaveSummary()
                        {
                            EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                            EmployeeNumber = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                            OfficialEmail = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),

                            UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                            UnitName = reader["unit_nm"] == DBNull.Value ? string.Empty : reader["unit_nm"].ToString(),

                            DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                            DepartmentName = reader["dept_nm"] == DBNull.Value ? string.Empty : reader["dept_nm"].ToString(),

                            LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                            LocationName = reader["loc_nm"] == DBNull.Value ? string.Empty : reader["loc_nm"].ToString(),

                            CompanyCode = reader["coy_id"] == DBNull.Value ? string.Empty : reader["coy_id"].ToString(),
                            CompanyName = reader["coy_nm"] == DBNull.Value ? string.Empty : reader["coy_nm"].ToString(),

                            LeaveYear = reader["lvs_yr"] == DBNull.Value ? 1900 : (int)reader["lvs_yr"],

                            NumberOfAnnualLeaveDaysDue = reader["lvs_opn_blc"] == DBNull.Value ? 0L : (long)reader["lvs_opn_blc"],
                            NumberOfDaysAdded = reader["no_dys_gvn"] == DBNull.Value ? 0L : (long)reader["no_dys_gvn"],
                            NumberOfDaysDeducted = reader["no_dys_ded"] == DBNull.Value ? 0L : (long)reader["no_dys_ded"],
                            NumberOfDaysUsed = reader["no_dys_usd"] == DBNull.Value ? 0L : (long)reader["no_dys_usd"],
                            NumberOfDaysUnused = reader["no_dys_unusd"] == DBNull.Value ? 0L : (long)reader["no_dys_unusd"],
                            PreviousYearsBalanceBroughtFoward = reader["lvs_prv_blc"] == DBNull.Value ? 0L : (long)reader["lvs_prv_blc"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return annualLeaveSummaryList;
        }

        #endregion
    }
}
