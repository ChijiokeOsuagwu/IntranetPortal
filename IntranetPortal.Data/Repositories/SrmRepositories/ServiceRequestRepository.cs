using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Base.Repositories.SrmRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.SrmRepositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        public IConfiguration _config { get; }

        public ServiceRequestRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Service Incidents Data Access Methods
        public async Task<long> AddServiceIncidentAsync(ServiceIncident incident)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_inf(inc_desc, inc_imp, inc_svrt, ");
            sb.Append("inc_dt, inc_emp_id, inc_rpt_by, inc_rpt_dt, inc_sts, ");
            sb.Append("inc_isfp, inc_sys_id, inc_loc_id, inc_unit_id) ");
            sb.Append("VALUES (@inc_desc, @inc_imp, @inc_svrt, @inc_dt, @inc_emp_id, ");
            sb.Append("@inc_rpt_by, @inc_rpt_dt, @inc_sts, @inc_isfp, @inc_sys_id, ");
            sb.Append("@inc_loc_id, @inc_unit_id) RETURNING inc_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_desc = cmd.Parameters.Add("@inc_desc", NpgsqlDbType.Text);
                    var inc_imp = cmd.Parameters.Add("@inc_imp", NpgsqlDbType.Text);
                    var inc_svrt = cmd.Parameters.Add("@inc_svrt", NpgsqlDbType.Integer);
                    var inc_dt = cmd.Parameters.Add("@inc_dt", NpgsqlDbType.Timestamp);
                    var inc_emp_id = cmd.Parameters.Add("@inc_emp_id", NpgsqlDbType.Text);
                    var inc_rpt_by = cmd.Parameters.Add("@inc_rpt_by", NpgsqlDbType.Text);
                    var inc_rpt_dt = cmd.Parameters.Add("@inc_rpt_dt", NpgsqlDbType.Timestamp);
                    var inc_sts = cmd.Parameters.Add("@inc_sts", NpgsqlDbType.Text);
                    var inc_isfn = cmd.Parameters.Add("@inc_isfp", NpgsqlDbType.Boolean);
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    var inc_loc_id = cmd.Parameters.Add("@inc_loc_id", NpgsqlDbType.Integer);
                    var inc_unit_id = cmd.Parameters.Add("@inc_unit_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_desc.Value = incident.Description;
                    inc_imp.Value = incident.Impact;
                    inc_svrt.Value = incident.Severity;
                    inc_dt.Value = incident.IncidentTime;
                    inc_emp_id.Value = incident.IncidentEmployeeId;
                    inc_rpt_by.Value = incident.ReportedByEmployeeName;
                    inc_rpt_dt.Value = incident.ReportedTime;
                    inc_sts.Value = incident.IncidentStatus;
                    inc_isfn.Value = incident.IsFalseNegative;
                    inc_desc.Value = incident.Description;
                    inc_desc.Value = incident.Description;
                    inc_desc.Value = incident.Description;
                    inc_desc.Value = incident.Description;


                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }


        #endregion

        #region Settings Data Access Methods
        #region Service Systems Action Methods
        public async Task<int> AddServiceSystemAsync(ServiceSystem system)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_sys(inc_sys_nm) ");
            sb.Append("VALUES (@inc_sys_nm) RETURNING inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_nm = cmd.Parameters.Add("@inc_sys_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    inc_sys_nm.Value = system.Name;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateServiceSystemAsync(ServiceSystem system)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_sys SET inc_sys_nm=@inc_sys_nm ");
            sb.Append("WHERE inc_sys_id=@inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    var inc_sys_nm = cmd.Parameters.Add("@inc_sys_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    inc_sys_id.Value = system.Id;
                    inc_sys_nm.Value = system.Name;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceSystemAsync(int systemId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.srm_inc_sys ");
            sb.Append("WHERE inc_sys_id=@inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_sys_id.Value = systemId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<List<ServiceSystem>> GetServiceSystemByIdAsync(int systemId)
        {
            List<ServiceSystem> serviceSystemList = new List<ServiceSystem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_sys_id, inc_sys_nm FROM public.srm_inc_sys ");
            sb.Append("WHERE inc_sys_id = @inc_sys_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    inc_sys_id.Value = systemId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceSystemList.Add(new ServiceSystem
                            {
                                Id = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                Name = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceSystemList;
        }
        public async Task<List<ServiceSystem>> GetServiceSystemsByNameAsync(string name)
        {
            List<ServiceSystem> serviceSystemList = new List<ServiceSystem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_sys_id, inc_sys_nm FROM public.srm_inc_sys ");
            sb.Append("WHERE LOWER(inc_sys_nm) = LOWER(@inc_sys_nm) ");
            sb.Append("ORDER BY inc_sys_nm DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_sys_nm = cmd.Parameters.Add("@inc_sys_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    inc_sys_nm.Value = name;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceSystemList.Add(new ServiceSystem
                            {
                                Id = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                Name = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceSystemList;
        }
        public async Task<List<ServiceSystem>> GetServiceSystemsAsync()
        {
            List<ServiceSystem> serviceSystemList = new List<ServiceSystem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_sys_id, inc_sys_nm FROM public.srm_inc_sys ");
            sb.Append("ORDER BY inc_sys_nm;");
            string query = sb.ToString();
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
                            serviceSystemList.Add(new ServiceSystem
                            {
                                Id = reader["inc_sys_id"] == DBNull.Value ? 0 : (int)reader["inc_sys_id"],
                                Name = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceSystemList;
        }

        #endregion

        #region Service Centers Action Methods
        public async Task<int> AddServiceCenterAsync(ServiceCenter center)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_srv_cnt(srv_cnt_nm) ");
            sb.Append("VALUES (@srv_cnt_nm) RETURNING srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_nm = cmd.Parameters.Add("@srv_cnt_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    srv_cnt_nm.Value = center.Name;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateServiceCenterAsync(ServiceCenter center)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_srv_cnt SET srv_cnt_nm=@srv_cnt_nm ");
            sb.Append("WHERE srv_cnt_id=@srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_id = cmd.Parameters.Add("@srv_cnt_id", NpgsqlDbType.Integer);
                    var srv_cnt_nm = cmd.Parameters.Add("@srv_cnt_nm", NpgsqlDbType.Text);
                    cmd.Prepare();
                    srv_cnt_id.Value = center.Id;
                    srv_cnt_nm.Value = center.Name;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceCenterAsync(int serviceCenterId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.srm_srv_cnt ");
            sb.Append("WHERE srv_cnt_id=@srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_id = cmd.Parameters.Add("@srv_cnt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    srv_cnt_id.Value = serviceCenterId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<List<ServiceCenter>> GetServiceCenterByIdAsync(int serviceCenterId)
        {
            List<ServiceCenter> serviceCenterList = new List<ServiceCenter>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT srv_cnt_id, srv_cnt_nm FROM public.srm_srv_cnt ");
            sb.Append("WHERE srv_cnt_id = @srv_cnt_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_id = cmd.Parameters.Add("@srv_cnt_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    srv_cnt_id.Value = serviceCenterId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceCenterList.Add(new ServiceCenter
                            {
                                Id = reader["srv_cnt_id"] == DBNull.Value ? 0 : (int)reader["srv_cnt_id"],
                                Name = reader["srv_cnt_nm"] == DBNull.Value ? "" : reader["srv_cnt_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceCenterList;
        }
        public async Task<List<ServiceCenter>> GetServiceCentersByNameAsync(string serviceCenterName)
        {
            List<ServiceCenter> serviceCenterList = new List<ServiceCenter>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT srv_cnt_id, srv_cnt_nm FROM public.srm_srv_cnt ");
            sb.Append("WHERE LOWER(srv_cnt_nm) = LOWER(@srv_cnt_nm) ");
            sb.Append("ORDER BY srv_cnt_nm DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var srv_cnt_nm = cmd.Parameters.Add("@srv_cnt_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    srv_cnt_nm.Value = serviceCenterName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceCenterList.Add(new ServiceCenter
                            {
                                Id = reader["srv_cnt_id"] == DBNull.Value ? 0 : (int)reader["srv_cnt_id"],
                                Name = reader["srv_cnt_nm"] == DBNull.Value ? "" : reader["srv_cnt_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceCenterList;
        }
        public async Task<List<ServiceCenter>> GetServiceCentersAsync()
        {
            List<ServiceCenter> serviceCenterList = new List<ServiceCenter>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT srv_cnt_id, srv_cnt_nm FROM public.srm_srv_cnt ");
            sb.Append("ORDER BY srv_cnt_nm DESC;");
            string query = sb.ToString();
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
                            serviceCenterList.Add(new ServiceCenter
                            {
                                Id = reader["srv_cnt_id"] == DBNull.Value ? 0 : (int)reader["srv_cnt_id"],
                                Name = reader["srv_cnt_nm"] == DBNull.Value ? "" : reader["srv_cnt_nm"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceCenterList;
        }

        #endregion

        #region Service Types Action Methods
        public async Task<long> AddServiceTypeAsync(ServiceType serviceType)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_typ(inc_typ_ds, inc_sys_id) ");
            sb.Append("VALUES (@inc_typ_ds, @inc_sys_id) RETURNING inc_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_ds = cmd.Parameters.Add("@inc_typ_ds", NpgsqlDbType.Text);
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_typ_ds.Value = serviceType.Name;
                    inc_sys_id.Value = serviceType.ServiceSystemId;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateServiceTypeAsync(ServiceType serviceType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_typ SET inc_typ_ds=@inc_typ_ds, ");
            sb.Append("inc_sys_id=@inc_sys_id WHERE (inc_typ_id=@inc_typ_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_ds = cmd.Parameters.Add("@inc_typ_ds", NpgsqlDbType.Text);
                    var inc_sys_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    var inc_typ_id = cmd.Parameters.Add("@inc_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_typ_ds.Value = serviceType.Name;
                    inc_sys_id.Value = serviceType.ServiceSystemId ?? (object)DBNull.Value;
                    inc_typ_id.Value = serviceType.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceTypeAsync(int serviceTypeId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.srm_inc_typ ");
            sb.Append("WHERE inc_typ_id=@inc_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_id = cmd.Parameters.Add("@inc_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    inc_typ_id.Value = serviceTypeId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<ServiceType> GetServiceTypeByIdAsync(int serviceTypeId)
        {
            List<ServiceType> serviceTypeList = new List<ServiceType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.inc_typ_id, t.inc_typ_ds, t.inc_sys_id, ");
            sb.Append("s.inc_sys_nm FROM public.srm_inc_typ t ");
            sb.Append("INNER JOIN public.srm_inc_sys s ON s.inc_sys_id = t.inc_sys_id ");
            sb.Append("WHERE t.inc_typ_id = @inc_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_id = cmd.Parameters.Add("@inc_sys_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    inc_typ_id.Value = serviceTypeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceTypeList.Add(new ServiceType
                            {
                                Id = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],
                                Name = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? (int?)null : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceTypeList[0];
        }
        public async Task<ServiceType> GetServiceTypesByNameAsync(string serviceTypeName)
        {
            List<ServiceType> serviceTypeList = new List<ServiceType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.inc_typ_id, t.inc_typ_ds, t.inc_sys_id, ");
            sb.Append("s.inc_sys_nm FROM public.srm_inc_typ t ");
            sb.Append("INNER JOIN public.srm_inc_sys s ON s.inc_sys_id = t.inc_sys_id ");
            sb.Append("WHERE LOWER(t.inc_typ_ds) = LOWER(@inc_typ_ds) ");
            sb.Append("ORDER BY t.inc_typ_ds DESC;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_typ_ds = cmd.Parameters.Add("@inc_typ_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    inc_typ_ds.Value = serviceTypeName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceTypeList.Add(new ServiceType
                            {
                                Id = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],
                                Name = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? (int?)null : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceTypeList[0];
        }
        public async Task<List<ServiceType>> GetServiceTypesAsync()
        {
            List<ServiceType> serviceTypeList = new List<ServiceType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.inc_typ_id, t.inc_typ_ds, t.inc_sys_id, ");
            sb.Append("s.inc_sys_nm FROM public.srm_inc_typ t ");
            sb.Append("INNER JOIN public.srm_inc_sys s ON s.inc_sys_id = t.inc_sys_id ");
            sb.Append("ORDER BY t.inc_typ_ds DESC;");
            string query = sb.ToString();
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
                            serviceTypeList.Add(new ServiceType
                            {
                                Id = reader["inc_typ_id"] == DBNull.Value ? 0 : (int)reader["inc_typ_id"],
                                Name = reader["inc_typ_ds"] == DBNull.Value ? "" : reader["inc_typ_ds"].ToString(),
                                ServiceSystemId = reader["inc_sys_id"] == DBNull.Value ? (int?)null : (int)reader["inc_sys_id"],
                                ServiceSystemName = reader["inc_sys_nm"] == DBNull.Value ? "" : reader["inc_sys_nm"].ToString(),

                            });
                        }
                }
                await conn.CloseAsync();
            }
            return serviceTypeList;

        }

        #endregion

        #region Service Request Note Action Methods
        public async Task<List<ServiceRequestNote>> GetServiceRequestNotesByIncidentIdAsync(long serviceIncidentId)
        {
            List<ServiceRequestNote> serviceRequestNotes = new List<ServiceRequestNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT nts_id, nts_tm, nts_ds, nts_by, inc_id, is_ccl, ccl_by, ccl_dt ");
            sb.Append("FROM public.srm_inc_nts WHERE (inc_id = @inc_id ");
            sb.Append("WHERE (inc_id = @inc_id) ORDER BY nts_id DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    inc_id.Value = serviceIncidentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            serviceRequestNotes.Add(new ServiceRequestNote
                            {

                                NoteId = reader["nts_id"] == DBNull.Value ? 0 : (long)reader["nts_id"],
                                NoteTime = reader["nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["nts_tm"],
                                NoteContent = reader["nts_ds"] == DBNull.Value ? string.Empty : reader["nts_ds"].ToString(),
                                NoteWrittenBy = reader["nts_by"] == DBNull.Value ? string.Empty : reader["nts_by"].ToString(),
                                ServiceIncidentId = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                IsCancelled = reader["is_ccl"] == DBNull.Value ? false : (bool)reader["is_ccl"],
                                CancelledBy = reader["ccl_by"] == DBNull.Value ? "" : reader["ccl_by"].ToString(),
                                CancelledOn = reader["ccl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ccl_dt"],
                            });
                        }
                }
            }
            return serviceRequestNotes;
        }
        public async Task<bool> AddNoteAsync(ServiceRequestNote n)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_nts(nts_tm, nts_ds, nts_by, ");
            sb.Append("inc_id, is_ccl, ccl_by, ccl_dt) Values (@nts_tm, @nts_ds, ");
            sb.Append("@nts_by, @inc_id, @is_ccl, @ccl_by, @ccl_dt); ");
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
                    var inc_id = cmd.Parameters.Add("@tsk_id", NpgsqlDbType.Bigint);
                    var is_ccl = cmd.Parameters.Add("@is_ccl", NpgsqlDbType.Boolean);
                    var ccl_by = cmd.Parameters.Add("@ccl_by", NpgsqlDbType.Text);
                    var ccl_dt = cmd.Parameters.Add("@ccl_dt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    nts_tm.Value = n.NoteTime;
                    nts_ds.Value = n.NoteContent;
                    nts_by.Value = n.NoteWrittenBy ?? (object)DBNull.Value;
                    inc_id.Value = n.ServiceIncidentId;
                    is_ccl.Value = false;
                    ccl_by.Value = n.CancelledBy ?? (object)DBNull.Value;
                    ccl_dt.Value = n.CancelledOn ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateServiceRequestNoteToIsCancelledAsync(long serviceRequestNoteId, bool isCancelled, string cancelledBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.srm_inc_nts SET is_ccl=@is_ccl, ");
            sb.Append("ccl_by=@ccl_by, ccl_dt=@ccl_dt  ");
            sb.Append("WHERE (nts_id=@nts_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var is_ccl = cmd.Parameters.Add("@is_ccl", NpgsqlDbType.Boolean);
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    var ccl_dt = cmd.Parameters.Add("@ccl_dt", NpgsqlDbType.Timestamp);
                    var ccl_by = cmd.Parameters.Add("@ccl_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    nts_id.Value = serviceRequestNoteId;
                    is_ccl.Value = isCancelled;
                    ccl_dt.Value = DateTime.Now;
                    ccl_by.Value = cancelledBy ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceRequestNoteAsync(long serviceRequestNoteId)
        {
            int rows = 0;
            string query = "DELETE FROM public.srm_inc_nts WHERE (nts_id=@nts_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    nts_id.Value = serviceRequestNoteId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        #endregion

        #region Service Request Activity Log Action Methods
        public async Task<List<ServiceRequestActivity>> GetServiceRequestActivitysByServiceIncidentIdAsync(long serviceIncidentId)
        {
            List<ServiceRequestActivity> activityLogs = new List<ServiceRequestActivity>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT inc_hst_id, inc_hst_tm, inc_hst_ds, ");
            sb.Append("inc_hst_by, inc_id FROM public.srm_inc_hst ");
            sb.Append("WHERE (inc_id = @inc_id) ");
            sb.Append("ORDER BY inc_hst_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    inc_id.Value = serviceIncidentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            activityLogs.Add(new ServiceRequestActivity
                            {
                                ActivityHistoryId = reader["inc_hst_id"] == DBNull.Value ? 0 : (long)reader["inc_hst_id"],
                                ServiceIncidentId = reader["inc_id"] == DBNull.Value ? 0 : (long)reader["inc_id"],
                                ActivityTime = reader["inc_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["inc_hst_tm"],
                                ActivityDescription = reader["inc_hst_ds"] == DBNull.Value ? string.Empty : reader["inc_hst_ds"].ToString(),
                                ActivityBy = reader["inc_hst_by"] == DBNull.Value ? string.Empty : reader["inc_hst_by"].ToString(),
                            });
                        }
                }
            }
            return activityLogs;
        }
        public async Task<bool> AddServiceRequestActivityAsync(ServiceRequestActivity log)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.srm_inc_hst(inc_hst_tm, inc_hst_ds, inc_hst_by, inc_id) ");
            sb.Append("VALUES (@inc_hst_tm, @inc_hst_ds, @inc_hst_by, @inc_id );");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_hst_tm = cmd.Parameters.Add("@inc_hst_tm", NpgsqlDbType.Timestamp);
                    var inc_hst_ds = cmd.Parameters.Add("@inc_hst_ds", NpgsqlDbType.Text);
                    var inc_hst_by = cmd.Parameters.Add("@inc_hst_by", NpgsqlDbType.Text);
                    var inc_id = cmd.Parameters.Add("@inc_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_hst_tm.Value = log.ActivityTime;
                    inc_hst_ds.Value = log.ActivityDescription;
                    inc_hst_by.Value = log.ActivityBy;
                    inc_id.Value = log.ServiceIncidentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteServiceRequestActivityAsync(long activityLogId)
        {
            int rows = 0;
            string query = "DELETE FROM public.srm_inc_hst WHERE (inc_hst_id = @inc_hst_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var inc_hst_id = cmd.Parameters.Add("@inc_hst_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    inc_hst_id.Value = activityLogId;
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
