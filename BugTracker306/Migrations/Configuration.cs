// 2017 revisions
//  3/07 JMz    added Roles.
namespace BugTracker306.Migrations
{
    using System;
    using System.Data.Entity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Configuration;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker306.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker306.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );

            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));
            string stRole = "Admin";
            if (!context.Roles.Any(r => r.Name == stRole))
            {
                roleManager.Create(new IdentityRole { Name = stRole });
            }
            stRole = "Project Manager";
            if (!context.Roles.Any(r => r.Name == stRole))
            {
                roleManager.Create(new IdentityRole { Name = stRole });
            }
            stRole = "Developer";
            if (!context.Roles.Any(r => r.Name == stRole))
            {
                roleManager.Create(new IdentityRole { Name = stRole });
            }
            stRole = "Lead";
            if (!context.Roles.Any(r => r.Name == stRole))
            {
                roleManager.Create(new IdentityRole { Name = stRole });
            }
            stRole = "Submitter";
            if (!context.Roles.Any(r => r.Name == stRole))
            {
                roleManager.Create(new IdentityRole { Name = stRole });
            }
            stRole = "Demo";
            if (!context.Roles.Any(r => r.Name == stRole))
            {
                roleManager.Create(new IdentityRole { Name = stRole });
            }
            stRole = "AdminStepDown";
            if (!context.Roles.Any(r => r.Name == stRole))
            {
                roleManager.Create(new IdentityRole { Name = stRole });
            }

            var UserManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            string AdminPassword = "gitIgnore";
            string stUserNmEqualsEmail = "J@m.es";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    CreateDt = DateTimeOffset.Now,
                    // It would be nice if this had the create date in there.
                    FirstName = "James",
                    LastName = "Martinez",
                    DisplayName = "James M"
                }, AdminPassword);
            }
            var userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Admin");



            // A. - Don't Know.  It went away
            // QQA3 what does tgus mean?  
            // Either the parameter @objname is ambiguous or the claimed @objtype(COLUMN) is wrong.

            // ** QQQ4 Object reference not set to an instance of an object.
            string CommonPassword = WebConfigurationManager.AppSettings["password-common-1"];
            stUserNmEqualsEmail = "unassigned";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "unassigned",
                    LastName = "unassigned",
                    DisplayName = "unassigned"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Developer");

            stUserNmEqualsEmail = "Devon@MailInAtor.com";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Devon",
                    LastName = "Developer",
                    DisplayName = "Devon D"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Developer");

            stUserNmEqualsEmail = "Susan@MailInAtor.com";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Susan",
                    LastName = "Submitter",
                    DisplayName = "Susan S"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Submitter");

            stUserNmEqualsEmail = "Preston@MailInAtor.com";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Preston",
                    LastName = "Project Manager",
                    DisplayName = "Preston P"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Project Manager");

            stUserNmEqualsEmail = "Adrian@MailInAtor.com";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Adrian",
                    LastName = "Admin",
                    DisplayName = "Adrian A"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Admin");

            stUserNmEqualsEmail = "Dave@seed.edu";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Dave",
                    LastName = "Chappelle",
                    DisplayName = "Dave"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Developer");


            stUserNmEqualsEmail = "Don@seed.edu";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Don",
                    LastName = "King",
                    DisplayName = "Don K"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Developer");

            stUserNmEqualsEmail = "Peter@seed.edu";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Peter",
                    LastName = "Pan",
                    DisplayName = "Pete Pan"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Project Manager");


            stUserNmEqualsEmail = "Paul@seed.edu";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Paul",
                    LastName = "Allen",
                    DisplayName = "Paul Allen"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Project Manager");

            stUserNmEqualsEmail = "Sam@seed.edu";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Sam",
                    LastName = "Sneed",
                    DisplayName = "Sam"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Submitter");

            stUserNmEqualsEmail = "Steve@seed.edu";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Steve",
                    LastName = "Harvey",
                    DisplayName = "Steve"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Submitter");

            stUserNmEqualsEmail = "Tom@seed.edu";
            if (!context.Users.Any(u => u.Email == stUserNmEqualsEmail))
            {
                UserManager.Create(new ApplicationUser
                {
                    UserName = stUserNmEqualsEmail,
                    Email = stUserNmEqualsEmail,
                    FirstName = "Tom",
                    LastName = "Thumb",
                    DisplayName = "Tom"
                }, CommonPassword);
            }
            userId = UserManager.FindByEmail(stUserNmEqualsEmail).Id;
            UserManager.AddToRole(userId, "Tester");

            /******* LookUpClasses TICKET STATUS *******
            var roleManager2 = new RoleManager<TicketStatus>(
            new RoleStore<TicketRole>(context));
            string stName;
            stName = "Urgent";
            if (!context.TicketStatus.Any(r => r.Name == stName))
            { roleManager2.Create(new TicketStatus { StsName = stName });
            }  ****/

            // seeding per Antonio 3/7/17
            /**        if (! context.TicketPriorities.Any (u => u.Name == "high"))
                   { context.TicketPriorities.Add (new TicketPriority { Name = "High" } )); }
                   **/

            /*  Using the MSFT method above where they deal with people.  */
            // Note that we have #-verbiage so the column will sort in DataTables
            context.TicketPriorities.AddOrUpdate(p => p.Name,
                  new TicketPriority { Name = "0-Unprioritized" },
                  new TicketPriority { Name = "1-Low" },
                  new TicketPriority { Name = "2-Medium" },
                  new TicketPriority { Name = "3-High" },
                  new TicketPriority { Name = "4-Urgent" }
               );
