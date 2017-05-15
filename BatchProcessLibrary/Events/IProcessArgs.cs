// <copyright file="IProcessArgs.cs" company="City of San Diego">
// Copyright (c) City of San Diego. All rights reserved.
// </copyright>

namespace BatchProcessLibrary.Events
{
    /// <summary>
    /// Generic interface for passing arguments to Process event handlers.
    /// </summary>
    public interface IProcessArgs
    {
        /// <summary>
        /// Gets string message to pass to event handler / delegate.
        /// </summary>
        string Message { get; }
    }
}