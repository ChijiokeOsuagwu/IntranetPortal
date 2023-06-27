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
    public class UserLoginRepository:IUserLoginRepository
    {
        public IConfiguration _config { get; }
        public UserLoginRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<bool> AddAsync(UserLoginHistory userLoginHistory)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.sct_lgn_hst(lgn_hst_tm, lgn_usr_nm, lgn_usr_id, ");
            sb.Append("lgn_is_scs, lgn_src, lgn_typ) VALUES (@lgn_hst_tm, ");
            sb.Append("@lgn_usr_nm, @lgn_usr_id, @lgn_is_scs, @lgn_src, @lgn_typ); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var lgn_hst_tm = cmd.Parameters.Add("@lgn_hst_tm", NpgsqlDbType.TimestampTz);
                    var lgn_usr_nm = cmd.Parameters.Add("@lgn_usr_nm", NpgsqlDbType.Text);
                    var lgn_usr_id = cmd.Parameters.Add("@lgn_usr_id", NpgsqlDbType.Text);
                    var lgn_is_scs = cmd.Parameters.Add("@lgn_is_scs", NpgsqlDbType.Boolean);
                    var lgn_src = cmd.Parameters.Add("@lgn_src", NpgsqlDbType.Text);
                    var lgn_typ = cmd.Parameters.Add("@lgn_typ", NpgsqlDbType.Integer);

                    cmd.Prepare();
                    lgn_hst_tm.Value = userLoginHistory.LoginTime ?? (object)DBNull.Value;
                    lgn_usr_nm.Value = userLoginHistory.LoginUserName ?? (object)DBNull.Value;
                    lgn_usr_id.Value = userLoginHistory.LoginUserID ?? (object)DBNull.Value;
                    lgn_is_scs.Value = userLoginHistory.LoginIsSucceful;
                    lgn_src.Value = userLoginHistory.LoginSourceInfo ?? (object)DBNull.Value;
                    lgn_typ.Value = (int)userLoginHistory.UserLoginType;

                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception)
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<IList<UserLoginHistory>> GetByYearAndMonthAsync(int loginYear, int loginMonth)
        {
            IList<UserLoginHistory> loginHistories = new List<UserLoginHistory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lgn_hst_id, lgn_hst_tm, lgn_usr_nm, lgn_usr_id,  ");
            sb.Append("lgn_is_scs, lgn_src, lgn_typ FROM public.sct_lgn_hst ");
            sb.Append($"WHERE (date_part('year', lgn_hst_tm) = @year) ");
            sb.Append($"AND (date_part('month', lgn_hst_tm) = @month) ");
            sb.Append($"ORDER BY lgn_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var login_year = cmd.Parameters.Add("@year", NpgsqlDbType.Integer);
                    var login_month = cmd.Parameters.Add("@month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    login_year.Value = loginYear;
                    login_month.Value = loginMonth;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            loginHistories.Add(new UserLoginHistory
                            {
                                ID = reader["lgn_hst_id"] == DBNull.Value ? (long?)null : (long)reader["lgn_hst_id"],
                                LoginTime = reader["lgn_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lgn_hst_tm"],
                                LoginUserName = reader["lgn_usr_nm"] == DBNull.Value ? string.Empty : reader["lgn_usr_nm"].ToString(),
                                LoginUserID = reader["lgn_usr_id"] == DBNull.Value ? string.Empty : reader["lgn_usr_id"].ToString(),
                                LoginIsSucceful = reader["lgn_is_scs"] == DBNull.Value ? false : (bool)reader["lgn_is_scs"],
                                LoginSourceInfo = reader["lgn_src"] == DBNull.Value ? string.Empty : reader["lgn_src"].ToString(),
                                UserLoginType = reader["lgn_typ"] == DBNull.Value ? LoginType.None : (LoginType)reader["lgn_typ"],
                                LoginTimeFormatted = reader["lgn_hst_tm"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["lgn_hst_tm"]).ToLongDateString()} {((DateTime)reader["lgn_hst_tm"]).ToLongTimeString()} GMT",
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
            return loginHistories;
        }

        public async Task<IList<UserLoginHistory>> GetByYearAndMonthAndDayAsync(int loginYear, int loginMonth, int loginDay)
        {
            IList<UserLoginHistory> loginHistories = new List<UserLoginHistory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lgn_hst_id, lgn_hst_tm, lgn_usr_nm, lgn_usr_id,  ");
            sb.Append("lgn_is_scs, lgn_src, lgn_typ FROM public.sct_lgn_hst ");
            sb.Append($"WHERE (date_part('year', lgn_hst_tm) = @year) ");
            sb.Append($"AND (date_part('month', lgn_hst_tm) = @month) ");
            sb.Append($"AND (date_part('day', lgn_hst_tm) = @day) ");
            sb.Append($"ORDER BY lgn_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var login_year = cmd.Parameters.Add("@year", NpgsqlDbType.Integer);
                    var login_month = cmd.Parameters.Add("@month", NpgsqlDbType.Integer);
                    var login_day = cmd.Parameters.Add("@day", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    login_year.Value = loginYear;
                    login_month.Value = loginMonth;
                    login_day.Value = loginDay;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            loginHistories.Add(new UserLoginHistory
                            {
                                ID = reader["lgn_hst_id"] == DBNull.Value ? (long?)null : (long)reader["lgn_hst_id"],
                                LoginTime = reader["lgn_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lgn_hst_tm"],
                                LoginUserName = reader["lgn_usr_nm"] == DBNull.Value ? string.Empty : reader["lgn_usr_nm"].ToString(),
                                LoginUserID = reader["lgn_usr_id"] == DBNull.Value ? string.Empty : reader["lgn_usr_id"].ToString(),
                                LoginIsSucceful = reader["lgn_is_scs"] == DBNull.Value ? false : (bool)reader["lgn_is_scs"],
                                LoginSourceInfo = reader["lgn_src"] == DBNull.Value ? string.Empty : reader["lgn_src"].ToString(),
                                UserLoginType = reader["lgn_typ"] == DBNull.Value ? LoginType.None : (LoginType)reader["lgn_typ"],
                                LoginTimeFormatted = reader["lgn_hst_tm"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["lgn_hst_tm"]).ToLongDateString()} {((DateTime)reader["lgn_hst_tm"]).ToLongTimeString()} GMT",
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
            return loginHistories;
        }

        public async Task<IList<UserLoginHistory>> GetByUserNameAndYearAsync(string loginUserName, int loginYear)
        {
            IList<UserLoginHistory> loginHistories = new List<UserLoginHistory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lgn_hst_id, lgn_hst_tm, lgn_usr_nm, lgn_usr_id,  ");
            sb.Append("lgn_is_scs, lgn_src, lgn_typ FROM public.sct_lgn_hst ");
            sb.Append("WHERE (lgn_usr_nm = @lgn_usr_nm) ");
            sb.Append("AND (date_part('year', lgn_hst_tm) = @year) ");
            sb.Append("ORDER BY lgn_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var login_user_name = cmd.Parameters.Add("@lgn_usr_nm", NpgsqlDbType.Text);
                    var login_year = cmd.Parameters.Add("@year", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    login_user_name.Value = loginUserName;
                    login_year.Value = loginYear;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            loginHistories.Add(new UserLoginHistory
                            {
                                ID = reader["lgn_hst_id"] == DBNull.Value ? (long?)null : (long)reader["lgn_hst_id"],
                                LoginTime = reader["lgn_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lgn_hst_tm"],
                                LoginUserName = reader["lgn_usr_nm"] == DBNull.Value ? string.Empty : reader["lgn_usr_nm"].ToString(),
                                LoginUserID = reader["lgn_usr_id"] == DBNull.Value ? string.Empty : reader["lgn_usr_id"].ToString(),
                                LoginIsSucceful = reader["lgn_is_scs"] == DBNull.Value ? false : (bool)reader["lgn_is_scs"],
                                LoginSourceInfo = reader["lgn_src"] == DBNull.Value ? string.Empty : reader["lgn_src"].ToString(),
                                UserLoginType = reader["lgn_typ"] == DBNull.Value ? LoginType.None : (LoginType)reader["lgn_typ"],
                                LoginTimeFormatted = reader["lgn_hst_tm"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["lgn_hst_tm"]).ToLongDateString()} {((DateTime)reader["lgn_hst_tm"]).ToLongTimeString()} GMT",
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
            return loginHistories;
        }

        public async Task<IList<UserLoginHistory>> GetByUserNameAndYearAndMonthAsync(string loginUserName, int loginYear, int loginMonth)
        {
            IList<UserLoginHistory> loginHistories = new List<UserLoginHistory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lgn_hst_id, lgn_hst_tm, lgn_usr_nm, lgn_usr_id,  ");
            sb.Append("lgn_is_scs, lgn_src, lgn_typ FROM public.sct_lgn_hst ");
            sb.Append("WHERE (lgn_usr_nm = @lgn_usr_nm) ");
            sb.Append("AND (date_part('year', lgn_hst_tm) = @year) ");
            sb.Append("AND (date_part('month', lgn_hst_tm) = @month) ");
            sb.Append("ORDER BY lgn_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var login_user_name = cmd.Parameters.Add("@lgn_usr_nm", NpgsqlDbType.Text);
                    var login_year = cmd.Parameters.Add("@year", NpgsqlDbType.Integer);
                    var login_month = cmd.Parameters.Add("@month", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    login_user_name.Value = loginUserName;
                    login_year.Value = loginYear;
                    login_month.Value = loginMonth;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            loginHistories.Add(new UserLoginHistory
                            {
                                ID = reader["lgn_hst_id"] == DBNull.Value ? (long?)null : (long)reader["lgn_hst_id"],
                                LoginTime = reader["lgn_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lgn_hst_tm"],
                                LoginUserName = reader["lgn_usr_nm"] == DBNull.Value ? string.Empty : reader["lgn_usr_nm"].ToString(),
                                LoginUserID = reader["lgn_usr_id"] == DBNull.Value ? string.Empty : reader["lgn_usr_id"].ToString(),
                                LoginIsSucceful = reader["lgn_is_scs"] == DBNull.Value ? false : (bool)reader["lgn_is_scs"],
                                LoginSourceInfo = reader["lgn_src"] == DBNull.Value ? string.Empty : reader["lgn_src"].ToString(),
                                UserLoginType = reader["lgn_typ"] == DBNull.Value ? LoginType.None : (LoginType)reader["lgn_typ"],
                                LoginTimeFormatted = reader["lgn_hst_tm"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["lgn_hst_tm"]).ToLongDateString()} {((DateTime)reader["lgn_hst_tm"]).ToLongTimeString()} GMT",
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
            return loginHistories;
        }

        public async Task<IList<UserLoginHistory>> GetByUserNameAndYearAndMonthAndDayAsync(string loginUserName, int loginYear, int loginMonth, int loginDay)
        {
            IList<UserLoginHistory> loginHistories = new List<UserLoginHistory>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT lgn_hst_id, lgn_hst_tm, lgn_usr_nm, lgn_usr_id,  ");
            sb.Append("lgn_is_scs, lgn_src, lgn_typ FROM public.sct_lgn_hst ");
            sb.Append("WHERE (lgn_usr_nm = @lgn_usr_nm) ");
            sb.Append("AND (date_part('year', lgn_hst_tm) = @year) ");
            sb.Append($"AND (date_part('month', lgn_hst_tm) = @month) ");
            sb.Append($"AND (date_part('day', lgn_hst_tm) = @day) ");
            sb.Append($"ORDER BY lgn_hst_id DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var login_user_name = cmd.Parameters.Add("@lgn_usr_nm", NpgsqlDbType.Text);
                    var login_year = cmd.Parameters.Add("@year", NpgsqlDbType.Integer);
                    var login_month = cmd.Parameters.Add("@month", NpgsqlDbType.Integer);
                    var login_day = cmd.Parameters.Add("@day", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    login_user_name.Value = loginUserName;
                    login_year.Value = loginYear;
                    login_month.Value = loginMonth;
                    login_day.Value = loginDay;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            loginHistories.Add(new UserLoginHistory
                            {
                                ID = reader["lgn_hst_id"] == DBNull.Value ? (long?)null : (long)reader["lgn_hst_id"],
                                LoginTime = reader["lgn_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["lgn_hst_tm"],
                                LoginUserName = reader["lgn_usr_nm"] == DBNull.Value ? string.Empty : reader["lgn_usr_nm"].ToString(),
                                LoginUserID = reader["lgn_usr_id"] == DBNull.Value ? string.Empty : reader["lgn_usr_id"].ToString(),
                                LoginIsSucceful = reader["lgn_is_scs"] == DBNull.Value ? false : (bool)reader["lgn_is_scs"],
                                LoginSourceInfo = reader["lgn_src"] == DBNull.Value ? string.Empty : reader["lgn_src"].ToString(),
                                UserLoginType = reader["lgn_typ"] == DBNull.Value ? LoginType.None : (LoginType)reader["lgn_typ"],
                                LoginTimeFormatted = reader["lgn_hst_tm"] == DBNull.Value ? string.Empty : $"{((DateTime)reader["lgn_hst_tm"]).ToLongDateString()} {((DateTime)reader["lgn_hst_tm"]).ToLongTimeString()} GMT",
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
            return loginHistories;
        }
    }
}
