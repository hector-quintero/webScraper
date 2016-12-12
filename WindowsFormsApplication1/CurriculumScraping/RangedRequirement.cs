using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
namespace CourseScraper.CurriculumScraping
{

    [DataContract]
    class RangedRequirement
    {
        [DataMember]
        public int rangeMin;
        [DataMember]
        public int rangeMax;
        [DataMember]
        public int credits;
        [DataMember]
        public string department;
    }
}
