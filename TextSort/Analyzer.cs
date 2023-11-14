using Core.TextSort;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TextSort
{
    internal static class Analyzer
    {
        public static AnalyzeResult Evaluate(BaseTextSort textSort)
        {
            string algoName = textSort.GetType().Name;
            AnalyzeResult report = new AnalyzeResult(algoName);

            Array.Sort(TxtReader.fileNames, (x, y) =>
            {
                int xWordCount = int.Parse(Regex.Match(x, @"\d+").Value);
                int yWordCount = int.Parse(Regex.Match(y, @"\d+").Value);
                return xWordCount.CompareTo(yWordCount);
            });

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