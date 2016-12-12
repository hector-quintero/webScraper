using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScraper.Hierarchy
{
    public static class ExtensionHelper
    {

        public static String HectorsCrazyThing(this string baladsf)
        {
            return baladsf.Substring(2);
        }

        public static String toString(this RequisiteType req)
        {
            switch (req)
            {
                case RequisiteType.C: return "Corequisite";
                case RequisiteType.ER: return "Enrollment Restriction";
                case RequisiteType.NC: return "Nonrepeatable Corequisite";
                case RequisiteType.NP: return "Nonrepeatable Prerequisite";
                case RequisiteType.P: return "Prerequisite";
                case RequisiteType.R: return "Recommended Course";
                case RequisiteType.NA: return "Not Applicable";
            }
            return "";
        }

        public static String toStringAbbreviation(this RequisiteType req)
        {
            switch (req)
            {
                case RequisiteType.C: return "C";
                case RequisiteType.ER: return "ER";
                case RequisiteType.NC: return "NC";
                case RequisiteType.NP: return "NP";
                case RequisiteType.P: return "P";
                case RequisiteType.R: return "R";
                case RequisiteType.NA: return "NA";
            }
            return "";
        }

        public static RequisiteType toRequisiteType(String req)
        {
            foreach(RequisiteType curReq in Enum.GetValues(typeof(RequisiteType)))
            {
                if(req.Equals(curReq.toStringAbbreviation()))
                {
                    return curReq;
                }
            }
            return RequisiteType.NA;
        }
    }
}
