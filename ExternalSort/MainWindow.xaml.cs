using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core.ExternalSort;
using CoreHelper;
using CoreHelper.ExternalSort;
using static System.Net.Mime.MediaTypeNames;

namespace CoreHelper.ExternalSort
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private IExternalSort sortingAlgorithm;
        int CNumber;
        string methodOfSorting;
       
        public ObservableCollection<CellsLine> CellsLines { get; set; }

        string path = "data.txt";
        

        ColumnType typeOfSorting;
        public MainWindow()
        {
            InitializeComponent();
           
            CellsLines = new() { new CellsLine(0), new CellsLine(1), new CellsLine(2), new CellsLine(3) };
            ObservableCollection<ExternalSteps> externalSteps = Logger.Logs;
            logListView.ItemsSource = externalSteps;
            Canvas2.ItemsSource = CellsLines;
            
        }
        private void method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            methodOfSorting = selectedItem.Content.ToString();
        }
        private void type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            string type = selectedItem.Content.ToString();
            switch (type)
            {
                case "Число":
                    typeOfSorting = ColumnType.integer;
                    break;
                case "Строка":
                    typeOfSorting = ColumnType.str;
                    break;
            }              
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (StreamReader st = new StreamReader(path)) 
            {
                int counter = 0;
                while (counter != 10) 
                {
                    string str = st.ReadLine();
                    string value = str.Split(";")[CNumber];
                    CellsLines[0].Cells[counter].Update(Action.None, value);
                    counter++;
                } 
            }
            Logger.Logs.Clear();
            switch (methodOfSorting)
            {
                case "Прямое слияние":
                    sortingAlgorithm = new DirectMerge(CellsLines);
                    break;
                case "Естественное слияние":
                    sortingAlgorithm = new NaturalMergeSort(CellsLines);
                    break;
                case "Многопутевое слияние":
                    sortingAlgorithm = new MultipathMergeSort(path, CNumber, typeOfSorting);
                    break;
                
            }

            await sortingAlgorithm.Sort(path, typeOfSorting, CNumber);
        }

        private void columnNumber_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            CNumber = Convert.ToInt32(columnNumber.Text);
        }
        private void Update()
        {
            foreach (var line in CellsLines)
            {
                foreach (var cell in line.Cells)
                {
                    cell.Update(Action.None, cell.Value);
                }
            }
        }

    }
}


