using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Login()
        {
            if (Session["AdminID"] != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Admin login
            if (model.Username == "admin" &&
                model.Password == "admin123")
            {
                Session["AdminID"] = 1;
                Session["AdminUsername"] = "admin";

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password.";

            return View(model);
        }

        public ActionResult Dashboard()
        {
            if (Session["AdminID"] == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new AdminDashboardViewModel();

            try
            {
                var students = DatabaseHelper.GetAllStudents() ?? new List<Student>();
                var subjects = DatabaseHelper.GetAllSubjects() ?? new List<Subject>();
                var results = DatabaseHelper.GetAllResults() ?? new List<Result>();

                viewModel.TotalStudents = students.Count;
                viewModel.TotalSubjects = subjects.Count;
                viewModel.TotalResults = results.Count;

                int passedCount = results.Count(r => r.Marks >= r.PassingMarks);
                int failedCount = results.Count - passedCount;

                viewModel.TotalPassed = passedCount;
                viewModel.TotalFailed = failedCount;
                viewModel.PassPercentage = results.Count > 0
                    ? Math.Round((double)passedCount / results.Count * 100, 1)
                    : 0;
                viewModel.AverageScore = results.Count > 0
                    ? Math.Round(results.Average(r => (double)r.Marks), 1)
                    : 0;

                // Subject averages for chart
                var subjectGroups = results
                    .GroupBy(r => string.IsNullOrWhiteSpace(r.SubjectName) ? "Subject" : r.SubjectName.Trim())
                    .ToList();

                viewModel.SubjectLabels = subjectGroups.Select(g => g.Key).ToList();
                viewModel.SubjectAverages = subjectGroups.Select(g => Math.Round(g.Average(r => (double)r.Marks), 1)).ToList();

                // Grade Distribution Breakdown
                int distinction = 0;
                int firstClass = 0;
                int secondClass = 0;
                int passClass = 0;
                int failed = 0;

                foreach (var r in results)
                {
                    double pct = r.MaxMarks > 0 ? ((double)r.Marks / r.MaxMarks) * 100.0 : r.Marks;
                    if (pct >= 85) distinction++;
                    else if (pct >= 70) firstClass++;
                    else if (pct >= 55) secondClass++;
                    else if (pct >= 40) passClass++;
                    else failed++;
                }

                viewModel.GradeLabels = new List<string> { "Distinction (85-100%)", "First Class (70-84%)", "Second Class (55-69%)", "Pass Class (40-54%)", "Failed (<40%)" };
                viewModel.GradeCounts = new List<int> { distinction, firstClass, secondClass, passClass, failed };

                // Result Publishing Status (default: published)
                viewModel.IsResultPublished = HttpContext.Application["ResultsPublished"] == null
                    || (bool)HttpContext.Application["ResultsPublished"];
            }
            catch (Exception ex)
            {
                ViewBag.DbWarning = "Database connection notice: " + ex.Message;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult TogglePublishStatus()
        {
            if (Session["AdminID"] == null)
            {
                return RedirectToAction("Login");
            }

            bool current = HttpContext.Application["ResultsPublished"] == null
                || (bool)HttpContext.Application["ResultsPublished"];

            HttpContext.Application["ResultsPublished"] = !current;

            return RedirectToAction("Dashboard");
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
