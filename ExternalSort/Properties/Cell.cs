using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CoreHelper.ExternalSort
{

    public enum Action
    {
        Compare,
        MoveAction,
        None
    }
    public class Cell : INotifyPropertyChanged
    {

        private int fileN;
        private Action action;
        private object _value;
        private int width;
        private int height;
        public int FileN { get { return fileN; } private set { fileN = value; NotifyPropertyChanged(nameof(FileN)); } }
        public Action Action { get { return action; } private set { action = value; NotifyPropertyChanged(nameof(Action)); NotifyPropertyChanged(nameof(Color)); } }
        public object Value { get { return _value; } private set { _value = value; NotifyPropertyChanged(nameof(Value)); } }
        public SolidColorBrush Color { get { switch (Action)
                {
                    case Action.Compare:
                        return new SolidColorBrush(Colors.Orange);
                    case Action.MoveAction:
                        return new SolidColorBrush(Colors.Blue);
                    default:
                        return new SolidColorBrush(Colors.White);
                }
            } private set { _ = value; NotifyPropertyChanged(nameof(Color)); } }
        public int Width { get { return width; } private set { width = value; NotifyPropertyChanged(nameof(Width)); } }
        public int Height { get { return height; } private set { height = value; NotifyPropertyChanged(nameof(Height)); } }
        public int Num { get; set; }

        public Thickness Margin { get { return new Thickness(Convert.ToDouble(width * Num), 0, 0, 0);    } }

        public Cell(int fileN, Action action, object value, int num)
        {
            FileN = fileN;
            Action = action;
            Value = value;
            Width = 32;
            Height = 32;
            Num = num;
        }

        public void Update(Action action, object value)
        {
            Action = action;
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
