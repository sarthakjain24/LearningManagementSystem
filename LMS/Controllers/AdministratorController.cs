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
    /// A controller class that represents the administrator
    /// </summary>
    /// <Author>Sarthak Jain, Bryce Fairbanks, Daniel Kopta</Author>
    /// <Class> CS 5530 Spring 2022 </Class>
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the number and the name of the courses where the subject of the course matches the subject passed into the function
                var queryGetCourses = from courses in db.Courses
                                      where courses.Subject == subject
                                      select new { number = courses.Num, name = courses.Name };
                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetCourses.ToArray());
            }
        }





        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets the last name, first name and the uid of the professor that teaches the subject passed into the function
                var queryGetProfessors = from professors in db.Professors
                                         where professors.Subject == subject
                                         select new
                                         {
                                             lname = professors.LastName,
                                             fname = professors.FirstName,
                                             uid = professors.UId
                                         };
                //Returns a JSON where it converts the query to an array that can be parsed
                return Json(queryGetProfessors.ToArray());
            }
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false},
        /// false if the Course already exists.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets all the courseID where's the course name and number matches the name and number passed into the function
                var queryGetCourses = from courses in db.Courses
                                      where courses.Name == name && courses.Num == number
                                      select courses.CourseId;

                //If there already exists a course in the database, then return false
                if (queryGetCourses.ToList().Count != 0)
                {
                    return Json(new { success = false });
                }

                //Creates a Course object 
                Courses newCourse = new Courses();

                //Sets the fields of the Course object based on the paramters
                newCourse.Name = name;
                newCourse.Num = (uint)number;
                newCourse.Subject = subject;

                //Adds the object to the table in the database
                db.Courses.Add(newCourse);

                //Saves the change made in the database
                db.SaveChanges();
            }
            //Indicates a success for successfully creating a course
            return Json(new { success = true });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            //Uses the Team93LMSContext to make a query
            using (Team93LMSContext db = new Team93LMSContext())
            {
                //Gets all the courseID where's the course name and number matches the name and number passed into the function
                var queryGetCourses = from courses in db.Courses
                                      where courses.Subject == subject && courses.Num == number
                                      select courses.CourseId;

                //Sets the courseID to what the query returns
                uint courseID = uint.MaxValue;
                foreach (var queryCourse in queryGetCourses)
                {
                    courseID = queryCourse;
                }

                //If location is null, then returns false 
                if (location == null)
                {
                    return Json(new { success = false });
                }

                // Check if another class occupies the same location during any time 
                // within the start-end range in the same semester, or if there is already
                // a Class offering of the same Course in the same Semester
                var queryGetClasses = from classes in db.Classes
                                      where (classes.Loc == location && classes.Semester == season && classes.Year == year &&
                                            ((classes.Start <= start.TimeOfDay && classes.End >= start.TimeOfDay) || (classes.Start <= end.TimeOfDay && classes.End >= end.TimeOfDay)))
                                            || (classes.Semester == season && classes.Year == year && classes.CourseId == courseID)
                                      select classes.ClassId;

                //If a class already exists, then returns false
                if (queryGetClasses.ToList().Count != 0)
                {
                    return Json(new { success = false });
                }

                //Creates a newClass object
                Classes newClass = new Classes();
                //Sets the fields of the newClass object based on the parameters
                newClass.CourseId = courseID;
                newClass.Semester = season;
                newClass.Year = (uint)year;
                newClass.Loc = location;
                newClass.Start = start.TimeOfDay;
                newClass.End = end.TimeOfDay;
                newClass.UId = instructor;
                //Adds the class to the Classes table in the database
                db.Classes.Add(newClass);
                //Saves the change made in the database
                db.SaveChanges();
            }
            //Indicates a success for successfully creating a class
            return Json(new { success = true });
        }


        /*******End code to modify********/

    }
}