using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InternalSort
{
    public interface IInternalSort
    {
        public List<SortStep> Sort(int[] arr);
    }
}
