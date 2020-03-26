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
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]

        public IActionResult GetStudent(int id)
        {
          using(var connection = new SqlConnection("Data Source=db-mssql;" +
                                                   "Initial Catalog=s18507;Integrated Security=True"))
          using (var com = new SqlCommand())
          {
              com.Connection = connection;
              com.CommandText = "Select * from Student";
              
              connection.Open();
              var dr = com.ExecuteReader();
              var student = new List<Student>();
              while (dr.Read())
              {
                  student.Add(new Student
                  {
                      IndexNumber = dr["IndexNumber"].ToString(),
                      FirstName = dr["FirstName"].ToString(),
                      LastName = dr["LastName"].ToString(),
                      BirthDate = dr["BirthDate"].ToString(),
                      IdEnrollment = IntegerType.FromObject(dr["IdEnrollment"])
                  });
              }

          }
          return Ok();
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