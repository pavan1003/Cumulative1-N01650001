using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cumulative1.Models;

namespace Cumulative1.Controllers
{
    public class StudentController : Controller
    {
        /// <summary>
        /// Displays the index view.
        /// </summary>
        /// <returns>The index view.</returns>
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays a list of students optionally filtered by search key.
        /// </summary>
        /// <param name="SearchKey">The search key to filter students.</param>
        /// <returns>The view displaying the list of students.</returns>
        // GET: /Student/List
        public ActionResult List(string SearchKey = null)
        {
            StudentDataController controller = new StudentDataController();
            IEnumerable<Student> Students = controller.ListStudents(SearchKey);
            return View(Students);
        }

        /// <summary>
        /// Displays details of a specific student.
        /// </summary>
        /// <param name="id">The ID of the student to display.</param>
        /// <returns>The view displaying the details of the student.</returns>
        // GET: /Student/Show/{id}
        public ActionResult Show(int id)
        {
            StudentDataController controller = new StudentDataController();
            Student NewStudent = controller.FindStudent(id);

            return View(NewStudent);
        }

    }
}
