namespace ProcessExample
{
    using BatchProcessLibrary.Processes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Example process to demonstrate use of class library
    /// </summary>
    public class ProcessExampleMain : ProcessBase, IProcess
    {
        public bool IsRunning { get; private set; }

        public string ProcessName { get; private set; }

        public void StartProcess()
        {
            throw new NotImplementedException();
        }

        public void StopProcess()
        {
            throw new NotImplementedException();
        }
    }
}
