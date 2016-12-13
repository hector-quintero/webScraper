using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CourseScraper.Hierarchy;
using CourseScraper.CurriculumScraping;
using System.Runtime.Serialization.Json;

namespace CourseScraper.GraphPlanner
{
    class GraphEngine
    {
        private int completePlan(University uwm, SortedSet<string> sortedKeys, SortedDictionary<Course, CourseNode> plan)
        {
            int absoluteMaxLevel = 0;
            Course currentCourse = null;
            foreach (string key in sortedKeys)
            {
                // get pre-req info for course
                // create a CourseNode object for course with pre and post info 
                if ((currentCourse = uwm.getCourse(key)) != null)
                {
                    //courseList.Add(currentCourse);
                    //plan.ElementAt(0).Add(currentCourse, new CourseNode(currentCourse));
                    plan.Add(currentCourse, new CourseNode(currentCourse));
                }
            }

            foreach (CourseNode node in plan.Values)
            {
                node.decidePrereqs(uwm);
                if (node.hasPrereqs())
                {
                    int maxLevel = 0;
                    foreach (Course course in node.Prereqs)
                    {
                        if (plan.ContainsKey(course))
                            maxLevel = Math.Max(maxLevel, plan[course].Level);
                    }
                    node.Level = maxLevel + 1;
                    absoluteMaxLevel = Math.Max(node.Level, absoluteMaxLevel);
                }
            }
            foreach (CourseNode node in plan.Values)
            {
                foreach (Course preq in node.Prereqs)
                {
                    if (plan.ContainsKey(preq))
                        plan[preq].addPostreq(node.Course);
                }
            }

            return absoluteMaxLevel;
        }

        private void completeRealPlan(SortedDictionary<Course, CourseNode> plan, List<List<CourseNode>> realPlan)
        {
            foreach(CourseNode node in plan.Values)
            {
                if (node.Prereqs.Count <= 0) continue;
                foreach(List<CourseNode> innerList in realPlan)
                {
                    foreach(CourseNode realNode in innerList)
                    {
                        if(node.Prereqs.Contains(realNode.Course) && node.Level <= realNode.Level)
                        {
                            node.Level = realNode.Level + 1;
                            realNode.addPostreq(node.Course);
                            foreach (Course course in node.Postreqs)
                            {
                                plan[course].Level = node.Level + 1;
                            }
                        }
                    }
                }
            }
            foreach (CourseNode node in plan.Values)
            {
                if (node.Course.Key.IndexOf("COMPSCI395") > -1)
                {
                    node.Level = 4;
                }
                if (realPlan.Count <= node.Level)
                {
                    int realCount = realPlan.Count;
                    for(int i = realCount; i <= node.Level; ++i)
                    {
                        realPlan.Add(new List<CourseNode>());
                    }
                }
                
                realPlan.ElementAt(node.Level).Add(node);
            }
        }

