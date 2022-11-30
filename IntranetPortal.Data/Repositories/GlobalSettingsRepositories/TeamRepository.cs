using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Repositories.GlobalSettingsRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.GlobalSettingsRepositories
{
    public class TeamRepository : ITeamRepository
    {
        public IConfiguration _config { get; }
        public TeamRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============== Teams Action Methods =====================================================//
        #region Teams Action Methods

        public async Task<Team> GetTeamByIdAsync(string teamId)
        {
            Team team = new Team();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tm_id, t.tm_nm, t.tm_ds, t.tm_mb, t.tm_md, t.tm_cb, t.tm_cd, t.tm_loc,l.locname ");
            sb.Append("FROM public.gst_tms t LEFT OUTER JOIN public.gst_locs l ON t.tm_loc = l.locqk WHERE (tm_id = @tm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var team_id = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    team_id.Value = teamId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            team.TeamID = reader["tm_id"] == DBNull.Value ? String.Empty : reader["tm_id"].ToString();
                            team.TeamName = reader["tm_nm"] == DBNull.Value ? String.Empty : reader["tm_nm"].ToString();
                            team.TeamDescription = reader["tm_ds"] == DBNull.Value ? String.Empty : reader["tm_ds"].ToString();
                            team.TeamLocationID = reader["tm_loc"] == DBNull.Value ? 0 : (int)reader["tm_loc"];
                            team.TeamLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString();
                            team.ModifiedBy = reader["tm_mb"] == DBNull.Value ? String.Empty : reader["tm_mb"].ToString();
                            team.ModifiedDate = reader["tm_md"] == DBNull.Value ? String.Empty : reader["tm_md"].ToString();
                            team.CreatedBy = reader["tm_cb"] == DBNull.Value ? String.Empty : reader["tm_cb"].ToString();
                            team.CreatedDate = reader["tm_cd"] == DBNull.Value ? String.Empty : reader["tm_cd"].ToString();
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return team;
        }

        public async Task<IList<Team>> GetTeamByNameAsync(string teamName)
        {
            List<Team> teamList = new List<Team>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tm_id, t.tm_nm, t.tm_ds, t.tm_mb, t.tm_md, t.tm_cb, t.tm_cd, t.tm_loc,l.locname ");
            sb.Append("FROM public.gst_tms t LEFT OUTER JOIN public.gst_locs l ON t.tm_loc = l.locqk ");
            sb.Append($"WHERE(LOWER(tm_nm) LIKE '%'||LOWER(@tm_nm)||'%') ORDER BY tm_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var team_name = cmd.Parameters.Add("@tm_nm", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    team_name.Value = teamName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            teamList.Add(new Team()
                            {
                                TeamID = reader["tm_id"] == DBNull.Value ? String.Empty : reader["tm_id"].ToString(),
                                TeamName = reader["tm_nm"] == DBNull.Value ? String.Empty : reader["tm_nm"].ToString(),
                                TeamDescription = reader["tm_ds"] == DBNull.Value ? String.Empty : reader["tm_ds"].ToString(),
                                TeamLocationID = reader["tm_loc"] == DBNull.Value ? 0 : (int)reader["tm_loc"],
                                TeamLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                                ModifiedBy = reader["tm_mb"] == DBNull.Value ? String.Empty : reader["tm_mb"].ToString(),
                                ModifiedDate = reader["tm_md"] == DBNull.Value ? String.Empty : reader["tm_md"].ToString(),
                                CreatedBy = reader["tm_cb"] == DBNull.Value ? String.Empty : reader["tm_cb"].ToString(),
                                CreatedDate = reader["tm_cd"] == DBNull.Value ? String.Empty : reader["tm_cd"].ToString()
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
            return teamList;
        }

        public async Task<IList<Team>> GetTeamsAsync()
        {
            List<Team> teamList = new List<Team>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.tm_id, t.tm_nm, t.tm_ds, t.tm_mb, t.tm_md, t.tm_cb, t.tm_cd, t.tm_loc,l.locname ");
            sb.Append("FROM public.gst_tms t LEFT OUTER JOIN public.gst_locs l ON t.tm_loc = l.locqk ");
            sb.Append("ORDER BY tm_nm;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.PrepareAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        teamList.Add(new Team()
                        {
                            TeamID = reader["tm_id"] == DBNull.Value ? String.Empty : reader["tm_id"].ToString(),
                            TeamName = reader["tm_nm"] == DBNull.Value ? String.Empty : reader["tm_nm"].ToString(),
                            TeamDescription = reader["tm_ds"] == DBNull.Value ? String.Empty : reader["tm_ds"].ToString(),
                            TeamLocationID = reader["tm_loc"] == DBNull.Value ? 0 : (int)reader["tm_loc"],
                            TeamLocationName = reader["locname"] == DBNull.Value ? string.Empty : reader["locname"].ToString(),
                            ModifiedBy = reader["tm_mb"] == DBNull.Value ? String.Empty : reader["tm_mb"].ToString(),
                            ModifiedDate = reader["tm_md"] == DBNull.Value ? String.Empty : reader["tm_md"].ToString(),
                            CreatedBy = reader["tm_cb"] == DBNull.Value ? String.Empty : reader["tm_cb"].ToString(),
                            CreatedDate = reader["tm_cd"] == DBNull.Value ? String.Empty : reader["tm_cd"].ToString()
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
            return teamList;
        }

        public async Task<bool> AddTeamAsync(Team team)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_tms(tm_id, tm_nm, tm_ds, tm_loc, tm_mb, tm_md, tm_cb, tm_cd) ");
            sb.Append("VALUES (@tm_id, @tm_nm, @tm_ds, @tm_loc, @tm_mb, @tm_md, @tm_mb, @tm_md);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var teamId = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    var teamName = cmd.Parameters.Add("@tm_nm", NpgsqlDbType.Text);
                    var teamDescription = cmd.Parameters.Add("@tm_ds", NpgsqlDbType.Text);
                    var teamLocationId = cmd.Parameters.Add("@tm_loc", NpgsqlDbType.Integer);
                    var actionBy = cmd.Parameters.Add("@tm_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@tm_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    teamId.Value = Guid.NewGuid().ToString();
                    teamName.Value = team.TeamName;
                    teamDescription.Value = team.TeamDescription ?? (object)DBNull.Value;
                    teamLocationId.Value = team.TeamLocationID ?? (object)DBNull.Value;
                    actionBy.Value = team.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value;
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

        public async Task<bool> EditTeamAsync(Team team)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_tms SET tm_nm=@tm_nm, tm_ds=@tm_ds, tm_loc=@tm_loc, tm_mb=@tm_mb, ");
            sb.Append("tm_md=@tm_md WHERE (tm_id=@tm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var teamId = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    var teamName = cmd.Parameters.Add("@tm_nm", NpgsqlDbType.Text);
                    var teamDescription = cmd.Parameters.Add("@tm_ds", NpgsqlDbType.Text);
                    var teamLocationId = cmd.Parameters.Add("@tm_loc", NpgsqlDbType.Integer);
                    var actionBy = cmd.Parameters.Add("@tm_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@tm_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    teamId.Value = team.TeamID;
                    teamName.Value = team.TeamName;
                    teamDescription.Value = team.TeamDescription ?? (object)DBNull.Value;
                    teamLocationId.Value = team.TeamLocationID ?? (object)DBNull.Value;
                    actionBy.Value = team.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value;
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

        public async Task<bool> DeleteTeamAsync(string teamId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_tms WHERE (tm_id=@tm_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var team_id = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    cmd.Prepare();
                    team_id.Value = teamId;
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

        //============== Team Members Action Methods =============================================//
        #region Team Members Action Methods
        public async Task<IEnumerable<TeamMember>> GetTeamMembersByTeamIdAsync(string teamId)
        {
            List<TeamMember> teamMembers = new List<TeamMember>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.id, p.fullname, p.sex, p.phone1, p.phone2, e.emp_id, e.emp_no_1, e.emp_no_2, e.official_email, ");
            sb.Append("m.tm_mb_id, m.tm_id, m.mbr_id, m.mbr_rl, t.tm_nm, m.tmb_mb, m.tmb_md, m.tmb_cb, m.tmb_cd FROM public.gst_tmbrs m ");
            sb.Append("INNER JOIN public.gst_tms t ON m.tm_id = t.tm_id INNER JOIN public.gst_prsns p ON m.mbr_id = p.id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON p.id = e.emp_id ");
            sb.Append("WHERE (m.tm_id = @tm_id) ORDER BY fullname;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var team_id = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    team_id.Value = teamId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            teamMembers.Add(new TeamMember()
                            {
                                TeamMemberID = reader["tm_mb_id"] == DBNull.Value ? 0 : (int)reader["tm_mb_id"],
                                TeamID = reader["tm_id"] == DBNull.Value ? String.Empty : reader["tm_id"].ToString(),
                                TeamName = reader["tm_nm"] == DBNull.Value ? String.Empty : reader["tm_nm"].ToString(),
                                MemberID = reader["mbr_id"] == DBNull.Value ? string.Empty : reader["mbr_id"].ToString(),
                                FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                MemberRole = reader["mbr_rl"] == DBNull.Value ? string.Empty : reader["mbr_rl"].ToString(),
                                EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString(),
                                EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                                Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                                PhoneNo = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                AltPhoneNo = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),
                                ModifiedBy = reader["tmb_mb"] == DBNull.Value ? String.Empty : reader["tmb_mb"].ToString(),
                                ModifiedDate = reader["tmb_md"] == DBNull.Value ? String.Empty : reader["tmb_md"].ToString(),
                                CreatedBy = reader["tmb_cb"] == DBNull.Value ? String.Empty : reader["tmb_cb"].ToString(),
                                CreatedDate = reader["tmb_cd"] == DBNull.Value ? String.Empty : reader["tmb_cd"].ToString(),

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
            return teamMembers;
        }

        public async Task<TeamMember> GetTeamMemberByIdAsync(int teamMemberId)
        {
            TeamMember teamMember = new TeamMember();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.id, p.fullname, p.sex, p.phone1, p.phone2, e.emp_id, e.emp_no_1, e.emp_no_2, e.official_email, ");
            sb.Append("m.tm_mb_id, m.tm_id, m.mbr_id, m.mbr_rl, t.tm_nm, m.tmb_mb, m.tmb_md, m.tmb_cb, m.tmb_cd FROM public.gst_tmbrs m ");
            sb.Append("INNER JOIN public.gst_tms t ON m.tm_id = t.tm_id INNER JOIN public.gst_prsns p ON m.mbr_id = p.id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON p.id = e.emp_id WHERE (m.tm_mb_id = @tm_mb_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var team_member_id = cmd.Parameters.Add("@tm_mb_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    team_member_id.Value = teamMemberId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            teamMember.TeamMemberID = reader["tm_mb_id"] == DBNull.Value ? 0 : (int)reader["tm_mb_id"];
                            teamMember.TeamID = reader["tm_id"] == DBNull.Value ? String.Empty : reader["tm_id"].ToString();
                            teamMember.TeamName = reader["tm_nm"] == DBNull.Value ? String.Empty : reader["tm_nm"].ToString();
                            teamMember.MemberID = reader["mbr_id"] == DBNull.Value ? string.Empty : reader["mbr_id"].ToString();
                            teamMember.FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString();
                            teamMember.MemberRole = reader["mbr_rl"] == DBNull.Value ? string.Empty : reader["mbr_rl"].ToString();
                            teamMember.EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : reader["emp_no_1"].ToString();
                            teamMember.EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString();
                            teamMember.Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString();
                            teamMember.PhoneNo = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString();
                            teamMember.AltPhoneNo = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString();
                            teamMember.Email = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString();
                            teamMember.ModifiedBy = reader["tmb_mb"] == DBNull.Value ? String.Empty : reader["tmb_mb"].ToString();
                            teamMember.ModifiedDate = reader["tmb_md"] == DBNull.Value ? String.Empty : reader["tmb_md"].ToString();
                            teamMember.CreatedBy = reader["tmb_cb"] == DBNull.Value ? String.Empty : reader["tmb_cb"].ToString();
                            teamMember.CreatedDate = reader["tmb_cd"] == DBNull.Value ? String.Empty : reader["tmb_cd"].ToString();

                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                throw new Exception(ex.Message);
            }
            return teamMember;
        }

        public async Task<IEnumerable<TeamMember>> GetTeamMembersByMemberNameAsync(string teamId, string memberName)
        {
            if (string.IsNullOrWhiteSpace(teamId)) { throw new ArgumentNullException($"Required parameter TeamID cannot be null."); }
            if (string.IsNullOrWhiteSpace(memberName)) { throw new ArgumentNullException($"Required parameter MemberName cannot be null."); }
            List<TeamMember> teamMembers = new List<TeamMember>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.id, p.fullname, p.sex, p.phone1, p.phone2, e.emp_id, e.emp_no_1, e.emp_no_2, e.official_email, m.tm_mb_id, ");
            sb.Append(" m.tm_id, m.mbr_id, m.mbr_rl, t.tm_nm, m.tmb_mb, m.tmb_md, m.tmb_cb, m.tmb_cd FROM public.gst_tmbrs m ");
            sb.Append("INNER JOIN public.gst_tms t ON m.tm_id = t.tm_id INNER JOIN public.gst_prsns p ON m.mbr_id = p.id ");
            sb.Append("INNER JOIN public.erm_emp_inf e ON p.id = e.emp_id ");
            sb.Append("WHERE (m.tm_id = @tm_id) AND (LOWER(fullname) LIKE '%'||LOWER(@fullname)||'%') ORDER BY fullname;");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var team_id = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    var member_name = cmd.Parameters.Add("@fullname", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    team_id.Value = teamId;
                    member_name.Value = memberName;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            teamMembers.Add(new TeamMember()
                            {
                                TeamMemberID = reader["tm_mb_id"] == DBNull.Value ? 0 : (int)reader["tm_mb_id"],
                                TeamID = reader["tm_id"] == DBNull.Value ? String.Empty : reader["tm_id"].ToString(),
                                TeamName = reader["tm_nm"] == DBNull.Value ? String.Empty : reader["tm_nm"].ToString(),
                                MemberID = reader["mbr_id"] == DBNull.Value ? string.Empty : reader["mbr_id"].ToString(),
                                FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                MemberRole = reader["mbr_rl"] == DBNull.Value ? string.Empty : reader["mbr_rl"].ToString(),
                                EmployeeNo1 = reader["emp_no_1"] == DBNull.Value? string.Empty : reader["emp_no_1"].ToString(),
                                EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : reader["emp_no_2"].ToString(),
                                Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                                PhoneNo = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                AltPhoneNo = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                Email = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),
                                ModifiedBy = reader["tmb_mb"] == DBNull.Value ? String.Empty : reader["tmb_mb"].ToString(),
                                ModifiedDate = reader["tmb_md"] == DBNull.Value ? String.Empty : reader["tmb_md"].ToString(),
                                CreatedBy = reader["tmb_cb"] == DBNull.Value ? String.Empty : reader["tmb_cb"].ToString(),
                                CreatedDate = reader["tmb_cd"] == DBNull.Value ? String.Empty : reader["tmb_cd"].ToString(),
                                
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
            return teamMembers;
        }

        public async Task<IEnumerable<Employee>> GetNonTeamMembersAsync(string teamId)
        {
            if (string.IsNullOrWhiteSpace(teamId)) { throw new ArgumentNullException($"Required parameter TeamID cannot be null."); }
            List<Employee> employees = new List<Employee>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.id, p.fullname, p.sex, p.phone1, p.phone2, e.emp_id, e.emp_no_1, e.emp_no_2, official_email ");
            sb.Append("FROM public.gst_prsns p INNER JOIN public.erm_emp_inf e ON p.id = e.emp_id ");
            sb.Append("WHERE e.emp_id NOT IN (SELECT mbr_id FROM public.gst_tmbrs WHERE tm_id = @tm_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var team_id = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    team_id.Value = teamId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee()
                            {
                                EmployeeID = reader["emp_id"] == DBNull.Value ? string.Empty : (reader["emp_id"]).ToString(),
                                EmployeeNo1 = reader["emp_no_1"] == DBNull.Value ? string.Empty : (reader["emp_no_1"]).ToString(),
                                EmployeeNo2 = reader["emp_no_2"] == DBNull.Value ? string.Empty : (reader["emp_no_2"]).ToString(),
                                PersonID = reader["id"] == DBNull.Value ? String.Empty : reader["id"].ToString(),
                                Sex = reader["sex"] == DBNull.Value ? string.Empty : reader["sex"].ToString(),
                                FullName = reader["fullname"] == DBNull.Value ? string.Empty : reader["fullname"].ToString(),
                                PhoneNo1 = reader["phone1"] == DBNull.Value ? string.Empty : reader["phone1"].ToString(),
                                PhoneNo2 = reader["phone2"] == DBNull.Value ? string.Empty : reader["phone2"].ToString(),
                                OfficialEmail = reader["official_email"] == DBNull.Value ? string.Empty : reader["official_email"].ToString(),
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
            return employees;
        }

        public async Task<bool> AddTeamMemberAsync(TeamMember teamMember)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.gst_tmbrs (tm_id, mbr_id, mbr_rl, tmb_mb, tmb_md, tmb_cb, tmb_cd) ");
            sb.Append("VALUES (@tm_id, @mbr_id, @mbr_rl, @tmb_mb, @tmb_md, @tmb_mb, @tmb_md);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var teamId = cmd.Parameters.Add("@tm_id", NpgsqlDbType.Text);
                    var memberId = cmd.Parameters.Add("@mbr_id", NpgsqlDbType.Text);
                    var teamMemberRole = cmd.Parameters.Add("@mbr_rl", NpgsqlDbType.Text);
                    var actionBy = cmd.Parameters.Add("@tmb_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@tmb_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    teamId.Value = teamMember.TeamID;
                    memberId.Value = teamMember.MemberID;
                    teamMemberRole.Value = teamMember.MemberRole;
                    actionBy.Value = teamMember.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value;
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

        public async Task<bool> EditTeamMemberAsync(TeamMember teamMember)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.gst_tmbrs SET mbr_rl=@mbr_rl, tmb_mb=@tmb_mb, tmb_md=@tmb_md ");
            sb.Append("WHERE (tm_mb_id=@tm_mb_id);");
            string query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var teamMemberId = cmd.Parameters.Add("@tm_mb_id", NpgsqlDbType.Integer);
                    var teamMemberRole = cmd.Parameters.Add("@mbr_rl", NpgsqlDbType.Text);
                    var actionBy = cmd.Parameters.Add("@tmb_mb", NpgsqlDbType.Text);
                    var modifiedDate = cmd.Parameters.Add("@tmb_md", NpgsqlDbType.Text);
                    cmd.Prepare();
                    teamMemberId.Value = teamMember.TeamMemberID;
                    teamMemberRole.Value = teamMember.MemberRole;
                    actionBy.Value = teamMember.ModifiedBy ?? (object)DBNull.Value;
                    modifiedDate.Value = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT" ?? (object)DBNull.Value;
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

        public async Task<bool> DeleteTeamMemberAsync(int teamMemberId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.gst_tmbrs WHERE (tm_mb_id = @tm_mb_id);";
            try
            {
                await conn.OpenAsync();
                //Insert data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var team_member_id = cmd.Parameters.Add("@tm_mb_id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    team_member_id.Value = teamMemberId;
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
