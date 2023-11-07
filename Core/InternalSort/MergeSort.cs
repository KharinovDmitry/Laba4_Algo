using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InternalSort
{
    internal class MergeSort : IInternalSort
    {
        private List<SortStep> res;

        public List<SortStep> Sort(IComparable[] arr)
        {
            StartSort(arr, 0, arr.Length - 1);
            return res;
        }

        private IComparable[] StartSort(IComparable[] array, int lowIndex, int highIndex)
        {
            if (lowIndex < highIndex)
            {
                var middleIndex = (lowIndex + highIndex) / 2;
                StartSort(array, lowIndex, middleIndex);
                StartSort(array, middleIndex + 1, highIndex);

                Merge(array, lowIndex, middleIndex, highIndex);
            }

            return array;
        }

        private void Merge(IComparable[] array, int lowIndex, int middleIndex, int highIndex)
        {
            var left = lowIndex;
            var right = middleIndex + 1;
            var tempArray = new IComparable[highIndex - lowIndex + 1];
            var index = 0;

            while (left <= middleIndex && right <= highIndex)
            {
                res.Add(new SortStep(left, right, SortOperation.Compare));
                if (array[left].CompareTo(array[right]) < 0)
                {
                    tempArray[index] = array[left];
                    left++;
                }
                else
                {
                    tempArray[index] = array[right];
                    right++;
                }

                index++;
            }

            var c = 0;

            for (var i = left; i <= middleIndex; i++)
            {
                res.Add(new SortStep(lowIndex + c, i, SortOperation.Swap));
                c++;

                tempArray[index] = array[i];
                index++;
            }

            for (var i = right; i <= highIndex; i++)
            {
                res.Add(new SortStep(lowIndex + c, i, SortOperation.Swap));
                c++;

                tempArray[index] = array[i];
                index++;
            }

            for (var i = 0; i < tempArray.Length; i++)
            {
                array[lowIndex + i] = tempArray[i];
            }
        }


    }
}
