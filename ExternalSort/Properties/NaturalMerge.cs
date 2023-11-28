using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.ExternalSort;
using CoreHelper.ExternalSort;

namespace CoreHelper.ExternalSort
{
    public class NaturalMergeSort : IExternalSort
    {
        public Logger logger = new Logger();
        //private ObservableCollection<ObservableCollection<Cell>> _cells = new();
        private int _columnNumber = 0;
        private ColumnType _columnType = ColumnType.str;
        string FileInput;
        int _segments = 1;
        private ObservableCollection<CellsLine> _cells;


        public NaturalMergeSort(ObservableCollection<CellsLine> cells)
        {
            _cells = cells;
        }
        public async Task Sort(string filename, ColumnType type, int columnNumber)
        {                
            FileInput = filename;
            _columnNumber = columnNumber;
            _columnType = type;
            switch (type)
            {
                case ColumnType.str:
                    await SortAsString();
                    break;
                case ColumnType.integer:
                    await SortAsInt();
                    break;
            }        
        }

        public async Task SortAsString()
        {
            while (true)
            {
                await SplitToFilesAsString();
                if (_segments == 1)
                {
                    break;
                }
                await MergePairsAsString();
            }
        }
        public async Task SortAsInt()
        {
            
            while (true)
            {
                await SplitToFilesAsInt();
                if (_segments == 1)
                {
                    break;
                }
                await MergePairsAsInt();
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

        private async Task SplitToFilesAsInt()
        {
            _segments = 1;
            int indexInput = 0;
            int indexA = 0;
            int indexB = 0;
            logger.AddLog(new ExternalSteps("Info", "Разделение на два файла"));
            using StreamReader br = new StreamReader(File.OpenRead(FileInput));
            using StreamWriter writerA = new StreamWriter(File.Create("a.txt"));
            using StreamWriter writerB = new StreamWriter(File.Create("b.txt"));
            bool flag = true;
            string str1 = null;
            string str2;
            int element1 = 0;
            int element2 = 0;
            
            while (true)
            {
                indexInput = Math.Min(9, indexInput);
                indexA = Math.Min(9, indexA);
                indexB = Math.Min(9, indexB);
                if (str1 is null)
                {
                    str1 = br.ReadLine();
                    element1 = int.Parse(str1?.Split(";")[_columnNumber]);
                    writerA.WriteLine(str1);
                    _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, element1 );
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);
                    indexA++;
                    indexInput++;
                    indexInput = Math.Min(9, indexInput);
                    indexA = Math.Min(9, indexA);
                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                }

                str2 = br.ReadLine();
                if (str2 is null | String.Compare(str2, "", StringComparison.Ordinal) == 0) break;
                element2 = int.Parse(str2.Split(";")[_columnNumber]);

                if (element1 > element2)
                {
                    //_cells[Index("a.txt")].Cells[indexA - 1].Update(Action.Compare, element1);
                    //_cells[Index(FileInput)].Cells[indexInput].Update(Action.Compare, null);
                    flag = !flag;
                    _segments++;
                    //indexA++;
                    //indexInput++;
                    indexInput = Math.Min(9, indexInput);
                    indexA = Math.Min(9, indexA);

                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                }

                if (flag)
                {
                    writerA.WriteLine(str2);
                    indexInput = Math.Min(9, indexInput);
                    indexA = Math.Min(9, indexA);
                    _cells[Index(FileInput)].Cells[indexInput - 1].Update(Action.Compare, element1);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.Compare, element2);
                    await Task.Delay(500);
                    _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, element2);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);
                    indexA++;
                    indexInput++;
                
                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                }
                else
                {
                    writerB.WriteLine(str2);
                    indexInput = Math.Min(9, indexInput);
                    indexB = Math.Min(9, indexB);
                    _cells[Index(FileInput)].Cells[indexInput - 1].Update(Action.Compare, element1);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.Compare, element2);

                    await Task.Delay(500);

