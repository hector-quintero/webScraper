using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseScraper.Hierarchy;
using System.Runtime.Serialization;

namespace CourseScraper.CurriculumScraping
{
    [DataContract]
    class RequirementGroup
    {
        [DataMember]
        public string numberRequired = "0";

        public enum LogicalConnection { AND, OR };
        //[DataMember]
        //public LogicalConnection logicalConnection;
        //[DataMember]
        //private HashSet<CourseKey> courseList = new HashSet<CourseKey>();

        [DataMember]
        private HashSet<String> courseListing = new HashSet<string>();

        private const string allRequired = "ALL";
        public void requireAllCourses()
        {
            numberRequired = allRequired;
        }

        public void Add(CourseKey key)
        {
            //courseList.Add(key);
            courseListing.Add(key.ToString());
        }

        public bool Contains(CourseKey key)
        {
            bool ret = courseListing.Contains(key.ToString());
            return courseListing.Contains(key.ToString());
        }

        public void Add(String key)
        {
            courseListing.Add(key);
            //courseList.Add(CourseKey.ToCourseKey(key));
        }

        private HashSet<String> getAllCoursesRequired()
        {
            return courseListing;
        }

        

        private static string[] precedence = { "COMPSCI422", "COMPSCI423", "COMPSCI469",
                                                "COMPSCI552", "COMPSCI423", "COMPSCI469",
                                                "COMPSCI", "MATH231", "MATH232",
                                                "MATH233", "MATH234", "MATH116" };
        //private static string[] precedence = { "dlksjafl;ajksdf" };
        public HashSet<String> getDefaultCourses()
        {
            
            if (numberRequired.Equals("ALL"))
            {
                return getAllCoursesRequired();
            }
            int numReq = int.Parse(numberRequired);
            int numAdded = 0;
            HashSet<String> defaultListing = new HashSet<string>();
            foreach(string prec in precedence)
            {
                for (int i = 0; i < courseListing.Count; ++i)
                {
                    if(courseListing.ElementAt(i).IndexOf(prec) > -1)
                    {
                        numAdded++;
                        foreach (string course in courseListing.ElementAt(i).Split('&'))
                        {
                            defaultListing.Add(course);
                        }
                        if (numAdded >= numReq) return defaultListing;
                    }
                }
            }

            bool contained = false;
            foreach(string course in courseListing)
            {
                foreach(string defaultCourse in defaultListing)
                {
                    if(course.IndexOf(defaultCourse, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        contained = true;
                    }
                }
                if(contained == false)
                {
                    numAdded++;
                    foreach (string allCourse in course.Split('&'))
                    {
                        defaultListing.Add(allCourse);
                    }
                    if (numAdded >= numReq) return defaultListing;
                }
                contained = false;
            }

            return defaultListing;
        }

    }
}
