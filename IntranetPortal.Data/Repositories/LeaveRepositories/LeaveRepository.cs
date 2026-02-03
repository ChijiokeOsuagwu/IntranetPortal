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


        #region Leave Plan Write Action Methods
        //===  Leave Write Action Methods =======//
        public async Task<long> AddLeavePlanAsync(LeavePlan e)
        {
            long newLeaveId = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lvm_lvs_plns(emp_id, unit_id, dept_id, loc_id, ");
            sb.Append("lvs_yr, lvs_typ_cd, lvs_rsn, lvs_sts, is_aprv, aprv_by, aprv_dt, ");
            sb.Append("prp_lvs_sdt, prp_lvs_edt, prp_lvs_dur, prp_dur_ds, prp_rsmptn_dt, ");
            sb.Append("apv_lvs_sdt, apv_lvs_edt, apv_lvs_dur, apv_dur_ds, apv_rsmptn_dt, ");
            sb.Append("prp_lvs_dur_typ, apv_lvs_dur_typ) VALUES (@emp_id, @unit_id, ");
            sb.Append("@dept_id, @loc_id, @lvs_yr, @lvs_typ_cd, @lvs_rsn, @lvs_sts, ");
            sb.Append("@is_aprv, @aprv_by, @aprv_dt, @prp_lvs_sdt, @prp_lvs_edt, @prp_lvs_dur, ");
            sb.Append("@prp_dur_ds, @prp_rsmptn_dt, @apv_lvs_sdt, @apv_lvs_edt, @apv_lvs_dur, ");
            sb.Append("@apv_dur_ds, @apv_rsmptn_dt, @prp_lvs_dur_typ, @apv_lvs_dur_typ) ");
            sb.Append("RETURNING lvs_pln_id; ");
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
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    
                    var is_aprv = cmd.Parameters.Add("@is_aprv", NpgsqlDbType.Boolean);
                    var aprv_by = cmd.Parameters.Add("@aprv_by", NpgsqlDbType.Text);
                    var aprv_dt = cmd.Parameters.Add("@aprv_dt", NpgsqlDbType.Timestamp);

                    var prp_lvs_sdt = cmd.Parameters.Add("@prp_lvs_sdt", NpgsqlDbType.Timestamp);
                    var prp_lvs_edt = cmd.Parameters.Add("@prp_lvs_edt", NpgsqlDbType.Timestamp);
                    var prp_lvs_dur = cmd.Parameters.Add("@prp_lvs_dur", NpgsqlDbType.Integer);
                    var prp_lvs_dur_typ = cmd.Parameters.Add("@prp_lvs_dur_typ", NpgsqlDbType.Integer);
                    var prp_dur_ds = cmd.Parameters.Add("@prp_dur_ds", NpgsqlDbType.Integer);
                    var prp_rsmptn_dt = cmd.Parameters.Add("@prp_rsmptn_dt", NpgsqlDbType.Timestamp);

                    var apv_lvs_sdt = cmd.Parameters.Add("@apv_lvs_sdt", NpgsqlDbType.Timestamp);
                    var apv_lvs_edt = cmd.Parameters.Add("@apv_lvs_edt", NpgsqlDbType.Timestamp);
                    var apv_lvs_dur = cmd.Parameters.Add("@apv_lvs_dur", NpgsqlDbType.Integer);
                    var apv_lvs_dur_typ = cmd.Parameters.Add("@apv_lvs_dur_typ", NpgsqlDbType.Integer);
                    var apv_dur_ds = cmd.Parameters.Add("@apv_dur_ds", NpgsqlDbType.Integer);
                    var apv_rsmptn_dt = cmd.Parameters.Add("@apv_rsmptn_dt", NpgsqlDbType.Timestamp);

                    cmd.Prepare();

                    emp_id.Value = e.LeaveEmployeeId;
                    unit_id.Value = e.LeaveUnitId;
                    dept_id.Value = e.LeaveDepartmentId;
                    loc_id.Value = e.LeaveLocationId;

                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason ?? (object)DBNull.Value;
                    lvs_sts.Value = e.LeavePlanStatus;
                    
                    is_aprv.Value = e.IsApproved;
                    aprv_by.Value = e.ApprovedBy ?? (object)DBNull.Value;
                    aprv_dt.Value = e.ApprovedTime ?? (object)DBNull.Value;

                    prp_lvs_sdt.Value = e.ProposedStartDate ?? (object)DBNull.Value;
                    prp_lvs_edt.Value = e.ProposedEndDate ?? (object)DBNull.Value;
                    prp_lvs_dur.Value = e.ProposedDuration;
                    prp_lvs_dur_typ.Value = e.ProposedDurationTypeId;
                    prp_dur_ds.Value = e.ProposedDurationDescription ?? (object)DBNull.Value;
                    prp_rsmptn_dt.Value = e.ProposedResumptionDate ?? (object)DBNull.Value;

                    apv_lvs_sdt.Value = e.ApprovedStartDate ?? (object)DBNull.Value;
                    apv_lvs_edt.Value = e.ApprovedEndDate ?? (object)DBNull.Value;
                    apv_lvs_dur.Value = e.ApprovedDuration;
                    apv_lvs_dur_typ.Value = e.ApprovedDurationTypeId;
                    apv_dur_ds.Value = e.ApprovedDurationDescription ?? (object)DBNull.Value;
                    apv_rsmptn_dt.Value = e.ApprovedResumptionDate ?? (object)DBNull.Value;

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
            //sb.Append("DELETE FROM public.lms_lvs_logs WHERE (lvs_inf_id = @lvs_inf_id); ");
            //sb.Append("DELETE FROM public.lms_lvs_msgs WHERE (lvs_inf_id = @lvs_inf_id); ");
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
            sb.Append("lvs_sts=@lvs_sts, is_aprv=@is_aprv, aprv_by=@aprv_by, ");
            sb.Append("aprv_dt=@aprv_dt, prp_lvs_sdt=@prp_lvs_sdt, ");
            sb.Append("prp_lvs_edt=@prp_lvs_edt, prp_lvs_dur=@prp_lvs_dur, ");
            sb.Append("prp_dur_ds=@prp_dur_ds, prp_rsmptn_dt=@prp_rsmptn_dt, ");
            sb.Append("apv_lvs_sdt=@apv_lvs_sdt, apv_lvs_edt=@apv_lvs_edt, ");
            sb.Append("apv_lvs_dur=@apv_lvs_dur, apv_dur_ds=@apv_dur_ds, ");
            sb.Append("apv_rsmptn_dt=@apv_rsmptn_dt, prp_lvs_dur_typ=@prp_lvs_dur_typ, ");
            sb.Append("apv_lvs_dur_typ=@apv_lvs_dur_typ ");
            sb.Append("WHERE (lvs_pln_id = @lvs_pln_id); ");

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
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);

                    var is_aprv = cmd.Parameters.Add("@is_aprv", NpgsqlDbType.Boolean);
                    var aprv_by = cmd.Parameters.Add("@aprv_by", NpgsqlDbType.Text);
                    var aprv_dt = cmd.Parameters.Add("@aprv_dt", NpgsqlDbType.Timestamp);

                    var prp_lvs_sdt = cmd.Parameters.Add("@prp_lvs_sdt", NpgsqlDbType.Timestamp);
                    var prp_lvs_edt = cmd.Parameters.Add("@prp_lvs_edt", NpgsqlDbType.Timestamp);
                    var prp_lvs_dur = cmd.Parameters.Add("@prp_lvs_dur", NpgsqlDbType.Integer);
                    var prp_lvs_dur_typ = cmd.Parameters.Add("@prp_lvs_dur_typ", NpgsqlDbType.Integer);
                    var prp_dur_ds = cmd.Parameters.Add("@prp_dur_ds", NpgsqlDbType.Integer);
                    var prp_rsmptn_dt = cmd.Parameters.Add("@prp_rsmptn_dt", NpgsqlDbType.Timestamp);

                    var apv_lvs_sdt = cmd.Parameters.Add("@apv_lvs_sdt", NpgsqlDbType.Timestamp);
                    var apv_lvs_edt = cmd.Parameters.Add("@apv_lvs_edt", NpgsqlDbType.Timestamp);
                    var apv_lvs_dur = cmd.Parameters.Add("@apv_lvs_dur", NpgsqlDbType.Integer);
                    var apv_lvs_dur_typ = cmd.Parameters.Add("@apv_lvs_dur_typ", NpgsqlDbType.Integer);
                    var apv_dur_ds = cmd.Parameters.Add("@apv_dur_ds", NpgsqlDbType.Integer);
                    var apv_rsmptn_dt = cmd.Parameters.Add("@apv_rsmptn_dt", NpgsqlDbType.Timestamp);

                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);

                    cmd.Prepare();

                    lvs_yr.Value = e.LeaveYear;
                    lvs_typ_cd.Value = e.LeaveTypeCode;
                    lvs_rsn.Value = e.LeaveReason ?? (object)DBNull.Value;
                    lvs_sts.Value = e.LeavePlanStatus;

                    is_aprv.Value = e.IsApproved;
                    aprv_by.Value = e.ApprovedBy ?? (object)DBNull.Value;
                    aprv_dt.Value = e.ApprovedTime ?? (object)DBNull.Value;

                    prp_lvs_sdt.Value = e.ProposedStartDate ?? (object)DBNull.Value;
                    prp_lvs_edt.Value = e.ProposedEndDate ?? (object)DBNull.Value;
                    prp_lvs_dur.Value = e.ProposedDuration;
                    prp_lvs_dur_typ.Value = e.ProposedDurationTypeId;
                    prp_dur_ds.Value = e.ProposedDurationDescription ?? (object)DBNull.Value;
                    prp_rsmptn_dt.Value = e.ProposedResumptionDate ?? (object)DBNull.Value;

                    apv_lvs_sdt.Value = e.ApprovedStartDate ?? (object)DBNull.Value;
                    apv_lvs_edt.Value = e.ApprovedEndDate ?? (object)DBNull.Value;
                    apv_lvs_dur.Value = e.ApprovedDuration;
                    apv_lvs_dur_typ.Value = e.ApprovedDurationTypeId;
                    apv_dur_ds.Value = e.ApprovedDurationDescription ?? (object)DBNull.Value;
                    apv_rsmptn_dt.Value = e.ApprovedResumptionDate ?? (object)DBNull.Value;

                    lvs_pln_id.Value = e.LeavePlanId;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeavePlanStatusAsync(long leavePlanId, string newStatus)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_plns SET lvs_sts=@lvs_sts ");
            sb.Append("WHERE (lvs_pln_id=@lvs_pln_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    var lvs_sts = cmd.Parameters.Add("@lvs_sts", NpgsqlDbType.Text);
                    cmd.Prepare();
                    lvs_pln_id.Value = leavePlanId;
                    lvs_sts.Value = newStatus;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> UpdateLeavePlanApprovalStatusAsync(long leavePlanId, bool isApproved, string approvedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lvm_lvs_plns SET is_aprv=@is_aprv,  ");
            sb.Append("aprv_by=@aprv_by, aprv_dt=@aprv_dt ");
            sb.Append("WHERE (lvs_pln_id=@lvs_pln_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lvs_pln_id = cmd.Parameters.Add("@lvs_pln_id", NpgsqlDbType.Bigint);
                    var is_aprv = cmd.Parameters.Add("@is_aprv", NpgsqlDbType.Boolean);
                    var aprv_by = cmd.Parameters.Add("@aprv_by", NpgsqlDbType.Text);
                    var aprv_dt = cmd.Parameters.Add("@aprv_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    lvs_pln_id.Value = leavePlanId;
                    is_aprv.Value = isApproved;
                    aprv_by.Value = approvedBy ?? (object)DBNull.Value;
                    aprv_dt.Value = DateTime.Now;
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
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, p.loc_id, ");
            sb.Append("p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_sts, p.is_aprv, p.aprv_by, ");
            sb.Append("p.aprv_dt, p.prp_lvs_sdt, p.prp_lvs_edt, p.prp_lvs_dur, ");
            sb.Append("p.prp_dur_ds, p.prp_rsmptn_dt, p.apv_lvs_sdt, p.apv_lvs_edt, ");
            sb.Append("p.apv_lvs_dur, p.apv_dur_ds, p.apv_rsmptn_dt, p.prp_lvs_dur_typ, ");
            sb.Append("p.apv_lvs_dur_typ, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (p.lvs_pln_id = @lvs_pln_id) ");
            sb.Append("ORDER BY p.prp_lvs_sdt; ");

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
                            LeavePlanStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),

                            IsApproved = reader["is_aprv"] == DBNull.Value ? false : (bool)reader["is_aprv"],
                            ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString(),
                            ApprovedTime = reader["aprv_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["aprv_dt"],

                            ProposedStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),
                            ProposedResumptionDate = reader["prp_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_rsmptn_dt"],

                            ApprovedStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),
                            ApprovedResumptionDate = reader["apv_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_rsmptn_dt"],
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
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, p.loc_id, ");
            sb.Append("p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_sts, p.is_aprv, p.aprv_by, ");
            sb.Append("p.aprv_dt, p.prp_lvs_sdt, p.prp_lvs_edt, p.prp_lvs_dur, ");
            sb.Append("p.prp_dur_ds, p.prp_rsmptn_dt, p.apv_lvs_sdt, p.apv_lvs_edt, ");
            sb.Append("p.apv_lvs_dur, p.apv_dur_ds, p.apv_rsmptn_dt, p.prp_lvs_dur_typ, ");
            sb.Append("p.apv_lvs_dur_typ, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (emp_id = @emp_id AND lvs_yr=@lvs_yr) ");
            sb.Append("ORDER BY p.prp_lvs_sdt; ");

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
                            LeavePlanStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),
                            
                            IsApproved = reader["is_aprv"] == DBNull.Value ? false : (bool)reader["is_aprv"],
                            ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString(),
                            ApprovedTime = reader["aprv_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["aprv_dt"],

                            ProposedStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),
                            ProposedResumptionDate = reader["prp_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_rsmptn_dt"],

                            ApprovedStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),
                            ApprovedResumptionDate = reader["apv_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_rsmptn_dt"],
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
            sb.Append("SELECT p.lvs_pln_id, p.emp_id, p.unit_id, p.dept_id, p.loc_id, ");
            sb.Append("p.lvs_yr, p.lvs_typ_cd, p.lvs_rsn, p.lvs_sts, p.is_aprv, p.aprv_by, ");
            sb.Append("p.aprv_dt, p.prp_lvs_sdt, p.prp_lvs_edt, p.prp_lvs_dur, ");
            sb.Append("p.prp_dur_ds, p.prp_rsmptn_dt, p.apv_lvs_sdt, p.apv_lvs_edt, ");
            sb.Append("p.apv_lvs_dur, p.apv_dur_ds, p.apv_rsmptn_dt, p.prp_lvs_dur_typ, ");
            sb.Append("p.apv_lvs_dur_typ, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = p.emp_id) ");
            sb.Append("as emp_nm, (SELECT lvs_typ_nm FROM public.lvm_lvs_typs ");
            sb.Append("WHERE lvs_typ_cd = p.lvs_typ_cd) as lvs_typ_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = p.unit_id) ");
            sb.Append("as unit_nm, (SELECT deptname FROM public.gst_depts WHERE deptqk ");
            sb.Append("= p.dept_id) as dept_nm, (SELECT locname FROM public.gst_locs ");
            sb.Append("WHERE locqk = p.loc_id) as loc_nm ");
            sb.Append("FROM public.lvm_lvs_plns p ");
            sb.Append("WHERE (lvs_yr=@lvs_yr) AND ");
            sb.Append("(p.emp_id IN (SELECT id FROM public.gst_prsns ");
            sb.Append("WHERE fullname = @emp_nm)) ");
            sb.Append("ORDER BY p.prp_lvs_sdt; ");

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
                            LeavePlanStatus = reader["lvs_sts"] == DBNull.Value ? string.Empty : reader["lvs_sts"].ToString(),

                            IsApproved = reader["is_aprv"] == DBNull.Value ? false : (bool)reader["is_aprv"],
                            ApprovedBy = reader["aprv_by"] == DBNull.Value ? string.Empty : reader["aprv_by"].ToString(),
                            ApprovedTime = reader["aprv_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["aprv_dt"],

                            ProposedStartDate = reader["prp_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_sdt"],
                            ProposedEndDate = reader["prp_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_lvs_edt"],
                            ProposedDuration = reader["prp_lvs_dur"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur"],
                            ProposedDurationTypeId = reader["prp_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["prp_lvs_dur_typ"],
                            ProposedDurationDescription = reader["prp_dur_ds"] == DBNull.Value ? string.Empty : reader["prp_dur_ds"].ToString(),
                            ProposedResumptionDate = reader["prp_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["prp_rsmptn_dt"],

                            ApprovedStartDate = reader["apv_lvs_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_sdt"],
                            ApprovedEndDate = reader["apv_lvs_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_lvs_edt"],
                            ApprovedDuration = reader["apv_lvs_dur"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur"],
                            ApprovedDurationTypeId = reader["apv_lvs_dur_typ"] == DBNull.Value ? 0 : (int)reader["apv_lvs_dur_typ"],
                            ApprovedDurationDescription = reader["apv_dur_ds"] == DBNull.Value ? string.Empty : reader["apv_dur_ds"].ToString(),
                            ApprovedResumptionDate = reader["apv_rsmptn_dt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["apv_rsmptn_dt"],
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
    }
}
