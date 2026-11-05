using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace StudentResultManagement.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }

        [Required(ErrorMessage = "Subject name is required")]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Maximum Marks")]
        public int MaxMarks { get; set; } = 100;

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Passing Marks")]
        public int PassingMarks { get; set; } = 40;
    }
}