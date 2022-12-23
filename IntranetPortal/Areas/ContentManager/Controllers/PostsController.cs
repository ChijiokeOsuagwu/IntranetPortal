using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ContentManager.Models;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IDataProtector _dataProtector;
        public PostsController(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration,
                                    IContentManagerService contentManager, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _webHostEnvironment = webHostingEnvironment;
            _configuration = configuration;
            _contentManager = contentManager;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "PCMARTVWL, XYALLACCZ")]
        public async Task<IActionResult> List(string ss, int? pt)
        {
            PostListViewModel model = new PostListViewModel();
            IList<Post> entities = new List<Post>();
            if(pt != null)
            {
                model.pt = pt.Value;
                entities = await _contentManager.GetPostsByPostTypeId(pt.Value);
            }
            else if(!string.IsNullOrWhiteSpace(ss))
            {
                model.ss = ss;
                entities = await _contentManager.SearchPostsByTitle(ss);
            }
            else
            {
                entities = await _contentManager.GetAllPostsAsync();
            }
            
            if(entities != null) {
                foreach( var i in entities)
                {
                    i.PostTypeName = i.PostTypeId switch
                    {
                        0 => "Banner",
                        1 => "Celebrant",
                        2 => "Article",
                        3 => "Announcement",
                        4 => "Event",
                        _ => "Post",
                    };
                }
                model.PostList = entities.ToList(); 
            }
            return View(model);
        }

        [Authorize(Roles = "PCMARTADN, XYALLACCZ")]
        public IActionResult Add()
        {
            PostViewModel model = new PostViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMARTADN, XYALLACCZ")]
        public async Task<IActionResult> Add(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;

                if (model.ImageFile != null & model.ImageFile.Length > 0)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.ImageFile.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }

                    if(fileInfo.Length / (1048576) > 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, this image is too large. Image size must not exceed 1MB.";
                        return View(model);
                    }
                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    await model.ImageFile.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                }
                Post post = new Post
                {
                    PostTitle = model.PostTitle,
                    ImagePath = uploadsFolder,
                    EnableComment = model.EnableComments,
                    IsHidden = model.IsHidden,
                    PostSummary = model.PostSummary,
                    PostTypeId = model.PostTypeId,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = HttpContext.User.Identity.Name ?? string.Empty,
                    CreatedBy = HttpContext.User.Identity.Name ?? string.Empty
                };

                if (await _contentManager.CreatePostAsync(post))
                {
                    model.ViewModelSuccessMessage = $"Congratulations! New Post was added successfully.";
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. New Post could not be added.";
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PCMARTDLT, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            int PostId = Convert.ToInt32(_dataProtector.Unprotect(id));
            PostViewModel model = new PostViewModel();
            var post = await _contentManager.GetPostByIdAsync(PostId);
            model.PostId = post.PostId;
            model.ImagePath = post.ImagePath;
            model.PostSummary = post.PostSummary;
            model.PostTitle = post.PostTitle;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMARTDLT, XYALLACCZ")]
        public async Task<IActionResult> Delete(PostViewModel model)
        {
            if (model == null || model.PostId < 1)
            {
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Article could not be deleted.";
                return View(model);
            }
            int PostId = model.PostId.Value;
            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(model.ImagePath))
            {
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, model.ImagePath);
            }
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
            var result = await _contentManager.DeletePostAsync(PostId);
            if (!result)
            {
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Post could not be deleted.";
                return View(model);
            }
            TempData["DeleteSuccessMessage"] = "The Post was successfully deleted!";
            return RedirectToAction("Index", "Articles");
        }

        [Authorize(Roles = "PCMARTEDT, XYALLACCZ")]
        public async Task<IActionResult> Edit(int id)
        {
            int PostId = 0;
            if (id > 0) { PostId = id; }
            PostViewModel model = new PostViewModel();
            var post = await _contentManager.GetPostByIdAsync(PostId);
            model.PostId = post.PostId;
            model.ImagePath = post.ImagePath;
            model.PostSummary = post.PostSummary;
            model.PostTitle = post.PostTitle;
            model.EnableComments = post.EnableComment;
            model.IsHidden = post.IsHidden;
            model.PostTypeId = (int)post.PostTypeId;
            //model.Id = article.PostId;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMARTEDT, XYALLACCZ")]
        public async Task<IActionResult> Edit(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post();
                string absoluteFilePath = string.Empty;
                string uploadsFolder = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.ImageFile.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }

                    if (fileInfo.Length / (1048576) > 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, this image is too large. Image size must not exceed 1MB.";
                        return View(model);
                    }
                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    await model.ImageFile.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                }

                post.IsHidden = model.IsHidden;
                post.PostSummary = model.PostSummary;
                post.PostTypeId = model.PostTypeId;
                post.ModifiedDate = DateTime.UtcNow;
                post.ModifiedBy = HttpContext.User.Identity.Name ?? string.Empty;
                post.PostTitle = model.PostTitle;
                post.ImagePath = uploadsFolder;
                post.EnableComment = model.EnableComments;


                if (await _contentManager.UpdatePostAsync(post))
                {
                    model.ViewModelSuccessMessage = $"Congratulations! Post was updated successfully.";
                }
                else
                {
                    FileInfo file = new FileInfo( Path.Combine(_webHostEnvironment.WebRootPath, model.ImagePath));
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. Post Could not be added. Please try again.";
                }
            }
            return View(model);
        }

    }
}