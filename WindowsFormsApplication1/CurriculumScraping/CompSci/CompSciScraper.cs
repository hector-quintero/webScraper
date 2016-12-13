using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using CourseScraper.Hierarchy;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace CourseScraper.CurriculumScraping
{
    class CompSciScraper
    {
        private String csURL;

        public CompSciScraper(String url)
        {
            csURL = url;
        }

        private static readonly String fileName = Program.CURRENT_PATH+@"\CS Curriculum JSON.json";
        public CompSciCurriculum scrape()
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri(csURL));
            CompSciCurriculum csCurriculum = new CompSciCurriculum();

            var pageTables = homePage.Html.CssSelect("table");
            
            foreach (HtmlAgilityPack.HtmlNode table in pageTables)
            {
                if (table.PreviousSibling?.PreviousSibling?.PreviousSibling?.PreviousSibling?.InnerText.IndexOf("Computer Science Major") > -1)
                {
                    scrapeMajorCourses(table, csCurriculum);
                    scrapeTechnicalElectives(csCurriculum);
                }
                else if(table.PreviousSibling?.PreviousSibling?.InnerText.IndexOf("Natural Science Requirement") > -1)
                {
                    scrapeNaturalSciCourses(table, csCurriculum);
                }
                else if (table.PreviousSibling?.PreviousSibling?.InnerText.IndexOf("GER Distribution Requirement") > -1)
                {
                    scrapeGERCourses(table, csCurriculum);
                }
                else if (table.PreviousSibling?.PreviousSibling?.InnerText.IndexOf("Applied Mathematics") > -1)
                {
                    scrapeAppliedMathCourses(table, csCurriculum);
                }
            }

            DataContractJsonSerializer serialized = new DataContractJsonSerializer(typeof(CompSciCurriculum));
            using (System.IO.FileStream file =
            new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                serialized.WriteObject(file, csCurriculum);
            }

            return csCurriculum;
        }

        private static void scrapeMajorCourses(HtmlAgilityPack.HtmlNode table, CompSciCurriculum csCurriculum)
        {
            String department = "";
            int courseNumber = 0;
            int deptCourseDivider = -1;
            foreach (var row in table.CssSelect("tr"))
            {
                if (row.InnerText.IndexOf("(recommended)") == -1)
                {
                    var firstCell = row.CssSelect("td").ElementAt(0);
                    deptCourseDivider = determineDivision(firstCell.InnerText);
                    department = firstCell.InnerText.Substring(0, deptCourseDivider);
                    courseNumber = int.Parse(firstCell.InnerText.Substring(deptCourseDivider));
                    csCurriculum.majorCore.Add(new CourseKey(department, courseNumber));
                }
            }
            csCurriculum.majorCore.requireAllCourses();
        }

        private const string amper = "&";
        private static void scrapeNaturalSciCourses(HtmlAgilityPack.HtmlNode table, CompSciCurriculum csCurriculum)
        {
            string department;
            string firstCourse;
            string secondCourse;
            int deptCourseDivider;
            HtmlAgilityPack.HtmlNode firstCell;
            string[] cNumbers;

            foreach (var row in table.CssSelect("tr"))
            {
                if (row.InnerText.IndexOf("(recommended)") == -1 && row.CssSelect("td").Count() > 1)
                {
                    firstCell = row.CssSelect("td").ElementAt(0);
                    deptCourseDivider = determineDivision(firstCell.InnerText);
                    department = firstCell.InnerText.Substring(0, deptCourseDivider);
                    cNumbers = firstCell.InnerText.Substring(deptCourseDivider).Split('-');
                    firstCourse = new CourseKey(department, int.Parse(cNumbers[0])).ToString();
                    secondCourse = new CourseKey(department, int.Parse(cNumbers[1])).ToString();
                    csCurriculum.naturalSciCore.Add(firstCourse + amper + secondCourse);
                }
            }
            csCurriculum.naturalSciCore.numberRequired = "1";
        }
        
        private static void scrapeGERCourses(HtmlAgilityPack.HtmlNode table, CompSciCurriculum csCurriculum)
        {
            string department;
            int deptCourseDivider;
            HtmlAgilityPack.HtmlNode firstCell;
            HtmlAgilityPack.HtmlNode thirdCell;
            int courseNumber;

            foreach (var row in table.CssSelect("tr"))
            {
                firstCell = row.CssSelect("td").ElementAt(0);
                deptCourseDivider = determineDivision(firstCell.InnerText);
                if (deptCourseDivider > -1)
                {
                    department = firstCell.InnerText.Substring(0, deptCourseDivider);
                    deptCourseDivider = determineDivision(firstCell.InnerText);
                    department = firstCell.InnerText.Substring(0, deptCourseDivider);
                    courseNumber = int.Parse(firstCell.InnerText.Substring(deptCourseDivider));
                    csCurriculum.gerSpecific.Add(new CourseKey(department, courseNumber).ToString());
                }
                else
                {
                    thirdCell = row.CssSelect("td").ElementAt(2);
                    csCurriculum.updateGeneralGER(firstCell.InnerText, int.Parse(thirdCell.InnerText));
                }
            }
            csCurriculum.gerSpecific.requireAllCourses();
        }

        private static void scrapeAppliedMathCourses(HtmlAgilityPack.HtmlNode table, CompSciCurriculum csCurriculum)
        {
            string department;
            string firstCourse;
            int deptCourseDivider;
            int courseNumber;
            HtmlAgilityPack.HtmlNode firstCell;

            foreach (var row in table.CssSelect("tr"))
            {
                if (row.InnerText.IndexOf("(recommended)") == -1)
                {
                    firstCell = row.CssSelect("td").ElementAt(0);
                    deptCourseDivider = determineDivision(firstCell.InnerText);
                    department = firstCell.InnerText.Substring(0, deptCourseDivider);
                    courseNumber = int.Parse(firstCell.InnerText.Substring(deptCourseDivider));
                    firstCourse = new CourseKey(department, courseNumber).ToString();
                    csCurriculum.appliedMathReq.Add(firstCourse);
                }
            }
            csCurriculum.appliedMathReq.numberRequired = "2";
        }

        private const string csCoursesUrl = "https://www4.uwm.edu/academics/undergraduatecatalog.cfm?u=SC/C_262.html";
        private const string csDept = "COMPSCI";
        private static readonly Regex numberRegex = new Regex(@"\d{" + COURSE_NUM_LEN + @"} ");
        private const int COURSE_NUM_LEN = 3;
        private static void scrapeTechnicalElectives(CompSciCurriculum csCurriculum)
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri(csCoursesUrl));
            Match numMatch;
            int courseNum;
            CourseKey key;

            foreach(HtmlAgilityPack.HtmlNode strong in homePage.Html.CssSelect("strong"))
            {
                numMatch = numberRegex.Match(strong.InnerText);
                if(numMatch.Success 
                    && numMatch.Index == 0 
                    && (courseNum = int.Parse(numMatch.Value)) >= csCurriculum.technicalElectives.rangeMin
                    && courseNum <= csCurriculum.technicalElectives.rangeMax
                    && !csCurriculum.majorCore.Contains(key = new CourseKey(csDept, courseNum)))
                {
                    csCurriculum.technicalCore.Add(key);
                }
            }
            csCurriculum.technicalCore.numberRequired = "4";
        }

        private static int determineDivision(String rowInnerText)
        {
            int curSpaceIndex = rowInnerText.IndexOf(" ");
            while(curSpaceIndex > -1 &&
                    !Char.IsDigit(Convert.ToChar(rowInnerText.Substring(curSpaceIndex + 1, 1))))
            {
                curSpaceIndex = rowInnerText.IndexOf(" ", curSpaceIndex + 1);
            }
            return curSpaceIndex;
        }
    }
}
