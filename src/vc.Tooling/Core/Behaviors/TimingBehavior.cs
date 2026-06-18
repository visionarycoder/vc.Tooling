using System.Diagnostics;

namespace VisionaryCoder.Tooling.Core.Behaviors;

public sealed class TimingBehavior(Action<string> log) : IProxyBehavior
{
    private readonly Action<string> _log = log ?? throw new ArgumentNullException(paramName: nameof(log));

    public async Task InvokeAsync(BehaviorContext context, Func<Task> next)
    {
        var sw = Stopwatch.StartNew();

        await next();

        sw.Stop();
        _log(obj: $"[Tooling] Duration: {context.OperationName} took {sw.ElapsedMilliseconds} ms");
    }
}