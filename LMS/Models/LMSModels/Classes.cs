using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enroll = new HashSet<Enroll>();
        }

        public uint ClassId { get; set; }
        public string Semester { get; set; }
        public uint Year { get; set; }
        public string UId { get; set; }
        public uint CourseId { get; set; }
        public string Loc { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public virtual Courses Course { get; set; }
        public virtual Professors U { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enroll> Enroll { get; set; }
    }
}
