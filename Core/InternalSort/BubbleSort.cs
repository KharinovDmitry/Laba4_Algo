using Core.ExternalSort;
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
    public class BubbleSort : IInternalSort
    {
        public List<SortStep> Sort(int[] arr)
        {
            var res = new List<SortStep>();
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr.Length - 1; j++)
                {
                    res.Add(new SortStep(j, j + 1, SortOperation.Compare));
                    if (arr[j] > arr[j + 1])
                    {
                        res.Add(new SortStep(j, j + 1, SortOperation.Swap));
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                    }
                }
            }
            return res;
        }
    }
}