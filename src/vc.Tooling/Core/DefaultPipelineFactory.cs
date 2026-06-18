using System;
using System.Collections.Generic;
using VisionaryCoder.Tooling.Core.Behaviors;

namespace VisionaryCoder.Tooling.Core
{
    /// <summary>
    /// Creates the default behavior pipeline for tooling operations.
    /// </summary>
    public static class DefaultPipelineFactory
    {
        /// <summary>
        /// Creates a ToolingOperationInvoker with the standard behavior set.
        /// </summary>
        /// <param name="log">A logging delegate for text messages.</param>
        /// <param name="logException">A logging delegate for exceptions.</param>
        /// <param name="validator">Optional request validator. If null, validation is skipped.</param>
        public static ToolingOperationInvoker Create(
            Action<string> log,
            Action<Exception> logException,
            Func<object?, bool>? validator = null)
        {
            if (log is null)
                throw new ArgumentNullException(paramName: nameof(log));

            if (logException is null)
                throw new ArgumentNullException(paramName: nameof(logException));

            var behaviors = new List<IProxyBehavior>
            {
                new ExceptionBehavior(log: logException),
                new LoggingBehavior(log: log),
                new TimingBehavior(log: log)
            };

            if (validator is not null)
                behaviors.Add(item: new ValidationBehavior(validator: validator));

            return new ToolingOperationInvoker(behaviors: behaviors);
        }
    }
}
