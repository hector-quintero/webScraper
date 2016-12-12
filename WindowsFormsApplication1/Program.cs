using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CourseScraper.Hierarchy;
using CourseScraper.CurriculumScraping;
using CourseScraper.GraphPlanner;

namespace CourseScraper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            other();
        }

        static void other()
        {
            AllCoursesScraper coursesScraper = new AllCoursesScraper();
            University uwm = coursesScraper.scrape();

            
            AllCurriculumScraper curriculumsScraper = new AllCurriculumScraper();
            List<EngineeringCurriculum> curriculumList = curriculumsScraper.scrape();

            AllGerDistributionScraper gerScraper = new AllGerDistributionScraper();
            Dictionary<string, List<string>> gerDictionary = gerScraper.scrape();

            GraphEngine engine = new GraphEngine();
            engine.buildPlan(uwm, curriculumList, gerDictionary);
        }
    }
}
