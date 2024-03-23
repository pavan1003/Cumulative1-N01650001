using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cumulative1.Models;
using System.Diagnostics;

namespace Cumulative1.Controllers
{
    public class TeacherDataController : Controller
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Returns a list of teachers in the system filtered by an optional search key.
        /// </summary>
        /// <param name="SearchKey">Optional search key to filter teachers by first name, last name, full name, hiredate or salary.</param>
        /// <returns>A list of teacher objects.</returns>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}/{HireDateStartSearchKey?}/{HireDateEndSearchKey?}/{SalarySearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            // Create a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query with optional search key
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE LOWER(teacherfname) LIKE LOWER(@Key) OR LOWER(teacherlname) LIKE LOWER(@Key) OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) or hiredate Like @Key or DATE_FORMAT(hiredate, '%d-%m-%Y') Like @Key or salary LIKE @Key ";
            cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");

            foreach (MySqlParameter parameter in cmd.Parameters)
            {
                Trace.WriteLine("Parameter Name: " + parameter.ParameterName);
                Trace.WriteLine("Parameter Value: " + parameter.Value);
            }

            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Create a list to hold teacher objects
            List<Teacher> Teachers = new List<Teacher>();

            // Loop through each row in the result set
            while (ResultSet.Read())
            {
                // Retrieve column information
                int TeacherId = Convert.ToInt32(ResultSet["teacherId"]);
                string TeacherFname = ResultSet["teacherFname"].ToString();
                string TeacherLname = ResultSet["teacherLname"].ToString();
                string EmployeeNumber = ResultSet["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                // Create a new Teacher object and populate its properties
                Teacher NewTeacher = new Teacher
                {
                    TeacherId = TeacherId,
                    TeacherFname = TeacherFname,
                    TeacherLname = TeacherLname,
                    EmployeeNumber = EmployeeNumber,
                    HireDate = HireDate,
                    Salary = Salary
                };

                // Add the teacher to the list
                Teachers.Add(NewTeacher);
            }

            // Close the database connection
            Conn.Close();

            // Return the list of teachers
            return Teachers;
        }

        /// <summary>
        /// Finds a teacher in the system given an ID and retrieves associated classes.
        /// </summary>
        /// <param name="id">The teacher's primary key.</param>
        /// <returns>A teacher object with associated classes.</returns>
        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            // Create a new Teacher object
            Teacher NewTeacher = new Teacher();

            // Create a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query to retrieve teacher information
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Populate the teacher object with information from the result set
            while (ResultSet.Read())
            {
                NewTeacher.TeacherId = Convert.ToInt32(ResultSet["teacherId"]);
                NewTeacher.TeacherFname = ResultSet["teacherFname"].ToString();
                NewTeacher.TeacherLname = ResultSet["teacherLname"].ToString();
                NewTeacher.EmployeeNumber = ResultSet["employeenumber"].ToString();
                NewTeacher.HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                NewTeacher.Salary = Convert.ToDecimal(ResultSet["salary"]);
            }
            ResultSet.Close(); // Close the result set

            // Prepare SQL query to retrieve classes associated with the teacher
            MySqlCommand classCmd = Conn.CreateCommand();
            classCmd.CommandText = "SELECT * FROM Classes WHERE teacherid = @id";
            classCmd.Parameters.AddWithValue("@id", id);
            classCmd.Prepare();

            // Execute the query
            MySqlDataReader ClassResultSet = classCmd.ExecuteReader();

            // Create a list to hold class objects
            List<Class> Classes = new List<Class>();

            // Loop through each row in the class result set
            while (ClassResultSet.Read())
            {
                // Retrieve column information
                int ClassId = Convert.ToInt32(ClassResultSet["ClassId"]);
                string ClassCode = ClassResultSet["ClassCode"].ToString();
                string ClassName = ClassResultSet["ClassName"].ToString();

                // Create a new Class object and populate its properties
                //note: Id needed for a link to class page. 
                Class NewClass = new Class
                {
                    ClassId = ClassId,
                    ClassCode = ClassCode,
                    ClassName = ClassName
                };

                // Add the class to the list
                Classes.Add(NewClass);
            }

            // Add the list of classes to the teacher object
            NewTeacher.Classes = Classes;

            // Close the database connection
            Conn.Close();

            // Return the teacher object
            return NewTeacher;
        }

    }
}
