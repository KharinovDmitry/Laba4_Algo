using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public enum SortOperation
    {
        Swap,
        Compare,
        SetValue
    }

    public class SortStep
    {
        public readonly int FromIndex;
        public readonly int ToIndex;
        public readonly SortOperation Operation;

        public SortStep(int fromIndex, int toIndex, SortOperation operation)
        {
            FromIndex = fromIndex; 
            ToIndex = toIndex;
            Operation = operation;
        }
     

        public override string ToString()
        {
            switch (Operation)
            {
                case SortOperation.Swap:
                    return $"Поменяли местами {FromIndex} и {ToIndex}";
                case SortOperation.Compare:
                    return $"Сравниваем {FromIndex} и {ToIndex}";
                case SortOperation.SetValue:
                    return $"В {ToIndex} записываем {FromIndex}";
                default:
                    throw new ArgumentException();
            }
        }

    }
}
