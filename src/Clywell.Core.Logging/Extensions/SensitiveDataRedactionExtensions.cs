using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Clywell.Core.Logging.Policies;

namespace Clywell.Core.Logging.Extensions;

/// <summary>
/// Extension methods for registering sensitive data redaction policies with dependency injection.
/// </summary>
public static class SensitiveDataRedactionExtensions
{
    /// <summary>
    /// Adds the default sensitive data redaction policy to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSensitiveDataRedaction(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IDestructuringPolicy>(SensitiveDataRedactionPolicy.Default);
        return services;
    }

    /// <summary>
    /// Adds a configured sensitive data redaction policy to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for the redaction policy.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSensitiveDataRedaction(
        this IServiceCollection services,
        Action<SensitiveDataRedactionPolicyOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var policyConfig = SensitiveDataRedactionPolicyOptions.Create();
        configure(policyConfig);
        var policy = policyConfig.Build();

        services.AddSingleton<IDestructuringPolicy>(policy);
        return services;
    }
}
