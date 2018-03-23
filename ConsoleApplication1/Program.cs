using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Management;
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


        public static void Shutdown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams =
                mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                    mboShutdownParams, null);
            }
        }

        static void Main(string[] args)
        {

            List<Thread> threads = new List<Thread>();
            Person Egor = new Person("Egor", ".txt", 4000000);
            Person anotherPerson = new Person("AnotherPerson", ".txt", 4000000);
            Thread t = new Thread(delegate ()
            {
                Egor.SetDna();
                Console.WriteLine("----------------------------------------------------");
                anotherPerson.SetDna();
                CheckForUnique(Egor, anotherPerson);
            });
            threads.Add(t);


            Parallel.For(0, 1, (i) =>
            {
                threads.ForEach(f => f.Start());
                threads.ForEach(f => f.Join());
                //Thread checkForUniqueThread = new Thread(() => CheckForUnique(Egor, anotherPerson));
                //checkForUniqueThread.Start();
                //checkForUniqueThread.Join();
            });

            Console.WriteLine("Всё прошло успешно!");
            Console.WriteLine("WE ARE SHUTTING DOWN");
            Console.WriteLine("------------------------------");
            //Shutdown();
            Console.WriteLine("------------------------------");
        }

        static void CheckForUnique(Person source, Person compared)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            string[] sourceDna = SplitDna(ReadFromFile(source));
            string[] comparedDna = SplitDna(ReadFromFile(compared));

            for (int i = 0; i < sourceDna.Length; i++)
            {
                Console.WriteLine($@"sourceDna = {sourceDna[i]} , comparedDna = { comparedDna[i]}");
                GetProc(sourceDna[i].Length, LevensteinImplementation(sourceDna[i], comparedDna[i]) + ".01");
            }
            Console.WriteLine("Сходство между {0} и {1}  = {2} % ", source.NameOfPerson, compared.NameOfPerson, Procenty.Average());
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Results.txt", true))
            {
                timer.Stop();
                string resultString =
                    $"| Date  - {DateTime.Now} , Прошло секунд - {timer.ElapsedMilliseconds / 1000} , {(timer.ElapsedMilliseconds / 1000) / 60} мин " +
                    $"Average - {Procenty.Average()} %  , Count of DNA - {Procenty.Count}|";
                
                string upstr = "";
               
                for (int i = 0; i < resultString.Length; i++)
                {
                    upstr += "-";
                    Console.WriteLine(upstr);
                }
                sw.WriteLine(upstr);
                sw.WriteLine(resultString);
                sw.WriteLine(upstr);
            }

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
                Console.WriteLine($"Сходство = {100.00 - (floatCompared / source) * 100}");
                // ReSharper disable once EmptyStatement
                Procenty.Add(100.00 - (floatCompared / source) * 100);
            }
            else
            {
                Console.WriteLine($"Сходство = {0}");
                Procenty.Add(0.0);
            }
        }
        static string[] SplitDna(StringBuilder dna)
        {
            return dna.ToString().Split(',');
        }
        static StringBuilder ReadFromFile(Person p)
        {
            using (StreamReader str = new StreamReader(p.NameOfPerson + p.Extension))
            {
                return new StringBuilder(str.ReadToEnd());
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
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            List<int> a = new List<int>();
            for (int i = 0; i < count_of_Dna; i++)
            {
                a.Add(rnd.Next(0, 4));
            }

            for (int item = 0; item < a.Count; item++)
            {

                switch (a.ElementAt(item))
                {
                    case 0:
                        {
                            sb.Append('А');
                            break;
                        }
                    case 1:
                        {
                            sb.Append('Г');
                            break;
                        }
                    case 2:
                        {
                            sb.Append('Т');
                            break;
                        }
                    case 3:
                        {
                            sb.Append('Ц');
                            break;
                        }

                }
                switch (item % 4 == 0 && item != 0)
                {
                    case true:
                        {
                            sb.Append(',');
                            break;
                        }
                }
            }
            string s = sb.ToString();
            Console.WriteLine(s);
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

