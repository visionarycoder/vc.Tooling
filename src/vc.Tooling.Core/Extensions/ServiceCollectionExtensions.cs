using System;
using Microsoft.Extensions.DependencyInjection;

namespace VisionaryCoder.Tooling.Core
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
                throw new ArgumentNullException(nameof(services));

            if (log is null)
                throw new ArgumentNullException(nameof(log));

            if (logException is null)
                throw new ArgumentNullException(nameof(logException));

            // Register behaviors in deterministic order
            services.AddSingleton<IProxyBehavior>(new ExceptionBehavior(logException));
            services.AddSingleton<IProxyBehavior>(new LoggingBehavior(log));
            services.AddSingleton<IProxyBehavior>(new TimingBehavior(log));

            if (validator is not null)
                services.AddSingleton<IProxyBehavior>(new ValidationBehavior(validator));

            // Register the invoker using the DefaultPipelineFactory
            services.AddSingleton<ToolingOperationInvoker>(sp =>
            {
                var behaviors = sp.GetServices<IProxyBehavior>();
                return new ToolingOperationInvoker(behaviors);
            });

            return services;
        }
    }
}
