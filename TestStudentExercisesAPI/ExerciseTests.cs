using Newtonsoft.Json;
using StudentExercises;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestStudentExercisesAPI
{
    public class ExerciseTests
    {
        [Fact]
        public async Task Test_Create_Exercise()
        {
            /*
                Generate a new instance of an HttpClient that you can
                use to generate HTTP requests to your API controllers.
                The `using` keyword will automatically dispose of this
                instance of HttpClient once your code is done executing.
            */
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                // Construct a new student object to be sent to the API
                Exercise trestlebridge = new Exercise
                {
                    ExerciseName = "Trestlebridge",
                    ProgrammingLanguage = "C#"
                };

                // Serialize the C# object into a JSON string
                var trestlebridgeAsJSON = JsonConvert.SerializeObject(trestlebridge);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/exercise",
                    new StringContent(trestlebridgeAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Student
                var newTrestlebridge = JsonConvert.DeserializeObject<Exercise>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Trestlebridge", newTrestlebridge.ExerciseName);
                Assert.Equal("C#", newTrestlebridge.ProgrammingLanguage);
            }
        }

        [Fact]
        public async Task Test_Get_All_Exercises()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/exercise");


                string responseBody = await response.Content.ReadAsStringAsync();
                var exerciseList = JsonConvert.DeserializeObject<List<Exercise>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(exerciseList.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Modify_Exercise()
        {
            // New last name to change to and test
            string newExerciseName = "Trestlebridge Farms";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Exercise modifiedExercise = new Exercise
                {
                    ExerciseName = newExerciseName,
                    ProgrammingLanguage = "C#"
                };
                var modifiedExerciseAsJSON = JsonConvert.SerializeObject(modifiedExercise);

                var response = await client.PutAsync(
                    "/api/exercise/4005",
                    new StringContent(modifiedExerciseAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getExercise = await client.GetAsync("/api/exercise/4005");
                getExercise.EnsureSuccessStatusCode();

                string getExerciseBody = await getExercise.Content.ReadAsStringAsync();
                Exercise newExercise = JsonConvert.DeserializeObject<Exercise>(getExerciseBody);

                Assert.Equal(HttpStatusCode.OK, getExercise.StatusCode);
                Assert.Equal(newExerciseName, newExercise.ExerciseName);
            }
        }
        [Fact]
        public async Task Test_Delete_Exercise()
        {
            /*
                Generate a new instance of an HttpClient that you can
                use to generate HTTP requests to your API controllers.
                The `using` keyword will automatically dispose of this
                instance of HttpClient once your code is done executing.
            */
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                // Nothing needed here since we're deleting


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.DeleteAsync(
                    "/api/exercise/4005"
                );


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}