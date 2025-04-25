using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Base.Repositories.WspRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.WspRepositories
{
    public class DeskspaceRepository : IDeskspaceRepository
    {
        public IConfiguration _config { get; }
        public DeskspaceRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Workspace Repository
        #region Workspace Write Actions
        public async Task<long> AddWorkspaceAsync(Workspace workspace)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_wki_drw(drw_nm, is_main, ");
            sb.Append("is_xm, emp_id, ctd, ctb) VALUES (@drw_nm, ");
            sb.Append("@is_main, @is_xm, @emp_id, @ctd, @ctb) ");
            sb.Append("RETURNING drw_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var drw_nm = cmd.Parameters.Add("@drw_nm", NpgsqlDbType.Text);
                    var is_main = cmd.Parameters.Add("@is_main", NpgsqlDbType.Boolean);
                    var is_xm = cmd.Parameters.Add("@is_xm", NpgsqlDbType.Boolean);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var ctd = cmd.Parameters.Add("@ctd", NpgsqlDbType.Timestamp);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    cmd.Prepare();
                    drw_nm.Value = workspace.Title;
                    is_main.Value = workspace.IsMain;
                    is_xm.Value = workspace.IsExecutiveManagement;
                    emp_id.Value = workspace.OwnerID;
                    ctd.Value = workspace.CreatedTime ?? DateTime.Now;
                    ctb.Value = workspace.CreatedBy ?? (object)DBNull.Value;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                    await conn.CloseAsync();
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateWorkspaceAsync(long workspaceId, string newTitle)
        {
            int rows = 0;
            string query = "UPDATE public.wsp_wki_drw SET drw_nm=@drw_nm WHERE (drw_id=@drw_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var drw_id = cmd.Parameters.Add("@drw_id", NpgsqlDbType.Bigint);
                    var drw_nm = cmd.Parameters.Add("@drw_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    drw_id.Value = workspaceId;
                    drw_nm.Value = newTitle;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
                await conn.CloseAsync();
            }

            return rows > 0;
        }
        public async Task<bool> DeleteWorkspaceAsync(long workspaceId)
        {
            int rows = 0;
            string query = "DELETE FROM public.wsp_wki_drw WHERE (drw_id=@drw_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var drw_id = cmd.Parameters.Add("@drw_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    drw_id.Value = workspaceId;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }

            }
            return rows > 0;
        }
        #endregion

        #region Workspace Read Actions
        public async Task<Workspace> GetWorkspaceByIdAsync(long workspaceId)
        {
            Workspace workspace = new Workspace();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT drw_id, drw_nm, is_main, is_xm, emp_id, ctd, ctb ");
            sb.Append("FROM public.wsp_wki_drw WHERE (drw_id=@drw_id);");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var drw_id = cmd.Parameters.Add("@drw_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    drw_id.Value = workspaceId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspace.Id = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"];
                            workspace.Title = reader["drw_nm"] == DBNull.Value ? "" : reader["drw_nm"].ToString();
                            workspace.IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"];
                            workspace.IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"];
                            workspace.OwnerID = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                            workspace.CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"];
                            workspace.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                        }
                }
                await conn.CloseAsync();
            }


            return workspace;
        }
        public async Task<Workspace> GetMainWorkspaceByOwnerIdAsync(string ownerId)
        {
            Workspace workspace = new Workspace();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT drw_id, drw_nm, is_main, is_xm, emp_id, ctd, ctb ");
            sb.Append("FROM public.wsp_wki_drw WHERE (emp_id=@emp_id ");
            sb.Append("AND is_main = true);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspace.Id = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"];
                            workspace.Title = reader["drw_nm"] == DBNull.Value ? "" : reader["drw_nm"].ToString();
                            workspace.IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"];
                            workspace.IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"];
                            workspace.OwnerID = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                            workspace.CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"];
                            workspace.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            workspace.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return workspace;
        }
        public async Task<List<Workspace>> GetWorkspacesByOwnerIdAsync(string ownerId)
        {
            List<Workspace> workspaces = new List<Workspace>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT drw_id, drw_nm, is_main, is_xm, emp_id, ctd, ctb ");
            sb.Append("FROM public.wsp_wki_drw WHERE (emp_id=@emp_id) ");
            sb.Append("ORDER BY drw_id;");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspaces.Add(new Workspace
                            {
                                Id = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                Title = reader["drw_nm"] == DBNull.Value ? "" : reader["drw_nm"].ToString(),
                                IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"],
                                OwnerID = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return workspaces;
        }
        public async Task<List<Workspace>> GetWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle)
        {
            List<Workspace> workspaces = new List<Workspace>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT drw_id, drw_nm, is_main, is_xm, emp_id, ctd, ctb ");
            sb.Append("FROM public.wsp_wki_drw WHERE (emp_id=@emp_id) ");
            sb.Append("AND LOWER(drw_nm) = LOWER(@drw_nm) ");
            sb.Append("ORDER BY drw_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var drw_nm = cmd.Parameters.Add("@drw_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    drw_nm.Value = workspaceTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspaces.Add(new Workspace
                            {
                                Id = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                Title = reader["drw_nm"] == DBNull.Value ? "" : reader["drw_nm"].ToString(),
                                IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"],
                                OwnerID = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return workspaces;
        }
        public async Task<List<Workspace>> SearchWorkspacesByOwnerIdAndTitleAsync(string ownerId, string workspaceTitle)
        {
            List<Workspace> workspaces = new List<Workspace>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT drw_id, drw_nm, is_main, is_xm, emp_id, ctd, ctb ");
            sb.Append("FROM public.wsp_wki_drw WHERE (emp_id=@emp_id) ");
            sb.Append("AND LOWER(drw_nm) LIKE '%'||LOWER(@drw_nm)||'%') ");
            sb.Append("ORDER BY drw_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var drw_nm = cmd.Parameters.Add("@drw_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    drw_nm.Value = workspaceTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            workspaces.Add(new Workspace
                            {
                                Id = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                Title = reader["drw_nm"] == DBNull.Value ? "" : reader["drw_nm"].ToString(),
                                IsMain = reader["is_main"] == DBNull.Value ? false : (bool)reader["is_main"],
                                IsExecutiveManagement = reader["is_xm"] == DBNull.Value ? false : (bool)reader["is_xm"],
                                OwnerID = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return workspaces;
        }

        #endregion

        #endregion

        #region Work Item Folders Repository

        #region Work Item Folders Write Actions
        public async Task<long> AddWorkItemFolderAsync(WorkItemFolder w)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_wki_fdr(wki_fdr_nm, wki_fdr_typ, ");
            sb.Append("is_reusable, is_archived, archived_on, ctd, ctb, emp_id, ");
            sb.Append("is_lckd, drw_id, dt_frm, dt_to, mdd, mdb) VALUES ");
            sb.Append("(@wki_fdr_nm, @wki_fdr_typ, @is_reusable, @is_archived, ");
            sb.Append("@archived_on, @ctd, @ctb, @emp_id, @is_lckd, @drw_id, ");
            sb.Append("@dt_frm, @dt_to, @ctd, @ctb) RETURNING wki_fdr_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_nm = cmd.Parameters.Add("@wki_fdr_nm", NpgsqlDbType.Text);
                    var wki_fdr_typ = cmd.Parameters.Add("@wki_fdr_typ", NpgsqlDbType.Integer);
                    var is_reusable = cmd.Parameters.Add("@is_reusable", NpgsqlDbType.Boolean);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var archived_on = cmd.Parameters.Add("@archived_on", NpgsqlDbType.Timestamp);
                    var ctd = cmd.Parameters.Add("@ctd", NpgsqlDbType.Timestamp);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_lckd = cmd.Parameters.Add("@is_lckd", NpgsqlDbType.Boolean);
                    var drw_id = cmd.Parameters.Add("@drw_id", NpgsqlDbType.Bigint);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    wki_fdr_nm.Value = w.Title;
                    wki_fdr_typ.Value = w.FolderTypeId;
                    is_reusable.Value = w.IsReuseable;
                    is_archived.Value = w.IsArchived;
                    archived_on.Value = w.ArchivedTime ?? (object)DBNull.Value;
                    ctd.Value = w.CreatedTime ?? DateTime.Now;
                    ctb.Value = w.CreatedBy ?? (object)DBNull.Value;
                    emp_id.Value = w.OwnerId;
                    is_lckd.Value = w.IsLocked;
                    drw_id.Value = w.WorkspaceId ?? (object)DBNull.Value;
                    dt_frm.Value = w.PeriodStartDate ?? (object)DBNull.Value;
                    dt_to.Value = w.PeriodEndDate ?? (object)DBNull.Value;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateWorkItemFolderAsync(WorkItemFolder w)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_wki_fdr SET wki_fdr_nm=@wki_fdr_nm, ");
            sb.Append("dt_frm=@dt_frm, dt_to=@dt_to, mdd=@mdd, mdb=@mdb ");
            sb.Append("WHERE (wki_fdr_id=@wki_fdr_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var wki_fdr_nm = cmd.Parameters.Add("@wki_fdr_nm", NpgsqlDbType.Text);
                    var mdd = cmd.Parameters.Add("@mdd", NpgsqlDbType.Timestamp);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    wki_fdr_id.Value = w.Id;
                    wki_fdr_nm.Value = w.Title;
                    mdd.Value = w.UpdatedTime ?? DateTime.Now;
                    mdb.Value = w.UpdatedBy ?? (object)DBNull.Value;
                    dt_frm.Value = w.PeriodStartDate ?? (object)DBNull.Value;
                    dt_to.Value = w.PeriodEndDate ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateWorkItemFolderArchiveAsync(long workItemFolderId, bool isArchived)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_wki_fdr SET is_archived = @is_archived, ");
            sb.Append("archived_on = @archived_on ");
            sb.Append("WHERE (wki_fdr_id = @wki_fdr_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var archived_on = cmd.Parameters.Add("@archived_on", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    wki_fdr_id.Value = workItemFolderId;
                    is_archived.Value = isArchived;
                    archived_on.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateWorkItemFolderLockStatusAsync(long workItemFolderId, bool isLocked)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_wki_fdr SET is_lckd = @is_lckd  ");
            sb.Append("WHERE (wki_fdr_id = @wki_fdr_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var is_lckd = cmd.Parameters.Add("@is_lckd", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    wki_fdr_id.Value = workItemFolderId;
                    is_lckd.Value = isLocked;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteWorkItemFoldersAsync(long workItemFolderId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wsp_fdr_sbms WHERE (wki_fdr_id = @wki_fdr_id); ");
            sb.Append("DELETE FROM public.wsp_eval_smr WHERE (wki_fdr_id = @wki_fdr_id); ");
            sb.Append("DELETE FROM public.wsp_eval_hdr WHERE (wki_fdr_id = @wki_fdr_id); ");
            sb.Append("DELETE FROM public.wsp_eval_rtns WHERE (wki_fdr_id = @wki_fdr_id); ");
            sb.Append("DELETE FROM public.wsp_wki_nts WHERE (wki_fdr_id = @wki_fdr_id); ");
            sb.Append("DELETE FROM public.wsp_wki_hst WHERE (wki_fdr_id = @wki_fdr_id); ");
            sb.Append("DELETE FROM public.wsp_wki_fdr WHERE (wki_fdr_id = @wki_fdr_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    wki_fdr_id.Value = workItemFolderId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #region Work Item Folder Read Actions
        public async Task<WorkItemFolder> GetWorkItemFolderByIdAsync(long workItemFolderId)
        {
            WorkItemFolder w = new WorkItemFolder();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.wki_fdr_id = @wki_fdr_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = workItemFolderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            w.Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"];
                            w.Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString();
                            w.FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"];
                            w.FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"];
                            w.FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString();
                            w.IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"];
                            w.IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"];
                            w.ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"];
                            w.IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"];
                            w.OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                            w.OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();
                            w.WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"];
                            w.PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"];
                            w.PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"];
                            w.CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"];
                            w.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            w.UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"];
                            w.UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();

                        }
                }
                await conn.CloseAsync();
            }
            return w;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAsync(string ownerId)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusAsync(string ownerId, bool isArchived)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) AND (f.is_archived = @is_archived) ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    is_archived.Value = isArchived;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnCreatedYearAsync(string ownerId, int createdYear)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) AND (date_part('year', f.ctd) = @ctd_yr)  ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    ctd_yr.Value = createdYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusnCreatedYearAsync(string ownerId, bool isArchived, int createdYear)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) AND (date_part('year', f.ctd) = @ctd_yr)  ");
            sb.Append("AND (f.is_archived = @is_archived) ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                emp_id.Value = ownerId;
                is_archived.Value = isArchived;
                ctd_yr.Value = createdYear;

                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        folderList.Add(new WorkItemFolder
                        {
                            Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                            Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                            FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                            FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                            FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                            IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                            IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                            ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                            IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                            OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                            OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                            WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                            PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                            PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                            CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                            UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                        });
                    }
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnCreatedYearnCreatedMonthAsync(string ownerId, int createdYear, int createdMonth)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) AND (date_part('year', f.ctd) = @ctd_yr)  ");
            sb.Append("AND (date_part('month', f.ctd) = @ctd_mn) ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                    var ctd_mn = cmd.Parameters.Add("@ctd_mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    ctd_yr.Value = createdYear;
                    ctd_mn.Value = createdMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchivedStatusnCreaatedYearnCreatedMonthAsync(string ownerId, bool isArchived, int createdYear, int createdMonth)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) AND (date_part('year', f.ctd) = @ctd_yr)  ");
            sb.Append("AND (date_part('month', f.ctd) = @ctd_mn) ");
            sb.Append("AND (f.is_archived = @is_archived)  ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                    var ctd_mn = cmd.Parameters.Add("@ctd_mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    is_archived.Value = isArchived;
                    ctd_yr.Value = createdYear;
                    ctd_mn.Value = createdMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchiveStatusnArchivedDateAsync(string ownerId, bool isArchived, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            if (fromDate == null) { fromDate = DateTime.Now.AddMonths(-6); }
            if (toDate == null) { toDate = DateTime.Now.AddDays(1); }
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) AND (f.archived_on >= @from_date)  ");
            sb.Append("AND (f.archived_on <= @to_date) ");
            sb.Append("AND (f.is_archived = @is_archived)  ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var from_date = cmd.Parameters.Add("@from_date", NpgsqlDbType.Timestamp);
                    var to_date = cmd.Parameters.Add("@to_date", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    is_archived.Value = isArchived;
                    from_date.Value = fromDate;
                    to_date.Value = toDate;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return folderList;
        }
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdAndFolderTitleAsync(string ownerId, string folderTitle)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id)  ");
            sb.Append("AND LOWER(f.wki_fdr_nm) = LOWER(@wki_fdr_nm)  ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var wki_fdr_nm = cmd.Parameters.Add("@wki_fdr_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    wki_fdr_nm.Value = folderTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
            }
            return folderList;
        }


        //========= Get By Dates =================//
        public async Task<List<WorkItemFolder>> GetWorkItemFoldersByOwnerIdnArchivednDatesAsync(string ownerId, bool IsArchived, DateTime? startDate = null, DateTime? endDate = null)
        {
            List<WorkItemFolder> folderList = new List<WorkItemFolder>();
            DateTime start_date = startDate ?? DateTime.Now.AddMonths(-3);
            DateTime end_date = endDate ?? DateTime.Now.AddMonths(+3);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT f.wki_fdr_id, f.wki_fdr_nm, f.wki_fdr_typ, f.is_reusable, ");
            sb.Append("f.is_archived, f.archived_on, f.ctd, f.ctb, f.emp_id, f.is_lckd, ");
            sb.Append("f.drw_id, f.dt_frm, f.dt_to, f.mdd, f.mdb, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm, ");
            sb.Append("(SELECT drw_nm FROM public.wsp_wki_drw WHERE drw_id = f.drw_id) as drw_nm, ");
            sb.Append("CASE WHEN f.wki_fdr_typ = 0 THEN 'Task Folder' ");
            sb.Append("WHEN f.wki_fdr_typ = 1 THEN 'Project Folder' ");
            sb.Append("ELSE 'Unknown Folder' END as wki_fdr_typ_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = f.emp_id) as emp_nm ");
            sb.Append("FROM public.wsp_wki_fdr f ");
            sb.Append("WHERE (f.emp_id = @emp_id) AND (f.is_archived = @is_archived) ");
            sb.Append("AND (f.dt_frm >= @dt_frm) AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY f.wki_fdr_id DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    emp_id.Value = ownerId;
                    is_archived.Value = IsArchived;
                    dt_frm.Value = start_date;
                    dt_to.Value = end_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderList.Add(new WorkItemFolder
                            {
                                Id = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                Title = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                FolderTypeId = reader["wki_fdr_typ"] == DBNull.Value ? 0 : (int)reader["wki_fdr_typ"],
                                FolderType = reader["wki_fdr_typ"] == DBNull.Value ? WorkItemFolderType.TaskFolder : (WorkItemFolderType)reader["wki_fdr_typ"],
                                FolderTypeDescription = reader["wki_fdr_typ_ds"] == DBNull.Value ? "" : reader["wki_fdr_typ_ds"].ToString(),
                                IsReuseable = reader["is_reusable"] == DBNull.Value ? false : (bool)reader["is_reusable"],
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                ArchivedTime = reader["archived_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["archived_on"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                OwnerId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                OwnerName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                WorkspaceId = reader["drw_id"] == DBNull.Value ? 0 : (long)reader["drw_id"],
                                PeriodStartDate = reader["dt_frm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_frm"],
                                PeriodEndDate = reader["dt_to"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_to"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                UpdatedTime = reader["mdd"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdd"],
                                UpdatedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            });
                        }
                }
            }
            return folderList;
        }

        #endregion

        #endregion

        #region Work Item Note Action Methods
        public async Task<List<WorkItemNote>> GetWorkItemNotesByFolderIdAsync(long folderId)
        {
            List<WorkItemNote> folderNotes = new List<WorkItemNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT nts_id, nts_tm, nts_ds, nts_by, tsk_id, ");
            sb.Append("wki_fdr_id, prj_id, is_ccl, ccl_by, ccl_dt ");
            sb.Append("FROM public.wsp_wki_nts ");
            sb.Append("WHERE (wki_fdr_id = @wki_fdr_id ");
            sb.Append("AND wki_fdr_id IS NOT NULL) ");
            sb.Append("ORDER BY nts_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = folderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderNotes.Add(new WorkItemNote
                            {
                                TaskItemId = reader["tsk_id"] == DBNull.Value ? 0 : (long)reader["tsk_id"],
                                WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                ProjectId = reader["prj_id"] == DBNull.Value ? 0 : (long)reader["prj_id"],
                                NoteId = reader["nts_id"] == DBNull.Value ? 0 : (long)reader["nts_id"],
                                NoteTime = reader["nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["nts_tm"],
                                NoteContent = reader["nts_ds"] == DBNull.Value ? string.Empty : reader["nts_ds"].ToString(),
                                NoteWrittenBy = reader["nts_by"] == DBNull.Value ? string.Empty : reader["nts_by"].ToString(),
                                IsCancelled = reader["is_ccl"] == DBNull.Value ? false : (bool)reader["is_ccl"],
                                CancelledBy = reader["ccl_by"] == DBNull.Value ? "" : reader["ccl_by"].ToString(),
                                CancelledOn = reader["ccl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ccl_dt"],
                            });
                        }
                }
            }
            return folderNotes;
        }
        public async Task<List<WorkItemNote>> GetWorkItemNotesByTaskIdAsync(long taskId)
        {
            List<WorkItemNote> folderNotes = new List<WorkItemNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT nts_id, nts_tm, nts_ds, nts_by, tsk_id, ");
            sb.Append("wki_fdr_id, prj_id, is_ccl, ccl_by, ccl_dt ");
            sb.Append("FROM public.wsp_wki_nts ");
            sb.Append("WHERE (tsk_id = @tsk_id ");
            sb.Append("AND tsk_id IS NOT NULL) ");
            sb.Append("ORDER BY nts_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_id = cmd.Parameters.Add("@tsk_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tsk_id.Value = taskId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderNotes.Add(new WorkItemNote
                            {
                                TaskItemId = reader["tsk_id"] == DBNull.Value ? 0 : (long)reader["tsk_id"],
                                WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                ProjectId = reader["prj_id"] == DBNull.Value ? 0 : (long)reader["prj_id"],
                                NoteId = reader["nts_id"] == DBNull.Value ? 0 : (long)reader["nts_id"],
                                NoteTime = reader["nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["nts_tm"],
                                NoteContent = reader["nts_ds"] == DBNull.Value ? string.Empty : reader["nts_ds"].ToString(),
                                NoteWrittenBy = reader["nts_by"] == DBNull.Value ? string.Empty : reader["nts_by"].ToString(),
                                IsCancelled = reader["is_ccl"] == DBNull.Value ? false : (bool)reader["is_ccl"],
                                CancelledBy = reader["ccl_by"] == DBNull.Value ? "" : reader["ccl_by"].ToString(),
                                CancelledOn = reader["ccl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ccl_dt"],
                            });
                        }
                }
            }
            return folderNotes;
        }
        public async Task<List<WorkItemNote>> GetWorkItemNotesByProjectIdAsync(long projectId)
        {
            List<WorkItemNote> folderNotes = new List<WorkItemNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT nts_id, nts_tm, nts_ds, nts_by, tsk_id, ");
            sb.Append("wki_fdr_id, prj_id, is_ccl, ccl_by, ccl_dt ");
            sb.Append("FROM public.wsp_wki_nts ");
            sb.Append("WHERE (prj_id = @prj_id ");
            sb.Append("AND prj_id IS NOT NULL) ");
            sb.Append("ORDER BY nts_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var prj_id = cmd.Parameters.Add("@prj_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    prj_id.Value = projectId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            folderNotes.Add(new WorkItemNote
                            {
                                TaskItemId = reader["tsk_id"] == DBNull.Value ? 0 : (long)reader["tsk_id"],
                                WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                ProjectId = reader["prj_id"] == DBNull.Value ? 0 : (long)reader["prj_id"],
                                NoteId = reader["nts_id"] == DBNull.Value ? 0 : (long)reader["nts_id"],
                                NoteTime = reader["nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["nts_tm"],
                                NoteContent = reader["nts_ds"] == DBNull.Value ? string.Empty : reader["nts_ds"].ToString(),
                                NoteWrittenBy = reader["nts_by"] == DBNull.Value ? string.Empty : reader["nts_by"].ToString(),
                                IsCancelled = reader["is_ccl"] == DBNull.Value ? false : (bool)reader["is_ccl"],
                                CancelledBy = reader["ccl_by"] == DBNull.Value ? "" : reader["ccl_by"].ToString(),
                                CancelledOn = reader["ccl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ccl_dt"],
                            });
                        }
                }
            }
            return folderNotes;
        }
        public async Task<bool> AddNoteAsync(WorkItemNote n)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_wki_nts(nts_tm, nts_ds, nts_by, ");
            sb.Append("tsk_id, wki_fdr_id, prj_id) ");
            sb.Append("VALUES (@nts_tm, @nts_ds, @nts_by, @tsk_id, ");
            sb.Append("@wki_fdr_id, @prj_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_tm = cmd.Parameters.Add("@nts_tm", NpgsqlDbType.Timestamp);
                    var nts_ds = cmd.Parameters.Add("@nts_ds", NpgsqlDbType.Text);
                    var nts_by = cmd.Parameters.Add("@nts_by", NpgsqlDbType.Text);
                    var tsk_id = cmd.Parameters.Add("@tsk_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var prj_id = cmd.Parameters.Add("@prj_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    nts_tm.Value = n.NoteTime;
                    nts_ds.Value = n.NoteContent;
                    nts_by.Value = n.NoteWrittenBy ?? (object)DBNull.Value;
                    wki_fdr_id.Value = n.WorkItemFolderId ?? (object)DBNull.Value;
                    tsk_id.Value = n.TaskItemId ?? (object)DBNull.Value;
                    prj_id.Value = n.ProjectId ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateWorkItemNoteToIsCancelledAsync(long workItemNoteId, string cancelledBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_wki_nts SET is_ccl=true, ccl_dt=@ccl_dt, ");
            sb.Append("ccl_by=@ccl_by WHERE (nts_id = @nts_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    var ccl_dt = cmd.Parameters.Add("@ccl_dt", NpgsqlDbType.Timestamp);
                    var ccl_by = cmd.Parameters.Add("@ccl_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    nts_id.Value = workItemNoteId;
                    ccl_dt.Value = DateTime.Now;
                    ccl_by.Value = cancelledBy ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteWorkItemNoteAsync(long workItemNoteId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_wki_nts SET is_ccl=true, ccl_dt=@ccl_dt, ");
            sb.Append("ccl_by=@ccl_by WHERE (nts_id=@nts_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    var ccl_dt = cmd.Parameters.Add("@ccl_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    nts_id.Value = workItemNoteId;
                    ccl_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #region Work Item Activity Log Action Methods
        public async Task<List<WorkItemActivityLog>> GetWorkItemActivityLogByFolderIdAsync(long folderId)
        {
            List<WorkItemActivityLog> activityLogs = new List<WorkItemActivityLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tsk_hst_id, tsk_hst_tm, tsk_hst_ds, ");
            sb.Append("tsk_hst_by, tsk_itm_id, wki_fdr_id, prj_id ");
            sb.Append("FROM public.wsp_wki_hst ");
            sb.Append("WHERE (wki_fdr_id = @wki_fdr_id ");
            sb.Append("AND wki_fdr_id IS NOT NULL) ");
            sb.Append("ORDER BY tsk_hst_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = folderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            activityLogs.Add(new WorkItemActivityLog
                            {
                                Id = reader["tsk_hst_id"] == DBNull.Value ? 0 : (long)reader["tsk_hst_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                ProjectId = reader["prj_id"] == DBNull.Value ? 0 : (long)reader["prj_id"],
                                Time = reader["tsk_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["tsk_hst_tm"],
                                Description = reader["tsk_hst_ds"] == DBNull.Value ? string.Empty : reader["tsk_hst_ds"].ToString(),
                                ActivityBy = reader["tsk_hst_by"] == DBNull.Value ? string.Empty : reader["tsk_hst_by"].ToString(),
                            });
                        }
                }
            }
            return activityLogs;
        }
        public async Task<List<WorkItemActivityLog>> GetWorkItemActivityLogByTaskIdAsync(long taskId)
        {
            List<WorkItemActivityLog> activityLogs = new List<WorkItemActivityLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tsk_hst_id, tsk_hst_tm, tsk_hst_ds, ");
            sb.Append("tsk_hst_by, tsk_itm_id, wki_fdr_id, prj_id ");
            sb.Append("FROM public.wsp_wki_hst ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id ");
            sb.Append("AND tsk_itm_id IS NOT NULL) ");
            sb.Append("ORDER BY tsk_hst_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tsk_itm_id.Value = taskId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            activityLogs.Add(new WorkItemActivityLog
                            {
                                Id = reader["tsk_hst_id"] == DBNull.Value ? 0 : (long)reader["tsk_hst_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                ProjectId = reader["prj_id"] == DBNull.Value ? 0 : (long)reader["prj_id"],
                                Time = reader["tsk_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["tsk_hst_tm"],
                                Description = reader["tsk_hst_ds"] == DBNull.Value ? string.Empty : reader["tsk_hst_ds"].ToString(),
                                ActivityBy = reader["tsk_hst_by"] == DBNull.Value ? string.Empty : reader["tsk_hst_by"].ToString(),
                            });
                        }
                }
            }
            return activityLogs;
        }
        public async Task<List<WorkItemActivityLog>> GetWorkItemActivityLogByProjectIdAsync(long projectId)
        {
            List<WorkItemActivityLog> activityLogs = new List<WorkItemActivityLog>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tsk_hst_id, tsk_hst_tm, tsk_hst_ds, ");
            sb.Append("tsk_hst_by, tsk_itm_id, wki_fdr_id, prj_id ");
            sb.Append("FROM public.wsp_wki_hst ");
            sb.Append("WHERE (prj_id = @prj_id ");
            sb.Append("AND prj_id IS NOT NULL) ");
            sb.Append("ORDER BY tsk_hst_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var prj_id = cmd.Parameters.Add("@prj_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    prj_id.Value = projectId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            activityLogs.Add(new WorkItemActivityLog
                            {
                                Id = reader["tsk_hst_id"] == DBNull.Value ? 0 : (long)reader["tsk_hst_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                ProjectId = reader["prj_id"] == DBNull.Value ? 0 : (long)reader["prj_id"],
                                Time = reader["tsk_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["tsk_hst_tm"],
                                Description = reader["tsk_hst_ds"] == DBNull.Value ? string.Empty : reader["tsk_hst_ds"].ToString(),
                                ActivityBy = reader["tsk_hst_by"] == DBNull.Value ? string.Empty : reader["tsk_hst_by"].ToString(),
                            });
                        }
                }
            }
            return activityLogs;
        }
        public async Task<bool> AddWorkItemActivityLogAsync(WorkItemActivityLog log)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_wki_hst(tsk_hst_tm, tsk_hst_ds, ");
            sb.Append("tsk_hst_by, tsk_itm_id, wki_fdr_id, prj_id) VALUES ( ");
            sb.Append("@tsk_hst_tm, @tsk_hst_ds, @tsk_hst_by, @tsk_itm_id, ");
            sb.Append("@wki_fdr_id, @prj_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_hst_tm = cmd.Parameters.Add("@tsk_hst_tm", NpgsqlDbType.Timestamp);
                    var tsk_hst_ds = cmd.Parameters.Add("@tsk_hst_ds", NpgsqlDbType.Text);
                    var tsk_hst_by = cmd.Parameters.Add("@tsk_hst_by", NpgsqlDbType.Text);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var prj_id = cmd.Parameters.Add("@prj_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_hst_tm.Value = log.Time;
                    tsk_hst_ds.Value = log.Description;
                    tsk_hst_by.Value = log.ActivityBy; //?? (object)DBNull.Value;
                    wki_fdr_id.Value = log.WorkItemFolderId ?? (object)DBNull.Value;
                    tsk_itm_id.Value = log.TaskItemId ?? (object)DBNull.Value;
                    prj_id.Value = log.ProjectId ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteWorkItemActivityLogAsync(long activityLogId)
        {
            int rows = 0;
            string query = "DELETE FROM public.wsp_wki_hst WHERE (tsk_hst_id = @tsk_hst_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_hst_id = cmd.Parameters.Add("@tsk_hst_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_hst_id.Value = activityLogId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #region Task Items Repository

        #region Task Items Read Action Methods
        public async Task<TaskItem> GetTaskItemByIdAsync(long taskItemId)
        {
            TaskItem task = new TaskItem();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
            sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
            sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
            sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
            sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
            sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
            sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
            sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
            sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
            sb.Append("WHEN 2 THEN 'Approved for Execution' ");
            sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
            sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
            sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");

            sb.Append("CASE t.prgs_stts WHEN 0 THEN 'Not Yet Started' ");
            sb.Append("WHEN 1 THEN 'In Progress' ");
            sb.Append("WHEN 2 THEN 'Completed' ");
            sb.Append("WHEN 3 THEN 'On Hold' END AS prgs_stts_ds, ");

            sb.Append("CASE t.apprv_stts WHEN 0 THEN 'Pending' ");
            sb.Append("WHEN 1 THEN 'Approved' ");
            sb.Append("WHEN 2 THEN 'Declined' END AS apprv_stts_ds, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm ");
            sb.Append("FROM public.wsp_tsk_itms t ");
            sb.Append("WHERE (t.tsk_itm_id=@tsk_itm_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tsk_itm_id.Value = taskItemId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            task.Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"];
                            task.Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString();
                            task.Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString();
                            task.MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString();
                            task.WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"];
                            task.WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString();
                            task.MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"];
                            task.LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString();
                            task.LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString();
                            task.LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"];
                            task.TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString();
                            task.TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString();
                            task.AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString();
                            task.AssignedToName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString();
                            task.AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"];
                            task.StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"];
                            task.StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString();
                            task.Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"];

                            task.ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"];
                            task.ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"];
                            task.ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString();

                            task.ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"];
                            task.ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"];
                            task.ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString();

                            task.ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"];
                            task.ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString();
                            task.ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"];
                            task.ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"];
                            task.ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"];
                            task.ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"];

                            task.IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"];
                            task.CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString();
                            task.CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"];

                            task.IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"];
                            task.ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString();
                            task.ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"];

                            task.UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"];
                            task.UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString();
                            task.DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"];
                            task.DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString();
                            task.LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"];
                            task.LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString();

                            task.CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"];
                            task.CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString();
                            task.CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"];
                            task.IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"];

                            task.CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"];
                            task.CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString();
                            task.LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"];
                            task.LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString();

                            task.IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"];
                            task.AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"];
                        }
                }
                await conn.CloseAsync();
            }
            return task;
        }
        public async Task<List<TaskItem>> GetTaskItemsByFolderIdAsync(long folderId)
        {
            List<TaskItem> taskList = new List<TaskItem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
            sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
            sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
            sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
            sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
            sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
            sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
            sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
            sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
            sb.Append("WHEN 2 THEN 'Approved for Execution' ");
            sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
            sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
            sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");

            sb.Append("CASE t.prgs_stts WHEN 0 THEN 'Not Yet Started' ");
            sb.Append("WHEN 1 THEN 'In Progress' ");
            sb.Append("WHEN 2 THEN 'Completed' ");
            sb.Append("WHEN 3 THEN 'On Hold' END AS prgs_stts_ds, ");

            sb.Append("CASE t.apprv_stts WHEN 0 THEN 'Pending' ");
            sb.Append("WHEN 1 THEN 'Approved' ");
            sb.Append("WHEN 2 THEN 'Declined' END AS apprv_stts_ds, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm ");
            sb.Append("FROM public.wsp_tsk_itms t ");
            sb.Append("WHERE (t.wki_fdr_id=@wki_fdr_id) ");
            sb.Append("ORDER BY t.tsk_itm_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = folderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskList.Add(new TaskItem
                            {
                                Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                                WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                                LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                                LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                                LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                                AssignedToName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                                StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                                Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],

                                ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                                ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                                ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString(),

                                ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                                ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                                ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString(),

                                ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                                ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                                IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                                CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                                CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                                IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                                ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                                ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                                UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                                UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                                DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                                CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                                CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                                CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                                IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                                CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskList;
        }
        public async Task<List<TaskItem>> GetTaskItemsByOwnerIdnDescriptionnFolderIdAsync(string ownerId, string taskDescription, long? folderId)
        {
            List<TaskItem> taskList = new List<TaskItem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
            sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
            sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
            sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
            sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
            sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
            sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
            sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
            sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
            sb.Append("WHEN 2 THEN 'Approved for Execution' ");
            sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
            sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
            sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");

            sb.Append("CASE t.prgs_stts WHEN 0 THEN 'Not Yet Started' ");
            sb.Append("WHEN 1 THEN 'In Progress' ");
            sb.Append("WHEN 2 THEN 'Completed' ");
            sb.Append("WHEN 3 THEN 'On Hold' END AS prgs_stts_ds, ");

            sb.Append("CASE t.apprv_stts WHEN 0 THEN 'Pending' ");
            sb.Append("WHEN 1 THEN 'Approved' ");
            sb.Append("WHEN 2 THEN 'Declined' END AS apprv_stts_ds, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm ");
            sb.Append("FROM public.wsp_tsk_itms t ");
            sb.Append("WHERE (t.tsk_owner_id = @tsk_owner_id) ");
            sb.Append("AND LOWER(t.tsk_itm_ds) = LOWER(@tsk_itm_ds)  ");
            sb.Append("AND (t.wki_fdr_id=@wki_fdr_id OR t.wki_fdr_id IS NULL) ");
            sb.Append("ORDER BY t.tsk_itm_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var tsk_itm_ds = cmd.Parameters.Add("@tsk_itm_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    tsk_owner_id.Value = ownerId;
                    wki_fdr_id.Value = folderId;
                    tsk_itm_ds.Value = taskDescription;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskList.Add(new TaskItem
                            {
                                Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                                WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                                LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                                LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                                LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                                AssignedToName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                                StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                                Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],

                                ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                                ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                                ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString(),

                                ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                                ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                                ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString(),

                                ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                                ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                                IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                                CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                                CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                                IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                                ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                                ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                                UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                                UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                                DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                                CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                                CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                                CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                                IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                                CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskList;
        }
        public async Task<long> GetTaskItemsCountByFolderIdAsync(long folderId)
        {
            long _totalCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(tsk_itm_id) as total ");
            sb.Append("FROM public.wsp_tsk_itms ");
            sb.Append("WHERE (wki_fdr_id=@wki_fdr_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = folderId;
                    var obj = await cmd.ExecuteScalarAsync();
                    _totalCount = Convert.ToInt64(obj);
                }
                await conn.CloseAsync();
            }
            return _totalCount;
        }

        //========= Pending Task Items ==================//
        public async Task<List<TaskItem>> GetTaskItemsPendingByOwnerIdAsync(string ownerId)
        {
            List<TaskItem> taskList = new List<TaskItem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
            sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
            sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
            sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
            sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
            sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
            sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
            sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
            sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
            sb.Append("WHEN 2 THEN 'Approved for Execution' ");
            sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
            sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
            sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm ");
            sb.Append("FROM public.wsp_tsk_itms t ");
            sb.Append("WHERE (t.tsk_owner_id=@tsk_owner_id) AND (t.wki_fdr_id IS NULL) ");
            sb.Append("ORDER BY t.tsk_itm_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    tsk_owner_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskList.Add(new TaskItem
                            {
                                Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                                WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                                LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                                LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                                LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                                AssignedToName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                                StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                                Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],
                                ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                                ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                                ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                                ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                                ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                                ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                                IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                                CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                                CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                                IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                                ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                                ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                                UnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                                UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                DepartmentId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                                DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                LocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                                CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                                CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                                CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                                IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                                CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                AssignmentId = reader["assgnmt_id"] == DBNull.Value ? 0 : (int)reader["assgnmt_id"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskList;
        }
        public async Task<long> GetTaskItemsPendingCountByOwnerIdAsync(string ownerId)
        {
            long item_count = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(tsk_itm_id) as total ");
            sb.Append("FROM public.wsp_tsk_itms t ");
            sb.Append("WHERE (t.tsk_owner_id=@tsk_owner_id) AND (t.wki_fdr_id IS NULL); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    tsk_owner_id.Value = ownerId;
                    var obj = await cmd.ExecuteScalarAsync();
                    item_count = (long)obj;
                }
                await conn.CloseAsync();
            }
            return item_count;
        }

        #endregion

        #region Task Items Write Action Methods
        public async Task<long> AddTaskItemAsync(TaskItem task)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_tsk_itms(tsk_itm_no, tsk_itm_ds, ");
            sb.Append("tsk_itm_inf, wki_fdr_id, mst_tsk_id, prj_no, prg_no, ");
            sb.Append("prg_dt, tsk_owner_id, assgnd_emp_id, assigned_dt, ");
            sb.Append("tsk_itm_stg, prgs_stts, apprv_stts, approved_dt, ");
            sb.Append("approved_by, exp_start_dt, act_start_dt, exp_due_dt, ");
            sb.Append("act_due_dt, unit_id, dept_id, loc_id, mod_by, crt_by, ");
            sb.Append("assgnmt_id, crt_dt, mod_dt) ");
            sb.Append("VALUES (@tsk_itm_no, @tsk_itm_ds, @tsk_itm_inf, ");
            sb.Append("@wki_fdr_id, @mst_tsk_id, @prj_no, @prg_no, @prg_dt, ");
            sb.Append("@tsk_owner_id, @assgnd_emp_id, @assigned_dt, ");
            sb.Append("@tsk_itm_stg, @prgs_stts, @apprv_stts, @approved_dt, ");
            sb.Append("@approved_by, @exp_start_dt, @act_start_dt, ");
            sb.Append("@exp_due_dt, @act_due_dt, @unit_id, @dept_id, @loc_id, ");
            sb.Append("@crt_by, @crt_by, @assgnmt_id, @crt_dt, @crt_dt) ");
            sb.Append("RETURNING tsk_itm_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_no = cmd.Parameters.Add("@tsk_itm_no", NpgsqlDbType.Text);
                    var tsk_itm_ds = cmd.Parameters.Add("@tsk_itm_ds", NpgsqlDbType.Text);
                    var tsk_itm_inf = cmd.Parameters.Add("@tsk_itm_inf", NpgsqlDbType.Text);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var mst_tsk_id = cmd.Parameters.Add("@mst_tsk_id", NpgsqlDbType.Bigint);
                    var prj_no = cmd.Parameters.Add("@prj_no", NpgsqlDbType.Text);
                    var prg_no = cmd.Parameters.Add("@prg_no", NpgsqlDbType.Text);
                    var prg_dt = cmd.Parameters.Add("@prg_dt", NpgsqlDbType.Timestamp);
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var assgnd_emp_id = cmd.Parameters.Add("@assgnd_emp_id", NpgsqlDbType.Text);
                    var assigned_dt = cmd.Parameters.Add("@assigned_dt", NpgsqlDbType.Timestamp);
                    var tsk_itm_stg = cmd.Parameters.Add("@tsk_itm_stg", NpgsqlDbType.Integer);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    var apprv_stts = cmd.Parameters.Add("@apprv_stts", NpgsqlDbType.Integer);
                    var approved_dt = cmd.Parameters.Add("@approved_dt", NpgsqlDbType.Timestamp);
                    var approved_by = cmd.Parameters.Add("@approved_by", NpgsqlDbType.Text);
                    var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.Timestamp);
                    var act_start_dt = cmd.Parameters.Add("@act_start_dt", NpgsqlDbType.Timestamp);
                    var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.Timestamp);
                    var act_due_dt = cmd.Parameters.Add("@act_due_dt", NpgsqlDbType.Timestamp);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    var crt_dt = cmd.Parameters.Add("@crt_dt", NpgsqlDbType.Timestamp);
                    var crt_by = cmd.Parameters.Add("@crt_by", NpgsqlDbType.Text);
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    tsk_itm_no.Value = task.Number;
                    tsk_itm_ds.Value = task.Description;
                    tsk_itm_inf.Value = task.MoreInformation ?? (object)DBNull.Value;
                    wki_fdr_id.Value = task.WorkFolderId ?? (object)DBNull.Value;
                    mst_tsk_id.Value = task.MasterTaskId ?? (object)DBNull.Value;
                    prj_no.Value = task.LinkProjectNumber ?? (object)DBNull.Value;
                    prg_no.Value = task.LinkProgramCode ?? (object)DBNull.Value;
                    prg_dt.Value = task.LinkProgramDate ?? (object)DBNull.Value;
                    tsk_owner_id.Value = task.TaskOwnerId;
                    assgnd_emp_id.Value = task.AssignedToId ?? (object)DBNull.Value;
                    assigned_dt.Value = task.AssignedTime ?? (object)DBNull.Value;
                    tsk_itm_stg.Value = task.StageId;
                    prgs_stts.Value = task.ProgressStatusId;
                    apprv_stts.Value = task.ApprovalStatusId;
                    approved_dt.Value = task.ApprovedTime ?? (object)DBNull.Value;
                    approved_by.Value = task.ApprovedBy ?? (object)DBNull.Value;
                    exp_start_dt.Value = task.ExpectedStartTime ?? (object)DBNull.Value;
                    act_start_dt.Value = task.ActualStartTime ?? (object)DBNull.Value;
                    exp_due_dt.Value = task.ExpectedDueTime ?? (object)DBNull.Value;
                    act_due_dt.Value = task.ActualDueTime ?? (object)DBNull.Value;
                    unit_id.Value = task.UnitId;
                    dept_id.Value = task.DepartmentId;
                    loc_id.Value = task.LocationId;
                    mod_by.Value = task.CreatedBy ?? (object)DBNull.Value;
                    mod_dt.Value = task.CreatedTime ?? DateTime.Now;
                    assgnmt_id.Value = task.AssignmentId ?? (object)DBNull.Value;
                    crt_by.Value = task.CreatedBy ?? (object)DBNull.Value;
                    crt_dt.Value = task.CreatedTime ?? DateTime.Now;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateTaskItemAsync(TaskItem task)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET tsk_itm_ds=@tsk_itm_ds,  ");
            sb.Append("tsk_itm_inf=@tsk_itm_inf, prj_no=@prj_no, prg_no=@prg_no, ");
            sb.Append("prg_dt=@prg_dt, exp_start_dt=@exp_start_dt, ");
            sb.Append("exp_due_dt=@exp_due_dt, mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_ds = cmd.Parameters.Add("@tsk_itm_ds", NpgsqlDbType.Text);
                    var tsk_itm_inf = cmd.Parameters.Add("@tsk_itm_inf", NpgsqlDbType.Text);
                    var prj_no = cmd.Parameters.Add("@prj_no", NpgsqlDbType.Text);
                    var prg_no = cmd.Parameters.Add("@prg_no", NpgsqlDbType.Text);
                    var prg_dt = cmd.Parameters.Add("@prg_dt", NpgsqlDbType.Timestamp);
                    var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.Timestamp);
                    var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.Timestamp);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_itm_ds.Value = task.Description;
                    tsk_itm_inf.Value = task.MoreInformation ?? (object)DBNull.Value;
                    prj_no.Value = task.LinkProjectNumber ?? (object)DBNull.Value;
                    prg_no.Value = task.LinkProgramCode ?? (object)DBNull.Value;
                    prg_dt.Value = task.LinkProgramDate ?? (object)DBNull.Value;
                    exp_start_dt.Value = task.ExpectedStartTime ?? (object)DBNull.Value;
                    exp_due_dt.Value = task.ExpectedDueTime ?? (object)DBNull.Value;
                    mod_dt.Value = task.LastModifiedTime ?? DateTime.Now;
                    mod_by.Value = task.LastModifiedBy ?? (object)DBNull.Value;
                    tsk_itm_id.Value = task.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemResolutionAsync(long taskId, string taskResolution, string updatedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET tsk_itm_inf=@tsk_itm_inf, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_inf = cmd.Parameters.Add("@tsk_itm_inf", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_itm_inf.Value = taskResolution ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    mod_by.Value = updatedBy ?? (object)DBNull.Value;
                    tsk_itm_id.Value = taskId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteTaskItemAsync(long taskItemId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wsp_wki_hst WHERE tsk_itm_id = @tsk_itm_id; ");
            sb.Append("DELETE FROM public.wsp_wki_nts WHERE tsk_id = @tsk_itm_id; ");
            sb.Append("DELETE FROM public.wsp_eval_rtns WHERE (tsk_itm_id = @tsk_itm_id); ");
            sb.Append("DELETE FROM public.wsp_eval_dtl WHERE (tsk_itm_id = @tsk_itm_id); ");
            sb.Append("DELETE FROM public.wsp_tsk_itms WHERE (tsk_itm_id = @tsk_itm_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemFolderIdAsync(long taskItemId, string modifiedBy, long? taskFolderId = null)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET wki_fdr_id=@wki_fdr_id, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    wki_fdr_id.Value = taskFolderId ?? (object)DBNull.Value;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemFolderIdForPendingTaskItemsAsync(string taskOwnerId, long taskFolderId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET wki_fdr_id=@wki_fdr_id ");
            sb.Append("WHERE (tsk_owner_id=@tsk_owner_id AND wki_fdr_id IS NULL);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    tsk_owner_id.Value = taskOwnerId;
                    wki_fdr_id.Value = taskFolderId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemProgressStatusAsync(long taskItemId, int newProgressStatus, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET prgs_stts=@prgs_stts, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var prgs_stts = cmd.Parameters.Add("@prgs_stts", NpgsqlDbType.Integer);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    prgs_stts.Value = newProgressStatus;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemStageAsync(long taskItemId, int newStage, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET tsk_itm_stg=@tsk_itm_stg, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var tsk_itm_stg = cmd.Parameters.Add("@tsk_itm_stg", NpgsqlDbType.Integer);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    tsk_itm_stg.Value = newStage;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemOpenCloseStatusAsync(long taskItemId, bool closeTask, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET is_closed=@is_closed, ");
            sb.Append("closed_dt=@closed_dt, closed_by=@closed_by, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var is_closed = cmd.Parameters.Add("@is_closed", NpgsqlDbType.Boolean);
                    var closed_dt = cmd.Parameters.Add("@closed_dt", NpgsqlDbType.Timestamp);
                    var closed_by = cmd.Parameters.Add("@closed_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    is_closed.Value = closeTask;
                    closed_dt.Value = DateTime.Now;
                    closed_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemCompletionStatusAsync(long taskItemId, bool isCompleted, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET completion_is_confirmed=@completion_is_confirmed, ");
            sb.Append("completion_confirmed_on=@completion_confirmed_on, completion_confirmed_by=@completion_confirmed_by, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var completion_is_confirmed = cmd.Parameters.Add("@completion_is_confirmed", NpgsqlDbType.Boolean);
                    var completion_confirmed_on = cmd.Parameters.Add("@completion_confirmed_on", NpgsqlDbType.Timestamp);
                    var completion_confirmed_by = cmd.Parameters.Add("@completion_confirmed_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    completion_is_confirmed.Value = isCompleted;
                    completion_confirmed_on.Value = DateTime.Now;
                    completion_confirmed_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemOpenCloseStatusByFolderIdAsync(long taskFolderId, bool closeTask, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET is_closed=@is_closed, ");
            sb.Append("closed_dt=@closed_dt, closed_by=@closed_by, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (wki_fdr_id=@wki_fdr_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var is_closed = cmd.Parameters.Add("@is_closed", NpgsqlDbType.Boolean);
                    var closed_dt = cmd.Parameters.Add("@closed_dt", NpgsqlDbType.Timestamp);
                    var closed_by = cmd.Parameters.Add("@closed_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    wki_fdr_id.Value = taskFolderId;
                    is_closed.Value = closeTask;
                    closed_dt.Value = DateTime.Now;
                    closed_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemApprovalStatusAsync(long taskItemId, ApprovalStatus newApprovalStatus, string approvedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET apprv_stts=@apprv_stts, ");
            sb.Append("approved_dt=@approved_dt, approved_by=@approved_by, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var apprv_stts = cmd.Parameters.Add("@apprv_stts", NpgsqlDbType.Integer);
                    var approved_dt = cmd.Parameters.Add("@approved_dt", NpgsqlDbType.Timestamp);
                    var approved_by = cmd.Parameters.Add("@approved_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    apprv_stts.Value = (int)newApprovalStatus;
                    approved_dt.Value = DateTime.Now;
                    approved_by.Value = approvedBy;
                    mod_by.Value = approvedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemTimelineAsync(long taskItemId, string modifiedBy, DateTime? previousStartDate, DateTime? newStartDate, DateTime? previousEndDate, DateTime? newEndDate)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET exp_start_dt=@exp_start_dt, ");
            sb.Append("exp_due_dt=@exp_due_dt, mod_by=@mod_by, mod_dt=@mod_dt, ");
            sb.Append("apprv_stts=@apprv_stts, approved_dt=@approved_dt, ");
            sb.Append("approved_by=@approved_by WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var exp_start_dt = cmd.Parameters.Add("@exp_start_dt", NpgsqlDbType.TimestampTz);
                    var exp_due_dt = cmd.Parameters.Add("@exp_due_dt", NpgsqlDbType.TimestampTz);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    var apprv_stts = cmd.Parameters.Add("@apprv_stts", NpgsqlDbType.Integer);
                    var approved_dt = cmd.Parameters.Add("@approved_dt", NpgsqlDbType.TimestampTz);
                    var approved_by = cmd.Parameters.Add("@approved_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    exp_start_dt.Value = newStartDate ?? previousStartDate;
                    exp_due_dt.Value = newEndDate ?? previousEndDate;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    apprv_stts.Value = 0;
                    approved_dt.Value = DBNull.Value;
                    approved_by.Value = DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemProgramLinkAsync(long taskItemId, string programCode, DateTime? programDate, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET prg_no=@prg_no, ");
            sb.Append("prg_dt=@prg_dt, mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var prg_no = cmd.Parameters.Add("@prg_no", NpgsqlDbType.Text);
                    var prg_dt = cmd.Parameters.Add("@prg_dt", NpgsqlDbType.TimestampTz);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    prg_no.Value = programCode;
                    prg_dt.Value = programDate;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemProjectLinkAsync(long taskItemId, string projectCode, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET prj_no=@prj_no, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var prj_no = cmd.Parameters.Add("@prj_no", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    prj_no.Value = projectCode;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = $"{DateTime.UtcNow.ToLongDateString()} as at {DateTime.UtcNow.ToLongTimeString()}";
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemOwnerAsync(long taskItemId, long newTaskFolderId, string newTaskOwnerId, int newUnitId, int newDeptId, int newLocationId, string modifiedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET wki_fdr_id=@wki_fdr_id, ");
            sb.Append("tsk_owner_id=@tsk_owner_id, assgnd_emp_id=@tsk_owner_id, ");
            sb.Append("unit_id=@unit_id, dept_id=@dept_id, loc_id=@loc_id, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    wki_fdr_id.Value = newTaskFolderId;
                    tsk_owner_id.Value = newTaskOwnerId;
                    unit_id.Value = newUnitId;
                    dept_id.Value = newDeptId;
                    loc_id.Value = newLocationId;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemActualStartDateAsync(long taskItemId, DateTime? newStartDate = null, string modifiedBy = null)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET act_start_dt=@act_start_dt, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt  ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var act_start_dt = cmd.Parameters.Add("@act_start_dt", NpgsqlDbType.Timestamp);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    act_start_dt.Value = newStartDate ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    tsk_itm_id.Value = taskItemId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateTaskItemActualDueDateAsync(long taskItemId, DateTime? newDueDate = null, string modifiedBy = null)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_tsk_itms SET act_due_dt=@act_due_dt, ");
            sb.Append("mod_by=@mod_by, mod_dt=@mod_dt  ");
            sb.Append("WHERE (tsk_itm_id=@tsk_itm_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var act_due_dt = cmd.Parameters.Add("@act_due_dt", NpgsqlDbType.Timestamp);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Timestamp);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    act_due_dt.Value = newDueDate ?? (object)DBNull.Value;
                    mod_dt.Value = DateTime.Now;
                    mod_by.Value = modifiedBy ?? (object)DBNull.Value;
                    tsk_itm_id.Value = taskItemId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #endregion

        #region Task Timeline Change Repository

        public async Task<bool> AddTaskTimelineChangeAsync(TaskTimelineChange taskTimelineChange)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_tsk_tml(tsk_itm_id, prev_start_dt, ");
            sb.Append("prev_end_dt, new_start_dt, new_end_dt, diff_in_days, mod_by, ");
            sb.Append("mod_dt, wki_fdr_id) VALUES (@tsk_itm_id, @prev_start_dt, ");
            sb.Append("@prev_end_dt, @new_start_dt, @new_end_dt, @diff_in_days, ");
            sb.Append("@mod_by, @mod_dt, @wki_fdr_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var prev_start_dt = cmd.Parameters.Add("@prev_start_dt", NpgsqlDbType.Timestamp);
                    var prev_end_dt = cmd.Parameters.Add("@prev_end_dt", NpgsqlDbType.Timestamp);
                    var new_start_dt = cmd.Parameters.Add("@new_start_dt", NpgsqlDbType.Timestamp);
                    var new_end_dt = cmd.Parameters.Add("@new_end_dt", NpgsqlDbType.Timestamp);
                    var diff_in_days = cmd.Parameters.Add("@diff_in_days", NpgsqlDbType.Numeric);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskTimelineChange.TaskItemId;
                    wki_fdr_id.Value = taskTimelineChange.WorkItemFolderId;
                    prev_start_dt.Value = taskTimelineChange.PreviousStartDate ?? (object)DBNull.Value;
                    prev_end_dt.Value = taskTimelineChange.PreviousEndDate ?? (object)DBNull.Value;
                    new_start_dt.Value = taskTimelineChange.NewStartDate ?? (object)DBNull.Value;
                    new_end_dt.Value = taskTimelineChange.NewEndDate ?? (object)DBNull.Value;
                    diff_in_days.Value = taskTimelineChange.DifferentInDays;
                    mod_by.Value = taskTimelineChange.ModifiedBy ?? (object)DBNull.Value;
                    mod_dt.Value = taskTimelineChange.ModifiedTime ?? $"{DateTime.Now.ToLongDateString()} as at {DateTime.Now.ToLongTimeString()}";

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<List<TaskTimelineChange>> GetTaskTimelineByTaskItemIdAsync(long taskItemId)
        {
            List<TaskTimelineChange> taskTimelineChanges = new List<TaskTimelineChange>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tml_chg_id, tsk_itm_id, prev_start_dt, prev_end_dt, ");
            sb.Append("new_start_dt, new_end_dt, diff_in_days, mod_by, mod_dt, ");
            sb.Append("wki_fdr_id FROM public.wsp_tsk_tml ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id) ");
            sb.Append("ORDER BY tml_chg_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tsk_itm_id.Value = taskItemId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskTimelineChanges.Add(new TaskTimelineChange
                            {
                                TimelineChangeId = reader["tml_chg_id"] == DBNull.Value ? 0 : (long)reader["tml_chg_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                PreviousStartDate = reader["prev_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_start_dt"],
                                PreviousEndDate = reader["prev_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_end_dt"],
                                NewStartDate = reader["new_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_start_dt"],
                                NewEndDate = reader["new_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_end_dt"],
                                DifferentInDays = reader["diff_in_days"] == DBNull.Value ? 0.00M : (decimal)reader["diff_in_days"],
                                ModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),
                                ModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskTimelineChanges;
        }

        public async Task<TaskTimelineChange> GetTaskTimelineBytimelineIdAsync(long timelineChangeId)
        {
            TaskTimelineChange t = new TaskTimelineChange();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tml_chg_id, tsk_itm_id, prev_start_dt, prev_end_dt, ");
            sb.Append("new_start_dt, new_end_dt, diff_in_days, mod_by, mod_dt, ");
            sb.Append("wki_fdr_id FROM public.wsp_tsk_tml ");
            sb.Append("WHERE (tml_chg_id = @tml_chg_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tml_chg_id = cmd.Parameters.Add("@tml_chg_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tml_chg_id.Value = timelineChangeId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            t.TimelineChangeId = reader["tml_chg_id"] == DBNull.Value ? 0 : (long)reader["tml_chg_id"];
                            t.TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"];
                            t.WorkItemFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"];
                            t.PreviousStartDate = reader["prev_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_start_dt"];
                            t.PreviousEndDate = reader["prev_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prev_end_dt"];
                            t.NewStartDate = reader["new_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_start_dt"];
                            t.NewEndDate = reader["new_end_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["new_end_dt"];
                            t.DifferentInDays = reader["diff_in_days"] == DBNull.Value ? 0.00M : (decimal)reader["diff_in_days"];
                            t.ModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString();
                            t.ModifiedTime = reader["mod_dt"] == DBNull.Value ? string.Empty : reader["mod_dt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return t;
        }

        #endregion

        #region Folder Submission Action Methods
        public async Task<bool> AddFolderSubmissionAsync(FolderSubmission submission)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_fdr_sbms(wki_fdr_id, frm_emp_id, ");
            sb.Append("to_emp_id, sbm_typ_id, sbm_dt, sbm_msg, is_xtn, dt_xtn) ");
            sb.Append("VALUES (@wki_fdr_id, @frm_emp_id, @to_emp_id, @sbm_typ_id, ");
            sb.Append("@sbm_dt, @sbm_msg, @is_xtn, @dt_xtn); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var frm_emp_id = cmd.Parameters.Add("@frm_emp_id", NpgsqlDbType.Text);
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var sbm_typ_id = cmd.Parameters.Add("@sbm_typ_id", NpgsqlDbType.Integer);
                    var sbm_dt = cmd.Parameters.Add("@sbm_dt", NpgsqlDbType.TimestampTz);
                    var sbm_msg = cmd.Parameters.Add("@sbm_msg", NpgsqlDbType.Text);
                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    wki_fdr_id.Value = submission.FolderId;
                    frm_emp_id.Value = submission.FromEmployeeId;
                    to_emp_id.Value = submission.ToEmployeeId;
                    sbm_typ_id.Value = (int)submission.SubmissionType;
                    sbm_dt.Value = submission.DateSubmitted;
                    sbm_msg.Value = submission.Comment ?? (object)DBNull.Value;
                    is_xtn.Value = submission.IsActioned;
                    dt_xtn.Value = submission.DateActioned ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateFolderSubmissionAsync(long folderSubmissionId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_fdr_sbms ");
            sb.Append("SET is_xtn=@is_xtn, dt_xtn=@dt_xtn ");
            sb.Append("WHERE (fdr_sbm_id=@fdr_sbm_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var fdr_sbm_id = cmd.Parameters.Add("@fdr_sbm_id", NpgsqlDbType.Bigint);
                    var is_xtn = cmd.Parameters.Add("@is_xtn", NpgsqlDbType.Boolean);
                    var dt_xtn = cmd.Parameters.Add("@dt_xtn", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    fdr_sbm_id.Value = folderSubmissionId;
                    is_xtn.Value = true;
                    dt_xtn.Value = DateTime.Now;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteFolderSubmissionAsync(long submissionId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wsp_fdr_sbms ");
            sb.Append("WHERE (fdr_sbm_id=@fdr_sbm_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var fdr_sbm_id = cmd.Parameters.Add("@fdr_sbm_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    fdr_sbm_id.Value = submissionId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<FolderSubmission> GetFolderSubmissionByIdAsync(long submissionId)
        {
            FolderSubmission submission = new FolderSubmission();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE s.fdr_sbm_id = @fdr_sbm_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var fdr_sbm_id = cmd.Parameters.Add("@fdr_sbm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    fdr_sbm_id.Value = submissionId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submission.FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"];
                            submission.FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"];
                            submission.FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString();
                            submission.FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString();
                            submission.FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString();
                            submission.FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString();
                            submission.ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString();
                            submission.ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString();
                            submission.SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"];
                            submission.DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"];
                            submission.Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString();
                            submission.IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"];
                            submission.DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"];
                        }
                }
                await conn.CloseAsync();
            }
            return submission;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAsync(string toEmployeeId)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }

            return submissions;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByFolderIdAsync(long folderId)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE (s.wki_fdr_id = @wki_fdr_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = folderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return submissions;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFolderIdAsync(string toEmployeeId, long folderId)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND s.wki_fdr_id = @wki_fdr_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    wki_fdr_id.Value = folderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }

            return submissions;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAsync(string toEmployeeId, int submittedYear)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("ORDER BY s.fdr_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    dt_yr.Value = submittedYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return submissions;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, int submittedYear, int submittedMonth)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("AND date_part('month', s.sbm_dt) = @dt_mm ");
            sb.Append("ORDER BY s.wkm_tsk_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var dt_mm = cmd.Parameters.Add("@dt_mm", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    dt_yr.Value = submittedYear;
                    dt_mm.Value = submittedMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return submissions;
        }

        //============= From Employee Name =============================//
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAsync(string toEmployeeId, string fromEmployeeName)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND LOWER(frm_emp_nm) = LOWER(@frm_emp_nm) ");
            sb.Append("ORDER BY s.fdr_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    frm_emp_nm.Value = fromEmployeeName;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return submissions;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(string toEmployeeId, string fromEmployeeName, int submittedYear)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("AND LOWER(frm_emp_nm) = LOWER(@frm_emp_nm) ");
            sb.Append("ORDER BY s.fdr_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    dt_yr.Value = submittedYear;
                    frm_emp_nm.Value = fromEmployeeName;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return submissions;
        }
        public async Task<List<FolderSubmission>> GetFolderSubmissionsByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, string fromEmployeeName, int submittedYear, int submittedMonth)
        {
            List<FolderSubmission> submissions = new List<FolderSubmission>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.fdr_sbm_id, s.wki_fdr_id, s.frm_emp_id, s.to_emp_id,  ");
            sb.Append("s.sbm_typ_id, s.sbm_dt, s.sbm_msg, s.is_xtn, s.dt_xtn, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr ");
            sb.Append("WHERE wki_fdr_id = s.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT p1.fullname FROM public.gst_prsns p1 ");
            sb.Append("WHERE p1.id = s.frm_emp_id) as frm_emp_nm, ");
            sb.Append("(SELECT p2.fullname FROM public.gst_prsns p2 ");
            sb.Append("WHERE p2.id = s.to_emp_id) as to_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk IN ");
            sb.Append("(SELECT unit_id FROM public.erm_emp_inf WHERE emp_id = s.frm_emp_id)) as unitname ");
            sb.Append("FROM public.wsp_fdr_sbms s ");
            sb.Append("WHERE LOWER(s.to_emp_id) = LOWER(@to_emp_id) ");
            sb.Append("AND date_part('year', s.sbm_dt) = @dt_yr ");
            sb.Append("AND date_part('month', s.sbm_dt) = @dt_mm ");
            sb.Append("AND LOWER(frm_emp_nm) = LOWER(@frm_emp_nm) ");
            sb.Append("ORDER BY s.fdr_sbm_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var to_emp_id = cmd.Parameters.Add("@to_emp_id", NpgsqlDbType.Text);
                    var dt_yr = cmd.Parameters.Add("@dt_yr", NpgsqlDbType.Integer);
                    var dt_mm = cmd.Parameters.Add("@dt_mm", NpgsqlDbType.Integer);
                    var frm_emp_nm = cmd.Parameters.Add("@frm_emp_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    to_emp_id.Value = toEmployeeId;
                    dt_yr.Value = submittedYear;
                    dt_mm.Value = submittedMonth;
                    frm_emp_nm.Value = fromEmployeeName;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            submissions.Add(new FolderSubmission
                            {
                                FolderSubmissionId = reader["fdr_sbm_id"] == DBNull.Value ? 0 : (long)reader["fdr_sbm_id"],
                                FolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                FolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                FromEmployeeId = reader["frm_emp_id"] == DBNull.Value ? string.Empty : reader["frm_emp_id"].ToString(),
                                FromEmployeeName = reader["frm_emp_nm"] == DBNull.Value ? string.Empty : reader["frm_emp_nm"].ToString(),
                                FromEmployeeUnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                ToEmployeeId = reader["to_emp_id"] == DBNull.Value ? string.Empty : reader["to_emp_id"].ToString(),
                                ToEmployeeName = reader["to_emp_nm"] == DBNull.Value ? string.Empty : reader["to_emp_nm"].ToString(),
                                SubmissionType = reader["sbm_typ_id"] == DBNull.Value ? WorkItemSubmissionType.Approval : (WorkItemSubmissionType)reader["sbm_typ_id"],
                                DateSubmitted = reader["sbm_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["sbm_dt"],
                                Comment = reader["sbm_msg"] == DBNull.Value ? string.Empty : reader["sbm_msg"].ToString(),
                                IsActioned = reader["is_xtn"] == DBNull.Value ? false : (bool)reader["is_xtn"],
                                DateActioned = reader["dt_xtn"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_xtn"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return submissions;
        }

        #endregion

        #region Task Item Evaluation Repository

        #region Task Evaluation Header Read by Task OwnerID Action Methods
        //======== Get By Task Owner ID ========//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.eval_dt, ");
            sb.Append("h.total_no_of_tasks, f.wki_fdr_nm, f.dt_frm, f.dt_to, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT current_designation FROM public.gst_prsns WHERE id = h.owner_emp_id) as current_designation, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("WHERE (h.owner_emp_id = @owner_emp_id) ");
            sb.Append("AND (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    owner_emp_id.Value = taskOwnerId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }
        #endregion

        #region Task Evaluation Header Read by Task Folder ID Action Methods
        //======== Get By Task List ID =============================//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByTaskFolderIdAndEvaluatorIdAsync(long taskFolderId, string taskEvaluatorId)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id= h.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE (h.wki_fdr_id = @wki_fdr_id) ");
            sb.Append("AND (h.eval_emp_id = @eval_emp_id) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = taskFolderId;
                    eval_emp_id.Value = taskEvaluatorId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByIdAsync(long taskEvaluationHeaderId)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id= h.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE (h.eval_hdr_id = @eval_hdr_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    eval_hdr_id.Value = taskEvaluationHeaderId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }
        #endregion

        #region Task Evaluation Header Read by Reports To Employee ID Action Methods
        //============== Get By Reports To Employee ID =============//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByReportsToEmployeeIdAsync(string reportsToEmployeeId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, f.wki_fdr_nm, f.dt_frm, f.dt_to, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE h.owner_emp_id IN (SELECT emp_id FROM ");
            sb.Append("public.erm_emp_rpts WHERE rpt_emp_id = @rpt_emp_id ");
            sb.Append("AND rpt_nds >= CURRENT_DATE) ");
            sb.Append("AND (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY  h.eval_hdr_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var rpt_emp_id = cmd.Parameters.Add("@rpt_emp_id", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    rpt_emp_id.Value = reportsToEmployeeId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }
        #endregion

        #region Task Evaluation Header Read by Location, Department and Unit Action Methods
        //======== Get By Unit ID =========//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByUnitIdAsync(int taskOwnerUnitId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, f.dt_frm, f.dt_to, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE h.owner_unit_id = @unit_id ");
            sb.Append("AND (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    unit_id.Value = taskOwnerUnitId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }

        //======== Get By Unit ID & Location ID =========//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByUnitIdAndLocationIdAsync(int taskOwnerUnitId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, f.wki_fdr_nm, f.dt_frm, f.dt_to, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE (h.owner_unit_id = @unit_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    unit_id.Value = taskOwnerUnitId;
                    location_id.Value = taskOwnerLocationId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }

        //======== Get By Department ID ========//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByDepartmentIdAsync(int taskOwnerDepartmentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, f.wki_fdr_nm, f.dt_frm, f.dt_to, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE h.owner_dept_id = @dept_id ");
            sb.Append("AND (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    dept_id.Value = taskOwnerDepartmentId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }

        //======== Get By Department ID and Location ID ========//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByDepartmentIdAndLocationIdAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, f.wki_fdr_id, f.dt_frm, f.dt_to, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE (h.owner_dept_id = @dept_id) AND (h.owner_loc_id = @location_id) ");
            sb.Append("AND (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    dept_id.Value = taskOwnerDepartmentId;
                    location_id.Value = taskOwnerLocationId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }

        //======== Get By Location ID ========//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersByLocationIdAsync(int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, f.wki_fdr_nm, f.dt_frm, f.dt_to, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE (h.owner_loc_id = @location_id) ");
            sb.Append("AND (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var location_id = cmd.Parameters.Add("@location_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    location_id.Value = taskOwnerLocationId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }

        //===== Get by Year and Month Only ======//
        public async Task<List<TaskEvaluationHeader>> GetTaskEvaluationHeadersAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationHeader> taskEvaluationHeaders = new List<TaskEvaluationHeader>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT h.eval_hdr_id, h.wki_fdr_id, h.eval_emp_id, h.owner_emp_id, ");
            sb.Append("h.owner_unit_id, h.owner_dept_id, h.owner_loc_id, h.total_no_of_tasks, ");
            sb.Append("e.current_designation, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = h.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = h.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = h.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = h.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = h.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_hdr h ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = h.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = h.owner_emp_id ");
            sb.Append("WHERE (f.dt_frm >= @dt_frm) ");
            sb.Append("AND (f.dt_to <= @dt_to) ");
            sb.Append("ORDER BY h.eval_hdr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationHeaders.Add(new TaskEvaluationHeader
                            {
                                Id = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),

                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                EvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskOwnerDesignation = reader["current_designation"] == DBNull.Value ? string.Empty : reader["current_designation"].ToString(),
                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationHeaders;
        }
        #endregion

        #region Task Evaluation Header Write Action Methods
        public async Task<long> AddTaskEvaluationHeaderAsync(TaskEvaluationHeader taskEvaluationHeader)
        {
            long id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_eval_hdr(wki_fdr_id, eval_emp_id, ");
            sb.Append("owner_emp_id, owner_unit_id, owner_dept_id, owner_loc_id, ");
            sb.Append("total_no_of_tasks)  ");
            sb.Append("VALUES (@wki_fdr_id, @eval_emp_id, @owner_emp_id, ");
            sb.Append("@owner_unit_id, @owner_dept_id, @owner_loc_id, @total_no_of_tasks) ");
            sb.Append("returning eval_hdr_id;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var owner_unit_id = cmd.Parameters.Add("@owner_unit_id", NpgsqlDbType.Integer);
                    var owner_dept_id = cmd.Parameters.Add("@owner_dept_id", NpgsqlDbType.Integer);
                    var owner_loc_id = cmd.Parameters.Add("@owner_loc_id", NpgsqlDbType.Integer);
                    var total_no_of_tasks = cmd.Parameters.Add("@total_no_of_tasks", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    wki_fdr_id.Value = taskEvaluationHeader.TaskFolderId;
                    eval_emp_id.Value = taskEvaluationHeader.EvaluatorId;
                    owner_emp_id.Value = taskEvaluationHeader.TaskOwnerId;
                    owner_unit_id.Value = taskEvaluationHeader.TaskOwnerUnitId;
                    owner_dept_id.Value = taskEvaluationHeader.TaskOwnerDeptId;
                    owner_loc_id.Value = taskEvaluationHeader.TaskOwnerLocationId;
                    total_no_of_tasks.Value = taskEvaluationHeader.TotalNumberOfTasks;
                    var result = await cmd.ExecuteScalarAsync();
                    id = (long)result;
                }
                await conn.CloseAsync();
            }
            return id;
        }
        public async Task<bool> UpdateTaskEvaluationHeaderAsync(TaskEvaluationHeader taskEvaluationHeader)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_eval_hdr SET wki_fdr_id=@wki_fdr_id, ");
            sb.Append("eval_emp_id=@eval_emp_id, owner_emp_id=@owner_emp_id, ");
            sb.Append("owner_unit_id=@owner_unit_id, owner_dept_id=@owner_dept_id, ");
            sb.Append("owner_loc_id=@owner_loc_id, total_no_of_tasks=@total_no_of_tasks ");
            sb.Append("WHERE (eval_hdr_id=@eval_hdr_id); ");

            string query = sb.ToString();
            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                var owner_unit_id = cmd.Parameters.Add("@owner_unit_id", NpgsqlDbType.Integer);
                var owner_dept_id = cmd.Parameters.Add("@owner_dept_id", NpgsqlDbType.Integer);
                var owner_loc_id = cmd.Parameters.Add("@owner_loc_id", NpgsqlDbType.Integer);
                var total_no_of_tasks = cmd.Parameters.Add("@total_no_of_tasks", NpgsqlDbType.Bigint);
                cmd.Prepare();
                eval_hdr_id.Value = taskEvaluationHeader.Id;
                wki_fdr_id.Value = taskEvaluationHeader.TaskFolderId;
                eval_emp_id.Value = taskEvaluationHeader.EvaluatorId;
                owner_emp_id.Value = taskEvaluationHeader.TaskOwnerId;
                owner_unit_id.Value = taskEvaluationHeader.TaskOwnerUnitId;
                owner_dept_id.Value = taskEvaluationHeader.TaskOwnerDeptId;
                owner_loc_id.Value = taskEvaluationHeader.TaskOwnerLocationId;
                total_no_of_tasks.Value = taskEvaluationHeader.TotalNumberOfTasks;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion

        #region Task Evaluation Detail Read Action Methods
        public async Task<TaskEvaluationDetail> GetTaskEvaluationDetailByTaskEvaluationDetailIdAsync(long taskEvaluationDetailId)
        {
            List<TaskEvaluationDetail> taskEvaluationDetails = new List<TaskEvaluationDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.eval_dtl_id, d.eval_hdr_id, d.wki_fdr_id, d.tsk_itm_id, d.eval_dt, ");
            sb.Append("d.completion_score, d.quality_score, d.eval_comments, d.unit_id, d.dept_id, ");
            sb.Append("d.loc_id, d.tsk_owner_id, d.eval_emp_id, i.tsk_itm_ds, i.tsk_itm_no, i.tsk_itm_inf, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id= d.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = d.unit_id) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = d.dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = d.loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.eval_emp_id) as eval_emp_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("INNER JOIN public.wsp_tsk_itms i ON i.tsk_itm_id = d.tsk_itm_id ");
            sb.Append("WHERE (d.eval_dtl_id = @eval_dtl_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_dtl_id = cmd.Parameters.Add("@eval_dtl_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    eval_dtl_id.Value = taskEvaluationDetailId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationDetails.Add(new TaskEvaluationDetail
                            {
                                TaskEvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNo = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString(),
                                TaskItemMoreInfo = reader["tsk_itm_inf"] == DBNull.Value ? string.Empty : reader["tsk_itm_inf"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                CompletionScore = reader["completion_score"] == DBNull.Value ? 0 : (long)reader["completion_score"],
                                QualityScore = reader["quality_score"] == DBNull.Value ? 0 : (long)reader["quality_score"],
                                EvaluatorsComment = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),

                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationDetails[0];
        }
        public async Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsByTaskEvaluationHeaderIdAndTaskItemIdAsync(long taskEvaluationHeaderId, long taskItemId)
        {
            List<TaskEvaluationDetail> taskEvaluationDetails = new List<TaskEvaluationDetail>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.eval_dtl_id, d.eval_hdr_id, d.wki_fdr_id, d.tsk_itm_id, d.eval_dt, ");
            sb.Append("d.completion_score, d.quality_score, d.eval_comments, d.unit_id, d.dept_id, ");
            sb.Append("d.loc_id, d.tsk_owner_id, d.eval_emp_id, i.tsk_itm_ds, i.tsk_itm_no, i.tsk_itm_inf, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id= d.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = d.unit_id) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = d.dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = d.loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.eval_emp_id) as eval_emp_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("INNER JOIN public.wsp_tsk_itms i ON i.tsk_itm_id = d.tsk_itm_id ");
            sb.Append("WHERE (d.eval_hdr_id = @eval_hdr_id) ");
            sb.Append("AND (d.tsk_itm_id = @tsk_itm_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    eval_hdr_id.Value = taskEvaluationHeaderId;
                    tsk_itm_id.Value = taskItemId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationDetails.Add(new TaskEvaluationDetail
                            {
                                TaskEvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNo = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString(),
                                TaskItemMoreInfo = reader["tsk_itm_inf"] == DBNull.Value ? string.Empty : reader["tsk_itm_inf"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                CompletionScore = reader["completion_score"] == DBNull.Value ? 0 : (long)reader["completion_score"],
                                QualityScore = reader["quality_score"] == DBNull.Value ? 0 : (long)reader["quality_score"],
                                EvaluatorsComment = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),

                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationDetails;
        }
        public async Task<List<TaskEvaluationDetail>> GetTaskEvaluationDetailsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId)
        {
            List<TaskEvaluationDetail> taskEvaluationDetails = new List<TaskEvaluationDetail>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT d.eval_dtl_id, d.eval_hdr_id, d.wki_fdr_id, d.tsk_itm_id, d.eval_dt, ");
            sb.Append("d.completion_score, d.quality_score, d.eval_comments, d.unit_id, d.dept_id, ");
            sb.Append("d.loc_id, d.tsk_owner_id, d.eval_emp_id, i.tsk_itm_ds, i.tsk_itm_no, i.tsk_itm_inf, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id= d.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = d.unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = d.dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = d.loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.eval_emp_id) as eval_emp_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("INNER JOIN public.wsp_tsk_itms i ON i.tsk_itm_id = d.tsk_itm_id ");
            sb.Append("WHERE (d.eval_hdr_id = @eval_hdr_id) ");
            sb.Append("ORDER BY d.tsk_itm_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    eval_hdr_id.Value = taskEvaluationHeaderId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationDetails.Add(new TaskEvaluationDetail
                            {
                                TaskEvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNo = reader["tsk_itm_no"] == DBNull.Value ? string.Empty : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? string.Empty : reader["tsk_itm_ds"].ToString(),
                                TaskItemMoreInfo = reader["tsk_itm_inf"] == DBNull.Value ? string.Empty : reader["tsk_itm_inf"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                CompletionScore = reader["completion_score"] == DBNull.Value ? 0 : (long)reader["completion_score"],
                                QualityScore = reader["quality_score"] == DBNull.Value ? 0 : (long)reader["quality_score"],
                                EvaluatorsComment = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),

                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? 0 : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDeptId = reader["dept_id"] == DBNull.Value ? 0 : (int)reader["dept_id"],
                                TaskOwnerDeptName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? 0 : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationDetails;
        }
        public async Task<long> GetTaskEvaluationItemsCountByFolderIdnEvaluatorIdAsync(long taskFolderId, string evaluatorId)
        {
            long _totalCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT (tsk_itm_id) as total ");
            sb.Append("FROM public.wsp_eval_dtl ");
            sb.Append("WHERE (wki_fdr_id = @wki_fdr_id) ");
            sb.Append("AND (eval_emp_id = @eval_emp_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = taskFolderId;
                    eval_emp_id.Value = evaluatorId;
                    var obj = await cmd.ExecuteScalarAsync();
                    _totalCount = Convert.ToInt64(obj);
                }
                await conn.CloseAsync();
            }
            return _totalCount;
        }


        #endregion

        #region Task Evaluation Detail Write Action Methods
        public async Task<bool> AddTaskEvaluationDetailsAsync(TaskEvaluationDetail taskEvaluationDetail)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_eval_dtl (eval_hdr_id, ");
            sb.Append("wki_fdr_id, tsk_itm_id, eval_dt, completion_score, ");
            sb.Append("quality_score, eval_comments, unit_id, dept_id, ");
            sb.Append("loc_id, tsk_owner_id, eval_emp_id) ");
            sb.Append("VALUES (@eval_hdr_id, @wki_fdr_id, @tsk_itm_id, ");
            sb.Append("@eval_dt, @completion_score, @quality_score, ");
            sb.Append("@eval_comments, @unit_id, @dept_id, ");
            sb.Append("@loc_id, @tsk_owner_id, @eval_emp_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.Timestamp);
                    var completion_score = cmd.Parameters.Add("@completion_score", NpgsqlDbType.Integer);
                    var quality_score = cmd.Parameters.Add("@quality_score", NpgsqlDbType.Integer);
                    var eval_comments = cmd.Parameters.Add("@eval_comments", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    eval_hdr_id.Value = taskEvaluationDetail.TaskEvaluationHeaderId;
                    wki_fdr_id.Value = taskEvaluationDetail.TaskFolderId;
                    tsk_itm_id.Value = taskEvaluationDetail.TaskItemId;
                    eval_dt.Value = taskEvaluationDetail.EvaluationDate ?? DateTime.Now;
                    completion_score.Value = taskEvaluationDetail.CompletionScore;
                    quality_score.Value = taskEvaluationDetail.QualityScore;
                    eval_comments.Value = taskEvaluationDetail.EvaluatorsComment ?? (object)DBNull.Value;
                    unit_id.Value = taskEvaluationDetail.TaskOwnerUnitId;
                    dept_id.Value = taskEvaluationDetail.TaskOwnerDeptId;
                    loc_id.Value = taskEvaluationDetail.TaskOwnerLocationId;
                    tsk_owner_id.Value = taskEvaluationDetail.TaskOwnerId;
                    eval_emp_id.Value = taskEvaluationDetail.TaskEvaluatorId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateTaskEvaluationDetailsAsync(TaskEvaluationDetail taskEvaluationDetail)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_eval_dtl SET eval_hdr_id=@eval_hdr_id, ");
            sb.Append("wki_fdr_id=@wki_fdr_id, tsk_itm_id=@tsk_itm_id, eval_dt=@eval_dt, ");
            sb.Append("completion_score=@completion_score, quality_score=@quality_score, ");
            sb.Append("eval_comments=@eval_comments, unit_id=@unit_id, dept_id=@dept_id, ");
            sb.Append("loc_id=@loc_id, tsk_owner_id=@tsk_owner_id, eval_emp_id=@eval_emp_id ");
            sb.Append("WHERE (eval_dtl_id=@eval_dtl_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_dtl_id = cmd.Parameters.Add("@eval_dtl_id", NpgsqlDbType.Bigint);
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.Timestamp);
                    var completion_score = cmd.Parameters.Add("@completion_score", NpgsqlDbType.Integer);
                    var quality_score = cmd.Parameters.Add("@quality_score", NpgsqlDbType.Integer);
                    var eval_comments = cmd.Parameters.Add("@eval_comments", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    eval_dtl_id.Value = taskEvaluationDetail.TaskEvaluationDetailId;
                    eval_hdr_id.Value = taskEvaluationDetail.TaskEvaluationHeaderId;
                    wki_fdr_id.Value = taskEvaluationDetail.TaskFolderId;
                    tsk_itm_id.Value = taskEvaluationDetail.TaskItemId;
                    eval_dt.Value = taskEvaluationDetail.EvaluationDate ?? DateTime.Now;
                    completion_score.Value = taskEvaluationDetail.CompletionScore;
                    quality_score.Value = taskEvaluationDetail.QualityScore;
                    eval_comments.Value = taskEvaluationDetail.EvaluatorsComment ?? (object)DBNull.Value;
                    unit_id.Value = taskEvaluationDetail.TaskOwnerUnitId;
                    dept_id.Value = taskEvaluationDetail.TaskOwnerDeptId;
                    loc_id.Value = taskEvaluationDetail.TaskOwnerLocationId;
                    tsk_owner_id.Value = taskEvaluationDetail.TaskOwnerId;
                    eval_emp_id.Value = taskEvaluationDetail.TaskEvaluatorId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #region Evaluation Returns Action Methods
        public async Task<bool> AddTaskEvaluationReturnAsync(TaskEvaluationReturns taskEvaluationReturn)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_eval_rtns(wki_fdr_id, tsk_itm_id, ");
            sb.Append("rtn_dt, rtn_by, reason_typ, reason_dtl, unit_id, dept_id, ");
            sb.Append("loc_id, tsk_owner_id, is_xmpt) VALUES (@wki_fdr_id, @tsk_itm_id, ");
            sb.Append("@rtn_dt, @rtn_by, @reason_typ, @reason_dtl, @unit_id, ");
            sb.Append("@dept_id, @loc_id, @tsk_owner_id, @is_xmpt);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var rtn_dt = cmd.Parameters.Add("@rtn_dt", NpgsqlDbType.Timestamp);
                    var rtn_by = cmd.Parameters.Add("@rtn_by", NpgsqlDbType.Text);
                    var reason_typ = cmd.Parameters.Add("@reason_typ", NpgsqlDbType.Text);
                    var reason_dtl = cmd.Parameters.Add("@reason_dtl", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var is_xmpt = cmd.Parameters.Add("@is_xmpt", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    wki_fdr_id.Value = taskEvaluationReturn.TaskFolderId;
                    tsk_itm_id.Value = taskEvaluationReturn.TaskItemId;
                    rtn_dt.Value = taskEvaluationReturn.ReturnedDate ?? DateTime.Now;
                    rtn_by.Value = taskEvaluationReturn.ReturnedBy ?? (object)DBNull.Value;
                    reason_typ.Value = taskEvaluationReturn.ReasonType ?? (object)DBNull.Value;
                    reason_dtl.Value = taskEvaluationReturn.ReasonDetails ?? (object)DBNull.Value;
                    unit_id.Value = taskEvaluationReturn.TaskOwnerUnitId ?? (object)DBNull.Value;
                    dept_id.Value = taskEvaluationReturn.TaskOwnerDepartmentId ?? (object)DBNull.Value;
                    loc_id.Value = taskEvaluationReturn.TaskOwnerLocationId ?? (object)DBNull.Value;
                    tsk_owner_id.Value = taskEvaluationReturn.TaskOwnerId;
                    is_xmpt.Value = taskEvaluationReturn.ExemptFromEvaluation;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteTaskEvaluationReturnAsync(long taskItemId, string returnedBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wsp_eval_rtns ");
            sb.Append("WHERE (tsk_itm_id = @tsk_itm_id) ");
            sb.Append("AND (rtn_by = @rtn_by); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    var rtn_by = cmd.Parameters.Add("@rtn_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    rtn_by.Value = returnedBy;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskItemIdAsync(long taskItemId)
        {
            List<TaskEvaluationReturns> taskEvaluationReturns = new List<TaskEvaluationReturns>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.eval_rtn_id, r.eval_hdr_id, r.wki_fdr_id, r.tsk_itm_id, r.rtn_dt, r.rtn_by, ");
            sb.Append("r.reason_typ, r.reason_dtl, r.unit_id, r.dept_id, r.loc_id, r.tsk_owner_id, ");
            sb.Append("t.tsk_itm_no, tsk_itm_ds, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = r.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = r.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = r.loc_id) as loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm ");
            sb.Append("FROM public.wsp_eval_rtns r ");
            sb.Append("INNER JOIN public.wsp_tsk_itms t  ON t.tsk_itm_id = r.tsk_itm_id ");
            sb.Append("WHERE (r.tsk_itm_id = @tsk_itm_id) ");
            sb.Append("ORDER BY r.rtn_dt DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_itm_id = cmd.Parameters.Add("@tsk_itm_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    tsk_itm_id.Value = taskItemId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationReturns.Add(new TaskEvaluationReturns
                            {
                                EvaluationReturnId = reader["eval_rtn_id"] == DBNull.Value ? 0 : (long)reader["eval_rtn_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNumber = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                TaskOwnerDepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),
                                ReturnedBy = reader["rtn_by"] == DBNull.Value ? "" : reader["rtn_by"].ToString(),
                                ReturnedDate = reader["rtn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rtn_dt"],
                                ReasonType = reader["reason_typ"] == DBNull.Value ? string.Empty : reader["reason_typ"].ToString(),
                                ReasonDetails = reader["reason_dtl"] == DBNull.Value ? string.Empty : reader["reason_dtl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationReturns;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) { fromDate = DateTime.Now.AddMonths(-6); }
            if (toDate == null) { toDate = DateTime.Now; }

            List<TaskEvaluationReturns> taskEvaluationReturns = new List<TaskEvaluationReturns>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.eval_rtn_id, r.eval_hdr_id, r.wki_fdr_id, r.tsk_itm_id, r.rtn_dt, r.rtn_by, ");
            sb.Append("r.reason_typ, r.reason_dtl, r.unit_id, r.dept_id, r.loc_id, r.tsk_owner_id, ");
            sb.Append("t.tsk_itm_no, tsk_itm_ds, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = r.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = r.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = r.loc_id) as loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm ");
            sb.Append("FROM public.wsp_eval_rtns r ");
            sb.Append("INNER JOIN public.wsp_tsk_itms t  ON t.tsk_itm_id = r.tsk_itm_id ");
            sb.Append("WHERE (r.tsk_owner_id = @tsk_owner_id) ");
            sb.Append("AND (r.rtn_dt BETWEEN @from_date AND @to_date) ");
            sb.Append("ORDER BY r.rtn_dt DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_owner_id = cmd.Parameters.Add("@tsk_owner_id", NpgsqlDbType.Text);
                    var from_date = cmd.Parameters.Add("@from_date", NpgsqlDbType.Timestamp);
                    var to_date = cmd.Parameters.Add("@to_date", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    tsk_owner_id.Value = taskOwnerId;
                    from_date.Value = fromDate;
                    to_date.Value = toDate;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationReturns.Add(new TaskEvaluationReturns
                            {
                                EvaluationReturnId = reader["eval_rtn_id"] == DBNull.Value ? 0 : (long)reader["eval_rtn_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNumber = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                TaskOwnerDepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),
                                ReturnedBy = reader["rtn_by"] == DBNull.Value ? "" : reader["rtn_by"].ToString(),
                                ReturnedDate = reader["rtn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rtn_dt"],
                                ReasonType = reader["reason_typ"] == DBNull.Value ? string.Empty : reader["reason_typ"].ToString(),
                                ReasonDetails = reader["reason_dtl"] == DBNull.Value ? string.Empty : reader["reason_dtl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationReturns;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskFolderIdAsync(long taskFolderId)
        {
            List<TaskEvaluationReturns> taskEvaluationReturns = new List<TaskEvaluationReturns>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.eval_rtn_id, r.eval_hdr_id, r.wki_fdr_id, r.tsk_itm_id, r.rtn_dt, r.rtn_by, ");
            sb.Append("r.reason_typ, r.reason_dtl, r.unit_id, r.dept_id, r.loc_id, r.tsk_owner_id, ");
            sb.Append("t.tsk_itm_no, t.tsk_itm_ds, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = r.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = r.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = r.loc_id) as loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm ");
            sb.Append("FROM public.wsp_eval_rtns r ");
            sb.Append("INNER JOIN public.wsp_tsk_itms t ON t.tsk_itm_id = r.tsk_itm_id ");
            sb.Append("WHERE (r.wki_fdr_id = @wki_fdr_id) ");
            sb.Append("ORDER BY r.rtn_dt DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = taskFolderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationReturns.Add(new TaskEvaluationReturns
                            {
                                EvaluationReturnId = reader["eval_rtn_id"] == DBNull.Value ? 0 : (long)reader["eval_rtn_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNumber = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                TaskOwnerDepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),
                                ReturnedBy = reader["rtn_by"] == DBNull.Value ? "" : reader["rtn_by"].ToString(),
                                ReturnedDate = reader["rtn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rtn_dt"],
                                ReasonType = reader["reason_typ"] == DBNull.Value ? string.Empty : reader["reason_typ"].ToString(),
                                ReasonDetails = reader["reason_dtl"] == DBNull.Value ? string.Empty : reader["reason_dtl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationReturns;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskFolderIdAsync(long taskFolderId, bool isExempted = false)
        {
            List<TaskEvaluationReturns> taskEvaluationReturns = new List<TaskEvaluationReturns>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.eval_rtn_id, r.eval_hdr_id, r.wki_fdr_id, r.tsk_itm_id, r.rtn_dt, r.rtn_by, ");
            sb.Append("r.reason_typ, r.reason_dtl, r.unit_id, r.dept_id, r.loc_id, r.tsk_owner_id, ");
            sb.Append("t.tsk_itm_no, tsk_itm_ds, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = r.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = r.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = r.loc_id) as loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm ");
            sb.Append("FROM public.wsp_eval_rtns r ");
            sb.Append("INNER JOIN public.wsp_tsk_itms t  ON t.tsk_itm_id = r.tsk_itm_id ");
            sb.Append("WHERE (r.wki_fdr_id = @wki_fdr_id) AND (r.is_xmpt = @is_xmpt) ");
            sb.Append("ORDER BY r.rtn_dt DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var is_xmpt = cmd.Parameters.Add("@is_xmpt", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = taskFolderId;
                    is_xmpt.Value = isExempted;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationReturns.Add(new TaskEvaluationReturns
                            {
                                EvaluationReturnId = reader["eval_rtn_id"] == DBNull.Value ? 0 : (long)reader["eval_rtn_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNumber = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                TaskOwnerDepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),
                                ReturnedBy = reader["rtn_by"] == DBNull.Value ? "" : reader["rtn_by"].ToString(),
                                ReturnedDate = reader["rtn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rtn_dt"],
                                ReasonType = reader["reason_typ"] == DBNull.Value ? string.Empty : reader["reason_typ"].ToString(),
                                ReasonDetails = reader["reason_dtl"] == DBNull.Value ? string.Empty : reader["reason_dtl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationReturns;
        }

        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId)
        {
            List<TaskEvaluationReturns> taskEvaluationReturns = new List<TaskEvaluationReturns>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.eval_rtn_id, r.eval_hdr_id, r.wki_fdr_id, r.tsk_itm_id, r.rtn_dt, r.rtn_by, ");
            sb.Append("r.reason_typ, r.reason_dtl, r.unit_id, r.dept_id, r.loc_id, r.tsk_owner_id, ");
            sb.Append("t.tsk_itm_no, t.tsk_itm_ds, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = r.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = r.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = r.loc_id) as loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm ");
            sb.Append("FROM public.wsp_eval_rtns r ");
            sb.Append("INNER JOIN public.wsp_tsk_itms t ON t.tsk_itm_id = r.tsk_itm_id ");
            sb.Append("WHERE (r.eval_hdr_id = @eval_hdr_id) ");
            sb.Append("ORDER BY r.rtn_dt DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    eval_hdr_id.Value = taskEvaluationHeaderId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationReturns.Add(new TaskEvaluationReturns
                            {
                                EvaluationReturnId = reader["eval_rtn_id"] == DBNull.Value ? 0 : (long)reader["eval_rtn_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNumber = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                TaskOwnerDepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),
                                ReturnedBy = reader["rtn_by"] == DBNull.Value ? "" : reader["rtn_by"].ToString(),
                                ReturnedDate = reader["rtn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rtn_dt"],
                                ReasonType = reader["reason_typ"] == DBNull.Value ? string.Empty : reader["reason_typ"].ToString(),
                                ReasonDetails = reader["reason_dtl"] == DBNull.Value ? string.Empty : reader["reason_dtl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationReturns;
        }
        public async Task<List<TaskEvaluationReturns>> GetTaskEvaluationReturnsByTaskEvaluationHeaderIdAsync(long taskEvaluationHeaderId, bool isExempted = false)
        {
            List<TaskEvaluationReturns> taskEvaluationReturns = new List<TaskEvaluationReturns>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.eval_rtn_id, r.eval_hdr_id, r.wki_fdr_id, r.tsk_itm_id, r.rtn_dt, r.rtn_by, ");
            sb.Append("r.reason_typ, r.reason_dtl, r.unit_id, r.dept_id, r.loc_id, r.tsk_owner_id, ");
            sb.Append("t.tsk_itm_no, tsk_itm_ds, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = r.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = r.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = r.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = r.loc_id) as loc_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm ");
            sb.Append("FROM public.wsp_eval_rtns r ");
            sb.Append("INNER JOIN public.wsp_tsk_itms t  ON t.tsk_itm_id = r.tsk_itm_id ");
            sb.Append("WHERE (r.eval_hdr_id = @eval_hdr_id) AND (r.is_xmpt = @is_xmpt) ");
            sb.Append("ORDER BY r.rtn_dt DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    var is_xmpt = cmd.Parameters.Add("@is_xmpt", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    eval_hdr_id.Value = taskEvaluationHeaderId;
                    is_xmpt.Value = isExempted;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationReturns.Add(new TaskEvaluationReturns
                            {
                                EvaluationReturnId = reader["eval_rtn_id"] == DBNull.Value ? 0 : (long)reader["eval_rtn_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskItemId = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                TaskItemNumber = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                TaskItemDescription = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                TaskOwnerUnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                TaskOwnerUnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                TaskOwnerDepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                TaskOwnerLocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),
                                ReturnedBy = reader["rtn_by"] == DBNull.Value ? "" : reader["rtn_by"].ToString(),
                                ReturnedDate = reader["rtn_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rtn_dt"],
                                ReasonType = reader["reason_typ"] == DBNull.Value ? string.Empty : reader["reason_typ"].ToString(),
                                ReasonDetails = reader["reason_dtl"] == DBNull.Value ? string.Empty : reader["reason_dtl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationReturns;
        }
        #endregion

        #region Task Evaluation Summary Write Action Methods
        public async Task<long> AddTaskEvaluationSummaryAsync(TaskEvaluationSummary s)
        {
            long id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wsp_eval_smr(eval_hdr_id, ");
            sb.Append("wki_fdr_id, eval_emp_id, owner_emp_id, owner_unit_id, ");
            sb.Append("owner_dept_id, owner_loc_id, eval_dt, total_no_of_tasks, ");
            sb.Append("no_completed_tasks, no_uncompleted_tasks, ");
            sb.Append("total_completion_score, average_completion_score, ");
            sb.Append("total_quality_score, average_quality_score) ");
            sb.Append("VALUES (@eval_hdr_id, @wki_fdr_id, @eval_emp_id, ");
            sb.Append("@owner_emp_id, @owner_unit_id, @owner_dept_id, ");
            sb.Append("@owner_loc_id, @eval_dt, @total_no_of_tasks, ");
            sb.Append("@no_completed_tasks, @no_uncompleted_tasks, ");
            sb.Append("@total_completion_score, @average_completion_score, ");
            sb.Append("@total_quality_score, @average_quality_score) ");
            sb.Append("returning eval_smr_id;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var owner_unit_id = cmd.Parameters.Add("@owner_unit_id", NpgsqlDbType.Integer);
                    var owner_dept_id = cmd.Parameters.Add("@owner_dept_id", NpgsqlDbType.Integer);
                    var owner_loc_id = cmd.Parameters.Add("@owner_loc_id", NpgsqlDbType.Integer);
                    var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.Timestamp);
                    var total_no_of_tasks = cmd.Parameters.Add("@total_no_of_tasks", NpgsqlDbType.Bigint);
                    var no_completed_tasks = cmd.Parameters.Add("@no_completed_tasks", NpgsqlDbType.Bigint);
                    var no_uncompleted_tasks = cmd.Parameters.Add("@no_uncompleted_tasks", NpgsqlDbType.Bigint);
                    var total_completion_score = cmd.Parameters.Add("@total_completion_score", NpgsqlDbType.Numeric);
                    var average_completion_score = cmd.Parameters.Add("@average_completion_score", NpgsqlDbType.Numeric);
                    var total_quality_score = cmd.Parameters.Add("@total_quality_score", NpgsqlDbType.Numeric);
                    var average_quality_score = cmd.Parameters.Add("@average_quality_score", NpgsqlDbType.Numeric);
                    cmd.Prepare();
                    eval_hdr_id.Value = s.TaskEvaluationHeaderId;
                    wki_fdr_id.Value = s.TaskFolderId;
                    eval_emp_id.Value = s.TaskEvaluatorId;
                    owner_emp_id.Value = s.TaskOwnerId;
                    owner_unit_id.Value = s.TaskOwnerUnitId;
                    owner_dept_id.Value = s.TaskOwnerDepartmentId;
                    owner_loc_id.Value = s.TaskOwnerLocationId;
                    eval_dt.Value = s.EvaluationDate ?? DateTime.Now;
                    total_no_of_tasks.Value = s.TotalNoOfTasks;
                    no_completed_tasks.Value = s.TotalNoOfCompletedTasks;
                    no_uncompleted_tasks.Value = s.TotalNoOfUncompletedTasks;
                    total_completion_score.Value = s.TotalCompletionScore;
                    average_completion_score.Value = s.AverageCompletionScore;
                    total_quality_score.Value = s.TotalQualityScore;
                    average_quality_score.Value = s.AverageQualityScore;
                    var result = await cmd.ExecuteScalarAsync();
                    id = (long)result;
                }
                await conn.CloseAsync();
            }
            return id;
        }
        public async Task<bool> UpdateTaskEvaluationSummaryAsync(TaskEvaluationSummary s)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wsp_eval_smr SET eval_hdr_id=@eval_hdr_id, ");
            sb.Append("wki_fdr_id=@wki_fdr_id, eval_emp_id=@eval_emp_id, ");
            sb.Append("owner_emp_id=@owner_emp_id, owner_unit_id=@owner_unit_id, ");
            sb.Append("owner_dept_id=@owner_dept_id, owner_loc_id=@owner_loc_id, ");
            sb.Append("eval_dt=@eval_dt, total_no_of_tasks=@total_no_of_tasks, ");
            sb.Append("no_completed_tasks=@no_completed_tasks, ");
            sb.Append("no_uncompleted_tasks=@no_uncompleted_tasks, ");
            sb.Append("total_completion_score=@total_completion_score, ");
            sb.Append("average_completion_score=@average_completion_score, ");
            sb.Append("total_quality_score=@total_quality_score, ");
            sb.Append("average_quality_score=@average_quality_score ");
            sb.Append("WHERE (eval_smr_id=@eval_smr_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_smr_id = cmd.Parameters.Add("@eval_smr_id", NpgsqlDbType.Bigint);
                    var eval_hdr_id = cmd.Parameters.Add("@eval_hdr_id", NpgsqlDbType.Bigint);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var owner_unit_id = cmd.Parameters.Add("@owner_unit_id", NpgsqlDbType.Integer);
                    var owner_dept_id = cmd.Parameters.Add("@owner_dept_id", NpgsqlDbType.Integer);
                    var owner_loc_id = cmd.Parameters.Add("@owner_loc_id", NpgsqlDbType.Integer);
                    var eval_dt = cmd.Parameters.Add("@eval_dt", NpgsqlDbType.Timestamp);
                    var total_no_of_tasks = cmd.Parameters.Add("@total_no_of_tasks", NpgsqlDbType.Bigint);
                    var no_completed_tasks = cmd.Parameters.Add("@no_completed_tasks", NpgsqlDbType.Bigint);
                    var no_uncompleted_tasks = cmd.Parameters.Add("@no_uncompleted_tasks", NpgsqlDbType.Bigint);
                    var total_completion_score = cmd.Parameters.Add("@total_completion_score", NpgsqlDbType.Numeric);
                    var average_completion_score = cmd.Parameters.Add("@average_completion_score, ", NpgsqlDbType.Numeric);
                    var total_quality_score = cmd.Parameters.Add("@total_quality_score", NpgsqlDbType.Numeric);
                    var average_quality_score = cmd.Parameters.Add("@average_quality_score", NpgsqlDbType.Numeric);
                    cmd.Prepare();
                    eval_smr_id.Value = s.Id;
                    eval_hdr_id.Value = s.TaskEvaluationHeaderId;
                    wki_fdr_id.Value = s.TaskFolderId;
                    eval_emp_id.Value = s.TaskEvaluatorId;
                    owner_emp_id.Value = s.TaskOwnerId;
                    owner_unit_id.Value = s.TaskOwnerUnitId;
                    owner_dept_id.Value = s.TaskOwnerDepartmentId;
                    owner_loc_id.Value = s.TaskOwnerLocationId;
                    eval_dt.Value = s.EvaluationDate ?? DateTime.Now;
                    total_no_of_tasks.Value = s.TotalNoOfTasks;
                    no_completed_tasks.Value = s.TotalNoOfCompletedTasks;
                    no_uncompleted_tasks.Value = s.TotalNoOfUncompletedTasks;
                    total_completion_score.Value = s.TotalCompletionScore;
                    average_completion_score.Value = s.AverageCompletionScore;
                    total_quality_score.Value = s.TotalQualityScore;
                    average_quality_score.Value = s.AverageQualityScore;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }


        #endregion

        #region Task Evaluation Summary By Location, Department and Unit Action Methods

        //======= Get By Task Owner ID ====//
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaries = new List<TaskEvaluationSummary>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.eval_smr_id, s.eval_hdr_id, s.wki_fdr_id, s.eval_emp_id, s.owner_emp_id, ");
            sb.Append("s.owner_unit_id, s.owner_dept_id, s.owner_loc_id, s.eval_dt, s.total_no_of_tasks, ");
            sb.Append("s.no_completed_tasks, s.no_uncompleted_tasks, s.total_completion_score, ");
            sb.Append("ROUND(s.average_completion_score, 2) AS average_completion_score, s.total_quality_score, ");
            sb.Append("ROUND(s.average_quality_score, 2) AS average_quality_score, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = s.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = s.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = s.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = s.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_smr s  ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = s.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = s.owner_emp_id ");
            sb.Append("WHERE s.owner_emp_id = @owner_emp_id ");
            sb.Append("AND (s.eval_dt >= @dt_frm) ");
            sb.Append("AND (s.eval_dt <= @dt_to) ");
            sb.Append("ORDER BY s.eval_smr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    owner_emp_id.Value = taskOwnerId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationSummaries.Add(new TaskEvaluationSummary
                            {
                                Id = reader["eval_smr_id"] == DBNull.Value ? 0 : (long)reader["eval_smr_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                TaskEvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],

                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDepartmentName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TotalNoOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                TotalNoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                TotalNoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],
                                TotalCompletionScore = reader["total_completion_score"] == DBNull.Value ? 0 : (long)reader["total_completion_score"],
                                TotalQualityScore = reader["total_quality_score"] == DBNull.Value ? 0 : (long)reader["total_quality_score"],

                                AverageCompletionScore = reader["average_completion_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_completion_score"],
                                AverageQualityScore = reader["average_quality_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationSummaries;
        }

        //======= Get By Unit ID =======//
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByUnitIdAsync(int taskOwnerUnitId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaries = new List<TaskEvaluationSummary>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.eval_smr_id, s.eval_hdr_id, s.wki_fdr_id, s.eval_emp_id, s.owner_emp_id, ");
            sb.Append("s.owner_unit_id, s.owner_dept_id, s.owner_loc_id, s.eval_dt, s.total_no_of_tasks, ");
            sb.Append("s.no_completed_tasks, s.no_uncompleted_tasks, s.total_completion_score, ");
            sb.Append("ROUND(s.average_completion_score, 2) AS average_completion_score, s.total_quality_score, ");
            sb.Append("ROUND(s.average_quality_score, 2) AS average_quality_score, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = s.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = s.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = s.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = s.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_smr s  ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = s.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = s.owner_emp_id ");
            sb.Append("WHERE s.owner_unit_id = @unit_id ");
            sb.Append("AND (s.eval_dt >= @dt_frm) ");
            sb.Append("AND (s.eval_dt <= @dt_to) ");
            sb.Append("ORDER BY s.eval_smr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    unit_id.Value = taskOwnerUnitId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationSummaries.Add(new TaskEvaluationSummary
                            {
                                Id = reader["eval_smr_id"] == DBNull.Value ? 0 : (long)reader["eval_smr_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                TaskEvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],

                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDepartmentName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TotalNoOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                TotalNoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                TotalNoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],

                                TotalCompletionScore = reader["total_completion_score"] == DBNull.Value ? 0 : (long)reader["total_completion_score"],
                                TotalQualityScore = reader["total_quality_score"] == DBNull.Value ? 0 : (long)reader["total_quality_score"],

                                AverageCompletionScore = reader["average_completion_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_completion_score"],
                                AverageQualityScore = reader["average_quality_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationSummaries;
        }
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByUnitIdAndLocationIdAsync(int taskOwnerUnitId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaries = new List<TaskEvaluationSummary>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.eval_smr_id, s.eval_hdr_id, s.wki_fdr_id, s.eval_emp_id, s.owner_emp_id, ");
            sb.Append("s.owner_unit_id, s.owner_dept_id, s.owner_loc_id, s.eval_dt, s.total_no_of_tasks, ");
            sb.Append("s.no_completed_tasks, s.no_uncompleted_tasks, s.total_completion_score, ");
            sb.Append("ROUND(s.average_completion_score, 2) AS average_completion_score, s.total_quality_score, ");
            sb.Append("ROUND(s.average_quality_score, 2) AS average_quality_score, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = s.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = s.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = s.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = s.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_smr s  ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = s.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = s.owner_emp_id ");
            sb.Append("WHERE (s.owner_unit_id = @unit_id) AND (s.owner_loc_id = @loc_id) ");
            sb.Append("AND (s.eval_dt >= @dt_frm) ");
            sb.Append("AND (s.eval_dt <= @dt_to) ");
            sb.Append("ORDER BY s.eval_smr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    unit_id.Value = taskOwnerUnitId;
                    loc_id.Value = taskOwnerLocationId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationSummaries.Add(new TaskEvaluationSummary
                            {
                                Id = reader["eval_smr_id"] == DBNull.Value ? 0 : (long)reader["eval_smr_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                TaskEvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],

                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDepartmentName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TotalNoOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                TotalNoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                TotalNoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],

                                TotalCompletionScore = reader["total_completion_score"] == DBNull.Value ? 0 : (long)reader["total_completion_score"],
                                TotalQualityScore = reader["total_quality_score"] == DBNull.Value ? 0 : (long)reader["total_quality_score"],

                                AverageCompletionScore = reader["average_completion_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_completion_score"],
                                AverageQualityScore = reader["average_quality_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationSummaries;

        }

        //======= Get By Department ID ====//
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByDepartmentIdAsync(int taskOwnerDepartmentId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaries = new List<TaskEvaluationSummary>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.eval_smr_id, s.eval_hdr_id, s.wki_fdr_id, s.eval_emp_id, s.owner_emp_id, ");
            sb.Append("s.owner_unit_id, s.owner_dept_id, s.owner_loc_id, s.eval_dt, s.total_no_of_tasks, ");
            sb.Append("s.no_completed_tasks, s.no_uncompleted_tasks, s.total_completion_score, ");
            sb.Append("ROUND(s.average_completion_score, 2) AS average_completion_score, s.total_quality_score, ");
            sb.Append("ROUND(s.average_quality_score, 2) AS average_quality_score, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = s.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = s.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = s.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = s.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_smr s  ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = s.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = s.owner_emp_id ");
            sb.Append("WHERE s.owner_dept_id = @dept_id ");
            sb.Append("AND (s.eval_dt >= @dt_frm) ");
            sb.Append("AND (s.eval_dt <= @dt_to) ");
            sb.Append("ORDER BY s.eval_smr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    dept_id.Value = taskOwnerDepartmentId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationSummaries.Add(new TaskEvaluationSummary
                            {
                                Id = reader["eval_smr_id"] == DBNull.Value ? 0 : (long)reader["eval_smr_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                TaskEvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],

                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDepartmentName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TotalNoOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                TotalNoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                TotalNoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],

                                TotalCompletionScore = reader["total_completion_score"] == DBNull.Value ? 0 : (long)reader["total_completion_score"],
                                TotalQualityScore = reader["total_quality_score"] == DBNull.Value ? 0 : (long)reader["total_quality_score"],

                                AverageCompletionScore = reader["average_completion_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_completion_score"],
                                AverageQualityScore = reader["average_quality_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationSummaries;
        }
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByDepartmentIdAndLocationIdAsync(int taskOwnerDepartmentId, int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaries = new List<TaskEvaluationSummary>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.eval_smr_id, s.eval_hdr_id, s.wki_fdr_id, s.eval_emp_id, s.owner_emp_id, ");
            sb.Append("s.owner_unit_id, s.owner_dept_id, s.owner_loc_id, s.eval_dt, s.total_no_of_tasks, ");
            sb.Append("s.no_completed_tasks, s.no_uncompleted_tasks, s.total_completion_score, ");
            sb.Append("ROUND(s.average_completion_score, 2) AS average_completion_score, s.total_quality_score, ");
            sb.Append("ROUND(s.average_quality_score, 2) AS average_quality_score, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = s.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = s.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = s.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = s.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_smr s  ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = s.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = s.owner_emp_id ");
            sb.Append("WHERE (s.owner_dept_id = @dept_id) AND (s.owner_loc_id = @loc_id) ");
            sb.Append("AND (s.eval_dt >= @dt_frm) ");
            sb.Append("AND (s.eval_dt <= @dt_to) ");
            sb.Append("ORDER BY s.eval_smr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    dept_id.Value = taskOwnerDepartmentId;
                    loc_id.Value = taskOwnerLocationId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationSummaries.Add(new TaskEvaluationSummary
                            {
                                Id = reader["eval_smr_id"] == DBNull.Value ? 0 : (long)reader["eval_smr_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                TaskEvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],

                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDepartmentName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TotalNoOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                TotalNoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                TotalNoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],

                                TotalCompletionScore = reader["total_completion_score"] == DBNull.Value ? 0 : (long)reader["total_completion_score"],
                                TotalQualityScore = reader["total_quality_score"] == DBNull.Value ? 0 : (long)reader["total_quality_score"],

                                AverageCompletionScore = reader["average_completion_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_completion_score"],
                                AverageQualityScore = reader["average_quality_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationSummaries;

        }

        //======= Get By Location ID ======//
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryByLocationIdAsync(int taskOwnerLocationId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaries = new List<TaskEvaluationSummary>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.eval_smr_id, s.eval_hdr_id, s.wki_fdr_id, s.eval_emp_id, s.owner_emp_id, ");
            sb.Append("s.owner_unit_id, s.owner_dept_id, s.owner_loc_id, s.eval_dt, s.total_no_of_tasks, ");
            sb.Append("s.no_completed_tasks, s.no_uncompleted_tasks, s.total_completion_score, ");
            sb.Append("ROUND(s.average_completion_score, 2) AS average_completion_score, s.total_quality_score, ");
            sb.Append("ROUND(s.average_quality_score, 2) AS average_quality_score, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = s.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = s.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = s.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = s.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_smr s  ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = s.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = s.owner_emp_id ");
            sb.Append("WHERE s.owner_loc_id = @loc_id ");
            sb.Append("AND (s.eval_dt >= @dt_frm) ");
            sb.Append("AND (s.eval_dt <= @dt_to) ");
            sb.Append("ORDER BY s.eval_smr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    loc_id.Value = taskOwnerLocationId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationSummaries.Add(new TaskEvaluationSummary
                            {
                                Id = reader["eval_smr_id"] == DBNull.Value ? 0 : (long)reader["eval_smr_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                TaskEvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],

                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDepartmentName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TotalNoOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                TotalNoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                TotalNoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],

                                TotalCompletionScore = reader["total_completion_score"] == DBNull.Value ? 0 : (long)reader["total_completion_score"],
                                TotalQualityScore = reader["total_quality_score"] == DBNull.Value ? 0 : (long)reader["total_quality_score"],

                                AverageCompletionScore = reader["average_completion_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_completion_score"],
                                AverageQualityScore = reader["average_quality_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationSummaries;
        }
        public async Task<List<TaskEvaluationSummary>> GetTaskEvaluationSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationSummary> taskEvaluationSummaries = new List<TaskEvaluationSummary>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT s.eval_smr_id, s.eval_hdr_id, s.wki_fdr_id, s.eval_emp_id, s.owner_emp_id, ");
            sb.Append("s.owner_unit_id, s.owner_dept_id, s.owner_loc_id, s.eval_dt, s.total_no_of_tasks, ");
            sb.Append("s.no_completed_tasks, s.no_uncompleted_tasks, s.total_completion_score, ");
            sb.Append("ROUND(s.average_completion_score, 2) AS average_completion_score, s.total_quality_score, ");
            sb.Append("ROUND(s.average_quality_score, 2) AS average_quality_score, f.wki_fdr_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.owner_emp_id) as owner_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = s.eval_emp_id) as eval_emp_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = s.owner_unit_id ) as owner_unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = s.owner_dept_id ) as owner_dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = s.owner_loc_id ) as owner_loc_nm, ");
            sb.Append("(SELECT current_designation FROM public.erm_emp_inf WHERE emp_id = s.eval_emp_id ) as eval_emp_designation ");
            sb.Append("FROM public.wsp_eval_smr s  ");
            sb.Append("INNER JOIN public.wsp_wki_fdr f ON f.wki_fdr_id = s.wki_fdr_id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON e.emp_id = s.owner_emp_id ");
            sb.Append("WHERE (s.eval_dt >= @dt_frm) AND (s.eval_dt <= @dt_to) ");
            sb.Append("ORDER BY s.eval_smr_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationSummaries.Add(new TaskEvaluationSummary
                            {
                                Id = reader["eval_smr_id"] == DBNull.Value ? 0 : (long)reader["eval_smr_id"],
                                TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                TaskFolderName = reader["wki_fdr_nm"] == DBNull.Value ? string.Empty : reader["wki_fdr_nm"].ToString(),
                                TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString(),
                                TaskOwnerName = reader["owner_emp_nm"] == DBNull.Value ? string.Empty : reader["owner_emp_nm"].ToString(),
                                TaskEvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                TaskEvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString(),
                                TaskEvaluatorDesignation = reader["eval_emp_designation"] == DBNull.Value ? string.Empty : reader["eval_emp_designation"].ToString(),
                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],

                                TaskOwnerUnitId = reader["owner_unit_id"] == DBNull.Value ? 0 : (int)reader["owner_unit_id"],
                                TaskOwnerUnitName = reader["owner_unit_nm"] == DBNull.Value ? string.Empty : reader["owner_unit_nm"].ToString(),
                                TaskOwnerDepartmentId = reader["owner_dept_id"] == DBNull.Value ? 0 : (int)reader["owner_dept_id"],
                                TaskOwnerDepartmentName = reader["owner_dept_nm"] == DBNull.Value ? string.Empty : reader["owner_dept_nm"].ToString(),
                                TaskOwnerLocationId = reader["owner_loc_id"] == DBNull.Value ? 0 : (int)reader["owner_loc_id"],
                                TaskOwnerLocationName = reader["owner_loc_nm"] == DBNull.Value ? string.Empty : reader["owner_loc_nm"].ToString(),

                                TotalNoOfTasks = reader["total_no_of_tasks"] == DBNull.Value ? 0 : (long)reader["total_no_of_tasks"],
                                TotalNoOfCompletedTasks = reader["no_completed_tasks"] == DBNull.Value ? 0 : (long)reader["no_completed_tasks"],
                                TotalNoOfUncompletedTasks = reader["no_uncompleted_tasks"] == DBNull.Value ? 0 : (long)reader["no_uncompleted_tasks"],

                                TotalCompletionScore = reader["total_completion_score"] == DBNull.Value ? 0 : (long)reader["total_completion_score"],
                                TotalQualityScore = reader["total_quality_score"] == DBNull.Value ? 0 : (long)reader["total_quality_score"],

                                AverageCompletionScore = reader["average_completion_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_completion_score"],
                                AverageQualityScore = reader["average_quality_score"] == DBNull.Value ? 0.00M : (decimal)reader["average_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationSummaries;
        }

        #endregion

        #region Task Item Evaluation Read Action Methods
        public async Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsByTaskFolderIdAndEvaluatorIdAsync(long taskFolderId, string evaluatorId)
        {
            List<TaskItemEvaluation> taskItemEvaluations = new List<TaskItemEvaluation>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
            sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
            sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
            sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
            sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
            sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
            sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
            sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
            sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
            sb.Append("WHEN 2 THEN 'Approved for Execution' ");
            sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
            sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
            sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");

            sb.Append("CASE t.prgs_stts WHEN 0 THEN 'Not Yet Started' ");
            sb.Append("WHEN 1 THEN 'In Progress' ");
            sb.Append("WHEN 2 THEN 'Completed' ");
            sb.Append("WHEN 3 THEN 'On Hold' END AS prgs_stts_ds, ");

            sb.Append("CASE t.apprv_stts WHEN 0 THEN 'Pending' ");
            sb.Append("WHEN 1 THEN 'Approved' ");
            sb.Append("WHEN 2 THEN 'Declined' END AS apprv_stts_ds, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("d.eval_dtl_id, d.eval_hdr_id, d.eval_dt, d.completion_score, d.quality_score, ");
            sb.Append("d.eval_comments, d.eval_emp_id, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.eval_emp_id) as eval_emp_nm ");
            sb.Append("FROM public.wsp_tsk_itms t  ");

            sb.Append("LEFT JOIN public.wsp_eval_hdr h ON (h.wki_fdr_id = t.wki_fdr_id ");
            sb.Append("AND LOWER(h.eval_emp_id) = LOWER(@eval_emp_id)) ");

            sb.Append("LEFT JOIN public.wsp_eval_dtl d ON d.tsk_itm_id = t.tsk_itm_id AND d.wki_fdr_id = @wki_fdr_id ");
            sb.Append("WHERE (t.wki_fdr_id = @wki_fdr_id) ");
            sb.Append("ORDER BY t.tsk_itm_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = taskFolderId;
                    eval_emp_id.Value = evaluatorId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItemEvaluations.Add(new TaskItemEvaluation
                            {
                                Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                                WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                                LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                                LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                                LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                                AssignedToName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                                StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                                Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],

                                ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                                ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                                ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString(),

                                ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                                ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                                ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString(),

                                ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                                ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                                IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                                CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                                CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                                IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                                ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                                ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                                UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                                CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                                CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                                CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                                IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                                CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                AssignmentId = reader["assgnmt_id"] == DBNull.Value ? (long?)null : (long)reader["assgnmt_id"],

                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                EvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                                EvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                CompletionScore = reader["completion_score"] == DBNull.Value ? 0 : (long)reader["completion_score"],
                                QualityScore = reader["quality_score"] == DBNull.Value ? 0 : (long)reader["quality_score"],
                                EvaluatorComments = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),
                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString()
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskItemEvaluations;
        }
        public async Task<List<TaskItemEvaluation>> GetTaskItemEvaluationsByTaskFolderIdAsync(long taskFolderId)
        {
            List<TaskItemEvaluation> taskItemEvaluations = new List<TaskItemEvaluation>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tsk_itm_id, t.tsk_itm_no, t.tsk_itm_ds, t.tsk_itm_inf, t.wki_fdr_id, ");
            sb.Append("t.mst_tsk_id, t.prj_no, t.prg_no, t.prg_dt, t.tsk_owner_id, t.assgnd_emp_id, ");
            sb.Append("t.assigned_dt, t.tsk_itm_stg, t.prgs_stts, t.apprv_stts, t.approved_dt, ");
            sb.Append("t.approved_by, t.exp_start_dt, t.act_start_dt, t.exp_due_dt, t.act_due_dt, ");
            sb.Append("t.is_cancelled, t.cancelled_dt, t.cancelled_by, t.is_closed, t.closed_dt, ");
            sb.Append("t.closed_by, t.unit_id, t.dept_id, t.loc_id, t.completion_is_confirmed, ");
            sb.Append("t.completion_confirmed_by, t.completion_confirmed_on, t.is_carried_over, ");
            sb.Append("t.mod_by, t.crt_by, t.assgnmt_id, t.is_lckd, t.crt_dt, t.mod_dt, ");
            sb.Append("CASE t.tsk_itm_stg WHEN 0 THEN 'Not Yet Approved' ");
            sb.Append("WHEN 1 THEN 'Submitted for Approval' ");
            sb.Append("WHEN 2 THEN 'Approved for Execution' ");
            sb.Append("WHEN 3 THEN 'Submitted For Evaluation' ");
            sb.Append("WHEN 4 THEN 'Evaluation Completed' ");
            sb.Append("WHEN 5 THEN 'Cancelled' END AS itm_stg_nm, ");

            sb.Append("CASE t.prgs_stts WHEN 0 THEN 'Not Yet Started' ");
            sb.Append("WHEN 1 THEN 'In Progress' ");
            sb.Append("WHEN 2 THEN 'Completed' ");
            sb.Append("WHEN 3 THEN 'On Hold' END AS prgs_stts_ds, ");

            sb.Append("CASE t.apprv_stts WHEN 0 THEN 'Pending' ");
            sb.Append("WHEN 1 THEN 'Approved' ");
            sb.Append("WHEN 2 THEN 'Declined' END AS apprv_stts_ds, ");

            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.tsk_owner_id) as owner_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = t.assgnd_emp_id) as assgnd_to_nm, ");
            sb.Append("(SELECT unitname FROM public.gst_units WHERE unitqk = t.unit_id) as unit_nm, ");
            sb.Append("(SELECT deptname FROM public.gst_depts WHERE deptqk = t.dept_id) as dept_nm, ");
            sb.Append("(SELECT locname FROM public.gst_locs WHERE locqk = t.loc_id) as loc_nm, ");
            sb.Append("(SELECT wki_fdr_nm FROM public.wsp_wki_fdr WHERE wki_fdr_id = t.wki_fdr_id) as wki_fdr_nm, ");
            sb.Append("d.eval_dtl_id, d.eval_hdr_id, d.eval_dt, d.completion_score, d.quality_score, ");
            sb.Append("d.eval_comments, d.eval_emp_id, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.eval_emp_id) as eval_emp_nm ");
            sb.Append("FROM public.wsp_tsk_itms t  ");

            sb.Append("LEFT JOIN public.wsp_eval_hdr h ON (h.wki_fdr_id = t.wki_fdr_id) ");

            sb.Append("LEFT JOIN public.wsp_eval_dtl d ON d.tsk_itm_id = t.tsk_itm_id AND d.wki_fdr_id = @wki_fdr_id ");
            sb.Append("WHERE (t.wki_fdr_id = @wki_fdr_id) ");
            sb.Append("ORDER BY d.eval_emp_id, t.tsk_itm_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    wki_fdr_id.Value = taskFolderId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskItemEvaluations.Add(new TaskItemEvaluation
                            {
                                Id = reader["tsk_itm_id"] == DBNull.Value ? 0 : (long)reader["tsk_itm_id"],
                                Number = reader["tsk_itm_no"] == DBNull.Value ? "" : reader["tsk_itm_no"].ToString(),
                                Description = reader["tsk_itm_ds"] == DBNull.Value ? "" : reader["tsk_itm_ds"].ToString(),
                                MoreInformation = reader["tsk_itm_inf"] == DBNull.Value ? "" : reader["tsk_itm_inf"].ToString(),
                                WorkFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"],
                                WorkFolderName = reader["wki_fdr_nm"] == DBNull.Value ? "" : reader["wki_fdr_nm"].ToString(),
                                MasterTaskId = reader["mst_tsk_id"] == DBNull.Value ? 0 : (long)reader["mst_tsk_id"],
                                LinkProjectNumber = reader["prj_no"] == DBNull.Value ? "" : reader["prj_no"].ToString(),
                                LinkProgramCode = reader["prg_no"] == DBNull.Value ? "" : reader["prg_no"].ToString(),
                                LinkProgramDate = reader["prg_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["prg_dt"],
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? "" : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["owner_nm"] == DBNull.Value ? string.Empty : reader["owner_nm"].ToString(),
                                AssignedToId = reader["assgnd_emp_id"] == DBNull.Value ? "" : reader["assgnd_emp_id"].ToString(),
                                AssignedToName = reader["assgnd_to_nm"] == DBNull.Value ? string.Empty : reader["assgnd_to_nm"].ToString(),
                                AssignedTime = reader["assigned_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assigned_dt"],
                                StageId = reader["tsk_itm_stg"] == DBNull.Value ? 0 : (int)reader["tsk_itm_stg"],
                                StageDescription = reader["itm_stg_nm"] == DBNull.Value ? string.Empty : reader["itm_stg_nm"].ToString(),
                                Stage = reader["tsk_itm_stg"] == DBNull.Value ? TaskItemStage.NotYetApproved : (TaskItemStage)reader["tsk_itm_stg"],

                                ProgressStatusId = reader["prgs_stts"] == DBNull.Value ? 0 : (int)reader["prgs_stts"],
                                ProgressStatus = reader["prgs_stts"] == DBNull.Value ? 0 : (WorkItemProgressStatus)reader["prgs_stts"],
                                ProgressStatusDescription = reader["prgs_stts_ds"] == DBNull.Value ? string.Empty : reader["prgs_stts_ds"].ToString(),

                                ApprovalStatusId = reader["apprv_stts"] == DBNull.Value ? 0 : (int)reader["apprv_stts"],
                                ApprovalStatus = reader["apprv_stts"] == DBNull.Value ? ApprovalStatus.Pending : (ApprovalStatus)reader["apprv_stts"],
                                ApprovalStatusDescription = reader["apprv_stts_ds"] == DBNull.Value ? string.Empty : reader["apprv_stts_ds"].ToString(),

                                ApprovedTime = reader["approved_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["approved_dt"],
                                ApprovedBy = reader["approved_by"] == DBNull.Value ? string.Empty : reader["approved_by"].ToString(),
                                ExpectedStartTime = reader["exp_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_start_dt"],
                                ActualStartTime = reader["act_start_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_start_dt"],
                                ExpectedDueTime = reader["exp_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["exp_due_dt"],
                                ActualDueTime = reader["act_due_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["act_due_dt"],

                                IsCancelled = reader["is_cancelled"] == DBNull.Value ? false : (bool)reader["is_cancelled"],
                                CancelledBy = reader["cancelled_by"] == DBNull.Value ? "" : reader["cancelled_by"].ToString(),
                                CancelledTime = reader["cancelled_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["cancelled_dt"],

                                IsClosed = reader["is_closed"] == DBNull.Value ? false : (bool)reader["is_closed"],
                                ClosedBy = reader["closed_by"] == DBNull.Value ? "" : reader["closed_by"].ToString(),
                                ClosedTime = reader["closed_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["closed_dt"],

                                UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unit_nm"] == DBNull.Value ? "" : reader["unit_nm"].ToString(),
                                DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["dept_nm"] == DBNull.Value ? "" : reader["dept_nm"].ToString(),
                                LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["loc_nm"] == DBNull.Value ? "" : reader["loc_nm"].ToString(),

                                CompletionConfirmed = reader["completion_is_confirmed"] == DBNull.Value ? false : (bool)reader["completion_is_confirmed"],
                                CompletionConfirmedBy = reader["completion_confirmed_by"] == DBNull.Value ? "" : reader["completion_confirmed_by"].ToString(),
                                CompletionConfirmedTime = reader["completion_confirmed_on"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["completion_confirmed_on"],
                                IsCarriedOver = reader["is_carried_over"] == DBNull.Value ? false : (bool)reader["is_carried_over"],

                                CreatedTime = reader["crt_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crt_dt"],
                                CreatedBy = reader["crt_by"] == DBNull.Value ? string.Empty : reader["crt_by"].ToString(),
                                LastModifiedTime = reader["mod_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mod_dt"],
                                LastModifiedBy = reader["mod_by"] == DBNull.Value ? string.Empty : reader["mod_by"].ToString(),

                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                AssignmentId = reader["assgnmt_id"] == DBNull.Value ? (long?)null : (long)reader["assgnmt_id"],

                                EvaluationDate = reader["eval_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["eval_dt"],
                                EvaluationDetailId = reader["eval_dtl_id"] == DBNull.Value ? 0 : (long)reader["eval_dtl_id"],
                                EvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"],
                                CompletionScore = reader["completion_score"] == DBNull.Value ? 0 : (long)reader["completion_score"],
                                QualityScore = reader["quality_score"] == DBNull.Value ? 0 : (long)reader["quality_score"],
                                EvaluatorComments = reader["eval_comments"] == DBNull.Value ? string.Empty : reader["eval_comments"].ToString(),
                                EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString(),
                                EvaluatorName = reader["eval_emp_nm"] == DBNull.Value ? string.Empty : reader["eval_emp_nm"].ToString()
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskItemEvaluations;
        }
        #endregion

        #region Task Evaluation Scores Read Action Methods
        public async Task<TaskEvaluationScores> GetTaskEvaluationHeaderScoresByTaskFolderIdAndEvaluatorIdAsync(long taskFolderId, string evaluatorId)
        {
            TaskEvaluationScores taskEvaluationScores = new TaskEvaluationScores();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.eval_hdr_id, h.eval_emp_id, d.wki_fdr_id, h.owner_emp_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE eval_hdr_id = d.eval_hdr_id ");
            sb.Append("AND completion_score = 100) AS no_items_completed ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("INNER JOIN public.wsp_eval_hdr h ON h.eval_hdr_id = d.eval_hdr_id ");
            sb.Append("WHERE d.wki_fdr_id = @wki_fdr_id AND h.eval_emp_id = @eval_emp_id ");
            sb.Append("GROUP BY d.eval_hdr_id, h.eval_emp_id, d.wki_fdr_id, h.owner_emp_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eval_emp_id = cmd.Parameters.Add("@eval_emp_id", NpgsqlDbType.Text);
                    var wki_fdr_id = cmd.Parameters.Add("@wki_fdr_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    eval_emp_id.Value = evaluatorId;
                    wki_fdr_id.Value = taskFolderId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScores.TaskEvaluationHeaderId = reader["eval_hdr_id"] == DBNull.Value ? 0 : (long)reader["eval_hdr_id"];
                            taskEvaluationScores.TaskFolderId = reader["wki_fdr_id"] == DBNull.Value ? 0 : (long)reader["wki_fdr_id"];
                            taskEvaluationScores.EvaluatorId = reader["eval_emp_id"] == DBNull.Value ? string.Empty : reader["eval_emp_id"].ToString();
                            taskEvaluationScores.TaskOwnerId = reader["owner_emp_id"] == DBNull.Value ? string.Empty : reader["owner_emp_id"].ToString();
                            taskEvaluationScores.TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"];
                            taskEvaluationScores.NoOfCompletedTasks = reader["no_items_completed"] == DBNull.Value ? 0 : (long)reader["no_items_completed"];
                            taskEvaluationScores.TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]);
                            taskEvaluationScores.TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]);
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScores;
        }
        public async Task<TaskEvaluationScores> GetTaskEvaluationScoresByTaskOwnerIdAsync(string taskOwnerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            TaskEvaluationScores taskEvaluationScore = new TaskEvaluationScores();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.tsk_owner_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("AVG(d.completion_score) :: NUMERIC(18,2) AS avg_completion_score, ");
            sb.Append("AVG(d.quality_score) :: NUMERIC(18,2) AS avg_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl ");
            sb.Append("WHERE tsk_owner_id = @owner_emp_id AND completion_score = 100 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_completed, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE tsk_owner_id = @owner_emp_id AND completion_score = 0 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_uncompleted, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("WHERE d.tsk_owner_id = @owner_emp_id ");
            sb.Append("AND (d.eval_dt >= @dt_frm) ");
            sb.Append("AND (d.eval_dt <= @dt_to) ");
            sb.Append("GROUP BY d.tsk_owner_id, tsk_owner_nm; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var owner_emp_id = cmd.Parameters.Add("@owner_emp_id", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    owner_emp_id.Value = taskOwnerId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScore.TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString();
                            taskEvaluationScore.TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString();
                            taskEvaluationScore.TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"];
                            taskEvaluationScore.NoOfCompletedTasks = reader["no_completed"] == DBNull.Value ? 0 : (long)reader["no_completed"];
                            taskEvaluationScore.NoOfUncompletedTasks = reader["no_uncompleted"] == DBNull.Value ? 0 : (long)reader["no_uncompleted"];
                            taskEvaluationScore.TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]);
                            taskEvaluationScore.TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]);
                            taskEvaluationScore.AverageCompletionScore = reader["avg_completion_score"] == DBNull.Value ? 0M : (decimal)reader["avg_completion_score"];
                            taskEvaluationScore.AverageQualityScore = reader["avg_quality_score"] == DBNull.Value ? 0M : (decimal)reader["avg_quality_score"];
                            taskEvaluationScore.PercentageCompletion = (taskEvaluationScore.NoOfCompletedTasks / taskEvaluationScore.TotalNumberOfTasks) * 100;
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScore;
        }
        public async Task<TaskEvaluationScores> GetTaskEvaluationScoresByTaskOwnerNameAsync(string taskOwnerName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            TaskEvaluationScores taskEvaluationScore = new TaskEvaluationScores();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.tsk_owner_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("AVG(d.completion_score) :: NUMERIC(18,2) AS avg_completion_score, ");
            sb.Append("AVG(d.quality_score) :: NUMERIC(18,2) AS avg_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 100 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_completed, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 0 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_uncompleted, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("WHERE tsk_owner_nm = @tsk_owner_nm ");
            sb.Append("AND (d.eval_dt >= @dt_frm) ");
            sb.Append("AND (d.eval_dt <= @dt_to) ");
            sb.Append("GROUP BY d.tsk_owner_id, tsk_owner_nm; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_owner_nm = cmd.Parameters.Add("@tsk_owner_nm", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    tsk_owner_nm.Value = taskOwnerName;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScore.TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString();
                            taskEvaluationScore.TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString();
                            taskEvaluationScore.TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"];
                            taskEvaluationScore.NoOfCompletedTasks = reader["no_completed"] == DBNull.Value ? 0 : (long)reader["no_completed"];
                            taskEvaluationScore.NoOfUncompletedTasks = reader["no_uncompleted"] == DBNull.Value ? 0 : (long)reader["no_uncompleted"];
                            taskEvaluationScore.TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]);
                            taskEvaluationScore.TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]);
                            taskEvaluationScore.AverageCompletionScore = reader["avg_completion_score"] == DBNull.Value ? 0M : (decimal)reader["avg_completion_score"];
                            taskEvaluationScore.AverageQualityScore = reader["avg_quality_score"] == DBNull.Value ? 0M : (decimal)reader["avg_quality_score"];
                            taskEvaluationScore.PercentageCompletion = (taskEvaluationScore.NoOfCompletedTasks / taskEvaluationScore.TotalNumberOfTasks) * 100;
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScore;
        }
        public async Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByUnitIdAsync(int unitId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationScores> taskEvaluationScores = new List<TaskEvaluationScores>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.unit_id, d.tsk_owner_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("AVG(d.completion_score) :: NUMERIC(18,2) AS avg_completion_score, ");
            sb.Append("AVG(d.quality_score) :: NUMERIC(18,2) AS avg_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 100 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_completed, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 0 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_uncompleted, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("WHERE (d.unit_id = @unit_id) ");
            sb.Append("AND (d.eval_dt >= @dt_frm) ");
            sb.Append("AND (d.eval_dt <= @dt_to) ");
            sb.Append("GROUP BY d.unit_id, d.tsk_owner_id, tsk_owner_nm; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    unit_id.Value = unitId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScores.Add(new TaskEvaluationScores
                            {
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"],
                                NoOfCompletedTasks = reader["no_completed"] == DBNull.Value ? 0 : (long)reader["no_completed"],
                                NoOfUncompletedTasks = reader["no_uncompleted"] == DBNull.Value ? 0 : (long)reader["no_uncompleted"],
                                TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]),
                                TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]),
                                AverageCompletionScore = reader["avg_completion_score"] == DBNull.Value ? 0M : (decimal)reader["avg_completion_score"],
                                AverageQualityScore = reader["avg_quality_score"] == DBNull.Value ? 0M : (decimal)reader["avg_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScores;
        }
        public async Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByUnitIdnLocationIdAsync(int locationId, int unitId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationScores> taskEvaluationScores = new List<TaskEvaluationScores>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.loc_id, d.unit_id, d.tsk_owner_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("AVG(d.completion_score) :: NUMERIC(18,2) AS avg_completion_score, ");
            sb.Append("AVG(d.quality_score) :: NUMERIC(18,2) AS avg_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 100 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_completed, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 0 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_uncompleted, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("WHERE (d.unit_id = @unit_id) AND (d.loc_id = @loc_id) ");
            sb.Append("AND (d.eval_dt >= @dt_frm) AND (d.eval_dt <= @dt_to) ");
            sb.Append("GROUP BY d.loc_id, d.unit_id, d.tsk_owner_id, tsk_owner_nm; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    unit_id.Value = unitId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScores.Add(new TaskEvaluationScores
                            {
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"],
                                NoOfCompletedTasks = reader["no_completed"] == DBNull.Value ? 0 : (long)reader["no_completed"],
                                NoOfUncompletedTasks = reader["no_uncompleted"] == DBNull.Value ? 0 : (long)reader["no_uncompleted"],
                                TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]),
                                TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]),
                                AverageCompletionScore = reader["avg_completion_score"] == DBNull.Value ? 0M : (decimal)reader["avg_completion_score"],
                                AverageQualityScore = reader["avg_quality_score"] == DBNull.Value ? 0M : (decimal)reader["avg_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScores;
        }
        public async Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByDepartmentIdnLocationIdAsync(int locationId, int deptId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationScores> taskEvaluationScores = new List<TaskEvaluationScores>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.loc_id, d.dept_id, d.tsk_owner_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("AVG(d.completion_score) :: NUMERIC(18,2) AS avg_completion_score, ");
            sb.Append("AVG(d.quality_score) :: NUMERIC(18,2) AS avg_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 100 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_completed, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 0 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_uncompleted, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("WHERE (d.dept_id = @dept_id) AND (d.loc_id = @loc_id) ");
            sb.Append("AND (d.eval_dt >= @dt_frm) AND (d.eval_dt <= @dt_to) ");
            sb.Append("GROUP BY d.loc_id, d.dept_id, d.tsk_owner_id, tsk_owner_nm ");
            sb.Append("ORDER BY d.loc_id, d.dept_id, d.tsk_owner_id, tsk_owner_nm;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    dept_id.Value = deptId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScores.Add(new TaskEvaluationScores
                            {
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"],
                                NoOfCompletedTasks = reader["no_completed"] == DBNull.Value ? 0 : (long)reader["no_completed"],
                                NoOfUncompletedTasks = reader["no_uncompleted"] == DBNull.Value ? 0 : (long)reader["no_uncompleted"],
                                TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]),
                                TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]),
                                AverageCompletionScore = reader["avg_completion_score"] == DBNull.Value ? 0M : (decimal)reader["avg_completion_score"],
                                AverageQualityScore = reader["avg_quality_score"] == DBNull.Value ? 0M : (decimal)reader["avg_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScores;
        }
        public async Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByDepartmentIdAsync(int deptId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationScores> taskEvaluationScores = new List<TaskEvaluationScores>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.dept_id, d.unit_id, d.tsk_owner_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("AVG(d.completion_score) :: NUMERIC(18,2) AS avg_completion_score, ");
            sb.Append("AVG(d.quality_score) :: NUMERIC(18,2) AS avg_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 100 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_completed, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 0 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_uncompleted, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("WHERE (d.dept_id = @dept_id) ");
            sb.Append("AND (d.eval_dt >= @dt_frm) ");
            sb.Append("AND (d.eval_dt <= @dt_to) ");
            sb.Append("GROUP BY d.dept_id, d.unit_id, d.tsk_owner_id, tsk_owner_nm; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    dept_id.Value = deptId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScores.Add(new TaskEvaluationScores
                            {
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"],
                                NoOfCompletedTasks = reader["no_completed"] == DBNull.Value ? 0 : (long)reader["no_completed"],
                                NoOfUncompletedTasks = reader["no_uncompleted"] == DBNull.Value ? 0 : (long)reader["no_uncompleted"],
                                TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]),
                                TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]),
                                AverageCompletionScore = reader["avg_completion_score"] == DBNull.Value ? 0M : (decimal)reader["avg_completion_score"],
                                AverageQualityScore = reader["avg_quality_score"] == DBNull.Value ? 0M : (decimal)reader["avg_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScores;
        }
        public async Task<List<TaskEvaluationScores>> GetTaskEvaluationScoresByLocationIdAsync(int locationId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<TaskEvaluationScores> taskEvaluationScores = new List<TaskEvaluationScores>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-3);
            DateTime to_date = toDate ?? DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT d.loc_id, d.tsk_owner_id, ");
            sb.Append("SUM(d.completion_score) AS sum_completion_score, ");
            sb.Append("SUM(d.quality_score) AS sum_quality_score, ");
            sb.Append("AVG(d.completion_score) :: NUMERIC(18,2) AS avg_completion_score, ");
            sb.Append("AVG(d.quality_score) :: NUMERIC(18,2) AS avg_quality_score, ");
            sb.Append("COUNT(d.tsk_itm_id) AS total_no_items, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 100 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_completed, ");
            sb.Append("(SELECT COUNT(tsk_itm_id) FROM public.wsp_eval_dtl  ");
            sb.Append("WHERE tsk_owner_id = d.tsk_owner_id AND completion_score = 0 ");
            sb.Append("AND eval_dt >= @dt_frm AND eval_dt <= @dt_to) as no_uncompleted, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = d.tsk_owner_id) as tsk_owner_nm ");
            sb.Append("FROM public.wsp_eval_dtl d ");
            sb.Append("WHERE (d.loc_id = @loc_id) ");
            sb.Append("AND (d.eval_dt >= @dt_frm) ");
            sb.Append("AND (d.eval_dt <= @dt_to) ");
            sb.Append("GROUP BY d.loc_id, d.tsk_owner_id, tsk_owner_nm ");
            sb.Append("ORDER BY d.loc_id, d.tsk_owner_id, tsk_owner_nm; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    loc_id.Value = locationId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskEvaluationScores.Add(new TaskEvaluationScores
                            {
                                TaskOwnerId = reader["tsk_owner_id"] == DBNull.Value ? string.Empty : reader["tsk_owner_id"].ToString(),
                                TaskOwnerName = reader["tsk_owner_nm"] == DBNull.Value ? string.Empty : reader["tsk_owner_nm"].ToString(),
                                TotalNumberOfTasks = reader["total_no_items"] == DBNull.Value ? 0 : (long)reader["total_no_items"],
                                NoOfCompletedTasks = reader["no_completed"] == DBNull.Value ? 0 : (long)reader["no_completed"],
                                NoOfUncompletedTasks = reader["no_uncompleted"] == DBNull.Value ? 0 : (long)reader["no_uncompleted"],
                                TotalCompletionScore = reader["sum_completion_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_completion_score"]),
                                TotalQualityScore = reader["sum_quality_score"] == DBNull.Value ? 0 : Convert.ToInt64(reader["sum_quality_score"]),
                                AverageCompletionScore = reader["avg_completion_score"] == DBNull.Value ? 0M : (decimal)reader["avg_completion_score"],
                                AverageQualityScore = reader["avg_quality_score"] == DBNull.Value ? 0M : (decimal)reader["avg_quality_score"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return taskEvaluationScores;
        }

        #endregion

        #region Work Item Return Reason Read Action Methods
        public async Task<List<WorkItemReturnReason>> GetWorkItemReturnReasonsAsync()
        {
            List<WorkItemReturnReason> _returnReasons = new List<WorkItemReturnReason>();
            string query = "SELECT rsns_id, rsns_ds FROM public.wsp_rtn_rsns;";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            _returnReasons.Add(
                                new WorkItemReturnReason
                                {
                                    Id = reader["rsns_id"] == DBNull.Value ? 0 : (int)reader["rsns_id"],
                                    Description = reader["rsns_ds"] == DBNull.Value ? string.Empty : reader["rsns_ds"].ToString()
                                });
                        }
                }
                await conn.CloseAsync();
            }
            return _returnReasons;
        }

        #endregion

        #endregion
    }
}
