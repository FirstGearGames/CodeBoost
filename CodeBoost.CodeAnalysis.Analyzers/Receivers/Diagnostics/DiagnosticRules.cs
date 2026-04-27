using Microsoft.CodeAnalysis;

namespace CodeBoost.CodeAnalysis.Analyzers.Receivers.Diagnostics;

/// <summary>
/// Provides shared <see cref="DiagnosticDescriptor"/> instances used by CodeBoost analyzers.
/// </summary>
public static class DiagnosticRules
{
    /// <summary>
    /// The descriptor for a CodeHealthReporter warning. The message format placeholder {0} is the message.
    /// </summary>
    public static readonly DiagnosticDescriptor CodeHealthReporterWarning = new(
        id: "CODEHEALTHREPORTER00",
        title: "CodeHealthReporter Warning",
        messageFormat: "{0}",
        category: "CodeHealthReporter",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);    

    /// <summary>
    /// The descriptor for a CodeHealthReporter error. The message format placeholder {0} is the message.
    /// </summary>
    public static readonly DiagnosticDescriptor CodeHealthReporterError = new(
        id: "CODEHEALTHREPORTER01",
        title: "CodeHealthReporter Error",
        messageFormat: "{0}",
        category: "CodeHealthReporter",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);    


}