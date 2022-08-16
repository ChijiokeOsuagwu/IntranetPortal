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
    public class UserPermissionRepository : IUserPermissionRepository
    {
        public IConfiguration _config { get; }
        public UserPermissionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<bool> AddUserPermissionAsync(string userId, string roleId, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            //sb.Append("INSERT INTO public.sct_usr_pms(usr_acct_id, usr_rls_id, usr_pms_mb, usr_pms_md) VALUES ");
            //sb.Append("(@usr_acct_id, @usr_rls_id, @usr_pms_mb, @usr_pms_md);");

            sb.Append("INSERT INTO public.sct_usr_pms(usr_pms_id, usr_acct_id, usr_rls_id, usr_pms_mb, usr_pms_md) ");
            sb.Append("VALUES (@usr_pms_id, @usr_acct_id, @usr_rls_id, @usr_pms_mb, @usr_pms_md);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var userPermissionId = cmd.Parameters.Add("@usr_pms_id", NpgsqlDbType.Text);
                    var userAccountId = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                    var userRoleId = cmd.Parameters.Add("@usr_rls_id", NpgsqlDbType.Text);
                    var actionBy = cmd.Parameters.Add("@usr_pms_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@usr_pms_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    userPermissionId.Value = Guid.NewGuid().ToString();
                    userAccountId.Value = userId;
                    userRoleId.Value = roleId;
                    actionBy.Value = modifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value;
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

        public async Task<bool> DeleteUserPermissionAsync(string userId, string roleId, string modifiedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.sct_usr_pms WHERE (usr_acct_id = @usr_acct_id) AND (usr_rls_id = @usr_rls_id) ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var userAccountId = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                    var userRoleId = cmd.Parameters.Add("@usr_rls_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    userAccountId.Value = userId;
                    userRoleId.Value = roleId;
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
        
        public async Task<bool> DeleteUserPermissionAsync(int userPermissionId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.sct_usr_pms WHERE (usr_pms_id = @usr_pms_id) ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var permissionId = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    permissionId.Value = userPermissionId;
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

        public async Task<bool> DeleteUserPermissionByUserIdAsync(string userId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.sct_usr_pms WHERE (usr_acct_id = @usr_acct_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var userAccountId = cmd.Parameters.Add("@usr_acct_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    userAccountId.Value = userId;
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

        public async Task<IList<UserPermission>> GetUserPermissionsByUserIdAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [User ID] is null or has an invalid value."); }

            List<UserPermission> permissionList = new List<UserPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT p.usr_pms_id, p.usr_acct_id, p.usr_rls_id, p.usr_pms_mb, p.usr_pms_md, r.rl_nm, ");
            sb.Append($"r.rl_ds, r.rl_app, r.rl_rk, a.app_ds FROM public.sct_usr_pms p INNER JOIN public.sct_usr_rls r ");
            sb.Append($"ON r.rl_id = p.usr_rls_id INNER JOIN public.sct_usr_apps a ON r.rl_app = a.app_cd ");
            sb.Append($"WHERE (p.usr_acct_id = @usr_id) ORDER BY r.rl_app, r.rl_rk;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var appUserId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    appUserId.Value = userId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        permissionList.Add(new UserPermission()
                        {
                            PermissionID = reader["usr_pms_id"] == DBNull.Value ? string.Empty : reader["usr_pms_id"].ToString(),
                            UserID = reader["usr_acct_id"] == DBNull.Value ? string.Empty : (reader["usr_acct_id"]).ToString(),
                            RoleID = reader["usr_rls_id"] == DBNull.Value ? string.Empty : (reader["usr_rls_id"]).ToString(),
                            RoleName = reader["rl_nm"] == DBNull.Value ? string.Empty : (reader["rl_nm"]).ToString(),
                            RoleDescription = reader["rl_ds"] == DBNull.Value ? string.Empty : (reader["rl_ds"]).ToString(),
                            ApplicationID = reader["rl_app"] == DBNull.Value ? string.Empty : (reader["rl_app"]).ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : (reader["app_ds"]).ToString(),
                            ModifiedBy = reader["usr_pms_mb"] == DBNull.Value ? string.Empty : reader["usr_pms_mb"].ToString(),
                            ModifiedDate = reader["usr_pms_md"] == DBNull.Value ? string.Empty : reader["usr_pms_md"].ToString(),
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
            return permissionList;
        }

        public async Task<IList<UserPermission>> GetUserPermissionsByUserIdAndApplicationIdAsync(string userId, string applicationId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [User ID] is null or has an invalid value."); }
            if (String.IsNullOrEmpty(applicationId)) { throw new ArgumentNullException("The required parameter [Application ID] is null or has an invalid value."); }

            List<UserPermission> permissionList = new List<UserPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT p.usr_pms_id, p.usr_acct_id, p.usr_rls_id, p.usr_pms_mb, p.usr_pms_md, r.rl_nm, ");
            sb.Append($"r.rl_ds, r.rl_app, r.rl_rk, a.app_ds FROM public.sct_usr_pms p INNER JOIN public.sct_usr_rls r ");
            sb.Append($"ON r.rl_id = p.usr_rls_id INNER JOIN public.sct_usr_apps a ON r.rl_app = a.app_cd ");
            sb.Append($"WHERE (p.usr_acct_id = @usr_id) AND (a.app_cd = @app_id) ");
            sb.Append($"ORDER BY r.rl_app, r.rl_rk;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var appUserId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    var appId = cmd.Parameters.Add("@app_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    appUserId.Value = userId;
                    appId.Value = applicationId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        permissionList.Add(new UserPermission()
                        {
                            PermissionID = reader["usr_pms_id"] == DBNull.Value ? string.Empty : reader["usr_pms_id"].ToString(),
                            UserID = reader["usr_acct_id"] == DBNull.Value ? string.Empty : (reader["usr_acct_id"]).ToString(),
                            RoleID = reader["usr_rls_id"] == DBNull.Value ? string.Empty : (reader["usr_rls_id"]).ToString(),
                            RoleName = reader["rl_nm"] == DBNull.Value ? string.Empty : (reader["rl_nm"]).ToString(),
                            RoleDescription = reader["rl_ds"] == DBNull.Value ? string.Empty : (reader["rl_ds"]).ToString(),
                            ApplicationID = reader["rl_app"] == DBNull.Value ? string.Empty : (reader["rl_app"]).ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : (reader["app_ds"]).ToString(),
                            ModifiedBy = reader["usr_pms_mb"] == DBNull.Value ? string.Empty : reader["usr_pms_mb"].ToString(),
                            ModifiedDate = reader["usr_pms_md"] == DBNull.Value ? string.Empty : reader["usr_pms_md"].ToString(),
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
            return permissionList;
        }
  
        public async Task<IList<UserPermission>> GetUserPermissionsByUserIdAndRoleIdAsync(string userId, string roleId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [User ID] is null or has an invalid value."); }
            if (String.IsNullOrEmpty(roleId)) { throw new ArgumentNullException("The required parameter [Role ID] is null or has an invalid value."); }

            List<UserPermission> permissionList = new List<UserPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT p.usr_pms_id, p.usr_acct_id, p.usr_rls_id, p.usr_pms_mb, p.usr_pms_md, r.rl_nm, ");
            sb.Append($"r.rl_ds, r.rl_app, r.rl_rk, a.app_ds FROM public.sct_usr_pms p INNER JOIN public.sct_usr_rls r ");
            sb.Append($"ON r.rl_id = p.usr_rls_id INNER JOIN public.sct_usr_apps a ON r.rl_app = a.app_cd ");
            sb.Append($"WHERE (p.usr_acct_id = @usr_id) AND (p.usr_rls_id = @usr_rls_id) ");
            sb.Append($"AND (r.rl_app != 'SYS') ORDER BY r.rl_app, r.rl_rk;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var appUserId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    var appRoleId = cmd.Parameters.Add("@usr_rls_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    appUserId.Value = userId;
                    appRoleId.Value = roleId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        permissionList.Add(new UserPermission()
                        {
                            PermissionID = reader["usr_pms_id"] == DBNull.Value ? string.Empty : reader["usr_pms_id"].ToString(),
                            UserID = reader["usr_acct_id"] == DBNull.Value ? string.Empty : (reader["usr_acct_id"]).ToString(),
                            RoleID = reader["usr_rls_id"] == DBNull.Value ? string.Empty : (reader["usr_rls_id"]).ToString(),
                            RoleName = reader["rl_nm"] == DBNull.Value ? string.Empty : (reader["rl_nm"]).ToString(),
                            RoleDescription = reader["rl_ds"] == DBNull.Value ? string.Empty : (reader["rl_ds"]).ToString(),
                            ApplicationID = reader["rl_app"] == DBNull.Value ? string.Empty : (reader["rl_app"]).ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : (reader["app_ds"]).ToString(),
                            ModifiedBy = reader["usr_pms_mb"] == DBNull.Value ? string.Empty : reader["usr_pms_mb"].ToString(),
                            ModifiedDate = reader["usr_pms_md"] == DBNull.Value ? string.Empty : reader["usr_pms_md"].ToString(),
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
            return permissionList;
        }

        public async Task<IList<UserPermission>> GetFullUserPermissionsByUserIdAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [User ID] is null or has an invalid value."); }

            List<UserPermission> permissionList = new List<UserPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT a.app_cd, a.app_ds, r.rl_id, r.rl_nm, r.rl_rk, p.usr_pms_id, p.usr_acct_id, ");
            sb.Append($"CASE WHEN p.usr_pms_id > 0 THEN true ELSE false END as is_grtd ");
            sb.Append($"FROM public.sct_usr_apps a INNER JOIN public.sct_usr_rls r ON a.app_cd = r.rl_app ");
            sb.Append($"LEFT OUTER JOIN public.sct_usr_pms p ON r.rl_id = p.usr_rls_id ");
            sb.Append($"WHERE (p.usr_acct_id = @usr_id OR p.usr_acct_id IS NULL) ");
            sb.Append($" AND (a.app_cd != 'SYS') ORDER BY a.app_cd, r.rl_rk;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var appUserId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    appUserId.Value = userId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        permissionList.Add(new UserPermission()
                        {
                            PermissionID = reader["usr_pms_id"] == DBNull.Value ? string.Empty : reader["usr_pms_id"].ToString(),
                            UserID = reader["usr_acct_id"] == DBNull.Value ? string.Empty : (reader["usr_acct_id"]).ToString(),
                            RoleID = reader["usr_rls_id"] == DBNull.Value ? string.Empty : (reader["usr_rls_id"]).ToString(),
                            RoleName = reader["rl_nm"] == DBNull.Value ? string.Empty : (reader["rl_nm"]).ToString(),
                            RoleDescription = reader["rl_ds"] == DBNull.Value ? string.Empty : (reader["rl_ds"]).ToString(),
                            ApplicationID = reader["rl_app"] == DBNull.Value ? string.Empty : (reader["rl_app"]).ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : (reader["app_ds"]).ToString(),
                            IsGranted = reader["is_grtd"] == DBNull.Value ? false : (bool)reader["is_grtd"],
                            ModifiedBy = reader["usr_pms_mb"] == DBNull.Value ? string.Empty : reader["usr_pms_mb"].ToString(),
                            ModifiedDate = reader["usr_pms_md"] == DBNull.Value ? string.Empty : reader["usr_pms_md"].ToString(),
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
            return permissionList;
        }

        public async Task<IList<UserPermission>> GetFullUserPermissionsByUserIdAndApplicationIdAsync(string userId, string applicationId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [User ID] is null or has an invalid value."); }
            if (String.IsNullOrEmpty(applicationId)) { throw new ArgumentNullException("The required parameter [Application ID] is null or has an invalid value."); }

            List<UserPermission> permissionList = new List<UserPermission>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT a.app_cd, a.app_ds, r.rl_id, r.rl_nm, r.rl_rk, p.usr_pms_id, p.usr_acct_id, ");
            sb.Append($"CASE WHEN p.usr_pms_id IS NOT NULL THEN true ELSE false END as is_grtd ");
            sb.Append($"FROM public.sct_usr_apps a INNER JOIN public.sct_usr_rls r ON a.app_cd = r.rl_app ");
            sb.Append($"LEFT OUTER JOIN public.sct_usr_pms p ON r.rl_id = p.usr_rls_id ");
            sb.Append($"WHERE (p.usr_acct_id = @usr_id OR p.usr_acct_id IS NULL) AND (a.app_cd = @app_id) ");
            sb.Append($" AND (a.app_cd != 'SYS') ORDER BY a.app_cd, r.rl_rk;");

            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var appUserId = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    var appId = cmd.Parameters.Add("@app_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    appUserId.Value = userId;
                    appId.Value = applicationId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        permissionList.Add(new UserPermission()
                        {
                            PermissionID = reader["usr_pms_id"] == DBNull.Value ? string.Empty : reader["usr_pms_id"].ToString(),
                            UserID = reader["usr_acct_id"] == DBNull.Value ? string.Empty : (reader["usr_acct_id"]).ToString(),
                            RoleID = reader["rl_id"] == DBNull.Value ? string.Empty : (reader["rl_id"]).ToString(),
                            RoleName = reader["rl_nm"] == DBNull.Value ? string.Empty : (reader["rl_nm"]).ToString(),
                            ApplicationID = reader["app_cd"] == DBNull.Value ? string.Empty : (reader["app_cd"]).ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : (reader["app_ds"]).ToString(),
                            IsGranted = reader["is_grtd"] == DBNull.Value ? false : (bool)reader["is_grtd"]
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
            return permissionList;
        }
    }
}
