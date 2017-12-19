﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public class CourseDetails
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string AuthorID { get; set; }
        public string AuthorName { get; set; }
        public float Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public int  VideoCount { get; set; }
    }
}
