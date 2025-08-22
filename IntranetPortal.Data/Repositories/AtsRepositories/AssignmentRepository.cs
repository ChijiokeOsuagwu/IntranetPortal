using IntranetPortal.Base.Models.AtsModels;
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
            sb.Append("assigned_by_id, evnt_ctr, iscnf, assgn_no, assigned_to_rl) ");
            sb.Append("VALUES (@assgn_tl, @assgn_ds, @evnt_typ_id, @start_time, @end_time, ");
            sb.Append("@station_id, @assigned_to_id, @evnt_venue, @evnt_state, @bzns_id, ");
            sb.Append("@liaison_nm, @liaison_phn, @approval_status, @progress_status, ");
            sb.Append("@ispd, @islv, @isus, @ispr, @ctb, @ctt, @due_date, @assigned_by_id, ");
            sb.Append("@evnt_ctr, @iscnf, @assgn_no, @assigned_to_rl) RETURNING assgn_id; ");
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
            sb.Append("evnt_ctr=@evnt_ctr, iscnf=@iscnf ");
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
            sb.Append("l.locname, c.mdb, c.mdt, c.ctb, c.ctt, srv_rtg, att_sts, rmks, ");
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
                            assignmentCrew.AttendanceStatus = reader["att_sts"] == DBNull.Value ? string.Empty : reader["att_sts"].ToString();
                            assignmentCrew.ServiceRating = reader["srv_rtg"] == DBNull.Value ? string.Empty : reader["srv_rtg"].ToString();
                            assignmentCrew.Remarks = reader["rmks"] == DBNull.Value ? string.Empty : reader["rmks"].ToString();
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
            sb.Append("l.locname, c.mdb, c.mdt, c.ctb, c.ctt, srv_rtg, att_sts, rmks, ");
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
                                AttendanceStatus = reader["att_sts"] == DBNull.Value ? string.Empty : reader["att_sts"].ToString(),
                                ServiceRating = reader["srv_rtg"] == DBNull.Value ? string.Empty : reader["srv_rtg"].ToString(),
                                Remarks = reader["rmks"] == DBNull.Value ? string.Empty : reader["rmks"].ToString(),
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
            sb.Append("l.locname, c.mdb, c.mdt, c.ctb, c.ctt, srv_rtg, att_sts, rmks, ");
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
                    var emp_id = cmd.Parameters.Add("@emp_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
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
                                AttendanceStatus = reader["att_sts"] == DBNull.Value ? string.Empty : reader["att_sts"].ToString(),
                                ServiceRating = reader["srv_rtg"] == DBNull.Value ? string.Empty : reader["srv_rtg"].ToString(),
                                Remarks = reader["rmks"] == DBNull.Value ? string.Empty : reader["rmks"].ToString(),
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
        public async Task<long> AddAssignmentCrewMemberAsync(AssignmentCrewMember assignmentCrewMember)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.ats_assg_crw(assgn_id, emp_id, ");
            sb.Append("emp_rl1, unit_id, dept_id, loc_id, is_ld, emp_rl2, ");
            sb.Append("emp_rl3, mdb, mdt, ctb, ctt, srv_rtg, att_sts, rmks) ");
            sb.Append("VALUES (@assgn_id, @emp_id, @emp_rl1, @unit_id, ");
            sb.Append("@dept_id, @loc_id, @is_ld, @emp_rl2, @emp_rl3, @ctb, ");
            sb.Append("@ctt, @ctb, @ctt, @srv_rtg, @att_sts, @rmks) ");
            sb.Append("RETURNING assg_crw_id; ");
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
                    var srv_rtg = cmd.Parameters.Add("@srv_rtg", NpgsqlDbType.Text);
                    var att_sts = cmd.Parameters.Add("@att_sts", NpgsqlDbType.Text);
                    var rmks = cmd.Parameters.Add("@rmks", NpgsqlDbType.Text);
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
                    srv_rtg.Value = assignmentCrewMember.ServiceRating ?? (object)DBNull.Value;
                    att_sts.Value = assignmentCrewMember.AttendanceStatus ?? (object)DBNull.Value;
                    rmks.Value = assignmentCrewMember.Remarks ?? (object)DBNull.Value;

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
        public async Task<bool> UpdateAssignmentCrewParticipationAsync(long assignmentCrewId, string serviceRating, string attendanceStatus, string remarks, string updatedBy)
        {
            long inserted_row_id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.ats_assg_crw SET srv_rtg = @srv_rtg, ");
            sb.Append("att_sts=@att_sts, rmks=@rmks,mdb=@mdb, mdt=@mdt ");
            sb.Append("WHERE (assg_crw_id=@assg_crw_id);");
            string query = sb.ToString();
            using (var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection")))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assg_crw_id = cmd.Parameters.Add("@assg_crw_id", NpgsqlDbType.Bigint);
                    var srv_rtg = cmd.Parameters.Add("@srv_rtg", NpgsqlDbType.Text);
                    var att_sts = cmd.Parameters.Add("@att_sts", NpgsqlDbType.Text);
                    var rmks = cmd.Parameters.Add("@rmks", NpgsqlDbType.Text);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Timestamp);
                    cmd.Prepare();
                    assg_crw_id.Value = assignmentCrewId;
                    srv_rtg.Value = serviceRating ?? (object)DBNull.Value;
                    att_sts.Value = attendanceStatus ?? (object)DBNull.Value;
                    rmks.Value = remarks ?? (object)DBNull.Value;
                    mdb.Value = updatedBy;
                    mdt.Value = DateTime.Now;

                    var obj = await cmd.ExecuteScalarAsync();
                    inserted_row_id = (long)obj;
                }
                await conn.CloseAsync();
            }
            return inserted_row_id > 0;
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
