using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ExternalSort;

namespace CoreHelper.ExternalSort
{
    public interface IExternalSort
    {
        public List<ExAction> Sort(string filename, ColumnType columnType, int columnNumber);
    }
}
