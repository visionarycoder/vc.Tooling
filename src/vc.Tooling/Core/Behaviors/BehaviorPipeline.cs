namespace VisionaryCoder.Tooling.Core.Behaviors
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
                : new List<IProxyBehavior>(collection: behaviors);
        }

        /// <summary>
        /// Executes the pipeline for the given context and final operation.
        /// </summary>
        public Task ExecuteAsync(BehaviorContext context, Func<Task> terminal)
        {
            if (context is null)
                throw new ArgumentNullException(paramName: nameof(context));

            if (terminal is null)
                throw new ArgumentNullException(paramName: nameof(terminal));

            return InvokeNextAsync(index: 0, context: context, terminal: terminal);
        }

        private Task InvokeNextAsync(int index, BehaviorContext context, Func<Task> terminal)
        {
            if (index >= _behaviors.Count)
                return terminal();

            var behavior = _behaviors[index: index];

            return behavior.InvokeAsync(
                context: context,
                next: () => InvokeNextAsync(index: index + 1, context: context, terminal: terminal)
            );
        }
    }
}
