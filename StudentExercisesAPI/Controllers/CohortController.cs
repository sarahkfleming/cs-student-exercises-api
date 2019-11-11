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
    public class CohortController : ControllerBase
    {
        private string _connectionString;

        public CohortController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: api/Cohort
        [HttpGet]
        public async Task<IActionResult> GetCohorts()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                            SELECT c.Id, c.CohortName,
                                                        s.Id AS StudentId, s.FirstName AS StudentFirstName, s.LastName AS StudentLastName, s.SlackHandle AS StudentSlackHandle, s.CohortId AS StudentCohortId,
                                                        i.Id AS InstructorId, i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName, i.SlackHandle AS InstructorSlackHandle, i.CohortId AS InstructorCohortId,
					                                    se.ExerciseId, e.ExerciseName, e.ProgrammingLanguage

                                               FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id
                                                       LEFT JOIN Instructor i ON i.CohortId = c.Id
                                                       LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                                       INNER JOIN Exercise e ON se.ExerciseId = e.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohort cohort = null;
                        var cohortId = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!cohorts.Any(cohort => cohort.Id == cohortId))
                        {
                            cohort = new Cohort()
                            {
                                Id = cohortId,
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            };
                            cohorts.Add(cohort);
                        }

                        Cohort existingCohort = cohorts.Find(cohort => cohort.Id == cohortId);
                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")) && !existingCohort.Instructors.Any(instructor => instructor.Id == reader.GetInt32(reader.GetOrdinal("InstructorId"))))
                        {
                            Instructor instructor = new Instructor
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("InstructorCohortId"))
                            };
                            existingCohort.Instructors.Add(instructor);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")) && !existingCohort.Students.Any(student => student.Id == reader.GetInt32(reader.GetOrdinal("StudentId"))))
                        {
                            Student student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("StudentCohortId"))
                            };
                            existingCohort.Students.Add(student);
                        }

                    }
                    reader.Close();

                    return Ok(cohorts);
                }
            }
        }

        //// GET: api/Cohort/5
        [HttpGet("{id}")]
        public IActionResult GetCohort(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS CohortId, c.CohortName,
                                                                s.Id AS StudentId, s.FirstName AS StudentFirstName, s.LastName AS StudentLastName, s.SlackHandle AS StudentSlackHandle, s.CohortId AS StudentCohortId,
                                                                i.Id AS InstructorId, i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName, i.SlackHandle AS InstructorSlackHandle, i.CohortId AS InstructorCohortId
                                                       FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id
                                                       LEFT JOIN Instructor i ON i.CohortId = c.Id
                                                        WHERE c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort aCohort = null;
                    while (reader.Read())
                    {
                        if (aCohort == null)
                        {
                            aCohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")) && !aCohort.Instructors.Any(instructor => instructor.Id == reader.GetInt32(reader.GetOrdinal("InstructorId"))))
                        {
                            Instructor instructor = new Instructor
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("InstructorCohortId"))
                            };
                            aCohort.Instructors.Add(instructor);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")) && !aCohort.Students.Any(student => student.Id == reader.GetInt32(reader.GetOrdinal("StudentId"))))
                        {
                            Student student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("StudentCohortId"))
                            };
                            aCohort.Students.Add(student);
                        }
                    }
                    reader.Close();

                    if (aCohort == null)
                    {
                        return NotFound();
                    }

                    return Ok(aCohort);
                }
            }
        }

        //// POST: api/Cohort
        [HttpPost]
        public void AddCohort([FromBody] Cohort cohort)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohort (CohortName)
                                        VALUES (@CohortName)";
                    cmd.Parameters.Add(new SqlParameter("@CohortName", cohort.CohortName));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //// PUT: api/Cohort/5
        [HttpPut("{id}")]
        public void UpdateCohort(int id, [FromBody] Cohort cohort)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Cohort
                                           SET CohortName= @CohortName
                                         WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@CohortName", cohort.CohortName));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //// DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteCohort(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Cohort WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
