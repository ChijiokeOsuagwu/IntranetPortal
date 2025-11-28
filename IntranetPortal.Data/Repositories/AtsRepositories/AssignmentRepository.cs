using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Repositories.AtsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.AtsRepositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        public IConfiguration _config { get; }
        public AssignmentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Assignment Repository

        #region Assignment Read Action Methods
        public async Task<List<Assignment>> GetAssignmentsByClientIdAsync(string clientId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<Assignment> listOfAssignments = new List<Assignment>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-2);
            DateTime to_date = toDate ?? DateTime.Now.AddMonths(1);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.assgn_id, a.assgn_tl, a.assgn_ds, a.evnt_typ_id, ");
            sb.Append("a.start_time, a.end_time, a.station_id, a.assigned_to_id, ");
            sb.Append("a.evnt_venue, a.evnt_state, a.bzns_id, a.liaison_nm, a.liaison_phn, ");
            sb.Append("a.approval_status, a.progress_status, a.ispd, a.islv, a.isus, a.ispr, ");
            sb.Append("a.ctb, a.ctt, a.due_date, a.assigned_by_id, a.evnt_ctr, ");
            sb.Append("a.iscnf, a.assgn_no, a.assigned_to_rl, ");
            sb.Append("(SELECT evnt_typ_ds FROM public.ats_evnt_typs WHERE evnt_typ_id = a.evnt_typ_id) as evnt_typ_nm, ");
            sb.Append("(SELECT locname  FROM public.gst_locs WHERE locqk = a.station_id) as station_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_to_id) as assigned_to_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_by_id) as assigned_by_nm, ");
            sb.Append("(SELECT bzns_nm FROM public.bpm_bzns_inf WHERE bzns_id = a.bzns_id) as bzns_nm ");
            sb.Append("FROM public.ats_assg_inf a ");
            sb.Append("WHERE (a.bzns_id = @bzns_id) ");
            sb.Append("AND (a.start_time >= @dt_frm) ");
            sb.Append("AND (a.end_time <= @dt_to) ");
            sb.Append("ORDER BY a.start_time DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    bzns_id.Value = clientId;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            listOfAssignments.Add(new Assignment
                            {
                                Id = reader["assgn_id"] == DBNull.Value ? 0 : (long)reader["assgn_id"],
                                Title = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                Description = reader["assgn_ds"] == DBNull.Value ? string.Empty : reader["assgn_ds"].ToString(),
                                EventTypeId = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                EventStartTime = reader["start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_time"],
                                EventEndTime = reader["end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_time"],
                                StationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                AssignedToId = reader["assigned_to_id"] == DBNull.Value ? string.Empty : reader["assigned_to_id"].ToString(),
                                EventVenue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString(),
                                EventState = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString(),
                                ClientId = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                ClientName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                                ContactPerson = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString(),
                                ContactPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString(),
                                ApprovalStatus = reader["approval_status"] == DBNull.Value ? string.Empty : reader["approval_status"].ToString(),
                                ProgressStatus = reader["progress_status"] == DBNull.Value ? string.Empty : reader["progress_status"].ToString(),
                                IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"],
                                IsLive = reader["islv"] == DBNull.Value ? false : (bool)reader["islv"],
                                IsUsed = reader["isus"] == DBNull.Value ? false : (bool)reader["isus"],
                                IsPriority = reader["ispr"] == DBNull.Value ? false : (bool)reader["ispr"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                                ReportDueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["due_date"],
                                AssignedById = reader["assigned_by_id"] == DBNull.Value ? string.Empty : reader["assigned_by_id"].ToString(),
                                EventCountry = reader["evnt_ctr"] == DBNull.Value ? string.Empty : reader["evnt_ctr"].ToString(),
                                EventTypeTitle = reader["evnt_typ_nm"] == DBNull.Value ? string.Empty : reader["evnt_typ_nm"].ToString(),
                                StationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                AssignedToName = reader["assigned_to_nm"] == DBNull.Value ? string.Empty : reader["assigned_to_nm"].ToString(),
                                AssignedByName = reader["assigned_by_nm"] == DBNull.Value ? string.Empty : reader["assigned_by_nm"].ToString(),
                                IsConfirmed = reader["iscnf"] == DBNull.Value ? false : (bool)reader["iscnf"],
                                No = reader["assgn_no"] == DBNull.Value ? string.Empty : reader["assgn_no"].ToString(),
                                AssignedToRole = reader["assigned_to_rl"] == DBNull.Value ? string.Empty : reader["assigned_to_rl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return listOfAssignments;
        }
        public async Task<List<Assignment>> GetAssignmentsByClientNameAsync(string clientName, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<Assignment> listOfAssignments = new List<Assignment>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-2);
            DateTime to_date = toDate ?? DateTime.Now.AddMonths(1);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.assgn_id, a.assgn_tl, a.assgn_ds, a.evnt_typ_id, ");
            sb.Append("a.start_time, a.end_time, a.station_id, a.assigned_to_id, ");
            sb.Append("a.evnt_venue, a.evnt_state, a.bzns_id, a.liaison_nm, a.liaison_phn, ");
            sb.Append("a.approval_status, a.progress_status, a.ispd, a.islv, a.isus, a.ispr, ");
            sb.Append("a.ctb, a.ctt, a.due_date, a.assigned_by_id, a.evnt_ctr, ");
            sb.Append("a.iscnf, a.assgn_no, a.assigned_to_rl, ");
            sb.Append("(SELECT evnt_typ_ds FROM public.ats_evnt_typs WHERE evnt_typ_id = a.evnt_typ_id) as evnt_typ_nm, ");
            sb.Append("(SELECT locname  FROM public.gst_locs WHERE locqk = a.station_id) as station_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_to_id) as assigned_to_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_by_id) as assigned_by_nm, ");
            sb.Append("(SELECT bzns_nm FROM public.bpm_bzns_inf WHERE bzns_id = a.bzns_id) as bzns_nm ");
            sb.Append("FROM public.ats_assg_inf a ");
            sb.Append("WHERE (bzns_nm = @bzns_nm) ");
            sb.Append("AND (a.start_time >= @dt_frm) ");
            sb.Append("AND (a.end_time <= @dt_to) ");
            sb.Append("ORDER BY a.start_time DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_nm = cmd.Parameters.Add("@bzns_nm", NpgsqlDbType.Text);
                    var dt_frm = cmd.Parameters.Add("@dt_frm", NpgsqlDbType.Timestamp);
                    var dt_to = cmd.Parameters.Add("@dt_to", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    bzns_nm.Value = clientName;
                    dt_frm.Value = from_date;
                    dt_to.Value = to_date;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            listOfAssignments.Add(new Assignment
                            {
                                Id = reader["assgn_id"] == DBNull.Value ? 0 : (long)reader["assgn_id"],
                                Title = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                Description = reader["assgn_ds"] == DBNull.Value ? string.Empty : reader["assgn_ds"].ToString(),
                                EventTypeId = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                EventStartTime = reader["start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_time"],
                                EventEndTime = reader["end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_time"],
                                StationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                AssignedToId = reader["assigned_to_id"] == DBNull.Value ? string.Empty : reader["assigned_to_id"].ToString(),
                                EventVenue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString(),
                                EventState = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString(),
                                ClientId = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                ClientName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                                ContactPerson = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString(),
                                ContactPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString(),
                                ApprovalStatus = reader["approval_status"] == DBNull.Value ? string.Empty : reader["approval_status"].ToString(),
                                ProgressStatus = reader["progress_status"] == DBNull.Value ? string.Empty : reader["progress_status"].ToString(),
                                IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"],
                                IsLive = reader["islv"] == DBNull.Value ? false : (bool)reader["islv"],
                                IsUsed = reader["isus"] == DBNull.Value ? false : (bool)reader["isus"],
                                IsPriority = reader["ispr"] == DBNull.Value ? false : (bool)reader["ispr"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                                ReportDueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["due_date"],
                                AssignedById = reader["assigned_by_id"] == DBNull.Value ? string.Empty : reader["assigned_by_id"].ToString(),
                                EventCountry = reader["evnt_ctr"] == DBNull.Value ? string.Empty : reader["evnt_ctr"].ToString(),
                                EventTypeTitle = reader["evnt_typ_nm"] == DBNull.Value ? string.Empty : reader["evnt_typ_nm"].ToString(),
                                StationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                AssignedToName = reader["assigned_to_nm"] == DBNull.Value ? string.Empty : reader["assigned_to_nm"].ToString(),
                                AssignedByName = reader["assigned_by_nm"] == DBNull.Value ? string.Empty : reader["assigned_by_nm"].ToString(),
                                IsConfirmed = reader["iscnf"] == DBNull.Value ? false : (bool)reader["iscnf"],
                                No = reader["assgn_no"] == DBNull.Value ? string.Empty : reader["assgn_no"].ToString(),
                                AssignedToRole = reader["assigned_to_rl"] == DBNull.Value ? string.Empty : reader["assigned_to_rl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return listOfAssignments;
        }
        public async Task<List<string>> GetAssignmentNumbersByCreatedDateAsync(DateTime createdDate)
        {
            List<string> listOfAssignmentNumbers = new List<string>();
            string _newNumber = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assgn_no FROM public.ats_assg_inf ");
            sb.Append("WHERE date_part('year', ctt) = date_part('year', @ctt) ");
            sb.Append("AND date_part('month', ctt) = date_part('month', @ctt) ");
            sb.Append("ORDER BY assgn_no DESC; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Timestamp);
                    await cmd.PrepareAsync();
                    ctt.Value = createdDate;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            _newNumber = reader["assgn_no"] == DBNull.Value ? string.Empty : reader["assgn_no"].ToString();
                            listOfAssignmentNumbers.Add(_newNumber);
                        }
                }
                await conn.CloseAsync();
            }
            return listOfAssignmentNumbers;
        }
        public async Task<Assignment> GetAssignmentByIdAsync(long assignmentId)
        {
            Assignment assignment = new Assignment();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.assgn_id, a.assgn_tl, a.assgn_ds, a.evnt_typ_id, ");
            sb.Append("a.start_time, a.end_time, a.station_id, a.assigned_to_id, ");
            sb.Append("a.evnt_venue, a.evnt_state, a.bzns_id, a.liaison_nm, a.liaison_phn, ");
            sb.Append("a.approval_status, a.progress_status, a.ispd, a.islv, a.isus, a.ispr, ");
            sb.Append("a.ctb, a.ctt, a.due_date, a.assigned_by_id, a.evnt_ctr, ");
            sb.Append("a.iscnf, a.assgn_no, a.assigned_to_rl, ");
            sb.Append("(SELECT evnt_typ_ds FROM public.ats_evnt_typs WHERE evnt_typ_id = a.evnt_typ_id) as evnt_typ_nm, ");
            sb.Append("(SELECT locname  FROM public.gst_locs WHERE locqk = a.station_id) as station_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_to_id) as assigned_to_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_by_id) as assigned_by_nm, ");
            sb.Append("(SELECT bzns_nm FROM public.bpm_bzns_inf WHERE bzns_id = a.bzns_id) as bzns_nm ");
            sb.Append("FROM public.ats_assg_inf a ");
            sb.Append("WHERE (a.assgn_id = @assgn_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignment.Id = reader["assgn_id"] == DBNull.Value ? 0 : (long)reader["assgn_id"];
                            assignment.Title = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString();
                            assignment.Description = reader["assgn_ds"] == DBNull.Value ? string.Empty : reader["assgn_ds"].ToString();
                            assignment.EventTypeId = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"];
                            assignment.EventStartTime = reader["start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_time"];
                            assignment.EventEndTime = reader["end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_time"];
                            assignment.StationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            assignment.AssignedToId = reader["assigned_to_id"] == DBNull.Value ? string.Empty : reader["assigned_to_id"].ToString();
                            assignment.EventVenue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString();
                            assignment.EventState = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString();
                            assignment.ClientId = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString();
                            assignment.ClientName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString();
                            assignment.ContactPerson = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString();
                            assignment.ContactPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString();
                            assignment.ApprovalStatus = reader["approval_status"] == DBNull.Value ? string.Empty : reader["approval_status"].ToString();
                            assignment.ProgressStatus = reader["progress_status"] == DBNull.Value ? string.Empty : reader["progress_status"].ToString();
                            assignment.IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"];
                            assignment.IsLive = reader["islv"] == DBNull.Value ? false : (bool)reader["islv"];
                            assignment.IsUsed = reader["isus"] == DBNull.Value ? false : (bool)reader["isus"];
                            assignment.IsPriority = reader["ispr"] == DBNull.Value ? false : (bool)reader["ispr"];
                            assignment.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            assignment.CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"];
                            assignment.ReportDueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["due_date"];
                            assignment.AssignedById = reader["assigned_by_id"] == DBNull.Value ? string.Empty : reader["assigned_by_id"].ToString();
                            assignment.EventCountry = reader["evnt_ctr"] == DBNull.Value ? string.Empty : reader["evnt_ctr"].ToString();
                            assignment.EventTypeTitle = reader["evnt_typ_nm"] == DBNull.Value ? string.Empty : reader["evnt_typ_nm"].ToString();
                            assignment.StationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString();
                            assignment.AssignedToName = reader["assigned_to_nm"] == DBNull.Value ? string.Empty : reader["assigned_to_nm"].ToString();
                            assignment.AssignedByName = reader["assigned_by_nm"] == DBNull.Value ? string.Empty : reader["assigned_by_nm"].ToString();
                            assignment.IsConfirmed = reader["iscnf"] == DBNull.Value ? false : (bool)reader["iscnf"];
                            assignment.No = reader["assgn_no"] == DBNull.Value ? string.Empty : reader["assgn_no"].ToString();
                            assignment.AssignedToRole = reader["assigned_to_rl"] == DBNull.Value ? string.Empty : reader["assigned_to_rl"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return assignment;
        }
        public async Task<List<Assignment>> GetAssignmentsByDateRangeAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<Assignment> listOfAssignments = new List<Assignment>();
            DateTime from_date = fromDate ?? DateTime.Now.AddMonths(-1);
            DateTime to_date = toDate ?? DateTime.Now.AddMonths(1);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.assgn_id, a.assgn_tl, a.assgn_ds, a.evnt_typ_id, ");
            sb.Append("a.start_time, a.end_time, a.station_id, a.assigned_to_id, ");
            sb.Append("a.evnt_venue, a.evnt_state, a.bzns_id, a.liaison_nm, a.liaison_phn, ");
            sb.Append("a.approval_status, a.progress_status, a.ispd, a.islv, a.isus, a.ispr, ");
            sb.Append("a.ctb, a.ctt, a.due_date, a.assigned_by_id, a.evnt_ctr, ");
            sb.Append("a.iscnf, a.assgn_no, a.assigned_to_rl, ");
            sb.Append("(SELECT evnt_typ_ds FROM public.ats_evnt_typs WHERE evnt_typ_id = a.evnt_typ_id) as evnt_typ_nm, ");
            sb.Append("(SELECT locname  FROM public.gst_locs WHERE locqk = a.station_id) as station_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_to_id) as assigned_to_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = a.assigned_by_id) as assigned_by_nm, ");
            sb.Append("(SELECT bzns_nm FROM public.bpm_bzns_inf WHERE bzns_id = a.bzns_id) as bzns_nm ");
            sb.Append("FROM public.ats_assg_inf a ");
            sb.Append("WHERE (a.start_time >= @dt_frm) ");
            sb.Append("AND (a.end_time <= @dt_to) ");
            sb.Append("ORDER BY a.start_time DESC; ");

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
                            listOfAssignments.Add(new Assignment
                            {
                                Id = reader["assgn_id"] == DBNull.Value ? 0 : (long)reader["assgn_id"],
                                Title = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                Description = reader["assgn_ds"] == DBNull.Value ? string.Empty : reader["assgn_ds"].ToString(),
                                EventTypeId = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                EventStartTime = reader["start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_time"],
                                EventEndTime = reader["end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["end_time"],
                                StationId = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                AssignedToId = reader["assigned_to_id"] == DBNull.Value ? string.Empty : reader["assigned_to_id"].ToString(),
                                EventVenue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString(),
                                EventState = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString(),
                                ClientId = reader["bzns_id"] == DBNull.Value ? string.Empty : reader["bzns_id"].ToString(),
                                ClientName = reader["bzns_nm"] == DBNull.Value ? string.Empty : reader["bzns_nm"].ToString(),
                                ContactPerson = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString(),
                                ContactPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString(),
                                ApprovalStatus = reader["approval_status"] == DBNull.Value ? string.Empty : reader["approval_status"].ToString(),
                                ProgressStatus = reader["progress_status"] == DBNull.Value ? string.Empty : reader["progress_status"].ToString(),
                                IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"],
                                IsLive = reader["islv"] == DBNull.Value ? false : (bool)reader["islv"],
                                IsUsed = reader["isus"] == DBNull.Value ? false : (bool)reader["isus"],
                                IsPriority = reader["ispr"] == DBNull.Value ? false : (bool)reader["ispr"],
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                                ReportDueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["due_date"],
                                AssignedById = reader["assigned_by_id"] == DBNull.Value ? string.Empty : reader["assigned_by_id"].ToString(),
                                EventCountry = reader["evnt_ctr"] == DBNull.Value ? string.Empty : reader["evnt_ctr"].ToString(),
                                EventTypeTitle = reader["evnt_typ_nm"] == DBNull.Value ? string.Empty : reader["evnt_typ_nm"].ToString(),
                                StationName = reader["station_nm"] == DBNull.Value ? string.Empty : reader["station_nm"].ToString(),
                                AssignedToName = reader["assigned_to_nm"] == DBNull.Value ? string.Empty : reader["assigned_to_nm"].ToString(),
                                AssignedByName = reader["assigned_by_nm"] == DBNull.Value ? string.Empty : reader["assigned_by_nm"].ToString(),
                                IsConfirmed = reader["iscnf"] == DBNull.Value ? false : (bool)reader["iscnf"],
                                No = reader["assgn_no"] == DBNull.Value ? string.Empty : reader["assgn_no"].ToString(),
                                AssignedToRole = reader["assigned_to_rl"] == DBNull.Value ? string.Empty : reader["assigned_to_rl"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return listOfAssignments;
        }

        #endregion

        #region Assignment Write Action Methods
        public async Task<long> AddAssignmentAsync(Assignment assignment)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_inf(assgn_tl, assgn_ds, evnt_typ_id, ");
            sb.Append("start_time, end_time, station_id, assigned_to_id, evnt_venue, ");
            sb.Append("evnt_state, bzns_id, liaison_nm, liaison_phn, approval_status, ");
            sb.Append("progress_status, ispd, islv, isus, ispr, ctb, ctt, due_date, ");
            sb.Append("assigned_by_id, evnt_ctr, iscnf, assgn_no, assigned_to_rl, mdb, mdt) ");
            sb.Append("VALUES (@assgn_tl, @assgn_ds, @evnt_typ_id, @start_time, @end_time, ");
            sb.Append("@station_id, @assigned_to_id, @evnt_venue, @evnt_state, @bzns_id, ");
            sb.Append("@liaison_nm, @liaison_phn, @approval_status, @progress_status, ");
            sb.Append("@ispd, @islv, @isus, @ispr, @ctb, @ctt, @due_date, @assigned_by_id, ");
            sb.Append("@evnt_ctr, @iscnf, @assgn_no, @assigned_to_rl, @ctb, @ctt) ");
            sb.Append("RETURNING assgn_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_tl = cmd.Parameters.Add("@assgn_tl", NpgsqlDbType.Text);
                    var assgn_ds = cmd.Parameters.Add("@assgn_ds", NpgsqlDbType.Text);
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    var start_time = cmd.Parameters.Add("@start_time", NpgsqlDbType.Timestamp);
                    var end_time = cmd.Parameters.Add("@end_time", NpgsqlDbType.Timestamp);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var assigned_to_id = cmd.Parameters.Add("@assigned_to_id", NpgsqlDbType.Text);
                    var assigned_by_id = cmd.Parameters.Add("@assigned_by_id", NpgsqlDbType.Text);
                    var evnt_venue = cmd.Parameters.Add("@evnt_venue", NpgsqlDbType.Text);
                    var evnt_state = cmd.Parameters.Add("@evnt_state", NpgsqlDbType.Text);
                    var evnt_ctr = cmd.Parameters.Add("@evnt_ctr", NpgsqlDbType.Text);
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var liaison_nm = cmd.Parameters.Add("@liaison_nm", NpgsqlDbType.Text);
                    var liaison_phn = cmd.Parameters.Add("@liaison_phn", NpgsqlDbType.Text);
                    var approval_status = cmd.Parameters.Add("@approval_status", NpgsqlDbType.Text);
                    var progress_status = cmd.Parameters.Add("@progress_status", NpgsqlDbType.Text);
                    var ispd = cmd.Parameters.Add("@ispd", NpgsqlDbType.Boolean);
                    var islv = cmd.Parameters.Add("@islv", NpgsqlDbType.Boolean);
                    var isus = cmd.Parameters.Add("@isus", NpgsqlDbType.Boolean);
                    var ispr = cmd.Parameters.Add("@ispr", NpgsqlDbType.Boolean);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Timestamp);
                    var due_date = cmd.Parameters.Add("@due_date", NpgsqlDbType.Timestamp);
                    var iscnf = cmd.Parameters.Add("iscnf", NpgsqlDbType.Boolean);
                    var assgn_no = cmd.Parameters.Add("@assgn_no", NpgsqlDbType.Text);
                    var assigned_to_rl = cmd.Parameters.Add("@assigned_to_rl", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assgn_tl.Value = assignment.Title;
                    assgn_ds.Value = assignment.Description ?? (object)DBNull.Value;
                    evnt_typ_id.Value = assignment.EventTypeId;
                    start_time.Value = assignment.EventStartTime ?? (object)DBNull.Value;
                    end_time.Value = assignment.EventEndTime ?? (object)DBNull.Value;
                    station_id.Value = assignment.StationId ?? (object)DBNull.Value;
                    assigned_to_id.Value = assignment.AssignedToId ?? (object)DBNull.Value;
                    assigned_by_id.Value = assignment.AssignedById ?? (object)DBNull.Value;
                    evnt_venue.Value = assignment.EventVenue ?? (object)DBNull.Value;
                    evnt_state.Value = assignment.EventState ?? (object)DBNull.Value;
                    evnt_ctr.Value = assignment.EventCountry ?? (object)DBNull.Value;
                    bzns_id.Value = assignment.ClientId;
                    liaison_nm.Value = assignment.ContactPerson ?? (object)DBNull.Value;
                    liaison_phn.Value = assignment.ContactPhone ?? (object)DBNull.Value;
                    approval_status.Value = assignment.ApprovalStatus ?? (object)DBNull.Value;
                    progress_status.Value = assignment.ProgressStatus ?? (object)DBNull.Value;
                    ispd.Value = assignment.IsPaid;
                    islv.Value = assignment.IsLive;
                    isus.Value = assignment.IsUsed;
                    ispr.Value = assignment.IsPriority;
                    ctb.Value = assignment.CreatedBy;
                    ctt.Value = assignment.CreatedTime ?? DateTime.Now;
                    due_date.Value = assignment.ReportDueDate ?? (object)DBNull.Value;
                    iscnf.Value = assignment.IsConfirmed;
                    assgn_no.Value = assignment.No;
                    assigned_to_rl.Value = assignment.AssignedToRole;

                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentAsync(Assignment assignment)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_inf SET assgn_tl=@assgn_tl, ");
            sb.Append("assgn_ds=@assgn_ds, evnt_typ_id=@evnt_typ_id, ");
            sb.Append("start_time=@start_time, end_time=@end_time, ");
            sb.Append("station_id=@station_id, evnt_venue=@evnt_venue, ");
            sb.Append("evnt_state=@evnt_state, bzns_id=@bzns_id, ");
            sb.Append("approval_status=@approval_status, ");
            sb.Append("progress_status=@progress_status, ispd=@ispd, islv=@islv, ");
            sb.Append("isus=@isus, ispr=@ispr, due_date=@due_date, ");
            sb.Append("evnt_ctr=@evnt_ctr, iscnf=@iscnf, mdb=@mdb, mdt=@mdt ");
            sb.Append("WHERE (assgn_id=@assgn_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_tl = cmd.Parameters.Add("@assgn_tl", NpgsqlDbType.Text);
                    var assgn_ds = cmd.Parameters.Add("@assgn_ds", NpgsqlDbType.Text);
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    var start_time = cmd.Parameters.Add("@start_time", NpgsqlDbType.Timestamp);
                    var end_time = cmd.Parameters.Add("@end_time", NpgsqlDbType.Timestamp);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var evnt_venue = cmd.Parameters.Add("@evnt_venue", NpgsqlDbType.Text);
                    var evnt_state = cmd.Parameters.Add("@evnt_state", NpgsqlDbType.Text);
                    var bzns_id = cmd.Parameters.Add("@bzns_id", NpgsqlDbType.Text);
                    var approval_status = cmd.Parameters.Add("@approval_status", NpgsqlDbType.Text);
                    var progress_status = cmd.Parameters.Add("@progress_status", NpgsqlDbType.Text);
                    var ispd = cmd.Parameters.Add("@ispd", NpgsqlDbType.Boolean);
                    var islv = cmd.Parameters.Add("@islv", NpgsqlDbType.Boolean);
                    var isus = cmd.Parameters.Add("@isus", NpgsqlDbType.Boolean);
                    var ispr = cmd.Parameters.Add("@ispr", NpgsqlDbType.Boolean);
                    var due_date = cmd.Parameters.Add("@due_date", NpgsqlDbType.Timestamp);
                    var evnt_ctr = cmd.Parameters.Add("@evnt_ctr", NpgsqlDbType.Text);
                    var iscnf = cmd.Parameters.Add("@iscnf", NpgsqlDbType.Boolean);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assgn_tl.Value = assignment.Title;
                    assgn_ds.Value = assignment.Description ?? (object)DBNull.Value;
                    evnt_typ_id.Value = assignment.EventTypeId == null ? (object)DBNull.Value : assignment.EventTypeId;
                    start_time.Value = assignment.EventStartTime ?? (object)DBNull.Value;
                    end_time.Value = assignment.EventEndTime ?? (object)DBNull.Value;
                    station_id.Value = assignment.StationId ?? (object)DBNull.Value;
                    evnt_venue.Value = assignment.EventVenue ?? (object)DBNull.Value;
                    evnt_state.Value = assignment.EventState ?? (object)DBNull.Value;
                    bzns_id.Value = assignment.ClientId ?? (object)DBNull.Value;
                    approval_status.Value = assignment.ApprovalStatus ?? (object)DBNull.Value;
                    progress_status.Value = assignment.ProgressStatus ?? (object)DBNull.Value;
                    ispd.Value = assignment.IsPaid;
                    islv.Value = assignment.IsLive;
                    isus.Value = assignment.IsUsed;
                    ispr.Value = assignment.IsPriority;
                    due_date.Value = assignment.ReportDueDate ?? (object)DBNull.Value;
                    evnt_ctr.Value = assignment.EventCountry ?? (object)DBNull.Value;
                    iscnf.Value = assignment.IsConfirmed;
                    assgn_id.Value = assignment.Id;
                    mdb.Value = assignment.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignment.ModifiedTime ?? DateTime.Now;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> UpdateAssignmentProgressStatusAsync(long assignmentId, string progressStatus)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_inf SET progress_status=@progress_status ");
            sb.Append("WHERE (assgn_id=@assgn_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var progress_status = cmd.Parameters.Add("@progress_status", NpgsqlDbType.Text);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    progress_status.Value = progressStatus ?? (object)DBNull.Value;
                    assgn_id.Value = assignmentId;

                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        public async Task<bool> DeleteAssignmentAsync(long assignmentId)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM public.wsp_wki_hst WHERE tsk_itm_id IN (SELECT ");
            sb.Append("tsk_itm_id FROM public.wsp_tsk_itms WHERE assgnmt_id = @assgn_id);");
            sb.Append("DELETE FROM public.wsp_tsk_itms WHERE(assgnmt_id = @assgn_id); ");

            sb.Append("DELETE FROM public.ats_assg_crw WHERE (assgn_id=@assgn_id); ");
            sb.Append("DELETE FROM public.ats_assg_hst WHERE (assgn_id=@assgn_id); ");
            sb.Append("DELETE FROM public.ats_assg_nts WHERE (assgn_id=@assgn_id); ");
            sb.Append("DELETE FROM public.ats_assg_inf WHERE (assgn_id=@assgn_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assgn_id.Value = assignmentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #region Assignment Crew Action Methods
        public async Task<AssignmentCrewMember> GetAssignmentCrewMemberbyIdAsync(long assignmentCrewId)
        {
            AssignmentCrewMember assignmentCrew = new AssignmentCrewMember();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.assg_crw_id, c.assgn_id, c.emp_id, c.emp_rl1, c.unit_id, ");
            sb.Append("c.dept_id, c.loc_id, c.is_ld, c.emp_rl2, c.emp_rl3, u.unitname, d.deptname, ");
            sb.Append("l.locname, c.mdb, c.mdt, c.ctb, c.ctt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = c.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = c.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_assg_crw c ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON c.unit_id=u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = c.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = c.loc_id ");
            sb.Append("WHERE (c.assg_crw_id = @assg_crw_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_crw_id = cmd.Parameters.Add("@assg_crw_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assg_crw_id.Value = assignmentCrewId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentCrew.Id = reader["assg_crw_id"] == DBNull.Value ? 0L : (long)reader["assg_crw_id"];
                            assignmentCrew.AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"];
                            assignmentCrew.CrewMemberId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                            assignmentCrew.CrewMemberName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();
                            assignmentCrew.CrewMemberRole1 = reader["emp_rl1"] == DBNull.Value ? string.Empty : reader["emp_rl1"].ToString();
                            assignmentCrew.CrewMemberRole2 = reader["emp_rl2"] == DBNull.Value ? string.Empty : reader["emp_rl2"].ToString();
                            assignmentCrew.CrewMemberRole3 = reader["emp_rl3"] == DBNull.Value ? string.Empty : reader["emp_rl3"].ToString();
                            assignmentCrew.IsTeamLead = reader["is_ld"] == DBNull.Value ? false : (bool)reader["is_ld"];
                            assignmentCrew.UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"];
                            assignmentCrew.UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString();
                            assignmentCrew.DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"];
                            assignmentCrew.DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString();
                            assignmentCrew.LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"];
                            assignmentCrew.LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            assignmentCrew.AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString();
                            assignmentCrew.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            assignmentCrew.CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"];
                            assignmentCrew.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            assignmentCrew.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentCrew;
        }
        public async Task<AssignmentCrewMember> GetAssignmentCrewMemberbyAssignmentIdnEmployeeIdAsync(long assignmentId, string employeeId)
        {
            AssignmentCrewMember assignmentCrew = new AssignmentCrewMember();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.assg_crw_id, c.assgn_id, c.emp_id, c.emp_rl1, c.unit_id, ");
            sb.Append("c.dept_id, c.loc_id, c.is_ld, c.emp_rl2, c.emp_rl3, u.unitname, d.deptname, ");
            sb.Append("l.locname, c.mdb, c.mdt, c.ctb, c.ctt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = c.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = c.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_assg_crw c ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON c.unit_id=u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = c.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = c.loc_id ");
            sb.Append("WHERE (c.assgn_id = @assgn_id) AND (c.emp_id = @emp_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    emp_id.Value = employeeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentCrew.Id = reader["assg_crw_id"] == DBNull.Value ? 0L : (long)reader["assg_crw_id"];
                            assignmentCrew.AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"];
                            assignmentCrew.CrewMemberId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                            assignmentCrew.CrewMemberName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();
                            assignmentCrew.CrewMemberRole1 = reader["emp_rl1"] == DBNull.Value ? string.Empty : reader["emp_rl1"].ToString();
                            assignmentCrew.CrewMemberRole2 = reader["emp_rl2"] == DBNull.Value ? string.Empty : reader["emp_rl2"].ToString();
                            assignmentCrew.CrewMemberRole3 = reader["emp_rl3"] == DBNull.Value ? string.Empty : reader["emp_rl3"].ToString();
                            assignmentCrew.IsTeamLead = reader["is_ld"] == DBNull.Value ? false : (bool)reader["is_ld"];
                            assignmentCrew.UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"];
                            assignmentCrew.UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString();
                            assignmentCrew.DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"];
                            assignmentCrew.DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString();
                            assignmentCrew.LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"];
                            assignmentCrew.LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            assignmentCrew.AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString();
                            assignmentCrew.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            assignmentCrew.CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"];
                            assignmentCrew.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            assignmentCrew.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentCrew;
        }
        public async Task<List<AssignmentCrewMember>> GetAssignmentCrewMembersbyAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentCrewMember> assignmentCrewList = new List<AssignmentCrewMember>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.assg_crw_id, c.assgn_id, c.emp_id, c.emp_rl1, c.unit_id, ");
            sb.Append("c.dept_id, c.loc_id, c.is_ld, c.emp_rl2, c.emp_rl3, u.unitname, d.deptname, ");
            sb.Append("l.locname, c.mdb, c.mdt, c.ctb, c.ctt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = c.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = c.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_assg_crw c ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON c.unit_id=u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = c.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = c.loc_id ");
            sb.Append("WHERE (c.assgn_id = @assgn_id) ORDER BY is_ld DESC, emp_nm ASC;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentCrewList.Add(new AssignmentCrewMember
                            {
                                Id = reader["assg_crw_id"] == DBNull.Value ? 0L : (long)reader["assg_crw_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                CrewMemberId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                CrewMemberName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                CrewMemberRole1 = reader["emp_rl1"] == DBNull.Value ? string.Empty : reader["emp_rl1"].ToString(),
                                CrewMemberRole2 = reader["emp_rl2"] == DBNull.Value ? string.Empty : reader["emp_rl2"].ToString(),
                                CrewMemberRole3 = reader["emp_rl3"] == DBNull.Value ? string.Empty : reader["emp_rl3"].ToString(),
                                IsTeamLead = reader["is_ld"] == DBNull.Value ? false : (bool)reader["is_ld"],
                                UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentCrewList;
        }
        public async Task<List<AssignmentCrewMember>> GetAssignmentCrewMembersbyCrewMemberIdAsync(string employeeId)
        {
            List<AssignmentCrewMember> assignmentCrewList = new List<AssignmentCrewMember>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.assg_crw_id, c.assgn_id, c.emp_id, c.emp_rl1, c.unit_id, ");
            sb.Append("c.dept_id, c.loc_id, c.is_ld, c.emp_rl2, c.emp_rl3, u.unitname, d.deptname, ");
            sb.Append("l.locname, c.mdb, c.mdt, c.ctb, c.ctt, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = c.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = c.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_assg_crw c ");
            sb.Append("LEFT OUTER JOIN public.gst_units u ON c.unit_id=u.unitqk ");
            sb.Append("LEFT OUTER JOIN public.gst_depts d ON d.deptqk = c.dept_id ");
            sb.Append("LEFT OUTER JOIN public.gst_locs l ON l.locqk = c.loc_id ");
            sb.Append("WHERE (c.emp_id = @emp_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    //var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    //assgn_id.Value = assignmentId;
                    emp_id.Value = employeeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentCrewList.Add(new AssignmentCrewMember
                            {
                                Id = reader["assg_crw_id"] == DBNull.Value ? 0L : (long)reader["assg_crw_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                CrewMemberId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                CrewMemberName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                CrewMemberRole1 = reader["emp_rl1"] == DBNull.Value ? string.Empty : reader["emp_rl1"].ToString(),
                                CrewMemberRole2 = reader["emp_rl2"] == DBNull.Value ? string.Empty : reader["emp_rl2"].ToString(),
                                CrewMemberRole3 = reader["emp_rl3"] == DBNull.Value ? string.Empty : reader["emp_rl3"].ToString(),
                                IsTeamLead = reader["is_ld"] == DBNull.Value ? false : (bool)reader["is_ld"],
                                UnitId = reader["unit_id"] == DBNull.Value ? (int?)null : (int)reader["unit_id"],
                                UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                                DepartmentId = reader["dept_id"] == DBNull.Value ? (int?)null : (int)reader["dept_id"],
                                DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                                LocationId = reader["loc_id"] == DBNull.Value ? (int?)null : (int)reader["loc_id"],
                                LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ctt"],
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentCrewList;
        }
        
        public async Task<List<Employee>> GetAssignmentEmployeesByAssignmentIdAsync(long assignmentId)
        {
            List<Employee> employeeList = new List<Employee>();
            if (assignmentId < 1) { throw new ArgumentNullException(nameof(assignmentId)); }

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.emp_id, e.emp_no_1, e.emp_no_2, e.start_up_date, ");
            sb.Append("e.yrs_of_experience, e.start_up_designation, e.place_of_engagement, ");
            sb.Append("e.confirmation_date, e.current_designation, e.job_grade, e.employment_status, ");
            sb.Append("e.date_of_last_promotion, e.official_email, e.state_of_origin, e.is_dx, ");
            sb.Append("e.lga_of_origin, e.religion, e.geo_political_region, e.next_of_kin_name, ");
            sb.Append("e.next_of_kin_relationship, e.modified_by, e.modified_date, e.dx_time, e.dx_by, ");
            sb.Append("e.created_by, e.created_date, e.next_of_kin_address, e.next_of_kin_phone, ");
            sb.Append("e.next_of_kin_email, e.dept_id, e.unit_id, e.loc_id, e.coy_id, p.id, p.title, ");
            sb.Append("p.sname, p.fname, p.oname, p.fullname, p.sex, p.phone1, p.phone2,");
            sb.Append("p.email AS personal_email, p.address, p.mdb, p.mdt, p.ctb, p.ctt, ");
            sb.Append("p.imgp, p.birthday, p.birthmonth, p.birthyear, p.maritalstatus, l.locname, ");
            sb.Append("l.loctype, l.lochq1, l.lochq2, l.locmb, l.locmd, l.loccb, l.loccd, l.locctr, ");
            sb.Append("l.locst, l.locqk, c.coy_code, c.coy_name, d.deptname, d.depthd1, d.depthd2, ");
            sb.Append("d.deptqk, u.unitname, u.unithd1, u.unithd2, u.unitqk ");
            sb.Append("FROM erm_emp_inf e JOIN gst_prsns p ON e.emp_id = p.id ");
            //sb.Append("AND e.is_dx = false ");
            sb.Append("LEFT JOIN gst_locs l ON e.loc_id = l.locqk ");
            sb.Append("LEFT JOIN gst_coys c ON e.coy_id = c.coy_code ");
            sb.Append("LEFT JOIN gst_depts d ON e.dept_id = d.deptqk ");
            sb.Append("LEFT JOIN gst_units u ON e.unit_id = u.unitqk ");
            sb.Append("WHERE e.emp_id IN (SELECT emp_id FROM public.ats_assg_crw WHERE assgn_id = @assgn_id) ");
            sb.Append("ORDER BY p.fullname;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;

                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        employeeList.Add(new Employee()
                        {
                            EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                            EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                            EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                            StartUpDate = reader["start_up_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["start_up_date"],
                            YearsOfExperience = reader["yrs_of_experience"] == DBNull.Value ? 0 : (int)reader["yrs_of_experience"],
                            StartUpDesignation = reader["start_up_designation"] == DBNull.Value ? String.Empty : reader["start_up_designation"].ToString(),
                            PlaceOfEngagement = reader["place_of_engagement"] == DBNull.Value ? String.Empty : reader["place_of_engagement"].ToString(),
                            ConfirmationDate = reader["confirmation_date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["confirmation_date"],
                            CurrentDesignation = reader["current_designation"] == DBNull.Value ? String.Empty : reader["current_designation"].ToString(),
                            JobGrade = reader["job_grade"] == DBNull.Value ? String.Empty : reader["job_grade"].ToString(),
                            EmploymentStatus = reader["employment_status"] == DBNull.Value ? String.Empty : reader["employment_status"].ToString(),
                            DateOfLastPromotion = reader["date_of_last_promotion"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["date_of_last_promotion"],
                            LengthOfService = reader["start_up_date"] == DBNull.Value ? 0 : (int)((DateTime.Now - (DateTime)reader["start_up_date"]).TotalDays),

                            OfficialEmail = reader["official_email"] == DBNull.Value ? String.Empty : reader["official_email"].ToString(),
                            StateOfOrigin = reader["state_of_origin"] == DBNull.Value ? String.Empty : reader["state_of_origin"].ToString(),
                            LgaOfOrigin = reader["lga_of_origin"] == DBNull.Value ? String.Empty : reader["lga_of_origin"].ToString(),
                            Religion = reader["religion"] == DBNull.Value ? String.Empty : reader["religion"].ToString(),
                            GeoPoliticalRegion = reader["geo_political_region"] == DBNull.Value ? String.Empty : reader["geo_political_region"].ToString(),
                            NextOfKinName = reader["next_of_kin_name"] == DBNull.Value ? String.Empty : reader["next_of_kin_name"].ToString(),
                            NextOfKinRelationship = reader["next_of_kin_relationship"] == DBNull.Value ? String.Empty : reader["next_of_kin_relationship"].ToString(),
                            NextOfKinAddress = reader["next_of_kin_address"] == DBNull.Value ? String.Empty : reader["next_of_kin_address"].ToString(),
                            NextOfKinPhone = reader["next_of_kin_phone"] == DBNull.Value ? String.Empty : reader["next_of_kin_phone"].ToString(),
                            NextOfKinEmail = reader["next_of_kin_email"] == DBNull.Value ? String.Empty : reader["next_of_kin_email"].ToString(),
                            CompanyID = reader["coy_id"] == DBNull.Value ? string.Empty : (reader["coy_id"]).ToString(),
                            DepartmentID = reader["dept_id"] == DBNull.Value ? 0 : (int)(reader["dept_id"]),
                            UnitID = reader["unit_id"] == DBNull.Value ? 0 : (int)(reader["unit_id"]),
                            LocationID = reader["loc_id"] == DBNull.Value ? 0 : (int)(reader["loc_id"]),
                            EmployeeModifiedBy = reader["modified_by"] == DBNull.Value ? string.Empty : reader["modified_by"].ToString(),
                            EmployeeModifiedDate = reader["modified_date"] == DBNull.Value ? string.Empty : reader["modified_date"].ToString(),
                            EmployeeCreatedBy = reader["created_by"] == DBNull.Value ? string.Empty : reader["created_by"].ToString(),
                            EmployeeCreatedDate = reader["created_date"] == DBNull.Value ? string.Empty : reader["created_date"].ToString(),
                            IsDeactivated = reader["is_dx"] == DBNull.Value ? true : (bool)reader["is_dx"],
                            DeactivationTime = reader["dx_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["dx_time"],
                            DeactivatedBy = reader["dx_by"] == DBNull.Value ? string.Empty : reader["dx_by"].ToString(),

                            PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                            Title = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            Surname = reader["sname"] == DBNull.Value ? string.Empty : reader["sname"].ToString(),
                            FirstName = reader["fname"] == DBNull.Value ? string.Empty : reader["fname"].ToString(),
                            OtherNames = reader["oname"] == DBNull.Value ? string.Empty : reader["oname"].ToString(),
                            FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                            Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                            MaritalStatus = reader["maritalstatus"] == DBNull.Value ? String.Empty : reader["maritalstatus"].ToString(),
                            BirthDay = reader["birthday"] == DBNull.Value ? 0 : (int)reader["birthday"],
                            BirthMonth = reader["birthmonth"] == DBNull.Value ? 0 : (int)reader["birthmonth"],
                            BirthYear = reader["birthyear"] == DBNull.Value ? 0 : (int)reader["birthyear"],
                            PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                            PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                            Email = reader["personal_email"] == DBNull.Value ? string.Empty : reader["personal_email"].ToString(),
                            Address = reader["address"] == DBNull.Value ? string.Empty : reader["address"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),

                            LocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            LocationType = reader["loctype"] == DBNull.Value ? string.Empty : reader["loctype"].ToString(),
                            LocationHead1 = reader["lochq1"] == DBNull.Value ? string.Empty : reader["lochq1"].ToString(),
                            LocationHead2 = reader["lochq2"] == DBNull.Value ? string.Empty : reader["lochq2"].ToString(),
                            LocationCountry = reader["locctr"] == DBNull.Value ? string.Empty : reader["locctr"].ToString(),
                            LocationState = reader["locst"] == DBNull.Value ? string.Empty : reader["locst"].ToString(),
                            CompanyName = reader["coy_name"] == DBNull.Value ? string.Empty : reader["coy_name"].ToString(),
                            DepartmentHead1 = reader["depthd1"] == DBNull.Value ? string.Empty : reader["depthd1"].ToString(),
                            DepartmentHead2 = reader["depthd2"] == DBNull.Value ? string.Empty : reader["depthd2"].ToString(),
                            DepartmentName = reader["deptname"] == DBNull.Value ? string.Empty : reader["deptname"].ToString(),
                            UnitHead1 = reader["unithd1"] == DBNull.Value ? string.Empty : reader["unithd1"].ToString(),
                            UnitHead2 = reader["unithd2"] == DBNull.Value ? string.Empty : reader["unithd2"].ToString(),
                            UnitName = reader["unitname"] == DBNull.Value ? string.Empty : reader["unitname"].ToString(),
                        });
                    }
                }
                await conn.CloseAsync();
            }
            return employeeList;

        }

        public async Task<long> AddAssignmentCrewMemberAsync(AssignmentCrewMember assignmentCrewMember)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_crw(assgn_id, emp_id, ");
            sb.Append("emp_rl1, unit_id, dept_id, loc_id, is_ld, emp_rl2, ");
            sb.Append("emp_rl3, mdb, mdt, ctb, ctt) ");
            sb.Append("VALUES (@assgn_id, @emp_id, @emp_rl1, @unit_id, ");
            sb.Append("@dept_id, @loc_id, @is_ld, @emp_rl2, @emp_rl3, @ctb, ");
            sb.Append("@ctt, @ctb, @ctt) RETURNING assg_crw_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var emp_rl1 = cmd.Parameters.Add("@emp_rl1", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var is_ld = cmd.Parameters.Add("@is_ld", NpgsqlDbType.Boolean);
                    var emp_rl2 = cmd.Parameters.Add("@emp_rl2", NpgsqlDbType.Text);
                    var emp_rl3 = cmd.Parameters.Add("@emp_rl3", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assgn_id.Value = assignmentCrewMember.AssignmentId;
                    emp_id.Value = assignmentCrewMember.CrewMemberId;
                    emp_rl1.Value = assignmentCrewMember.CrewMemberRole1;
                    unit_id.Value = assignmentCrewMember.UnitId;
                    dept_id.Value = assignmentCrewMember.DepartmentId;
                    loc_id.Value = assignmentCrewMember.LocationId;
                    is_ld.Value = assignmentCrewMember.IsTeamLead;
                    emp_rl2.Value = assignmentCrewMember.CrewMemberRole2 ?? (object)DBNull.Value;
                    emp_rl3.Value = assignmentCrewMember.CrewMemberRole3 ?? (object)DBNull.Value;
                    ctb.Value = assignmentCrewMember.ModifiedBy ?? (object)DBNull.Value;
                    ctt.Value = assignmentCrewMember.ModifiedTime ?? DateTime.Now;

                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentCrewMemberAsync(AssignmentCrewMember assignmentCrewMember)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_crw SET emp_id=@emp_id, emp_rl1=@emp_rl1, ");
            sb.Append("unit_id=@unit_id, dept_id=@dept_id, loc_id=@loc_id, emp_rl2=@emp_rl2, ");
            sb.Append("emp_rl3=@emp_rl3, mdb=@mdb, mdt=@mdt WHERE (assg_crw_id=@assg_crw_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_crw_id = cmd.Parameters.Add("@assg_crw_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var emp_rl1 = cmd.Parameters.Add("@emp_rl1", NpgsqlDbType.Text);
                    var unit_id = cmd.Parameters.Add("@unit_id", NpgsqlDbType.Integer);
                    var dept_id = cmd.Parameters.Add("@dept_id", NpgsqlDbType.Integer);
                    var loc_id = cmd.Parameters.Add("@loc_id", NpgsqlDbType.Integer);
                    var emp_rl2 = cmd.Parameters.Add("@emp_rl2", NpgsqlDbType.Text);
                    var emp_rl3 = cmd.Parameters.Add("@emp_rl3", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assg_crw_id.Value = assignmentCrewMember.Id;
                    emp_id.Value = assignmentCrewMember.CrewMemberId;
                    emp_rl1.Value = assignmentCrewMember.CrewMemberRole1;
                    unit_id.Value = assignmentCrewMember.UnitId;
                    dept_id.Value = assignmentCrewMember.DepartmentId;
                    loc_id.Value = assignmentCrewMember.LocationId;
                    emp_rl2.Value = assignmentCrewMember.CrewMemberRole2 ?? (object)DBNull.Value;
                    emp_rl3.Value = assignmentCrewMember.CrewMemberRole3 ?? (object)DBNull.Value;
                    mdb.Value = assignmentCrewMember.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentCrewMember.ModifiedTime ?? DateTime.Now;

                    inserted_row_id = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return inserted_row_id > 0;
        }
        public async Task<bool> UpdateAssignmentCrewLeadAsync(long assignmentCrewId, bool isLead, string updatedBy)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_crw SET is_ld=@is_ld, ");
            sb.Append("mdb=@mdb, mdt=@mdt ");
            sb.Append("WHERE (assg_crw_id=@assg_crw_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_crw_id = cmd.Parameters.Add("@assg_crw_id", NpgsqlDbType.Bigint);
                    var is_ld = cmd.Parameters.Add("@is_ld", NpgsqlDbType.Boolean);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assg_crw_id.Value = assignmentCrewId;
                    is_ld.Value = isLead;
                    mdb.Value = updatedBy;
                    mdt.Value = DateTime.Now;

                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id > 0;
        }
        public async Task<bool> DeleteAssignmentCrewMemberAsync(long assignmentCrewId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_crw WHERE (assg_crw_id=@assg_crw_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_crw_id = cmd.Parameters.Add("@assg_crw_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assg_crw_id.Value = assignmentCrewId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #region Assignment Crew Report Action Methods
        public async Task<AssignmentCrewReport> GetAssignmentCrewReportByIdAsync(long assignmentCrewReportId)
        {
            AssignmentCrewReport assignmentCrewReport = new AssignmentCrewReport();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT r.crw_rpt_id, assgn_id, r.emp_id, r.is_ld, r.att_sts, r.arrvl_typ, ");
            sb.Append("r.arrvl_time, r.depart_typ, r.depart_time, r.incidence, r.feedback, ");
            sb.Append("r.mdb, r.mdt, r.assg_crw_id, r.has_incid, r.has_fdbk, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = r.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_crw_rpt r ");
            sb.Append("WHERE (r.crw_rpt_id = @crw_rpt_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var crw_rpt_id = cmd.Parameters.Add("@crw_rpt_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    crw_rpt_id.Value = assignmentCrewReportId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentCrewReport.CrewReportId = reader["crw_rpt_id"] == DBNull.Value ? 0L : (long)reader["crw_rpt_id"];
                            assignmentCrewReport.AssignmentCrewId = reader["assg_crw_id"] == DBNull.Value ? 0L : (long)reader["assg_crw_id"];
                            assignmentCrewReport.AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"];
                            assignmentCrewReport.IsTeamLead = reader["is_ld"] == DBNull.Value ? false : (bool)reader["is_ld"];
                            assignmentCrewReport.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                            assignmentCrewReport.EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();
                            assignmentCrewReport.AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString();
                            assignmentCrewReport.AttendanceStatus = reader["att_sts"] == DBNull.Value ? string.Empty : reader["att_sts"].ToString();
                            assignmentCrewReport.ArrivalType = reader["arrvl_typ"] == DBNull.Value ? string.Empty : reader["arrvl_typ"].ToString();
                            assignmentCrewReport.ArrivalTime = reader["arrvl_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["arrvl_time"];
                            assignmentCrewReport.DepartureType = reader["depart_typ"] == DBNull.Value ? string.Empty : reader["depart_typ"].ToString();
                            assignmentCrewReport.DepartureTime = reader["depart_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["depart_time"];
                            assignmentCrewReport.HasIncidents = reader["has_incid"] == DBNull.Value ? false : (bool)reader["has_incid"];
                            assignmentCrewReport.IncidenceDetails = reader["incidence"] == DBNull.Value ? string.Empty : reader["incidence"].ToString();
                            assignmentCrewReport.HasFeedback = reader["has_fdbk"] == DBNull.Value ? false : (bool)reader["has_fdbk"];
                            assignmentCrewReport.FeedbackDetails = reader["feedback"] == DBNull.Value ? string.Empty : reader["feedback"].ToString();
                            assignmentCrewReport.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            assignmentCrewReport.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentCrewReport;
        }
        public async Task<List<AssignmentCrewReport>> GetAssignmentCrewReportsByAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentCrewReport> assignmentCrewReportList = new List<AssignmentCrewReport>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.crw_rpt_id, assgn_id, r.emp_id, r.is_ld, r.att_sts, r.arrvl_typ, ");
            sb.Append("r.arrvl_time, r.depart_typ, r.depart_time, r.incidence, r.feedback, ");
            sb.Append("r.mdb, r.mdt, r.assg_crw_id, r.has_incid, r.has_fdbk, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = r.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_crw_rpt r ");
            sb.Append("WHERE (r.assgn_id = @assgn_id) ORDER BY r.is_ld DESC, emp_nm ASC;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentCrewReportList.Add(new AssignmentCrewReport
                            {
                                CrewReportId = reader["crw_rpt_id"] == DBNull.Value ? 0L : (long)reader["crw_rpt_id"],
                                AssignmentCrewId = reader["assg_crw_id"] == DBNull.Value ? 0L : (long)reader["assg_crw_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                IsTeamLead = reader["is_ld"] == DBNull.Value ? false : (bool)reader["is_ld"],
                                EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                AttendanceStatus = reader["att_sts"] == DBNull.Value ? string.Empty : reader["att_sts"].ToString(),
                                ArrivalType = reader["arrvl_typ"] == DBNull.Value ? string.Empty : reader["arrvl_typ"].ToString(),
                                ArrivalTime = reader["arrvl_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["arrvl_time"],
                                DepartureType = reader["depart_typ"] == DBNull.Value ? string.Empty : reader["depart_typ"].ToString(),
                                DepartureTime = reader["depart_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["depart_time"],
                                HasIncidents = reader["has_incid"] == DBNull.Value ? false : (bool)reader["has_incid"],
                                HasFeedback = reader["has_fdbk"] == DBNull.Value ? false : (bool)reader["has_fdbk"],
                                IncidenceDetails = reader["incidence"] == DBNull.Value ? string.Empty : reader["incidence"].ToString(),
                                FeedbackDetails = reader["feedback"] == DBNull.Value ? string.Empty : reader["feedback"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentCrewReportList;
        }
        public async Task<List<AssignmentCrewReport>> GetAssignmentCrewReportsByAssignmentIdnEmployeeIdAsync(long assignmentId, string employeeId)
        {
            List<AssignmentCrewReport> assignmentCrewReportList = new List<AssignmentCrewReport>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT r.crw_rpt_id, assgn_id, r.emp_id, r.is_ld, r.att_sts, r.arrvl_typ, ");
            sb.Append("r.arrvl_time, r.depart_typ, r.depart_time, r.incidence, r.feedback, ");
            sb.Append("r.mdb, r.mdt, r.assg_crw_id, r.has_incid, r.has_fdbk, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = r.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = r.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_crw_rpt r ");
            sb.Append("WHERE (r.assgn_id = @assgn_id) AND (r.emp_id = @emp_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    emp_id.Value = employeeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentCrewReportList.Add(new AssignmentCrewReport
                            {
                                CrewReportId = reader["crw_rpt_id"] == DBNull.Value ? 0L : (long)reader["crw_rpt_id"],
                                AssignmentCrewId = reader["assg_crw_id"] == DBNull.Value ? 0L : (long)reader["assg_crw_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                IsTeamLead = reader["is_ld"] == DBNull.Value ? false : (bool)reader["is_ld"],
                                EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                AttendanceStatus = reader["att_sts"] == DBNull.Value ? string.Empty : reader["att_sts"].ToString(),
                                ArrivalType = reader["arrvl_typ"] == DBNull.Value ? string.Empty : reader["arrvl_typ"].ToString(),
                                ArrivalTime = reader["arrvl_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["arrvl_time"],
                                DepartureType = reader["depart_typ"] == DBNull.Value ? string.Empty : reader["depart_typ"].ToString(),
                                DepartureTime = reader["depart_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["depart_time"],
                                HasIncidents = reader["has_incid"] == DBNull.Value ? false : (bool)reader["has_incid"],
                                HasFeedback = reader["has_fdbk"] == DBNull.Value ? false : (bool)reader["has_fdbk"],
                                IncidenceDetails = reader["incidence"] == DBNull.Value ? string.Empty : reader["incidence"].ToString(),
                                FeedbackDetails = reader["feedback"] == DBNull.Value ? string.Empty : reader["feedback"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentCrewReportList;
        }
        public async Task<long> AddAssignmentCrewReportAsync(AssignmentCrewReport assignmentCrewReport)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_crw_rpt(assgn_id, emp_id, is_ld, ");
            sb.Append("att_sts, arrvl_typ, arrvl_time, depart_typ, depart_time, ");
            sb.Append("incidence, feedback, mdb, mdt, assg_crw_id, has_incid, has_fdbk) ");
            sb.Append("VALUES (@assgn_id, @emp_id, @is_ld, @att_sts, @arrvl_typ, ");
            sb.Append("@arrvl_time, @depart_typ, @depart_time, @incidence, @feedback, ");
            sb.Append("@mdb, @mdt, @assg_crw_id, @has_incid, @has_fdbk) ");
            sb.Append("RETURNING crw_rpt_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_crw_id = cmd.Parameters.Add("@assg_crw_id", NpgsqlDbType.Bigint);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var is_ld = cmd.Parameters.Add("@is_ld", NpgsqlDbType.Boolean);
                    var att_sts = cmd.Parameters.Add("@att_sts", NpgsqlDbType.Text);
                    var arrvl_typ = cmd.Parameters.Add("@arrvl_typ", NpgsqlDbType.Text);
                    var arrvl_time = cmd.Parameters.Add("@arrvl_time", NpgsqlDbType.Timestamp);
                    var depart_typ = cmd.Parameters.Add("@depart_typ", NpgsqlDbType.Text);
                    var depart_time = cmd.Parameters.Add("@depart_time", NpgsqlDbType.Timestamp);
                    var incidence = cmd.Parameters.Add("@incidence", NpgsqlDbType.Text);
                    var feedback = cmd.Parameters.Add("@feedback", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    var has_incid = cmd.Parameters.Add("@has_incid", NpgsqlDbType.Boolean);
                    var has_fdbk = cmd.Parameters.Add("@has_fdbk", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    assg_crw_id.Value = assignmentCrewReport.AssignmentCrewId;
                    assgn_id.Value = assignmentCrewReport.AssignmentId;
                    emp_id.Value = assignmentCrewReport.EmployeeId;
                    is_ld.Value = assignmentCrewReport.IsTeamLead;
                    att_sts.Value = assignmentCrewReport.AttendanceStatus ?? (object)DBNull.Value;
                    arrvl_typ.Value = assignmentCrewReport.ArrivalType ?? (object)DBNull.Value;
                    arrvl_time.Value = assignmentCrewReport.ArrivalTime ?? (object)DBNull.Value;
                    depart_typ.Value = assignmentCrewReport.DepartureType ?? (object)DBNull.Value;
                    depart_time.Value = assignmentCrewReport.DepartureTime ?? (object)DBNull.Value;
                    incidence.Value = assignmentCrewReport.IncidenceDetails ?? (object)DBNull.Value;
                    feedback.Value = assignmentCrewReport.FeedbackDetails ?? (object)DBNull.Value;
                    mdb.Value = assignmentCrewReport.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentCrewReport.ModifiedTime ?? DateTime.Now;
                    has_incid.Value = assignmentCrewReport.HasIncidents;
                    has_fdbk.Value = assignmentCrewReport.HasFeedback;

                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentCrewReportAsync(AssignmentCrewReport assignmentCrewReport)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.ats_crw_rpt SET att_sts=@att_sts, arrvl_typ=@arrvl_typ, ");
            sb.Append("arrvl_time=@arrvl_time, depart_typ=@depart_typ, depart_time=@depart_time, ");
            sb.Append("incidence=@incidence, feedback=@feedback, mdb=@mdb, mdt=@mdt, ");
            sb.Append("has_incid=@has_incid, has_fdbk=@has_fdbk ");
            sb.Append("WHERE(crw_rpt_id=@crw_rpt_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var crw_rpt_id = cmd.Parameters.Add("@crw_rpt_id", NpgsqlDbType.Bigint);
                    var att_sts = cmd.Parameters.Add("@att_sts", NpgsqlDbType.Text);
                    var arrvl_typ = cmd.Parameters.Add("@arrvl_typ", NpgsqlDbType.Text);
                    var arrvl_time = cmd.Parameters.Add("@arrvl_time", NpgsqlDbType.Timestamp);
                    var depart_typ = cmd.Parameters.Add("@depart_typ", NpgsqlDbType.Text);
                    var depart_time = cmd.Parameters.Add("@depart_time", NpgsqlDbType.Timestamp);
                    var incidence = cmd.Parameters.Add("@incidence", NpgsqlDbType.Text);
                    var feedback = cmd.Parameters.Add("@feedback", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    var has_incid = cmd.Parameters.Add("@has_incid", NpgsqlDbType.Boolean);
                    var has_fdbk = cmd.Parameters.Add("@has_fdbk", NpgsqlDbType.Boolean);
                    cmd.Prepare();
                    crw_rpt_id.Value = assignmentCrewReport.CrewReportId;
                    att_sts.Value = assignmentCrewReport.AttendanceStatus ?? (object)DBNull.Value;
                    arrvl_typ.Value = assignmentCrewReport.ArrivalType ?? (object)DBNull.Value;
                    arrvl_time.Value = assignmentCrewReport.ArrivalTime ?? (object)DBNull.Value;
                    depart_typ.Value = assignmentCrewReport.DepartureType ?? (object)DBNull.Value;
                    depart_time.Value = assignmentCrewReport.DepartureTime ?? (object)DBNull.Value;
                    incidence.Value = assignmentCrewReport.IncidenceDetails ?? (object)DBNull.Value;
                    feedback.Value = assignmentCrewReport.FeedbackDetails ?? (object)DBNull.Value;
                    mdb.Value = assignmentCrewReport.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentCrewReport.ModifiedTime ?? DateTime.Now;
                    has_incid.Value = assignmentCrewReport.HasIncidents;
                    has_fdbk.Value = assignmentCrewReport.HasFeedback;

                    inserted_row_id = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return inserted_row_id > 0;
        }
        public async Task<bool> DeleteAssignmentCrewReportAsync(long assignmentCrewReportId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_crw_rpt WHERE (crw_rpt_id=@crw_rpt_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var crw_rpt_id = cmd.Parameters.Add("@crw_rpt_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    crw_rpt_id.Value = assignmentCrewReportId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #region Assignment Editing Report Action Methods
        public async Task<AssignmentEngReport> GetAssignmentEngReportByIdAsync(long assignmentEngReportId)
        {
            AssignmentEngReport assignmentEngReport = new AssignmentEngReport();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.eng_rpt_id, e.assgn_id, e.emp_id, e.aud_qlt, e.vid_qlt, ");
            sb.Append("e.scr_avl, e.mat_avl, e.rpt_avl, e.rpt_arrv_time, e.edt_start_time, ");
            sb.Append("e.edt_end_time, e.edt_sts, e.feedback, e.mdb, e.mdt, ");
            sb.Append("CASE WHEN e.aud_qlt = 3 THEN 'Good' WHEN aud_qlt =  2 THEN 'Fair' ");
            sb.Append("WHEN aud_qlt = 1 THEN 'Poor' ELSE 'None' END AS aud_qlt_ds, ");
            sb.Append("CASE WHEN e.vid_qlt = 3 THEN 'Good' WHEN vid_qlt =  2 THEN 'Fair' ");
            sb.Append("WHEN vid_qlt = 1 THEN 'Poor' ELSE 'None' END AS vid_qlt_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = e.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_eng_rpt e ");
            sb.Append("WHERE (e.eng_rpt_id = @eng_rpt_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var eng_rpt_id = cmd.Parameters.Add("@eng_rpt_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    eng_rpt_id.Value = assignmentEngReportId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEngReport.EngReportId = reader["eng_rpt_id"] == DBNull.Value ? 0L : (long)reader["eng_rpt_id"];
                            assignmentEngReport.AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"];
                            assignmentEngReport.AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString();
                            assignmentEngReport.EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString();
                            assignmentEngReport.EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString();
                            assignmentEngReport.AudioQuality = reader["aud_qlt"] == DBNull.Value ? 0 : (int)reader["aud_qlt"];
                            assignmentEngReport.AudioQualityDescription = reader["aud_qlt_ds"] == DBNull.Value ? string.Empty : reader["aud_qlt_ds"].ToString();
                            assignmentEngReport.VideoQuality = reader["vid_qlt"] == DBNull.Value ? 0 : (int)reader["vid_qlt"];
                            assignmentEngReport.VideoQualityDescription = reader["vid_qlt_ds"] == DBNull.Value ? string.Empty : reader["vid_qlt_ds"].ToString();
                            assignmentEngReport.ScriptIsAvailable = reader["scr_avl"] == DBNull.Value ? false : (bool)reader["scr_avl"];
                            assignmentEngReport.MaterialsAreAvailable = reader["mat_avl"] == DBNull.Value ? false : (bool)reader["mat_avl"];
                            assignmentEngReport.ReporterIsAvailable = reader["rpt_avl"] == DBNull.Value ? false : (bool)reader["rpt_avl"];
                            assignmentEngReport.ReporterArrivalTime = reader["rpt_arrv_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_arrv_time"];
                            assignmentEngReport.EditingStartTime = reader["edt_start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["edt_start_time"];
                            assignmentEngReport.EditingEndTime = reader["edt_end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["edt_end_time"];
                            assignmentEngReport.EditingStatus = reader["edt_sts"] == DBNull.Value ? string.Empty : reader["edt_sts"].ToString();
                            assignmentEngReport.Feedback = reader["feedback"] == DBNull.Value ? string.Empty : reader["feedback"].ToString();
                            assignmentEngReport.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            assignmentEngReport.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEngReport;
        }
        public async Task<List<AssignmentEngReport>> GetAssignmentEngReportsByAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentEngReport> assignmentEngReportList = new List<AssignmentEngReport>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT e.eng_rpt_id, e.assgn_id, e.emp_id, e.aud_qlt, e.vid_qlt, ");
            sb.Append("e.scr_avl, e.mat_avl, e.rpt_avl, e.rpt_arrv_time, e.edt_start_time, ");
            sb.Append("e.edt_end_time, e.edt_sts, e.feedback, e.mdb, e.mdt, ");
            sb.Append("CASE WHEN e.aud_qlt = 3 THEN 'Good' WHEN aud_qlt =  2 THEN 'Fair' ");
            sb.Append("WHEN aud_qlt = 1 THEN 'Poor' ELSE 'None' END AS aud_qlt_ds, ");
            sb.Append("CASE WHEN e.vid_qlt = 3 THEN 'Good' WHEN vid_qlt =  2 THEN 'Fair' ");
            sb.Append("WHEN vid_qlt = 1 THEN 'Poor' ELSE 'None' END AS vid_qlt_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = e.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_eng_rpt e ");
            sb.Append("WHERE (e.assgn_id = @assgn_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEngReportList.Add(new AssignmentEngReport
                            {
                                EngReportId = reader["eng_rpt_id"] == DBNull.Value ? 0L : (long)reader["eng_rpt_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                AudioQuality = reader["aud_qlt"] == DBNull.Value ? 0 : (int)reader["aud_qlt"],
                                AudioQualityDescription = reader["aud_qlt_ds"] == DBNull.Value ? string.Empty : reader["aud_qlt_ds"].ToString(),
                                VideoQuality = reader["vid_qlt"] == DBNull.Value ? 0 : (int)reader["vid_qlt"],
                                VideoQualityDescription = reader["vid_qlt_ds"] == DBNull.Value ? string.Empty : reader["vid_qlt_ds"].ToString(),
                                ScriptIsAvailable = reader["scr_avl"] == DBNull.Value ? false : (bool)reader["scr_avl"],
                                MaterialsAreAvailable = reader["mat_avl"] == DBNull.Value ? false : (bool)reader["mat_avl"],
                                ReporterIsAvailable = reader["rpt_avl"] == DBNull.Value ? false : (bool)reader["rpt_avl"],
                                ReporterArrivalTime = reader["rpt_arrv_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_arrv_time"],
                                EditingStartTime = reader["edt_start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["edt_start_time"],
                                EditingEndTime = reader["edt_end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["edt_end_time"],
                                EditingStatus = reader["edt_sts"] == DBNull.Value ? string.Empty : reader["edt_sts"].ToString(),
                                Feedback = reader["feedback"] == DBNull.Value ? string.Empty : reader["feedback"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEngReportList;
        }
        public async Task<List<AssignmentEngReport>> GetAssignmentEngReportsByAssignmentIdnEmployeeIdAsync(long assignmentId, string employeeId)
        {
            List<AssignmentEngReport> assignmentEngReportList = new List<AssignmentEngReport>();
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT e.eng_rpt_id, e.assgn_id, e.emp_id, e.aud_qlt, e.vid_qlt, ");
            sb.Append("e.scr_avl, e.mat_avl, e.rpt_avl, e.rpt_arrv_time, e.edt_start_time, ");
            sb.Append("e.edt_end_time, e.edt_sts, e.feedback, e.mdb, e.mdt, ");
            sb.Append("CASE WHEN e.aud_qlt = 3 THEN 'Good' WHEN aud_qlt =  2 THEN 'Fair' ");
            sb.Append("WHEN aud_qlt = 1 THEN 'Poor' ELSE 'None' END AS aud_qlt_ds, ");
            sb.Append("CASE WHEN e.vid_qlt = 3 THEN 'Good' WHEN vid_qlt =  2 THEN 'Fair' ");
            sb.Append("WHEN vid_qlt = 1 THEN 'Poor' ELSE 'None' END AS vid_qlt_ds, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = e.emp_id ) as emp_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = e.assgn_id) as assgn_tl ");
            sb.Append("FROM public.ats_eng_rpt e ");
            sb.Append("WHERE (e.assgn_id = @assgn_id) AND (e.emp_id = @emp_id); ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    emp_id.Value = employeeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEngReportList.Add(new AssignmentEngReport
                            {
                                EngReportId = reader["eng_rpt_id"] == DBNull.Value ? 0L : (long)reader["eng_rpt_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                EmployeeId = reader["emp_id"] == DBNull.Value ? string.Empty : reader["emp_id"].ToString(),
                                EmployeeName = reader["emp_nm"] == DBNull.Value ? string.Empty : reader["emp_nm"].ToString(),
                                AudioQuality = reader["aud_qlt"] == DBNull.Value ? 0 : (int)reader["aud_qlt"],
                                AudioQualityDescription = reader["aud_qlt_ds"] == DBNull.Value ? string.Empty : reader["aud_qlt_ds"].ToString(),
                                VideoQuality = reader["vid_qlt"] == DBNull.Value ? 0 : (int)reader["vid_qlt"],
                                VideoQualityDescription = reader["vid_qlt_ds"] == DBNull.Value ? string.Empty : reader["vid_qlt_ds"].ToString(),
                                ScriptIsAvailable = reader["scr_avl"] == DBNull.Value ? false : (bool)reader["scr_avl"],
                                MaterialsAreAvailable = reader["mat_avl"] == DBNull.Value ? false : (bool)reader["mat_avl"],
                                ReporterIsAvailable = reader["rpt_avl"] == DBNull.Value ? false : (bool)reader["rpt_avl"],
                                ReporterArrivalTime = reader["rpt_arrv_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["rpt_arrv_time"],
                                EditingStartTime = reader["edt_start_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["edt_start_time"],
                                EditingEndTime = reader["edt_end_time"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["edt_end_time"],
                                EditingStatus = reader["edt_sts"] == DBNull.Value ? string.Empty : reader["edt_sts"].ToString(),
                                Feedback = reader["feedback"] == DBNull.Value ? string.Empty : reader["feedback"].ToString(),
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEngReportList;
        }
        public async Task<long> AddAssignmentEngReportAsync(AssignmentEngReport assignmentEngReport)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_eng_rpt(assgn_id, emp_id, aud_qlt, ");
            sb.Append("vid_qlt, scr_avl, mat_avl, rpt_avl, rpt_arrv_time, ");
            sb.Append("edt_start_time, edt_end_time, edt_sts, feedback, mdb, mdt) ");
            sb.Append("VALUES(@assgn_id, @emp_id, @aud_qlt, @vid_qlt, @scr_avl, ");
            sb.Append("@mat_avl, @rpt_avl, @rpt_arrv_time, @edt_start_time, ");
            sb.Append("@edt_end_time, @edt_sts, @feedback, @mdb, @mdt) ");
            sb.Append("RETURNING eng_rpt_id; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    var aud_qlt = cmd.Parameters.Add("@aud_qlt", NpgsqlDbType.Integer);
                    var vid_qlt = cmd.Parameters.Add("@vid_qlt", NpgsqlDbType.Integer);
                    var scr_avl = cmd.Parameters.Add("@scr_avl", NpgsqlDbType.Boolean);
                    var mat_avl = cmd.Parameters.Add("@mat_avl", NpgsqlDbType.Boolean);
                    var rpt_avl = cmd.Parameters.Add("@rpt_avl", NpgsqlDbType.Boolean);
                    var rpt_arrv_time = cmd.Parameters.Add("@rpt_arrv_time", NpgsqlDbType.Timestamp);
                    var edt_start_time = cmd.Parameters.Add("@edt_start_time", NpgsqlDbType.Timestamp);
                    var edt_end_time = cmd.Parameters.Add("@edt_end_time", NpgsqlDbType.Timestamp);
                    var edt_sts = cmd.Parameters.Add("@edt_sts", NpgsqlDbType.Text);
                    var feedback = cmd.Parameters.Add("@feedback", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assgn_id.Value = assignmentEngReport.AssignmentId;
                    emp_id.Value = assignmentEngReport.EmployeeId;
                    aud_qlt.Value = assignmentEngReport.AudioQuality;
                    vid_qlt.Value = assignmentEngReport.VideoQuality;
                    scr_avl.Value = assignmentEngReport.ScriptIsAvailable;
                    mat_avl.Value = assignmentEngReport.MaterialsAreAvailable;
                    rpt_avl.Value = assignmentEngReport.ReporterIsAvailable;
                    rpt_arrv_time.Value = assignmentEngReport.ReporterArrivalTime ?? (object)DBNull.Value;
                    edt_start_time.Value = assignmentEngReport.EditingStartTime ?? (object)DBNull.Value;
                    edt_end_time.Value = assignmentEngReport.EditingEndTime ?? (object)DBNull.Value;
                    edt_sts.Value = assignmentEngReport.EditingStatus ?? (object)DBNull.Value;
                    feedback.Value = assignmentEngReport.Feedback ?? (object)DBNull.Value;
                    mdb.Value = assignmentEngReport.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentEngReport.ModifiedTime ?? DateTime.Now;

                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentEngReportAsync(AssignmentEngReport assignmentEngReport)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE public.ats_eng_rpt SET aud_qlt=@aud_qlt, ");
            sb.Append("vid_qlt=@vid_qlt, scr_avl=@scr_avl, mat_avl=@mat_avl, ");
            sb.Append("rpt_avl=@rpt_avl, rpt_arrv_time=@rpt_arrv_time, ");
            sb.Append("edt_start_time=@edt_start_time, edt_end_time=@edt_end_time, ");
            sb.Append("edt_sts=@edt_sts, feedback=@feedback, mdb=@mdb, mdt=@mdt ");
            sb.Append("WHERE(eng_rpt_id=@eng_rpt_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eng_rpt_id = cmd.Parameters.Add("@eng_rpt_id", NpgsqlDbType.Bigint);
                    var aud_qlt = cmd.Parameters.Add("@aud_qlt", NpgsqlDbType.Integer);
                    var vid_qlt = cmd.Parameters.Add("@vid_qlt", NpgsqlDbType.Integer);
                    var scr_avl = cmd.Parameters.Add("@scr_avl", NpgsqlDbType.Boolean);
                    var mat_avl = cmd.Parameters.Add("@mat_avl", NpgsqlDbType.Boolean);
                    var rpt_avl = cmd.Parameters.Add("@rpt_avl", NpgsqlDbType.Boolean);
                    var rpt_arrv_time = cmd.Parameters.Add("@rpt_arrv_time", NpgsqlDbType.Timestamp);
                    var edt_start_time = cmd.Parameters.Add("@edt_start_time", NpgsqlDbType.Timestamp);
                    var edt_end_time = cmd.Parameters.Add("@edt_end_time", NpgsqlDbType.Timestamp);
                    var edt_sts = cmd.Parameters.Add("@edt_sts", NpgsqlDbType.Text);
                    var feedback = cmd.Parameters.Add("@feedback", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    eng_rpt_id.Value = assignmentEngReport.EngReportId;
                    aud_qlt.Value = assignmentEngReport.AudioQuality;
                    vid_qlt.Value = assignmentEngReport.VideoQuality;
                    scr_avl.Value = assignmentEngReport.ScriptIsAvailable;
                    mat_avl.Value = assignmentEngReport.MaterialsAreAvailable;
                    rpt_avl.Value = assignmentEngReport.ReporterIsAvailable;
                    rpt_arrv_time.Value = assignmentEngReport.ReporterArrivalTime ?? (object)DBNull.Value;
                    edt_start_time.Value = assignmentEngReport.EditingStartTime ?? (object)DBNull.Value;
                    edt_end_time.Value = assignmentEngReport.EditingEndTime ?? (object)DBNull.Value;
                    edt_sts.Value = assignmentEngReport.EditingStatus ?? (object)DBNull.Value;
                    feedback.Value = assignmentEngReport.Feedback ?? (object)DBNull.Value;
                    mdb.Value = assignmentEngReport.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentEngReport.ModifiedTime ?? DateTime.Now;

                    inserted_row_id = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return inserted_row_id > 0;
        }
        public async Task<bool> DeleteAssignmentEngReportAsync(long assignmentEngReportId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_eng_rpt WHERE (eng_rpt_id=@eng_rpt_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var eng_rpt_id = cmd.Parameters.Add("@eng_rpt_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    eng_rpt_id.Value = assignmentEngReportId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion


        #region Assignment Equipment Action Methods
        public async Task<AssignmentEquipment> GetAssignmentEquipmentByIdAsync(long assignmentEquipmentId)
        {
            AssignmentEquipment assignmentEquipment = new AssignmentEquipment();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.assg_eqmt_id, q.assgn_id, q.asset_id, q.mdb, q.mdt, q.asst_typ_id, ");
            sb.Append("q.asst_grp_id, q.asst_clss_id, q.assg_by_emp_id, q.assg_to_emp_id, q.asst_ctg_id, ");
            sb.Append("k.asst_ctgs_nm, c.clss_nm, g.grp_nm, t.typ_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = q.assgn_id) as assgn_tl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_to_emp_id ) as assg_to_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_by_emp_id ) as assg_by_emp_nm, ");
            sb.Append("(SELECT asst_nm FROM public.asm_stt_asst WHERE asst_id = q.asset_id) as asset_nm ");
            sb.Append("FROM public.ats_assg_eqmt q ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_ctgs k ON k.asst_ctgs_id = q.asst_ctg_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_clss c ON c.clss_id = q.asst_clss_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_grps g ON g.grp_id = q.asst_grp_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_typs t ON t.typ_id = q.asst_typ_id ");
            sb.Append("WHERE (q.assg_eqmt_id = @assg_eqmt_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_eqmt_id = cmd.Parameters.Add("@assg_eqmt_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assg_eqmt_id.Value = assignmentEquipmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEquipment.AssignmentEquipmentId = reader["assg_eqmt_id"] == DBNull.Value ? 0L : (long)reader["assg_eqmt_id"];
                            assignmentEquipment.AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"];
                            assignmentEquipment.AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString();
                            assignmentEquipment.AssetId = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString();
                            assignmentEquipment.AssetName = reader["asset_nm"] == DBNull.Value ? string.Empty : reader["asset_nm"].ToString();
                            assignmentEquipment.AssetTypeId = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"];
                            assignmentEquipment.AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString();

                            assignmentEquipment.AssetGroupId = reader["asst_grp_id"] == DBNull.Value ? 0 : (int)reader["asst_grp_id"];
                            assignmentEquipment.AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString();

                            assignmentEquipment.AssetClassId = reader["asst_clss_id"] == DBNull.Value ? 0 : (int)reader["asst_clss_id"];
                            assignmentEquipment.AssetClassName = reader["clss_nm"] == DBNull.Value ? string.Empty : reader["clss_nm"].ToString();

                            assignmentEquipment.AssetCategoryId = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"];
                            assignmentEquipment.AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString();

                            assignmentEquipment.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            assignmentEquipment.ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"];
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEquipment;
        }
        public async Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssignmentIdnAssetTypeNameAsync(long assignmentId, string assetTypeName)
        {
            List<AssignmentEquipment> assignmentEquipmentList = new List<AssignmentEquipment>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.assg_eqmt_id, q.assgn_id, q.asset_id, q.mdb, q.mdt, q.asst_typ_id, ");
            sb.Append("q.asst_grp_id, q.asst_clss_id, q.assg_by_emp_id, q.assg_to_emp_id, q.asst_ctg_id, ");
            sb.Append("k.asst_ctgs_nm, c.clss_nm, g.grp_nm, t.typ_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = q.assgn_id) as assgn_tl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_to_emp_id ) as assg_to_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_by_emp_id ) as assg_by_emp_nm, ");
            sb.Append("(SELECT asst_nm FROM public.asm_stt_asst WHERE asst_id = q.asset_id) as asset_nm ");
            sb.Append("FROM public.ats_assg_eqmt q ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_ctgs k ON k.asst_ctgs_id = q.asst_ctg_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_clss c ON c.clss_id = q.asst_clss_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_grps g ON g.grp_id = q.asst_grp_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_typs t ON t.typ_id = q.asst_typ_id ");
            sb.Append("WHERE (q.assgn_id = @assgn_id) AND (t.typ_nm = @typ_nm) ");
            sb.Append("ORDER BY asset_nm; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var typ_nm = cmd.Parameters.Add("@typ_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    typ_nm.Value = assetTypeName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEquipmentList.Add(new AssignmentEquipment
                            {
                                AssignmentEquipmentId = reader["assg_eqmt_id"] == DBNull.Value ? 0L : (long)reader["assg_eqmt_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                AssetId = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asset_nm"] == DBNull.Value ? string.Empty : reader["asset_nm"].ToString(),
                                AssetTypeId = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),

                                AssetGroupId = reader["asst_grp_id"] == DBNull.Value ? 0 : (int)reader["asst_grp_id"],
                                AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),

                                AssetClassId = reader["asst_clss_id"] == DBNull.Value ? 0 : (int)reader["asst_clss_id"],
                                AssetClassName = reader["clss_nm"] == DBNull.Value ? string.Empty : reader["clss_nm"].ToString(),

                                AssetCategoryId = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),

                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEquipmentList;
        }
        public async Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssignmentIdnAssetClassIdAsync(long assignmentId, int assetClassId)
        {
            List<AssignmentEquipment> assignmentEquipmentList = new List<AssignmentEquipment>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.assg_eqmt_id, q.assgn_id, q.asset_id, q.mdb, q.mdt, q.asst_typ_id, ");
            sb.Append("q.asst_grp_id, q.asst_clss_id, q.assg_by_emp_id, q.assg_to_emp_id, q.asst_ctg_id, ");
            sb.Append("k.asst_ctgs_nm, c.clss_nm, g.grp_nm, t.typ_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = q.assgn_id) as assgn_tl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_to_emp_id ) as assg_to_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_by_emp_id ) as assg_by_emp_nm, ");
            sb.Append("(SELECT asst_nm FROM public.asm_stt_asst WHERE asst_id = q.asset_id) as asset_nm ");
            sb.Append("FROM public.ats_assg_eqmt q ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_ctgs k ON k.asst_ctgs_id = q.asst_ctg_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_clss c ON c.clss_id = q.asst_clss_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_grps g ON g.grp_id = q.asst_grp_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_typs t ON t.typ_id = q.asst_typ_id ");
            sb.Append("WHERE (q.assgn_id = @assgn_id) AND (q.asst_clss_id = @asst_clss_id) ");
            sb.Append("ORDER BY asset_nm; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var asst_clss_id = cmd.Parameters.Add("@asst_clss_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    asst_clss_id.Value = assetClassId;

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEquipmentList.Add(new AssignmentEquipment
                            {
                                AssignmentEquipmentId = reader["assg_eqmt_id"] == DBNull.Value ? 0L : (long)reader["assg_eqmt_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                AssetId = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asset_nm"] == DBNull.Value ? string.Empty : reader["asset_nm"].ToString(),
                                AssetTypeId = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),

                                AssetGroupId = reader["asst_grp_id"] == DBNull.Value ? 0 : (int)reader["asst_grp_id"],
                                AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),

                                AssetClassId = reader["asst_clss_id"] == DBNull.Value ? 0 : (int)reader["asst_clss_id"],
                                AssetClassName = reader["clss_nm"] == DBNull.Value ? string.Empty : reader["clss_nm"].ToString(),

                                AssetCategoryId = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),

                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEquipmentList;
        }
        public async Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentEquipment> assignmentEquipmentList = new List<AssignmentEquipment>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.assg_eqmt_id, q.assgn_id, q.asset_id, q.mdb, q.mdt, q.asst_typ_id, ");
            sb.Append("q.asst_grp_id, q.asst_clss_id, q.assg_by_emp_id, q.assg_to_emp_id, q.asst_ctg_id, ");
            sb.Append("k.asst_ctgs_nm, c.clss_nm, g.grp_nm, t.typ_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = q.assgn_id) as assgn_tl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_to_emp_id ) as assg_to_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_by_emp_id ) as assg_by_emp_nm, ");
            sb.Append("(SELECT asst_nm FROM public.asm_stt_asst WHERE asst_id = q.asset_id) as asset_nm ");
            sb.Append("FROM public.ats_assg_eqmt q ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_ctgs k ON k.asst_ctgs_id = q.asst_ctg_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_clss c ON c.clss_id = q.asst_clss_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_grps g ON g.grp_id = q.asst_grp_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_typs t ON t.typ_id = q.asst_typ_id ");
            sb.Append("WHERE (q.assgn_id = @assgn_id) ");
            sb.Append("ORDER BY asset_nm; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEquipmentList.Add(new AssignmentEquipment
                            {
                                AssignmentEquipmentId = reader["assg_eqmt_id"] == DBNull.Value ? 0L : (long)reader["assg_eqmt_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),

                                AssignedByEmployeeId = reader["assg_by_emp_id"] == DBNull.Value ? string.Empty : reader["assg_by_emp_id"].ToString(),
                                AssignedByEmployeeName = reader["assg_by_emp_nm"] == DBNull.Value ? string.Empty : reader["assg_by_emp_nm"].ToString(),

                                AssignedToEmployeeId = reader["assg_to_emp_id"] == DBNull.Value ? string.Empty : reader["assg_to_emp_id"].ToString(),
                                AssignedToEmployeeName = reader["assg_to_emp_nm"] == DBNull.Value ? string.Empty : reader["assg_to_emp_nm"].ToString(),

                                AssetId = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asset_nm"] == DBNull.Value ? string.Empty : reader["asset_nm"].ToString(),
                                AssetTypeId = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),

                                AssetGroupId = reader["asst_grp_id"] == DBNull.Value ? 0 : (int)reader["asst_grp_id"],
                                AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),

                                AssetClassId = reader["asst_clss_id"] == DBNull.Value ? 0 : (int)reader["asst_clss_id"],
                                AssetClassName = reader["clss_nm"] == DBNull.Value ? string.Empty : reader["clss_nm"].ToString(),

                                AssetCategoryId = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),

                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEquipmentList;
        }
        public async Task<List<AssignmentEquipment>> GetAssignmentEquipmentByAssetIdAsync(string assetId)
        {
            List<AssignmentEquipment> assignmentEquipmentList = new List<AssignmentEquipment>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT q.assg_eqmt_id, q.assgn_id, q.asset_id, q.mdb, q.mdt, q.asst_typ_id, ");
            sb.Append("q.asst_grp_id, q.asst_clss_id, q.assg_by_emp_id, q.assg_to_emp_id, q.asst_ctg_id, ");
            sb.Append("k.asst_ctgs_nm, c.clss_nm, g.grp_nm, t.typ_nm, ");
            sb.Append("(SELECT assgn_tl FROM public.ats_assg_inf WHERE assgn_id = q.assgn_id) as assgn_tl, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_to_emp_id ) as assg_to_emp_nm, ");
            sb.Append("(SELECT fullname FROM public.gst_prsns WHERE id = q.assg_by_emp_id ) as assg_by_emp_nm, ");
            sb.Append("(SELECT asst_nm FROM public.asm_stt_asst WHERE asst_id = q.asset_id) as asset_nm ");
            sb.Append("FROM public.ats_assg_eqmt q ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_ctgs k ON k.asst_ctgs_id = q.asst_ctg_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_clss c ON c.clss_id = q.asst_clss_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_grps g ON g.grp_id = q.asst_grp_id ");
            sb.Append("LEFT OUTER JOIN public.asm_stt_typs t ON t.typ_id = q.asst_typ_id ");
            sb.Append("WHERE (q.asset_id = @asset_id) ");
            sb.Append("ORDER BY asset_nm; ");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    asset_id.Value = assetId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEquipmentList.Add(new AssignmentEquipment
                            {
                                AssignmentEquipmentId = reader["assg_eqmt_id"] == DBNull.Value ? 0L : (long)reader["assg_eqmt_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                AssignmentTitle = reader["assgn_tl"] == DBNull.Value ? string.Empty : reader["assgn_tl"].ToString(),
                                AssetId = reader["asset_id"] == DBNull.Value ? string.Empty : reader["asset_id"].ToString(),
                                AssetName = reader["asset_nm"] == DBNull.Value ? string.Empty : reader["asset_nm"].ToString(),
                                AssetTypeId = reader["asst_typ_id"] == DBNull.Value ? 0 : (int)reader["asst_typ_id"],
                                AssetTypeName = reader["typ_nm"] == DBNull.Value ? string.Empty : reader["typ_nm"].ToString(),

                                AssetGroupId = reader["asst_grp_id"] == DBNull.Value ? 0 : (int)reader["asst_grp_id"],
                                AssetGroupName = reader["grp_nm"] == DBNull.Value ? string.Empty : reader["grp_nm"].ToString(),

                                AssetClassId = reader["asst_clss_id"] == DBNull.Value ? 0 : (int)reader["asst_clss_id"],
                                AssetClassName = reader["clss_nm"] == DBNull.Value ? string.Empty : reader["clss_nm"].ToString(),

                                AssetCategoryId = reader["asst_ctg_id"] == DBNull.Value ? 0 : (int)reader["asst_ctg_id"],
                                AssetCategoryName = reader["asst_ctgs_nm"] == DBNull.Value ? string.Empty : reader["asst_ctgs_nm"].ToString(),

                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mdt"],
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEquipmentList;
        }

        public async Task<long> AddAssignmentEquipmentAsync(AssignmentEquipment assignmentEquipment)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_eqmt(assgn_id, asset_id, mdb, ");
            sb.Append("mdt, asst_typ_id, asst_grp_id, asst_clss_id, assg_by_emp_id, ");
            sb.Append("assg_to_emp_id, asst_ctg_id) VALUES (@assgn_id, @asset_id, ");
            sb.Append("@mdb, @mdt, @asst_typ_id, @asst_grp_id, @asst_clss_id, ");
            sb.Append("@assg_by_emp_id, @assg_to_emp_id, @asst_ctg_id) ");
            sb.Append("RETURNING assg_eqmt_id;");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);

                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_grp_id = cmd.Parameters.Add("@asst_grp_id", NpgsqlDbType.Integer); 
                    var asst_clss_id = cmd.Parameters.Add("@asst_clss_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);

                    var assg_by_emp_id = cmd.Parameters.Add("@assg_by_emp_id", NpgsqlDbType.Text);
                    var assg_to_emp_id = cmd.Parameters.Add("@assg_to_emp_id", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assgn_id.Value = assignmentEquipment.AssignmentId;
                    asset_id.Value = assignmentEquipment.AssetId;
                    asst_typ_id.Value = assignmentEquipment.AssetTypeId;
                    asst_grp_id.Value = assignmentEquipment.AssetGroupId;
                    asst_clss_id.Value = assignmentEquipment.AssetClassId;
                    asst_ctg_id.Value = assignmentEquipment.AssetCategoryId;
                    assg_by_emp_id.Value = assignmentEquipment.AssignedByEmployeeId;
                    assg_to_emp_id.Value = assignmentEquipment.AssignedToEmployeeId;
                    mdb.Value = assignmentEquipment.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentEquipment.ModifiedTime ?? DateTime.Now;

                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentEquipmentAsync(AssignmentEquipment assignmentEquipment)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_eqmt SET asset_id=@asset_id, mdb=@mdb,  ");
            sb.Append("mdt=@mdt, asst_typ_id=@asst_typ_id, asst_grp_id=@asst_grp_id, ");
            sb.Append("asst_clss_id=@asst_clss_id, assg_by_emp_id=@assg_by_emp_id, ");
            sb.Append("assg_to_emp_id=@assg_to_emp_id, asst_ctg_id=@asst_ctg_id ");
            sb.Append("WHERE (assg_eqmt_id = @assg_eqmt_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_eqmt_id = cmd.Parameters.Add("@assg_eqmt_id", NpgsqlDbType.Bigint);
                    var asset_id = cmd.Parameters.Add("@asset_id", NpgsqlDbType.Text);

                    var asst_typ_id = cmd.Parameters.Add("@asst_typ_id", NpgsqlDbType.Integer);
                    var asst_grp_id = cmd.Parameters.Add("@asst_grp_id", NpgsqlDbType.Integer);
                    var asst_clss_id = cmd.Parameters.Add("@asst_clss_id", NpgsqlDbType.Integer);
                    var asst_ctg_id = cmd.Parameters.Add("@asst_ctg_id", NpgsqlDbType.Integer);

                    var assg_by_emp_id = cmd.Parameters.Add("@assg_by_emp_id", NpgsqlDbType.Text);
                    var assg_to_emp_id = cmd.Parameters.Add("@assg_to_emp_id", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assg_eqmt_id.Value = assignmentEquipment.AssignmentEquipmentId;
                    asset_id.Value = assignmentEquipment.AssetId;
                    asst_typ_id.Value = assignmentEquipment.AssetTypeId;
                    asst_grp_id.Value = assignmentEquipment.AssetGroupId;
                    asst_clss_id.Value = assignmentEquipment.AssetClassId;
                    asst_ctg_id.Value = assignmentEquipment.AssetCategoryId;
                    assg_by_emp_id.Value = assignmentEquipment.AssignedByEmployeeId;
                    assg_to_emp_id.Value = assignmentEquipment.AssignedToEmployeeId;
                    mdb.Value = assignmentEquipment.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentEquipment.ModifiedTime ?? DateTime.Now;

                    inserted_row_id = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return inserted_row_id > 0;
        }
        public async Task<bool> DeleteAssignmentEquipmentAsync(long assignmentEquipmentId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_eqmt WHERE (assg_eqmt_id=@assg_eqmt_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_eqmt_id = cmd.Parameters.Add("@assg_eqmt_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assg_eqmt_id.Value = assignmentEquipmentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #endregion

        #region Assignment Settings

        #region Assignment EventType

        //===== Write Action Methods =====//
        public async Task<int> AddAssignmentEventTypeAsync(AssignmentEventType eventType)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_evnt_typs(evnt_typ_ds) VALUES (@evnt_typ_ds) ");
            sb.Append("RETURNING evnt_typ_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_ds = cmd.Parameters.Add("@evnt_typ_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    evnt_typ_ds.Value = eventType.Description;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentEventTypeAsync(AssignmentEventType eventType)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_evnt_typs SET evnt_typ_ds=@evnt_typ_ds ");
            sb.Append("WHERE (evnt_typ_id=@evnt_typ_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_ds = cmd.Parameters.Add("@evnt_typ_ds", NpgsqlDbType.Text);
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    evnt_typ_ds.Value = eventType.Description;
                    evnt_typ_id.Value = eventType.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentEventTypeAsync(int assignmentEventTypeId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_evnt_typs WHERE (evnt_typ_id=@evnt_typ_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    evnt_typ_id.Value = assignmentEventTypeId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        //===== Read Action Methods ====//
        public async Task<AssignmentEventType> GetAssignmentEventTypeByIdAsync(int assignmentEventTypeId)
        {
            AssignmentEventType assignmentEventType = new AssignmentEventType();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT evnt_typ_id, evnt_typ_ds ");
            sb.Append("FROM public.ats_evnt_typs ");
            sb.Append("WHERE (evnt_typ_id=@evnt_typ_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    evnt_typ_id.Value = assignmentEventTypeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEventType.Id = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"];
                            assignmentEventType.Description = reader["evnt_typ_ds"] == DBNull.Value ? "" : reader["evnt_typ_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEventType;
        }
        public async Task<List<AssignmentEventType>> GetAssignmentEventTypesByDescriptionAsync(string description)
        {
            List<AssignmentEventType> assignmentEventTypeList = new List<AssignmentEventType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT evnt_typ_id, evnt_typ_ds ");
            sb.Append("FROM public.ats_evnt_typs ");
            sb.Append("WHERE LOWER(evnt_typ_ds) = LOWER(@evnt_typ_ds) ");
            sb.Append("ORDER BY evnt_typ_ds;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var evnt_typ_ds = cmd.Parameters.Add("@evnt_typ_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    evnt_typ_ds.Value = description;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEventTypeList.Add(new AssignmentEventType
                            {
                                Id = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                Description = reader["evnt_typ_ds"] == DBNull.Value ? "" : reader["evnt_typ_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEventTypeList;
        }
        public async Task<List<AssignmentEventType>> GetAssignmentEventTypesAsync()
        {
            List<AssignmentEventType> assignmentEventTypeList = new List<AssignmentEventType>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT evnt_typ_id, evnt_typ_ds ");
            sb.Append("FROM public.ats_evnt_typs ");
            sb.Append("ORDER BY evnt_typ_ds;");
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
                            assignmentEventTypeList.Add(new AssignmentEventType
                            {
                                Id = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                Description = reader["evnt_typ_ds"] == DBNull.Value ? "" : reader["evnt_typ_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentEventTypeList;
        }
        #endregion

        #region Assignment Role

        //===== Write Action Methods =====//
        public async Task<int> AddAssignmentRoleAsync(AssignmentRole role)
        {
            int inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_rls(assg_rls_ds) ");
            sb.Append("VALUES (@assg_rls_ds) RETURNING assg_rls_id; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_ds = cmd.Parameters.Add("@assg_rls_ds", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assg_rls_ds.Value = role.Description;
                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (int)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id;
        }
        public async Task<bool> UpdateAssignmentEventTypeAsync(AssignmentRole role)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_rls SET assg_rls_ds=@assg_rls_ds ");
            sb.Append("WHERE (assg_rls_id=@assg_rls_id); ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_ds = cmd.Parameters.Add("@assg_rls_ds", NpgsqlDbType.Text);
                    var assg_rls_id = cmd.Parameters.Add("@assg_rls_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    assg_rls_ds.Value = role.Description;
                    assg_rls_id.Value = role.Id;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentRoleAsync(int assignmentRoleId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_rls WHERE (assg_rls_id=@assg_rls_id); ";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_id = cmd.Parameters.Add("@assg_rls_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    assg_rls_id.Value = assignmentRoleId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        //===== Read Action Methods ====//
        public async Task<AssignmentRole> GetAssignmentRoleByIdAsync(int assignmentRoleId)
        {
            AssignmentRole assignmentRole = new AssignmentRole();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_rls_id, assg_rls_ds ");
            sb.Append("FROM public.ats_assg_rls ");
            sb.Append("WHERE (assg_rls_id=@assg_rls_id);");

            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_id = cmd.Parameters.Add("@assg_rls_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    assg_rls_id.Value = assignmentRoleId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentRole.Id = reader["assg_rls_id"] == DBNull.Value ? 0 : (int)reader["assg_rls_id"];
                            assignmentRole.Description = reader["assg_rls_ds"] == DBNull.Value ? "" : reader["assg_rls_ds"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentRole;
        }
        public async Task<List<AssignmentRole>> GetAssignmentRolesByDescriptionAsync(string assignmentRoleDescription)
        {
            List<AssignmentRole> assignmentRoleList = new List<AssignmentRole>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_rls_id, assg_rls_ds ");
            sb.Append("FROM public.ats_assg_rls ");
            sb.Append("WHERE (assg_rls_ds=@assg_rls_ds) ");
            sb.Append("ORDER BY assg_rls_ds;");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_rls_ds = cmd.Parameters.Add("@assg_rls_ds", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assg_rls_ds.Value = assignmentRoleDescription;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentRoleList.Add(new AssignmentRole
                            {
                                Id = reader["assg_rls_id"] == DBNull.Value ? 0 : (int)reader["assg_rls_id"],
                                Description = reader["assg_rls_ds"] == DBNull.Value ? "" : reader["assg_rls_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentRoleList;
        }

        public async Task<List<AssignmentRole>> GetAssignmentRolesAsync()
        {
            List<AssignmentRole> assignmentRoleList = new List<AssignmentRole>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_rls_id, assg_rls_ds ");
            sb.Append("FROM public.ats_assg_rls ");
            sb.Append("ORDER BY assg_rls_ds;");
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
                            assignmentRoleList.Add(new AssignmentRole
                            {
                                Id = reader["assg_rls_id"] == DBNull.Value ? 0 : (int)reader["assg_rls_id"],
                                Description = reader["assg_rls_ds"] == DBNull.Value ? "" : reader["assg_rls_ds"].ToString(),
                            });
                        }
                }
                await conn.CloseAsync();
            }
            return assignmentRoleList;
        }

        #endregion

        #region Assignment Note Action Methods
        public async Task<List<AssignmentNote>> GetAssignmentNotesByAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentNote> assignmentNotes = new List<AssignmentNote>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT nts_id, nts_tm, nts_ds, nts_by, ");
            sb.Append("assgn_id, is_ccl, ccl_by, ccl_dt ");
            sb.Append("FROM public.ats_assg_nts ");
            sb.Append("WHERE (assgn_id = @assgn_id) ");
            sb.Append("ORDER BY nts_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentNotes.Add(new AssignmentNote
                            {
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0 : (long)reader["assgn_id"],
                                NoteId = reader["nts_id"] == DBNull.Value ? 0 : (long)reader["nts_id"],
                                NoteTime = reader["nts_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["nts_tm"],
                                NoteContent = reader["nts_ds"] == DBNull.Value ? string.Empty : reader["nts_ds"].ToString(),
                                NoteWrittenBy = reader["nts_by"] == DBNull.Value ? string.Empty : reader["nts_by"].ToString(),
                                IsCancelled = reader["is_ccl"] == DBNull.Value ? false : (bool)reader["is_ccl"],
                                CancelledBy = reader["ccl_by"] == DBNull.Value ? string.Empty : reader["ccl_by"].ToString(),
                                CancelledOn = reader["ccl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ccl_dt"],
                            });
                        }
                }
            }
            return assignmentNotes;
        }
        public async Task<bool> AddNoteAsync(AssignmentNote n)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_nts( nts_tm, ");
            sb.Append("nts_ds, nts_by, assgn_id) ");
            sb.Append("VALUES (@nts_tm, @nts_ds, @nts_by, @assgn_id); ");
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
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    nts_tm.Value = n.NoteTime;
                    nts_ds.Value = n.NoteContent;
                    nts_by.Value = n.NoteWrittenBy ?? (object)DBNull.Value;
                    assgn_id.Value = n.AssignmentId ?? (object)DBNull.Value;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> CancelAssignmentNoteAsync(long assignmentNoteId, string cancelledBy)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_nts SET is_ccl=true, ccl_dt=@ccl_dt, ");
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
                    var ccl_by = cmd.Parameters.Add("@ccl_by", NpgsqlDbType.Text);
                    cmd.Prepare();
                    nts_id.Value = assignmentNoteId;
                    ccl_dt.Value = DateTime.Now;
                    ccl_by.Value = cancelledBy;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentNoteAsync(long assignmentNoteId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_nts WHERE (nts_id=@nts_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var nts_id = cmd.Parameters.Add("@nts_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    nts_id.Value = assignmentNoteId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }

        #endregion

        #region Assignment History Action Methods
        public async Task<List<AssignmentHistory>> GetAssignmentHistoryByAssignmentIdAsync(long assignmentId)
        {
            List<AssignmentHistory> assignmentHistory = new List<AssignmentHistory>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT assg_hst_id, assg_hst_tm, assg_hst_ds, ");
            sb.Append("assg_hst_by, assgn_id  FROM public.ats_assg_hst ");
            sb.Append("WHERE (assgn_id = @assgn_id) ");
            sb.Append("ORDER BY assg_hst_id DESC; ");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    await cmd.PrepareAsync();
                    assgn_id.Value = assignmentId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentHistory.Add(new AssignmentHistory
                            {
                                Id = reader["assg_hst_id"] == DBNull.Value ? 0L : (long)reader["assg_hst_id"],
                                AssignmentId = reader["assgn_id"] == DBNull.Value ? 0L : (long)reader["assgn_id"],
                                ActivityTime = reader["assg_hst_tm"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["assg_hst_tm"],
                                ActivityDescription = reader["assg_hst_ds"] == DBNull.Value ? string.Empty : reader["assg_hst_ds"].ToString(),
                                ActivityBy = reader["assg_hst_by"] == DBNull.Value ? string.Empty : reader["assg_hst_by"].ToString(),
                            });
                        }
                }
            }
            return assignmentHistory;
        }
        public async Task<bool> AddAssignmentHistoryAsync(AssignmentHistory assignmentHistory)
        {
            int rows = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_hst(assg_hst_tm, ");
            sb.Append("assg_hst_ds, assg_hst_by, assgn_id) VALUES ");
            sb.Append("(@assg_hst_tm, @assg_hst_ds, @assg_hst_by, @assgn_id); ");
            string query = sb.ToString();

            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_hst_tm = cmd.Parameters.Add("@assg_hst_tm", NpgsqlDbType.Timestamp);
                    var assg_hst_ds = cmd.Parameters.Add("@assg_hst_ds", NpgsqlDbType.Text);
                    var assg_hst_by = cmd.Parameters.Add("@assg_hst_by", NpgsqlDbType.Text);
                    var assgn_id = cmd.Parameters.Add("@assgn_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assg_hst_tm.Value = assignmentHistory.ActivityTime;
                    assg_hst_ds.Value = assignmentHistory.ActivityDescription;
                    assg_hst_by.Value = assignmentHistory.ActivityBy;
                    assgn_id.Value = assignmentHistory.AssignmentId;
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            return rows > 0;
        }
        public async Task<bool> DeleteAssignmentHistoryAsync(long assignmentHistoryId)
        {
            int rows = 0;
            string query = "DELETE FROM public.ats_assg_hst WHERE (assg_hst_id = @assg_hst_id);";
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_hst_id = cmd.Parameters.Add("@assg_hst_id", NpgsqlDbType.Bigint);
                    cmd.Prepare();
                    assg_hst_id.Value = assignmentHistoryId;
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
