using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        [Display(Name = "Ticket Title")]
        public int TicketId { get; set; }
        [Display(Name = "Attached document uploader")]
        public string UserId { get; set; }
        public string MirrorAttUser { get; set; }
        public string FilePath { get; set; }    // per CF 3/6
        public string FileURL { get; set; }     // per CF 3/6
        public string Description { get; set; } // per CF 3/6
        [Display(Name = "Create date")]
        public DateTimeOffset CreateDt { get; set; }        // per CF 3/6
        public virtual Ticket ticket { get; set; }
        public virtual ApplicationUser User { get; set; }   // per CF 3/6
    }
}