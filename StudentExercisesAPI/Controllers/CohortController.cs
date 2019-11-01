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
                                                        s.Id, s.FirstName AS StudentFirstName, s.LastName AS StudentLastName, s.SlackHandle AS StudentSlackHandle, s.CohortId,
                                                        i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName, i.SlackHandle AS InstructorSlackHandle,
					                                    se.ExerciseId, e.ExerciseName, e.ProgrammingLanguage

                                               FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id
                                                       LEFT JOIN Instructor i ON i.CohortId = c.Id
                                                       LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                                       INNER JOIN Exercise e ON se.ExerciseId = e.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort()
                        {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                        };
                        cohorts.Add(cohort);
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
                    cmd.CommandText = @"SELECT Id, CohortName
                                                FROM Cohort
                                    WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort aCohort = null;
                    if (reader.Read())
                    {
                        aCohort = new Cohort()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            //Students = new List<Student>(),
                            //Instructors = new List<Instructor>()
                        };
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
