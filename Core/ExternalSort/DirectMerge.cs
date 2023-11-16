using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.ExternalSort;
using static System.Collections.Specialized.BitVector32;

namespace CoreHelper.ExternalSort
{
    public class DirectMerge : IExternalSort
    {
        public Logger logger = new Logger();
        public string FileInput { get; set; }
        private int _columnNumber = 0;
        private ColumnType _columnType = ColumnType.str;
        private long iterations, segments;

        /*
        public DirectMerge(string filename)
        {
            FileInput = filename;
            iterations = 1;           
        }
        */
        /*
        public DirectMerge(string filename, int columnNumber)
        {
            FileInput = filename;
            _columnNumber = columnNumber;
            iterations = 1;
        }
        */


        public List<ExAction> Sort(string filename, ColumnType type, int columnNumber)
        {
            FileInput = filename;
            _columnNumber = columnNumber;
            _columnType = type;
            iterations = 1;

            if (_columnType == ColumnType.str)
            {
                return SortAsString();
            }
            if (_columnType == ColumnType.integer)
            {
                return SortAsInt();
            }
            return null;
        }
        public List<ExAction> SortAsInt()
        {
            List<ExAction> actions = new();
            while (true)
            {
                SplitToFiles();
                if (segments == 1)
                {
                    break;
                }
                actions = actions.Concat(MergePairsAsInt()).ToList();
            }
            return actions;
        }
        public List<ExAction> SortAsString()
        {
            List<ExAction> actions = new();
           
            while (true)
            {
                SplitToFiles();
                if (segments == 1)
                {
                    break;
                }
                actions = actions.Concat(MergePairsAsString()).ToList();
            }
            return actions;
        }
        private void SplitToFiles()
        {
            segments = 1;
            using (StreamReader sr = new StreamReader(FileInput))
            using (StreamWriter writerA = new StreamWriter("a.txt"))
            using (StreamWriter writerB = new StreamWriter("b.txt"))
            {
                long counter = 0;
                bool flag = true;

                while (!sr.EndOfStream)
                {
                    if (counter == iterations)
                    {
                        flag = !flag;
                        counter = 0;
                        segments++;
                    }
                    string element = sr.ReadLine();

                    if (flag)
                    {
                        writerA.WriteLine(element);
                    }
                    else
                    {
                        writerB.WriteLine(element);
                    }
                    counter++;
                }
            }
        }

