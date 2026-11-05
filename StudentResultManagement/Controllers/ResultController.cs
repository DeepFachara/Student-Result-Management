using System;
using System.Linq;
using System.Web.Mvc;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{
    public class ResultController : Controller
    {
        private bool IsAdmin()
        {
            return Session["AdminID"] != null;
        }

        // RESULT LIST
        // =========================

        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            return View(DatabaseHelper.GetAllResults());
        }

        // CREATE RESULT
        // =========================

        public ActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            ViewBag.Students = new SelectList(
                DatabaseHelper.GetAllStudents(),
                "StudentID",
                "Name");

            ViewBag.Subjects = new SelectList(
                DatabaseHelper.GetAllSubjects(),
                "SubjectID",
                "SubjectName");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Result result)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Subject subject =
                DatabaseHelper.GetSubjectById(result.SubjectID);

            if (subject != null &&
                (result.Marks < 0 ||
                 result.Marks > subject.MaxMarks))
            {
                ModelState.AddModelError(
                    "Marks",
                    "Marks must be between 0 and "
                    + subject.MaxMarks + ".");
            }

            bool alreadyExists =
                DatabaseHelper.ResultExists(
                    result.StudentID,
                    result.SubjectID);

            if (alreadyExists)
            {
                ModelState.AddModelError(
                    "",
                    "Marks for this student and subject already exist.");
            }

            if (ModelState.IsValid)
            {
                DatabaseHelper.AddResult(result);

                TempData["Success"] =
                    "Marks added successfully.";

                return RedirectToAction("Index");
            }

            ViewBag.Students = new SelectList(
                DatabaseHelper.GetAllStudents(),
                "StudentID",
                "Name",
                result.StudentID);

            ViewBag.Subjects = new SelectList(
                DatabaseHelper.GetAllSubjects(),
                "SubjectID",
                "SubjectName",
                result.SubjectID);

            return View(result);
        }

        // EDIT RESULT
        // =========================

        public ActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Result result =
                DatabaseHelper.GetResultById(id);

            if (result == null)
                return HttpNotFound();

            ViewBag.Students = new SelectList(
                DatabaseHelper.GetAllStudents(),
                "StudentID",
                "Name",
                result.StudentID);

            ViewBag.Subjects = new SelectList(
                DatabaseHelper.GetAllSubjects(),
                "SubjectID",
                "SubjectName",
                result.SubjectID);

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(Result result)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Subject subject =
                DatabaseHelper.GetSubjectById(result.SubjectID);

            if (subject != null &&
                (result.Marks < 0 ||
                 result.Marks > subject.MaxMarks))
            {
                ModelState.AddModelError(
                    "Marks",
                    "Marks must be between 0 and "
                    + subject.MaxMarks + ".");
            }

            if (ModelState.IsValid)
            {
                Result existing =
                    DatabaseHelper.GetResultById(result.ResultID);

                if (existing == null)
                    return HttpNotFound();

                DatabaseHelper.UpdateResult(result);

                TempData["Success"] =
                    "Marks updated successfully.";

                return RedirectToAction("Index");
            }

            ViewBag.Students = new SelectList(
                DatabaseHelper.GetAllStudents(),
                "StudentID",
                "Name",
                result.StudentID);

            ViewBag.Subjects = new SelectList(
                DatabaseHelper.GetAllSubjects(),
                "SubjectID",
                "SubjectName",
                result.SubjectID);

            return View(result);
        }

        // DELETE RESULT
        // =========================

        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Result result =
                DatabaseHelper.GetResultById(id);

            if (result == null)
                return HttpNotFound();

            return View(result);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            DatabaseHelper.DeleteResult(id);

            TempData["Success"] =
                "Marks deleted successfully.";

            return RedirectToAction("Index");
        }

        // STUDENT-WISE RESULT
        // =========================

        public ActionResult StudentResult(int? studentId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            ViewBag.Students = new SelectList(
                DatabaseHelper.GetAllStudents(),
                "StudentID",
                "Name",
                studentId);

            if (!studentId.HasValue)
                return View();

            Student student =
                DatabaseHelper.GetStudentById(studentId.Value);

            if (student == null)
                return HttpNotFound();

            var studentResults =
                DatabaseHelper.GetStudentResults(studentId.Value);

            ViewBag.Student = student;

            if (studentResults.Count > 0)
            {
                int totalMarks =
                    studentResults.Sum(r => r.Marks);

                int totalMaximum =
                    studentResults.Sum(r => r.MaxMarks);

                double percentage =
                    totalMaximum > 0
                    ? (double)totalMarks / totalMaximum * 100
                    : 0;

                int backlogs = studentResults.Count(r => r.Marks < r.PassingMarks);
                bool passed = backlogs == 0;
                double cgpa = Math.Round(percentage / 10.0, 2);

                ViewBag.TotalMarks = totalMarks;
                ViewBag.TotalMaximum = totalMaximum;
                ViewBag.Percentage = percentage;
                ViewBag.Grade = CalculateGrade(percentage);
                ViewBag.ResultStatus = passed ? "PASS" : "FAIL";
                ViewBag.Backlogs = backlogs;
                ViewBag.CGPA = cgpa;
            }

            return View(studentResults);
        }

        // GRADE
        // =========================

        private string CalculateGrade(double percentage)
        {
            if (percentage >= 90)
                return "A+";

            if (percentage >= 80)
                return "A";

            if (percentage >= 70)
                return "B+";

            if (percentage >= 60)
                return "B";

            if (percentage >= 50)
                return "C";

            if (percentage >= 40)
                return "D";

            return "F";
        }
    }
}