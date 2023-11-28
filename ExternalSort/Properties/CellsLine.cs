using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CoreHelper.ExternalSort;

namespace CoreHelper.ExternalSort
{
    public class CellsLine : INotifyPropertyChanged
    {
        public string Filename { get; set; }
        //private ObservableCollection<Cell> _cells;
        public ObservableCollection<Cell> Cells { get; set; }

        public CellsLine(int fileN)
        {
            Cells = new ObservableCollection<Cell>();
            for (int i = 0; i < 10; i++)
            {
                Cells.Add(new(0, Action.MoveAction, 0, i));
            }
            Filename = "ASDASDASDASDASD" + fileN.ToString();
        }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
