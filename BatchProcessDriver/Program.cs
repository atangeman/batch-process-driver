
namespace BatchProcessDriver
{
    using Helpers;
    using ProcessLibrary.Helpers;
    using ProcessLibrary.Processes;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    class Program
    {
        /// <summary>
        /// Basic enum type for menu options
        /// </summary>
        /// 
        private enum CmdTypes { Quit = 0, LoadProcessQueue, PrintProcessQueue, Options };
        /// <summary>
        /// Private queue to manage the running of tasks in order added
        /// </summary>
        private static Queue<IProcess> m_ProcessQueue;

        /// <summary>
        /// Boolean set to true if user opens the console app.
        /// </summary>
        private static bool m_LiveConsoleMode = false;

        private static CmdTypes Selection { get; set; }

        static void Main(string[] args)
        {
            m_ProcessQueue = new Queue<IProcess>();
            if (args.Length < 1) { StartPrompt(); }
            else
            {
                foreach (string arg in args)
                {
                    // do some magical shit here
                }
            }
        }
        /// <summary>
        /// Method to prompt the user to load Process queue with process objects to run
        /// </summary>
        private static void LoadProcessQueue()
        {
            Console.WriteLine();
            m_LiveConsoleMode = true; // Console window must be open, so this should be true
            ProcessTypes procType;
            //-- User input control logic --
            procType = ConsoleHelpers.ReadEnum<ProcessTypes>("Select a process to add to queue: "); // use console helper to prompt user
            switch (procType) // cliche switch statement
            {
                case ProcessTypes.PROCESS1:
                    //m_ProcessQueue.Enqueue(new RefreshProcess());
                    Console.WriteLine("Process added to queue");
                    break;
                case ProcessTypes.PROCESS2:
                    //m_ProcessQueue.Enqueue(new RoadNetProcess());
                    Console.WriteLine("Process added to queue");
                    break;
                case ProcessTypes.PROCESS3:
                    //m_ProcessQueue.Enqueue(new QAReportProcess());
                    Console.WriteLine("Process added to queue");
                    break;
                default:
                    Console.WriteLine("not a valid process");
                    break;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Starts generic process of type IProcess.
        /// </summary>
        /// <param name="proc">Generic Process of type IProcess</param>
        private static void StartProcess(IProcess proc)
        {
            try
            {
                Console.WriteLine();
                ConsoleHelpers.WriteBorder(); // uses consolehelper method to write a neat ascii border
                Console.WriteLine(proc.ProcessName);
                ConsoleHelpers.WriteBorder(); // uses consolehelper method to write a neat ascii border
                Console.WriteLine();

                proc.OnProcessChangedEvent += ProcessController_OnProcessChangedEvent; // subscribe to process events
                proc.OnProcessCompleteEvent += ProcessController_OnProcessCompleteEvent; // subscribe to close event
                proc.StartProcess();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"The following exception was encountered when attempting to run the process with the name {proc.ProcessName}");
                Console.WriteLine(ex.Message);
                //ToDo: Add call to logging method.
            }
        }

        /// <summary>
        /// Delegate handler for messages returned from RefreshProcess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ProcessController_OnProcessChangedEvent(object sender, ProcessEventArgs e)
        {
            if (m_LiveConsoleMode)
                Console.WriteLine($"{sender}:: {e.Message}"); // write out to console if open

            // ToDo: Add call to Pearson's logging library here
        }

        /// <summary>
        /// Delegate handler for messages returned from RefreshProcess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ProcessController_OnProcessCompleteEvent(object sender, ProcessCompletedArgs e)
        {
            if (m_LiveConsoleMode)
                Console.WriteLine($"{sender}:: {e.Message}"); // write out to console if open
            if (e.ReturnType == FireProcessReturnCodes.SUCCESS) // if previous job was successful, then proceed to next job in queue
            {
                IProcess nextProcess = m_ProcessQueue.Dequeue();
                StartProcess(nextProcess);
            }
            // ToDo: Add call to Pearson's logging library here
        }

        /// <summary>
        /// Method to prompt the user for input if no command-line arguments are provided.
        /// </summary>
        private static void StartPrompt()
        {
            m_LiveConsoleMode = true; // Console window must be open, so this should be true

            PrintBanner(); // print a cool banner to greet our users

            //-- User input control logic --
            bool exitPrompt = false;
            do
            {
                Selection = ConsoleHelpers.ReadEnum<CmdTypes>("Select an Action: "); // use console helper to prompt user
                switch (Selection) // cliche switch statement
                {
                    case CmdTypes.LoadProcessQueue:
                        LoadProcessQueue();
                        break;
                    case CmdTypes.PrintProcessQueue:
                        PrintProcessQueue();
                        break;
                    case CmdTypes.Quit:
                        exitPrompt = true;
                        break;
                    default:
                        break;
                }
            } while (Selection != CmdTypes.Quit && exitPrompt == false); // Keep prompting user until they make a decision.

            EndPrompt(true); // end console session
        }

        /// <summary>
        /// Prints all active processes to console if console window active.
        /// </summary>
        private static void PrintProcessQueue()
        {
            Console.WriteLine();
            Console.WriteLine("Current Processes in Queue: ");
            foreach (IProcess proc in m_ProcessQueue)
            {
                Console.WriteLine("{proc.ProcessName}");
            }
            Console.WriteLine("Number of elements in the Queue: {0}", m_ProcessQueue.Count);
            Console.WriteLine();
        }

        /// <summary>
        /// Prints super neat banner with program info displayed.
        /// </summary>
        private static void PrintBanner()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            ConsoleHelpers.WriteBorder(); // uses consolehelper method to write a neat ascii border
            Console.WriteLine($"---- {asm.FullName.Split(',')[0]}"); // print crappy banner, needs more ascii
            Console.WriteLine($"---- {asm.FullName.Split(',')[1].Replace('=', ' ').TrimStart()}"); // print crappy banner, needs more ascii
            ConsoleHelpers.WriteBorder();
        }

        /// <summary>
        /// Method to control exit during interactive console operation.
        /// </summary>
        static void EndPrompt(bool promptOnExit = false)
        {
            if (promptOnExit)
            {
                Console.Write("\nPress any key to exit...");
                Console.ReadLine();
            }
            else
            {
                Console.Write("\nApp will exit in 10 seconds...");
                Thread.Sleep(5000);
            }
        }
    }
}
