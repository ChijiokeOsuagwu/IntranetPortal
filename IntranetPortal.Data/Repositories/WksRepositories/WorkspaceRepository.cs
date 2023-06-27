using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Base.Repositories.WksRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.WksRepositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        public IConfiguration _config { get; }
        public WorkspaceRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Workspace Write Actions
        public async Task<bool> AddAsync(Workspace workspace)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_wsp_inf(title, is_main, is_xm, owner_id, ctd, ctb, ");
            sb.Append("is_dlt, dlt_by, dlt_on) VALUES (@title, @is_main, @is_xm, @owner_id, @ctd, ");
            sb.Append(" @ctb, @is_dlt, @dlt_by, @dlt_on);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var is_main = cmd.Parameters.Add("@is_main", NpgsqlDbType.Boolean);
                    var is_xm = cmd.Parameters.Add("@is_xm", NpgsqlDbType.Boolean);
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    var ctd = cmd.Parameters.Add("@ctd", NpgsqlDbType.TimestampTz);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var is_dlt = cmd.Parameters.Add("@is_dlt", NpgsqlDbType.Boolean);
                    var dlt_on = cmd.Parameters.Add("@dlt_on", NpgsqlDbType.TimestampTz);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    title.Value = workspace.Title;
                    is_main.Value = workspace.IsMain;
                    is_xm.Value = workspace.IsExecutiveManagement;
                    owner_id.Value = workspace.OwnerID;
                    ctd.Value = workspace.CreatedTime;
                    ctb.Value = workspace.CreatedBy ?? (object)DBNull.Value;
                    is_dlt.Value = workspace.IsDeleted;
                    dlt_on.Value = workspace.DeletedOn ?? (object)DBNull.Value;
                    dlt_by.Value = workspace.DeletedBy ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateTitleAsync(int workspaceId, string newTitle)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.wkm_wsp_inf SET title = @title WHERE (id = @id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    cmd.Prepare();
                    id.Value = workspaceId;
                    title.Value = newTitle;

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

        public async Task<bool> UpdateToDeletedAsync(int workspaceId, string deletedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_wsp_inf SET is_dlt = true, dlt_by = @dlt_by, ");
            sb.Append("dlt_on = @dlt_on WHERE (id = @id AND is_main = false);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    var dlt_on = cmd.Parameters.Add("@dlt_on", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    id.Value = workspaceId;
                    dlt_by.Value = deletedBy;
                    dlt_on.Value = DateTime.UtcNow;

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
        #endregion

        #region Workspace Read Actions
        public async Task<Workspace> GetByIdAsync(int workspaceId)
        {
            Workspace workspace = new Workspace();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, is_main, is_xm, owner_id, ctd, ctb, is_dlt, dlt_by, ");
            sb.Append("dlt_on FROM public.wkm_wsp_inf WHERE (id = @id AND is_dlt = false);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    id.Value = workspaceId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspace.ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"];
                            workspace.Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString();
                            workspace.IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"];
                            workspace.IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"];
                            workspace.OwnerID = reader["owner_id"] == DBNull.Value ? string.Empty : reader["owner_id"].ToString();
                            workspace.CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"];
                            workspace.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workspace;
        }

        public async Task<Workspace> GetMainByOwnerIdAsync(string ownerId)
        {
            Workspace workspace = new Workspace();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, is_main, is_xm, owner_id, ctd, ctb, is_dlt, dlt_by, ");
            sb.Append("dlt_on FROM public.wkm_wsp_inf WHERE (owner_id = @owner_id ");
            sb.Append("AND is_main = true AND is_dlt = false);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    owner_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspace.ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"];
                            workspace.Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString();
                            workspace.IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"];
                            workspace.IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"];
                            workspace.OwnerID = reader["owner_id"] == DBNull.Value ? string.Empty : reader["owner_id"].ToString();
                            workspace.CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"];
                            workspace.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return workspace;
        }
        
        public async Task<IList<Workspace>> GetByOwnerIdAsync(string ownerId)
        {
            IList<Workspace> workspaces = new List<Workspace>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, is_main, is_xm, owner_id, ctd, ctb, is_dlt, dlt_by, dlt_on ");
            sb.Append("FROM public.wkm_wsp_inf WHERE (owner_id = @owner_id AND is_dlt = false) ");
            sb.Append("ORDER BY id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    owner_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspaces.Add(new Workspace
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"],
                                OwnerID = reader["owner_id"] == DBNull.Value ? string.Empty : reader["owner_id"].ToString(),
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
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
            return workspaces;
        }

        public async Task<IList<Workspace>> GetByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle)
        {
            IList<Workspace> workspaces = new List<Workspace>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, is_main, is_xm, owner_id, ctd, ctb, is_dlt, dlt_by, dlt_on ");
            sb.Append("FROM public.wkm_wsp_inf WHERE (owner_id = @owner_id AND is_dlt = false ");
            sb.Append("AND LOWER(title) = LOWER(@title)) ORDER BY id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    owner_id.Value = ownerId;
                    title.Value = workspaceTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspaces.Add(new Workspace
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"],
                                OwnerID = reader["owner_id"] == DBNull.Value ? string.Empty : reader["owner_id"].ToString(),
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
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
            return workspaces;
        }

        public async Task<IList<Workspace>> SearchByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle)
        {
            IList<Workspace> workspaces = new List<Workspace>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, is_main, is_xm, owner_id, ctd, ctb, is_dlt, dlt_by, dlt_on ");
            sb.Append("FROM public.wkm_wsp_inf WHERE (owner_id = @owner_id AND is_dlt = false ");
            sb.Append("AND LOWER(title) LIKE '%'||LOWER(@title)||'%') ORDER BY id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    owner_id.Value = ownerId;
                    title.Value = workspaceTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspaces.Add(new Workspace
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"],
                                OwnerID = reader["owner_id"] == DBNull.Value ? string.Empty : reader["owner_id"].ToString(),
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
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
            return workspaces;
        }

        #endregion
    }
}
