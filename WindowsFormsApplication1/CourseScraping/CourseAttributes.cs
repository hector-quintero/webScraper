using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CourseScraper.Hierarchy
{
    [DataContract]
    class CourseAttributes
    {

        Course internalCourse;
        [DataMember] private String title;
        [DataMember] private String credits;
        [DataMember] private List<String> prereqList = new List<String>();

        public CourseAttributes()
        {

        }

        public CourseAttributes(Course course)
        {
            internalCourse = course;
            title = course.Title;
            credits = course.Credits;
        }


    }
}
