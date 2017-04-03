using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        [Display(Name = "Ticket number")]
        public int TicketId { get; set; }
        [Display(Name = "Property changed")]
        public string Property { get; set; }
        [Display(Name = "Old value")]
        public string OldValue { get; set; }
        [Display(Name = "New value")]
        public string NewValue { get; set; }
        [Display(Name = "User who made change")]
        public string UserId { get; set; }
        [Display(Name = "Change date")]
        [DisplayFormat(DataFormatString = "{0:dddd, M/dd/yy a\t hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTimeOffset ChangeDt { get; set; }
        public virtual Ticket ticket { get; set;  }
        // I prefer UserId to be an email address.  The following "virtual" line set up a Foreign Key (FK)
        // constraint.  (Which I right clicked on in SQL Server Management Studio and removed.)
        // public virtual ApplicationUser User { get; set; }
    }
}