using System.Collections.Generic;
using CW3.Models;

namespace CW3.DAL
{
    public interface IDbService
        {
            public IEnumerable<Student> GetStudents();
            public void DeleteStudents(int id);
        }
    } 

