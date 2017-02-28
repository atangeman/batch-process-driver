using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchProcessDriver.Helpers
{
    /// <summary>
    /// Author: Andrew Tangeman
    /// Purpose: Event delegate to enable parent-level subscription to events thrown from subroutines or subprocesses.
    /// Notes:
    /// </summary>
    /// 
    public delegate void ProcessChangedEventHandler(object sender, ProcessEventArgs e);

    /// <summary>
    /// Author: Andrew Tangeman
    /// Purpose: Event delegate to enable parent-level subscription to events thrown from subroutines or subprocesses.
    /// Notes:
    /// </summary>
    public delegate void ProcessCompletedEventHandler(object sender, ProcessCompletedArgs e);

}
