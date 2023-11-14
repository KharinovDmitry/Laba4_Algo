using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace CoreHelper
{
    public class Logger
    {
        public static ObservableCollection<ExternalSteps> Logs = new ObservableCollection<ExternalSteps>();

        public void AddLog(ExternalSteps step)
        {
            Logs.Add(step);
        }


        public string LogString(ExternalSteps record)
        {
            return (String.Format("[{0}] {1}", record.Action, record.Message));
        }
    }
}
