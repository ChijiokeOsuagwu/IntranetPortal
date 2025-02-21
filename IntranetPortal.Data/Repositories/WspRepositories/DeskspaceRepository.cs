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
            sb.Append("UPDATE public.wsp_wki_fdr SET is_archived = @is_archived  ");
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
                    cmd.Prepare();
                    wki_fdr_id.Value = workItemFolderId;
                    is_archived.Value = isArchived;
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
            sb.Append("DELETE FROM public.wsp_wki_hst WHERE tsk_itm_id IN (SELECT tsk_itm_id ");
            sb.Append("FROM public.wsp_tsk_itms WHERE (wki_fdr_id = @wki_fdr_id)); ");
            sb.Append("DELETE FROM public.wsp_wki_nts WHERE tsk_id IN (SELECT tsk_itm_id ");
            sb.Append("FROM public.wsp_tsk_itms WHERE (wki_fdr_id = @wki_fdr_id)); ");

            sb.Append("DELETE FROM public.wsp_tsk_itms WHERE (wki_fdr_id = @wki_fdr_id);");
            sb.Append("DELETE FROM public.wsp_wki_nts WHERE (wki_fdr_id = @wki_fdr_id);");
            sb.Append("DELETE FROM public.wsp_wki_hst WHERE (wki_fdr_id = @wki_fdr_id);");
            sb.Append("DELETE FROM public.wsp_wki_fdr WHERE (wki_fdr_id = @wki_fdr_id);");
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
                    tsk_hst_by.Value = log.ActivityBy;
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
        public async Task<bool> DeleteTaskItemAsync(long taskItemId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wsp_wki_hst WHERE tsk_itm_id = @tsk_itm_id; ");
            sb.Append("DELETE FROM public.wsp_wki_nts WHERE tsk_id = @tsk_itm_id; ");
            sb.Append("DELETE FROM public.wsp_tsk_itms WHERE (tsk_itm_id = @tsk_itm_id);");
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
                    var approved_dt = cmd.Parameters.Add("@approved_dt", NpgsqlDbType.TimestampTz);
                    var approved_by = cmd.Parameters.Add("@approved_by", NpgsqlDbType.Text);
                    var mod_by = cmd.Parameters.Add("@mod_by", NpgsqlDbType.Text);
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
                    cmd.Prepare();
                    tsk_itm_id.Value = taskItemId;
                    apprv_stts.Value = (int)newApprovalStatus;
                    approved_dt.Value = DateTime.UtcNow;
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
                    var mod_dt = cmd.Parameters.Add("@mod_dt", NpgsqlDbType.Text);
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
        #endregion


        #endregion

    }
}
