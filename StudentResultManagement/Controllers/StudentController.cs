using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{
    public class StudentController : Controller
    {
        private bool IsAdmin()
        {
            return Session["AdminID"] != null;
        }

        private bool IsStudent()
        {
            return Session["StudentID"] != null;
        }

        // =========================
        // STUDENT LOGIN
        // =========================

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(StudentLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                Student student = DatabaseHelper.GetStudent(
                    model.EnrollmentNo != null ? model.EnrollmentNo.Trim() : "",
                    model.Password != null ? model.Password.Trim() : ""
                );

                if (student != null)
                {
                    Session["StudentID"] = student.StudentID;
                    Session["StudentName"] = student.Name;
                    Session["StudentEnrollment"] = student.EnrollmentNo;

                    return RedirectToAction("Dashboard");
                }

                ViewBag.Error = "Invalid enrollment number or password.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database notice: " + ex.Message;
            }

            return View(model);
        }

        // =========================
        // STUDENT DASHBOARD
        // =========================

        public ActionResult Dashboard()
        {
            if (!IsStudent())
            {
                return RedirectToAction("Login");
            }

            int studentId = (int)Session["StudentID"];

            Student student = DatabaseHelper.GetStudentById(studentId);

            if (student == null)
            {
                Session.Clear();
                return RedirectToAction("Login");
            }

            bool isPublished = HttpContext.Application["ResultsPublished"] == null
                || (bool)HttpContext.Application["ResultsPublished"];

            ViewBag.IsResultPublished = isPublished;

            if (isPublished)
            {
                var studentResults = DatabaseHelper.GetStudentResults(studentId);
                if (studentResults != null && studentResults.Count > 0)
                {
                    int totalMarks = studentResults.Sum(r => r.Marks);
                    int totalMaximum = studentResults.Sum(r => r.MaxMarks);
                    double percentage = totalMaximum > 0 ? (double)totalMarks / totalMaximum * 100 : 0;
                    int backlogs = studentResults.Count(r => r.Marks < r.PassingMarks);
                    bool passed = backlogs == 0;
                    double cgpa = Math.Round(percentage / 10.0, 2);

                    ViewBag.HasResults = true;
                    ViewBag.TotalSubjects = studentResults.Count;
                    ViewBag.TotalMarks = totalMarks;
                    ViewBag.TotalMaximum = totalMaximum;
                    ViewBag.Percentage = Math.Round(percentage, 2);
                    ViewBag.Grade = CalculateGrade(percentage);
                    ViewBag.ResultStatus = passed ? "PASS" : "FAIL";
                    ViewBag.Backlogs = backlogs;
                    ViewBag.CGPA = cgpa;
                }
                else
                {
                    ViewBag.HasResults = false;
                }
            }

            return View(student);
        }

        // =========================
        // MY RESULT
        // =========================

        public ActionResult MyResult()
        {
            if (!IsStudent())
            {
                return RedirectToAction("Login");
            }

            int studentId = (int)Session["StudentID"];

            Student student = DatabaseHelper.GetStudentById(studentId);

            if (student == null)
            {
                Session.Clear();
                return RedirectToAction("Login");
            }

            bool isPublished = HttpContext.Application["ResultsPublished"] == null
                || (bool)HttpContext.Application["ResultsPublished"];

            if (!isPublished)
            {
                ViewBag.ResultsNotPublished = true;
                ViewBag.Student = student;
                return View(new List<Result>());
            }

            var studentResults =
                DatabaseHelper.GetStudentResults(studentId);

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

        // MY PROFILE
        public new ActionResult Profile()
        {
            if (!IsStudent())
                return RedirectToAction("Login", "Student");

            int studentId = (int)Session["StudentID"];

            Student student = DatabaseHelper.GetStudentById(studentId);

            if (student == null)
            {
                Session.Clear();
                return RedirectToAction("Login", "Student");
            }

            return View(student);
        }

        // STUDENT LOGOUT
        // =========================

        public ActionResult Logout()
        {
            Session.Remove("StudentID");
            Session.Remove("StudentName");
            Session.Remove("StudentEnrollment");

            return RedirectToAction("Login");
        }

        // =========================
        // ADMIN STUDENT CRUD
        // =========================

        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            return View(DatabaseHelper.GetAllStudents());
        }

        // =========================
        // CREATE STUDENT
        // =========================

        public ActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Student student)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            if (ModelState.IsValid)
            {
                var students =
                    DatabaseHelper.GetAllStudents();

                bool enrollmentExists =
                    students.Any(
                        s => s.EnrollmentNo == student.EnrollmentNo);

                if (enrollmentExists)
                {
                    ModelState.AddModelError(
                        "EnrollmentNo",
                        "This enrollment number already exists.");

                    return View(student);
                }

                DatabaseHelper.AddStudent(student);

                TempData["Success"] =
                    "Student added successfully.";

                return RedirectToAction("Index");
            }

            return View(student);
        }

        // =========================
        // EDIT STUDENT
        // =========================

        public ActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Student student =
                DatabaseHelper.GetStudentById(id);

            if (student == null)
                return HttpNotFound();

            return View(student);
        }

        [HttpPost]
        public ActionResult Edit(Student student)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            if (ModelState.IsValid)
            {
                Student existing =
                    DatabaseHelper.GetStudentById(
                        student.StudentID);

                if (existing == null)
                    return HttpNotFound();

                var students =
                    DatabaseHelper.GetAllStudents();

                bool enrollmentExists =
                    students.Any(
                        s => s.EnrollmentNo == student.EnrollmentNo
                             && s.StudentID != student.StudentID);

                if (enrollmentExists)
                {
                    ModelState.AddModelError(
                        "EnrollmentNo",
                        "This enrollment number already exists.");

                    return View(student);
                }

                DatabaseHelper.UpdateStudent(student);

                TempData["Success"] =
                    "Student updated successfully.";

                return RedirectToAction("Index");
            }

            return View(student);
        }

        // DELETE STUDENT
        // =========================

        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Student student =
                DatabaseHelper.GetStudentById(id);

            if (student == null)
                return HttpNotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Student student =
                DatabaseHelper.GetStudentById(id);

            if (student != null)
            {
                DatabaseHelper.DeleteStudent(id);

                TempData["Success"] =
                    "Student deleted successfully.";
            }

            return RedirectToAction("Index");
        }

        // GRADE CALCULATION
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