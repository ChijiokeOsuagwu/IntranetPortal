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
    public class LeaveProfileDetailRepository : ILeaveProfileDetailRepository
    {
        public IConfiguration _config { get; }
        public LeaveProfileDetailRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //====== Leave Profile Details Action Methods =======//
        #region Leave Profile Details Action Methods
        public async Task<List<LeaveProfileDetail>> GetByProfileIdAsync(int profileId)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.lvs_dur, d.dur_typ, d.is_yrly, d.cancarryover, d.is_mntz, ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("d.carryoverends, CASE WHEN d.carryoverends = 1 THEN 'January' ");
            sb.Append("WHEN d.carryoverends = 2 THEN 'February' ");
            sb.Append("WHEN d.carryoverends = 3 THEN 'March' ");
            sb.Append("WHEN d.carryoverends = 4 THEN 'April' ");
            sb.Append("WHEN d.carryoverends = 5 THEN 'May' ");
            sb.Append("WHEN d.carryoverends = 6 THEN 'June' ");
            sb.Append("WHEN d.carryoverends = 7 THEN 'July' ");
            sb.Append("WHEN d.carryoverends = 8 THEN 'August' ");
            sb.Append("WHEN d.carryoverends = 9 THEN 'September' ");
            sb.Append("WHEN d.carryoverends = 10 THEN 'October' ");
            sb.Append("WHEN d.carryoverends = 11 THEN 'November' ");
            sb.Append("WHEN d.carryoverends = 12 THEN 'December' ");
            sb.Append("END as carryoverends_month ");
            sb.Append("FROM public.lms_pfl_dtls d ");
            sb.Append("WHERE (d.lvs_pfl_id = @lvs_pfl_id) ");
            sb.Append("ORDER BY d.lvs_typ_cd; ");
            query = sb.ToString();
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
                            CarryOverEndMonth = reader["carryoverends"] == DBNull.Value ? 0 : (int)reader["carryoverends"],
                            CarryOverEndMonthName = reader["carryoverends_month"] == DBNull.Value ? string.Empty : reader["carryoverends_month"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetByIdAsync(int id)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.lvs_dur, d.dur_typ, d.is_yrly, d.cancarryover, d.is_mntz, ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("d.carryoverends, CASE WHEN d.carryoverends = 1 THEN 'January' ");
            sb.Append("WHEN d.carryoverends = 2 THEN 'February' ");
            sb.Append("WHEN d.carryoverends = 3 THEN 'March' ");
            sb.Append("WHEN d.carryoverends = 4 THEN 'April' ");
            sb.Append("WHEN d.carryoverends = 5 THEN 'May' ");
            sb.Append("WHEN d.carryoverends = 6 THEN 'June' ");
            sb.Append("WHEN d.carryoverends = 7 THEN 'July' ");
            sb.Append("WHEN d.carryoverends = 8 THEN 'August' ");
            sb.Append("WHEN d.carryoverends = 9 THEN 'September' ");
            sb.Append("WHEN d.carryoverends = 10 THEN 'October' ");
            sb.Append("WHEN d.carryoverends = 11 THEN 'November' ");
            sb.Append("WHEN d.carryoverends = 12 THEN 'December' ");
            sb.Append("END as carryoverends_month ");
            sb.Append("FROM public.lms_pfl_dtls d ");
            sb.Append("WHERE (d.pfl_dtl_id = @pfl_dtl_id); ");
            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var pfl_dtl_id = cmd.Parameters.Add("@pfl_dtl_id", NpgsqlTypes.NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    pfl_dtl_id.Value = id;

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
                            CarryOverEndMonth = reader["carryoverends"] == DBNull.Value ? 0 : (int)reader["carryoverends"],
                            CarryOverEndMonthName = reader["carryoverends_month"] == DBNull.Value ? string.Empty : reader["carryoverends_month"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails;
        }
        public async Task<List<LeaveProfileDetail>> GetByProfileIdnLeaveTypeAsync(int profileId, string leaveTypeCode)
        {
            List<LeaveProfileDetail> leaveProfileDetails = new List<LeaveProfileDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.pfl_dtl_id, d.lvs_pfl_id, d.lvs_typ_cd, ");
            sb.Append("d.lvs_dur, d.dur_typ, d.is_yrly, d.cancarryover, d.is_mntz, ");
            sb.Append("CASE WHEN d.dur_typ = 0 THEN 'Working Day(s)' ");
            sb.Append("WHEN d.dur_typ = 1 THEN 'Day(s)' ");
            sb.Append("WHEN d.dur_typ = 2 THEN 'Week(s)' ");
            sb.Append("WHEN d.dur_typ = 3 THEN 'Month(s)' ");
            sb.Append("WHEN d.dur_typ = 4 THEN 'Year(s)' END as dur_typ_ds, ");
            sb.Append("(SELECT lvs_pfl_nm FROM public.lms_lvs_pfls ");
            sb.Append("WHERE lvs_pfl_id = d.lvs_pfl_id) as lvs_pfl_nm, ");
            sb.Append("(SELECT lvs_typ_nm FROM public.lms_lvs_typs  ");
            sb.Append("WHERE lvs_typ_cd = d.lvs_typ_cd) as lvs_typ_nm,  ");
            sb.Append("d.carryoverends, CASE WHEN d.carryoverends = 1 THEN 'January' ");
            sb.Append("WHEN d.carryoverends = 2 THEN 'February' ");
            sb.Append("WHEN d.carryoverends = 3 THEN 'March' ");
            sb.Append("WHEN d.carryoverends = 4 THEN 'April' ");
            sb.Append("WHEN d.carryoverends = 5 THEN 'May' ");
            sb.Append("WHEN d.carryoverends = 6 THEN 'June' ");
            sb.Append("WHEN d.carryoverends = 7 THEN 'July' ");
            sb.Append("WHEN d.carryoverends = 8 THEN 'August' ");
            sb.Append("WHEN d.carryoverends = 9 THEN 'September' ");
            sb.Append("WHEN d.carryoverends = 10 THEN 'October' ");
            sb.Append("WHEN d.carryoverends = 11 THEN 'November' ");
            sb.Append("WHEN d.carryoverends = 12 THEN 'December' ");
            sb.Append("END as carryoverends_month ");
            sb.Append("FROM public.lms_pfl_dtls d ");
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
                            CarryOverEndMonth = reader["carryoverends"] == DBNull.Value ? 0 : (int)reader["carryoverends"],
                            CarryOverEndMonthName = reader["carryoverends_month"] == DBNull.Value ? string.Empty : reader["carryoverends_month"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return leaveProfileDetails;
        }
        public async Task<bool> AddAsync(LeaveProfileDetail leaveProfileDetail)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.lms_pfl_dtls(lvs_pfl_id, ");
            sb.Append("lvs_typ_cd, is_yrly, cancarryover, is_mntz, ");
            sb.Append("lvs_dur, dur_typ, carryoverends) VALUES (@lvs_pfl_id, ");
            sb.Append("@lvs_typ_cd, @is_yrly, @cancarryover, ");
            sb.Append("@is_mntz, @lvs_dur, @dur_typ, @carryoverends); ");
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
                    var carryoverends = cmd.Parameters.Add("@carryoverends", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    lvs_pfl_id.Value = leaveProfileDetail.ProfileId;
                    lvs_typ_cd.Value = leaveProfileDetail.LeaveTypeCode;
                    is_yrly.Value = leaveProfileDetail.IsYearly;
                    cancarryover.Value = leaveProfileDetail.CanBeCarriedOver;
                    is_mntz.Value = leaveProfileDetail.CanBeMonetized;
                    lvs_dur.Value = leaveProfileDetail.Duration;
                    dur_typ.Value = leaveProfileDetail.DurationTypeId;
                    carryoverends.Value = leaveProfileDetail.CarryOverEndMonth;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            int rows = 0;
            string query = "DELETE FROM public.lms_pfl_dtls WHERE (pfl_dtl_id = @pfl_dtl_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pfl_dtl_id = cmd.Parameters.Add("@pfl_dtl_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pfl_dtl_id.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        public async Task<bool> EditAsync(LeaveProfileDetail leaveProfileDetail)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.lms_pfl_dtls SET lvs_pfl_id=@lvs_pfl_id, ");
            sb.Append("lvs_typ_cd=@lvs_typ_cd, is_yrly=@is_yrly, ");
            sb.Append("cancarryover=@cancarryover, is_mntz=@is_mntz, ");
            sb.Append("lvs_dur=@lvs_dur, dur_typ=@dur_typ, ");
            sb.Append("carryoverends=@carryoverends ");
            sb.Append("WHERE (pfl_dtl_id=@pfl_dtl_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var pfl_dtl_id = cmd.Parameters.Add("@pfl_dtl_id", NpgsqlDbType.Integer);
                    var lvs_pfl_id = cmd.Parameters.Add("@lvs_pfl_id", NpgsqlDbType.Integer);
                    var lvs_typ_cd = cmd.Parameters.Add("@lvs_typ_cd", NpgsqlDbType.Text);
                    var is_yrly = cmd.Parameters.Add("@is_yrly", NpgsqlDbType.Boolean);
                    var cancarryover = cmd.Parameters.Add("@cancarryover", NpgsqlDbType.Boolean);
                    var is_mntz = cmd.Parameters.Add("@is_mntz", NpgsqlDbType.Boolean);
                    var lvs_dur = cmd.Parameters.Add("@lvs_dur", NpgsqlDbType.Integer);
                    var dur_typ = cmd.Parameters.Add("@dur_typ", NpgsqlDbType.Integer);
                    var carryoverends = cmd.Parameters.Add("@carryoverends", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    pfl_dtl_id.Value = leaveProfileDetail.Id;
                    lvs_pfl_id.Value = leaveProfileDetail.ProfileId;
                    lvs_typ_cd.Value = leaveProfileDetail.LeaveTypeCode;
                    is_yrly.Value = leaveProfileDetail.IsYearly;
                    cancarryover.Value = leaveProfileDetail.CanBeCarriedOver;
                    is_mntz.Value = leaveProfileDetail.CanBeMonetized;
                    lvs_dur.Value = leaveProfileDetail.Duration;
                    dur_typ.Value = leaveProfileDetail.DurationTypeId;
                    carryoverends.Value = leaveProfileDetail.CarryOverEndMonth;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }
        #endregion
    }
}
