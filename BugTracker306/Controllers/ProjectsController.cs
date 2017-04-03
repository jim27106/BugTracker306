/* 2017 revisions
 * 4/01         private void ViewBagPriTix(Project theProject) {
 *  3/27 refactored all or some to just use pIndex.  deleted code since it isin bu2undo folder.
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

namespace BugTracker306.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper urh = new UserRolesHelper();  //310JMz
        private ProjectsHelper prjHlp = new ProjectsHelper(); // from JT 310

        private void ViewBagPriTix(Project theProject) {
            int count = prjHlp.PriHighTix(theProject, "Urgent");
            ViewBag.PriUrgTix = count;
             count = prjHlp.PriHighTix(theProject, "High");
            ViewBag.PriHighTix = count;
             count = prjHlp.PriHighTix(theProject, "Medium");
            ViewBag.PriMedTix = count;
             count = prjHlp.PriHighTix(theProject, "Low");
            ViewBag.PriLowTix = count;
        }

        // GET: Projects
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult pAll()
        {
            return RedirectToAction("pIndex", "Projects", new { pSomeOrAll = 1 });
        }

        public ActionResult pOnlyMy()
        {
            return RedirectToAction("pIndex", "Projects", new { pSomeOrAll = 0 });
        }
        //Todo: was here 3/30 12:30 am
        public ActionResult pIndex(int? pSomeOrAll)
        {
            string sGUID = User.Identity.GetUserId();
            int iSomeOrAllFlag = pSomeOrAll ?? 0;  // 1=all. 0=some
            ViewBag.APMAll = (1 == iSomeOrAllFlag) ? "ALL" : "OnlyMy";
            var listProjects = db.Projects.AsQueryable();
            // List<Project> ourProjects = null;
            // copied from BlogPostsController.cs/ actionresult details
            if (0 == iSomeOrAllFlag)
            {
                var ourProjects = listProjects.Where(pLamb4 => pLamb4.Users
                            .Any(usr311 => usr311.Id == sGUID));
                if (ourProjects.Count() == 0)
                {
                    iSomeOrAllFlag = 1;
                    ViewBag.APMAll = "OnlyAll";
                }
                else listProjects = ourProjects;
            }
            // if (1 == iSomeOrAllFlag) ourProjects = listProjects;
            return View(listProjects);
               // .Where(softdel => softdel.ArchiveLevel <= 0)  // do not show deleted posts
               // .OrderByDescending(p => p.CreateDt);

        }

        // GET: Projects/Assign/5
        public ActionResult pAssign(int? id)
        {
            if ( ! (User.IsInRole("Admin") || User.IsInRole("Project Manager")))
            {   // They shouldn't be able to get here.
                // they have hacked.  I'm tempted to redirect them to dump.com.
                // DO NOT VISIT that site - you have been warned - JMz
                return RedirectToAction("hError","Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int ProjId = id ?? 1;  // Trying to avoid CS1503
            // MultiSelectList 1st parm is objects.  
            // 2nd parm, Id is what it works with as option id or name.  
            // 3rd parm: We show users name as option value (?) or between the <option> and </option> tags
            // need to pull out list of Ids from the helper mehtod return of role objects user is in.
            // Kyle B: Fourth argument for new MultiSelectList is Ids.  It is not objects even if that is what documentation says.
            // Jason T: Fourth argument is objects
            Project OurProject = db.Projects.Find(ProjId);
            if (OurProject == null)
            {
                return HttpNotFound();
            }


            if (0 == OurProject.Users.Count) ViewBag.UserCount = "No";
            else ViewBag.UserCount = OurProject.Users.Count.ToString();
            if (1 == OurProject.Users.Count) ViewBag.UserCount += " User";
            else ViewBag.UserCount += " Users";

            var UsersOnTheProjectIds = prjHlp.UsersOnProjectGUID(ProjId); 
            ViewBag.UserId310u = new MultiSelectList(db.Users, "Id", "DisplayName", UsersOnTheProjectIds);
            ViewBagPriTix(OurProject);
            return View(OurProject);
            // on the view end we need html helper SelectList - see it in blog.  drop down list 
            // Html.DropDown List ("PostId309", null)
        }

        // POST: Projects/pAssign/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //secty [ValidateAntiForgeryToken]
        public ActionResult pAssign([Bind(Include = "Id")] Project project, List<string> UserId310u)
        {
            if (! (User.IsInRole("Admin") || User.IsInRole("Project Manager")))
            {   // They shouldn't be able to get here.
                // they have hacked.  I'm tempted to redirect them to dump.com.
                // DO NOT VISIT that site - you have been warned - JMz
                return RedirectToAction("hError");
            }
            if (ModelState.IsValid)
            {
                // prjh.RemoveUsersOnProjectExcept(project.Id, UserId310u);
                // Exception Details: System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
                foreach (ApplicationUser uu in db.Users)
                    prjHlp.RemoveUserFromProject(uu.Id, project.Id);
                prjHlp.AddUsersToProject(project.Id, UserId310u);

                //db.Entry(project).State = EntityState.Modified;
                // done in helper methods db.SaveChanges();
                return RedirectToAction("pIndex");
            }
            ViewBagPriTix(project);
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult pCreate()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager")]  
        //309 so the bind says incoming from the view is a project variable of type project.
        // containing the properties listed in the Include.
        // this tells C# what do expect out of this project.
        // it helps determine if the model state is valid
        // nullable items can be skipped.
        // kyle whittles items down to what he needs from user.
        // automatic stuff doesn't to be listed in bind(Include=...
        public ActionResult pCreate([Bind(Include = "Id,Name,Description,Organization,SchedBeginDt,SchedEndDt")]
                Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                if (User.IsInRole("Project Manager"))
                {
                    prjHlp.AddUserToProject(User.Identity.GetUserId(), project.Id);
                }
                return RedirectToAction("pIndex");
            }

            return View(project);
        }

        // GET: Projects/Details/5
        public ActionResult pDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBagPriTix(project);
            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult pEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBagPriTix(project);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult pEdit([Bind(Include = "Id,Name,Description,Organization,SchedBeginDt,SchedEndDt")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("pIndex");
            }
            ViewBagPriTix(project);
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult pDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("pDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult pDeleteConfirmed(int id)
        {
            string sGUID = User.Identity.GetUserId();
            prjHlp.RemoveUserFromProject(sGUID, id);
            return RedirectToAction("pOnlyMy");
            //401JMz not really going to delete a project.
            // Project project = db.Projects.Find(id);
            //db.Projects.Remove(project);
            //db.SaveChanges();
            //return RedirectToAction("Index");
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
