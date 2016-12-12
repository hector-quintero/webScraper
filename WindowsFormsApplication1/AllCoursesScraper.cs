using System;
using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using CourseScraper.Hierarchy;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using System.Reflection;
using System.Reflection.Emit;

namespace CourseScraper
{
    class AllCoursesScraper
    {
        private static readonly String mainPage = "https://www4.uwm.edu/academics/undergraduatecatalog.cfm";
        private static readonly String uwmName = "University of Wisconsin - Milwaukee";
        private static readonly String collegesTagging = "Schools and Colleges";
        private static readonly String departmentTagging = "Courses: ";
        private static readonly String uwmSiteStub = "https://www4.uwm.edu";
        private static readonly String fileName = @"C:\Users\Public\TestFolder\UWM Courses JSON.txt";

        public University scrape()
        {
            //ScrapingBrowser browser = new ScrapingBrowser();
            University uwm = new University(uwmName);
            scrapeUniversity(uwm, mainPage);


            //DataContractJsonSerializer serialized = new DataContractJsonSerializer(typeof(Course));

            
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("{");
                file.WriteLine("  \"courses\": {");

                for(int i = 0; i < uwm.collegeList.Count; ++i)
                {
                    for(int j = 0; j < uwm.collegeList.ElementAt(i).departmentListing.Count; ++j)
                    {
                        for (int k = 0; k < uwm.collegeList.ElementAt(i).departmentListing.ElementAt(j).courseListing.Count; ++k) 
                        {
                            file.Write("  " + uwm.collegeList.ElementAt(i).departmentListing.ElementAt(j).courseListing.ElementAt(k).Value.ToJSON());
                            if(i < uwm.collegeList.Count - 1
                                || j < uwm.collegeList.ElementAt(i).departmentListing.Count - 1
                                || k < uwm.collegeList.ElementAt(i).departmentListing.ElementAt(j).courseListing.Count - 1)
                            {
                                file.Write(",\n");
                            }
                            else
                            {
                                file.Write("\n");
                            }
                        }
                    }
                }

                //foreach (College college in uwm.collegeList)
                //{
                //    foreach (Department department in college.departmentListing)
                //    {
                //        foreach (Course course in department.courseListing)
                //        {
                //            //FieldAttributes attributes = new FieldAttributes();

                //            //typeBuilder.DefineField(course.Key, typeof(String), FieldAttributes.Static);
                //            //serialized.WriteObject(file, course);
                //            file.WriteLine("  "+course.ToJSON());
                //        }
                //    }
                //}
                file.WriteLine("  }\n}");

                return uwm;
            }
        }

        private void scrapeUniversity(University university, String universityPage)
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri(universityPage));

            //List<College> colleges = new List<College>();
            Dictionary<String, College> colleges = new Dictionary<string, College>();

            var listsWithinList = homePage.Html.CssSelect("li > ul");

            foreach(var list in listsWithinList)
            {
                if(list.ParentNode.OuterHtml.IndexOf(collegesTagging) > -1)
                {
                    foreach (var element in list.CssSelect("a"))
                    {
                        colleges[element.Attributes["href"].Value.ToString()] = new College(element.InnerText.ToString().CleanInnerText());
                    }
                }
            }

            foreach(String url in colleges.Keys)
            {
                if (!colleges.ElementAt(1).Key.Equals(url) && !colleges.ElementAt(4).Key.Equals(url) && !colleges.ElementAt(8).Key.Equals(url)) continue;
                scrapeCollege(colleges[url], uwmSiteStub + url);
                university.collegeList.Add(colleges[url]);
            }
        }

        private void scrapeCollege(College college, String collegePage)
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri(collegePage));

            Dictionary<String, Department> departments = new Dictionary<string, Department>();

            var linkList = homePage.Html.CssSelect("a");

            foreach (var linkElement in linkList)
            {
                if (linkElement.OuterHtml.IndexOf(departmentTagging) > -1)
                {
                    departments[linkElement.Attributes["href"].Value.ToString()] = new Department(linkElement.InnerText.ToString().CleanInnerText());
                }
            }

            foreach (String url in departments.Keys)
            {
                //if (!departments.ElementAt(1).Key.Equals(url) 
                    //&& ( departments.Count < 33
                    //|| !departments.ElementAt(32).Key.Equals(url) )) continue;
                scrapeDepartment(departments[url], uwmSiteStub + url);
                college.departmentListing.Add(departments[url]);
            }
        }

        private void scrapeDepartment(Department department, String departmentPage)
        {
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage homePage = browser.NavigateToPage(new Uri(departmentPage));
            Dictionary<string, Course> courseListing = new Dictionary<string, Course>();
            String departmentName = homePage.Html.CssSelect("h2").First().InnerHtml;
            var allDlTags = homePage.Html.CssSelect("dl");  //all div elements
            foreach (var singleDl in allDlTags)
            {
                if (singleDl.InnerHtml.IndexOf("strong") > -1)
                {
                    foreach (var strongTag in singleDl.CssSelect("strong"))
                    {
                        if(Char.IsDigit(strongTag.InnerText.Substring(0,1).ToCharArray()[0]))
                        {
                            Course course = new Course(strongTag.InnerText, department, strongTag.NextSibling.OuterHtml);
                            if(!courseListing.ContainsKey(course.Key))
                                courseListing.Add(course.Key, course);
                        }
                    }
                }
            }
            department.courseListing = courseListing;
        }


        public AssemblyBuilder GetAssemblyBuilder(string assemblyName)
        {
            AssemblyName aname = new AssemblyName(assemblyName);
            AppDomain currentDomain = AppDomain.CurrentDomain; // Thread.GetDomain();
            AssemblyBuilder builder = currentDomain.DefineDynamicAssembly(aname,
                                       AssemblyBuilderAccess.Run);
            return builder;
        }

        public ModuleBuilder GetModule(AssemblyBuilder asmBuilder)
        {
            ModuleBuilder builder = asmBuilder.DefineDynamicModule("EmitMethods",
                                                   "EmitMethods.dll");
            return builder;
        }

        public TypeBuilder GetType(ModuleBuilder modBuilder, string className)
        {
            TypeBuilder builder = modBuilder.DefineType(className, TypeAttributes.Public);
            return builder;
        }
    }


    //String className = "CourseSerialization";
    //AssemblyBuilder assemblyBuilder = GetAssemblyBuilder("Assembly");
    //ModuleBuilder modBuilder = GetModule(assemblyBuilder);
    //TypeBuilder typeBuilder = GetType(modBuilder, className);


    //DataContractJsonSerializer cereal = new DataContractJsonSerializer(typeof(DynamicSkeleton));
    //DynamicSkeleton skel = new DynamicSkeleton();
    //        using (var file =
    //            System.IO.File.Create(fileName))
    //        {
                
    //            FieldBuilder nameField = typeBuilder.DefineField("CS150", typeof(String), FieldAttributes.Public);
    //nameField.SetCustomAttribute(new DataMemberAttribute());
    //            cereal.WriteObject(file, skel);
    //        }
}