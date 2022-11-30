using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntranetPortal.Areas.BAMS.Models;
using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Helpers;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntranetPortal.Areas.BAMS.Controllers
{
    [Area("Bams")]
    public class NotificationsController : Controller
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBamsManagerService _bamsManagerService;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _employeeRecordService;

        public NotificationsController(IConfiguration configuration, IBaseModelService baseModelService,
                                        IBamsManagerService bamsManagerService, IGlobalSettingsService globalSettingsService,
                                        IErmService employeeRecordService)
        {
            _configuration = configuration;
            _baseModelService = baseModelService;
            _bamsManagerService = bamsManagerService;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateMessage(int? id, int? tp)
        {
            CreateMessageViewModel model = new CreateMessageViewModel();
            AssignmentEvent assignmentEvent = new AssignmentEvent();
            try
            {
                if (id != null)
                {
                    assignmentEvent = await _bamsManagerService.GetAssignmentEventByIdAsync(id.Value);
                }
                model.MessageType = tp;
                model.MessageID = Guid.NewGuid().ToString();
                model.SentBy = "OfficeManager";
                model.SentTime = DateTime.Now;
                model.ActionUrl = $"~/Bams/Home/AssignmentDetails/{id}";
                switch (tp)
                {
                    case 0:
                        model.Subject = "New Live Broadcast Assignment";
                        model.MessageBody = GetNewAssignmentNotificationMessage();
                        break;
                    default:
                        break;
                }

                bool messageIsSent = await _baseModelService.SendMessageAsync(model.ConvertToMessage());
                if (messageIsSent)
                {
                    return RedirectToAction("AddRecipient", new { id = id.Value, t = tp, m = model.MessageID });
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            TempData["ErrorMessage"] = "Sorry, sending notification failed. Please try again.";
            return RedirectToAction("AssignmentDetails", "Home", new { id = id.Value });
        }

        public IActionResult AddRecipient(int id, int t, string m)
        {
            AddRecipientViewModel model = new AddRecipientViewModel();
            model.MessageID = m;
            model.MessageType = t;
            model.AssignmentEventID = id;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipient(AddRecipientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = await _employeeRecordService.GetEmployeeByNameAsync(model.RecipientName);
                var assignment = await _bamsManagerService.GetAssignmentEventByIdAsync(model.AssignmentEventID);
                if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID))
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected recipient.";
                }
                else
                {
                    model.RecipientID = employee.EmployeeID;
                    string recipientEmail = employee.OfficialEmail;
                    string eventVenue = $"{ assignment.Venue} located in { assignment.State} state";
                    

                    MessageDetail md = model.ConvertToMessage();

                    if (await _baseModelService.AddMessageRecipientAsync(md))
                    {
                        EmailModel email = new EmailModel();
                        email.RecipientName = model.RecipientName;
                        email.RecipientEmail = recipientEmail;
                        email.SenderName = "OfficeManager";
                        email.SenderEmail = "officemanager@channelstv.com";
                        switch (model.MessageType)
                        {
                            case 0:
                                email.Subject = "New Live Broadcast Assignment";
                                email.PlainContent = GetNewAssignmentEmailTextMessage(model.RecipientName, "New Live Broadcast Assignment", eventVenue);
                                email.HtmlContent = GetNewAssignmentEmailHtmlMessage(model.RecipientName, "New Live Broadcast Assignment", eventVenue);
                                break;
                            default:
                                break;
                        }

                        UtilityHelper _utilityHelper = new UtilityHelper(_configuration);
                        if(_utilityHelper.SendEmailWithSendGrid(email))
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Notification and email message sent successfully!";
                        }
                        else
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Email message could not be sent, but the notification was sent successfully.";
                        }
                    }
                }
            }
            return View(model);
        }

        //================== Controller Helper Objects ============================================================//
        #region Controller Helper Objects
        private string GetNewAssignmentEmailHtmlMessage(string recipient, string title, string venue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body style=\"font - family:sans - serif; font - size:110 %; \">");
            sb.Append($"<div>Dear {recipient},</div><br/><div>I trust this email meets you well.</div><br/>");
            sb.Append("This is to bring to your notice that a live broadcast event has just been scheduled ");
            sb.Append($"as follows:<br/><p>Event: &nbsp;&nbsp;<strong>{title}");
            sb.Append($"</strong><br/>Venue: &nbsp;<strong>{venue}</strong><br/>");
            sb.Append($"Time: &nbsp;&nbsp;&nbsp;<strong>{DateTime.Now.ToLongDateString()}  ");
            sb.Append($"{DateTime.Now.ToLongTimeString()}</strong> <br/></p>");
            sb.Append("You may want to click on the link below or login to OfficeManager for ");
            sb.Append("your relevant action.</div><br/> <div>Kindly <a href=\"#\"><strong>Click Here ");
            sb.Append("</strong></a>to action this request.</div><br/><div>Regards</div>");
            sb.Append("<div><strong>OfficeManager</strong></div></body></html>");
            return sb.ToString();
        }

        private string GetNewAssignmentEmailTextMessage(string recipient, string title, string venue)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {recipient},");
            sb.Append(" I trust this email meets you well.");
            sb.Append("This is to bring to your notice that a live broadcast assignment ");
            sb.AppendLine("has just been scheduled as follows:");
            sb.Append($"Event: {title}");
            sb.Append($"Venue: {venue}");
            sb.Append($"Time: {DateTime.Now.ToLongDateString()}  ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} ");
            sb.AppendLine("Kindly Click Here for your relevant action.");
            sb.AppendLine("Regards");
            sb.Append("OfficeManager");
            return sb.ToString();
        }

        private string GetNewAssignmentNotificationMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("A live broadcast assignment has just been scheduled.");
            sb.Append("Click on the link below for your relevant action.");
            return sb.ToString();
        }
        #endregion
    }
}