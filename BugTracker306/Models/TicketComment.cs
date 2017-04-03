using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        [Display(Name = "Comment author")]
        public string UserId { get; set; }
        public string MirrorCmtUser { get; set;}
        public virtual ApplicationUser User { get; set; }
        public string Verbiage { get; set; }    // JMz renamed from Comment
        [Display(Name = "Ticket")]
        public int TicketId { get; set; }
        public virtual Ticket ticket { get; set; }
        [Display(Name = "Create date")]
        public DateTimeOffset CreateDt { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        public TicketComment()
        {
            // We are using lazy loading, so this is taking to load the database into memory when the application starts.
            Attachments = new HashSet<TicketAttachment>();
        }
    }
}