using Microsoft.CodeAnalysis;

namespace CodeBoost.CodeAnalysis.Analyzers.Receivers.Diagnostics;

public static class DiagnosticRules
{
    /// <summary>
    /// 0: Message.
    /// </summary>
    public static readonly DiagnosticDescriptor CodeHealthReporterWarning = new(
        id: "CODEHEALTHREPORTER00",
        title: "CodeHealthReporter Warning",
        messageFormat: "{0}",
        category: "CodeHealthReporter",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);    

    /// <summary>
    /// 0: Message.
    /// </summary>
    public static readonly DiagnosticDescriptor CodeHealthReporterError = new(
        id: "CODEHEALTHREPORTER01",
        title: "CodeHealthReporter Error",
        messageFormat: "{0}",
        category: "CodeHealthReporter",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);    


}