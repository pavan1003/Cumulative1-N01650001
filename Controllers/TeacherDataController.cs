using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cumulative1.Models;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Cors;
using Mysqlx.Datatypes;

namespace Cumulative1.Controllers
{
    public class TeacherDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Returns a list of teachers in the system filtered by an optional search key.
        /// </summary>
        /// <param name="SearchKey">Optional search key to filter teachers by first name, last name, full name, hire date, or salary.</param>
        /// <returns>
        /// A list of teacher objects.
        /// Each teacher object contains the following properties:
        /// - TeacherId (int): The unique identifier of the teacher.
        /// - TeacherFname (string): The first name of the teacher.
        /// - TeacherLname (string): The last name of the teacher.
        /// - EmployeeNumber (string): The employee number of the teacher.
        /// - HireDate (DateTime): The date when the teacher was hired.
        /// - Salary (decimal): The salary of the teacher.
        /// </returns>
        /// <example>
        /// Example of GET request:
        /// GET api/TeacherData/ListTeachers?SearchKey=Pavan
        /// GET api/TeacherData/ListTeachers?SearchKey=04-05
        /// GET api/TeacherData/ListTeachers?SearchKey=66
        /// </example>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            // Create a connection to the database
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            // Prepare SQL query with optional search key
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE LOWER(teacherfname) LIKE LOWER(@Key) OR LOWER(teacherlname) LIKE LOWER(@Key) OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) or hiredate Like @Key or DATE_FORMAT(hiredate, '%d-%m-%Y') Like @Key or salary LIKE @Key ";
            cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");

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
        /// <returns>
        /// A teacher object with associated classes.
        /// The teacher object contains the following properties:
        /// - TeacherId (int): The unique identifier of the teacher.
        /// - TeacherFname (string): The first name of the teacher.
        /// - TeacherLname (string): The last name of the teacher.
        /// - EmployeeNumber (string): The employee number of the teacher.
        /// - HireDate (DateTime): The date when the teacher was hired.
        /// - Salary (decimal): The salary of the teacher.
        /// - Classes (List<Class>): A list of class objects associated with the teacher.
        /// </returns>
        /// <example>
        /// Example of GET request:
        /// GET api/TeacherData/FindTeacher/12
        /// </example>
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

        /// <summary>
        /// Adds a teacher to the MySQL Database. Non-Deterministic.
        /// </summary>
        /// <param name="NewTeacher">An object with fields that map to the columns of the teacher's table.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation.
        /// Returns a 400 Bad Request response if the provided information is missing or incorrect.
        /// Returns a 200 OK response if the teacher is added successfully.
        /// </returns>
        /// <example>
        /// POST api/TeacherData/AddTeacher 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"TeacherFname":"Pavan",
        ///	"TeacherLname":"Mistry",
        ///	"EmployeeNumber":"T123",
        ///	"HireDate":"05-04-2024"
        ///	"Salary": 66
        /// }
        /// </example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        [Route("api/TeacherData/AddTeacher")]
        public IHttpActionResult AddTeacher([FromBody] Teacher NewTeacher)
        {

            if (string.IsNullOrEmpty(NewTeacher.TeacherFname) || string.IsNullOrEmpty(NewTeacher.TeacherLname) ||
                string.IsNullOrEmpty(NewTeacher.EmployeeNumber) || NewTeacher.HireDate == null || NewTeacher.HireDate > DateTime.Now || NewTeacher.Salary < 0)
            {
                // Return a 400 Bad Request response with an error message
                return BadRequest("Invalid data provided for adding the teacher.");
            }

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Debug.WriteLine("id: " + id);
            //Debug.WriteLine("TeacherFname: " + TeacherInfo.TeacherFname);
            //Debug.WriteLine("TeacherLname: " + TeacherInfo.TeacherLname);
            //Debug.WriteLine("EmployeeNumber: " + TeacherInfo.EmployeeNumber);
            //Debug.WriteLine("HireDate: " + TeacherInfo.HireDate);
            //Debug.WriteLine("Salary: " + TeacherInfo.Salary);

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) values (@TeacherFname,@TeacherLname,@Employeenumber, @HireDate, @Salary)";
            cmd.Parameters.AddWithValue("@TeacherFname", NewTeacher.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", NewTeacher.TeacherLname);
            cmd.Parameters.AddWithValue("@EmployeeNumber", NewTeacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", NewTeacher.HireDate);
            cmd.Parameters.AddWithValue("@Salary", NewTeacher.Salary);

            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();
            return Ok("Teacher added successfully");
        }

        /// <summary>
        /// Deletes a teacher from the connected MySQL Database if the ID of that teacher exists. It Also maintains relational integrity.
        /// </summary>
        /// <param name="id">The ID of the teacher.</param>
        /// <returns>
        /// A response indicating the success of the operation..
        /// Returns a 200 OK response if the teacher is updated successfully.
        /// </returns>
        /// <example>POST /api/TeacherData/DeleteTeacher/3</example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        [Route("api/TeacherData/DeleteTeacher/{id}")]
        public IHttpActionResult DeleteTeacher(int id)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL QUERY
            // Delete from teachers table where teacherid = @id
            cmd.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            // Delete from classes table where teacherid = @id
            cmd.CommandText = "UPDATE classes SET teacherid=null WHERE teacherid=@id";

            cmd.ExecuteNonQuery();

            cmd.ExecuteNonQuery();

            Conn.Close();
            return Ok("Teacher Deleted successfully");
        }

        /// <summary>
        /// Updates the information of a specific teacher in the MySQL Database.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="TeacherInfo">An object containing the updated information of the teacher.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation.
        /// Returns a 400 Bad Request response if the provided information is missing or incorrect.
        /// Returns a 200 OK response if the teacher is updated successfully.
        /// </returns>
        /// <example>
        /// Example curl request: curl -d @testdata.json -H "Content-Type: application/json" http://localhost63364/api/TeacherData/UpdateTeacher/10
        /// Example of POST request body:
        /// POST /api/TeacherData/UpdateTeacher/{id}
        /// {
        ///     "TeacherFname": "UpdatedFirstName",
        ///     "TeacherLname": "UpdatedLastName",
        ///     "EmployeeNumber": "UpdatedEmployeeNumber",
        ///     "HireDate": "2024-04-15",
        ///     "Salary": 100
        /// }
        /// </example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        [Route("api/TeacherData/UpdateTeacher/{id}")]
        public IHttpActionResult UpdateTeacher(int id, [FromBody] Teacher TeacherInfo)
        {
            if (string.IsNullOrEmpty(TeacherInfo.TeacherFname) || string.IsNullOrEmpty(TeacherInfo.TeacherLname) ||
                string.IsNullOrEmpty(TeacherInfo.EmployeeNumber) || TeacherInfo.HireDate == null || TeacherInfo.HireDate > DateTime.Now || TeacherInfo.Salary < 0)
            {
                // Return a 400 Bad Request response with an error message
                return BadRequest("Invalid data provided for updating the teacher.");
            }

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Debug.WriteLine("id: " + id);
            //Debug.WriteLine("TeacherFname: " + TeacherInfo.TeacherFname);
            //Debug.WriteLine("TeacherLname: " + TeacherInfo.TeacherLname);
            //Debug.WriteLine("EmployeeNumber: " + TeacherInfo.EmployeeNumber);
            //Debug.WriteLine("HireDate: " + TeacherInfo.HireDate);
            //Debug.WriteLine("Salary: " + TeacherInfo.Salary);


            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "UPDATE teachers SET teacherfname=@TeacherFname, teacherlname=@TeacherLname, employeenumber=@EmployeeNumber, hiredate=@HireDate, salary=@Salary  where teacherid=@TeacherId";
            cmd.Parameters.AddWithValue("@TeacherFname", TeacherInfo.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", TeacherInfo.TeacherLname);
            cmd.Parameters.AddWithValue("@EmployeeNumber", TeacherInfo.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", TeacherInfo.HireDate);
            cmd.Parameters.AddWithValue("@Salary", TeacherInfo.Salary);
            cmd.Parameters.AddWithValue("@TeacherId", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();

            return Ok("Teacher updated successfully");
        }

    }
}
