using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Base.Repositories.ClmRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.ClmRepositories
{
    public class CourseRepository : ICourseRepository
    {
        public IConfiguration _config { get; }
        public CourseRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Course Read Action Methods
        public async Task<IList<Course>> GetAllAsync()
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("ORDER BY c.crs_nm;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                await cmd.PrepareAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetByCourseIdAsync(int courseId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_id = @crs_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_id.Value = courseId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetByCourseTitleAsync(string courseTitle)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE LOWER(c.crs_nm) = LOWER(@crs_nm);");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_nm = cmd.Parameters.Add("@crs_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                crs_nm.Value = courseTitle;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> SearchByCourseTitleAsync(string courseTitle)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE (LOWER(c.crs_nm) LIKE '%'||LOWER(@crs_nm)||'%');");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_nm = cmd.Parameters.Add("@crs_nm", NpgsqlDbType.Text);
                await cmd.PrepareAsync();
                crs_nm.Value = courseTitle;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetByCourseTypeIdAsync(int courseTypeId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_typ_id = @crs_typ_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_typ_id.Value = courseTypeId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetBySubjectAreaIdAsync(int subjectAreaId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_sbj_id = @crs_sbj_id;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_sbj_id = cmd.Parameters.Add("@crs_sbj_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_sbj_id.Value = subjectAreaId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetByCourseLevelIdAsync(int courseLevelId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_lvl_id = @crs_lvl_id ");
            sb.Append("ORDER BY c.crs_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_lvl_id = cmd.Parameters.Add("@crs_lvl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_lvl_id.Value = courseLevelId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetByCourseTypeIdnCourseLevelIdAsync(int courseTypeId, int courseLevelId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_typ_id = @crs_typ_id AND c.crs_lvl_id = @crs_lvl_id ");
            sb.Append("ORDER BY c.crs_nm;");
            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                var crs_lvl_id = cmd.Parameters.Add("@crs_lvl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_typ_id.Value = courseTypeId;
                crs_lvl_id.Value = courseLevelId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetBySubjectAreaIdnCourseLevelIdAsync(int subjectAreaId, int courseLevelId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_sbj_id = @crs_sbj_id AND c.crs_lvl_id = @crs_lvl_id ");
            sb.Append("ORDER BY c.crs_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_sbj_id = cmd.Parameters.Add("@crs_sbj_id", NpgsqlDbType.Integer);
                var crs_lvl_id = cmd.Parameters.Add("@crs_lvl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_sbj_id.Value = subjectAreaId;
                crs_lvl_id.Value = courseLevelId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetByCourseTypeIdnSubjectAreaIdAsync(int courseTypeId, int subjectAreaId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_sbj_id = @crs_sbj_id AND c.crs_typ_id = @crs_typ_id ");
            sb.Append("ORDER BY c.crs_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                var crs_sbj_id = cmd.Parameters.Add("@crs_sbj_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_typ_id.Value = courseTypeId;
                crs_sbj_id.Value = subjectAreaId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        public async Task<IList<Course>> GetByCourseTypeIdnSubjectAreaIdnCourseLevelIdAsync(int courseTypeId, int subjectAreaId, int courseLevelId)
        {
            List<Course> coursesList = new List<Course>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.crs_id, c.crs_nm, c.crs_ds, c.crs_typ_id, c.crs_sbj_id, ");
            sb.Append("c.crs_lvl_id, c.crd_to, c.crs_src, c.dur_hr, c.rec_aud, c.req_enrl, ");
            sb.Append("c.upl_by, c.upl_dt, t.crs_typ_ds, s.crs_sbjs_ds, ");
            sb.Append("CASE WHEN c.crs_lvl_id = 1 THEN 'Beginner' ");
            sb.Append("WHEN c.crs_lvl_id = 2 THEN 'Intermediate' ");
            sb.Append("WHEN c.crs_lvl_id = 3 THEN 'Advanced' ELSE 'All' END crs_lvl_ds ");
            sb.Append("FROM public.clm_crs_inf c ");
            sb.Append("INNER JOIN public.clm_crs_typs t ON c.crs_typ_id = t.crs_typ_id ");
            sb.Append("INNER JOIN public.clm_crs_sbjs s ON c.crs_sbj_id = s.crs_sbjs_id ");
            sb.Append("WHERE c.crs_sbj_id = @crs_sbj_id AND c.crs_typ_id = @crs_typ_id ");
            sb.Append("AND c.crs_lvl_id = @crs_lvl_id ORDER BY c.crs_nm;");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                var crs_sbj_id = cmd.Parameters.Add("@crs_sbj_id", NpgsqlDbType.Integer);
                var crs_lvl_id = cmd.Parameters.Add("@crs_lvl_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                crs_typ_id.Value = courseTypeId;
                crs_sbj_id.Value = subjectAreaId;
                crs_lvl_id.Value = courseLevelId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    coursesList.Add(new Course()
                    {
                        CourseId = reader["crs_id"] == DBNull.Value ? 0 : (int)(reader["crs_id"]),
                        CourseTitle = reader["crs_nm"] == DBNull.Value ? string.Empty : reader["crs_nm"].ToString(),
                        CourseOverview = reader["crs_ds"] == DBNull.Value ? string.Empty : reader["crs_ds"].ToString(),
                        CourseTypeId = reader["crs_typ_id"] == DBNull.Value ? 0 : (int)reader["crs_typ_id"],
                        CourseTypeDescription = reader["crs_typ_ds"] == DBNull.Value ? string.Empty : (reader["crs_typ_ds"]).ToString(),
                        SubjectAreaId = reader["crs_sbj_id"] == DBNull.Value ? 0 : (int)reader["crs_sbj_id"],
                        SubjectAreaDescription = reader["crs_sbjs_ds"] == DBNull.Value ? string.Empty : reader["crs_sbjs_ds"].ToString(),
                        CourseLevelId = reader["crs_lvl_id"] == DBNull.Value ? 0 : (int)reader["crs_lvl_id"],
                        CourseLevelDescription = reader["crs_lvl_ds"] == DBNull.Value ? string.Empty : reader["crs_lvl_ds"].ToString(),
                        CreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        CourseSource = reader["crs_src"] == DBNull.Value ? string.Empty : reader["crs_src"].ToString(),
                        DurationInHours = reader["dur_hr"] == DBNull.Value ? 0 : (int)reader["dur_hr"],
                        RecommendedAudience = reader["rec_aud"] == DBNull.Value ? string.Empty : reader["rec_aud"].ToString(),
                        RequiresEnrollment = reader["req_enrl"] == DBNull.Value ? false : (bool)reader["req_enrl"],
                        UploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        UploadedTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                    });
                }
            }
            await conn.CloseAsync();
            return coursesList;
        }

        #endregion

        #region Course Write Action Methods
        public async Task<bool> AddAsync(Course course)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.clm_crs_inf(crs_nm, crs_ds, crs_typ_id, ");
            sb.Append("crs_sbj_id, crs_lvl_id, crd_to, crs_src, dur_hr, rec_aud, ");
            sb.Append("req_enrl, upl_by, upl_dt) ");
            sb.Append("VALUES (@crs_nm, @crs_ds, @crs_typ_id, @crs_sbj_id, ");
            sb.Append("@crs_lvl_id, @crd_to, @crs_src, @dur_hr, ");
            sb.Append("@rec_aud, @req_enrl, @upl_by, @upl_dt); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_nm = cmd.Parameters.Add("@crs_nm", NpgsqlDbType.Text);
                var crs_ds = cmd.Parameters.Add("@crs_ds", NpgsqlDbType.Text);
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                var crs_sbj_id = cmd.Parameters.Add("@crs_sbj_id", NpgsqlDbType.Integer);
                var crs_lvl_id = cmd.Parameters.Add("@crs_lvl_id", NpgsqlDbType.Integer);
                var crd_to = cmd.Parameters.Add("@crd_to", NpgsqlDbType.Text);
                var crs_src = cmd.Parameters.Add("@crs_src", NpgsqlDbType.Text);
                var dur_hr = cmd.Parameters.Add("@dur_hr", NpgsqlDbType.Integer);
                var rec_aud = cmd.Parameters.Add("@rec_aud", NpgsqlDbType.Text);
                var req_enrl = cmd.Parameters.Add("@req_enrl", NpgsqlDbType.Boolean);
                var upl_by = cmd.Parameters.Add("@upl_by", NpgsqlDbType.Text);
                var upl_dt = cmd.Parameters.Add("@upl_dt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                crs_nm.Value = course.CourseTitle;
                crs_ds.Value = course.CourseOverview;
                crs_typ_id.Value = course.CourseTypeId;
                crs_sbj_id.Value = course.SubjectAreaId;
                crs_lvl_id.Value = course.CourseLevelId;
                crd_to.Value = course.CreditTo ?? (object)DBNull.Value;
                crs_src.Value = course.CourseSource ?? (object)DBNull.Value;
                dur_hr.Value = course.DurationInHours ?? 0;
                rec_aud.Value = course.RecommendedAudience ?? (object)DBNull.Value;
                req_enrl.Value = course.RequiresEnrollment;
                upl_by.Value = course.UploadedBy ?? (object)DBNull.Value;
                upl_dt.Value = course.UploadedTime ?? DateTime.UtcNow;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(Course course)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.clm_crs_inf SET crs_nm=@crs_nm, ");
            sb.Append("crs_ds=@crs_ds, crs_typ_id=@crs_typ_id, ");
            sb.Append("crs_sbj_id=@crs_sbj_id, crs_lvl_id=@crs_lvl_id, ");
            sb.Append("crd_to=@crd_to, crs_src=@crs_src, dur_hr=@dur_hr, ");
            sb.Append("rec_aud=@rec_aud, req_enrl=@req_enrl WHERE crs_id = @crs_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
                var crs_nm = cmd.Parameters.Add("@crs_nm", NpgsqlDbType.Text);
                var crs_ds = cmd.Parameters.Add("@crs_ds", NpgsqlDbType.Text);
                var crs_typ_id = cmd.Parameters.Add("@crs_typ_id", NpgsqlDbType.Integer);
                var crs_sbj_id = cmd.Parameters.Add("@crs_sbj_id", NpgsqlDbType.Integer);
                var crs_lvl_id = cmd.Parameters.Add("@crs_lvl_id", NpgsqlDbType.Integer);
                var crd_to = cmd.Parameters.Add("@crd_to", NpgsqlDbType.Text);
                var crs_src = cmd.Parameters.Add("@crs_src", NpgsqlDbType.Text);
                var dur_hr = cmd.Parameters.Add("@dur_hr", NpgsqlDbType.Integer);
                var rec_aud = cmd.Parameters.Add("@rec_aud", NpgsqlDbType.Text);
                var req_enrl = cmd.Parameters.Add("@req_enrl", NpgsqlDbType.Boolean);
                cmd.Prepare();
                crs_id.Value = course.CourseId;
                crs_nm.Value = course.CourseTitle;
                crs_ds.Value = course.CourseOverview;
                crs_typ_id.Value = course.CourseTypeId;
                crs_sbj_id.Value = course.SubjectAreaId;
                crs_lvl_id.Value = course.CourseLevelId;
                crd_to.Value = course.CreditTo ?? (object)DBNull.Value;
                crs_src.Value = course.CourseSource ?? (object)DBNull.Value;
                dur_hr.Value = course.DurationInHours ?? 0;
                rec_aud.Value = course.RecommendedAudience ?? (object)DBNull.Value;
                req_enrl.Value = course.RequiresEnrollment;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int courseId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.clm_crs_inf WHERE(crs_id = @crs_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                crs_id.Value = courseId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        //public async Task<bool> UpdateTextFlagAsync(int courseId, bool hasText)
        //{
        //    int rows = 0;
        //    var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("UPDATE public.clm_crs_inf SET has_txt=@has_txt ");
        //    sb.Append("WHERE crs_id = @crs_id; ");

        //    string query = sb.ToString();

        //    await conn.OpenAsync();
        //    //Insert data
        //    using (var cmd = new NpgsqlCommand(query, conn))
        //    {
        //        var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
        //        var has_txt = cmd.Parameters.Add("@has_txt", NpgsqlDbType.Boolean);
        //        cmd.Prepare();
        //        crs_id.Value = courseId;
        //        has_txt.Value = hasText;

        //        rows = await cmd.ExecuteNonQueryAsync();
        //    }
        //    await conn.CloseAsync();
        //    return rows > 0;
        //}

        //public async Task<bool> UpdateAudioFlagAsync(int courseId, bool hasAudio)
        //{
        //    int rows = 0;
        //    var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("UPDATE public.clm_crs_inf SET has_aud=@has_aud ");
        //    sb.Append("WHERE crs_id = @crs_id; ");

        //    string query = sb.ToString();

        //    await conn.OpenAsync();
        //    //Insert data
        //    using (var cmd = new NpgsqlCommand(query, conn))
        //    {
        //        var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
        //        var has_aud = cmd.Parameters.Add("@has_aud", NpgsqlDbType.Boolean);
        //        cmd.Prepare();
        //        crs_id.Value = courseId;
        //        has_aud.Value = hasAudio;

        //        rows = await cmd.ExecuteNonQueryAsync();
        //    }
        //    await conn.CloseAsync();
        //    return rows > 0;
        //}

        //public async Task<bool> UpdateVideoFlagAsync(int courseId, bool hasVideo)
        //{
        //    int rows = 0;
        //    var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("UPDATE public.clm_crs_inf SET has_vid=@has_vid ");
        //    sb.Append("WHERE crs_id = @crs_id; ");

        //    string query = sb.ToString();

        //    await conn.OpenAsync();
        //    //Insert data
        //    using (var cmd = new NpgsqlCommand(query, conn))
        //    {
        //        var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
        //        var has_vid = cmd.Parameters.Add("@has_vid", NpgsqlDbType.Boolean);
        //        cmd.Prepare();
        //        crs_id.Value = courseId;
        //        has_vid.Value = hasVideo;

        //        rows = await cmd.ExecuteNonQueryAsync();
        //    }
        //    await conn.CloseAsync();
        //    return rows > 0;
        //}

        //public async Task<bool> UpdateCourseFlagAsync(int courseId, ContentFormat format, bool hasContent)
        //{
        //    int rows = 0;
        //    var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
        //    StringBuilder sb = new StringBuilder();
        //    string query = string.Empty;
        //    switch (format)
        //    {
        //        case ContentFormat.Text:
        //            sb.Append("UPDATE public.clm_crs_inf SET has_txt=@has_txt ");
        //            sb.Append("WHERE crs_id = @crs_id; ");
        //            query = sb.ToString();
        //            await conn.OpenAsync();
        //            //Insert data
        //            using (var cmd = new NpgsqlCommand(query, conn))
        //            {
        //                var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
        //                var has_txt = cmd.Parameters.Add("@has_txt", NpgsqlDbType.Boolean);
        //                cmd.Prepare();
        //                crs_id.Value = courseId;
        //                has_txt.Value = hasContent;
        //                rows = await cmd.ExecuteNonQueryAsync();
        //            }
        //            await conn.CloseAsync();
        //            break;
        //        case ContentFormat.Image:
        //            break;
        //        case ContentFormat.Audio:
        //            sb.Append("UPDATE public.clm_crs_inf SET has_aud=@has_aud ");
        //            sb.Append("WHERE crs_id = @crs_id; ");
        //            query = sb.ToString();
        //            await conn.OpenAsync();
        //            //Insert data
        //            using (var cmd = new NpgsqlCommand(query, conn))
        //            {
        //                var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
        //                var has_aud = cmd.Parameters.Add("@has_aud", NpgsqlDbType.Boolean);
        //                cmd.Prepare();
        //                crs_id.Value = courseId;
        //                has_aud.Value = hasContent;
        //                rows = await cmd.ExecuteNonQueryAsync();
        //            }
        //            await conn.CloseAsync();
        //            break;
        //        case ContentFormat.Video:
        //            sb.Append("UPDATE public.clm_crs_inf SET has_vid=@has_vid ");
        //            sb.Append("WHERE crs_id = @crs_id; ");
        //            query = sb.ToString();
        //            await conn.OpenAsync();
        //            //Insert data
        //            using (var cmd = new NpgsqlCommand(query, conn))
        //            {
        //                var crs_id = cmd.Parameters.Add("@crs_id", NpgsqlDbType.Integer);
        //                var has_vid = cmd.Parameters.Add("@has_vid", NpgsqlDbType.Boolean);
        //                cmd.Prepare();
        //                crs_id.Value = courseId;
        //                has_vid.Value = hasContent;
        //                rows = await cmd.ExecuteNonQueryAsync();
        //            }
        //            await conn.CloseAsync();
        //            break;
        //        default:
        //            break;
        //    }

        //    return rows > 0;
        //}

        #endregion
    }
}
