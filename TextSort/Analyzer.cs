using Core.TextSort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSort
{
    internal static class Analyzer
    {
        public const string pathToTexts = "..\\..\\..\\..\\Core\\TextSort\\Texts";
        public static string[] fileNames = new string[]
        {
            "100words.txt",
            "500words.txt",
            "1000words.txt",
            "2000words.txt",
            "5000words.txt",
        };

        public static AnalyzeResult Evaluate(BaseTextSort textSort)
        {
            string algoName = textSort.GetType().Name;
            AnalyzeResult report = new AnalyzeResult(algoName);

            foreach (string fileName in TxtReader.fileNames)
            {
                string text = TxtReader.ReadText(fileName);

                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();
                string[] words = textSort.Sort(text);
                sw.Stop();

                report.AddMeasurement(words.Length, sw.Elapsed.TotalSeconds);
            }

            return report;
        }
    }
}
