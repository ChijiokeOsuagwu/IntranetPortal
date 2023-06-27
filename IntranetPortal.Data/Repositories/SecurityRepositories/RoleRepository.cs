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
    public class RoleRepository : IRoleRepository
    {
        public IConfiguration _config { get; }
        public RoleRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IList<ApplicationRole>> GetUnGrantedRolesByUserIdAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationRole> applicationRoleList = new List<ApplicationRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT rl_id, rl_nm, rl_ds, rl_app, rl_rk, a.app_ds ");
            sb.Append("FROM public.sct_usr_rls r INNER JOIN public.sct_usr_apps a ");
            sb.Append("ON a.app_cd = r.rl_app WHERE (rl_app != 'SYS') ");
            sb.Append("AND rl_id NOT IN (SELECT usr_rls_id ");
            sb.Append("FROM public.sct_usr_pms WHERE LOWER(usr_acct_id) = LOWER(@usr_id));");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    usr_id.Value = userId;
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        applicationRoleList.Add(new ApplicationRole()
                        {
                            Id = reader["rl_id"] == DBNull.Value ? string.Empty : (reader["rl_id"]).ToString(),
                            Name = reader["rl_nm"] == DBNull.Value ? string.Empty : (reader["rl_nm"]).ToString(),
                            Description = reader["rl_ds"] == DBNull.Value ? string.Empty : (reader["rl_ds"]).ToString(),
                            ApplicationID = reader["rl_app"] == DBNull.Value ? string.Empty : reader["rl_app"].ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                            RoleRank = reader["rl_rk"] == DBNull.Value ? 999 : (int)reader["rl_rk"],
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
            return applicationRoleList;
        }

        public async Task<IList<ApplicationRole>> GetUnGrantedRolesByUserIdAndApplicationIdAsync(string userId, string applicationId)
        {
            if (String.IsNullOrEmpty(userId)) { throw new ArgumentNullException("The required parameter [UserID] is null or has an invalid value."); }

            List<ApplicationRole> applicationRoleList = new List<ApplicationRole>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.rl_id, r.rl_nm, r.rl_ds, r.rl_app, r.rl_rk, a.app_ds ");
            sb.Append("FROM public.sct_usr_rls r INNER JOIN public.sct_usr_apps a ");
            sb.Append("ON a.app_cd = r.rl_app WHERE (r.rl_app = @app_id) ");
            sb.Append("AND (r.rl_app != 'SYS') AND r.rl_id NOT IN (SELECT usr_rls_id ");
            sb.Append("FROM public.sct_usr_pms WHERE LOWER(usr_acct_id) = LOWER(@usr_id));");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var usr_id = cmd.Parameters.Add("@usr_id", NpgsqlDbType.Text);
                    var app_id = cmd.Parameters.Add("@app_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    usr_id.Value = userId;
                    app_id.Value = applicationId; 
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        applicationRoleList.Add(new ApplicationRole()
                        {
                            Id = reader["rl_id"] == DBNull.Value ? string.Empty : (reader["rl_id"]).ToString(),
                            Name = reader["rl_nm"] == DBNull.Value ? string.Empty : (reader["rl_nm"]).ToString(),
                            Description = reader["rl_ds"] == DBNull.Value ? string.Empty : (reader["rl_ds"]).ToString(),
                            ApplicationID = reader["rl_app"] == DBNull.Value ? string.Empty : reader["rl_app"].ToString(),
                            ApplicationName = reader["app_ds"] == DBNull.Value ? string.Empty : reader["app_ds"].ToString(),
                            RoleRank = reader["rl_rk"] == DBNull.Value ? 999 : (int)reader["rl_rk"],
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
            return applicationRoleList;
        }
    }
}
