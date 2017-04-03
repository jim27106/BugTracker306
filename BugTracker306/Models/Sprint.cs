using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class Sprint
    {
        public int Id { get; set; }
        public string Name { get; set;  }  // eg, Dev1, Dev2,  PhaseTwo 33, PhaseTwo 34
        DateTimeOffset StartDt { get; set; }
        DateTimeOffset FinishDt { get; set;  }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public Sprint()
        {
            Tickets = new HashSet<Ticket>(); // We are using lazy loading, so this is taking to load the database into memory when the application starts.
        } /**/
    }
}