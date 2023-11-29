using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private ObservableCollection<CellsLine> _cells;
        public string FileInput { get; set; }
        private int _columnNumber = 0;
        private ColumnType _columnType = ColumnType.str;
        private long iterations, segments;

        public DirectMerge(ObservableCollection<CellsLine> cells)
        {
            _cells = cells;
        }

        public async Task Sort(string filename, ColumnType type, int columnNumber)
        {
            FileInput = filename;
            _columnNumber = columnNumber;
            _columnType = type;
            iterations = 1;

            if (_columnType == ColumnType.str)
            {
                await SortAsString(columnNumber);
               
            }
            if (_columnType == ColumnType.integer)
            {
                await SortAsInt(columnNumber);
            }
            
        }
        public async Task SortAsInt(int columnnumber)
        {
            
            while (true)
            {
                await SplitToFiles(columnnumber);
                if (segments == 1)
                {
                    break;
                }
                await MergePairsAsInt();
            }
          
        }
        public async Task SortAsString(int columnnumber)
        {
            

            while (true)
            {
                await SplitToFiles(columnnumber);
                if (segments == 1)
                {
                    break;
                }
                await MergePairsAsString();
            }

        }
        private async Task SplitToFiles(int columnnumber)
        {
            segments = 1;
            int indexInput = 0;
            int indexA = 0;
            int indexB = 0;
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
                        indexInput = Math.Min(9, indexInput);
                        indexA = Math.Min(9, indexA);
                        _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, element.Split(";")[columnnumber]);
                        _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);
                        indexA++;
                        indexInput++;

                        await Task.Delay(500);
                        Update();
                        await Task.Delay(100);
                    }
                    else
                    {
                        writerB.WriteLine(element);
                        indexInput = Math.Min(9, indexInput);
                        indexB = Math.Min(9, indexB);
                        _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, element.Split(";")[columnnumber]);
                        _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);
                        indexB++;
                        indexInput++;
                        
                        await Task.Delay(500);
                        Update();
                        await Task.Delay(100);
                    }
                    counter++;
                }
            }
        }

        private async Task MergePairsAsInt()
        {
            int indexInput = 0;
            int indexA = 0;
            int indexB = 0;
            ExAction actionMove = new();
            ExAction actionCompare = new();
            List<ExAction> actions = new();
            using (StreamReader readerA = new StreamReader(File.OpenRead("a.txt")))
            using (StreamReader readerB = new StreamReader(File.OpenRead("b.txt")))
            using (StreamWriter sr = new StreamWriter(File.Create("data.txt")))
            {
                long counterA = iterations, counterB = iterations;
                int elementA = 0, elementB = 0;
                string strA = null, strB = null;
                bool pickedA = false, pickedB = false;
                while (!readerA.EndOfStream || !readerB.EndOfStream || pickedA || pickedB)
                {
                    indexInput = Math.Min(9, indexInput);
                    indexA = Math.Min(9, indexA);
                    indexB = Math.Min(9, indexB);
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
                            _cells[Index("b.txt")].Cells[indexB].Update(Action.Compare, elementB);
                            _cells[Index("a.txt")].Cells[indexA].Update(Action.Compare, elementA);

                            if (elementA < elementB)
                            {
                                await Task.Delay(1000);
                                Update();
                                await Task.Delay(100);
                                logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementA} из файла \"a.txt\" в файл \"{FileInput}\"."));
                                sr.WriteLine(strA);
                                counterA--;
                                pickedA = false;
                                _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                                _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                                indexInput++;
                                indexA++;
                            }

                            else
                            {
                                await Task.Delay(1000);
                                Update();
                                await Task.Delay(1000);
                                logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                                sr.WriteLine(strB);
                                counterB--;
                                pickedB = false;
                                _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                                _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                                indexInput++;
                                indexB++;
                            }
                            await Task.Delay(1000);
                            Update();
                            await Task.Delay(100);

                        }
                        else
                        {
                            sr.WriteLine(strA);
                            Update();
                            counterA--;
                            pickedA = false;
                            _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                            _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                            await Task.Delay(1000);
                            indexA++;
                            indexInput++;
                        }
                    }
                    else if (pickedB)
                    {
                        Update();
                        logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                        sr.WriteLine(strB);
                        counterB--;
                        pickedB = false;

                        _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                        _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                        await Task.Delay(1000);
                        indexInput++;
                        indexB++;


                    }
                    actionCompare = (ExAction)actionMove.Clone();

                    actions.Add(actionCompare);
                    actions.Add(actionMove);
                    Update();
                }
                Update();
                sr.Close();
                readerA.Close();
                readerB.Close();
                iterations *= 2;
            }
        }
        private async Task MergePairsAsString()
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
                    indexInput = Math.Min(9, indexInput);
                    indexA = Math.Min(9, indexA);
                    indexB = Math.Min(9, indexB);

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
                            _cells[Index("b.txt")].Cells[indexB].Update(Action.Compare, elementB);
                            _cells[Index("a.txt")].Cells[indexA].Update(Action.Compare, elementA);

                            if (String.CompareOrdinal(elementA, elementB) < 0)
                            {
                                await Task.Delay(1000);
                                Update();
                                await Task.Delay(100);
                                logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementA} из файла \"a.txt\" в файл \"{FileInput}\"."));
                                sr.WriteLine(strA);
                                counterA--;
                                pickedA = false;
                                _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                                _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                                indexInput++;
                                indexA++;
                            }
                            else
                            {
                                await Task.Delay(1000);
                                Update();
                                await Task.Delay(1000);
                                logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                                sr.WriteLine(strB);
                                counterB--;
                                pickedB = false;
                                _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                                _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                                indexInput++;
                                indexB++;
                            }
                            await Task.Delay(1000);
                            Update();
                            await Task.Delay(100);

                        }
                        else
                        {
                            sr.WriteLine(strA);
                            Update();
                            counterA--;
                            pickedA = false;
                            _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                            _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                            await Task.Delay(1000);
                            indexA++;
                            indexInput++;
                        }
                    }
                    else if (pickedB)
                    {
                        Update();
                        logger.AddLog(new ExternalSteps("Write", $"Добавляем {elementB} из файла \"b.txt\" в файл \"{FileInput}\"."));
                        sr.WriteLine(strB);
                        counterB--;
                        pickedB = false;

                        _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                        _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                        await Task.Delay(1000);
                        indexInput++;
                        indexB++;
                    }
                    Update();
                    actionCompare = (ExAction)actionMove.Clone();

                    actions.Add(actionCompare);
                    actions.Add(actionMove);
                }
                await Task.Delay(100);
                Update();

                iterations *= 2;
                Console.WriteLine();
                sr.Close();
            }

        }

        private void Update()
        {
            foreach (var line in _cells)
            {
                foreach(var cell in line.Cells)
                {
                    cell.Update(Action.None, cell.Value);
                }
            }
        }

        private static int Index(string filename)
        {
            switch (filename)
            {
                case "data.txt": return 0;

                case "a.txt": return 1;
                case "b.txt": return 2;
                default:
                    return -1;
            }
        }
    }







}