using FluentAssertions;
using MhLabs.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace UnitTests;

public class LoggerTests
{
    private readonly ILogger<LoggerTests> _logger = new ServiceCollection()
        .AddMhLogging()
        .BuildServiceProvider()
        .GetRequiredService<ILogger<LoggerTests>>();

    [Fact]
    public void Logger_LogInformation_DoesNotThrow()
    {
        var act = () => _logger.LogInformation("Abc");

        act.Should().NotThrow();
    }

    [Fact]
    public void Logger_LogWarning_DoesNotThrow()
    {
        var act = () => _logger.LogWarning("Abc");

        act.Should().NotThrow();
    }

    [Fact]
    public void Logger_LogError_DoesNotThrow()
    {
        var act = () => _logger.LogError("Abc");

        act.Should().NotThrow();
    }

    [Fact]
    public void Logger_LogCritical_DoesNotThrow()
    {
        var act = () => _logger.LogCritical("Abc");

        act.Should().NotThrow();
    }

    [Fact]
    public void Logger_LogDebug_DoesNotThrow()
    {
        var act = () => _logger.LogDebug("Abc");

        act.Should().NotThrow();
    }

    [Fact]
    public void Logger_LogTrace_DoesNotThrow()
    {
        var act = () => _logger.LogTrace("Abc");

        act.Should().NotThrow();
    }
}