using System;
using CodeBoost.Environment;
using CodeBoost.Logging;
using Xunit;

namespace CodeBoost.Tests;

[Collection("EnvironmentTests")]
public class EnvironmentTests
{
    private sealed class MockApplicationState : IApplicationState
    {
        public bool IsQuittingValue;
        public bool IsPlayingValue;
        public bool IsEditorValue;
        public bool IsDevelopmentValue;
        public bool IsGuiBuildValue;
        public bool IsHeadlessBuildValue;
        public int QuitCallCount;

        public event IApplicationState.FocusChangeEventHandler? FocusChanged;

        public bool IsQuitting() => IsQuittingValue;
        public bool IsPlaying() => IsPlayingValue;
        public void Quit() => QuitCallCount++;
        public bool IsEditor() => IsEditorValue;
        public bool IsDevelopment() => IsDevelopmentValue;
        public bool IsGuiBuild() => IsGuiBuildValue;
        public bool IsHeadlessBuild() => IsHeadlessBuildValue;

        public void RaiseFocusChanged(bool isFocused) => FocusChanged?.Invoke(isFocused);
    }

    /// <summary>
    /// A logger that counts what reaches it, under a setting the test chooses.
    /// </summary>
    private sealed class CountingLogger : ILogger
    {
        /// <summary>
        /// How many messages reached this logger at any level.
        /// </summary>
        public int LoggedCount;

        /// <summary>
        /// The setting this logger reports.
        /// </summary>
        private readonly LoggerSetting _loggerSetting;

        /// <summary>
        /// Creates a logger that reports <paramref name="loggerSetting"/>.
        /// </summary>
        /// <param name="loggerSetting">The setting to report.</param>
        public CountingLogger(LoggerSetting loggerSetting) => _loggerSetting = loggerSetting;

        /// <inheritdoc/>
        public LoggerSetting GetLoggerSetting() => _loggerSetting;

        /// <inheritdoc/>
        public bool DisableUnconditionalDevelopmentStacktrace() => true;

        /// <inheritdoc/>
        public void LogInformation(string message) => LoggedCount++;

        /// <inheritdoc/>
        public void LogWarning(string message) => LoggedCount++;

        /// <inheritdoc/>
        public void LogError(string message) => LoggedCount++;
    }

    private static void RunWithMock(MockApplicationState mock, Action body)
    {
        ApplicationStateService.UseApplicationState(mock);
        try
        {
            body();
        }
        finally
        {
            ApplicationStateService.UseApplicationState(new IdeApplicationState());
        }
    }

    // ── LoggingService follows the application state ────────────────────

    /// <summary>
    /// A logger registered before the application state takes its level from the state set afterwards, so a host may register the
    /// two in either order. A release build logs errors only, and the editor logs information again once it is back.
    /// </summary>
    [Fact]
    public void LoggingService_LoggerSetBeforeTheApplicationState_TakesTheLevelOfTheStateSetAfterIt()
    {
        ILogger? previousLogger = LoggingService.Logger;
        LoggingService.UseLogger(new ConsoleLogger());

        try
        {
            RunWithMock(new() { IsEditorValue = false, IsDevelopmentValue = false }, () =>
            {
                Assert.False(LoggingService.IsInformationEnabled, "a release build kept the information level of the state before it");
                Assert.True(LoggingService.IsErrorEnabled, "a release build stopped logging errors");
            });

            Assert.True(LoggingService.IsInformationEnabled, "the editor state did not bring the information level back");
        }
        finally
        {
            if (previousLogger is not null)
                LoggingService.UseLogger(previousLogger);
        }
    }

    /// <summary>
    /// A logger set to <see cref="LoggerLevel.Disabled"/> logs nothing, as the level documents. Disabled is the highest value of the
    /// enum and every level check is at-or-above, so it read as the most verbose level and logged information, warnings and errors.
    /// </summary>
    [Fact]
    public void LoggingService_DisabledLevel_LogsNothing()
    {
        ILogger? previousLogger = LoggingService.Logger;
        CountingLogger countingLogger = new(new() { Editor = LoggerLevel.Disabled, DevelopmentBuilds = LoggerLevel.Disabled, ReleaseBuilds = LoggerLevel.Disabled });
        LoggingService.UseLogger(countingLogger);

        try
        {
            Logger.LogInformation("information");
            Logger.LogWarning("warning");
            Logger.LogError("error");
            LoggingService.LogError("error");

            Assert.False(Logger.IsEnabled(LoggerLevel.Error), "a disabled logger reported errors as enabled");
            Assert.Equal(0, countingLogger.LoggedCount);
        }
        finally
        {
            if (previousLogger is not null)
                LoggingService.UseLogger(previousLogger);
        }
    }

