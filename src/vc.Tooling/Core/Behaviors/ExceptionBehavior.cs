namespace VisionaryCoder.Tooling.Core.Behaviors;

public sealed class ExceptionBehavior(Action<Exception> log) : IProxyBehavior
{
    private readonly Action<Exception> _log = log ?? throw new ArgumentNullException(paramName: nameof(log));

    public async Task InvokeAsync(BehaviorContext context, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            _log(obj: ex);
            throw;
        }
    }
}