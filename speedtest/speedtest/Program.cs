using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace speedtest
{
    class TestRun
    {
        public MemberInfo MethodName { get; set; }
        public TimeSpan TimeSpan { get; set; }

        public override string ToString()
        {
            return $"{TimeSpan} - {MethodName.Name}";
        }
    }

    class Program
    {
        static void ConvertToInt32Test()
        {
            for (int i = 0; i < 10000; i++)
                Convert.ToInt32("300");
        }

        static void IntParseTest()
        {
            for (int i = 0; i < 10000; i++)
                int.Parse("300");
        }

        static void Int32ParseTest()
        {
            for (int i = 0; i < 10000; i++)
                Int32.Parse("300");
        }

        static void Main(string[] args)
        {
            Action[] methods = new Action[] { ConvertToInt32Test, IntParseTest, Int32ParseTest };

            Stopwatch sw = new Stopwatch();

            List<TestRun> results = new List<TestRun>();

            int numberOfTestRuns = 1000;
            for (int i = 0; i < numberOfTestRuns; i++)
            {
                for (int j = 0; j < methods.Length; j++)
                {
                    sw.Restart();
                    methods[j]();
                    sw.Stop();

                    results.Add(new TestRun()
                    {
                        MethodName = methods[j].Method,
                        TimeSpan = sw.Elapsed
                    });
                }
            }


            var q = from x in results
                    group x by x.MethodName.Name into g
                    select new
                    {
                        METHOD = g.Key,
                        AVG_SCORE = Math.Round(g.Average(a => a.TimeSpan.TotalMilliseconds), 6)
                    };

            StreamWriter swr = new StreamWriter("_OUTPUT.txt");
            results.ForEach(x => swr.WriteLine(x));
            swr.Close();

            q.OrderByDescending(x => x.AVG_SCORE).ToList().ForEach(x => {
                Console.WriteLine($"{x.AVG_SCORE} ms : {x.METHOD}");
            });
        }
    }
}
