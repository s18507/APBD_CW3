
using System;
using System.Collections.Generic;
using System.Linq;
using CW3.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CW3.DAL
{
    
    public class MockDbService : IDbService
    {
        private static List<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student {IdStudent = 1, FirstName = "Jan", LastName = "Kowalski"},
                new Student {IdStudent = 2, FirstName = "Anna", LastName = "Malewski"},
                new Student {IdStudent = 3, FirstName = "Andrzej", LastName = "Andrzejewski"}
            };

        }


        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public void DeleteStudents(int id)
        {
            _students.Remove(_students.First(stud => stud.IdStudent == id));
        }
    }
}