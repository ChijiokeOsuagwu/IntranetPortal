using System;
using System.Security;
using System.Security.Claims;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IntranetPortal.Models;
using Microsoft.Extensions.Configuration;
using IntranetPortal.Base.Services;
using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Base.Models.SecurityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using IntranetPortal.Base.Models.BaseModels;

namespace IntranetPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly IContentManagerService _contentManager;
        private readonly IBaseModelService _baseModelService;
        public HomeController(IConfiguration configuration, IContentManagerService contentManager, ISecurityService securityService,
                                IBaseModelService baseModelService)
        {
            _configuration = configuration;
            _contentManager = contentManager;
            _securityService = securityService;
            _baseModelService = baseModelService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            GeneralIndexViewModel model = new GeneralIndexViewModel();
            var claims = HttpContext.User.Claims.ToList();
            string recipientId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            if (await _baseModelService.DbConnectionIsOpenAsync())
            {
                var bs = await _contentManager.GetUnhiddenBannersAsync();
                var ts = await _contentManager.GetAllOtherUnhiddenPostsAsync();
                var an = await _contentManager.GetUnhiddenAnnouncementsAsync();

                if (!string.IsNullOrWhiteSpace(recipientId))
                {
                    ViewData["UnreadMessageCount"] = await _baseModelService.GetUnreadMessagesCount(recipientId);
                }

                model.Banners = bs.ToList();
                model.PostList = ts.ToList();
                model.Announcements = an.ToList();
            }
            else { model.ViewModelErrorMessage = $"Connection could not be established with the database. Please try refreshing the page."; }

            return View(model);
        }

        public async Task<IActionResult> Apps()
        {
            var claims = HttpContext.User.Claims.ToList();
            string recipientId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            if (!string.IsNullOrWhiteSpace(recipientId))
            {
                ViewData["UnreadMessageCount"] = await _baseModelService.GetUnreadMessagesCount(recipientId);
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string UserName = model.Login;
            //string PasswordHash = _securityService.CreatePasswordHash(model.Password);
            bool IsPersistent = model.RememberMe;

            var users = _securityService.GetUsersByLoginId(model.Login).Result;
            if (users == null || users.Count < 1)
            {
                model.ViewModelErrorMessage = "Invalid Login Attempt.";
                model.OperationIsCompleted = false;
                model.OperationIsSuccessful = false;
                return View(model);
            }
            ApplicationUser user = users.FirstOrDefault();
            if (user.UserName == model.Login && _securityService.ValidatePassword(model.Password, user.PasswordHash))
            {

                var claims = new List<Claim>();
                if (!string.IsNullOrEmpty(user.FullName)) { claims.Add(new Claim(ClaimTypes.Name, user.FullName)); }
                if (!string.IsNullOrWhiteSpace(user.Email)) { claims.Add(new Claim(ClaimTypes.Email, user.Email)); }
                if (!string.IsNullOrWhiteSpace(user.Id)) { claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id)); }
                if (!string.IsNullOrWhiteSpace(user.UserType)) { claims.Add(new Claim("UserType", user.UserType)); }
                if (!string.IsNullOrWhiteSpace(user.CompanyCode)) { claims.Add(new Claim("Company", user.CompanyCode)); }

                var roleList = _securityService.GetUserPermissionListByUserIdAsync(user.Id).Result.ToList();
                if (roleList != null && roleList.Count > 0)
                {
                    foreach (var role in roleList)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = model.RememberMe
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                var identity = new ClaimsIdentity(claims, SecurityConstants.ChxCookieAuthentication);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(SecurityConstants.ChxCookieAuthentication, claimsPrincipal, authProperties);

                return RedirectToAction("Index");
            }
            model.ViewModelErrorMessage = "Invalid Login Attempt.";
            model.OperationIsCompleted = false;
            model.OperationIsSuccessful = false;
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
            //return RedirectToAction("Index");
            return LocalRedirect("/Home/Index");
        }

        public async Task<IActionResult> Read(int? id)
        {
            Post post = new Post();
            if (id != null) { post = await _contentManager.GetPostByIdAsync(id.Value); }
            return View(post);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> MessageList(string tp = null)
        {
            MessagesListViewModel model = new MessagesListViewModel();
            var claims = HttpContext.User.Claims.ToList();
            string recipientId = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

            model.MessageRecipientID = recipientId;
            switch (tp)
            {
                case "read":
                    model.MessageList = await _baseModelService.GetReadMessages(recipientId);
                    break;
                case "unread":
                    model.MessageList = await _baseModelService.GetUnreadMessages(recipientId);
                    break;
                default:
                    model.MessageList = await _baseModelService.GetAllMessages(recipientId);
                    break;
            }
            
            model.UnreadMessagesCount = await _baseModelService.GetUnreadMessagesCount(recipientId);
            ViewData["UnreadMessageCount"] = model.UnreadMessagesCount;
            model.ReadMessagesCount = await _baseModelService.GetReadMessagesCount(recipientId);
            model.TotalMesssagesCount = await _baseModelService.GetTotalMessagesCount(recipientId);
            return View(model);
        }


        [HttpPost]
        public async Task<string> DeleteMessage(int id)
        {
            if (id > 0)
            {
                bool IsRemoved = await _baseModelService.DeleteMessageByMessageDetailIDAsync(id);
                if (IsRemoved)
                {
                    return "done";
                }
                else
                {
                    return "failed";
                }
            }
            else
            {
                return "none";
            }
        }

        [HttpPost]
        public async Task<string> DeleteUnRead(string rd)
        {
            if (!string.IsNullOrWhiteSpace(rd))
            {
                bool IsRemoved = await _baseModelService.DeleteUnReadMessagesByRecipientIdAsync(rd);
                if (IsRemoved)
                {
                    return "done";
                }
                else
                {
                    return "failed";
                }
            }
            else
            {
                return "none";
            }
        }

        [HttpPost]
        public async Task<string> DeleteRead(string rd)
        {
            if (!string.IsNullOrWhiteSpace(rd))
            {
                bool IsRemoved = await _baseModelService.DeleteReadMessagesByRecipientIdAsync(rd);
                if (IsRemoved)
                {
                    return "done";
                }
                else
                {
                    return "failed";
                }
            }
            else
            {
                return "none";
            }
        }

        [HttpPost]
        public async Task<string> ReadMessage(int id)
        {
            if (id > 0)
            {
                bool IsRead = await _baseModelService.UpdateReadStatusAsync(id);
                if (IsRead)
                {
                    return "done";
                }
                else
                {
                    return "failed";
                }
            }
            else
            {
                return "none";
            }
        }


        public IActionResult Blank()
        {
            return View();
        }

        public IActionResult Authorize()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult BlueTest()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
