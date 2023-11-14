using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreHelper;
using CoreHelper.ExternalSort;

namespace Core.ExternalSort
{
    public class MultipathMergeSort
    {
        private int _timeout = 0;
        public Logger logger = new Logger();
        private int _columnNumber = 0;
        private ColumnType _columnType = ColumnType.str;
        string FileInput = "data.txt";
        int maxWays = 3;
        LineComparer _lineComparer;
        public MultipathMergeSort(string filename, int columnNumber, ColumnType type, int maxWays, int time)
        {
            FileInput = filename;
            _columnNumber = columnNumber;
            _columnType = type;
            _lineComparer = new(_columnNumber, _columnType);
            this.maxWays = maxWays;
            this.SplitToFiles();
            _timeout = time;
        }
        public MultipathMergeSort(string filename, int columnNumber, ColumnType type, int time)
        {
            FileInput = filename;
            _columnNumber = columnNumber;
            _columnType = type;
            _lineComparer = new(_columnNumber, _columnType);
            this.SplitToFiles();
            _timeout = time;
        }
        public MultipathMergeSort(string filename, int columnNumber, int time)
        {
            FileInput = filename;
            _columnNumber = columnNumber;
            _lineComparer = new(_columnNumber, _columnType);
            this.SplitToFiles();
            _timeout = time;
        }
        public void Sort()
        {
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
                if (count == 1)
                {
                    break;
                }
                while (numOfValidInputs > 0)
                {
                    while (numOfValidInputs > 0)
                    {
                        int minElemIndex = IndexOfMin(currElements, isInputValid);

                        outputFiles[currentOutputFileId].WriteLine(currElements[minElemIndex]);
                        logger.AddLog(new ExternalSteps("Write", $"Записываем элемент {currElements[minElemIndex]} в файл f{(!isOrderInversed ? currentOutputFileId + maxWays : currentOutputFileId)}"));
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


        private void SplitToFiles()
        {
            logger.AddLog(new ExternalSteps("Info", $"Разделение на {maxWays} файла(ов)"));
            var origFile = new StreamReader(File.OpenRead(FileInput));
            List<StreamWriter> files = new();
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
            var j = 0;
            for (; ; )
            {
                str1 = str2;
                readenString = origFile.ReadLine();
                if (readenString is null) break;
                str2 = readenString.Split(';')[_columnNumber];
                if (_columnType == ColumnType.integer)
                {
                    if (int.Parse(str1) > int.Parse(str2))
                    {
                        j++;
                    }
                }
                else
                {
                    if (String.CompareOrdinal(str1, str2) > 0)
                    {
                        j++;
                    }
                }

                if (j >= maxWays) j = 0;
                files[j].WriteLine(readenString);
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
    }
}
