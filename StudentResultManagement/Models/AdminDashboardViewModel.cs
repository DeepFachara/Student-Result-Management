using System.Collections.Generic;

namespace StudentResultManagement.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalResults { get; set; }
        public int TotalPassed { get; set; }
        public int TotalFailed { get; set; }
        public double PassPercentage { get; set; }
        public double AverageScore { get; set; }

        // Result Publishing Status
        public bool IsResultPublished { get; set; } = true;

        // Chart Data - Grade Distribution
        public List<string> GradeLabels { get; set; } = new List<string>();
        public List<int> GradeCounts { get; set; } = new List<int>();

        // Chart Data - Subject Averages
        public List<string> SubjectLabels { get; set; } = new List<string>();
        public List<double> SubjectAverages { get; set; } = new List<double>();
    }
}
