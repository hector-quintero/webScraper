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
    class EngineeringCurriculum
    {
        [DataMember]
        public Dictionary<String, int> gerGeneral = new Dictionary<string, int>();
        [DataMember]
        public RequirementGroup gerSpecific = new RequirementGroup();

        [DataMember]
        public RequirementGroup majorCore = new RequirementGroup();

        //public HashSet<CourseKey> majorCoreReq = new HashSet<CourseKey>();
        [DataMember]
        public RequirementGroup mathCore = new RequirementGroup();
        //public HashSet<String> mathCoreReq = new HashSet<String>();
        [DataMember]
        public RequirementGroup naturalSciCore = new RequirementGroup();
        //public HashSet<String> naturalSciCoreReq = new HashSet<String>();
        //[DataMember]
        public RequirementGroup miscCoreReq = new RequirementGroup();

        public void updateGeneralGER(String name, int credit)
        {
            if(name.ToLower().IndexOf("social") > -1 && name.ToLower().IndexOf("science") > -1)
            {
                name = "Social Science";
            }
            if(gerGeneral.ContainsKey(name))
            {
                gerGeneral[name] = credit;
            }
            else
            {
                gerGeneral.Add(name, credit);
            }
        }
    }
}
