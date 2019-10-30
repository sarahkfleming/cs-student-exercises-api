using System.Collections.Generic;


namespace StudentExercises
{
    public class Cohort
    {
        public int Id { get; set; }
        public string CohortName { get; set; }
        public List<Student> students { get; set; }  = new List<Student>();
        public List<Instructor> instructors { get; set; }  = new List<Instructor>();
    }
}