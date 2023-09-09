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
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "BAMSNDNTF, XYALLACCZ")]
        public async Task<IActionResult> CreateMessage(int? id, int? tp)
        {
            CreateMessageViewModel model = new CreateMessageViewModel();
            AssignmentEvent assignmentEvent = new AssignmentEvent();
            try
            {
                string eventTitle = "[N/A]";
                if (id != null)
                {
                    assignmentEvent = await _bamsManagerService.GetAssignmentEventByIdAsync(id.Value);
                    if (assignmentEvent != null && !string.IsNullOrWhiteSpace(assignmentEvent.Title))
                    { eventTitle = assignmentEvent.Title; }
                }
                model.MessageType = tp;
                model.MessageID = Guid.NewGuid().ToString();
                model.SentBy = "OfficeManager";
                model.SentTime = DateTime.Now;
                model.ActionUrl = $"./Bams/Home/AssignmentDetails/{id}";
                switch (tp)
                {
                    case 0:
                        model.Subject = "New Live Broadcast Assignment";
                        model.MessageBody = GetNewAssignmentNotificationMessage();
                        break;
                    case 1:
                        model.Subject = "Notice of Assignment Event Extension";
                        model.MessageBody = GetAssignmentExtensionNotificationMessage(eventTitle);
                        break;
                    case 2:
                        model.Subject = "Notice of Assignment Event Cancellation";
                        model.MessageBody = GetAssignmentCancellationNotificationMessage(eventTitle);
                        break;
                    case 3:
                        model.Subject = "New Assignment Status Update";
                        model.MessageBody = GetAssignmentUpdateNotificationMessage(eventTitle);
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

        [Authorize(Roles = "BAMSNDNTF, XYALLACCZ")]
        public IActionResult AddRecipient(int id, int t, string m)
        {
            AddRecipientViewModel model = new AddRecipientViewModel();
            model.MessageID = m;
            model.MessageType = t;
            model.AssignmentEventID = id;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "BAMSNDNTF, XYALLACCZ")]
        public async Task<IActionResult> AddRecipient(AddRecipientViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string previousEndTime = "[N/A]";
                    string currentEndTime = "[N/A]";

                    var employee = await _employeeRecordService.GetEmployeeByNameAsync(model.RecipientName);
                    var assignment = await _bamsManagerService.GetAssignmentEventByIdAsync(model.AssignmentEventID);
                    AssignmentExtension assignmentExtension = new AssignmentExtension();
                    if (model.MessageType != null && model.MessageType.Value == 2)
                    {
                        var entities = await _bamsManagerService.GetAssignmentExtensionsByAssignmentEventIdAsync(assignment.ID.Value);
                        assignmentExtension = entities.FirstOrDefault();
                        if (assignmentExtension.FromTime != null)
                        {
                            previousEndTime = $"{assignmentExtension.FromTime.Value.ToLongDateString()} {assignmentExtension.FromTime.Value.ToLongTimeString()}";
                        }

                        if (assignmentExtension.ToTime != null)
                        {
                            currentEndTime = $"{assignmentExtension.ToTime.Value.ToLongDateString()} {assignmentExtension.ToTime.Value.ToLongTimeString()}";
                        }
                    }

                    if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID))
                    {
                        model.ViewModelErrorMessage = "Sorry, no record was found for the selected recipient.";
                    }
                    else
                    {
                        model.RecipientID = employee.EmployeeID;
                        string recipientEmail = employee.OfficialEmail;
                        string eventVenue = $"{ assignment.Venue} located in { assignment.State} state";

                        string eventStartTime = "[N/A]";
                        if (assignment.StartTime != null)
                        {
                            eventStartTime = $"{assignment.StartTime.Value.ToLongDateString()} {assignment.StartTime.Value.ToLongTimeString()}";
                        }

                        string eventEndTime = "[N/A]";
                        if (assignment.EndTime != null)
                        {
                            eventEndTime = $"{assignment.EndTime.Value.ToLongDateString()} {assignment.EndTime.Value.ToLongTimeString()}";
                        }

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
                                    email.PlainContent = GetNewAssignmentEmailTextMessage(model.RecipientName, assignment.Title, eventVenue, eventStartTime, eventEndTime);
                                    email.HtmlContent = GetNewAssignmentEmailHtmlMessage(model.RecipientName, assignment.Title, eventVenue, eventStartTime, eventEndTime);
                                    break;
                                case 1:
                                    email.Subject = "Notice of Assignment Event Extension";
                                    email.PlainContent = GetAssignmentExtensionEmailTextMessage(model.RecipientName, assignment.Title, previousEndTime, currentEndTime);
                                    email.HtmlContent = GetAssignmentExtensionEmailHtmlMessage(model.RecipientName, assignment.Title, previousEndTime, currentEndTime);
                                    break;
                                case 2:
                                    email.Subject = "Notice of Assignment Event Cancellation";
                                    email.PlainContent = GetAssignmentCancellationEmailTextMessage(model.RecipientName, assignment.Title);
                                    email.HtmlContent = GetAssignmentCancellationEmailHtmlMessage(model.RecipientName, assignment.Title);
                                    break;
                                case 3:
                                    email.Subject = "New Assignment Status Update";
                                    email.PlainContent = GetAssignmentUpdateEmailTextMessage(model.RecipientName, assignment.Title);
                                    email.HtmlContent = GetAssignmentUpdateEmailHtmlMessage(model.RecipientName, assignment.Title);
                                    break;
                                default:
                                    break;
                            }

                            UtilityHelper _utilityHelper = new UtilityHelper(_configuration);
                            if (_utilityHelper.SendEmailWithSendGrid(email))
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
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        //================== Controller Helper Objects ============================================================//
        #region Controller Helper Objects

        //================= New Assignment Notification Messages ====================================//
        private string GetNewAssignmentEmailHtmlMessage(string recipient, string title, string venue, string startTime, string endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body style=\"font - family:sans - serif; font - size:110 %; \">");
            sb.Append($"<div>Dear {recipient},</div><br/><div>I trust this email meets you well.</div><br/>");
            sb.Append("<div>This is to bring to your notice that a new live broadcast assignment has just been scheduled ");
            sb.Append($"as follows:</div><div>Event: &nbsp;&nbsp;<strong>{title}</strong></div>");
            sb.Append($"<div>Venue: &nbsp;<strong>{venue}</strong></div>");
            sb.Append($"<div>Starts: &nbsp;&nbsp;&nbsp;<strong>{startTime}</strong></div>");
            sb.Append($"<div>Ends: &nbsp;&nbsp;&nbsp;<strong>{endTime}</strong></div>");
            sb.Append("<div>Kindly login to OfficeManager for your relevant action.</div><br/> ");
            sb.Append("<div>Regards</div>");
            sb.Append("<div><strong>OfficeManager</strong></div><br/><br/>");
            sb.Append("<div>[NB: This a system generated email. Please do not reply.]</div>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private string GetNewAssignmentEmailTextMessage(string recipient, string title, string venue, string startTime, string endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {recipient},");
            sb.AppendLine("");
            sb.AppendLine("I trust this email meets you well.");
            sb.Append("This is to bring to your notice that a new live broadcast assignment ");
            sb.AppendLine("has just been scheduled as follows:");
            sb.AppendLine($"Event: {title}");
            sb.AppendLine($"Venue: {venue}");
            sb.AppendLine($"Starts: {startTime}");
            sb.AppendLine($"Ends: {endTime}");
            sb.AppendLine("Kindly login to OfficeManager for your relevant action.");
            sb.AppendLine("");
            sb.AppendLine("Regards");
            sb.Append("OfficeManager");
            sb.Append("[NB: This is a system generated email. Please do not reply.]");
            return sb.ToString();
        }

        private string GetNewAssignmentNotificationMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("A new live broadcast assignment has just been scheduled.");
            sb.Append("Click on the link below to see the details of this assignment and carry out any relevant actions on it.");
            return sb.ToString();
        }


        //================= Assignment Event Extension Notification Messages ====================================//
        private string GetAssignmentExtensionEmailHtmlMessage(string recipient, string title, string oldTime, string newTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body style=\"font - family:sans - serif; font - size:110 %; \">");
            sb.Append($"<div>Dear {recipient},</div><br/><div>I trust this email meets you well.</div>");
            sb.Append("<div>This is to bring to your notice that the schedule of the following assignment event ");
            sb.Append($"has just been adjusted as follows:</div><div>Event: &nbsp;&nbsp;<strong>{title}</strong></div>");
            sb.Append($"<div>Previous Closing Time: <strong>{oldTime}</strong></div>");
            sb.Append($"<div>Current Closing Time: &nbsp;&nbsp;&nbsp;<strong>{newTime}</strong></div>");
            sb.Append("<div>Kindly login to OfficeManager for your relevant actions.</div><br/> ");
            sb.Append("<div>Regards</div>");
            sb.Append("<div><strong>OfficeManager</strong></div><br/><br/>");
            sb.Append("<div>[NB: This a system generated email. Please do not reply.]</div>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private string GetAssignmentExtensionEmailTextMessage(string recipient, string title, string oldTime, string newTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {recipient},");
            sb.AppendLine("");
            sb.Append(" I trust this email meets you well.");
            sb.Append($"This is to bring to your notice that the schedule of the following assignment event ");
            sb.AppendLine("has just been adjusted as follows:");
            sb.AppendLine($"Event: {title}");
            sb.AppendLine($"Previous Closing Time: {oldTime}");
            sb.AppendLine($"Current Closing Time: {newTime}");
            sb.AppendLine("Kindly login to OfficeManager for your relevant actions.");
            sb.AppendLine("");
            sb.AppendLine("Regards");
            sb.Append("OfficeManager");
            sb.Append("[NB: This is a system generated email. Please do not reply.]");
            return sb.ToString();
        }

        private string GetAssignmentExtensionNotificationMessage(string eventTitle)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"The following assignment event: [{eventTitle}] has just been extended.");
            sb.Append("Click on the link below to see the details of this extension and carry out any relevant actions accordingly.");
            return sb.ToString();
        }


        //================= Assignment Event Cancellation Notification Messages ====================================//
        private string GetAssignmentCancellationEmailHtmlMessage(string recipient, string title)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body style=\"font - family:sans - serif; font - size:110 %; \">");
            sb.Append($"<div>Dear {recipient},</div><br/><div>I trust this email meets you well.</div>");
            sb.Append("<div>This is to bring to your notice that the following assignment event ");
            sb.Append($"has just been cancelled.</div><div>Event: &nbsp;&nbsp;<strong>{title}</strong></div>");
            sb.Append("<div>Kindly login to OfficeManager for your relevant actions.</div><br/> ");
            sb.Append("<div>Regards</div>");
            sb.Append("<div><strong>OfficeManager</strong></div><br/><br/>");
            sb.Append("<div>[NB: This a system generated email. Please do not reply.]</div>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private string GetAssignmentCancellationEmailTextMessage(string recipient, string title)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {recipient},");
            sb.AppendLine("");
            sb.Append("I trust this email meets you well.");
            sb.Append($"This is to bring to your notice that the assignment event ");
            sb.AppendLine("has just been cancelled.");
            sb.AppendLine($"Event: {title}");
            sb.AppendLine("Kindly login to OfficeManager for your relevant actions.");
            sb.AppendLine("");
            sb.AppendLine("Regards");
            sb.Append("OfficeManager");
            sb.Append("");
            sb.Append("[NB: This is a system generated email. Please do not reply.]");

            return sb.ToString();
        }

        private string GetAssignmentCancellationNotificationMessage(string eventTitle)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"This is is to notify you that the following assignment event: [{eventTitle}] has just been cancelled.");
            sb.Append("Click on the link below for details and carry out any relevant actions accordingly.");
            return sb.ToString();
        }


        //================= Assignment Update Notification Messages ====================================//
        private string GetAssignmentUpdateEmailHtmlMessage(string recipient, string title)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body style=\"font - family:sans - serif; font - size:110 %; \">");
            sb.Append($"<div>Dear {recipient},</div><br/><div>I trust this email meets you well.</div>");
            sb.Append("<div>This is to bring to your notice that a new status update has just been logged for the following assignment.</div>");
            sb.Append($"<div>Event: &nbsp;&nbsp;<strong>{title}</strong></div>");
            sb.Append("<div>Kindly login to OfficeManager for your relevant actions.</div><br/> ");
            sb.Append("<div>Regards</div>");
            sb.Append("<div><strong>OfficeManager</strong></div><br/><br/>");
            sb.Append("<div>[NB: This a system generated email. Please do not reply.]</div>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private string GetAssignmentUpdateEmailTextMessage(string recipient, string title)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {recipient},");
            sb.AppendLine("");
            sb.Append(" I trust this email meets you well.");
            sb.Append($"This is to bring to your notice that a new status update has just been logged for the following assignment.");
            sb.AppendLine($"Event: {title}");
            sb.AppendLine("Kindly login to OfficeManager for your relevant actions.");
            sb.AppendLine("");
            sb.AppendLine("Regards");
            sb.Append("OfficeManager");
            sb.AppendLine("");
            sb.Append("[NB: This is a system generated email. Please do not reply.]");

            return sb.ToString();
        }

        private string GetAssignmentUpdateNotificationMessage(string eventTitle)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"A new status update has just been logged for the following assignment: [{eventTitle}].");
            sb.Append("Click on the link below for details and carry out any relevant actions accordingly.");
            return sb.ToString();
        }

        #endregion
    }
}