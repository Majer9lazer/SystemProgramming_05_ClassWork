using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        public static long FibRecursive(long n)
            => n == 1 ? 1 :
            (n == 0 ? 0 : (FibRecursive(n - 1) + FibRecursive(n - 2)));
        static long a;
        static int LevensteinImplementation(string first, string second)
        {
            int firstLength = first.Length;
            int secondLenth = second.Length;
            int[,] matrix = new int[firstLength + 1, secondLenth + 1];
            for (int i = 0; i < firstLength + 1; i++)
            {
                for (int j = 0; j < secondLenth + 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (i == 0)
                    {
                        matrix[i, j] = matrix[i, j - 1] + 1;
                    }
                    else if (j == 0)
                    {
                        matrix[i, j] = matrix[i - 1, j] + 1;
                    }
                    else
                    {
                        if (first[i - 1] == second[j - 1])
                        {
                            matrix[i, j] = matrix[i - 1, j - 1];
                        }
                        else
                        {
                            matrix[i, j] = 1 + Math.Min(matrix[i - 1, j - 1],
                                Math.Min(matrix[i - 1, j], matrix[i, j - 1]));
                        }
                    }
                }
            }
            return matrix[firstLength, secondLenth];
        }
        static void Main(string[] args)
        {

            //Task.Run(() => 
            //{
            //    Console.WriteLine(FibRecursive(45));
            //});
            //StringBuilder sb = new StringBuilder();
            //string s = sb.ToString();
            //Random rnd = new Random();
            //int k = 0;
            //for (int i = 1; i < 40000; i++)
            //{
            //    if (i % 4 == 0)
            //    {
            //        s += ",";
            //        Console.WriteLine(i);
            //    }
            //    else
            //    {
            //        if (rnd.Next(0, 4) == 0)
            //        {
            //            s += "А";
            //            k++;
            //        }
            //        if (rnd.Next(0, 4) == 1)
            //        {
            //            s += "Г";
            //            k++;
            //        }
            //        if (rnd.Next(0, 4) == 2)
            //        {
            //            s += "Ц";
            //            k++;
            //        }
            //        if (rnd.Next(0, 4) == 3)
            //        {
            //            s += "Т";
            //            k++;
            //        }
            //    }
            //}

            //using (StreamWriter sw = new StreamWriter("1.txt", true))
            //{
            //    sw.Write(s);
            //}
            //Console.WriteLine(s);

            Console.WriteLine(LevensteinImplementation("ABCDEF", "ABGIK"));

        }
    }
}
