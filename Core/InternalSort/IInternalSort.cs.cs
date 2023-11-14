using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreHelper.InternalSort
{
    internal interface IInternalSort
    {
        public List<SortStep> Sort(IComparable[] arr);
    }
}
