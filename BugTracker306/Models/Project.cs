using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class Project        // JMz
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }    // JMz JasonA: Don't know  What is [brackets] around this in diagram.  JT o
        public string Organization { get; set;  } // Jmz
        [Display(Name = "Scheduled begin date")]
        public DateTimeOffset SchedBeginDt { get; set; } // Jmz
        [Display(Name = "Scheduled end date")]
        public DateTimeOffset SchedEndDt { get; set; } // Jmz
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; } // part 2 of setting up one Project to many tickets
        public Project() { 
            Users = new HashSet<ApplicationUser>(); // this is half of creating a many-to-many table 
            Tickets = new HashSet<Ticket>();    // part 3 of setting up one Project to many tickets
            // db design has a key ... he spoke to fast. ...
        }
    }
}