using OfficeOpenXml;

namespace TextSort
{
    internal class CsvWriter
    {
        public static void WriteAnalyzeResult(AnalyzeResult result)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            
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

                Console.WriteLine($"> Алгоритм {result.Title} запущен!\n");
                Console.WriteLine("Количество слов | Время в сек.");
                Console.WriteLine("------------------------------");

                worksheet.Cells[1, 1].Value = "ArrLength";
                worksheet.Cells[1, 2].Value = "Time";

                for (int i = 0; i < result.Measurements.Count; i++)
                {
                    var ArrLength = result.Measurements[i].ArrayLength;
                    var Time = result.Measurements[i].TimeInS;

                    Console.Write($"{ArrLength}{string.Join("", Enumerable.Repeat(" ", 16 - ArrLength.ToString().Length))}|");
                    Console.WriteLine($" {Time}");

                    worksheet.Cells[i + 2, 1].Value = ArrLength;
                    worksheet.Cells[i + 2, 2].Value = Time;
                }

                Console.WriteLine($"\n> Алгоритм {result.Title} выполнен!\n");
                package.SaveAs(file);
            }
        }
    }
}