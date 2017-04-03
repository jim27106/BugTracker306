using BugTracker306.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace BugTracker306.Helpers
{
    public class NotificationHelper
    {      
        public async Task HandleNotifications(Ticket oldTicket, Ticket newTicket)
        {
            await GenerateEmailNotification2(oldTicket, newTicket);
            //GenerateOrStoreNotifiUpdateNotifircationTable();
        }

        public void UpdateNotificationTable(EmailModel email322)
        {
            return;
        }

        //324 cloned from HomeController
        // public async Task<ActionResult> Contact(EmailModel model)
        public async Task GenerateEmailNotification2(
            Ticket oldTicket, Ticket newTicket,
            string ReasonForEmail = "assignment")
        {
            string sSentence;
            switch (ReasonForEmail)
            {
                case "attachment":
                    sSentence = " There is a new attachment to ticket # ";
                    break;
                case "comment":
                    sSentence = " There is a new comment on ticket # ";
                    break;
                default:
                case "assignment":
                    sSentence = " You have been assigned to ticket # ";
                    break;
            }
            try
            {
                EmailModel emModel = new EmailModel();
                var body = "<p>Email From: <bold>{0}</bold>        "
                    + "({1})</p><p>Message:</p><p>{2}</p>";
                emModel.FromName = "Bug Tracker";
                emModel.FromEmail = "JamesSGCF@gmail.com";
        
                emModel.Body = "This is a message from BT306.  "
                + sSentence + newTicket.Id.ToString() + ".<br />"
                + " The title is " + newTicket.Title + ".<br />"
                    ;
                string eMailAddrFormat = "{0} <{1}>";
                string FromEmailNameAddr = string.Format(eMailAddrFormat, 
                    emModel.FromName, emModel.FromEmail);
                if (null == newTicket.Developer )
                {
                    string TempData = "There is no developer assigned to this ticket. [V10913327]";
                    return;
                }
                string ToEmailNameAddr = string.Format(eMailAddrFormat,
                    newTicket.Developer.DisplayName, newTicket.Developer.Email );
                var email = new MailMessage(FromEmailNameAddr, ToEmailNameAddr)
                {
                    Subject = "Ticket " + newTicket.Id.ToString() + " " + ReasonForEmail,
                    Body = string.Format(body, emModel.FromName,
                        emModel.FromEmail, emModel.Body),
                    IsBodyHtml = true
                };

                var svc = new PersonalEmail();
                await svc.SendAsync(email);
                if ("done" == svc.status)
                {
                return; //  RedirectToAction("Sent");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
                string TEMPDATA_Notification_ERR = "Sorry, there was an error.";
            }
        }




//need to use class WebConfigurationManager


    }
}