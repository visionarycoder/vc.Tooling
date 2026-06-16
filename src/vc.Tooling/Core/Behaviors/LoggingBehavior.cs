using System;
using System.Threading.Tasks;

namespace VisionaryCoder.Tooling.Core;

    public sealed class LoggingBehavior : IProxyBehavior
    {
        private readonly Action<string> _log;

        public LoggingBehavior(Action<string> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task InvokeAsync(BehaviorContext context, Func<Task> next)
        {
            _log($"[Tooling] → Starting: {context.OperationName}");

            await next();

            _log($"[Tooling] ← Completed: {context.OperationName}");
        }
    }


public sealed class TimingBehavior : IProxyBehavior
    {
        private readonly Action<string> _log;

        public TimingBehavior(Action<string> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task InvokeAsync(BehaviorContext context, Func<Task> next)
        {
            var sw = Stopwatch.StartNew();

            await next();

            sw.Stop();
            _log($"[Tooling] Duration: {context.OperationName} took {sw.ElapsedMilliseconds} ms");
        }
    }


public sealed class ExceptionBehavior : IProxyBehavior
    {
        private readonly Action<Exception> _log;

        public ExceptionBehavior(Action<Exception> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task InvokeAsync(BehaviorContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                _log(ex);
                throw;
            }
        }
    }

    public sealed class ValidationBehavior : IProxyBehavior
    {
        private readonly Func<object?, bool> _validator;

        public ValidationBehavior(Func<object?, bool> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public Task InvokeAsync(BehaviorContext context, Func<Task> next)
        {
            if (!_validator(context.Request))
                throw new InvalidOperationException(
                    $"Validation failed for operation '{context.OperationName}'.");

            return next();
        }
    }

