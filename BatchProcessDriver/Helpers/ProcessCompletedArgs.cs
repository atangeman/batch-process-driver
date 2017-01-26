using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchProcessDriver.Helpers
{
    /// <summary>
    /// Author: Andrew Tangeman
    /// Purpose: Arguments to be passed in event handlers between subroutines or subprocesses.
    /// Notes:
    /// </summary>
    public class ProcessCompletedArgs
    {
        public FireProcessReturnCodes ReturnType { get; set; }
        public string Message { get; set; } = "";

        public ProcessCompletedArgs(FireProcessReturnCodes errorType, string message = "")
        {
            ReturnType = errorType;
            Message = message;
        }
    }
}
