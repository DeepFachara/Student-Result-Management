using System.Web.Mvc;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{
    public class SubjectController : Controller
    {
        private bool IsAdmin()
        {
            return Session["AdminID"] != null;
        }

        // =========================
        // SUBJECT LIST
        // =========================

        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            return View(DatabaseHelper.GetAllSubjects());
        }

        // =========================
        // CREATE SUBJECT
        // =========================

        public ActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Subject subject)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            if (ModelState.IsValid)
            {
                DatabaseHelper.AddSubject(subject);

                TempData["Success"] =
                    "Subject added successfully.";

                return RedirectToAction("Index");
            }

            return View(subject);
        }

        // =========================
        // EDIT SUBJECT
        // =========================

        public ActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Subject subject =
                DatabaseHelper.GetSubjectById(id);

            if (subject == null)
                return HttpNotFound();

            return View(subject);
        }

        [HttpPost]
        public ActionResult Edit(Subject subject)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            if (ModelState.IsValid)
            {
                Subject existing =
                    DatabaseHelper.GetSubjectById(subject.SubjectID);

                if (existing == null)
                    return HttpNotFound();

                DatabaseHelper.UpdateSubject(subject);

                TempData["Success"] =
                    "Subject updated successfully.";

                return RedirectToAction("Index");
            }

            return View(subject);
        }

        // =========================
        // DELETE SUBJECT
        // =========================

        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            Subject subject =
                DatabaseHelper.GetSubjectById(id);

            if (subject == null)
                return HttpNotFound();

            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Admin");

            DatabaseHelper.DeleteSubject(id);

            TempData["Success"] =
                "Subject deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}