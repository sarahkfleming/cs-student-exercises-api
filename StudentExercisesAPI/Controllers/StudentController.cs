using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercises;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private string _connectionString;

        public StudentController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: api/Student
        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId,
					                                    c.CohortName, se.ExerciseId, e.ExerciseName, e.ProgrammingLanguage
                                            FROM Student s INNER JOIN Cohort c ON s.CohortId = c.Id
					                                    LEFT JOIN StudentExercise se ON se.StudentId = s.Id
					                                    INNER JOIN Exercise e ON se.ExerciseId = e.Id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Student> students = new Dictionary<int, Student>();
                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(reader.GetOrdinal("id"));
                        if (! students.ContainsKey(studentId))
                        {
                            Student newStudent = new Student()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                },
                            };

                            students.Add(studentId, newStudent);
                        }
                        // At this point we're certain that a student is in the dictionary
                        Student fromDictionary = students[studentId];

                        // Add exercise(s) to the students
                        if (! reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                        {
                            Exercise anExercise = new Exercise()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage"))
                            };

                            fromDictionary.Exercises.Add(anExercise);
                        }
                    }

                    reader.Close();

                    return Ok(students.Values);
                }
            }
        }

        //// GET: api/Student/5
        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle,  s.CohortId, c.CohortName
                                                FROM Student s LEFT JOIN Cohort c ON c.Id = s.CohortId
                                         WHERE s.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student aStudent = null;
                    if (reader.Read())
                    {
                        aStudent = new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            }
                        };
                    }
                    reader.Close();

                    if (aStudent == null)
                    {
                        return NotFound();
                    }

                    return Ok(aStudent);
                }
            }
        }

        //// POST: api/Student
        [HttpPost]
        public void AddStudent([FromBody] Student student)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId)
                                        VALUES (@FirstName, @LastName, @SlackHandle, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //// PUT: api/Student/5
        [HttpPut("{id}")]
        public void UpdateStudent(int id, [FromBody] Student student)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Student 
                            SET FirstName = @FirstName,
                             LastName = @LastName,
                             SlackHandle = @SlackHandle,
                             CohortId = @CohortId
                                    WHERE Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //// DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteStudent(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Student WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
