using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CoreHelper;

namespace Core.ExternalSort
{
    public enum Action
    {
        Compare,
        MoveAction
    }
    public class ExAction : ICloneable, INotifyPropertyChanged
    {
        public Action Action; 
        public int FromIndex;
        public int ToIndex;
        public string FromFile;
        public string ToFile;
        public object ElementA;
        public object ElementB; 


        public ExAction() { }
        public ExAction(Action action, int fromIndex, int toIndex, string fromFile, string toFile, object elementA, object elementB) 
        {
            Action = action;
            FromIndex = fromIndex;
            ToIndex = toIndex;     
            FromFile = fromFile;
            ToFile = toFile;
            ElementA = elementA;
            ElementB = elementB;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            switch (Action)
            {
                case Action.Compare:
                    return $"Сравниваем {FromIndex} и {ToIndex}";
                case Action.MoveAction:
                    return $"Поменяли местами {FromIndex} и {ToIndex}";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
