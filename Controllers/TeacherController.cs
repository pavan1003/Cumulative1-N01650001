using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cumulative1.Models;

namespace Cumulative1.Controllers
{
    public class TeacherController : Controller
    {
        /// <summary>
        /// Displays the index view.
        /// </summary>
        /// <returns>The index view.</returns>
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays a list of teachers optionally filtered by search key.
        /// </summary>
        /// <param name="SearchKey">The search key to filter teachers.</param>
        /// <returns>The view displaying the list of teachers.</returns>
        /// <example>
        /// GET: /Teacher/List
        /// </example>
        public ActionResult List(string SearchKey = null)
        {
            TeacherDataController controller = new TeacherDataController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey);
            return View(Teachers);
        }

        /// <summary>
        /// Displays details of a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to display.</param>
        /// <returns>The view displaying the details of the teacher.</returns>
        /// <example>
        /// GET: /Teacher/Show/{id}
        /// </example>
        public ActionResult Show(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);

            return View(NewTeacher);
        }

        /// <summary>
        /// Displays a form to add a new teacher.
        /// </summary>
        /// <returns>The view displaying a form to add a new teacher.</returns>
        /// <example>
        /// GET : /Teacher/New
        /// </example>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Creates a new teacher with the provided information.
        /// </summary>
        /// <param name="TeacherFname">The first name of the teacher.</param>
        /// <param name="TeacherLname">The last name of the teacher.</param>
        /// <param name="EmployeeNumber">The employee number of the teacher.</param>
        /// <param name="HireDate">The hire date of the teacher.</param>
        /// <param name="Salary">The salary of the teacher.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation.
        /// Returns a 200 OK response if the teacher is added successfully.
        /// Returns a 400 Bad Request response if the provided information is missing or incorrect.
        /// </returns>
        /// /// <example>
        /// Example of POST request body
        /// POST /Teacher/Create
        /// {
        ///     "TeacherFname": "John",
        ///     "TeacherLname": "Doe",
        ///     "EmployeeNumber": "E12345",
        ///     "HireDate": "2024-04-06T00:00:00",
        ///     "Salary": 50000
        /// }
        /// </example>
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal? Salary)
        {
            // Check for missing information
            if (string.IsNullOrEmpty(TeacherFname) || string.IsNullOrEmpty(TeacherLname) ||
                string.IsNullOrEmpty(EmployeeNumber) || HireDate == null || HireDate > DateTime.Now || Salary == null || Salary < 0)
            {
                // Return a 400 Bad Request response with an error message
                Debug.WriteLine("Missing or incorrect information when adding a teacher");
                Trace.WriteLine("Missing or incorrect information when adding a teacher");
                Response.StatusCode = 400;
                return Content("Missing or incorrect information when adding a teacher", "text/plain");
            }
            Teacher NewTeacher = new Teacher();
            NewTeacher.TeacherFname = TeacherFname;
            NewTeacher.TeacherLname = TeacherLname;
            NewTeacher.EmployeeNumber = EmployeeNumber;
            NewTeacher.HireDate = HireDate;
            NewTeacher.Salary = Salary ?? 0;

            TeacherDataController controller = new TeacherDataController();
            controller.AddTeacher(NewTeacher);

            // Return a 200 OK response with a success message
            Response.StatusCode = 200;
            return Content("Teacher added successfully", "text/plain");
        }

        /// <summary>
        /// Displays a confirmation page to delete a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>The view displaying a confirmation page to delete the teacher.</returns>
        /// <example>
        /// GET : /Teacher/DeleteConfirm/{id}
        /// </example>
        public ActionResult DeleteConfirm(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);


            return View(NewTeacher);
        }

        /// <summary>
        /// Deletes a specific teacher from the system.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        /// <example>
        /// Example of POST request:
        /// POST /Teacher/Delete/123
        /// </example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            controller.DeleteTeacher(id);

            // Return a 200 OK response
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
