using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class ContentManagerService : IContentManagerService
    {
        private readonly IPostRepository _postRepository;
        public ContentManagerService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        //====== Posts Write Service Methods ========//
        #region Posts Write Service Methods
        public async Task<bool> CreatePostAsync(Post post)
        {
            if (post == null) { throw new ArgumentNullException(nameof(post), "Required parameter [post] is missing."); }
            return await _postRepository.AddPostAsync(post);
        }
        public async Task<bool> DeletePostAsync(int postId)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.DeletePostAsync(postId);
        }
        public async Task<bool> UpdatePostAsync(Post post)
        {
            if (post == null) { throw new ArgumentNullException(nameof(post), "Required parameter [post] is missing."); }
            //post.PostTypeId = (int)PostType.Banner;
            if (!string.IsNullOrWhiteSpace(post.ImagePath))
            {
                return await _postRepository.EditPostAsync(post);
            }
            return await _postRepository.EditPostWithoutImageAsync(post);
        }
        public async Task<bool> HidePostAsync(int postId)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.DeletePostAsync(postId);
        }
        #endregion

        #region Posts Read Service Methods

        public async Task<IList<Post>> GetAllPostsAsync()
        {
            return await _postRepository.GetAllAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.GetPostByIdAsync(postId);
        }

        public async Task<IList<Post>> SearchPostsByTitle(string postTitle)
        {
            return await _postRepository.GetByTitleAsync(postTitle);
        }

        #endregion

        #region Articles Read Service Methods

        public async Task<IList<Post>> GetAllArticlesAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Article);
        }

        public async Task<IList<Post>> GetUnhiddenArticlesAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Article, false);
        }

        public async Task<IList<Post>> GetHiddenArticlesAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Article, true);
        }

        #endregion

        #region Events Read Service Methods
        public async Task<IList<Post>> GetAllEventsAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Event);
        }

        public async Task<IList<Post>> GetUnhiddenEventsAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Event, false);
        }

        public async Task<IList<Post>> GetHiddenEventsAsync()
        {
            return await _postRepository.GetByTypeIdAsync((int)PostType.Event, true);
        }

        #endregion


        #region 
        public async Task<IList<Post>> GetAllOtherPostsAsync()
        {
            return await _postRepository.GetPostsWithoutBannersAndAnnouncementsAsync();
        }

        public async Task<IList<Post>> GetAllOtherUnhiddenPostsAsync()
        {
            return await _postRepository.GetUnhiddenPostsWithoutBannersAndAnnouncementsAsync();
        }

        public async Task<IList<Post>> GetPostsByPostTypeId (int postTypeId)
        {
           return await _postRepository.GetByTypeIdAsync(postTypeId);
        }

        public async Task<bool> UpdatePostDetailsAsync(int postId, string htmlContent, string modifiedBy, DateTime modifiedDate)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] has invalid value."); }
            if (string.IsNullOrEmpty(htmlContent)) { throw new ArgumentNullException(nameof(htmlContent), "Required parameter [htmlContent] has invalid value."); }
            if (string.IsNullOrEmpty(modifiedBy)) { throw new ArgumentNullException(nameof(modifiedBy), "Required parameter [modifiedBy] has invalid value."); }
            if (modifiedDate == null) { throw new ArgumentNullException(nameof(modifiedDate), "Required parameter [modifiedDate] has invalid value."); }

            return await _postRepository.AddPostDetailAsync(postId, htmlContent, modifiedBy, modifiedDate);
        }
        #endregion

        //=============== Banners Action Methods =============================================================//
        #region Banners Action Methods
        public async Task<IList<Post>> GetAllBannersAsync()
        {
            return await _postRepository.GetAllBannersAsync();
        }

        public async Task<IList<Post>> GetUnhiddenBannersAsync()
        {
            return await _postRepository.GetUnhiddenBannersAsync();
        }

        #endregion

        //=============== Announcements Action Methods =============================================================//
        #region Announcements Action Methods
        public async Task<IList<Post>> GetAllAnnouncementsAsync()
        {
            return await _postRepository.GetAllAnnouncementsAsync();
        }

        public async Task<IList<Post>> GetUnhiddenAnnouncementsAsync()
        {
            return await _postRepository.GetUnhiddenAnnouncementsAsync();
        }

        #endregion

    }
}
