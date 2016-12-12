using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScraper.Hierarchy
{
    class University
    {
        String name;
        public List<College> collegeList = new List<College>();

        public University(String name)
        {
            this.name = name;
        }

        public Course getCourse(string key)
        {
            Course ret = null;
            foreach(College coll in collegeList)
            {
                ret = coll.getCourse(key);
                if (ret != null) break;
            }
            return ret;
        }
    }
}
