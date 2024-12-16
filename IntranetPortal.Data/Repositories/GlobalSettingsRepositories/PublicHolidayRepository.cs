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
    public class PublicHolidayRepository : IPublicHolidayRepository
    {
        public IConfiguration _config { get; }
        public PublicHolidayRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //====== Public Holidays Action Methods =======//
        #region Public Holidays Action Methods

        public async Task<bool> AddAsync(PublicHoliday publicHoliday)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_pub_hdys(hol_dy_nm, hol_dy_rsn, ");
            sb.Append("hol_dy_typ, hol_dy_sdt, hol_dy_edt, no_dys, hol_dy_yr) ");
            sb.Append("VALUES (@hol_dy_nm, @hol_dy_rsn, @hol_dy_typ, @hol_dy_sdt, ");
            sb.Append("@hol_dy_edt, @no_dys, @hol_dy_yr); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var hol_dy_nm = cmd.Parameters.Add("@hol_dy_nm", NpgsqlDbType.Text);
                    var hol_dy_rsn = cmd.Parameters.Add("@hol_dy_rsn", NpgsqlDbType.Text);
                    var hol_dy_typ = cmd.Parameters.Add("@hol_dy_typ", NpgsqlDbType.Text);
                    var hol_dy_sdt = cmd.Parameters.Add("@hol_dy_sdt", NpgsqlDbType.Timestamp);
                    var hol_dy_edt = cmd.Parameters.Add("@hol_dy_edt", NpgsqlDbType.Timestamp);
                    var no_dys = cmd.Parameters.Add("@no_dys", NpgsqlDbType.Integer);
                    var hol_dy_yr = cmd.Parameters.Add("@hol_dy_yr", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    hol_dy_nm.Value = publicHoliday.Name;
                    hol_dy_rsn.Value = publicHoliday.Reason ?? (object)DBNull.Value;
                    hol_dy_typ.Value = publicHoliday.Type ?? (object)DBNull.Value;
                    hol_dy_sdt.Value = publicHoliday.StartDate; 
                    hol_dy_edt.Value = publicHoliday.EndDate; 
                    no_dys.Value = publicHoliday.NoOfDays;
                    hol_dy_yr.Value = publicHoliday.HolidayYear;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            int rows = 0;
            string query = "DELETE FROM public.gst_pub_hdys WHERE (hol_dy_id = @hol_dy_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var hol_dy_id = cmd.Parameters.Add("@hol_dy_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    hol_dy_id.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<bool> EditAsync(PublicHoliday publicHoliday)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_pub_hdys SET hol_dy_nm=@hol_dy_nm, ");
            sb.Append("hol_dy_rsn=@hol_dy_rsn, hol_dy_typ=@hol_dy_typ, ");
            sb.Append("hol_dy_sdt=@hol_dy_sdt, hol_dy_edt=@hol_dy_edt, ");
            sb.Append("no_dys=@no_dys, hol_dy_yr=@hol_dy_yr ");
            sb.Append("WHERE (hol_dy_id=@hol_dy_id);");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var hol_dy_id = cmd.Parameters.Add("@hol_dy_id", NpgsqlDbType.Integer);
                    var hol_dy_nm = cmd.Parameters.Add("@hol_dy_nm", NpgsqlDbType.Text);
                    var hol_dy_rsn = cmd.Parameters.Add("@hol_dy_rsn", NpgsqlDbType.Text);
                    var hol_dy_typ = cmd.Parameters.Add("@hol_dy_typ", NpgsqlDbType.Text);
                    var hol_dy_sdt = cmd.Parameters.Add("@hol_dy_sdt", NpgsqlDbType.Timestamp);
                    var hol_dy_edt = cmd.Parameters.Add("@hol_dy_edt", NpgsqlDbType.Timestamp);
                    var no_dys = cmd.Parameters.Add("@no_dys", NpgsqlDbType.Integer);
                    var hol_dy_yr = cmd.Parameters.Add("@hol_dy_yr", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    hol_dy_id.Value = publicHoliday.Id;
                    hol_dy_nm.Value = publicHoliday.Name;
                    hol_dy_rsn.Value = publicHoliday.Reason ?? (object)DBNull.Value;
                    hol_dy_typ.Value = publicHoliday.Type ?? (object)DBNull.Value;
                    hol_dy_sdt.Value = publicHoliday.StartDate;
                    hol_dy_edt.Value = publicHoliday.EndDate;
                    no_dys.Value = publicHoliday.NoOfDays;
                    hol_dy_yr.Value = publicHoliday.HolidayYear;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            return rows > 0;
        }

        public async Task<PublicHoliday> GetByIdAsync(int id)
        {
            PublicHoliday holiday = new PublicHoliday();
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (id < 1) { return null; }
            sb.Append("SELECT hol_dy_id, hol_dy_nm, hol_dy_rsn, ");
            sb.Append("hol_dy_typ, hol_dy_sdt, hol_dy_edt, ");
            sb.Append("no_dys, hol_dy_yr FROM public.gst_pub_hdys ");
            sb.Append("WHERE (hol_dy_id = @hol_dy_id);");
            query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var hol_dy_id = cmd.Parameters.Add("@hol_dy_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    hol_dy_id.Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            holiday.Id = reader["hol_dy_id"] == DBNull.Value ? 0 : (int)(reader["hol_dy_id"]);
                            holiday.Name = reader["hol_dy_nm"] == DBNull.Value ? string.Empty : reader["hol_dy_nm"].ToString();
                            holiday.Reason = reader["hol_dy_rsn"] == DBNull.Value ? string.Empty : reader["hol_dy_rsn"].ToString();
                            holiday.Type = reader["hol_dy_typ"] == DBNull.Value ? string.Empty : reader["hol_dy_typ"].ToString();
                            holiday.StartDate = reader["hol_dy_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hol_dy_sdt"];
                            holiday.EndDate = reader["hol_dy_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hol_dy_edt"];
                            holiday.NoOfDays = reader["no_dys"] == DBNull.Value ? 0 : (int)reader["no_dys"];
                            holiday.HolidayYear = reader["hol_dy_yr"] == DBNull.Value ? 0 : (int)reader["hol_dy_yr"];
                        }
                }
                await conn.CloseAsync();
            }
            return holiday;
        }

        public async Task<List<PublicHoliday>> GetByYearAsync(int year)
        {
            List<PublicHoliday> holidays = new List<PublicHoliday>();
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT hol_dy_id, hol_dy_nm, hol_dy_rsn, ");
            sb.Append("hol_dy_typ, hol_dy_sdt, hol_dy_edt, ");
            sb.Append("no_dys, hol_dy_yr FROM public.gst_pub_hdys ");
            sb.Append("WHERE (hol_dy_yr = @hol_dy_yr) ");
            sb.Append("ORDER BY hol_dy_sdt; ");
            query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var hol_dy_yr = cmd.Parameters.Add("@hol_dy_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    hol_dy_yr.Value = year;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        holidays.Add(new PublicHoliday()
                        {
                            Id = reader["hol_dy_id"] == DBNull.Value ? 0 : (int)(reader["hol_dy_id"]),
                            Name = reader["hol_dy_nm"] == DBNull.Value ? string.Empty : reader["hol_dy_nm"].ToString(),
                            Reason = reader["hol_dy_rsn"] == DBNull.Value ? string.Empty : reader["hol_dy_rsn"].ToString(),
                            Type = reader["hol_dy_typ"] == DBNull.Value ? string.Empty : reader["hol_dy_typ"].ToString(),
                            StartDate = reader["hol_dy_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hol_dy_sdt"],
                            EndDate = reader["hol_dy_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hol_dy_edt"],
                            NoOfDays = reader["no_dys"] == DBNull.Value ? 0 : (int)reader["no_dys"],
                            HolidayYear = reader["hol_dy_yr"] == DBNull.Value ? 0 : (int)reader["hol_dy_yr"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return holidays;
        }

        public async Task<List<PublicHoliday>> GetByDateRangeAsync(DateTime StartDate, DateTime EndDate)
        {
            if(StartDate == null || EndDate == null) { throw new Exception("Required parameter Date Range cannot be null."); }
            List<PublicHoliday> holidays = new List<PublicHoliday>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT hol_dy_id, hol_dy_nm, hol_dy_rsn, ");
            sb.Append("hol_dy_typ, hol_dy_sdt, hol_dy_edt, ");
            sb.Append("no_dys, hol_dy_yr FROM public.gst_pub_hdys ");
            sb.Append("WHERE (@start_date BETWEEN hol_dy_sdt AND hol_dy_edt) ");
            sb.Append("OR (@end_date BETWEEN hol_dy_sdt AND hol_dy_edt) ");
            sb.Append("ORDER BY hol_dy_sdt; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var start_date = cmd.Parameters.Add("@start_date", NpgsqlDbType.TimestampTz);
                    var end_date = cmd.Parameters.Add("@end_date", NpgsqlDbType.TimestampTz);
                    await cmd.PrepareAsync();
                    start_date.Value = StartDate;
                    end_date.Value = EndDate;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        holidays.Add(new PublicHoliday()
                        {
                            Id = reader["hol_dy_id"] == DBNull.Value ? 0 : (int)(reader["hol_dy_id"]),
                            Name = reader["hol_dy_nm"] == DBNull.Value ? string.Empty : reader["hol_dy_nm"].ToString(),
                            Reason = reader["hol_dy_rsn"] == DBNull.Value ? string.Empty : reader["hol_dy_rsn"].ToString(),
                            Type = reader["hol_dy_typ"] == DBNull.Value ? string.Empty : reader["hol_dy_typ"].ToString(),
                            StartDate = reader["hol_dy_sdt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hol_dy_sdt"],
                            EndDate = reader["hol_dy_edt"] == DBNull.Value ? DateTime.Now : (DateTime)reader["hol_dy_edt"],
                            NoOfDays = reader["no_dys"] == DBNull.Value ? 0 : (int)reader["no_dys"],
                            HolidayYear = reader["hol_dy_yr"] == DBNull.Value ? 0 : (int)reader["hol_dy_yr"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return holidays;
        }

        public async Task<List<PublicHoliday>> GetAllAsync()
        {
            List<PublicHoliday> holidays = new List<PublicHoliday>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT hol_dy_id, hol_dy_nm, hol_dy_rsn, ");
            sb.Append("hol_dy_typ, hol_dy_sdt, hol_dy_edt, ");
            sb.Append("no_dys, hol_dy_yr FROM public.gst_pub_hdys ");
            sb.Append("ORDER BY hol_dy_sdt; ");
            string query = sb.ToString();
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
                        holidays.Add(new PublicHoliday()
                        {
                            Id = reader["hol_dy_id"] == DBNull.Value ? 0 : (int)(reader["hol_dy_id"]),
                            Name = reader["hol_dy_nm"] == DBNull.Value ? String.Empty : reader["hol_dy_nm"].ToString(),
                            Reason = reader["hol_dy_rsn"] == DBNull.Value ? String.Empty : reader["hol_dy_rsn"].ToString(),
                            Type = reader["hol_dy_typ"] == DBNull.Value ? String.Empty : reader["hol_dy_typ"].ToString(),
                            StartDate = reader["hol_dy_sdt"] == DBNull.Value ? DateTime.Today : (DateTime)reader["hol_dy_sdt"],
                            EndDate = reader["hol_dy_edt"] == DBNull.Value ? DateTime.Today : (DateTime)reader["hol_dy_edt"],
                            NoOfDays = reader["no_dys"] == DBNull.Value ? 0 : (int)reader["no_dys"],
                            HolidayYear = reader["hol_dy_yr"] == DBNull.Value ? 0 : (int)reader["hol_dy_yr"],
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return holidays;
        }

        #endregion

        //====== Public Holiday Type Action Methods ========//
        #region Public Holiday Types Action Methods
        public async Task<IList<HolidayType>> GetPublicHolidayTypesAsync()
        {
            List<HolidayType> holidayTypes = new List<HolidayType>();
            string query = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT hdy_typ_id, hdy_typ_nm, hdy_typ_ds ");
            sb.Append("FROM public.gst_hdy_typs ");
            sb.Append("ORDER BY hdy_typ_nm; ");
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
                        holidayTypes.Add(new HolidayType()
                        {
                            Id = reader["hdy_typ_id"] == DBNull.Value ? 0 : (int)(reader["hdy_typ_id"]),
                            Name = reader["hdy_typ_nm"] == DBNull.Value ? string.Empty : reader["hdy_typ_nm"].ToString(),
                            Description = reader["hdy_typ_ds"] == DBNull.Value ? string.Empty : reader["hdy_typ_ds"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return holidayTypes;
        }

        #endregion
    }
}
