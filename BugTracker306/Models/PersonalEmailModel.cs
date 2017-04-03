using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace BugTracker306.Models
{
    public class PersonalEmail
    {
        // per w09d4 Email Service document put in AppStart/IdentityConfig.cs
        // light bulb seems to be using system.net.mail and system.web.configuration for WebConfigurationManager
        // using System.net for NetworkCredential
        // 3/30/17 moved to a separate model file as suggested by Jason Twichell
        public string status { get; set; }
        public async Task SendAsync(MailMessage message)
        {
            var GmailUsername = WebConfigurationManager.AppSettings["username"];
            var GmailPassword = WebConfigurationManager.AppSettings["password"];
            var host = WebConfigurationManager.AppSettings["host"];
            int port = Convert.ToInt32(WebConfigurationManager.AppSettings["port"]);
            status = "trying port" + WebConfigurationManager.AppSettings["port"];
            using (var smtp = new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(GmailUsername, GmailPassword)
            })
            {
                try
                {
                    await smtp.SendMailAsync(message);
                    status = "done";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); await Task.FromResult(0);
                    status = e.Message;
                }
            };
        }
    }
}

