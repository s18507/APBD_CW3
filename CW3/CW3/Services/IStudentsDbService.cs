using System.Collections;
using CW3.Models;
using Microsoft.AspNetCore.Mvc;

namespace CW3.Services
{
    public interface IStudentsDbService
    {
        public IActionResult CreateStudent(Student stud);
        public IActionResult GetStudent(int id);
        public IActionResult GetStudent(string orderBy);
        public IActionResult Promotion(Promotion promotion);

        public bool CheckIfExists(string index);
    }
}