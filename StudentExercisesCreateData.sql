-- 1. Create tables for each entity. These should match the dbdiagram ERD you created in Student Exercises Part 1.​

--CREATE TABLE Exercise (
--    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
--   ExerciseName VARCHAR(55) NOT NULL,
--   ProgrammingLanguage VARCHAR(55) NOT NULL
--);

--CREATE TABLE Cohort (
--    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
--    CohortName VARCHAR(55) NOT NULL
--);

--CREATE TABLE Instructor (
--    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
--    FirstName VARCHAR(55) NOT NULL,
--    LastName VARCHAR(55) NOT NULL,
--    SlackHandle VARCHAR(55) NOT NULL,
--    CohortId INTEGER NOT NULL
--	CONSTRAINT FK_Instructor_Cohort FOREIGN KEY(CohortId) REFERENCES Cohort(Id)
--);

--CREATE TABLE Student (
--    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
--    FirstName VARCHAR(55) NOT NULL,
--    LastName VARCHAR(55) NOT NULL,
--    SlackHandle VARCHAR(55) NOT NULL,
--    CohortId INTEGER NOT NULL
--	CONSTRAINT FK_Student_Cohort FOREIGN KEY(CohortId) REFERENCES Cohort(Id)
--);
--​
--CREATE TABLE StudentExercise (
--    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
--    StudentId INTEGER NOT NULL,
--    ExerciseId INTEGER NOT NULL,
--    CONSTRAINT FK_Assigned_Student FOREIGN KEY(StudentId) REFERENCES Students(Id),
--    CONSTRAINT FK_Assigned_Exercise FOREIGN KEY(ExerciseId) REFERENCES Exercises(Id)
--);
​
-- 2. Populate each table with data. You should have 2-3 cohorts, 5-10 students, 4-8 instructors, 2-5 exercises and each student should be assigned 1-2 exercises.
​
--INSERT INTO Exercise (Name, Language) VALUES ('ChickenMonkey', 'JavaScript')
--INSERT INTO Exercise (Name, Language) VALUES ('Plan Your Heist', 'C#')
--INSERT INTO Exercise (Name, Language) VALUES ('Kneel Diamond', 'JavaScript')
--INSERT INTO Exercise (Name, Language) VALUES ('Departments and Employees', 'SQL')

--INSERT INTO Cohort (CohortName) VALUES ('Day 34')
--INSERT INTO Cohort (CohortName) VALUES ('Evening 09')
--INSERT INTO Cohort (CohortName) VALUES ('Day 35')

--INSERT INTO Students (FirstName, LastName, SlackHandle, CohortId) VALUES ('Sarah', 'Fleming', '@sarah', 1)
--INSERT INTO Students (FirstName, LastName, SlackHandle, CohortId) VALUES ('Jennifer', 'Barnes', '@jenny', 2)
--INSERT INTO Students (FirstName, LastName, SlackHandle, CohortId) VALUES ('Doris', 'Delaney', '@doris', 3)
--INSERT INTO Students (FirstName, LastName, SlackHandle, CohortId) VALUES ('Heather', 'Jackson', '@heather', 3)
--INSERT INTO Students (FirstName, LastName, SlackHandle, CohortId) VALUES ('Annette', 'Browning', '@annette', 2)

--INSERT INTO Instructor (FirstName, LastName, SlackHandle, CohortId) VALUES ('Andy', 'Collins', '@andyc', 1)
--INSERT INTO Instructor (FirstName, LastName, SlackHandle, CohortId) VALUES ('Steve', 'Brownlee', '@coach', 2)
--INSERT INTO Instructor (FirstName, LastName, SlackHandle, CohortId) VALUES ('Bryan', 'Nilsen', '@bry5', 1)
--INSERT INTO Instructor (FirstName, LastName, SlackHandle, CohortId) VALUES ('Jenna', 'Solis', '@jenna', 1)

--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (1, 1)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (1, 2)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (2, 1)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (2, 3)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (3, 2)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (3, 3)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (4, 3)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (4, 4)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (5, 4)
--INSERT INTO StudentExercise (StudentId, ExerciseId) VALUES (5, 2)​

​
-- 3. Write a query to return all student first and last names with their cohort's name.
​
--SELECT s.FirstName,
--       s.LastName,
--	   c.Name
--FROM Students s
--	LEFT JOIN Cohorts c on s.CohortId = c.Id
--;
​
-- 4. Write a query to return student first and last names with their cohort's name only for a single cohort.
​
--SELECT s.FirstName,
--       s.LastName,
--	   c.Name
--FROM Students s
--	LEFT JOIN Cohorts c on s.CohortId = c.Id
--WHERE c.Id = 3
--;
​
-- 5. Write a query to return all instructors ordered by their last name.
--SELECT FirstName,
--	   LastName
--FROM Instructors
--ORDER BY LastName 
--;
​
-- 6. Write a query to return a list of unique instructor specialties.
--SELECT DISTINCT Specialty
--FROM Instructors
--;
​
-- 7. Write a query to return a list of all student names along with the names of the exercises they have been assigned. If an exercise has not been assigned, it should not be in the result.
--SELECT s.FirstName,
--	   s.LastName,
--	   e.Name
--FROM StudentExercises se
--	INNER JOIN Students s on se.StudentId = s.Id
--	INNER JOIN Exercises e on se.ExerciseId = e.Id
--;
​
-- 8. Return a list of student names along with the count of exercises assigned to each student.
--SELECT COUNT (se.ExerciseId) NumberOfExercisesAssigned,
--		s.FirstName,
--		s.LastName
--FROM StudentExercises se
--	INNER JOIN Students s on se.StudentId = s.Id
--GROUP BY s.FirstName, s.LastName
--;