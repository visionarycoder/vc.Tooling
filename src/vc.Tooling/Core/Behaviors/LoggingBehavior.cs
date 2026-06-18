namespace VisionaryCoder.Tooling.Core.Behaviors;

    public sealed class LoggingBehavior(Action<string> log) : IProxyBehavior
    {
        private readonly Action<string> _log = log ?? throw new ArgumentNullException(paramName: nameof(log));

        public async Task InvokeAsync(BehaviorContext context, Func<Task> next)
        {
            _log(obj: $"[Tooling] → Starting: {context.OperationName}");

            await next();

            _log(obj: $"[Tooling] ← Completed: {context.OperationName}");
        }
    }