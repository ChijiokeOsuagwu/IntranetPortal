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
    public class UserRepository : IUserRepository
    {
        public IConfiguration _config { get; }
        public UserRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<IList<ApplicationUser>> GetUsersByLoginIdAsync(string login)
        {
            if (String.IsNullOrEmpty(login)) { throw new ArgumentNullException("The required parameter [LoginID] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.usr_typ, u.usr_afc, u.usr_ccs, ");
            sb.Append("u.usr_mlc, u.lck_enb, u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, ");
            sb.Append("u.coy_cd, u.usr_cb, u.usr_cd, u.usr_md, u.usr_mb, p.id, p.fullname, ");
            sb.Append("p.sex, p.phone1, p.phone2, p.email, p.sex  ");
            sb.Append("FROM public.sct_usr_acct u ");
            sb.Append("INNER JOIN public.gst_prsns p ON u.usr_id = p.id ");
            sb.Append("WHERE LOWER(u.usr_nm) = LOWER(@login) ");
            sb.Append("AND (u.is_dx = false);");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var appUserLogin = cmd.Parameters.Add("@login", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                appUserLogin.Value = login;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? string.Empty : (reader["usr_id"]).ToString(),
                        UserName = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        UserType = reader["usr_typ"] == DBNull.Value ? string.Empty : (reader["usr_typ"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        CompanyCode = reader["coy_cd"] == DBNull.Value ? string.Empty : (reader["coy_cd"]).ToString(),
                        ConcurrencyStamp = reader["usr_ccs"] == DBNull.Value ? string.Empty : (reader["usr_ccs"]).ToString(),
                        EmailConfirmed = reader["usr_mlc"] == DBNull.Value ? false : (bool)reader["usr_mlc"],
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                        PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? string.Empty : reader["usr_md"].ToString(),
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? string.Empty : reader["usr_cd"].ToString(),
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> GetOtherUsersWithSameLoginIdAsync(string userId, string login)
        {
            if (String.IsNullOrEmpty(login)) { throw new ArgumentNullException("The required parameter [LoginID] is null or has an invalid value."); }
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT usr_id, usr_nm, usr_typ, usr_afc, usr_ccs, usr_mlc, ");
            sb.Append("lck_enb, lck_end, usr_pwh, usr_stp, usr_tfe, coy_cd, ");
            sb.Append("usr_cb, usr_cd, usr_md, usr_mb FROM public.sct_usr_acct ");
            sb.Append("WHERE (LOWER(usr_nm) = LOWER(@login)) AND (usr_id <> @usr_id) ");
            sb.Append("AND (is_dx = false);");
            string query = sb.ToString();

            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var appUserLogin = cmd.Parameters.Add("@login", NpgsqlDbType.Text);
                var appUserId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                appUserLogin.Value = login;
                appUserId.Value = userId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    applicationUserList.Add(new ApplicationUser()
                    {
                        Id = reader["usr_id"] == DBNull.Value ? string.Empty : (reader["usr_id"]).ToString(),
                        UserName = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                        UserType = reader["usr_typ"] == DBNull.Value ? string.Empty : (reader["usr_typ"]).ToString(),
                        AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                        CompanyCode = reader["coy_cd"] == DBNull.Value ? string.Empty : (reader["coy_cd"]).ToString(),
                        ConcurrencyStamp = reader["usr_ccs"] == DBNull.Value ? string.Empty : (reader["usr_ccs"]).ToString(),
                        EmailConfirmed = reader["usr_mlc"] == DBNull.Value ? false : (bool)reader["usr_mlc"],
                        LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                        LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                        PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                        SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                        TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],
                        FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                        Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                        PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                        PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                        ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                        ModifiedTime = reader["usr_md"] == DBNull.Value ? string.Empty : reader["usr_md"].ToString(),
                        CreatedTime = reader["usr_cd"] == DBNull.Value ? string.Empty : reader["usr_cd"].ToString(),
                        CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return applicationUserList;
        }

        public async Task<IList<ApplicationUser>> GetUsersByUserIdAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationUser> applicationUserList = new List<ApplicationUser>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.usr_id, u.usr_nm, u.usr_typ, u.usr_afc, u.usr_ccs, ");
            sb.Append("u.usr_mlc, u.lck_enb, u.lck_end, u.usr_pwh, u.usr_stp, u.usr_tfe, ");
            sb.Append("u.coy_cd, u.usr_cb, u.usr_cd, u.usr_md, u.usr_mb, p.id, ");
            sb.Append("p.fullname, p.sex, p.phone1, p.phone2, p.email, p.sex ");
            sb.Append("FROM public.sct_usr_acct u INNER JOIN public.gst_prsns p ");
            sb.Append("ON u.usr_id = p.id WHERE LOWER(u.usr_id)=LOWER(@userId)  ");
            sb.Append("AND (u.is_dx = false);");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var appUserId = cmd.Parameters.Add("@userId", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    appUserId.Value = userId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        applicationUserList.Add(new ApplicationUser()
                        {
                            Id = reader["usr_id"] == DBNull.Value ? string.Empty : (reader["usr_id"]).ToString(),
                            UserName = reader["usr_nm"] == DBNull.Value ? string.Empty : (reader["usr_nm"]).ToString(),
                            UserType = reader["usr_typ"] == DBNull.Value ? string.Empty : (reader["usr_typ"]).ToString(),
                            AccessFailedCount = reader["usr_afc"] == DBNull.Value ? 0 : (int)reader["usr_afc"],
                            CompanyCode = reader["coy_cd"] == DBNull.Value ? string.Empty : (reader["coy_cd"]).ToString(),
                            ConcurrencyStamp = reader["usr_ccs"] == DBNull.Value ? string.Empty : (reader["usr_ccs"]).ToString(),
                            EmailConfirmed = reader["usr_mlc"] == DBNull.Value ? false : (bool)reader["usr_mlc"],
                            LockoutEnabled = reader["lck_enb"] == DBNull.Value ? false : (bool)reader["lck_enb"],
                            LockoutEnd = reader["lck_end"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["lck_end"]),
                            PasswordHash = reader["usr_pwh"] == DBNull.Value ? string.Empty : reader["usr_pwh"].ToString(),
                            SecurityStamp = reader["usr_stp"] == DBNull.Value ? string.Empty : reader["usr_stp"].ToString(),
                            TwoFactorEnabled = reader["usr_tfe"] == DBNull.Value ? false : (bool)reader["usr_tfe"],

                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["email"] == DBNull.Value ? string.Empty : reader["email"].ToString(),
                            ModifiedBy = reader["usr_mb"] == DBNull.Value ? string.Empty : reader["usr_mb"].ToString(),
                            ModifiedTime = reader["usr_md"] == DBNull.Value ? string.Empty : reader["usr_md"].ToString(),
                            CreatedTime = reader["usr_cd"] == DBNull.Value ? string.Empty : reader["usr_cd"].ToString(),
                            CreatedBy = reader["usr_cb"] == DBNull.Value ? string.Empty : reader["usr_cb"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                applicationUserList = null;
            }
            return applicationUserList;
        }

        public async Task<bool> AddUserAccountAsync(ApplicationUser applicationUser)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO public.sct_usr_acct(usr_id, usr_nm, usr_typ, usr_afc, usr_ccs, usr_mlc, lck_enb, ");
            sb.Append($"lck_end, usr_pwh, usr_stp, usr_tfe, coy_cd, usr_cb, usr_cd, usr_md, usr_mb) VALUES (@usr_id, ");
            sb.Append($"@usr_nm, @usr_typ, @usr_afc, @usr_ccs, @usr_mlc, @lck_enb, @lck_end, @usr_pwh, @usr_stp, ");
            sb.Append($"@usr_tfe, @coy_cd, @usr_cb, @usr_cd, @usr_md, @usr_mb); ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var userId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    var loginId = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                    var userType = cmd.Parameters.Add("@usr_typ", NpgsqlDbType.Text);
                    var accessFailedCount = cmd.Parameters.Add("@usr_afc", NpgsqlDbType.Integer);
                    var concurrencyStamp = cmd.Parameters.Add("@usr_ccs", NpgsqlDbType.Text);
                    var emailConfirmed = cmd.Parameters.Add("@usr_mlc", NpgsqlDbType.Boolean);
                    var lockEnabled = cmd.Parameters.Add("@lck_enb", NpgsqlDbType.Boolean);
                    var lockEnd = cmd.Parameters.Add("@lck_end", NpgsqlDbType.TimestampTz);
                    var passwordHash = cmd.Parameters.Add("@usr_pwh", NpgsqlDbType.Text);
                    var securityStamp = cmd.Parameters.Add("@usr_stp", NpgsqlDbType.Text);
                    var twoFactorEnabled = cmd.Parameters.Add("@usr_tfe", NpgsqlDbType.Boolean);
                    var companyCode = cmd.Parameters.Add("@coy_cd", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@usr_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@usr_md", NpgsqlDbType.Text);
                    var createdBy = cmd.Parameters.Add("@usr_cb", NpgsqlDbType.Text);
                    var createdDate = cmd.Parameters.Add("@usr_cd", NpgsqlDbType.Text);

                    cmd.Prepare();
                    userId.Value = applicationUser.Id;
                    loginId.Value = applicationUser.UserName;
                    userType.Value = applicationUser.UserType;
                    accessFailedCount.Value = applicationUser.AccessFailedCount;
                    concurrencyStamp.Value = applicationUser.ConcurrencyStamp ?? (object)DBNull.Value;
                    emailConfirmed.Value = applicationUser.EmailConfirmed;
                    lockEnabled.Value = applicationUser.LockoutEnabled;
                    lockEnd.Value = applicationUser.LockoutEnd ?? (object)DBNull.Value;
                    passwordHash.Value = applicationUser.PasswordHash ?? (object)DBNull.Value;
                    securityStamp.Value = applicationUser.SecurityStamp ?? (object)DBNull.Value;
                    twoFactorEnabled.Value = applicationUser.TwoFactorEnabled;
                    companyCode.Value = applicationUser.CompanyCode ?? (object)DBNull.Value;
                    modifiedBy.Value = applicationUser.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = applicationUser.ModifiedTime ?? (object)DBNull.Value;
                    createdBy.Value = applicationUser.CreatedBy ?? (object)DBNull.Value;
                    createdDate.Value = applicationUser.CreatedTime ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateUserAccountAsync(ApplicationUser applicationUser)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.sct_usr_acct	SET usr_nm=@usr_nm, lck_enb=@lck_enb, lck_end=lck_end, usr_md=@usr_md, ");
            sb.Append($"usr_mb=@usr_mb  WHERE usr_id=@usr_id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var userId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    var loginId = cmd.Parameters.Add("@usr_nm", NpgsqlDbType.Text);
                    var lockEnabled = cmd.Parameters.Add("@lck_enb", NpgsqlDbType.Boolean);
                    var lockEnd = cmd.Parameters.Add("@lck_end", NpgsqlDbType.TimestampTz);
                    var modifiedBy = cmd.Parameters.Add("@usr_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@usr_md", NpgsqlDbType.Text);

                    cmd.Prepare();
                    userId.Value = applicationUser.Id;
                    loginId.Value = applicationUser.UserName;
                    lockEnabled.Value = applicationUser.LockoutEnabled;
                    lockEnd.Value = applicationUser.LockoutEnd ?? (object)DBNull.Value;
                    modifiedBy.Value = applicationUser.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = applicationUser.ModifiedTime ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateUserPasswordAsync(ApplicationUser applicationUser)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE public.sct_usr_acct SET usr_pwh=@usr_pwh, usr_md=@usr_md, usr_mb=@usr_mb ");
            sb.Append($"WHERE usr_id = @usr_id;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var userId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    var passwordHash = cmd.Parameters.Add("@usr_pwh", NpgsqlDbType.Text);
                    var modifiedBy = cmd.Parameters.Add("@usr_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@usr_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    userId.Value = applicationUser.Id;
                    passwordHash.Value = applicationUser.PasswordHash ?? (object)DBNull.Value;
                    modifiedBy.Value = applicationUser.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = applicationUser.ModifiedTime ?? (object)DBNull.Value;

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


        public async Task<bool> UpdateUserActivationAsync(string userId, string modifiedBy, bool deactivate)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.sct_usr_acct SET is_dx = @is_dx, dx_dt = @dx_dt, ");
            sb.Append("usr_md=@usr_md, usr_mb=@usr_mb, dx_by = @dx_by  ");
            sb.Append("WHERE usr_id = @usr_id;");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                var is_dx = cmd.Parameters.Add("@is_dx", NpgsqlDbType.Boolean);
                var usr_mb = cmd.Parameters.Add("@usr_mb", NpgsqlDbType.Text);
                var usr_md = cmd.Parameters.Add("@usr_md", NpgsqlDbType.Text);
                var dx_by = cmd.Parameters.Add("@dx_by", NpgsqlDbType.Text);
                var dx_dt = cmd.Parameters.Add("@dx_dt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                usr_id.Value = userId;
                is_dx.Value = deactivate;
                usr_mb.Value = modifiedBy ?? (object)DBNull.Value;
                usr_md.Value = DateTime.UtcNow;
                dx_by.Value = modifiedBy ?? (object)DBNull.Value;
                dx_dt.Value = DateTime.UtcNow; ;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }


        public async Task<bool> DeleteUserAccountByIdAsync(string userId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"DELETE FROM public.sct_usr_acct	WHERE (usr_id = @usr_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var Id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);

                    cmd.Prepare();
                    Id.Value = userId;
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
    }
}
