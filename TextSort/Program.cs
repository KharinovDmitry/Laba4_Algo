using Core.TextSort;

namespace TextSort
{
    internal class Program
    {
        static int[] availableValues = new int[] { 100, 500, 1000, 2000, 5000, 10000, 25000 };
        static BaseTextSort[] algorithms = new BaseTextSort[]
        {
                new HeapSort(),
                new RadixSort()
        };

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("> Выберите действие:");
                Console.WriteLine("> 1 - сравнить быстродействие двух алгоритмов сортировки");
                Console.WriteLine("> 2 - отсортировать слова из текста заданной длины с помощью одного из двух алгоритмов и узнать,\n" +
                                  ">     сколько раз в тексте встречается каждое слово и количество уникальных слов");

                if (int.TryParse(Console.ReadLine(), out int n))
                {
                    Console.WriteLine();
                    if (n == 1)
                    {
                        ComparePerfomance();
                        break;
                    }
                    else if (n == 2)
                    {
                        SortOneText();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("> Некорректный ввод!\n");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("> Некорректный ввод!\n");
                    continue;
                }
            }
        }

        public static void ComparePerfomance()
        {
            List<AnalyzeResult> results = new List<AnalyzeResult>()
            {
                Analyzer.Evaluate(algorithms[0]),
                Analyzer.Evaluate(algorithms[1])
            };

            results.ForEach(it => CsvWriter.WriteAnalyzeResult(it));
        }

        public static void SortOneText()
        {
            while (true)
            {
                Console.WriteLine("> Выберите количество слов в тексте (100, 500, 1000, 2000, 5000, 10000, 25000):");
                if (int.TryParse(Console.ReadLine(), out int countOfWords))
                {
                    Console.WriteLine();
                    if (availableValues.Contains(countOfWords))
                    {
                        while (true)
                        {
                            Console.WriteLine("> Выберите алгоритм сортировки:");
                            Console.WriteLine("> 1 - HeapSort");
                            Console.WriteLine("> 2 - RadixSort");

                            if (int.TryParse(Console.ReadLine(), out int numberOfAlgorithm))
                            {
                                Console.WriteLine();
                                if (numberOfAlgorithm == 1 || numberOfAlgorithm == 2)
                                {
                                    string text = TxtReader.ReadText(countOfWords);
                                    string[] words = algorithms[numberOfAlgorithm - 1].Sort(text)[1..];

                                    Dictionary<string, int> countOfEachWord = GetCountOfEachWord(words);
                                    PrintCountOfEachWord(countOfEachWord);

                                    int countOfUniqueWords = GetCountOfUniqueWords(countOfEachWord);
                                    Console.WriteLine("> Количество уникальных слов: " + countOfUniqueWords);

                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("> Некорректный ввод!\n");
                                    continue;
                                }
                            }
                            else
                            {
                                Console.WriteLine("> Некорректный ввод!\n");
                                continue;
                            }
                        }

                        break;
                    }
                    else
                    {
                        Console.WriteLine("> Некорректный ввод!\n");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("> Некорректный ввод!\n");
                    continue;
                }
            }
        }

        public static Dictionary<string, int> GetCountOfEachWord(string[] words)
        {
            Dictionary<string, int> countOfEachWord = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (!countOfEachWord.ContainsKey(word))
                {
                    countOfEachWord[word] = 1;
                }
                else
                {
                    countOfEachWord[word]++;
                }
            }

            return countOfEachWord;
        }

        public static void PrintCountOfEachWord(Dictionary<string, int> countOfEachWord)
        {
            int maxWordLength = countOfEachWord.Keys.Max(it => it.Length);
            Console.Write("Слово" + string.Join("", Enumerable.Repeat(" ", maxWordLength - 5)));
            Console.WriteLine("| Количество" + string.Join("", Enumerable.Repeat(" ", 10)));
            Console.WriteLine(string.Join("", Enumerable.Repeat("-", maxWordLength + 12)));

            foreach(string word in countOfEachWord.Keys)
            {
                Console.Write(word + string.Join("", Enumerable.Repeat(" ", maxWordLength - word.Length)));
                Console.WriteLine("|" + string.Join("", Enumerable.Repeat(" ", 5)) + countOfEachWord[word]);
            }

            Console.WriteLine();
        }

        public static int GetCountOfUniqueWords(Dictionary<string, int> countOfEachWord)
        {
            int countOfUniqueWords = 0;

            foreach (string word in countOfEachWord.Keys)
            {
                if (countOfEachWord[word] == 1)
                {
                    countOfUniqueWords++;
                }
            }

            return countOfUniqueWords;
        }
    }
}