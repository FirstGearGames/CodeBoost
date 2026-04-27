using CodeBoost.Extensions;

namespace CodeBoost.Environment;

/// <summary>
/// Provides a static ApplicationState which uses the currently registered IApplicationState.
/// </summary>
public static class ApplicationState
{
    static ApplicationState()
    {
        ApplicationStateService.ApplicationStateSet += ApplicationStateService_OnApplicationStateSet;

        /* If an IApplicationState is already set then call
         * value set callback to initialize for it. */
        if (ApplicationStateService.ApplicationState is not null)
            ApplicationStateService_OnApplicationStateSet(previousApplicationState: null, ApplicationStateService.ApplicationState);
    }

    private static void ApplicationStateService_OnApplicationStateSet(IApplicationState previousApplicationState, IApplicationState applicationState)
    {
        if (previousApplicationState is not null)
            previousApplicationState.FocusChanged -= ApplicationState_OnFocusChanged;

        if (applicationState is not null)
            applicationState.FocusChanged += ApplicationState_OnFocusChanged;
    }
        

    /// <summary>
    /// Invoked when the application focus state changes.
    /// </summary>
    public static event IApplicationState.FocusChangeEventHandler FocusChanged;

    private static void ApplicationState_OnFocusChanged(bool isFocused) => FocusChanged?.Invoke(isFocused);
        
    /// <summary>
    /// Returns whether the application is quitting.
    /// </summary>
    /// <returns>True if the application is quitting; otherwise false.</returns>
    public static bool IsQuitting() => ApplicationStateService.IsQuitting();

    /// <summary>
    /// Returns whether the application is playing.
    /// </summary>
    /// <returns>True if the application is playing; otherwise false.</returns>
    public static bool IsPlaying() =>  ApplicationStateService.IsPlaying();

    /// <summary>
    /// Quits the application for the editor or a build.
    /// </summary>
    public static void Quit() => ApplicationStateService.Quit();

    /// <summary>
    /// Returns whether the application is being run within an editor.
    /// </summary>
    /// <returns>True if the application is being run within an editor; otherwise false.</returns>
    public static bool IsEditor() => ApplicationStateService.IsEditor();

    /// <summary>
    /// Returns whether the application is a build with development or debugging enabled.
    /// </summary>
    /// <returns>True if the application is a development or debug build; otherwise false.</returns>
    public static bool IsDevelopmentBuild() => ApplicationStateService.IsDevelopmentBuild();

    /// <summary>
    /// Returns whether the application is a GUI build, such as a client build.
    /// </summary>
    /// <returns>True if the application is a GUI build; otherwise false.</returns>
    public static bool IsGuiBuild() => ApplicationStateService.IsGuiBuild();

    /// <summary>
    /// Returns whether the application is a headless build, such as a server build.
    /// </summary>
    /// <returns>True if the application is a headless build; otherwise false.</returns>
    public static bool IsHeadlessBuild() => ApplicationStateService.IsHeadlessBuild();
}