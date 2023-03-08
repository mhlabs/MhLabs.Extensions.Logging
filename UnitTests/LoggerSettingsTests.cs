using System;
using FluentAssertions;
using MhLabs.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace UnitTests;

public class LoggerSettingsTests
{
    [Fact]
    public void Constructor_Verify_DefaultValues()
    {
        // Act
        var settings = new LoggingSettings();

        // Assert
        settings.StructuredException.Should().BeFalse();
        settings.Properties.Should().BeEmpty();
        settings.Omissions.Should().BeEmpty();
    }

    [Fact]
    public void EnableSourceContext_WithoutArgument_IsSetToTrue()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.EnableSourceContext();

        // Assert
        settings.SourceContext.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnableMessageTemplate_WithArgument_IsSet(bool value)
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.EnableMessageTemplate(value);

        // Assert
        settings.MessageTemplate.Should().Be(value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnableSourceContext_WithArgument_IsSet(bool value)
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.EnableSourceContext(value);

        // Assert
        settings.SourceContext.Should().Be(value);
    }

    [Fact]
    public void EnableStructuredException_WithoutArgument_IsSetToTrue()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.EnableStructuredException();

        // Assert
        settings.StructuredException.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnableStructuredException_WithArgument_IsSet(bool value)
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.EnableStructuredException(value);

        // Assert
        settings.StructuredException.Should().Be(value);
    }

    [Fact]
    public void UseDefaultSettings_BuildShould_NotThrow()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        var act = () => settings.UseDefaultSettings().Build();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Build_Should_NotThrow()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        var act = () => settings.Build();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AddProperty_IsAdded_ToProperties()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.AddProperty("foo", 1);

        // Assert
        settings.Properties.Should().SatisfyRespectively(x =>
        {
            x.Key.Should().Be("foo");
            x.Value.Should().Be(1);
        });
    }

    [Fact]
    public void AddProperty_TwiceIsAdded_ToProperties()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.AddProperty("foo", 1);
        settings.AddProperty("bar", 2);

        // Assert
        settings.Properties.Should().SatisfyRespectively(x =>
        {
            x.Key.Should().Be("foo");
            x.Value.Should().Be(1);
        }, x =>
        {
            x.Key.Should().Be("bar");
            x.Value.Should().Be(2);
        });
    }


    [Fact]
    public void AddOmission_NullSource_Throws()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        var act = () => settings.AddOmission(null!, LogLevel.Critical);

        act.Should().Throw<ArgumentNullException>();
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void AddProperty_Overrides_ExistingProperty(int value)
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.AddProperty("foo", 1);
        settings.AddProperty("foo", value);

        // Assert
        settings.Properties.Should().SatisfyRespectively(x =>
        {
            x.Key.Should().Be("foo");
            x.Value.Should().Be(value);
        });
    }

    [Fact]
    public void AddOmission_IsAdded_ToOmissions()
    {
        // Arrange
        var settings = new LoggingSettings();

        settings.AddOmission("System", LogLevel.Warning);

        settings.Omissions.Should().SatisfyRespectively(x =>
        {
            x.Key.Should().Be("System");
            x.Value.Should().Be(LogLevel.Warning);
        });
    }

    [Theory]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Information)]
    public void AddOmission_Overrides_ExistingOmission(LogLevel logLevel)
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.AddOmission("foo", LogLevel.Warning);
        settings.AddOmission("foo", logLevel);

        // Assert
        settings.Omissions.Should().SatisfyRespectively(x =>
        {
            x.Key.Should().Be("foo");
            x.Value.Should().Be(logLevel);
        });
    }

    [Theory]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Debug)]
    public void AddDefaultOmissions_WithLevel_LevelIsApplied(LogLevel level)
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.AddDefaultOmissions(level);

        settings.Omissions.Should().SatisfyRespectively(
            x =>
            {
                x.Key.Should().Be("AWSSDK");
                x.Value.Should().Be(level);
            },
            x =>
            {
                x.Key.Should().Be("Microsoft");
                x.Value.Should().Be(level);
            },
            x =>
            {
                x.Key.Should().Be("System");
                x.Value.Should().Be(level);
            }
        );
    }

    [Fact]
    public void AddDefaultOmissions_WithoutLevel_LevelIsDefaulted()
    {
        // Arrange
        var settings = new LoggingSettings();

        // Act
        settings.AddDefaultOmissions();

        settings.Omissions.Should().SatisfyRespectively(
            x =>
            {
                x.Key.Should().Be("AWSSDK");
                x.Value.Should().Be(LogLevel.Warning);
            },
            x =>
            {
                x.Key.Should().Be("Microsoft");
                x.Value.Should().Be(LogLevel.Warning);
            },
            x =>
            {
                x.Key.Should().Be("System");
                x.Value.Should().Be(LogLevel.Warning);
            }
        );
    }
}