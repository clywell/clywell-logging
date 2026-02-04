using System.Text.RegularExpressions;

namespace Clywell.Core.Logging.Policies;

/// <summary>
/// Configuration builder for customizing sensitive data redaction patterns.
/// Allows users to add custom patterns or disable default patterns.
/// </summary>
public sealed class SensitiveDataRedactionPolicyOptions
{
    private readonly List<Regex> _customPatterns = [];
    private bool _includeCreditCard = true;
    private bool _includeSocialSecurity = true;
    private bool _includePassword = true;
    private bool _includeApiKey = true;
    private bool _includeEmailPassword = true;

    /// <summary>
    /// Creates a new configuration for sensitive data redaction.
    /// </summary>
    /// <returns>A new configuration instance.</returns>
    public static SensitiveDataRedactionPolicyOptions Create()
    {
        return new SensitiveDataRedactionPolicyOptions();
    }

    /// <summary>
    /// Adds a custom regex pattern for redaction.
    /// </summary>
    /// <param name="pattern">The regex pattern to match sensitive data.</param>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions AddCustomPattern(string pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        _customPatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
        return this;
    }

    /// <summary>
    /// Adds a custom regex pattern for redaction with options.
    /// </summary>
    /// <param name="pattern">The regex pattern to match sensitive data.</param>
    /// <param name="options">Regex options.</param>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions AddCustomPattern(string pattern, RegexOptions options)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        _customPatterns.Add(new Regex(pattern, options | RegexOptions.Compiled));
        return this;
    }

    /// <summary>
    /// Disables credit card pattern redaction.
    /// </summary>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions DisableCreditCardRedaction()
    {
        _includeCreditCard = false;
        return this;
    }

    /// <summary>
    /// Disables social security number pattern redaction.
    /// </summary>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions DisableSocialSecurityRedaction()
    {
        _includeSocialSecurity = false;
        return this;
    }

    /// <summary>
    /// Disables password pattern redaction.
    /// </summary>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions DisablePasswordRedaction()
    {
        _includePassword = false;
        return this;
    }

    /// <summary>
    /// Disables API key pattern redaction.
    /// </summary>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions DisableApiKeyRedaction()
    {
        _includeApiKey = false;
        return this;
    }

    /// <summary>
    /// Disables email password pattern redaction.
    /// </summary>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions DisableEmailPasswordRedaction()
    {
        _includeEmailPassword = false;
        return this;
    }

    /// <summary>
    /// Disables all default patterns.
    /// </summary>
    /// <returns>This configuration instance for chaining.</returns>
    public SensitiveDataRedactionPolicyOptions DisableAllDefaults()
    {
        _includeCreditCard = false;
        _includeSocialSecurity = false;
        _includePassword = false;
        _includeApiKey = false;
        _includeEmailPassword = false;
        return this;
    }

    /// <summary>
    /// Builds the configured redaction policy.
    /// </summary>
    /// <returns>A new SensitiveDataRedactionPolicy instance.</returns>
    public SensitiveDataRedactionPolicy Build()
    {
        return new SensitiveDataRedactionPolicy(
            customPatterns: _customPatterns,
            includeCreditCard: _includeCreditCard,
            includeSocialSecurity: _includeSocialSecurity,
            includePassword: _includePassword,
            includeApiKey: _includeApiKey,
            includeEmailPassword: _includeEmailPassword);
    }
}
