using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    /// <summary>
    /// A controller class that represents the common methods
    /// </summary>
    /// <Author>Sarthak Jain, Bryce Fairbanks, Daniel Kopta</Author>
    /// <Class> CS 5530 Spring 2022 </Class>
    public class CommonController : Controller
    {

        /*******Begin code to modify********/


        protected Team93LMSContext db;

        public CommonController()
        {
            db = new Team93LMSContext();
        }


        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */


        public void UseLMSContext(Team93LMSContext ctx)
        {
            db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the name and the subjects of a department
                var queryGetDepartments = from departments in db.Departments
                                          select new
                                          {
                                              name = departments.Name,
                                              subject = departments.Subject
                                          };

                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetDepartments.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Makes a query to return all the courses name and number offered by all the departments in the database
                var queryGetCatalog = from departments in db.Departments
                                      select new
                                      {
                                          subject = departments.Subject,
                                          dname = departments.Name,
                                          courses = from courses in db.Courses
                                                    where courses.Subject == departments.Subject
                                                    select new
                                                    {
                                                        number = courses.Num,
                                                        cname = courses.Name
                                                    }
                                      };

                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetCatalog.ToArray());
            }
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Makes a query to return all the offerings of a class in a particular course
                var queryGetClassOfferings = from course in db.Courses
                                             join classes in db.Classes on course.CourseId equals classes.CourseId
                                             join prof in db.Professors on classes.UId equals prof.UId
                                             where course.Subject == subject && course.Num == number
                                             select new
                                             {
                                                 season = classes.Semester,
                                                 year = classes.Year,
                                                 location = classes.Loc,
                                                 start = classes.Start,
                                                 end = classes.End,
                                                 fname = prof.FirstName,
                                                 lname = prof.LastName
                                             };

                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetClassOfferings.ToArray());
            }
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the contents of an assignment in a class based on its name
                var queryGetAssignmentContent = from classes in db.Classes
                                                join courses in db.Courses on classes.CourseId equals courses.CourseId
                                                join assignCategory in db.AssignmentCategories on classes.ClassId equals assignCategory.ClassId
                                                join assignment in db.Assignments on assignCategory.CatId equals assignment.CatId
                                                where courses.Subject == subject && courses.Num == num && classes.Semester == season && classes.Year == year && assignCategory.Name == category && assignment.Name == asgname
                                                select assignment.Contents;


                //Returns the first assignment content as a string for the content
                return Content(queryGetAssignmentContent.First().ToString());
            }
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the contents of a submission of a student based on the assignment in a class
                var queryGetSubmission = from c in db.Classes
                                         join courses in db.Courses on c.CourseId equals courses.CourseId
                                         join assignCategory in db.AssignmentCategories on c.ClassId equals assignCategory.ClassId
                                         join assignment in db.Assignments on assignCategory.CatId equals assignment.CatId
                                         join submission in db.Submission on assignment.AssignId equals submission.AssignId
                                         where courses.Subject == subject && courses.Num == num && c.Semester == season && c.Year == year && assignCategory.Name == category && assignment.Name == asgname && submission.UId == uid
                                         select submission.Contents;

                //If there is no submission, then returns an empty string for the content
                if (queryGetSubmission.Count() == 0)
                {
                    return Content("");
                }

                //If there is a submission, then returns the first submission as a string for the content
                return Content(queryGetSubmission.First().ToString());
            }
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {

                //Sees if the uid entered is of a student's
                var queryStudent = from students in db.Students
                                   join departments in db.Departments on students.Subject equals departments.Subject
                                   where students.UId == uid
                                   select new
                                   {
                                       fname = students.FirstName,
                                       lname = students.LastName,
                                       uid = students.UId,
                                       department = departments.Name
                                   };

                //Sees if the uid entered is of a professor's
                var queryProf = from professors in db.Professors
                                join departments in db.Departments on professors.Subject
                                equals departments.Subject
                                where professors.UId == uid
                                select new
                                {
                                    fname = professors.FirstName,
                                    lname = professors.LastName,
                                    uid = professors.UId,
                                    department = departments.Name
                                };

                //Sees if the uid entered is of a administrator's
                var queryAdmin = from administrator in db.Administrators
                                 where administrator.UId == uid
                                 select new
                                 {
                                     fname = administrator.FirstName,
                                     lname = administrator.LastName,
                                     uid = administrator.UId
                                 };


                //Returns the first query for the student if the query for the student returned something
                if (queryStudent.Count() != 0)
                {

                    return Json(queryStudent.First());
                }
                //Returns the first query for the professor if the query for the professor returned something
                else if (queryProf.Count() != 0)
                {
                    return Json(queryProf.First());
                }
                //Returns the first query for the administrator if the query for the administrator returned something
                else if (queryAdmin.Count() != 0)
                {
                    return Json(queryAdmin.First());

                }
                //If user doesn't exist then returns false
                {
                    return Json(new { success = false });
                }
            }
        }

        /*******End code to modify********/

    }
}