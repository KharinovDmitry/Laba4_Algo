namespace Core.TextSort
{
    public class HeapSort : BaseTextSort
    {
        public override string[] Sort(string text)
        {
            string[] words = SplitText(text);

            int n = words.Length;

            for (int i = n / 2 - 1; i >= 0; i--)
            {
                Heapify(words, n, i);
            }

            for (int i = n - 1; i >= 0; i--)
            {
                string temp = words[0];
                words[0] = words[i];
                words[i] = temp;

                Heapify(words, i, 0);
            }

            return words;
        }

        public void Heapify(string[] words, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && words[left].CompareTo(words[largest]) > 0)
            {
                largest = left;
            }

            if (right < n && words[right].CompareTo(words[largest]) > 0)
            {
                largest = right;
            }

            if (largest != i)
            {
                string temp = words[i];
                words[i] = words[largest];
                words[largest] = temp;

                Heapify(words, n, largest);
            }
        }
    }
}