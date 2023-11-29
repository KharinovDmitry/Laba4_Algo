using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.ExternalSort;
using static System.Collections.Specialized.BitVector32;

namespace CoreHelper.ExternalSort
{
    public class MultipathMergeSort : IExternalSort
    {
        
        public Logger logger = new Logger();
        private int _columnNumber = 0;
        private ColumnType _columnType = ColumnType.str;
        string FileInput = "C:\\Users\\terro\\Source\\Repos\\Laba4_Algo\\Core\\ExternalSort\\data.txt";
        int maxWays = 3;
        LineComparer _lineComparer;
        private ObservableCollection<CellsLine> _cells;

        public MultipathMergeSort(ObservableCollection<CellsLine> cells)
        {
            _cells = cells;
        }
       

     
        public async Task Sort(string filename, ColumnType type, int columnNumber)
        {
            FileInput = filename;
            _columnNumber = columnNumber;
            _columnType = type;
            _lineComparer = new(_columnNumber, _columnType);
            await this.SplitToFiles(columnNumber);
            List<StreamReader> inputFiles = new();
            List<StreamWriter> outputFiles = new();
            List<bool> isInputValid = new();
            List<bool> isFileReadable = new();
            List<string?> currElements = new();
            bool isOrderInversed = false;
            int currentOutputFileId = 0;
            int numOfValidInputs = maxWays;
            int count = 0;
            string str1;
            string str2;
            string? readenString;
            int[] arrayindex = new int[10];
            for (int i = 0; i < maxWays; i++)
            {
                inputFiles.Add(new StreamReader($"f{i}.txt"));
                outputFiles.Add(new StreamWriter($"f{i + maxWays}.txt"));
                isInputValid.Add(true);
                isFileReadable.Add(true);
                var temp = inputFiles[i].ReadLine();
                logger.AddLog(new ExternalSteps("Read", $"Считываем элемент {temp} из файла f{i}"));
                if (temp != null)
                {
                    currElements.Add(temp);
                    count++;
                }

            }
            
            while (true)
            {
                Update();
                arrayindex = new int[10];
                if (count == 1)
                {
                    break;
                }
                while (numOfValidInputs > 0)
                {
                    while (numOfValidInputs > 0)
                    {
                        arrayindex[currentOutputFileId] = Math.Min(arrayindex[currentOutputFileId], 9);
                        int minElemIndex = IndexOfMin(currElements, isInputValid);

                        outputFiles[currentOutputFileId].WriteLine(currElements[minElemIndex]);
                        logger.AddLog(new ExternalSteps("Write", $"Записываем элемент {currElements[minElemIndex]} в файл f{(!isOrderInversed ? currentOutputFileId + maxWays : currentOutputFileId)}"));

                        _cells[currentOutputFileId].Cells[arrayindex[currentOutputFileId]++].Update(Action.MoveAction, currElements[minElemIndex].Split(";")[columnNumber]);
                        await Task.Delay(500);
                       


                        string? nextElem = inputFiles[minElemIndex].ReadLine();
                        logger.AddLog(new ExternalSteps("Read", $"Считываем элемент {nextElem} из файла f{(!isOrderInversed ? minElemIndex : minElemIndex + maxWays)}"));

                        if (nextElem is null)
                        {
                            isFileReadable[minElemIndex] = false;
                            isInputValid[minElemIndex] = false;
                            numOfValidInputs--;
                        }
                        else if (_lineComparer.Compare(nextElem, currElements[minElemIndex]) < 0)
                        {

                            isInputValid[minElemIndex] = false;
                            numOfValidInputs--;
                        }

                        currElements[minElemIndex] = nextElem;
                    }


                    for (int i = 0; i < isInputValid.Count; i++)
                    {
                        isInputValid[i] = isFileReadable[i];
                        numOfValidInputs += isInputValid[i] ? 1 : 0;
                    }
                    currentOutputFileId = currentOutputFileId >= maxWays - 1 ? 0 : currentOutputFileId + 1;
                }
                int numsOfNull = 0;
                bool isEnd = false;
                logger.AddLog(new ExternalSteps("Info", $"Меняем входные и выходные файлы местами"));
                for (int i = 0; i < maxWays; i++)
                {
                    inputFiles[i].Close();
                    outputFiles[i].Close();

                    if (isOrderInversed)
                    {

                        inputFiles[i] = new StreamReader($"f{i + maxWays}.txt");
                        outputFiles[i] = new StreamWriter($"f{i}.txt");
                    }
                    else
                    {

                        inputFiles[i] = new StreamReader($"f{i}.txt");
                        outputFiles[i] = new StreamWriter($"f{i + maxWays}.txt");
                    }
                    string? firstElem = inputFiles[i].ReadLine();

                    if (firstElem is null)
                    {
                        isInputValid[i] = false;
                        isFileReadable[i] = false;
                    }
                    else
                    {
                        isInputValid[i] = true;
                        isFileReadable[i] = true;
                    }
                    currElements[i] = firstElem;
                    numsOfNull += currElements[i] == null ? 1 : 0;
                    numOfValidInputs = maxWays - numsOfNull;

                }
                if (numsOfNull == maxWays - 1)
                {
                    for (int i = 0; i < isInputValid.Count; i++)
                        if (isInputValid[i])
                        {
                            inputFiles[i].BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                            inputFiles[i].DiscardBufferedData();
                            string res = inputFiles[i].ReadToEnd();
                            var resFile = new StreamWriter(File.OpenWrite("result.txt"));
                            resFile.Write(res);
                            resFile.Close();
                        }
                    break;
                }
                isOrderInversed = !isOrderInversed;

            }
            for (int i = 0; i < maxWays; i++)
            {
                inputFiles[i].Close();
                outputFiles[i].Close();
            }
        }

