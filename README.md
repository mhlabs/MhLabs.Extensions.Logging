## MhLabs.Extensions.Logging


## Usage

This is a `NuGet` package that can be used in order to achieve logging by using `ILogger<T>` from [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging/).


## LoggingProvider

The implemented `LoggingProvider` is using [SeriLog](https://github.com/serilog/serilog).
As a consumer of this library you will not be able to access SeriLog types, It's can only be accessed internally in the library by design.


## Examples

```csharp
var serviceProvider = new ServiceCollection()
                          .AddMhLogging()
                          .Build();


ILogger<MyService> logger = serviceProvider.GetRequiredService<ILogger<MyService>>

logger.LogInformation("Hello World");
```


You can also configure the logger.

```csharp
var serviceProvider = new ServiceCollection()
                          .AddMhLogging(x => x.EnableStructuredException())
                          .Build();


ILogger<MyService> logger = serviceProvider.GetRequiredService<ILogger<MyService>>

logger.LogInformation("Hello World");
```
