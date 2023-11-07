using Core.ExternalSort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InternalSort
{
    internal class BubbleSort : IInternalSort
    {
        public List<SortStep> Sort(IComparable[] arr)
        {
            var res = new List<SortStep>();
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr.Length - 1; j++)
                {
                    res.Add(new SortStep(i, i + 1, SortOperation.Compare));
                    if (arr[i].CompareTo(arr[i + 1]) > 0)
                    {
                        res.Add(new SortStep(i, i + 1, SortOperation.Compare));
                        (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]);
                    }
                }
            }
            return res;
        }
    }
}