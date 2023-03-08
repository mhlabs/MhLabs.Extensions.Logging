using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MhLabs.Extensions.Logging.Internal;

internal record Settings(
    bool IsStructuredExceptionEnabled,
    bool IsSourceContextEnabled,
    bool IsMessageTemplateEnabled,
    IReadOnlyDictionary<string, object> AdditionalProperties,
    IReadOnlyDictionary<string, LogLevel> Omissions);