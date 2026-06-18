namespace VisionaryCoder.Tooling.Core.Behaviors
{
    public interface IProxyBehavior
    {
        /// <summary>
        /// Executes the behavior around a tooling operation.
        /// </summary>
        /// <param name="context">The behavior context for this operation.</param>
        /// <param name="next">The next behavior in the chain.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InvokeAsync(BehaviorContext context, Func<Task> next);
    }
}
