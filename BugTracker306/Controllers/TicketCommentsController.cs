using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker306.Models;
using Microsoft.AspNet.Identity;
using BugTracker306.Helpers;
using System.Threading.Tasks;

namespace BugTracker306.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private NotificationHelper ntfHlp = new NotificationHelper();

        // GET: TicketComments
        public ActionResult tcIndex()
        {
            var ticketComments = db.TicketComments.Include(t => t.ticket).Include(t => t.User);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult tcDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult tcCreate()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> tcCreate([Bind(Include = "Verbiage,TicketId")] TicketComment ticketComment)
        {
            if (ModelState.IsValid && ticketComment.Verbiage.Length > 5)
            {
                ticketComment.CreateDt = DateTimeOffset.Now;
                ticketComment.UserId = User.Identity.GetUserId();
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                // above code is working with the TicketID.
                // It does not load the entire ticket.
                Ticket theTicket = db.Tickets.Find(ticketComment.TicketId);

                await ntfHlp.GenerateEmailNotification2(theTicket, theTicket, "comment");  
                return RedirectToAction("tDetails", "Tickets", new { id = ticketComment.TicketId });
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName", ticketComment.UserId);
            //320jmz return RedirectToAction("tDetails", "Tickets", new { id = ticketComment.TicketId });
            return View(); // error.  Let them edit it. ticketComment);
        }

        // GET: TicketComments/Edit/5
        public ActionResult tcEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult tcEdit([Bind(Include = "Id,UserId,Verbiage,TicketId,CreateDt")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName", ticketComment.UserId);
 // oops.  paste error.  316jmz.  may want it later.           return RedirectToAction("tDetails", "Tickets", new { id = ticketComment.TicketId });
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult tcDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("tcDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
