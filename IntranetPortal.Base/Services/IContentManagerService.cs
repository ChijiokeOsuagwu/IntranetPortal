using IntranetPortal.Base.Models.ContentManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IContentManagerService
    {
        #region Post Service Actions
        Task<bool> CreatePostAsync(Post post);
        Task<bool> UpdatePostAsync(Post post);
        Task<bool> DeletePostAsync(long postId);
        Task<bool> HidePostAsync(long postId);
        Task<Post> GetPostByIdAsync(long postId);
        Task<IList<Post>> GetAllPostsAsync();
        Task<IList<Post>> GetAllPostsExceptBannersnAnnouncementsAsync();
        Task<IList<Post>> GetAllUnhiddenPostsExceptBannersnAnnouncementsAsync();
        Task<IList<Post>> GetPostsByPostTypeId(int postTypeId);
        Task<IList<Post>> SearchPostsByTitle(string postTitle, int postTypeId);
        Task<bool> UpdatePostDetailsAsync(long postId, string htmlContent, string modifiedBy, DateTime modifiedDate);
        #endregion

        #region Photos Service Actions
        Task<IList<Post>> GetAllPhotosAsync();
        Task<IList<Post>> GetUnhiddenPhotosAsync();
        Task<IList<Post>> GetHiddenPhotosAsync();
        #endregion

        #region Banner Service Actions
        Task<IList<Post>> GetAllBannersAsync();
        Task<IList<Post>> GetUnhiddenBannersAsync();
        #endregion

        #region Announcements Service Actions
        Task<IList<Post>> GetAllAnnouncementsAsync();
        Task<IList<Post>> GetUnhiddenAnnouncementsAsync();
        #endregion

        #region Article Service Actions
        Task<IList<Post>> GetAllArticlesAsync();
        Task<IList<Post>> GetUnhiddenArticlesAsync();
        #endregion

        #region Post Media Service Interfaces
        Task<bool> AddPostMediaAsync(PostMedia postMedia);
        Task<bool> DeletePostMediaAsync(long postMediaId);
        Task<PostMedia> GetPostMediaByIdAsync(long postMediaId);
        Task<IList<PostMedia>> GetPostMediasByMasterPostId(long masterPostId);
        #endregion
    }
}
