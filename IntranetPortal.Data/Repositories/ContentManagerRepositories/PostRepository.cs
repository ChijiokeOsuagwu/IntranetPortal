using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Data.Repositories.ContentManagerRepositories
{
    public class PostRepository : IPostRepository
    {
        public IConfiguration _config { get; }
        public PostRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        //============= Posts Read Action Methods =====================//
        #region Post Read Action Methods
        public async Task<Post> GetPostByIdAsync(int id)
        {
            Post post = new Post();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (id < 1) { return null; }
            sb.Append("SELECT id, title, summary, details, imgp, mdby, ");
            sb.Append("crby, typ_id, enable_com, is_hdn, crdt, mddt, ");
            sb.Append("hs_cm, hs_md, dtl_rw FROM public.pcm_psts ");
            sb.Append("WHERE id = @id;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var postId = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    postId.Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            post.PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]);
                            post.PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString();
                            post.PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString();
                            post.ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString();
                            post.ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString();
                            post.ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"];
                            post.CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString();
                            post.CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"];
                            post.EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"];
                            post.IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"];
                            post.PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"];
                            post.HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"];
                            post.HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"];
                            post.PostDetails = reader["details"] == DBNull.Value ? String.Empty : reader["details"].ToString();
                            post.PostDetailsRaw = reader["dtl_rw"] == DBNull.Value ? String.Empty : reader["dtl_rw"].ToString();

                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByLoginIdAsync";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                return null;
            }
            return post;
        }

        public async Task<IList<Post>> GetAllAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts ");
            sb.Append("WHERE (typ_id != 0) ORDER BY crdt DESC;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
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
            return postlist;

        }

        public async Task<IList<Post>> GetPostsWithoutBannersAndAnnouncementsAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts ");
            sb.Append("WHERE (typ_id != 0 AND typ_id != 3) ORDER BY crdt DESC;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
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
            return postlist;

        }

        public async Task<IList<Post>> GetUnhiddenPostsWithoutBannersAndAnnouncementsAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts ");
            sb.Append("WHERE (typ_id != 0 AND typ_id != 3 AND is_hdn = false) ");
            sb.Append("ORDER BY crdt DESC;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
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
            return postlist;

        }

        public async Task<IList<Post>> GetByTypeIdAsync(int typeId)
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, summary, details, imgp, mdby, crby, typ_id, ");
            sb.Append("enable_com, is_hdn, crdt, mddt, hs_cm, hs_md, dtl_rw ");
            sb.Append("FROM public.pcm_psts WHERE (typ_id = @typ_id) ");
            sb.Append("AND (typ_id != 0) ORDER BY crdt DESC;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    typ_id.Value = typeId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            postlist.Add(new Post()
                            {
                                PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                                PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                                PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                                ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                                ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                                ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                                CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                                CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                                EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                                IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                                PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                                HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                                HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
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
            return postlist;
        }

        public async Task<IList<Post>> GetByTitleAsync(string postTitle)
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, summary, details, imgp, mdby, crby, typ_id, ");
            sb.Append("enable_com, is_hdn, crdt, mddt, hs_cm, hs_md, dtl_rw ");
            sb.Append("FROM public.pcm_psts  WHERE (typ_id != 0) ");
            sb.Append("AND (LOWER(title) LIKE '%'||LOWER(@title)||'%') ");
            sb.Append("ORDER BY crdt DESC;");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                    await cmd.PrepareAsync();
                    title.Value = postTitle;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            postlist.Add(new Post()
                            {
                                PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                                PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                                PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                                ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                                ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                                ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                                CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                                CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                                EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                                IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                                PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                                HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                                HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
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
            return postlist;
        }

        #endregion

        //============= Posts Write Action Methods ====================//
        #region Post Write Action Methods

        public async Task<bool> AddPostAsync(Post post)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO public.pcm_psts( title, summary, ");
            sb.Append("details, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append("is_hdn, crdt, mddt, dtl_rw) ");
            sb.Append("VALUES (@title, @summary, @details, @imgp, ");
            sb.Append("@mdby, @crby, @typ_id, @enable_com, ");
            sb.Append("@is_hdn, @crdt, @mddt, @dtl_rw); ");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                var summary = cmd.Parameters.Add("@summary", NpgsqlDbType.Text);
                var details = cmd.Parameters.Add("@details", NpgsqlDbType.Text);
                var imgp = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                var mdby = cmd.Parameters.Add("@mdby", NpgsqlDbType.Text);
                var mddt = cmd.Parameters.Add("@mddt", NpgsqlDbType.TimestampTz);
                var crby = cmd.Parameters.Add("@crby", NpgsqlDbType.Text);
                var crdt = cmd.Parameters.Add("@crdt", NpgsqlDbType.TimestampTz);
                var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var enable_com = cmd.Parameters.Add("@enable_com", NpgsqlDbType.Boolean);
                var is_hdn = cmd.Parameters.Add("@is_hdn", NpgsqlDbType.Boolean);
                var dtl_rw = cmd.Parameters.Add("@dtl_rw", NpgsqlDbType.Text);
                cmd.Prepare();
                title.Value = post.PostTitle;
                summary.Value = post.PostSummary;
                details.Value = post.PostDetails ?? (object)DBNull.Value;
                imgp.Value = post.ImagePath ?? (object)DBNull.Value;
                mdby.Value = post.ModifiedBy;
                mddt.Value = post.ModifiedDate;
                crby.Value = post.CreatedBy;
                crdt.Value = post.CreatedDate;
                typ_id.Value = post.PostTypeId;
                enable_com.Value = post.EnableComment;
                is_hdn.Value = post.IsHidden;
                dtl_rw.Value = post.PostDetailsRaw ?? (object)DBNull.Value;

                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"DELETE FROM public.pcm_psts WHERE (id = @id);";
            try
            {
                await conn.OpenAsync();
                //Delete data
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var postid = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    postid.Value = id;
                    rows = await cmd.ExecuteNonQueryAsync();
                    await conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_AddApplicationUserAsync";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                rows = -1;
            }
            return rows > 0;
        }

        public async Task<bool> EditPostAsync(Post post)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pcm_psts SET title=@title, summary=@summary, ");
            sb.Append("details=@details, imgp=@imgp, mdby=@mdby, typ_id=@typ_id, ");
            sb.Append("enable_com=@enable_com, is_hdn=@is_hdn, mddt=@mddt, ");
            sb.Append("dtl_rw=@dtl_rw WHERE (id=@id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                var summary = cmd.Parameters.Add("@summary", NpgsqlDbType.Text);
                var details = cmd.Parameters.Add("@details", NpgsqlDbType.Text);
                var imgp = cmd.Parameters.Add("@imgp", NpgsqlDbType.Text);
                var mdby = cmd.Parameters.Add("@mdby", NpgsqlDbType.Text);
                var mddt = cmd.Parameters.Add("@mddt", NpgsqlDbType.TimestampTz);
                var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var enable_com = cmd.Parameters.Add("@enable_com", NpgsqlDbType.Boolean);
                var is_hdn = cmd.Parameters.Add("@is_hdn", NpgsqlDbType.Boolean);
                var dtl_rw = cmd.Parameters.Add("@dtl_rw", NpgsqlDbType.Text);
                cmd.Prepare();
                id.Value = post.PostId;
                title.Value = post.PostTitle;
                summary.Value = post.PostSummary;
                typ_id.Value = post.PostTypeId;
                details.Value = post.PostDetails ?? (object)DBNull.Value;
                imgp.Value = post.ImagePath ?? (object)DBNull.Value;
                mdby.Value = post.ModifiedBy;
                mddt.Value = post.ModifiedDate;
                enable_com.Value = post.EnableComment;
                is_hdn.Value = post.IsHidden;
                dtl_rw.Value = post.PostDetailsRaw ?? (object)DBNull.Value;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }

        public async Task<bool> EditPostWithoutImageAsync(Post post)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE public.pcm_psts SET title=@title, summary=@summary, ");
            sb.Append("details=@details, mdby=@mdby, typ_id=@typ_id, ");
            sb.Append("enable_com=@enable_com, is_hdn=@is_hdn, mddt=@mddt, ");
            sb.Append("dtl_rw=@dtl_rw WHERE (id=@id);");

            string query = sb.ToString();

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                var title = cmd.Parameters.Add("@title", NpgsqlDbType.Text);
                var summary = cmd.Parameters.Add("@summary", NpgsqlDbType.Text);
                var details = cmd.Parameters.Add("@details", NpgsqlDbType.Text);
                var mdby = cmd.Parameters.Add("@mdby", NpgsqlDbType.Text);
                var mddt = cmd.Parameters.Add("@mddt", NpgsqlDbType.TimestampTz);
                var typ_id = cmd.Parameters.Add("@typ_id", NpgsqlDbType.Integer);
                var enable_com = cmd.Parameters.Add("@enable_com", NpgsqlDbType.Boolean);
                var is_hdn = cmd.Parameters.Add("@is_hdn", NpgsqlDbType.Boolean);
                var dtl_rw = cmd.Parameters.Add("@dtl_rw", NpgsqlDbType.Text);
                cmd.Prepare();
                id.Value = post.PostId;
                title.Value = post.PostTitle;
                summary.Value = post.PostSummary;
                typ_id.Value = post.PostTypeId;
                details.Value = post.PostDetails ?? (object)DBNull.Value;
                mdby.Value = post.ModifiedBy;
                mddt.Value = post.ModifiedDate;
                enable_com.Value = post.EnableComment;
                is_hdn.Value = post.IsHidden;
                dtl_rw.Value = post.PostDetailsRaw ?? (object)DBNull.Value;
                rows = await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
            return rows > 0;
        }
        #endregion

        //============= PostDetails Action Methods ====================//
        #region Post Details
        public async Task<PostDetail> GetPostDetailsByIdAsync(int id)
        {
            PostDetail post = new PostDetail();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (id < 1) { return null; }
            sb.Append($"SELECT id, title, details, typ_id, mdby, mddt, dtl_rw  ");
            sb.Append($"FROM public.pcm_psts WHERE (id = @id);");
            query = sb.ToString();
            try
            {
                await conn.OpenAsync();
                // Retrieve all rows
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var postId = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                    await cmd.PrepareAsync();
                    postId.Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                        {
                            post.PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]);
                            post.PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString();
                            post.PostDetailsHtml = reader["details"] == DBNull.Value ? String.Empty : reader["details"].ToString();
                            post.PostDetailsRaw = reader["dtl_rw"] == DBNull.Value ? String.Empty : reader["dtl_rw"].ToString();
                            post.ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString();
                            post.ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"];
                            post.PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"];
                        }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByLoginIdAsync";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                return null;
            }
            return post;
        }
        public async Task<bool> AddPostDetailAsync(int postId, string htmlContent, string modifiedBy, DateTime modifiedDate)
        {
            int rows = 0;
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = $"UPDATE public.pcm_psts	SET dtl_rw=@dtl_rw, mdby=@mdby, mddt=@mddt WHERE(id = @id);";

            await conn.OpenAsync();
            //Insert data
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                var id = cmd.Parameters.Add("@id", NpgsqlDbType.Integer);
                var dtl_rw = cmd.Parameters.Add("@dtl_rw", NpgsqlDbType.Text);
                var mdby = cmd.Parameters.Add("@mdby", NpgsqlDbType.Text);
                var mddt = cmd.Parameters.Add("@mddt", NpgsqlDbType.TimestampTz);
                cmd.Prepare();
                id.Value = postId;
                dtl_rw.Value = htmlContent;
                mdby.Value = modifiedBy;
                mddt.Value = modifiedDate;

                rows = await cmd.ExecuteNonQueryAsync();

            }
            await conn.CloseAsync();
            return rows > 0;
        }

        #endregion

        //============= Banners Action Methods ========================//
        #region Banners Action Methods
        public async Task<IList<Post>> GetAllBannersAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append($"is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts WHERE typ_id = 0 ; ");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByDatabaseId";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                postlist = null;
            }
            return postlist;

        }

        public async Task<IList<Post>> GetUnhiddenBannersAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append($"is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts WHERE typ_id = 0 AND is_hdn = false; ");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByDatabaseId";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                postlist = null;
            }
            return postlist;

        }

        #endregion

        //============= Announcements Action Methods ==================//
        #region Announcements Action Methods
        public async Task<IList<Post>> GetAllAnnouncementsAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts  ");
            sb.Append("WHERE (typ_id = 3) ORDER BY id DESC;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByDatabaseId";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                postlist = null;
            }
            return postlist;

        }

        public async Task<IList<Post>> GetUnhiddenAnnouncementsAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts  ");
            sb.Append("WHERE typ_id = 3 AND is_hdn = false;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]
                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                postlist = null;
            }
            return postlist;

        }

        #endregion

        //============= Other Action Methods ==========================//
        #region Other Action Methods
        public Task<IList<Post>> GetAnnouncementsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Post>> GetUnhiddenArticlesAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append($"is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts ");
            sb.Append($"WHERE typ_id = 2 AND is_hdn = false;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]

                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByDatabaseId";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                postlist = null;
            }
            return postlist;

        }

        public async Task<IList<Post>> GetAllArticlesAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append($"is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts ");
            sb.Append($"WHERE typ_id = 2;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]

                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByDatabaseId";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                postlist = null;
            }
            return postlist;

        }

        public async Task<IList<Post>> GetBannersAsync()
        {
            List<Post> postlist = new List<Post>();
            var conn = new NpgsqlConnection(_config.GetConnectionString("PortalConnection"));
            string query = String.Empty;
            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT id, title, summary, imgp, mdby, crby, typ_id, enable_com, ");
            sb.Append($"is_hdn, crdt, mddt, hs_cm, hs_md FROM public.pcm_psts ");
            sb.Append($"WHERE typ_id = 0 AND is_hdn = false;");
            query = sb.ToString();
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
                        postlist.Add(new Post()
                        {
                            PostId = reader["id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id"]),
                            PostTitle = reader["title"] == DBNull.Value ? String.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? String.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? String.Empty : reader["imgp"].ToString(),
                            ModifiedBy = reader["mdby"] == DBNull.Value ? string.Empty : reader["mdby"].ToString(),
                            ModifiedDate = reader["mddt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["mddt"],
                            CreatedBy = reader["crby"] == DBNull.Value ? string.Empty : reader["crby"].ToString(),
                            CreatedDate = reader["crdt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["crdt"],
                            EnableComment = reader["enable_com"] == DBNull.Value ? false : (bool)reader["enable_com"],
                            IsHidden = reader["is_hdn"] == DBNull.Value ? false : (bool)reader["is_hdn"],
                            PostTypeId = reader["typ_id"] == DBNull.Value ? -1 : (int)reader["typ_id"],
                            HasComments = reader["hs_cm"] == DBNull.Value ? false : (bool)reader["hs_cm"],
                            HasMedia = reader["hs_md"] == DBNull.Value ? false : (bool)reader["hs_md"]

                        });
                    }
                }
                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                //ErrorRepository errorRepository = new ErrorRepository(_config);
                //ErrorEntity errorEntity = new ErrorEntity();
                //errorEntity.ErrorMessage = ex.Message;
                //errorEntity.ErrorDetail = ex.ToString();
                //errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
                //errorEntity.ErrorInnerSource = ex.Source;
                //errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByDatabaseId";
                //errorRepository.AddError(errorEntity);
                await conn.CloseAsync();
                postlist = null;
            }
            return postlist;
        }

        public Task<IList<Post>> GetCelebrantsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Post>> GetEventsAsync()
        {
            throw new NotImplementedException();
        }


        //public async Task<IList<ApplicationUserEntity>> GetUsersByDatabaseIdAsync(string databaseId)
        //{
        //    List<ApplicationUserEntity> userlist = new List<ApplicationUserEntity>();
        //    using var conn = new NpgsqlConnection(_config.GetConnectionString("NexxitConnection"));
        //    string query = String.Empty;
        //    StringBuilder sb = new StringBuilder();
        //    if (String.IsNullOrEmpty(databaseId))
        //    {
        //        return null;
        //    }

        //    sb.Append($"SELECT u.srxqk, u.srxnm, u.srxnnm, u.srxfn, u.sbrqk, u.srxph, u.srxss, u.srxccs, ");
        //    sb.Append($"u.srxtfe, u.srxisl, u.srxled, u.srxafc, u.srxml, u.srxnml, u.srxmlc, u.srxpn, u.srxpnc, ");
        //    sb.Append($"u.srximg, u.srxmb, u.srxmd, u.srxcb, u.srxcd, u.srxisd, u.srxisy, u.dbxqk, d.dbxds, ");
        //    sb.AppendLine($"d.dbxcx, t.sbrnr, t.sbrnm FROM utlsy010t u JOIN utlsy005t d ON u.dbxqk = d.dbxqk ");
        //    sb.AppendLine($"JOIN utlsy003t t ON u.sbrqk = t.sbrqk::text WHERE u.srxisd = false ");
        //    sb.AppendLine($"AND u.dbxqk = @dbxqk ;");
        //    query = sb.ToString();
        //    try
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var dbxqk = cmd.Parameters.Add("@dbxqk", NpgsqlDbType.Text);
        //            await cmd.PrepareAsync();
        //            dbxqk.Value = databaseId.ToUpperInvariant();
        //            await using (var reader = await cmd.ExecuteReaderAsync())
        //                while (await reader.ReadAsync())
        //                {
        //                    userlist.Add(new ApplicationUserEntity(_crypto)
        //                    {
        //                        AccessFailedCount = reader["srxafc"] == DBNull.Value ? 0 : Convert.ToInt32(reader["srxafc"]),
        //                        ConcurrencyStamp = reader["srxccs"] == DBNull.Value ? String.Empty : reader["srxccs"].ToString(),
        //                        Email = reader["srxml"] == DBNull.Value ? String.Empty : reader["srxml"].ToString(),
        //                        EmailIsConfirmed = reader["srxmlc"] == DBNull.Value ? false : (bool)reader["srxmlc"],
        //                        FullName = reader["srxfn"] == DBNull.Value ? String.Empty : reader["srxfn"].ToString(),
        //                        ImagePath = reader["srximg"] == DBNull.Value ? String.Empty : reader["srximg"].ToString(),
        //                        IsDeleted = reader["srxisd"] == DBNull.Value ? false : (bool)reader["srxisd"],
        //                        IsSystem = reader["srxisy"] == DBNull.Value ? false : (bool)reader["srxisy"],
        //                        LockOutEnd = reader["srxled"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["srxled"],
        //                        LockOutIsEnabled = reader["srxisl"] == DBNull.Value ? false : (bool)reader["srxisl"],
        //                        NormalizedEmail = reader["srxnml"] == DBNull.Value ? String.Empty : reader["srxnml"].ToString(),
        //                        NormalizedUsername = reader["srxnnm"] == DBNull.Value ? String.Empty : reader["srxnnm"].ToString(),
        //                        PasswordHash = reader["srxph"] == DBNull.Value ? String.Empty : reader["srxph"].ToString(),
        //                        PhoneNumber = reader["srxpn"] == DBNull.Value ? String.Empty : reader["srxpn"].ToString(),
        //                        PhoneNumberIsConfirmed = reader["srxpnc"] == DBNull.Value ? false : (bool)reader["srxpnc"],
        //                        SecurityStamp = reader["srxss"] == DBNull.Value ? String.Empty : reader["srxss"].ToString(),
        //                        TwoFactorAuthenticationIsEnabled = reader["srxtfe"] == DBNull.Value ? false : (bool)reader["srxtfe"],
        //                        UserName = reader["srxnm"] == DBNull.Value ? String.Empty : reader["srxnm"].ToString(),
        //                        UserID = reader["srxqk"] == DBNull.Value ? String.Empty : reader["srxqk"].ToString(),
        //                        ModifiedBy = reader["srxmb"] == DBNull.Value ? string.Empty : reader["srxmb"].ToString(),
        //                        ModifiedDate = reader["srxmd"] == DBNull.Value ? string.Empty : reader["srxmd"].ToString(),
        //                        CreatedDate = reader["srxcd"] == DBNull.Value ? string.Empty : reader["srxcd"].ToString(),
        //                        CreatedBy = reader["srxcb"] == DBNull.Value ? string.Empty : reader["srxcb"].ToString(),
        //                        TenantID = reader["sbrqk"] == DBNull.Value ? string.Empty : reader["sbrqk"].ToString(),
        //                        CompanyName = reader["dbxds"] == DBNull.Value ? string.Empty : reader["dbxds"].ToString(),
        //                        Connection = reader["dbxcx"] == DBNull.Value ? string.Empty : reader["dbxcx"].ToString(),
        //                        DatabaseID = reader["dbxqk"] == DBNull.Value ? string.Empty : reader["dbxqk"].ToString(),
        //                        TenantName = reader["sbrnm"] == DBNull.Value ? string.Empty : reader["sbrnm"].ToString(),
        //                        TenantNumber = reader["sbrnr"] == DBNull.Value ? string.Empty : reader["sbrnr"].ToString(),
        //                    });
        //                }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorRepository errorRepository = new ErrorRepository(_config);
        //        ErrorEntity errorEntity = new ErrorEntity();
        //        errorEntity.ErrorMessage = ex.Message;
        //        errorEntity.ErrorDetail = ex.ToString();
        //        errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
        //        errorEntity.ErrorInnerSource = ex.Source;
        //        errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByDatabaseId";
        //        errorRepository.AddError(errorEntity);
        //        await conn.CloseAsync();
        //        userlist = null;
        //    }
        //    return userlist;
        //}

        //public async Task<ApplicationUserEntity> GetUserByLoginIdAsync(string loginId)
        //{
        //    ApplicationUserEntity userEntity = new ApplicationUserEntity(_crypto);
        //    using var conn = new NpgsqlConnection(_config.GetConnectionString("NexxitConnection"));
        //    string query = String.Empty;
        //    StringBuilder sb = new StringBuilder();
        //    if (String.IsNullOrEmpty(loginId))
        //    {
        //        return null;
        //    }

        //    sb.Append($"SELECT u.srxqk, u.srxnm, u.srxnnm, u.srxfn, u.sbrqk, u.srxph, u.srxss, u.srxccs, ");
        //    sb.Append($"u.srxtfe, u.srxisl, u.srxled, u.srxafc, u.srxml, u.srxnml, u.srxmlc, u.srxpn, u.srxpnc, ");
        //    sb.Append($"u.srximg, u.srxmb, u.srxmd, u.srxcb, u.srxcd, u.srxisd, u.srxisy, u.dbxqk, d.dbxds, ");
        //    sb.AppendLine($"d.dbxcx, t.sbrnr, t.sbrnm FROM utlsy010t u JOIN utlsy005t d ON u.dbxqk = d.dbxqk ");
        //    sb.AppendLine($"JOIN utlsy003t t ON u.sbrqk = t.sbrqk::text WHERE u.srxisd = false ");
        //    sb.AppendLine($"AND u.srxnnm = @srxnnm ;");
        //    query = sb.ToString();
        //    try
        //    {
        //        await conn.OpenAsync();
        //        // Retrieve all rows
        //        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
        //        {
        //            var srxnnm = cmd.Parameters.Add("@srxnnm", NpgsqlDbType.Text);
        //            await cmd.PrepareAsync();
        //            srxnnm.Value = loginId.ToUpperInvariant();
        //            await using (var reader = await cmd.ExecuteReaderAsync())
        //                while (await reader.ReadAsync())
        //                {
        //                    userEntity.AccessFailedCount = reader["srxafc"] == DBNull.Value ? 0 : Convert.ToInt32(reader["srxafc"]);
        //                    userEntity.ConcurrencyStamp = reader["srxccs"] == DBNull.Value ? String.Empty : reader["srxccs"].ToString();
        //                    userEntity.Email = reader["srxml"] == DBNull.Value ? String.Empty : reader["srxml"].ToString();
        //                    userEntity.EmailIsConfirmed = reader["srxmlc"] == DBNull.Value ? false : (bool)reader["srxmlc"];
        //                    userEntity.FullName = reader["srxfn"] == DBNull.Value ? String.Empty : reader["srxfn"].ToString();
        //                    userEntity.ImagePath = reader["srximg"] == DBNull.Value ? String.Empty : reader["srximg"].ToString();
        //                    userEntity.IsDeleted = reader["srxisd"] == DBNull.Value ? false : (bool)reader["srxisd"];
        //                    userEntity.IsSystem = reader["srxisy"] == DBNull.Value ? false : (bool)reader["srxisy"];
        //                    userEntity.LockOutEnd = reader["srxled"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["srxled"];
        //                    userEntity.LockOutIsEnabled = reader["srxisl"] == DBNull.Value ? false : (bool)reader["srxisl"];
        //                    userEntity.NormalizedEmail = reader["srxnml"] == DBNull.Value ? String.Empty : reader["srxnml"].ToString();
        //                    userEntity.NormalizedUsername = reader["srxnnm"] == DBNull.Value ? String.Empty : reader["srxnnm"].ToString();
        //                    userEntity.PasswordHash = reader["srxph"] == DBNull.Value ? String.Empty : reader["srxph"].ToString();
        //                    userEntity.PhoneNumber = reader["srxpn"] == DBNull.Value ? String.Empty : reader["srxpn"].ToString();
        //                    userEntity.PhoneNumberIsConfirmed = reader["srxpnc"] == DBNull.Value ? false : (bool)reader["srxpnc"];
        //                    userEntity.SecurityStamp = reader["srxss"] == DBNull.Value ? String.Empty : reader["srxss"].ToString();
        //                    userEntity.TwoFactorAuthenticationIsEnabled = reader["srxtfe"] == DBNull.Value ? false : (bool)reader["srxtfe"];
        //                    userEntity.UserName = reader["srxnm"] == DBNull.Value ? String.Empty : reader["srxnm"].ToString();
        //                    userEntity.UserID = reader["srxqk"] == DBNull.Value ? String.Empty : reader["srxqk"].ToString();
        //                    userEntity.ModifiedBy = reader["srxmb"] == DBNull.Value ? string.Empty : reader["srxmb"].ToString();
        //                    userEntity.ModifiedDate = reader["srxmd"] == DBNull.Value ? string.Empty : reader["srxmd"].ToString();
        //                    userEntity.CreatedDate = reader["srxcd"] == DBNull.Value ? string.Empty : reader["srxcd"].ToString();
        //                    userEntity.CreatedBy = reader["srxcb"] == DBNull.Value ? string.Empty : reader["srxcb"].ToString();
        //                    userEntity.TenantID = reader["sbrqk"] == DBNull.Value ? string.Empty : reader["sbrqk"].ToString();
        //                    userEntity.CompanyName = reader["dbxds"] == DBNull.Value ? string.Empty : reader["dbxds"].ToString();
        //                    userEntity.Connection = reader["dbxcx"] == DBNull.Value ? string.Empty : reader["dbxcx"].ToString();
        //                    userEntity.DatabaseID = reader["dbxqk"] == DBNull.Value ? string.Empty : reader["dbxqk"].ToString();
        //                    userEntity.TenantName = reader["sbrnm"] == DBNull.Value ? string.Empty : reader["sbrnm"].ToString();
        //                    userEntity.TenantNumber = reader["sbrnr"] == DBNull.Value ? string.Empty : reader["sbrnr"].ToString();
        //                }
        //        }
        //        await conn.CloseAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorRepository errorRepository = new ErrorRepository(_config);
        //        ErrorEntity errorEntity = new ErrorEntity();
        //        errorEntity.ErrorMessage = ex.Message;
        //        errorEntity.ErrorDetail = ex.ToString();
        //        errorEntity.ErrorTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} (UTC)";
        //        errorEntity.ErrorInnerSource = ex.Source;
        //        errorEntity.ErrorSource = "ApplicationUserRepository_GetUsersByLoginIdAsync";
        //        errorRepository.AddError(errorEntity);
        //        await conn.CloseAsync();
        //        return null;
        //    }
        //    return userEntity;
        //}
        #endregion
    }
}
