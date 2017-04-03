// 2017 revisions
//  3/ 9 JMz    per documenation.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker306.Models
{
    public class EmailModel  {
        // To address is in private.config which is linked from <appSettings>
            [Required, Display(Name = "Name")]
            public string FromName { get; set; }
            [Required, Display(Name = "Email"), EmailAddress]
            public string FromEmail { get; set; }
            //[Required]
            public string Subject { get; set; }
            //[Required]
            public string Body { get; set; }
    }
}