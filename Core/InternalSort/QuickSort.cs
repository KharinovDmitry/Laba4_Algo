using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InternalSort
{
    internal class QuickSort : IInternalSort
    {
        private List<SortStep> res;

        public List<SortStep> Sort(IComparable[] arr)
        {
            QuickSortStart(arr, 0, arr.Length - 1);
            return res;
        }

        private int Partition(IComparable[] array, int minIndex, int maxIndex)
        {
            var pivot = minIndex - 1;
            for (var i = minIndex; i < maxIndex; i++)
            {
                res.Add(new SortStep(i, maxIndex, SortOperation.Compare));

                if (array[i].CompareTo(array[maxIndex]) < 0)
                {
                    pivot++;
                    res.Add(new SortStep(i, pivot, SortOperation.Swap));
                    (array[pivot], array[i]) = (array[i], array[pivot]);
                }
            }

            pivot++;
            (array[pivot], array[maxIndex]) = (array[maxIndex], array[pivot]);
            res.Add(new SortStep(maxIndex, pivot, SortOperation.Swap));
            return pivot;
        }

        private IComparable[] QuickSortStart(IComparable[] array, int minIndex, int maxIndex)
        {
            if (minIndex >= maxIndex)
            {
                return array;
            }

            var pivotIndex = Partition(array, minIndex, maxIndex);
            QuickSortStart(array, minIndex, pivotIndex - 1);
            QuickSortStart(array, pivotIndex + 1, maxIndex);

            return array;
        }
    }
}
