using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ConsoleApplication1
{
    class Program
    {
        public static long FibRecursive(long n)
            => n == 1 ? 1 :
            (n == 0 ? 0 : (FibRecursive(n - 1) + FibRecursive(n - 2)));
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
            Person Egor = new Person("Egor", ".txt", 40000000);
            Person anotherPerson = new Person("AnotherPerson", ".txt", 40000000);
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Egor.SetDna();
                    anotherPerson.SetDna();
                    CheckForUnique(Egor, anotherPerson, i);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
           
            }
            Console.WriteLine("Всё прошло успешно!");
            Console.WriteLine("WE ARE SHUTTING DOWN");
            //Shutdown();
            Console.ReadLine();
        }

        static void CheckForUnique(Person source, Person compared, int resultNumber)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            string[] sourceDna = (ReadFromFile(source)).ToString().Split(',').AsParallel().ToArray();
            string[] comparedDna = (ReadFromFile(compared)).ToString().Split(',').AsParallel().ToArray();
            List<double> percents = new List<double>();
            Thread t = new Thread(delegate ()
            {
                for (int i = 0; i < sourceDna.Length; i++)
                {
                    GetProc(sourceDna[i].Length, LevensteinImplementation(sourceDna[i], comparedDna[i]) + ".00000001", ref percents);
                }
            })
            { Priority = ThreadPriority.AboveNormal };
            t.Start();
            t.Join();
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "ResultsTest.txt", true))
            {
                timer.Stop();
                string resultString =
                    $"|Номер результата :{resultNumber} , Date  - {DateTime.Now} ,Источник - {source.NameOfPerson} , Сравниваемый - {compared.NameOfPerson} , Прошло  - {timer.ElapsedMilliseconds} миллисекунд, {timer.ElapsedMilliseconds / 1000.00} сек , {(timer.ElapsedMilliseconds / 1000.00) / 60.00} мин " +
                    $"Average - {percents.Where(f => f != double.NegativeInfinity).Average()} %  , Count of DNA - {percents.Count}|";
                string upstr = "";
                Thread resThread = new Thread(delegate ()
                {
                    Parallel.For(0, resultString.Length, (i) =>
                    {
                        upstr += "-";
                    });
                })
                { Priority = ThreadPriority.AboveNormal };
                resThread.Start();
                resThread.Join();
                sw.WriteLine(upstr);
                sw.WriteLine(resultString);
                sw.WriteLine(upstr);
                Console.WriteLine(upstr + "\n" + resultString + "\n" + upstr);
            }
        }

        static void GetProc(int source, string compared, ref List<double> procenty)
        {
            double floatCompared = Convert.ToDouble(compared.Replace(".", ","));
            double a = (floatCompared / source);
            switch (a.ToString(CultureInfo.InvariantCulture).StartsWith("1"))
            {
                case true:
                    {
                        procenty.Add(0.00);
                        break;
                    }
                case false:
                    {
                        procenty.Add(100.00 - (floatCompared / source) * 100);
                        break;
                    }
            }
        }

        static StringBuilder ReadFromFile(Person p)
        {
            using (StreamReader str = new StreamReader(p.NameOfPerson + p.Extension))
            {
                StringBuilder bs= new StringBuilder(str.ReadToEnd());
                return bs;
            }
        }
    }

    class Person
    {
        public Person(string nameOfPerson, string extensionOfFile, int countOfDna)
        {
            this.NameOfPerson = nameOfPerson;
            Extension = extensionOfFile;
            this.CountOfDna = countOfDna;
        }
        public int CountOfDna { get; set; }
        public void SetDna()
        {
            Set_DNA(CountOfDna);
        }
        public string NameOfPerson { get; set; }
        public string Extension { get; set; }
        private void Set_DNA(int countOfDna)
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            int[] a = new int[countOfDna];
            for (int i = 0; i < countOfDna; i++)
            {
                a[i] = rnd.Next(0, 4);
            }
            for (int item = 0; item < a.Length; item++)
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
            ToWritetoFile(sb.ToString(), NameOfPerson, Extension);
        }
        public void ToWritetoFile(string source, string nameOfFile, string extension)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + nameOfFile + extension, true))
                {
                    stream.Write(source + ",");
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

