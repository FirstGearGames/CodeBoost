using System;
using CodeBoost.Environment;
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
