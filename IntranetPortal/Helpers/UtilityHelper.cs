using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IntranetPortal.Helpers
{
    public class UtilityHelper
    {
        public IConfiguration _config { get; }
        public UtilityHelper(IConfiguration configuration)
        {
            _config = configuration;
        }
        //public static bool IsValidFile(string[] ValidFileTypes, HttpPostedFileBase UploadedFile)
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
            var apiKey = _config.GetSection("AppSettings:SendGrid_API_KEY").Value;
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
    }
}