/************
            context.TicketStatuses.AddOrUpdate(p => p.StsName,
                // need codes are so we can sort in DataTables
                  // This will causes a Sequence error since the data needs to be removed.
                  new TicketStatus { StsName = "new", StsGroup = "open" },
                  new TicketStatus { StsName = "assigned", StsGroup = "open" },
                  new TicketStatus { StsName = "viewed by developer", StsGroup = "open" },
                  new TicketStatus { StsName = "accepted by developer", StsGroup = "open" },
                  new TicketStatus { StsName = "in progress", StsGroup = "coding" },
                  new TicketStatus { StsName = "put aside", StsGroup = "coding" },
                  new TicketStatus { StsName = "unit test", StsGroup = "testing" },
                  new TicketStatus { StsName = "integration test", StsGroup = "testing" },
                  new TicketStatus { StsName = "acceptance test", StsGroup = "testing" },
                  new TicketStatus { StsName = "unit test", StsGroup = "testing" },
                  new TicketStatus { StsName = "duplicate", StsGroup = "resolved" },
                  // interestingly, I think if someone submits a duplicate ticket
                  // they might should be given access to comment, etc on the ticket that
                  // was duplicated.
                  new TicketStatus { StsName = "finished", StsGroup = "resolved" },
                  new TicketStatus { StsName = "rejected", StsGroup = "resolved" },
                  new TicketStatus { StsName = "abandoned", StsGroup = "resolved" }
               );
*******/
            context.TicketSeverities.AddOrUpdate(p => p.Name,
                // letter codes are so we can sort in DataTables
                new TicketSeverity { Name = "J-security/availability" }, 
                new TicketSeverity { Name = "J-security/editing" }, // someone could edit data: integrity issue
                new TicketSeverity { Name = "H-security/viewing" }, // unauthorized users could view data: confidentiality issue
                new TicketSeverity { Name = "G-grammar" }, //, spelling, and translation errors" },
                new TicketSeverity { Name = "F-inconveniences user" },
                new TicketSeverity { Name = "E-reasonable workarounds exist" },
                new TicketSeverity { Name = "D-no reasonable workaround" },
                new TicketSeverity { Name = "C-Loss of entered data" },
                new TicketSeverity { Name = "B-Loss of existing data" },
                         new TicketSeverity { Name = "H-new feature" },
                                  new TicketSeverity { Name = "D-documentation" }
                );

            context.TicketTypes.AddOrUpdate( p => p.Name,
                  new TicketType { Name = "Unknown" },
                  new TicketType { Name = "Production Fix" },
                  new TicketType { Name = "Software Update" },
                  new TicketType { Name = "New Feature" },
                  new TicketType { Name = "Bug Fix" }
               );
        }
    }
}

