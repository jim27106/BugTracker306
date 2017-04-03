using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class Ticket
    {   // QQQ1 Why does applicationuser_id show up in the network diagram
        // Tried tocomment out dependency
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name = "Create date")]
        [DisplayFormat(DataFormatString = "{0:dddd, M/dd/yy a\t hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTimeOffset CreateDt { get; set; }
        [Display(Name = "Update date")]
        [DisplayFormat(DataFormatString = "{0:dddd, M/dd/yy a\t hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? UpdateDt { get; set; }

        [Display(Name = "Hours total")]
        public int EstHours { get; set; }
        [Display(Name = "Hours remaining")]
        public int EstRemaining { get; set; }
        [Display(Name = "Hours worked")]
        public int ElapsedHours { get; set; }

        //public int DependencyId { get; set;  }
        //public virtual Ticket Dependency { get; set; } // Note, if this was a collection this program could do scheduling.
        // JMz if I know what tickets this is dependent on it can be scheduled.
        // JMz there is also that if I set it up for successor tickets then tickets with lots
        // jmz of successor tickets should probably be scheduled first.
        [Display(Name = "Sprint")]
        public int? sprintId { get; set; }   // JMz
        public virtual Sprint sprint { get; set; }
        // these items have foreign keys
        [Display(Name = "Project")]
        public int ProjId { get; set; } // part 1 of setting up one Project to many tickets
        // In a perfect world these would be computed.
        [Display(Name = "How many tickets are waiting for this ticket to be finished.")]
        public int iTktsThatNeedThisDone { get; set; }
        [Display(Name = "How many tickets that are holding up work on this.")]
        public int iTktsHoldingUpThisTkt { get; set; }
        [Display(Name = "Notes on tickets that are waiting for this ticket to be finished.")]
        public string sTktsThatNeedThisDone { get; set; }
        [Display(Name = "Notes on tickets that are holding up work on this.")]
        public string sTktsHoldingUpThisTkt { get; set; }
        [Display(Name = "Project")]
        public virtual Project Proj { get; set; }
        [Display(Name = "Priority")]
        public int PriorityId { get; set; }
        public virtual TicketPriority Priority { get; set; }
        [Display(Name = "Severity")]
        public int SeverityId { get; set; }
        public virtual TicketSeverity Severity { get; set; }    // JMz
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        public virtual TicketStatus Status { get; set; }
        [Display(Name = "Type")]
        public int TypeId { get; set; }
        public virtual TicketType Type { get; set; }
        public virtual ApplicationUser Submitter { get; set; }
        // Antonio: I think I should have a TestUser - it should wait.  easy later.
        // Antonio: Is there any benefit for doing this data annotation?  maybe.  upto me.
        [StringLength(128)] // JMz for a GUID
        public string SubmitterId { get; set; }     // Primary key in ApplicationUsers is a string
        public string MirrorSub {get; set;}
        [StringLength(128)] // JMz for a GUID
        public virtual ApplicationUser Developer { get; set; }
        public string DeveloperId { get; set; }    // Primary key in ApplicationUsers is a string
        public string MirrorDev {get; set;}
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketHistory> Histories { get; set; }
        public virtual ICollection<TicketNotification> Notifications { get; set; }
        public Ticket()
        {
            // We are using lazy loading, so this is taking to load the database into memory when the application starts.
            Attachments = new HashSet<TicketAttachment>();
            Comments = new HashSet<TicketComment>();
            Histories = new HashSet<TicketHistory>();
            Notifications = new HashSet<TicketNotification>();
        }
    }
}