        private int IndexOfMin(IList<string> self, List<bool> isInputValid)
        {
            int minValidIndex = isInputValid.IndexOf(true);
            string min = self[minValidIndex];
            int minIndex = minValidIndex;

            for (int i = minValidIndex + 1; i < self.Count; ++i)
            {
                if (isInputValid[i])
                {
                    if (_lineComparer.Compare(self[i], min) < 0)
                    {
                        min = self[i];
                        minIndex = i;
                    }
                }
            }

            return minIndex;
        }


        private async Task SplitToFiles(int columnnumber)
        {
            Update();
            logger.AddLog(new ExternalSteps("Info", $"Разделение на {maxWays} файла(ов)"));
            var origFile = new StreamReader(File.OpenRead(FileInput));
            List<StreamWriter> files = new();
            int indexInput = 0;
            int indexf0 = 0;
            int indexf1 = 0;
            int indexf2 = 0;
            int[] arrayindex = new int[3];
            for (int i = 0; i < maxWays; i++)
            {
                
                files.Add(new StreamWriter(File.Create($"f{i}.txt")));
                File.Create($"f{i + maxWays}.txt").Close();
            }
            string str1;
            string str2;
            string? readenString;
            readenString = origFile.ReadLine();
            str2 = readenString.Split(';')[_columnNumber];
            files[0].WriteLine(readenString);

           

            _cells[0].Cells[indexInput].Update(Action.MoveAction, null);
            _cells[1].Cells[arrayindex[0]].Update(Action.MoveAction, readenString.Split(";")[columnnumber]);
            arrayindex[0]++;
            indexInput++;
            await Task.Delay(500);
            Update();

            var j = 0;
        
            for (; ; )
            {
                indexInput = Math.Min(9, indexInput);
                arrayindex[j] = Math.Min(9, arrayindex[j]);
                str1 = str2;
                readenString = origFile.ReadLine();
                if (readenString is null) break;
                str2 = readenString.Split(';')[_columnNumber];
                
                if (_columnType == ColumnType.integer)
                {
                    _cells[0].Cells[indexInput - 1].Update(Action.Compare, str1);
                    _cells[0].Cells[indexInput].Update(Action.Compare, str2);

                    indexInput = Math.Min(9, indexInput);
                    await Task.Delay(500);
                    Update();
                    if (int.Parse(str1) > int.Parse(str2))
                    {
                        j++;                       
                    }
                }
                else
                {
                    _cells[0].Cells[indexInput - 1].Update(Action.Compare, str1);
                    _cells[0].Cells[indexInput].Update(Action.Compare, str2);

                    indexInput = Math.Min(9, indexInput);
                    await Task.Delay(500);
                    Update();
                    if (String.CompareOrdinal(str1, str2) > 0)
                    {
                        j++;  
                    }
                }

                if (j >= maxWays)
                {
                    j = 0;
                    
                }
                files[j].WriteLine(readenString);

                _cells[0].Cells[indexInput].Update(Action.MoveAction, null);
                _cells[j + 1].Cells[arrayindex[j]].Update(Action.MoveAction, readenString.Split(";")[columnnumber]);
                await Task.Delay(1000);
                Update();
                indexInput++;
                arrayindex[j]++;

            }
            for (var i = 0; i < maxWays; i++)
                files[i].Close();
        }

        public class LineComparer : Comparer<String>
        {
            private int columnNumber;
            ColumnType type;
            public LineComparer(int columnNumber, ColumnType type)
            {
                this.columnNumber = columnNumber;
                this.type = type;
            }

            public override int Compare(String x, String y)
            {
                var str1 = x.Split(';')[columnNumber];
                var str2 = y.Split(';')[columnNumber];
                if (type == ColumnType.integer)
                {
                    return int.Parse(str1).CompareTo(int.Parse(str2));
                }
                else
                {
                    return String.CompareOrdinal(str1, str2);
                }
            }
        }
        private static int Index(string filename)
        {
            switch (filename)
            {
                case "data.txt": return 0;

                case "f0.txt": return 1;
                case "f1.txt": return 2;
                case "f2.txt": return 3;
                default:
                    return -1;
            }
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
