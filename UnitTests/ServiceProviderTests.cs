using System;
using FluentAssertions;
using MhLabs.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace UnitTests;

public class ServiceProviderTests
{
    private readonly IServiceProvider _serviceProvider = new ServiceCollection()
        .AddMhLogging()
        .BuildServiceProvider();


    [Fact]
    public void GetRequiredService_Logger_WillNotThrow()
    {
        // Act
        var act = () => _serviceProvider.GetRequiredService<ILogger<ServiceProviderTests>>();

        act.Should().NotThrow();
    }
}