using CoreHelper;
using CoreHelper.InternalSort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreHelper;

namespace Core.InternalSort
{
    public class InsertionSort : IInternalSort
    {
        public List<SortStep> Sort(int[] arr)
        {
            var res = new List<SortStep>();
            for (var i = 1; i < arr.Length; i++)
            {
                var key = arr[i];
                var j = i;
                res.Add(new SortStep(j - 1, i, SortOperation.Compare));
                while ((j > 0) && (arr[j - 1] > key))
                {
                    if(res.Count > 1)
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