        public void buildPlan(University uwm, List<EngineeringCurriculum> curriculumList, Dictionary<string, List<string>> gerListing)
        {
            int x = 0;
            int currentMaxLevel = 0;
            SortedDictionary<Course, CourseNode> plan = new SortedDictionary<Course, CourseNode>();

            foreach(EngineeringCurriculum curriculum in curriculumList)
            {
                if(curriculum is CompSciCurriculum)
                {
                    CompSciCurriculum csCurriculum = (CompSciCurriculum)curriculum;
                    
                    //List<Course> courseList = new List<Course>();
                    SortedSet<string> sortedKeys = new SortedSet<string>();
                    List<List<CourseNode>> realPlan = new List<List<CourseNode>>();

                    HashSet<string> majorKeys = csCurriculum.majorCore.getDefaultCourses();
                    HashSet<string> mathKeys = csCurriculum.mathCore.getDefaultCourses();
                    HashSet<string> appliedMathKeys = csCurriculum.appliedMathReq.getDefaultCourses();
                    HashSet<string> gerSpecificKeys = csCurriculum.gerSpecific.getDefaultCourses();
                    HashSet<string> natSciKeys = csCurriculum.naturalSciCore.getDefaultCourses();
                    HashSet<string> appliedTechKeys = csCurriculum.technicalCore.getDefaultCourses();
                    HashSet<string> otherKeys = csCurriculum.getRemainingDefault().getDefaultCourses();

                    List<HashSet<string>> keyListList = new List<HashSet<string>>();

                    keyListList.Add(mathKeys);
                    keyListList.Add(appliedMathKeys);
                    keyListList.Add(majorKeys);
                    keyListList.Add(gerSpecificKeys);
                    keyListList.Add(natSciKeys);
                    keyListList.Add(appliedTechKeys);
                    keyListList.Add(otherKeys);

                    foreach (HashSet<string> keyList in keyListList)
                    {
                        foreach (string key in keyList)
                        {
                            sortedKeys.Add(key);
                        }

                        currentMaxLevel = completePlan(uwm, sortedKeys, plan);

                        completeRealPlan(plan, realPlan);

                        sortedKeys.Clear();
                        plan.Clear();
                    }

                    for (int i = 0; i < realPlan.Count; ++i)
                    {
                        foreach(CourseNode node in realPlan.ElementAt(i))
                        {
                            Console.Out.Write(node.Course.Key + ", ");
                        }
                        Console.Out.WriteLine();
                    }

                    int creditLoad = 0;

                    int totalCredits = 0;
                    bool semesterCreditsToHigh = true;
                    while(semesterCreditsToHigh)
                    {
                        semesterCreditsToHigh = false;
                        foreach (List<CourseNode> innerList in realPlan)
                        {
                            totalCredits += sumCredits(innerList);
                            semesterCreditsToHigh = semesterCreditsToHigh || sumCredits(innerList) > 18;
                        }

                        foreach (List<CourseNode> innerList in realPlan)
                        {
                            creditLoad = sumCredits(innerList);
                            if (creditLoad > 18)
                            {
                                int creditsRemoved = 0;
                                for (int i = innerList.Count - 1; i >= 0 && creditLoad - creditsRemoved > 18; --i)
                                {
                                    innerList[i].Level++;
                                    foreach (Course postReq in innerList[i].Postreqs)
                                    {
                                        adjustPostReqs(realPlan, postReq, innerList[i].Level);
                                    }
                                    creditsRemoved += innerList[i].Course.getCredits();
                                }
                            }
                        }

                        List<List<CourseNode>> realAdjustedPlan = new List<List<CourseNode>>();
                        int maxLevel = 0;
                        foreach(List<CourseNode> innerList in realPlan)
                        {
                            foreach(CourseNode node in innerList)
                            {
                                maxLevel = Math.Max(maxLevel, node.Level);
                            }
                        }
                        for(int i = 0; i <= maxLevel; ++i)
                        {
                            realAdjustedPlan.Add(new List<CourseNode>());
                        }

                        List<CourseNode> addAtEndList = new List<CourseNode>();
                        foreach (List<CourseNode> innerList in realPlan)
                        {
                            foreach (CourseNode node in innerList)
                            {
                                if(node.Level == realPlan.IndexOf(innerList))
                                {
                                    realAdjustedPlan.ElementAt(node.Level).Add(node);
                                }
                                else
                                {
                                    addAtEndList.Add(node);
                                }
                            }
                        }
                        foreach(CourseNode node in addAtEndList)
                        {
                            realAdjustedPlan.ElementAt(node.Level).Add(node);
                        }
                        realPlan = realAdjustedPlan;
                    }

                    for (int i = 0; i < realPlan.Count; ++i)
                    {
                        foreach (CourseNode node in realPlan.ElementAt(i))
                        {
                            Console.Out.Write(node.Course.Key + ", ");
                        }
                        Console.Out.WriteLine();
                    }

                    DataContractJsonSerializer serialized = new DataContractJsonSerializer(typeof(List<List<CourseNode>>));
                    using (System.IO.FileStream file =
                    new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                    {
                        serialized.WriteObject(file, realPlan);
                    }

                    x = 6;
                }
                x = 7;
            }
            x = 8;
        }

        private static string fileName = Program.CURRENT_PATH + @"\Default CS Plan JSON.json";
        private void adjustPostReqs(List<List<CourseNode>> realPlan, Course postReq, int preNewLevel)
        {
            foreach(List<CourseNode> innerList in realPlan)
            {
                foreach(CourseNode node in innerList)
                {
                    if(node.Course.CompareTo(postReq) == 0
                        && node.Level >= preNewLevel)
                    {
                        node.Level = preNewLevel + 1;
                        foreach (Course postPostReq in node.Postreqs)
                        {
                            adjustPostReqs(realPlan, postPostReq, node.Level);
                        }
                    }
                }
            }
        }

        private int sumCredits(List<CourseNode> list)
        {
            int credits = 0;
            foreach(CourseNode node in list)
            {
                credits += node.Course.getCredits();
            }
            return credits;
        }
    }
}
