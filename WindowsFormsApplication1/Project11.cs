using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScraper
{

    class Project11
    {

        public int x = 5;

        public Project11()
        {

        }

        private static int[][] nums = {new int[]{8,91,77,50,12,52,78,7,5,4,75,0,40,0,15,38,97,22,2,8},
                                            new int[]{0,62,56,4,48,69,43,98,40,17,87,60,57,18,81,17,40,99,49,49},
                                            new int[]{65,36,13,49,3,30,88,53,67,40,71,93,29,14,79,55,73,31,49,81},
                                            new int[]{91,36,2,37,71,56,32,1,56,68,24,69,42,11,60,4,23,95,70,52},
                                            new int[]{80,13,33,66,28,40,40,22,54,36,92,41,89,63,67,51,71,16,31,22},
                                            new int[]{50,12,17,35,20,84,36,78,53,33,75,44,2,45,3,99,60,32,47,24},
                                            new int[]{70,64,38,18,66,70,54,59,67,40,38,26,10,67,23,64,28,81,98,32},
                                            new int[]{21,94,49,66,91,40,8,63,39,94,63,95,20,12,62,2,68,20,26,67},
                                            new int[]{72,63,89,34,88,14,83,96,78,78,17,97,26,99,73,66,5,58,55,24},
                                            new int[]{95,33,31,34,97,33,61,0,14,35,45,20,44,76,0,75,9,23,36,21},
                                            new int[]{92,56,53,9,14,16,62,4,80,3,94,15,67,31,75,22,28,53,17,78},
                                            new int[]{57,85,29,36,24,54,17,0,24,88,58,55,47,31,35,96,42,5,39,16},
                                            new int[]{58,17,54,51,58,21,60,44,37,44,44,5,7,89,71,35,48,0,56,86},
                                            new int[]{40,55,89,4,77,17,52,86,13,92,73,28,69,47,94,5,68,81,80,19},
                                            new int[]{66,98,27,33,79,26,26,16,32,57,97,7,16,99,35,97,83,8,52,4},
                                            new int[]{69,53,93,63,32,12,55,46,67,33,46,3,72,20,62,57,87,68,36,88},
                                            new int[]{36,76,62,40,32,29,46,8,18,72,94,24,11,39,25,38,73,16,42,4},
                                            new int[]{16,36,4,74,85,59,67,82,69,99,62,34,88,23,30,72,41,36,69,20},
                                            new int[]{54,5,57,23,16,81,86,48,71,49,31,74,1,90,31,78,29,35,73,20},
                                            new int[]{48,67,19,89,1,52,43,61,48,33,92,16,69,54,51,83,71,54,70,1}};

        public static void findBiggest()
        {
            int largest = 0;
            int cur = 0;
            for(int i = 0; i < 20; ++i)
            {
                for(int j = 0; j < 20; ++j)
                {
                    if(j >= 3) cur = nums[i][j] * nums[i][j - 1] * nums[i][j - 2] * nums[i][j - 3];
                    if (cur > largest) largest = cur;
                    if(j <= 16) cur = nums[i][j] * nums[i][j + 1] * nums[i][j + 2] * nums[i][j + 3];
                    if (cur > largest) largest = cur;
                    if (j >= 3 && i >=3) cur = nums[i][j] * nums[i-1][j - 1] * nums[i-2][j - 2] * nums[i-3][j - 3];
                    if (cur > largest) largest = cur;
                    if (j <= 16 && i <= 16) cur = nums[i][j] * nums[i+1][j + 1] * nums[i+2][j + 2] * nums[i+3][j + 3];
                    if (cur > largest) largest = cur;
                    if (j >= 3 && i <= 16) cur = nums[i][j] * nums[i+1][j - 1] * nums[i+2][j - 2] * nums[i+3][j - 3];
                    if (cur > largest) largest = cur;
                    if (j <= 16 && i >= 3) cur = nums[i][j] * nums[i-1][j + 1] * nums[i-2][j + 2] * nums[i-3][j + 3];
                    if (cur > largest) largest = cur;
                    if (i >= 3) cur = nums[i][j] * nums[i-1][j] * nums[i-2][j] * nums[i-3][j];
                    if (cur > largest) largest = cur;
                    if (i <= 16) cur = nums[i+1][j] * nums[i+2][j] * nums[i+3][j] * nums[i][j];
                    if (cur > largest) largest = cur;
                }
            }
            Console.Out.Write("largest " + largest);
        }
    }
}
