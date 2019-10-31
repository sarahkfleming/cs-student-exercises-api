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
    public class ExerciseController : ControllerBase
    {
        private string _connectionString;

        public ExerciseController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: api/Exercise
        [HttpGet]
        public IEnumerable<Exercise> GetExercises()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, ExerciseName, ProgrammingLanguage FROM Exercise";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();
                    while (reader.Read())
                    {
                        Exercise newExercise = new Exercise()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage"))
                        };
                        exercises.Add(newExercise);
                    }

                    reader.Close();

                    return exercises;
                }
            }

        }

        // GET: api/Exercise/5
        [HttpGet("{id}")]
        public IActionResult GetExercise(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ExerciseName, ProgrammingLanguage FROM Exercise
                                         WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise anExercise = null;
                    if (reader.Read())
                    {
                        anExercise = new Exercise()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage"))
                        };
                    }

                    reader.Close();

                    if (anExercise == null)
                    {
                        return NotFound();
                    }

                    return Ok(anExercise);
                }
            }
        }

        // POST: api/Exercise
        [HttpPost]
        public void AddExercise([FromBody] Exercise newExercise)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (ExerciseName, ProgrammingLanguage)
                                        VALUES (@ExerciseName, @ProgrammingLanguage)";
                    cmd.Parameters.Add(new SqlParameter("@ExerciseName", newExercise.ExerciseName));
                    cmd.Parameters.Add(new SqlParameter("@ProgrammingLanguage", newExercise.ProgrammingLanguage));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // PUT: api/Exercise/5
        [HttpPut("{id}")]
        public void UpdateExercise(int id, [FromBody] Exercise exercise)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Exercise
                                           SET ExerciseName = @ExerciseName, ProgrammingLanguage = @ProgrammingLanguage
                                         WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@ExerciseName", exercise.ExerciseName));
                    cmd.Parameters.Add(new SqlParameter("@ProgrammingLanguage", exercise.ProgrammingLanguage));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteExercise(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Exercise WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
