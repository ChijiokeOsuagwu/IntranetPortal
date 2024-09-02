using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Helpers
{
    public class UtilityHelper
    {
        private IConfiguration _config { get; }
        public UtilityHelper(IConfiguration configuration)
        {
            _config = configuration;
        }

        public static bool IsValidFile(string[] ValidFileTypes, IFormFile UploadedFile)
        {
            bool isvalid = false;
            string ext = Path.GetExtension(UploadedFile.FileName);
            for (int i = 0; i < ValidFileTypes.Length; i++)
            {
                if (ext == "." + ValidFileTypes[i])
                {
                    isvalid = true;
                }
            }
            return isvalid;
        }

        public static bool SaveTextToFile(string textToSave, string fileName, string filePath)
        {

            bool writeSuccess = false;
            string fileExtension = ".txt";
            string completeFileName = filePath + fileName + fileExtension;
            using (StreamWriter sw = new StreamWriter(completeFileName))
            {
                sw.WriteLine(textToSave);
                //File.WriteAllLines()
            }

            // Read to confirm that the text was successfully written to the specified file.
            string line = "";
            using (StreamReader sr = new StreamReader(completeFileName))
            {
                if (!String.IsNullOrWhiteSpace(line = sr.ReadLine()))
                {
                    writeSuccess = true;
                }
            }
            return writeSuccess;
        }

        public static string ReadTextFromFile(string fileName, string filePath)
        {
            string fileExtension = ".txt";
            string completeFileName = filePath + fileName + fileExtension;

            string line = "";
            using (StreamReader sr = new StreamReader(completeFileName))
            {
                if (String.IsNullOrWhiteSpace(line = sr.ReadLine()))
                {
                    return String.Empty;
                }
            }
            return line;
        }

        public bool SendEmailWithSendGrid(EmailModel model)
        {
            if (string.IsNullOrWhiteSpace(model.RecipientEmail))
            {
                throw new Exception("Operation completed successfully. But email could not be sent because no email address was found for the recipient.");
            }
            var apiKey = _config.GetSection("EmailSettings:SendGrid_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("officemanager@channelstv.com", "Channels OfficeManager");
            var subject = model.Subject;
            var to = new EmailAddress(model.RecipientEmail, model.RecipientName);
            var bcc = new EmailAddress(model.SenderEmail, model.SenderName);
            var plainContent = model.PlainContent;
            var htmlContent = model.HtmlContent;
            var mailMessage = MailHelper.CreateSingleEmail(from, to, subject, plainContent, htmlContent);
            // await sendGridClient.SendEmailAsync(mailMessage);
            //var msg = MailHelper.CreateSingleTemplateEmail(from, new EmailAddress(to), templateId, dynamicTemplateData);
            try
            {
                var response = client.SendEmailAsync(mailMessage).Result;
                if (response.StatusCode != HttpStatusCode.OK
                    && response.StatusCode != HttpStatusCode.Accepted)
                {
                    var errorMessage = response.Body.ReadAsStringAsync().Result;
                    throw new Exception($"Failed to send mail to {to}, status code {response.StatusCode}, {errorMessage}");
                    //return false;
                }
            }
            catch (WebException exc)
            {
                var errorMessage = new StreamReader(exc.Response.GetResponseStream()).ReadToEnd();
                throw new WebException(new StreamReader(exc.Response.GetResponseStream()).ReadToEnd(), exc);
                //return false;
            }
            return true;
        }

        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                sw,
                new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

        
        //================ Final Evaluation Notification Contents ======================//
        public static string GetPerformanceContractApprovalEmailHtmlContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head>");
            sb.Append("<body style='font-family:sans-serif; font-size:1.2rem;'>");
            sb.Append($"<div>Dear {RecipientName},</div>");
            sb.Append("<p>I trust this email finds you well.</p>");
            sb.Append("<p>A performance appraisal record has just been submitted to you. ");
            sb.Append($"It was submitted by <strong>{AppraiseeName}</strong>, on ");
            sb.Append($"{DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT, for the review and approval ");
            sb.Append("of their Goals and Key Performance Accountabilities (KPAs). ");
            sb.Append("Kindly log in to Channels OfficeManager to action this request. </p>");
            sb.Append("<p>Thank you.</p><div>Regards</div>");
            sb.AppendLine("<div><strong>Channels OfficeManager</strong></div><br/>");
            sb.Append("<div><em>[This is an auto-generated email. <strong>Please do not reply.</strong>]</em></div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        public static string GetPerformanceContractApprovalEmailPlainContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {RecipientName},");
            sb.AppendLine("I trust this email meets you well.");
            sb.Append("A performance appraisal record has just been submitted to you. ");
            sb.Append($"It was submitted by {AppraiseeName}, on ");
            sb.Append($"{DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT, for the review and approval ");
            sb.Append("of their Goals and Key Performance Accountabilities (KPAs). ");
            sb.AppendLine("Kindly log in to Channels OfficeManager to action this request. ");
            sb.AppendLine("Thank you.");
            sb.AppendLine("Regards");
            sb.AppendLine("OfficeManager");
            sb.AppendLine(" ");
            sb.Append("[This is an auto-generated email. Please do not reply.]");

            return sb.ToString();
        }

        public static string GetPerformanceContractApprovalMessageContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("A performance appraisal record has just been submitted to you. ");
            sb.Append($"It was submitted by {AppraiseeName}, on ");
            sb.Append($"{DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT, for the review and approval ");
            sb.Append("of their Goals and Key Performance Accountabilities (KPAs). ");

            return sb.ToString();
        }
        
        //================ Final Evaluation Notification Contents ======================//
        public static string GetRequestForFinalEvaluationEmailHtmlContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head>");
            sb.Append("<body style='font-family:sans-serif; font-size:1.2rem;'>");
            sb.Append($"<div>Dear {RecipientName},</div>");
            sb.Append("<p>I trust this email finds you well.</p>");
            sb.Append($"<p><strong>{AppraiseeName}</strong> has just submitted a performance appraisal record to you for Final Evaluation. ");
            sb.Append($"It was submitted on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT. Kindly log in to Channels OfficeManager to action this request.</p> ");
            sb.Append("<p>Thank you.</p><div>Regards</div>");
            sb.AppendLine("<div><strong>Channels OfficeManager</strong></div><br/>");
            sb.Append("<div><em>[This is an auto-generated email. <strong>Please do not reply.</strong>]</em></div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        public static string GetRequestForFinalEvaluationEmailPlainContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {RecipientName},");
            sb.AppendLine("I trust this email meets you well.");
            sb.Append($"{AppraiseeName} has just submitted a performance appraisal record to you for Final Evaluation. ");
            sb.Append($"It was submitted on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.AppendLine($"{DateTime.Now.ToLongTimeString()} WAT. Kindly log in to Channels OfficeManager to action this request. ");
            sb.AppendLine("Thank you.");
            sb.AppendLine("Regards");
            sb.AppendLine("OfficeManager");
            sb.AppendLine(" ");
            sb.Append("[This is an auto-generated email. Please do not reply.]");

            return sb.ToString();
        }

        public static string GetRequestForFinalEvaluationMessageContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{AppraiseeName} has just submitted a performance appraisal record to you for Final Evaluation. ");
            sb.Append($"It was submitted on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.AppendLine($"{DateTime.Now.ToLongTimeString()} WAT.");

            return sb.ToString();
        }


        //================ Evaluation Result Approval Notification Contents ======================//
        public static string GetRequestForEvaluationResultApprovalEmailHtmlContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head>");
            sb.Append("<body style='font-family:sans-serif; font-size:1.2rem;'>");
            sb.Append($"<div>Dear {RecipientName},</div>");
            sb.Append("<p>I trust this email finds you well.</p>");
            sb.Append($"<p>Please this request is for the approval of the Final Evaluation Result for <strong>{AppraiseeName}</strong>. ");
            sb.Append($"It was submitted to you on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT. Kindly log in to Channels OfficeManager to action this request.</p> ");
            sb.Append("<p>Thank you.</p><div>Regards</div>");
            sb.AppendLine("<div><strong>Channels OfficeManager</strong></div><br/>");
            sb.Append("<div><em>[This is an auto-generated email. <strong>Please do not reply.</strong>]</em></div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        public static string GetRequestForEvaluationResultApprovalEmailPlainContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {RecipientName},");
            sb.AppendLine("I trust this email meets you well.");
            sb.Append($"Please this request is for the approval of the Final Evaluation Result for {AppraiseeName}. ");
            sb.Append($"It was submitted to you on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.AppendLine($"{DateTime.Now.ToLongTimeString()} WAT. Kindly log in to Channels OfficeManager to action this request.");
            sb.AppendLine("Thank you.");
            sb.AppendLine("Regards");
            sb.AppendLine("OfficeManager");
            sb.AppendLine(" ");
            sb.Append("[This is an auto-generated email. Please do not reply.]");

            return sb.ToString();
        }

        public static string GetRequestForEvaluationResultApprovalMessageContent(string RecipientName, string AppraiseeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Please this request is for the approval of the Final Evaluation Result for {AppraiseeName}. ");
            sb.Append($"It was submitted to you on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.AppendLine($"{DateTime.Now.ToLongTimeString()} WAT.");

            return sb.ToString();
        }


        //================ HR Department Return Notification Contents ======================//
        public static string GetHrReturnAppraisalEmailHtmlContent(string RecipientName, string HrRepName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head>");
            sb.Append("<body style='font-family:sans-serif; font-size:1.2rem;'>");
            sb.Append($"<div>Dear {RecipientName},</div>");
            sb.Append("<p>I trust this email finds you well.</p>");
            sb.Append("<p>The HR Department has just returned your performance appraisal record. ");
            sb.Append("This is to enable you make some necessary corrections. ");
            sb.Append("Kindly check your appraisal notes for instructions from the HR Department. ");
            sb.Append($"You may also want to contact <strong>{HrRepName}</strong>, for further guidance. ");
            sb.Append($"It was returned on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT. ");
            sb.Append("Please login to Channels OfficeManager for your necessary action. </p>");
            sb.Append("<p>Thank you.</p><div>Regards</div>");
            sb.AppendLine("<div><strong>Channels OfficeManager</strong></div><br/>");
            sb.Append("<div><em>[This is an auto-generated email. <strong>Please do not reply.</strong>]</em></div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        public static string GetHrReturnAppraisalEmailPlainContent(string RecipientName, string HrRepName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Dear {RecipientName},");
            sb.AppendLine("I trust this email meets you well.");
            sb.Append("The HR Department has just returned your performance appraisal record. ");
            sb.Append("This is to enable you make some necessary corrections. ");
            sb.Append("Kindly check your appraisal notes for instructions from the HR Department. ");
            sb.Append($"You may also want to contact {HrRepName}, for further guidance. ");
            sb.Append($"It was returned on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT. ");
            sb.Append("Please login to Channels OfficeManager for your necessary action. ");
            sb.AppendLine("Thank you.");
            sb.AppendLine("Regards");
            sb.AppendLine("OfficeManager");
            sb.AppendLine(" ");
            sb.Append("[This is an auto-generated email. Please do not reply.]");

            return sb.ToString();
        }

        public static string GetHrReturnAppraisalMessageContent(string RecipientName, string HrRepName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("The HR Department returned your performance appraisal record. ");
            sb.Append("This is to enable you make some necessary corrections. ");
            sb.Append("Check your appraisal notes for instructions from the HR Department. ");
            sb.Append($"Or contact {HrRepName}, for further guidance. ");
            sb.Append($"It was returned on {DateTime.Now.ToLongDateString()} at exactly ");
            sb.Append($"{DateTime.Now.ToLongTimeString()} WAT. ");

            return sb.ToString();
        }

    }
}