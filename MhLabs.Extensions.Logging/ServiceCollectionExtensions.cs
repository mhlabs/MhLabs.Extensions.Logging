using System;
using System.Linq;
using MhLabs.Extensions.Logging.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Templates;

namespace MhLabs.Extensions.Logging;

/// <summary>
///     Contains extension methods for logging.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string ExceptionDetail = "ExceptionDetail";

    private static Settings DefaultSettings => new LoggingSettings().UseDefaultSettings().Build();

    private static LogEventLevel ToLogEventLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.Trace => LogEventLevel.Verbose,
            _ => LogEventLevel.Information
        };
    }

    /// <summary>
    ///     Adds logging dependencies to <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">
    ///     The <see cref="IServiceCollection" /> for the dependencies to be added to.
    /// </param>
    /// <param name="configure">
    ///     An <see cref="Action{T}" /> that can used to configure <see cref="LoggingSettings" />.
    /// </param>
    public static IServiceCollection AddMhLogging(
        this IServiceCollection serviceCollection,
        Action<LoggingSettings>? configure = null
    )
    {
        var configuration = configure is null
            ? DefaultSettings
            : BuildConfiguration(configure);

        var logConfiguration = CreateLoggerConfig(configuration);

        // We could add a none Generic logging (ILogger) option by adding the following in the future.
        //.AddSingleton<ILogger>(x => x.GetRequiredService<ILogger<Logger>>())
        return serviceCollection
            .AddLogging(x => x.AddSerilog(logConfiguration.CreateLogger(), true));

        static Settings BuildConfiguration(Action<LoggingSettings> configure)
        {
            var builder = new LoggingSettings();
            configure(builder);
            return builder.Build();
        }
    }

    // https://nblumhardt.com/2021/06/customize-serilog-json-output/
    // https://github.com/serilog/serilog-expressions
    private static ExpressionTemplate ExpressionTemplate(Settings settings)
    {
        var properties = string.Join(", ", settings.AdditionalProperties.Keys);
        var logStructure = @$"
{{
    Message:@m,
    MessageTemplate:if {settings.IsMessageTemplateEnabled} and @mt <> @m then @mt else undefined(),
    TimeStamp:@t,
    Level:if @l = '{LogEventLevel.Information}' then undefined() else @l,
    SourceContext: if {settings.IsSourceContextEnabled} then SourceContext else undefined(),
    {(string.IsNullOrWhiteSpace(properties) ? null : $"{properties},")} 
    Exception:if {settings.IsStructuredExceptionEnabled} then {ExceptionDetail} else @x, 
    ..rest()
}} 
";
        return new ExpressionTemplate($" {{ {logStructure} }} {Environment.NewLine}");
    }

    private static LoggerConfiguration CreateLoggerConfig(Settings settings)
    {
        var config = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext();

        if (settings.Omissions is {Count: > 0} omissions)
            config = omissions
                .Aggregate(config, (x, y) => x.MinimumLevel.Override(y.Key, ToLogEventLevel(y.Value)));

        if (settings.AdditionalProperties is {Count: > 0} properties)
            config = properties
                .Aggregate(config, (current, property) => current.Enrich.WithProperty(property.Key, property.Value));

        if (settings.IsStructuredExceptionEnabled)
            config = config.Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithRootName(ExceptionDetail)
            );

        return config.WriteTo.Console(ExpressionTemplate(settings));
    }
}