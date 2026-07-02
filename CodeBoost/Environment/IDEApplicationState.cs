namespace CodeBoost.Environment;

/// <summary>
/// Provides application states for development within an IDE.
/// </summary>
public class IdeApplicationState : IApplicationState
{
    /// <inheritdoc/>
    /// <remarks>
    /// This event never invokes for this type.
    /// </remarks>
    public event IApplicationState.FocusChangeEventHandler? FocusChanged;

    /// <inheritdoc/>
    public bool IsQuitting() => System.Environment.HasShutdownStarted;

    /// <inheritdoc/>
    public bool IsPlaying() => !IsQuitting();

    /// <inheritdoc/>
    public void Quit()
    {
        System.Environment.Exit(exitCode: 0);
    }

    /// <inheritdoc/>
    public bool IsEditor() => true;

    /// <inheritdoc/>
    public bool IsDevelopment()
    {
        #if DEBUG
        return true;
        #else
            return false;
        #endif
    }

    /// <inheritdoc/>
    public bool IsGuiBuild() => false;

    /// <inheritdoc/>
    public bool IsHeadlessBuild() => false;
}