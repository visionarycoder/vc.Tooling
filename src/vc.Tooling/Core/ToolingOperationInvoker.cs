using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisionaryCoder.Tooling.Core.Behaviors;

namespace VisionaryCoder.Tooling.Core
{
    /// <summary>
    /// Executes tooling operations through a behavior pipeline.
    /// </summary>
    public sealed class ToolingOperationInvoker
    {
        private readonly BehaviorPipeline _pipeline;

        public ToolingOperationInvoker(IEnumerable<IProxyBehavior> behaviors)
        {
            if (behaviors is null)
                throw new ArgumentNullException(paramName: nameof(behaviors));

            _pipeline = new BehaviorPipeline(behaviors: behaviors);
        }

        /// <summary>
        /// Executes a tooling operation with the given name and request payload.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="request">The request payload.</param>
        /// <param name="operation">The actual operation to execute.</param>
        public Task InvokeAsync(
            string operationName,
            object? request,
            Func<Task> operation)
        {
            if (string.IsNullOrWhiteSpace(value: operationName))
                throw new ArgumentException(message: "Operation name must be provided.", paramName: nameof(operationName));

            if (operation is null)
                throw new ArgumentNullException(paramName: nameof(operation));

            var context = new BehaviorContext(operationName: operationName, request: request);

            return _pipeline.ExecuteAsync(context: context, terminal: operation);
        }

        /// <summary>
        /// Executes a tooling operation and returns a typed result.
        /// </summary>
        public async Task<T> InvokeAsync<T>(
            string operationName,
            object? request,
            Func<Task<T>> operation)
        {
            if (string.IsNullOrWhiteSpace(value: operationName))
                throw new ArgumentException(message: "Operation name must be provided.", paramName: nameof(operationName));

            if (operation is null)
                throw new ArgumentNullException(paramName: nameof(operation));

            T? result = default;

            var context = new BehaviorContext(operationName: operationName, request: request);

            await _pipeline.ExecuteAsync(
                context: context,
                terminal: async () => { result = await operation(); }
            );

            return result!;
        }
    }
}
