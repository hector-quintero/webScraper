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
    class CompSciCurriculum : EngineeringCurriculum
    {
        [DataMember]
        public RequirementGroup appliedMathReq = new RequirementGroup();
        //[DataMember]
        public RangedRequirement technicalElectives = new RangedRequirement();
        [DataMember]
        public RequirementGroup technicalCore = new RequirementGroup();

        public CompSciCurriculum()
        {
            initMathReq();
            initTechnicalElectives();
            initOtherRequirements();
        }

        private static readonly int[] mathReqs = { 221, 222, 231, 232 };
        private const string mathDept = "MATH";
        private const string amper = "&";
        private readonly static String mathOred = mathDept + mathReqs[0]
                                    + "," + mathDept + mathReqs[1]
                                    + "|" + mathDept + mathReqs[2]
                                    + "," + mathDept + mathReqs[3];
        private void initMathReq()
        {
            //mathCoreReq.Add(mathOred);
            mathCore.Add(mathDept + mathReqs[0] + amper + mathDept + mathReqs[1]);
            mathCore.Add(mathDept + mathReqs[2] + amper + mathDept + mathReqs[3]);
            mathCore.numberRequired = "1";
        }

        private const int technicalRangeMin = 400;
        private const int technicalRangeMax = 699;
        private const int technicalCredits = 12;
        private const string technicalDepartment = "COMPSCI";
        private void initTechnicalElectives()
        {
            technicalElectives.rangeMin = technicalRangeMin;
            technicalElectives.rangeMax = technicalRangeMax;
            technicalElectives.credits = technicalCredits;
            technicalElectives.department = technicalDepartment;
        }

        private void initOtherRequirements()
        {
            gerGeneral.Add("Natural Science", 3);
        }

        public RequirementGroup getRemainingDefault()
        {
            RequirementGroup remaining = new RequirementGroup();
            remaining.Add(new CourseKey("PSYCH", 101));
            remaining.Add(new CourseKey("LINGUIS", 100));
            remaining.Add(new CourseKey("ART", 101));
            remaining.Add(new CourseKey("GEO SCI", 100));
            remaining.numberRequired = "ALL";
            return remaining;
        }
    }
}
