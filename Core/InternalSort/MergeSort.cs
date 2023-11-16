using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreHelper;

namespace Core.InternalSort
{
    public class MergeSort : IInternalSort
    {
        private List<SortStep> res = new List<SortStep>();

        public List<SortStep> Sort(int[] arr)
        {
            StartSort(arr, 0, arr.Length - 1);
            return res;
        }

        private int[] StartSort(int[] array, int lowIndex, int highIndex)
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

        private void Merge(int[] array, int lowIndex, int middleIndex, int highIndex)
        {
            int[] arrCopy = new int[array.Length];
            array.CopyTo(arrCopy, 0);

            var left = lowIndex;
            var right = middleIndex + 1;
            var tempArray = new int[highIndex - lowIndex + 1];
            var index = 0;

            while (left <= middleIndex && right <= highIndex)
            {
                res.Add(new SortStep(left, right, SortOperation.Compare));
                if (array[left] < array[right])
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

            for (var i = left; i <= middleIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = right; i <= highIndex; i++)
            {
                tempArray[index] = array[i];
                index++;
            }

            for (var i = 0; i < tempArray.Length; i++)
            {
                res.Add(new SortStep(tempArray[i], lowIndex + i, SortOperation.SetValue));
                array[lowIndex + i] = tempArray[i];
            }
        }
    }
}
