using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InternalSort
{
    internal class InsertionSort : IInternalSort
    {
        public List<SortStep> Sort(IComparable[] arr)
        {
            var res = new List<SortStep>();
            for (var i = 1; i < arr.Length; i++)
            {
                var key = arr[i];
                var j = i;
                while ((j > 1) && (arr[j - 1].CompareTo(key) > 0))
                {
                    res.Add(new SortStep(j - 1, i, SortOperation.Compare));
                    res.Add(new SortStep(j, j - 1, SortOperation.Swap));

                    (arr[j - 1], arr[j]) = (arr[j], arr[j - 1]);
                    j--;
                }
                arr[j] = key;
            }

            return res;
        }
    }
}
