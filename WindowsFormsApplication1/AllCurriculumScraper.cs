using System;
using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using CourseScraper.Hierarchy;

using System.Runtime.Serialization.Json;
using CourseScraper.CurriculumScraping;

namespace CourseScraper
{
    class AllCurriculumScraper
    {

        private static readonly String[] curriculumURLs =
        {
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_Biomed.html",
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_CompEng.html",
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_CS.html",
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_CE.html",
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_IE.html",
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_IE.html",
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_M.html",
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_ME.html"
        };

        private static readonly String[] compSciURL =
        {
            "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/D_EAS_CS.html"
        };

        public List<EngineeringCurriculum> scrape()
        {
            List<EngineeringCurriculum> curriculumList = new List<EngineeringCurriculum>();
            foreach(String url in compSciURL)
            {
                curriculumList.Add(scrapeCurriculum(url));
            }
            return curriculumList;
        }


        private EngineeringCurriculum scrapeCurriculum(String curriculumURL)
        {
            CompSciScraper csScraper = new CompSciScraper(curriculumURL);
            return csScraper.scrape();
        }

        static AllCurriculumScraper()
        {

        }
    }
}
