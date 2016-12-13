using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CourseScraper.Hierarchy;
using CourseScraper.CurriculumScraping;
using CourseScraper.GraphPlanner;
using System.IO;

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

        public static string CURRENT_PATH = Directory.GetCurrentDirectory() + @"\OUTPUT_FOLDER";
        static void other()
        {
            MessageBox.Show("Now scraping all UWM courses.\nThis may take 10 to 15 minutes.");
            AllCoursesScraper coursesScraper = new AllCoursesScraper();
            University uwm = coursesScraper.scrape();
            MessageBox.Show("Now FINISHED scraping all UWM courses.\nNext is scraping curriculums. This will be quick.");
            AllCurriculumScraper curriculumsScraper = new AllCurriculumScraper();
            List<EngineeringCurriculum> curriculumList = curriculumsScraper.scrape();
            MessageBox.Show("Now FINISHED scraping curriculums.\nNext is scraping GER courses. This will also be quick.");
            AllGerDistributionScraper gerScraper = new AllGerDistributionScraper();
            Dictionary<string, List<string>> gerDictionary = gerScraper.scrape();
            MessageBox.Show("Now FINISHED scraping GER courses.\nNext is building default curriculum plans. This will be quick.");
            GraphEngine engine = new GraphEngine();
            engine.buildPlan(uwm, curriculumList, gerDictionary);
            MessageBox.Show("Course Scraper & Default Course Planning Complete!");
            MessageBox.Show("All output files can be found in the directory:\n"+CURRENT_PATH);
        }
    }
}
