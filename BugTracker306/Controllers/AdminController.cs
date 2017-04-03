using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker306.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTracker306.Helpers;

namespace BugTracker306.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper urh = new UserRolesHelper();  //309JMz


        // GET: ApplicationUsers
        public ActionResult NoRight()
        {
            return View();
        }


        // GET: ApplicationUsers
        public ActionResult aIndex()
        {
            //   An exception of type 'System.InvalidOperationException' occurred in EntityFramework.dll but was not handled in user code
            //         Additional information: Multiple object sets per type are not supported.The object sets 'ApplicationUsers' and 'Users' can both contain instances of type 'BugTracker306.Models.ApplicationUser'.
            // try ;MultipleActiveResultSets=True on connection string
            return View(db.Users.ToList());  // fouled up 309e - 3 hours spinning my wheels on this incomprehinsible nonsense
                                                        //309e this didn't alleviate problem  return View();
        }

        /* fouled up 309e */
        // GET: Admin/ChangeRoles
        //[Authorize]
        public ActionResult ChangeRole(string Id) // passed from action link styled as a button
        {
            if (string.IsNullOrEmpty(Id))    //  (null == Id || "" == Id)
            {
                //ViewBag.ErrorMsg = "No User for role change provided. [E10913301].";
                //// 315 JasonA - can I have a generic home error page which shows an arbitrary message?
                //// THIS IS AN IMPORTANT DEBUGGING TOOL!
                //// look up TempData - Kevin Did it.  Jason will slack me a snip it.
                //error controller.  multiple views or multiplee data.
                //    viewbag message.

                //return RedirectToAction("hError", "Home controller"); // won't get the viewbag
                return View("aIndex");
            }
            ApplicationUser myUser = db.Users.Find(Id);
            // generate a list of roles occupied.
            List<String> RoleIds = urh.ListUserRoleGUIDs(Id);
            // db.Roles will generate a list of all roles.

            // computer works with role objects.  
            // Id is what it works with as option id or name.  
            // We show users name as option value (?) or between the <option> and </option> tags
            // need to pull out list of Ids from the helper mehtod return of role 
            // objects user is in.
            ViewBag.PostId309a = new MultiSelectList(db.Roles, "Id", "Name", RoleIds);
            return View(myUser);
            // on the view end we need html helper SelectList - see it in blog.  drop down list 
            // Html.DropDown List ("PostId309", null)
        }


        // POST: Admin/ChangeRoles
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize]  // error on recently registered user on jm-blog.azure.
                     //[ValidateAntiForgeryToken]
                     // 223jmz fails [ValidateAntiForgeryToken]
        public ActionResult ChangeRole(string Id , List<string> PostId309a /* RoleGUIDs */  )
        // I have no idea what was or is ... IdentityRole userrole

        {
            // The model item passed into the dictionary is of type
            // 'System.Collections.Generic.List`1[BugTracker306.Models.ApplicationUser]', 
            // but this dictionary requires a model item of type 'BugTracker306.Models.ApplicationUser'.

            if (ModelState.IsValid)
            {  // 309e
                ApplicationUser user = db.Users.Find(Id); // Find only works with primary key
                // clear the roles, (and then add them back)
                user.Roles.Clear();
                // loop through selected roles.
                // assign those roles.
                 foreach (var roleGUID309b in PostId309a)
                {
                    urh.AddUserToRoleGUID(Id, roleGUID309b);
                } 
                db.SaveChanges();

                return RedirectToAction("aIndex");
            }

            return View(db.Users.ToList());  // error should be better behaved.
        }

    /* end comment of code 309e */
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
