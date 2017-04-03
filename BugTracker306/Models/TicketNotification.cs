using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string NotifAddress { get; set;  }   //JMz probably easier just to record where it was sent.
        public DateTimeOffset SentDt { get; set; }

        public virtual Ticket ticket { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}