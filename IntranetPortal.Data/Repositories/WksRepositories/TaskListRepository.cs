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
    public class TaskListRepository : ITaskListRepository
    {
        public IConfiguration _config { get; }
        public TaskListRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region TaskList Write Actions
        public async Task<bool> AddAsync(TaskList taskList)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.wkm_tsk_lst(tsk_lst_nm, tsk_lst_ds, ");
            sb.Append("ctd, is_archived, is_dlt, dlt_on, ");
            sb.Append("dlt_by, ctb, lst_emp_id, is_lckd) ");
            sb.Append("VALUES (@tsk_lst_nm, @tsk_lst_ds, @ctd, ");
            sb.Append("@is_archived, @is_dlt, @dlt_on, ");
            sb.Append("@dlt_by, @ctb, @lst_emp_id, @is_lckd);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_nm = cmd.Parameters.Add("@tsk_lst_nm", NpgsqlDbType.Text);
                    var tsk_lst_ds = cmd.Parameters.Add("@tsk_lst_ds", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    var ctd = cmd.Parameters.Add("@ctd", NpgsqlDbType.TimestampTz);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var is_dlt = cmd.Parameters.Add("@is_dlt", NpgsqlDbType.Boolean);
                    var dlt_on = cmd.Parameters.Add("@dlt_on", NpgsqlDbType.TimestampTz);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    var is_lckd = cmd.Parameters.Add("@is_lckd", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    tsk_lst_nm.Value = taskList.Name;
                    tsk_lst_ds.Value = taskList.Description ?? (object)DBNull.Value; 
                    is_archived.Value = taskList.IsArchived;
                    lst_emp_id.Value = taskList.OwnerId;
                    ctd.Value = taskList.CreatedTime ?? DateTime.UtcNow;
                    ctb.Value = taskList.CreatedBy ?? (object)DBNull.Value;
                    is_dlt.Value = false;
                    dlt_on.Value = (object)DBNull.Value;
                    dlt_by.Value = (object)DBNull.Value;
                    is_lckd.Value = false;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(TaskList taskList)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_lst SET tsk_lst_nm=@tsk_lst_nm, ");
            sb.Append("tsk_lst_ds=@tsk_lst_ds, is_archived=@is_archived ");
            sb.Append("WHERE (tsk_lst_id=@tsk_lst_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var tsk_lst_nm = cmd.Parameters.Add("@tsk_lst_nm", NpgsqlDbType.Text);
                    var tsk_lst_ds = cmd.Parameters.Add("@tsk_lst_ds", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    tsk_lst_id.Value = taskList.Id;
                    tsk_lst_nm.Value = taskList.Name;
                    tsk_lst_ds.Value = taskList.Description ?? (object)DBNull.Value;
                    is_archived.Value = taskList.IsArchived;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> UpdateArchiveAsync(int taskListId, bool isArchived)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_lst SET is_archived = @is_archived  ");
            sb.Append("WHERE (tsk_lst_id = @tsk_lst_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                cmd.Prepare();
                tsk_lst_id.Value = taskListId;
                is_archived.Value = isArchived;

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateLockAsync(int taskListId, bool isLocked)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_lst SET is_lckd = @is_lckd ");
            sb.Append("WHERE (tsk_lst_id = @tsk_lst_id); ");
            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                var is_lckd = cmd.Parameters.Add("@is_lckd", NpgsqlDbType.Boolean);
                cmd.Prepare();
                tsk_lst_id.Value = taskListId;
                is_lckd.Value = isLocked;

                rows = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateToDeletedAsync(int taskListId, string deletedBy)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.wkm_tsk_lst SET is_dlt=true, dlt_by=@dlt_by, ");
            sb.Append("dlt_on=@dlt_on WHERE (tsk_lst_id = @tsk_lst_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var dlt_by = cmd.Parameters.Add("@dlt_by", NpgsqlDbType.Text);
                    var dlt_on = cmd.Parameters.Add("@dlt_on", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    tsk_lst_id.Value = taskListId;
                    dlt_by.Value = deletedBy;
                    dlt_on.Value = DateTime.UtcNow;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int taskListId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wkm_tsk_lst ");
            sb.Append("WHERE (tsk_lst_id = @tsk_lst_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    tsk_lst_id.Value = taskListId;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #region TaskList Read Actions
        public async Task<TaskList> GetByIdAsync(int taskListId)
        {
            TaskList taskList = new TaskList();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (l.tsk_lst_id = @tsk_lst_id) ");
            sb.Append("AND l.is_dlt = false;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskList.Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"];
                            taskList.Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString();
                            taskList.Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString();
                            taskList.OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString();
                            taskList.OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString();
                            taskList.IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"];
                            taskList.CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"];
                            taskList.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            taskList.IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"];
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return taskList;
        }

        public async Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId)
        {
            IList<TaskList> taskLists = new List<TaskList>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (LOWER(l.lst_emp_id) = LOWER(@lst_emp_id) ");
            sb.Append("AND l.is_dlt = false) ");
            sb.Append("ORDER BY l.tsk_lst_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lst_emp_id.Value = ownerId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskLists.Add(new TaskList
                            {
                                Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString(),
                                OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString(),
                                OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],

                        });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskLists;
        }

        public async Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, bool isArchived)
        {
            IList<TaskList> taskLists = new List<TaskList>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (LOWER(l.lst_emp_id) = LOWER(@lst_emp_id) ");
            sb.Append("AND (l.is_archived = @is_archived) AND (l.is_dlt = false)) ");
            sb.Append("ORDER BY l.tsk_lst_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    await cmd.PrepareAsync();
                    lst_emp_id.Value = ownerId;
                    is_archived.Value = isArchived;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskLists.Add(new TaskList
                            {
                                Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString(),
                                OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString(),
                                OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],

                        });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskLists;
        }

        public async Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, int createdYear)
        {
            IList<TaskList> taskLists = new List<TaskList>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (LOWER(l.lst_emp_id) = LOWER(@lst_emp_id) ");
            sb.Append("AND (l.is_dlt = false) ");
            sb.Append("AND (date_part('year', l.ctd) = @ctd_yr)) ");
            sb.Append("ORDER BY l.tsk_lst_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lst_emp_id.Value = ownerId;
                    ctd_yr.Value = createdYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskLists.Add(new TaskList
                            {
                                Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString(),
                                OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString(),
                                OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],

                        });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskLists;
        }

        public async Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, bool isArchived, int createdYear)
        {
            IList<TaskList> taskLists = new List<TaskList>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, l.is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (LOWER(l.lst_emp_id) = LOWER(@lst_emp_id) ");
            sb.Append("AND (l.is_archived = @is_archived) AND (l.is_dlt = false) ");
            sb.Append("AND (date_part('year', l.ctd) = @ctd_yr)) ");
            sb.Append("ORDER BY l.tsk_lst_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lst_emp_id.Value = ownerId;
                    is_archived.Value = isArchived;
                    ctd_yr.Value = createdYear;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskLists.Add(new TaskList
                            {
                                Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString(),
                                OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString(),
                                OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskLists;
        }

        public async Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, int createdYear, int createdMonth)
        {
            IList<TaskList> taskLists = new List<TaskList>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, l.is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (LOWER(l.lst_emp_id) = LOWER(@lst_emp_id) ");
            sb.Append("AND (l.is_dlt = false) ");
            sb.Append("AND (date_part('year', l.ctd) = @ctd_yr) ");
            sb.Append("AND (date_part('month', l.ctd) = @ctd_mn)) ");
            sb.Append("ORDER BY l.tsk_lst_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                    var ctd_mn = cmd.Parameters.Add("@ctd_mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lst_emp_id.Value = ownerId;
                    ctd_yr.Value = createdYear;
                    ctd_mn.Value = createdMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskLists.Add(new TaskList
                            {
                                Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString(),
                                OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString(),
                                OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskLists;
        }

        public async Task<IList<TaskList>> GetByOwnerIdAsync(string ownerId, bool isArchived, int createdYear, int createdMonth)
        {
            IList<TaskList> taskLists = new List<TaskList>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, l.is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (LOWER(l.lst_emp_id) = LOWER(@lst_emp_id) ");
            sb.Append("AND (l.is_archived = @is_archived) AND (l.is_dlt = false) ");
            sb.Append("AND (date_part('year', l.ctd) = @ctd_yr) ");
            sb.Append("AND (date_part('month', l.ctd) = @ctd_mn)) ");
            sb.Append("ORDER BY l.tsk_lst_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    var is_archived = cmd.Parameters.Add("@is_archived", NpgsqlDbType.Boolean);
                    var ctd_yr = cmd.Parameters.Add("@ctd_yr", NpgsqlDbType.Integer);
                    var ctd_mn = cmd.Parameters.Add("@ctd_mn", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    lst_emp_id.Value = ownerId;
                    is_archived.Value = isArchived;
                    ctd_yr.Value = createdYear;
                    ctd_mn.Value = createdMonth;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskLists.Add(new TaskList
                            {
                                Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString(),
                                OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString(),
                                OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskLists;
        }

        public async Task<IList<TaskList>> GetByOwnerIdAndNameAsync(string ownerId, string taskListName)
        {
            IList<TaskList> taskLists = new List<TaskList>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT l.tsk_lst_id, l.tsk_lst_nm, l.tsk_lst_ds, l.ctd, ");
            sb.Append("l.is_archived, l.ctb, l.lst_emp_id, p.fullname, l.is_lckd ");
            sb.Append("FROM public.wkm_tsk_lst l ");
            sb.Append("INNER JOIN public.gst_prsns p ON p.id = l.lst_emp_id ");
            sb.Append("WHERE (LOWER(l.lst_emp_id) = LOWER(@lst_emp_id) ");
            sb.Append("AND LOWER(l.tsk_lst_nm) = LOWER(@tsk_lst_nm) ");
            sb.Append("AND l.is_dlt = false) ");
            sb.Append("ORDER BY l.tsk_lst_id DESC;");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var lst_emp_id = cmd.Parameters.Add("@lst_emp_id", NpgsqlDbType.Text);
                    var tsk_lst_nm = cmd.Parameters.Add("@tsk_lst_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    lst_emp_id.Value = ownerId;
                    tsk_lst_nm.Value = taskListName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskLists.Add(new TaskList
                            {
                                Id = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                Name = reader["tsk_lst_nm"] == DBNull.Value ? string.Empty : reader["tsk_lst_nm"].ToString(),
                                Description = reader["tsk_lst_ds"] == DBNull.Value ? string.Empty : reader["tsk_lst_ds"].ToString(),
                                OwnerId = reader["lst_emp_id"] == DBNull.Value ? string.Empty : reader["lst_emp_id"].ToString(),
                                OwnerName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                IsArchived = reader["is_archived"] == DBNull.Value ? false : (bool)reader["is_archived"],
                                IsLocked = reader["is_lckd"] == DBNull.Value ? false : (bool)reader["is_lckd"],
                                CreatedTime = reader["ctd"] == DBNull.Value ? DateTime.Now : (DateTime)reader["ctd"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskLists;
        }
        #endregion

        #region TaskList Note Action Methods
        public async Task<IList<TaskListNote>> GetNotesByTaskListIdAsync(int taskListId)
        {
            IList<TaskListNote> taskListNotes = new List<TaskListNote>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ntt_nts_id, ntt_nts_tm, ntt_nts_ds, ");
            sb.Append("ntt_nts_by, tsk_id, tsk_lst_id, is_cnc, dt_cnc ");
            sb.Append("FROM public.gst_ntt_nts ");
            sb.Append("WHERE (tsk_lst_id = @tsk_lst_id ");
            sb.Append("AND tsk_lst_id IS NOT NULL) ");
            sb.Append("ORDER BY ntt_nts_id DESC; ");

            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    tsk_lst_id.Value = taskListId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            taskListNotes.Add(new TaskListNote
                            {
                                TaskListId = reader["tsk_lst_id"] == DBNull.Value ? 0 : (int)reader["tsk_lst_id"],
                                NoteId = reader["ntt_nts_id"] == DBNull.Value ? 0 : (long)reader["ntt_nts_id"],
                                NoteTime = reader["ntt_nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ntt_nts_tm"],
                                NoteDescription = reader["ntt_nts_ds"] == DBNull.Value ? string.Empty : reader["ntt_nts_ds"].ToString(),
                                NoteWrittenBy = reader["ntt_nts_by"] == DBNull.Value ? string.Empty : reader["ntt_nts_by"].ToString(),
                                IsCancelled = reader["is_cnc"] == DBNull.Value ? false : (bool)reader["is_cnc"],
                                CancelledOn = reader["dt_cnc"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dt_cnc"],
                            });
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return taskListNotes;
        }
        public async Task<bool> AddNoteAsync(TaskListNote taskListNote)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_ntt_nts(ntt_nts_tm, ntt_nts_ds, ");
            sb.Append("ntt_nts_by, tsk_lst_id, is_cnc, dt_cnc) ");
            sb.Append("VALUES (@ntt_nts_tm, @ntt_nts_ds, @ntt_nts_by, ");
            sb.Append("@tsk_lst_id, @is_cnc, @dt_cnc); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var tsk_lst_id = cmd.Parameters.Add("@tsk_lst_id", NpgsqlDbType.Integer);
                    var ntt_nts_tm = cmd.Parameters.Add("@ntt_nts_tm", NpgsqlDbType.TimestampTz);
                    var ntt_nts_ds = cmd.Parameters.Add("@ntt_nts_ds", NpgsqlDbType.Text);
                    var ntt_nts_by = cmd.Parameters.Add("@ntt_nts_by", NpgsqlDbType.Text);
                    var is_cnc = cmd.Parameters.Add("@is_cnc", NpgsqlDbType.Boolean);
                    var dt_cnc = cmd.Parameters.Add("@dt_cnc", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    tsk_lst_id.Value = taskListNote.TaskListId;
                    ntt_nts_tm.Value = taskListNote.NoteTime;
                    ntt_nts_ds.Value = taskListNote.NoteDescription;
                    ntt_nts_by.Value = taskListNote.NoteWrittenBy;
                    is_cnc.Value = taskListNote.IsCancelled;
                    dt_cnc.Value = taskListNote.CancelledOn ?? (object)DBNull.Value;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateToIsCancelledAsync(long taskListNoteId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_ntt_nts SET is_cnc=true, dt_cnc=@dt_cnc ");
            sb.Append("WHERE (ntt_nts_id = @ntt_nts_id); ");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var ntt_nts_id = cmd.Parameters.Add("@ntt_nts_id", NpgsqlDbType.Bigint);
                    var dt_cnc = cmd.Parameters.Add("@dt_cnc", NpgsqlDbType.TimestampTz);
                    cmd.Prepare();
                    ntt_nts_id.Value = taskListNoteId;
                    dt_cnc.Value = DateTime.UtcNow;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion
    }
}