                    _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, element2);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);
                    indexB++;
                    indexInput++;
                    
                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                }

                str1 = str2;
                element1 = element2;
            }
            await Task.Delay(100);
            Update();
            await Task.Delay(100);
            br.Close();
            writerA.Close();
            writerB.Close();
        }
        private async Task MergePairsAsInt()
        {
            int indexInput = 0;
            int indexA = 0;
            int indexB = 0;
            ExAction actionMove = new();
            ExAction actionCompare = new();
            List<ExAction> actions = new();
            using StreamReader readerA = new StreamReader(File.OpenRead("a.txt"));
            using StreamReader readerB = new StreamReader(File.OpenRead("b.txt"));
            using StreamWriter bw = new StreamWriter(File.Create(FileInput));
            int elementA = 0, elementB = 0;
            string strA = null, strB = null;
            bool pickedA = false, pickedB = false, endA = false, endB = false;
            while (!endA || !endB)
            {
                indexInput = Math.Min(9, indexInput);
                indexA = Math.Min(9, indexA);
                indexB = Math.Min(9, indexB);
                if (!endA & !pickedA)
                {
                    strA = readerA.ReadLine();
                    if (strA is null | String.Compare(strA, "", StringComparison.Ordinal) == 0) endA = true;
                    else
                    {
                        elementA = int.Parse(strA.Split(";")[_columnNumber]);
                        pickedA = true;
                    }
                }

                if (!endB & !pickedB)
                {
                    strB = readerB.ReadLine();
                    if (strB is null | String.Compare(strB, "", StringComparison.Ordinal) == 0) endB = true;
                    else
                    {
                        elementB = int.Parse(strB.Split(";")[_columnNumber]);
                        pickedB = true;
                    }
                }

                if (pickedA)
                {
                    if (pickedB)
                    {
                        logger.AddLog(new ExternalSteps("Compare", $"Сравнение {elementA} и {elementB}"));
                        _cells[Index("a.txt")].Cells[indexA].Update(Action.Compare, elementA);
                        _cells[Index("b.txt")].Cells[indexB].Update(Action.Compare, elementB);
                        //indexB++;
                        //indexA++;
                        indexA = Math.Min(9, indexA);
                        indexB = Math.Min(9, indexB);
                        await Task.Delay(1000);
                        Update();
                        await Task.Delay(1000);

                        if (elementA < elementB)
                        {
                            bw.WriteLine(strA);
                            indexA = Math.Min(9, indexA);
                            indexInput = Math.Min(9, indexInput);                            
                            logger.AddLog(new ExternalSteps("Info", $"Запись {elementA}"));
                            pickedA = false; 
                            _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                            _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                            indexInput++;
                            indexA++;

                            await Task.Delay(1000);
                            Update();
                            await Task.Delay(1000);
                        }
                        else
                        {
                            bw.WriteLine(strB);
                            indexA = Math.Min(9, indexA);
                            indexInput = Math.Min(9, indexInput);
                            logger.AddLog(new ExternalSteps("Info", $"Запись {elementB}"));
                            pickedB = false;
                            _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                            _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                            indexInput++;
                            indexB++;
                            
                            await Task.Delay(1000);
                            Update();
                            await Task.Delay(1000);
                        }
                    }
                    else
                    {
                        bw.WriteLine(strA);
                        indexA = Math.Min(9, indexA);
                        indexInput = Math.Min(9, indexInput);
                        logger.AddLog(new ExternalSteps("Info", $"Запись {elementA}"));
                        pickedA = false;
                        _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                        _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                        indexInput++;
                        indexA++;

                        await Task.Delay(1000);
                        Update();
                        await Task.Delay(1000);
                        
                    }
                }
                else
                {
                    bw.WriteLine(strB);
                    indexB = Math.Min(9, indexB);
                    indexInput = Math.Min(9, indexInput);
                    logger.AddLog(new ExternalSteps("Info", $"Запись {elementB}"));
                    pickedB = false;
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                    _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                    indexInput++;
                    indexB++;

                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);

                }
                await Task.Delay(100);
                Update();
                await Task.Delay(100);
            }
        }

        private async Task SplitToFilesAsString()
        {
            int indexA = 0;
            int indexB = 0;
            int indexInput = 0;
            _segments = 1;
            logger.AddLog(new ExternalSteps("Info", "Разделение на два файла"));
            using StreamReader sr = new StreamReader(File.OpenRead(FileInput));
            using StreamWriter writerA = new StreamWriter(File.Create("a.txt"));
            using StreamWriter writerB = new StreamWriter(File.Create("b.txt"));
            bool flag = true;
            string str1 = null;
            string str2;
            string element1 = null;
            string element2;
            while (true)
            {
                indexInput = Math.Min(9, indexInput);
                indexA = Math.Min(9, indexA);
                indexB = Math.Min(9, indexB);

                if (str1 is null)
                {
                    str1 = sr.ReadLine();
                    element1 = str1.Split(";")[_columnNumber];
                    writerA.WriteLine(str1);
                    _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, element1);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);
                    indexA++;
                    indexInput++;
                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                }

                str2 = sr.ReadLine();
               
                if (str2 is null | String.Compare(str2, "", StringComparison.Ordinal) == 0) break;
                element2 = str2.Split(";")[_columnNumber];
                //_cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, element2);
                //indexInput++;

                if (String.CompareOrdinal(element1, element2) > 0)
                {
                    //indexInput = Math.Min(9, indexInput);
                    //indexA = Math.Min(9, indexA);
                    //_cells[Index(FileInput)].Cells[indexInput].Update(Action.Compare, element2);
                    //_cells[Index("a.txt")].Cells[indexA].Update(Action.Compare, element1);
                    flag = !flag;
                    _segments++;
                    //indexA++;
                    //indexInput++;
                }

                if (flag)
                {
                    indexInput = Math.Min(9, indexInput);
                    indexA = Math.Min(9, indexA);
                    writerA.WriteLine(str2);
                    _cells[Index(FileInput)].Cells[indexInput - 1].Update(Action.Compare, element1);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.Compare, element2);
                    await Task.Delay(500);
                    _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, element2);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);
                    indexA++;
                    indexInput++;
                    
                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                  
                }
                else
                {
                    writerB.WriteLine(str2);
                    indexInput = Math.Min(9, indexInput);
                    indexA = Math.Min(9, indexA);                   
                    _cells[Index(FileInput)].Cells[indexInput - 1].Update(Action.Compare, element1);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, element2);

                    await Task.Delay(500);

                    _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, element2);
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, null);

                    indexB++;
                    indexInput++;
                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                }

                str1 = str2;
                element1 = element2;
            }
            await Task.Delay(100);
            Update();
            await Task.Delay(100);
            sr.Close();
            writerA.Close();
            writerB.Close();
        }

        private async Task MergePairsAsString()
        {
            int indexInput = 0;
            int indexA = 0;
            int indexB = 0;
            ExAction actionMove = new();
            ExAction actionCompare = new();
            List<ExAction> actions = new();
            using StreamReader readerA = new StreamReader(File.OpenRead("a.txt"));
            using StreamReader readerB = new StreamReader(File.OpenRead("b.txt"));
            using StreamWriter bw = new StreamWriter(File.Create(FileInput));
            string elementA = null, elementB = null, strA = null, strB = null;
            bool pickedA = false, pickedB = false, endA = false, endB = false;
            while (!endA || !endB)
            {
                indexInput = Math.Min(9, indexInput);
                indexA = Math.Min(9, indexA);
                indexB = Math.Min(9, indexB);
                if (!endA & !pickedA)
                {
                    strA = readerA.ReadLine();
                    if (strA is null | String.Compare(strA, "", StringComparison.Ordinal) == 0) endA = true;
                    else
                    {
                        elementA = strA.Split(";")[_columnNumber];
                        pickedA = true;
                    }
                }

                if (!endB & !pickedB)
                {
                    strB = readerB.ReadLine();
                    if (strB is null | String.Compare(strB, "", StringComparison.Ordinal) == 0) endB = true;
                    else
                    {
                        elementB = strB.Split(";")[_columnNumber];
                        pickedB = true;
                    }
                }

                if (pickedA)
                {
                    if (pickedB)
                    {
                        logger.AddLog(new ExternalSteps("Info", $"Сравнение {elementA} и {elementB}"));
                        _cells[Index("a.txt")].Cells[indexA].Update(Action.Compare, elementA);
                        _cells[Index("b.txt")].Cells[indexB].Update(Action.Compare, elementB);
                        //indexB++;
                        //indexA++;
                        indexA = Math.Min(9, indexA);
                        indexB = Math.Min(9, indexB);
                        await Task.Delay(1000);
                        Update();
                        await Task.Delay(1000);

                       

                        if (String.CompareOrdinal(elementA, elementB) < 0)
                        {
                            indexA = Math.Min(9, indexA);
                            indexInput = Math.Min(9, indexInput);
                            logger.AddLog(new ExternalSteps("Info", $"Запись {elementA}"));
                            pickedA = false;
                            _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                            _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                            indexInput++;
                            indexA++;

                            await Task.Delay(1000);
                            Update();
                            await Task.Delay(1000);
                        }
                        else
                        {
                            bw.WriteLine(strB);
                            indexA = Math.Min(9, indexA);
                            indexInput = Math.Min(9, indexInput);
                            logger.AddLog(new ExternalSteps("Info", $"Запись {elementB}"));
                            pickedB = false;
                            _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                            _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                            indexInput++;
                            indexB++;

                            await Task.Delay(1000);
                            Update();
                            await Task.Delay(1000);
                        }
                    }
                    else
                    {
                        bw.WriteLine(strA);
                        indexA = Math.Min(9, indexA);
                        indexInput = Math.Min(9, indexInput);
                        logger.AddLog(new ExternalSteps("Info", $"Запись {elementA}"));
                        pickedA = false;
                        _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementA);
                        _cells[Index("a.txt")].Cells[indexA].Update(Action.MoveAction, null);
                        indexInput++;
                        indexA++;

                        await Task.Delay(1000);
                        Update();
                        await Task.Delay(1000);
                    }
                }
                else
                {
                    bw.WriteLine(strB);
                    indexB = Math.Min(9, indexB);
                    indexInput = Math.Min(9, indexInput);
                    logger.AddLog(new ExternalSteps("Info", $"Запись {elementB}"));
                    pickedB = false;
                    _cells[Index(FileInput)].Cells[indexInput].Update(Action.MoveAction, elementB);
                    _cells[Index("b.txt")].Cells[indexB].Update(Action.MoveAction, null);
                    indexInput++;
                    indexB++;

                    await Task.Delay(1000);
                    Update();
                    await Task.Delay(1000);
                }
            }

            await Task.Delay(100);
            Update();
            await Task.Delay(100);
        }
        private void Update()
        {
            foreach (var line in _cells)
            {
                foreach (var cell in line.Cells)
                {
                    cell.Update(Action.None, cell.Value);
                }
            }
        }

    }
}
