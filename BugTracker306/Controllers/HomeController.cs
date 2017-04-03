// 2017 revisions
//  3/10 JMz    cosmetic cnahges while looking for subject error
//              try lowercase for gMail.com; try 587 instead of 465 (private.config)
//  3/ 9 JMz    email per documenation.  also private.config, idenityfconfig.cs, eMailModel.cs
//          Task requires using system.threading.tasks; 
//          eMailModel needed using static BugTracker306.Models.eMailModel;
//          ConfigurationManager. called for using System.Configuration;
//           MailMessage  called for using System.Net.Mail;
using BugTracker306.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BugTracker306.Models.EmailModel;

namespace BugTracker306.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
                return RedirectToAction("pIndex", "Projects");
            if (User.IsInRole("Developer") || User.IsInRole("Submitter"))
                return RedirectToAction("tIndex", "Tickets");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Bug Tracker 306.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Send me a message";

            return View();
        }


        public ActionResult hError()
        {
            ViewBag.Message = "Error in request";

            return View();
        }


        public ActionResult hLanding()
        {
            ViewBag.Message = "Error in request";

            return View();
        }


        public ActionResult hWelcome()
        {
            ViewBag.Welcome = "newbie";

            return View("About");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body1 = "<p>Email From: <bold>{0}</bold>        "
                        + "({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "Bug Tracker <JamesSGCF@gmail.com>";
                    model.Body = "This is a message from your portfolio site.  The name and"
                        + "the email of the contacting person is above.";

                    var email = new MailMessage(from,
                        ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Portfolio Contact Email",
                        Body = string.Format(body1, model.FromName,
                            model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    if ("done" == svc.status)
                    {
                        return RedirectToAction("Sent");
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); await Task.FromResult(0); }
            }
            @ViewBag.Message = "Sorry, there was an error.";
            return View(model);
        }

        public ActionResult Sent()
        {
            return View();
        }
    } // HomeController
} // namespace