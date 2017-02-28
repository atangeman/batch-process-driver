namespace ProcessLibrary.Helpers
{
    /// <summary>
    /// Author: Andrew Tangeman
    /// Purpose: Arguments to be passed in event handlers between subroutines or subprocesses.
    /// Notes:
    /// </summary>
    public class ProcessEventArgs
    {
        /// <summary>
        /// Type of event to attach to event.
        /// </summary>
        public ProcessEventTypes EventType { get; set; }

        /// <summary>
        /// Message to attach to event
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Default constructor for ProcessEventArgs
        /// </summary>
        /// <param name="eventType">Type of event to attach to event.</param>
        /// <param name="message">Message to attach to event.</param>
        public ProcessEventArgs(ProcessEventTypes eventType, string message = "")
        {
            EventType = eventType;
            Message = message;
        }
    }
}
