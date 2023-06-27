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
    public class ProjectFolderRepository : IProjectFolderRepository
    {
        public IConfiguration _config { get; }
        public ProjectFolderRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Project Folders Write Actions
        public async Task<bool> AddAsync(ProjectFolder projectFolder)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_fdr_inf(title, wsp_id, ctd, is_archived, is_dlt, ");
            sb.Append("dlt_on, dlt_by, ctb, owner_id, descr) VALUES (@title, @wsp_id, @ctd, ");
            sb.Append(" @is_archived, @is_dlt, @dlt_on, @dlt_by, @ctb, @owner_id, @descr);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var descr = cmd.Parameters.Add("@descr", NpgsqlDbType.Text);
                    var wsp_id = cmd.Parameters.Add("@wsp_id", NpgsqlDbType.Integer);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    var ctd = cmd.Parameters.Add("@ctd", NpgsqlDbType.TimestampTz);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var is_dlt = cmd.Parameters.Add("@is_dlt", NpgsqlDbType.Boolean);
                    var dlt_on = cmd.Parameters.Add("@dlt_on", NpgsqlDbType.TimestampTz);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    title.Value = projectFolder.Title;
                    descr.Value = projectFolder.Description;
                    wsp_id.Value = projectFolder.WorkspaceID;
                    is_archived.Value = projectFolder.IsArchived;
                    owner_id.Value = projectFolder.OwnerID;
                    ctd.Value = projectFolder.CreatedTime;
                    ctb.Value = projectFolder.CreatedBy ?? (object)DBNull.Value;
                    is_dlt.Value = projectFolder.IsDeleted;
                    dlt_on.Value = projectFolder.DeletedOn ?? (object)DBNull.Value;
                    dlt_by.Value = projectFolder.DeletedBy ?? (object)DBNull.Value;

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

        public async Task<bool> UpdateAsync(ProjectFolder projectFolder)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_fdr_inf SET title=@title, is_archived=@is_archived, ");
            sb.Append("descr=@descr WHERE (id = @id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var descr = cmd.Parameters.Add("@descr", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    id.Value = projectFolder.ID;
                    title.Value = projectFolder.Title;
                    descr.Value = projectFolder.Description;
                    is_archived.Value = projectFolder.IsArchived;

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

        public async Task<bool> UpdateToDeletedAsync(int projectFolderId, string deletedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_fdr_inf SET is_dlt=true, dlt_by=@dlt_by, ");
            sb.Append("dlt_on=@dlt_on WHERE (id = @id);");
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
                    id.Value = projectFolderId;
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

        #region Project Folders Read Actions
        public async Task<ProjectFolder> GetByIdAsync(int projectFolderId)
        {
            ProjectFolder projectFolder = new ProjectFolder();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id ");
            sb.Append("WHERE (f.id = @id AND f.is_dlt = false);");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    id.Value = projectFolderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolder.ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"];
                            projectFolder.Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString();
                            projectFolder.Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString();
                            projectFolder.WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"];
                            projectFolder.WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString();
                            projectFolder.InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"];
                            projectFolder.IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"];
                            projectFolder.IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"];
                            projectFolder.OwnerID = reader["owner_id"] == DBNull.Value ? string.Empty : reader["owner_id"].ToString();
                            projectFolder.CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"];
                            projectFolder.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return projectFolder;
        }

        public async Task<List<ProjectFolder>> GetByTitleAndOwnerIdAsync(string folderTitle, string ownerId)
        {
            List<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id ");
            sb.Append("WHERE (f.is_dlt = false) AND ");
            sb.Append("LOWER(f.owner_id) = LOWER(@owner_id) AND LOWER(f.title) = LOWER(@title);");

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
                    title.Value = folderTitle;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> GetByWorkspaceIdAsync(int workspaceId)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id ");
            sb.Append("WHERE (f.wsp_id = @wsp_id AND f.is_dlt = false) ");
            sb.Append("ORDER BY f.id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wsp_id = cmd.Parameters.Add("@wsp_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    wsp_id.Value = workspaceId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> GetByOwnerIdAsync(string ownerId)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id ");
            sb.Append("WHERE (LOWER(f.owner_id) = LOWER(@owner_id) AND f.is_dlt = false) ");
            sb.Append("ORDER BY f.id;");

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
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> GetByOwnerIdAsync(string ownerId, bool isArchived)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id WHERE (f.is_archived = @is_archived ");
            sb.Append("AND LOWER(f.owner_id) = LOWER(@owner_id) AND f.is_dlt = false) ");
            sb.Append("ORDER BY f.id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    owner_id.Value = ownerId;
                    is_archived.Value = is_archived;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> GetByWorkspaceIdAsync(int workspaceId, bool isArchived)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id WHERE (f.wsp_id = @wsp_id ");
            sb.Append("AND f.is_archived = @is_archived AND f.is_dlt = false) ");
            sb.Append("ORDER BY f.id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wsp_id = cmd.Parameters.Add("@wsp_id", NpgsqlDbType.Integer);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    wsp_id.Value = workspaceId;
                    is_archived.Value = isArchived;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> GetMainFoldersByOwnerIdAsync(string ownerId)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id ");
            sb.Append("WHERE (f.owner_id = @owner_id AND w.is_main = true ");
            sb.Append("AND f.is_dlt = false) ORDER BY f.id;");

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
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> GetMainFoldersByOwnerIdAsync(string ownerId, bool isArchived)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id WHERE (f.owner_id = @owner_id ");
            sb.Append("AND w.is_main = true AND f.is_archived = @is_archived ");
            sb.Append("AND f.is_dlt = false) ORDER BY f.id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_id = cmd.Parameters.Add("@owner_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    owner_id.Value = ownerId;
                    is_archived.Value = isArchived;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> SearchByWorkspaceIdAndTitleAsync(int workspaceId, string folderTitle)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id WHERE (f.wsp_id = @wsp_id AND f.is_dlt = false ");
            sb.Append("AND LOWER(f.title) LIKE '%'||LOWER(@title)||'%') ");
            sb.Append("ORDER BY f.id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wsp_id = cmd.Parameters.Add("@wsp_id", NpgsqlDbType.Integer);
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    wsp_id.Value = workspaceId;
                    title.Value = folderTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        public async Task<IList<ProjectFolder>> SearchByWorkspaceIdAndTitleAsync(int workspaceId, string folderTitle, bool isArchived)
        {
            IList<ProjectFolder> projectFolders = new List<ProjectFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.id, f.title, f.wsp_id, f.ctd, f.is_archived, f.is_dlt, ");
            sb.Append("f.ctb, f.owner_id, f.descr, w.title AS wks_title, w.is_main ");
            sb.Append("FROM public.wkm_fdr_inf f INNER JOIN public.wkm_wsp_inf w ");
            sb.Append("ON f.wsp_id = w.id WHERE (f.wsp_id = @wsp_id ");
            sb.Append("AND f.is_archived = @is_archived AND f.is_dlt = false ");
            sb.Append("AND LOWER(f.title) LIKE '%'||LOWER(@title)||'%') ");
            sb.Append("ORDER BY f.id;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wsp_id = cmd.Parameters.Add("@wsp_id", NpgsqlDbType.Integer);
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    wsp_id.Value = workspaceId;
                    title.Value = folderTitle;
                    is_archived.Value = isArchived;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            projectFolders.Add(new ProjectFolder
                            {
                                ID = reader["id"] == DBNull.Value ? 0 : (int)reader["id"],
                                Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                                Description = reader["descr"] == DBNull.Value ? string.Empty : reader["descr"].ToString(),
                                WorkspaceID = reader["wsp_id"] == DBNull.Value ? 0 : (int)reader["wsp_id"],
                                WorkspaceTitle = reader["wks_title"] == DBNull.Value ? string.Empty : reader["wks_title"].ToString(),
                                InMainWorkspace = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsDeleted = reader["is_dlt"] == DBNull.Value ? false : (bool)reader["is_dlt"],
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
            return projectFolders;
        }

        #endregion
    }
}
