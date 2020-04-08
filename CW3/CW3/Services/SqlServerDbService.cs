using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using CW3.Models;
using Microsoft.AspNetCore.Mvc;

namespace CW3.Services
{
    public class SqlServerDbService : ControllerBase, IStudentsDbService
    {
        private const string connection = "Data Source=db-mssql;" +
                                          "Initial Catalog=s18507;Integrated Security=True";
        
        public IActionResult CreateStudent(Student student)
        {
            var list = new List<Enrollment>();
            using (SqlConnection con = new SqlConnection(connection))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from studies";
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    com.CommandText = "select * from studies where name=@benc";
                    com.Parameters.AddWithValue("@stud", student.StudiesName);
                    com.Transaction = transaction;
                    SqlDataReader dataReader = com.ExecuteReader();

                    if (!dataReader.HasRows)
                        return BadRequest("Study doesn't exist");
                    dataReader.Close();
                    com.CommandText =
                        "select * from Enrollment inner join studies on Studies.IdStudy=Enrollment.IdStudy where Studies.Name=@bstud and semester=1";
                    dataReader = com.ExecuteReader();
                    if (!dataReader.HasRows)
                    {
                        dataReader.Close();
                        com.CommandText =
                            "insert into enrollment(idEnrollment,semester,idstudy,startdate)values((select Max(idEnrollment)+1 from enrollment),1,(select idstudy from studies where name=@stud),GETDATE())";
                        com.ExecuteNonQuery();
                    }

                    dataReader.Close();
                    com.CommandText = "select * from students where IndexNumber=@id";
                    com.Parameters.AddWithValue("@id", student.IndexNumber);
                    dataReader = com.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        return NotFound("Has already exist");
                    }
                    else
                    {
                        dataReader.Close();
                        com.CommandText =
                            "insert into students(indexNumber,FirstName,LastName,BirthDate,IdEnrollment)values(@id,@name,@surname,(CONVERT(datetime, @birth, 104)),(select idEnrollment from enrollment inner join studies on enrollment.IdStudy=studies.IdStudy where studies.Name=@stud and semester=1))";
                        com.Parameters.AddWithValue("@name", student.FirstName);
                        com.Parameters.AddWithValue("@surname", student.LastName);
                        com.Parameters.AddWithValue("@birth", student.BirthDate);
                        com.ExecuteNonQuery();
                    }
                    dataReader.Close();
                    com.CommandText =
                        "select IdEnrollment,Semester,Name,StartDate from enrollment inner join studies on enrollment.IdStudy=studies.IdStudy where studies.Name=@stud and semester=1";
                    dataReader = com.ExecuteReader();
                    if (dataReader.Read())
                    {
                        list.Add(new Enrollment()
                        {
                            IdEnrollment = Convert.ToInt32(dataReader["IdEnrollment"]),
                            Semester = Convert.ToInt32(dataReader["Semestr"]),
                            Study = dataReader["Name"].ToString(),
                            StartDate = dataReader["StartDate"].ToString()
                        });
                    }
                    dataReader.Close();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return NotFound(e);
                }
            }

            return StatusCode((int) HttpStatusCode.Created, list);
        }

        public IActionResult GetStudent(int id)
        {
            var list = new List<Enrollment>();
            var connection = new SqlConnection("Data Source=db-mssql;" +
                                               "Initial Catalog=s18507;Integrated Security=True");
            var com = new SqlCommand()
            {
                Connection = connection,
                CommandText = $"select * from Enrollment e inner join Student s on e.IdEnrollment = s.IdEnrollment where s.IndexNumber = cast(@id as varchar)"

            };
            com.Parameters.AddWithValue("index", id);
            connection.Open();

            SqlDataReader dataReader = com.ExecuteReader();
            while (dataReader.Read())
            {
                list.Add(new Enrollment()
                {
                    IdEnrollment = Convert.ToInt32(dataReader["IdEnrollment"]),
                    Semester = Convert.ToInt32(dataReader["Semestr"]),
                    IdStudy = Convert.ToInt32(dataReader["IdStudy"]),
                    StartDate = dataReader["StartDate"].ToString()
                });
            }
            
            return Ok(list);

        }

        public IActionResult Promotion(Promotion promotion)
        {
            var list = new List<Enrollment>();
            using (SqlConnection con = new SqlConnection(connection))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "select * from enrollment inner join studies on studies.idstudy=enrollment.idstudy where enrollment.semester=@semester and studies.name=@studies";
                com.Parameters.AddWithValue("@studies", promotion.Studies);
                com.Parameters.AddWithValue("@semester", promotion.Semester);
                SqlDataReader dataReader = com.ExecuteReader();

                if (!dataReader.HasRows)
                    return NotFound("No such studies at this semestr");
                dataReader.Close();
                com.CommandText = "exec PromoteStudents @studies, @semester";
                com.ExecuteNonQuery();
                dataReader.Close();

                com.CommandText = "select IdEnrollment,StartDate from Enrollment inner join Studies on Enrollment.IdStudy=Studies.IdStudy where name=@studies and semester=@semester";
                dataReader = com.ExecuteReader();
                
                if (dataReader.Read())
                {
                    list.Add(new Enrollment()
                    {
                        IdEnrollment = Convert.ToInt32(dataReader["IdEnrollment"]),
                        Semester = promotion.Semester,
                        Study = promotion.Studies,
                        StartDate = dataReader["StartDate"].ToString()
                    });
                   
                }
                dataReader.Close();

            }
            return StatusCode((int)HttpStatusCode.Created,list);
        }

        public bool CheckIfExists(string index)
        {
            
            using (SqlConnection con = new SqlConnection(connection))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "select * from students where IndexNumber=@tmpId";
                com.Parameters.AddWithValue("@tmpId", index);
                SqlDataReader dr = com.ExecuteReader();

                return dr.HasRows;
            }
        }

        public IActionResult GetStudent(String orderBy)
        {
            var list = new List<Student>();

            using (SqlConnection con = new SqlConnection(connection))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select indexNumber,FirstName,LastName,BirthDate,Studies.name,Enrollment.semester from students inner join enrollment on students.idEnrollment=Enrollment.idEnrollment inner join studies on Enrollment.idStudy=studies.idstudy";

                con.Open();
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
    }
}