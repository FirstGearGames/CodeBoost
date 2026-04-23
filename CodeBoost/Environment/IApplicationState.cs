namespace CodeBoost.Environment;

public interface IApplicationState
{
    /// <summary>
    /// Invoked when the application focus state changes.
    /// </summary>
    public event FocusChangeEventHandler FocusChanged;

    public delegate void FocusChangeEventHandler(bool isFocused);

    /// <summary>
    /// Returns whether the application is quitting.
    /// </summary>
    /// <returns>True if the application is quitting; otherwise false.</returns>
    public bool IsQuitting();

    /// <summary>
    /// Returns whether the application is playing.
    /// </summary>
    /// <returns>True if the application is playing; otherwise false.</returns>
    public bool IsPlaying();

    /// <summary>
    /// Quits the application for the editor or a build.
    /// </summary>
    public void Quit();

    /// <summary>
    /// Returns whether the application is being run within an editor.
    /// </summary>
    /// <returns>True if the application is being run within an editor; false if it is a build.</returns>
    public bool IsEditor();

    /// <summary>
    /// Returns whether the application is in development mode.
    /// </summary>
    /// <remarks>This should be true if running in the editor.</remarks>
    /// <returns>True if the application is in development mode; false if it is in release mode.</returns>
    public bool IsDevelopment();

    /// <summary>
    /// Returns whether the application is a GUI build, such as a client build.
    /// </summary>
    /// <returns>True if the application is a GUI build; otherwise false.</returns>
    public bool IsGuiBuild();

    /// <summary>
    /// Returns whether the application is a headless build, such as a server build.
    /// </summary>
    /// <returns>True if the application is a headless build; otherwise false.</returns>
    public bool IsHeadlessBuild();
}