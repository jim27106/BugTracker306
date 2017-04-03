/* 2017 revisions
 * TBD There are a dozen break points in this controller.  Enabled break
    points are on code that has not been tested.  I disable the break point
    as it is triggered.  A disabled break point is a clue of at least one test.
// TBD make sure develpers and submitters can edit assignments.
// TBD seach for secty and fix any security holes
// TBD  update needs to updatedt set
// TBD can admin change submitter?  PM?
// createdt needs to be hidden in view.
// 3/23 tCreate uses a different viewbag that the create post and edit.
// 3/20 tAssign calls helper for notification when developer assigned a  ticket
// 3/20 commented out 8 lines in favor of TicketViewBag sub-method.
// 3/15 tEdit
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
using Microsoft.AspNet.Identity;
using BugTracker306.Helpers;
using System.Threading.Tasks;

namespace BugTracker306.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketHelper tktHlp = new TicketHelper();
        private ProjectsHelper prjHlp = new ProjectsHelper();
        private UserRolesHelper usrHlp = new UserRolesHelper();
        private NotificationHelper ntfHlp = new NotificationHelper();

        private void TicketViewBag (Ticket ticket)
        {
            // Make a viewbag to send info that is already in a ticket to the view.
            ICollection<ApplicationUser> Developers = usrHlp.UsersInRole("Developer");
            //-- unassigned has role of developer
            ICollection<ApplicationUser> Submitters = usrHlp.UsersInRole("Submitter");
            if ((null == ticket.Developer) && (null == ticket.DeveloperId))
                ticket.DeveloperId = tktHlp.UnassignedDeveloperId();

            ViewBag.DeveloperId = new SelectList(Developers, "Id", "DisplayName", ticket.DeveloperId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjId = new SelectList(db.Projects, "Id", "Name", ticket.ProjId);
            ViewBag.SeverityId = new SelectList(db.TicketSeverities, "Id", "Name", ticket.SeverityId);
            ViewBag.sprintId = new SelectList(db.Sprints, "Id", "Name", ticket.sprintId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "StsName", ticket.StatusId);
            ViewBag.SubmitterId = new SelectList(Submitters, "Id", "DisplayName", ticket.SubmitterId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TypeId);
            ViewBag.CreateDt = DateTime.Now; //
        }

        // GET: Tickets
        [Authorize(Roles = "Admin,Developer")]
        public ActionResult tAll()
        {
            return RedirectToAction("tIndex", "Tickets", new { SomeOrAll = 1 });
        }

        public ActionResult tOnlyMy()
        {
            return RedirectToAction("tIndex", "Tickets", new { SomeOrAll = 0 });
        }

        // GET: Tickets
        [Authorize]  // allow any user who has logged in.
        // secty
        public ActionResult tIndex(int? SomeOrAll, int? ProjectId, string Priority)
        {
            int iSomeOrAllFlag = SomeOrAll ?? 0;  // 1=all. 0=some
            ViewBag.DevAll = (1 == iSomeOrAllFlag) ? "ALL" : "OnlyMy";
            if (-1 == SomeOrAll)
            {
                Priority = "OOPS";
            }
            int OurPriId = prjHlp.GetPriorityId(Priority);
            // 3/15 JasonA: 
            // YES. The t => is referring to stuff that is included in the model we are sending to the view.
            // If there is no .Include then everything is included.
            // if not included then it sends just the id, and not the name from the lookup table.
            List<Ticket> thierTickets;
            List<Ticket> tickets = db.Tickets.Include(t => t.Developer)
                .Include(t => t.Priority).Include(t => t.Proj)
                .Include(t => t.Severity).Include(t => t.sprint)
                .Include(t => t.Status).Include(t => t.Submitter).Include(t => t.Type).ToList();

            string sGUID = User.Identity.GetUserId();
            if (1 == iSomeOrAllFlag && User.IsInRole("Tester"))
            {
                tickets = tickets.Where(t86 => t86.ProjId == iSomeOrAllFlag).ToList();
            }
            if (0 == iSomeOrAllFlag && User.IsInRole("Tester"))
            {
                tickets = tickets.Where(t13 => t13.SubmitterId == sGUID).ToList();
            }
            if (0 == iSomeOrAllFlag && User.IsInRole("Submitter"))
            {
                thierTickets = tickets.Where(t13 => t13.SubmitterId == sGUID).ToList();
                if (thierTickets.Count > 0)
                    tickets = thierTickets;
                else
                    ViewBag.Warning = "No tickets created by you.  Showing all tickets. [V10913329]";
            }
            if (1 == iSomeOrAllFlag && User.IsInRole("Developer"))
            {
                // Jason lecture 2:39 PM 3 / 17 / 2017 - I asked for code of entire helper method.
                string userId = User.Identity.GetUserId();
                tickets = db.Users.Find(userId)   // a single record
                    .Projects // get all his/her projects
                    .SelectMany(t => t.Tickets).ToList();
                    // NO GO .AsQueryAble()
                    // NO GO .ToList() ;  // use lambda to get all of the tickets
            }
            if (0 == iSomeOrAllFlag && User.IsInRole("Developer"))
            {
                thierTickets = tickets.Where(t13 => t13.DeveloperId == sGUID).ToList();
                if (thierTickets.Count > 0)
                    tickets = thierTickets;
                else
                    ViewBag.Warning += "No tickets assigned to you. "
                        + " Showing all tickets on your projects. [V10913330]";
            }
            if (0 != OurPriId) // if a developer hacks to here, fine.
            {
                thierTickets = tickets.Where(t12 => t12.PriorityId == OurPriId).ToList();
                if (thierTickets.Count > 0)
                    tickets = thierTickets;
                else
                    ViewBag.Warning += " No tickets with priority "
                        + Priority + ".  Showing all tickets. [V10913332]";
            }
            if (0 != OurPriId && 0 == iSomeOrAllFlag && User.IsInRole("Project Manager"))
            {   // A request for a specific priority is against a specific project
                // so the request for the some tickets in his/her projects makes no sense.
                ICollection<Project> hisProjects = prjHlp.ListUserProjects(sGUID);
                thierTickets = new List<Ticket>(); // 328
                foreach (var project315 in hisProjects)
                    foreach (var ticket in project315.Tickets)
                        thierTickets.Add(ticket);
                if (thierTickets.Count > 0)
                    tickets = thierTickets;
                else
                    ViewBag.Warning = "No tickets in your "
                    + hisProjects.Count + " project(s).  Showing all tickets. [V10913331]";
                //328 return View(myTickets);
            }
            if (0 == tickets.Count)
            {
                //Todo: High/ If they request project tickets and they have none on the project customize message.
                string tempdata = "You have no tickets assigned.  Please contact your administrator. [V10913302]";
                RedirectToAction("Index", "Home") ;
            }
            else if (null != ProjectId && ProjectId > 0)
            {
                tickets = tickets.Where(t13 => t13.ProjId == ProjectId).ToList(); // 328
            }
            if (User.IsInRole("Developer") || User.IsInRole("Submitter")
                || User.IsInRole("Admin") || User.IsInRole("Project Manager"))

                return View(tickets.ToList());
            else
            { //Todo: tempdata = "You have no role assigned.  Please contact your administrator. [V10913303]"
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Tickets/Details/5
//Todo: secty for get ticket details
        public ActionResult tDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("tIndex");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                string TempData_ErrorMessage = "Unable to find that ticket. [F10913328]";
                return RedirectToAction("tIndex");
                //return HttpNotFound();
            }
            return View(ticket);
        }
 
        // GET: Tickets/Create
        // secty
        public ActionResult tCreate(int? iWelcome)
        {
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.ProjId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.SeverityId = new SelectList(db.TicketSeverities, "Id", "Name");
            ViewBag.sprintId = new SelectList(db.Sprints, "Id", "Name");
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "Id", "StsName");
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // secty
        public ActionResult tCreate([Bind(Include = "Id,Title,Description,CreateDt,UpdateDt,EstHours,EstRemaining,ElapsedHours,PriorityId,SeverityId,sprintId,ProjId,iTktsThatNeedThisDone,iTktsHoldingUpThisTkt,sTktsThatNeedThisDone,sTktsHoldingUpThisTkt,StatusId,TypeId,SubmitterId,DeveloperId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.CreateDt = DateTime.Now;
                ticket.UpdateDt = DateTime.Now;
                ticket.StatusId = tktHlp.InitialStatus();    // TBD - should get this from a helper class which pulls it from DB.
                ticket.SubmitterId = User.Identity.GetUserId();
                // Can't get using virtual member until I do virtual changes
                // ticket.MirrorSub = ticket.Submitter.Email;
                ticket.MirrorSub = tktHlp.EmailForUser(ticket.SubmitterId);
                // 322jmzticket.DeveloperId = null;
                ticket.DeveloperId = tktHlp.UnassignedDeveloperId();
                ticket.EstHours = 0;
                ticket.EstRemaining = 0;
                ticket.ElapsedHours = DateTime.Now.Minute*100+DateTime.Now.Second; //debug
                ticket.sprintId = tktHlp.InitialSprint();
                string How_do_make_the_box_for_severity_wider_GOOGLE;
                // Not part of SRS.  Not part of submitter's job.
                ticket.iTktsHoldingUpThisTkt = 0;
                ticket.iTktsThatNeedThisDone = 0;
                ticket.sTktsHoldingUpThisTkt = "";
                ticket.sTktsThatNeedThisDone = "";
                //bool MassTicketCreation = false; (This is special test prep logic)
                //if (MassTicketCreation)
                //for (int pp = 1; pp <= 6; pp++)
                //{
                //    ticket.ProjId = pp;
                    db.Tickets.Add(ticket);
                    db.SaveChanges();
                //}

                return RedirectToAction("tIndex");
            }
            TicketViewBag(ticket);
            return View(ticket);
        }

        // cloned from GET: Tickets/Delete/5
        // secty
        public ActionResult tAssign(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ICollection<ApplicationUser> DeveloperObjsOnProj 
                = prjHlp.ListProjectDeveloperObjs(ticket.ProjId);
            DeveloperObjsOnProj.Add(tktHlp.UnassignedDeveloperObj());
            ViewBag.DeveloperId = new SelectList(DeveloperObjsOnProj, "Id", "DisplayName", ticket.DeveloperId);
            return View(ticket);
        }

        // POST: Tickets/Assign (a developer)
        // secty
        [HttpPost, ActionName("tAssign")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> tAssign([Bind(Include = "Id,DeveloperId")] Ticket ticket)
        {   // EntityCommandExecutionException - maybe I forgot the bind?
            ICollection<ApplicationUser> DeveloperObjsOnProj;
            if (ModelState.IsValid)
            {
                Ticket oldTicket = db.Tickets.AsNoTracking().First(tt => tt.Id == ticket.Id);
                string UserGUID = User.Identity.GetUserId();
                db.Tickets.Attach(ticket); // Does this fix InvalidOperationException?
                db.Entry(ticket).Property("DeveloperId").IsModified = true;
                if (null != ticket.DeveloperId) ticket.MirrorDev = tktHlp.EmailForUser(ticket.DeveloperId);
                db.Entry(ticket).Property("MirrorDev").IsModified = true;
                db.Entry(ticket).Property("UpdateDt").IsModified = true;
                ticket.UpdateDt = DateTime.Now;
                try { db.SaveChanges(); }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    DeveloperObjsOnProj
                        = prjHlp.ListProjectDeveloperObjs(ticket.ProjId);
                    ViewBag.DeveloperId = new SelectList(DeveloperObjsOnProj, "Id", "DisplayName", ticket.DeveloperId);
                    return View(ticket);
                }
                Ticket newTicket = db.Tickets.AsNoTracking().First(tt => tt.Id == ticket.Id);
                tktHlp.RecordHistory(UserGUID, oldTicket, newTicket);
                // Notify go InavidOperationException - an async transaction can't be started at this time.
                // I probably need to look at contact us rather than login for eMail template/ source/ plagaiarmas/ other word
                await ntfHlp.GenerateEmailNotification2(oldTicket, newTicket);   // (UserGUID, ticket , "assigned" );
                return RedirectToAction("tDetails"); // ,"Tickets",new {Id = Id});
            }
            DeveloperObjsOnProj
                = prjHlp.ListProjectDeveloperObjs(ticket.ProjId);
            DeveloperObjsOnProj.Add(tktHlp.UnassignedDeveloperObj());
            ViewBag.DeveloperId = new SelectList(DeveloperObjsOnProj, "Id", "DisplayName", ticket.DeveloperId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        // secty
        public ActionResult tEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketViewBag(ticket);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // secty
        //322 Trying to follow Jason's presentation.
        // This needed using System.Threading.Tasks; - or so said 
        public async Task<ActionResult> tEdit([Bind(Include = "Id,Title,Description,CreateDt,UpdateDt,EstHours,EstRemaining,ElapsedHours,PriorityId,SeverityId,sprintId,ProjId,iTktsThatNeedThisDone,iTktsHoldingUpThisTkt,sTktsThatNeedThisDone,sTktsHoldingUpThisTkt,StatusId,TypeId,SubmitterId,DeveloperId")] Ticket ticket)
        //322JMz public ActionResult tEdit([Bind(Include = "Id,Title,Description,CreateDt,UpdateDt,EstHours,EstRemaining,ElapsedHours,PriorityId,SeverityId,sprintId,ProjId,iTktsThatNeedThisDone,iTktsHoldingUpThisTkt,sTktsThatNeedThisDone,sTktsHoldingUpThisTkt,StatusId,TypeId,SubmitterId,DeveloperId")] Ticket ticket)
        {
            Ticket item = ticket;
            if ((User.IsInRole("Project Manager") && prjHlp.IsUserOnProject(User.Identity.GetUserId()
                , ticket.ProjId))
              || (User.IsInRole("Developer") && item.DeveloperId == User.Identity.GetUserId())
              || (User.IsInRole("Submitter") && item.SubmitterId == User.Identity.GetUserId())
              )
            {
                Console.Write("TktCon 262/ OKay to proceed");
            }
            else
            {
                if (User.IsInRole("Admin")) ViewBag.Message = "<p class='text-danger'>Admins are not allowed to edit tickets. [E10913304]</p>";
                else ViewBag.Message = "<p class='text-danger'>access denied</p>";
                TicketViewBag(ticket);
                return View(ticket);
            }

            if (ModelState.IsValid)
            {
                Ticket oldTicket = db.Tickets.AsNoTracking().First(tt => tt.Id == ticket.Id);
                if (null != ticket.SubmitterId) ticket.MirrorSub = tktHlp.EmailForUser(ticket.SubmitterId);
                if (null != ticket.DeveloperId) ticket.MirrorSub = tktHlp.EmailForUser(ticket.DeveloperId);
                ticket.UpdateDt = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                Ticket newTicket = db.Tickets.AsNoTracking().First(tt => tt.Id == ticket.Id);
                string UserGUID = User.Identity.GetUserId();
                tktHlp.RecordHistory(UserGUID, oldTicket, newTicket);

                // do notifications.  it would be possible to have the RecordHisotry() inform
                // the notification logic, but Jason thinks it better to keep them separate.
                var ntfHlp = new NotificationHelper();
                await ntfHlp.GenerateEmailNotification2(oldTicket, newTicket);
                return RedirectToAction("tIndex");

            }
            TicketViewBag(ticket);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        // secty
        public ActionResult tClose(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        // secty
        [HttpPost, ActionName("tClose")]
        [ValidateAntiForgeryToken]
        public ActionResult tCloseConfirmed(int id)
        {
            Ticket oldTicket = db.Tickets.AsNoTracking().First(tt => tt.Id == id);
            // No hard delete for me db.Tickets.Remove(ticket);

            Ticket ticket = db.Tickets.Find(id); // see line 216 if exception
            ticket.StatusId = tktHlp.FinishedStatus();
            ticket.UpdateDt = DateTime.Now;
            db.Entry(ticket).Property("StatusId").IsModified = true;
            db.Entry(ticket).Property("UpdateDt").IsModified = true;
            db.SaveChanges();
            Ticket newTicket = db.Tickets.AsNoTracking().First(tt => tt.Id == id);
            string UserGUID = User.Identity.GetUserId();
            tktHlp.RecordHistory(UserGUID, oldTicket, newTicket);
            string Notify_Developer_On_CloseQQ;
            return RedirectToAction("tIndex");
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
