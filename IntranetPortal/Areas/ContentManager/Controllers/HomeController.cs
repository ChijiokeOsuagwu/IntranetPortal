using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ContentManager.Models;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.ContentManager.Controllers
{
    [Area("ContentManager")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IDataProtector _dataProtector;
        public HomeController(IConfiguration configuration,
                                    IContentManagerService contentManager, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _configuration = configuration;
            _contentManager = contentManager;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> AddDetails(string id)
        {
            AddPostDetailsViewModel model = new AddPostDetailsViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                model.CodedPostId = id;
                model.PostId = Convert.ToInt32(_dataProtector.Unprotect(id));
                var post = await _contentManager.GetPostByIdAsync(model.PostId);
                if (post != null && post.PostId > 0)
                {
                    model.PostDetailsHtml = post.PostDetailsRaw;
                    model.PostDetailsRaw = post.PostDetailsRaw;
                    model.PostTypeId = post.PostTypeId;
                    model.PostTitle = post.PostTitle;
                    model.ModifiedBy = post.ModifiedBy;
                    model.ModifiedDate = post.ModifiedDate;
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<IActionResult> AddDetails(AddPostDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model != null)
                {
                    model.ModifiedDate = DateTime.UtcNow;
                    model.ModifiedBy = HttpContext.User.Identity.Name ?? "System Administrator";
                }

                if (await _contentManager.UpdatePostDetailsAsync(model.PostId, model.PostDetailsHtml,model.ModifiedBy,model.ModifiedDate.Value))
                {
                    model.ViewModelSuccessMessage = $"Congratulations! New Content was added successfully.";
                }
                else
                {
                    model.ViewModelErrorMessage = $"Error! An error was encountered. Content could not be added.";
                }
            }
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "PCMMGACNT, XYALLACCZ")]
        public async Task<string> AddHtmlDetails(string id, string htmlFormat)
        {
            string modifiedBy = "System Administrator";
            DateTime modifiedDate = DateTime.UtcNow;
            string result = "error";
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(htmlFormat))
            {
                int postId = Convert.ToInt32(id);
                var returnValue = await _contentManager.UpdatePostDetailsAsync(postId,htmlFormat,modifiedBy, modifiedDate);
                if (returnValue) { result = "pass"; }
            }
            return result;
        }
    }
}