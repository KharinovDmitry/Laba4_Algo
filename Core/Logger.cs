using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreHelper
{
    public class Logger
    {
        public List<ExternalSteps> Logs = new List<ExternalSteps>();

        public void AddLog(ExternalSteps step)
        {
            Logs.Add(step);
        }
    }
}
