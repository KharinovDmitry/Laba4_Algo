using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ExternalSort
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private IExternalSort sortingAlgorithm;
        //private Logger logger = new();
        //delegate void SortingDelegate();
        //SortingDelegate sortingMethod;
        int CNumber;
        string methodOfSorting;
        Renderer renderer;
        string path = "data.txt";
        
        ColumnType typeOfSorting;
        public MainWindow()
        {
            InitializeComponent();
            renderer = new(CanvasImage);
            ObservableCollection<ExternalSteps> externalSteps = Logger.Logs;
            logListView.ItemsSource = externalSteps;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (methodOfSorting)
            {
                case "Прямое слияние":
                    sortingAlgorithm = new DirectMerge();
                    break;
                case "Естественное слияние":
                    sortingAlgorithm = new NaturalMergeSort();
                    break;
                case "Многопутевое слияние":
                    break;
                
            }
            renderer.Render(sortingAlgorithm.Sort(path, typeOfSorting, CNumber));
        }

        private void columnNumber_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            CNumber = Convert.ToInt32(columnNumber.Text);
        }

    
    }
}
