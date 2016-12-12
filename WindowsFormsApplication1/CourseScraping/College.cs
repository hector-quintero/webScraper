using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScraper.Hierarchy
{
    class College
    {
        String name;
        public List<Department> departmentListing = new List<Department>();

        public College(String name)
        {
            this.name = name;
        }

        public Course getCourse(string key)
        {
            Course ret = null;
            foreach (Department dep in departmentListing)
            {
                ret = dep.getCourse(key);
                if (ret != null) break;
            }
            return ret;
        }
    }
}
