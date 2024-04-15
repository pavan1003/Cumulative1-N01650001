using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cumulative1.Models;
using Mysqlx.Datatypes;

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
        /// Displays a form to add a new teacher.
        /// </summary>
        /// <returns>The view displaying a form to add a new teacher using AJAX request.</returns>
        /// <example>
        /// GET : /Teacher/Ajax_New
        /// </example>
        public ActionResult Ajax_New()
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
        /// <example>
        /// Example of POST request body
        /// POST /Teacher/Create
        /// {
        ///     "TeacherFname": "Pavan",
        ///     "TeacherLname": "Mistry",
        ///     "EmployeeNumber": "T123",
        ///     "HireDate": "2024-04-06",
        ///     "Salary": 50
        /// }
        /// </example>
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal? Salary)
        {
            // Check for missing information
            if (string.IsNullOrEmpty(TeacherFname) || string.IsNullOrEmpty(TeacherLname) ||
                string.IsNullOrEmpty(EmployeeNumber) || HireDate == null || HireDate > DateTime.Now || Salary == null || Salary < 0)
            {
                // Return the view with an error message
                ViewBag.Message = "Missing or incorrect information when adding a teacher";
                return View("New");
            }
            Teacher NewTeacher = new Teacher();
            NewTeacher.TeacherFname = TeacherFname;
            NewTeacher.TeacherLname = TeacherLname;
            NewTeacher.EmployeeNumber = EmployeeNumber;
            NewTeacher.HireDate = HireDate;
            NewTeacher.Salary = Salary ?? 0;

            TeacherDataController controller = new TeacherDataController();
            controller.AddTeacher(NewTeacher);

            // Return the view to list page
            return RedirectToAction("List");
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
        /// Displays a confirmation page to delete a specific teacher using AJAX.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>The view displaying a confirmation page to delete the teacher using AJAX request.</returns>
        /// <example>
        /// GET : /Teacher/Ajax_DeleteConfirm/{id}
        /// </example>
        public ActionResult Ajax_DeleteConfirm(int id)
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

        /// <summary>
        /// Routes to a dynamically generated "Teacher Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Teacher</param>
        /// <returns>A dynamic "Update Teacher" webpage which provides the current information of the teacher and asks the user for new information as part of a form.</returns>
        /// <example>
        /// Example of GET request:
        /// GET /Teacher/Update/123
        /// </example>
        public ActionResult Update(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher SelectedTeacher = controller.FindTeacher(id);

            return View(SelectedTeacher);
        }

        /// <summary>
        /// Routes to a dynamically generated "Teacher Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Teacher</param>
        /// <returns>A dynamic "Update Teacher" webpage which provides the current information of the teacher and asks the user for new information as part of a Ajax request.</returns>
        /// <example>
        /// Example of GET request:
        /// GET /Teacher/Ajax_Update/123
        /// </example>
        public ActionResult Ajax_Update(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher SelectedTeacher = controller.FindTeacher(id);

            return View(SelectedTeacher);
        }

        /// <summary>
        /// Updates the information of a specific teacher in the system.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="TeacherFname">The updated first name of the teacher.</param>
        /// <param name="TeacherLname">The updated last name of the teacher.</param>
        /// <param name="EmployeeNumber">The updated employee number of the teacher.</param>
        /// <param name="HireDate">The updated hire date of the teacher.</param>
        /// <param name="Salary">The updated salary of the teacher.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation.
        /// Returns a redirect to the details page of the updated teacher if successful.
        /// Returns the "Update Teacher" view with an error message if the provided information is missing or incorrect.
        /// </returns>
        /// <example>
        /// Example of POST request body:
        /// POST /Teacher/Update/{id}
        /// {
        ///     "TeacherFname": "UpdatedFirstName",
        ///     "TeacherLname": "UpdatedLastName",
        ///     "EmployeeNumber": "UpdatedEmployeeNumber",
        ///     "HireDate": "2024-04-15",
        ///     "Salary": 100
        /// }
        /// </example>
        [HttpPost]
        public ActionResult Update(int id, string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal? Salary)
        {
            TeacherDataController controller = new TeacherDataController();
            if (string.IsNullOrEmpty(TeacherFname) || string.IsNullOrEmpty(TeacherLname) ||
    string.IsNullOrEmpty(EmployeeNumber) || HireDate == null || HireDate > DateTime.Now || Salary == null || Salary < 0)
            {
                // Return the view with an error message
                ViewBag.Message = "Missing or incorrect information when updating a teacher";
                Teacher SelectedTeacher = controller.FindTeacher(id);
                return View("Update", SelectedTeacher);
            }
            Teacher TeacherInfo = new Teacher();
            TeacherInfo.TeacherFname = TeacherFname;
            TeacherInfo.TeacherLname = TeacherLname;
            TeacherInfo.EmployeeNumber = EmployeeNumber;
            TeacherInfo.HireDate = HireDate;
            TeacherInfo.Salary = Salary ?? 0;

            controller.UpdateTeacher(id, TeacherInfo);

            return RedirectToAction("Show/" + id);
        }
    }
}