        private List<ExAction> MergePairsAsInt()
        {
            int indexInput = 0;
            int indexA = 0;
            int indexB = 0;
            ExAction actionMove = new();
            ExAction actionCompare = new();
            List<ExAction> actions = new();
            using StreamReader readerA = new StreamReader(File.OpenRead("a.txt"));
            using StreamReader readerB = new StreamReader(File.OpenRead("b.txt"));
            using StreamWriter sr = new StreamWriter(File.Create("data.txt"));
            long counterA = iterations, counterB = iterations;
            int elementA = 0, elementB = 0;
            string strA = null, strB = null;
            bool pickedA = false, pickedB = false;
            while (!readerA.EndOfStream || !readerB.EndOfStream || pickedA || pickedB)
            {
                if (counterA == 0 && counterB != 0)
                {
                    logger.AddLog(new ExternalSteps("Write", $"Серия a закончилась, дописываем {counterB} элементов серии b."));
                }
                if (counterB == 0 && counterA != 0)
                {
                    logger.AddLog(new ExternalSteps("Write", $"Серия b закончилась, дописываем {counterA} элементов серии a."));
                }
                if (counterA == 0 && counterB == 0)
                {
                    counterA = iterations;
                    counterB = iterations;
                }
                

                if (!readerA.EndOfStream)
                {
                    if (counterA > 0 && !pickedA)
                    {
                        strA = readerA.ReadLine();
                        elementA = int.Parse(strA.Split(";")[_columnNumber]);
                        logger.AddLog(new ExternalSteps("Read", $"Считываем элемент {elementA} с файла \"a.txt\"."));
                        pickedA = true;
                    }
                }

                if (!readerB.EndOfStream)
                {
                    if (counterB > 0 && !pickedB)
                    {
                        strB = readerB.ReadLine();
                        elementB = int.Parse(strB.Split(";")[_columnNumber]);
                        logger.AddLog(new ExternalSteps("Read", $"Считываем элемент {elementB} с файла \"b.txt\"."));
                        pickedB = true;
                    }
                }

                if (pickedA)
                {
                    if (pickedB)
                    {
                        
                        if (elementA < elementB)
                        {
                            logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementA} из файла \"a.txt\" в файл \"{FileInput}\"."));
                            sr.WriteLine(strA);
                            counterA--;
                            pickedA = false;
                            actionMove.ToFile = FileInput;
                            actionMove.FromFile = "a.txt";
                            actionMove.FromIndex = indexA;
                            actionMove.ToIndex = indexInput;
                            indexInput++;
                            indexA++;
                        }
                        else
                        {
                            logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                            sr.WriteLine(strB);
                            counterB--;
                            pickedB = false;
                            actionMove.ToFile = FileInput;
                            actionMove.FromFile = "b.txt";
                            actionMove.FromIndex = indexB;
                            actionMove.ToIndex = indexInput;
                            indexInput++;
                            indexB++;
                        }
                        actionMove.Action = Core.ExternalSort.Action.Compare;
                        
                    }
                    else
                    {
                        sr.WriteLine(strA);
                        counterA--;
                        pickedA = false;
                    }
                }
                else if (pickedB)
                {
                    logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                    sr.WriteLine(strB);
                    counterB--;
                    pickedB = false;
                    actionMove.ToFile = FileInput;
                    actionMove.FromFile = "b.txt";
                    actionMove.FromIndex = indexB;
                    actionMove.ToIndex = indexInput;
                    indexInput++;
                    indexB++;
                }
                actionCompare = (ExAction)actionMove.Clone();
                actionCompare.Action = Core.ExternalSort.Action.Compare;
                actions.Add(actionCompare);
                actions.Add(actionMove);

            }
            sr.Close();
            readerA.Close();
            readerB.Close();
            iterations *= 2;

            Console.WriteLine();
            return actions;
        }
        private List<ExAction> MergePairsAsString()
        {
            int indexInput = 0;
            int indexA = 0;
            int indexB = 0;
            ExAction actionMove = new();
            ExAction actionCompare = new();
            List<ExAction> actions = new();
            using (StreamReader readerA = new StreamReader("a.txt"))
            using (StreamReader readerB = new StreamReader("b.txt"))
            using (StreamWriter sr = new StreamWriter(FileInput))
            {
                long counterA = iterations, counterB = iterations;
                string elementA = null, elementB = null;
                string strA = null, strB = null;
                bool pickedA = false, pickedB = false;
                while (!readerA.EndOfStream || !readerB.EndOfStream || pickedA || pickedB)
                {
                    
                    if (counterA == 0 && counterB != 0)
                    {
                        logger.AddLog(new ExternalSteps("Write", $"Серия a закончилась, дописываем {counterB} элементов серии b."));
                    }
                    if (counterB == 0 && counterA != 0)
                    {
                        logger.AddLog(new ExternalSteps("Write", $"Серия b закончилась, дописываем {counterA} элементов серии a."));
                    }
                    if (counterA == 0 && counterB == 0)
                    {
                        counterA = iterations;
                        counterB = iterations;
                    }

                    
                    if (!readerA.EndOfStream)
                    {
                        if (counterA > 0 && !pickedA)
                        {
                            strA = readerA.ReadLine();
                            elementA = strA.Split(";")[_columnNumber];
                            logger.AddLog(new ExternalSteps("Read", $"Считываем элемент {elementA} с файла \"a.txt\"."));
                            pickedA = true;
                        }
                    }

                    if (!readerB.EndOfStream)
                    {
                        if (counterB > 0 && !pickedB)
                        {
                            strB = readerB.ReadLine();
                            elementB = strB.Split(";")[_columnNumber];
                            logger.AddLog(new ExternalSteps("Read", $"Считываем элемент {elementB} с файла \"b.txt\"."));
                            pickedB = true;
                        }
                    }

                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (String.CompareOrdinal(elementA, elementB) < 0)
                            {
                                logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementA} из файла \"a.txt\" в файл \"{FileInput}\"."));
                                sr.WriteLine(strA);
                                counterA--;
                                pickedA = false;
                                actionMove.ToFile = FileInput;
                                actionMove.FromFile = "a.txt";
                                actionMove.FromIndex = indexA;
                                actionMove.ToIndex = indexInput;
                                indexInput++;
                                indexA++;
                            }
                            else
                            {
                                logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                                sr.WriteLine(strB);
                                counterB--;
                                pickedB = false;
                                actionMove.ToFile = FileInput;
                                actionMove.FromFile = "b.txt";
                                actionMove.FromIndex = indexB;
                                actionMove.ToIndex = indexInput;
                                indexInput++;
                                indexB++;
                            }

                        }
                        else
                        {
                            logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementA} из файла \"a.txt\" в файл \"{FileInput}\"."));
                            sr.WriteLine(strA);
                            counterA--;
                            pickedA = false;
                            actionMove.ToFile = FileInput;
                            actionMove.FromFile = "a.txt";
                            actionMove.FromIndex = indexA;
                            actionMove.ToIndex = indexInput;
                            indexInput++;
                            indexA++;
                        }
                    }
                    else if (pickedB)
                    {
                        logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                        sr.WriteLine(strB);
                        counterB--;
                        pickedB = false;
                        actionMove.ToFile = FileInput;
                        actionMove.FromFile = "b.txt";
                        actionMove.FromIndex = indexB;
                        actionMove.ToIndex = indexInput;
                        indexInput++;
                        indexB++;
                    }
                    actionCompare = (ExAction) actionMove.Clone();
                    actionCompare.Action = Core.ExternalSort.Action.Compare;
                    actions.Add(actionCompare);
                    actions.Add(actionMove);

                }

                iterations *= 2;
                Console.WriteLine();
               
            }
            return actions;
        }
    }
}
