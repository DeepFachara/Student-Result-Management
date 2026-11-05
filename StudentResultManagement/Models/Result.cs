using System.ComponentModel.DataAnnotations;

namespace StudentResultManagement.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        [Required(ErrorMessage = "Please select a student")]
        [Display(Name = "Student")]
        public int StudentID { get; set; }

        [Required(ErrorMessage = "Please select a subject")]
        [Display(Name = "Subject")]
        public int SubjectID { get; set; }

        [Required(ErrorMessage = "Marks are required")]
        [Range(0, 1000, ErrorMessage = "Enter valid marks")]
        public int Marks { get; set; }

        public string StudentName { get; set; }

        public string EnrollmentNo { get; set; }

        public string SubjectName { get; set; }

        public int MaxMarks { get; set; }

        public int PassingMarks { get; set; }
    }
}