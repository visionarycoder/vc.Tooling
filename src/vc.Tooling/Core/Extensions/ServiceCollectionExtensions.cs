using Microsoft.Extensions.DependencyInjection;
using VisionaryCoder.Tooling.Core.Behaviors;

namespace VisionaryCoder.Tooling.Core.Extensions
{
    /// <summary>
    /// Dependency injection extensions for Tooling Core.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default tooling pipeline and behaviors to the DI container.
        /// </summary>
        public static IServiceCollection AddToolingPipeline(
            this IServiceCollection services,
            Action<string> log,
            Action<Exception> logException,
            Func<object?, bool>? validator = null)
        {
            if (services is null)
                throw new ArgumentNullException(paramName: nameof(services));

            if (log is null)
                throw new ArgumentNullException(paramName: nameof(log));

            if (logException is null)
                throw new ArgumentNullException(paramName: nameof(logException));

            // Register behaviors in deterministic order
            services.AddSingleton<IProxyBehavior>(implementationInstance: new ExceptionBehavior(log: logException));
            services.AddSingleton<IProxyBehavior>(implementationInstance: new LoggingBehavior(log: log));
            services.AddSingleton<IProxyBehavior>(implementationInstance: new TimingBehavior(log: log));

            if (validator is not null)
                services.AddSingleton<IProxyBehavior>(implementationInstance: new ValidationBehavior(validator: validator));

            // Register the invoker using the DefaultPipelineFactory
            services.AddSingleton<ToolingOperationInvoker>(implementationFactory: sp =>
            {
                var behaviors = sp.GetServices<IProxyBehavior>();
                return new ToolingOperationInvoker(behaviors: behaviors);
            });

            return services;
        }
    }
}
