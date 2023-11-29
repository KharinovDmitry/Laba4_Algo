using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Core;
using Core.InternalSort;
using CoreHelper;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using CoreHelper;

namespace InternalSort
{
    internal class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private const int height = 400;
        public int Length { get; set; }
        public string Algorithm { get; set; }
        public int Delay { get; set; }

        private int operations;
        public int Operations
        {
            get
            {
                return operations;
            }
            set
            {
                operations = value;
                OnPropertyChanged(nameof(Operations));
            }
        }

        public ObservableCollection<string> Algorithms { get; }

        private ObservableCollection<string> steps;
        public ObservableCollection<string> Steps
        {
            get { return steps; }
            private set
            {
                steps = value;
                OnPropertyChanged(nameof(Steps));
            }
        }

        private ObservableCollection<ElementVM> elements;
        public ObservableCollection<ElementVM> Elements
        {
            get { return elements; }
            private set
            {
                elements = value;
                OnPropertyChanged(nameof(Elements));
            }
        }

        private bool isRunning = false;

        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }

        public MainWindowVM()
        {
            Start = new RelayCommandAsync(start);
            Stop = new RelayCommandAsync(stop);
            Algorithms = new ObservableCollection<string>()
                {
                    "Сортировка пузырьком",
                    "Сортировка вставками",
                    "Сортировка слиянием",
                    "Быстрая сортировка"
                };
            Algorithm = Algorithms[1];
            Steps = new ObservableCollection<string>();
            Elements= new ObservableCollection<ElementVM>();
            Length = 10;
            Delay = 50;
        }
        private async Task stop()
        {
            isRunning = false;
        }
        private async Task start()
        {
            isRunning = true;
            var arr = GenArray();
            DrawArray(arr);

            int[] tmpArr = new int[arr.Length];

            arr.CopyTo(tmpArr, 0);
            var logs = GetSelectedSort().Sort(tmpArr);
            Steps.Clear();
            Operations = 0;

            foreach (var log in logs)
            {
                if (!isRunning)
                    break;

                var waveOut = new WaveOutEvent();
                
                Steps.Insert(0, log.ToString());
                Operations++;

                if (log.Operation == SortOperation.Swap)
                {
                    (arr[log.FromIndex], arr[log.ToIndex]) = (arr[log.ToIndex], arr[log.FromIndex]);
                    DrawArray(arr);

                    Elements[log.FromIndex].SetGreen();
                    Elements[log.ToIndex].SetGreen();

                    var waveProvider = new SignalGenerator()
                    {
                        Gain = 0.05,
                        Frequency = arr[log.FromIndex] * 15,
                        Type = SignalGeneratorType.SawTooth,
                    };
                    waveOut.Init(waveProvider);
                } 
                else if(log.Operation == SortOperation.Compare)
                {
                    Elements[log.FromIndex].SetYellow();
                    Elements[log.ToIndex].SetYellow();

                    var waveProvider = new SignalGenerator()
                    {
                        Gain = 0.05,
                        Frequency = arr[log.FromIndex] * 15,
                        Type = SignalGeneratorType.SawTooth,
                    };
                    waveOut.Init(waveProvider);
                }
                else
                {
                    arr[log.ToIndex] = log.FromIndex;
                    DrawArray(arr);

                    Elements[log.ToIndex].SetGreen();

                    var waveProvider = new SignalGenerator()
                    {
                        Gain = 0.05,
                        Frequency = arr[log.ToIndex] * 15,
                        Type = SignalGeneratorType.SawTooth,
                    };
                    waveOut.Init(waveProvider);
                }

                waveOut.Play();
                await Task.Delay(1100 - Delay * 10);
                waveOut.Stop();

                if(log.Operation != SortOperation.SetValue)
                    Elements[log.FromIndex].SetColorByValue(arr[log.FromIndex]);
                Elements[log.ToIndex].SetColorByValue(arr[log.ToIndex]);
            }
        }

        private void DrawArray(int[] array)
        {
            Elements.Clear();
            for (int i = 0; i < array.Length; i++)
            {

                var el = new ElementVM(i * 10, i * 10, height, height - (array[i] * 4 + 4));
                el.SetColorByValue(array[i]);
                Elements.Add(el);
            }
        }

        private IInternalSort GetSelectedSort()
        {
            switch (Algorithm)
            {
                case "Сортировка пузырьком":
                    return new BubbleSort();
                case "Сортировка вставками":
                    return new InsertionSort();
                case "Сортировка слиянием":
                    return new MergeSort();
                case "Быстрая сортировка":
                    return new QuickSort();
                default: 
                    throw new ArgumentException(Algorithm);
            }
        }

        private int[] GenArray() 
        {
            int[] array = new int[Length];
            for (int i = 0; i < Length; i++)
            {
                array[i] = new Random().Next(1, 100);
            }
            return array;
        }
    }

    internal class ElementVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private int x1;
        public int X1 { 
            get { return x1; }
            private set {
                x1 = value;
                OnPropertyChanged(nameof(X1));
            }
        }
        private int x2;
        public int X2
        {
            get { return x2; }
            private set
            {
                x2 = value;
                OnPropertyChanged(nameof(X2));
            }
        }

        private int y1;
        public int Y1
        {
            get { return y1; }
            private set
            {
                y1 = value;
                OnPropertyChanged(nameof(Y1));
            }
        }
        private int y2;
        public int Y2
        {
            get { return y2; }
            private set
            {
                y2 = value;
                OnPropertyChanged(nameof(Y2));
            }
        }

        private SolidColorBrush colour;
        public SolidColorBrush Colour
        {
            get { return colour; }
            private set { 
                colour = value;
                OnPropertyChanged(nameof(Colour));
            }
        }

        public ElementVM(int x1, int x2, int y1, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Colour = new SolidColorBrush(Color.FromRgb(0,0,0));
        }

        public void SwapWith(ElementVM el)
        {
            (el.x1, x1) = (x1, el.x1);
            (el.x2, x2) = (x2, el.x2);
            (el.y1, y1) = (y1, el.y1);
            (el.y2, y2) = (y2, el.y2);
        }

        public void SetBlack()
        {
            Colour = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        public void SetGreen()
        {
            Colour = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }
        public void SetYellow()
        {
            Colour = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        }

        public void SetColorByValue(int value)
        {
            byte val = (byte)(value * 2.55f);
            Colour = new SolidColorBrush(Color.FromRgb(0, val, val));
        }
    }
}
