using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreHelper;

namespace Core.ExternalSort
{
    public enum Action
    {
        Compare,
        Move
    }
    public class ExAction : ICloneable
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
                case Action.Move:
                    return $"Поменяли местами {FromIndex} и {ToIndex}";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
