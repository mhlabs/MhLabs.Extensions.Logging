using System;
using System.Collections.Generic;
using MhLabs.Extensions.Logging.Internal;
using Microsoft.Extensions.Logging;

namespace MhLabs.Extensions.Logging;

/// <summary>
///     A builder class with methods to setup <see cref="ILogger{TCategoryName}" />.
/// </summary>
public class LoggingSettings
{
    internal LoggingSettings()
    {
        Properties = new Dictionary<string, object>();
        Omissions = new Dictionary<string, LogLevel>();
        StructuredException = false;
        SourceContext = false;
    }

    internal Dictionary<string, object> Properties { get; }
    internal Dictionary<string, LogLevel> Omissions { get; }
    internal bool StructuredException { get; private set; }
    internal bool SourceContext { get; private set; }
    internal bool MessageTemplate { get; private set; }

    /// <summary>
    ///     Omit logs that are below the <paramref name="minimumLevel" /> that originate from <paramref name="source" />.
    /// </summary>
    /// <param name="source">The (partial) namespace or type name to set the override for.</param>
    /// <param name="minimumLevel">The minimum level applied to loggers for matching sources.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="source" /> is <code>null</code></exception>
    public LoggingSettings AddOmission(string source, LogLevel minimumLevel)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        Omissions[source] = minimumLevel;

        return this;
    }

    /// <summary>
    ///     Omit logs that are below the <paramref name="minimumLevel" /> that originate from:
    ///     <code>'AWSSDK','Microsoft','System'</code>
    /// </summary>
    /// <param name="minimumLevel">The minimum level applied to loggers for matching sources.</param>
    public LoggingSettings AddDefaultOmissions(LogLevel minimumLevel = LogLevel.Warning)
    {
        return AddOmission("AWSSDK", minimumLevel)
            .AddOmission("Microsoft", minimumLevel)
            .AddOmission("System", minimumLevel);
    }

    /// <summary>
    ///     Will convert the exception to a structured object instead of a string representation.
    /// </summary>
    /// <code>
    ///  Exception": {
    ///     "Type": "System.ArgumentException",
    ///     "Data": {
    ///         "MyExtraData": 2
    ///     },
    ///     "HResult": -2147024809,
    ///     "Message": "Something went wrong (Parameter 'myParam')",
    ///     "Source": "inbound_prognosis",
    ///     "StackTrace": "...",
    ///     "TargetSite": "...",
    ///     "ParamName": "myParam"
    /// }
    ///  </code>
    public LoggingSettings EnableStructuredException(bool value = true)
    {
        StructuredException = value;

        return this;
    }

    /// <summary>
    ///     Determines whether the property 'MessageTemplate' should be included in every log.
    /// </summary>
    public LoggingSettings EnableMessageTemplate(bool value = true)
    {
        MessageTemplate = value;

        return this;
    }


    /// <summary>
    ///     Determines whether the property 'SourceContext' should be included in every log.
    /// </summary>
    public LoggingSettings EnableSourceContext(bool value = true)
    {
        SourceContext = value;

        return this;
    }

    /// <summary>
    ///     Adds the <paramref name="key" /> and <paramref name="value" /> as a property to each log.
    /// </summary>
    public LoggingSettings AddProperty(string key, object value)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        Properties[key] = value;

        return this;
    }

    /// <summary>
    ///     Uses the default settings.
    /// </summary>
    public LoggingSettings UseDefaultSettings()
    {
        return AddDefaultOmissions()
            .EnableMessageTemplate()
            .EnableSourceContext();
    }

    internal Settings Build()
    {
        return new Settings(StructuredException, SourceContext, MessageTemplate, Properties, Omissions);
    }
}