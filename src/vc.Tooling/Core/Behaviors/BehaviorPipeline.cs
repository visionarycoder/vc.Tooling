using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VisionaryCoder.Tooling.Core
{
    /// <summary>
    /// Executes a sequence of behaviors around a tooling operation.
    /// </summary>
    public sealed class BehaviorPipeline
    {
        private readonly IReadOnlyList<IProxyBehavior> _behaviors;

        public BehaviorPipeline(IEnumerable<IProxyBehavior> behaviors)
        {
            _behaviors = behaviors is IReadOnlyList<IProxyBehavior> list
                ? list
                : new List<IProxyBehavior>(behaviors);
        }

        /// <summary>
        /// Executes the pipeline for the given context and final operation.
        /// </summary>
        public Task ExecuteAsync(BehaviorContext context, Func<Task> terminal)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (terminal is null)
                throw new ArgumentNullException(nameof(terminal));

            return InvokeNextAsync(0, context, terminal);
        }

        private Task InvokeNextAsync(int index, BehaviorContext context, Func<Task> terminal)
        {
            if (index >= _behaviors.Count)
                return terminal();

            var behavior = _behaviors[index];

            return behavior.InvokeAsync(
                context,
                () => InvokeNextAsync(index + 1, context, terminal)
            );
        }
    }
}
