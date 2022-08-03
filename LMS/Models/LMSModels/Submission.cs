using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string UId { get; set; }
        public uint AssignId { get; set; }
        public string Contents { get; set; }
        public DateTime SubmitTime { get; set; }
        public uint Score { get; set; }

        public virtual Assignments Assign { get; set; }
        public virtual Students U { get; set; }
    }
}
