using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace CourseScraper.Hierarchy
{
    [DataContract]
    class Department
    {
        String name;
        [DataMember] String nameAbbr;
        public Dictionary<string, Course> courseListing;

        private static readonly Regex departmentRegex = new Regex(@"\([^\)]*\)");
        public Department(String name)
        {
            this.name = name;
            this.nameAbbr = departmentRegex.Match(name).ToString();
            if(nameAbbr.Length > 0)
            {
                nameAbbr = nameAbbr.Substring("(".Length, nameAbbr.Length - "()".Length);
            }
        }

        public String ShortName
        {
            get { return nameAbbr; }
        }

        public Course getCourse(string key)
        {
            if(courseListing.ContainsKey(key)) return courseListing[key];
            return null;
        }
    }
}
