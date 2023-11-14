using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreHelper
{
    public enum SortOperation
    {
        Swap,
        Compare
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
    }
}
