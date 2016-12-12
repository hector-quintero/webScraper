using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CourseScraper.Hierarchy
{
    [DataContract]
    class CourseKey
    {

        [DataMember] private String department;
        [DataMember] private int number;
        private RequisiteType requisite;

        public CourseKey(String department, int number, RequisiteType requisite = RequisiteType.NA)
        {
            this.department = department.ToUpper().Trim();
            this.number = number;
            this.requisite = requisite;
        }

        public static CourseKey ToCourseKey(String courseKey)
        {
            string num = courseKey.Substring(courseKey.LastIndexOf(" "));
            string requisite;
            string dept;

            if (num.IndexOf("(") > -1)
            {
                requisite = courseKey.Substring(courseKey.IndexOf("(")+1);
                requisite = requisite.Substring(0, requisite.IndexOf(")"));
                num = num.Substring(0, num.IndexOf("("));
            }
            else
            {
                requisite = RequisiteType.NA.toStringAbbreviation();
            }

            num = num.Trim();
            dept = courseKey.Substring(0, courseKey.LastIndexOf(" ") - 1);
            return new CourseKey(dept, int.Parse(num),  ExtensionHelper.toRequisiteType(requisite));
        }

        override
        public string ToString()
        {
            return department + number + 
                (requisite == RequisiteType.NA ? "" : "(" + requisite.toStringAbbreviation() + ")");
        }

    }
}
