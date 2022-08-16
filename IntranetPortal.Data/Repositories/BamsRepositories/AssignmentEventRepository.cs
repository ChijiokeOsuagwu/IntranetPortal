using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Repositories.BamsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.BamsRepositories
{
    public class AssignmentEventRepository : IAssignmentEventRepository
    {
        public IConfiguration _config { get; }
        public AssignmentEventRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== AssignmentEvent Action Methods =====================================================//
        #region AssignmentEvents Action Methods

        public async Task<AssignmentEvent> GetByIdAsync(int Id)
        {
            AssignmentEvent assignmentEvent = new AssignmentEvent();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.assgnmt_id, a.assgnmt_tl, a.assgnmt_ds, a.evnt_typ_id, a.evnt_starts, ");
            sb.Append("a.evnt_ends, a.station_id, a.evnt_venue, a.evnt_state, a.evnt_bzns_id, ");
            sb.Append("a.liaison_nm, a.liaison_phn, a.status_id, a.ispd, a.mdb, a.mdt, a.ctb, ");
            sb.Append("a.ctt, t.evnt_typ_ds, l.locname, b.bzns_name, s.stts_nm FROM public.bam_assgnmts a ");
            sb.Append("INNER JOIN public.bam_evnt_typs t ON a.evnt_typ_id = t.evnt_typ_id ");
            sb.Append("INNER JOIN public.gst_locs l ON a.station_id = l.locqk ");
            sb.Append("INNER JOIN public.gst_bzns b ON a.evnt_bzns_id = b.bzns_id ");
            sb.Append("INNER JOIN public.bam_stt_stts s ON a.status_id = s.stts_id ");
            sb.Append("WHERE (a.assgnmt_id = @assgnmt_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var assignment_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    assignment_id.Value = Id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEvent.ID = reader["assgnmt_id"] == DBNull.Value ? (int?)null : (int)reader["assgnmt_id"];
                            assignmentEvent.Title = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString();
                            assignmentEvent.Description = reader["assgnmt_ds"] == DBNull.Value ? string.Empty : reader["assgnmt_ds"].ToString();
                            assignmentEvent.EventTypeID = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"];
                            assignmentEvent.EventTypeName = reader["evnt_typ_ds"] == DBNull.Value ? string.Empty : reader["evnt_typ_ds"].ToString();
                            assignmentEvent.StartTime = reader["evnt_starts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_starts"];
                            assignmentEvent.EndTime = reader["evnt_ends"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_ends"];
                            assignmentEvent.StationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"];
                            assignmentEvent.StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            assignmentEvent.Venue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString();
                            assignmentEvent.State = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString();
                            assignmentEvent.CustomerID = reader["evnt_bzns_id"] == DBNull.Value ? string.Empty : reader["evnt_bzns_id"].ToString();
                            assignmentEvent.CustomerName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString();
                            assignmentEvent.LiaisonName = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString();
                            assignmentEvent.LiaisonPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString();
                            assignmentEvent.StatusID = reader["status_id"] == DBNull.Value ? 0 : (int)reader["status_id"];
                            assignmentEvent.StatusDescription = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString();
                            assignmentEvent.IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"];
                            assignmentEvent.ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString();
                            assignmentEvent.ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString();
                            assignmentEvent.CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString();
                            assignmentEvent.CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return assignmentEvent;
        }

        public async Task<IList<AssignmentEvent>> SearchByCustomerNameAsync(string customerName)
        {
            IList<AssignmentEvent> assignmentEvents = new List<AssignmentEvent>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT a.assgnmt_id, a.assgnmt_tl, a.assgnmt_ds, a.evnt_typ_id, a.evnt_starts, ");
            sb.Append($"a.evnt_ends, a.station_id, a.evnt_venue, a.evnt_state, a.evnt_bzns_id, ");
            sb.Append($"a.liaison_nm, a.liaison_phn, a.status_id, a.ispd, a.mdb, a.mdt, a.ctb, ");
            sb.Append($"a.ctt, t.evnt_typ_ds, l.locname, b.bzns_name, s.stts_nm FROM public.bam_assgnmts a ");
            sb.Append($"INNER JOIN public.bam_evnt_typs t ON a.evnt_typ_id = t.evnt_typ_id ");
            sb.Append($"INNER JOIN public.gst_locs l ON a.station_id = l.locqk ");
            sb.Append($"INNER JOIN public.gst_bzns b ON a.evnt_bzns_id = b.bzns_id ");
            sb.Append($"INNER JOIN public.bam_stt_stts s ON a.status_id = s.stts_id ");
            sb.Append($"WHERE (LOWER(b.bzns_name) LIKE '%'||LOWER(@bzns_name)||'%') ");
            sb.Append($"ORDER BY a.evnt_starts DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var bzns_name = cmd.Parameters.Add("@bzns_name", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    bzns_name.Value = customerName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEvents.Add(new AssignmentEvent
                            {
                                ID = reader["assgnmt_id"] == DBNull.Value ? (int?)null : (int)reader["assgnmt_id"],
                            Title = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                            Description = reader["assgnmt_ds"] == DBNull.Value ? string.Empty : reader["assgnmt_ds"].ToString(),
                            EventTypeID = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                            EventTypeName = reader["evnt_typ_ds"] == DBNull.Value ? string.Empty : reader["evnt_typ_ds"].ToString(),
                            StartTime = reader["evnt_starts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_starts"],
                            EndTime = reader["evnt_ends"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_ends"],
                            StationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                            StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            Venue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString(),
                            State = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString(),
                            CustomerID = reader["evnt_bzns_id"] == DBNull.Value ? string.Empty : reader["evnt_bzns_id"].ToString(),
                            CustomerName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString(),
                            LiaisonName = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString(),
                            LiaisonPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString(),
                            StatusID = reader["status_id"] == DBNull.Value ? 0 : (int)reader["status_id"],
                            StatusDescription = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString(),
                            IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"],
                            ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                            ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                            CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                            CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
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
            return assignmentEvents;
        }

        public async Task<IList<AssignmentEvent>> GetOpenAsync()
        {
            IList<AssignmentEvent> assignmentEvents = new List<AssignmentEvent>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT a.assgnmt_id, a.assgnmt_tl, a.assgnmt_ds, a.evnt_typ_id, a.evnt_starts, ");
            sb.Append($"a.evnt_ends, a.station_id, a.evnt_venue, a.evnt_state, a.evnt_bzns_id, ");
            sb.Append($"a.liaison_nm, a.liaison_phn, a.status_id, a.ispd, a.mdb, a.mdt, a.ctb, ");
            sb.Append($"a.ctt, t.evnt_typ_ds, l.locname, b.bzns_name, s.stts_nm FROM public.bam_assgnmts a ");
            sb.Append($"INNER JOIN public.bam_evnt_typs t ON a.evnt_typ_id = t.evnt_typ_id ");
            sb.Append($"INNER JOIN public.gst_locs l ON a.station_id = l.locqk ");
            sb.Append($"INNER JOIN public.gst_bzns b ON a.evnt_bzns_id = b.bzns_id ");
            sb.Append($"INNER JOIN public.bam_stt_stts s ON a.status_id = s.stts_id ");
            sb.Append($"WHERE (a.evnt_ends IS NULL) ");
            sb.Append($"ORDER BY a.evnt_starts DESC;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            assignmentEvents.Add(new AssignmentEvent
                            {
                                ID = reader["assgnmt_id"] == DBNull.Value ? (int?)null : (int)reader["assgnmt_id"],
                                Title = reader["assgnmt_tl"] == DBNull.Value ? string.Empty : reader["assgnmt_tl"].ToString(),
                                Description = reader["assgnmt_ds"] == DBNull.Value ? string.Empty : reader["assgnmt_ds"].ToString(),
                                EventTypeID = reader["evnt_typ_id"] == DBNull.Value ? 0 : (int)reader["evnt_typ_id"],
                                EventTypeName = reader["evnt_typ_ds"] == DBNull.Value ? string.Empty : reader["evnt_typ_ds"].ToString(),
                                StartTime = reader["evnt_starts"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_starts"],
                                EndTime = reader["evnt_ends"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["evnt_ends"],
                                StationID = reader["station_id"] == DBNull.Value ? 0 : (int)reader["station_id"],
                                StationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                Venue = reader["evnt_venue"] == DBNull.Value ? string.Empty : reader["evnt_venue"].ToString(),
                                State = reader["evnt_state"] == DBNull.Value ? string.Empty : reader["evnt_state"].ToString(),
                                CustomerID = reader["evnt_bzns_id"] == DBNull.Value ? string.Empty : reader["evnt_bzns_id"].ToString(),
                                CustomerName = reader["bzns_name"] == DBNull.Value ? string.Empty : reader["bzns_name"].ToString(),
                                LiaisonName = reader["liaison_nm"] == DBNull.Value ? string.Empty : reader["liaison_nm"].ToString(),
                                LiaisonPhone = reader["liaison_phn"] == DBNull.Value ? string.Empty : reader["liaison_phn"].ToString(),
                                StatusID = reader["status_id"] == DBNull.Value ? 0 : (int)reader["status_id"],
                                StatusDescription = reader["stts_nm"] == DBNull.Value ? string.Empty : reader["stts_nm"].ToString(),
                                IsPaid = reader["ispd"] == DBNull.Value ? false : (bool)reader["ispd"],
                                ModifiedBy = reader["mdb"] == DBNull.Value ? string.Empty : reader["mdb"].ToString(),
                                ModifiedTime = reader["mdt"] == DBNull.Value ? string.Empty : reader["mdt"].ToString(),
                                CreatedBy = reader["ctb"] == DBNull.Value ? string.Empty : reader["ctb"].ToString(),
                                CreatedTime = reader["ctt"] == DBNull.Value ? string.Empty : reader["ctt"].ToString(),
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
            return assignmentEvents;
        }

        #endregion

        //============== AssignmentEvent CRUD Action Methods =================================================//
        #region AssignmentEvent CRUD Action Methods
        public async Task<bool> AddAsync(AssignmentEvent assignmentEvent)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.bam_assgnmts(assgnmt_tl, assgnmt_ds, evnt_typ_id, ");
            sb.Append("evnt_starts, evnt_ends, station_id, evnt_venue, evnt_state, evnt_bzns_id, ");
            sb.Append("liaison_nm, liaison_phn, status_id, ispd, mdb, mdt, ctb, ctt) ");
            sb.Append("VALUES (@assgnmt_tl, @assgnmt_ds, @evnt_typ_id, @evnt_starts, @evnt_ends, ");
            sb.Append("@station_id, @evnt_venue, @evnt_state, @evnt_bzns_id, @liaison_nm, ");
            sb.Append("@liaison_phn, @status_id, @ispd, @mdb, @mdt, @ctb, @ctt);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_tl = cmd.Parameters.Add("@assgnmt_tl", NpgsqlDbType.Text);
                    var assgnmt_ds = cmd.Parameters.Add("@assgnmt_ds", NpgsqlDbType.Text);
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    var evnt_starts = cmd.Parameters.Add("@evnt_starts", NpgsqlDbType.TimestampTz);
                    var evnt_ends = cmd.Parameters.Add("@evnt_ends", NpgsqlDbType.TimestampTz);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var evnt_venue = cmd.Parameters.Add("@evnt_venue", NpgsqlDbType.Text);
                    var evnt_state = cmd.Parameters.Add("@evnt_state", NpgsqlDbType.Text);
                    var evnt_bzns_id = cmd.Parameters.Add("@evnt_bzns_id", NpgsqlDbType.Text);
                    var liaison_nm = cmd.Parameters.Add("@liaison_nm", NpgsqlDbType.Text);
                    var liaison_phn = cmd.Parameters.Add("@liaison_phn", NpgsqlDbType.Text);
                    var status_id = cmd.Parameters.Add("@status_id", NpgsqlDbType.Integer);
                    var ispd = cmd.Parameters.Add("@ispd", NpgsqlDbType.Boolean);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assgnmt_tl.Value = assignmentEvent.Title;
                    assgnmt_ds.Value = assignmentEvent.Description;
                    evnt_typ_id.Value = assignmentEvent.EventTypeID;
                    evnt_starts.Value = assignmentEvent.StartTime ?? (object)DBNull.Value;
                    evnt_ends.Value = assignmentEvent.EndTime ?? (object)DBNull.Value;
                    station_id.Value = assignmentEvent.StationID;
                    evnt_venue.Value = assignmentEvent.Venue;
                    evnt_state.Value = assignmentEvent.State;
                    evnt_bzns_id.Value = assignmentEvent.CustomerID;
                    liaison_nm.Value = assignmentEvent.LiaisonName ?? (object)DBNull.Value;
                    liaison_phn.Value = assignmentEvent.LiaisonPhone ?? (object)DBNull.Value;
                    status_id.Value = assignmentEvent.StatusID;
                    ispd.Value = assignmentEvent.IsPaid;
                    mdb.Value = assignmentEvent.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentEvent.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = assignmentEvent.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = assignmentEvent.CreatedTime ?? (object)DBNull.Value;
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

        public async Task<bool> EditAsync(AssignmentEvent assignmentEvent)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.bam_assgnmts SET assgnmt_tl = @assgnmt_tl, ");
            sb.Append("assgnmt_ds = @assgnmt_ds, evnt_typ_id = @evnt_typ_id, ");
            sb.Append("evnt_starts = @evnt_starts, evnt_ends = @evnt_starts, ");
            sb.Append("station_id = @station_id, evnt_venue = @evnt_venue, ");
            sb.Append("evnt_state = @evnt_state, evnt_bzns_id = @evnt_bzns_id, ");
            sb.Append("liaison_nm = @liaison_nm, liaison_phn = @liaison_phn, ");
            sb.Append("status_id = @status_id, ispd = @ispd, mdb = @mdb, ");
            sb.Append("mdt = @mdt, ctb = @ctb, ctt = @ctt ");
            sb.Append("WHERE (assgnmt_id = @assgnmt_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    var assgnmt_tl = cmd.Parameters.Add("@assgnmt_tl", NpgsqlDbType.Text);
                    var assgnmt_ds = cmd.Parameters.Add("@assgnmt_ds", NpgsqlDbType.Text);
                    var evnt_typ_id = cmd.Parameters.Add("@evnt_typ_id", NpgsqlDbType.Integer);
                    var evnt_starts = cmd.Parameters.Add("@evnt_starts", NpgsqlDbType.TimestampTz);
                    var evnt_ends = cmd.Parameters.Add("@evnt_ends", NpgsqlDbType.TimestampTz);
                    var station_id = cmd.Parameters.Add("@station_id", NpgsqlDbType.Integer);
                    var evnt_venue = cmd.Parameters.Add("@evnt_venue", NpgsqlDbType.Text);
                    var evnt_state = cmd.Parameters.Add("@evnt_state", NpgsqlDbType.Text);
                    var evnt_bzns_id = cmd.Parameters.Add("@evnt_bzns_id", NpgsqlDbType.Text);
                    var liaison_nm = cmd.Parameters.Add("@liaison_nm", NpgsqlDbType.Text);
                    var liaison_phn = cmd.Parameters.Add("@liaison_phn", NpgsqlDbType.Text);
                    var status_id = cmd.Parameters.Add("@status_id", NpgsqlDbType.Integer);
                    var ispd = cmd.Parameters.Add("@ispd", NpgsqlDbType.Boolean);
                    var mdb = cmd.Parameters.Add("@mdb", NpgsqlDbType.Text);
                    var mdt = cmd.Parameters.Add("@mdt", NpgsqlDbType.Text);
                    var ctb = cmd.Parameters.Add("@ctb", NpgsqlDbType.Text);
                    var ctt = cmd.Parameters.Add("@ctt", NpgsqlDbType.Text);
                    cmd.Prepare();
                    assgnmt_id.Value = assignmentEvent.ID;
                    assgnmt_tl.Value = assignmentEvent.Title;
                    assgnmt_ds.Value = assignmentEvent.Description;
                    evnt_typ_id.Value = assignmentEvent.EventTypeID;
                    evnt_starts.Value = assignmentEvent.StartTime ?? (object)DBNull.Value;
                    evnt_ends.Value = assignmentEvent.EndTime ?? (object)DBNull.Value;
                    station_id.Value = assignmentEvent.StationID;
                    evnt_venue.Value = assignmentEvent.Venue;
                    evnt_state.Value = assignmentEvent.State;
                    evnt_bzns_id.Value = assignmentEvent.CustomerID;
                    liaison_nm.Value = assignmentEvent.LiaisonName ?? (object)DBNull.Value;
                    liaison_phn.Value = assignmentEvent.LiaisonPhone ?? (object)DBNull.Value;
                    status_id.Value = assignmentEvent.StatusID;
                    ispd.Value = assignmentEvent.IsPaid;
                    mdb.Value = assignmentEvent.ModifiedBy ?? (object)DBNull.Value;
                    mdt.Value = assignmentEvent.ModifiedTime ?? (object)DBNull.Value;
                    ctb.Value = assignmentEvent.CreatedBy ?? (object)DBNull.Value;
                    ctt.Value = assignmentEvent.CreatedTime ?? (object)DBNull.Value;

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

        public async Task<bool> DeleteAsync(int assignmentEventId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.bam_assgnmts WHERE (assgnmt_id = @assgnmt_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var assgnmt_id = cmd.Parameters.Add("@assgnmt_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    assgnmt_id.Value = assignmentEventId;
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
    }
}
