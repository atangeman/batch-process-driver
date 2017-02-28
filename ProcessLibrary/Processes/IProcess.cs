namespace ProcessLibrary.Processes
{
    using Events;
    /// <summary>
    /// Author: Andrew Tangeman
    /// Purpose: Interface to use as a template for building Process classes
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Name for process when reporting to message and debug delegates.
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// Subscribe to this event to receive general messages from the subroutine.
        /// </summary>
        event ProcessChangedEventHandler OnProcessChangedEvent;

        /// <summary>
        /// Subscribe to this event to receive notifications when process completes.
        /// </summary>
        event ProcessCompletedEventHandler OnProcessCompleteEvent;

        /// <summary>
        /// Boolean set to true if process is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the process or subroutine.
        /// </summary>
        void StartProcess();

        /// <summary>
        /// Stops the process or subroutine.
        /// </summary>
        void StopProcess();
    }
}
