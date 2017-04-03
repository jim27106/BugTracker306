using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker306.Models;

namespace BugTracker306.Controllers
{
    public class TicketHistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketHistories
        public ActionResult ThIndex(int ?id)
        {
            var ticketHistories = db.TicketHistories.AsNoTracking()
                .Where(th => th.TicketId == id)
                .Include(t => t.ticket);
            if (id != null)
            {   // showing one tickets.
                ViewBag.ListVolume = "1";    // display the ticket title in the heading
//ToDo: Jason, Is this implementation good, bad, or ugly?
                ViewBag.TicketTitle = ticketHistories.First().ticket.Title;
                ViewBag.ProjectName = ticketHistories.First().ticket.Proj.Name;
                ticketHistories = ticketHistories.Where(th => th.TicketId == id)
                    // System.NotSupportedException was unhandled by user code
                    // Message = LINQ to Entities does not recognize the method 'System.Linq.IQueryable`1[BugTracker306.Models.TicketHistory] Reverse[TicketHistory]
                    // .Reverse()
                    ;
            }
            else if (User.IsInRole("Admin"))
            {   // showing multiple tickets.
                ViewBag.ListVolume = "many";    // display the ticket title column
                // ticketHistories = ticketHistories.Reverse();
            }
            else if (!User.IsInRole("Admin"))
            {   //ToDo: use Temp Data to send an error message  
                string sErrTEMPDATA = "Request for all histories ignored.  You may go to"
                    + " the Tickets screen and choose the action History to see "
                    + " the history of a particular ticket. [V10913324]";
                return RedirectToAction("hError", "Home");
            }
            //324 var ticketHistories = db.TicketHistories.Include(t => t.ticket);
            return View(ticketHistories.ToList());
        }

        // GET: TicketHistories/Details/5
        public ActionResult ThDetails(int? id)
        {
            if (id == null)
            {
                string TempData = "Oh no you don't. [V10913325]";
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                string TempData = "Nothing to see here.  Move along. [V10913326]";
                return HttpNotFound();
            }
//ToDo: Jason, Is this implementation good, bad, or ugly?
            ViewBag.TicketTitle = ticketHistory.ticket.Title;
            ViewBag.ProjectName = ticketHistory.ticket.Proj.Name;
            return View(ticketHistory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
