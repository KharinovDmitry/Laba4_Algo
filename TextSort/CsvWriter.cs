using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSort
{
    internal class CsvWriter
    {
        public static void WriteAnalyzeResult(AnalyzeResult result)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            
            FileInfo file = new FileInfo("Charts.xlsx");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet? worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == result.Title);
                if (worksheet == null)
                {
                    worksheet = package.Workbook.Worksheets.Add(result.Title);
                }
                else
                {
                    worksheet.Cells.Clear();
                }

                worksheet.Cells[1, 1].Value = "ArrLength";
                worksheet.Cells[1, 2].Value = "Time";

                Console.WriteLine($"> Algorithm {result.Title} run!\n");
                Console.WriteLine("Count of words | Time in seconds");
                Console.WriteLine("--------------------------------");

                worksheet.Cells[1, 1].Value = "ArrLength";
                worksheet.Cells[1, 2].Value = "Time";

                for (int i = 0; i < result.Measurements.Count; i++)
                {
                    var ArrLength = result.Measurements[i].ArrayLength;
                    var Time = result.Measurements[i].TimeInS;

                    Console.Write($"{ArrLength}{string.Join("", Enumerable.Repeat(" ", 15 - ArrLength.ToString().Length))}|");
                    Console.WriteLine($" {Time}");

                    worksheet.Cells[i + 2, 1].Value = ArrLength;
                    worksheet.Cells[i + 2, 2].Value = Time;
                }

                Console.WriteLine($"\n> Algorithm {result.Title} done!\n");
                package.SaveAs(file);
            }
        }
    }
}
