namespace BatchProcessLibrary.Events
{
    /// <summary>
    /// Enumtype with levels of notification for assigned process arguments.
    /// </summary>
    public enum ProcessEventTypes { DEBUG = 0, INFO = 1, PROCESS_START = 2, PROCESS_COMPLETE = 3, WARNING = 4, EXCEPTION = 5 }

    /// <summary>
    /// Enumtype with levels of notification for assigned process arguments.
    /// </summary>
    public enum FireProcessReturnCodes { SUCCESS = 0, GENERAL_FAILURE = 1, PROCESS_TIMEOUT = 2, UNEXPECTED_SHUTDOWN = 3, LICENSE_CHECKOUT_ERROR = 4 }
}
