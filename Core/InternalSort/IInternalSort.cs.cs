using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InternalSort
{
    internal interface IInternalSort
    {
        public List<SortStep> Sort(string path, int key);
    }
}
