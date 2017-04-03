using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker306.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Family Name")]
        public string LastName  { get; set; }
        [Display(Name = "Notification Preference")]
        public int    NotifPref { get; set; }
        // These email fields are provided as part of the framework.
        // public string eMailAddress { get; set; }
        // public string eMailConfirmed { get; set; }
        [Display(Name = "Alternate Notification Address")]
        public string altNotificationAddress { get; set; }
        [Display(Name = "Notification Frequency")]
        // don't send emails (etc) more frequently then every X minutes.  240 = 4 hrs. 1440 = one day.
        public int NotifFreq { get; set; }

        public string sAvatar   { get; set; }   // File name of an uploaded file
        [Display(Name = "Favorite Color")]
        public string sFavColor { get; set; }  // users favorite color for the by line.
        public string sCreateIP { get; set; } 
        public string sUpdateIP { get; set; }

        // The DateTimeOffset structure represents a date and time value, together with an offset that indicates how much that value differs from UTC
        public DateTimeOffset CreateDt { get; set; }    // JMz
        public DateTimeOffset LastVisitDt { get; set; }
        //public virtual ICollection<Ticket> TicketsSubmitted { get; set; }   // owned
        //public virtual ICollection<Ticket> TicketsAssigned { get; set; }    // developer
        public virtual ICollection<Project> Projects { get; set; }
        public ApplicationUser ()
        {
            // We are using lazy loading, so this is taking to load the database into memory when the application starts.
            //TicketsAssigned = new HashSet<Ticket>();
            //TicketsSubmitted = new HashSet<Ticket>();
            Projects = new HashSet<Project>(); // this is creating a link 
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        // Lookup classes
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<TicketSeverity> TicketSeverities { get; set; }
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }

        public DbSet<Project> Projects { get; set; }
//        public DbSet<ProjectUser> ProjectUsers { get; set; }  Not for many to many tables.
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }
        public DbSet<TicketNotification> TicketNotifications { get; set; }

        //309JT told me to get rid of this line that Bill Gates had inserted.  public System.Data.Entity.DbSet<BugTracker306.Models.ApplicationUser> ApplicationUsers { get; set; }

        //308JT not needed.        public System.Data.Entity.DbSet<BugTracker306.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}