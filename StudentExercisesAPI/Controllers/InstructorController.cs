using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercises;

namespace StudentInstructorsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private string _connectionString;

        public InstructorController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: api/Instructor
        [HttpGet]
        public IEnumerable<Instructor> GetInstructors()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, i.Specialty, i.CohortId, c.CohortName
                                                FROM Instructor i LEFT JOIN Cohort c ON c.Id = i.CohortId";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };
                        instructors.Add(instructor);
                    }
                    reader.Close();

                    return instructors;
                }
            }
        }

        // GET: api/Instructor/5
        [HttpGet("{id}")]
        public IActionResult GetInstructor(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, i.SlackHandle, i.Specialty, i.CohortId, c.CohortName
                                                FROM Instructor i LEFT JOIN Cohort c ON c.Id = i.CohortId
                                         WHERE i.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor anInstructor = null;
                    if (reader.Read())
                    {
                        anInstructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };
                    }
                    reader.Close();

                    if (anInstructor == null)
                    {
                        return NotFound();
                    }

                    return Ok(anInstructor);
                }
            }
        }

        //// POST: api/Instructor
        [HttpPost]
        public void AddInstructor([FromBody] Instructor newInstructor)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, SlackHandle, Specialty, CohortId)
                                        VALUES (@FirstName, @LastName, @SlackHandle, @Specialty, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", newInstructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", newInstructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", newInstructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@Specialty", newInstructor.Specialty));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", newInstructor.CohortId));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // PUT: api/Instructor/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
