using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using log4net;
using RestSharp;
using RestSharp.Authenticators;
using System.ComponentModel;
using App.Core.Units.Log4Net;
using Sophie.Units;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Sophie.Services.EmailSenderService
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly ILogManager _logManager;
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(EmailSender));
        private readonly ILogger<EmailSender> _logger;
        private readonly IStringLocalizer _localizer;

        public bool SendGridIsEnabled { get; set; }
        public string SendGridUser { get; set; }
        public string SendGridAPIKey { get; set; }

        public bool MailgunIsEnabled { get; set; }
        public string Email_Mailgun_Api { get; set; }
        public string Email_Mailgun_Domain { get; set; }

        public bool SMTPIsEnabled { get; set; }
        public string Email_SMTP_Host { get; set; }
        public string Email_SMTP_Port { get; set; }
        public string Email_SMTP_Username { get; set; }
        public string Email_SMTP_Password { get; set; }

        // https://myaccount.google.com/u/2/lesssecureapps
        // https://accounts.google.com/b/0/DisplayUnlockCaptcha
        public EmailSender(ILogManager logManager, ILogger<EmailSender> logger, IStringLocalizer localizer)
        {
            _logManager = logManager;
            _logger = logger;
            _localizer = localizer;

            // SendGrid
            SendGridIsEnabled = ReadConfig.ReadWithKey<bool>("Email:SendGrid:IsEnabled");
            if (SendGridIsEnabled)
            {
                SendGridUser = ReadConfig.ReadWithKey<string>("Email:SendGrid:SendGridUser");//cnttitk36@gmail.com
                SendGridAPIKey = ReadConfig.ReadWithKey<string>("Email:SendGrid:SendGridAPIKey");//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
            }

            // Mailgun
            MailgunIsEnabled = ReadConfig.ReadWithKey<bool>("Email:Mailgun:IsEnabled");//f9a348b4e7d388bc72fb0b3e613c6131-24e2ac64-85bc531e
            if (MailgunIsEnabled)
            {
                Email_Mailgun_Api = ReadConfig.ReadWithKey<string>("Email:Mailgun:Email_Mailgun_Api");//d27f5b3786b6544933060a5510669002-24e2ac64-b0571c1e
                Email_Mailgun_Domain = ReadConfig.ReadWithKey<string>("Email:Mailgun:Email_Mailgun_Domain");//mail.payroll.vn
            }

            // SMTP
            SMTPIsEnabled = ReadConfig.ReadWithKey<bool>("Email:SMTP:IsEnabled");
            if (SMTPIsEnabled)
            {
                Email_SMTP_Host = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Host", false);//smtp.gmail.com
                Email_SMTP_Port = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Port", false);//587
                Email_SMTP_Username = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Username", false);//calculator.com.vn@gmail.com
                Email_SMTP_Password = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Password", false);//Abc@1234
            }
        }

        public Task SendEmail(string email, string subject, string htmlContent, MemoryStream? fileStream = null, string? fileName = null)
        {
            if (SendGridIsEnabled)
            {
                _ = Execute_SendGrid(email, subject, htmlContent, fileStream, fileName);
            }
            else if (MailgunIsEnabled)
            {
                _ = Execute_Mailgun(email, subject, htmlContent, fileStream, fileName);
            }
            if (SMTPIsEnabled)
            {
                _ = Execute_SMTP(email, subject, htmlContent, fileStream, fileName);
            }
            return Task.CompletedTask;
        }


        //=== 1. SendGrid
        private async Task<Task> Execute_SendGrid(string email, string subject, string htmlContent, MemoryStream? fileStream = null, string? fileName = null)
        {
            Logs.debug($"SendGrid: SendGridUser={SendGridUser}, SendGridAPIKey={SendGridAPIKey}");

            var client = new SendGridClient(SendGridAPIKey);
            var from = new EmailAddress(SendGridUser, Constants.AppDomain ?? "Payroll.vn");
            var to = new EmailAddress(email, "User");
            string htmlContentEdit = HtmlContent.HtmlAdvance(_localizer, htmlContent);//Basic-Advance
            string decodedBody = WebUtility.HtmlDecode(htmlContentEdit);

            // Send
            var msg = MailHelper.CreateSingleEmail(from, to, subject, decodedBody, htmlContent);
            msg.SetClickTracking(false, false); //Disable click tracking. See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            if (fileStream != null && fileName != null)
            {
                Logs.debug($"Attachment: {fileName}");
                await msg.AddAttachmentAsync(fileName, fileStream);
            }
            Response response = await client.SendEmailAsync(msg);
            Logs.debug($"SendGrid: {Newtonsoft.Json.JsonConvert.SerializeObject(response.StatusCode, JsonSettings.SettingForNewtonsoftPretty)}");
            return Task.CompletedTask;
        }


        public static async Task<Task> Test_SendGrid(MemoryStream? fileStream = null, string? fileName = null)
        {
            var SendGridUser = ReadConfig.ReadWithKey<string>("Email:SendGrid:SendGridUser");//cnttitk36@gmail.com
            var SendGridAPIKey = ReadConfig.ReadWithKey<string>("Email:SendGrid:SendGridAPIKey");//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
            Logs.debug($"SendGrid: SendGridUser={SendGridUser}, SendGridAPIKey={SendGridAPIKey}");

            var client = new SendGridClient(SendGridAPIKey);
            var from = new EmailAddress(SendGridUser, Constants.AppDomain ?? "Payroll.vn");
            var to = new EmailAddress("khoaluantotnghiep062017@gmail.com", "User");

            string subject = $"Test send email use SendGrid at {DateTimes.Now()}";
            string htmlContentEdit = HtmlContent.Example_HtmlAdvance();//Basic-Advance
            string decodedBody = WebUtility.HtmlDecode(htmlContentEdit);

            // Send
            var msg = MailHelper.CreateSingleEmail(from, to, subject, decodedBody, htmlContentEdit);
            msg.SetClickTracking(false, false); //Disable click tracking. See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            if (fileStream != null && fileName != null)
            {
                Logs.debug($"Attachment: {fileName}");
                await msg.AddAttachmentAsync(fileName, fileStream);
            }
            Response response = await client.SendEmailAsync(msg);
            Logs.debug($"SendGrid: {Newtonsoft.Json.JsonConvert.SerializeObject(response.StatusCode, JsonSettings.SettingForNewtonsoftPretty)}");
            return Task.CompletedTask;
        }
        //=== 1. SendGrid ./



        //=== 2. Mailgun
        private async Task<Task> Execute_Mailgun(string email, string subject, string htmlContent, MemoryStream? fileStream = null, string? fileName = null)
        {
            Logs.debug($"Mailgun: Email_Mailgun_Api={Email_Mailgun_Api}, Email_Mailgun_Domain={Email_Mailgun_Domain}");

            string htmlContentEdit = HtmlContent.HtmlAdvance(_localizer, htmlContent);//Basic-Advance
            string decodedBody = WebUtility.HtmlDecode(htmlContentEdit);

            // Send
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", Email_Mailgun_Api);
            client.Timeout = -1;
            RestRequest request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("X-Atlassian-Token", "no-check");
            request.AddParameter("domain", Email_Mailgun_Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{Constants.AppDomain ?? "Payroll.vn"} <noreply@{Email_Mailgun_Domain}>");
            request.AddParameter("to", email);
            //request.AddParameter("cc", "cnttitk36@gmail.com");
            request.AddParameter("subject", subject);
            request.AddParameter("text", subject);
            request.AddParameter("html", decodedBody);
            if (fileStream != null && fileName != null)
            {
                Logs.debug($"Attachment: {fileName}");
                var fileBytes = fileStream.ToArray();
                request.AddFile("attachment", fileBytes, fileName);
            }
            try
            {
                IRestResponse response = await client.ExecuteAsync(request);
                Logs.debug($"Mailgun: {Newtonsoft.Json.JsonConvert.SerializeObject(response.Content.ToString(), JsonSettings.SettingForNewtonsoftPretty)}");
            }
            catch (Exception ex)
            {
                Logs.debug($"Exception: {ex}");
            }
            finally
            {
                client.ClearHandlers();
            }
            return Task.CompletedTask;
        }

        public static async Task<Task> Test_Mailgun(MemoryStream? fileStream = null, string? fileName = null)
        {
            var Email_Mailgun_Api = ReadConfig.ReadWithKey<string>("Email:Mailgun:Email_Mailgun_Api");//d27f5b3786b6544933060a5510669002-24e2ac64-b0571c1e
            var Email_Mailgun_Domain = ReadConfig.ReadWithKey<string>("Email:Mailgun:Email_Mailgun_Domain");//mail.payroll.vn
            Logs.debug($"Mailgun: Email_Mailgun_Api={Email_Mailgun_Api}, Email_Mailgun_Domain={Email_Mailgun_Domain}");

            string subject = $"Test send email use Mailgun at {DateTimes.Now()}";
            string htmlContentEdit = HtmlContent.Example_HtmlAdvance();//Basic-Advance
            string decodedBody = WebUtility.HtmlDecode(htmlContentEdit);

            // Send
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", Email_Mailgun_Api);
            client.Timeout = -1;
            RestRequest request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("X-Atlassian-Token", "no-check");
            request.AddParameter("domain", Email_Mailgun_Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{Constants.AppDomain ?? "Payroll.vn"} <noreply@{Email_Mailgun_Domain}>");
            request.AddParameter("to", "khoaluantotnghiep062017@gmail.com");
            //request.AddParameter("cc", "cnttitk36@gmail.com");
            request.AddParameter("subject", subject);
            request.AddParameter("text", subject);
            request.AddParameter("html", decodedBody);
            if (fileStream != null && fileName != null)
            {
                Logs.debug($"Attachment: {fileName}");
                var fileBytes = fileStream.ToArray();
                request.AddFile("attachment", fileBytes, fileName);
            }
            try
            {
                IRestResponse response = await client.ExecuteAsync(request);
                Logs.debug($"Mailgun: {Newtonsoft.Json.JsonConvert.SerializeObject(response.Content.ToString(), JsonSettings.SettingForNewtonsoftPretty)}");
            }
            catch (Exception ex)
            {
                Logs.debug($"Exception: {ex}");
            }
            finally
            {
                client.ClearHandlers();
            }
            return Task.CompletedTask;
        }
        //=== 2. Mailgun ./



        //=== 3. SMTP
        private async Task<Task> Execute_SMTP(string email, string subject, string htmlContent, MemoryStream? fileStream = null, string? fileName = null)
        {
            Logs.debug($"SendSMTP: Email_SMTP_Host={Email_SMTP_Host}, Email_SMTP_Port={Email_SMTP_Port}, Email_SMTP_Username={Email_SMTP_Username}, Email_SMTP_Password={Email_SMTP_Password}");

            string htmlContentEdit = HtmlContent.HtmlAdvance(_localizer, htmlContent);//Basic-Advance
            string decodedBody = WebUtility.HtmlDecode(htmlContentEdit);
            AlternateView textView = AlternateView.CreateAlternateViewFromString(decodedBody, Encoding.UTF8, MediaTypeNames.Text.Plain);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlContentEdit, Encoding.UTF8, MediaTypeNames.Text.Html);
            htmlView = HtmlContent.AddImageForHtmlAdvance(htmlView);//Basic-Advance

            MailMessage message = new MailMessage();
            message.From = new MailAddress("noreply@payroll.vn", Constants.AppDomain ?? "Payroll.vn");
            message.To.Add(new MailAddress(email, "User"));
            message.IsBodyHtml = true;
            message.BodyTransferEncoding = TransferEncoding.Base64;
            message.Subject = subject;
            message.Body = decodedBody;
            message.AlternateViews.Add(textView);
            message.AlternateViews.Add(htmlView);
            if (fileStream != null && fileName != null)
            {
                Logs.debug($"Attachment: {fileName}");
                System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(fileStream, fileName, MediaTypeNames.Application.Octet);
                message.Attachments.Add(data);
                message.Priority = MailPriority.Normal;
            }
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            // Send
            SmtpClient client = new SmtpClient(Email_SMTP_Host);
            client.Port = int.Parse(Email_SMTP_Port);
            if (string.IsNullOrEmpty(Email_SMTP_Username) && string.IsNullOrEmpty(Email_SMTP_Username))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(Email_SMTP_Username, Email_SMTP_Password);
            }
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = (Email_SMTP_Host == "localhost" ? false : true);
            client.Timeout = 15 * 1000;
            try
            {
                client.SendCompleted += (object sender, AsyncCompletedEventArgs e) => {
                    client.Dispose();
                    message.Dispose();
                    if (e.Cancelled)
                    {
                        Logs.debug($"SendSMTP: Cancelled");
                    }
                    else if (e.Error != null)
                    {
                        Logs.debug($"SendSMTP: Error {e.Error}");
                    }
                    else
                    {
                        Logs.debug($"SendSMTP: {Newtonsoft.Json.JsonConvert.SerializeObject(sender, JsonSettings.SettingForNewtonsoftPretty)}");
                    }
                };
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Logs.debug($"Exception: {ex}");
                _logManager.ErrorLog4net(_log4net, ex, ex.Message);
            }
            finally
            {
                message.Dispose();
                client.Dispose();
            }
            return Task.CompletedTask;
        }

        public static async Task<Task> Test_SMTP(MemoryStream? fileStream = null, string? fileName = null)
        {
            var Email_SMTP_Host = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Host", false);//smtp.gmail.com
            var Email_SMTP_Port = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Port", false);//587
            var Email_SMTP_Username = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Username", false);//calculator.com.vn@gmail.com
            var Email_SMTP_Password = ReadConfig.ReadWithKey<string>("Email:SMTP:Email_SMTP_Password", false);//Abc@1234
            Logs.debug($"SendSMTP: Email_SMTP_Host={Email_SMTP_Host}, Email_SMTP_Port={Email_SMTP_Port}, Email_SMTP_Username={Email_SMTP_Username}, Email_SMTP_Password={Email_SMTP_Password}");

            string subject = $"Test send email use SMTP at {DateTimes.Now()}";
            string htmlContentEdit = HtmlContent.Example_HtmlAdvance();//Basic-Advance
            string decodedBody = WebUtility.HtmlDecode(htmlContentEdit);
            AlternateView textView = AlternateView.CreateAlternateViewFromString(decodedBody, Encoding.UTF8, MediaTypeNames.Text.Plain);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlContentEdit, Encoding.UTF8, MediaTypeNames.Text.Html);
            htmlView = HtmlContent.AddImageForHtmlAdvance(htmlView);//Basic-Advance

            MailMessage message = new MailMessage();
            message.From = new MailAddress("noreply@payroll.vn", Constants.AppDomain ?? "Payroll.vn");
            message.To.Add(new MailAddress("khoaluantotnghiep062017@gmail.com", "User"));
            message.IsBodyHtml = true;
            message.BodyTransferEncoding = TransferEncoding.Base64;
            message.Subject = subject;
            message.Body = decodedBody;
            message.AlternateViews.Add(textView);
            message.AlternateViews.Add(htmlView);
            if (fileStream != null && fileName != null)
            {
                Logs.debug($"Attachment: {fileName}");
                System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(fileStream, fileName, MediaTypeNames.Application.Octet);
                message.Attachments.Add(data);
                message.Priority = MailPriority.Normal;
            }
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            // Send
            SmtpClient client = new SmtpClient(Email_SMTP_Host);
            client.Port = int.Parse(Email_SMTP_Port);
            if (string.IsNullOrEmpty(Email_SMTP_Username) && string.IsNullOrEmpty(Email_SMTP_Username))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(Email_SMTP_Username, Email_SMTP_Password);
            }
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = (Email_SMTP_Host == "localhost" ? false : true);
            client.Timeout = 15 * 1000;
            try
            {
                client.SendCompleted += (object sender, AsyncCompletedEventArgs e) => {
                    client.Dispose();
                    message.Dispose();
                    if (e.Cancelled)
                    {
                        Logs.debug($"SendSMTP: Cancelled");
                    }
                    else if (e.Error != null)
                    {
                        Logs.debug($"SendSMTP: Error {e.Error}");
                    }
                    else
                    {
                        Logs.debug($"SendSMTP: {Newtonsoft.Json.JsonConvert.SerializeObject(sender, JsonSettings.SettingForNewtonsoftPretty)}");
                    }
                };
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Logs.debug($"Exception: {ex}");
            }
            finally
            {
                message.Dispose();
                client.Dispose();
            }
            return Task.CompletedTask;
        }
        //=== 3. SMTP ./
    }

    public static class ReadConfig
    {
        public static T ReadWithKey<T>(string key, bool isDecrypt = true)
        {
            var currentDirectoryPath = Directory.GetCurrentDirectory();
            var envSettingsPath = Path.Combine(currentDirectoryPath, "envsettings.json");
            var envSettings = JObject.Parse(System.IO.File.ReadAllText(envSettingsPath));
            var enviromentValue = envSettings["ASPNETCORE_ENVIRONMENT"].ToString();
            // If debug allway set enviroment Development in launchSettings.json
            //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            //{
            //    enviromentValue = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //}

            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                   .AddJsonFile($"appsettings.{enviromentValue}.json", optional: true, reloadOnChange: false)
                   .Build();

            object value = configuration[key];
            //Logs.debug($"[{key}] : {Newtonsoft.Json.JsonConvert.SerializeObject(value, JsonSettings.SettingForNewtonsoft)}");

            if (value.ToString().ToLower() == "true" || value.ToString().ToLower() == "false")
            {
                object value_bool = bool.Parse(value.ToString());
                return (T)value_bool;
            }
            else
            {
                string value_str = value.ToString();
                try
                {
                    if (!string.IsNullOrEmpty(value_str) && isDecrypt)
                    {
                        value = new App.Core.Units.Encrypt(value_str).DecryptString();
                        return (T)value;
                    }
                    return (T)(object)value_str;
                }
                catch (Exception ex)
                {
                    Logs.debug($"[{key}] : {Newtonsoft.Json.JsonConvert.SerializeObject(value, JsonSettings.SettingForNewtonsoft)}");
                    Logs.debug($"Exception: {ex.StackTrace}");
                    return default(T);
                }
            }
        }

    }
}
