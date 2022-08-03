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
    /// A controller class that represents the professor
    /// </summary>
    /// <Author>Sarthak Jain, Bryce Fairbanks, Daniel Kopta</Author>
    /// <Class> CS 5530 Spring 2022 </Class>
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)

        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Makes a query to get all students enrolled in a class
                var queryGetStudents = from course in db.Courses
                                       join classes in db.Classes on course.CourseId equals classes.CourseId
                                       join enroll in db.Enroll on classes.ClassId equals enroll.ClassId
                                       join students in db.Students on enroll.UId equals students.UId
                                       where course.Subject == subject && course.Num == num && classes.Semester == season && classes.Year == year
                                       select new
                                       {
                                           fname = students.FirstName,
                                           lname = students.LastName,
                                           uid = students.UId,
                                           dob = students.Dob,
                                           grade = enroll.Grade
                                       };

                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetStudents.ToArray());
            }
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //If the category is null, then gets the assignment name, assignment category,
                //assignment due date and the number of submissions from all assignments in all categories
                if (category == null)
                {
                    var queryGetAssignmentCategories = from classes in db.Classes
                                                       join courses in db.Courses on classes.CourseId equals courses.CourseId
                                                       join assignmentCategory in db.AssignmentCategories on classes.ClassId equals assignmentCategory.ClassId
                                                       join assignment in db.Assignments on assignmentCategory.CatId equals assignment.CatId
                                                       where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year
                                                       select new
                                                       {
                                                           aname = assignment.Name,
                                                           cname = assignmentCategory.Name,
                                                           due = assignment.DueDate,
                                                           submissions = (from s in db.Submission where s.AssignId == assignment.AssignId select s.AssignId).Count()
                                                       };

                    //Returns a JSON where it converts the query to an array that can be parsed
                    return Json(queryGetAssignmentCategories.ToArray());
                }
                //If the category is specified, then gets the assignment name, assignment category, assignment due date
                //and the number of submissions from all assignments from a particular category
                else
                {
                    var queryGetAssignmentCategories = from classes in db.Classes
                                                       join courses in db.Courses on classes.CourseId equals courses.CourseId
                                                       join assignmentCategory in db.AssignmentCategories on classes.ClassId equals assignmentCategory.ClassId
                                                       join assignment in db.Assignments on assignmentCategory.CatId equals assignment.CatId
                                                       where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year && assignmentCategory.Name == category
                                                       select new
                                                       {
                                                           aname = assignment.Name,
                                                           cname = assignmentCategory.Name,
                                                           due = assignment.DueDate,
                                                           submissions = (from s in db.Submission where s.AssignId == assignment.AssignId select s.AssignId).Count()
                                                       };


                    //Returns a JSON where it converts the query to an array that can be parsed
                    return Json(queryGetAssignmentCategories.ToArray());
                }
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {

                //Gets all the assignment category names and weights from a certain class
                var queryGetAssignmentCategories = from classes in db.Classes
                                                   join courses in db.Courses on classes.CourseId equals courses.CourseId
                                                   join assignmentCategory in db.AssignmentCategories on classes.ClassId equals assignmentCategory.ClassId
                                                   where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year
                                                   select new
                                                   {
                                                       name = assignmentCategory.Name,
                                                       weight = assignmentCategory.Weight
                                                   };


                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetAssignmentCategories.ToArray());
            }
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false},
        ///	false if an assignment category with the same name already exists in the same class.</returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the classID based on the subject, number, semester and year
                var queryClassID = from classes in db.Classes
                                   join courses in db.Courses on classes.CourseId equals courses.CourseId
                                   where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year
                                   select classes.ClassId;

                //Sets the classID based on the query
                uint classId = uint.MaxValue;
                foreach (var cID in queryClassID)
                {
                    classId = cID;
                }

                // Gets the assignment cateogies based on the category and the classID
                var queryGetAssignmentCategories = from assignmentCategories in db.AssignmentCategories
                                                   where assignmentCategories.ClassId == classId && assignmentCategories.Name == category
                                                   select assignmentCategories.CatId;

                //If there is no categories, then returns false
                if (queryGetAssignmentCategories.Count() != 0)
                {
                    return Json(new { success = false });
                }

                //Creates a new AssignmentCategory object
                AssignmentCategories assignmentCategory = new AssignmentCategories();

                //Sets the fields of the AssignmentCategory object based on the paramters
                assignmentCategory.Name = category;
                assignmentCategory.ClassId = classId;
                assignmentCategory.Weight = (uint)catweight;

                //Adds the object to the table in the database
                db.AssignmentCategories.Add(assignmentCategory);
                //Saves the change made in the database
                db.SaveChanges();

                //Indicates a success for checking a book out
                return Json(new { success = true });
            }
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false,
        /// false if an assignment with the same name already exists in the same assignment category.</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the categoryID based on the subject, number, semester and year and the category entered
                var queryAssignmentCategoryID = from classes in db.Classes
                                                join courses in db.Courses on classes.CourseId equals courses.CourseId
                                                join assignmentCategories in db.AssignmentCategories on classes.ClassId equals assignmentCategories.ClassId
                                                where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year && assignmentCategories.Name == category
                                                select assignmentCategories.CatId;

                //Sets the catID based on the query
                uint categoryID = uint.MaxValue;
                foreach (var catID in queryAssignmentCategoryID)
                {
                    categoryID = catID;
                }
                // Gets the assignmentID based on the categoryID and the assignmentName
                var queryGetAssignmentID = from assignments in db.Assignments
                                           where assignments.CatId == categoryID && assignments.Name == asgname
                                           select assignments.AssignId;

                //If there is an assignment ID return false
                if (queryGetAssignmentID.Count() != 0)
                {
                    return Json(new { success = false });
                }

                //Creates an Assignment object to indicate a new Assignment being created
                Assignments newAssignment = new Assignments();

                //Sets the fields of the Assignment object based on the paramters
                newAssignment.Name = asgname;
                newAssignment.Points = (uint)asgpoints;
                newAssignment.DueDate = asgdue;
                newAssignment.Contents = asgcontents;
                newAssignment.CatId = categoryID;

                //Adds the object to the table in the database
                db.Assignments.Add(newAssignment);

                //Saves the change made in the database
                db.SaveChanges();

                //Gets all the uid of all the students enrolled in this particular class
                var queryGetStudentsUID = from courses in db.Courses
                                          join classes in db.Classes on courses.CourseId equals classes.CourseId
                                          join enroll in db.Enroll on classes.ClassId equals enroll.ClassId
                                          where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year
                                          select enroll.UId;

                //Iterates through all the students and updates their grade as a new assignment has been created
                foreach (var studentUID in queryGetStudentsUID)
                {
                    UpdateGrade(subject, num, season, year, studentUID);
                }

                //Indicates a success for checking a book out
                return Json(new { success = true });
            }
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the information required to show the submission info to an assignment of a particular class
                var queryGetSubmissions = from classes in db.Classes
                                          join courses in db.Courses on classes.CourseId equals courses.CourseId
                                          join assignmentCategories in db.AssignmentCategories on classes.ClassId equals assignmentCategories.ClassId
                                          join assignments in db.Assignments on assignmentCategories.CatId equals assignments.CatId
                                          join submissions in db.Submission on assignments.AssignId equals submissions.AssignId
                                          join students in db.Students on submissions.UId equals students.UId
                                          where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year && assignmentCategories.Name == category && assignments.Name == asgname
                                          select new
                                          {
                                              fname = students.FirstName,
                                              lname = students.LastName,
                                              uid = students.UId,
                                              time = submissions.SubmitTime,
                                              score = submissions.Score
                                          };


                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetSubmissions.ToArray());
            }
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the submission info to an assignment of a particular class
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

                //Gets the first submission from this query, which is the most recent submission
                var submission = querySubmission.First();

                //Creates a new submission object and sets the field according to the submission variable's parameter
                Submission newSubmission = new Submission();
                newSubmission.UId = submission.uID;
                newSubmission.AssignId = submission.assignId;
                newSubmission.Contents = submission.contents;
                newSubmission.SubmitTime = submission.submitTime;
                newSubmission.Score = (uint)score;

                //Updates the database by adding the new submission, or updating a previous submission
                db.Submission.Update(newSubmission);

                //Saves the changes in the database
                db.SaveChanges();

                //Updates the grade of the student
                UpdateGrade(subject, num, season, year, uid);
            }
            //Indicates a success for checking a book out
            return Json(new { success = true });
        }

        /// <summary>
        /// A helper method to update the grades of all students
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid">The uid of the student who's grade is being updated</param>
        private void UpdateGrade(string subject, int num, string season, int year, string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Makes a query to get all the information from an assignment
                var queryGetAssignmentInfo = from courses in db.Courses
                                             join classes in db.Classes on courses.CourseId equals classes.CourseId
                                             join assignmentCategories in db.AssignmentCategories on classes.ClassId equals assignmentCategories.ClassId
                                             join assignment in db.Assignments on assignmentCategories.CatId equals assignment.CatId
                                             where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year
                                             select new
                                             {
                                                 classId = classes.ClassId,
                                                 assignId = assignment.AssignId,
                                                 acName = assignmentCategories.Name,
                                                 weight = assignmentCategories.Weight,
                                                 maxScore = assignment.Points
                                             };

                //Makes a query to get the score from all assignments based on their submission
                var queryGetAssignmentScore = from getAssignmentInfo in queryGetAssignmentInfo
                                              join submission in db.Submission
                                              on new
                                              {
                                                  A = getAssignmentInfo.assignId,
                                                  B = uid
                                              } equals new
                                              {
                                                  A = submission.AssignId,
                                                  B = submission.UId
                                              }
                                              into fullAssignment
                                              from fullAssignmentDef in fullAssignment.DefaultIfEmpty()
                                              select new
                                              {
                                                  acName = getAssignmentInfo.acName,
                                                  weight = getAssignmentInfo.weight,
                                                  maxScore = getAssignmentInfo.maxScore,
                                                  score = fullAssignmentDef == null ? 0 : (decimal)fullAssignmentDef.Score
                                              };



                string grade = "";

                // A Dictionary to keep track of the scores of a student based on assignment cateogory being the key
                // and the value being a list, where the first item in the list is the assignment weight, the second is
                // the assignment score and the third item is the assignment max score
                Dictionary<string, List<double>> totScores = new Dictionary<string, List<double>>();

                //Fills up the dictionary based on the assignment scores of a student
                foreach (var assign in queryGetAssignmentScore)
                {
                    if (!totScores.ContainsKey(assign.acName))
                    {
                        totScores.Add(assign.acName, new List<double>() { assign.weight, (double)assign.score, assign.maxScore });
                    }
                    else
                    {
                        List<double> getList = totScores[assign.acName];
                        getList[1] = getList[1] + (double)assign.score;
                        getList[2] = getList[2] + assign.maxScore;
                        totScores[assign.acName] = getList;
                    }
                }

                double totalPercentage = 0;
                double totalWeight = 0;

                //Adds up the total percentage and the total weight
                foreach (string key in totScores.Keys)
                {
                    totScores.TryGetValue(key, out List<double> scoreList);
                    totalWeight += scoreList[0];
                    totalPercentage += (scoreList[1] / scoreList[2]) * scoreList[0];
                }

                //Divides the total percentage by the total weight to get the total percentage of a student
                double totalGrades = totalPercentage / totalWeight;

                //If the total grade is between a particular range, then their grade is set based on that scale
                if (totalGrades >= .93)
                {
                    grade = "A";
                }
                else if (totalGrades < .93 && totalGrades >= .9)
                {
                    grade = "A-";
                }
                else if (totalGrades < .9 && totalGrades >= .87)
                {
                    grade = "B+";
                }
                else if (totalGrades < .87 && totalGrades >= .83)
                {
                    grade = "B";
                }
                else if (totalGrades < .83 && totalGrades >= .8)
                {
                    grade = "B-";
                }
                else if (totalGrades < .8 && totalGrades >= .77)
                {
                    grade = "C+";
                }
                else if (totalGrades < .77 && totalGrades >= .73)
                {
                    grade = "C";
                }
                else if (totalGrades < .73 && totalGrades >= .7)
                {
                    grade = "C-";
                }
                else if (totalGrades < .7 && totalGrades >= .67)
                {
                    grade = "D+";
                }
                else if (totalGrades < .67 && totalGrades >= .63)
                {
                    grade = "D";
                }
                else if (totalGrades < .63 && totalGrades >= .6)
                {
                    grade = "D-";
                }
                else
                {
                    grade = "E";
                }

                //Creates a new Enroll object 
                Enroll newGrade = new Enroll();
                //Sets the values of the enrolled object
                newGrade.ClassId = queryGetAssignmentInfo.First().classId;
                newGrade.UId = uid;
                newGrade.Grade = grade;
                //Updates the enroll table and saves the changes
                db.Enroll.Update(newGrade);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {

                //Gets the subject, number, name, season and year of a class based on what class a professor is teaching
                var queryGetClassInfo = from classes in db.Classes
                                        join courses in db.Courses on classes.CourseId equals courses.CourseId
                                        where classes.UId == uid
                                        select new
                                        {
                                            subject = courses.Subject,
                                            number = courses.Num,
                                            name = courses.Name,
                                            season = classes.Semester,
                                            year = classes.Year
                                        };


                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetClassInfo.ToArray());
            }

        }


        /*******End code to modify********/

    }
}