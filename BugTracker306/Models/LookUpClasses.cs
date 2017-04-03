// 2017 revisions
//  3/07  JMz   Added StsGroup, Severity; putting in the ICollection and Hashset pattern per W10d2*.png
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{

    public class TicketPriority
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        /*** per AR 3/07 2:30pm public virtual ICollection<Ticket> Tickets { get; set; }
        public TicketPriority()
        {
            Tickets = new HashSet<Ticket>(); // We are using lazy loading, so this is taking to load the database into memory when the application starts.
        } ***/
    }

    public class TicketSeverity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        /*** per AR 3/07 2:30pm
        public virtual ICollection<Ticket> Tickets { get; set; }
        public TicketSeverity 
            Tickets = new HashSet<Ticket>(); // We are using lazy loading, so this is taking to load the database into memory when the application starts.
        } ***/
    }

    public class TicketStatus
    {
        public int Id { get; set; }
        [Required]
        public string StsName { get; set; }
        [Required]
        public string StsGroup { get; set;  } // Open, Coding, Testing, Closed
        /*** per AR 3/07 2:30pmpublic virtual ICollection<Ticket> Tickets { get; set; }
        public TicketStatus ()
        {
            Tickets = new HashSet<Ticket>(); // We are using lazy loading, so this is taking to load the database into memory when the application starts.
        } ***/

    }



    public class TicketType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        /*** per AR 3/07 2:30pm public virtual ICollection<Ticket> Tickets { get; set; }
        public TicketType()
        {
            Tickets = new HashSet<Ticket>(); // We are using lazy loading, so this is taking to load the database into memory when the application starts.
        } ***/
    }

}