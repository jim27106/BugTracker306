// 2017 Revisions to AccountViewModels.cs
//  3/26 JMz    started work on BTCon
//  3/19 JMz    added break points to everything.  I will disable then once that code has been tested.
//  3/08 JMz   original from PDF by Antonio
using BugTracker306.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker306.Helpers
{
    public class DatabaseNotSeededException : Exception
    {
        int Code { get; set; }
        string sMessage { get; set; }
//ToDo: DatabaseNotSeededException
    }

    public static class BTCon
    {
        // These constants are design to be quick enough to type 
        // and generally unique enough that they are searchable.
        // I need to remember right click and peek definition is available in MSFT VS 2015.
        public static string DVR = "Developer";
        public static string PJM = "Project Manager";  // I want to put in "P Leader" and this causes logic errors.  (which are worse than compile time errors.)
    }

    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> userManager 
            = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(
                        new ApplicationDbContext()));
        // instance all methods use.
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInRole(string userIdGUID, string roleName)
        {   // no sure why we want this wrapper.
            // 308JMz changed parm to make it clear in Intelliense userId is GUID
            return userManager.IsInRole(userIdGUID, roleName);
        }

        public ICollection<string> ListUserRoleNames(string userIdGUID)
        {
            return userManager.GetRoles(userIdGUID);
        }

        public List<string> ListUserRoleGUIDs(string userIdGUID)
        {
            // InvalidOperationUser Exception if page is just called.
            IList<string> RoleNames = userManager.GetRoles(userIdGUID);
            List<string> RoleGUIDs = new List<string>();
            if (null == RoleNames) throw new DatabaseNotSeededException();
            foreach (var rolename in RoleNames)
            {
                IdentityRole rr309 = db.Roles.FirstOrDefault(Rl => Rl.Name == rolename);
                RoleGUIDs.Add(rr309.Id);
            }
            return RoleGUIDs;
        }

        public List<IdentityRole> ListUserRoleObjects(string userIdGUID)
        {
            IList<string> RoleNames = userManager.GetRoles(userIdGUID);
            List<IdentityRole> RoleObjects = new List<IdentityRole>();
            foreach (var rolename in RoleNames)
            {
                RoleObjects.Add(db.Roles.FirstOrDefault(Rl => Rl.Name == rolename));
            }
            return RoleObjects;
        }

        public List<IdentityRole> ListAllRoles(string userIdGUID)
        {
            return /* RoleObjects */ db.Roles.ToList();
        }

        public ICollection<string> ListUserOpportunities(string userIdGUID)
        {
            var resultList = new List<string>();
            var List = new List<string> ();
            db.Roles.ToList();
            List.Add("Admin");
            List.Add("Tester");
            List.Add("Developer");
            foreach (var roleName in List)
            {
                if (false == IsUserInRole(userIdGUID, roleName))
                    resultList.Add(roleName);
            }
            return resultList;
        }

        public bool AddUserToRoleName(string userIdGUID, string roleName)
        {
            var result = userManager.AddToRole(userIdGUID, roleName);
            string sForDebug =  result.ToString();
            return result.Succeeded;
        }
/* fouled up 309e */
        public bool AddUserToRoleGUID(string userIdGUID, string roleGUID)
        {
            var roleName = db.Roles.Find(roleGUID).Name;
            var result = userManager.AddToRole(userIdGUID, roleName);
            string sForDebug = result.ToString();
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            // should these throw an exception if they don't succeeed?
            return result.Succeeded;
        }
      /* end 309e */  
        public ICollection<ApplicationUser> UsersInRole(string roleName)
        {
            return UsersRoleCheck(roleName, true);
        }

        public ICollection<ApplicationUser> UsersNotInRole(string roleName)
        {
            return UsersRoleCheck(roleName, false);
        }

        public ICollection<ApplicationUser> UsersRoleCheck(string roleName, bool InOrNot)
        {
            var resultList = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var userXX in List)
            {
                if (InOrNot == IsUserInRole(userXX.Id, roleName))
                    resultList.Add(userXX);
            }
            return resultList;
        }

        public bool DemoRole(string sUserGUID, string roleName)
        {

            string JasonAsNOTrackingRedSquiggle = "I can't figure this out.";
            var uCurrentUser = db.Users  // .AsNoTracking()
                    .FirstOrDefault(uu => uu.Id == sUserGUID);

            string saveRole;
            // Note: It is darn important we check for demo and do ELSE IF on admin
            // because a demo user could have been temporarily put in Admin.
  //          if (uCurrentUser.IsInRole("Demo")) saveRole = "Demo";
    //        else if (User.IsInRole("Admin")) saveRole = "AdminStepDown";
           
            // if user is in role demo
            // saverole = demo
            // else if user is in role admin
            // SaveRole = AdminStepDown

            // remove all roles
            // put in role SaveRole
            // add in roleName 
    //        AddUserToRoleName(userIdGUID, saveRole);
       //     return userManager.IsInRole(userIdGUID, roleName);
       return false;
        }
    }
}