using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using CourseScraper.Hierarchy;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

namespace CourseScraper
{
    class AllGerDistributionScraper
    {
        public delegate List<string> flexScraper(string url);
        private flexScraper gerNaturalScience;
        private flexScraper gerArt;
        private flexScraper gerSocialScience;
        private flexScraper gerHumanities;

        private static readonly Dictionary<flexScraper, string> gerURLS = new Dictionary<flexScraper, string>();

        public AllGerDistributionScraper()
        {
            gerURLS.Add(gerNaturalScience = gerNaturalScienceScrape, "https://www4.uwm.edu/academics/ger-naturalsciences.cfm");
            gerURLS.Add(gerArt = gerArtScrape, "https://www4.uwm.edu/academics/ger-arts.cfm");
            gerURLS.Add(gerSocialScience = gerSocialScienceScrape, "https://www4.uwm.edu/academics/ger-socialsciences.cfm");
            gerURLS.Add(gerHumanities = gerHumanitiesScrape, "https://www4.uwm.edu/academics/ger-humanities.cfm");
        }


        public List<string> gerNaturalScienceScrape(string url) { return genericGerScraper(url); }
        public List<string> gerArtScrape(string url) { return genericGerScraper(url); }
        public List<string> gerHumanitiesScrape(string url) { return genericGerScraper(url); }
        public List<string> gerSocialScienceScrape(string url) { return genericGerScraper(url); }


        private const int courseNumberCell = 4;
        // Create a method for a delegate.
        public List<string> genericGerScraper(string url)
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri(url));
            List<string> list = new List<string>();

            IEnumerable<HtmlAgilityPack.HtmlNode> pageTables = homePage.Html.CssSelect("table");
            HtmlAgilityPack.HtmlNode tableToScrape = null;
            foreach (HtmlAgilityPack.HtmlNode table in pageTables)
            {
                if(table.GetAttributeValue("summary").IndexOf("GER") > -1)
                {
                    tableToScrape = table;
                }
            }

            Department curDept = null;
            int curNumber = 0;
            foreach(HtmlAgilityPack.HtmlNode row in tableToScrape.CssSelect("tr"))
            {
                if(row.CssSelect("td").Count() == 1)
                {
                    curDept = new Department(row.InnerText);
                }
                else
                {
                    curNumber = int.Parse(row.CssSelect("td").ElementAt(courseNumberCell).InnerText);
                    list.Add(new CourseKey(curDept.ShortName, curNumber).ToString());
                }
            }

            return list;
        }

        private const string fileName = @"C:\Users\Public\TestFolder\Ger Distributions JSON.txt";
        public Dictionary<string, List<string>> scrape()
        {
            Dictionary<string, List<string>> gerListings = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<flexScraper, string> pair in gerURLS)
            {
                gerListings.Add(pair.Key.Method.Name.ToString(), pair.Key(pair.Value));
            }

            DataContractJsonSerializer serialized = new DataContractJsonSerializer(gerListings.GetType());
            using (System.IO.FileStream file =
            new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                serialized.WriteObject(file, gerListings);
            }

            return gerListings;
        }
        
    }
}
