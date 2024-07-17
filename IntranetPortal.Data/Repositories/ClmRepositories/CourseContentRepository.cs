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
    public class CourseContentRepository : ICourseContentRepository
    {
        public IConfiguration _config { get; }
        public CourseContentRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Course Content Read Action Methods
        public async Task<IList<CourseContent>> GetByCourseIdAsync(int courseId)
        {
            List<CourseContent> courseContentsList = new List<CourseContent>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.crs_cnt_id, t.cnt_fmt_id, t.cnt_hdn, ");
            sb.Append("t.cnt_seq_no, t.cnt_ttl, t.cnt_ds, t.cnt_bdy, ");
            sb.Append("t.cnt_lnk, t.crd_to, t.cnt_src, t.dur_min, ");
            sb.Append("t.rec_for, t.has_ass, t.upl_by, t.upl_dt, ");
            sb.Append("t.clm_crs_id, t.cnt_pth, ");
            sb.Append("(SELECT c.crs_nm FROM public.clm_crs_inf c ");
            sb.Append("WHERE c.crs_id = t.clm_crs_id) as clm_crs_nm, ");
            sb.Append("CASE WHEN cnt_fmt_id = 0 THEN 'Text'  ");
            sb.Append("WHEN cnt_fmt_id = 1 THEN 'Image' ");
            sb.Append("WHEN cnt_fmt_id = 2 THEN 'Audio' ");
            sb.Append("WHEN cnt_fmt_id = 3 THEN 'Video'  ");
            sb.Append("WHEN cnt_fmt_id = 4 THEN 'Pdf'  ");
            sb.Append("ELSE 'Unknown' END cnt_fmt_ds  ");
            sb.Append("FROM public.clm_crs_cnt t ");
            sb.Append("WHERE t.clm_crs_id = @clm_crs_id ");
            sb.Append("ORDER BY t.cnt_seq_no; ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var clm_crs_id = cmd.Parameters.Add("@clm_crs_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                clm_crs_id.Value = courseId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseContentsList.Add(new CourseContent()
                    {
                        CourseContentId = reader["crs_cnt_id"] == DBNull.Value ? 0 : (long)reader["crs_cnt_id"],
                        ContentFormatId = reader["cnt_fmt_id"] == DBNull.Value ? 0 : (int)reader["cnt_fmt_id"],
                        ContentFormatDescription = reader["cnt_fmt_ds"] == DBNull.Value ? string.Empty : reader["cnt_fmt_ds"].ToString(),
                        ContentHeading = reader["cnt_hdn"] == DBNull.Value ? string.Empty : reader["cnt_hdn"].ToString(),
                        SequenceNo = reader["cnt_seq_no"] == DBNull.Value ? 0 : (int)reader["cnt_seq_no"],
                        ContentTitle = reader["cnt_ttl"] == DBNull.Value ? string.Empty : reader["cnt_ttl"].ToString(),
                        ContentDescription = reader["cnt_ds"] == DBNull.Value ? string.Empty : reader["cnt_ds"].ToString(),
                        ContentBody = reader["cnt_bdy"] == DBNull.Value ? string.Empty : reader["cnt_bdy"].ToString(),
                        ContentLink = reader["cnt_lnk"] == DBNull.Value ? string.Empty : reader["cnt_lnk"].ToString(),
                        ContentFullPath = reader["cnt_pth"] == DBNull.Value ? string.Empty : reader["cnt_pth"].ToString(),

                        ContentCreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        ContentSource = reader["cnt_src"] == DBNull.Value ? string.Empty : reader["cnt_src"].ToString(),
                        DurationInMinutes = reader["dur_min"] == DBNull.Value ? 0 : (int)reader["dur_min"],
                        ContentAudience = reader["rec_for"] == DBNull.Value ? string.Empty : reader["rec_for"].ToString(),
                        HasAssessment = reader["has_ass"] == DBNull.Value ? false : (bool)reader["has_ass"],
                        ContentUploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        ContentUploadTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                        CourseId = reader["clm_crs_id"] == DBNull.Value ? 0 : (int)(reader["clm_crs_id"]),
                        CourseTitle = reader["clm_crs_nm"] == DBNull.Value ? string.Empty : reader["clm_crs_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseContentsList;
        }

        public async Task<IList<CourseContent>> GetByCourseIdnFormatIdAsync(int courseId, int formatId)
        {
            List<CourseContent> courseContentsList = new List<CourseContent>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.crs_cnt_id, t.cnt_fmt_id, t.cnt_hdn, ");
            sb.Append("t.cnt_seq_no, t.cnt_ttl, t.cnt_ds, t.cnt_bdy, ");
            sb.Append("t.cnt_lnk, t.crd_to, t.cnt_src, t.dur_min, ");
            sb.Append("t.rec_for, t.has_ass, t.upl_by, t.upl_dt, ");
            sb.Append("t.clm_crs_id, t.cnt_pth, ");
            sb.Append("(SELECT c.crs_nm FROM public.clm_crs_inf c ");
            sb.Append("WHERE c.crs_id = t.clm_crs_id) as clm_crs_nm, ");
            sb.Append("CASE WHEN t.cnt_fmt_id = 0 THEN 'Text'  ");
            sb.Append("WHEN cnt_fmt_id = 1 THEN 'Image' ");
            sb.Append("WHEN cnt_fmt_id = 2 THEN 'Audio' ");
            sb.Append("WHEN t.cnt_fmt_id = 3 THEN 'Video'  ");
            sb.Append("WHEN cnt_fmt_id = 4 THEN 'Pdf'  ");
            sb.Append("ELSE 'Unknown' END cnt_fmt_ds  ");
            sb.Append("FROM public.clm_crs_cnt t ");
            sb.Append("WHERE t.clm_crs_id = @clm_crs_id ");
            sb.Append("AND t.cnt_fmt_id = @cnt_fmt_id ");
            sb.Append("ORDER BY t.cnt_seq_no; ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var clm_crs_id = cmd.Parameters.Add("@clm_crs_id", NpgsqlDbType.Integer);
                var cnt_fmt_id = cmd.Parameters.Add("@cnt_fmt_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                clm_crs_id.Value = courseId;
                cnt_fmt_id.Value = formatId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseContentsList.Add(new CourseContent()
                    {
                        CourseContentId = reader["crs_cnt_id"] == DBNull.Value ? 0 : (long)reader["crs_cnt_id"],
                        ContentFormatId = reader["cnt_fmt_id"] == DBNull.Value ? 0 : (int)reader["cnt_fmt_id"],
                        ContentFormatDescription = reader["cnt_fmt_ds"] == DBNull.Value ? string.Empty : reader["cnt_fmt_ds"].ToString(),
                        ContentHeading = reader["cnt_hdn"] == DBNull.Value ? string.Empty : reader["cnt_hdn"].ToString(),
                        SequenceNo = reader["cnt_seq_no"] == DBNull.Value ? 0 : (int)reader["cnt_seq_no"],
                        ContentTitle = reader["cnt_ttl"] == DBNull.Value ? string.Empty : reader["cnt_ttl"].ToString(),
                        ContentDescription = reader["cnt_ds"] == DBNull.Value ? string.Empty : reader["cnt_ds"].ToString(),
                        ContentBody = reader["cnt_bdy"] == DBNull.Value ? string.Empty : reader["cnt_bdy"].ToString(),
                        ContentLink = reader["cnt_lnk"] == DBNull.Value ? string.Empty : reader["cnt_lnk"].ToString(),
                        ContentFullPath = reader["cnt_pth"] == DBNull.Value ? string.Empty : reader["cnt_pth"].ToString(),
                        ContentCreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        ContentSource = reader["cnt_src"] == DBNull.Value ? string.Empty : reader["cnt_src"].ToString(),
                        DurationInMinutes = reader["dur_min"] == DBNull.Value ? 0 : (int)reader["dur_min"],
                        ContentAudience = reader["rec_for"] == DBNull.Value ? string.Empty : reader["rec_for"].ToString(),
                        HasAssessment = reader["has_ass"] == DBNull.Value ? false : (bool)reader["has_ass"],
                        ContentUploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        ContentUploadTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                        CourseId = reader["clm_crs_id"] == DBNull.Value ? 0 : (int)(reader["clm_crs_id"]),
                        CourseTitle = reader["clm_crs_nm"] == DBNull.Value ? string.Empty : reader["clm_crs_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseContentsList;
        }

        public async Task<IList<CourseContent>> GetByIdAsync(long courseContentId)
        {
            List<CourseContent> courseContentsList = new List<CourseContent>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.crs_cnt_id, t.cnt_fmt_id, t.cnt_hdn, ");
            sb.Append("t.cnt_seq_no, t.cnt_ttl, t.cnt_ds, t.cnt_bdy, ");
            sb.Append("t.cnt_lnk, t.crd_to, t.cnt_src, t.dur_min, ");
            sb.Append("t.rec_for, t.has_ass, t.upl_by, t.upl_dt, ");
            sb.Append("t.clm_crs_id, t.cnt_pth, ");
            sb.Append("(SELECT c.crs_nm FROM public.clm_crs_inf c ");
            sb.Append("WHERE c.crs_id = t.clm_crs_id) as clm_crs_nm, ");
            sb.Append("CASE WHEN cnt_fmt_id = 0 THEN 'Text'  ");
            sb.Append("WHEN cnt_fmt_id = 1 THEN 'Image' ");
            sb.Append("WHEN cnt_fmt_id = 2 THEN 'Audio' ");
            sb.Append("WHEN cnt_fmt_id = 3 THEN 'Video'  ");
            sb.Append("WHEN cnt_fmt_id = 4 THEN 'Pdf'  ");
            sb.Append("ELSE 'Unknown' END cnt_fmt_ds  ");
            sb.Append("FROM public.clm_crs_cnt t ");
            sb.Append("WHERE  t.crs_cnt_id = @crs_cnt_id ");
            sb.Append("ORDER BY t.cnt_seq_no; ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var crs_cnt_id = cmd.Parameters.Add("@crs_cnt_id", NpgsqlDbType.Bigint);
                await cmd.PrepareAsync();
                crs_cnt_id.Value = courseContentId;
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseContentsList.Add(new CourseContent()
                    {
                        CourseContentId = reader["crs_cnt_id"] == DBNull.Value ? 0 : (long)reader["crs_cnt_id"],
                        ContentFormatId = reader["cnt_fmt_id"] == DBNull.Value ? 0 : (int)reader["cnt_fmt_id"],
                        ContentFormatDescription = reader["cnt_fmt_ds"] == DBNull.Value ? string.Empty : reader["cnt_fmt_ds"].ToString(),
                        ContentHeading = reader["cnt_hdn"] == DBNull.Value ? string.Empty : reader["cnt_hdn"].ToString(),
                        SequenceNo = reader["cnt_seq_no"] == DBNull.Value ? 0 : (int)reader["cnt_seq_no"],
                        ContentTitle = reader["cnt_ttl"] == DBNull.Value ? string.Empty : reader["cnt_ttl"].ToString(),
                        ContentDescription = reader["cnt_ds"] == DBNull.Value ? string.Empty : reader["cnt_ds"].ToString(),
                        ContentBody = reader["cnt_bdy"] == DBNull.Value ? string.Empty : reader["cnt_bdy"].ToString(),
                        ContentLink = reader["cnt_lnk"] == DBNull.Value ? string.Empty : reader["cnt_lnk"].ToString(),
                        ContentFullPath = reader["cnt_pth"] == DBNull.Value ? string.Empty : reader["cnt_pth"].ToString(),
                        ContentCreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        ContentSource = reader["cnt_src"] == DBNull.Value ? string.Empty : reader["cnt_src"].ToString(),
                        DurationInMinutes = reader["dur_min"] == DBNull.Value ? 0 : (int)reader["dur_min"],
                        ContentAudience = reader["rec_for"] == DBNull.Value ? string.Empty : reader["rec_for"].ToString(),
                        HasAssessment = reader["has_ass"] == DBNull.Value ? false : (bool)reader["has_ass"],
                        ContentUploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        ContentUploadTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                        CourseId = reader["clm_crs_id"] == DBNull.Value ? 0 : (int)(reader["clm_crs_id"]),
                        CourseTitle = reader["clm_crs_nm"] == DBNull.Value ? string.Empty : reader["clm_crs_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseContentsList;
        }

        public async Task<IList<CourseContent>> GetByContentTitleAsync(int courseId, string courseContentTitle)
        {
            List<CourseContent> courseContentsList = new List<CourseContent>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.crs_cnt_id, t.cnt_fmt_id, t.cnt_hdn, ");
            sb.Append("t.cnt_seq_no, t.cnt_ttl, t.cnt_ds, t.cnt_bdy, ");
            sb.Append("t.cnt_lnk, t.crd_to, t.cnt_src, t.dur_min, ");
            sb.Append("t.rec_for, t.has_ass, t.upl_by, t.upl_dt, ");
            sb.Append("t.clm_crs_id, t.cnt_pth, ");
            sb.Append("(SELECT c.crs_nm FROM public.clm_crs_inf c ");
            sb.Append("WHERE c.crs_id = t.clm_crs_id) as clm_crs_nm, ");
            sb.Append("CASE WHEN cnt_fmt_id = 0 THEN 'Text'  ");
            sb.Append("WHEN cnt_fmt_id = 1 THEN 'Image' ");
            sb.Append("WHEN cnt_fmt_id = 2 THEN 'Audio' ");
            sb.Append("WHEN cnt_fmt_id = 3 THEN 'Video'  ");
            sb.Append("WHEN cnt_fmt_id = 4 THEN 'Pdf'  ");
            sb.Append("ELSE 'Unknown' END cnt_fmt_ds  ");
            sb.Append("FROM public.clm_crs_cnt t ");
            sb.Append("WHERE t.cnt_ttl = @cnt_ttle ");
            sb.Append("AND t.clm_crs_id = @clm_crs_id ");
            sb.Append("ORDER BY t.cnt_seq_no; ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var cnt_ttle = cmd.Parameters.Add("@cnt_ttle", NpgsqlDbType.Text);
                var clm_crs_id = cmd.Parameters.Add("@clm_crs_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                cnt_ttle.Value = courseContentTitle;
                clm_crs_id.Value = courseId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseContentsList.Add(new CourseContent()
                    {
                        CourseContentId = reader["crs_cnt_id"] == DBNull.Value ? 0 : (long)reader["crs_cnt_id"],
                        ContentFormatId = reader["cnt_fmt_id"] == DBNull.Value ? 0 : (int)reader["cnt_fmt_id"],
                        ContentFormatDescription = reader["cnt_fmt_ds"] == DBNull.Value ? string.Empty : reader["cnt_fmt_ds"].ToString(),
                        ContentHeading = reader["cnt_hdn"] == DBNull.Value ? string.Empty : reader["cnt_hdn"].ToString(),
                        SequenceNo = reader["cnt_seq_no"] == DBNull.Value ? 0 : (int)reader["cnt_seq_no"],
                        ContentTitle = reader["cnt_ttl"] == DBNull.Value ? string.Empty : reader["cnt_ttl"].ToString(),
                        ContentDescription = reader["cnt_ds"] == DBNull.Value ? string.Empty : reader["cnt_ds"].ToString(),
                        ContentBody = reader["cnt_bdy"] == DBNull.Value ? string.Empty : reader["cnt_bdy"].ToString(),
                        ContentLink = reader["cnt_lnk"] == DBNull.Value ? string.Empty : reader["cnt_lnk"].ToString(),
                        ContentFullPath = reader["cnt_pth"] == DBNull.Value ? string.Empty : reader["cnt_pth"].ToString(),

                        ContentCreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        ContentSource = reader["cnt_src"] == DBNull.Value ? string.Empty : reader["cnt_src"].ToString(),
                        DurationInMinutes = reader["dur_min"] == DBNull.Value ? 0 : (int)reader["dur_min"],
                        ContentAudience = reader["rec_for"] == DBNull.Value ? string.Empty : reader["rec_for"].ToString(),
                        HasAssessment = reader["has_ass"] == DBNull.Value ? false : (bool)reader["has_ass"],
                        ContentUploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        ContentUploadTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                        CourseId = reader["clm_crs_id"] == DBNull.Value ? 0 : (int)(reader["clm_crs_id"]),
                        CourseTitle = reader["clm_crs_nm"] == DBNull.Value ? string.Empty : reader["clm_crs_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseContentsList;
        }

        public async Task<IList<CourseContent>> GetByHeadingAsync(int courseId, string contentHeading)
        {
            List<CourseContent> courseContentsList = new List<CourseContent>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.crs_cnt_id, t.cnt_fmt_id, t.cnt_hdn, ");
            sb.Append("t.cnt_seq_no, t.cnt_ttl, t.cnt_ds, t.cnt_bdy, ");
            sb.Append("t.cnt_lnk, t.crd_to, t.cnt_src, t.dur_min, ");
            sb.Append("t.rec_for, t.has_ass, t.upl_by, t.upl_dt, ");
            sb.Append("t.clm_crs_id, t.cnt_pth, ");
            sb.Append("(SELECT c.crs_nm FROM public.clm_crs_inf c ");
            sb.Append("WHERE c.crs_id = t.clm_crs_id) as clm_crs_nm, ");
            sb.Append("CASE WHEN cnt_fmt_id = 0 THEN 'Text'  ");
            sb.Append("WHEN cnt_fmt_id = 1 THEN 'Image' ");
            sb.Append("WHEN cnt_fmt_id = 2 THEN 'Audio' ");
            sb.Append("WHEN cnt_fmt_id = 3 THEN 'Video'  ");
            sb.Append("WHEN cnt_fmt_id = 4 THEN 'Pdf'  ");
            sb.Append("ELSE 'Unknown' END cnt_fmt_ds  ");
            sb.Append("FROM public.clm_crs_cnt t ");
            sb.Append("WHERE t.cnt_hdn = @cnt_hdn ");
            sb.Append("AND t.clm_crs_id = @clm_crs_id ");
            sb.Append("ORDER BY t.cnt_seq_no; ");

            string query = sb.ToString();
            await conn.OpenAsync();
            // Retrieve all rows
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                var cnt_hdn = cmd.Parameters.Add("@cnt_hdn", NpgsqlDbType.Text);
                var clm_crs_id = cmd.Parameters.Add("@clm_crs_id", NpgsqlDbType.Integer);
                await cmd.PrepareAsync();
                cnt_hdn.Value = contentHeading;
                clm_crs_id.Value = courseId;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    courseContentsList.Add(new CourseContent()
                    {
                        CourseContentId = reader["crs_cnt_id"] == DBNull.Value ? 0 : (long)reader["crs_cnt_id"],
                        ContentFormatId = reader["cnt_fmt_id"] == DBNull.Value ? 0 : (int)reader["cnt_fmt_id"],
                        ContentFormatDescription = reader["cnt_fmt_ds"] == DBNull.Value ? string.Empty : reader["cnt_fmt_ds"].ToString(),
                        ContentHeading = reader["cnt_hdn"] == DBNull.Value ? string.Empty : reader["cnt_hdn"].ToString(),
                        SequenceNo = reader["cnt_seq_no"] == DBNull.Value ? 0 : (int)reader["cnt_seq_no"],
                        ContentTitle = reader["cnt_ttl"] == DBNull.Value ? string.Empty : reader["cnt_ttl"].ToString(),
                        ContentDescription = reader["cnt_ds"] == DBNull.Value ? string.Empty : reader["cnt_ds"].ToString(),
                        ContentBody = reader["cnt_bdy"] == DBNull.Value ? string.Empty : reader["cnt_bdy"].ToString(),
                        ContentLink = reader["cnt_lnk"] == DBNull.Value ? string.Empty : reader["cnt_lnk"].ToString(),
                        ContentFullPath = reader["cnt_pth"] == DBNull.Value ? string.Empty : reader["cnt_pth"].ToString(),
                        ContentCreditTo = reader["crd_to"] == DBNull.Value ? string.Empty : reader["crd_to"].ToString(),
                        ContentSource = reader["cnt_src"] == DBNull.Value ? string.Empty : reader["cnt_src"].ToString(),
                        DurationInMinutes = reader["dur_min"] == DBNull.Value ? 0 : (int)reader["dur_min"],
                        ContentAudience = reader["rec_for"] == DBNull.Value ? string.Empty : reader["rec_for"].ToString(),
                        HasAssessment = reader["has_ass"] == DBNull.Value ? false : (bool)reader["has_ass"],
                        ContentUploadedBy = reader["upl_by"] == DBNull.Value ? string.Empty : reader["upl_by"].ToString(),
                        ContentUploadTime = reader["upl_dt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["upl_dt"],
                        CourseId = reader["clm_crs_id"] == DBNull.Value ? 0 : (int)(reader["clm_crs_id"]),
                        CourseTitle = reader["clm_crs_nm"] == DBNull.Value ? string.Empty : reader["clm_crs_nm"].ToString(),
                    });
                }
            }
            await conn.CloseAsync();
            return courseContentsList;
        }

        #endregion

        #region Course Content Write Action Methods
        public async Task<bool> AddAsync(CourseContent courseContent)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.clm_crs_cnt(cnt_fmt_id, ");
            sb.Append("cnt_hdn, cnt_seq_no, cnt_ttl, cnt_ds, ");
            sb.Append("crd_to, cnt_src, dur_min, rec_for, ");
            sb.Append("upl_by, upl_dt, clm_crs_id, cnt_pth) ");
            sb.Append("VALUES (@cnt_fmt_id, @cnt_hdn, @cnt_seq_no, ");
            sb.Append("@cnt_ttl, @cnt_ds, @crd_to, ");
            sb.Append("@cnt_src, @dur_min, @rec_for, ");
            sb.Append("@upl_by, @upl_dt, @clm_crs_id, @cnt_pth); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var cnt_fmt_id = cmd.Parameters.Add("@cnt_fmt_id", NpgsqlDbType.Integer);
                var cnt_hdn = cmd.Parameters.Add("@cnt_hdn", NpgsqlDbType.Text);
                var cnt_seq_no = cmd.Parameters.Add("@cnt_seq_no", NpgsqlDbType.Integer);
                var cnt_ttl = cmd.Parameters.Add("@cnt_ttl", NpgsqlDbType.Text);
                var cnt_ds = cmd.Parameters.Add("@cnt_ds", NpgsqlDbType.Text);
                var crd_to = cmd.Parameters.Add("@crd_to", NpgsqlDbType.Text);
                var cnt_src = cmd.Parameters.Add("@cnt_src", NpgsqlDbType.Text);
                var dur_min = cmd.Parameters.Add("@dur_min", NpgsqlDbType.Integer);
                var rec_for = cmd.Parameters.Add("@rec_for", NpgsqlDbType.Text);
                var upl_by = cmd.Parameters.Add("@upl_by", NpgsqlDbType.Text);
                var upl_dt = cmd.Parameters.Add("@upl_dt", NpgsqlDbType.TimestampTz);
                var clm_crs_id = cmd.Parameters.Add("@clm_crs_id", NpgsqlDbType.Integer);
                var cnt_pth = cmd.Parameters.Add("cnt_pth", NpgsqlDbType.Text);
                cmd.Prepare();
                cnt_fmt_id.Value = courseContent.ContentFormatId;
                cnt_hdn.Value = courseContent.ContentHeading ?? (object)DBNull.Value;
                cnt_seq_no.Value = courseContent.SequenceNo;
                cnt_ttl.Value = courseContent.ContentTitle;
                cnt_ds.Value = courseContent.ContentDescription ?? (object)DBNull.Value;
                crd_to.Value = courseContent.ContentCreditTo ?? (object)DBNull.Value;
                cnt_src.Value = courseContent.ContentSource ?? (object)DBNull.Value;
                dur_min.Value = courseContent.DurationInMinutes;
                rec_for.Value = courseContent.ContentAudience ?? (object)DBNull.Value;
                upl_by.Value = courseContent.ContentUploadedBy ?? (object)DBNull.Value;
                upl_dt.Value = courseContent.ContentUploadTime ?? (object)DBNull.Value;
                clm_crs_id.Value = courseContent.CourseId;
                cnt_pth.Value = courseContent.ContentFullPath ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        public async Task<bool> UpdateAsync(CourseContent courseContent)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.clm_crs_cnt SET cnt_fmt_id=@cnt_fmt_id, ");
            sb.Append("cnt_hdn=@cnt_hdn, cnt_seq_no=@cnt_seq_no, cnt_ttl=@cnt_ttl, ");
            sb.Append("cnt_ds=@cnt_ds, crd_to=@crd_to, cnt_src=@cnt_src, ");
            sb.Append("dur_min=@dur_min, rec_for=@rec_for, clm_crs_id=@clm_crs_id ");
            sb.Append("WHERE crs_cnt_id = @crs_cnt_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_cnt_id = cmd.Parameters.Add("@crs_cnt_id", NpgsqlDbType.Bigint);
                var cnt_fmt_id = cmd.Parameters.Add("@cnt_fmt_id", NpgsqlDbType.Integer);
                var cnt_hdn = cmd.Parameters.Add("@cnt_hdn", NpgsqlDbType.Text);
                var cnt_seq_no = cmd.Parameters.Add("@cnt_seq_no", NpgsqlDbType.Integer);
                var cnt_ttl = cmd.Parameters.Add("@cnt_ttl", NpgsqlDbType.Text);
                var cnt_ds = cmd.Parameters.Add("@cnt_ds", NpgsqlDbType.Text);
                var crd_to = cmd.Parameters.Add("@crd_to", NpgsqlDbType.Text);
                var cnt_src = cmd.Parameters.Add("@cnt_src", NpgsqlDbType.Text);
                var dur_min = cmd.Parameters.Add("@dur_min", NpgsqlDbType.Integer);
                var rec_for = cmd.Parameters.Add("@rec_for", NpgsqlDbType.Text);
                var clm_crs_id = cmd.Parameters.Add("@clm_crs_id", NpgsqlDbType.Integer);
                cmd.Prepare();
                crs_cnt_id.Value = courseContent.CourseContentId;
                cnt_fmt_id.Value = courseContent.ContentFormatId;
                cnt_hdn.Value = courseContent.ContentHeading ?? (object)DBNull.Value;
                cnt_seq_no.Value = courseContent.SequenceNo;
                cnt_ttl.Value = courseContent.ContentTitle;
                cnt_ds.Value = courseContent.ContentDescription ?? (object)DBNull.Value;
                crd_to.Value = courseContent.ContentCreditTo ?? (object)DBNull.Value;
                cnt_src.Value = courseContent.ContentSource ?? (object)DBNull.Value;
                dur_min.Value = courseContent.DurationInMinutes;
                rec_for.Value = courseContent.ContentAudience ?? (object)DBNull.Value;
                clm_crs_id.Value = courseContent.CourseId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        public async Task<bool> DeleteAsync(long courseContentId)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));

            string query = "DELETE FROM public.clm_crs_cnt WHERE(crs_cnt_id = @crs_cnt_id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_cnt_id = cmd.Parameters.Add("@crs_cnt_id", NpgsqlDbType.Bigint);
                cmd.Prepare();
                crs_cnt_id.Value = courseContentId;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        public async Task<bool> UpdateTextAsync(long courseContentId, string textContent)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.clm_crs_cnt SET cnt_bdy=@cnt_bdy ");
            sb.Append("WHERE crs_cnt_id = @crs_cnt_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_cnt_id = cmd.Parameters.Add("@crs_cnt_id", NpgsqlDbType.Bigint);
                var cnt_bdy = cmd.Parameters.Add("@cnt_bdy", NpgsqlDbType.Text);
                cmd.Prepare();
                crs_cnt_id.Value = courseContentId;
                cnt_bdy.Value = textContent ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        public async Task<bool> UpdateLinkAsync(long courseContentId, string contentLink, string contentFullPath)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.clm_crs_cnt SET cnt_lnk=@cnt_lnk, ");
            sb.Append("cnt_pth=@cnt_pth ");
            sb.Append("WHERE crs_cnt_id = @crs_cnt_id; ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var crs_cnt_id = cmd.Parameters.Add("@crs_cnt_id", NpgsqlDbType.Bigint);
                var cnt_lnk = cmd.Parameters.Add("@cnt_lnk", NpgsqlDbType.Text);
                var cnt_pth = cmd.Parameters.Add("@cnt_pth", NpgsqlDbType.Text);
                cmd.Prepare();
                crs_cnt_id.Value = courseContentId;
                cnt_lnk.Value = contentLink ?? (object)DBNull.Value;
                cnt_pth.Value = contentFullPath ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion
    }
}
