using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cumulative1.Models;
using System.Web.Http;

namespace Cumulative1.Controllers
{
    public class StudentDataController : ApiController
    {
        // Database context class for accessing the MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Returns a list of students from the system filtered by an optional search key.
        /// </summary>
        /// <param name="SearchKey">Optional search key to filter students by first name, last name, or full name.</param>
        /// <returns>A list of student objects. Each student object has the following properties:
        /// - StudentId: An integer representing the unique identifier of the student.
        /// - StudentFname: A string representing the first name of the student.
        /// - StudentLname: A string representing the last name of the student.
        /// - StudentNumber: A string representing the student number.
        /// - EnrolDate: A DateTime object representing the enrollment date of the student.</returns>
        /// <example>
        /// Example of GET request:
        /// GET /api/StudentData/ListStudents?SearchKey=Pavan
        /// </example>
        [HttpGet]
        [Route("api/StudentData/ListStudents/{SearchKey?}")]
        public IEnumerable<Student> ListStudents(string SearchKey = null)
        {
            // Establish a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query with optional search key
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Students WHERE LOWER(studentfname) LIKE LOWER(@key) OR LOWER(studentlname) LIKE LOWER(@key) OR LOWER(CONCAT(studentfname, ' ', studentlname)) LIKE LOWER(@key)";
            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Create a list to store student objects
            List<Student> Students = new List<Student>();

            // Iterate through each row in the result set
            while (ResultSet.Read())
            {
                // Retrieve column information
                int StudentId = Convert.ToInt32(ResultSet["studentid"]);
                string StudentFname = ResultSet["studentfname"].ToString();
                string StudentLname = ResultSet["studentlname"].ToString();
                string StudentNumber = ResultSet["studentnumber"].ToString();
                DateTime EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]);

                // Create a new Student object and populate its properties
                Student NewStudent = new Student
                {
                    StudentId = StudentId,
                    StudentFname = StudentFname,
                    StudentLname = StudentLname,
                    StudentNumber = StudentNumber,
                    EnrolDate = EnrolDate
                };

                // Add the student to the list
                Students.Add(NewStudent);
            }

            // Close the database connection
            Conn.Close();

            // Return the list of students
            return Students;
        }

        /// <summary>
        /// Returns an individual student from the database by specifying the primary key studentid.
        /// </summary>
        /// <param name="id">The student's ID in the database.</param>
        /// <returns>A student object. The student object has the following properties:
        /// - StudentId: An integer representing the unique identifier of the student.
        /// - StudentFname: A string representing the first name of the student.
        /// - StudentLname: A string representing the last name of the student.
        /// - StudentNumber: A string representing the student number.
        /// - EnrolDate: A DateTime object representing the enrollment date of the student.</returns>
        /// <example>
        /// Example of GET request:
        /// GET /api/StudentData/FindStudent/12
        /// </example>
        [HttpGet]
        [Route("api/StudentData/FindStudent/{id}")]
        public Student FindStudent(int id)
        {
            // Create a new Student object
            Student NewStudent = new Student();

            // Establish a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query to retrieve student information
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Students WHERE studentid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            // Execute the query
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            // Populate the student object with information from the result set
            while (ResultSet.Read())
            {
                NewStudent.StudentId = Convert.ToInt32(ResultSet["studentid"]);
                NewStudent.StudentFname = ResultSet["studentfname"].ToString();
                NewStudent.StudentLname = ResultSet["studentlname"].ToString();
                NewStudent.StudentNumber = ResultSet["studentnumber"].ToString();
                NewStudent.EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]);
            }

            // Close the result set
            ResultSet.Close();

            // Close the database connection
            Conn.Close();

            // Return the student object
            return NewStudent;
        }
    }
}