namespace VisionaryCoder.Tooling.Core.Behaviors;

public sealed class ValidationBehavior(Func<object?, bool> validator) : IProxyBehavior
{
    private readonly Func<object?, bool> _validator = validator ?? throw new ArgumentNullException(paramName: nameof(validator));

    public Task InvokeAsync(BehaviorContext context, Func<Task> next)
    {
        if (!_validator(arg: context.Request))
            throw new InvalidOperationException(
                message: $"Validation failed for operation '{context.OperationName}'.");

        return next();
    }
}