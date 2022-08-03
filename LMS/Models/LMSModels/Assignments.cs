using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submission = new HashSet<Submission>();
        }

        public uint AssignId { get; set; }
        public string Name { get; set; }
        public uint CatId { get; set; }
        public string Contents { get; set; }
        public DateTime DueDate { get; set; }
        public uint Points { get; set; }

        public virtual AssignmentCategories Cat { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
