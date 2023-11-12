using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.FinancialDayCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSort
{
    internal class TxtReader
    {
        public const string pathToTexts = "..\\..\\..\\..\\Core\\TextSort\\Texts";
        public static string[] fileNames = new string[]
        {
            "100words.txt",
            "500words.txt",
            "1000words.txt",
            "2000words.txt",
            "5000words.txt",
            "10000words.txt",
            "25000words.txt",
        };

        public static string ReadText(string fileName)
        {
            string text = "";

            using (StreamReader reader = new StreamReader($"{pathToTexts}\\{fileName}"))
            {
                text = reader.ReadToEnd();
            }

            return text.ToString();
        }

        public static string ReadText(int countOfWords)
        {
            string text = "";

            using (StreamReader reader = new StreamReader($"{pathToTexts}\\{countOfWords}words.txt"))
            {
                text = reader.ReadToEnd();
            }

            return text.ToString();
        }
    }
}
