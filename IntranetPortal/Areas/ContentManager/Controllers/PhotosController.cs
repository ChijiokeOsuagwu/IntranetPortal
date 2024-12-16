using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ContentManager.Models;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Enums;
using IntranetPortal.Helpers;

namespace IntranetPortal.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize]
    public class PhotosController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IDataProtector _dataProtector;
        public PhotosController(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration,
                                    IContentManagerService contentManager, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _webHostEnvironment = webHostingEnvironment;
            _configuration = configuration;
            _contentManager = contentManager;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }


        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Index()
        {
            PostListViewModel model = new PostListViewModel();
            if (TempData["DeleteSuccessMessage"] != null)
            {
                model.ViewModelSuccessMessage = TempData["DeleteSuccessMessage"].ToString();
            }

            var result = await _contentManager.GetAllPhotosAsync();
            model.PostList = result.ToList();
            foreach (var post in model.PostList)
            {
                post.CodedPostId = _dataProtector.Protect(post.PostId.ToString());
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public IActionResult AddPhoto()
        {
            PostViewModel model = new PostViewModel();
            model.PostTypeId = (int)PostType.Photos;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> AddPhoto(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;

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

                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);

                    using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                Post post = new Post
                {
                    PostTitle = model.PostTitle,
                    ImagePath = "/" + uploadsFolder,
                    ImageFullPath = absoluteFilePath,
                    PostDetails = model.PostDetails,
                    PostDetailsRaw = model.PostDetailsRaw,
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
                    return RedirectToAction("Index", "Photos");
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        if (!file.IsFileOpen())
                        {
                            await Task.Run(() =>
                            {
                                file.Delete();
                            });
                        }
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. New Post could not be added.";
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> EditPhoto(int id)
        {
            PostEditViewModel model = new PostEditViewModel();
            model.PostId = id;
            if (id > 0)
            {
                var entity = await _contentManager.GetPostByIdAsync(id);
                if (entity != null)
                {
                    model = model.ExtraFromPost(entity);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> EditPhoto(PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;
                string previousUploadsFolder = null;
                string previousAbsoluteFilePath = null;
                bool newFileUploadedSuccessfully = false;

                var entity = await _contentManager.GetPostByIdAsync(model.PostId.Value);
                if (entity != null)
                {
                    previousAbsoluteFilePath = entity.ImageFullPath;
                    previousUploadsFolder = entity.ImagePath;
                }

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

                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    //await model.ImageFile.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                    using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    newFileUploadedSuccessfully = true;
                }
                else
                {
                    uploadsFolder = previousUploadsFolder;
                    absoluteFilePath = previousAbsoluteFilePath;
                }

                Post post = model.ConvertToPost();
                post.ImagePath = uploadsFolder;
                post.ImageFullPath = absoluteFilePath;
                post.ModifiedDate = DateTime.UtcNow;
                post.ModifiedBy = HttpContext.User.Identity.Name ?? string.Empty;

                if (await _contentManager.UpdatePostAsync(post))
                {
                    if (newFileUploadedSuccessfully)
                    {
                        FileInfo file = new FileInfo(previousAbsoluteFilePath);
                        if (file.Exists)
                        {
                            if (!file.IsFileOpen())
                            {
                                await Task.Run(() =>
                                {
                                    file.Delete();
                                });
                            }
                        }
                    }
                    model.ViewModelSuccessMessage = "Congratulations! Update operation completed successfully.";
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        if (file.Exists)
                        {
                            if (!file.IsFileOpen())
                            {
                                await Task.Run(() =>
                                {
                                    file.Delete();
                                });
                            }
                        }
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. New Post could not be added.";
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(int id)
        {
            int PostId = id; //Convert.ToInt32(_dataProtector.Unprotect(id));

            PostViewModel model = new PostViewModel();
            var post = await _contentManager.GetPostByIdAsync(PostId);
            model.PostId = post.PostId;
            model.ImagePath = post.ImagePath;
            model.PostSummary = post.PostSummary;
            model.PostTitle = post.PostTitle;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(PostViewModel model)
        {
            if (model == null || model.PostId == null || model.PostId < 1)
            {
                model.ViewModelErrorMessage = "Error! Required parameter PostID coult not be found. Album could not be deleted.";
                return View(model);
            }
            long PostId = model.PostId.Value;
            string mediaLocationFullPath = null;

            var entity = await _contentManager.GetPostByIdAsync(PostId);
            if (entity != null)
            {
                mediaLocationFullPath = entity.ImageFullPath;
                if (entity.HasMedia || entity.HasComments)
                {
                    model.ViewModelErrorMessage = "This album contains media/comments. Kindly delete all media/comments in this album and try again.";
                    return View(model);
                }

                var result = await _contentManager.DeletePostAsync(PostId);
                if (!result)
                {
                    model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. Post could not be deleted.";
                    return View(model);
                }

                FileInfo file = new FileInfo(mediaLocationFullPath);
                if (file.Exists)
                {
                    if (!file.IsFileOpen())
                    {
                        await Task.Run(() =>
                        {
                            file.Delete();
                        });
                    }
                }

                TempData["DeleteSuccessMessage"] = "Delete operation completed successfully!";
                return RedirectToAction("Index", "Photos");
            }
            model.ViewModelErrorMessage = "Sorry, this record could not be found.";
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public IActionResult UploadPhoto(int id)
        {
            PostMediaViewModel model = new PostMediaViewModel();
            model.MasterPostId = id;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> UploadPhoto(PostMediaViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;

                if (model.MediaFile != null && model.MediaFile.Length > 0)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp3", ".mp4" };
                    FileInfo fileInfo = new FileInfo(model.MediaFile.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid file format. Only files of type jpg, jpeg, png, gif, mp3 and mp4 are permitted.";
                        return View(model);
                    }

                    //if(fileInfo.Length / (1048576) > 1)
                    //{
                    //    model.ViewModelErrorMessage = "Sorry, this image is too large. Image size must not exceed 1MB.";
                    //    return View(model);
                    //}

                    uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.MediaFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    //await model.MediaFile.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                    using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
                    {
                        await model.MediaFile.CopyToAsync(fileStream);
                    }
                }

                PostMedia postMedia = new PostMedia
                {
                    Caption = model.Caption,
                    MediaType = (MediaType)model.MediaTypeId,
                    MasterPostId = model.MasterPostId,
                    MediaLocationFullPath = absoluteFilePath,
                    MediaLocationPath = "/" + uploadsFolder
                };

                if (await _contentManager.AddPostMediaAsync(postMedia))
                {
                    model.ViewModelSuccessMessage = "New Media was uploaded successfully!";
                    //return RedirectToAction("Index", "Photos");
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        if (!file.IsFileOpen())
                        {
                            await Task.Run(() =>
                            {
                                file.Delete();
                            });
                        }
                    }
                    model.ViewModelErrorMessage = "Sorry, an error was encountered. New Post could not be added.";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> MediaList(int id)
        {
            PostMediaListViewModel model = new PostMediaListViewModel();
            Post post = await _contentManager.GetPostByIdAsync(id);
            if (post != null)
            {
                model.MasterPost = post;
            }

            var entities = await _contentManager.GetPostMediasByMasterPostId(id);
            if (entities != null)
            {
                model.MediaList = entities.ToList();
            }

            if (TempData["SuccessMessage"] != null)
            {
                model.ViewModelSuccessMessage = TempData["SuccessMessage"].ToString();
            }

            if(TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }

        public async Task<IActionResult> DeletePhoto(long id, long pd)
        {
            long PostMediaId = id;
            long MasterPostId = pd;
            string mediaFileFullPath = null;
            try
            {
            PostMedia media = await _contentManager.GetPostMediaByIdAsync(PostMediaId);
            if(media != null)
            {
                 mediaFileFullPath = media.MediaLocationFullPath;
                if (await _contentManager.DeletePostMediaAsync(PostMediaId))
                {
                    if (!string.IsNullOrWhiteSpace(mediaFileFullPath))
                    {
                        FileInfo file = new FileInfo(mediaFileFullPath);
                        if (file.Exists)
                        {
                            if (!file.IsFileOpen())
                            {
                                await Task.Run(() =>
                                {
                                    file.Delete();
                                });
                            }
                        }
                    }
                    TempData["SuccessMessage"] = "Delete operation completed successfully!";
                }
            }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("MediaList", new { id = MasterPostId });
        }

        public async Task<IActionResult> FullScreen(long id)
        {
            PostMediaViewModel model = new PostMediaViewModel();
            PostMedia media = await _contentManager.GetPostMediaByIdAsync(id);
            if (media != null)
            {
                model.Caption = media.Caption;
                model.MasterPostId = media.MasterPostId;
                model.MediaLocationPath = media.MediaLocationPath;
                model.MediaTypeId = (int)media.MediaType;
                model.PostMediaId = media.PostMediaId;
            }
            return View(model);
        }


    }
}