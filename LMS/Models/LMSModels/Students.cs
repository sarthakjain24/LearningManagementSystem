using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            Enroll = new HashSet<Enroll>();
            Submission = new HashSet<Submission>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string Subject { get; set; }

        public virtual Departments SubjectNavigation { get; set; }
        public virtual ICollection<Enroll> Enroll { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
