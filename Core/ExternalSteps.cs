using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreHelper
{
    public class ExternalSteps
    {
        public string Action { get; }
        public string Message { get; }

        public ExternalSteps(string action, string message)
        {
            this.Action = action;
            this.Message = message;
        }

        public static void PrintLog(ExternalSteps step)
        {
            Console.WriteLine(String.Format("[{0}] {1}", step.Action, step.Message));
        }
    }
}
