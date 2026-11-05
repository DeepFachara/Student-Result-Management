using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace StudentResultManagement.Models
{
    public class StudentLoginModel
    {
        [Required(ErrorMessage = "Enrollment number is required")]
        [Display(Name = "Enrollment No.")]
        public string EnrollmentNo { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}