    // ── IdeApplicationState ──────────────────────────────────────────────

    [Fact]
    public void IdeApplicationState_IsEditor_ReturnsTrue()
    {
        IdeApplicationState state = new();

        Assert.True(state.IsEditor());
    }

    [Fact]
    public void IdeApplicationState_IsGuiBuild_ReturnsFalse()
    {
        IdeApplicationState state = new();

        Assert.False(state.IsGuiBuild());
    }

    [Fact]
    public void IdeApplicationState_IsHeadlessBuild_ReturnsFalse()
    {
        IdeApplicationState state = new();

        Assert.False(state.IsHeadlessBuild());
    }

    [Fact]
    public void IdeApplicationState_IsQuitting_FalseDuringNormalRun()
    {
        IdeApplicationState state = new();

        Assert.False(state.IsQuitting());
    }

    [Fact]
    public void IdeApplicationState_IsPlaying_TrueDuringNormalRun()
    {
        IdeApplicationState state = new();

        Assert.True(state.IsPlaying());
    }

    // ── ApplicationStateService bug fixes ────────────────────────────────

    [Fact]
    public void IsPlaying_DelegatesToStateIsPlaying_NotIsQuitting()
    {
        MockApplicationState mock = new()
        {
            IsPlayingValue = true,
            IsQuittingValue = false,
        };

        RunWithMock(mock, () =>
        {
            Assert.True(ApplicationState.IsPlaying());
        });

        MockApplicationState invertedMock = new()
        {
            IsPlayingValue = false,
            IsQuittingValue = true,
        };

        RunWithMock(invertedMock, () =>
        {
            Assert.False(ApplicationState.IsPlaying());
        });
    }

    [Fact]
    public void Quit_DoesNotThrow_AfterSuccessfulCall()
    {
        MockApplicationState mock = new();

        RunWithMock(mock, () =>
        {
            ApplicationState.Quit();
        });

        Assert.Equal(1, mock.QuitCallCount);
    }

    // ── ApplicationState chain ───────────────────────────────────────────

    [Fact]
    public void ApplicationState_DelegatesIsEditor()
    {
        MockApplicationState mock = new() { IsEditorValue = false };

        RunWithMock(mock, () =>
        {
            Assert.False(ApplicationState.IsEditor());
        });
    }

    [Fact]
    public void ApplicationState_DelegatesIsGuiBuild()
    {
        MockApplicationState mock = new() { IsGuiBuildValue = true };

        RunWithMock(mock, () =>
        {
            Assert.True(ApplicationState.IsGuiBuild());
        });
    }

    [Fact]
    public void ApplicationState_DelegatesIsHeadlessBuild()
    {
        MockApplicationState mock = new() { IsHeadlessBuildValue = true };

        RunWithMock(mock, () =>
        {
            Assert.True(ApplicationState.IsHeadlessBuild());
        });
    }

    [Fact]
    public void ApplicationState_DelegatesIsDevelopmentBuild()
    {
        MockApplicationState mock = new() { IsDevelopmentValue = true };

        RunWithMock(mock, () =>
        {
            Assert.True(ApplicationState.IsDevelopmentBuild());
        });
    }

    [Fact]
    public void ApplicationState_DelegatesIsQuitting()
    {
        MockApplicationState mock = new() { IsQuittingValue = true };

        RunWithMock(mock, () =>
        {
            Assert.True(ApplicationState.IsQuitting());
        });
    }

    [Fact]
    public void ApplicationState_FocusChanged_FiresWhenInnerStateRaises()
    {
        MockApplicationState mock = new();
        bool? capturedFocus = null;

        IApplicationState.FocusChangeEventHandler handler = isFocused => capturedFocus = isFocused;
        ApplicationState.FocusChanged += handler;

        try
        {
            RunWithMock(mock, () =>
            {
                mock.RaiseFocusChanged(true);
            });

            Assert.True(capturedFocus);
        }
        finally
        {
            ApplicationState.FocusChanged -= handler;
        }
    }
}
