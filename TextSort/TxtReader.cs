namespace TextSort
{
    internal class TxtReader
    {
        public const string pathToTexts = "..\\..\\..\\..\\Core\\TextSort\\Texts";
        public static string[] fileNames = Directory.GetFiles(pathToTexts);

        public static string ReadText(string fileName)
        {
            string text = "";

            using (StreamReader reader = new StreamReader($"{fileName}"))
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