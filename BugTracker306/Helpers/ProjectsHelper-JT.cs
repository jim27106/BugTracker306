using BugTracker306.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

/* 2017 revisions
 * 3/29 JMz find priority ID given string for use in TicketController from view/pDetails
 * 3/19 JMz initial project allows newly registered users to be immediately productive.
 *          call AddUsertoProject without a project ID (which will default to -1.)
 * 3/16 JMz   added Break Points to everything.  If one is disabled it means the code has been tested.
 */

namespace BugTracker306.Helpers
{
    public class ProjectDashboardEntry
    {

    }
    // file:///C:/Users/Jim/Downloads/MVC%20-%20Project%20Helper%20ClassPDF.pdf
    public class ProjectHelperException : Exception { }

    public class ProjectsHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public int PriHighTix(Project project, string sPriorityToCountTickets)
        {
            int debugcount;
            int priId = GetPriorityId(sPriorityToCountTickets);
            //var project = db.Projects.Find(projectId);
            //Todo: this isn't getting the right numbers.  Do I need an asquerable or .include?
            var tickets = project.Tickets.Where(tt => tt.PriorityId == priId);
            return debugcount = tickets.Count();
        }

        public int PriHighTix(int projectId, string sPriorityToCountTickets)
        {
            int debugcount;
            int priId = GetPriorityId(sPriorityToCountTickets);
            var project = db.Projects.Find(projectId);
            //Todo: this isn't getting the right numbers.  Do I need an asquerable or .include?
            var tickets = project.Tickets.Where(tt => tt.PriorityId == priId);
            return debugcount = tickets.Count();
        }
        
        public int GetPriorityId(string PriorityNameContainsThis)
        {
            // I am using contains because "high" is actually in the data
            // base as "3-high" so it sorts above "2-medium".
            // A better design would be to have a second hidden column
            // that DataTables uses for the sort.  In that case I would
            // make the priorities 20,40,60,80 to give me even more flexibility.
            TicketPriority Priority = db.TicketPriorities.FirstOrDefault(pp => 
                    pp.Name.Contains(PriorityNameContainsThis));
            if (null == Priority) return 0;
            else return Priority.Id;
        }

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }

        public bool CanCreateTicket(string userId, int projectId)
        { 
            UserRolesHelper urh = new UserRolesHelper();
            if (urh.IsUserInRole(userId, "Admin")) return true;
            if (urh.IsUserInRole(userId, "Developer")) return false;
            if (urh.IsUserInRole(userId, BTCon.PJM)
                || urh.IsUserInRole(userId, "Summiter"))
                return IsUserOnProject(userId, projectId);
            else return false;     
        }

        public ICollection<ApplicationUser> ListProjectDeveloperObjs(int projectId)
        {   // THIS CODE COVERED
            // note: if there are no developers on project this is empty.
            List<ApplicationUser> uDeveloperList = new List<ApplicationUser>();
            ICollection<ApplicationUser> uProjList = db.Projects.Find(projectId).Users;
            UserRolesHelper urh = new UserRolesHelper();
            if (uProjList != null)
                foreach (ApplicationUser uuu in uProjList)
                {
                    if (urh.IsUserInRole(uuu.Id, "Developer"))
                        uDeveloperList.Add(uuu);
                }
            return uDeveloperList;
        }

        public ICollection<Project> ListUserProjects(string userId)
        {   //  Code has been tested.
            ApplicationUser user = db.Users.Find(userId);
            var projects = user.Projects.ToList();
            return (projects);
        }

        public void AddUserToProject(string userId, int projectId = -1 /* initial project */)
        {
            if (-1 == projectId)
            {
                //             return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
                projectId = db.Projects.FirstOrDefault(p => p.Name == "Miscellaneous").Id;
            }
            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);
                proj.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public int AddUsersToProject(int projectId, List<string> userIds)
        {
            int iCountSuccess = 0;
            Project proj = db.Projects.Find(projectId);
            if (null != userIds) foreach (string userId in userIds)
                    if (!IsUserOnProject(userId, projectId))
                    {
                        var newUser = db.Users.Find(userId);
                        proj.Users.Add(newUser);
                        db.SaveChanges();
                        iCountSuccess++;
                    }
            return iCountSuccess;
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);
                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified; // just saves this obj instance.
                db.SaveChanges();
            }
        }

        // SELECT Id as "GUID" from Projects  PP
        // join Users UU where UU.Id = PP.Id
        // join Roles RR where UU.Role = "Developer"
        // join UserRoles UR where UR.User_Id = UU.Id and UR.Role_Id = RR.Id
        public List<string> ListDeveloperGUIDs(int projectId) //315JMz
        {   // THIS CODE HAS NOT BEEN USED SUCCESSFULLY IN PRODUCTION
            // note: if there are no developers on project this is empty.
            UserRolesHelper urh = new UserRolesHelper();
            List<string> theList = new List<string>();
            ICollection<ApplicationUser> UserList = db.Projects.Find(projectId).Users;
            if (UserList != null)
                foreach (ApplicationUser uuu in UserList)
                {
                    if (urh.IsUserInRole(uuu.Id, "Developer"))
                        theList.Add(uuu.Id);
                }
            return theList;
        }


        public void RemoveAllUsersOnProject(int projectId, List<string> KeepAssignedUserIds)
        {   // THIS CODE HAS NOT BEEN USED SUCCESSFULLY IN PRODUCTION
            // note: If KeepAssi == null then this will remove all users.
            ICollection<ApplicationUser> UserList = db.Projects.Find(projectId).Users;
            if (UserList != null)
                foreach (ApplicationUser uuu in UserList)
                    if (null == KeepAssignedUserIds
                    || !KeepAssignedUserIds.Contains(uuu.Id))
                        RemoveUserFromProject(uuu.Id, projectId);
            return;
        }


        public void RemoveUsersOnProjectExcept(int projectId, List<string> KeepAssignedUserIds)
        {   // note: If KeepAssi == null then this will remove all users.
            ICollection<ApplicationUser> UserList = db.Projects.Find(projectId).Users;
            if (UserList != null)
                foreach (ApplicationUser uuu in UserList)
                    if (null == KeepAssignedUserIds 
                    || ! KeepAssignedUserIds.Contains(uuu.Id))
                        RemoveUserFromProject(uuu.Id, projectId);
            return;
        }

        public List<string> UsersOnProjectGUID(int projectId)
        {
            //if (db.Projects.Find(projectId).Users != null)
            ICollection<ApplicationUser> usrs = db.Projects.Find(projectId).Users;
            List<string> GUIDs = new List<string>();
            if (usrs != null) 
                foreach (ApplicationUser uu in usrs)
                    GUIDs.Add(uu.Id);
            return GUIDs;
        }

        public ICollection<ApplicationUser> UsersOnProjectObjects(int projectId)
        {
            //if (db.Projects.Find(projectId).Users != null)
            return db.Projects.Find(projectId).Users;
            //else return null;
        }

        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }
    }
}