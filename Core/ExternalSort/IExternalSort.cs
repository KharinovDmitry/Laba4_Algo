using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ExternalSort
{
    public interface IExternalSort
    {
        public List<SortStep> Sort(string path, int key);
    }
}
