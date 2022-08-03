using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    /// <summary>
    /// A controller class that represents the students
    /// </summary>
    /// <Author>Sarthak Jain, Bryce Fairbanks, Daniel Kopta</Author>
    /// <Class> CS 5530 Spring 2022 </Class>
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Makes a query to get all the classes of a student that they are enrolled in
                var queryGetClasses = from course in db.Courses
                                      join classes in db.Classes on course.CourseId equals classes.CourseId
                                      join enroll in db.Enroll on classes.ClassId equals enroll.ClassId
                                      where enroll.UId == uid
                                      select new
                                      {
                                          subject = course.Subject,
                                          number = course.Num,
                                          name = course.Name,
                                          season = classes.Semester,
                                          year = classes.Year,
                                          grade = enroll.Grade
                                      };

                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetClasses.ToArray());
            }
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the information about the assignment
                var queryAssignmentInfo = from classes in db.Classes
                                          join courses in db.Courses on classes.CourseId equals courses.CourseId
                                          join assignmentCategories in db.AssignmentCategories on classes.ClassId equals assignmentCategories.ClassId
                                          join assignments in db.Assignments on assignmentCategories.CatId equals assignments.CatId
                                          where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year
                                          select new
                                          {
                                              aname = assignments.Name,
                                              cname = assignmentCategories.Name,
                                              due = assignments.DueDate,
                                              assignId = assignments.AssignId
                                          };
                //Makes a query to get the student's information about the assignment they worked on using information from the assignment
                var queryStudentPerspectiveForAssignment = from assignmentInfo in queryAssignmentInfo
                                                           join s in db.Submission
                                                           on new
                                                           {
                                                               A = assignmentInfo.assignId,
                                                               B = uid
                                                           } equals new
                                                           {
                                                               A = s.AssignId,
                                                               B = s.UId
                                                           }
                                                           into fullAssignment
                                                           from fullAssignmentDef in fullAssignment.DefaultIfEmpty()
                                                           select new
                                                           {
                                                               aname = assignmentInfo.aname,
                                                               cname = assignmentInfo.cname,
                                                               due = assignmentInfo.due,
                                                               score = fullAssignmentDef.UId == null ? null : (decimal?)fullAssignmentDef.Score
                                                           };


                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryStudentPerspectiveForAssignment.ToArray());
            }
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// Does *not* automatically reject late submissions.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}.</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Makes a query to get the submission info if an assignment has been submitted
                var querySubmission = from classes in db.Classes
                                      join courses in db.Courses on classes.CourseId equals courses.CourseId
                                      join assignmentCategories in db.AssignmentCategories on classes.ClassId equals assignmentCategories.ClassId
                                      join assignments in db.Assignments on assignmentCategories.CatId equals assignments.CatId
                                      join submissions in db.Submission on assignments.AssignId equals submissions.AssignId
                                      where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year && assignmentCategories.Name == category && assignments.Name == asgname && submissions.UId == uid
                                      select new
                                      {
                                          uID = submissions.UId,
                                          assignId = submissions.AssignId,
                                          contents = submissions.Contents,
                                          submitTime = submissions.SubmitTime,
                                          score = submissions.Score
                                      };
                //If there was a submission already, then removes the submission from the table of submissions
                if (querySubmission.Count() != 0)
                {
                    foreach (var submission in querySubmission)
                    {
                        //Creates a submission object to remove from the database
                        Submission removeSubmission = new Submission();
                        removeSubmission.UId = submission.uID;
                        removeSubmission.AssignId = submission.assignId;
                        removeSubmission.Contents = submission.contents;
                        removeSubmission.SubmitTime = submission.submitTime;
                        removeSubmission.Score = submission.score;

                        //Removes the object from the database
                        db.Submission.Remove(removeSubmission);
                    }
                }

                //Makes a query to get the assignment ID for an assignment
                var queryAssignID = from classes in db.Classes
                                    join courses in db.Courses on classes.CourseId equals courses.CourseId
                                    join assignmentCategory in db.AssignmentCategories on classes.ClassId equals assignmentCategory.ClassId
                                    join assignment in db.Assignments on assignmentCategory.CatId equals assignment.CatId
                                    where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year && assignmentCategory.Name == category && assignment.Name == asgname
                                    select assignment.AssignId;

                //Sets the assignID to the assignID from the query
                uint assignID = uint.MaxValue;
                foreach (var aID in queryAssignID)
                {
                    assignID = aID;
                }

                //Creates a submission object
                Submission newSubmission = new Submission();

                //Sets the fields of the CheckedOut object based on the paramters
                newSubmission.UId = uid;
                newSubmission.AssignId = (uint)assignID;
                newSubmission.Contents = contents;
                newSubmission.SubmitTime = DateTime.Now;
                newSubmission.Score = 0;

                //Adds the object to the table in the database
                db.Submission.Add(newSubmission);
                //Saves the change made in the database
                db.SaveChanges();
            }
            //Indicates a success for updating the submission
            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false},
        /// false if the student is already enrolled in the Class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Makes a query to get the classID for a certain class based on the subject, number, season and year
                var queryClassID = from courses in db.Courses
                                   join classes in db.Classes on courses.CourseId equals classes.CourseId
                                   where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year
                                   select classes.ClassId;
                uint classId = uint.MaxValue;
                //Gets the classID for the class
                foreach (var cID in queryClassID)
                {
                    classId = cID;
                }

                //Makes a query to see if the student is already enrolled in a class
                var queryIsAlreadyEnrolled = from enroll in db.Enroll
                                             where enroll.ClassId == classId && enroll.UId == uid
                                             select enroll.UId;
                //If the student is already enrolled, then return false
                if (queryIsAlreadyEnrolled.ToList().Count != 0)
                {
                    return Json(new { success = false });
                }

                //Creates an Enroll object
                Enroll newEnroll = new Enroll();

                //Sets the fields of the Enroll object based on the paramters
                newEnroll.UId = uid;
                newEnroll.ClassId = classId;
                //Sets the default grade as "--"
                newEnroll.Grade = "--";

                //Adds the object to the table in the database
                db.Enroll.Add(newEnroll);

                //Saves the change made in the database
                db.SaveChanges();
            }
            //Indicates a success for checking a book out
            return Json(new { success = true });
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student does not have any grades, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                // Makes a query to get the grades of all the classes a student is enrolled in
                var queryGetGradesOfStudent = from enroll in db.Enroll
                                              where enroll.UId == uid
                                              select enroll.Grade;

                //If there is no grade available, then returns the gpa as 0.0
                if (queryGetGradesOfStudent.Count() == 0)
                {
                    return Json(new { gpa = 0.0 });
                }

                //Creates a dictionary to represent the grades of a student mapping each letter grade to a GPA
                Dictionary<string, double> gradeMap = new Dictionary<string, double>();
                gradeMap.Add("A", 4.0);
                gradeMap.Add("A-", 3.7);
                gradeMap.Add("B+", 3.3);
                gradeMap.Add("B", 3.0);
                gradeMap.Add("B-", 2.7);
                gradeMap.Add("C+", 2.3);
                gradeMap.Add("C", 2.0);
                gradeMap.Add("C-", 1.7);
                gradeMap.Add("D+", 1.3);
                gradeMap.Add("D", 1.0);
                gradeMap.Add("D-", 0.7);
                gradeMap.Add("E", 0);

                double totalGrades = 0;
                double totalClasses = 0;
                //For each individual grade, maps the grade to the GPA value on the map
                foreach (var grade in queryGetGradesOfStudent)
                {
                    //If the grade is "--" or doesn't exist, then just continues
                    if (grade == "--" || !gradeMap.ContainsKey(grade))
                    {
                        continue;
                    }

                    gradeMap.TryGetValue(grade, out double currGrade);
                    totalGrades += currGrade;
                    totalClasses++;
                }
                //Calculates the GPA by dividing the totalGrades from the totalClasses
                double gpa = 0.0;
                if (totalClasses != 0)
                {
                    gpa = totalGrades / totalClasses;
                }

                //Returns a JSON where it passes in a gpa
                return Json(new { gpa = gpa });
            }
        }

        /*******End code to modify********/

    }
}