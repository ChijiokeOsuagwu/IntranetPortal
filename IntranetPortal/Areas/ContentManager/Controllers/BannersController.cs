using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ContentManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Authorization;

namespace IntranetPortal.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize]
    public class BannersController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IDataProtector _dataProtector;
        public BannersController(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration, 
                                    IContentManagerService contentManager, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _webHostEnvironment = webHostingEnvironment;
            _configuration = configuration;
            _contentManager = contentManager;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> List()
        {
            BannerListViewModel model = new BannerListViewModel();
            if (TempData["DeleteSuccessMessage"] != null)
            {
                model.ViewModelSuccessMessage = TempData["DeleteSuccessMessage"].ToString();
            }
           
            var result = await _contentManager.GetAllBannersAsync();
            model.Banners = result.ToList();
            foreach (var banner in model.Banners)
            {
                banner.CodedPostId = _dataProtector.Protect(banner.PostId.ToString());
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public IActionResult Add()
        {
            BannerAddViewModel model = new BannerAddViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Add(BannerAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.BannerImage != null)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.BannerImage.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }
                    string uploadsFolder = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.BannerImage.FileName;
                    string absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    await model.BannerImage.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));

                    Post post = new Post
                    {
                        PostTitle = model.Title,
                        ImagePath = uploadsFolder,
                        EnableComment = model.EnableComments,
                        IsHidden = model.IsHidden,
                        PostSummary = model.Summary,
                        PostTypeId = (int)PostType.Banner,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = HttpContext.User.Identity.Name ?? "System Administrator",
                        CreatedBy = HttpContext.User.Identity.Name ?? "System Administrator"
                    };

                    if (await _contentManager.CreatePostAsync(post))
                    {
                        model.ViewModelSuccessMessage = $"Congratulations! New Banner was added successfully.";
                    }
                    else
                    {
                        FileInfo file = new FileInfo(absoluteFilePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        model.ViewModelErrorMessage = $"Error! An error was encountered. New banner could not be added.";
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            int PostId = Convert.ToInt32(_dataProtector.Unprotect(id));
            BannerDeleteViewModel model = new BannerDeleteViewModel();
            var banner = await _contentManager.GetPostByIdAsync(PostId);
            model.Id = banner.PostId;
            model.ImagePath = banner.ImagePath;
            model.Summary = banner.PostSummary;
            model.Title = banner.PostTitle;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Delete(BannerDeleteViewModel model)
        {
            if (model == null || model.Id < 1)
            {
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. New banner could not be deleted.";
                return View(model);
            }
            int PostId = model.Id;
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
                model.ViewModelErrorMessage = $"Error! Sorry, an error was encountered. New banner could not be deleted.";
                return View(model);
            }
            TempData["DeleteSuccessMessage"] = "The Banner was successfully deleted!";
            return RedirectToAction("List", "Banners");
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Edit(string id)
        {
            int PostId = 0;
            if (!string.IsNullOrEmpty(id)) { PostId = Convert.ToInt32(_dataProtector.Unprotect(id)); }
            BannerEditViewModel model = new BannerEditViewModel();
            var banner = await _contentManager.GetPostByIdAsync(PostId);
            model.Id = banner.PostId;
            model.ImagePath = banner.ImagePath;
            model.OldImagePath = banner.ImagePath;
            model.Summary = banner.PostSummary;
            model.Title = banner.PostTitle;
            model.EnableComments = banner.EnableComment;
            model.IsHidden = banner.IsHidden;
            model.Id = banner.PostId;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> Edit(BannerEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post();
                string absoluteFilePath = string.Empty;
                string uploadedFilePath = string.Empty;

                if (model.BannerImage != null)
                {
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.BannerImage.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }
                    uploadedFilePath = "uploads/cms/" + Guid.NewGuid().ToString() + "_" + model.BannerImage.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadedFilePath);
                    await model.BannerImage.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));

                    FileInfo oldFile = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath, model.OldImagePath));
                    if (oldFile.Exists)
                    {
                        oldFile.Delete();
                    }
                }

                post.IsHidden = model.IsHidden;
                post.PostSummary = model.Summary;
                post.PostTypeId = (int)PostType.Banner;
                post.ModifiedDate = DateTime.UtcNow;
                post.ModifiedBy = HttpContext.User.Identity.Name ?? "Unknown";
                post.PostTitle = model.Title;
                post.ImagePath = uploadedFilePath;
                post.EnableComment = model.EnableComments;

                if (await _contentManager.UpdatePostAsync(post))
                {
                    model.ViewModelSuccessMessage = $"Congratulations! Banner was updated successfully.";
                }
                else
                {
                    FileInfo file = new FileInfo(absoluteFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    model.ViewModelErrorMessage = $"Error! An error was encountered. New banner could not be added.";
                }
            }
            return View(model);
        }

    }
}