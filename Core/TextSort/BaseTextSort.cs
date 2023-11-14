using System.Text;

namespace Core.TextSort
{
    public abstract class BaseTextSort
    {
        public abstract string[] Sort(string text);

        public string[] SplitText(string text)
        {
            char[] symbols = new char[]
            {
                '.', ',', ':', ';', '!', '?', '*', '\'', '"', '(', ')', '[', ']',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '\r', '\n'
            };

            StringBuilder newText = new StringBuilder();

            text = text.Replace("--", " ");

            foreach (char sym in text)
            {
                if (!symbols.Contains(sym))
                {
                    newText.Append(sym);
                }
            }

            string[] words = newText.ToString().ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return words;
        }
    }
}