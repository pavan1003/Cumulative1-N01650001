using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cumulative1.Models;

namespace Cumulative1.Controllers
{

    public class ClassController : Controller
    {
        /// <summary>
        /// Displays the index view.
        /// </summary>
        /// <returns>The index view.</returns>
        // GET: Class
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays a list of classes optionally filtered by search key.
        /// </summary>
        /// <param name="SearchKey">The search key to filter classes.</param>
        /// <returns>The view displaying the list of classes.</returns>
        // GET: /Class/List
        public ActionResult List(string SearchKey = null)
        {
            ClassDataController controller = new ClassDataController();
            IEnumerable<Class> Classes = controller.ListClasses(SearchKey);
            return View(Classes);
        }

        /// <summary>
        /// Displays details of a specific class.
        /// </summary>
        /// <param name="id">The ID of the class to display.</param>
        /// <returns>The view displaying the details of the class.</returns>
        // GET: /Class/Show/{id}
        public ActionResult Show(int id)
        {
            ClassDataController controller = new ClassDataController();
            Class NewClass = controller.FindClass(id);

            return View(NewClass);
        }

    }
}
