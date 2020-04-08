using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CW3.DAL;
using CW3.Models;
using CW3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;

namespace CW3.Controllers
{
    
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;

        public EnrollmentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }
        

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            if (student.IndexNumber == null || student.FirstName == null || student.LastName == null || student.BirthDate == null || student.StudiesName == null)
            {
                return BadRequest("Can't be null");  
            }
            else if (student.IndexNumber.Length < 1 || student.FirstName.Length < 1 || student.LastName.Length < 1 || student.BirthDate.Length < 1 || student.StudiesName.Length < 1)
            {
                return BadRequest();
            }
            
            return _dbService.CreateStudent(student);
            
        }
    
    
        [HttpPost("promotions")]
        public IActionResult Promotion(Promotion promotion)
        {
            
            return _dbService.Promotion(promotion);
            
        }
    }
}