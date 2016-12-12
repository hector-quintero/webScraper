using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseScraper.Hierarchy;
using System.Runtime.Serialization;

namespace CourseScraper.GraphPlanner
{
    [DataContract]
    class CourseNode
    {
        [DataMember]
        private Course course;
        [DataMember]
        private int level;
        [DataMember]
        private List<Course> postReqs = new List<Course>();
        [DataMember]
        private List<Course> preReqs = new List<Course>();

        public bool hasPrereqs()
        {
            return preReqs.Count > 0;
        }

        public CourseNode(Course current)
        {
            course = current;
        }

        public List<Course> Prereqs
        {
            get { return preReqs; }
        }
        public List<Course> Postreqs
        {
            get { return postReqs; }
        }


        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public void addPostreq(Course inCourse)
        {
            foreach(Course c in postReqs)
            {
                if (inCourse.CompareTo(c) == 0)
                {
                    return;
                }
            }
            postReqs.Add(inCourse);
        }

        private static string[] precedence = { "COMPSCI", "MATH232", "MATH231", "MATH116" };
        public void decidePrereqs(University uwm)
        {
            Course current = null;
            foreach(string preq in course.Prereqs)
            {
                if (preq.Contains("COMPSCI361") && preq.Contains("COMPSCI535")
                && preq.Contains("COMPSCI458") && preq.Contains("COMPSCI537"))
                {
                    preReqs.Add(uwm.getCourse(removeRequisite("COMPSCI361")));
                    preReqs.Add(uwm.getCourse(removeRequisite("COMPSCI458")));
                    preReqs.Add(uwm.getCourse(removeRequisite("COMPSCI535")));
                    preReqs.Add(uwm.getCourse(removeRequisite("COMPSCI537")));
                    break;
                }
                if (preq.IndexOf("|") > -1)
                {
                    for(int i = 0; i < precedence.Length; ++i)
                    {
                        foreach (string key in preq.Split('|'))
                        {
                            if(key.IndexOf(precedence[i]) > -1)
                            {
                                current = uwm.getCourse(removeRequisite(key));
                                if (current != null)
                                {
                                    preReqs.Add(current);
                                    i = precedence.Length;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    current = uwm.getCourse(removeRequisite(preq));
                    if(current != null)
                        preReqs.Add(current);
                }
            }

            int x = 5;
        }



        private string removeRequisite(string preq)
        {
            return preq.IndexOf("(") > -1 ? preq.Substring(0, preq.IndexOf("(")) : preq ;
        }

        public Course Course
        {
            get { return course; }
        }
    }
}
