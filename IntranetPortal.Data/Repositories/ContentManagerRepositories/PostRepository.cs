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
            sb.Append("hs_cm, hs_md, dtl_rw, flpth FROM public.pcm_psts ");
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
                            post.ImageFullPath = reader["flpth"] == DBNull.Value ? String.Empty : reader["flpth"].ToString();
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
            catch (Exception)
            {
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
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md, flpth FROM public.pcm_psts ");
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
                            ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md, flpth FROM public.pcm_psts ");
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
                            ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            sb.Append("is_hdn, crdt, mddt, hs_cm, hs_md, flpth FROM public.pcm_psts ");
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
                            ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            sb.Append("enable_com, is_hdn, crdt, mddt, hs_cm, hs_md, dtl_rw, flpth ");
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
                                ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            sb.Append("enable_com, is_hdn, crdt, mddt, hs_cm, hs_md, dtl_rw, flpth ");
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
                                ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            sb.Append("is_hdn, crdt, mddt, dtl_rw, flpth) ");
            sb.Append("VALUES (@title, @summary, @details, @imgp, ");
            sb.Append("@mdby, @crby, @typ_id, @enable_com, ");
            sb.Append("@is_hdn, @crdt, @mddt, @dtl_rw, @flpth); ");

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
                var flpth = cmd.Parameters.Add("@flpth", NpgsqlDbType.Text);
                cmd.Prepare();
                title.Value = post.PostTitle ?? (object)DBNull.Value;
                summary.Value = post.PostSummary ?? (object)DBNull.Value;
                details.Value = post.PostDetails ?? (object)DBNull.Value;
                imgp.Value = post.ImagePath ?? (object)DBNull.Value;
                mdby.Value = post.ModifiedBy ?? (object)DBNull.Value;
                mddt.Value = post.ModifiedDate ?? (object)DBNull.Value;
                crby.Value = post.CreatedBy ?? (object)DBNull.Value;
                crdt.Value = post.CreatedDate ?? (object)DBNull.Value;
                typ_id.Value = post.PostTypeId;
                enable_com.Value = post.EnableComment;
                is_hdn.Value = post.IsHidden;
                dtl_rw.Value = post.PostDetailsRaw ?? (object)DBNull.Value;
                flpth.Value = post.ImageFullPath ?? (object)DBNull.Value;

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
            sb.Append("dtl_rw=@dtl_rw, flpth=@flpth WHERE (id=@id);");

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
                var flpth = cmd.Parameters.Add("@flpth", NpgsqlDbType.Text);
                cmd.Prepare();
                id.Value = post.PostId;
                title.Value = post.PostTitle ?? (object)DBNull.Value;
                summary.Value = post.PostSummary ?? (object)DBNull.Value;
                typ_id.Value = post.PostTypeId;
                details.Value = post.PostDetails ?? (object)DBNull.Value;
                imgp.Value = post.ImagePath ?? (object)DBNull.Value;
                mdby.Value = post.ModifiedBy ?? (object)DBNull.Value;
                mddt.Value = post.ModifiedDate ?? (object)DBNull.Value;
                enable_com.Value = post.EnableComment;
                is_hdn.Value = post.IsHidden;
                dtl_rw.Value = post.PostDetailsRaw ?? (object)DBNull.Value;
                flpth.Value = post.ImageFullPath ?? (object)DBNull.Value;

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
                title.Value = post.PostTitle ?? (object)DBNull.Value;
                summary.Value = post.PostSummary ?? (object)DBNull.Value;
                typ_id.Value = post.PostTypeId;
                details.Value = post.PostDetails ?? (object)DBNull.Value;
                mdby.Value = post.ModifiedBy ?? (object)DBNull.Value;
                mddt.Value = post.ModifiedDate ?? (object)DBNull.Value;
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

            sb.Append("SELECT id, title, summary, imgp, mdby, crby, typ_id, ");
            sb.Append("enable_com, is_hdn, crdt, mddt, hs_cm, hs_md,");
            sb.Append("flpth FROM public.pcm_psts WHERE typ_id = 0 ; ");
            
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
                            PostTitle = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? string.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            catch (Exception)
            {
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

            sb.Append("SELECT id, title, summary, imgp, mdby, crby, typ_id, ");
            sb.Append("enable_com, is_hdn, crdt, mddt, hs_cm, hs_md, flpth ");
            sb.Append("FROM public.pcm_psts WHERE typ_id = 0 AND is_hdn = false; ");
            
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
                            PostTitle = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? string.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            catch (Exception)
            {
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
            sb.Append($"is_hdn, crdt, mddt, hs_cm, hs_md, flpth FROM public.pcm_psts ");
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
                            PostTitle = reader["title"] == DBNull.Value ? string.Empty : reader["title"].ToString(),
                            PostSummary = reader["summary"] == DBNull.Value ? string.Empty : reader["summary"].ToString(),
                            ImagePath = reader["imgp"] == DBNull.Value ? string.Empty : reader["imgp"].ToString(),
                            ImageFullPath = reader["flpth"] == DBNull.Value ? string.Empty : reader["flpth"].ToString(),
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
            catch (Exception)
            {
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
        #endregion
    }
}
