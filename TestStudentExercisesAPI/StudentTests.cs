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
    public class StudentTests
    {
        [Fact]
        public async Task Test_Create_Student()
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
                Student jack = new Student
                {
                    FirstName = "Jack",
                    LastName = "Spaniel",
                    SlackHandle = "@jack",
                    CohortId = 2
                };

                // Serialize the C# object into a JSON string
                var jackAsJSON = JsonConvert.SerializeObject(jack);


                /*
                    ACT
                */

                // Use the client to send the request and store the response
                var response = await client.PostAsync(
                    "/api/student",
                    new StringContent(jackAsJSON, Encoding.UTF8, "application/json")
                );

                // Store the JSON body of the response
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an instance of Student
                var newJack = JsonConvert.DeserializeObject<Student>(responseBody);


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Jack", newJack.FirstName);
                Assert.Equal("Spaniel", newJack.LastName);
                Assert.Equal("@jack", newJack.SlackHandle);
                Assert.Equal(2, newJack.CohortId);
            }
        }

        [Fact]
        public async Task Test_Get_All_Students()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/student");


                string responseBody = await response.Content.ReadAsStringAsync();
                var studentList = JsonConvert.DeserializeObject<List<Student>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(studentList.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Modify_Student()
        {
            // New last name to change to and test
            string newStudentSlackHandle = "@annette2";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Student modifiedStudent = new Student
                {
                    FirstName = "Annette",
                    LastName = "Browning",
                    SlackHandle = newStudentSlackHandle,
                    CohortId = 2
                };
                var modifiedStudentAsJSON = JsonConvert.SerializeObject(modifiedStudent);

                var response = await client.PutAsync(
                    "/api/student/15",
                    new StringContent(modifiedStudentAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getStudent = await client.GetAsync("/api/student/15");
                getStudent.EnsureSuccessStatusCode();

                string getStudentBody = await getStudent.Content.ReadAsStringAsync();
                Student newStudent = JsonConvert.DeserializeObject<Student>(getStudentBody);

                Assert.Equal(HttpStatusCode.OK, getStudent.StatusCode);
                Assert.Equal(newStudentSlackHandle, newStudent.SlackHandle);
            }
        }
        [Fact]
        public async Task Test_Delete_Student()
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
                    "/api/student/2011"
                );


                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}