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
        public IEnumerable<Cohort> GetCohorts()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CohortName
                                                        FROM Cohort";
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

                    return cohorts;
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
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort aCohort = null;
                    if (reader.Read())
                    {
                        aCohort = new Cohort()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            Students = new List<Student>(),
                            Instructors = new List<Instructor>()
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
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Cohort/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
