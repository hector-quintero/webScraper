using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace CourseScraper.Hierarchy
{
    [DataContract]
    class Course : IComparable
    {
        private static readonly int COURSE_NUM_LEN = 3;

        [DataMember] private String key;
        private String title;
        private String credits;
        private List<String> prereqList = new List<String>();

        [DataMember] CourseAttributes attributes;

        private int number;
        private Department department;
        private int formerNumber;
        private String designation;
        private String description;
        private String prereqDescription;

        public int getCredits()
        {
            if(credits.IndexOf("-") < 0)
            {
                return int.Parse(credits);
            }
            return int.Parse(credits.Substring(credits.IndexOf("-")));
        }

        public bool hasPreReqs()
        {
            return prereqList.Count > 0;
        }

        public List<String> Prereqs
        {
            get { return prereqList; }
            set {  }
        }

        public Course()
        {

        }

        public Course(String designation, Department department, String description)
        {
            description = ExtensionHelper.HectorsCrazyThing(description);
                
                description.HectorsCrazyThing();
            
            this.designation = designation;
            this.department = department;
            this.description = description;
            parseDesignation();
            parseDescription();
            attributes = new CourseAttributes(this);
            String test = ToJSON();
        }

        private static readonly Regex numberRegex = new Regex(@"\d{"+ COURSE_NUM_LEN + @"} ");
        private static readonly Regex formerNumberRegex = new Regex(@"\d{" + COURSE_NUM_LEN + @"} \(\d{" + COURSE_NUM_LEN + @"}\)");
        private static readonly Regex creditRegex = new Regex(@"\..+\d cr\.");
        private static readonly Regex numberRegexAlt1 = new Regex(@"\d{" + 1 + @"} ");
        private static readonly Regex numberRegexAlt2 = new Regex(@"\d{" + 2 + @"} ");
        private void parseDesignation()
        {
            String numberString;
            String formerNumString;
            String titleString;
            String creditString;
            //String departmentString;

            int titleStartIndex;
            int titleEndIndex;

            numberString = numberRegex.Match(designation).ToString();
            creditString = creditRegex.Match(designation).ToString();
            formerNumString = formerNumberRegex.Match(designation).ToString();
            //departmentString = departmentRegex.Match(department).ToString();
            //departmentString = department.Substring(0, department.IndexOf(departmentString) - 1).Trim();

            if(numberString.Length == 0)
            {
                numberString = numberRegexAlt2.Match(designation).ToString();
                if(numberString.Length == 0)
                {
                    numberString = numberRegexAlt1.Match(designation).ToString();
                }
            }

            if (formerNumString.Length > 0)
            {
                formerNumString = formerNumString.Substring(formerNumString.IndexOf("(") + "(".Length, COURSE_NUM_LEN);
                formerNumber = int.Parse(formerNumString);
                titleStartIndex = designation.IndexOf(formerNumString) + formerNumString.Length + ")".Length;
            }
            else { titleStartIndex = designation.IndexOf(numberString) + numberString.Length; }

            if (creditString.Length > 0)
            {
                titleEndIndex = designation.IndexOf(creditString);
                creditString = creditString.Split('.')[1];
                creditString = creditString.Split('c')[0].Trim();
            }
            else { titleEndIndex = designation.Length - titleStartIndex; }

            number = int.Parse(numberString);
            credits = creditString;
            titleString = designation.Substring(titleStartIndex, titleEndIndex - titleStartIndex).Trim();
            title = titleString;

            key = department.ShortName + number;

            //attributeList.Add(credits);
            //attributeList.Add(prereqList);
            //attributeList.Add(title);

            //Console.WriteLine(titleString);
        }

        private static readonly String prereqTagging = "Prereq: ";
        private static readonly String noneTagging = "none.";
        private static readonly char pipeSeparator = '|';
        private static readonly char reqDivider = ';';
        private void parseDescription()
        {
            String prereqs = "";
            if (description.IndexOf(pipeSeparator) > -1
                && description.IndexOf(pipeSeparator) < description.IndexOf(prereqTagging))
            {
                prereqDescription = prereqs = description.Split(pipeSeparator)[1].Trim();
                if(prereqs.IndexOf(prereqTagging + noneTagging) == -1)
                {
                    prereqs = prereqs.Substring(prereqs.IndexOf(prereqTagging) + prereqTagging.Length);
                    //prereqs = preProcessPrereqs(prereqs);
                    //foreach (String preq in prereqs.Split(reqDivider))
                    //{
                        parsePreq(prereqs);
                    //}
                }
            }
            
        }

        private String preProcessPrereqs(String prereqs)
        {
            prereqs = prereqs.Replace(";", "&&");
            prereqs = prereqs.Replace("; & ", ", &");
            return prereqs.Replace("; or", ", or");
        }

        private static readonly String[] removeList;
        private static readonly String[] classStandings = { "so st", "jr st", "sr st" };
        private static readonly String[] standingMorphisms = { ",", " & ", " or " };
        private static readonly String[] otherConsiderations = 
        {
            "or grad st",
            "or cons instr",
            "or cons dept chair",
            "or admis to Inter-Arts/IAT prog",
            "or pre-Inter-Arts/IAT prog",
            "see Schedule of Classes."
        };

        static Course()
        {
            int otherReqsLength = classStandings.Length * standingMorphisms.Length + otherConsiderations.Length;
            removeList = new String[otherReqsLength];
            for(int i = 0; i < otherConsiderations.Length; ++i)
            {
                removeList[i] = otherConsiderations[i];
            }
            for (int i = 0; i < classStandings.Length; ++i)
            {
                for(int j = 0; j < standingMorphisms.Length; ++j)
                {
                    removeList[otherConsiderations.Length + i * classStandings.Length + j] 
                        = classStandings[i] + standingMorphisms[j];
                }
            }

        }

        private static readonly Regex requisiteTypeRegex = new Regex(@"\([^\)]{1,2}\)");
        private static readonly Regex prereqNumberRegex = new Regex(@"\d{" + COURSE_NUM_LEN + @"}\(");
        private static readonly Regex prereqDeptRegex = new Regex(@"\s[A-Z]+[a-z]*[A-Z]*[a-z]*\s+\d{" + COURSE_NUM_LEN + @"}\(");
        private static readonly Regex multipleAndRegex = new Regex(@" & \d{" + COURSE_NUM_LEN + @"}\(");
        private static readonly Regex multipleOrRegex = new Regex(@" or \d{" + COURSE_NUM_LEN + @"}\(");
        private static readonly Regex andDividerRegex = new Regex(@";");
        private static readonly Regex formerNumberPreqRegex = new Regex(@"\(\d{" + COURSE_NUM_LEN + @"}\)");

        private static readonly String pipeDivider = "|";

        public static StringBuilder errorList = new StringBuilder();
        public static StringBuilder departmentErrors = new StringBuilder();
        private void parsePreq(String preq)
        {
            preq = fixCommaAmpers(preq);
            preq = preq.Replace("&", ";");
            preq = preq.Replace(" (", "(");
            preq = " " + preq;

            foreach(String removable in removeList)
            {
                while(preq.IndexOf(removable) > -1)
                {
                    preq = preq.Remove(preq.IndexOf(removable), removable.Length);
                }
            }
            
            String dept;
            int num;
            String reqType;

            Match formerNumberMatch = formerNumberPreqRegex.Match(preq);
            while (formerNumberMatch.Success)
            {
                preq = preq.Replace(formerNumberMatch.ToString(), "");
                formerNumberMatch = formerNumberMatch.NextMatch();
            }

            Match numMatch = prereqNumberRegex.Match(preq);
            Match reqTypeMatch = requisiteTypeRegex.Match(preq);
            Match deptMatch = prereqDeptRegex.Match(preq);
            //Match multipleAndMatch = multipleAndRegex.Match(preq);
            //Match multipleOrMatch = multipleOrRegex.Match(preq);

            Match andDividerMatch = andDividerRegex.Match(preq);
            while (andDividerMatch.Success && andDividerMatch.Index < numMatch.Index)
            {
                andDividerMatch = andDividerMatch.NextMatch();
            }

            if (!numMatch.Success && !reqTypeMatch.Success && !deptMatch.Success)
            {
                errorList.Append(preq + Environment.NewLine);
            }

            if(numMatch.Success && reqTypeMatch.Success && !deptMatch.Success)
            {
                departmentErrors.Append(preq + Environment.NewLine);
                return;
            }

            String currentCourseList = "";

            while (numMatch.Success && reqTypeMatch.Success)
            {
                reqType = reqTypeMatch.ToString();
                reqType = reqType.Substring("(".Length, reqType.Length - "()".Length);
                num = int.Parse(numMatch.ToString().Substring(0, COURSE_NUM_LEN));
                dept = deptMatch.ToString();
                dept = dept.Substring(0, dept.Length - COURSE_NUM_LEN - "(".Length);
                dept = expandDepartment(preq, dept, deptMatch, 3);
                dept = expandDepartment(preq, dept, deptMatch, 4);


                CourseKey curKey = new CourseKey(dept, num, ExtensionHelper.toRequisiteType(reqType));
                currentCourseList = currentCourseList + pipeDivider + curKey.ToString();

                numMatch = numMatch.NextMatch();
                reqTypeMatch = reqTypeMatch.NextMatch();

                if (andDividerMatch.Success && andDividerMatch.Index < numMatch.Index)
                {
                    while (andDividerMatch.Success && andDividerMatch.Index < numMatch.Index)
                    {
                        andDividerMatch = andDividerMatch.NextMatch();
                    }
                    prereqList.Add(currentCourseList.Substring(1));
                    currentCourseList = "";
                }

                if (deptMatch.NextMatch().Success && deptMatch.NextMatch().Index < numMatch.Index)
                {
                    deptMatch = deptMatch.NextMatch();
                }
            }

            if(currentCourseList.Length > 0)
            {
                prereqList.Add(currentCourseList.Substring(1));
                currentCourseList = "";

                int x = prereqList.First().Length;
            }
        }

        private static String expandDepartment(String preq, String dept, Match deptMatch, int numCharacters)
        {
            char letterBack = deptMatch.Index >= numCharacters ? Convert.ToChar(preq.Substring(deptMatch.Index - numCharacters, 1)) : ' ';
            if (letterBack > 'A' && letterBack < 'Z')
            {
                bool onlyLetters = true;
                char curChar = ' ';
                for (int i = 1; i < numCharacters && onlyLetters; i++)
                {
                    curChar = Convert.ToChar(preq.Substring(deptMatch.Index - i, 1));
                    if (
                            !((curChar > 'a' && curChar < 'z')
                            || (curChar > 'A' && curChar < 'Z')
                            )
                        )
                    {
                        onlyLetters = false;
                    }
                }
                if (onlyLetters)
                {
                    dept = preq.Substring(deptMatch.Index - numCharacters, dept.Length + numCharacters);
                }
            }
            return dept;
        }

        private static String fixCommaAmpers(String preq)
        {
            if(preq.IndexOf(", &") == -1 && preq.IndexOf(") &") == -1)
            {
                return preq;
            }
            preq = preq.Replace(", &", " &");
            bool fixingAmps = false;
            for(int i = preq.Length - 1; i > 2; --i)
            {
                if( preq.Substring(i, 1).Equals("&")
                    && 
                    (preq.Substring(i - 2, 1).Equals(",") || preq.Substring(i - 2, 1).Equals(")")))
                {
                    fixingAmps = true;
                }
                if(fixingAmps && preq.Substring(i, 1).Equals(","))
                {
                    preq = preq.Substring(0, i) + "&" + preq.Substring(i + 1);
                }
                if(preq.Substring(i,1).Equals(";"))
                {
                    fixingAmps = false;
                }
            }
            return preq;
        }

        public String ToJSON()
        {
            String preqsInJSON = "";
            if(prereqList.Count > 0)
            {
                preqsInJSON += "  \"requirements\":[";
                for(int i = 0; i < prereqList.Count; ++i)
                {
                    preqsInJSON += " \"" + prereqList.ElementAt(i) + "\"";
                    if(i != prereqList.Count - 1)
                    {
                        preqsInJSON += ",";
                    }
                }
                preqsInJSON += "]\n    ";
            }
            return  "  \"" + key.ToString() + "\": {\n" +
                    "    \"credits\": \"" + credits + "\",\n" +
                    "    \"prereq\": {\n" +
                    "    "+ preqsInJSON + "\n"+
                    "    },\n" +
                    "    \"title\": \""+title+"\"\n"+
                    "    }";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Course otherCourse = obj as Course;
            if (otherCourse != null)
                return this.Key.CompareTo(otherCourse.Key);
            else
                throw new ArgumentException("Object is not a Temperature");
        }

        private String Designation
        {
            set { designation = value; }
            get { return designation; }
        }

        public String Key
        {
            set { key = value; }
            get { return key; }
        }
        
        public String Title
        {
            set { title = value; }
            get { return title; }
        }

        public Department Dept
        {
            set { department = value;}
            get { return department; }
        }

        public int Number
        {
            set { number = value; }
            get { return number; }
        }

        public String Credits
        {
            get { return credits; }
        }
    }
}
