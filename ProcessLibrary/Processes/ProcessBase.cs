namespace ProcessLibrary.Processes
{
    using Events;
    using System;
    using System.Runtime.ExceptionServices;

    /// <summary>
    /// Base class for all Processes to inherit. Provides event handlers to relay messages up to calling method.
    /// </summary>
    /// <remarks>
    /// Author: Andrew Tangeman
    /// Date: 1/19/2017
    /// Notes: [atangeman20170119] - Added event triggers, delegates, and handlers. 
    ///                              ToDo: Add exception handlers and delegates.
    /// </remarks>
    public class ProcessBase
    {
        /// <summary>
        /// Subscribe to this event to receive general messages from the subroutine.
        /// </summary>
        public event ProcessChangedEventHandler OnProcessChangedEvent;

        /// <summary>
        /// Subscribe to this event to receive notifications when process completes.
        /// </summary>
        public event ProcessCompletedEventHandler OnProcessCompleteEvent;

        /// <summary>
        /// Subscribe to this event to receive notifications when process completes.
        /// </summary>
        public event ProcessExceptionEventHandler OnProcessExceptionEvent;

        /// <summary>
        /// Notifies all event subscribers of process event. Use this method to send important
        /// log and exception related information for business logic decisions. Supports "chaining" events of 
        /// the same type together.
        /// </summary>
        /// <param name="sender">Sender invoking the call to the delegate</param>
        /// <param name="e">ProcessEventArgs to pass to event listeners.</param>
        protected virtual void RaiseProcessChangedEvent(object sender, ProcessEventArgs e)
        {
            this.OnProcessChangedEvent(this, e);
        }

        /// <summary>
        /// Notifies all event subscribers of process event. Use this method to send important 
        /// log and exception related information for business logic decisions. 
        /// </summary>
        /// <param name="eventType">Notification level for process.</param>
        /// <param name="message">Message to send as a notification.</param>
        internal virtual void RaiseProcessEvent(ProcessEventTypes eventType, string message)
        {
            if (OnProcessChangedEvent != null)
            {
                ProcessEventArgs args = new ProcessEventArgs(eventType, message);
                OnProcessChangedEvent(this, args);
            }
        }

        /// <summary>
        /// Notifies all event subscribers of process event. 
        /// Use this method to send basic informational messages.
        /// </summary>
        /// <param name="message">Message to send as a notification.</param>
        internal virtual void RaiseLogEvent(string message)
        {
            if (OnProcessChangedEvent != null)
            {
                ProcessEventArgs args = new ProcessEventArgs(ProcessEventTypes.INFO, message);
                OnProcessChangedEvent(this, args);
            }
        }

        /// <summary>
        /// Notifies all event subscribers of process event. Use this method to send important 
        /// log and exception related information for business logic decisions.
        /// </summary>
        /// <param name="message">Message to send as a notification.</param>
        internal virtual void RaiseDebugEvent(string message)
        {
            if (OnProcessChangedEvent != null)
            {
                ProcessEventArgs args = new ProcessEventArgs(ProcessEventTypes.DEBUG, message);
                OnProcessChangedEvent(this, args);
            }
        }

        /// <summary>
        /// Notifies all subscribers when the process completes. Can be used to initiate the start of 
        /// a second process, or check the status of a completed process. Supports "chaining" events of 
        /// the same type together.
        /// </summary>
        /// <param name="sender">Sender invoking the call to the delegate</param>
        /// <param name="e">ProcessCompletedArgs to pass to event listeners.</param>
        protected virtual void ProcessCompletedEventHandler(object sender, ProcessCompletedArgs e)
        {
            this.OnProcessCompleteEvent(this, e);
        }
        

        /// <summary>
        /// Notifies all subscribers when the process completes. Can be used to initiate the start of 
        /// a second process, or check the status of a completed process.
        /// </summary>
        /// <param name="returnCode">Return code corresponding to the status of the process when ended.</param>
        internal virtual void RaiseProcessComplete(FireProcessReturnCodes returnCode, string message = "")
        {
            if (OnProcessCompleteEvent != null)
            {
                ProcessCompletedArgs args = new ProcessCompletedArgs(returnCode, message);
                OnProcessCompleteEvent(this, args);
            }
        }

        /// <summary>
        /// Notifies all event subscribers of process event. Use this method to send important
        /// exception related information for business logic decisions.
        /// </summary>
        /// <param name="ex">Exception encountered by process.</param>
        protected virtual void RaiseProcessException(Exception ex)
        {
            if (this.OnProcessExceptionEvent != null)
            {
                this.OnProcessExceptionEvent.Invoke(this, ex);
                return;
            }
            else
            {
                // allows for capture and re-throw while still retaining stack trace info [atangeman20170228]
                ExceptionDispatchInfo.Capture(ex).Throw(); 
            }
        }

        /// <summary>
        /// Notifies all event subscribers of process event. Use this method to send important
        /// exception related information for business logic decisions.
        /// </summary>
        /// <param name="ex">Exception encountered by process.</param>
        /// <param name="message">Message to send as a notification.</param>
        protected virtual void RaiseProcessException(Exception ex, string message)
        {
            Exception ex2 = new Exception(message, ex);
            if (this.OnProcessExceptionEvent != null)
            {
                this.OnProcessExceptionEvent.Invoke(this, ex2);
            }
            else
            {
                // allows for capture and re-throw while still retaining stack trace info [atangeman20170228]
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }
    }
}
