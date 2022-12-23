using IntranetPortal.Base.Models.ContentManagerModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IContentManagerService
    {
        //========== Post Service Actions ===================//
        #region Post Service Actions
        Task<bool> CreatePostAsync(Post post);
        Task<bool> UpdatePostAsync(Post post);
        Task<bool> DeletePostAsync(int postId);
        Task<bool> HidePostAsync(int postId);
        Task<Post> GetPostByIdAsync(int postId);
        Task<IList<Post>> GetAllPostsAsync();
        Task<IList<Post>> GetAllOtherPostsAsync();
        Task<IList<Post>> GetAllOtherUnhiddenPostsAsync();
        Task<IList<Post>> GetPostsByPostTypeId(int postTypeId);
        Task<IList<Post>> SearchPostsByTitle(string postTitle);
        Task<bool> UpdatePostDetailsAsync(int postId, string htmlContent, string modifiedBy, DateTime modifiedDate);
        #endregion

        //========== Banner Service Actions ===================//
        #region Banner Service Actions
        Task<IList<Post>> GetAllBannersAsync();
        Task<IList<Post>> GetUnhiddenBannersAsync();
        #endregion

        //========== Announcements Service Actions ===================//
        #region Announcements Service Actions
        Task<IList<Post>> GetAllAnnouncementsAsync();
        Task<IList<Post>> GetUnhiddenAnnouncementsAsync();
        #endregion


        //========== Article Service Actions ===================//
        #region Article Service Actions
        Task<IList<Post>> GetAllArticlesAsync();
        Task<IList<Post>> GetUnhiddenArticlesAsync();
        #endregion
    }
}
