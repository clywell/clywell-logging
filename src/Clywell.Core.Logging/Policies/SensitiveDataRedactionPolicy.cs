using System.Text.RegularExpressions;
using Serilog.Core;
using Serilog.Events;

namespace Clywell.Core.Logging.Policies;

/// <summary>
/// Destructuring policy that redacts sensitive data from log messages.
/// Supports credit cards, social security numbers, passwords, and custom patterns.
/// Can be configured with custom patterns through SensitiveDataRedactionPolicyOptions.
/// </summary>
public sealed partial class SensitiveDataRedactionPolicy : IDestructuringPolicy
{
    private const string RedactedPlaceholder = "***REDACTED***";

    private static readonly Lazy<SensitiveDataRedactionPolicy> DefaultInstance =
        new(() => new SensitiveDataRedactionPolicy());

    /// <summary>
    /// Gets the default policy instance with all standard patterns enabled.
    /// </summary>
    public static SensitiveDataRedactionPolicy Default => DefaultInstance.Value;

    private static readonly List<Regex> DefaultSensitivePatterns =
    [
        CreditCardRegex(),
        SocialSecurityRegex(),
        PasswordRegex(),
        ApiKeyRegex(),
        EmailPasswordRegex()
    ];

    private readonly List<Regex> _patterns;

    /// <summary>
    /// Initializes a new instance with default patterns.
    /// </summary>
    public SensitiveDataRedactionPolicy()
    {
        _patterns = new List<Regex>(DefaultSensitivePatterns);
    }

    /// <summary>
    /// Initializes a new instance with custom configuration.
    /// </summary>
    /// <param name="customPatterns">Custom regex patterns to add.</param>
    /// <param name="includeCreditCard">Include credit card pattern.</param>
    /// <param name="includeSocialSecurity">Include social security pattern.</param>
    /// <param name="includePassword">Include password pattern.</param>
    /// <param name="includeApiKey">Include API key pattern.</param>
    /// <param name="includeEmailPassword">Include email password pattern.</param>
    internal SensitiveDataRedactionPolicy(
        List<Regex>? customPatterns = null,
        bool includeCreditCard = true,
        bool includeSocialSecurity = true,
        bool includePassword = true,
        bool includeApiKey = true,
        bool includeEmailPassword = true)
    {
        _patterns = [];

        if (includeCreditCard)
        {
            _patterns.Add(CreditCardRegex());
        }

        if (includeSocialSecurity)
        {
            _patterns.Add(SocialSecurityRegex());
        }

        if (includePassword)
        {
            _patterns.Add(PasswordRegex());
        }

        if (includeApiKey)
        {
            _patterns.Add(ApiKeyRegex());
        }

        if (includeEmailPassword)
        {
            _patterns.Add(EmailPasswordRegex());
        }

        if (customPatterns != null)
        {
            _patterns.AddRange(customPatterns);
        }
    }
    /// <summary>
    /// Attempts to destructure the value, redacting sensitive data if found.
    /// </summary>
    /// <param name="value">The value to destructure.</param>
    /// <param name="propertyValueFactory">Factory for creating property values.</param>
    /// <param name="result">The destructured result.</param>
    /// <returns>True if destructuring was successful; otherwise, false.</returns>
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        if (value is string stringValue)
        {
            var redacted = RedactSensitiveDataInternal(stringValue);
            if (redacted != stringValue)
            {
                result = new ScalarValue(redacted);
                return true;
            }
        }

        result = null!;
        return false;
    }

    /// <summary>
    /// Redacts sensitive data from the input string using the default policy patterns.
    /// </summary>
    /// <param name="input">The input string to redact.</param>
    /// <returns>The redacted string.</returns>
    public static string RedactSensitiveData(string input)
    {
        return Default.RedactSensitiveDataInternal(input);
    }

    /// <summary>
    /// Redacts sensitive data from the input string using this policy's patterns.
    /// </summary>
    /// <param name="input">The input string to redact.</param>
    /// <returns>The redacted string.</returns>
    public string RedactSensitiveDataInternal(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var redacted = input;
        foreach (var pattern in _patterns)
        {
            redacted = pattern.Replace(redacted, RedactedPlaceholder);
        }

        return redacted;
    }

    [GeneratedRegex(@"\b\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex CreditCardRegex();

    [GeneratedRegex(@"\b\d{3}-\d{2}-\d{4}\b", RegexOptions.Compiled)]
    private static partial Regex SocialSecurityRegex();

    [GeneratedRegex(@"(password|pwd|passwd)[\s:=]+[^\s]+", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PasswordRegex();

    [GeneratedRegex(@"(api[_-]?key|apikey|access[_-]?token)[\s:=]+[^\s]+", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ApiKeyRegex();

    [GeneratedRegex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}[\s:]+[^\s]+", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailPasswordRegex();
}
