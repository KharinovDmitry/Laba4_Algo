namespace Core.TextSort
{
    public class RadixSort : BaseTextSort
    {
        public override string[] Sort(string text)
        {
            string[] words = SplitText(text);

            int n = words.Length;
            int maxLen = words.Max(s => s.Length);

            for (int i = maxLen - 1; i >= 0; i--)
            {
                string[] temp = new string[n];
                int[] count = new int[256];

                for (int j = 0; j < n; j++)
                {
                    if (words[j].Length > i)
                    {
                        count[words[j][i]]++;
                    }
                    else
                    {
                        count[0]++;
                    }
                }

                for (int j = 1; j < 256; j++)
                {
                    count[j] += count[j - 1];
                }

                for (int j = n - 1; j >= 0; j--)
                {
                    int index;
                    if (words[j].Length > i)
                    {
                        index = count[words[j][i]] - 1;
                        count[words[j][i]]--;
                    }
                    else
                    {
                        index = count[0] - 1;
                        count[0]--;
                    }

                    temp[index] = words[j];
                }

                for (int j = 0; j < n; j++)
                {
                    words[j] = temp[j];
                }
            }

            return words;
        }
    }
}