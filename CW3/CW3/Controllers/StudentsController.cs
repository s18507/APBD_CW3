using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CW3.DAL;
using CW3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;

namespace CW3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;


        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }


        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            
            using(var connection = new SqlConnection("Data Source=db-mssql;" +
                                                     "Initial Catalog=s18507;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = connection;
                com.CommandText = "select indexNumber,FirstName,LastName,BirthDate,Studies.name,Enrollment.semester from students inner join enrollment on students.idEnrollment=Enrollment.idEnrollment inner join studies on Enrollment.idStudy=studies.idstudy";

                connection.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.StudiesName = dr["name"].ToString();
                    st.Semester = dr["semester"].ToString();

                    list.Add(st);
                }

            }
            
            return Ok(list);
        }

        [HttpGet("{id}")]

        public IActionResult GetStudent(int id)
        {
            var list = new List<Enrollment>();

            using (var con = new SqlConnection("Data Source=db-mssql;" +
                                               "Initial Catalog=s18507;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = $"select indexNumber,FirstName,LastName,BirthDate,Studies.name,Enrollment.semester from students inner join enrollment on students.idEnrollment=Enrollment.idEnrollment inner join studies on Enrollment.idStudy=studies.idstudy where indexnumber=('s0'+cast(@id as varchar))";
                com.Parameters.AddWithValue("id", id);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment = new Enrollment();
                    enrollment.Semester = Convert.ToInt32(dr["Semester"]);
                    enrollment.IdStudy = Convert.ToInt32(dr["IdStudy"]);
                    enrollment.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"]);
                    enrollment.StartDate = dr["StartDate"].ToString();
                    list.Add(enrollment);
                    
                }

                return Ok(list);
            }

           
        }


        [HttpPost]
        //dodanie elementow
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            if (id <= 0 | id >= 4)
                return NotFound();

            _dbService.DeleteStudents(id);
            return Ok("Usuwanie ukonczone");
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(Student stud)
        {
            return Ok($"Aktualizacja dokonczona");

        }

        [HttpGet]
        public IActionResult Get(){
            SqlConnection connection = new SqlConnection("Data Source=db-mssql;" +
                                                         "Initial Catalog=s18507;Integrated Security=True");
            SqlCommand command;
            return Ok();
        }

        
    }
    
    
}