using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Repositories.ContentManagerRepositories;
using System;
using System.Collections.Generic;
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

        //================ Posts Action Methods ====================================================================//
        #region Post Action Methods
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
            post.PostTypeId = (int)PostType.Banner;
            return await _postRepository.EditPostAsync(post);
        }
        public async Task<bool> HidePostAsync(int postId)
        {
            if (postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.DeletePostAsync(postId);
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            if(postId < 1) { throw new ArgumentNullException(nameof(postId), "Required parameter [postId] is missing."); }
            return await _postRepository.GetPostByIdAsync(postId);
        }

        public async Task<IList<Post>> GetAllPostsAsync()
        {
            return await _postRepository.GetAllPostsAsync();
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


        //=============== Articles Action Methods =============================================================//
        #region Articles Action Methods
        public async Task<IList<Post>> GetAllArticlesAsync()
        {
            return await _postRepository.GetAllArticlesAsync();
        }

        public async Task<IList<Post>> GetUnhiddenArticlesAsync()
        {
            return await _postRepository.GetUnhiddenArticlesAsync();
        }

        #endregion
    }
}
