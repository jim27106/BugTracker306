/* 2017 revisions
 * 3/27 notifications
 3/16 Jmz   working on Action TaCreate
 3/16 JMz   scaffolded
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker306.Models;
using BugTracker306.Helpers;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Threading.Tasks;

namespace BugTracker306.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private NotificationHelper ntfHlp = new NotificationHelper();
        // GET: TicketAttachments
        public ActionResult TaIndex()
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.ticket).Include(t => t.User);
            return View(ticketAttachments.ToList());
        }

        // GET: TicketAttachments/Details/5
        public ActionResult TaDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        public ActionResult TaCreate()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName");
            return View();
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // secty
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaCreate([Bind(Include = "Id,TicketId,FileURL,Description")]
            TicketAttachment ticketAttachment,
            HttpPostedFileBase image)
        {
            if (ModelState.IsValid && null != image)
            {
                string NeedToCheckFileExtension = "Jason screen shot had it I think.";
                // from Kyle B. - but this is razor code.  hpefully i can refactor it.
                // @if(item.FileUrl != null && (Path.GetExtension(@item.FileUrl) == ".png" 
                //|| Path.GetExtension(@item.FileUrl) == ".jpg" 
                //|| Path.GetExtension(@item.FileUrl) == ".gif" 
                //|| Path.GetExtension(@item.FileUrl) == ".jpeg"))


                if (ImageValidator.IsWebFriendlyImage(image))
                {
                    // if it purports to be an image and isn't then I should reject it.
                }
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    ticketAttachment.FileURL = "/Uploads/" + fileName;
                ticketAttachment.CreateDt = DateTimeOffset.Now;
                ticketAttachment.UserId = User.Identity.GetUserId();
                // setting the virtual member should get the Id set when I do a dbSaveChanges
                // and in this case vis-versa.  I set the ID and hope to get the user later.
                TicketHelper tkthlp = new TicketHelper();
                ticketAttachment.MirrorAttUser = tkthlp.EmailForUser(User.Identity.GetUserId());
                db.TicketAttachments.Add(ticketAttachment);
                db.SaveChanges();
                // above code is working with the TicketID.
                // It does not load the entire ticket.
                Ticket theTicket = db.Tickets.Find(ticketAttachment.TicketId);
                await ntfHlp.GenerateEmailNotification2(ticketAttachment.ticket,
                    ticketAttachment.ticket,
                    "attachment");   // (UserGUID, ticket , "assigned" );
                return RedirectToAction("tDetails", "Tickets", 
                        new { Id = ticketAttachment.TicketId } );
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,FilePath,FileURL,Description,CreateDt")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "DisplayName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
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
