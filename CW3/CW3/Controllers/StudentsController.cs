using System;
using System.Linq;
using CW3.DAL;
using CW3.Models;
using Microsoft.AspNetCore.Mvc;

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
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }
            else if (id == 3)
            {
                return Ok("Andrzejewski");
            }

            return NotFound("Nie znaleziono studenta");
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


    }
}