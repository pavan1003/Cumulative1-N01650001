using Cumulative1.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Cumulative1.Controllers
{
    public class ClassDataController : ApiController
    {
        // Database context class for accessing the MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Returns a list of classes from the system filtered by an optional search key.
        /// </summary>
        /// <param name="SearchKey">Optional search key to filter classes by class name or class code.</param>
        /// <returns>A list of class objects. Each class object has the following properties:
        /// - ClassId: An integer representing the unique identifier of the class.
        /// - ClassCode: A string representing the code of the class.
        /// - TeacherId: An integer representing the ID of the teacher associated with the class.
        /// - StartDate: A DateTime object representing the start date of the class.
        /// - FinishDate: A DateTime object representing the finish date of the class.
        /// - ClassName: A string representing the name of the class.</returns>
        /// <example>
        /// Example of GET request:
        /// GET /api/ClassData/ListClasses?SearchKey=Math
        /// </example>
        [HttpGet]
        [Route("api/ClassData/ListClasses/{SearchKey?}")]
        public IEnumerable<Class> ListClasses(string SearchKey = null)
        {
            // Establish a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query with optional search key
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Classes WHERE LOWER(ClassName) LIKE LOWER(@key) OR LOWER(ClassCode) LIKE LOWER(@key)";
            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Create a list to store class objects
            List<Class> Classes = new List<Class>();

            // Iterate through each row in the result set
            while (ResultSet.Read())
            {
                // Retrieve column information
                int ClassId = Convert.ToInt32(ResultSet["ClassId"]);
                string ClassCode = ResultSet["ClassCode"].ToString();
                int TeacherId = Convert.ToInt32(ResultSet["TeacherId"]);
                DateTime StartDate = Convert.ToDateTime(ResultSet["StartDate"]);
                DateTime FinishDate = Convert.ToDateTime(ResultSet["FinishDate"]);
                string ClassName = ResultSet["ClassName"].ToString();

                // Create a new Class object and populate its properties
                Class NewClass = new Class
                {
                    ClassId = ClassId,
                    ClassCode = ClassCode,
                    TeacherId = TeacherId,
                    StartDate = StartDate,
                    FinishDate = FinishDate,
                    ClassName = ClassName
                };

                // Add the class to the list
                Classes.Add(NewClass);
            }

            // Close the database connection
            Conn.Close();

            // Return the list of classes
            return Classes;
        }

        /// <summary>
        /// Returns an individual class from the database by specifying the primary key classid.
        /// </summary>
        /// <param name="id">The class's ID in the database.</param>
        /// <returns>A class object. The class object has the following properties:
        /// - ClassId: An integer representing the unique identifier of the class.
        /// - ClassCode: A string representing the code of the class.
        /// - TeacherId: An integer representing the ID of the teacher associated with the class.
        /// - StartDate: A DateTime object representing the start date of the class.
        /// - FinishDate: A DateTime object representing the finish date of the class.
        /// - ClassName: A string representing the name of the class.</returns>
        /// <example>
        /// Example of GET request:
        /// GET /api/ClassData/FindClass/12
        /// </example>
        [HttpGet]
        [Route("api/ClassData/FindClass/{id}")]
        public Class FindClass(int id)
        {
            // Create a new Class object
            Class NewClass = new Class();

            // Establish a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query to retrieve class information
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Classes WHERE ClassId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Populate the class object with information from the result set
            while (ResultSet.Read())
            {
                NewClass.ClassId = Convert.ToInt32(ResultSet["ClassId"]);
                NewClass.ClassCode = ResultSet["ClassCode"].ToString();
                NewClass.TeacherId = Convert.ToInt32(ResultSet["TeacherId"]);
                NewClass.StartDate = Convert.ToDateTime(ResultSet["StartDate"]);
                NewClass.FinishDate = Convert.ToDateTime(ResultSet["FinishDate"]);
                NewClass.ClassName = ResultSet["ClassName"].ToString();
            }

            // Close the result set
            ResultSet.Close();

            // Close the database connection
            Conn.Close();

            // Return the class object
            return NewClass;
        }
    }
}
