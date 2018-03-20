using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

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



            //  string[] split = s.Split(',');
            //Console.WriteLine(a);
            //using (StreamWriter sw = new StreamWriter("1.txt", true))
            //{
            //    sw.WriteLine(s);

            //}
            /* string s_;
             using (StreamReader str = new StreamReader("1.txt"))
             {
                 StringBuilder strb = new StringBuilder();
                 s_ = str.ReadToEnd();

             }
             string[] app = s_.Split(',');
             Thread t = new Thread(delegate ()
             {

             });          
             Console.WriteLine("Success!");*/
            //Console.WriteLine(s);

            //Console.WriteLine(LevensteinImplementation("ABCDEF", "ABGIK"));


            //CheckForUnique(Set_DNA(10000).Split(','), Set_DNA(10000).Split(','));


            // ToWritetoFile(Set_DNA(1000), nameOfSource);
            // ReSharper disable once InconsistentNaming
            List<Thread> threads = new List<Thread>();
        
                Thread t = new Thread(delegate()
                {
                    Person Egor = new Person("Egor", ".txt", 400000);
                    Egor.SetDna();
                    Person anotherPerson = new Person("AnotherPerson", ".txt", 400000);
                    anotherPerson.SetDna();
                    CheckForUnique(Egor, anotherPerson);
                });
                threads.Add(t);
            
          threads.ForEach(f=>f.Start());
        }

        static void CheckForUnique(Person source, Person compared)
        {
            string[] sourceDna = SplitDna(ReadFromFile(source));
            string[] comparedDna = SplitDna(ReadFromFile(compared));
            int[] arr = new int[sourceDna.Length];
            for (int i = 0; i < sourceDna.Length; i++)
            {
                arr[i] = LevensteinImplementation(sourceDna[i], comparedDna[i]);
                Console.WriteLine($@"sourceDna = {sourceDna[i]} , comparedDna = { comparedDna[i]}");
                GetProc(sourceDna[i].Length, arr[i] + ".01");
            }
            Console.WriteLine("Сходство между {0} и {1}  = {2}", source.NameOfPerson, compared.NameOfPerson, Procenty.Average());
        }
        public static List<double> Procenty = new List<double>();
        static void GetProc(int source, string compared)
        {
            var l = compared.Substring(0, compared.IndexOf(".", StringComparison.Ordinal));
            double floatCompared = Convert.ToDouble(compared.Replace(".", ","));
            Double a = (floatCompared / source);
            var ass = a.ToString(CultureInfo.InvariantCulture);
            if (!a.ToString(CultureInfo.InvariantCulture).StartsWith("1"))
            {
                Console.WriteLine($"Сходство = {(floatCompared / source) * 100}");
                // ReSharper disable once EmptyStatement

                Procenty.Add((floatCompared / source) * 100);
            }
            else
            {
                Console.WriteLine($"Сходство = {0}");
                Procenty.Add(0.0);
            }
        }
        static string[] SplitDna(string dna)
        {
            return dna.Split(',');
        }
        static string ReadFromFile(Person p)
        {
            using (StreamReader str = new StreamReader(p.NameOfPerson + p.Extension))
            {
                return str.ReadToEnd();
            }
        }
    }

    class Person
    {
        private Person() { }
        public Person(string NameOfPerson, string extensionOfFile, int count_of_Dna)
        {
            this.NameOfPerson = NameOfPerson;
            Extension = extensionOfFile;
            this.count_of_Dna = count_of_Dna;
        }

        public int count_of_Dna { get; set; }
        public void SetDna()
        {
            DNA_Code = Set_DNA(count_of_Dna);
        }
        public string DNA_Code { get; set; }
        public string NameOfPerson { get; set; }
        public string Extension { get; set; }
        private string Set_DNA(int count_of_Dna)
        {

            string s = "";
            Random rnd = new Random();
            List<int> a = new List<int>();
            Parallel.For(0, count_of_Dna, (i) =>
            {
                a.Add(rnd.Next(0, 4));
                Console.WriteLine(i);
            });

            for (int item = 0; item < a.Count; item++)
            {
                if (item % 4 == 0 && item != 0)
                {
                    Console.WriteLine(item);
                    if (a.ElementAt(item) == 0)
                    {
                        s += "А";
                    }
                    if (a.ElementAt(item) == 1)
                    {
                        s += "Г";

                    }
                    if (a.ElementAt(item) == 2)
                    {
                        s += "Ц";

                    }
                    if (a.ElementAt(item) == 3)
                    {
                        s += "Т";
                    }
                    s += ",";
                }
                else
                {

                    if (a.ElementAt(item) == 0)
                    {
                        s += "А";
                    }
                    else if (a.ElementAt(item) == 1)
                    {
                        s += "Г";
                    }
                    else if (a.ElementAt(item) == 2)
                    {
                        s += "Ц";
                    }
                    else if (a.ElementAt(item) == 3)
                    {
                        s += "Т";
                    }
                }
            }
            ToWritetoFile(s, NameOfPerson, Extension);
            return s;
        }



        public void ToWritetoFile(string source, string nameOfFile, string extension)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + nameOfFile + extension, true))
                {
                    stream.WriteLine(source);
                    Console.WriteLine("Запись прошла успешно");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }
    }
}

