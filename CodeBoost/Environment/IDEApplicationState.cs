namespace CodeBoost.Environment;

/// <summary>
/// Provides application states for development within an IDE.
/// </summary>
public class IdeApplicationState : IApplicationState
{
    /// <summary>
    /// Invoked when the application focus state changes.
    /// </summary>
    /// <remarks>This event never invokes for this type.</remarks>
    public event IApplicationState.FocusChangeEventHandler FocusChanged;

    /// <summary>
    /// Returns the value of System.Environment.HasShutdownStarted.
    /// </summary>
    /// <returns>The value of System.Environment.HasShutdownStarted.</returns>
    public bool IsQuitting() => System.Environment.HasShutdownStarted;

    /// <summary>
    /// Returns whether IsQuitting is false.
    /// </summary>
    /// <returns>True if IsQuitting is false; otherwise false.</returns>
    public bool IsPlaying() => !IsQuitting();

    /// <summary>
    /// Exits the process via System.Environment.Exit.
    /// </summary>
    public void Quit()
    {
        System.Environment.Exit(exitCode: 0);
    }

    /// <summary>
    /// Unconditionally returns true.
    /// </summary>
    /// <returns>Always true.</returns>
    public bool IsEditor() => true;

    /// <summary>
    /// Returns whether the DEBUG preprocessor symbol is active.
    /// </summary>
    /// <returns>True if the DEBUG preprocessor symbol is active; otherwise false.</returns>
    public bool IsDevelopment()
    {
        #if DEBUG
        return true;
        #else
            return false;
        #endif
    }

    /// <summary>
    /// Unconditionally returns false.
    /// </summary>
    /// <returns>Always false.</returns>
    public bool IsGuiBuild() => false;

    /// <summary>
    /// Unconditionally returns false.
    /// </summary>
    /// <returns>Always false.</returns>
    public bool IsHeadlessBuild() => false;
}